using System;

namespace CRIAudio.Container.AFS2 {
    public class AFS2Track {
        	public ushort ID { get; set; }
			public uint Offset { get; set; }
			public uint Size { get; set; }
			public uint Signature { get; set; }
			public byte[] Binary { get; set; }
    }
}