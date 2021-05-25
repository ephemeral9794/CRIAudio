using CRIAudio.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Decoder.HCA
{
	public static class HCADecoder
	{
		public static HCAData ReadData(byte[] bin) => ReadData(new EndianBinaryReader(bin, Endian.LITTLE_ENDIAN));
		public static HCAData ReadData(EndianBinaryReader reader)
		{
			var data = new HCAData();

			return data;
		}
	}
}
