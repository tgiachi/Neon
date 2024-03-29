﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Neon.WebApi.Controllers
{

	[Route("api/health")]
	[ApiController]
	public class HealthController : ControllerBase
	{
		public ActionResult<string> Ping()
		{
			return Ok("pong");
		}
	}
}
