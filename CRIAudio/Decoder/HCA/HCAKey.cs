using System;
using CRIAudio.Utility;

namespace CRIAudio.Decoder.HCA {
    public class HCAKey {
        public enum EncryptType {
            Type0 = 0,
            Type1 = 1,
            Type56 = 56
        }
        public byte[] Table { get; } = new byte[256];
        public ulong Key { get; }
        public ushort Subkey { get; }
        public EncryptType Type { get; }

        public HCAKey(ulong key, ushort subkey = 0) {
            if (subkey != 0)
			{
                key *= ((ulong)subkey << 16) | ((ushort)~subkey + 2u);
            }
            Subkey = subkey;
            Key = key;
            Type = EncryptType.Type56;
            Table.FillArray<byte>(0);

            Init56();
        }

        public HCAKey(uint type) {
            Table.FillArray<byte>(0);
            Key = 0;
            Subkey = 0;
            switch (type) {
                case 0:
                    Type = EncryptType.Type0;
                    Init0(); 
                    break;
                case 1:
                    Type = EncryptType.Type1;
                    Init1(); 
                    break;
                default:
                    throw new ArgumentOutOfRangeException("");
            }
        }

        public void Decrypt(byte[] data) {
            for (var i = 0; i < data.Length; i++)
			{
				data[i] = Table[data[i]];
			}
        }

        private void Init0() {
            for (var i = 0; i < 256; i++) {
                Table[i] = (byte)i;
            }
        }

        private void Init1() {
            int xor = 0;
			const int mult = 13;
			const int inc = 11;
			int outPos = 1;
			Table.FillArray<byte>(0);

			for (int i = 0; i < 256; i++)
			{
				xor = (xor * mult + inc) % 256;
				if (xor != 0 && xor != 0xff)
				{
					Table[outPos++] = (byte)xor;
				}
			}

			Table[0xff] = 0xff;
        }

        private void Init56() {
            var kc = BitConverter.GetBytes(Key - 1);
            var seed = new byte[16];
			var base_r = new byte[16];
			var base_c = new byte[16];
			var base_ = new byte[256];

            seed[0] = kc[1];
            seed[1] = (byte)(kc[6] ^ kc[1]);
            seed[2] = (byte)(kc[2] ^ kc[3]);
            seed[3] = kc[2];
            seed[4] = (byte)(kc[1] ^ kc[2]);
            seed[5] = (byte)(kc[3] ^ kc[4]);
            seed[6] = kc[3];
            seed[7] = (byte)(kc[2] ^ kc[3]);
            seed[8] = (byte)(kc[4] ^ kc[5]);
            seed[9] = kc[4];
            seed[10] = (byte)(kc[3] ^ kc[4]);
            seed[11] = (byte)(kc[5] ^ kc[6]);
            seed[12] = kc[5];
            seed[13] = (byte)(kc[4] ^ kc[5]);
            seed[14] = (byte)(kc[6] ^ kc[1]);
            seed[15] = kc[6];

            /* init base table */
			CreateTable(base_r, kc[0]);
			for (int r = 0; r < 16; r++)
			{
				byte nb;
				CreateTable(base_c, seed[r]);
				nb = (byte)(base_r[r] << 4);
				for (int c = 0; c < 16; c++)
				{
					base_[r * 16 + c] = (byte)(nb | base_c[c]); /* combine nibbles */
				}
			}

			/* final shuffle table */
			uint x = 0;
			uint pos = 1;

			for (int i = 0; i < 256; i++)
			{
				x = (x + 17) & 0xFF;
				if (base_[x] != 0 && base_[x] != 0xFF)
					Table[pos++] = base_[x];
			}
			Table[0] = 0;
			Table[0xFF] = 0xFF;
        }

		private void CreateTable(byte[] r, byte key)
		{
			int mul = ((key & 1) << 3) | 5;
			int add = (key & 0xE) | 1;
			uint i;

			key >>= 4;
			for (i = 0; i < 16; i++)
			{
				key = (byte)((key * mul + add) & 0xF);
				r[i] = key;
			}
		}
    }
}