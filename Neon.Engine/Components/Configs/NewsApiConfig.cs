using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Neon.Api.Data.Config.Common;
using Neon.Api.Impl.Components;

namespace Neon.Engine.Components.Configs
{
	public class NewsApiConfig : BaseComponentConfig
	{
		public string ApiKey { get; set; }

		public string Language { get; set; }

		public int PageSize { get; set; }

		public List<string> Sources { get; set; }

		public NewsApiConfig()
		{
			ApiKey = "change_me";
			PageSize = 10;
			Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToUpper();
			Sources = new List<string>();
		}
	}
}
