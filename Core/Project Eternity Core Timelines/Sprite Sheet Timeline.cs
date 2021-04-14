using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using System.Drawing.Design;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class SpriteSheetTimeline : CoreTimeline
    {
        private const string TimelineType = "SpriteSheet";

        public Texture2D SpriteSheet;
        public string SpriteSheetName;
        public Rectangle SourceRectangle;
        public SpriteSheetHelper SpriteSheetHelperDialog;

        public SpriteSheetTimeline()
            : base(TimelineType, "New Sprite Sheet")
        {
            Origin = new Point(Width / 2, Height / 2);
        }

        public SpriteSheetTimeline(string SpriteSheetName, Texture2D SpriteSheet, Rectangle SourceRectangle)
            : this()
        {
            this.SpriteSheetName = SpriteSheetName;
            this.SpriteSheet = SpriteSheet;
            this.SourceRectangle = SourceRectangle;
            Origin = new Point(Width / 2, Height / 2);
        }

        public SpriteSheetTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            SpriteSheetName = BR.ReadString();
            SourceRectangle = new Rectangle();
            SourceRectangle.X = BR.ReadInt32();
            SourceRectangle.Y = BR.ReadInt32();
            SourceRectangle.Width = BR.ReadInt32();
            SourceRectangle.Height = BR.ReadInt32();

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                VisibleAnimationObjectKeyFrame NewAnimatedBitmapKeyFrame = new VisibleAnimationObjectKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }

            if (Content != null)
            {
                SpriteSheet = Content.Load<Texture2D>("Animations/Sprite Sheets/" + SpriteSheetName);
            }

            Origin = new Point(Width / 2, Height / 2);
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new SpriteSheetTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(SpriteSheetName);
            BW.Write(SourceRectangle.X);
            BW.Write(SourceRectangle.Y);
            BW.Write(SourceRectangle.Width);
            BW.Write(SourceRectangle.Height);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            SpriteSheetTimeline NewSpawnAnimatedBitmapEvent = new SpriteSheetTimeline(SpriteSheetName, SpriteSheet, SourceRectangle);

            NewSpawnAnimatedBitmapEvent.SpriteSheetHelperDialog = new SpriteSheetHelper();
            NewSpawnAnimatedBitmapEvent.SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet = new Dictionary<string, Texture2D>();

            ListViewItem NewListViewItem = new ListViewItem(SpriteSheetName);
            NewListViewItem.Tag = SpriteSheet;

            NewSpawnAnimatedBitmapEvent.SpriteSheetHelperDialog.lvSpriteSheets.Items.Add(NewListViewItem);
            NewSpawnAnimatedBitmapEvent.SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet.Add(SpriteSheetName, SpriteSheet);

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override void OnAnimationEditorLoad(AnimationClass ActiveAnimation)
        {
            SpriteSheetHelperDialog = new SpriteSheetHelper();
            SpriteSheetHelperDialog.SpriteSheetViewer.Preload();
            SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet = new Dictionary<string, Texture2D>();
            if (SpriteSheetName != null)
            {
                ListViewItem NewListViewItem = new ListViewItem(SpriteSheetName);
                NewListViewItem.Tag = SpriteSheet;

                SpriteSheetHelperDialog.lvSpriteSheets.Items.Add(NewListViewItem);
                SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet.Add(SpriteSheetName, SpriteSheet);
            }
            else
            {
                for (int L = ActiveAnimation.ListAnimationLayer.Count - 1; L >= 0; --L)
                {
                    foreach (KeyValuePair<int, List<Timeline>> ActiveEvent in ActiveAnimation.ListAnimationLayer[L].DicTimelineEvent)
                    {
                        for (int E = ActiveEvent.Value.Count - 1; E >= 0; --E)
                        {
                            SpriteSheetTimeline ActiveTimeline = ActiveEvent.Value[E] as SpriteSheetTimeline;

                            if (ActiveTimeline != null)
                            {
                                if (!SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet.ContainsKey(ActiveTimeline.SpriteSheetName))
                                {
                                    ListViewItem NewListViewItem = new ListViewItem(ActiveTimeline.SpriteSheetName);
                                    NewListViewItem.Tag = ActiveTimeline.SpriteSheet;

                                    SpriteSheetHelperDialog.lvSpriteSheets.Items.Add(NewListViewItem);
                                    SpriteSheetHelperDialog.SpriteSheetViewer.DicSpriteSheet.Add(ActiveTimeline.SpriteSheetName, ActiveTimeline.SpriteSheet);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            SpriteSheetHelperDialog.SpriteSheetViewer.DicActiveSpriteSheetBitmap.Clear();
            if (SpriteSheetHelperDialog.ShowDialog() == DialogResult.OK && SpriteSheetHelperDialog.SpriteSheetViewer.DicActiveSpriteSheetBitmap.Count > 0)
            {
                foreach (KeyValuePair<Tuple<int, int>, SpriteSheetTimeline> ActiveBitmap in SpriteSheetHelperDialog.SpriteSheetViewer.DicActiveSpriteSheetBitmap)
                {
                    SpriteSheetTimeline NewSpawnSpriteSheetBitmap = ActiveBitmap.Value;

                    NewSpawnSpriteSheetBitmap.Position = new Vector2(535, 170);
                    NewSpawnSpriteSheetBitmap.SpawnFrame = KeyFrame;
                    NewSpawnSpriteSheetBitmap.DeathFrame = KeyFrame + 10;
                    NewSpawnSpriteSheetBitmap.IsUsed = true;//Disable the spawner as we spawn the SpawnSpriteSheetBitmap manually.
                    NewSpawnSpriteSheetBitmap.Add(
                        KeyFrame, new VisibleAnimationObjectKeyFrame(new Vector2(NewSpawnSpriteSheetBitmap.Position.X, NewSpawnSpriteSheetBitmap.Position.Y),
                                                        true, -1));

                    ReturnValue.Add(NewSpawnSpriteSheetBitmap);
                }
            }

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            //An Event is being executed.
            if (NextEvent != null)
            {
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

            VisibleAnimationObjectKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;

                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    ActiveKeyFrame = ActiveAnimationSpriteKeyFrame;
                    if (ActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(ActiveKeyFrame, KeyFrame, NextKeyFrame);
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

            g.Draw(SpriteSheet,
                new Vector2(Position.X, Position.Y),
                SourceRectangle, Color.FromNonPremultiplied(255, 255, 255, Alpha),
                Angle, new Vector2(Origin.X, Origin.Y),
                new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);
        }

        [Editor(typeof(SpriteSheetSelector), typeof(UITypeEditor)),
        CategoryAttribute("Spawner Attributes"),
        DescriptionAttribute(".")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public SpriteSheetHelper Bitmap
        {
            get
            {
                return SpriteSheetHelperDialog;
            }
            set
            {
                foreach (SpriteSheetTimeline ActiveSprite in value.SpriteSheetViewer.DicActiveSpriteSheetBitmap.Values)
                {
                    SourceRectangle = ActiveSprite.SourceRectangle;
                    break;
                }

                foreach (KeyValuePair<string, Texture2D> ActiveSpriteSheet in value.SpriteSheetViewer.DicSpriteSheet)
                {
                    SpriteSheetName = ActiveSpriteSheet.Key;
                    SpriteSheet = ActiveSpriteSheet.Value;
                    break;
                }

                Origin = new Point(Width / 2, Height / 2);
            }
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return SourceRectangle.Width; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return SourceRectangle.Height; } }
    }
}
