using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Decoder.HCA
{
	public static class HCAConstants
	{
		public const int SubframesPerFrame = 8;
		public const int SubFrameSamplesBits = 7;
		public const int SamplesPerSubFrame = 1 << SubFrameSamplesBits;
		public const int SamplesPerFrame = SubframesPerFrame * SamplesPerSubFrame;
	}
}
