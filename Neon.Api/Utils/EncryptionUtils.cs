using System.IO;
using System.Security.Cryptography;

namespace Neon.Api.Utils
{
	public static class EncryptionExtensions
	{
		public static readonly byte[] IV = new byte[] { 0x56, 0x2e, 0x17, 0x99, 0x6d, 0x09, 0x3d, 0x28, 0xdd, 0xb3, 0xba, 0x69, 0x5a, 0x2e, 0x6f, 0x58 };
		public static readonly byte[] InitialKey = new byte[] { 0x09, 0x76, 0x28, 0x34, 0x3f, 0xe9, 0x9e, 0x23, 0x76, 0x5c, 0x15, 0x13, 0xac, 0xcf, 0x8b, 0x02 };

		public static byte[] Encrypt(this byte[] input, byte[] encryptionKey = null)
		{
			byte[] encrypted;
			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.IV = IV;
				aesAlg.Key = encryptionKey ?? InitialKey;
				aesAlg.Mode = CipherMode.CBC;

				// Create an encrytor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt
						= new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						csEncrypt.Write(input, 0, input.Length);
						//csEncrypt.FlushFinalBlock();
						encrypted = msEncrypt.ToArray();
					}
				}
			}
			return encrypted;
		}

		//Decrypt byte[]
		public static byte[] Decrypt(this byte[] input, byte[] encryptionKey = null)
		{
			byte[] decrypted = new byte[input.Length];

			Aes aes = Aes.Create();
			aes.IV = IV;
			aes.Key = encryptionKey ?? InitialKey;
			aes.Mode = CipherMode.CBC;
			aes.Padding = PaddingMode.None;

			// Create an decrytor to perform the stream transform.
			ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

			// Create the streams used for decryption.
			using (MemoryStream msDecrypt = new MemoryStream())
			{
				using (CryptoStream csDecrypt
					= new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Write))
				{
					csDecrypt.Write(input, 0, input.Length);
					decrypted = msDecrypt.ToArray();
				}
			}
			return decrypted;
		}

		//public static byte[] Decrypt(this byte[] input, byte[] encryptionKey = null)
		//{
		//    byte[] decrypted;
		//    using (Aes aesAlg = Aes.Create())
		//    {
		//        aesAlg.IV = IV;
		//        aesAlg.Key = encryptionKey ?? InitialKey;
		//        aesAlg.Mode = CipherMode.CBC;

		//        // Create an decrytor to perform the stream transform.
		//        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

		//        // Create the streams used for decryption.
		//        using (MemoryStream msDecrypt = new MemoryStream(input))
		//        {
		//            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
		//            {
		//                decrypted = new byte[input.Length];
		//                var decryptedBytes = csDecrypt.Read(decrypted, 0, input.Length);

		//            }
		//        }
		//    }
		//    return decrypted;
		//}


	}
}
