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
	public enum ChannelType
	{
		Discrete = 0,
		StereoPrimary = 1,
		StereoSecondary = 2
	}

	public class HCAChannel
	{
		public ChannelType Type { get; set; }
		public uint CodedCount { get; set; } 
		public uint[] ScaleFactors { get; private set; } = new uint[SamplesPerSubFrame].FillArray(0U);
		public uint[] Intensity { get; private set; } = new uint[SamplesPerFrame].FillArray(0U);
		public uint[] HfrScale { get; private set; } = new uint[SamplesPerFrame].FillArray(0U);

		public int[] Resolution { get; private set; } = new int[SamplesPerSubFrame].FillArray(0);
		public uint[] Noises { get; private set; } = new uint[SamplesPerSubFrame].FillArray(0U);

		public int NoiseCount { get; private set; }
		public int ValidCount { get; private set; }

		public double[] Gain { get; private set; } = new double[SamplesPerSubFrame].FillArray(0.0);
		public double[] QuantizedSpectra { get; private set; } = new double[SamplesPerSubFrame].FillArray(0.0);
		public double[] Spectra { get; private set; } = new double[SamplesPerSubFrame].FillArray(0.0);

		public bool UnpackScaleFactors(BitReader reader, uint hfrGroupCount, uint version) {
			uint count = CodedCount;
			uint excount = 0;

			if (Type == StereoSecondary || hfrGroupCount <= 0 || version <= 0x0200) {
				excount = 0;
			}
			else {
				excount = hfrGroupCount;
				count = count + excount;

				/* just in case */
				if (count > SamplesPerSubFrame)
					return false;
			}

			var deltaBit = reader.GetBit(3);
			if (deltaBit == 0) {
				ScaleFactors.FillArray(0U);
			} else if (deltaBit >= 6) {
				for (var i = 0; i < count; i++) {
					ScaleFactors[i] = (uint)reader.GetBit(6);
				}
			} else {
				var expectDelta = 1 << (deltaBit - 1);
				var value = (uint)reader.GetBit(6);
				ScaleFactors[0] = value;

				for (var i = 1; i < count; i++) {
					var delta = reader.GetBit(deltaBit);
					delta -= (1 << deltaBit - 1) - 1;
					if (delta < expectDelta)
					{
						value = (uint)(value + delta);
						if (value < 0 || value >= 64)
						{
							return false;
						}
						value &= 0x3F;
					} else
					{
						value = (uint)reader.GetBit(6);
					}
					ScaleFactors[i] = value;
				}	
			}
			
			return true;
		}

		public bool UnpackIntensity(BitReader reader, uint hfrGroupCount, uint version)
		{
			if (Type == StereoSecondary)
			{
				if (version <= VersionV200)
				{
					var value = (byte)reader.CheckBit(4);
					Intensity[0] = value;
					if (value < 15)
					{
						reader.AddBit(4);
						for (var i = 1; i < SamplesPerFrame; i++)
						{
							Intensity[i] = (byte)reader.GetBit(4);
						}
					}
				} else
				{
					uint value = (byte)reader.CheckBit(4);
					int deltaBits;

					if (value < 15)
					{
						reader.AddBit(4);

						deltaBits = reader.GetBit(2); /* +1 */

						Intensity[0] = value;
						if (deltaBits == 3)
						{ /* 3+1 = 4b */
							/* fixed intensities */
							for (var i = 1; i < SamplesPerFrame; i++)
							{
								Intensity[i] = (byte)reader.GetBit(4);
							}
						}
						else
						{
							/* delta intensities */
							int bmax = (2 << deltaBits) - 1;
							int bits = deltaBits + 1;

							for (var i = 1; i < SamplesPerFrame; i++)
							{
								int delta = reader.GetBit(bits);
								if (delta == bmax)
								{
									value = (byte)reader.GetBit(4); /* encoded */
								}
								else
								{
									value = (uint)(value - (bmax >> 1) + delta); /* differential */
									if (value > 15) //todo check
										return false; /* not done in lib */
								}

								Intensity[i] = value;
							}
						}
					}
					else
					{
						reader.AddBit(4);
						for (var i = 0; i < SamplesPerFrame; i++)
						{
							Intensity[i] = 7;
						}
					}
				}
			} else
			{
				if (version <= VersionV200)
				{
					/* pointer in v2.0 lib for v2.0 files is base+stereo bands, while v3.0 lib for v2.0 files
					 * is last HFR. No output difference but v3.0 files need that to handle HFR */
					if (hfrGroupCount > 0)
					{
						for (int i = 0; i < hfrGroupCount; i++)
						{
							HfrScale[i] = (byte)reader.GetBit(6);
						}
					}
				}
			}

			return true;
		}

		public void CalculateResolution(int packedNoiseLevel, byte[] athCurve, uint minResolution, uint maxResolution)
		{
			var count = CodedCount;
			for (var i = 0; i < count; i++)
			{
				int newResolution = 0;
				int scaleFactor = (byte)ScaleFactors[i];

				/* curve values are 0 in v1.2>= so ath_curve is actually removed in CRI's code */
				int noiseLevel = athCurve[i] + ((packedNoiseLevel + i) >> 8);
				int curve_position = noiseLevel + 1 - ((5 * scaleFactor) >> 1);

				/* v2.0<= allows max 56 + sets rest to 1, while v3.0 table has 1 for 57..65 and
				 * clamps to min_resolution below, so v2.0 files are still supported */
				if (curve_position < 0)
				{
					newResolution = 15;
				}
				else if (curve_position <= 65)
				{
					newResolution = HCATable.ScaleInvertTable[curve_position];
				}
				else
				{
					newResolution = 0;
				}

				/* added in v3.0 (before, min_resolution was always 1) */
				if (newResolution > maxResolution)
					newResolution = (int)maxResolution;
				else if (newResolution < minResolution)
					newResolution = (int)minResolution;

				/* save resolution 0 (not encoded) indexes (from 0..N), and regular indexes (from N..0) */
				if (newResolution < 1)
				{
					Noises[NoiseCount] = (uint)i;
					NoiseCount++;
				}
				else
				{
					Noises[SamplesPerSubFrame - 1 - ValidCount] = (uint)i;
					ValidCount++;
				}

				Resolution[i] = newResolution;
			}


		}

		public void CalculateGain()
		{
			uint count = CodedCount;

			for (var i = 0; i < count; i++)
			{
				var scalefactor_scale = HCATable.ScalingTable[ScaleFactors[i]];
				var resolution_scale = HCATable.RangeTable[Resolution[i]];
				Gain[i] = scalefactor_scale * resolution_scale;
			}
		}

		public void DequantizeCoefficient(BitReader reader)
		{
			var count = CodedCount;

			for (var i = 0; i < count; i++)
			{
				var resolution = Resolution[i];
				int bits = HCATable.MaxBitTable[resolution];
				var code = reader.CheckBit(bits);
				double qc = 0.0;

				if (resolution < 8)
				{
					bits = HCATable.QuantizedSpectrumBits[resolution][code];
					qc = HCATable.QuantizedSpectrumValue[resolution][code];
				}
				else
				{
					/* parse values in sign-magnitude form (lowest bit = sign) */
					int quantizedCoefficient = code / 2 * (1 - (code % 2 * 2));
					if (quantizedCoefficient == 0)
					{
						bits--;
					}
					qc = quantizedCoefficient;
				}
				reader.AddBit(bits);
				Spectra[i] = qc * Gain[i];
			}
			Array.Clear(Spectra, (int)count, (int)(SamplesPerSubFrame - count));
		}

		public void ReconstructNoise(uint minResolution, uint msStereo, ref int random)
		{
			if (minResolution > 0) /* added in v3.0 */
				return;
			if (ValidCount <= 0 || NoiseCount <= 0)
				return;
			if (!(msStereo != 0 || Type == StereoPrimary))
				return;

			int rnd = random;
			for (var i = 0; i < NoiseCount; i++)
			{
				random = 0x343FD * random + 0x269EC3; /* typical rand() */

				/* can't go over 128 */
				var random_index = SamplesPerSubFrame - ValidCount + (((random & 0x7FFF) * ValidCount) >> 15); 

				/* points to next resolution 0 index and random non-resolution 0 index */
				var noise_index = Noises[i];
				var valid_index = Noises[random_index];

				/* get final scale index */
				var sf_noise = ScaleFactors[noise_index];
				var sf_valid = ScaleFactors[valid_index];
				var sc_index = (sf_noise - sf_valid + 62) & ~((sf_noise - sf_valid + 62) >> 31);

				Spectra[noise_index] = HCATable.ScaleConversionTable[sc_index] * Spectra[valid_index];
			}
		}

		public void ReconstructHighFrequency(HCAInfo info)
		{
			if (info.BandsPerHfrGroup == 0) /* added in v2.0, skipped in v2.0 files with 0 bands too */
				return;
			if (Type == StereoSecondary)
				return;

			uint group_limit;
			uint start_band = info.StereoBandCount + info.BaseBandCount;
			uint highband = start_band;
			uint lowband = start_band - 1;
			
			if (info.Version <= VersionV200)
			{
				group_limit = info.HfrGroupCount;
			}
			else
			{
				group_limit = (info.HfrGroupCount >= 0) ? info.HfrGroupCount : info.HfrGroupCount + 1;
				group_limit = group_limit >> 1;
			}

			for (var group = 0; group < info.HfrGroupCount; group++)
			{
				uint lowband_sub = (group < group_limit) ? 1 : 0; // move lowband towards 0 until group reachs limit 
				for (var i = 0; i < info.BandsPerHfrGroup; i++)
				{
					if (highband >= info.TotalBandCount || lowband < 0)
						break;

					var sc_index = HfrScale[group] - ScaleFactors[lowband] + 63;
					sc_index = sc_index & ~(sc_index >> 31); // clamped in v3.0 lib (in theory 6b sf are 0..128)

					Spectra[highband] = HCATable.ScaleConversionTable[sc_index] * Spectra[lowband];

					highband += 1;
					lowband -= lowband_sub;
				}
			}

			// last spectrum coefficient is 0 (normally highband = 128, but perhaps could 'break' before) 
			Spectra[highband - 1] = 0.0f;
		}

	}
}
