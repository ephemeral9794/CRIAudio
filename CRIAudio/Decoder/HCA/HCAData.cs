using System;
using System.Collections.Generic;
using System.IO;
using CRIAudio.Utility;
using static CRIAudio.Decoder.HCA.HCAConstants;
using static CRIAudio.Utility.Extension;

namespace CRIAudio.Decoder.HCA
{
    public class HCAData
    {
        enum HCASignature
        {
            HCA = 0x48434100,
            Format = 0x666D7400,
            Compress = 0x636F6D70,
            Dec = 0x64656300,
            Ath = 0x61746800,
            Loop = 0x6C6F6F70,
            Ciph = 0x63697068,
            Rva = 0x72766100,
            Comment = 0x636F6D6D,
            Pad = 0x70616400,
        }

        private static CRC16 CRC { get; } = new CRC16(0x8005);
        private static bool CheckSignature(uint s, HCASignature ss) => (s & 0x7f7f7f7f) == (uint)ss;

        public static HCAData ReadData(byte[] bin, HCAKey key = null) => ReadData(new EndianBinaryReader(bin, Endian.BIG_ENDIAN), key);
        public static HCAData ReadData(EndianBinaryReader reader, HCAKey key = null)
        {
			var data = new HCAData();
			var info = data.Info;
			reader.Reset();

			// Header
			uint signature = reader.PeekUInt32();
			if (CheckSignature(signature, HCASignature.HCA))
			{
				reader.ReadSkip(sizeof(uint));
				info.Version = reader.ReadUInt16();
				info.DataOffset = reader.ReadUInt16();
			}
			else
			{
				throw new InvalidDataException("Not a valid HCA file");
			}

			bool hasAthChunk = false, hasRvaChunk = false;
			while (reader.Position < info.DataOffset)
			{
				signature = reader.PeekUInt32() & 0x7f7f7f7f;
				switch ((HCASignature)signature)
				{
					case HCASignature.Format:
						ReadFormatChunk(reader, ref info);
						break;
					case HCASignature.Compress:
						ReadCompChunk(reader, ref info);
						break;
					case HCASignature.Dec:
						ReadDecChunk(reader, ref info);
						break;
					case HCASignature.Ath:
						ReadAthChunk(reader, ref info);
						hasAthChunk = true;
						break;
					case HCASignature.Loop:
						ReadLoopChunk(reader, ref info);
						break;
					case HCASignature.Ciph:
						ReadCiphChunk(reader, ref info);
						break;
					case HCASignature.Rva:
						ReadRvaChunk(reader, ref info);
						hasRvaChunk = true;
						break;
					case HCASignature.Comment:
						ReadCommentChunk(reader, ref info);
						reader.Position = info.DataOffset;
						break;
					case HCASignature.Pad:
						reader.Position = info.DataOffset;
						break;
					default:
						break;
				}
			}

			if (info.Version < 0x0200 && !hasAthChunk)
				info.UseAthTable = true;
			if (info.TrackCount < 1)
				info.TrackCount = 1;
			if (!hasRvaChunk)
				info.Volume = 1.0f;

			data.Random = 1;
			data.Info = info;

			if (key == null)
			{
				if (info.EncryptType == 0 || info.EncryptType == 1)
				{
					data.Key = new HCAKey(info.EncryptType);
				}
				else
				{
					throw new ArgumentNullException("Key is required for the 56-bit encryption type.");
				}
			}
			else
			{
				data.Key = key;
			}

			data.AudioData = new byte[info.FrameCount][];
			for (int i = 0; i < info.FrameCount; i++)
			{
				byte[] bin = reader.ReadBytes((int)info.FrameSize);
				int crc = CRC.Compute(bin, bin.Length - 2);
				int expectedCrc = bin[bin.Length - 2] << 8 | bin[bin.Length - 1];
				if (crc != expectedCrc)
				{
					// TODO: Decide how to handle bad CRC
					Console.Error.WriteLine($"Frame#{i}: Bad CRC.");
				}

				data.AudioData[i] = bin;
			}

			return data;
		}

