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
        private int ActiveBuildingIndex;
        private BuildingConquest ActiveBuilding;

        public ActionPanelCapture(ConquestMap Map)
            : base(PanelName, Map)
        {
        }

        public ActionPanelCapture(ConquestMap Map, int ActivePlayerIndex, int ActiveUnitIndex, int ActiveBuildingIndex)
            : base(PanelName, Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveUnitIndex = ActiveUnitIndex;
            this.ActiveBuildingIndex = ActiveBuildingIndex;

            ActiveUnit = Map.ListPlayer[ActivePlayerIndex].ListUnit[ActiveUnitIndex];
            ActiveBuilding = Map.ListBuilding[ActiveBuildingIndex];
        }

        public override void OnSelect()
        {
            Map.FinalizeMovement(ActiveUnit, 0, new System.Collections.Generic.List<Vector3>());
            ActiveUnit.UpdateSkillsLifetime(SkillEffect.LifetimeTypeOnAction);
            TerrainConquest ActiveTerrain = Map.GetTerrain(ActiveUnit.Components);

            if (ActiveBuilding.CapturedTeamIndex != Map.ListPlayer[Map.ActivePlayerIndex].TeamIndex)
            {
                ActiveBuilding.CurrentHP = Math.Max(0, ActiveBuilding.CurrentHP - ActiveUnit.HP);
                if (ActiveBuilding.CurrentHP == 0)
                {
                    ActiveBuilding.CapturedTeamIndex = Map.ActivePlayerIndex;
                    string RootPathBuilding = ActiveBuilding.RelativePath.Split('/', '\\')[0];
                    string RootPathUnit = ActiveUnit.RelativePath.Split('/', '\\')[0];
                    string NewBuildingPath = RootPathUnit + "/" + ActiveBuilding.RelativePath.Substring(RootPathBuilding.Length + 1);
                    var NewBuilding = new BuildingConquest(NewBuildingPath, Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget);
                    NewBuilding.CapturedTeamIndex = Map.ActivePlayerIndex;

                    Map.ListBuilding.RemoveAt(ActiveBuildingIndex);
                    Map.SpawnBuilding(ActivePlayerIndex, NewBuilding, 0, ActiveBuilding.Position - new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0));
                }
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
