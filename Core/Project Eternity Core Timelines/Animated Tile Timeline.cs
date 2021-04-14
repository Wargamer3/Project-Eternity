using System;
using System.IO;
using System.Drawing.Design;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimatedTileTimeline : CoreTimeline
    {
        public class AnimatedTileKeyFrame : VisibleAnimationObjectKeyFrame
        {
            private float _PixelPerSecond;
            private int _MaxWidth;

            private AnimatedTileKeyFrame()
            {
            }

            public AnimatedTileKeyFrame(Vector2 NextPosition, bool IsProgressive, int NextKeyFrame, float FramesPerSecond, int MaxWidth)
                : base(NextPosition, IsProgressive, NextKeyFrame)
            {
                this.PixelPerSecond = FramesPerSecond;
                this.MaxWidth = MaxWidth;
            }

            public AnimatedTileKeyFrame(BinaryReader BR)
                : base(BR)
            {
                PixelPerSecond = BR.ReadSingle();
                _MaxWidth = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(PixelPerSecond);
                BW.Write(_MaxWidth);
            }

            protected override VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                AnimatedTileKeyFrame NewAnimatedBitmapKeyFrame = new AnimatedTileKeyFrame();

                NewAnimatedBitmapKeyFrame.UpdateFrom(this);
                NewAnimatedBitmapKeyFrame._PixelPerSecond = _PixelPerSecond;
                NewAnimatedBitmapKeyFrame._MaxWidth = _MaxWidth;

                return NewAnimatedBitmapKeyFrame;
            }

            #region Properties

            [CategoryAttribute("Animated Bitmap Move Event"),
            DescriptionAttribute(".")]
            public float PixelPerSecond
            {
                get
                {
                    return _PixelPerSecond;
                }
                set
                {
                    _PixelPerSecond = value;
                }
            }

            [CategoryAttribute("Animated Bitmap Move Event"),
            DescriptionAttribute(".")]
            public int MaxWidth
            {
                get
                {
                    return _MaxWidth;
                }
                set
                {
                    _MaxWidth = value;
                }
            }

            #endregion
        }

        private const string TimelineType = "Tile";

        public string BitmapName;

        public float PixelPerSecondOld;
        public float AnimationValue;

        public int MaxWidthOld;
        public float MaxWidthValue;

        public float PixelPerSecond;
        private int _MaxWidth;
        public float Offset;

        public Texture2D ActiveSprite;

        public AnimatedTileTimeline()
            : base(TimelineType, "New Tile")
        {
            _MaxWidth = 312;
        }

        public AnimatedTileTimeline(string BitmapName, Texture2D ActiveSprite)
            : this()
        {
            this.BitmapName = BitmapName;

            this.ActiveSprite = ActiveSprite;
            Origin = new Point(Width / 2, Height / 2);
        }

        public AnimatedTileTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            Origin = new Point(BR.ReadInt32(), BR.ReadInt32());

            BitmapName = BR.ReadString();
            if (Content != null)
            {
                ActiveSprite = Content.Load<Texture2D>("Animations/Sprites/" + BitmapName);
            }

            _MaxWidth = BR.ReadInt32();

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                AnimatedTileKeyFrame NewAnimatedBitmapKeyFrame = new AnimatedTileKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new AnimatedTileTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(Origin.X);
            BW.Write(Origin.Y);

            BW.Write(BitmapName);
            BW.Write(_MaxWidth);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            AnimatedTileTimeline NewSpawnAnimatedBitmapEvent = new AnimatedTileTimeline();

            NewSpawnAnimatedBitmapEvent.BitmapName = BitmapName;
            NewSpawnAnimatedBitmapEvent.ActiveSprite = ActiveSprite;

            NewSpawnAnimatedBitmapEvent.PixelPerSecondOld = PixelPerSecondOld;

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            AnimatedBitmapSpawnerHelper NewSpawner = new AnimatedBitmapSpawnerHelper();
            if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimatedTileTimeline NewAnimatedBitmapSpawner = new AnimatedTileTimeline(NewSpawner.SpawnViewer.BitmapName, NewSpawner.SpawnViewer.Bitmap);

                NewAnimatedBitmapSpawner.Position = new Vector2(535, 170);
                NewAnimatedBitmapSpawner.SpawnFrame = KeyFrame;
                NewAnimatedBitmapSpawner.DeathFrame = KeyFrame + 10;
                NewAnimatedBitmapSpawner.IsUsed = true;//Disable the spawner as we spawn the AnimatedBitmap manually.
                NewAnimatedBitmapSpawner.Add(KeyFrame, new AnimatedTileKeyFrame(NewAnimatedBitmapSpawner.Position,
                                                                                true, -1, NewAnimatedBitmapSpawner.PixelPerSecond, 312));

                ReturnValue.Add(NewAnimatedBitmapSpawner);
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
                PixelPerSecond = PixelPerSecondOld + (int)(AnimationValue * (KeyFrame - EventKeyFrameOld));
                _MaxWidth = MaxWidthOld + (int)(MaxWidthValue * (KeyFrame - EventKeyFrameOld));

                UpdateAnimationSprite(KeyFrame);
            }

            Offset += PixelPerSecond * (1 / 60f);

            AnimatedTileKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = (AnimatedTileKeyFrame)ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;

                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);
                PixelPerSecond = ActiveKeyFrame.PixelPerSecond;
                MaxWidth = ActiveKeyFrame.MaxWidth;

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    ActiveKeyFrame = (AnimatedTileKeyFrame)ActiveAnimationSpriteKeyFrame;
                    if (ActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(ActiveKeyFrame, KeyFrame, NextKeyFrame);

                        PixelPerSecondOld = PixelPerSecond;
                        MaxWidthOld = MaxWidth;

                        float KeyFrameChange = KeyFrame - NextKeyFrame;
                        //Calculate of how many pixel the AnimatedBitmap will move per step.
                        AnimationValue = (PixelPerSecond - ActiveKeyFrame.PixelPerSecond) / KeyFrameChange;
                        MaxWidthValue = (MaxWidth - ActiveKeyFrame.MaxWidth) / KeyFrameChange;
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

            int FinalSpriteWidth = (int)(ActiveSprite.Width * ScaleFactor.X);
            int OffsetX = Origin.X + ActiveSprite.Width / 2;
            int ExtraFilling = _MaxWidth % ActiveSprite.Width;

            int AnimationStart = (int)Offset % ActiveSprite.Width;
            float StartX = Position.X - (OffsetX + ExtraFilling + AnimationStart) * ScaleFactor.X;
            if (AnimationStart + ExtraFilling < ActiveSprite.Width)
            {
                g.Draw(ActiveSprite, new Vector2(Position.X + (ActiveSprite.Width - ExtraFilling - OffsetX - AnimationStart) * ScaleFactor.X, Position.Y),
                    new Rectangle(0, 0, ExtraFilling + AnimationStart, ActiveSprite.Height), Color.White, 0,
                        new Vector2(0, Origin.Y),
                    new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);

                int RepeatX = _MaxWidth / ActiveSprite.Width - 1;
                g.Draw(ActiveSprite, new Vector2(StartX - RepeatX * FinalSpriteWidth + AnimationStart * ScaleFactor.X, Position.Y),
                    new Rectangle(AnimationStart, 0, ActiveSprite.Width - AnimationStart, ActiveSprite.Height), Color.White, 0,
                        new Vector2(0, Origin.Y),
                    new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);
            }
            else
            {
                int AnimationEnd = ActiveSprite.Width - AnimationStart;
                StartX = Position.X - (OffsetX + ExtraFilling + AnimationStart - ActiveSprite.Width) * ScaleFactor.X;

                g.Draw(ActiveSprite, new Vector2(Position.X + (ActiveSprite.Width - ExtraFilling - OffsetX - AnimationStart + ActiveSprite.Width) * ScaleFactor.X,  Position.Y),
                    new Rectangle(0, 0, ExtraFilling - AnimationEnd, ActiveSprite.Height), Color.White, 0,
                        new Vector2(0, Origin.Y),
                    new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);


                g.Draw(ActiveSprite, new Vector2(Position.X - (_MaxWidth + AnimationStart - ActiveSprite.Width) * ScaleFactor.X, Position.Y),
                    new Rectangle(0, 0, ActiveSprite.Width, ActiveSprite.Height), Color.White, 0,
                        new Vector2(0, Origin.Y),
                    new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);

                g.Draw(ActiveSprite, new Vector2(Position.X - _MaxWidth * ScaleFactor.X, Position.Y),
                    new Rectangle(AnimationStart, 0, ActiveSprite.Width - AnimationStart, ActiveSprite.Height), Color.White, 0,
                        new Vector2(0, Origin.Y),
                    new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);
            }
            if (_MaxWidth > ActiveSprite.Width)
            {
                int RepeatX = _MaxWidth / ActiveSprite.Width;
                for (int i = 0; i < RepeatX - 1; ++i)
                {
                    float RealX = StartX - i * FinalSpriteWidth;
                    g.Draw(ActiveSprite, new Vector2(RealX, Position.Y),
                        new Rectangle(0, 0, ActiveSprite.Width, ActiveSprite.Height), Color.White, 0,
                        new Vector2(0, Origin.Y),
                        new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);
                }
            }
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

                ActiveSprite = value.SpawnViewer.content.Load<Texture2D>("Animations/Sprites/" + BitmapName);
                Origin = new Point(Width / 2, Height / 2);
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public int MaxWidth { get { return _MaxWidth; } set { _MaxWidth = value; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return ActiveSprite.Width; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return ActiveSprite.Height; } }
    }
}
