using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Neon.Api.Utils
{
	public static class NetworkUtils
	{
		public static async void SendWakeUpOnLanPackage(string macAddress)
		{
			macAddress = Regex.Replace(macAddress, "[-|:]", "");
			var sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
			{
				EnableBroadcast = true
			};

			var payloadIndex = 0;

			/* The magic packet is a broadcast frame containing anywhere within its payload 6 bytes of all 255 (FF FF FF FF FF FF in hexadecimal), followed by sixteen repetitions of the target computer's 48-bit MAC address, for a total of 102 bytes. */
			var payload = new byte[1024];    // Our packet that we will be broadcasting

			// Add 6 bytes with value 255 (FF) in our payload
			for (var i = 0; i < 6; i++)
			{
				payload[payloadIndex] = 255;
				payloadIndex++;
			}

			// Repeat the device MAC address sixteen times
			for (var j = 0; j < 16; j++)
			{
				for (var k = 0; k < macAddress.Length; k += 2)
				{
					var s = macAddress.Substring(k, 2);
					payload[payloadIndex] = byte.Parse(s, NumberStyles.HexNumber);
					payloadIndex++;
				}
			}

			sock.SendTo(payload, new IPEndPoint(IPAddress.Parse("255.255.255.255"), 0));
			await Task.Delay(3000);
			sock.Close();
			sock.Dispose();
		}

	}
}
