using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CRIAudio.Utility
{
	public static class Extension
	{
		public static short ToInt16(this byte[] bin, int startIndex, Endian endian) {
			if (endian == Endian.BIG_ENDIAN) {
				byte[] a16 = new byte[2];
				Array.Copy(bin, startIndex, a16, 0, a16.Length);
				Array.Reverse(a16);
				return BitConverter.ToInt16(a16, 0);
			}
			else return BitConverter.ToInt16(bin, startIndex);
		}
		public static ushort ToUInt16(this byte[] bin, int startIndex, Endian endian) {
			if (endian == Endian.BIG_ENDIAN) {
				byte[] a16 = new byte[2];
				Array.Copy(bin, startIndex, a16, 0, a16.Length);
				Array.Reverse(a16);
				return BitConverter.ToUInt16(a16, 0);
			}
			else return BitConverter.ToUInt16(bin, startIndex);
		}
		public static int ToInt32(this byte[] bin, int startIndex, Endian endian) {
			if (endian == Endian.BIG_ENDIAN) {
				byte[] a32 = new byte[4];
				Array.Copy(bin, startIndex, a32, 0, a32.Length);
				Array.Reverse(a32);
				return BitConverter.ToInt32(a32, 0);
			}
			else return BitConverter.ToInt32(bin, startIndex);
		}
		public static uint ToUInt32(this byte[] bin, int startIndex, Endian endian) {
			if (endian == Endian.BIG_ENDIAN) {
				byte[] a32 = new byte[4];
				Array.Copy(bin, startIndex, a32, 0, a32.Length);
				Array.Reverse(a32);
				return BitConverter.ToUInt32(a32, 0);
			}
			else return BitConverter.ToUInt32(bin, startIndex);
		}
		public static float ToFloat(this uint n) {
			var buf = BitConverter.GetBytes(n);
			return BitConverter.ToSingle(buf);
		}
		public static float ToFloat(this int n)
		{
			var buf = BitConverter.GetBytes(n);
			return BitConverter.ToSingle(buf);
		}
		public static T[] FillArray<T>(this T[] array, T n) {
			for (int i = 0; i < array.Length; i++) {
				array[i] = n;
			}
			return array;
		}
		public static T[] FillArray<T>(this T[] array, int offset, int length, T n) {
			if (length > array.Length || offset >= array.Length) {
				throw new ArgumentOutOfRangeException();
			}
			for (int i = offset; i < length; i++) {
				array[i] = n;
			}
			return array;
		}
		//public static int DivideByRoundUp(this int value, int divisor) => (int)Math.Ceiling((double)value / divisor);
		public static uint DivideByRoundUp(this uint value, uint divisor) => (uint)Math.Ceiling((double)value / divisor);
		public static byte GetLowNibble(this byte value) => (byte)(value & 0x0F);
		public static byte GetHighNibble(this byte value) => (byte)((value >> 4) & 0x0F);
		public static List<string> ParseString(this byte[] data) {
			return ParseString(new EndianBinaryReader(data));
		}
		public static List<string> ParseString(this EndianBinaryReader stream) {
			List<string> list = new List<string>();
			try {
				while(true) {
					list.Add(stream.ReadStringToNull());
				}
			} catch (System.IO.EndOfStreamException) { }
			return list;
		}
		public static uint ReverseUInt(this uint n) {
			byte[] bytes = BitConverter.GetBytes(n);
			Array.Reverse(bytes);
			return BitConverter.ToUInt32(bytes, 0);
		}
		public static string ReverseUIntToString(this uint n) {
			return UIntToString(ReverseUInt(n));
		}
		public static string UIntToString(this uint n) {
			return UIntToString(n, Endian.LITTLE_ENDIAN);
		}
		public static string UIntToString(this uint n, Endian type) {
			if (type == Endian.LITTLE_ENDIAN) {
				byte[] bytes = BitConverter.GetBytes(n);
				Array.Reverse(bytes);
				return Encoding.GetEncoding("ASCII").GetString(bytes);
			} else {
				return Encoding.GetEncoding("ASCII").GetString(BitConverter.GetBytes(n));
			}
		}
		public static T[][] InitializeJaggedArray<T>(this T[][] array, int length)
		{
			for (var i = 0; i < array.Length; i++)
			{
				array[i] = new T[length];
			}
			return array;
		}
		public static int Clamp(int value, int min, int max)
		{
			if (value < min)
				return min;
			if (value > max)
				return max;
			return value;
		}

		// List.ToString Extention
		// http://kamiya.hatenadiary.jp/entry/2014/03/11/023140
		public static string ToString<T>(this List<T> list,Func<T,string> toStr = null,string format = "{0},"){
			//default
			if (toStr == null) toStr = (t) => t.ToString();
			
			var strs = (from p in list select (toStr(p))).ToArray();
			var sb = new StringBuilder("[");
			foreach (var str in strs) {
				sb.AppendFormat(format, str);
			}
			sb.Append(']');
			return sb.ToString();
		}
		public static string ToString<T>(this T[] array, Func<T,string> toStr = null, string format = "{0},")
		{
			//default
			if (toStr == null) toStr = (t) => t.ToString();

			var strs = (from p in array select (toStr(p))).ToArray();
			var sb = new StringBuilder("[");
			foreach (var str in strs)
			{
				sb.AppendFormat(format, str);
			}
			sb.Append(']');
			return sb.ToString();
		}

		public static string intToStr(uint i)
		{
			var b = BitConverter.GetBytes(i);
			var c = new char[b.Length];
			for (var n = 0; n < b.Length; n++)
			{
				c[n] = (char)b[n];
			}
			return new string(c);
		}

		public static uint Ceil2(uint a, uint b)
		{
			return (b > 0) ? (uint)(a / b + ((a % b) != 0 ? 1 : 0)) : 0;
		}
	}
}
