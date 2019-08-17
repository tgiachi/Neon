using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Neon.Api.Core;
using Neon.Api.Interfaces;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Neon.WebApi
{
	public class Program
	{
		public static INeonManager NeonManager { get; set; }


		public static void Main(string[] args)
		{





			try
			{
				CreateWebHostBuilder(args).Run();
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IWebHost CreateWebHostBuilder(string[] args)
		{

			NeonManager = new NeonManager();

			var host = WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseLibuv()
				.UseIISIntegration()
				.UseSerilog().Build();

			return host;
		}



	}
}
