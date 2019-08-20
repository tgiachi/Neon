using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;

namespace Neon.Engine.Components
{
	[NeonComponent("test", "v1.0", "TEST", typeof(TestComponentConfig))]
	public class TestComponent : AbstractNeonComponent<TestComponentConfig>
	{
		public TestComponent(ILoggerFactory loggerFactory, IIoTService ioTService) : base(loggerFactory, ioTService)
		{

		}

		[ComponentPollRate(10)]
		public override Task Poll()
		{
			Logger.LogInformation($"Test");
			return base.Poll();
		}

		public override object GetDefaultConfig()
		{
			return new TestComponentConfig() {IsEnabled = true, TestValue = "1234"};
		}
	}
}
