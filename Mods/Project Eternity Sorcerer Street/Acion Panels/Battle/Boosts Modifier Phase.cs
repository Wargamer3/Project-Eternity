﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelBattleBoostsModifierPhase : BattleMapActionPanel
    {
        public static string RequirementName = "Sorcerer Street Boosts Phase";

        private readonly SorcererStreetMap Map;

        public ActionPanelBattleBoostsModifierPhase(ActionPanelHolder ListActionMenuChoice, SorcererStreetMap Map)
            : base("Battle Boosts Modifier Phase", ListActionMenuChoice, false)
        {
            this.Map = Map;
        }

        public override void OnSelect()
        {
            for (int X = 0; X < Map.MapSize.X; ++X)
            {
                for (int Y = 0; Y < Map.MapSize.Y; ++Y)
                {
                    TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(X, Y, Map.ActiveLayerIndex);

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
            AddToPanelListAndSelect(new ActionPanelBattleAttackPhase(ListActionMenuChoice, Map));
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
