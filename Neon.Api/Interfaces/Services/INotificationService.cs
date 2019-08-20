using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Services
{
	public interface INotificationService : INeonService
	{
		void Broadcast<T>(object obj) where T : INotification;

		Task<TOut> RpcProcess<TIn, TOut>(TIn obj) where TIn : IRequest<TOut>;
	}
}
