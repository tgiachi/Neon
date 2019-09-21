using System;
using System.Collections.Generic;
using System.Text;

namespace Neon.Api.Data.ScriptEngine
{
	public class ScriptExecutionResult
	{
		public string Script { get; set; }

		public object Result { get;set; }

		public bool IsError { get;set; }

		public string ErrorMessage { get; set; }
	}
}
