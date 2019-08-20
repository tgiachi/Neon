using System;

namespace Neon.Api.Attributes.ScriptEngine
{

	[AttributeUsage(AttributeTargets.Method)]
	public class ScriptFunctionAttribute : Attribute
	{
		public string FunctionName { get; set; }

		public string HelpText { get; set; }

		public ScriptFunctionAttribute(string functionName, string helpText)
		{
			FunctionName = functionName;
			HelpText = helpText;
		}
	}
}
