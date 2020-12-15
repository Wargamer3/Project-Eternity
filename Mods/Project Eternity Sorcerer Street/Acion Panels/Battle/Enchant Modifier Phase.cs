using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleEnchantModifierPhase : BattleMapActionPanel
    {
        public static string RequirementName = "Sorcerer Street Enchant Phase";
        
        private readonly SorcererStreetMap Map;

        public ActionPanelBattleEnchantModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base("Battle Enchant Modifier Phase", ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
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
            AddToPanelListAndSelect(new ActionPanelBattleItemModifierPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
