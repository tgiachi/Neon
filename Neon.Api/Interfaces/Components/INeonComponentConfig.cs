namespace Neon.Api.Interfaces.Components
{
	public interface INeonComponentConfig
	{
		int PollingSeconds { get; set; }

		bool IsEnabled { get; set; }
	}
}
