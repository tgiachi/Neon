﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Neon.Engine.Data
{
	public class SunSetData
	{
		[JsonProperty("status")]
		public string Status { get;set; }

		[JsonProperty("results")]
		public SunSetResultData Results { get; set; }

	}
}
