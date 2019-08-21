using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Entity;
using System;
using System.Threading.Tasks;
using Neon.Api.Data.Entities;

namespace Neon.Api.Interfaces.Services
{
	public interface IIoTService : INeonService
	{
		Task PersistEntity<T>(T entity) where T : INeonIoTEntity;

		/// <summary>
		///     Subscribing to this event it is possible to receive entity modifications
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		IObservable<T> GetEventStream<T>() where T : INeonIoTEntity;

		string GetEntityTypeByName(string name);

		T GetEntityByType<T>(string name, string type) where T : NeonIoTBaseEntity;

	}
}
