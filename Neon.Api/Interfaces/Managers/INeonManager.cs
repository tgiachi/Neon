﻿using Autofac;
using Neon.Api.Data.Config.Root;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Managers
{
	/// <summary>
	/// Interface for create Neon Manager
	/// </summary>
	public interface INeonManager
	{
		ContainerBuilder ContainerBuilder { get; }

		List<Type> AvailableServices { get; }

		NeonConfig Config { get; }

		bool IsRunningInDocker { get; }

		/// <summary>
		/// Pre load service for startup
		/// </summary>
		/// <returns></returns>
		bool Init();


		/// <summary>
		/// Start Neon service
		/// </summary>
		/// <returns></returns>
		Task Start();


		/// <summary>
		/// Build container
		/// </summary>
		/// <returns></returns>
		IContainer Build();

		/// <summary>
		/// Shutdown Neon
		/// </summary>
		/// <returns></returns>
		Task Shutdown();


		/// <summary>
		/// Resolve from container passing generic type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Resolve<T>();


		/// <summary>
		/// Resolve from container passing generic type
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		object Resolve(Type t);


	}
}
