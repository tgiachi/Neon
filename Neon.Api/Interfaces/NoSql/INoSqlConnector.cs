using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neon.Api.Interfaces.Entity;

namespace Neon.Api.Interfaces.NoSql
{
	public interface INoSqlConnector
	{
		Task<bool> Start();

		Task<bool> Stop();

		Task<bool> Configure(string connectionString);

		List<TEntity> List<TEntity>(string collectionName) where TEntity : INeonEntity;

		IQueryable<TEntity> Query<TEntity>(string collectionName) where TEntity : INeonEntity;

		TEntity Insert<TEntity>(string collectionName, TEntity obj) where TEntity : INeonEntity;

		TEntity Update<TEntity>(string collectionName, TEntity obj) where TEntity : INeonEntity;

		bool Delete<TEntity>(string collectionName, TEntity obj) where TEntity : INeonEntity;
	}
}
