using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.Commands;
using Neon.Api.Interfaces.Services;
using System.Collections.Generic;

namespace Neon.WebApi.Controllers
{

	[ApiController]
	[Route("api/commands")]
	public class CommandsController : ControllerBase
	{
		private readonly ICommandDispatcherService _commandDispatcherService;

		public CommandsController(ICommandDispatcherService commandDispatcherService)
		{
			_commandDispatcherService = commandDispatcherService;
		}


		[HttpGet]
		[Route("available")]
		public ActionResult<List<CommandPreloadData>> GetAvailableCommands()
		{
			var list = _commandDispatcherService.CommandPreload; 
			return Ok(list);
		}
	}
}
