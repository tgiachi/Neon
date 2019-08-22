using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Neon.Api.Attributes.Services;
using Neon.Api.Interfaces.Services;
using System.Threading.Tasks;
using Neon.Api.Data.UserInteraction;

namespace Neon.Engine.Services
{
	[NeonService("User interaction service", "Manager user interaction")]
	public class UserInteractionService : IUserInteractionService
	{
		private readonly Dictionary<string, Action<UserInteractionData>> _configNotifiers =
			new Dictionary<string, Action<UserInteractionData>>();

		public ObservableCollection<UserInteractionData> UserInteractionData { get; set; }

		public List<UserInteractionData> NeedUserInteractionData => UserInteractionData.ToList();

		public UserInteractionService()
		{
			UserInteractionData = new ObservableCollection<UserInteractionData>();
		}

		
		public Task<bool> Start()
		{
			return Task.FromResult(true);
		}

		public Task<bool> Stop()
		{
			return Task.FromResult(true);
		}

		public void AddUserInteractionData(UserInteractionData data, Action<UserInteractionData> onConfigAdd)
		{
			_configNotifiers.Add(data.Name, onConfigAdd);
			UserInteractionData.Add(data);
		}

		public void CompileEntry(string name, string field, object value)
		{
			var ui = UserInteractionData.ToList().FirstOrDefault(s => s.Name == name);

			var uiField = ui.Fields.FirstOrDefault(f => f.FieldName == field);

			uiField.FieldValue = value;

			_configNotifiers[name](ui);
		}
	}
}