		private static void ReadFormatChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.ChannelCount = reader.ReadByte();
			info.SampleRate = Extension.ToUInt32(new byte[] { 0, reader.ReadByte(), reader.ReadByte(), reader.ReadByte() }, 0, Endian.BIG_ENDIAN);
			info.FrameCount = reader.ReadUInt32();
			info.MuteHeader = reader.ReadUInt16();
			info.MuteFooter = reader.ReadUInt16();
			info.SampleCount = info.FrameCount * HCAConstants.SamplesPerFrame - info.MuteHeader - info.MuteFooter;
		}
		private static void ReadCompChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.FrameSize = reader.ReadUInt16();
			info.MinResolution = reader.ReadByte();
			info.MaxResolution = reader.ReadByte();
			info.TrackCount = reader.ReadByte();
			info.ChannelConfig = reader.ReadByte();
			info.TotalBandCount = reader.ReadByte();
			info.BaseBandCount = reader.ReadByte();
			info.StereoBandCount = reader.ReadByte();
			info.BandsPerHfrGroup = reader.ReadByte();
			info.MSStereo = reader.ReadByte();
			reader.ReadSkip(1);
		}
		private static void ReadDecChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.FrameSize = reader.ReadUInt16();
			info.MinResolution = reader.ReadByte();
			info.MaxResolution = reader.ReadByte();
			info.TotalBandCount = reader.ReadByte() + 1U;
			info.BaseBandCount = reader.ReadByte() + 1U;

			var v = reader.ReadByte();
			info.TrackCount = (byte)((v & 0xF0) >> 4);
			info.ChannelConfig = (byte)(v & 0x0F);
			var StereoType = reader.ReadByte();

			if (StereoType == 0)
			{
				info.BaseBandCount = info.TotalBandCount;
				info.StereoBandCount = 0;
			}
			else
			{
				info.StereoBandCount = info.TotalBandCount - info.BaseBandCount;
			}

			info.BandsPerHfrGroup = 0;
		}
		private static void ReadAthChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.UseAthTable = reader.ReadUInt16() == 1;
		}
		private static void ReadLoopChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.Looping = true;
			info.LoopStartFrame = reader.ReadUInt32();
			info.LoopEndFrame = reader.ReadUInt32();
			info.PreLoopSamples = reader.ReadUInt16();
			info.PostLoopSamples = reader.ReadUInt16();
			info.SampleCount = Math.Min(info.SampleCount, info.LoopEndSample);
		}
		private static void ReadCiphChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.EncryptType = reader.ReadUInt16();
		}
		private static void ReadRvaChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.Volume = reader.ReadSingle();
		}
		private static void ReadCommentChunk(EndianBinaryReader reader, ref HCAInfo info)
		{
			reader.ReadSkip(sizeof(uint));
			info.Comment = reader.ReadStringToNull();
		}

		public HCAInfo Info { get; set; } = new HCAInfo();
        public HCAKey Key { get; set; }
        public byte[][] AudioData { get; set; }
		public int Random { get; set; }
		public double[][] Waves { get; private set; }

        private HCAData() { }

		public void Decode()
		{
			Waves = new double[Info.ChannelCount][].InitializeJaggedArray((int)Info.SampleCount);
			var frame = new HCAFrame(this);

            for (int i = 0; i < Info.FrameCount; i++)
            {
				var audio = AudioData[i];
				Key.Decrypt(audio);
				//Console.Write($"Frame#{i}:");
                frame.DecodeFrame(audio, out double[][] output);
				//Console.WriteLine("Complete");
				CopyWaveBuffer(Waves, output, i);
            }
		}

		private void CopyWaveBuffer(double[][] waves, double[][] input, int frame)
		{
			int currentSample = (int)(frame * SamplesPerFrame - Info.MuteHeader);
			int remainingSamples = (int)Math.Min(Info.SampleCount - currentSample, Info.SampleCount);
			int srcStart = Clamp(0 - currentSample, 0, SamplesPerFrame);
			int destStart = Math.Max(currentSample, 0);

			int length = Math.Min(SamplesPerFrame - srcStart, remainingSamples);
			if (length <= 0) return;

			for (int c = 0; c < waves.Length; c++)
			{
				Array.Copy(input[c], srcStart, waves[c], destStart, length);
			}
		}
    }
}
