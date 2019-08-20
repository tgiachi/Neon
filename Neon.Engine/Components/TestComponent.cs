using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;

namespace Neon.Engine.Components
{
	[NeonComponent("test", "v1.0", "TEST", typeof(TestComponentConfig))]
	public class TestComponent : AbstractNeonComponent<TestComponentConfig>
	{
		public TestComponent(ILoggerFactory loggerFactory, IIoTService ioTService) : base(loggerFactory, ioTService)
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

			return base.Poll();
		}

		public override object GetDefaultConfig()
		{
			return new TestComponentConfig() {IsEnabled = true, TestValue = "1234"};
		}
	}
}
