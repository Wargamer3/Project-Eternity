using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.Core.Item
{
    public class AnimatedSprite
    {
        public float FramesPerSecond;
        public Vector2 Position;
        public Vector2 Origin;

        private double AnimationSpritePosition;
        private int AnimationSpriteRealPosition;
        public Texture2D ActiveSprite;
        public int SpriteWidth;
        public int SpriteHeight;
        private Rectangle[] ArraySpriteSource;

        public bool IsOnLastFrame { get { return AnimationSpriteRealPosition == ArraySpriteSource.Length - 1; } }
        public bool AnimationEnded { get { return AnimationSpriteRealPosition >= ArraySpriteSource.Length; } }

        protected AnimatedSprite()
        {
            AnimationSpritePosition = 0d;
        }

        public AnimatedSprite(Microsoft.Xna.Framework.Content.ContentManager Content, string Path, Vector2 AnimationPosition, float FramesPerSecond = 30f)
            : this()
        {
            ActiveSprite = Content.Load<Texture2D>(Path);
            this.Position = AnimationPosition;
            this.FramesPerSecond = FramesPerSecond;

            ParseFileName(Path);
        }

        public AnimatedSprite(Microsoft.Xna.Framework.Content.ContentManager Content, string Path, Vector2 AnimationPosition, float FramesPerSecond, int NumberOfLines, int ImagesPerLines)
            : this()
        {
            ActiveSprite = Content.Load<Texture2D>(Path);
            this.Position = AnimationPosition;
            this.FramesPerSecond = FramesPerSecond;

            CreateSpriteSource(NumberOfLines, ImagesPerLines);
        }

        protected void ParseFileName(string FileName)
        {
            int NumberOfLines = 1;
            int ImagesPerLines = 0;
            int StripIndex = FileName.IndexOf("_strip");
            if (StripIndex == -1)
            {
                ImagesPerLines = 1;
            }
            else
            {
                StripIndex += 6;
                string ImageInformation = FileName.Substring(StripIndex);

                //Check if the animation is split on multiple lines
                int StripIndexMultiline = ImageInformation.IndexOf("_");
                string[] Parameters = ImageInformation.Split(new string[1] { "_" }, StringSplitOptions.RemoveEmptyEntries);
                if (StripIndexMultiline >= 0)
                {
                    NumberOfLines = Convert.ToInt32(Parameters[1]);
                    ImagesPerLines = Convert.ToInt32(Parameters[0]) / NumberOfLines;
                }
                else
                {
                    ImagesPerLines = Convert.ToInt32(ImageInformation);
                }
            }

            CreateSpriteSource(NumberOfLines, ImagesPerLines);
        }

        protected void CreateSpriteSource(int NumberOfLines, int ImagesPerLines)
        {
            SpriteWidth = (int)Math.Ceiling(ActiveSprite.Width / (float)ImagesPerLines);
            SpriteHeight = ActiveSprite.Height / NumberOfLines;

            Origin = new Vector2(SpriteWidth / 2, SpriteHeight / 2);

            ArraySpriteSource = new Rectangle[ImagesPerLines * NumberOfLines];
            for (int Y = 0; Y < NumberOfLines; ++Y)
            {
                for (int X = 0; X < ImagesPerLines; ++X)
                {
                    ArraySpriteSource[Y * ImagesPerLines + X] = new Rectangle(X * SpriteWidth, Y * SpriteHeight, SpriteWidth, SpriteHeight);
                }
            }
        }

        public AnimatedSprite Copy()
        {
            AnimatedSprite NewAnimatedSprite = new AnimatedSprite();

            NewAnimatedSprite.ActiveSprite = ActiveSprite;
            NewAnimatedSprite.FramesPerSecond = FramesPerSecond;
            NewAnimatedSprite.Position = Position;
            NewAnimatedSprite.Origin = Origin;

            NewAnimatedSprite.AnimationSpritePosition = AnimationSpritePosition;
            NewAnimatedSprite.AnimationSpriteRealPosition = AnimationSpriteRealPosition;
            NewAnimatedSprite.SpriteWidth = SpriteWidth;
            NewAnimatedSprite.SpriteHeight = SpriteHeight;
            NewAnimatedSprite.ArraySpriteSource = ArraySpriteSource;

            return NewAnimatedSprite;
        }

        public void Update(GameTime gameTime)
        {
            AnimationSpritePosition += FramesPerSecond * gameTime.ElapsedGameTime.TotalSeconds;
            AnimationSpriteRealPosition = (int)Math.Truncate(AnimationSpritePosition);
        }

        public void Update(float Progress)
        {
            AnimationSpritePosition += Progress;
            AnimationSpriteRealPosition = (int)Math.Truncate(AnimationSpritePosition);
        }

        public void EndAnimation()
        {
            AnimationSpritePosition = 0d;
            AnimationSpriteRealPosition = ArraySpriteSource.Length + 1;
        }

        public void RestartAnimation()
        {
            AnimationSpritePosition = 0d;
            AnimationSpriteRealPosition = 0;
        }

        public void LoopAnimation()
        {
            AnimationSpriteRealPosition -= ArraySpriteSource.Length;
            AnimationSpritePosition = AnimationSpriteRealPosition;
        }

        public void SetFrame(int NewFrame)
        {
            AnimationSpritePosition = NewFrame;
            AnimationSpriteRealPosition = NewFrame;
        }

        public void SetRandomFrame()
        {
            int NewFrame = RandomHelper.Next(ArraySpriteSource.Length);
            AnimationSpritePosition = NewFrame;
            AnimationSpriteRealPosition = NewFrame;
        }

        public int GetFrame()
        {
            return AnimationSpriteRealPosition;
        }

        public Rectangle GetCollisionBox()
        {
            Rectangle CollisionBox = ArraySpriteSource[AnimationSpriteRealPosition];

            CollisionBox.Location = new Point((int)(Position.X - Origin.X), (int)(Position.Y - Origin.Y));

            return CollisionBox;
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.Draw(ActiveSprite, Position, ArraySpriteSource[AnimationSpriteRealPosition], Color.White, 0, Origin, 1, SpriteEffects.None, 0);
        }

        public void Draw(CustomSpriteBatch g, Vector2 Position, Color DrawingColor)
        {
            g.Draw(ActiveSprite, Position, ArraySpriteSource[AnimationSpriteRealPosition], DrawingColor, 0, Origin, 1, SpriteEffects.None, 0);
        }

        public void Draw(CustomSpriteBatch g, Rectangle Position, Color DrawingColor)
        {
            g.Draw(ActiveSprite, Position, ArraySpriteSource[AnimationSpriteRealPosition], DrawingColor, 0, Origin, SpriteEffects.None, 0);
        }

        public void Draw(CustomSpriteBatch g, Vector2 Position, Color DrawingColor, float Depth)
        {
            g.Draw(ActiveSprite, Position, ArraySpriteSource[AnimationSpriteRealPosition], DrawingColor, 0, Origin, 1, SpriteEffects.None, Depth);
        }

        public void Draw(CustomSpriteBatch g, Vector2 Position, Color DrawingColor, float Angle, float Depth)
        {
            g.Draw(ActiveSprite, Position, ArraySpriteSource[AnimationSpriteRealPosition], DrawingColor, Angle, Origin, 1, SpriteEffects.None, Depth);
        }

        public void Draw(CustomSpriteBatch g, Vector2 Position, Color DrawingColor, float Angle, float Depth, Vector2 Scale, SpriteEffects Effects)
        {
            g.Draw(ActiveSprite, Position, ArraySpriteSource[AnimationSpriteRealPosition], DrawingColor, Angle, Origin, Scale, Effects, Depth);
        }
    }
}
