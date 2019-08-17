using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neon.Api.Core;
using Neon.Api.Interfaces;
using Neon.Api.Logger;
using Neon.Api.Utils;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;

namespace Neon.WebApi
{
	public class Startup
	{
		public IContainer ApplicationContainer { get; set; }
		public IConfiguration Configuration { get; }
		private readonly ILoggerFactory _loggerFactory;

		

		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			Configuration = configuration;
			_loggerFactory = loggerFactory;
		}


		// This method gets called by the runtime. Use this method to add services to the container.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Info() { Title = "Neon", Version = "v1" });
			});


			services.AddMvc();
			services.AddSingleton(typeof(ILogger<>), typeof(LoggerEx<>));

			services.AddMediatR(AssemblyUtils.GetAppAssemblies().ToArray());

			Program.NeonManager.ContainerBuilder.Populate(services);

			Program.NeonManager.Init();
			ApplicationContainer = Program.NeonManager.Build();

			return new AutofacServiceProvider(ApplicationContainer);

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime appLifetime)
		{

			appLifetime.ApplicationStopped.Register(async () => await Program.NeonManager.Shutdown());

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseSwagger();
			app.UseHttpsRedirection();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neon v1");
			});
			app.UseMvc();

			await Program.NeonManager.Start();
		}
	}
}
