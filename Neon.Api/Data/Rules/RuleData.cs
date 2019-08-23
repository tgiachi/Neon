using Neon.Api.Interfaces.Entity;
using System;

namespace Neon.Api.Data.Rules
{
	public class RuleData
	{
		public string RuleName { get; set; }

		public object RuleCondition { get; set; }

		public Type EntityType { get; set; }

		public Action<INeonIoTEntity> Action { get; set; }

		public RuleTypeEnum RuleType { get; set; }

		public bool IsEnabled { get; set; }

	}
}
