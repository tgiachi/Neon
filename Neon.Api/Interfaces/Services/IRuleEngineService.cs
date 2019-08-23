using Neon.Api.Data.Rules;
using Neon.Api.Interfaces.Base;
using Neon.Api.Interfaces.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Neon.Api.Interfaces.Services
{
	public interface IRuleEngineService : INeonService
	{

		/// <summary>
		/// Add rule with string condition with Lambda parser
		/// </summary>
		/// <param name="ruleName"></param>
		/// <param name="condition"></param>
		/// <param name="action"></param>
		void AddRule(string ruleName, Type entityType, string condition, Action<INeonIoTEntity> action);

		/// <summary>
		/// Add rule with function boolean 
		/// </summary>
		/// <param name="ruleName"></param>
		/// <param name="condition"></param>
		/// <param name="action"></param>
		void AddRule(string ruleName, Type entityType, Func<INeonIoTEntity, bool> condition, Action<INeonIoTEntity> action);

		/// <summary>
		/// Get all rules
		/// </summary>
		ObservableCollection<RuleData> Rules { get; }


		List<RuleInfo> RulesExecutionInfo { get; }
	}
}
