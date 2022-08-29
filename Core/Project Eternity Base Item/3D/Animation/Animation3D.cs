using System;
using System.Collections.Generic;

namespace ProjectEternity.Core
{
    public class Animation3D
    {
        public List<BoneTimeline> ListTimeline = new List<BoneTimeline>();

        public string Name;
        public double Duration;

        public bool IsLooping = true;
    }
}
