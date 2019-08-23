using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.OAuth;
using Neon.Api.Data.OAuth;
using Neon.Api.Data.Scheduler;
using Neon.Api.Data.UserInteraction;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Oauth;
using Neon.Api.Interfaces.Services;
using Neon.Api.Utils;
using Neon.Engine.Components.Configs.Music;
using Neon.Engine.Components.Events;
using Neon.Engine.Vaults;
using SpotifyAPI.Web;

namespace Neon.Engine.Components.Music
{
	[OAuthReceiver("spotify")]
	[NeonComponent("spotify", "v1.0.0.0", "MUSIC", typeof(SpotifyConfig))]
	public class SpotifyComponent : AbstractNeonComponent<SpotifyConfig>, IOAuthReceiver
	{
		private const string TokenAuthUrl = "https://accounts.spotify.com/api/token";
		private const string RedirectUrl = "https://localhost:5001/spotify/auth";
		private const string AuthorizeUrl = "https://accounts.spotify.com/authorize";

		private readonly IUserInteractionService _userInteractionService;
		private readonly ISchedulerService _schedulerService;
		private readonly HttpClient _httpClient;

		private SpotifyWebAPI _spotifyWebApi = null;

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
			ISchedulerService schedulerService,
			IUserInteractionService userInteractionService,
			IComponentsService componentsService) : base(loggerFactory, ioTService, componentsService)
		{

			_schedulerService = schedulerService;
			_userInteractionService = userInteractionService;
			_httpClient = httpClientFactory.CreateClient();

		}

		public override Task<bool> Start()
		{
			if (Config.ClientSecret.ClientSecret == "change_me" && Config.ClientSecret.ClientId == "change_me")
				throw ThrowComponentNeedConfiguration("ClientId", "ClientSecret");

			if (string.IsNullOrEmpty(_spotifyVault.AccessToken.AccessToken) && string.IsNullOrEmpty(_spotifyVault.AccessToken.RefreshToken))
			{
				BuildUserTokenRequest();
			}
			else if (_spotifyVault.AccessToken.ExpireOn < DateTime.Now)
			{
				Logger.LogInformation($"Token expired");
				BuildUserTokenRequest();
			}
			else
			{
				InitSpotifyClient();
				RefreshTokenJob();
			}

			return Task.FromResult(true);
		}

		private void RefreshTokenJob()
		{
			Logger.LogInformation($"Adding refresh token job");
			_schedulerService.AddJob(async () => await RefreshToken(), "SpotifyRefreshToken", (int)TimeSpan.FromMinutes(30).TotalSeconds, false);

		}

		private async Task GetDevices()
		{
			var devices = await _spotifyWebApi.GetDevicesAsync();

			if (!devices.HasError())
				devices.Devices.ForEach(device =>
				{
					var deviceEntity = BuildEntity<SpotifyDeviceEvent>();


					deviceEntity.DeviceId = device.Id;
					deviceEntity.VolumePercent = device.VolumePercent;
					deviceEntity.Name = device.Name;
					deviceEntity.DeviceType = device.Type;
					deviceEntity.IsActive = device.IsActive;
					deviceEntity.IsRestricted = device.IsRestricted;


					PublishEntity(deviceEntity);
				});
			else
				Logger.LogError($"Error during Get Devices: {devices.Error.Status} - {devices.Error.Message}");
		}

		private async Task GetCurrentPlayback()
		{
			var currentPlayback = await _spotifyWebApi.GetPlaybackAsync();

			if (!currentPlayback.HasError())
			{
				if (currentPlayback.Item != null)
				{
					var entity = BuildEntity<SpotifyCurrentTrackEvent>();

					entity.ArtistName = currentPlayback.Item.Artists[0].Name;
					entity.SongName = currentPlayback.Item.Name;
					entity.Uri = currentPlayback.Item.Href;
					entity.IsPlaying = currentPlayback.IsPlaying;

					PublishEntity(entity);
				}
			}
			else
			{
				Logger.LogError($"Error during Get Current Playback: {currentPlayback.Error.Status} - {currentPlayback.Error.Message}");
			}
		}

		public override async Task Poll()
		{
			await GetDevices();
			await GetCurrentPlayback();
			await base.Poll();
		}

