﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRIAudio.Utility;

namespace CRIAudio.Decoder.HCA
{
	public static class HCATable
	{
        public readonly static byte[] AthTable = {
            0x78,0x5F,0x56,0x51,0x4E,0x4C,0x4B,0x49,0x48,0x48,0x47,0x46,0x46,0x45,0x45,0x45,
            0x44,0x44,0x44,0x44,0x43,0x43,0x43,0x43,0x43,0x43,0x42,0x42,0x42,0x42,0x42,0x42,
            0x42,0x42,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x40,0x40,0x40,0x40,
            0x40,0x40,0x40,0x40,0x40,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,
            0x3F,0x3F,0x3F,0x3E,0x3E,0x3E,0x3E,0x3E,0x3E,0x3D,0x3D,0x3D,0x3D,0x3D,0x3D,0x3D,
            0x3C,0x3C,0x3C,0x3C,0x3C,0x3C,0x3C,0x3C,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,
            0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,
            0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3B,0x3C,0x3C,0x3C,0x3C,0x3C,0x3C,0x3C,0x3C,
            0x3D,0x3D,0x3D,0x3D,0x3D,0x3D,0x3D,0x3D,0x3E,0x3E,0x3E,0x3E,0x3E,0x3E,0x3E,0x3F,
            0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,0x3F,
            0x3F,0x3F,0x3F,0x3F,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,
            0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x40,0x41,0x41,0x41,0x41,0x41,0x41,0x41,
            0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x41,
            0x41,0x41,0x41,0x41,0x41,0x41,0x41,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,
            0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x42,0x43,0x43,0x43,
            0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x43,0x44,0x44,
            0x44,0x44,0x44,0x44,0x44,0x44,0x44,0x44,0x44,0x44,0x44,0x44,0x45,0x45,0x45,0x45,
            0x45,0x45,0x45,0x45,0x45,0x45,0x45,0x45,0x46,0x46,0x46,0x46,0x46,0x46,0x46,0x46,
            0x46,0x46,0x47,0x47,0x47,0x47,0x47,0x47,0x47,0x47,0x47,0x47,0x48,0x48,0x48,0x48,
            0x48,0x48,0x48,0x48,0x49,0x49,0x49,0x49,0x49,0x49,0x49,0x49,0x4A,0x4A,0x4A,0x4A,
            0x4A,0x4A,0x4A,0x4A,0x4B,0x4B,0x4B,0x4B,0x4B,0x4B,0x4B,0x4C,0x4C,0x4C,0x4C,0x4C,
            0x4C,0x4D,0x4D,0x4D,0x4D,0x4D,0x4D,0x4E,0x4E,0x4E,0x4E,0x4E,0x4E,0x4F,0x4F,0x4F,
            0x4F,0x4F,0x4F,0x50,0x50,0x50,0x50,0x50,0x51,0x51,0x51,0x51,0x51,0x52,0x52,0x52,
            0x52,0x52,0x53,0x53,0x53,0x53,0x54,0x54,0x54,0x54,0x54,0x55,0x55,0x55,0x55,0x56,
            0x56,0x56,0x56,0x57,0x57,0x57,0x57,0x57,0x58,0x58,0x58,0x59,0x59,0x59,0x59,0x5A,
            0x5A,0x5A,0x5A,0x5B,0x5B,0x5B,0x5B,0x5C,0x5C,0x5C,0x5D,0x5D,0x5D,0x5D,0x5E,0x5E,
            0x5E,0x5F,0x5F,0x5F,0x60,0x60,0x60,0x61,0x61,0x61,0x61,0x62,0x62,0x62,0x63,0x63,
            0x63,0x64,0x64,0x64,0x65,0x65,0x66,0x66,0x66,0x67,0x67,0x67,0x68,0x68,0x68,0x69,
            0x69,0x6A,0x6A,0x6A,0x6B,0x6B,0x6B,0x6C,0x6C,0x6D,0x6D,0x6D,0x6E,0x6E,0x6F,0x6F,
            0x70,0x70,0x70,0x71,0x71,0x72,0x72,0x73,0x73,0x73,0x74,0x74,0x75,0x75,0x76,0x76,
            0x77,0x77,0x78,0x78,0x78,0x79,0x79,0x7A,0x7A,0x7B,0x7B,0x7C,0x7C,0x7D,0x7D,0x7E,
            0x7E,0x7F,0x7F,0x80,0x80,0x81,0x81,0x82,0x83,0x83,0x84,0x84,0x85,0x85,0x86,0x86,
            0x87,0x88,0x88,0x89,0x89,0x8A,0x8A,0x8B,0x8C,0x8C,0x8D,0x8D,0x8E,0x8F,0x8F,0x90,
            0x90,0x91,0x92,0x92,0x93,0x94,0x94,0x95,0x95,0x96,0x97,0x97,0x98,0x99,0x99,0x9A,
            0x9B,0x9B,0x9C,0x9D,0x9D,0x9E,0x9F,0xA0,0xA0,0xA1,0xA2,0xA2,0xA3,0xA4,0xA5,0xA5,
            0xA6,0xA7,0xA7,0xA8,0xA9,0xAA,0xAA,0xAB,0xAC,0xAD,0xAE,0xAE,0xAF,0xB0,0xB1,0xB1,
            0xB2,0xB3,0xB4,0xB5,0xB6,0xB6,0xB7,0xB8,0xB9,0xBA,0xBA,0xBB,0xBC,0xBD,0xBE,0xBF,
            0xC0,0xC1,0xC1,0xC2,0xC3,0xC4,0xC5,0xC6,0xC7,0xC8,0xC9,0xC9,0xCA,0xCB,0xCC,0xCD,
            0xCE,0xCF,0xD0,0xD1,0xD2,0xD3,0xD4,0xD5,0xD6,0xD7,0xD8,0xD9,0xDA,0xDB,0xDC,0xDD,
            0xDE,0xDF,0xE0,0xE1,0xE2,0xE3,0xE4,0xE5,0xE6,0xE7,0xE8,0xE9,0xEA,0xEB,0xED,0xEE,
            0xEF,0xF0,0xF1,0xF2,0xF3,0xF4,0xF5,0xF7,0xF8,0xF9,0xFA,0xFB,0xFC,0xFD,0xFF,0xFF,
        };

