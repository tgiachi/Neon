using Microsoft.Extensions.Logging;
using Neon.Api.Attributes;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.OAuth;
using Neon.Api.Data.OAuth;
using Neon.Api.Data.Scheduler;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Oauth;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.Configs;
using Neon.Engine.Components.Events;
using Neon.Engine.Vaults;
using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Neon.Engine.Components.Lights
{
	[OAuthReceiver("philip_hue")]
	[NeonComponent("philip_hue", "v1.0.0.0", "LIGHTS", typeof(PhilipHueConfig))]
	public class PhilipHueComponent : AbstractNeonComponent<PhilipHueConfig>, IOAuthReceiver
	{
		private IRemoteAuthenticationClient _remoteAuthentication;
		private PhilipHueVault _philipHueVault;
		private ILocalHueClient _localHueClient;
		private IRemoteHueClient _remoteHueClient;
		private bool _linkedButtonPressed = false;
		private bool _isPhilipConfigured = false;
		public PhilipHueComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService)
			: base(loggerFactory, ioTService, componentsService)
		{

		}

		public async void OnOAuthReceived(string provider, OAuthResult result)
		{
			if (!string.IsNullOrEmpty(result.Code))
			{
				var response = _remoteAuthentication.ProcessAuthorizeResponse(result.RequestUrl);

				var token = await _remoteAuthentication.GetToken(response.Code);

				var vault = LoadVault<PhilipHueVault>();
				vault.AccessToken = token;

				SaveVault(vault);

				InitToken();

			}
		}

		[ComponentPollRate((int)SchedulerServicePollingEnum.NormalPolling)]
		public override async Task Poll()
		{
			if (_isPhilipConfigured)
			{
				if (!Config.UseRemote)
					await PollLocal();
				else
					await PollRemote();

			}
		}

		private async Task PollLocal()
		{
			if (_localHueClient != null)
			{
				var lights = await _localHueClient.GetLightsAsync();
				var groups = await _localHueClient.GetGroupsAsync();


				if (groups != null)
				{
					Logger.LogDebug($"Found {groups.Count} light groups");

					groups.ToList().ForEach(group =>
					{
						var groupEntity = BuildEntity<PhilipHueGroupEvent>();

						groupEntity.Name = group.Name;
						groupEntity.LightsCount = group.Lights.Count;
						groupEntity.AllOn = group.State.AllOn ?? false;
						groupEntity.AnyOn = group.State.AnyOn ?? false;

						groupEntity.Model = group.ModelId;

						PublishEntity(groupEntity);
					});
				}

				if (lights != null)
				{
					Logger.LogDebug($"Found {lights.Count()} lights");

					lights.ToList().ForEach(light =>
					{
						var lightEntity = BuildEntity<PhilipHueLightEvent>();
						lightEntity.LightId = light.Id;
						lightEntity.Name = light.Name;
						lightEntity.IsOn = light.State.On;
						lightEntity.ColorMode = light.State.ColorMode;
						lightEntity.LightType = light.Type;
						lightEntity.ColorCoordinates = light.State.ColorCoordinates;
						lightEntity.LightGroup = GetGroupName(light.Id, groups.ToList());

						PublishEntity(lightEntity);
					});
				}
			}
		}

		private string GetGroupName(string lightId, List<Group> groups)
		{
			var res = groups.Where(s => s.Lights.Any(l => l == lightId)).ToList();
			if (res.Count > 0)
				return res[0].Name;
			else return "";
		}

		private Task PollRemote()
		{
			return Task.CompletedTask;

		}

		public override async Task<bool> Start()
		{
			if (Config.UseRemote)
				StartRemote();
			else
				await StartLocal();

			return true;
		}


		public override Task<bool> Init(object config)
		{
			_philipHueVault = LoadVault<PhilipHueVault>();
			return base.Init(config);
		}


		private async Task StartLocal()
		{
			if (!string.IsNullOrEmpty(Config.LocalConfig.BridgeIpAddress))
			{
				_localHueClient = new LocalHueClient(Config.LocalConfig.BridgeIpAddress, _philipHueVault.LocalApiKey);
				_isPhilipConfigured = true;
				await PollLocal();
			}
			else
			{
				var locator = new HttpBridgeLocator();
				var ips = await locator.LocateBridgesAsync(TimeSpan.FromSeconds(6));

				if (ips.Any())
				{
					var bridge = ips.ToList().FirstOrDefault();
					Config.LocalConfig.BridgeIpAddress = bridge.IpAddress;

					if (!string.IsNullOrEmpty(_philipHueVault.LocalApiKey))
					{
						_localHueClient = new LocalHueClient(bridge.IpAddress, _philipHueVault.LocalApiKey);
						Logger.LogInformation($"Philip Hue component configured");
					}
					else
					{
						Logger.LogInformation("Starting Button pressed watcher");

						_ = Task.Factory.StartNew(async () => await WatchButtonPressed(bridge.IpAddress));
					}
				}
				else
				{
					Logger.LogWarning($"No PhilipHue bridge found!");
				}
			}

		}

		private async Task WatchButtonPressed(string bridgeIp)
		{
			_localHueClient = new LocalHueClient(bridgeIp);

			while (!_linkedButtonPressed)
			{
				try
				{
					await Task.Delay(10000);

					var key = await _localHueClient.RegisterAsync("Neon", "Neon");

					Logger.LogInformation($"Button pressed");
					_philipHueVault.LocalApiKey = key;

					SaveVault(_philipHueVault);

					_linkedButtonPressed = true;

					Config.LocalConfig.BridgeIpAddress = bridgeIp;
					SaveConfig();

					_localHueClient = new LocalHueClient(bridgeIp, _philipHueVault.LocalApiKey);
					_isPhilipConfigured = true;

				}
				catch
				{

				}

			}
		}

		private void InitToken()
		{
			if (_philipHueVault.AccessToken != null)
			{
				if (!string.IsNullOrEmpty(_philipHueVault.AccessToken.Access_token))
				{
					_remoteHueClient = new RemoteHueClient(async () =>
					{
						if (_remoteAuthentication != null)
							return await _remoteAuthentication.GetValidToken();
						return _philipHueVault.AccessToken.Access_token;

					});


				}
			}
		}

		private void StartRemote()
		{
			_remoteAuthentication = new RemoteAuthenticationClient(Config.RemoteConfig.ClientId,
				Config.RemoteConfig.ClientSecret, Config.RemoteConfig.AppId);

			var authUrl = _remoteAuthentication.BuildAuthorizeUri("neon_state", "Neon");

			Logger.LogInformation($"URL: {authUrl}");
		}

		[ComponentCommand("light_status", "Turn off/on light")]
		public async Task<bool> SendCommandLight(string lightId, bool status)
		{
			var hueResult = await _localHueClient.SendCommandAsync(new LightCommand()
			{
				On = status
			}, new List<string>() { lightId });

			return true;
		}

		[ComponentCommand("group_status", "Turn off/on group")]
		public async Task<bool> SendCommandGroup(string groupName, bool status)
		{
			//_localHueClient.SendGroupCommandAsync(new })
			var lights = IoTService.GetEntitiesByType<PhilipHueLightEvent>();
			lights = lights.Where(p => p.GroupName == groupName).ToList();

			if (lights.Any())
			{
				var hueResult = await _localHueClient.SendCommandAsync(new LightCommand()
				{
					On = status
				}, lights.Select(p => p.LightId));

				return true;
			}

			return false;

		}
	}
}
