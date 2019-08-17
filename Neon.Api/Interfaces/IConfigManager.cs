using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neon.Api.Data.Config;
using Neon.Api.Data.Config.Root;

namespace Neon.Api.Interfaces
{
	public interface IConfigManager
	{
		NeonConfig Configuration { get; set; }

		bool LoadConfig();
	}
}
