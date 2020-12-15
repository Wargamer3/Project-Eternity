using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleItemModifierPhase : BattleMapActionPanel
    {
        public static string RequirementName = "Sorcerer Street Item Phase";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleItemModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base("Battle Item Modifier Phase", ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            if (Map.GlobalSorcererStreetBattleContext.InvaderItem != null)
            {
                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

                Map.GlobalSorcererStreetBattleContext.InvaderItem.ActivateSkill(RequirementName);
            }

            if (Map.GlobalSorcererStreetBattleContext.DefenderItem != null)
            {
                Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
                Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

                Map.GlobalSorcererStreetBattleContext.DefenderItem.ActivateSkill(RequirementName);
            }

            ContinueBattlePhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public void FinishPhase()
        {
            ContinueBattlePhase();
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleBoostsModifierPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
