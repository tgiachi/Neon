using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Neon.Api.Attributes.NoSql;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.NoSql;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.NoSqlConnector
{
	[NoSqlConnector("mongo_db")]
	public class MongoDbConnector : INoSqlConnector
	{
		private IMongoClient _mongoClient;
		private IMongoDatabase _mongoDatabase;

		private readonly ILogger _logger;

		private MongoUrl _mongoUrl;

		public MongoDbConnector(ILogger<MongoDbConnector> logger)
		{
			_logger = logger;
		}

		public Task<bool> Start()
		{
			_mongoClient = new MongoClient(_mongoUrl.ToString());
			_logger.LogInformation($"Connecting to {_mongoUrl.DatabaseName}");
			_mongoDatabase = _mongoClient.GetDatabase(_mongoUrl.DatabaseName);
			_mongoClient.ListDatabases().ToList();
			_logger.LogInformation($"Connected {_mongoUrl.DatabaseName}");

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			_mongoClient = null;
			return Task.FromResult(true);
		}

		public Task<bool> Configure(string connectionString)
		{
			_mongoUrl = MongoUrl.Create(connectionString);
			return Task.FromResult(true);
		}

		public List<TEntity> List<TEntity>(string collectionName) where TEntity : class, INeonEntity
		{
			return _mongoDatabase.GetCollection<TEntity>(collectionName).FindSync(entity => true).ToList();
		}

		public IQueryable<TEntity> Query<TEntity>(string collectionName) where TEntity : class, INeonEntity
		{
			return _mongoDatabase.GetCollection<TEntity>(collectionName).AsQueryable();
		}

		public TEntity Insert<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity
		{
			_mongoDatabase.GetCollection<TEntity>(collectionName).InsertOne(obj);
			return obj;
		}

		public TEntity Update<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity
		{
			var replaceResult = _mongoDatabase.GetCollection<TEntity>(collectionName).ReplaceOne(entity => entity.Id == obj.Id, obj);

			return obj;
		}

		public bool Delete<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity
		{
			var result = _mongoDatabase.GetCollection<TEntity>(collectionName).DeleteOne(entity => entity.Id == obj.Id);

			return result.DeletedCount > 0;
		}
	}
}
