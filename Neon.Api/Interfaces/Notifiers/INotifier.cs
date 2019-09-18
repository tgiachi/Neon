using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Notifiers
{
	public interface INotifier : IDisposable
	{
		Task<bool> Notify(string text, params object[] args);

		Task<bool> Init(object config);

		Task<bool> Start();

		object GetDefaultConfig();
	}
}
