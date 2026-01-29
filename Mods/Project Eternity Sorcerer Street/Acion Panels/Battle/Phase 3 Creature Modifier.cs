using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleCreatureModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleCreatureModifierPhase";

        public static string RequirementName = "Sorcerer Street Creature Phase";


        public static List<SkillActivationContext> ListSkillActivation;

        public ActionPanelBattleCreatureModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
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
            AddToPanelListAndSelect(new ActionPanelBattleEnchantModifierPhase(Map));
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
            return new ActionPanelBattleCreatureModifierPhase(Map);
        }
    }
}
