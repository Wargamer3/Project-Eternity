using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBoostsModifierPhase : BattleMapActionPanel
    {
        private const string PanelName = "BattleBoostsModifierPhase";

        public static string RequirementName = "Sorcerer Street Boosts Phase";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleBoostsModifierPhase(SorcererStreetMap Map)
            : base(PanelName, Map.ListActionMenuChoice, null, false)
        {
            this.Map = Map;
        }

        public ActionPanelBattleBoostsModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base(PanelName, ListActionMenuChoice, null, false)
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
                            Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Invader;
                            Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Defender;

                            ActiveTerrain.DefendingCreature.ActivateSkill(RequirementName);

                            Map.GlobalSorcererStreetBattleContext.UserCreature = Map.GlobalSorcererStreetBattleContext.Defender;
                            Map.GlobalSorcererStreetBattleContext.OpponentCreature = Map.GlobalSorcererStreetBattleContext.Invader;

                            ActiveTerrain.DefendingCreature.ActivateSkill(RequirementName);

                        }
                    }
                }
            }
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
            AddToPanelListAndSelect(new ActionPanelBattleAttackPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            Init();
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelBattleBoostsModifierPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
