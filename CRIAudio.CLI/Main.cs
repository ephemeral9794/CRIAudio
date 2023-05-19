using System.IO;
using CRIAudio.Container.Wave;
using CRIAudio.Decoder.HCA;
using CRIAudio.Utility;
using CRIAudio.Container.AFS2;
using CRIAudio.Container.UTF;

namespace CRIAudio.CLI
{
    class Program
	{
		static void Main(string[] args)
		{
			Log.Visible = true;
			string file = "";
			if (args.Length != 1) {
				Log.WriteLine("Usage: CRIAudio.CLI.exe [file]");
				return;
            }
            else {
				file = args[0];
			}

			var num = file.IndexOf('.');
			if (num > 0)
			{
				var extension = file.Substring(num + 1, file.Length - (num + 1));
				switch (extension)
				{
					case "awb":
						{	// awb(AFS2) file decode.
                            var bin = File.ReadAllBytes(file);
                            var afs2 = AFS2Reader.ReadData(bin);

                            Log.WriteLine($"AFS2 Ver.{string.Format("{0,0:X4}", afs2.Info.Version)}");
                            Log.WriteLine($"File Count       : {afs2.Info.FileCount}");
							Log.WriteLine();

							for ( int i = 0; i < afs2.Info.FileCount; i++ ) { 

								var hca = HCAData.ReadData(afs2[i].Binary, new HCAKey(0x0000450D608C479F, afs2.Info.SubKey));

								var info = hca.Info;
                                Log.WriteLine($"--- AFS2 Track #{i} ---");
                                Log.WriteLine($"HCA Data Ver.{string.Format("{0,0:X4}", info.Version)}");
								Log.WriteLine($"Data Offset	     : {string.Format("0x{0,0:X8}", info.DataOffset)}");
								Log.WriteLine($"Channel Count	 : {info.ChannelCount}");
								Log.WriteLine($"Sample Rate	     : {info.SampleRate}");
								Log.WriteLine($"Frame Count	     : {info.FrameCount}");
								Log.WriteLine($"Frame Size	     : {info.FrameSize}");
								Log.WriteLine($"Min Resolution   : {info.MinResolution}");
								Log.WriteLine($"Max Resolution   : {info.MaxResolution}");
								Log.WriteLine($"Track Count	     : {info.TrackCount}");
								Log.WriteLine($"Channel Config   : {info.ChannelConfig}");
								Log.WriteLine($"Total Band Count : {info.TotalBandCount}");
								Log.WriteLine($"Base Band Count  : {info.BaseBandCount}");
								Log.WriteLine($"Stereo Band Count: {info.StereoBandCount}");
								Log.WriteLine($"Hfr Band Cound   : {info.HfrBandCount}");
								Log.WriteLine($"Bands/HfrGroup   : {info.BandsPerHfrGroup}");
								Log.WriteLine($"Hfr Group Count  : {info.HfrGroupCount}");
								Log.WriteLine($"MS Stereo		 : {info.MSStereo}");
								Log.WriteLine();

								hca.Decode();

								var wav = WaveWriter.ConvertToInt16(hca.Waves);

								WaveInfo wavInfo = new WaveInfo
								{
									Format = WaveFormat.PCM,
									SampleRate = hca.Info.SampleRate,
									ChannelCount = (ushort)hca.Info.ChannelCount,
									BitsPerSample = 16
								};
								string outfile = string.Format(file.Substring(0, file.Length - 4) + "_{0:00}.wav", i);
								using (var stream = new FileStream(outfile, FileMode.OpenOrCreate, FileAccess.Write))
								{
									WaveWriter.WriteData(new WaveData { Info = wavInfo, AudioData = wav }, stream);
								}
							}
                        }
						break;
					case "acb":
						{   // acb(UTF) file decode.
                            var bin = File.ReadAllBytes(file);
							var utf = UTFData.ReadData(bin);

                            var info = utf.Info;
                            Log.WriteLine($"UTF Data Ver.{string.Format("{0,0:X4}", info.Version)}");
                            Log.WriteLine($"File Size : {string.Format("0x{0,0:X8}", info.FileSize)}");
							Log.WriteLine($"Version : {info.Version}");
                            Log.WriteLine($"Table Name : {info.TableName}");
                            Log.WriteLine($"Table Offset : {string.Format("0x{0,0:X8}", info.TableOffset)}");
                            Log.WriteLine($"String Offset : {string.Format("0x{0,0:X8}", info.StringOffset)}");
                            Log.WriteLine($"Binary Offset : {string.Format("0x{0,0:X8}", info.BinaryOffset)}");
                            Log.WriteLine($"Column Count : {info.ColumnCount}");
                            Log.WriteLine($"Row Width : {info.RowWidth}");
                            Log.WriteLine($"Row Count : {info.RowCount}");
                            Log.WriteLine();

							for (int i = 0;i < utf.Columns.Length;i++)
                            {
                                Log.WriteLine($"Name : {utf.Columns[i].Name}");
                                Log.WriteLine($"\tColumn Type : {utf.Columns[i].ColumnType}");
                                Log.WriteLine($"\tData Type   : {utf.Columns[i].DataType}");
                                Log.WriteLine($"\tData        : {utf.Columns[i].ColumnData}");
                            }
                        }
                        break;
					default:
						break;
				}
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
