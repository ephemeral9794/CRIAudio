using System;
using System.IO;
using CRIAudio.Utility;

namespace CRIAudio.Container.AFS2 {
    public static class AFS2Reader {
        public static AFS2Data ReadData(byte[] bin) => ReadData(new EndianBinaryReader(bin, Endian.LITTLE_ENDIAN));
        public static AFS2Data ReadData(EndianBinaryReader reader) {
            var data = new AFS2Data();

            reader.Order = Endian.BIG_ENDIAN;
            var signature = reader.ReadUInt32();
            if (signature != 0x41465332) {
                throw new InvalidDataException();
            }

            reader.Order = Endian.LITTLE_ENDIAN;
            var info = data.Info;
			info.Version = reader.ReadByte();
			info.SizeLength = reader.ReadByte();
			info.Reserve1 = reader.ReadByte();
			info.Reserve2 = reader.ReadByte();
			info.FileCount = reader.ReadUInt32();
			info.Alignment = reader.ReadUInt16();
			info.SubKey = reader.ReadUInt16();
            data.Info = info;

            var ID = new ushort[info.FileCount];
            for (var i = 0; i < info.FileCount; i++) {
                ID[i] = reader.ReadUInt16();
            }
            var size = new uint[info.FileCount + 1];
			for (var i = 0; i < info.FileCount + 1; i++)
			{
				switch (info.SizeLength) {
					case 1: size[i] = reader.ReadByte(); break;
					case 2: size[i] = reader.ReadUInt16(); break;
					case 4: size[i] = reader.ReadUInt32(); break;
				}
			}
            for (var i = 0; i < info.FileCount; i++)
			{
                var track = new AFS2Track();
                track.ID = ID[i];
				track.Offset = Aligned(size[i], info.Alignment);
				track.Size = size[i + 1] - track.Offset;
				reader.Reset();
				reader.ReadSkip(track.Offset);
				track.Signature = reader.ReadUInt32();
                reader.Position -= sizeof(uint);
                track.Binary = reader.ReadBytes((int)track.Size);
                data.Tracks.Add(track);
			}

            return data;
        }

        private static uint Aligned(uint v, ushort align) => (uint)(v + (align - 1) & (-1 ^ align - 1));
		private static bool CheckADX(uint data) => (data == 0x0080);
		private static bool CheckAFS2(uint data) => (data == 0x32534641);
		private static bool CheckCPK(uint data) => (data == 0x204B5043);
		private static bool CheckCRID(uint data) => (data == 0x44495243);
		private static bool CheckHCA(uint data) => ((data & 0x7F7F7F7F) == 0x00414348);
		private static bool CheckICE(uint data) => (data == 0x00454349);
		private static bool CheckUTF(uint data) => (data == 0x46545540);
		private static bool CheckPSMF(uint data) => (data == 0x464D5350);
		private static bool CheckRIFF(uint data) => (data == 0x46464952);
    }
}