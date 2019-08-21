using System;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using NLua;

namespace Neon.Engine.Std
{

	[ScriptModule]
	public class EventsScriptModule
	{
		private readonly IRuleEngineService _ruleEngineService;
		private readonly IIoTService _ioTService;
		private readonly ISchedulerService _schedulerService;
		private readonly CommonScriptModule _commonScriptModule;

		public EventsScriptModule(IRuleEngineService ruleEngineService,
			IIoTService ioTService,
			ISchedulerService schedulerService,
			CommonScriptModule commonScriptModule)
		{
			_ruleEngineService = ruleEngineService;
			_ioTService = ioTService;
			_schedulerService = schedulerService;
			_commonScriptModule = commonScriptModule;
			
		}

		[ScriptFunction("add_rule", "Add rule to rule engine")]
		public void AddRule(string ruleName, string entityName, string condition, LuaFunction function)
		{
			_ruleEngineService.AddRule(ruleName, AssemblyUtils.GetType(_commonScriptModule.GetEntityType(entityName)), condition,
				entity => { function.Call(entity); });
		}

		[ScriptFunction("add_alarm", "Add new alarm")]
		public void AddAlarm(string name, int hours, int minutes, LuaFunction function)
		{
			_schedulerService.AddJob(() => { function.Call(); }, name, hours, minutes);
		}

		[ScriptFunction("add_timer", "Add timer in seconds")]
		public void AddTimer(string name, int seconds, LuaFunction function)
		{
			_schedulerService.AddJob(() => { function.Call(); }, name, seconds, false);
		}


		[ScriptFunction("on_event_type", "Subscribe on event")]
		public void OnEventType(string eventType, LuaFunction function)
		{

			_ioTService.GetEventStream<INeonIoTEntity>().Subscribe(entity =>
			{
				if (string.Equals(Type.GetType(entity.EntityType).Name, eventType, StringComparison.CurrentCultureIgnoreCase))
				{
					function.Call(entity);
				}
			});
		}

		[ScriptFunction("on_event_name", "Subscribe on event")]
		public void OnEventName(string entityName, LuaFunction function)
		{
			_ioTService.GetEventStream<INeonIoTEntity>().Subscribe(entity =>
			{
				if (string.Equals(entity.Name, entityName, StringComparison.CurrentCultureIgnoreCase))
				{
					try
					{
						function.Call(entity);
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}
			});
		}
	}
}
