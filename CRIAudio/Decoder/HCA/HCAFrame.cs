using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRIAudio.Utility;
using static CRIAudio.Decoder.HCA.ChannelType;
using static CRIAudio.Decoder.HCA.HCAConstants;

namespace CRIAudio.Decoder.HCA
{
	public class HCAFrame
	{
		private static ChannelType[] GetChannelTypes(HCAInfo info)
		{
			uint channelsPerTrack = info.ChannelCount / info.TrackCount;
			if (info.StereoBandCount == 0 || channelsPerTrack == 1) { return new ChannelType[8]; }

			switch (channelsPerTrack)
			{
				case 2: return new[] { StereoPrimary, StereoSecondary };
				case 3: return new[] { StereoPrimary, StereoSecondary, Discrete };
				case 4 when info.ChannelConfig != 0: return new[] { StereoPrimary, StereoSecondary, Discrete, Discrete };
				case 4 when info.ChannelConfig == 0: return new[] { StereoPrimary, StereoSecondary, StereoPrimary, StereoSecondary };
				case 5 when info.ChannelConfig > 2: return new[] { StereoPrimary, StereoSecondary, Discrete, Discrete, Discrete };
				case 5 when info.ChannelConfig <= 2: return new[] { StereoPrimary, StereoSecondary, Discrete, StereoPrimary, StereoSecondary };
				case 6: return new[] { StereoPrimary, StereoSecondary, Discrete, Discrete, StereoPrimary, StereoSecondary };
				case 7: return new[] { StereoPrimary, StereoSecondary, Discrete, Discrete, StereoPrimary, StereoSecondary, Discrete };
				case 8: return new[] { StereoPrimary, StereoSecondary, Discrete, Discrete, StereoPrimary, StereoSecondary, StereoPrimary, StereoSecondary };
				default: return new ChannelType[channelsPerTrack];
			}
		}
		private static byte[] ScaleAthCurve(int frequency)
		{
			var ath = new byte[SamplesPerSubFrame];

			int acc = 0;
			int i;
			for (i = 0; i < ath.Length; i++)
			{
				acc += frequency;
				int index = acc >> 13;

				if (index >= HCATable.AthTable.Length)
				{
					break;
				}
				ath[i] = HCATable.AthTable[index];
			}

			for (; i < ath.Length; i++)
			{
				ath[i] = 0xff;
			}

			return ath;
		}

		HCAInfo info;
		HCAChannel[] channels;
		byte[] athCurve;
		int acceptableNoiseLevel;
		int evaluationBoundary;

		public HCAFrame(HCAInfo info)
		{
			this.info = info;
			channels = new HCAChannel[info.ChannelCount];
			var types = GetChannelTypes(info);

			for (int i = 0; i < channels.Length; i++)
            {
                channels[i] = new HCAChannel
                {
                    Type = types[i],
                    CodedCount = types[i] == StereoSecondary
                        ? info.BaseBandCount
                        : info.BaseBandCount + info.StereoBandCount
                };
            }

			athCurve = info.UseAthTable ? ScaleAthCurve((int)info.SampleRate) : new byte[SamplesPerSubFrame].FillArray<byte>(0);
		}

		public void DecodeFrame(byte[] bin, out double[,] output)
		{
			var reader = new BitReader(bin);

			UnpackFrame(reader);
			output = new double[8,128];
		}

		private bool UnpackFrame(BitReader reader) {
			var sync = reader.GetUInt16();
			if (sync != 0xffff) {

			}

			acceptableNoiseLevel = (int)reader.GetBit(9);
			evaluationBoundary = (int)reader.GetBit(7);

			foreach(var channel in channels) {
				channel.UnpackScale(reader, info.HfrGroupCount, info.Version);
			}

			return true;
		}
	}
}
