using Neon.Api.Attributes.SecretKey;
using Neon.Api.Interfaces.Managers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Neon.Api.Core
{
	public class SecretKeyManager : ISecretKeyManager
	{
		private readonly string _key;

		public SecretKeyManager(string key)
		{
			_key = key;
		}

		public T ProcessLoad<T>(T obj)
		{
			obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(p =>
			{
				if (p.GetCustomAttribute<SecretValueAttribute>() != null)
				{
					if (p.PropertyType == typeof(string))
					{
						var strValue = p.GetValue(obj) as string;

						if (strValue.StartsWith("secret:"))
						{
							strValue = strValue.Replace("secret:", "");

							p.SetValue(obj, DecryptString(strValue));
						}
					}
				}

			});

			return obj;
		}

		public T ProcessSave<T>(T obj)
		{
			obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList().ForEach(p =>
			{
				if (p.GetCustomAttribute<SecretValueAttribute>() != null)
				{
					if (p.PropertyType == typeof(string))
					{
						var strValue = p.GetValue(obj) as string;

						p.SetValue(obj, $"secret:{EncryptString(strValue)}");
					}
				}
			});

			return obj;
		}

		public string Encrypt(string text)
		{
			return EncryptString(text);
		}

		public string Decrypt(string text)
		{
			return DecryptString(text);
		}

		public string EncryptString(string text)
		{
			var key = Encoding.UTF8.GetBytes(_key);

			using (var aesAlg = Aes.Create())
			{
				using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
				{
					using (var msEncrypt = new MemoryStream())
					{
						using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
						using (var swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(text);
						}

						var iv = aesAlg.IV;

						var decryptedContent = msEncrypt.ToArray();

						var result = new byte[iv.Length + decryptedContent.Length];

						Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
						Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

						return Convert.ToBase64String(result);
					}
				}
			}
		}

		private string DecryptString(string cipherText)
		{
			var fullCipher = Convert.FromBase64String(cipherText);

			var iv = new byte[16];
			var cipher = new byte[16];

			Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
			Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
			var key = Encoding.UTF8.GetBytes(_key);

			using (var aesAlg = Aes.Create())
			{
				using (var decryptor = aesAlg.CreateDecryptor(key, iv))
				{
					string result;
					using (var msDecrypt = new MemoryStream(cipher))
					{
						using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
						{
							using (var srDecrypt = new StreamReader(csDecrypt))
							{
								result = srDecrypt.ReadToEnd();
							}
						}
					}

					return result;
				}
			}
		}
	}
}
