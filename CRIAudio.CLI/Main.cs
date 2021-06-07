using System;
using System.IO;
using System.Text;
using CRIAudio.Container.Wave;
using CRIAudio.Decoder.HCA;
using CRIAudio.Utility;
using CRIAudio.Container.AFS2;

namespace CRIAudio.CLI
{
	class Program
	{
		static void Main(string[] args)
		{
			Log.Visible = true;
			string file = "";
			if (args.Length != 1) {
				file = @"C:\Users\Administrator\Desktop\Develop\CRIWARE_ANALYZE\snd_bgm_live_1001_oke_01.awb";
				//file = @"C:\Users\147740\Desktop\Util\snd_bgm_live_1001_oke_01.awb";
			} else {
				file = args[0];
			}
			var bin = File.ReadAllBytes(file);
			var afs2 = AFS2Reader.ReadData(bin);

			var hca = HCAData.ReadData(afs2[0].Binary, new HCAKey(0x0000450D608C479F, afs2.Info.SubKey));

			var info = hca.Info;
			Log.WriteLine($"HCA Data Ver.{string.Format("{0,0:X4}", info.Version)}");
			Log.WriteLine($"Data Offset	  : {string.Format("0x{0,0:X8}", info.DataOffset)}");
			Log.WriteLine($"Channel Count	: {info.ChannelCount}");
			Log.WriteLine($"Sample Rate	  : {info.SampleRate}");
			Log.WriteLine($"Frame Count	  : {info.FrameCount}");
			Log.WriteLine($"Frame Size	   : {info.FrameSize}");
			Log.WriteLine($"Min Resolution   : {info.MinResolution}");
			Log.WriteLine($"Max Resolution   : {info.MaxResolution}");
			Log.WriteLine($"Track Count	  : {info.TrackCount}");
			Log.WriteLine($"Channel Config   : {info.ChannelConfig}");
			Log.WriteLine($"Total Band Count : {info.TotalBandCount}");
			Log.WriteLine($"Base Band Count  : {info.BaseBandCount}");
			Log.WriteLine($"Stereo Band Count: {info.StereoBandCount}");
			Log.WriteLine($"Hfr Band Cound   : {info.HfrBandCount}");
			Log.WriteLine($"Bands/HfrGroup   : {info.BandsPerHfrGroup}");
			Log.WriteLine($"Hfr Group Count  : {info.HfrGroupCount}");
			Log.WriteLine($"MS Stereo		: {info.MSStereo}");
			Log.WriteLine();

			hca.Decode();

			var wav = WaveWriter.ConvertToInt16(hca.Waves);

			WaveInfo wavInfo = new WaveInfo { 
				Format = WaveFormat.PCM,
				SampleRate = hca.Info.SampleRate,
				ChannelCount =  (ushort)hca.Info.ChannelCount,
				BitsPerSample = 16
			};
			string outfile = @"output.wav";
			using (var stream = new FileStream(outfile, FileMode.OpenOrCreate, FileAccess.Write))
			{
				WaveWriter.WriteData(new WaveData { Info = wavInfo, AudioData = wav }, stream);
			}

			//var arrays = ArrayUnpacker.UnpackArrays(ArrayUnpacker.PackedTables);
			//var quantizespectrumbits = (byte[][])arrays[0];
			//var quantizespectrumvalue = (byte[][])arrays[1];
			//var quantizedspectrumbits = (byte[][])arrays[2];
			//var quantizedspectrummaxbits = (byte[])arrays[3];
			//var quantizedspectrumvalue = (sbyte[][])arrays[4];
			//var scaletoresolutioncurve = (byte[])arrays[5];
			//var athcurve = (byte[])arrays[6];
			//var mdctwindow = (double[])arrays[7];
			//var defaultchannelmapping = (byte[])arrays[8];
			//var validchannelmappings = (byte[][])arrays[9];

			/*for (var i = 0; i < arrays.Length; i++)
			{
				var builder = new StringBuilder();
				builder.Append("[");
				foreach (var v in arrays[i])
				{
					if (v.GetType() == typeof(byte[]))
					{
						builder.Append("[");
						foreach (var n in (byte[])v)
						{
							builder.Append($"{n},");
						}
						builder.Append("],");
					}
					else if (v.GetType() == typeof(sbyte[]))
					{
						builder.Append("[");
						foreach (var n in (sbyte[])v)
						{
							builder.Append($"{n},");
						}
						builder.Append("],");
					}
					else
					{
						builder.Append($"{v},");
					}
				}
				builder.Append("]");
				Console.WriteLine($"{i}:{builder.ToString()}");
			}*/
		}

	}
}
