﻿using Neon.Api.Data.ScriptEngine;
using Neon.Api.Interfaces.Base;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Services
{
	public interface IScriptEngineService : INeonService
	{

		List<ScriptFunctionData> Functions { get; }

		List<ScriptEngineVariable> Variables { get; }
		void LoadFile(string fileName, bool immediateExecute);

		void RegisterFunction(string functionName, object obj, MethodInfo method);

		object ExecuteCode(string code);

		object StatementToObject(string code);

		Task<bool> Build();

		void AddVariable(string variable, object value);
	}
}
