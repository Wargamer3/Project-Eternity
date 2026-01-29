using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBoostsModifierPhase : ActionPanelBattle
    {
        private const string PanelName = "BattleBoostsModifierPhase";

        public static string RequirementName = "Sorcerer Street Boosts Phase";

        private enum AnimationPhases { Finished, InvaderIntro, InvaderActivation, DefenderIntro, DefenderActivation }

        public static List<SkillActivationContext> ListSkillActivation;

        public ActionPanelBattleBoostsModifierPhase(SorcererStreetMap Map)
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

        public static List<TerrainSorcererStreet> GetListBoostCreatures(SorcererStreetMap Map)
        {
            List<TerrainSorcererStreet> ListBoostCreature = new List<TerrainSorcererStreet>();

            foreach (TerrainSorcererStreet DefendingTerrain in Map.ListSummonedCreature)
            {
                bool BoostSkillFound = false;
                foreach (BaseAutomaticSkill ActiveSkill in DefendingTerrain.DefendingCreature.ListActiveSkill)
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
                                ListBoostCreature.Add(DefendingTerrain);
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
