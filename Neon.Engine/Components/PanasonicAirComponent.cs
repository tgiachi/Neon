using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Data.Scheduler;
using Neon.Api.Impl.Components;
using Neon.Api.Interfaces.Services;
using Neon.Engine.Components.AirCo;
using Neon.Engine.Components.AirCo.Model;
using Neon.Engine.Components.AirCo.Model.Response;
using Neon.Engine.Components.Configs.AirConditioned;
using Neon.Engine.Components.Events;
using Neon.Engine.Vaults;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Neon.Engine.Components
{
	[NeonComponent("panasonic_air", "v1.0.0.0", "AIR_CONDITIONED", typeof(PanasonicAirConfig))]
	public class PanasonicAirComponent : AbstractNeonComponent<PanasonicAirConfig>
	{
		private readonly AircoManager _aircoManager;
		private bool _isConfigured = false;

		private PanasonicAirVault _panasonicAirVault;
		public PanasonicAirComponent(ILoggerFactory loggerFactory, IIoTService ioTService, IComponentsService componentsService, IHttpClientFactory httpClientFactory) : base(loggerFactory, ioTService, componentsService)
		{
			_aircoManager = new AircoManager(httpClientFactory.CreateClient());
		}


		public override Task<bool> Init(object config)
		{
			_panasonicAirVault = LoadVault<PanasonicAirVault>();
			return base.Init(config);
		}

		public override async Task<bool> Start()
		{
			if (Config.Username == "change_me" && Config.Password == "change_me")
			{
				ThrowComponentNeedConfiguration("Username", "Password");
			}

			var result = await Login();

			if (!result)
			{
				Logger.LogError($"Can't login to Panasonic Ait Conditioned");
			}

			return await base.Start();
		}

		[ComponentPollRate((int)SchedulerServicePollingEnum.HalfNormalPolling)]
		public override async Task Poll()
		{
			if (_isConfigured)
			{
				await GetDevices();
			}
			await base.Poll();
		}

		private async Task GetDevices()
		{
			try
			{
				var groups = await _aircoManager.GetDeviceGroups();

				groups.GroupList.ForEach(g =>
				{
					g.DeviceIdList.ForEach(d =>
					{
						var entity = BuildEntity<PanasonicAirEvent>();
						entity.Name = d.DeviceName;
						entity.AirCondGroup = g.GroupName;
						entity.DeviceType = d.DeviceType;
						entity.DeviceGuid = d.DeviceGuid;
						entity.DeviceMode = GetDeviceMode(d);
						entity.IsOn = d.Parameters.Operate == OperateType.On;
						entity.Temperature = d.Parameters.TemperatureSet;
						entity.FanSpeed = d.Parameters.FanSpeed;
						entity.AirSwingLR = d.Parameters.AirSwingLR;
						entity.AirSwingUD = d.Parameters.AirSwingUD;

						PublishEntity(entity);
					});
				});

			}
			catch (Exception ex)
			{
				if (ex.Message == "Got HTTP Unauthorized: {\"message\":\"Token expires\",\"code\":4100}")
				{
					Logger.LogWarning($"Panasonic Token expired, re login");
					_panasonicAirVault.AccessToken = "";
					await Login();
				}
				else
				{
					Logger.LogError($"Error during get Device groups {ex.Message}");

				}
			}
		}

		private PanasonicAirModeEnum GetDeviceMode(DeviceId device)
		{
			if (device.AutoMode)
				return PanasonicAirModeEnum.Auto;

			if (device.CoolMode)
				return PanasonicAirModeEnum.Cool;

			if (device.DryMode)
				return PanasonicAirModeEnum.Dry;

			if (device.HeatMode)
				return PanasonicAirModeEnum.Heat;

			if (device.FanMode)
				return PanasonicAirModeEnum.Fan;

			return PanasonicAirModeEnum.None;
		}

		private async Task<bool> Login()
		{

			try
			{
				if (string.IsNullOrEmpty(_panasonicAirVault.AccessToken))
				{
					var result = await _aircoManager.Login("0", Config.Username, Config.Password);

					_panasonicAirVault.AccessToken = result.UToken;
					SaveVault(_panasonicAirVault);
					_isConfigured = true;
				}
				else
				{
					_aircoManager.SetAuthorizationToken(_panasonicAirVault.AccessToken);
					_isConfigured = true;
				}

				return true;
			}
			catch (Exception e)
			{
				Logger.LogError($"Error during login {e.Message}");

				return false;
			}
		}
	}
}
