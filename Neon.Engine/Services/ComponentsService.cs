using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Components;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Exceptions;
using Neon.Api.Data.Scheduler;
using Neon.Api.Interfaces.Components;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("Components Service", "Manage components", 6)]
	public class ComponentsService : IComponentsService
	{
		private readonly ILogger _logger;
		private readonly NeonConfig _neonConfig;
		private readonly ComponentsConfig _config;
		private readonly ISchedulerService _schedulerService;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly INeonManager _neonManager;
		private readonly Dictionary<Guid, INeonComponent> _runningComponents = new Dictionary<Guid, INeonComponent>();


		public ObservableCollection<ComponentData> ComponentsData { get; }

		public List<AvailableComponent> AvailableComponents { get; }

		public ComponentsService(ILogger<ComponentsService> logger,
			NeonConfig neonConfig,
			INeonManager neonManager,
			IFileSystemManager fileSystemManager,
			ISchedulerService schedulerService)
		{
			_logger = logger;
			_neonConfig = neonConfig;
			_fileSystemManager = fileSystemManager;
			_neonManager = neonManager;
			_config = _neonConfig.ComponentsConfig;
			_schedulerService = schedulerService;
			ComponentsData = new ObservableCollection<ComponentData>();
			AvailableComponents = new List<AvailableComponent>();
		}

		public Task<bool> Start()
		{
			EnsureComponentsDirectory();
			ScanAvailableComponents();
			LoadComponents();
			return Task.FromResult(true);
		}

		private void EnsureComponentsDirectory()
		{
			_fileSystemManager.CreateDirectory(_config.ConfigDirectory.DirectoryName);
		}

		private void LoadComponents()
		{
			var totalTime = TimeSpan.Zero;
			_config.ComponentsToLoad.ForEach(async c =>
			{
				var sw = new Stopwatch();
				sw.Start();
				await LoadComponent(c.Name);
				sw.Stop();
				totalTime += sw.Elapsed;
			});

			_logger.LogInformation($"{_config.ComponentsToLoad.Count} components load in {totalTime.TotalSeconds} seconds");
		}

		private void ScanAvailableComponents()
		{
			AssemblyUtils.GetAttribute<NeonComponentAttribute>().ForEach(f =>
			{
				var attr = f.GetCustomAttribute<NeonComponentAttribute>();

				AvailableComponents.Add(new AvailableComponent()
				{
					Name = attr.Name,
					ComponentType = f
				});
			});

			_logger.LogInformation($"Available components count: {AvailableComponents.Count}");
		}

		public Task<bool> Stop()
		{
			_runningComponents.Values.ToList().ForEach(c =>
			{
				c.Dispose();
			});

			return Task.FromResult(true);
		}


		public async Task<bool> LoadComponent(string name)
		{
			var cData = new ComponentData()
			{
				Id = Guid.NewGuid(),
				Status = ComponentStatusEnum.Starting,
				Info = new ComponentInfo()
				{
					Name = name
				}
			};

			ComponentsData.Add(cData);

			try
			{
				var availableComponent = AvailableComponents.FirstOrDefault(c => c.Name == name);

				if (availableComponent == null)
					throw new Exception($"Component name {name} not found!");

				var componentAttribute = availableComponent.ComponentType.GetCustomAttribute<NeonComponentAttribute>();
				cData.Info = new ComponentInfo()
				{
					Name = componentAttribute.Name,
					Version = componentAttribute.Version,
					Author = componentAttribute.Author,
					Category = componentAttribute.Category,
					Description = componentAttribute.Description
				};

				var componentObject = _neonManager.Resolve(availableComponent.ComponentType) as INeonComponent;

				if (componentObject == null)
					throw new Exception($"Component {name} not implement INeonComponent interface");

				var configFileName = Path.Combine(_config.ConfigDirectory.DirectoryName,
					GetComponentConfigFileName(cData));

				var componentConfig =
					_fileSystemManager.ReadFromFile(configFileName, componentAttribute.ConfigType);

				if (componentConfig == null)
				{
					_logger.LogWarning($"Component config {cData.Info.Name} ({GetComponentConfigFileName(cData)}) don't exists, creating default");
					componentConfig = componentObject.GetDefaultConfig();
					_fileSystemManager.WriteToFile(configFileName, componentConfig);
				}

				await componentObject.Init(componentConfig);

				await componentObject.Start();

				var polls = GetComponentPollAttribute(availableComponent.ComponentType);

				if (polls.Count == 0)
				{
					_schedulerService.AddPolling(async () => await componentObject.Poll(),
						$"COMPONENT_{name.ToUpper()}", SchedulerServicePollingEnum.NormalPolling);
				}
				else
				{
					polls.ForEach(p =>
					{
						if (p.ComponentPollRate.IsEnabled)
							_schedulerService.AddPolling(() => p.Method.Invoke(componentObject, null),
								$"COMPONENT_{name.ToUpper()}_{p.Method.Name}", p.ComponentPollRate.Rate);
					});
				}




				cData.Status = ComponentStatusEnum.Started;

				_runningComponents.Add(cData.Id, componentObject);

				return true;
			}
			catch (ComponentNeedConfigException ex)
			{
				_logger.LogError($"Error during load component {name} - config values need configuration: {string.Join(',', ex.ConfigKeys)}");

				cData.Status = ComponentStatusEnum.NeedConfiguration;
				cData.Error = ex;
				return false;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error during load component {name} - {ex.Message}");

				cData.Error = ex;
				cData.Status = ComponentStatusEnum.Error;

				return false;
			}
		}

		private static string GetComponentConfigFileName(ComponentData component)
		{
			return $"{component.Info.Name}.yaml";
		}

		private static List<ComponentPollMethodData> GetComponentPollAttribute(Type component)
		{
			var result = new List<ComponentPollMethodData>();
			component.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(m =>
			{
				var rateAttribute = m.GetCustomAttribute<ComponentPollRateAttribute>();
				if (rateAttribute != null)
				{
					result.Add(new ComponentPollMethodData()
					{
						ComponentPollRate = rateAttribute,
						Method = m
					});
				}
			});

			return result;
		}

		public Task<bool> StopComponent(string name)
		{
			var runComp = ComponentsData.FirstOrDefault(c => c.Info.Name == name);

			if (runComp == null)
				return Task.FromResult(false);

			var component = _runningComponents[runComp.Id];

			component.Dispose();
			runComp.Status = ComponentStatusEnum.Stopped;

			return Task.FromResult(true);

		}

		public async Task<bool> RestartComponent(string name)
		{
			await StopComponent(name);
			return await LoadComponent(name);
		}
	}
}
