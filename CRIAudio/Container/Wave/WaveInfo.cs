using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Container.Wave
{
	public enum WaveFormat
	{
		PCM = 1
	}

	public struct WaveInfo
	{
		public WaveFormat Format { get; set; }
		public ushort ChannelCount { get; set; }
		public uint SampleRate { get; set; }
		public uint BytesPerSecond => BlockSize * SampleRate;
		public ushort BlockSize => (ushort)(BitsPerSample / 8 * ChannelCount);
		public ushort BitsPerSample { get; set; }
	}
}
