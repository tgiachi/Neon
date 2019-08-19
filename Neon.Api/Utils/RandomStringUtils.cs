using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Neon.Api.Utils
{
	public class RandomStringUtils
	{
		public static string RandomString(int length)
		{
			const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+";
			var res = new StringBuilder();
			using (var rng = new RNGCryptoServiceProvider())
			{
				byte[] uintBuffer = new byte[sizeof(uint)];

				while (length-- > 0)
				{
					rng.GetBytes(uintBuffer);
					uint num = BitConverter.ToUInt32(uintBuffer, 0);
					res.Append(valid[(int)(num % (uint)valid.Length)]);
				}
			}

			return res.ToString();
		}
	}
}
