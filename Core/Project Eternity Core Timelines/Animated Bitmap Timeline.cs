using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimatedBitmapTimeline : CoreTimeline
    {
        public class AnimatedBitmapKeyFrame : VisibleAnimationObjectKeyFrame
        {
            private float _FramesPerSecond;

            private AnimatedBitmapKeyFrame()
            {
            }

            public AnimatedBitmapKeyFrame(Vector2 NextPosition, bool IsProgressive, int NextKeyFrame, float FramesPerSecond)
                : base(NextPosition, IsProgressive, NextKeyFrame)
            {
                this.FramesPerSecond = FramesPerSecond;
            }

            public AnimatedBitmapKeyFrame(BinaryReader BR)
                : base(BR)
            {
                FramesPerSecond = BR.ReadSingle();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(FramesPerSecond);
            }

            protected override VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                AnimatedBitmapKeyFrame NewAnimatedBitmapKeyFrame = new AnimatedBitmapKeyFrame();

                NewAnimatedBitmapKeyFrame.UpdateFrom(this);
                NewAnimatedBitmapKeyFrame._FramesPerSecond = _FramesPerSecond;

                return NewAnimatedBitmapKeyFrame;
            }

            #region Properties

            [CategoryAttribute("Animated Bitmap Move Event"),
            DescriptionAttribute(".")]
            public float FramesPerSecond
            {
                get
                {
                    return _FramesPerSecond;
                }
                set
                {
                    _FramesPerSecond = value;
                }
            }

            #endregion
        }

        private const string TimelineType = "Bitmap";

        public string BitmapName;

        public float AnimationSpeedOld;
        public float AnimationValue;

        public AnimatedSprite ActiveSprite;

        public AnimatedBitmapTimeline()
            : base(TimelineType, "New Bitmap")
        {
        }

        public AnimatedBitmapTimeline(ContentManager Content, string BitmapName)
            : this()
        {
            this.BitmapName = BitmapName;

            ActiveSprite = new AnimatedSprite(Content, "Animations/Sprites/" + BitmapName, Vector2.Zero);
            Origin = new Point(Width / 2, Height / 2);
        }

        public AnimatedBitmapTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            Origin = new Point(BR.ReadInt32(), BR.ReadInt32());


            BitmapName = BR.ReadString();
            if (Content != null)
            {
                ActiveSprite = new AnimatedSprite(Content, "Animations/Sprites/" + BitmapName, Vector2.Zero);
            }

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                AnimatedBitmapKeyFrame NewAnimatedBitmapKeyFrame = new AnimatedBitmapKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new AnimatedBitmapTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(Origin.X);
            BW.Write(Origin.Y);

            BW.Write(BitmapName);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            AnimatedBitmapTimeline NewSpawnAnimatedBitmapEvent = new AnimatedBitmapTimeline();

            NewSpawnAnimatedBitmapEvent.BitmapName = BitmapName;
            NewSpawnAnimatedBitmapEvent.ActiveSprite = ActiveSprite.Copy();

            NewSpawnAnimatedBitmapEvent.AnimationSpeedOld = AnimationSpeedOld;

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            AnimatedBitmapSpawnerHelper NewSpawner = new AnimatedBitmapSpawnerHelper();
            if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimatedBitmapTimeline NewAnimatedBitmapTimeline = new AnimatedBitmapTimeline(NewSpawner.SpawnViewer.content, NewSpawner.SpawnViewer.BitmapName);

                NewAnimatedBitmapTimeline.Position = new Vector2(535, 170);
                NewAnimatedBitmapTimeline.SpawnFrame = KeyFrame;
                NewAnimatedBitmapTimeline.DeathFrame = KeyFrame + 10;
                NewAnimatedBitmapTimeline.IsUsed = true;//Disable the spawner as we spawn the AnimatedBitmap manually.
                NewAnimatedBitmapTimeline.Add(KeyFrame, new AnimatedBitmapKeyFrame(NewAnimatedBitmapTimeline.Position,
                                                                                true, -1, NewAnimatedBitmapTimeline.ActiveSprite.FramesPerSecond));

                ReturnValue.Add(NewAnimatedBitmapTimeline);
            }

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            if (ActiveSprite == null)
            {
                return;
            }
            //An Event is being executed.
            if (NextEvent != null)
            {
                ActiveSprite.FramesPerSecond = AnimationSpeedOld + (int)(AnimationValue * (KeyFrame - EventKeyFrameOld));

                UpdateAnimationSprite(KeyFrame);
            }
            else
            {
                Position = PositionOld;
                ScaleFactor = ScaleFactorOld;
                Angle = AngleOld;
                Alpha = AlphaOld;
                DrawingDepth = DrawingDepthOld;
            }

            ActiveSprite.Update(ActiveSprite.FramesPerSecond * (1 / 60f));
            if (ActiveSprite.AnimationEnded)
            {
                ActiveSprite.LoopAnimation();
            }

            AnimatedBitmapKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = (AnimatedBitmapKeyFrame)ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;

                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);
                ActiveSprite.FramesPerSecond = ActiveKeyFrame.FramesPerSecond;

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    ActiveKeyFrame = (AnimatedBitmapKeyFrame)ActiveAnimationSpriteKeyFrame;
                    if (ActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(ActiveKeyFrame, KeyFrame, NextKeyFrame);

                        AnimationSpeedOld = ActiveSprite.FramesPerSecond;

                        float KeyFrameChange = KeyFrame - NextKeyFrame;
                        //Calculate of how many pixel the AnimatedBitmap will move per step.
                        AnimationValue = (ActiveSprite.FramesPerSecond - ActiveKeyFrame.FramesPerSecond) / KeyFrameChange;
                    }
                    else
                        NextEvent = null;
                }
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
            SpriteEffects ActiveEffect = SpriteEffects.None;
            if (ScaleFactor.X < 0)
                ActiveEffect = SpriteEffects.FlipHorizontally;
            if (ScaleFactor.Y < 0)
                ActiveEffect |= SpriteEffects.FlipVertically;

            ActiveSprite.Draw(g, Position, Color.FromNonPremultiplied(255, 255, 255, Alpha), Angle, DrawingDepth, new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect);
        }

        [Editor(typeof(AnimatedBitmapSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public AnimatedBitmapSpawnerHelper Bitmap
        {
            get
            {
                return new AnimatedBitmapSpawnerHelper(BitmapName);
            }
            set
            {
                this.BitmapName = value.SpawnViewer.BitmapName;

                ActiveSprite = new AnimatedSprite(value.SpawnViewer.content, "Animations/Sprites/" + BitmapName, Vector2.Zero);
                Origin = new Point(Width / 2, Height / 2);
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return ActiveSprite.SpriteWidth; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return ActiveSprite.SpriteHeight; } }
    }
}
