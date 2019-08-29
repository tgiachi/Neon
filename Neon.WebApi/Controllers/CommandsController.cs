using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.Commands;
using Neon.Api.Interfaces.Services;

namespace Neon.WebApi.Controllers
{

	[ApiController]
	[Route("/commands")]
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
			return Ok(_commandDispatcherService.CommandPreload);
		}
	}
}
