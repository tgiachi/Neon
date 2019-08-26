using Neon.Api.Data.UserInteraction;
using Neon.Api.Interfaces.Base;
using System;
using System.Collections.Generic;

namespace Neon.Api.Interfaces.Services
{
	public interface IUserInteractionService : INeonService
	{
		List<UserInteractionData> NeedUserInteractionData { get; }
		void AddUserInteractionData(UserInteractionData data, Action<UserInteractionData> onConfigAdd);

		void CompileEntry(string name, string field, object value);
	}
}
