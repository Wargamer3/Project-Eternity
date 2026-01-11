using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelSpellCardSelectionPhase : ActionPanelCardSelectionPhase
    {
        private const string PanelName = "SpellCard";

        private double ArrowAnimationTime;

        public ActionPanelSpellCardSelectionPhase(SorcererStreetMap Map)
            : base(PanelName, Map, SpellCard.SpellCardType)
        {
            DrawDrawInfo = true;
        }

        public ActionPanelSpellCardSelectionPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map.ListActionMenuChoice, Map, ActivePlayerIndex, SpellCard.SpellCardType)
        {
            DrawDrawInfo = true;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            ArrowAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;

            base.DoUpdate(gameTime);

            if (!ActivePlayer.IsPlayerControlled)
            {
                return;
            }

            if (InputHelper.InputUpPressed())
            {
                //Move menu downward and then open the dice menu
                PrepareToRollDice();
            }
            else if (InputHelper.InputDownPressed())
            {
                SwitchToMainMenu();
            }
        }

        protected override void FinaliseCardSelection(List<Card> ListCardToUseInHand)
        {
            for (int C = ListCardToUseInHand.Count - 1; C >= 0; C--)
            {
                SpellCard ActiveCard = (SpellCard)ListCardToUseInHand[C];

                if (!ActiveCard.ListSpell[0].CanActivateEffectsOnTarget(ActivePlayer.Effects))
                {
                    ListCardToUseInHand.RemoveAt(C);
                }
            }
            base.FinaliseCardSelection(ListCardToUseInHand);
        }

        protected override void HandleCardSelectionAI(GameTime gameTime)
        {
            if (PlayerAICardToUseIndex == -1 || PlayerAICardToUseIndex >= ActivePlayer.ListCardInHand.Count)
            {
                AITimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (AITimer >= 1)
                {
                    PrepareToRollDice();
                }
            }
            else
            {
                base.HandleCardSelectionAI(gameTime);
            }
        }

        public override void UpdatePassive(GameTime gameTime)
        {
            ArrowAnimationTime += gameTime.ElapsedGameTime.TotalSeconds;
            UpdateAnimationTimer();
        }

        public void SwitchToMainMenu()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelMainMenuPhase(Map, ActivePlayerIndex));
        }

        public override void Draw(CustomSpriteBatch g)
        {
            base.Draw(g);

            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);
            ActionPanelPlayerDefault.DrawPhase(g, Map, "Spell Selection");
        }

        public override void DrawArrows(CustomSpriteBatch g)
        {
            MenuHelper.DrawUpArrow(g);
            MenuHelper.DrawDownArrow(g);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelSpellCardSelectionPhase(Map);
        }
    }
}
