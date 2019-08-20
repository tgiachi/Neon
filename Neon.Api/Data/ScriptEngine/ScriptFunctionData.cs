using System.Collections.Generic;

namespace Neon.Api.Data.ScriptEngine
{
	public class ScriptFunctionData
	{
		public string ScriptModuleName { get; set; }
		public string FunctionName { get; set; }
		public string HelpText { get; set; }

		public List<ScriptFunctionParamData> Parameters { get; set; }

		public string OutputType { get; set; }

		public ScriptFunctionData()
		{
			Parameters = new List<ScriptFunctionParamData>();
		}

		public override string ToString()
		{
			return $"ScriptModuleName: {ScriptModuleName} - Function: {FunctionName}";
		}
	}
}
