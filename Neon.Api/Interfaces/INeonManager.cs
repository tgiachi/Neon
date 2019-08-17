using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Neon.Api.Interfaces
{
	/// <summary>
	/// Interface for create Neon Manager
	/// </summary>
	public interface INeonManager
	{
		ContainerBuilder ContainerBuilder { get; }

		/// <summary>
		/// Pre load service for startup
		/// </summary>
		/// <returns></returns>
		Task<bool> Init();


		/// <summary>
		/// Start Neon service
		/// </summary>
		/// <returns></returns>
		Task Start();




	}
}
