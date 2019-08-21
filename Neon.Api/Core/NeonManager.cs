using Autofac;
using MediatR;
using MediatR.Pipeline;
using Neon.Api.Attributes;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.NoSql;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Commands;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using Serilog;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
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
		private ContainerBuilder _containerBuilder;

		private IServicesManager _servicesManager;
		private IConfigManager _configManager;
		private IFileSystemManager _fileSystemManager;
		private ISecretKeyManager _secretKeyManager;

		public ContainerBuilder ContainerBuilder => _containerBuilder;
		public List<Type> AvailableServices { get; }
		public NeonConfig Config => _configManager.Configuration;

		private readonly List<CommandPreloadData> _commandPreloadData = new List<CommandPreloadData>();

		public bool IsRunningInDocker { get; }


		public NeonManager()
		{
			ConfigureLogger();

			_logger = Log.Logger;
			_logger.Debug($"Pre-loading assemblies");
			AssemblyUtils.GetAppAssemblies();

			AvailableServices = new List<Type>();
			IsRunningInDocker = Environment.GetEnvironmentVariables()["DOTNET_RUNNING_IN_CONTAINER"] != null;

			_containerBuilder = new ContainerBuilder();
			_containerBuilder.RegisterBuildCallback(container => { _logger.Debug($"Container is ready"); });

			_configManager = new ConfigManager(_logger, this, _containerBuilder);
			_configManager.LoadConfig();

			_secretKeyManager = new SecretKeyManager(Config.EngineConfig.SecretKey);

			_fileSystemManager = new FileSystemManager(_logger, Config, _secretKeyManager);

		}

		private void ConfigureLogger()
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
					outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] [{SourceContext:u3}] {Message}{NewLine}{Exception}")
				.CreateLogger();
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

			_logger.Debug($"Registering Mediator");
			RegisterMediator();

			_logger.Debug($"Registering Script Modules");
			RegisterScriptModules();

			_logger.Debug($"Registering NoSQL connectors");
			RegisterNoSqlConnectors();

			_logger.Debug("Registering Components");
			RegisterComponents();

			ScanTypes();

			_logger.Debug($"Registering Commands preload data");
			_containerBuilder.Register(n => _commandPreloadData).SingleInstance();

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
					_commandPreloadData.Add(commandEntry);
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
			_fileSystemManager.Start();

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
