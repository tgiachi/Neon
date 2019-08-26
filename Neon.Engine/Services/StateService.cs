using Microsoft.Extensions.Logging;
using Neon.Api.Attributes.Services;
using Neon.Api.Data.States;
using Neon.Api.Interfaces.Managers;
using Neon.Api.Interfaces.Services;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Neon.Engine.Services
{

	[NeonService("State service", "Manage states")]
	public class StateService : IStateService
	{
		private readonly ILogger _logger;
		private readonly INeonManager _neonManager;
		private readonly IFileSystemManager _fileSystemManager;
		private readonly string _stateFilename = Path.Combine("States", "states.yaml");
		private readonly object _stateWriteLock = new object();
		private StateData _states;

		public StateService(ILogger<StateService> logger, INeonManager neonManager, IFileSystemManager fileSystemManager)
		{
			_logger = logger;
			_neonManager = neonManager;
			_fileSystemManager = fileSystemManager;
		}

		public Task<bool> Start()
		{
			_fileSystemManager.CreateDirectory("States");

			LoadStates();

			if (_states == null)
			{
				_states = new StateData();

				SaveStates();
			}
			return Task.FromResult(true);
		}

		private void SaveStates()
		{
			lock (_stateWriteLock)
			{
				_fileSystemManager.WriteToFile(_stateFilename, _states);
			}
		}

		private void LoadStates()
		{
			lock (_stateWriteLock)
			{
				_states = _fileSystemManager.ReadFromFile<StateData>(_stateFilename);
			}
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public void SetState(string name, object value)
		{
			lock (_states)
			{
				var entry = _states.Values.FirstOrDefault(e => e.StateName == name);

				if (entry == null)
				{
					entry = new StateEntryData() { StateName = name, StateValue = value };
					_states.Values.Add(entry);
				}
				else
				{
					entry.StateValue = value;
				}
			}

			SaveStates();
		}

		public object GetState(string name)
		{
			lock (_states)
			{
				var entry = _states.Values.FirstOrDefault(e => e.StateName == name);

				return entry?.StateValue;

			}

		}

		public bool IncrementState(string name, int count = 1)
		{
			lock (_states)
			{
				var entry = _states.Values.FirstOrDefault(e => e.StateName == name);

				if (entry != null)
				{
					if (entry.StateValue is int)
					{
						entry.StateValue = ((int)entry.StateValue) + count;
					}
					else
					{
						var isInt = int.TryParse((string)entry.StateValue, out var outInt);

						if (isInt)
						{
							outInt += count;
							entry.StateValue = outInt;
						}
					}

					_states.Values.RemoveAll(data => data.StateName == name);
					_states.Values.Add(entry);


					SaveStates();

					return true;
				}
				else
				{
					entry = new StateEntryData()
					{
						StateName = name,
						StateValue = count
					};

					_states.Values.Add(entry);
				}

				SaveStates();
			}

			return false;
		}

		public bool DecrementState(string name, int count = 1)
		{
			lock (_states)
			{
				var entry = _states.Values.FirstOrDefault(e => e.StateName == name);

				if (entry != null)
				{
					if (entry.StateValue is int)
					{
						entry.StateValue = ((int)entry.StateValue) - count;
					}
					else
					{
						var isInt = int.TryParse((string)entry.StateValue, out var outInt);

						if (isInt)
						{
							outInt -= count;
							entry.StateValue = outInt;
						}
					}

					_states.Values.RemoveAll(data => data.StateName == name);
					_states.Values.Add(entry);

					SaveStates();

					return true;
				}
				else
				{
					entry = new StateEntryData()
					{
						StateName = name,
						StateValue = count
					};

					_states.Values.Add(entry);
				}

				SaveStates();
			}

			return false;
		}

		public bool SetBooleanState(string name, bool state)
		{
			SetState(name, state);
			return state;
		}

		public bool GetBooleanState(string name)
		{
			if (GetState(name) == null)
				return false;

			if (GetState(name) is string)
				return bool.Parse((string)GetState(name));

			return (bool)GetState(name);

		}
	}
}
