using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleEnchantModifierPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleEnchantModifierPhase";

        public static string RequirementName = "Sorcerer Street Enchant Phase";
        
        private readonly SorcererStreetMap Map;

        public ActionPanelBattleEnchantModifierPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleEnchantModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base(PanelName, ListActionMenuChoice, false)
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

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleEnchantModifierPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
