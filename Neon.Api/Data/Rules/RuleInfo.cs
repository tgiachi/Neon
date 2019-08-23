using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.Rules
{
	public class RuleInfo
	{
		public string RuleName { get; set; }

		public DateTime LastExecutionDateTime { get; set; }

		public int RuleExecutionCount { get; set; }
	}
}
