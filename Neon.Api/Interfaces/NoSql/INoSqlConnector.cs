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

		List<TEntity> List<TEntity>(string collectionName) where TEntity : class, INeonEntity;

		List<object> FindAllGeneric(string collectionName);

		IQueryable<TEntity> Query<TEntity>(string collectionName) where TEntity : class, INeonEntity;

		TEntity Insert<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity;

		TEntity Update<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity;

		bool Delete<TEntity>(string collectionName, TEntity obj) where TEntity : class, INeonEntity;
	}
}
