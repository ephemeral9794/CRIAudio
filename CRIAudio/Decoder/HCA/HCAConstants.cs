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
		
		public const uint VersionV110 = 0x0110;
		public const uint VersionV120 = 0x0120;
		public const uint VersionV130 = 0x0130;
		public const uint VersionV200 = 0x0200;
		public const uint VersionV300 = 0x0300;
	}
}
