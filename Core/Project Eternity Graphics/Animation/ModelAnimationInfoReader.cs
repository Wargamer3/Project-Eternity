using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core.Graphics
{
    public class ModelAnimationInfoReader : ContentTypeReader<ModelAnimationInfo>
    {
        protected override ModelAnimationInfo Read(ContentReader input, ModelAnimationInfo existingInstance)
        {
            ModelAnimationInfo NewAnimationInfo = new ModelAnimationInfo();
            NewAnimationInfo.ListBoneIndex = input.ReadObject<List<int>>();
            NewAnimationInfo.ListAnimation = input.ReadObject<List<Animation3D>>();

            return NewAnimationInfo;
        }
    }
}
