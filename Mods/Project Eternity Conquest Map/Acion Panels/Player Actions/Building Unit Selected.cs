using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerBuildingUnitSelected : ActionPanelConquest
    {
        private int ActivePlayerIndex;
        private int ActiveBuildingIndex;
        private BuildingConquest ActiveBuilding;

        public ActionPanelPlayerBuildingUnitSelected(ConquestMap Map, int ActivePlayerIndex, int ActiveBuildingIndex)
            : base("Player Building Selected", Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveBuildingIndex = ActiveBuildingIndex;

            ActiveBuilding = Map.ListBuilding[ActiveBuildingIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;
            }
            else if (InputHelper.InputDownPressed())
            {
                ActionMenuCursor += (ActionMenuCursor < ActiveBuilding.ListUnitToSpawn.Count - 1) ? 1 : 0;
            }
            else if (InputHelper.InputConfirmPressed() || MouseHelper.InputLeftButtonReleased())
            {
                UnitConquest NewUnit = new UnitConquest(ActiveBuilding.ListUnitToSpawn[ActionMenuCursor].RelativePath, Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect);
                //Make sure to give the Squad an unused ID.
                uint NewUnitID = Map.GetNextUnusedUnitID();

                NewUnit.EndTurn();
                NewUnit.SpawnID = NewUnitID;
                Map.SpawnUnit(Map.ActivePlayerIndex, NewUnit, 0, ActiveBuilding.Position - new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0));
                RemoveAllSubActionPanels();
            }
            else if (InputHelper.InputCancelPressed() || MouseHelper.InputRightButtonReleased())
            {
                //Reset the cursor.
                Map.sndCancel.Play();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ActiveBuildingIndex = BR.ReadInt32();
            ActionMenuCursor = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActiveBuildingIndex);
            BW.AppendInt32(ActionMenuCursor);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlayerBuildingUnitSelected(Map, ActivePlayerIndex, ActiveBuildingIndex);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            g.Draw(GameScreen.sprPixel, new Rectangle((int)(Map.CursorPosition.X + 1), (int)Map.CursorPosition.Y, 100, ActiveBuilding.ListUnitToSpawn.Count * 20), Color.Navy);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)(Map.CursorPosition.X + 1), (int)Map.CursorPosition.Y + ActionMenuCursor * 20, 100, 20), Color.FromNonPremultiplied(255, 255, 255, 127));
            for (int i = 0; i < ActiveBuilding.ListUnitToSpawn.Count; i++)
            {
                string RealName = ActiveBuilding.ListUnitToSpawn[i].ItemName;
                g.DrawString(Map.fntArial12, RealName + " " + ActiveBuilding.ListUnitToSpawn[i].Cost + "$", new Vector2((Map.CursorPosition.X + 1), Map.CursorPosition.Y + i * 20), Color.White);
            }
        }
    }
}
