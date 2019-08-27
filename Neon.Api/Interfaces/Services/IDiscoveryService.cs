using Neon.Api.Data.Discovery;
using Neon.Api.Interfaces.Base;
using System.Collections.ObjectModel;

namespace Neon.Api.Interfaces.Services
{
	public interface IDiscoveryService : INeonService
	{
		ObservableCollection<DiscoveryDevice> DiscoveredDevices { get; }
	}
}
