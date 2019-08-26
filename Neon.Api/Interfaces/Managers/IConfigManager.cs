using Neon.Api.Data.Config.Root;

namespace Neon.Api.Interfaces.Managers
{
	public interface IConfigManager
	{
		NeonConfig Configuration { get; }

		bool LoadConfig();

		void SaveConfig();
	}
}
