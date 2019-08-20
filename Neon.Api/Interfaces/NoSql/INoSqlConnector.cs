using Neon.Api.Interfaces.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.NoSql
{
	public interface INoSqlConnector
	{
		Task<bool> Start();

		Task<bool> Stop();

		Task<bool> Configure(string connectionString);

		List<TEntity> List<TEntity>(string collectionName) where TEntity : INeonIoTEntity;

		IQueryable<TEntity> Query<TEntity>(string collectionName) where TEntity : INeonIoTEntity;

		TEntity Insert<TEntity>(string collectionName, TEntity obj) where TEntity : INeonIoTEntity;

		TEntity Update<TEntity>(string collectionName, TEntity obj) where TEntity : INeonIoTEntity;

		bool Delete<TEntity>(string collectionName, TEntity obj) where TEntity : INeonIoTEntity;
	}
}
