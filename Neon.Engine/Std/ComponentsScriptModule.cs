using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Data.Config.Common;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.Std
{

	[ScriptModule]
	public class ComponentsScriptModule
	{
		private readonly IComponentsService _componentsService;
		private readonly IConfigManager _configManager;

		public ComponentsScriptModule(IComponentsService componentsService, IConfigManager configManager)
		{
			_componentsService = componentsService;
			_configManager = configManager;
		}

		[ScriptFunction("load_component", "Load component")]
		public void LoadComponent(string name, bool loadAtStartup = false)
		{
			_ = Task.Factory.StartNew(async () =>
			{
				await _componentsService.LoadComponent(name);
				if (loadAtStartup)
				{
					var confEntry =
						_configManager.Configuration.ComponentsConfig.ComponentsToLoad.FirstOrDefault(c =>
							c.Name == name);

					if (confEntry == null)
					{
						_configManager.Configuration.ComponentsConfig.ComponentsToLoad.Add(new ComponentConfig()
						{
							Name = name
						});

						_configManager.SaveConfig();
					}
				}
			});

		}

		[ScriptFunction("stop_component", "Stop running component")]
		public async void StopComponent(string name)
		{
			await _componentsService.StopComponent(name);
		}
	}
}
