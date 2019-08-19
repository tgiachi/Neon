using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Neon.Api.Data.Mqtt;
using Neon.Api.Interfaces.Base;

namespace Neon.Api.Interfaces.Services
{
	public interface IMqttService : INeonService
	{
		IObservable<MqttMessage> MqttMessageObservable { get; }

		Task<bool> SubscribeTopic(string topicName);

		Task<bool> SendMessage(MqttMessage message);
	}
}
