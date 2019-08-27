using Neon.Api.Utils;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Broadlink.NET
{
	/// <summary>
	/// Generic Broadlink device
	/// </summary>
	public class BroadlinkDevice
	{
		/// <summary>
		/// <see cref="IPEndPoint"/> of local network adapter used to send data and receive responses
		/// </summary>
		public IPEndPoint LocalIPEndPoint { get; set; }

		/// <summary>
		/// <see cref="IPEndPoint"/> of this Broadlink device
		/// </summary>
		public IPEndPoint EndPoint { get; set; }

		/// <summary>
		/// <see cref="short"/> mapping to a Broadlink device type
		/// </summary>
		public short DeviceType { get; set; }

		/// <summary>
		/// MAC address of this device
		/// </summary>
		public byte[] MacAddress { get; set; }

		/// <summary>
		/// Boolean indicating whether <see cref="AuthorizeAsync()"/> completed successfully
		/// </summary>
		/// <remarks>This should be true before other calls can be made</remarks>
		public bool IsAuthorized { get { return DeviceId != null && EncryptionKey != null; } }

		/// <summary>
		/// The Broadlink protocol uses a counter for the amount of packets that are sent
		/// </summary>
		public short PacketCount { get; private set; } = 1;

		/// <summary>
		/// This is sent with every call; for internal usage
		/// </summary>
		/// <remarks>Obtained by the <see cref="AuthorizeAsync()" /> method</remarks>
		public byte[] DeviceId { get; private set; }

		/// <summary>
		/// This is sent with every call; for internal usage
		/// </summary>
		/// <remarks>Obtained by the <see cref="AuthorizeAsync()" /> method</remarks>
		public byte[] EncryptionKey { get; private set; }

		/// <summary>
		/// Obtain the device id and encryption key needed for other calls
		/// </summary>
		/// <remarks>Run this method before making any other calls</remarks>
		/// <returns></returns>
		public async Task AuthorizeAsync()
		{
			byte[] authorizationPacket = PacketGenerator.GenerateAuthorizationPacket(this);

			var encryptedResponse = await SendAndWaitForResponseAsync(authorizationPacket);

			var encryptedPayload = encryptedResponse.Slice(0x38);
			var payload = encryptedPayload.Decrypt();

			var deviceId = new byte[4];
			Array.Copy(payload, 0x00, deviceId, 0, deviceId.Length);

			var encryptionKey = new byte[16];
			Array.Copy(payload, 0x04, encryptionKey, 0, encryptionKey.Length);

			DeviceId = deviceId;
			EncryptionKey = encryptionKey;
		}

		/// <summary>
		/// Send a packet and wait for the response
		/// </summary>
		/// <param name="packet">Packet to send</param>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected async Task<byte[]> SendAndWaitForResponseAsync(byte[] packet)
		{
			byte[] response = null;
			using (var client = new UdpClient(LocalIPEndPoint))
			{
				_ = Task.Run(async () =>
				{
					while (true)
					{
						var result = await client.ReceiveAsync();
						if (result != null)
						{
							response = result.Buffer;
						}
						return;
					}
				});

				await client.SendAsync(packet, packet.Length, EndPoint);
				PacketCount++;
				Debug.WriteLine("Message sent");

				await Task.Delay(3000);

				return response;
			}
		}

		protected async Task SendAsync(byte[] packet)
		{
			using (var client = new UdpClient(LocalIPEndPoint))
			{
				await client.SendAsync(packet, packet.Length, EndPoint);
				PacketCount++;
				Debug.WriteLine("Message sent");

				return;
			}
		}

	}
}
