using Autofac;
using MediatR;
using MediatR.Pipeline;
using Neon.Api.Attributes;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.Discovery;
using Neon.Api.Attributes.NoSql;
using Neon.Api.Attributes.OAuth;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Attributes.Services;
using Neon.Api.Attributes.WebHook;
using Neon.Api.Attributes.Websocket;
using Neon.Api.Data.Commands;
using Neon.Api.Data.Config.Common;
using Neon.Api.Data.Config.Root;
using Neon.Api.Data.Discovery;
using Neon.Api.Data.OAuth;
using Neon.Api.Data.WebHook;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace Neon.Api.Core
{

	/// <summary>
	/// Implementation of Neon Manager 
	/// </summary>
	public class NeonManager : INeonManager
	{
		private readonly ILogger _logger;
		private IContainer _container;
		private readonly ContainerBuilder _containerBuilder;

		private IServicesManager _servicesManager;
		private readonly IConfigManager _configManager;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly ISecretKeyManager _secretKeyManager;
		private readonly IPluginsManager _pluginsManager;
		public ContainerBuilder ContainerBuilder => _containerBuilder;
		public List<Type> AvailableServices { get; }
		public NeonConfig Config => _configManager.Configuration;

		private readonly List<CommandPreloadData> _commandsCacheData = new List<CommandPreloadData>();

		private readonly List<OAuthReceiverData> _oAuthReceiverData;

		private readonly List<WebHookReceiverData> _webHookReceiverData;

		private readonly List<DiscoveryListenerData> _deviceDiscoveryListeners = new List<DiscoveryListenerData>();

		public bool IsRunningInDocker { get; }


		public NeonManager()
		{
			ConfigureLogger(null);
			PrintHeader();
			_logger = Log.Logger.ForContext<NeonManager>();
			_logger.Debug($"Pre-loading assemblies");
			AssemblyUtils.GetAppAssemblies();

			AvailableServices = new List<Type>();
			IsRunningInDocker = Environment.GetEnvironmentVariables()["DOTNET_RUNNING_IN_CONTAINER"] != null;
			_oAuthReceiverData = new List<OAuthReceiverData>();
			_webHookReceiverData = new List<WebHookReceiverData>();
			_containerBuilder = new ContainerBuilder();
			_containerBuilder.RegisterBuildCallback(container => { _logger.Debug($"Container is ready"); });

			_configManager = new ConfigManager(_logger, this, _containerBuilder);
			_configManager.LoadConfig();

			_secretKeyManager = new SecretKeyManager(Config.EngineConfig.SecretKey);

			_fileSystemManager = new FileSystemManager(_logger, Config, _secretKeyManager);
			_fileSystemManager.Start();

			ConfigureLogger(_configManager);

			_pluginsManager = new PluginsManager(_logger, _fileSystemManager, Config);
			_pluginsManager.Start();
		}

		private void PrintHeader()
		{
			Console.WriteLine(@" 
 _   _                  
| \ | |                 
|  \| | ___  ___  _ __  
| . ` |/ _ \/ _ \| '_ \ 
| |\  |  __/ (_) | | | |
\_| \_/\___|\___/|_| |_|                       
                        ");
			Console.WriteLine($"Starting Neon (branch {ThisAssembly.Git.Branch}) {ThisAssembly.Git.Commit} sha: {ThisAssembly.Git.Sha}");
		}
		private void ConfigureLogger(IConfigManager configManager)
		{
			if (_configManager == null)
			{
				Log.Logger = new LoggerConfiguration()
					.Filter.ByExcluding(Matching.FromSource("Microsoft"))
					.Filter
					.ByExcluding(Matching.FromSource("System"))
					.Enrich.FromLogContext()
					.MinimumLevel.Debug()
					.WriteTo.File(new CompactJsonFormatter(), "logs/Neon.log",
						rollingInterval: RollingInterval.Day)
					.WriteTo.Console(
						theme: AnsiConsoleTheme.Literate,
						outputTemplate:
						"{Timestamp:HH:mm:ss} [{Level}] [{SourceContext:u3}] {Message}{NewLine}{Exception}")
					.CreateLogger();
			}
			else
			{
				var logConfiguration = new LoggerConfiguration()
					.Filter.ByExcluding(Matching.FromSource("Microsoft"))
					.Filter
					.ByExcluding(Matching.FromSource("System"))
					.Enrich.FromLogContext();

				if (_configManager.Configuration.EngineConfig.Logger.Level == LogLevelEnum.Debug)
					logConfiguration = logConfiguration.MinimumLevel.Debug();

				if (_configManager.Configuration.EngineConfig.Logger.Level == LogLevelEnum.Info)
					logConfiguration = logConfiguration.MinimumLevel.Information();

				if (_configManager.Configuration.EngineConfig.Logger.Level == LogLevelEnum.Warning)
					logConfiguration = logConfiguration.MinimumLevel.Warning();

				if (_configManager.Configuration.EngineConfig.Logger.Level == LogLevelEnum.Error)
					logConfiguration = logConfiguration.MinimumLevel.Error();

				if (!string.IsNullOrEmpty(_configManager.Configuration.EngineConfig.Logger.LogDirectory))
					logConfiguration = logConfiguration.WriteTo.File(new CompactJsonFormatter(), _fileSystemManager.BuildFilePath(Path.Combine(_configManager.Configuration.EngineConfig.Logger.LogDirectory, "Neon.log")),
						rollingInterval: RollingInterval.Day);

				logConfiguration = logConfiguration.WriteTo.Console(
					theme: AnsiConsoleTheme.Code,
					outputTemplate:
					"{Timestamp:HH:mm:ss} [{Level}] [{SourceContext:u3}] {Message}{NewLine}{Exception}");

				Log.Logger = logConfiguration.CreateLogger();
			}
		}

		public bool Init()
		{
			_logger.Debug($"Registering Container");
			_containerBuilder.Register(c => _container).AsSelf();

			_logger.Debug($"Registering NeonManager");
			_containerBuilder.Register(n => this).As<INeonManager>().SingleInstance();

			_logger.Debug($"Registering Config Manager");
			_containerBuilder.Register(n => _configManager).As<IConfigManager>().SingleInstance();

			_logger.Debug($"Registering Secret Keys Manager");
			_containerBuilder.Register(n => _secretKeyManager).As<ISecretKeyManager>().SingleInstance();

			_logger.Debug($"Registering FileSystem Manager");
			_containerBuilder.Register(n => _fileSystemManager).As<IFileSystemManager>().SingleInstance();

			_logger.Debug($"Registering Services Manager");
			_containerBuilder.RegisterType<ServicesManager>().As<IServicesManager>().SingleInstance();

			_logger.Debug("Registering Plugin Manager");
			_containerBuilder.Register(n => _pluginsManager).As<IPluginsManager>().SingleInstance();

			_logger.Debug($"Registering Mediator");
			RegisterMediator();

			_logger.Debug($"Registering Script Modules");
			RegisterScriptModules();

			_logger.Debug($"Registering NoSQL connectors");
			RegisterNoSqlConnectors();

			_logger.Debug("Registering Components");
			RegisterComponents();

			_logger.Debug("Registering OAuth providers");
			RegisterOAuthReceivers();

			_logger.Debug("Registering Web Hooks provider");
			RegisterWebHooks();

			_logger.Debug("Registering WebSocket hubs");
			RegisterWebSockets();

			_logger.Debug("Registering Device Discovery Listeners");
			RegisterDeviceDiscovery();

			ScanTypes();

			_logger.Debug($"Registering Commands preload data");
			_containerBuilder.Register(n => _commandsCacheData).SingleInstance();

			return true;
		}

		private void ScanTypes()
		{
			_logger.Debug($"Scan for services");
			AssemblyUtils.GetAttribute<NeonServiceAttribute>().ForEach(s =>
			{
				_logger.Debug($"Registering service {s.Name}");

				_containerBuilder.RegisterType(s).As(AssemblyUtils.GetInterfaceOfType(s)).SingleInstance();
				AvailableServices.Add(s);
			});
		}

		private void RegisterWebSockets()
		{
			_logger.Debug($"Scan for WebSockets hub");
			AssemblyUtils.GetAttribute<WebSocketHubAttribute>().ForEach(w =>
			{
				_logger.Debug($"Registering WebSocket {w.Name}");
				_containerBuilder.RegisterType(w).SingleInstance();
			});
		}
		private void RegisterOAuthReceivers()
		{
			_logger.Debug($"Scan for OAuth Receivers");
			AssemblyUtils.GetAttribute<OAuthReceiverAttribute>().ForEach(r =>
			{
				var attr = r.GetCustomAttribute<OAuthReceiverAttribute>();
				_logger.Debug($"Registering provider {attr.ProviderName}");
				_oAuthReceiverData.Add(new OAuthReceiverData()
				{
					ProviderName = attr.ProviderName,
					ProviderType = r
				});
			});

			_containerBuilder.Register(e => _oAuthReceiverData).SingleInstance();
		}

		private void RegisterWebHooks()
		{
			_logger.Debug($"Scan for Web Hooks Receivers");
			AssemblyUtils.GetAttribute<WebHookReceiverAttribute>().ForEach(r =>
			{
				var attr = r.GetCustomAttribute<WebHookReceiverAttribute>();
				_logger.Debug($"Registering provider {attr.ProviderName}");
				_webHookReceiverData.Add(new WebHookReceiverData()
				{
					ProviderName = attr.ProviderName,
					ProviderType = r
				});
			});

			_containerBuilder.Register(e => _webHookReceiverData).SingleInstance();
		}

		private void RegisterDeviceDiscovery()
		{
			_logger.Debug("Scan for Device discovery services");
			AssemblyUtils.GetAttribute<DiscoveryServiceAttribute>().ForEach(a =>
			{
				var attr = a.GetCustomAttribute<DiscoveryServiceAttribute>();
				_logger.Debug($"Registering device discovery listener");

				_deviceDiscoveryListeners.Add(new DiscoveryListenerData()
				{
					ServiceName = attr.ServiceName,
					TypeName = a
				});
			});

			_containerBuilder.Register(context => _deviceDiscoveryListeners).SingleInstance();
		}

		private void RegisterComponents()
		{
			_logger.Debug($"Scan for Components");
			AssemblyUtils.GetAttribute<NeonComponentAttribute>().ForEach(c =>
			{
				_containerBuilder.RegisterType(c).As(AssemblyUtils.GetInterfaceOfType(c)).As(c).SingleInstance();
				ScanForComponentsCommands(c);

			});
		}

		private void ScanForComponentsCommands(Type componentType)
		{
			_logger.Debug($"Scan {componentType.Name} for commands");
			componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(m =>
			{
				var attr = m.GetCustomAttribute<ComponentCommandAttribute>();

				if (attr != null)
				{
					var commandEntry = new CommandPreloadData()
					{
						CommandName = attr.Name,
						HelpText = attr.HelpText,
						SourceType = componentType,
						ReturnType = AssemblyUtils.GetGenericTaskGenericType(m.ReturnType).Name,
						IsAsync = AssemblyUtils.IsGenericTaskType(m.ReturnType),
						Method = m
					};
					foreach (var parameterInfo in m.GetParameters())
					{
						commandEntry.Params.Add(new CommandPreloadParam()
						{
							Order = parameterInfo.Position,
							ParamName = parameterInfo.Name,
							ParamType = parameterInfo.ParameterType.Name
						});
					}

					_logger.Debug($"Command name '{attr.Name}' in component {componentType.Name} method: {m.Name}");
					_commandsCacheData.Add(commandEntry);
				}
			});
		}



		private void RegisterScriptModules()
		{
			_logger.Debug($"Scan for Script Modules");
			AssemblyUtils.GetAttribute<ScriptModuleAttribute>().ForEach(m =>
				{
					_logger.Debug($"Registering Script module {m.Name}");
					ContainerBuilder.RegisterType(m).SingleInstance();
				});
		}

		private void RegisterMediator()
		{
			ContainerBuilder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

			AssemblyUtils.GetAppAssemblies().ForEach(a =>
			{
				ContainerBuilder
					.RegisterAssemblyTypes(a)
					.AsClosedTypesOf(typeof(IRequestHandler<,>))
					.AsImplementedInterfaces().SingleInstance(); ;

				ContainerBuilder
					.RegisterAssemblyTypes(a)
					.AsClosedTypesOf(typeof(INotificationHandler<>))
					.AsImplementedInterfaces().SingleInstance();
			});

			ContainerBuilder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
			ContainerBuilder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));

			ContainerBuilder.Register<ServiceFactory>(ctx =>
			{
				var c = ctx.Resolve<IComponentContext>();
				return t => c.Resolve(t);
			});
		}

		private void RegisterNoSqlConnectors()
		{
			AssemblyUtils.GetAttribute<NoSqlConnectorAttribute>().ForEach(t =>
				{
					ContainerBuilder.RegisterType(t).InstancePerDependency();
				});
		}

		public async Task Start()
		{
			await _servicesManager.Start();
		}

		/// <summary>
		/// Build container
		/// </summary>
		/// <returns></returns>
		public IContainer Build()
		{

			_container = _containerBuilder.Build();

			_servicesManager = _container.Resolve<IServicesManager>();

			return _container;
		}

		public async Task Shutdown()
		{

			await _servicesManager.Stop();
			_fileSystemManager.Stop();
		}

		public T Resolve<T>()
		{
			return _container.Resolve<T>();
		}

		public object Resolve(Type t)
		{
			return _container.Resolve(t);
		}
	}
}
