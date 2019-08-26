using Microsoft.Extensions.Logging;
using Neon.Api.Attributes;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using System;
using System.Threading.Tasks;

namespace Neon.Engine.Components
{
	[NeonComponent("test", "v1.0", "TEST", typeof(TestComponentConfig))]
	public class TestComponent : AbstractNeonComponent<TestComponentConfig>
	{
		public TestComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

		}

		[ComponentPollRate(30)]
		public override Task Poll()
		{
			var rnd = new Random();
			Logger.LogInformation($"Test");
			var entity = new TestEvent()
			{
				Name = "TEST_ENTITY",
				Value = rnd.Next(0, 5)
			};

			PublishEntity(entity);

			throw new Exception("error");
			//		return base.Poll();
		}

		[ComponentCommand("test", "test command dispatcher")]
		public string TestCommand(string name)
		{
			return "sync 1";
		}

		[ComponentCommand("test2", "test command dispatcher")]
		public async Task<string> TestCommand2(string name)
		{
			await Task.Delay(3000);
			return "async 3";
		}

		public override object GetDefaultConfig()
		{
			return new TestComponentConfig() { IsEnabled = true, TestValue = "1234" };
		}
	}
}
