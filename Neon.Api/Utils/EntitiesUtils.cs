using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Utils
{
	public static class EntitiesUtils
	{
		public static string GenerateId()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}
	}
}
