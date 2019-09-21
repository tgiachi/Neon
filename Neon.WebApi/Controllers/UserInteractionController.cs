using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.UserInteraction;
using Neon.Api.Interfaces.Services;

namespace Neon.WebApi.Controllers
{
	[Route("api/user/interaction")]
	public class UserInteractionController : ControllerBase
	{

		private readonly IUserInteractionService _interactionService;

		public UserInteractionController(IUserInteractionService userInteractionService)
		{
			_interactionService = userInteractionService;
		}

		[HttpGet]
		[Route("requests")]
		public ActionResult<UserInteractionData> GetData()
		{
			return Ok(_interactionService.NeedUserInteractionData);
		}

		[HttpPost]
		[Route("perform")]
		public ActionResult<bool> PerformUserInteraction([FromBody] UserInteractionInput input)
		{
			_interactionService.CompileEntry(input.Name, input.Field, input.Value);
			return Ok(true);

		}
	}
}
