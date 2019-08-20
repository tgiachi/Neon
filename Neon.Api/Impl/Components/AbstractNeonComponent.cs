using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Neon.Api.Interfaces.Components;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;

namespace Neon.Api.Impl.Components
{
	public abstract class AbstractNeonComponent<TConfig> : INeonComponent  where TConfig : INeonComponentConfig, new()
	{
		protected ILogger Logger { get; }

		private readonly IIoTService _ioTService;
		private TConfig _config = new TConfig();

		protected AbstractNeonComponent(ILoggerFactory loggerFactory, IIoTService ioTService)
		{
			Logger = loggerFactory.CreateLogger(GetType());
			_ioTService = ioTService;
		}
		
		public Task<bool> Init(object config)
		{
			_config = (TConfig)config;

			return Task.FromResult(true);
		}

		protected async void PublishEntity<T>(T entity) where T : INeonIoTEntity
		{
			await _ioTService.PersistEntity(entity);
		}

		public virtual Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public virtual object GetDefaultConfig()
		{
			return _config;
		}

		public virtual Task Poll()
		{
			return Task.CompletedTask;
			
		}

		public virtual void Dispose()
		{
		}
	}
}
