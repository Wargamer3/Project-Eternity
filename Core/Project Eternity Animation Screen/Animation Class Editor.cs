using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.AnimationScreen
{
    public class AnimationClassEditor : AnimationClass
    {
        public List<Timeline> ListTimelineEvent;
        public List<Timeline> ListSelectedObjects;
        public Rectangle MultipleSelectionRectangle;
        public Point MultipleSelectionOrigin;

        private TimeSpan TotalGameTime = new TimeSpan();
        public const int Width = 640;
        public const int Height = 480;

        public AnimationClassEditor(string AnimationPath)
            : base(AnimationPath)
        {
            ActiveKeyFrame = 0;
            ListTimelineEvent = new List<Timeline>();
            ListSelectedObjects = new List<Timeline>();
        }

        public override void Load()
        {
            foreach (KeyValuePair<string, Timeline> Timeline in LoadAllTimelines())
                DicTimeline.Add(Timeline.Key, Timeline.Value);

            base.Load();

            PolygonEffect = new BasicEffect(GraphicsDevice);

            PolygonEffect.VertexColorEnabled = true;
            PolygonEffect.TextureEnabled = true;
            PolygonEffect.View = Matrix.Identity;
            PolygonEffect.World = Matrix.Identity;

            InitRessources();
        }

        public override AnimationClass Copy()
        {
            return new AnimationClassEditor(AnimationPath);
        }

        public void Update(int Interval)
        {
            TimeSpan EllapsedGameTime = TimeSpan.FromMilliseconds(Interval);
            TotalGameTime += EllapsedGameTime;
            GameTime gameTime = new GameTime(TotalGameTime, EllapsedGameTime);
            base.Update(gameTime);
        }

        public void UpdateProjection(Matrix ProjectionMatrix)
        {
            PolygonEffect.Projection = ProjectionMatrix;
        }

        public void AddLayer(AnimationLayer NewLayer)
        {
            NewLayer.renderTarget = new RenderTarget2D(
                GraphicsDevice,
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

            ListAnimationLayer.Add(NewLayer);
        }

        public void CreateMultipleSelectionRectangle()
        {
            int MinX = 999999, MinY = 999999, MaxX = 0, MaxY = 0;

            foreach (Timeline ActiveObject in ListSelectedObjects)
            {
                VisibleTimeline ActiveVisibleTimeline = ActiveObject as VisibleTimeline;
                if (ActiveVisibleTimeline == null)
                {
                    continue;
                }

                int ObjectMinX;
                int ObjectMinY;
                int ObjectMaxX;
                int ObjectMaxY;
                ActiveVisibleTimeline.GetMinMax(out ObjectMinX, out ObjectMinY, out ObjectMaxX, out ObjectMaxY);

                if (ObjectMinX == ObjectMaxX || ObjectMinY == ObjectMaxY)
                {
                    continue;
                }

                if (ObjectMinX < MinX)
                    MinX = ObjectMinX;
                if (ObjectMaxX > MaxX)
                    MaxX = ObjectMaxX;

                if (ObjectMinY < MinY)
                    MinY = ObjectMinY;
                if (ObjectMaxY > MaxY)
                    MaxY = ObjectMaxY;
            }

            MultipleSelectionRectangle.X = MinX;
            MultipleSelectionRectangle.Y = MinY;
            MultipleSelectionRectangle.Width = MaxX - MinX;
            MultipleSelectionRectangle.Height = MaxY - MinY;
            MultipleSelectionOrigin.X = MultipleSelectionRectangle.Width / 2;
            MultipleSelectionOrigin.Y = MultipleSelectionRectangle.Height / 2;
        }

        public virtual void DrawEditor(CustomSpriteBatch g, int ScreenWidth, int ScreenHeight, bool IsInEditMode, bool ShowBorderBoxes, bool ShowNextPositions)
        {
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.White);

            if (ActiveAnimationBackground != null)
                ActiveAnimationBackground.Draw(g, ScreenWidth, ScreenHeight);

            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                AnimationLayer ActiveLayer = ListAnimationLayer[L];

                if (ActiveLayer.ListPolygonCutter.Count > 0)
                {
                    PolygonEffect.Texture = ActiveLayer.renderTarget;
                    PolygonEffect.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                    for (int P = 0; P < ActiveLayer.ListPolygonCutter.Count; P++)
                    {
                        ActiveLayer.ListPolygonCutter[P].Draw(g, IsInEditMode);
                    }
                }

                if (IsInEditMode || ActiveLayer.ListPolygonCutter.Count <= 0)
                    g.Draw(ActiveLayer.renderTarget, Vector2.Zero, Color.White);
            }

            g.End();

            //Draw HUD
            DrawOverlay(g);
        }

        public void DrawOverlay(CustomSpriteBatch g)
        {
            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            g.Draw(sprPixel, new Rectangle(0, 0, Width, 1), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            g.Draw(sprPixel, new Rectangle(0, 0, 1, Height), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            g.Draw(sprPixel, new Rectangle(0, Height, Width, 1), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);
            g.Draw(sprPixel, new Rectangle(Width, 0, 1, Height), null, Color.Black, 0, Vector2.Zero, SpriteEffects.None, 0);

            if (MultipleSelectionRectangle.Width > 0)
            {
                int RecX = MultipleSelectionRectangle.X;
                int RecY = MultipleSelectionRectangle.Y;
                g.Draw(sprPixel, new Rectangle(RecX, RecY, MultipleSelectionRectangle.Width, 1), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
                g.Draw(sprPixel, new Rectangle(RecX, RecY, 1, MultipleSelectionRectangle.Height), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
                g.Draw(sprPixel, new Rectangle(RecX, RecY + MultipleSelectionRectangle.Height, MultipleSelectionRectangle.Width, 1), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
                g.Draw(sprPixel, new Rectangle(RecX + MultipleSelectionRectangle.Width, RecY, 1, MultipleSelectionRectangle.Height), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);

                g.Draw(sprPixel, new Rectangle(RecX + MultipleSelectionOrigin.X - 2, RecY + MultipleSelectionOrigin.Y - 2, 5, 5), null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0);
            }

            g.End();
        }

        public void DrawLayer(CustomSpriteBatch g, AnimationLayer ActiveLayer, bool IsInEditMode, bool ShowBorderBoxes, bool ShowNextPositions, AnimationLayer Parent, bool DrawChild = true)
        {
            if (!ActiveLayer.IsVisible)
                return;

            if (DrawChild)
            {
                if (ActiveLayer.LayerBlendState == AnimationLayer.LayerBlendStates.Add)
                    g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
                else if (ActiveLayer.LayerBlendState == AnimationLayer.LayerBlendStates.Substract)
                    g.Begin(SpriteSortMode.BackToFront, AnimationClass.NegativeBlendState);
                else
                {
                    GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0, 0);
                    g.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, AlwaysStencilState, null, null);
                    DrawLayer(g, Parent, false, false, false, null, false);

                    g.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, EqualStencilState, null, null);
                    for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                    {
                        ActiveLayer.ListVisibleObject[A].Draw(g, IsInEditMode);
                    }
                }
            }

            if (ActiveLayer.LayerBlendState != AnimationLayer.LayerBlendStates.Merge)
            {
                for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                {
                    ActiveLayer.ListVisibleObject[A].Draw(g, IsInEditMode);
                }
            }

            for (int M = 0; M < ActiveLayer.ListActiveMarker.Count; M++)
            {
                if (ActiveLayer.ListActiveMarker[M].Sprite != null)
                {
                    SpriteEffects ActiveEffect = SpriteEffects.None;
                    if (ActiveLayer.ListActiveMarker[M].ScaleFactor.X < 0)
                        ActiveEffect = SpriteEffects.FlipHorizontally;
                    if (ActiveLayer.ListActiveMarker[M].ScaleFactor.Y < 0)
                        ActiveEffect |= SpriteEffects.FlipVertically;

                    g.Draw(ActiveLayer.ListActiveMarker[M].Sprite, new Vector2(ActiveLayer.ListActiveMarker[M].Position.X, ActiveLayer.ListActiveMarker[M].Position.Y),
                        null, Color.White, ActiveLayer.ListActiveMarker[M].Angle,
                        new Vector2(ActiveLayer.ListActiveMarker[M].Sprite.Width / 2, ActiveLayer.ListActiveMarker[M].Sprite.Height / 2),
                        new Vector2(Math.Abs(ActiveLayer.ListActiveMarker[M].ScaleFactor.X), Math.Abs(ActiveLayer.ListActiveMarker[M].ScaleFactor.Y)),
                        ActiveEffect, ActiveLayer.ListActiveMarker[M].DrawingDepth);
                }
            }

            g.End();

            if (DrawChild)
            {
                for (int L = 0; L < ActiveLayer.ListChildren.Count; L++)
                {
                    DrawLayer(g, ActiveLayer.ListChildren[L], IsInEditMode, ShowBorderBoxes, ShowNextPositions, ActiveLayer);
                }
            }

            g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);

            #region Edit Mode

            if (IsInEditMode)
            {
                for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                {
                    int MinX, MinY, MaxX, MaxY;
                    ActiveLayer.ListVisibleObject[A].GetMinMax(out MinX, out MinY, out MaxX, out MaxY);
                    int VisibleObjectWidth = MaxX - MinX;
                    int VisibleObjectHeight = MaxY - MinY;

                    Color DrawColor = Color.Black;
                    if (ListSelectedObjects.Contains(ActiveLayer.ListVisibleObject[A]))
                    {
                        DrawColor = Color.Red;
                        ActiveLayer.ListVisibleObject[A].DrawExtra(g, sprPixel);
                    }

                    #region Draw next positions

                    if (ShowNextPositions)
                    {
                        ActiveLayer.ListVisibleObject[A].DrawNextPositions(g, sprPixel, DrawColor);
                    }

                    #endregion

                    if (ShowBorderBoxes)
                    {
                        g.Draw(sprPixel, new Rectangle(MinX, MinY, VisibleObjectWidth, 1), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                        g.Draw(sprPixel, new Rectangle(MinX, MinY, 1, VisibleObjectHeight), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                        g.Draw(sprPixel, new Rectangle(MinX, MaxY, VisibleObjectWidth, 1), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                        g.Draw(sprPixel, new Rectangle(MaxX, MinY, 1, VisibleObjectHeight), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }

                    if (ListSelectedObjects.Contains(ActiveLayer.ListVisibleObject[A]) && MultipleSelectionRectangle.Width > 0)
                    {
                        g.Draw(sprPixel, new Rectangle((int)(ActiveLayer.ListVisibleObject[A].Position.X - 2) + 1, (int)(ActiveLayer.ListVisibleObject[A].Position.Y - 2) + 1, 3, 3), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }

                for (int M = 0; M < ActiveLayer.ListActiveMarker.Count; M++)
                {
                    int MinX, MinY, MaxX, MaxY;
                    ActiveLayer.ListActiveMarker[M].GetMinMax(out MinX, out MinY, out MaxX, out MaxY);

                    Color DrawColor = Color.Black;
                    if (ListSelectedObjects.Contains(ActiveLayer.ListActiveMarker[M]))
                        DrawColor = Color.Red;

                    //Draw next positions
                    if (ShowNextPositions)
                    {
                        ActiveLayer.ListActiveMarker[M].DrawNextPositionForMakers(g, sprPixel);
                    }

                    if (ShowBorderBoxes)
                    {
                        g.Draw(sprPixel, new Rectangle(MinX, MinY, ActiveLayer.ListActiveMarker[M].Width, 1), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                        g.Draw(sprPixel, new Rectangle(MinX, MinY, 1, ActiveLayer.ListActiveMarker[M].Height), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                        g.Draw(sprPixel, new Rectangle(MinX, MinY + ActiveLayer.ListActiveMarker[M].Height, ActiveLayer.ListActiveMarker[M].Width, 1), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                        g.Draw(sprPixel, new Rectangle(MinX + ActiveLayer.ListActiveMarker[M].Width, MinY, 1, ActiveLayer.ListActiveMarker[M].Height), null, DrawColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }

                    if (ListSelectedObjects.Contains(ActiveLayer.ListActiveMarker[M]) && MultipleSelectionRectangle.Width > 0)
                    {
                        g.Draw(sprPixel, new Rectangle((int)(ActiveLayer.ListActiveMarker[M].Position.X - 2) + 1, (int)(ActiveLayer.ListActiveMarker[M].Position.Y - 2) + 1, 3, 3), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    }
                }

                for (int P = 0; P < ActiveLayer.ListPolygonCutter.Count; P++)
                {
                    if (ListSelectedObjects.Contains(ActiveLayer.ListPolygonCutter[P]))
                    {
                        ActiveLayer.ListPolygonCutter[P].DrawExtra(g, sprPixel);
                    }
                }
            }

            #endregion

            g.End();
        }
    }
}
