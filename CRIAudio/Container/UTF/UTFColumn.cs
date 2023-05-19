using System;

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
        public UTFColumnData ColumnData { get; set; }
    }

    public struct UTFColumnData
    {
        public UTFColumnData(byte input) : this(new byte[] {input}, UTFDataType.UInt8) { }
        public UTFColumnData(sbyte input) : this(new byte[] { (byte)input }, UTFDataType.Int8) { }
        public UTFColumnData(ushort input) : this(BitConverter.GetBytes(input), UTFDataType.UInt16) { }
        public UTFColumnData(short input) : this(BitConverter.GetBytes(input), UTFDataType.Int16) { }
        public UTFColumnData(uint input) : this(BitConverter.GetBytes(input), UTFDataType.UInt32) { }
        public UTFColumnData(int input) : this(BitConverter.GetBytes(input), UTFDataType.Int32) { }
        public UTFColumnData(ulong input) : this(BitConverter.GetBytes(input), UTFDataType.UInt64) { }
        public UTFColumnData(long input) : this(BitConverter.GetBytes(input), UTFDataType.Int64) { }
        public UTFColumnData(float input) : this(BitConverter.GetBytes(input), UTFDataType.Float) { }
        public UTFColumnData(double input) : this(BitConverter.GetBytes(input), UTFDataType.Double) { }
        public UTFColumnData(byte input, UTFDataType type) : this(new byte[] { input }, type) { }
        public UTFColumnData(sbyte input, UTFDataType type) : this(new byte[] { (byte)input }, type) { }
        public UTFColumnData(ushort input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(short input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(uint input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(int input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(ulong input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(long input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(float input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        public UTFColumnData(double input, UTFDataType type) : this(BitConverter.GetBytes(input), type) { }
        private UTFColumnData(byte[] input, UTFDataType type)
        {
            data = new byte[8];
            this.type = type;
            Array.Clear(data);
            Array.Copy(input, 0, data, 0, input.Length);
        }

        public override string ToString()
        {
            switch (type)
            {
                case UTFDataType.UInt8:
                    return UInt8.ToString();
                case UTFDataType.Int8:
                    return Int8.ToString();
                case UTFDataType.UInt16:
                    return UInt16.ToString();
                case UTFDataType.Int16:
                    return Int16.ToString();
                case UTFDataType.UInt32:
                    return UInt32.ToString();
                case UTFDataType.Int32:
                    return Int32.ToString();
                case UTFDataType.UInt64:
                    return UInt64.ToString();
                case UTFDataType.Int64:
                    return Int64.ToString();
                case UTFDataType.Float:
                    return Float.ToString();
                case UTFDataType.Double:
                    return Double.ToString();
                case UTFDataType.String:
                    return String.ToString();
                case UTFDataType.Binary:
                    return Binary.ToString();
                default:
                    return string.Format("0x{0,0:X16}", BitConverter.ToUInt64(data));
            }
        }

        private UTFDataType type;
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
