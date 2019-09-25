using Humanizer;
using KellermanSoftware.CompareNetObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Entities;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Data.Entities;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.NoSql;
using Neon.Api.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Neon.Api.Utils;

namespace Neon.Engine.Services
{
	[NeonService("IoT Service", "Manage IoT entities", 2)]
	public class IoTService : IIoTService, INotificationHandler<INeonIoTEntity>
	{
		private static readonly string EntitiesCollectionName = "entities";

		private readonly IoTConfig _config;
		private readonly INoSqlService _noSqlService;
		private readonly ILogger _logger;
		private readonly Subject<INeonIoTEntity> _iotEntitiesBus = new Subject<INeonIoTEntity>();
		private readonly CompareLogic _compareLogic = new CompareLogic();
		private readonly Dictionary<string, Type> _eventsCollectionNames = new Dictionary<string, Type>();
		private INoSqlConnector _entitiesConnector;
		private INoSqlConnector _eventsConnector;

		public List<string> GetEventsNames => _eventsCollectionNames.Keys.ToList();

		public IoTService(ILogger<IoTService> logger, INoSqlService noSqlService, NeonConfig neonConfig)
		{
			_logger = logger;
			_noSqlService = noSqlService;
			_config = neonConfig.ServicesConfig.IoTConfig;
		}

		public async Task<bool> Start()
		{
			ScanEventCollectionNames();
			await InitEntitiesDatabase();
			await InitEventsDatabase();

			return true;
		}

		private void ScanEventCollectionNames()
		{
			AssemblyUtils.GetAttribute<EventsCollectionAttribute>().ForEach(t =>
			{
				_eventsCollectionNames.Add(GetCollectionName(t), t);
			});
		}

		private string GetCollectionName(Type t)
		{
			var attr = t.GetCustomAttribute<EventsCollectionAttribute>();
			var collectionName = t.GetType().Name.ToLower().Pluralize();
			if (!string.IsNullOrEmpty(attr.CollectionName))
				collectionName = attr.CollectionName.ToLower();

			return collectionName;
		}

		private async Task InitEntitiesDatabase()
		{
			_logger.LogInformation($"Starting Entities database: {_config.EntitiesDb.Name} - {_config.EntitiesDb.ConnectionString}");
			_entitiesConnector = _noSqlService.GetNoSqlConnector(_config.EntitiesDb.Name);
			await _entitiesConnector.Configure(_config.EntitiesDb.ConnectionString);
			await _entitiesConnector.Start();
		}

		private async Task InitEventsDatabase()
		{
			_logger.LogInformation($"Starting Events database: {_config.EventsDb.Name} - {_config.EntitiesDb.ConnectionString}");
			_eventsConnector = _noSqlService.GetNoSqlConnector(_config.EventsDb.Name);
			await _eventsConnector.Configure(_config.EventsDb.ConnectionString);
			await _eventsConnector.Start();
		}

		public Task PersistEntity<T>(T entity) where T : class, INeonIoTEntity
		{
			T obj = default(T);
			entity.EventDateTime = DateTime.Now;
			entity.EntityType = entity.GetType().FullName;

			if (string.IsNullOrEmpty(entity.Id))
				entity.Id = EntitiesUtils.GenerateId();

			if (string.IsNullOrEmpty(entity.Name))
				obj = _entitiesConnector.Query<T>(EntitiesCollectionName).FirstOrDefault(e => e.EntityType == typeof(T).FullName && e.GroupName == entity.GroupName);
			else
				obj = _entitiesConnector.Query<T>(EntitiesCollectionName).FirstOrDefault(e => e.Name == entity.Name && e.GroupName == entity.GroupName);

			if (obj == null)
			{
				_logger.LogDebug($"Inserting new entity type {entity.GetType().Namespace} - {entity.Name} {entity.GroupName}");
				entity = _entitiesConnector.Insert(EntitiesCollectionName, entity);
			}
			else
			{
				entity.Id = obj.Id;
				_entitiesConnector.Update(EntitiesCollectionName, entity);
			}

			if (!EntityHaveChanges(obj, entity)) return Task.CompletedTask;

			PersistEvent(entity);
			Publish(entity);

			return Task.CompletedTask;
		}

		public IObservable<T> GetEventStream<T>() where T : class, INeonIoTEntity
		{
			return _iotEntitiesBus.OfType<T>();
		}

		private void Publish<T>(T @event) where T : INeonIoTEntity
		{
			_logger.LogDebug($"Publishing changes of {@event.GetType().Name}/{@event.Name}");
			_iotEntitiesBus.OnNext(@event);
		}

		private void PersistEvent<T>(T entity) where T : class, INeonIoTEntity
		{
			entity.Id = EntitiesUtils.GenerateId();

			_eventsConnector.Insert(GetCollectionName(entity.GetType()), entity);
		}

		public async Task<bool> Stop()
		{
			await _entitiesConnector.Stop();
			await _eventsConnector.Stop();
			return true;
		}

		private bool EntityHaveChanges(object oldEntity, object newEntity)
		{
			if (oldEntity == null)
				return true;

			if (newEntity == null)
				return true;

			var result = CompareObjects(newEntity, oldEntity);

			if (!result.AreEqual)
				_logger.LogDebug($"Object {oldEntity} differences: {result.DifferencesString}");

			return !result.AreEqual;
		}

		public string GetEntityTypeByName(string name)
		{
			var entity = _entitiesConnector.Query<NeonIoTBaseEntity>(EntitiesCollectionName).FirstOrDefault(d => d.Name == name);

			return entity?.EntityType;
		}

		public T GetEntityByType<T>(string name, string type) where T : NeonIoTBaseEntity
		{
			return _entitiesConnector.Query<T>(EntitiesCollectionName).FirstOrDefault(document =>
				document.Name == name && document.EntityType == type);
		}

		public List<object> GetEntities()
		{
			return _entitiesConnector.FindAllGeneric(EntitiesCollectionName);

		}

		public List<object> GetEntitiesCollectionByType(Type type)
		{
			
			var openCast = _entitiesConnector.GetType().GetMethod(nameof(_entitiesConnector.Query));
			var closeCast = openCast.MakeGenericMethod(type);
			var queryable = closeCast.Invoke(_entitiesConnector, new object[] {GetCollectionName(type)}) as IQueryable<object>;

			return queryable.ToList();
		}

		public List<object> GetEntitiesCollectionByName(string name)
		{
			return _eventsCollectionNames.ContainsKey(name) ? GetEntitiesCollectionByType(_eventsCollectionNames[name]) : new List<object>();
		}

		public List<T> GetEntitiesByType<T>() where T : class, INeonIoTEntity
		{
			return _entitiesConnector.Query<T>(EntitiesCollectionName).Where(e => e.EntityType == typeof(T).FullName)
				.ToList();
		}

		private ComparisonResult CompareObjects(object obj, object obj2)
		{
			if (!_compareLogic.Config.AttributesToIgnore.Contains(typeof(IgnorePropertyCompareAttribute)))
				_compareLogic.Config.AttributesToIgnore.Add(typeof(IgnorePropertyCompareAttribute));

			return _compareLogic.Compare(obj, obj2);
		}

		public async Task Handle(INeonIoTEntity notification, CancellationToken cancellationToken)
		{
			await PersistEntity(notification);
		}
	}
}
