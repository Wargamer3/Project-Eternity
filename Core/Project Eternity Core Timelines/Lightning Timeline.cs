using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.LightningSystem;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class LightningTimeline : CoreTimeline
    {
        private const string TimelineType = "Lightning";

        private int InternalWidth;
        private int InternalHeight;

        private int RenderWidth;
        private int RenderHeight;

        LightningBolt LightningGenerator;

        public LightningTimeline()
            : base(TimelineType, "New Lightning Generator")
        {
            Origin = new Point(Width / 2, Height / 2);
        }

        public LightningTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            Matrix view = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = view * (HalfPixelOffset * Projection);

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                VisibleAnimationObjectKeyFrame NewAnimatedBitmapKeyFrame = new VisibleAnimationObjectKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }

            Origin = new Point(Width / 2, Height / 2);
        }

        public LightningTimeline(LightningBolt LightningGenerator)
            : this()
        {
            this.LightningGenerator = LightningGenerator;
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new ParticleEmitterTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            LightningTimeline NewSpawnAnimatedBitmapEvent = new LightningTimeline(LightningGenerator);

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            LightningSpawnerHelper NewSpawner = new LightningSpawnerHelper();
            if (NewSpawner.ShowDialog() == DialogResult.OK)
            {
                Matrix view = Matrix.Identity;

                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, ActiveLayer.renderTarget.Width, ActiveLayer.renderTarget.Height, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Projection = view * (HalfPixelOffset * Projection);

                LightningBolt LightningGenerator = new LightningBolt();
                //LightningGenerator.Init(NewSpawner.SpawnViewer.content, GameScreen.GraphicsDevice);
                LightningGenerator.SetWorldViewProjectionMatrix(Matrix.Identity, Matrix.Identity, Projection);

                LightningTimeline NewParticleEmitorTimeline = new LightningTimeline(LightningGenerator);

                NewParticleEmitorTimeline.Position = new Vector2(535, 170);
                NewParticleEmitorTimeline.SpawnFrame = KeyFrame;
                NewParticleEmitorTimeline.DeathFrame = KeyFrame + 10;
                NewParticleEmitorTimeline.IsUsed = true;//Disable the spawner as we spawn the AnimatedBitmap manually.
                NewParticleEmitorTimeline.Add(
                    KeyFrame, new VisibleAnimationObjectKeyFrame(new Vector2(NewParticleEmitorTimeline.Position.X, NewParticleEmitorTimeline.Position.Y),
                                                    true, -1));

                ReturnValue.Add(NewParticleEmitorTimeline);
            }

            return ReturnValue;
        }

        public override void SpawnItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame)
        {
            base.SpawnItem(ActiveAnimation, ActiveLayer, KeyFrame);
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            double TimeElapsedInSeconds = 1 / 60d;

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
            if (g.GraphicsDevice.PresentationParameters.BackBufferWidth != RenderWidth
                || g.GraphicsDevice.PresentationParameters.BackBufferHeight != RenderHeight)
            {
                RenderWidth = g.GraphicsDevice.PresentationParameters.BackBufferWidth;
                RenderHeight = g.GraphicsDevice.PresentationParameters.BackBufferHeight;
                Matrix View = Matrix.Identity;

                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, RenderWidth, RenderHeight, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Projection = View * (HalfPixelOffset * Projection);
                LightningGenerator.SetWorldViewProjectionMatrix(Matrix.Identity, View, Projection);
            }

            LightningGenerator.Draw(g);
        }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return InternalWidth; } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return InternalHeight; } }
    }
}
