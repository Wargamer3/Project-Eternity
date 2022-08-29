using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using ProjectEternity.Core;

namespace ProjectEternity.ContentProcessor
{
    [ContentTypeWriter]
    public class ModelAnimationInfoWriter : ContentTypeWriter<ModelAnimationInfo>
    {
        protected override void Write(ContentWriter output, ModelAnimationInfo AnimationInfo)
        {
            output.WriteObject(AnimationInfo.ListBoneIndex);
            output.WriteObject(AnimationInfo.ListAnimation);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ModelAnimationInfoReader).AssemblyQualifiedName;
        }
    }
}
