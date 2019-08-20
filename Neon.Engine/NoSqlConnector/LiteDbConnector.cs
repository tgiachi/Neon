using LiteDB;
using Neon.Api.Attributes.NoSql;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.NoSql;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.NoSqlConnector
{

	[NoSqlConnector("lite_db")]
	public class LiteDbConnector : INoSqlConnector
	{
		private readonly IFileSystemManager _fileSystemManager;
		private readonly object _databaseLock = new object();
		private LiteDatabase _liteDatabase;

		public LiteDbConnector(IFileSystemManager fileSystemManager)
		{
			_fileSystemManager = fileSystemManager;
		}

		public Task<bool> Start()
		{
			lock (_databaseLock)
			{
				_liteDatabase.Shrink();
			}

			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			lock (_databaseLock)
			{
				_liteDatabase.Dispose();
			}

			return Task.FromResult(true);
		}

		public Task<bool> Configure(string connectionString)
		{
			lock (_databaseLock)
			{
				CheckDatabaseDirectory(connectionString);
				connectionString = _fileSystemManager.BuildFilePath(connectionString);

				_liteDatabase = new LiteDatabase(connectionString);
			}

			return Task.FromResult(true);
		}

		private void CheckDatabaseDirectory(string connectionString)
		{
			var directory = Path.GetDirectoryName(connectionString);
			_fileSystemManager.CreateDirectory(directory);


		}

		public List<TEntity> List<TEntity>(string collectionName) where TEntity : INeonIoTEntity
		{
			lock (_databaseLock)
			{
				return _liteDatabase.GetCollection<TEntity>(collectionName).FindAll().ToList();
			}

		}

		public IQueryable<TEntity> Query<TEntity>(string collectionName) where TEntity : INeonIoTEntity
		{
			lock (_databaseLock)
			{
				return _liteDatabase.GetCollection<TEntity>(collectionName).FindAll().AsQueryable();
			}
		}

		public TEntity Insert<TEntity>(string collectionName, TEntity obj) where TEntity : INeonIoTEntity
		{
			lock (_databaseLock)
			{
				_liteDatabase.GetCollection<TEntity>(collectionName).Insert(obj);
				return obj;
			}
		}

		public TEntity Update<TEntity>(string collectionName, TEntity obj) where TEntity : INeonIoTEntity
		{
			lock (_databaseLock)
			{
				_liteDatabase.GetCollection<TEntity>().Update(obj);
				return obj;
			}
		}

		public bool Delete<TEntity>(string collectionName, TEntity obj) where TEntity : INeonIoTEntity
		{
			lock (_databaseLock)
			{
				return _liteDatabase.GetCollection<TEntity>(collectionName).Delete(entity => entity.Id == obj.Id) > 0;
			}
		}
	}
}
