using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.ControlHelper;
using Microsoft.Xna.Framework.Graphics;

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
            ActionPanelPlayerDefault.DrawPlayerInformation(g, Map, ActivePlayer, Constants.Width / 16, Constants.Height / 10);

            int ActionInfoBoxX = Constants.Width / 16;
            int ActionInfoBoxY = Constants.Height / 3;
            int ActionInfoBoxWidth = Constants.Width / 5;
            int ActionInfoBoxHeight = Constants.Height / 14;

            MenuHelper.DrawBorderlessBox(g, new Vector2(ActionInfoBoxX, ActionInfoBoxY), ActionInfoBoxWidth, ActionInfoBoxHeight);
            g.DrawStringCentered(Map.fntArial12, "Land Info", new Vector2(ActionInfoBoxX + ActionInfoBoxWidth / 2, ActionInfoBoxY + ActionInfoBoxHeight / 2), Color.White);

            ActionPanelPlayerDefault.DrawLandInformation(g, Map, ActiveTerrain, Constants.Width / 16f, Constants.Height - Constants.Height / 3.5f);

            MenuHelper.DrawDownArrow(g);
        }
    }
}
