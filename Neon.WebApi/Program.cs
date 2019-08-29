using App.Metrics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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
				.ConfigureKestrel(opts =>
				{
					opts.ListenAnyIP(5000, options => options.Protocols = HttpProtocols.Http1AndHttp2);
				})
				.UseStartup<Startup>()
				.UseLibuv()
				.UseMetrics()
				.UseIISIntegration()
				.UseSerilog();

			return host.Build();
		}



	}
}
