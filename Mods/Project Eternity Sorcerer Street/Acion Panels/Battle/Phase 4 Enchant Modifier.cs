using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleEnchantModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleEnchantModifierPhase";

        public static string RequirementName = "Sorcerer Street Enchant Phase";

        public static string ActivePhase;
        public static List<SkillActivationContext> ListSkillActivation;

        public ActionPanelBattleEnchantModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            if (!ActionPanelBattleItemModifierPhase.InitAnimations(Map.GlobalSorcererStreetBattleContext, RequirementName))
            {
                ContinueBattlePhase();
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (!HasFinishedUpdatingBars(gameTime, Map.GlobalSorcererStreetBattleContext))
                return;

            if (!ActionPanelBattleItemModifierPhase.UpdateAnimations(gameTime, Map.GlobalSorcererStreetBattleContext))
            {
                ContinueBattlePhase();
            }
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleItemModifierPhase(Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleEnchantModifierPhase(Map);
        }
    }
}
