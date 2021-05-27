using System;
using System.IO;
using System.Text;
using CRIAudio.Decoder.HCA;

namespace CRIAudio.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = @"C:\Users\Administrator\Desktop\Develop\CRIWARE_ANALYZE\snd_bgm_live_1001_oke_01.hca";
            var bin = File.ReadAllBytes(file);
            var hca = HCAData.ReadData(bin, new HCAKey(0x0000450D608C479F, 0x5CDE));

			var info = hca.Info;
			Console.WriteLine($"HCA Data Ver.{string.Format("{0,0:X4}", info.Version)}");
			Console.WriteLine($"Data Offset      : {string.Format("0x{0,0:X8}", info.DataOffset)}");
			Console.WriteLine($"Channel Count    : {info.ChannelCount}");
			Console.WriteLine($"Sample Rate      : {info.SampleRate}");
			Console.WriteLine($"Frame Count      : {info.FrameCount}");
			Console.WriteLine($"Frame Size       : {info.FrameSize}");
			Console.WriteLine($"Min Resolution   : {info.MinResolution}");
			Console.WriteLine($"Max Resolution   : {info.MaxResolution}");
			Console.WriteLine($"Track Count      : {info.TrackCount}");
			Console.WriteLine($"Channel Config   : {info.ChannelConfig}");
			Console.WriteLine($"Total Band Count : {info.TotalBandCount}");
			Console.WriteLine($"Base Band Count  : {info.BaseBandCount}");
			Console.WriteLine($"Stereo Band Count: {info.StereoBandCount}");
			Console.WriteLine($"Hfr Band Cound   : {info.HfrBandCount}");
			Console.WriteLine($"Bands/HfrGroup   : {info.BandsPerHfrGroup}");
			Console.WriteLine($"Hfr Group Count  : {info.HfrGroupCount}");
			Console.WriteLine($"MS Stereo        : {info.MSStereo}");
			Console.WriteLine();

			var arrays = ArrayUnpacker.UnpackArrays(ArrayUnpacker.PackedTables);
			for (var i = 0; i < arrays.Length; i++)
			{
				var builder = new StringBuilder();
				builder.Append("[");
				foreach (var v in arrays[i])
				{
					builder.Append($"{v},");
				}
				builder.Append("]");
				Console.WriteLine($"{i}:{builder.ToString()}");
			}
		}
    }
}
