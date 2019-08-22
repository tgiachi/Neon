using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Neon.Api.Utils
{
	public static class HttpClientUtils
	{
		public static FormUrlEncodedContent BuildFormParams(params KeyValuePair<string, string>[] args)
		{
			return new FormUrlEncodedContent(args);
		}
	}
}
