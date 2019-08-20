using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.Rules;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;
using NLua;
using NReco.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{
	[NeonService("Rule Engine", "Execute rules")]
	public class RuleEngineService : IRuleEngineService
	{
		private readonly ILogger _logger;
		private readonly IIoTService _ioTService;

		public ObservableCollection<RuleData> Rules { get; }

		public RuleEngineService(ILogger<IRuleEngineService> logger, IIoTService ioTService)
		{
			_logger = logger;
			_ioTService = ioTService;
			Rules = new ObservableCollection<RuleData>();
		}

		public Task<bool> Start()
		{
			_ioTService.GetEventStream<INeonIoTEntity>().Subscribe(ParseRule);

			return Task.FromResult(true);
		}

		private void ParseRule(INeonIoTEntity entity)
		{
			Rules.Where(r => r.EntityType == entity.GetType()).ToList().ForEach(r =>
			{
				switch (r.RuleType)
				{
					case RuleTypeEnum.CSharp:
						ExecuteCSharpRule(r, entity);
						break;
					case RuleTypeEnum.Lambda:
						ExecuteLuaRule(r, entity);
						break;
					case RuleTypeEnum.Lua:
						ExecuteLuaRule(r, entity);
						break;
				}
			});
		}

		private void ExecuteLuaRule(RuleData rule, INeonIoTEntity entity)
		{
			var lParser = new LambdaParser();
			var context = new Dictionary<string, object> { { "entity", entity }, { "rule", rule } };


			var bResult = (bool)lParser.Eval((string)rule.RuleCondition, context);

			if (bResult)
			{
				_logger.LogInformation($"Executing rule {rule.RuleName}");
				rule.Action.Invoke(entity);
			}
		}

		private void ExecuteCSharpRule(RuleData rule, INeonIoTEntity entity)
		{
			var func = (Func<INeonIoTEntity, bool>)rule.RuleCondition;

			if (func.Invoke(entity))
			{
				rule.Action.Invoke(entity);
			}
		}

		public void AddRule(string ruleName, Type entityType, string condition, Action<INeonIoTEntity> action)
		{
			Rules.Add(new RuleData()
			{
				IsEnabled = true,
				EntityType = entityType,
				Action = action,
				RuleCondition = condition,
				RuleName = ruleName,
				RuleType = RuleTypeEnum.Lambda
			});
		}

		public void AddRule(string ruleName, Type entityType, Func<INeonIoTEntity, bool> condition, Action<INeonIoTEntity> action)
		{
			Rules.Add(new RuleData()
			{
				IsEnabled = true,
				EntityType = entityType,
				Action = action,
				RuleCondition = condition,
				RuleName = ruleName,
				RuleType = RuleTypeEnum.CSharp
			});
		}

		public void AddRule(string ruleName, Type entityType, string condition, LuaFunction action)
		{
			Rules.Add(new RuleData()
			{
				IsEnabled = true,
				EntityType = entityType,
				Action = entity => { action.Call(entity); },
				RuleCondition = condition,
				RuleName = ruleName,
				RuleType = RuleTypeEnum.Lua
			});
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}
	}
}
