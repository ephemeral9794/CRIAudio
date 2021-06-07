using CRIAudio.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Container.Wave
{
	public enum QuantizeBit
	{
		PCM_8BIT = 8,
		PCM_16BIT = 16,
		PCM_24BIT = 24,
		PCM_FLOAT = 32,
	}

	public static class WaveWriter
	{
		public static void WriteData(WaveData data, Stream stream)
		{
			using (var writer = new EndianBinaryWriter(stream, Encoding.ASCII, Endian.LITTLE_ENDIAN))
			{
				writer.Write("RIFF".GetBytes());
				writer.Write(data.AudioData.Length + 36);
				writer.Write("WAVE".GetBytes());
				writer.Write("fmt ".GetBytes());
				writer.Write(16);
				writer.Write((short)data.Info.Format);
				writer.Write(data.Info.ChannelCount);
				writer.Write(data.Info.SampleRate);
				writer.Write(data.Info.BytesPerSecond);
				writer.Write(data.Info.BlockSize);
				writer.Write(data.Info.BitsPerSample);
				writer.Write("data".GetBytes());
				writer.Write(data.AudioData.Length);
				writer.Write(data.AudioData);
			}
		}
		public static byte[] ConvertToInt16(double[][] wav)
		{
			var channelCount = wav.Length;
			var chWave = new short[channelCount][];
			for (var ch = 0; ch < channelCount; ch++)
			{
				chWave[ch] = new short[wav[ch].Length];
				for (var i = 0; i < wav[ch].Length; i++)
				{
					var w = wav[ch][i];
					if (w > 1.0) w = 1.0;
					if (w < -1.0) w = -1.0;
					chWave[ch][i] = (short)(w * short.MaxValue);
				}
			}
			var output = new byte[chWave[0].Length * channelCount * sizeof(short)];
			for (var i = 0; i < chWave[0].Length; i++)
			{
				for (var ch = 0; ch < channelCount; ch++) {
					var bin = BitConverter.GetBytes(chWave[ch][i]);
					var now = i * channelCount + ch;
					for (var n = 0; n < sizeof(short); n++)
					{
						output[now * sizeof(short) + n] = bin[n];
					}
				}
			}
			return output;
		}
		/*public static byte[] ConvertToInt16(double[] wav)
		{
			var sWave = new short[wav.Length];
			for (var i = 0; i < wav.Length; i++) { 
					var w = wav[i];
					if (w > 1.0) w = 1.0;
					if (w < -1.0) w = -1.0;
				sWave[i] = (short)(w * short.MaxValue);
			}
			var bytes = new byte[sWave.Length * sizeof(short)];
			for (var i = 0; i < sWave.Length; i++)
			{
				var bin = BitConverter.GetBytes(sWave[i]);
				for (var n = 0; n < sizeof(short); n++)
				{
					bytes[i * sizeof(short) + n] = bin[n];
				}
			}
			return bytes;
		}*/
	}
}
