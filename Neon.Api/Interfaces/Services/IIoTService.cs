using Neon.Api.Data.Entities;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Entity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Services
{
	public interface IIoTService : INeonService
	{
		Task PersistEntity<T>(T entity) where T : class, INeonIoTEntity;

		/// <summary>
		///     Subscribing to this event it is possible to receive entity modifications
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IObservable<T> GetEventStream<T>() where T :class, INeonIoTEntity;

		string GetEntityTypeByName(string name);

		T GetEntityByType<T>(string name, string type) where T :  NeonIoTBaseEntity;

		List<object> GetEntitiesCollectionByType(Type type);

		List<object> GetEntitiesCollectionByName(string name);

		List<object> GetEntities();

		List<string> GetEventsNames { get; }

		List<T> GetEntitiesByType<T>() where T : class, INeonIoTEntity;

	}
}
