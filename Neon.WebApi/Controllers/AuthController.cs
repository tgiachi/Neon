﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Neon.Api.Data.OAuth;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Oauth;
using System.Collections.Generic;
using System.Linq;

namespace Neon.WebApi.Controllers
{
	[ApiController]
	[Route("api/oauth")]
	public class AuthController : ControllerBase
	{
		private readonly ILogger _logger;
		private readonly List<OAuthReceiverData> _oAuthReceiverData;
		private readonly INeonManager _neonManager;
		public AuthController(ILogger<AuthController> logger, List<OAuthReceiverData> oAuthReceiverData, INeonManager neonManager)
		{
			_logger = logger;
			_oAuthReceiverData = oAuthReceiverData;
			_neonManager = neonManager;
		}

		[HttpGet]
		[HttpPost]
		[Route("{provider}/auth")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
		[ProducesDefaultResponseType]
		public ActionResult OAuth(string provider)
		{
			var providerData = _oAuthReceiverData.FirstOrDefault(r => r.ProviderName == provider);

			if (providerData != null)
			{
				var oAuthResult = new OAuthResult();
				oAuthResult.RequestUrl = HttpContext.Request.Path + HttpContext.Request.QueryString;
				var oauthReceiver = _neonManager.Resolve(providerData.ProviderType) as IOAuthReceiver;


				if (!string.IsNullOrEmpty(HttpContext.Request.Query["code"]))
					oAuthResult.Code = HttpContext.Request.Query["code"];

				if (!string.IsNullOrEmpty(HttpContext.Request.Query["token"]))
					oAuthResult.Token = HttpContext.Request.Query["token"];

				if (!string.IsNullOrEmpty(HttpContext.Request.Query["status"]))
					oAuthResult.Status = HttpContext.Request.Query["status"];

				oauthReceiver?.OnOAuthReceived(providerData.ProviderName, oAuthResult);
				return Ok("ok");
			}

			return NotFound("oAuth Receiver not found!");
		}
	}
}
