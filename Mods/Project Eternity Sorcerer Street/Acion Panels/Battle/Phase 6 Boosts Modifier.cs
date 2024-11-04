using System;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBoostsModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleBoostsModifierPhase";

        public static string RequirementName = "Sorcerer Street Boosts Phase";

        public ActionPanelBattleBoostsModifierPhase(SorcererStreetMap Map)
            : base(Map, PanelName)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            Init(Map.GlobalSorcererStreetBattleContext);

            ContinueBattlePhase();
        }

        public static void Init(SorcererStreetBattleContext GlobalSorcererStreetBattleContext)
        {
            foreach (TerrainSorcererStreet ActiveBoostCreature in GlobalSorcererStreetBattleContext.ListBoostCreature)
            {
            }
        }

        public static List<CreatureCard> GetListBoostCreatures(SorcererStreetMap Map)
        {
            List<CreatureCard> ListBoostCreature = new List<CreatureCard>();

            foreach (CreatureCard DefendingCreature in Map.ListSummonedCreature)
            {
                bool BoostSkillFound = false;
                foreach (BaseAutomaticSkill ActiveSkill in DefendingCreature.ListActiveSkill)
                {
                    if (BoostSkillFound)
                    {
                        break;
                    }

                    foreach (BaseSkillActivation ActiveActivation in ActiveSkill.CurrentSkillLevel.ListActivation)
                    {
                        if (BoostSkillFound)
                        {
                            break;
                        }

                        foreach (BaseSkillRequirement ActiveRequirement in ActiveActivation.ListRequirement)
                        {
                            if (ActiveRequirement.SkillRequirementName == RequirementName)
                            {
                                ListBoostCreature.Add(DefendingCreature);
                                BoostSkillFound = true;
                                break;
                            }
                        }
                    }
                }
            }

            return ListBoostCreature;
        }

        public void FinishPhase()
        {
            ContinueBattlePhase();
        }

        private void ContinueBattlePhase()
        {
            RemoveFromPanelList(this);
            AddToPanelListAndSelect(new ActionPanelBattleAttackPhase(Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ReadPlayerInfo(BR, Map);
            Init(Map.GlobalSorcererStreetBattleContext);
        }

        public override void DoWrite(ByteWriter BW)
        {
            WritePlayerInfo(BW, Map);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleBoostsModifierPhase(Map);
        }
    }
}
