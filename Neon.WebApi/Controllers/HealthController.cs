using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Neon.WebApi.Controllers
{

	[Route("api/heath")]
	[ApiController]
	public class HealthController : ControllerBase
	{
		public ActionResult<string> Ping()
		{
			return Ok("pong");
		}
	}
}
