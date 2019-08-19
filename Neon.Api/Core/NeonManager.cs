﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using MediatR;
using MediatR.Pipeline;
using Neon.Api.Attributes;
using Neon.Api.Data.Config.Root;
using Neon.Api.Interfaces;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Utils;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;


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

		public ContainerBuilder ContainerBuilder => _containerBuilder;
		public List<Type> AvailableServices { get; }
		public NeonConfig Config => _configManager.Configuration;

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

			_logger.Debug($"Registering Services Manager");
			_containerBuilder.RegisterType<ServicesManager>().As<IServicesManager>().SingleInstance();


			RegisterMediator();

			ScanTypes();

			
			

			return true;
		}

		private void ScanTypes()
		{
			_logger.Debug($"Scan for services");
			AssemblyUtils.GetAttribute<NeonServiceAttribute>().ForEach(s =>
			{
				_logger.Debug($"Registering type {s.Name}");

				_containerBuilder.RegisterType(s).As(AssemblyUtils.GetInterfaceOfType(s)).SingleInstance();
				AvailableServices.Add(s);
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
