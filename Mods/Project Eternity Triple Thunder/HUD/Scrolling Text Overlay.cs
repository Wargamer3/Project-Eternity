using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public interface ScrollingTextOverlayBase
    {
        string NextLevelPath { get; }

        bool IsFinished { get; }

        void Update(GameTime gameTime);

        void Draw(CustomSpriteBatch g);
    }

    public class ScrollingTextOverlay : ScrollingTextOverlayBase
    {
        private enum AnimationStates { MovingIn, Bouncing, MovingOut, Waiting }
        private readonly Texture2D sprText;

        private AnimationStates AnimationState;
        private const double MovingInInSeconds = 0.5d;
        private const double BouncingInSeconds = 0.15d;
        private const double MovingOutInSeconds = 0.5d;
        private const double WaitingInSeconds = 1d;
        private double ElapsedTime;
        public string NextLevelPath { get; set; }

        public bool IsFinished { get; private set; }

        public ScrollingTextOverlay(Texture2D sprText, string NextLevelPath = null)
        {
            this.sprText = sprText;
            this.NextLevelPath = NextLevelPath;
            AnimationState = AnimationStates.MovingIn;
            IsFinished = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            switch (AnimationState)
            {
                case AnimationStates.MovingIn:
                    if (ElapsedTime >= MovingInInSeconds)
                    {
                        ElapsedTime -= MovingInInSeconds;
                        AnimationState = AnimationStates.Bouncing;
                    }
                    break;

                case AnimationStates.Bouncing:
                    if (ElapsedTime >= BouncingInSeconds)
                    {
                        ElapsedTime -= BouncingInSeconds;
                        AnimationState = AnimationStates.MovingOut;
                    }
                    break;

                case AnimationStates.MovingOut:
                    if (ElapsedTime >= MovingOutInSeconds)
                    {
                        ElapsedTime -= MovingOutInSeconds;
                        AnimationState = AnimationStates.Waiting;
                    }
                    break;

                case AnimationStates.Waiting:
                    if (ElapsedTime >= WaitingInSeconds)
                    {
                        ElapsedTime -= WaitingInSeconds;
                        AnimationState = AnimationStates.MovingIn;
                        IsFinished = true;
                    }
                    break;
            }
        }

        public virtual void Draw(CustomSpriteBatch g)
        {
            int StartX;
            int EndX;
            int Movement;
            int BouncingAmount = 10;

            switch (AnimationState)
            {
                case AnimationStates.MovingIn:
                    StartX = -sprText.Width / 2;
                    EndX = Constants.Width / 2 + BouncingAmount;
                    Movement = EndX - StartX;
                    g.Draw(sprText, new Vector2(StartX + (float)(ElapsedTime / MovingInInSeconds) * Movement, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprText.Width / 2, sprText.Height / 2), 1f, SpriteEffects.None, 0f);
                    break;

                case AnimationStates.Bouncing:
                    StartX = Constants.Width / 2 + BouncingAmount;
                    EndX = Constants.Width / 2 - BouncingAmount;
                    Movement = EndX - StartX;
                    g.Draw(sprText, new Vector2(StartX + (float)(ElapsedTime / BouncingInSeconds) * Movement, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprText.Width / 2, sprText.Height / 2), 1f, SpriteEffects.None, 0f);
                    break;

                case AnimationStates.MovingOut:
                    StartX = Constants.Width / 2 - BouncingAmount;
                    EndX = Constants.Width + sprText.Width / 2;
                    Movement = EndX - StartX;
                    g.Draw(sprText, new Vector2(StartX + (float)(ElapsedTime / MovingOutInSeconds) * Movement, Constants.Height / 2), null, Color.White, 0f, new Vector2(sprText.Width / 2, sprText.Height / 2), 1f, SpriteEffects.None, 0f);
                    break;

                case AnimationStates.Waiting:
                    break;
            }
        }
    }
}
