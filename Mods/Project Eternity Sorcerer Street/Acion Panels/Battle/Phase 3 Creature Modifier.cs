using System;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleCreatureModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleCreatureModifierPhase";

        public static string RequirementName = "Sorcerer Street Creature Phase";

        public ActionPanelBattleCreatureModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
        }

        public override void OnSelect()
        {
            Init();

            ContinueBattlePhase();
        }

        private void Init()
        {
            Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, RequirementName);
            Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, RequirementName);
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleEnchantModifierPhase(Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
            Init();
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleCreatureModifierPhase(Map);
        }
    }
}
