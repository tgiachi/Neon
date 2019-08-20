using Neon.Api.Interfaces.Base;
using System;
using System.Collections.Generic;

namespace Neon.Api.Interfaces.Services
{
	/// <summary>
	/// Service for create/save routines
	/// </summary>
	public interface IRoutineService : INeonService
	{
		/// <summary>
		/// Add routine
		/// </summary>
		/// <param name="name"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		bool AddRoutine(string name, Action action);

		/// <summary>
		/// Execute a routine
		/// </summary>
		/// <param name="name"></param>
		void ExecuteRoutine(string name);


		/// <summary>
		/// Get routine names
		/// </summary>
		List<string> RoutineNames { get; }
	}
}
