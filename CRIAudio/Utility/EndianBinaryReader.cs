using System;
using System.IO;

namespace CRIAudio.Utility
{
	public class EndianBinaryReader : BinaryReader
	{
		public Endian Order { get; set; }
		public long Position
		{
			get => base.BaseStream.Position;
			set => base.BaseStream.Position = value;
		}
		public long Length => base.BaseStream.Length;

		private byte[] a16 = new byte[2];
		private byte[] a32 = new byte[4];
		private byte[] a64 = new byte[8];

		public EndianBinaryReader(byte[] bin): this(new MemoryStream(bin), Endian.LITTLE_ENDIAN) {  }
		public EndianBinaryReader(Stream input) : this(input, Endian.LITTLE_ENDIAN) { }
		public EndianBinaryReader(byte[] bin, Endian endian): this(new MemoryStream(bin), endian) { }
		public EndianBinaryReader(Stream input, Endian endian): base(input) { Order = endian; }
		~EndianBinaryReader() { Dispose(); }
		public new void Dispose()
		{
			base.Dispose();
		}

		public long Reset()
		{
			return base.BaseStream.Seek(0, SeekOrigin.Begin);
		}

		public override byte[] ReadBytes(int count)
		{
			return base.ReadBytes(count);
		}

		public override bool ReadBoolean()
		{
			return base.ReadBoolean();
		}

		public override byte ReadByte()
		{
			try
			{
				return base.ReadByte();
			}
			catch
			{
				return 0;
			}
		}

		public override sbyte ReadSByte()
		{
			try
			{
				return base.ReadSByte();
			}
			catch
			{
				return 0;
			}
		}

		public override char ReadChar()
		{
			return base.ReadChar();
		}

		public override short ReadInt16()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a16 = base.ReadBytes(2);
				Array.Reverse(a16);
				return BitConverter.ToInt16(a16, 0);
			}
			else return base.ReadInt16();
		}

		public override int ReadInt32()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a32 = base.ReadBytes(4);
				Array.Reverse(a32);
				return BitConverter.ToInt32(a32, 0);
			}
			else return base.ReadInt32();
		}

		public override long ReadInt64()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a64 = base.ReadBytes(8);
				Array.Reverse(a64);
				return BitConverter.ToInt64(a64, 0);
			}
			else return base.ReadInt64();
		}

		public override ushort ReadUInt16()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a16 = base.ReadBytes(2);
				Array.Reverse(a16);
				return BitConverter.ToUInt16(a16, 0);
			}
			else return base.ReadUInt16();
		}

		public override uint ReadUInt32()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a32 = base.ReadBytes(4);
				Array.Reverse(a32);
				return BitConverter.ToUInt32(a32, 0);
			}
			else return base.ReadUInt32();
		}

		public override ulong ReadUInt64()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a64 = base.ReadBytes(8);
				Array.Reverse(a64);
				return BitConverter.ToUInt64(a64, 0);
			}
			else return base.ReadUInt64();
		}

		public override float ReadSingle()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a32 = base.ReadBytes(4);
				Array.Reverse(a32);
				return BitConverter.ToSingle(a32, 0);
			}
			else return base.ReadSingle();
		}

		public override double ReadDouble()
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				a64 = base.ReadBytes(8);
				Array.Reverse(a64);
				return BitConverter.ToUInt64(a64, 0);
			}
			else return base.ReadDouble();
		}

		public void ReadSkip(long count)
		{
			Position += count;
		}

		public string ReadStringToNull()
		{
			string result = "";
			char c;
			for (int i = 0; i < base.BaseStream.Length; i++)
			{
				if ((c = (char)base.ReadByte()) == 0)
				{
					break;
				}
				result += c.ToString();
			}
			return result;
		}

		public byte[] PeekBytes(int length)
		{
			var origin = Position;
			var buf = ReadBytes(length);
			Position = origin;
			return buf;
		}

		public byte PeekByte()
		{
			var origin = Position;
			var value = ReadByte();
			Position = origin;
			return value;
		}

		public sbyte PeekSByte()
		{
			var origin = Position;
			var value = ReadSByte();
			Position = origin;
			return value;
		}

		public short PeekInt16()
		{
			var origin = Position;
			var value = ReadInt16();
			Position = origin;
			return value;
		}

		public ushort PeekUInt16()
		{
			var origin = Position;
			var value = ReadUInt16();
			Position = origin;
			return value;
		}

		public int PeekInt32()
		{
			var origin = Position;
			var value = ReadInt32();
			Position = origin;
			return value;
		}

		public uint PeekUInt32()
		{
			var origin = Position;
			var value = ReadUInt32();
			Position = origin;
			return value;
		}

		public long PeekInt64()
		{
			var origin = Position;
			var value = ReadInt64();
			Position = origin;
			return value;
		}

		public ulong PeekUInt64()
		{
			var origin = Position;
			var value = ReadUInt64();
			Position = origin;
			return value;
		}

		public float PeekSingle()
		{
			var origin = Position;
			var value = ReadSingle();
			Position = origin;
			return value;
		}

		public double PeekDouble()
		{
			var origin = Position;
			var value = ReadDouble();
			Position = origin;
			return value;
		}
	}
}
