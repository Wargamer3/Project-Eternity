using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.Core
{
    public class BoneTimeline
    {
        public string Name = "";
        public List<KeyFrame3D> ListKeyFrame = new List<KeyFrame3D>();

        private AnimationBone ModelBone = null;

        private int CurrentKeyFrameIndex = 0;
        public KeyFrame3D CurrentKeyFrame;
        public KeyFrame3D NextKeyFrame;

        public void AssignBoneFromBaseModel(AnimationBone ModelBone)
        {
            SetKeyframes();
            SetPosition(0);
            this.ModelBone = ModelBone;
        }

        public void SetPosition(float ElapsedTime)
        {
            List<KeyFrame3D> keyframes = ListKeyFrame;
            if (keyframes.Count == 0)
                return;

            while (ElapsedTime >= NextKeyFrame.Time && CurrentKeyFrameIndex < ListKeyFrame.Count - 2)
            {
                CurrentKeyFrameIndex++;
                SetKeyframes();
            }

            Quaternion Rotation;
            Vector3 Translation;

            if (CurrentKeyFrame == NextKeyFrame)
            {
                Rotation = CurrentKeyFrame.Rotation;
                Translation = CurrentKeyFrame.Translation;
            }
            else
            {
                float Progression = (float)((ElapsedTime - CurrentKeyFrame.Time) / (NextKeyFrame.Time - CurrentKeyFrame.Time));

                Rotation = Quaternion.Slerp(CurrentKeyFrame.Rotation, NextKeyFrame.Rotation, Progression);
                Translation = Vector3.Lerp(CurrentKeyFrame.Translation, NextKeyFrame.Translation, Progression);
            }

            if (ModelBone != null)
            {
                Matrix TransformMatrix = Matrix.CreateFromQuaternion(Rotation);
                TransformMatrix.Translation = Translation;
                ModelBone.SetCompleteTransform(TransformMatrix);
            }
        }

        public BoneTimeline Clone()
        {
            BoneTimeline NewBoneTimeline = new BoneTimeline();

            NewBoneTimeline.Name = Name;
            foreach (KeyFrame3D ActiveKeyFrame in ListKeyFrame)
            {
                NewBoneTimeline.ListKeyFrame.Add(ActiveKeyFrame.Clone());
            }

            return NewBoneTimeline;
        }

        public void Rewind()
        {
            CurrentKeyFrameIndex = 0;
            SetKeyframes();
        }

        private void SetKeyframes()
        {
            CurrentKeyFrame = ListKeyFrame[CurrentKeyFrameIndex];

            if (CurrentKeyFrameIndex == ListKeyFrame.Count - 1)
                NextKeyFrame = CurrentKeyFrame;
            else
                NextKeyFrame = ListKeyFrame[CurrentKeyFrameIndex + 1];
        }
    }
}
