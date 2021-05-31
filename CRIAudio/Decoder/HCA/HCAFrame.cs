using System;
using System.Collections.Generic;
using System.IO;
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
		HCAData data;
		HCAChannel[] channels;
		byte[] athCurve;
		int acceptableNoiseLevel;
		int evaluationBoundary;

		public HCAFrame(HCAData data)
		{
			this.info = data.Info;
			this.data = data;
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
		    output = new double[8,128];

			//UnpackFrame(reader);
			var sync = reader.GetInt16();
			if (sync != 0xffff)
			{
				throw new InvalidDataException("");
			}

			acceptableNoiseLevel = reader.GetBit(9);
			evaluationBoundary = reader.GetBit(7);
			var packedNoiseLevel = (acceptableNoiseLevel << 8) - evaluationBoundary;

			foreach (var channel in channels)
			{
				channel.UnpackScaleFactors(reader, info.HfrGroupCount, info.Version);
				channel.UnpackIntensity(reader, info.HfrGroupCount, info.Version);

				channel.CalculateResolution(packedNoiseLevel, athCurve, info.MinResolution, info.MaxResolution);
				channel.CalculateGain();
			}

			for(var i = 0; i < channels.Length; i++) 
			{
				Log.WriteLine($"Channel #{i}");
				Log.WriteLine("Type        :" + channels[i].Type);
				Log.WriteLine("CodedCount  :" + channels[i].CodedCount);
				Log.WriteLine("NoiseCount  :" + channels[i].NoiseCount);
				Log.WriteLine("ValidCount  :" + channels[i].ValidCount);
				Log.WriteLine("ScaleFactors:" + channels[i].ScaleFactors.ToString(toStr: (n) => { return string.Format("{0,0:X2}", n); }));
				Log.WriteLine("Intensity   :" + channels[i].Intensity.ToString(toStr: (n) => { return string.Format("{0,0:X2}", n); }));
				Log.WriteLine("Resolution  :" + channels[i].Resolution.ToString(toStr: (n) => { return string.Format("{0,0:X2}", n); }));
				Log.WriteLine("Noises      :" + channels[i].Noises.ToString<uint>());
				Log.WriteLine("Gain        :" + channels[i].Gain.ToString<double>());
			}

			for (var i = 0; i < SamplesPerFrame; i++)
			{
				foreach (var channel in channels)
				{
					channel.DequantizeCoefficient(reader);
				}

				foreach (var channel in channels)
				{
					var random = data.Random;
					channel.ReconstructNoise(info.MinResolution, info.MSStereo, ref random);
					data.Random = random;
					channel.ReconstructHighFrequency(info);
				}

				/* restore missing joint stereo bands */
				if (info.StereoBandCount > 0)
				{
					for (var ch = 0; ch < channels.Length - 1; ch++)
					{
						ApplyIntensityStereo(ch, i);
						ApplyMSStereo(ch);
					}
				}

				foreach (var channel in channels) {
					channel.RunIMDCT(i);
				}

				for (var n = 0; n < channels.Length; n++)
				{
					Log.WriteLine($"Spectra#{i}({n}):" + channels[n].Spectra.ToString<double>());
					//Log.WriteLine($"Wave#{i}({n})   :" + channels[n].Wave.ToString<double>());
				}
			}
			
		}

		private void ApplyIntensityStereo(int ch, int subframe)
		{
			if (channels[ch].Type != StereoPrimary) 
				return;

			for (int sf = 0; sf < SubframesPerFrame; sf++)
			{
				double[] l = channels[ch].Spectra;
				double[] r = channels[ch + 1].Spectra;
				double ratioL = HCATable.IntensityRatioTable[channels[ch + 1].Intensity[sf]];
				double ratioR = ratioL - 2.0;
				for (uint b = info.BaseBandCount; b < info.TotalBandCount; b++)
				{
					r[b] = l[b] * ratioR;
					l[b] *= ratioL;
				}
			}
		}

		private void ApplyMSStereo(int ch)
		{
			if (info.MSStereo == 0) /* added in v3.0 */
				return;
			if (channels[ch].Type != StereoPrimary)
				return;

			const double ratio = 0.70710676908493; /* 0x3F3504F3 */
			double[] sp_l = channels[ch].Spectra;
			double[] sp_r = channels[ch + 1].Spectra;
			for (uint band = info.BaseBandCount; band < info.TotalBandCount; band++)
			{
				double coef_l = (sp_l[band] + sp_r[band]) * ratio;
				double coef_r = (sp_l[band] - sp_r[band]) * ratio;
				sp_l[band] = coef_l;
				sp_r[band] = coef_r;
			}
		}

		/*private bool UnpackFrame(BitReader reader) {
			var sync = reader.GetInt16();
			if (sync != 0xffff) {
				return false;
			}

			acceptableNoiseLevel = reader.GetBit(9);
			evaluationBoundary = reader.GetBit(7);
			var packedNoiseLevel = (acceptableNoiseLevel << 8) - evaluationBoundary;

			foreach (var channel in channels) {
				channel.UnpackScaleFactors(reader, info.HfrGroupCount, info.Version);
				channel.UnpackIntensity(reader, info.HfrGroupCount, info.Version);

				channel.CalculateResolution(packedNoiseLevel, athCurve, info.MinResolution, info.MaxResolution);
				channel.CalculateGain();

				//Console.WriteLine("ScaleFactors:" + channel.ScaleFactors.ToString(toStr: (n) => { return string.Format("{0,0:X2}",n);}));
				//Console.WriteLine("Intensity   :" + channel.Intensity.ToString(toStr: (n) => { return string.Format("{0,0:X2}", n); }));
				//Console.WriteLine("Resolution  :" + channel.Resolution.ToString(toStr: (n) => { return string.Format("{0,0:X2}", n); }));
				//Console.WriteLine("Gain        :" + channel.Gain.ToString<double>());
			}
			
			return true;
		}

		private void RestoreBands()
		{
			
		}*/
	}
}
