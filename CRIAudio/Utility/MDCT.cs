using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRIAudio.Utility
{
	public class MDCT
	{
		public int Bits { get; private set; }
		public int Size { get; private set; }
		public double Scale { get; private set; }

        private double[] mdctPrevious;
        private double[] imdctPrevious;
        private double[] imdctWindow;

        private double[] scratchMdct;
        private double[] scratchDct;

		public MDCT(int bits, double[] window, double scale) {
			Bits = bits;
			Size = 1 << bits;
			Scale = scale;

			mdctPrevious = new double[Size];
            imdctPrevious = new double[Size];
            scratchMdct = new double[Size];
            scratchDct = new double[Size];
            imdctWindow = window;
		}

		public void RunIMDCT(double[] input, double[] output) {
			if (input.Length < Size)
            {
                throw new ArgumentException("Input must be as long as the MDCT size.", nameof(input));
            }

            if (output.Length < Size)
            {
                throw new ArgumentException("Output must be as long as the MDCT size.", nameof(output));
            }

            int size = Size;
            int half = size / 2;
            double[] dctOut = scratchMdct;

            DCT4Slow(input, dctOut);

            for (int i = 0; i < half; i++)
            {
                output[i] = imdctWindow[i] * dctOut[i + half] + imdctPrevious[i];
                output[i + half] = imdctWindow[i + half] * -dctOut[size - 1 - i] - imdctPrevious[i + half];
                imdctPrevious[i] = imdctWindow[size - 1 - i] * -dctOut[half - i - 1];
                imdctPrevious[i + half] = imdctWindow[half - i - 1] * dctOut[i];
            }
		}

		private void DCT4Slow(double[] input, double[] output) {
			for (var k = 0; k < Size; k++) {
				var sample = 0.0;
				for (var n = 0; n < Size; n++) {
					var angle = Math.PI / Size * (k + 0.5) * (n + 0.5);
					sample += Math.Cos(angle) * input[n];
				}
				output[k] = sample * Scale;
			}
		}
	}
}
