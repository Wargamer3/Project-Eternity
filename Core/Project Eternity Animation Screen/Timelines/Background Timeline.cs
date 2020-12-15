using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class BackgroundTimeline : FixedTimeline
    {
        public class BackgroundKeyFrame : AnimationObjectKeyFrame
        {
            private float _SpeedX;
            private float _SpeedY;
            private float _SpeedZ;
            private float _SpeedYaw;
            private float _SpeedPitch;
            private float _SpeedRoll;

            internal BackgroundKeyFrame()
            {
                _SpeedX = 0;
                _SpeedY = 0;
                _SpeedZ = 0;
                _SpeedYaw = 0;
                _SpeedPitch = 0;
                _SpeedRoll = 0;
            }

            public BackgroundKeyFrame(AnimationObjectKeyFrame Copy)
            {
                BackgroundKeyFrame ActiveBackgroundKeyFrame = Copy as BackgroundKeyFrame;
                if (ActiveBackgroundKeyFrame != null)
                {
                    _SpeedX = ActiveBackgroundKeyFrame._SpeedX;
                    _SpeedY = ActiveBackgroundKeyFrame._SpeedY;
                    _SpeedZ = ActiveBackgroundKeyFrame._SpeedZ;
                    _SpeedYaw = ActiveBackgroundKeyFrame._SpeedYaw;
                    _SpeedPitch = ActiveBackgroundKeyFrame._SpeedPitch;
                    _SpeedRoll = ActiveBackgroundKeyFrame._SpeedRoll;
                }
            }

            public BackgroundKeyFrame(BinaryReader BR)
            {
                _SpeedX = BR.ReadSingle();
                _SpeedY = BR.ReadSingle();
                _SpeedZ = BR.ReadSingle();
                _SpeedYaw = BR.ReadSingle();
                _SpeedPitch = BR.ReadSingle();
                _SpeedRoll = BR.ReadSingle();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SpeedX);
                BW.Write(_SpeedY);
                BW.Write(_SpeedZ);
                BW.Write(_SpeedYaw);
                BW.Write(_SpeedPitch);
                BW.Write(_SpeedRoll);
            }

            public override AnimationObjectKeyFrame Copy(AnimationClass.AnimationLayer ActiveLayer)
            {
                BackgroundKeyFrame NewBackgroundKeyFrame = new BackgroundKeyFrame(this);
                return NewBackgroundKeyFrame;
            }

            #region Properties

            [CategoryAttribute("Background Event Attributes"),
            DescriptionAttribute(".")]
            public float SpeedX
            {
                get
                {
                    return _SpeedX;
                }
                set
                {
                    _SpeedX = value;
                }
            }

            [CategoryAttribute("Background Event Attributes"),
            DescriptionAttribute(".")]
            public float SpeedY
            {
                get
                {
                    return _SpeedY;
                }
                set
                {
                    _SpeedY = value;
                }
            }

            [CategoryAttribute("Background Event Attributes"),
            DescriptionAttribute(".")]
            public float SpeedZ
            {
                get
                {
                    return _SpeedZ;
                }
                set
                {
                    _SpeedZ = value;
                }
            }

            [CategoryAttribute("Background Event Attributes"),
            DescriptionAttribute(".")]
            public float SpeedYaw
            {
                get
                {
                    return _SpeedYaw;
                }
                set
                {
                    _SpeedYaw = value;
                }
            }

            [CategoryAttribute("Background Event Attributes"),
            DescriptionAttribute(".")]
            public float SpeedPitch
            {
                get
                {
                    return _SpeedPitch;
                }
                set
                {
                    _SpeedPitch = value;
                }
            }

            [CategoryAttribute("Background Event Attributes"),
            DescriptionAttribute(".")]
            public float SpeedRoll
            {
                get
                {
                    return _SpeedRoll;
                }
                set
                {
                    _SpeedRoll = value;
                }
            }

            #endregion
        }

        public BackgroundTimeline()
            : base("Background", "New Background")
        {
        }

        public BackgroundTimeline(BinaryReader BR)
            : this()
        {
            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            DicAnimationKeyFrame = new Dictionary<int, AnimationObjectKeyFrame>();
            for (int K = 0; K < DicAnimationSpriteKeyFrameCount; K++)
            {
                int Key = BR.ReadInt32();
                DicAnimationKeyFrame.Add(Key, new BackgroundKeyFrame(BR));
            }
        }

        protected override FixedTimeline DoLoadCopy(BinaryReader BR)
        {
            return new BackgroundTimeline(BR);
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            BackgroundTimeline NewSetBackgroundEvent = new BackgroundTimeline();

            NewSetBackgroundEvent.UpdateFrom(this, ActiveLayer);

            return NewSetBackgroundEvent;
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            BackgroundKeyFrame ActiveBackgroundKeyFrame = (BackgroundKeyFrame)DicAnimationKeyFrame[KeyFrame];

            if (ActiveAnimation.ActiveAnimationBackground != null)
            {
                ActiveAnimation.ActiveAnimationBackground.MoveSpeed = new Vector3(ActiveBackgroundKeyFrame.SpeedX, ActiveBackgroundKeyFrame.SpeedY, ActiveBackgroundKeyFrame.SpeedZ);
            }
        }

        protected override AnimationObjectKeyFrame CreateFirstKeyFrame()
        {
            return new BackgroundKeyFrame();
        }
    }
}
