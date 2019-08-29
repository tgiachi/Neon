using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.Components;
using Neon.Api.Interfaces.Services;

namespace Neon.WebApi.Controllers
{
	[ApiController]
	[Route("/components")]
	public class ComponentsController : ControllerBase
	{
		private readonly IComponentsService _componentsService;

		public ComponentsController(IComponentsService componentsService)
		{
			_componentsService = componentsService;
		}

		[Route("/available")]
		[HttpGet]
		public ActionResult<List<AvailableComponent>> GetAvailableComponents()
		{
			return Ok(_componentsService.AvailableComponents);
		}

		[Route("/running")]
		[HttpGet]
		public ActionResult<List<ComponentData>> GetRunningComponents()
		{
			return Ok(_componentsService.ComponentsData.ToList());
		}

		[HttpPost]
		[Route("/start/{name}")]
		public async Task<ActionResult<bool>> StartComponent(string name)
		{
			return	Ok(await _componentsService.LoadComponent(name));
		}

	}
}
