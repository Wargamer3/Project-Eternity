using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.BattleMapScreen.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public abstract class ActionPanelCardSelectionPhase : BattleMapActionPanel
    {
        private enum AnimationPhases { IntroAnimation, CardSelection }

        protected readonly SorcererStreetMap Map;
        protected int ActivePlayerIndex;
        protected Player ActivePlayer;
        private readonly string CardType;
        private AnimationPhases AnimationPhase;
        private int CardCursorIndex;
        private float AnimationTimer;
        private float MaxAnimationScale;
        private string EndCardText;//Card on the far right used to close the pannel.
        public bool DrawDrawInfo;

        public ActionPanelCardSelectionPhase(string Name, SorcererStreetMap Map, string CardType, string EndCardText = "")
            : base(Name, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
            this.CardType = CardType;
            this.EndCardText = EndCardText;

            MaxAnimationScale = 1.1f;
        }

        public ActionPanelCardSelectionPhase(string Name, ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map, int ActivePlayerIndex, string CardType, string EndCardText = "")
            : base(Name, ListActionMenuChoice, null, false)
        {
            this.Map = Map;
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.CardType = CardType;
            this.EndCardText = EndCardText;

            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            MaxAnimationScale = 1.1f;
        }

        public override void OnSelect()
        {
            AnimationPhase = AnimationPhases.IntroAnimation;
            CardCursorIndex = 0;
            AnimationTimer = 0;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (AnimationPhase == AnimationPhases.IntroAnimation)
            {
                AnimationPhase = AnimationPhases.CardSelection;
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
            else if (AnimationPhase == AnimationPhases.CardSelection)
            {
                HandleCardSelection();
            }
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

            if (InputHelper.InputLeftPressed() && --CardCursorIndex < 0)
            {
                if (EndCardText != string.Empty)
                {
                    CardCursorIndex = ActivePlayer.ListCardInHand.Count;
                }
                else
                {
                    CardCursorIndex = ActivePlayer.ListCardInHand.Count - 1;
                }
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
            else if (InputHelper.InputRightPressed() && ++CardCursorIndex >= ActivePlayer.ListCardInHand.Count)
            {
                if (EndCardText != string.Empty && CardCursorIndex == ActivePlayer.ListCardInHand.Count)
                {
                    CardCursorIndex = ActivePlayer.ListCardInHand.Count;
                }
                else
                {
                    CardCursorIndex = 0;
                }
                Map.OnlineClient.Host.Send(new UpdateMenuScriptClient(this));
            }
            else if (InputHelper.InputConfirmPressed())
            {
                if (CardType == null
                    || (CardCursorIndex < ActivePlayer.ListCardInHand.Count && ActivePlayer.ListCardInHand[CardCursorIndex].CardType == CardType && ActivePlayer.Magic >= ActivePlayer.ListCardInHand[CardCursorIndex].MagicCost))
                {
                    OnCardSelected(ActivePlayer.ListCardInHand[CardCursorIndex]);
                }
                else if (CardCursorIndex == ActivePlayer.ListCardInHand.Count)
                {
                    OnEndCardSelected();
                }
            }
        }

        public void PrepareToRollDice()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelRollDicePhase(Map, ActivePlayerIndex));
        }

        public void SwitchToTerritory()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex));
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
            else
            {
                RemoveFromPanelList(this);
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
            CardCursorIndex = ArrayUpdateData[1];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        public override byte[] DoWriteUpdate()
        {
            return new byte[] { (byte)AnimationPhase, (byte)CardCursorIndex };
        }

        public void DrawIntro(CustomSpriteBatch g)
        {
            //Have every cards flips to reveal themselves
            for (int C = 0; C < ActivePlayer.ListCardInHand.Count; C++)
            {
                Color CardColor = Color.White;
                if (CardType != null && (ActivePlayer.ListCardInHand[C].CardType != CardType || ActivePlayer.Magic < ActivePlayer.ListCardInHand[C].MagicCost))
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
                if (CardType != null && (ActivePlayer.ListCardInHand[C].CardType != CardType || ActivePlayer.Magic < ActivePlayer.ListCardInHand[C].MagicCost))
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

                float Scale = Constants.Width / 3764.70581f;
                for (int C = 0; C < ActivePlayer.ListCardInHand.Count; C++)
                {
                    Color CardColor = Color.White;
                    if (CardType != null && (ActivePlayer.ListCardInHand[C].CardType != CardType || ActivePlayer.Magic < ActivePlayer.ListCardInHand[C].MagicCost))
                    {
                        CardColor = Color.FromNonPremultiplied(100, 100, 100, 255);
                    }

                    DrawCardMiniature(g, ActivePlayer.ListCardInHand[C].sprCard, CardColor, C == CardCursorIndex, C * 80 + 80, Scale, AnimationTimer, 0.02f);
                }

                if (DrawDrawInfo && CardCursorIndex < ActivePlayer.ListCardInHand.Count)
                {
                    ActivePlayer.ListCardInHand[CardCursorIndex].DrawCardInfo(g, Map.Symbols, Map.fntArial12, 0, 0);
                }

                if (EndCardText != string.Empty)
                {
                    DrawCardMiniature(g, Map.sprEndTurn, Color.White, CardCursorIndex == ActivePlayer.ListCardInHand.Count, Constants.Width - 80, 0.30f, AnimationTimer, 0.05f);
                }
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

            Card.DrawCardMiniature(g, sprCard, GameScreen.sprPixel, CardFrontColor, X, Y, FinalScale, MaxScale, RealRotationTimer);
        }

        private static void DrawCardMiniature(CustomSpriteBatch g, Texture2D sprCard, Color CardFrontColor, bool Selected, float X, float MaxScale, float AnimationTimer, float ExtraAnimationScale)
        {
            float Y = Constants.Height - 100;

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
            Card.DrawCardMiniatureCentered(g, sprCard, GameScreen.sprPixel, CardFrontColor, X, Y, -FinalScale, FinalScale, MathHelper.Pi + MathHelper.PiOver2);
        }
    }
}
