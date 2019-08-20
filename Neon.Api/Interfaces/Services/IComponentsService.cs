using Neon.Api.Data.Components;
using Neon.Api.Interfaces.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Services
{
	public interface IComponentsService : INeonService
	{
		ObservableCollection<ComponentData> ComponentsData { get; }

		List<AvailableComponent> AvailableComponents { get; }

		Task<bool> LoadComponent(string name);

		Task<bool> StopComponent(string name);

		Task<bool> RestartComponent(string name);
	}
}
