using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleCreatureModifierPhase : BattleMapActionPanel
    {
        public static string RequirementName = "Sorcerer Street Creature Phase";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleCreatureModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base("Battle Creature Modifier Phase", ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
            Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

            Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(RequirementName);

            Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
            Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

            Map.GlobalSorcererStreetBattleContext.UserCreature.ActivateSkill(RequirementName);

            ContinueBattlePhase();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }
        
        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleEnchantModifierPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
