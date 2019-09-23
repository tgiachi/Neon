using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;

namespace Neon.WebApi.Controllers
{
	[ApiController]
	[Route("api/entities")]
	public class EntitiesController : ControllerBase
	{
		private readonly IIoTService _ioTService;
		public EntitiesController(IIoTService ioTService)
		{
			_ioTService = ioTService;
		}

		[HttpGet]
		[Route("all")]
		public ActionResult<List<object>> GetEntities()
		{
			return _ioTService.GetEntities();
		}
	}
}
