using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class ParticleEmitterTimeline : CoreTimeline
    {
        private const string TimelineType = "Particle Emitter";
        public enum ParticleBlendStates { AlphaBlend, Additive };

        public Particle3DSample.ParticleSystem ParticleSystem;

        private readonly Random Random = new Random();

        private int InternalWidth;
        private int InternalHeight;

        private ParticleBlendStates _ParticleBlendState;
        private Vector2 _SpawnOffset;
        private Vector2 _SpawnOffsetRandom;
        private double _ParticlesPerSeconds;
        private double TimeElapsedSinceLastParticle;
        private double TimeBetweenEachParticle;
        private int RenderWidth;
        private int RenderHeight;

        public ParticleEmitterTimeline()
            : base(TimelineType, "New Particle Emitter")
        {
            Origin = new Point(Width / 2, Height / 2);
            _ParticleBlendState = ParticleBlendStates.Additive;
        }

        public ParticleEmitterTimeline(Particle3DSample.ParticleSystem ParticleSystem)
            : this()
        {
            this.ParticleSystem = ParticleSystem;

            ParticlesPerSeconds = 1;
            TimeBetweenEachParticle = 1 / ParticlesPerSeconds;
            SpawnSpeed = new Vector2(-0.5f, 1.8f);
            SpawnSpeedRandom = new Vector2(1, 0);
            InternalWidth = 32;
            InternalHeight = 32;
            Origin = new Point(Width / 2, Height / 2);
        }

        public ParticleEmitterTimeline(BinaryReader BR, ContentManager Content)
            : base(BR, TimelineType)
        {
            Matrix view = Matrix.Identity;

            Matrix Projection = Matrix.CreateOrthographicOffCenter(0, 800, 600, 0, 0, 1);
            Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            Projection = view * (HalfPixelOffset * Projection);

            Particle3DSample.ParticleSettings ParticleSettings = new Particle3DSample.ParticleSettings();
            ParticleSettings.TextureName = BR.ReadString();
            ParticleSettings.MaxParticles = 20000;
            ParticleSettings.MinScale = new Vector2(1, 1);
            ParticleSettings.DurationInSeconds = 1d;
            ParticleSettings.Gravity = new Vector2(0, 0);
            ParticleSettings.NumberOfImages = 16;
            ParticleSettings.BlendState = BlendState.AlphaBlend;
            ParticleSettings.StartingAlpha = 0.7f;
            ParticleSettings.EndAlpha = 0.1f;

            _SpawnOffset = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            _SpawnOffsetRandom = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            SpawnSpeed = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            SpawnSpeedRandom = new Vector2(BR.ReadSingle(), BR.ReadSingle());

            ParticlesPerSeconds = BR.ReadDouble();
            ParticleSettings.DurationInSeconds = BR.ReadSingle();
            float SpeedMultiplier = BR.ReadSingle();
            ParticleSettings.NumberOfImages = BR.ReadInt32();
            ParticleSettings.Gravity = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            Vector2 Size = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            ParticleSettings.MinScale = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            ParticleSettings.StartingAlpha = BR.ReadSingle();
            ParticleSettings.EndAlpha = BR.ReadSingle();
            _ParticleBlendState = (ParticleBlendStates)BR.ReadByte();

            TimeBetweenEachParticle = 1 / ParticlesPerSeconds;
            ParticleSystem = new Particle3DSample.ParticleSystem(ParticleSettings);
            ParticleSystem.LoadContent(Content, GameScreen.GraphicsDevice, Projection);

            this.SpeedMultiplier = SpeedMultiplier;
            this.Size = Size;
            UpdateSize();

            int DicAnimationSpriteKeyFrameCount = BR.ReadInt32();
            for (int E = 0; E < DicAnimationSpriteKeyFrameCount; E++)
            {
                int Key = BR.ReadInt32();

                VisibleAnimationObjectKeyFrame NewAnimatedBitmapKeyFrame = new VisibleAnimationObjectKeyFrame(BR);

                DicAnimationKeyFrame.Add(Key, NewAnimatedBitmapKeyFrame);
            }

            Origin = new Point(Width / 2, Height / 2);
        }

        protected override VisibleTimeline DoLoadCopy(BinaryReader BR, ContentManager Content, AnimationClass.AnimationLayer ActiveLayer)
        {
            return new ParticleEmitterTimeline(BR, Content);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(ParticleSystem.settings.TextureName);

            BW.Write(SpawnOffset.X);
            BW.Write(SpawnOffset.Y);
            BW.Write(SpawnOffsetRandom.X);
            BW.Write(SpawnOffsetRandom.Y);
            BW.Write(SpawnSpeed.X);
            BW.Write(SpawnSpeed.Y);
            BW.Write(SpawnSpeedRandom.X);
            BW.Write(SpawnSpeedRandom.Y);

            BW.Write(ParticlesPerSeconds);
            BW.Write(DurationInSeconds);
            BW.Write(SpeedMultiplier);
            BW.Write(NumberOfImages);
            BW.Write(Gravity.X);
            BW.Write(Gravity.Y);
            BW.Write(Size.X);
            BW.Write(Size.Y);
            BW.Write(MinScale.X);
            BW.Write(MinScale.Y);
            BW.Write(StartingAlpha);
            BW.Write(EndAlpha);
            BW.Write((byte)_ParticleBlendState);

            BW.Write(DicAnimationKeyFrame.Count);
            foreach (KeyValuePair<int, VisibleAnimationObjectKeyFrame> KeyFrame in DicAnimationKeyFrame)
            {
                BW.Write(KeyFrame.Key);
                KeyFrame.Value.Save(BW);
            }
        }

        public override Timeline Copy(AnimationClass.AnimationLayer ActiveLayer)
        {
            ParticleEmitterTimeline NewSpawnAnimatedBitmapEvent = new ParticleEmitterTimeline(ParticleSystem);

            NewSpawnAnimatedBitmapEvent.UpdateFrom(this, ActiveLayer);

            return NewSpawnAnimatedBitmapEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            AnimatedBitmapSpawnerHelper NewSpawner = new AnimatedBitmapSpawnerHelper();
            if (NewSpawner.ShowDialog() == DialogResult.OK)
            {
                string SpriteName = "Animations/Sprites/" + NewSpawner.SpawnViewer.BitmapName;

                Matrix view = Matrix.Identity;

                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, ActiveLayer.renderTarget.Width, ActiveLayer.renderTarget.Height, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Projection = view * (HalfPixelOffset * Projection);

                Particle3DSample.ParticleSettings ParticleSettings = new Particle3DSample.ParticleSettings();
                ParticleSettings.TextureName = SpriteName;
                ParticleSettings.MaxParticles = 20000;
                ParticleSettings.MinScale = new Vector2(1, 1);
                ParticleSettings.DurationInSeconds = 1d;
                ParticleSettings.Gravity = new Vector2(0, 0);
                ParticleSettings.NumberOfImages = 1;
                ParticleSettings.BlendState = BlendState.AlphaBlend;
                ParticleSettings.StartingAlpha = 0.7f;
                ParticleSettings.EndAlpha = 0.1f;
                int StripIndex = SpriteName.IndexOf("_strip");
                if (StripIndex > 0)
                {
                    StripIndex += 6;
                    string ImageInformation = SpriteName.Substring(StripIndex);
                    ParticleSettings.NumberOfImages = Convert.ToInt32(ImageInformation);
                }
                ParticleSystem = new Particle3DSample.ParticleSystem(ParticleSettings);
                ParticleSystem.LoadContent(NewSpawner.SpawnViewer.content, GameScreen.GraphicsDevice, Projection);

                ParticleEmitterTimeline NewParticleEmitorTimeline = new ParticleEmitterTimeline(ParticleSystem);

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

            ParticleSystem.ClearParticles();
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            double TimeElapsedInSeconds = 1 / 60d;
            TimeElapsedSinceLastParticle += TimeElapsedInSeconds;

            while (TimeElapsedSinceLastParticle >= TimeBetweenEachParticle)
            {
                TimeElapsedSinceLastParticle -= TimeBetweenEachParticle;
                Vector2 SpawnPosition = new Vector2(Position.X + SpawnOffset.X + (float)Random.NextDouble() * SpawnOffsetRandom.X,
                    Position.Y + SpawnOffset.Y + (float)Random.NextDouble() * SpawnOffsetRandom.Y);

                Vector2 ParticleSpeed = new Vector2(SpawnSpeed.X + (float)Random.NextDouble() * SpawnSpeedRandom.X, SpawnSpeed.Y + (float)Random.NextDouble() * SpawnSpeedRandom.Y);

                ParticleSystem.AddParticle(SpawnPosition, ParticleSpeed);
            }

            ParticleSystem.Update(TimeElapsedInSeconds);
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

        private void UpdateSize()
        {
            InternalWidth = (int)Math.Max(32, Math.Abs(_SpawnOffsetRandom.X));
            InternalHeight = (int)Math.Max(32, Math.Abs(_SpawnOffsetRandom.Y));
            if (InternalWidth <= 32)
            {
                Origin.X = InternalWidth / 2 - (int)_SpawnOffset.X;
            }
            else
            {
                Origin.X = InternalWidth - (int)(_SpawnOffset.X + _SpawnOffsetRandom.X);
            }
            if (InternalHeight <= 32)
            {
                Origin.Y = InternalHeight / 2 - (int)_SpawnOffset.Y;
            }
            else
            {
                Origin.Y = InternalHeight - (int)(_SpawnOffset.Y + _SpawnOffsetRandom.Y);
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
                Matrix view = Matrix.Identity;

                Matrix Projection = Matrix.CreateOrthographicOffCenter(0, RenderWidth, RenderHeight, 0, 0, 1);
                Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

                Projection = view * (HalfPixelOffset * Projection);
                ParticleSystem.parameters["ViewProjection"].SetValue(Projection);
            }
            ParticleSystem.Draw(GameScreen.GraphicsDevice, new Vector2());
        }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public override int Width { get { return InternalWidth; } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public override int Height { get { return InternalHeight; } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 SpawnOffset { get { return _SpawnOffset; } set { _SpawnOffset = value; UpdateSize(); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 SpawnOffsetRandom { get { return _SpawnOffsetRandom; } set { _SpawnOffsetRandom = value; UpdateSize(); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 SpawnSpeed { get; set; }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 SpawnSpeedRandom { get; set; }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public double ParticlesPerSeconds
        {
            get { return _ParticlesPerSeconds; }
            set
            {
                _ParticlesPerSeconds = value;
                TimeBetweenEachParticle = 1 / _ParticlesPerSeconds;
            }
        }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public float DurationInSeconds { get { return ParticleSystem.parameters["Duration"].GetValueSingle(); } set { ParticleSystem.parameters["Duration"].SetValue(value); ParticleSystem.settings.DurationInSeconds = value; } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public float SpeedMultiplier { get { return ParticleSystem.parameters["SpeedMultiplier"].GetValueSingle(); } set { ParticleSystem.parameters["SpeedMultiplier"].SetValue(value); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public int NumberOfImages { get { return ParticleSystem.parameters["NumberOfImages"].GetValueInt32(); } set { ParticleSystem.parameters["NumberOfImages"].SetValue(value); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 Gravity { get { return ParticleSystem.parameters["Gravity"].GetValueVector2(); } set { ParticleSystem.parameters["Gravity"].SetValue(value); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 Size { get { return ParticleSystem.parameters["Size"].GetValueVector2(); } set { ParticleSystem.parameters["Size"].SetValue(value); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public Vector2 MinScale { get { return ParticleSystem.settings.MinScale; } set { ParticleSystem.settings.MinScale = value; } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public float StartingAlpha { get { return ParticleSystem.parameters["StartingAlpha"].GetValueSingle(); } set { ParticleSystem.parameters["StartingAlpha"].SetValue(value); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public float EndAlpha { get { return ParticleSystem.parameters["EndAlpha"].GetValueSingle(); } set { ParticleSystem.parameters["EndAlpha"].SetValue(value); } }

        [CategoryAttribute("Particle System Attributes"),
        DescriptionAttribute(".")]
        public ParticleBlendStates ParticleBlendState
        {
            get { return _ParticleBlendState; }
            set
            {
                _ParticleBlendState = value;
                if (_ParticleBlendState == ParticleBlendStates.AlphaBlend)
                    ParticleSystem.settings.BlendState = BlendState.AlphaBlend;
                else if (_ParticleBlendState == ParticleBlendStates.Additive)
                    ParticleSystem.settings.BlendState = BlendState.NonPremultiplied;
            }
        }
    }
}
