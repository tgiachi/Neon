using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Websocket;
using Neon.Api.Data.Config.Root;
using Neon.Api.Logger;
using Neon.Api.Utils;
using System;
using System.Reflection;
using WebSocketManager;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Neon.WebApi
{
	public class Startup
	{
		private IContainer ApplicationContainer { get; set; }

		private readonly ILogger _logger;

		private NeonConfig _config;

		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<Startup>();
		}

		public IServiceProvider ConfigureServices(IServiceCollection services)
		{

			_config = Program.NeonManager.Config;

			services.AddOpenApiDocument();

			services.AddCors(c =>
			{
				c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
			});


			services.AddMvc().AddMetrics()
				.SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
				.AddControllersAsServices();

			services.AddSingleton(typeof(ILogger<>), typeof(LoggerEx<>));

			services.AddMediatR(AssemblyUtils.GetAppAssemblies().ToArray());
			services.AddHttpClient();
			services.AddWebSocketManager();

			Program.NeonManager.ContainerBuilder.Populate(services);

			Program.NeonManager.Init();

			ApplicationContainer = Program.NeonManager.Build();

			return new AutofacServiceProvider(ApplicationContainer);

		}

		public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
		{

			appLifetime.ApplicationStopped.Register(async () => await Program.NeonManager.Shutdown());

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseCors(options => options.AllowAnyOrigin());
			//app.UseHttpsRedirection();

			if (_config.EngineConfig.UseSwagger)
			{
				_logger.LogInformation($"Configuring Swagger on /swagger/ endpoint");
				_logger.LogInformation($"Configuring ReDoc on /redoc/ endpoint");

				app.UseOpenApi(); // serve OpenAPI/Swagger documents
				app.UseSwaggerUi3(settings => settings.Path = "/swagger");
				app.UseReDoc(settings => settings.Path = "/redoc");
			}


			app.UseWebSockets();

			AssemblyUtils.GetAttribute<WebSocketHubAttribute>().ForEach(t =>
			{
				var wsAttr = t.GetCustomAttribute<WebSocketHubAttribute>();
				_logger.LogInformation($"Registering websocket path {wsAttr.Path} to {t.Name}");

				app.MapWebSocketManager(wsAttr.Path, ApplicationContainer.Resolve(t) as WebSocketHandler);
			});

			app.UseMvc();

			await Program.NeonManager.Start();
		}
	}
}
