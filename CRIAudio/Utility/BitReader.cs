using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Utility
{
	public class BitReader
	{
		byte[] data;
		public int Length { get; private set; }
		public int Position { get; private set; }
		public int Remaining => Length - Position;
		public BitReader(byte[] data)
		{
			this.data = data;
			Length = data.Length * 8;
			Position = 0;
		}

		public int CheckBit(int bitSize)
		{
			if (!(Position + bitSize <= Length))
				return 0;

			int byteIndex = Position / 8;
			int bitIndex = Position % 8;
			if (bitSize > Remaining)
			{
				if (Position >= Length) return 0;

				int extraBits = bitSize - Remaining;
				return CheckBitFallback(Remaining) << extraBits;
			}

			switch (bitSize)
			{
				case int i when i > 8 && Remaining <= 16:
					{
						int value = data[byteIndex] << 8 | data[byteIndex + 1];
						value &= 0xFFFF >> bitIndex;
						value >>= 16 - bitSize - bitIndex;
						return value;
					}
				case int i when i > 16 && Remaining <= 24:
					{
						int value = data[byteIndex] << 16 | data[byteIndex + 1] << 8 | data[byteIndex + 2];
						value &= 0xFFFFFF >> bitIndex;
						value >>= 24 - bitSize - bitIndex;
						return value;
					}
				case int i when i > 24 && Remaining <= 32:
					{
						int value = data[byteIndex] << 24 | data[byteIndex + 1] << 16 | data[byteIndex + 2] << 8 | data[byteIndex + 3];
						value &= (int)(0xFFFFFFFF >> bitIndex);
						value >>= 32 - bitSize - bitIndex;
						return value;
					}
				default:
					return CheckBitFallback(bitSize);
			}

			/*uint v = 0;
			if (bit + bitSize <= length)
			{
				uint[] mask = { 0xFFFFFF, 0x7FFFFF, 0x3FFFFF, 0x1FFFFF, 0x0FFFFF, 0x07FFFF, 0x03FFFF, 0x01FFFF };
				byte[] bin = new byte[data.Length - (bit >> 3)];
				Array.Copy(data, bit >> 3, bin, 0, bin.Length);
				v = data[0];
				v = (v << 8) | data[1];
				v = (v << 8) | data[2];
				v &= mask[bit & 7];
				v >>= 24 - (bit & 7) - bitSize;
			}
			return v;*/
		}
		private int CheckBitFallback(int bitSize)
		{
			int value = 0;
			int byteIndex = Position / 8;
			int bitIndex = Position % 8;

			while (bitSize > 0)
			{
				if (bitIndex >= 8)
				{
					bitIndex = 0;
					byteIndex++;
				}

				int bitsToRead = Math.Min(bitSize, 8 - bitIndex);
				int mask = 0xFF >> bitIndex;
				int currentByte = (mask & data[byteIndex]) >> (8 - bitIndex - bitsToRead);

				value = (value << bitsToRead) | currentByte;
				bitIndex += bitsToRead;
				bitSize -= bitsToRead;
			}
			return value;
		}

		public int GetBit(int bitSize)
		{
			int v = CheckBit(bitSize);
			Position += bitSize;
			return v;
		}
		public int PeekBit(int bitSize) => CheckBit(bitSize);
		public void AddBit(int bitSize)
		{
			Position += bitSize;
		}

		public int GetInt32()
		{
			return GetBit(32);
		}
		public int GetInt16()
		{
			return GetBit(16);
		}
		public int GetByte()
		{
			return GetBit(8);
		}
	}
}
