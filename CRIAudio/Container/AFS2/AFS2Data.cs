using System;
using System.Collections.Generic;

namespace CRIAudio.Container.AFS2 {
    public class AFS2Data {
        public AFS2Info Info { get; set; } = new AFS2Info();
        public List<AFS2Track> Tracks { get; } = new List<AFS2Track>();

        public AFS2Track this[int index] {
            get {
                return Tracks[index];
            }
        }
    }
}