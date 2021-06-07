using System;

namespace CRIAudio.Container.AFS2 {
    public struct AFS2Info {
		public byte Version { get; set; }
		public byte SizeLength { get; set; }
		public byte Reserve1 { get; set; }
		public byte Reserve2 { get; set; }
		public uint FileCount { get; set; }
		public ushort Alignment { get; set; }
		public ushort SubKey { get; set; }
    }
}