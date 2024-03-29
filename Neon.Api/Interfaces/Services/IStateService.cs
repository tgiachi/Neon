﻿using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Services
{
	public interface IStateService : INeonService
	{
		void SetState(string name, object value);

		object GetState(string name);

		bool IncrementState(string name, int count = 1);

		bool DecrementState(string name, int count = 1);

		bool SetBooleanState(string name, bool state);

		bool GetBooleanState(string name);
	}
}
