using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerBuildingUnitSelected : ActionPanelConquest
    {
        public ActionPanelPlayerBuildingUnitSelected(ConquestMap Map)
            : base("Player Building Selected", Map)
        {
        }

        public override void OnSelect()
        {
            Map.BuildingMenuCursor = 0;

            if (Map.ListTerrainType[Map.GetTerrain((int)Map.CursorPosition.X, (int)Map.CursorPosition.Y, 0).TerrainTypeIndex] == "Factory")
                Map.ListCurrentBuildingChoice = Map.DicBuildingChoice["Vehicule"];
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (Map.GetTerrain((int)Map.CursorPosition.X, (int)Map.CursorPosition.Y, 0).CapturedPlayerIndex == Map.ActivePlayerIndex &&
                Map.GetTerrain((int)Map.CursorPosition.X, (int)Map.CursorPosition.Y, 0).CapturePoints == 0)
            {
                if (InputHelper.InputUpPressed())
                {
                    Map.BuildingMenuCursor -= (Map.BuildingMenuCursor > 0) ? 1 : 0;
                }
                else if (InputHelper.InputDownPressed())
                {
                    Map.BuildingMenuCursor += (Map.BuildingMenuCursor < Map.ListCurrentBuildingChoice.Count - 1) ? 1 : 0;
                }
                else if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
                {
                    UnitConquest NewUnit = new UnitConquest(Map.ListCurrentBuildingChoice[Map.BuildingMenuCursor], Map.Content, Map.DicRequirement, Map.DicEffect);
                    //Make sure to give the Squad an unused ID.
                    uint NewUnitID = Map.GetNextUnusedUnitID();

                    NewUnit.EndTurn();
                    NewUnit.ID = NewUnitID;
                    Map.SpawnUnit(Map.ActivePlayerIndex, NewUnit, Map.CursorPosition);
                }
                else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
                {
                    //Reset the cursor.
                    Map.sndCancel.Play();
                }
            }
        }

        public override void DoRead(ByteReader BR)
        {
            Map.BuildingMenuCursor = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(Map.BuildingMenuCursor);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerBuildingUnitSelected(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(GameScreen.sprPixel, new Rectangle((int)(Map.CursorPosition.X + 1) * Map.TileSize.X, (int)Map.CursorPosition.Y * Map.TileSize.Y, 100, Map.ListCurrentBuildingChoice.Count * 20), Color.Navy);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)(Map.CursorPosition.X + 1) * Map.TileSize.X, (int)Map.CursorPosition.Y * Map.TileSize.Y + Map.BuildingMenuCursor * 20, 100, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
            for (int i = 0; i < Map.ListCurrentBuildingChoice.Count; i++)
            {
                string RealName = Map.ListCurrentBuildingChoice[i].Split(new string[1] { "\\" }, StringSplitOptions.None).Last();
                g.DrawString(Map.fntArial12, RealName + " " + Map.DicUnitCost[RealName] + "$", new Vector2((Map.CursorPosition.X + 1) * Map.TileSize.X, Map.CursorPosition.Y * Map.TileSize.Y + i * 20), Color.White);
            }
        }
    }
}
