using Microsoft.AspNetCore.Mvc;
using Neon.Api.Data.WebHook;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.WebHook;
using System.Collections.Generic;
using System.Linq;

namespace Neon.WebApi.Controllers
{
	[ApiController]
	[Route("api/hooks")]
	public class HooksController : ControllerBase
	{

		private readonly List<WebHookReceiverData> _webHookReceiverData;
		private readonly INeonManager _neonManager;
		public HooksController(List<WebHookReceiverData> webHookReceiverData, INeonManager neonManager)
		{
			_webHookReceiverData = webHookReceiverData;
			_neonManager = neonManager;
		}
		[HttpGet]
		[HttpPost]
		[Route("{provider}")]
		public ActionResult OAuth(string provider, [FromForm] string payload)
		{
			var providerData = _webHookReceiverData.FirstOrDefault(r => r.ProviderName == provider);

			if (providerData == null) return NotFound();


			var oauthReceiver = _neonManager.Resolve(providerData.ProviderType) as IWebHookReceiver;

			oauthReceiver?.OnHook(payload);

			return Ok();

		}
	}
}
