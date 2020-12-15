using System;
using System.IO;
using ProjectEternity.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationBackground2DImageComplex : AnimationBackground2DBase
    {
        public const string BackgroundTypeName = "Object";

        public string Path;
        public AnimationBackgroundObject2D BackgroundChain;

        public AnimationBackground2DImageComplex(ContentManager Content, string Path)
            : base(BackgroundTypeName)
        {
            this.Path = Path;
            BackgroundChain = new AnimationBackgroundObject2D(Content, Path);
        }

        public AnimationBackground2DImageComplex(ContentManager Content, BinaryReader BR)
            : base(BackgroundTypeName)
        {
            Path = BR.ReadString();
            BackgroundChain = new AnimationBackgroundObject2D(Content, Path);
            LoadBase(BR);
        }

        public AnimationBackground2DImageComplex(AnimationBackgroundObject2D BackgroundChain)
            : base(BackgroundTypeName)
        {
            this.BackgroundChain = BackgroundChain;

            RepeatX = false;
            RepeatY = false;
            Depth = 0.5f;
            UseParallaxScrolling = false;
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

            return FinalX <= Position.X && FinalX + BackgroundChain.sprBackground.Width >= Position.X
                && FinalY <= Position.Y && FinalY + BackgroundChain.sprBackground.Height >= Position.Y;
        }

        public override void Draw(CustomSpriteBatch g, float CameraX, float CameraY, int ScreenWidth, int ScreenHeight)
        {
            float FinalX = SpriteCenter.X;
            if (_UseParallaxScrolling)
                FinalX += ((CurrentPosition.X - CameraX) * (1 - _Depth)) % ScreenWidth;
            else
                FinalX += (CurrentPosition.X - CameraX) % ScreenWidth;

            float FinalY = SpriteCenter.Y;
            if (_UseParallaxScrolling)
                FinalY += ((CurrentPosition.Y - CameraY) * (1 - _Depth));
            else
                FinalY += (CurrentPosition.Y - CameraY) % ScreenHeight;

            int StartX = 0;
            int RepeatXNumber = 1;
            if (_RepeatX)
            {
                StartX = -1;
                RepeatXNumber = (int)Math.Ceiling(ScreenWidth / (double)BackgroundChain.sprBackground.Width) + 1;
            }

            int StartY = 0;
            int RepeatYNumber = 1;
            if (_RepeatY)
            {
                StartY = -1;
                RepeatYNumber = (int)Math.Ceiling(ScreenHeight / (double)BackgroundChain.sprBackground.Height) + 1;
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

                    Vector2 FinalPos = new Vector2(FinalX + X * BackgroundChain.sprBackground.Width, FinalY + Y * BackgroundChain.sprBackground.Height);
                    g.Draw(BackgroundChain.sprBackground,
                        FinalPos,
                        null, _Color, 0, SpriteCenter, 1, FlipEffect, _Depth);

                    foreach (AnimationBackgroundLink ActiveLink in BackgroundChain.ListBackgroundLink)
                    {
                        ActiveLink.Draw(g, FinalPos, _Depth, ScreenWidth, ScreenHeight);
                    }

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
