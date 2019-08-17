using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Neon.Api.Core;
using Neon.Api.Interfaces;

namespace Neon.WebApi
{
	public class Startup
	{

		private INeonManager _neonManager;

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public async void ConfigureServices(IServiceCollection services)
		{
			_neonManager =
				new NeonManager(LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<NeonManager>());

			await _neonManager.Init();
			services.AddControllers();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Neon API", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Neon API");
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});


			await _neonManager.Start();

		}
	}
}
