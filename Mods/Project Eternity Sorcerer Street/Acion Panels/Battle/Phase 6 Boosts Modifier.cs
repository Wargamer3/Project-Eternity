﻿using System;
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

        public static List<TerrainSorcererStreet> GetListBoostCreatures(SorcererStreetMap Map)
        {
            List<TerrainSorcererStreet> ListBoostCreature = new List<TerrainSorcererStreet>();

            for (int X = 0; X < Map.MapSize.X; ++X)
            {
                for (int Y = 0; Y < Map.MapSize.Y; ++Y)
                {
                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
                    {
                        TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(X, Y, L);

                        if (ActiveTerrain.TerrainTypeIndex == 0)
                        {
                            continue;
                        }

                        if (ActiveTerrain.DefendingCreature != null)
                        {
                            bool BoostSkillFound = false;
                            foreach (BaseAutomaticSkill ActiveSkill in ActiveTerrain.DefendingCreature.ListActiveSkill)
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
                                            ListBoostCreature.Add(ActiveTerrain);
                                            BoostSkillFound = true;
                                            break;
                                        }
                                    }
                                }
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
