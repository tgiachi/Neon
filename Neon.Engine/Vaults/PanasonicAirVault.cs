using System;
using System.Collections.Generic;
using System.Text;
using Q42.HueApi.Models;
using YamlDotNet.Serialization;

namespace Neon.Engine.Vaults
{
	public class PanasonicAirVault
	{
		[YamlMember(Alias = "token")]
		public string AccessToken { get; set; }

	}
}
