using System;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.ScriptEngine;
using Neon.Api.Interfaces.Services;
using System.Collections.Generic;

namespace Neon.WebApi.Controllers
{

	[ApiController]
	[Route("api/scriptengine/")]
	public class ScriptEngineController : ControllerBase
	{
		private readonly IScriptEngineService _scriptEngineService;

		public ScriptEngineController(IScriptEngineService scriptEngineService)
		{
			_scriptEngineService = scriptEngineService;
		}

		[HttpGet]
		[Route("functions")]
		public ActionResult<List<ScriptFunctionData>> GetAvailableFunctions()
		{
			return Ok(_scriptEngineService.Functions);
		}

		[HttpGet]
		[Route("variables")]
		public ActionResult<List<ScriptEngineVariable>> GetAvailableVariables()
		{
			return Ok(_scriptEngineService.Variables);
		}

		[HttpPost]
		[Route("execute/script")]
		public ActionResult<ScriptExecutionResult> ExecuteScript([FromBody] string script)
		{
			var result = new ScriptExecutionResult();
			result.Script = script;
			try
			{
			   result.Result = _scriptEngineService.ExecuteCode(script);
			}
			catch (Exception ex)
			{
				result.IsError = true;
				result.ErrorMessage = ex.Message;
			}


			return Ok(result);
		}
	}
}
