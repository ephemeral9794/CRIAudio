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
		int length;
		int bit;
		public BitReader(byte[] data)
		{
			this.data = data;
			length = data.Length * 8;
			bit = 0;
		}

		public uint CheckBit(int bitSize)
		{
			uint v = 0;
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
			return v;
		}
		public uint GetBit(int bitSize)
		{
			uint v = CheckBit(bitSize);
			bit += bitSize;
			return v;
		}
		public uint PeekBit(int bitSize) => CheckBit(bitSize);
		public void AddBit(int bitSize)
		{
			bit += bitSize;
		}

		public uint GetUInt32()
		{
			return GetBit(32);
		}
		public uint GetUInt16()
		{
			return GetBit(16);
		}
		public uint GetByte()
		{
			return GetBit(8);
		}
	}
}
