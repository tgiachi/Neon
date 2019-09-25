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

		[HttpGet]
		[Route("names")]
		public ActionResult<List<string>> GetEntitiesNames()
		{
			return _ioTService.GetEventsNames;
		}

		[HttpGet]
		[Route("get/all")]
		public ActionResult<List<INeonEntity>> GetAllEntities()
		{
			var result = new List<object>();
			_ioTService.GetEventsNames.ForEach(e =>
			{
				result.AddRange(_ioTService.GetEntitiesCollectionByName(e));

			});

			return Ok(result);
		}

		[HttpGet]
		[Route("get/{name}")]
		public ActionResult<List<INeonIoTEntity>> GetEntityCollection(string name)
		{
			return Ok(_ioTService.GetEntitiesCollectionByName(name));
		}
	}
}
