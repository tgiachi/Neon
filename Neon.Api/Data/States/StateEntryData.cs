using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.States
{
	public class StateEntryData
	{
		[YamlMember(Alias = "name")]
		public string StateName { get; set; }

		[YamlMember(Alias = "value")]
		public object StateValue { get; set; }
	}
}