        public readonly static byte[] ScaleInvertTable = {
            14,14,14,14,14,14,13,13, 13,13,13,13,12,12,12,12,
            12,12,11,11,11,11,11,11, 10,10,10,10,10,10,10, 9,
             9, 9, 9, 9, 9, 8, 8, 8,  8, 8, 8, 7, 6, 6, 5, 4,
             4, 4, 3, 3, 3, 2, 2, 2,  2, 1, 1, 1, 1, 1, 1, 1,
             1, 1,
        };
        public readonly static double[] ScalingTable;
        public readonly static double[] RangeTable;
        public readonly static byte[] MaxBitTable = {
            0,2,3,3,4,4,4,4, 5,6,7,8,9,10,11,12
        };
        public readonly static byte[] ReadBitTable = {
            0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0,
            1,1,2,2,0,0,0,0, 0,0,0,0,0,0,0,0,
            2,2,2,2,2,2,3,3, 0,0,0,0,0,0,0,0,
            2,2,3,3,3,3,3,3, 0,0,0,0,0,0,0,0,
            3,3,3,3,3,3,3,3, 3,3,3,3,3,3,4,4,
            3,3,3,3,3,3,3,3, 3,3,4,4,4,4,4,4,
            3,3,3,3,3,3,4,4, 4,4,4,4,4,4,4,4,
            3,3,4,4,4,4,4,4, 4,4,4,4,4,4,4,4,
        };
        public readonly static byte[][] QuantizedSpectrumBits = {
            new byte[] { 0,0,0,0,0,0,0,0, 0,0,0,0,0,0,0,0, },
            new byte[] { 1,1,2,2,0,0,0,0, 0,0,0,0,0,0,0,0, },
            new byte[] { 2,2,2,2,2,2,3,3, 0,0,0,0,0,0,0,0, },
            new byte[] { 2,2,3,3,3,3,3,3, 0,0,0,0,0,0,0,0, },
            new byte[] { 3,3,3,3,3,3,3,3, 3,3,3,3,3,3,4,4, },
            new byte[] { 3,3,3,3,3,3,3,3, 3,3,4,4,4,4,4,4, },
            new byte[] { 3,3,3,3,3,3,4,4, 4,4,4,4,4,4,4,4, },
            new byte[] { 3,3,4,4,4,4,4,4, 4,4,4,4,4,4,4,4, }
        };
        public readonly static sbyte[][] QuantizedSpectrumValue = {
            new sbyte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
            new sbyte[] {0, 0, 1,-1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,},
            new sbyte[] {0, 0, 1, 1,-1,-1, 2,-2, 0, 0, 0, 0, 0, 0, 0, 0,},
            new sbyte[] {0, 0, 1,-1, 2,-2, 3,-3, 0, 0, 0, 0, 0, 0, 0, 0,},
            new sbyte[] {0, 0, 1, 1,-1,-1, 2, 2,-2,-2, 3, 3,-3,-3, 4,-4,},
            new sbyte[] {0, 0, 1, 1,-1,-1, 2, 2,-2,-2, 3,-3, 4,-4, 5,-5,},
            new sbyte[] {0, 0, 1, 1,-1,-1, 2,-2, 3,-3, 4,-4, 5,-5, 6,-6,},
            new sbyte[] {0, 0, 1,-1, 2,-2, 3,-3, 4,-4, 5,-5, 6,-6, 7,-7,},
        };
        public readonly static double[] ReadValueTable = {
            +0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0, +0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,
            +0.0,+0.0,+1.0,-1.0,+0.0,+0.0,+0.0,+0.0, +0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,
            +0.0,+0.0,+1.0,+1.0,-1.0,-1.0,+2.0,-2.0, +0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,
            +0.0,+0.0,+1.0,-1.0,+2.0,-2.0,+3.0,-3.0, +0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,+0.0,
            +0.0,+0.0,+1.0,+1.0,-1.0,-1.0,+2.0,+2.0, -2.0,-2.0,+3.0,+3.0,-3.0,-3.0,+4.0,-4.0,
            +0.0,+0.0,+1.0,+1.0,-1.0,-1.0,+2.0,+2.0, -2.0,-2.0,+3.0,-3.0,+4.0,-4.0,+5.0,-5.0,
            +0.0,+0.0,+1.0,+1.0,-1.0,-1.0,+2.0,-2.0, +3.0,-3.0,+4.0,-4.0,+5.0,-5.0,+6.0,-6.0,
            +0.0,+0.0,+1.0,-1.0,+2.0,-2.0,+3.0,-3.0, +4.0,-4.0,+5.0,-5.0,+6.0,-6.0,+7.0,-7.0,
        };
        public readonly static double[] ScaleConversionTable;
        public readonly static double[] IntensityRatioTable;
        private readonly static ulong[] MDCTWindow_Hex = {
            0x3F46A09E00000000, 0x3F60307700000000, 0x3F6E18A700000000, 0x3F77724D00000000, 0x3F80950120000000, 0x3F86104000000000, 0x3F8C250980000000, 0x3F9167E2E0000000, 
            0x3F95073240000000, 0x3F98EFF7A0000000, 0x3F9D222200000000, 0x3FA0CEF9A0000000, 0x3FA331F880000000, 0x3FA5BA6B80000000, 0x3FA868C860000000, 0x3FAB3D9820000000, 
            0x3FAE397500000000, 0x3FB0AE83C0000000, 0x3FB2548260000000, 0x3FB40F1680000000, 0x3FB5DEA440000000, 0x3FB7C393C0000000, 0x3FB9BE4F60000000, 0x3FBBCF43A0000000, 
            0x3FBDF6DDA0000000, 0x3FC01AC560000000, 0x3FC145DB40000000, 0x3FC27CE540000000, 0x3FC3C01620000000, 0x3FC50F9E40000000, 0x3FC66BAAA0000000, 0x3FC7D46420000000, 
            0x3FC949EEA0000000, 0x3FCACC67E0000000, 0x3FCC5BE6E0000000, 0x3FCDF87A20000000, 0x3FCFA22700000000, 0x3FD0AC7440000000, 0x3FD18E56E0000000, 0x3FD276AC20000000, 
            0x3FD3655DE0000000, 0x3FD45A4DE0000000, 0x3FD5555560000000, 0x3FD6564440000000, 0x3FD75CE0C0000000, 0x3FD868E6E0000000, 0x3FD97A07A0000000, 0x3FDA8FE8C0000000, 
            0x3FDBAA2500000000, 0x3FDCC84B80000000, 0x3FDDE9DFE0000000, 0x3FDF0E5AE0000000, 0x3FE01A9520000000, 0x3FE0AED940000000, 0x3FE143A760000000, 0x3FE1D8A900000000, 
            0x3FE26D84A0000000, 0x3FE301DE40000000, 0x3FE3955840000000, 0x3FE4279440000000, 0x3FE4B834A0000000, 0x3FE546DCE0000000, 0x3FE5D33300000000, 0x3FE65CE0A0000000, 
            0x3FE6E393C0000000, 0x3FE766FFC0000000, 0x3FE7E6DE40000000, 0x3FE862F000000000, 0x3FE8DAFCC0000000, 0x3FE94ED480000000, 0x3FE9BE4F80000000, 0x3FEA294DE0000000, 
            0x3FEA8FB8A0000000, 0x3FEAF18060000000, 0x3FEB4E9DC0000000, 0x3FEBA710E0000000, 0x3FEBFAE0E0000000, 0x3FEC4A1B40000000, 0x3FEC94D320000000, 0x3FECDB2100000000, 
            0x3FED1D21C0000000, 0x3FED5AF620000000, 0x3FED94C220000000, 0x3FEDCAAC40000000, 0x3FEDFCDCE0000000, 0x3FEE2B7DE0000000, 0x3FEE56BA20000000, 0x3FEE7EBCC0000000, 
            0x3FEEA3B120000000, 0x3FEEC5C260000000, 0x3FEEE51AE0000000, 0x3FEF01E400000000, 0x3FEF1C4680000000, 0x3FEF346980000000, 0x3FEF4A72E0000000, 0x3FEF5E8720000000, 
            0x3FEF70C900000000, 0x3FEF8159C0000000, 0x3FEF905900000000, 0x3FEF9DE4C0000000, 0x3FEFAA1960000000, 0x3FEFB511C0000000, 0x3FEFBEE6E0000000, 0x3FEFC7B0C0000000, 
            0x3FEFCF8540000000, 0x3FEFD67980000000, 0x3FEFDCA0E0000000, 0x3FEFE20D80000000, 0x3FEFE6D060000000, 0x3FEFEAF940000000, 0x3FEFEE96C0000000, 0x3FEFF1B6C0000000, 
            0x3FEFF465C0000000, 0x3FEFF6AF60000000, 0x3FEFF89EC0000000, 0x3FEFFA3DA0000000, 0x3FEFFB95A0000000, 0x3FEFFCAF20000000, 0x3FEFFD9200000000, 0x3FEFFE45C0000000, 
            0x3FEFFED100000000, 0x3FEFFF3A00000000, 0x3FEFFF8640000000, 0x3FEFFFBB40000000, 0x3FEFFFDDA0000000, 0x3FEFFFF1E0000000, 0x3FEFFFFBE0000000, 0x3FEFFFFF80000000, 
        };
        public readonly static double[] MDCTWindow;

