using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class AnimationScreen : AnimationLooped
    {
        public enum QuoteTypes { BattleStart, Dodge, Damaged, Destroyed, SupportAttack, SupportDefend };

        private bool HorizontalMirror;
        private SpriteFont fntFinlanderFont;
        private Texture2D sprEnemyCard;

        public AnimationScreen(string AnimationPath, Texture2D sprEnemyCard, bool HorizontalMirror)
            : base(AnimationPath)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            IsOnTop = false;

            this.sprEnemyCard = sprEnemyCard;
            this.HorizontalMirror = HorizontalMirror;
        }

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

            foreach (KeyValuePair<string, Timeline> Timeline in LoadTimelines(typeof(CoreTimeline)))
            {
                if (Timeline.Value is AnimationOriginTimeline)
                    continue;

                DicTimeline.Add(Timeline.Key, Timeline.Value);
            }

            base.Load();

            InitRessources();
            
            for (int L = ListAnimationLayer.Count - 1; L >= 0; --L)
            {
                #region Markers

                foreach (List<Timeline> ListActiveEvent in ListAnimationLayer[L].DicTimelineEvent.Values)
                {
                    foreach (Timeline ActiveTimeline in ListActiveEvent)
                    {
                        MarkerTimeline ActiveMarkerEvent = ActiveTimeline as MarkerTimeline;
                        if (ActiveMarkerEvent == null)
                            continue;
                        
                        switch (ActiveMarkerEvent.MarkerType)
                        {
                            case "Card":
                                ActiveMarkerEvent.Sprite = sprEnemyCard;
                                break;
                        }
                        ActiveMarkerEvent.Sprite = sprEnemyCard;
                    }
                }

                #endregion
                
                #region Init renderTarget

                ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                    GraphicsDevice,
                    GraphicsDevice.PresentationParameters.BackBufferWidth,
                    GraphicsDevice.PresentationParameters.BackBufferHeight);

                #endregion
            }
        }
        
        public override void BeginDraw(CustomSpriteBatch g)
        {
            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                var ActiveLayer = ListAnimationLayer[L];

                for (int A = 0; A < ActiveLayer.ListVisibleObject.Count; A++)
                {
                    ActiveLayer.ListVisibleObject[A].BeginDraw(g);
                }
            }

            g.End();

            if (ActiveCharacter != null && ActiveCharacter.IsAnimated)
            {
                ActiveCharacter.ActiveAnimation.BeginDraw(g);
            }

            for (int L = 0; L < ListAnimationLayer.Count; L++)
            {
                if (ListAnimationLayer[L].renderTarget.Width != GraphicsDevice.PresentationParameters.BackBufferWidth ||
                    ListAnimationLayer[L].renderTarget.Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                {
                    ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                        GraphicsDevice,
                        GraphicsDevice.PresentationParameters.BackBufferWidth,
                        GraphicsDevice.PresentationParameters.BackBufferHeight);
                }

                GraphicsDevice.SetRenderTarget(ListAnimationLayer[L].renderTarget);
                GraphicsDevice.Clear(Color.Transparent);

                if (ListAnimationLayer[L].LayerBlendState == AnimationLayer.LayerBlendStates.Add)
                    g.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, ListAnimationLayer[L].SamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, TransformationMatrix);
                else if (ListAnimationLayer[L].LayerBlendState == AnimationLayer.LayerBlendStates.Substract)
                    g.Begin(SpriteSortMode.BackToFront, NegativeBlendState, ListAnimationLayer[L].SamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, TransformationMatrix);

                for (int M = 0; M < ListAnimationLayer[L].ListActiveMarker.Count; M++)
                {
                    if (ListAnimationLayer[L].ListActiveMarker[M].Sprite != null)
                    {
                        g.Draw(ListAnimationLayer[L].ListActiveMarker[M].Sprite, new Vector2(ListAnimationLayer[L].ListActiveMarker[M].Position.X, ListAnimationLayer[L].ListActiveMarker[M].Position.Y),
                            null, Color.White, 0, Vector2.Zero, ListAnimationLayer[L].ListActiveMarker[M].ScaleFactor, SpriteEffects.None, 1);
                    }
                }

                g.End();

                DrawLayer(g, ListAnimationLayer[L], false, null);
            }

            g.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            for (int A = ListAnimationLayer.Count - 1; A >= 0; --A)
            {
                if (ListAnimationLayer[A].ListPolygonCutter.Count > 0)
                {
                    PolygonEffect.Texture = ListAnimationLayer[A].renderTarget;
                    PolygonEffect.CurrentTechnique.Passes[0].Apply();

                    GraphicsDevice.RasterizerState = RasterizerState.CullNone;

                    for (int P = 0; P < ListAnimationLayer[A].ListPolygonCutter.Count; P++)
                        ListAnimationLayer[A].ListPolygonCutter[P].Draw(g, false);
                }
                else
                {
                    if (HorizontalMirror)
                    {
                        g.Draw(ListAnimationLayer[A].renderTarget, new Vector2(Constants.Width / 2, Constants.Height / 2), null, Color.White, 0, new Vector2(Constants.Width / 2, Constants.Height / 2), 1, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        g.Draw(ListAnimationLayer[A].renderTarget, Vector2.Zero, Color.White);
                    }
                }
            }
        }
    }
}
