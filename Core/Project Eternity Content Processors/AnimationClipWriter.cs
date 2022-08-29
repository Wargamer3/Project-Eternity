using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using ProjectEternity.Core;

namespace ProjectEternity.ContentProcessor
{
    [ContentTypeWriter]
    public class AnimationClipWriter : ContentTypeWriter<Animation3D>
    {
        protected override void Write(ContentWriter output, Animation3D ActiveAnimation)
        {
            output.Write(ActiveAnimation.Name);
            output.Write(ActiveAnimation.Duration);

            output.Write(ActiveAnimation.ListTimeline.Count);
            foreach (BoneTimeline ActiveTimeline in ActiveAnimation.ListTimeline)
            {
                output.Write(ActiveTimeline.Name);

                output.Write(ActiveTimeline.ListKeyFrame.Count);
                foreach (KeyFrame3D ActiveKeyFrame in ActiveTimeline.ListKeyFrame)
                {
                    output.Write(ActiveKeyFrame.Time);
                    output.Write(ActiveKeyFrame.Rotation);
                    output.Write(ActiveKeyFrame.Translation);
                }
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(AnimationClipReader).AssemblyQualifiedName;
        }
    }
}
