using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelCapture : ActionPanelConquest
    {
        private const string PanelName = "Capture";

        private int ActivePlayerIndex;
        private int ActiveUnitIndex;
        private UnitConquest ActiveUnit;

        public ActionPanelCapture(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelCapture(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
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

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActiveUnitIndex = BR.ReadInt32();
            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
            BW.AppendInt32(ActiveUnitIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelCapture(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
        }
    }
}
