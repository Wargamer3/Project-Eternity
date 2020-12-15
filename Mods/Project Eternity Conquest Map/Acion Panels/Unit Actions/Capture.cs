using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelCapture : ActionPanelConquest
    {
        private UnitConquest ActiveUnit;

        public ActionPanelCapture(ConquestMap Map, UnitConquest ActiveUnit)
            : base("Capture", Map)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void OnSelect()
        {
            Map.FinalizeMovement(ActiveUnit);
            ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            TerrainConquest ActiveTerrain = Map.GetTerrain(ActiveUnit.Components);

            if (ActiveTerrain.CapturedPlayerIndex != Map.ActivePlayerIndex)
            {
                ActiveTerrain.CapturePoints = Math.Max(0, ActiveTerrain.CapturePoints - ActiveUnit.HP);
                if (ActiveTerrain.CapturePoints == 0)
                    ActiveTerrain.CapturedPlayerIndex = Map.ActivePlayerIndex;
            }

            RemoveAllSubActionPanels();
        }

        public override void DoUpdate(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
