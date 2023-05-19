using System.IO;
using CRIAudio.Utility;

/*** ŽQlFhttps://subdiox.github.io/deresute/resource/criware.html ***/
namespace CRIAudio.Container.UTF
{
    public class UTFData
    {
        public static UTFData ReadData(byte[] bin) => ReadData(new EndianBinaryReader(bin, Endian.BIG_ENDIAN));
        public static UTFData ReadData(EndianBinaryReader reader)
        {
            var data = new UTFData();
            var info = data.Info;
            reader.Reset();

            // signature check
            var signature = reader.ReadUInt32();
            if (signature != 0x40555446)
            {
                throw new InvalidDataException();
            }

            // info
            info.FileSize = reader.ReadUInt32();
            info.Version = reader.ReadUInt16();
            info.TableOffset = reader.ReadUInt16();
            info.StringOffset = reader.ReadUInt32();
            info.BinaryOffset = reader.ReadUInt32();
            info.TableNameOffset = reader.ReadUInt32();
            info.ColumnCount = reader.ReadUInt16();
            info.RowWidth = reader.ReadUInt16();
            info.RowCount = reader.ReadUInt32();
            
            // columns
            data.Columns = new UTFColumn[info.ColumnCount];
            var Columns = data.Columns;
            for (int i = 0;i < info.ColumnCount; i++)
            {
                var ID = reader.ReadByte();
                var Offset = reader.ReadUInt32();
                Columns[i] = new UTFColumn
                {
                    ColumnType = (UTFColumnType)(ID & 0xF0),
                    DataType = (UTFDataType)(ID & 0x0F),
                    NameOffset = Offset
                };
            }

            // table name
            reader.Reset();
            reader.Position = info.StringOffset + 8 + info.TableNameOffset;
            info.TableName = reader.ReadStringToNull();

            // column name
            for (int i = 0; i < info.ColumnCount; i++)
            {
                var Offset = Columns[i].NameOffset;
                reader.Position = info.StringOffset + 8 + Offset;
                var Name = reader.ReadStringToNull();
                Columns[i].Name = Name;
            }

            // column data
            int pos = 0;
            for (int i = 0; i < info.ColumnCount; i++)
            {
                var DataType = Columns[i].DataType;
                reader.Position = info.TableOffset + 8 + pos;
                switch (DataType)
                {
                    case UTFDataType.UInt8:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadByte());
                            pos += 1;
                        }
                        break;
                    case UTFDataType.Int8:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadSByte());
                            pos += 1;
                        }
                        break;
                    case UTFDataType.UInt16:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadUInt16());
                            pos += 2;
                        }
                        break;
                    case UTFDataType.Int16:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadInt16());
                            pos += 2;
                        }
                        break;
                    case UTFDataType.UInt32:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadUInt32());
                            pos += 4;
                        }
                        break;
                    case UTFDataType.Int32:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadInt32());
                            pos += 4;
                        }
                        break;
                    case UTFDataType.UInt64:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadUInt64());
                            pos += 8;
                        }
                        break;
                    case UTFDataType.Int64:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadInt64());
                            pos += 8;
                        }
                        break;
                    case UTFDataType.Float:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadSingle());
                            pos += 4;
                        }
                        break;
                    case UTFDataType.Double:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadDouble());
                            pos += 8;
                        }
                        break;
                    case UTFDataType.String:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadUInt32(), UTFDataType.String);
                            pos += 4;
                        }
                        break;
                    case UTFDataType.Binary:
                        {
                            Columns[i].ColumnData = new UTFColumnData(reader.ReadUInt64(), UTFDataType.Binary);
                            pos += 8;
                        }
                        break;
                }
            }

            data.Columns = Columns;
            data.Info = info;
            return data;
        }

        public UTFInfo Info { get; set; } = new UTFInfo();
        public UTFColumn[] Columns { get; set; }
    }
}