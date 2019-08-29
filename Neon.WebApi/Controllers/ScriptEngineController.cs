using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.ScriptEngine;
using Neon.Api.Interfaces.Services;
using Tmds.Linux;

namespace Neon.WebApi.Controllers
{

	[ApiController]
	[Route("/scriptengine/")]
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
	}
}