		private async Task RefreshToken()
		{
			Logger.LogInformation("Refresh token");

			var res = await _httpClient.PostAsync(TokenAuthUrl,
				HttpClientUtils.BuildFormParams(
					new KeyValuePair<string, string>("grant_type", "refresh_token"),
					new KeyValuePair<string, string>("refresh_token", _spotifyVault.AccessToken.RefreshToken),
					new KeyValuePair<string, string>("client_id", Config.ClientSecret.ClientId),
					new KeyValuePair<string, string>("client_secret", Config.ClientSecret.ClientSecret)));
			var st = await res.Content.ReadAsStringAsync();

			var newToken = st.FromJson<OAuthTokenResult>();
			_spotifyVault.AccessToken.AccessToken = newToken.AccessToken;
			_spotifyVault.AccessToken.ExpireOn = DateTime.Now.AddSeconds(newToken.ExpiresIn);
			SaveVault(_spotifyVault);

			Logger.LogInformation($"Token refresh expire on: {_spotifyVault.AccessToken.ExpireOn}");

			InitSpotifyClient();
		}

		private void InitSpotifyClient()
		{
			if (_spotifyWebApi != null)
				lock (_spotifyWebApi)
				{
					_spotifyWebApi?.Dispose();
				}

			_spotifyWebApi = new SpotifyWebAPI { TokenType = _spotifyVault.AccessToken.TokenType, AccessToken = _spotifyVault.AccessToken.AccessToken };
		}

		private void BuildUserTokenRequest()
		{
			_userInteractionService.AddUserInteractionData(new UserInteractionData
			{
				Name = "SPOTIFY",
				Fields = new List<UserInteractionField>
				{
					new UserInteractionField().Build()
						.SetFieldName("AUTH_URL")
						.SetFieldValue(GenerateOAuthToken(_spotifyScopes))
						.SetFieldType(UserInteractionFieldTypeEnum.Link)
						.SetDescription("Click on link for authorize spotify API").SetIsRequired(true)
				}
			}, data => { });

			Logger.LogInformation($"Auth url is {GenerateOAuthToken(_spotifyScopes)}");
		}
		public override Task<bool> Init(object config)
		{
			_spotifyVault = LoadVault<SpotifyVault>();
			return base.Init(config);
		}

		private string GenerateOAuthToken(params string[] scopes)
		{
			return
				$"{AuthorizeUrl}?response_type=code&redirect_uri={RedirectUrl}&client_id={Config.ClientSecret.ClientId}&scope={string.Join("%20", scopes)}&state=k332yl";
		}

		public async void OnOAuthReceived(string provider, OAuthResult result)
		{
			if (!string.IsNullOrEmpty(result.Code))
			{
				var res = await _httpClient.PostAsync(TokenAuthUrl,
					HttpClientUtils.BuildFormParams(
						new KeyValuePair<string, string>("grant_type", "authorization_code"),
						new KeyValuePair<string, string>("code", result.Code),
						new KeyValuePair<string, string>("client_id", Config.ClientSecret.ClientId),
						new KeyValuePair<string, string>("client_secret", Config.ClientSecret.ClientSecret),
						new KeyValuePair<string, string>("redirect_uri", RedirectUrl)
					));


				if (res.StatusCode == HttpStatusCode.OK)
				{
					var tokenString = await res.Content.ReadAsStringAsync();
					var tokenInfo = tokenString.FromJson<OAuthTokenResult>();
					Logger.LogInformation(
						$"Spotify authentication OK, token expire {DateTime.Now.AddSeconds(tokenInfo.ExpiresIn).ToString()}");
					_spotifyVault.AccessToken.AccessToken = tokenInfo.AccessToken;
					_spotifyVault.AccessToken.TokenType = tokenInfo.TokenType;
					_spotifyVault.AccessToken.ExpireOn = DateTime.Now.AddSeconds(tokenInfo.ExpiresIn).ToLocalTime();
					_spotifyVault.AccessToken.RefreshToken = tokenInfo.RefreshToken;

					SaveVault(_spotifyVault);

					//	RefreshTokenJob();
					await Start();
				}
			}
		}
	}
}
