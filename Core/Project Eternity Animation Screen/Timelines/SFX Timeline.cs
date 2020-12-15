using System.IO;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using FMOD;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class SFXTimeline : FixedTimeline
    {
        public class SFXKeyFrame : AnimationObjectKeyFrame
        {
            private SFX _SFX;

            internal SFXKeyFrame()
            {
                _SFX = new SFX();
            }

            public SFXKeyFrame(AnimationObjectKeyFrame Copy)
            {
                _SFX = new SFX();
            }

            public SFXKeyFrame(BinaryReader BR)
            {
                string Path = BR.ReadString();
                bool Loop = BR.ReadBoolean();
                float Volume = BR.ReadSingle();

                _SFX = new SFX(Path, Loop);
                _SFX.Volume = Volume;
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SFX.SFXPath);
                BW.Write(_SFX.Loop);
                BW.Write(_SFX.Volume);
            }

            public override AnimationObjectKeyFrame Copy(AnimationClass.AnimationLayer ActiveLayer)
            {
                SFXKeyFrame NewSFXKeyFrame = new SFXKeyFrame(this);
                return NewSFXKeyFrame;
            }

            #region Properties

            [Editor(typeof(SFXSelector), typeof(UITypeEditor)),
            CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public SFX SFX
            {
                get
                {
                    return _SFX;
                }
                set
                {
                    _SFX = value;
                }
            }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public bool Loop { get { return _SFX.Loop; } set { _SFX.Loop = value; } }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute(".")]
            public float Volume { get { return _SFX.Volume; } set { _SFX.Volume = value; } }

            [CategoryAttribute("Spawner Attributes"),
            DescriptionAttribute("Use -1 to play the full song.")]
            public int Length { get { return _SFX.Length; } set { _SFX.Length = value; } }

            #endregion
        }

        public SFXTimeline()
            : base("SFX", "SFX Timeline")
        {
        }

        public SFXTimeline(BinaryReader BR)
            : this()
        {
            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            DicAnimationKeyFrame = new Dictionary<int, AnimationObjectKeyFrame>();
            for (int K = 0; K < DicAnimationSpriteKeyFrameCount; K++)
            {
                int Key = BR.ReadInt32();
                DicAnimationKeyFrame.Add(Key, new SFXKeyFrame(BR));
            }
        }

        protected override FixedTimeline DoLoadCopy(BinaryReader BR)
        {
            return new SFXTimeline(BR);
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            SFXTimeline NewSetSFXEvent = new SFXTimeline();

            NewSetSFXEvent.UpdateFrom(this, ActiveLayer);

            return NewSetSFXEvent;
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            SFX ActiveSetSFX = ((SFXKeyFrame)DicAnimationKeyFrame[KeyFrame]).SFX;
            if (string.IsNullOrEmpty(ActiveSetSFX.SFXPath))
                return;

            ActiveSetSFX.SFXSound = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + ActiveSetSFX.SFXPath);
            ActiveSetSFX.SFXSound.SetLoop(ActiveSetSFX.Loop);
            ActiveSetSFX.SFXSound.Play(ActiveSetSFX.Volume);
            ActiveSetSFX.DeathFrame = KeyFrame + ActiveSetSFX.Length;
            ActiveAnimation.ListActiveSFX.Add(ActiveSetSFX);
        }

        protected override AnimationObjectKeyFrame CreateFirstKeyFrame()
        {
            return new SFXKeyFrame();
        }
    }
}
