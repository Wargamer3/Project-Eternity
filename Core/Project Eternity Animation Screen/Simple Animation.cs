using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class SimpleAnimation
    {
        public string Name;
        public bool IsAnimated;
        public string Path;
        public Texture2D StaticSprite;
        public AnimatedSprite ActualSprite;
        public AnimationLooped ActiveAnimation;
        public Vector2 Position;
        public Vector2 Origin;
        private Vector2 _Scale;
        protected Vector2 AbsoluteScale;
        protected SpriteEffects ScaleEffect;
        public float Depth;
        public float Angle;
        public bool IsLooped;
        public bool HasEnded;

        public SimpleAnimation()
        {
            Name = string.Empty;
            IsAnimated = false;
            Path = string.Empty;
            StaticSprite = null;
            ActualSprite = null;
            ActiveAnimation = null;
            Position = Vector2.Zero;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Depth = 0;
            Angle = 0;
            IsLooped = true;
            HasEnded = false;
        }

        public SimpleAnimation(string Path, ContentManager Content)
            : this()
        {
            this.Name = Path;
            this.Path = Path;
            IsAnimated = false;
            if (Path.Contains("strip"))
                ActualSprite = new AnimatedSprite(Content, Path, Vector2.Zero);
            else
                StaticSprite = Content.Load<Texture2D>(Path);

            Rectangle Size = PositionRectangle;
            Origin = new Vector2(Size.Width / 2, Size.Height / 2);
        }

        public SimpleAnimation(string Name, string Path, Texture2D Sprite)
            : this()
        {
            this.Name = Name;
            IsAnimated = false;
            this.Path = Path;
            this.StaticSprite = Sprite;
        }

        public SimpleAnimation(string Name, string Path, AnimationLooped ActiveAnimation)
            : this()
        {
            this.Name = Name;
            this.Path = Path;
            IsAnimated = true;
            this.ActiveAnimation = ActiveAnimation;
        }

        public SimpleAnimation(BinaryReader BR, bool ReadScale)
            : this()
        {
            IsAnimated = BR.ReadBoolean();
            Path = BR.ReadString();
            Name = Path;
            if (ReadScale)
            {
                Scale = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }
        }

        public SimpleAnimation(BinaryReader BR, ContentManager Content, string TexturePathPrefix)
            : this()
        {
            IsAnimated = BR.ReadBoolean();
            Name = BR.ReadString();
            Path = BR.ReadString();

            if (IsAnimated)
            {
                ActiveAnimation = new AnimationLooped(Path);
                ActiveAnimation.Content = Content;
                foreach (KeyValuePair<string, Timeline> Timeline in AnimationClass.LoadAllTimelines())
                    ActiveAnimation.DicTimeline.Add(Timeline.Key, Timeline.Value);
                ActiveAnimation.Load();
            }
            else
            {
                if (Path.Contains("strip"))
                {
                    ActualSprite = new AnimatedSprite(Content, Path, Vector2.Zero);
                }
                else
                {
                    StaticSprite = Content.Load<Texture2D>(TexturePathPrefix + Path);
                }

                Rectangle Size = PositionRectangle;
                Origin = new Vector2(Size.Width / 2, Size.Height / 2);
            }
        }

        public SimpleAnimation(SimpleAnimation Copy)
        {
            IsAnimated = Copy.IsAnimated;
            Name = Copy.Name;
            Path = Copy.Path;

            if (Copy.IsAnimated)
            {
                ActiveAnimation = new AnimationLooped(Copy.Path);
                ActiveAnimation.Content = Copy.ActiveAnimation.Content;
                ActiveAnimation.DicTimeline = Copy.ActiveAnimation.DicTimeline;
                ActiveAnimation.Load();
                ActiveAnimation.UpdateKeyFrame(0);
            }
            else
            {
                StaticSprite = Copy.StaticSprite;
            }
        }

        public void Load(ContentManager Content, string TexturePathPrefix)
        {
            if (IsAnimated)
            {
                ActiveAnimation = new AnimationLooped(Path);
                ActiveAnimation.Content = Content;
                foreach (KeyValuePair<string, Timeline> Timeline in AnimationClass.LoadAllTimelines())
                    ActiveAnimation.DicTimeline.Add(Timeline.Key, Timeline.Value);
                ActiveAnimation.Load();
            }
            else
            {
                if (Path.Contains("strip"))
                    ActualSprite = new AnimatedSprite(Content, "Animations/Sprites/" + Path, Vector2.Zero);
                else
                    StaticSprite = Content.Load<Texture2D>(TexturePathPrefix + Path);
            }

            Rectangle Size = PositionRectangle;
            Origin = new Vector2(Size.Width / 2, Size.Height / 2);
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(IsAnimated);
            BW.Write(Name);
            BW.Write(_Scale.X);
            BW.Write(_Scale.Y);
        }

        public SimpleAnimation Copy()
        {
            SimpleAnimation NewSimpleAnimation = new SimpleAnimation();
            NewSimpleAnimation.Name = Name;
            NewSimpleAnimation.IsAnimated = IsAnimated;
            NewSimpleAnimation.Path = Path;
            NewSimpleAnimation.Position = Position;
            NewSimpleAnimation.Depth = Depth;
            NewSimpleAnimation.Angle = Angle;
            NewSimpleAnimation.IsLooped = IsLooped;
            NewSimpleAnimation.Origin = Origin;

            if (IsAnimated)
            {
                NewSimpleAnimation.ActiveAnimation = (AnimationLooped)ActiveAnimation.Copy();
            }
            else
            {
                if (StaticSprite != null)
                {
                    NewSimpleAnimation.StaticSprite = StaticSprite;
                }
                else if (ActualSprite != null)
                {
                    NewSimpleAnimation.ActualSprite = ActualSprite.Copy();
                }
            }

            return NewSimpleAnimation;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsAnimated)
            {
                ActiveAnimation.Update(gameTime);
                if (ActiveAnimation.HasLooped)
                {
                    HasEnded = true;
                }
            }
            else if (ActualSprite != null)
            {
                ActualSprite.Update(gameTime);

                if (ActualSprite.AnimationEnded)
                {
                    ActualSprite.LoopAnimation();
                    HasEnded = !IsLooped;
                }
            }
            else
            {
                HasEnded = !IsLooped;
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            if (IsAnimated)
            {
                ActiveAnimation.TransformationMatrix = Matrix.CreateTranslation(-ActiveAnimation.AnimationOrigin.Position.X, -ActiveAnimation.AnimationOrigin.Position.Y, 0)
                                                        * Matrix.CreateRotationZ(Angle)
                                                        * Matrix.CreateScale(Scale.X, Scale.Y, 1f)
                                                        * Matrix.CreateTranslation(Position.X,
                                                                                   Position.Y, 0);
                ActiveAnimation.BeginDraw(g);
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            Draw(g, Position);
        }

        public virtual void Draw(CustomSpriteBatch g, Vector2 Position)
        {
            if (IsAnimated)
            {
                ActiveAnimation.TransformationMatrix = Matrix.CreateTranslation(-ActiveAnimation.AnimationOrigin.Position.X, -ActiveAnimation.AnimationOrigin.Position.Y, 0)
                                                        * Matrix.CreateRotationZ(Angle)
                                                        * Matrix.CreateScale(Scale.X, Scale.Y, 1f)
                                                        * Matrix.CreateTranslation(Position.X,
                                                                                   Position.Y, 0);
                ActiveAnimation.Draw(g);
            }
            else if (StaticSprite != null)
            {
                g.Draw(StaticSprite, Position, new Rectangle(0, 0, StaticSprite.Width, StaticSprite.Height), Color.White, Angle, Origin, AbsoluteScale, ScaleEffect, Depth);
            }
            else
            {
                ActualSprite.Draw(g, Position, Color.White, Angle, Depth);
            }
        }

        public bool IsSameAnimation(SimpleAnimation OtherCharacter)
        {
            if (Name == OtherCharacter.Name && Path == OtherCharacter.Path && IsAnimated == OtherCharacter.IsAnimated)
                return true;

            return false;
        }

        public override string ToString()
        {
            return Name;
        }

        public Rectangle PositionRectangle
        {
            get
            {
                if (IsAnimated)
                {
                    int MinX = 999999, MinY = 999999, MaxX = 0, MaxY = 0;

                    for (int A = ActiveAnimation.ListAnimationLayer.Count - 1; A >= 0; --A)
                    {
                        AnimationClass.AnimationLayer ActiveLayer = ActiveAnimation.ListAnimationLayer[A];
                        foreach (var ActiveObject in ActiveLayer.ListVisibleObject)
                        {
                            int ObjectMinX;
                            int ObjectMinY;
                            int ObjectMaxX;
                            int ObjectMaxY;
                            ActiveObject.GetMinMax(out ObjectMinX, out ObjectMinY, out ObjectMaxX, out ObjectMaxY);

                            if (ObjectMinX < MinX)
                                MinX = ObjectMinX;
                            if (ObjectMaxX > MaxX)
                                MaxX = ObjectMaxX;

                            if (ObjectMinY < MinY)
                                MinY = ObjectMinY;
                            if (ObjectMaxY > MaxY)
                                MaxY = ObjectMaxY;
                        }
                    }

                    int PositionX = (int)Position.X - (int)ActiveAnimation.AnimationOrigin.Position.X;
                    int PositionY = (int)Position.Y - (int)ActiveAnimation.AnimationOrigin.Position.Y;

                    return new Rectangle(PositionX + MinX, PositionY + MinY, MaxX - MinX, MaxY - MinY);
                }
                else if (ActualSprite != null)
                {
                    return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), ActualSprite.SpriteWidth, ActualSprite.SpriteHeight);
                }
                else
                {
                    return new Rectangle((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y), StaticSprite.Width, StaticSprite.Height);
                }
            }
        }

        public Vector2 Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
                AbsoluteScale = new Vector2(Math.Abs(_Scale.X), Math.Abs(_Scale.Y));
                ScaleEffect = SpriteEffects.None;
                if (_Scale.X < 0)
                    ScaleEffect = SpriteEffects.FlipHorizontally;
                if (_Scale.Y < 0)
                    ScaleEffect |= SpriteEffects.FlipVertically;
            }
        }
    }

    public class AnimationLooped : AnimationClass
    {
        public bool HasLooped;

        public AnimationLooped(string AnimationPath)
            : base(AnimationPath)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            IsOnTop = false;
            HasLooped = false;
        }

        public override void Load()
        {
            base.Load();
            InitRessources();
        }

        public override AnimationClass Copy()
        {
            AnimationLooped NewAnimationClass = new AnimationLooped(AnimationPath);

            NewAnimationClass.UpdateFrom(this);

            return NewAnimationClass;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (LoopEnd > 0 && ActiveKeyFrame >= LoopEnd)
            {
                HasLooped = true;
                ActiveKeyFrame = LoopStart;
                CurrentQuote = "";
                ListActiveSFX.Clear();

                for (int L = 0; L < ListAnimationLayer.Count; L++)
                    ListAnimationLayer[L].ResetAnimationLayer();

                for (int F = 0; F <= ActiveKeyFrame; F++)
                    UpdateKeyFrame(F);
            }
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                if (ListAnimationLayer[L].renderTarget == null ||
                    ListAnimationLayer[L].renderTarget.Width != GameScreen.GraphicsDevice.PresentationParameters.BackBufferWidth ||
                    ListAnimationLayer[L].renderTarget.Height != GameScreen.GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                        GraphicsDevice,
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
                }

                GameScreen.GraphicsDevice.SetRenderTarget(ListAnimationLayer[L].renderTarget);
                GameScreen.GraphicsDevice.Clear(Color.Transparent);

                DrawLayer(g, ListAnimationLayer[L], false, null);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int L = ListAnimationLayer.Count - 1; L >= 0; --L)
            {
                g.Draw(ListAnimationLayer[L].renderTarget, Vector2.Zero, Color.White);
            }
        }
    }

    public class MovingSimpleAnimation : SimpleAnimation
    {
        private double TimeAliveInSeconds;
        private double TimeAliveInSecondsRemaining;
        private Vector2 Speed;
        private Vector2 Gravity;

        public MovingSimpleAnimation(double TimeAliveInSeconds, Vector2 Position, Vector2 Speed, Vector2 Gravity, float Angle)
        {
            this.TimeAliveInSeconds = TimeAliveInSecondsRemaining = TimeAliveInSeconds;
            this.Position = Position;
            this.Speed = Speed;
            this.Gravity = Gravity;
            this.Angle = Angle;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Position += Speed;
            Speed += Gravity;

            TimeAliveInSecondsRemaining -= gameTime.ElapsedGameTime.TotalSeconds;
            if (TimeAliveInSecondsRemaining <= 0)
            {
                HasEnded = true;
            }
        }

        public override void Draw(CustomSpriteBatch g, Vector2 Position)
        {
            if (IsAnimated)
            {
                ActiveAnimation.TransformationMatrix = Matrix.CreateTranslation(-ActiveAnimation.AnimationOrigin.Position.X, -ActiveAnimation.AnimationOrigin.Position.Y, 0)
                                                        * Matrix.CreateRotationZ(Angle)
                                                        * Matrix.CreateScale(Scale.X, Scale.Y, 1f)
                                                        * Matrix.CreateTranslation(Position.X,
                                                                                   Position.Y, 0);
                ActiveAnimation.Draw(g);
            }
            else if (StaticSprite != null)
            {
                g.Draw(StaticSprite, Position, new Rectangle(0, 0, StaticSprite.Width, StaticSprite.Height), Color.FromNonPremultiplied(255, 255, 255, 127), Angle, Origin, AbsoluteScale, ScaleEffect, Depth);
            }
            else
            {
                ActualSprite.Draw(g, Position, Color.FromNonPremultiplied(255, 255, 255, (int)(TimeAliveInSecondsRemaining / TimeAliveInSeconds * 127)), Angle, Depth);
            }
        }
    }
}
