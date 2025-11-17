using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ActionPanelPlayerBuildingUnitSelected : ActionPanelConquest
    {
        private int ActivePlayerIndex;
        private int ActiveBuildingIndex;
        private BuildingConquest ActiveBuilding;
        int FactoryMenuWidth;
        int FactoruMenuHeight;

        public ActionPanelPlayerBuildingUnitSelected(ConquestMap Map, int ActivePlayerIndex, int ActiveBuildingIndex)
            : base("Player Building Selected", Map)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            this.ActiveBuildingIndex = ActiveBuildingIndex;

            ActiveBuilding = Map.ListBuilding[ActiveBuildingIndex];
        }

        public override void OnSelect()
        {
            FactoruMenuHeight = (ActiveBuilding.ListUnitToSpawn.Count) * PannelHeight + 6 * 2;
            FactoryMenuWidth = 0;
            for (int U = 0; U < ActiveBuilding.ListUnitToSpawn.Count; U++)
            {
                string RealName = ActiveBuilding.ListUnitToSpawn[U].ItemName;
                int CurrentWidth = (int)TextHelper.fntShadowFont.MeasureString(RealName + " " + ActiveBuilding.ListUnitToSpawn[U].Cost + "$").X;

                if (CurrentWidth > FactoryMenuWidth)
                {
                    FactoryMenuWidth = CurrentWidth;
                }
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (ActiveInputManager.InputUpPressed())
            {
                ActionMenuCursor -= (ActionMenuCursor > 0) ? 1 : 0;
            }
            else if (ActiveInputManager.InputDownPressed())
            {
                ActionMenuCursor += (ActionMenuCursor < ActiveBuilding.ListUnitToSpawn.Count - 1) ? 1 : 0;
            }
            else if (ActiveInputManager.InputConfirmPressed())
            {
                UnitConquest NewUnit = new UnitConquest(ActiveBuilding.ListUnitToSpawn[ActionMenuCursor].RelativePath, Map.Content, Map.Params.DicRequirement, Map.Params.DicEffect);
                //Make sure to give the Squad an unused ID.
                uint NewUnitID = Map.GetNextUnusedUnitID();

                NewUnit.EndTurn();
                NewUnit.SpawnID = NewUnitID;
                Map.SpawnUnit(Map.ActivePlayerIndex, NewUnit, 0, ActiveBuilding.Position - new Vector3(Map.TileSize.X / 2, Map.TileSize.Y / 2, 0));
                RemoveAllSubActionPanels();
            }
            else if (ActiveInputManager.InputCancelPressed())
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
            //Draw the action panel.
            int X = FinalMenuX;
            int Y = FinalMenuY;
            X += 20;
            GameScreen.DrawBox(g, new Vector2(X, Y), FactoryMenuWidth + 50, FactoruMenuHeight, Color.White);
            X += 10;
            Y += 14;

            //Draw the choices.
            for (int U = 0; U < ActiveBuilding.ListUnitToSpawn.Count; U++)
            {
                string RealName = ActiveBuilding.ListUnitToSpawn[U].ItemName;
                TextHelper.DrawText(g, RealName + " " + ActiveBuilding.ListUnitToSpawn[U].Cost + "$", new Vector2(X, Y), Color.White);
                Y += PannelHeight;
            }

            Y = BaseMenuY;
            if (Y + FactoruMenuHeight >= Constants.Height)
                Y = Constants.Height - FactoruMenuHeight;
            //Draw the menu cursor.
            g.Draw(GameScreen.sprPixel, new Rectangle(X, 9 + Y + ActionMenuCursor * PannelHeight, FactoryMenuWidth - 20, PannelHeight - 5), Color.FromNonPremultiplied(255, 255, 255, 200));
        }
    }
}
