using System;
using System.IO;
using CRIAudio.Utility;
using static CRIAudio.Decoder.HCA.HCAConstants;
using static CRIAudio.Utility.Extension;


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

            data.Columns = Columns;
            data.Info = info;
            return data;
        }

        public UTFInfo Info { get; set; } = new UTFInfo();
        public UTFColumn[] Columns { get; set; }
    }
}