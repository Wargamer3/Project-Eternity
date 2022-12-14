using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class AnimationScreen : AnimationClass
    {
        public enum QuoteTypes { BattleStart, Dodge, Damaged, Destroyed, SupportAttack, SupportDefend };

        private UnitConquest ActiveUnit;
        private string ActiveTerrain;
        private bool HorizontalMirror;
        public bool HasEnded;

        public AnimationScreen(string AnimationPath, UnitConquest ActiveUnit, 
            string ActiveTerrain, bool HorizontalMirror)
            : base(AnimationPath)
        {
            RequireFocus = true;
            RequireDrawFocus = true;
            IsOnTop = false;
            HasEnded = false;

            this.ActiveUnit = ActiveUnit;
            this.ActiveTerrain = ActiveTerrain;
            this.HorizontalMirror = HorizontalMirror;
        }

        public override void Load()
        {

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
                            case "Player Idle":
                            case "Player Stand":
                            case "Player Standing":
                            case "Player Default":
                                ActiveMarkerEvent.AnimationMarker = new AnimationClass("Conquest/" + ActiveUnit.ArmourType + "/Idle");
                                break;

                            case "Player Attack":
                            case "Player Attacking":
                                ActiveMarkerEvent.AnimationMarker = new AnimationClass("Conquest/" + ActiveUnit.ArmourType + "/Attack");
                                break;
                        }
                        ActiveMarkerEvent.AnimationMarker.DicTimeline = DicTimeline;
                        ActiveMarkerEvent.AnimationMarker.Load();
                    }
                }

                #endregion
                
                #region Init renderTarget

                ListAnimationLayer[L].renderTarget = new RenderTarget2D(
                    GraphicsDevice,
                    ScreenWidth,
                    ScreenHeight);

                #endregion
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ActiveKeyFrame >= LoopEnd)
            {
                HasEnded = true;
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
            base.BeginDraw(g);

            if (ActiveCharacterSprite != null && ActiveCharacterSprite.IsAnimated)
            {
                ActiveCharacterSprite.ActiveAnimation.BeginDraw(g);
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

                for (int M = 0; M < ListAnimationLayer[L].ListActiveMarker.Count; M++)
                {
                    AnimationClass ActiveMarkerAnimation = ListAnimationLayer[L].ListActiveMarker[M].AnimationMarker;

                    for (int i = 0; i < ActiveMarkerAnimation.ListAnimationLayer.Count; i++)
                    {
                        if (ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget == null ||
                            ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget.Width != GraphicsDevice.PresentationParameters.BackBufferWidth ||
                            ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget.Height != GraphicsDevice.PresentationParameters.BackBufferHeight)
                        {
                            ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget = new RenderTarget2D(
                                GraphicsDevice,
                                GraphicsDevice.PresentationParameters.BackBufferWidth,
                                GraphicsDevice.PresentationParameters.BackBufferHeight);
                        }
                    }
                }

                for (int M = 0; M < ListAnimationLayer[L].ListActiveMarker.Count; M++)
                {
                    AnimationClass ActiveMarkerAnimation = ListAnimationLayer[L].ListActiveMarker[M].AnimationMarker;

                    for (int i = 0; i < ActiveMarkerAnimation.ListAnimationLayer.BasicLayerCount; i++)
                    {
                        GraphicsDevice.SetRenderTarget(ActiveMarkerAnimation.ListAnimationLayer[i].renderTarget);
                        GraphicsDevice.Clear(Color.Transparent);
                        //Don't draw submarkers.
                        DrawLayer(g, ActiveMarkerAnimation.ListAnimationLayer[i], false, false, null, true);
                    }
                }

                GraphicsDevice.SetRenderTarget(ListAnimationLayer[L].renderTarget);
                GraphicsDevice.Clear(Color.Transparent);

                DrawLayer(g, ListAnimationLayer[L], false, true, null, true);
            }
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
