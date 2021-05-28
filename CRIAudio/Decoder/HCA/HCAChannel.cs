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
		public uint[] ScaleFactors { get; private set; } = new uint[SamplesPerSubFrame];

		public bool UnpackScale(BitReader reader, uint hfrGroupCount, uint version) {
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

			uint deltaBit = reader.GetBit(3);
			if (deltaBit == 0) {
				ScaleFactors.FillArray(0U);
			} else if (deltaBit >= 6) {
				for (var i = 0; i < count; i++) {
					ScaleFactors[i] = reader.GetBit(6);
				}
			} else {
				var expectDelta = (1 << (int)deltaBit) - 1;
				var value = reader.GetBit(6);
				ScaleFactors[0] = value;

				for (var i = 0; i < count; i++) {
					var delta = reader.GetBit((int)deltaBit);
					
				}	
			}
			
			return true;
		}
	}
}
