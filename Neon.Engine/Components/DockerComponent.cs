using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs.AirConditioned;
using Neon.Engine.Components.Configs.System;
using Neon.Engine.Components.Events;

namespace Neon.Engine.Components
{
	[NeonComponent("docker", "v1.0.0.0", "SYSTEM", typeof(DockerConfig))]
	public class DockerComponent : AbstractNeonComponent<DockerConfig>
	{
		private IDockerClient _dockerClient;

		public DockerComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{


		}

		public override Task<bool> Start()
		{
			if (Config.ServerUrl == "change_me")
				ThrowComponentNeedConfiguration("ServerUrl");


			_dockerClient = new DockerClientConfiguration(new Uri(Config.ServerUrl)).CreateClient();

			return base.Start();
		}

		[ComponentPollRate(60)]
		public override async Task Poll()
		{

			await GetContainers();


		}

		private async Task GetContainers()
		{
			var containers = await _dockerClient.Containers.ListContainersAsync(new ContainersListParameters() { All = true });

			foreach (var container in containers)
			{
				var containerEvent = BuildEntity<DockerContainerEvent>();

				containerEvent.Name = container.Names[0];
				containerEvent.Created = container.Created;
				containerEvent.Image = container.Image;
				containerEvent.ImageId = container.ImageID;
				containerEvent.State = container.State;
				containerEvent.Status = container.Status;

				PublishEntity(containerEvent);
			}
		}

		public override void Dispose()
		{
			_dockerClient?.Dispose();
			base.Dispose();
		}
	}


}
