using Neon.Api.Utils;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Neon.Engine.Components.Broadlink.NET
{
	public static class PacketGenerator
	{
		public static byte[] GenerateDiscoveryPacket(IPAddress localIp, short sourcePort)
		{
			var packet = new byte[48];

			var timezone = TimeZoneInfo.Local;
			var offsetFromGmt = timezone.BaseUtcOffset.Hours;

			var offsetFromGmtBytes = offsetFromGmt.ToLittleEndianBytes();
			offsetFromGmtBytes.CopyTo(packet, 0x08);

			var now = DateTime.Now;
			var yearBytes = ((short)now.Year).ToLittleEndianBytes();
			yearBytes.CopyTo(packet, 0x0c);

			packet[0x0e] = (byte)now.Minute;
			packet[0x0f] = (byte)now.Hour;
			packet[0x10] = (byte)int.Parse(now.Year.ToString().Substring(2));
			packet[0x11] = (byte)now.DayOfWeek;
			packet[0x12] = (byte)now.Day;
			packet[0x13] = (byte)now.Month;

			localIp.MapToIPv4().GetAddressBytes().CopyTo(packet, 0x18);
			sourcePort.ToLittleEndianBytes().CopyTo(packet, 0x1c);

			packet[0x26] = 6;

			GenerateChecksum(packet).CopyTo(packet, 0x20);

			return packet;
		}

		private static byte[] GenerateChecksum(byte[] packet)
		{
			var checksum = 0xbeaf;

			for (int i = 0; i < packet.Length; i++)
			{
				checksum += packet[i];
			}

			checksum &= 0xffff;

			return checksum.ToLittleEndianBytes();
		}

		public static byte[] GenerateAuthorizationPacket(BroadlinkDevice device)
		{
			var payload = GenerateAuthorizationPayload();

			var command = (short)0x0065;
			var packet = GenerateCommandPacket(command, payload, device);
			return packet;
		}

		private static byte[] GenerateAuthorizationPayload()
		{
			var payload = new byte[80];

			var properties = IPGlobalProperties.GetIPGlobalProperties();
			var hostname = properties.HostName;
			var hostnameBytes = Encoding.ASCII.GetBytes(hostname);

			// addresses 0x04-0x12
			var fifteenLength = Math.Min(hostnameBytes.Length, 15);
			Array.Copy(hostnameBytes, 0, payload, 0x04, fifteenLength);
			//var fifteen31 = new byte[] { 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31, 0x31 };
			//fifteen31.CopyTo(payload, 0x04);

			// TODO conflict between protocol documentation and python implementation
			payload[0x13] = 0x01;
			//payload[0x1e] = 0x01;

			payload[0x2d] = 0x01;

			// addresses 0x30-0x7f
			var seventynineLength = Math.Min(hostnameBytes.Length, 79);
			Array.Copy(hostnameBytes, 0, payload, 0x30, seventynineLength);
			//Encoding.ASCII.GetBytes("Test  1").CopyTo(payload, 0x30);

			return payload;
		}

		public static byte[] GenerateReadTemperaturePacket(RMDevice device)
		{
			var payload = new byte[16];
			payload[0x00] = 1;

			var command = (short)0x006a;

			var packet = GenerateCommandPacket(command, payload, device);
			return packet;
		}

		private static byte[] GenerateCommandPacket(short commandCode, byte[] payload, BroadlinkDevice device)
		{
			var header = new byte[56];

			header[0x00] = 0x5a;
			header[0x01] = 0xa5;
			header[0x02] = 0xaa;
			header[0x03] = 0x55;
			header[0x04] = 0x5a;
			header[0x05] = 0xa5;
			header[0x06] = 0xaa;
			header[0x07] = 0x55;

			header[0x24] = 0x2a;
			header[0x25] = 0x27;

			commandCode.ToLittleEndianBytes().CopyTo(header, 0x26);
			device.PacketCount.ToLittleEndianBytes().CopyTo(header, 0x28);

			device.MacAddress.CopyTo(header, 0x2a);

			var deviceId = device.DeviceId;
			if (deviceId == null)
			{
				deviceId = new byte[4];
			}
			deviceId.CopyTo(header, 0x30);

			GenerateChecksum(payload).CopyTo(header, 0x34);

			var encryptedPayload = payload.Encrypt(device.EncryptionKey);

			var packet = new byte[header.Length + encryptedPayload.Length];
			header.CopyTo(packet, 0);
			encryptedPayload.CopyTo(packet, 0x38);

			GenerateChecksum(packet).CopyTo(packet, 0x20);

			return packet;
		}

		public static byte[] GenerateStartLearningModePacket(BroadlinkDevice device)
		{
			var payload = new byte[16];
			payload[0x00] = 0x03;

			var command = (short)0x006a;
			var packet = GenerateCommandPacket(command, payload, device);
			return packet;
		}

		public static byte[] GenerateReadLearningModePacket(BroadlinkDevice device)
		{
			var payload = new byte[16];
			payload[0x00] = 0x04;

			var command = (short)0x006a;
			var packet = GenerateCommandPacket(command, payload, device);
			return packet;
		}

		public static byte[] GenerateSendDataPacket(BroadlinkDevice device, byte[] data)
		{
			var payload = new byte[0x04 + data.Length];
			payload[0x00] = 0x02;
			data.CopyTo(payload, 0x04);

			var command = (short)0x006a;
			var packet = GenerateCommandPacket(command, payload, device);
			return packet;
		}
	}
}