        static HCATable()
		{
            ScalingTable = new double[64].Generate(DequantizerScalingFunction);
            RangeTable = new double[16].Generate(QuantizerStepSizeFunction);
            ScaleConversionTable = new double[128].Generate(ScaleConversionTableFunction);
            IntensityRatioTable = new double[15].Generate(IntensityRatioFunction);
            MDCTWindow = new double[MDCTWindow_Hex.Length].Generate(MDCTWindowConvertFunction);
        }
        private static double DequantizerScalingFunction(int x) => Math.Sqrt(128) * Math.Pow(Math.Pow(2, 53.0 / 128), x - 63);
        private static double QuantizerStepSizeFunction(int x) => x == 0 ? 0 : 1 / (((x < 8) ? x : (1 << (x - 4)) - 1) + 0.5);
        private static double ScaleConversionTableFunction(int x) => x > 1 && x < 127 ? Math.Pow(Math.Pow(2, 53.0 / 128), x - 64) : 0;
        private static double IntensityRatioFunction(int x) => (28 - x * 2) / 14.0;
        private static double MDCTWindowConvertFunction(int x) => BitConverter.ToDouble(BitConverter.GetBytes(MDCTWindow_Hex[x]));
        private static T[] GenerateArray<T>(int length, Func<int, T> func)
		{
            var array = new T[length];
            for (var i = 0; i < array.Length; i++)
			{
                array[i] = func(i);
			}
            return array;
		}
        private static T[] Generate<T>(this T[] array, Func<int, T> func)
		{
            for (var i = 0; i < array.Length; i++)
			{
                array[i] = func(i);
			}
            return array;
		}
    }
}
