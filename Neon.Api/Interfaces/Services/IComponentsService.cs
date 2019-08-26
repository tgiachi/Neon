using Neon.Api.Data.Components;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Components;
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

		void SaveComponentConfig(INeonComponent component, object config);

		void SaveVaultConfig(INeonComponent component, object config);

		T LoadVaultConfig<T>(INeonComponent component) where T : new();
	}
}
