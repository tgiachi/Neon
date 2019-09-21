using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Interfaces.Services;

namespace Neon.WebApi.Controllers
{

	[Route("api/routines")]
	[ApiController]
	public class RoutinesController : ControllerBase
	{

		private readonly IRoutineService _routineService;
		public RoutinesController(IRoutineService routineService)
		{
			_routineService = routineService;
		}

		[Route("names")]
		[HttpGet]
		public ActionResult<List<string>> GetRoutinesNames()
		{
			return Ok(_routineService.RoutineNames);
		}
	}
}
