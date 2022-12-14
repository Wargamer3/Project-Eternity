using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class BlankTimeline : CoreTimeline
    {
        private const string TimelineType = "Bitmap";

        public Rectangle SourceRectangle;

        public BlankTimeline()
            : base(TimelineType, "New Bitmap")
        {
        }

        public BlankTimeline(int X, int Y, int Width, int Height)
            : this()
        {
            Position = new Vector2(X, Y);
            SourceRectangle = new Rectangle(X, Y, Width, Height);
            Origin = new Point(Width / 2, Height / 2);
            DeathFrame = 60;
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new BlankTimeline(SourceRectangle.X, SourceRectangle.Y, SourceRectangle.Width, SourceRectangle.Height);
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            BlankTimeline NewSpawnAnimatedBitmapEvent = new BlankTimeline();

            NewSpawnAnimatedBitmapEvent.SourceRectangle = SourceRectangle;

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
        }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return SourceRectangle.Width; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return SourceRectangle.Height; } }
    }
}
