using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Neon.Api.Data.Config.Common;
using Neon.Api.Impl.Components;
using YamlDotNet.Serialization;

namespace Neon.Engine.Components.Configs
{
	public class DarkSkyConfig : BaseComponentConfig
	{
		[YamlMember(Alias = "api")]
		public ApiConfig ApiConfig { get; set; }

		[YamlMember(Alias = "language_code")]
		public string LanguageCode { get; set; }

		[YamlMember(Alias = "measurement_units")]
		public string MeasurementUnits { get; set; }

		public DarkSkyConfig()
		{
			ApiConfig = new ApiConfig()
			{
				ApiKey = "none"
			};

			MeasurementUnits = "si";
			LanguageCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		}
	}
}
