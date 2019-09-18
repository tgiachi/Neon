using System;
using System.Collections.Generic;
using System.Text;
using Neon.Api.Attributes.ScriptEngine;
using Neon.Api.Utils;

namespace Neon.Engine.Std
{

	[ScriptModule]
	public class NetworkScriptModule
	{
		[ScriptFunction("wakeup_on_lan", "Send magic packet for wake up")]
		public void WakeUpLan(string macAddress)
		{
			NetworkUtils.SendWakeUpOnLanPackage(macAddress);
		}
	}
}
