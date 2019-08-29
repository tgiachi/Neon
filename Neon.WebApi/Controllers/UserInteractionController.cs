using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.UserInteraction;
using Neon.Api.Interfaces.Services;

namespace Neon.WebApi.Controllers
{
	[Route("/user/interaction")]
	public class UserInteractionController : ControllerBase
	{

		private readonly IUserInteractionService _interactionService;

		public UserInteractionController(IUserInteractionService userInteractionService)
		{
			_interactionService = userInteractionService;
		}

		[HttpGet]
		[Route("/requests")]
		public ActionResult<UserInteractionData> GetData()
		{
			return Ok(_interactionService.NeedUserInteractionData);
		}
	}
}
