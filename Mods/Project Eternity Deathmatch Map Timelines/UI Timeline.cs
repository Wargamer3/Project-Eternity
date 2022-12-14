using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class UITimeline : DeathmatchMapTimeline
    {
        private const string TimelineType = "Deathmatch UI";

        private SpriteFont fntFinlanderFont;
        private Texture2D sprBarExtraLargeBackground;
        private Texture2D sprBarExtraLargeEN;
        private Texture2D sprBarExtraLargeHP;
        private Texture2D sprInfinity;

        private double Progress;
        private AnimationScreen Owner;
        public RenderTarget2D renderTarget;//Buffer used to render the AnimationLayer.

        public UITimeline()
            : this(null, "Deathmatch UI")
        {
        }

        private UITimeline(AnimationScreen Owner, string Name)
            : base(TimelineType, Name)
        {
            this.Owner = Owner;
            this.Name = Name;

            Origin = new Point(0, 0);
        }

        public UITimeline(AnimationScreen Owner, ContentManager Content)
            : this(Owner, "Deathmatch UI")
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            sprBarExtraLargeBackground = Content.Load<Texture2D>("Battle/Bars/Extra Long Bar");
            sprBarExtraLargeEN = Content.Load<Texture2D>("Battle/Bars/Extra Long Energy");
            sprBarExtraLargeHP = Content.Load<Texture2D>("Battle/Bars/Extra Long Health");
            sprInfinity = Content.Load<Texture2D>("Battle/Infinity");
        }


        private UITimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            sprBarExtraLargeBackground = Content.Load<Texture2D>("Battle/Bars/Extra Long Bar");
            sprBarExtraLargeEN = Content.Load<Texture2D>("Battle/Bars/Extra Long Energy");
            sprBarExtraLargeHP = Content.Load<Texture2D>("Battle/Bars/Extra Long Health");
            sprInfinity = Content.Load<Texture2D>("Battle/Infinity");

            _SpawnFrame = BR.ReadInt32();
            _DeathFrame = BR.ReadInt32();

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                VisibleAnimationObjectKeyFrame NewAnimatedBitmapKeyFrame = new VisibleAnimationObjectKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            UITimeline Copy = new UITimeline(BR, Content);
            Copy.Owner = Owner;
            return Copy;
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(SpawnFrame);
            BW.Write(DeathFrame);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            UITimeline NewUITimeline = new UITimeline(Owner, Name);

            NewUITimeline.UpdateFrom(this, ActiveLayer);

            return NewUITimeline;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            UITimeline NewUITimeline = new UITimeline(Owner, ActiveAnimation.Content);
            NewUITimeline.Position = new Vector2(0, 0);
            NewUITimeline.SpawnFrame = KeyFrame;
            NewUITimeline.DeathFrame = KeyFrame + 10;
            NewUITimeline.IsUsed = true;//Disable the spawner as we spawn the Marker manually.
            NewUITimeline.Add(KeyFrame,
                                    new VisibleAnimationObjectKeyFrame(new Vector2(NewUITimeline.Position.X, NewUITimeline.Position.Y),
                                                                        true, -1));

            ReturnValue.Add(NewUITimeline);

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            Progress = 0;
            //An Event is being executed.
            if (NextEvent != null)
            {
                Progress = (KeyFrame - EventKeyFrameOld) / (float)(NextEventKeyFrame - EventKeyFrameOld);
                UpdateAnimationSprite(KeyFrame);
            }

            VisibleAnimationObjectKeyFrame ActiveKeyFrame;
            VisibleAnimationObjectKeyFrame ActiveAnimationSpriteKeyFrame;

            if (DicAnimationKeyFrame.TryGetValue(KeyFrame, out ActiveAnimationSpriteKeyFrame))
            {
                ActiveKeyFrame = (VisibleAnimationObjectKeyFrame)ActiveAnimationSpriteKeyFrame;
                //If that animation has already been used, skip it.
                if (ActiveKeyFrame.IsUsed)
                    return;

                int NextKeyFrame = ActiveKeyFrame.NextKeyFrame;
                OnNewKeyFrameAnimationSprite(ActiveKeyFrame);

                if (DicAnimationKeyFrame.TryGetValue(NextKeyFrame, out ActiveAnimationSpriteKeyFrame))
                {
                    VisibleAnimationObjectKeyFrame NextActiveKeyFrame = (VisibleAnimationObjectKeyFrame)ActiveAnimationSpriteKeyFrame;
                    if (NextActiveKeyFrame.IsProgressive)
                    {
                        OnProgressiveNextKeyFrameAnimationSprite(NextActiveKeyFrame, KeyFrame, NextKeyFrame);
                    }
                    else
                        NextEvent = null;
                }
            }
        }

        public override bool CanSelect(int PosX, int PosY)
        {
            if (PosX >= 0 && PosX <= Constants.Width && PosY >= 0 && PosY <= 84)
            {
                return true;
            }
            return base.CanSelect(PosX, PosY);
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            if (renderTarget == null || renderTarget.Width != g.GraphicsDevice.PresentationParameters.BackBufferWidth ||
                renderTarget.Height != g.GraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                renderTarget = new RenderTarget2D(
                    g.GraphicsDevice,
                    g.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    g.GraphicsDevice.PresentationParameters.BackBufferHeight);
            }

            g.GraphicsDevice.SetRenderTarget(renderTarget);
            g.GraphicsDevice.Clear(Color.Transparent);

            if (Owner == null)
            {
                DrawUI(g);
            }
            else
            {
                Owner.DrawUI(g);
            }

            g.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Draw(CustomSpriteBatch g, bool IsInEditMode)
        {
            SpriteEffects ActiveEffect = SpriteEffects.None;
            if (ScaleFactor.X < 0)
                ActiveEffect = SpriteEffects.FlipHorizontally;
            if (ScaleFactor.Y < 0)
                ActiveEffect |= SpriteEffects.FlipVertically;

            float OriginX = (int)Origin.X;
            if (Owner != null && Owner.IsLeftAttacking)
            {
                if (ScaleFactor.X < 0)
                {
                    ActiveEffect = SpriteEffects.None;
                }
                else
                {
                    ActiveEffect = SpriteEffects.FlipHorizontally;
                    if ((ActiveEffect & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally)
                        OriginX = Constants.Width - 640;
                }
            }

            g.Draw(renderTarget,
                new Vector2(Position.X, Position.Y),
                null, Color.FromNonPremultiplied(255, 255, 255, Alpha),
                Angle, new Vector2(OriginX, Origin.Y),
                new Vector2(Math.Abs(ScaleFactor.X), Math.Abs(ScaleFactor.Y)), ActiveEffect, DrawingDepth);
        }

        public void DrawUI(CustomSpriteBatch g)
        {
            g.End();
            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            GameScreen.DrawBox(g, new Vector2(0, 0), Width / 2, 84, Color.Red);
            int PosX = 0;
            GameScreen.DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(PosX + 75, 30), 100, 100);
            GameScreen.DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(PosX + 75, 50), 100, 100);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(PosX + 40, 20), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "5000/5000", new Vector2(PosX + 242, 17), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(PosX + 40, 40), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "200/200", new Vector2(PosX + 242, 37), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(PosX + 7, 30, 32, 32), Color.White);

            PosX = Width / 2 + 68;
            GameScreen.DrawBox(g, new Vector2(Width / 2, 0), Width / 2, 84, Color.Blue);
            GameScreen.DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(PosX + 75, 30), 100, 100);
            GameScreen.DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(PosX + 75, 50), 100, 100);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(PosX + 40, 20), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "5000/5000", new Vector2(PosX + 242, 17), Color.White);
            g.DrawString(fntFinlanderFont, "EN", new Vector2(PosX + 40, 40), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, "200/200", new Vector2(PosX + 242, 37), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle(PosX + 7, 30, 32, 32), Color.White);
            g.Draw(sprInfinity, new Vector2((Width - sprInfinity.Width) / 2, 15), Color.White);

            GameScreen.DrawBox(g, new Vector2(0, Constants.Height - AnimationClass.VNBoxHeight), Width, AnimationClass.VNBoxHeight, Color.White);

            g.End();
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
        }

    [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return Constants.Width; } }

        [CategoryAttribute("Animated Bitmap Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return 84; } }
    }
}
