using MediatR;
using Neon.Api.Interfaces.Base;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Services
{
	public interface INotificationService : INeonService
	{
		void NotifyConnector(string connectorName, string text, params object[] args);
		void Broadcast<T>(object obj) where T : INotification;
		Task<TOut> RpcProcess<TIn, TOut>(TIn obj) where TIn : IRequest<TOut>;
	}
}
