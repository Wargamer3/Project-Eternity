using System;
using System.Collections.Generic;

namespace ProjectEternity.Core.Graphics
{
    public class ModelAnimationInfo
    {
        public List<int> ListBoneIndex = new List<int>();
        
        public List<Animation3D> ListAnimation = new List<Animation3D>();

        public ModelAnimationInfo Clone()
        {
            ModelAnimationInfo NewModelAnimationInfo = new ModelAnimationInfo();

            for (int B = 0; B < ListBoneIndex.Count; ++B)
            {
                NewModelAnimationInfo.ListBoneIndex.Add(ListBoneIndex[B]);
            }

            foreach (Animation3D ActiveAnimation in ListAnimation)
            {
                NewModelAnimationInfo.ListAnimation.Add(ActiveAnimation.Clone());
            }

            return NewModelAnimationInfo;
        }
    }
}
