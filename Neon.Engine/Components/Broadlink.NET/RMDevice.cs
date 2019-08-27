using Neon.Api.Utils;
using System;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Broadlink.NET
{
	/// <summary>
	/// Represents a remote control (IR/RF) device
	/// </summary>
	public class RMDevice : BroadlinkDevice
	{
		/// <summary>
		/// Start the remote control command learning mode.
		/// </summary>
		/// <returns></returns>
		public async Task EnterLearningModeAsync()
		{
			var packet = PacketGenerator.GenerateStartLearningModePacket(this);
			await SendAsync(packet);
		}

		/// <summary>
		/// Read the data for a remote control command.
		/// </summary>
		/// <returns>Byte array containing the packet for <see cref="SendRemoteCommandAsync(byte[])" /></returns>
		public async Task<byte[]> ReadLearningDataAsync()
		{
			var packet = PacketGenerator.GenerateReadLearningModePacket(this);
			var encryptedResponse = await SendAndWaitForResponseAsync(packet);

			var errorCode = BitConverter.ToInt16(encryptedResponse, 0x22);
			if (errorCode != 0)
			{
				throw new Exception($"Error {errorCode} in learning response");
			}

			var encryptedPayload = encryptedResponse.Slice(0x38);

			var payload = encryptedPayload.Decrypt(EncryptionKey);
			var learningData = payload.Slice(0x04);
			return learningData;
		}

		/// <summary>
		/// Execute a remote control command
		/// </summary>
		/// <param name="data">Packet obtained using <see cref="ReadLearningDataAsync()" /></param>
		/// <returns></returns>
		public async Task SendRemoteCommandAsync(byte[] data)
		{
			var packet = PacketGenerator.GenerateSendDataPacket(this, data);
			await SendAsync(packet);
		}

		/// <summary>
		/// Get the temperature
		/// </summary>
		/// <returns>temperature in degrees Celsius</returns>
		public async Task<float> GetTemperatureAsync()
		{
			var packet = PacketGenerator.GenerateReadTemperaturePacket(this);
			var encryptedResponse = await SendAndWaitForResponseAsync(packet);

			var errorCode = BitConverter.ToInt16(encryptedResponse, 0x22);
			if (errorCode != 0)
			{
				throw new Exception($"Error {errorCode} in temperature response");
			}

			var encryptedPayload = encryptedResponse.Slice(0x38);

			var payload = encryptedPayload.Decrypt(EncryptionKey);
			var temperatureData = payload.Slice(0x04, 0x05);
			var temperature = temperatureData[0] + (float)temperatureData[1] / 10;
			return temperature;
		}
	}
}
