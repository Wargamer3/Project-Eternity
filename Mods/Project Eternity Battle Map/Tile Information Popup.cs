using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TileInformationPopupManager
    {
        private struct TileInformationPopup
        {
            public double TimeOffset;
            public MovementAlgorithmTile CurrentTile;
            public Vector2 PopupPosition;

            public TileInformationPopup(ILayerHolderDrawable LayerHolder, double TimeOffset, MovementAlgorithmTile CurrentTile)
            {
                this.TimeOffset = TimeOffset;
                this.CurrentTile = CurrentTile;

                Point TileScreenPosition = LayerHolder.GetVisiblePosition(CurrentTile.WorldPosition);
                PopupPosition = new Vector2(TileScreenPosition.X, TileScreenPosition.Y);
            }
        }

        private Texture2D sprCircle;

        BattleMap Map;
        LayerHolder LayerHolder;

        private List<TileInformationPopup> ListTilePopup;
        private double CurrentTime;

        private float OffsetBetweenPopup = 0.3f;

        private float CirclePhase1LengthInSeconds = 0.3f;
        private float CirclePhase2LengthInSeconds = 0.0f;
        private float CirclePhase3LengthInSeconds = 0.2f;
        private float CirclePhase4LengthInSeconds = 0.2f;

        private double CirclePhaseLengthInSeconds;

        public TileInformationPopupManager(BattleMap Map, LayerHolder LayerHolder)
        {
            this.Map = Map;
            this.LayerHolder = LayerHolder;

            CirclePhaseLengthInSeconds = CirclePhase1LengthInSeconds + CirclePhase2LengthInSeconds + CirclePhase3LengthInSeconds + CirclePhase4LengthInSeconds + 4;

            ListTilePopup = new List<TileInformationPopup>();
        }
        public void Load(ContentManager Content)
        {
            sprCircle = Content.Load<Texture2D>("Circle");
        }

        public void SetPopups(List<Vector3> ListPosition)
        {
            ListTilePopup.Clear();

            double CurrentOffset = CurrentTime;

            foreach (Vector3 ActivePosition in ListPosition)
            {
                ListTilePopup.Add(new TileInformationPopup(LayerHolder.LayerHolderDrawable, CurrentOffset, Map.GetMovementTile((int)ActivePosition.X, (int)ActivePosition.Y, (int)ActivePosition.Z)));
                CurrentOffset += OffsetBetweenPopup;
            }
        }

        public void Update(GameTime gameTime)
        {
            CirclePhaseLengthInSeconds = CirclePhase1LengthInSeconds + CirclePhase2LengthInSeconds + CirclePhase3LengthInSeconds + CirclePhase4LengthInSeconds + 4;

            CurrentTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (CurrentTime > CirclePhaseLengthInSeconds)
            {
                CurrentTime -= CirclePhaseLengthInSeconds;
            }
        }

        public void Draw(CustomSpriteBatch g)
        {
            foreach (TileInformationPopup ActivePopup in ListTilePopup)
            {
                float PopupTime = (float)(CurrentTime - ActivePopup.TimeOffset);

                if (PopupTime > 0)
                {
                    if (PopupTime <= CirclePhase1LengthInSeconds)
                    {
                        DrawCircleGettingSmaller(g, ActivePopup, PopupTime);
                    }
                    else if (PopupTime <= CirclePhase1LengthInSeconds + CirclePhase2LengthInSeconds)
                    {
                        DrawPulsingCircle(g, ActivePopup, PopupTime);
                    }
                    else if (PopupTime <= CirclePhase1LengthInSeconds + CirclePhase2LengthInSeconds + CirclePhase3LengthInSeconds)
                    {
                        DrawLine(g, ActivePopup, PopupTime);
                    }
                    else
                    {
                        DrawPopup(g, ActivePopup, PopupTime);
                    }
                }
            }
        }

        private void DrawCircleGettingSmaller(CustomSpriteBatch g, TileInformationPopup ActiveTile, float CurrentTime)
        {
            float Progress = CurrentTime / CirclePhase1LengthInSeconds;
            float Scale = 100 * (1 - Progress) + 1;
            byte Alpha = (byte)((1 - Math.Cos(Progress * MathHelper.PiOver2)) * 255);

            g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, (int)(10 * Scale), (int)(10 * Scale)), null, Color.FromNonPremultiplied(255, 255, 255, Alpha), 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);

            int LineScale = 6 - (int)(Progress * 6);
            g.DrawLine(GameScreen.sprPixel, new Vector2(0, ActiveTile.PopupPosition.Y), new Vector2(ActiveTile.PopupPosition.X - Scale * 5 - LineScale, ActiveTile.PopupPosition.Y), Color.FromNonPremultiplied(255, 255, 255, Alpha), LineScale);
            g.DrawLine(GameScreen.sprPixel, new Vector2(ActiveTile.PopupPosition.X + Scale * 5, ActiveTile.PopupPosition.Y), new Vector2(Constants.Width, ActiveTile.PopupPosition.Y), Color.FromNonPremultiplied(255, 255, 255, Alpha), LineScale);

            g.DrawLine(GameScreen.sprPixel, new Vector2(ActiveTile.PopupPosition.X, 0), new Vector2(ActiveTile.PopupPosition.X, ActiveTile.PopupPosition.Y - Scale * 5 - LineScale), Color.FromNonPremultiplied(255, 255, 255, Alpha), LineScale);
            g.DrawLine(GameScreen.sprPixel, new Vector2(ActiveTile.PopupPosition.X, ActiveTile.PopupPosition.Y + Scale * 5), new Vector2(ActiveTile.PopupPosition.X, Constants.Height), Color.FromNonPremultiplied(255, 255, 255, Alpha), LineScale);
        }

        private void DrawPulsingCircle(CustomSpriteBatch g, TileInformationPopup ActiveTile, float CurrentTime)
        {
            float Progress = (CurrentTime - CirclePhase1LengthInSeconds) / CirclePhase2LengthInSeconds;

            g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 10, 10), null, Color.White, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);

            if (Progress <= CirclePhase2LengthInSeconds * 0.25f)
            {
                g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 5, 5), null, Color.FromNonPremultiplied(0, 0, 0, 127), 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);
            }
            else if (Progress <= CirclePhase2LengthInSeconds * 0.5f)
            {
                g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 5, 5), null, Color.Black, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);
            }
            else if (Progress <= CirclePhase2LengthInSeconds * 0.8f)
            {
                g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 5, 5), null, Color.FromNonPremultiplied(0, 0, 0, 127), 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);
            }
            else
            {
                g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 5, 5), null, Color.Black, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);
            }
        }

        private void DrawLine(CustomSpriteBatch g, TileInformationPopup ActiveTile, float CurrentTime)
        {
            float Progress = (CurrentTime - CirclePhase1LengthInSeconds - CirclePhase2LengthInSeconds) / CirclePhase3LengthInSeconds;

            g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 10, 10), null, Color.White, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);
            g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 5, 5), null, Color.Black, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);

            Vector2 Destination = new Vector2(30 * Progress, -30 * Progress);

            g.DrawLine(GameScreen.sprPixel, ActiveTile.PopupPosition + new Vector2(0, -2), ActiveTile.PopupPosition + Destination, Color.Blue, 3);
            g.DrawLine(GameScreen.sprPixel, ActiveTile.PopupPosition + new Vector2(0, -2), ActiveTile.PopupPosition + Destination, Color.Black, 1);
        }

        private void DrawPopup(CustomSpriteBatch g, TileInformationPopup ActiveTile, float CurrentTime)
        {
            float Progress = (CurrentTime - CirclePhase1LengthInSeconds - CirclePhase2LengthInSeconds - CirclePhase3LengthInSeconds) / CirclePhase4LengthInSeconds;

            g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 10, 10), null, Color.White, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);
            g.Draw(sprCircle, new Rectangle((int)ActiveTile.PopupPosition.X, (int)ActiveTile.PopupPosition.Y, 5, 5), null, Color.Black, 0f, new Vector2(sprCircle.Width / 2, sprCircle.Height / 2), SpriteEffects.None, 0f);

            Vector2 Destination = ActiveTile.PopupPosition + new Vector2(30, -30);

            g.DrawLine(GameScreen.sprPixel, ActiveTile.PopupPosition + new Vector2(0, -2), Destination, Color.Blue, 3);
            g.DrawLine(GameScreen.sprPixel, ActiveTile.PopupPosition + new Vector2(0, -2), Destination, Color.Black, 1);

            if (Progress < 1)
            {
                GameScreen.DrawBox(g, Destination + new Vector2(), 100, (int)(50 * Progress), Color.White);
            }
            else
            {
                GameScreen.DrawBox(g, Destination + new Vector2(), 100, 50, Color.White);
                TextHelper.DrawText(g, "Water tile", Destination + new Vector2(5, 5), Color.White);
            }
        }
    }
}
