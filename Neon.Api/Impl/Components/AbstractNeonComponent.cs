﻿using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Components;
using Neon.Api.Attributes.Entities;
using Neon.Api.Data.Exceptions;
using Neon.Api.Interfaces.Components;
using Neon.Api.Interfaces.Entity;
using Neon.Api.Interfaces.Services;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Neon.Api.Impl.Components
{
	public abstract class AbstractNeonComponent<TConfig> : INeonComponent where TConfig : INeonComponentConfig, new()
	{
		protected ILogger Logger { get; }

		protected IIoTService IoTService { get; }
		protected TConfig Config { get; set; }

		protected AbstractNeonComponent(ILoggerFactory loggerFactory, IIoTService ioTService)
		{
			Logger = loggerFactory.CreateLogger(GetType());
			Config = new TConfig();
			IoTService = ioTService;

		}

		private NeonComponentAttribute GetComponentAttribute()
		{
			return this.GetType().GetCustomAttribute<NeonComponentAttribute>();
		}

		protected ComponentNeedConfigException ThrowComponentNeedConfiguration(params string[] proprieties)
		{
			var ex = new ComponentNeedConfigException()
			{
				ComponentName = GetComponentAttribute().Name
			};

			proprieties.ToList().ForEach(p =>
			{
				ex.ConfigKeys.Add(p);
			});

			return ex;

		}
		public Task<bool> Init(object config)
		{
			Config = (TConfig)config;

			return Task.FromResult(true);
		}

		protected async void PublishEntity<T>(T entity) where T : INeonIoTEntity
		{
			await IoTService.PersistEntity(entity);
		}

		public virtual Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public virtual object GetDefaultConfig()
		{
			return Config;
		}

		protected TEntity BuildEntity<TEntity>() where TEntity : INeonIoTEntity, new()
		{
			var attribute = typeof(TEntity).GetCustomAttribute<IoTEntityAttribute>();
			var entity = new TEntity
			{
				GroupName = attribute == null ? typeof(TEntity).Name.ToUpper() : attribute.Group,
				Name = attribute?.Name ?? typeof(TEntity).Name.ToUpper(),
				Id = Guid.NewGuid(),
				EventDateTime = DateTime.Now,
				EntityType = typeof(TEntity).FullName
			};

			if (string.IsNullOrEmpty(entity.Name))
				entity.Name = typeof(TEntity).Name.ToUpper();


			return entity;
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
