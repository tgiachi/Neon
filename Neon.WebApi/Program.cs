using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Neon.Api.Core;
using Neon.Api.Interfaces.Managers;
using Serilog;

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
				.UseSerilog();

			host = host.UseUrls("http://0.0.0.0:5000", "https://0.0.0.0:5001", "http://::5000", "https://::5001");

			return host.Build();
		}



	}
}
