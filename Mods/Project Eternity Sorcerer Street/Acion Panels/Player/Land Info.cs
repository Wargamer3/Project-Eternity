using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelLandInfoPhase : ActionPanelSorcererStreet
    {
        private const string PanelName = "LandInfo";

        private enum Commands { Territory, Map, Info, Options, Help, End }

        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private TerrainSorcererStreet ActiveTerrain;

        public ActionPanelLandInfoPhase(SorcererStreetMap Map)
                : base(PanelName, Map, false)
        {
        }

        public ActionPanelLandInfoPhase(SorcererStreetMap Map, int ActivePlayerIndex)
            : base(PanelName, Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;

            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            ActiveTerrain = Map.GetTerrain(ActivePlayer.GamePiece);
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputDownPressed())
            {
                RemoveFromPanelList(this);
                switch (Map.ListTerrainType[ActiveTerrain.TerrainTypeIndex])
                {
                    case TerrainSorcererStreet.Castle:
                    case TerrainSorcererStreet.NorthTower:
                    case TerrainSorcererStreet.SouthTower:
                    case TerrainSorcererStreet.EastTower:
                    case TerrainSorcererStreet.WestTower:
                        AddToPanelListAndSelect(new ActionPanelTerritoryMenuPhase(Map, ActivePlayerIndex));
                        break;

                    default:
                        AddToPanelListAndSelect(new ActionPanelCreatureCardSelectionPhase(Map, ActivePlayerIndex));
                        break;
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelLandInfoPhase(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer);

            ActionPanelPlayerDefault.DrawPhase(g, Map, "Land Info");

            ActionPanelPlayerDefault.DrawLandInformationBottom(g, Map, ActiveTerrain);

            MenuHelper.DrawDownArrow(g);
        }
    }
}
