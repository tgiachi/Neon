using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;

namespace Neon.Api.Data.States
{
	public class StateData
	{
		[YamlMember(Alias = "states")]
		public List<StateEntryData> Values { get; set; }

		public StateData()
		{
			Values = new List<StateEntryData>();
		}
	}
}
