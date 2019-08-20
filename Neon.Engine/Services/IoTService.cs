﻿using Humanizer;
using KellermanSoftware.CompareNetObjects;
using MediatR;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Entities;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Config.Services;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.NoSql;
using Neon.Api.Interfaces.Services;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("IoT Service", "Manage IoT entities", 2)]
	public class IoTService : IIoTService, INotificationHandler<INeonIoTEntity>
	{
		private static string EntitiesCollectionName = "entities";

		private readonly IoTConfig _config;
		private readonly INoSqlService _noSqlService;
		private readonly ILogger _logger;
		private readonly Subject<INeonIoTEntity> _iotEntitiesBus = new Subject<INeonIoTEntity>();
		private readonly CompareLogic _compareLogic = new CompareLogic();
		private INoSqlConnector _entitiesConnector;
		private INoSqlConnector _eventsConnector;

		public IoTService(ILogger<IoTService> logger, INoSqlService noSqlService, NeonConfig neonConfig)
		{
			_logger = logger;
			_noSqlService = noSqlService;
			_config = neonConfig.ServicesConfig.IoTConfig;
		}

		public async Task<bool> Start()
		{
			await InitEntitiesDatabase();
			await InitEventsDatabase();

			return true;
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

		public Task PersistEntity<T>(T entity) where T : INeonIoTEntity
		{
			T obj = default;
			entity.EventDateTime = DateTime.Now;
			entity.EntityType = entity.GetType().FullName;

			if (entity.Id == Guid.Empty)
				entity.Id = Guid.NewGuid();

			if (string.IsNullOrEmpty(entity.Name))
				obj = _entitiesConnector.Query<T>(EntitiesCollectionName).FirstOrDefault(e => e.EntityType == typeof(T).FullName);
			else
				obj = _entitiesConnector.Query<T>(EntitiesCollectionName).FirstOrDefault(e => e.Name == entity.Name);

			if (obj == null)
			{
				entity = _entitiesConnector.Insert(EntitiesCollectionName, entity);
			}
			else
			{
				entity.Id = obj.Id;
				_entitiesConnector.Update(EntitiesCollectionName, entity);
			}

			if (EntityHaveChanges(entity, obj))
			{
				PersistEvent(entity);
				Publish(entity);
			}

			return Task.CompletedTask;
		}



		public IObservable<T> GetEventStream<T>() where T : INeonIoTEntity
		{
			return _iotEntitiesBus.OfType<T>();
		}

		private void Publish<T>(T @event) where T : INeonIoTEntity
		{
			_logger.LogDebug($"Publishing changes of {@event.GetType().Name}/{@event.Name}");
			_iotEntitiesBus.OnNext(@event);
		}


		private void PersistEvent<T>(T entity) where T : INeonIoTEntity
		{
			var collectionName = entity.GetType().Name.ToLower().Pluralize();
			var attr = entity.GetType().GetCustomAttribute<EventsCollectionAttribute>();

			if (attr != null)
				collectionName = attr.CollectionName;

			entity.Id = Guid.NewGuid();

			_eventsConnector.Insert(collectionName, entity);
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

			#region OldCode
			var properties = oldEntity.GetType().GetProperties();

			foreach (var pi in properties)
			{
				if (pi.CustomAttributes.Any(ca => ca.AttributeType == typeof(IgnorePropertyCompareAttribute))) continue;

				object oldValue = pi.GetValue(oldEntity), newValue = pi.GetValue(newEntity);

				if (!Equals(oldValue, newValue)) return true;
			}

			return false;
			#endregion

			//var result = CompareObjects(newEntity, oldEntity);

			//return !result.AreEqual;
		}

		private ComparisonResult CompareObjects(object obj, object obj2)
		{
			return _compareLogic.Compare(obj, obj2);
		}

		public async Task Handle(INeonIoTEntity notification, CancellationToken cancellationToken)
		{
			await PersistEntity(notification);
		}
	}
}
