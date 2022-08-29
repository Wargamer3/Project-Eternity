using System;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.Core
{
    public class AnimationClipReader : ContentTypeReader<Animation3D>
    {
        protected override Animation3D Read(ContentReader input, Animation3D existingInstance)
        {
            Animation3D NewAnimation = new Animation3D();
            NewAnimation.Name = input.ReadString();
            NewAnimation.Duration = input.ReadDouble();

            int ListTimelineCount = input.ReadInt32();
            for (int T = 0; T < ListTimelineCount; T++)
            {
                BoneTimeline NewTimeline = new BoneTimeline();
                NewAnimation.ListTimeline.Add(NewTimeline);

                NewTimeline.Name = input.ReadString();

                int ListKeyFrameCount = input.ReadInt32();

                for (int K = 0; K < ListKeyFrameCount; K++)
                {
                    KeyFrame3D NewKeyFrame = new KeyFrame3D();
                    NewKeyFrame.Time = input.ReadDouble();
                    NewKeyFrame.Rotation = input.ReadQuaternion();
                    NewKeyFrame.Translation = input.ReadVector3();

                    NewTimeline.ListKeyFrame.Add(NewKeyFrame);
                }
            }

            return NewAnimation;
        }
    }
}
