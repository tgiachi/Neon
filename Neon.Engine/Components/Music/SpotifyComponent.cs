using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs.Music;
using Neon.Engine.Vaults;
using SpotifyAPI.Web;

namespace Neon.Engine.Components.Music
{
	[NeonComponent("spotify", "v1.0.0.0", "MUSIC", typeof(SpotifyConfig))]
	public class SpotifyComponent : AbstractNeonComponent<SpotifyConfig>
	{
		private const string TokenAuthUrl = "https://accounts.spotify.com/api/token";
		private const string RedirectUrl = "https://localhost:5001/api/oauth/Authorize/spotify";
		private const string AuthorizeUrl = "https://accounts.spotify.com/authorize";

		private SpotifyWebAPI _spotifyWebApi;

		private readonly string[] _spotifyScopes =
		{
			"user-read-playback-state",
			"user-modify-playback-state",
			"playlist-read-private",
			"user-read-private",
			"user-read-email"
		};

		private SpotifyVault _spotifyVault;
		

		public SpotifyComponent(ILoggerFactory loggerFactory,
			IIoTService ioTService,
			IHttpClientFactory httpClientFactory,
			IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

			

		}

		public override Task<bool> Init(object config)
		{
			_spotifyVault = LoadVault<SpotifyVault>();
			return base.Init(config);
		}
	}
}
