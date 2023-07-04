using System;
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
            Init();

            ContinueBattlePhase();
        }

        private void Init()
        {
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
                            Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Invader, Map.GlobalSorcererStreetBattleContext.Defender, RequirementName);
                            Map.GlobalSorcererStreetBattleContext.ActiveSkill(Map.GlobalSorcererStreetBattleContext.Defender, Map.GlobalSorcererStreetBattleContext.Invader, RequirementName);

                        }
                    }
                }
            }
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
            Init();
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
