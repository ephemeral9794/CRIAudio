using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CRIAudio.Utility
{
	public class EndianBinaryWriter : BinaryWriter
	{
		public Endian Order { get; set; }
		public long Position
		{
			get => base.BaseStream.Position;
			set => base.BaseStream.Position = value;
		}
		public long Length => base.BaseStream.Length;

		public EndianBinaryWriter(Stream output, Endian endian = Endian.LITTLE_ENDIAN) : base(output) { Order = endian; }
		public EndianBinaryWriter(Stream output, Encoding encoding, Endian endian = Endian.LITTLE_ENDIAN) : base(output, encoding) { Order = endian; }
		~EndianBinaryWriter() { Dispose(); }
		public new void Dispose()
		{
			base.Dispose();
		}

		public long Reset()
		{
			return Seek(0, SeekOrigin.Begin);
		}

		public override void Write(byte value)
		{
			base.Write(value);
		}
		public override void Write(sbyte value)
		{
			base.Write(value);
		}
		public override void Write(bool value)
		{
			base.Write(value);
		}
		public override void Write(long value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
		public override void Write(ulong value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			} else { 
				base.Write(value);
			}
		}
		public override void Write(int value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
		public override void Write(uint value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
		public override void Write(short value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
		public override void Write(ushort value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
		public override void Write(float value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
		public override void Write(double value)
		{
			if (Order == Endian.BIG_ENDIAN)
			{
				var bin = BitConverter.GetBytes(value);
				Array.Reverse(bin);
				base.Write(bin);
			}
			else
			{
				base.Write(value);
			}
		}
	}
}
