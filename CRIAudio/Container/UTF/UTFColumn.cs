using System;
using System.Collections.Generic;
using System.Dynamic;

namespace CRIAudio.Container.UTF
{
    public enum UTFColumnType
    {
        ZeroFix = 0x10,
        Constant1 = 0x30,
        Common = 0x50,
        Constant2 = 0x70
    }
    public enum UTFDataType
    {
        UInt8 = 0x00,
        Int8 = 0x01,
        UInt16 = 0x02,
        Int16 = 0x03,
        UInt32 = 0x04,
        Int32 = 0x05,
        UInt64 = 0x06,
        Int64 = 0x07,
        Float = 0x08,
        Double = 0x09,
        String = 0x0A,
        Binary = 0x0B
    }

    public struct UTFColumn
    {
        public UTFColumnType ColumnType { get; set; }
        public UTFDataType DataType { get; set; }
        public uint NameOffset { get; set; }
        public string Name { get; set; }
    }

    public struct UTFColumnData
    {
        public UTFColumnData(byte input) : this(new byte[1] {input}) { }
        public UTFColumnData(sbyte input) : this(new byte[1] { (byte)input }) { }
        public UTFColumnData(ushort input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(short input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(uint input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(int input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(ulong input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(long input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(float input) : this(BitConverter.GetBytes(input)) { }
        public UTFColumnData(double input) : this(BitConverter.GetBytes(input)) { }
        private UTFColumnData(byte[] input)
        {
            data = new byte[8];
            Array.Clear(data);
            Array.Copy(input, 0, data, 0, input.Length);
        }

        public override string ToString()
        {
            return string.Format("0x{0,0:X16}", BitConverter.ToUInt64(data));
        }

        private byte[] data;
        public byte UInt8 { 
            get { 
                return data[0];
            }
        }
        public sbyte Int8
        {
            get
            {
                return (sbyte)data[0];
            }
        }
        public ushort UInt16
        {
            get
            {
                var buf = new byte[2];
                Array.Copy(data, buf, 2);
                return BitConverter.ToUInt16(buf);
            }
        }
        public short Int16
        {
            get
            {
                var buf = new byte[2];
                Array.Copy(data, buf, 2);
                return BitConverter.ToInt16(buf);
            }
        }
        public uint UInt32
        {
            get
            {
                var buf = new byte[4];
                Array.Copy(data, buf, 4);
                return BitConverter.ToUInt32(buf);
            }
        }
        public int Int32
        {
            get
            {
                var buf = new byte[4];
                Array.Copy(data, buf, 4);
                return BitConverter.ToInt32(buf);
            }
        }
        public ulong UInt64
        {
            get
            {
                return BitConverter.ToUInt64(data);
            }
        }
        public long Int64
        {
            get
            {
                return BitConverter.ToInt64(data);
            }
        }
        public float Float
        {
            get
            {
                var buf = new byte[4];
                Array.Copy(data, buf, 4);
                return BitConverter.ToSingle(buf);
            }
        }
        public double Double
        {
            get
            {
                return BitConverter.ToDouble(data);
            }
        }
        public uint String
        {
            get
            {
                return UInt32;
            }
        }
        public Tuple<uint, uint> Binary
        {
            get
            {
                var offset = new byte[4];
                var size = new byte[4];
                Array.Copy(data, 0, size, 0, 4);
                Array.Copy(data, 4, offset, 0, 4);

                var pair = new Tuple<uint, uint>(
                    BitConverter.ToUInt32(offset),
                    BitConverter.ToUInt32(size)
                );
                return pair;
            }
        }
    }
}
