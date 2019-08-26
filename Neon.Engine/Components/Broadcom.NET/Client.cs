using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Neon.Engine.Components.Broadcom.NET
{
	public class Client
	{
		private IPEndPoint LocalIPEndPoint { get; set; }

		public Client()
		{
			LocalIPEndPoint = GetLocalIpEndpoint();
		}



		public async Task<List<BroadlinkDevice>> DiscoverAsync()
		{
			var discoveredDevices = new List<BroadlinkDevice>();

			byte[] discoveryPacket = PacketGenerator.GenerateDiscoveryPacket(LocalIPEndPoint.Address, (short)LocalIPEndPoint.Port);

			IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, 80);

			using (var client = new UdpClient(LocalIPEndPoint))
			{
				_ = Task.Run(async () =>
				 {
					 while (true)
					 {
						 var result = await client.ReceiveAsync();
						 if (result != null)
						 {
							 var response = result.Buffer;
							 if (response != null)
							 {
								 var macArray = new byte[7];
								 Array.Copy(response, 0x3a, macArray, 0, 7);

								 var discoveredDevice = CreateBroadlinkDevice(BitConverter.ToInt16(response, 0x34));
								 discoveredDevice.LocalIPEndPoint = LocalIPEndPoint;
								 discoveredDevice.EndPoint = result.RemoteEndPoint;
								 discoveredDevice.MacAddress = macArray;

								 discoveredDevices.Add(discoveredDevice);
							 }
						 }
					 }
				 });

				await client.SendAsync(discoveryPacket, discoveryPacket.Length, ep);
				Debug.WriteLine("Message sent to the broadcast address");

				await Task.Delay(3000);

				return discoveredDevices;
			}
		}

		private BroadlinkDevice CreateBroadlinkDevice(short deviceType)
		{
			BroadlinkDevice device;
			switch (deviceType)
			{
				case 0x2712: // RM2
				case 0x2737: // RM Mini
				case 0x273d: // RM Pro Phicomm
				case 0x2783: // RM2 Home Plus
				case 0x277c: // RM2 Home Plus GDT
				case 0x272a: // RM2 Pro Plus
				case 0x2787: // RM2 Pro Plus2
				case 0x278b: // RM2 Pro Plus BL
				case 0x278f: // RM Mini Shate
					device = new RMDevice();
					break;
				default:
					device = new BroadlinkDevice();
					break;
			}
			device.DeviceType = deviceType;
			return device;
		}

		private IPEndPoint GetLocalIpEndpoint()
		{
			using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
			{
				socket.Connect("8.8.8.8", 53);
				var localIpEndpoint = socket.LocalEndPoint as IPEndPoint;
				localIpEndpoint.Address = localIpEndpoint.Address.MapToIPv4();
				return localIpEndpoint;
			}
		}


	}
}
