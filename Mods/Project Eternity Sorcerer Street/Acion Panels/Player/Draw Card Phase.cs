﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

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

            if (ActivePlayer.ListRemainingCardInDeck.Count == 0)
            {
                AddToPanelListAndSelect(new ActionPanelRefillDeckPhase(Map, ActivePlayerIndex));
            }

            int RandomCardIndex = RandomHelper.Next(ActivePlayer.ListRemainingCardInDeck.Count);

            DrawnCard = ActivePlayer.ListRemainingCardInDeck[RandomCardIndex];

            ActivePlayer.ListCardInHand.Add(DrawnCard);

            ActivePlayer.ListRemainingCardInDeck.RemoveAt(RandomCardIndex);
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AnimationPhase == AnimationPhases.IntroAnimation)
            {
                RotationTimer += 0.1f;

                if (RotationTimer > AnimationTime || InputHelper.InputConfirmPressed())
                {
                    AnimationPhase = AnimationPhases.CardSummary;

                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));

                    if (ActivePlayer.ListCardInHand.Count > 6)
                    {
                        AddToPanelListAndSelect(new ActionPanelDiscardCardPhase(Map, ActivePlayerIndex, 6));
                    }
                }
            }
            else if (AnimationPhase == AnimationPhases.CardSummary && InputHelper.InputConfirmPressed())
            {
                RotationTimer = 0f;
                AnimationPhase = AnimationPhases.Outro;
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
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

        public override void UpdatePassive(GameTime gameTime)
        {
            if (AnimationPhase == AnimationPhases.IntroAnimation)
            {
                RotationTimer += 0.1f;

                if (RotationTimer > AnimationTime)
                {
                    AnimationPhase = AnimationPhases.CardSummary;
                }
            }
            else if (AnimationPhase == AnimationPhases.Outro && RotationTimer < AnimationTime)
            {
                RotationTimer += 0.1f;
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
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];

            string DrawnCardType = BR.ReadString();
            string DrawnCardPath = BR.ReadString();


            ActivePlayer.ListCardInHand.Clear();

            byte CardInHand = BR.ReadByte();
            for (int C = 0; C < CardInHand; ++C)
            {
                string CardType = BR.ReadString();
                string CardPath = BR.ReadString();

                foreach (Card ActiveCard in ActivePlayer.ListCardInDeck)
                {
                    if (ActiveCard.CardType == DrawnCardType && ActiveCard.Path == DrawnCardPath)
                    {
                        DrawnCard = ActiveCard;
                    }
                    if (ActiveCard.CardType == CardType && ActiveCard.Path == CardPath)
                    {
                        ActivePlayer.ListCardInHand.Add(ActiveCard);
                        break;
                    }
                }
            }
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            AnimationPhase = (AnimationPhases)ArrayUpdateData[0];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);

            BW.AppendString(DrawnCard.CardType);
            BW.AppendString(DrawnCard.Path);

            BW.AppendByte((byte)ActivePlayer.ListCardInHand.Count);
            foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
            {
                BW.AppendString(ActiveCard.CardType);
                BW.AppendString(ActiveCard.Path);
            }
        }

        public override byte[] DoWriteUpdate()
        {
            return new byte[] { (byte)AnimationPhase };
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

            Card.DrawCardMiniature(g, DrawnCard.sprCard, Map.sprCardBack, Color.White, X, Y, FinalScale, MaxScale, RealRotationTimer);
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

            Card.DrawCardMiniature(g, DrawnCard.sprCard, Map.sprCardBack, Color.White, X, Y, FinalScale, ComputedScale, RealRotationTimer);
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
                DrawnCard.DrawCardInfo(g, Map.Symbols, Map.fntArial12, 0, 0);

                GameScreen.DrawBox(g, new Vector2(Constants.Width / 2 - 100, Constants.Height - 70), 200, 40, Color.White);
                g.DrawStringMiddleAligned(Map.fntArial12, "Drew 1 card", new Vector2(Constants.Width / 2, Constants.Height - 59), Color.White);
                g.Draw(Map.sprMenuHand, new Vector2(Constants.Width / 2 + 76, Constants.Height - 65), null, Color.White, 0f, Vector2.Zero, 0.7f, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0f);
            }
        }
    }
}
