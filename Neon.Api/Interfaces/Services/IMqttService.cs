using Neon.Api.Data.Mqtt;
using Neon.Api.Interfaces.Base;
using System;
using System.Threading.Tasks;

namespace Neon.Api.Interfaces.Services
{
	public interface IMqttService : INeonService
	{
		IObservable<MqttMessage> MqttMessageObservable { get; }

		Task<bool> SubscribeTopic(string topicName);

		Task<bool> SendMessage(MqttMessage message);
	}
}
