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
    public class AnimatedChainTimeline : CoreTimeline
    {
        public class AnimatedChainKeyFrame : VisibleAnimationObjectKeyFrame
        {
            private int _ChainLengthInPixel;
            public List<Vector2> ListChainSplinePoints;
            public List<Vector2> ListChainSplinePointsMoveValue;

            private AnimatedChainKeyFrame()
            {
                ListChainSplinePoints = new List<Vector2>();
                ListChainSplinePointsMoveValue = new List<Vector2>();
            }

            public AnimatedChainKeyFrame(Vector2 NextPosition, bool IsProgressive, int NextKeyFrame, int _ChainLengthInPixel)
                : base(NextPosition, IsProgressive, NextKeyFrame)
            {
                this._ChainLengthInPixel = _ChainLengthInPixel;
                ListChainSplinePoints = new List<Vector2>();
                ListChainSplinePointsMoveValue = new List<Vector2>();
            }

            public AnimatedChainKeyFrame(BinaryReader BR)
                : base(BR)
            {
                _ChainLengthInPixel = BR.ReadInt32();

                int ListChainSplinePointsCount = BR.ReadInt32();
                ListChainSplinePoints = new List<Vector2>(ListChainSplinePointsCount);
                ListChainSplinePointsMoveValue = new List<Vector2>(ListChainSplinePointsCount);
                for (int C = 0; C < ListChainSplinePointsCount; ++C)
                {
                    ListChainSplinePoints.Add(new Vector2(BR.ReadSingle(), BR.ReadSingle()));
                    ListChainSplinePointsMoveValue.Add(new Vector2());
                }
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_ChainLengthInPixel);

                BW.Write(ListChainSplinePoints.Count);
                for (int C = 0; C < ListChainSplinePoints.Count; ++C)
                {
                    BW.Write(ListChainSplinePoints[C].X);
                    BW.Write(ListChainSplinePoints[C].Y);
                }
            }

            protected override VisibleAnimationObjectKeyFrame CopyAsVisibleAnimationObjectKeyFrame(AnimationClass.AnimationLayer ActiveLayer)
            {
                AnimatedChainKeyFrame NewAnimatedBitmapKeyFrame = new AnimatedChainKeyFrame();

                NewAnimatedBitmapKeyFrame.UpdateFrom(this);
                NewAnimatedBitmapKeyFrame._ChainLengthInPixel = _ChainLengthInPixel;
                NewAnimatedBitmapKeyFrame.ListChainSplinePoints = new List<Vector2>(ListChainSplinePoints);
                NewAnimatedBitmapKeyFrame.ListChainSplinePointsMoveValue = new List<Vector2>(ListChainSplinePointsMoveValue);

                return NewAnimatedBitmapKeyFrame;
            }

            #region Properties

            [CategoryAttribute("Animated Bitmap Move Event"),
            DescriptionAttribute(".")]
            public int ChainLengthInPixel
            {
                get
                {
                    return _ChainLengthInPixel;
                }
                set
                {
                    _ChainLengthInPixel = value;
                }
            }

            #endregion
        }

        private const string TimelineType = "Chain";

        private int CurrentChainLengthInPixel;
        private List<Vector2> ListCurrentChainSplinePoints;
        private List<Vector2> ListCurrentChainSplinePointsOld;

        public string ChainLinkPath;
        public string ChainEndPath;
        public string ChainStartPath;

        public AnimatedSprite ChainLink;
        public AnimatedSprite ChainEnd;
        public AnimatedSprite ChainStart;

        private AnimatedChainKeyFrame NextActiveKeyFrame;

        public AnimatedChainTimeline()
            : base(TimelineType, "New Chain")
        {
            CurrentChainLengthInPixel = 0;
            ListCurrentChainSplinePoints = new List<Vector2>();
        }

        public AnimatedChainTimeline(ContentManager Content,
            string ChainLinkPath, Vector2 ChainLinkOrigin,
            string ChainEndPath, Vector2 ChainEndOrigin,
            string ChainStartPath, Vector2 ChainStartOrigin)
            : this()
        {
            this.ChainLinkPath = ChainLinkPath;
            this.ChainEndPath = ChainEndPath;
            this.ChainStartPath = ChainStartPath;

            if (!string.IsNullOrEmpty(ChainLinkPath))
            {
                ChainLink = new AnimatedSprite(Content, "Animations/Sprites/" + ChainLinkPath, Vector2.Zero);
                ChainLink.Origin = ChainLinkOrigin;
            }
            if (!string.IsNullOrEmpty(ChainEndPath))
            {
                ChainEnd = new AnimatedSprite(Content, "Animations/Sprites/" + ChainEndPath, Vector2.Zero);
                ChainEnd.Origin = ChainEndOrigin;
            }
            if (!string.IsNullOrEmpty(ChainStartPath))
            {
                ChainStart = new AnimatedSprite(Content, "Animations/Sprites/" + ChainStartPath, Vector2.Zero);
                ChainStart.Origin = ChainStartOrigin;
            }

            Origin = new Point(Width / 2, Height / 2);

            ListCurrentChainSplinePoints = new List<Vector2>();
        }

        public AnimatedChainTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            ListCurrentChainSplinePoints = new List<Vector2>();

            Origin = new Point(BR.ReadInt32(), BR.ReadInt32());

            ChainLinkPath = BR.ReadString();

            if (Content != null && !string.IsNullOrEmpty(ChainLinkPath))
            {
                ChainLink = new AnimatedSprite(Content, "Animations/Sprites/" + ChainLinkPath, Vector2.Zero);
                ChainLink.Origin = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            ChainEndPath = BR.ReadString();
            if (Content != null && !string.IsNullOrEmpty(ChainEndPath))
            {
                ChainEnd = new AnimatedSprite(Content, "Animations/Sprites/" + ChainEndPath, Vector2.Zero);
                ChainEnd.Origin = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            ChainStartPath = BR.ReadString();
            if (Content != null && !string.IsNullOrEmpty(ChainStartPath))
            {
                ChainStart = new AnimatedSprite(Content, "Animations/Sprites/" + ChainStartPath, Vector2.Zero);
                ChainStart.Origin = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                AnimatedChainKeyFrame NewAnimatedBitmapKeyFrame = new AnimatedChainKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new AnimatedChainTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(Origin.X);
            BW.Write(Origin.Y);

            BW.Write(ChainLinkPath);
            if (!string.IsNullOrEmpty(ChainLinkPath))
            {
                BW.Write(ChainLink.Origin.X);
                BW.Write(ChainLink.Origin.Y);
            }

            BW.Write(ChainEndPath);
            if (!string.IsNullOrEmpty(ChainEndPath))
            {
                BW.Write(ChainEnd.Origin.X);
                BW.Write(ChainEnd.Origin.Y);
            }

            BW.Write(ChainStartPath);
            if (!string.IsNullOrEmpty(ChainStartPath))
            {
                BW.Write(ChainStart.Origin.X);
                BW.Write(ChainStart.Origin.Y);
            }

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            AnimatedChainTimeline NewSpawnAnimatedBitmapEvent = new AnimatedChainTimeline();

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            AnimatedChainSpawnerHelper NewSpawner = new AnimatedChainSpawnerHelper();
            if (NewSpawner.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AnimatedChainTimeline NewAnimatedBitmapSpawner = new AnimatedChainTimeline(NewSpawner.ChainLinkViewer.content,
                    NewSpawner.ChainLinkPath, new Vector2((float)NewSpawner.txtChainLinkOriginX.Value, (float)NewSpawner.txtChainLinkOriginY.Value),
                    NewSpawner.ChainEndPath, new Vector2((float)NewSpawner.txtChainEndOriginX.Value, (float)NewSpawner.txtChainEndOriginY.Value),
                    NewSpawner.ChainStartPath, new Vector2((float)NewSpawner.txtChainStartOriginX.Value, (float)NewSpawner.txtChainStartOriginY.Value));

                NewAnimatedBitmapSpawner.Position = new Vector2(535, 170);
                NewAnimatedBitmapSpawner.SpawnFrame = KeyFrame;
                NewAnimatedBitmapSpawner.DeathFrame = KeyFrame + 10;
                NewAnimatedBitmapSpawner.IsUsed = true;//Disable the spawner as we spawn the AnimatedBitmap manually.
                NewAnimatedBitmapSpawner.Add(KeyFrame, new AnimatedChainKeyFrame(NewAnimatedBitmapSpawner.Position,
                                                                                true, -1, 50));

                ReturnValue.Add(NewAnimatedBitmapSpawner);
            }

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            //An Event is being executed.
            if (NextEvent != null)
            {
                UpdateAnimationSprite(KeyFrame);

                for (int L = 0; L < NextActiveKeyFrame.ListChainSplinePoints.Count; ++L)
                {
                    Vector2 Result = NextActiveKeyFrame.ListChainSplinePointsMoveValue[L] * (KeyFrame - EventKeyFrameOld);
                    ListCurrentChainSplinePoints[L] = new Vector2(ListCurrentChainSplinePointsOld[L].X + (int)Result.X, ListCurrentChainSplinePointsOld[L].Y + (int)Result.Y);

                }
            }

            if (ChainLink != null)
            {
                ChainLink.Update(ChainLink.FramesPerSecond * (1 / 60f));
                if (ChainLink.AnimationEnded)
                {
                    ChainLink.LoopAnimation();
                }
            }

            if (ChainEnd != null)
            {
                ChainEnd.Update(ChainEnd.FramesPerSecond * (1 / 60f));
                if (ChainEnd.AnimationEnded)
                {
                    ChainEnd.LoopAnimation();
                }
            }

            if (ChainStart != null)
            {
                ChainStart.Update(ChainStart.FramesPerSecond * (1 / 60f));
                if (ChainStart.AnimationEnded)
                {
                    ChainStart.LoopAnimation();
                }
            }

            AnimatedChainKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = (AnimatedChainKeyFrame)ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;

                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                CurrentChainLengthInPixel = ActiveKeyFrame.ChainLengthInPixel;
                ListCurrentChainSplinePoints = new List<Vector2>(ActiveKeyFrame.ListChainSplinePoints);
                ListCurrentChainSplinePointsOld = new List<Vector2>(ActiveKeyFrame.ListChainSplinePoints);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    NextActiveKeyFrame = (AnimatedChainKeyFrame)ActiveAnimationSpriteKeyFrame;
                    if (NextActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(NextActiveKeyFrame, KeyFrame, NextKeyFrame);

                        float KeyFrameChange = KeyFrame - NextKeyFrame;

                        for (int L = 0; L < ActiveKeyFrame.ListChainSplinePoints.Count; ++L)
                        {
                            NextActiveKeyFrame.ListChainSplinePointsMoveValue[L] = new Vector2((ActiveKeyFrame.ListChainSplinePoints[L].X - NextActiveKeyFrame.ListChainSplinePoints[L].X) / KeyFrameChange,
                                                                   (ActiveKeyFrame.ListChainSplinePoints[L].Y - NextActiveKeyFrame.ListChainSplinePoints[L].Y) / KeyFrameChange);

                        }
                    }
                    else
                    {
                        NextEvent = null;
                    }
                }
            }
        }

        public override bool MouseDownExtra(int RealX, int RealY)
        {
            for (int C = 0; C < ListCurrentChainSplinePoints.Count; C++)
            {
                int KeyFrameX = (int)ListCurrentChainSplinePoints[C].X - 2;
                int KeyFrameY = (int)ListCurrentChainSplinePoints[C].Y - 2;

                if (RealX >= KeyFrameX && RealX < KeyFrameX + 5 &&
                    RealY >= KeyFrameY && RealY < KeyFrameY + 5)
                {
                    return true;
                }
            }

            return base.MouseDownExtra(RealX, RealY);
        }

        public override void MouseMoveExtra(int KeyFrame, int RealX, int RealY, int MouseChangeX, int MouseChangeY)
        {
            AnimatedChainKeyFrame ActiveObjectKeyFrame = (AnimatedChainKeyFrame)CreateOrRetriveKeyFrame(null, KeyFrame);

            for (int C = 0; C < ActiveObjectKeyFrame.ListChainSplinePoints.Count; C++)
            {
                int KeyFrameX = (int)ActiveObjectKeyFrame.ListChainSplinePoints[C].X - 2;
                int KeyFrameY = (int)ActiveObjectKeyFrame.ListChainSplinePoints[C].Y - 2;

                if (RealX >= KeyFrameX && RealX < KeyFrameX + 5 &&
                    RealY >= KeyFrameY && RealY < KeyFrameY + 5)
                {
                    ActiveObjectKeyFrame.ListChainSplinePoints[C] =
                        new Vector2(ActiveObjectKeyFrame.ListChainSplinePoints[C].X + MouseChangeX,
                            ActiveObjectKeyFrame.ListChainSplinePoints[C].Y + MouseChangeY);

                    ListCurrentChainSplinePoints[C] = ActiveObjectKeyFrame.ListChainSplinePoints[C];
                    break;
                }
            }

            base.MouseMoveExtra(KeyFrame, RealX, RealY, MouseChangeX, MouseChangeY);
        }

        public override void MouseUpExtra()
        {
            for (int C = 0; C < ListCurrentChainSplinePoints.Count; C++)
            {
            }

            base.MouseUpExtra();
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
            if (ListCurrentChainSplinePoints.Count == 0)
            {
                return;
            }

            Vector2 LastPos = PositionOld;
            for (int L = 0; L < CurrentChainLengthInPixel; ++L)
            {
                float ProgressionValue = L / (float)CurrentChainLengthInPixel;

                float t = ProgressionValue;

                float x0 = Position.X;
                float x2 = ListCurrentChainSplinePoints[ListCurrentChainSplinePoints.Count - 1].X;
                float y0 = Position.Y;
                float y2 = ListCurrentChainSplinePoints[ListCurrentChainSplinePoints.Count - 1].Y;

                int n = ListCurrentChainSplinePoints.Count + 1;
                double ResultX = Math.Pow(1 - t, n) * x0;
                double ResultY = Math.Pow(1 - t, n) * y0;

                int nFac = 1;
                for (int k = 1; k <= n; k++)
                    nFac *= k;

                for (int C = 0; C < ListCurrentChainSplinePoints.Count; C++)
                {
                    int i = C + 1;

                    int niFac = 1;
                    int iFac = 1;

                    for (int k = 1; k <= n - i; k++)
                        niFac *= k;

                    for (int k = 1; k <= i; k++)
                        iFac *= k;

                    int nChoosek = nFac / (niFac * iFac);

                    //n! / ((n-i)!*i!) * ((1 - t)^(n-1)) * (t ^ i) * Point[i]
                    ResultX += nChoosek * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * ListCurrentChainSplinePoints[C].X;
                    ResultY += nChoosek * Math.Pow(1 - t, n - i) * Math.Pow(t, i) * ListCurrentChainSplinePoints[C].Y;
                }

                ResultX += Math.Pow(t, n) * x2;
                ResultY += Math.Pow(t, n) * y2;

                Vector2 DrawPosition = new Vector2((float)ResultX, (float)ResultY);

                float ChainAngle = (float)Math.Atan2(DrawPosition.Y - LastPos.Y, DrawPosition.X - LastPos.X);

                if (L == 0 && ChainStart != null)
                {
                    ChainStart.Draw(g, DrawPosition, Color.White, ChainAngle, 0f, new Vector2(1, 1), SpriteEffects.None);
                }
                else if (L == CurrentChainLengthInPixel - 1 && ChainEnd != null)
                {
                    ChainEnd.Draw(g, DrawPosition, Color.White, ChainAngle, 0f, new Vector2(1, 1), SpriteEffects.None);
                }
                else if (ChainLink != null)
                {
                    ChainLink.Draw(g, DrawPosition, Color.White, ChainAngle, 0f, new Vector2(1, 1), SpriteEffects.None);
                }

                LastPos = DrawPosition;
            }
        }

        public override void DrawExtra(CustomSpriteBatch g, Texture2D sprPixel)
        {
            base.DrawExtra(g, sprPixel);

            for (int C = 0; C < ListCurrentChainSplinePoints.Count; C++)
            {
                g.Draw(sprPixel, new Rectangle((int)ListCurrentChainSplinePoints[C].X - 2,
                                               (int)ListCurrentChainSplinePoints[C].Y - 2, 5, 5), Color.Red);

                if (C >= 1)
                {
                    g.DrawLine(sprPixel, new Vector2(ListCurrentChainSplinePoints[C - 1].X,
                                            ListCurrentChainSplinePoints[C - 1].Y),
                                new Vector2(ListCurrentChainSplinePoints[C].X,
                                            ListCurrentChainSplinePoints[C].Y), Color.LightGreen);
                }
                else
                {
                    g.DrawLine(sprPixel, new Vector2(PositionOld.X,
                                            PositionOld.Y),
                                new Vector2(ListCurrentChainSplinePoints[C].X,
                                            ListCurrentChainSplinePoints[C].Y), Color.LightGreen);
                }
            }
        }

        public override int Width => 32;

        public override int Height => 32;

        [Editor(typeof(AnimatedBitmapSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        public AnimatedChainSpawnerHelper Bitmap
        {
            get
            {
                return new AnimatedChainSpawnerHelper(ChainLink.ToString());
            }
            set
            {
                Origin = new Point(Width / 2, Height / 2);
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public int NumberOfLink
        {
            get { return ListCurrentChainSplinePoints.Count; }
            set
            {
                foreach (AnimatedChainKeyFrame ActiveKeyFrame in DicAnimationKeyFrame.Values)
                {
                    if (value < ActiveKeyFrame.ListChainSplinePoints.Count)
                    {
                        ActiveKeyFrame.ListChainSplinePointsMoveValue.RemoveRange(value, ActiveKeyFrame.ListChainSplinePoints.Count - value);
                        ActiveKeyFrame.ListChainSplinePoints.RemoveRange(value, ActiveKeyFrame.ListChainSplinePoints.Count - value);
                    }
                    else
                    {
                        while (value > ActiveKeyFrame.ListChainSplinePoints.Count)
                        {
                            ActiveKeyFrame.ListChainSplinePointsMoveValue.Add(new Vector2(Position.X, Position.Y));
                            ActiveKeyFrame.ListChainSplinePoints.Add(new Vector2(Position.X, Position.Y));
                        }
                    }
                }

                if (value < ListCurrentChainSplinePoints.Count)
                {
                    ListCurrentChainSplinePointsOld.RemoveRange(value, ListCurrentChainSplinePoints.Count - value);
                    ListCurrentChainSplinePoints.RemoveRange(value, ListCurrentChainSplinePoints.Count - value);
                }
                else
                {
                    while (value > ListCurrentChainSplinePoints.Count)
                    {
                        ListCurrentChainSplinePointsOld.Add(new Vector2(Position.X, Position.Y));
                        ListCurrentChainSplinePoints.Add(new Vector2(Position.X, Position.Y));
                    }
                }
            }
        }
    }
}
