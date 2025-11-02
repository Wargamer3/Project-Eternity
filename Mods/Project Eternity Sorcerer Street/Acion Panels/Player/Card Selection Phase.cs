using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelCardSelectionPhase : BattleMapActionPanel
    {
        protected enum AnimationPhases { IntroAnimation, CardSelection }

        protected readonly SorcererStreetMap Map;
        protected int ActivePlayerIndex;
        protected Player ActivePlayer;
        private readonly string CardType;
        protected AnimationPhases AnimationPhase;
        private float AnimationTimer;
        private string EndCardText;//Card on the far right used to close the pannel.
        public bool DrawDrawInfo;

        public ActionPanelCardSelectionPhase(string Name, SorcererStreetMap Map, string CardType, string EndCardText = "")
            : base(Name, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
            this.CardType = CardType;
            this.EndCardText = EndCardText;
        }

        public ActionPanelCardSelectionPhase(string Name, ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, int ActivePlayerIndex, string CardType, string EndCardText = "")
            : base(Name, ListActionMenuChoice, null, false)
        {
            this.Map = Map;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.CardType = CardType;
            this.EndCardText = EndCardText;

            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
            AnimationPhase = AnimationPhases.IntroAnimation;
            AnimationTimer = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AnimationPhase == AnimationPhases.IntroAnimation)
            {
                AnimationPhase = AnimationPhases.CardSelection;

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (AnimationPhase == AnimationPhases.CardSelection)
            {
                HandleCardSelection();
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            UpdateAnimationTimer();
        }

        public void UpdateAnimationTimer()
        {
            ++AnimationTimer;
            if (AnimationTimer > 300)
                AnimationTimer -= 300;
        }

        private void HandleCardSelection()
        {
            UpdateAnimationTimer();

            if (InputHelper.InputLeftPressed() && --ActionMenuCursor < 0)
            {
                if (EndCardText != string.Empty)
                {
                    ActionMenuCursor = ActivePlayer.ListCardInHand.Count;
                }
                else
                {
                    ActionMenuCursor = ActivePlayer.ListCardInHand.Count - 1;
                }

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (InputHelper.InputRightPressed() && ++ActionMenuCursor >= ActivePlayer.ListCardInHand.Count)
            {
                if (EndCardText != string.Empty && ActionMenuCursor == ActivePlayer.ListCardInHand.Count)
                {
                    ActionMenuCursor = ActivePlayer.ListCardInHand.Count;
                }
                else
                {
                    ActionMenuCursor = 0;
                }

                if (Map.OnlineClient != null)
                {
                    Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
                }
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (CanUseCard())
                {
                    OnCardSelected(ActivePlayer.ListCardInHand[ActionMenuCursor]);
                }
                else if (ActionMenuCursor == ActivePlayer.ListCardInHand.Count)
                {
                    OnEndCardSelected();
                }
            }
        }

        private bool CanUseCard()
        {
            if (ActionMenuCursor >= ActivePlayer.ListCardInHand.Count)
            {
                return false;
            }

            if (CardType == null || ActivePlayer.ListCardInHand[ActionMenuCursor].CardType == CardType)
            {
                return ActivePlayer.CanUseCard(ActivePlayer.ListCardInHand[ActionMenuCursor]);
            }

            if (CardType == "Support Creature")
            {
                if (ActivePlayer.ListCardInHand[ActionMenuCursor].CardType != CreatureCard.CreatureCardType && ActivePlayer.ListCardInHand[ActionMenuCursor].CardType != ItemCard.ItemCardType)
                {
                    return false;
                }

                bool CanUseSupportCreature;
                if (ActivePlayerIndex == Map.ActivePlayerIndex)
                {
                    CanUseSupportCreature = Map.GlobalSorcererStreetBattleContext.SelfCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).SupportCreature;
                }
                else
                {
                    CanUseSupportCreature = Map.GlobalSorcererStreetBattleContext.OpponentCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).SupportCreature;
                }

                CreatureCard ActiveCard = (CreatureCard)ActivePlayer.ListCardInHand[ActionMenuCursor];
                if (!CanUseSupportCreature && !ActiveCard.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Battle).ItemCreature)
                {
                    return false;
                }

                return ActivePlayer.CanUseCard(ActivePlayer.ListCardInHand[ActionMenuCursor]);
            }

            return false;
        }

        public void PrepareToRollDice()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelRollDicePhase(Map, ActivePlayerIndex));
        }

        protected override void OnCancelPanel()
        {
        }

        public virtual void OnCardSelected(Card CardSelected)
        {
            ActionPanelSorcererStreet NextPanel = CardSelected.ActivateOnMap(Map, ActivePlayerIndex);
            if (NextPanel != null)
            {
                //Spin card to card view screen
                AddToPanelListAndSelect(NextPanel);
            }
        }

        public virtual void OnEndCardSelected()
        { }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void ExecuteUpdate(byte[] ArrayUpdateData)
        {
            AnimationPhase = (AnimationPhases)ArrayUpdateData[0];
            ActionMenuCursor = ArrayUpdateData[1];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        public override byte[] DoWriteUpdate()
        {
            return new byte[] { (byte)AnimationPhase, (byte)ActionMenuCursor };
        }

        public void DrawIntro(CustomSpriteBatch g)
        {
            //Have every cards flips to reveal themselves
            for (int C = 0; C < ActivePlayer.ListCardInHand.Count; C++)
            {
                Color CardColor = Color.White;
                if (CardType != null && (ActivePlayer.ListCardInHand[C].CardType != CardType || ActivePlayer.CanUseCard(ActivePlayer.ListCardInHand[C])))
                {
                    CardColor = Color.FromNonPremultiplied(100, 100, 100, 255);
                }

                DrawCardMiniature(g, GameScreen.sprPixel, CardColor, C * 80 + 10, 0.26f);
            }

            if (EndCardText != string.Empty)
            {
                DrawCardMiniature(g, GameScreen.sprPixel, Color.Green, Constants.Width - 140, 0.26f);
            }
        }

        public void DrawOutro(CustomSpriteBatch g)
        {
            //Have every cards flips to reveal themselves
            for (int C = 0; C < ActivePlayer.ListCardInHand.Count; C++)
            {
                Color CardColor = Color.White;
                if (CardType != null && (ActivePlayer.ListCardInHand[C].CardType != CardType || ActivePlayer.CanUseCard(ActivePlayer.ListCardInHand[C])))
                {
                    CardColor = Color.FromNonPremultiplied(100, 100, 100, 255);
                }

                DrawCardMiniature(g, GameScreen.sprPixel, CardColor, C * 80 + 10, 0.26f);
            }

            if (EndCardText != string.Empty)
            {
                DrawCardMiniature(g, GameScreen.sprPixel, Color.Green, Constants.Width - 140, 0.26f);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (AnimationPhase == AnimationPhases.IntroAnimation)
            {
                DrawIntro(g);
            }
            else if (AnimationPhase == AnimationPhases.CardSelection)
            {
                DrawArrows(g);
                //Display END at the right of the cards in the hand to end your turn
                //Display Start at the right of the cards in the hand for a battle

                float Scale = 0.545f;
                int DistanceBetweenCard = 234;
                for (int C = 0; C < ActivePlayer.ListCardInHand.Count; C++)
                {
                    Color CardColor = Color.White;
                    if (CardType != null && (ActivePlayer.ListCardInHand[C].CardType != CardType ||  !ActivePlayer.CanUseCard(ActivePlayer.ListCardInHand[C])))
                    {
                        CardColor = Color.FromNonPremultiplied(100, 100, 100, 255);
                    }

                    DrawCardMiniature(g, ActivePlayer.ListCardInHand[C].sprCard, CardColor, C == ActionMenuCursor,
                        C * DistanceBetweenCard + DistanceBetweenCard + 27, Scale, AnimationTimer, 0.02f);
                }

                if (DrawDrawInfo && ActionMenuCursor < ActivePlayer.ListCardInHand.Count)
                {
                    ActivePlayer.ListCardInHand[ActionMenuCursor].DrawCardInfo(g, Map.Symbols, Map.fntMenuText, ActivePlayer, 0, 0);
                }

                if (EndCardText != string.Empty)
                {
                    DrawCardMiniature(g, Map.sprEndTurn, Color.White, ActionMenuCursor == ActivePlayer.ListCardInHand.Count, 6 * DistanceBetweenCard + DistanceBetweenCard, Scale, AnimationTimer, 0.05f);
                }

                MenuHelper.DrawFingerIcon(g, new Vector2(DistanceBetweenCard / 2 + DistanceBetweenCard * ActionMenuCursor, Constants.Height - Constants.Height / 6));
            }
        }

        public virtual void DrawArrows(CustomSpriteBatch g)
        {
        }

        private void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCard, Color CardFrontColor, float X, float MaxScale)
        {
            float RealRotationTimer = AnimationTimer % MathHelper.TwoPi;
            float Y = Constants.Width - 140;
            
            float FinalScale = (float)Math.Sin(RealRotationTimer) * MaxScale;

            Card.DrawCardMiniature(g, sprCard, GameScreen.sprPixel, CardFrontColor, X, Y, FinalScale, MaxScale, RealRotationTimer < MathHelper.Pi);
        }

        private static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCard, Color CardFrontColor, bool Selected, float X, float MaxScale, float AnimationTimer, float ExtraAnimationScale)
        {
            float Y = 870;

            float Scale = ExtraAnimationScale;
            if (Selected)
            {
                if (AnimationTimer < 150)
                {
                    Scale = AnimationTimer / 150f * ExtraAnimationScale;
                }
                else
                {
                    Scale = (300 - AnimationTimer) / 150f * ExtraAnimationScale;
                }
            }

            float FinalScale = MaxScale + Scale;
            Card.DrawCardMiniatureCentered(g, sprCard, GameScreen.sprPixel, CardFrontColor, X, Y, -FinalScale, FinalScale, false);
        }
    }
}
