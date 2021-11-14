﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelDrawCardPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "DrawCard";

        private enum AnimationPhases { IntroAnimation, CardSummary, Outro }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private Card DrawnCard;
        private AnimationPhases AnimationPhase;
        private float RotationTimer;
        private const float AnimationTime = 2 * MathHelper.TwoPi - MathHelper.PiOver2;
        bool IsInit = false;

        public ActionPanelDrawCardPhase(SorcererStreetMap Map)
            : base(PanelName, Map, false)
        {
        }

        public ActionPanelDrawCardPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            AnimationPhase = AnimationPhases.IntroAnimation;
            DrawnCard = ActivePlayer.ListRemainingCardInDeck[0];

            ActivePlayer.ListCardInHand.Add(DrawnCard);

            ActivePlayer.ListRemainingCardInDeck.RemoveAt(0);

            if (ActivePlayer.ListRemainingCardInDeck.Count == 0)
            {
                AddToPanelListAndSelect(new ActionPanelRefillDeckPhase(Map, ActivePlayerIndex));
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AnimationPhase == AnimationPhases.IntroAnimation)
            {
                RotationTimer += 0.1f;

                if (RotationTimer > AnimationTime || InputHelper.InputConfirmPressed())
                {
                    AnimationPhase = AnimationPhases.CardSummary;
                }
            }
            else if (AnimationPhase == AnimationPhases.CardSummary && InputHelper.InputConfirmPressed())
            {
                RotationTimer = 0f;
                AnimationPhase = AnimationPhases.Outro;
            }
            else if (AnimationPhase == AnimationPhases.Outro)
            {
                RotationTimer += 0.1f;

                if (RotationTimer > AnimationTime || InputHelper.InputConfirmPressed())
                {
                    FinishPhase();
                }
            }
        }

        public void FinishPhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelSpellCardSelectionPhase(Map, ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            AnimationPhase = AnimationPhases.IntroAnimation;

            ActivePlayerIndex = BR.ReadInt32();

            int ListCardInHandCount = BR.ReadInt32();
            ActivePlayer.ListCardInHand.Clear();
            for (int C = 0; C < ListCardInHandCount; ++C)
            {
                ActivePlayer.ListCardInHand.Add(Card.LoadCard(BR.ReadString(), Map.Content));
            }
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActivePlayer.ListCardInHand.Count);
            for (int C = 0; C < ActivePlayer.ListCardInHand.Count; ++C)
            {
                BW.AppendString(ActivePlayer.ListCardInHand[C].Path);
            }
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelDrawCardPhase(Map);
        }

        private void DrawIntro(CustomSpriteBatch g)
        {
            //Spin card from the left
            float MaxScale = 0.6f;
            float RealRotationTimer = RotationTimer % MathHelper.TwoPi;
            float FinalX = Constants.Width / 4;
            float StartX = -10;
            float DistanceX = FinalX - StartX;
            float X = StartX + (RotationTimer / AnimationTime) * DistanceX;
            float Y = Constants.Height / 10;
            float FinalScale = (float)Math.Sin(RealRotationTimer) * MaxScale;

            Card.DrawCardMiniature(g, DrawnCard.sprCard, GameScreen.sprPixel, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer);
        }

        private void DrawOutro(CustomSpriteBatch g)
        {
            float StartScale = 0.6f;
            float EndScale = 0.26f;
            float RealRotationTimer = RotationTimer % MathHelper.TwoPi;
            float StartX = Constants.Width / 4;
            float FinalX = ActivePlayer.ListCardInHand.Count * 80;
            float StartY = Constants.Height / 10;
            float FinalY = Constants.Height - 150;
            float DistanceX = FinalX - StartX;
            float DistanceY = FinalY - StartY;
            float X = StartX + (RotationTimer / AnimationTime) * DistanceX;
            float Y = StartY + (RotationTimer / AnimationTime) * DistanceY;
            float ComputedScale = StartScale + (EndScale - StartScale) * (RotationTimer / AnimationTime);
            float FinalScale = (float)Math.Sin(RealRotationTimer) * ComputedScale;

            Card.DrawCardMiniature(g, DrawnCard.sprCard, GameScreen.sprPixel, Color.White, X, Y, FinalScale, ComputedScale, RealRotationTimer);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (AnimationPhase == 0)
            {
                DrawIntro(g);
            }
            else if (AnimationPhase == AnimationPhases.Outro)
            {
                DrawOutro(g);
            }
            else
            {
                DrawnCard.DrawCard(g);
                DrawnCard.DrawCardInfo(g, Map.fntArial12);

                GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 70), 200, 30, Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Drew 1 card", new Vector2(Constants.Width / 2, Constants.Height - 65), Color.White);
                g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width / 2 + 76, Constants.Height - 65, 18, 18), Color.White);
            }
        }
    }
}
