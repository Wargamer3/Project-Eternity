using System;
using System.IO;
using ProjectEternity.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground2DImage : AnimationBackground2DBase
    {
        public const string BackgroundTypeName = "Image";

        public Texture2D sprBackground;
        public string Path;

        public AnimationBackground2DImage(ContentManager Content, string Path)
            : base(BackgroundTypeName)
        {
            this.Path = Path;
            sprBackground = Content.Load<Texture2D>("Animations/Background Sprites/" + Path);
            SpriteCenter = new Vector2(sprBackground.Width / 2, sprBackground.Height / 2);
            _RepeatXOffset = sprBackground.Width;
            _RepeatYOffset = sprBackground.Height;
        }

        public AnimationBackground2DImage(ContentManager Content, BinaryReader BR)
            : base(BackgroundTypeName)
        {
            Path = BR.ReadString();
            sprBackground = Content.Load<Texture2D>("Animations/Background Sprites/" + Path);
            SpriteCenter = new Vector2(sprBackground.Width / 2, sprBackground.Height / 2);

            LoadBase(BR);
        }

        protected override void DoSave(BinaryWriter BW)
        {
            BW.Write(Path);
            base.DoSave(BW);
        }

        public override bool CollideWith(Vector2 Position, float CameraX, float CameraY, int ScreenWidth, int ScreenHeight)
        {
            float FinalX = 0;
            if (_UseParallaxScrolling)
                FinalX += ((CurrentPosition.X - CameraX) * (1 - _Depth)) % ScreenWidth;
            else
                FinalX += (CurrentPosition.X - CameraX) % ScreenWidth;

            float FinalY = 0;
            if (_UseParallaxScrolling)
                FinalY += ((CurrentPosition.Y - CameraY) * (1 - _Depth));
            else
                FinalY += (CurrentPosition.Y - CameraY) % ScreenHeight;

            return FinalX <= Position.X && FinalX + sprBackground.Width >= Position.X
                && FinalY <= Position.Y && FinalY + sprBackground.Height >= Position.Y;
        }

        public override void Draw(CustomSpriteBatch g, float CameraX, float CameraY, int ScreenWidth, int ScreenHeight)
        {
            float FinalX = SpriteCenter.X;
            if (_UseParallaxScrolling)
            {
                FinalX += (CurrentPosition.X - CameraX) * (1 - _Depth);
            }
            else
            {
                FinalX += (CurrentPosition.X - CameraX);
            }

            float FinalY = SpriteCenter.Y;
            if (_UseParallaxScrolling)
            {
                FinalY += (CurrentPosition.Y - CameraY) * (1 - _Depth);
            }
            else
            {
                FinalY += (CurrentPosition.Y - CameraY);
            }


            int StartX = 0;
            int RepeatXNumber = 1;
            if (_RepeatX)
            {
                FinalX %= _RepeatXOffset;
                if (FinalX < 0)
                    FinalX += _RepeatXOffset;
                StartX = -1;
                RepeatXNumber = (int)Math.Ceiling(ScreenWidth / (double)_RepeatXOffset) + 1;
            }

            int StartY = 0;
            int RepeatYNumber = 1;
            if (_RepeatY)
            {
                FinalY %= _RepeatYOffset;
                if (FinalY < 0)
                    FinalY += _RepeatYOffset;
                StartY = -1;
                RepeatYNumber = (int)Math.Ceiling(ScreenHeight / (double)sprBackground.Height) + 1;
            }

            bool FlipX = false;
            bool FlipY = false;

            for (int X = StartX; X < RepeatXNumber; ++X)
            {
                for (int Y = StartY; Y < RepeatYNumber; ++Y)
                {
                    SpriteEffects FlipEffect = SpriteEffects.None;

                    if (FlipX)
                    {
                        FlipEffect = SpriteEffects.FlipHorizontally;
                    }

                    if (FlipY)
                    {
                        FlipEffect |= SpriteEffects.FlipVertically;
                    }

                    g.Draw(sprBackground,
                        new Vector2(FinalX + X * _RepeatXOffset, FinalY + Y * _RepeatYOffset),
                        null, _Color, 0, SpriteCenter, 1, FlipEffect, _Depth);

                    if (_FlipOnRepeatY)
                    {
                        FlipY = !FlipY;
                    }
                }

                if (_FlipOnRepeatX)
                {
                    FlipX = !FlipX;
                }
            }
        }

        public override string ToString()
        {
            return Path;
        }
    }
}
