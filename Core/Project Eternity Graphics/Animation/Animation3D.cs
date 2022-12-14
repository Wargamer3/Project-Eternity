using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Graphics
{
    public class Animation3D
    {
        public List<BoneTimeline> ListTimeline = new List<BoneTimeline>();

        public string Name;
        public double Duration;

        public bool IsLooping = true;

        public Animation3D Clone()
        {
            Animation3D NewAnimation3D = new Animation3D();

            NewAnimation3D.Name = Name;
            NewAnimation3D.Duration = Duration;
            NewAnimation3D.IsLooping = IsLooping;

            NewAnimation3D.ListTimeline = new List<BoneTimeline>(ListTimeline.Count);
            foreach (BoneTimeline ActiveTimeline in ListTimeline)
            {
                NewAnimation3D.ListTimeline.Add(ActiveTimeline.Clone());
            }

            return NewAnimation3D;
        }
    }
}
