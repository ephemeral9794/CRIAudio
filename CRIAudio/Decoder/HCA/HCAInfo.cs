using CRIAudio.Utility;

namespace CRIAudio.Decoder.HCA
{
	public struct HCAInfo
    {
        // Header
        public uint Version { get; set; }
        public uint DataOffset { get; set; }

        // Format
        public uint ChannelCount { get; set; }
        public uint SampleRate { get; set; }
        public uint SampleCount { get; set; }
        public uint FrameCount { get; set; }
        public uint MuteHeader { get; set; }
        public uint MuteFooter { get; set; }

        // Comp or Dec
        public uint FrameSize { get; set; }
        public uint MinResolution { get; set; }
        public uint MaxResolution { get; set; }
        public uint TrackCount { get; set; }
        public uint ChannelConfig { get; set; }
        public uint TotalBandCount { get; set; }
        public uint BaseBandCount { get; set; }
        public uint StereoBandCount { get; set; }
        public uint BandsPerHfrGroup { get; set; }
        public uint MSStereo { get; set; }
        public uint HfrBandCount => TotalBandCount - BaseBandCount - StereoBandCount;
        public uint HfrGroupCount => HfrBandCount.DivideByRoundUp(BandsPerHfrGroup);

        // Loop
        public bool Looping { get; set; }
        public uint LoopStartFrame { get; set; }
        public uint LoopEndFrame { get; set; }
        public uint PreLoopSamples { get; set; }
        public uint PostLoopSamples { get; set; }
        public uint LoopStartSample => LoopStartFrame * 1024 + PreLoopSamples - MuteHeader;
        public uint LoopEndSample => (LoopEndFrame + 1) * 1024 - PostLoopSamples - MuteHeader;

        // ATH
        public bool UseAthTable { get; set; }

        // Cipher
        public uint EncryptType { get; set; }

        // RVA
        public float Volume { get; set; }

        // Comment
        public string Comment { get; set; }
        public int CommentLength => Comment?.Length ?? 0;
    }
}
