using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

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
