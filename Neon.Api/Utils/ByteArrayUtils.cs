using System;

namespace Neon.Api.Utils
{
	public static class ByteArrayExtensions
	{
		/// <summary>
		/// Convert an <see cref="int"/> value to a <see cref="byte[]"/> with little endian ordering
		/// </summary>
		/// <param name="input"><see cref="int"/> value</param>
		/// <returns><see cref="byte[]"/> with little endian ordering</returns>
		public static byte[] ToLittleEndianBytes(this int input)
		{
			if (!BitConverter.IsLittleEndian)
			{
				throw new NotImplementedException("Computer is big endian and conversion is not implemented yet");
			}
			return BitConverter.GetBytes(input);
		}

		/// <summary>
		/// Convert a <see cref="short"/> value to a <see cref="byte[]"/> with little endian ordering
		/// </summary>
		/// <param name="input"><see cref="short"/> value</param>
		/// <returns><see cref="byte[]"/> with little endian ordering</returns>
		public static byte[] ToLittleEndianBytes(this short input)
		{
			if (!BitConverter.IsLittleEndian)
			{
				throw new NotImplementedException("Computer is big endian and conversion is not implemented yet");
			}
			return BitConverter.GetBytes(input);
		}

		///// <summary>
		///// Convenience function to get the slice of a <see cref="byte[]"/> from <paramref name="input"/>
		///// </summary>
		///// <param name="input">byte array to slice</param>
		///// <param name="startIndex">start index to slice from</param>
		///// <returns>slice of <paramref name="input"/> starting from <paramref name="startIndex"/></returns>
		//public static byte[] Slice(this byte[] input, int startIndex)
		//{
		//    var restArray = new byte[input.Length - startIndex];
		//    Array.Copy(input, startIndex, restArray, 0, restArray.Length);
		//    return restArray;
		//}

		/// <summary>
		/// Convenience function to get the slice of a <see cref="byte[]"/>
		/// </summary>
		/// <param name="input">byte array to slice</param>
		/// <param name="startIndex">start index to slice from</param>
		/// <param name="endIndex">endIndex to slice to. Use -1 to slice until the end</param>
		/// <returns>slice of <paramref name="input"/></returns>
		public static byte[] Slice(this byte[] input, int startIndex, int endIndex = -1)
		{
			if (endIndex <= 0)
			{
				endIndex = input.Length - 1;
			}

			if (endIndex <= startIndex || endIndex >= input.Length)
			{
				throw new ArgumentException();
			}

			var restArray = new byte[endIndex - startIndex + 1];

			Array.Copy(input, startIndex, restArray, 0, restArray.Length);
			return restArray;
		}
	}
}
