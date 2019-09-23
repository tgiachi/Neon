using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonFlatFileDataStore;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.NoSql;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.NoSql;

namespace Neon.Engine.NoSqlConnector
{
	[NoSqlConnector("flat_json_db")]
	public class FlatJsonDbConnector : INoSqlConnector
	{
		private readonly IFileSystemManager _fileSystemManager;
		private ILogger _logger;
		private readonly object _databaseLock = new object();
		private IDataStore _dataStore;

		public FlatJsonDbConnector(IFileSystemManager fileSystemManager, ILogger<FlatJsonDbConnector> logger)
		{
			_logger = logger;
			_fileSystemManager = fileSystemManager;
		}

		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_dataStore.Dispose();
			return Task.FromResult(true);
		}

		public Task<bool> Configure(string connectionString)
		{
			lock (_databaseLock)
			{

				connectionString = _fileSystemManager.BuildFilePath(connectionString);
				CheckDatabaseDirectory(connectionString);
				_dataStore = new DataStore(connectionString, keyProperty: nameof(INeonEntity.Id));
			}

			return Task.FromResult(true);
		}

		public List<TEntity> List<TEntity>(string collectionName) where TEntity : class, INeonEntity
		{
			return _dataStore.GetCollection<TEntity>(collectionName).AsQueryable().ToList();
		}

		public List<object> FindAllGeneric(string collectionName)
		{
			return _dataStore.GetCollection<object>(collectionName).AsQueryable().ToList();
		}

		public IQueryable<TEntity> Query<TEntity>(string collectionName) where TEntity : class, INeonEntity
		{
			return _dataStore.GetCollection<TEntity>(collectionName).AsQueryable().AsQueryable();
		}

		public TEntity Insert<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity
		{
			_dataStore.GetCollection<TEntity>(collectionName).InsertOne(obj);

			return obj;
		}

		public TEntity Update<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity
		{
			_dataStore.GetCollection<TEntity>(collectionName).UpdateOne(obj.Id, obj);
			return obj;
		}

		public bool Delete<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity
		{
			return _dataStore.GetCollection<TEntity>(collectionName).DeleteOne(obj.Id);
		}

		private void CheckDatabaseDirectory(string connectionString)
		{
			var directory = Path.GetDirectoryName(connectionString);
			_fileSystemManager.CreateDirectory(directory);
		}
	}
}
