using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class ActionPanelPlaceNewConstruction : ActionPanelWorldMap
    {
        int ConstructionMenuIndex;
        const int ConstructionPerLine = 3;
        bool[,] ArrayConstructionZoneEmpty;

        public ActionPanelPlaceNewConstruction(WorldMap Map)
            : base("Place New Construction", Map, false)
        {
            ConstructionMenuIndex = 0;
        }

        public override void OnSelect()
        {
            Map.ListPlayer[Map.ActivePlayerIndex].OpenConstructionMenu();
            UpdateConstructionZone();
        }

        public override void DoUpdate(GameTime gameTime)
        {
            #region Move Cursor

            if (InputHelper.InputRightPressed())
            {
                ConstructionMenuIndex++;
                if (ConstructionMenuIndex >= Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count)
                    ConstructionMenuIndex = (Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count / ConstructionPerLine) * ConstructionPerLine;
                else if (ConstructionMenuIndex % ConstructionPerLine == 0)
                    ConstructionMenuIndex -= ConstructionPerLine;

                UpdateConstructionZone();
            }
            else if (InputHelper.InputLeftPressed())
            {
                if (ConstructionMenuIndex % ConstructionPerLine == 0)
                {
                    ConstructionMenuIndex += ConstructionPerLine - 1;
                    if (ConstructionMenuIndex >= Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count)
                        ConstructionMenuIndex = Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count - 1;
                }
                else
                    ConstructionMenuIndex--;

                UpdateConstructionZone();
            }
            else if (InputHelper.InputUpPressed())
            {
                if ((ConstructionMenuIndex - ConstructionPerLine < 0))
                {
                    ConstructionMenuIndex = (Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count / ConstructionPerLine) * ConstructionPerLine + ConstructionMenuIndex % ConstructionPerLine;
                    if (ConstructionMenuIndex >= Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count)
                        ConstructionMenuIndex -= ConstructionPerLine;
                }
                else
                    ConstructionMenuIndex -= ConstructionPerLine;

                UpdateConstructionZone();
            }
            else if (InputHelper.InputDownPressed())
            {
                ConstructionMenuIndex += ConstructionPerLine;
                if (ConstructionMenuIndex >= Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count)
                    ConstructionMenuIndex = ConstructionMenuIndex % ConstructionPerLine;

                UpdateConstructionZone();
            }

            #endregion

            else if (InputHelper.InputConfirmPressed())
            {
                bool CanBuild = true;
                Point ConstructionSize = Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[ConstructionMenuIndex].MapSize;

                for (int X = 0; X < ConstructionSize.X && CanBuild; X++)
                {
                    for (int Y = 0; Y < ConstructionSize.Y && CanBuild; Y++)
                    {
                        if (!ArrayConstructionZoneEmpty[X, Y])
                            CanBuild = false;
                    }
                }

                if (CanBuild)
                {
                    Construction NewConstruction = Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[ConstructionMenuIndex].Copy();
                    Map.SpawnConstruction(Map.ActivePlayerIndex, NewConstruction, Map.CursorPosition);
                    UpdateConstructionZone();
                }
            }
            else if (InputHelper.InputCancelPressed())
            {
                RemoveAllSubActionPanels();
            }
        }

        public override void DoRead(ByteReader BR)
        {
            ConstructionMenuIndex = BR.ReadInt32();
            Map.ListPlayer[Map.ActivePlayerIndex].OpenConstructionMenu();
            UpdateConstructionZone();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ConstructionMenuIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelPlaceNewConstruction(Map);
        }

        public void UpdateConstructionZone()
        {
            Point ContructionSize = Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[ConstructionMenuIndex].MapSize;
            ArrayConstructionZoneEmpty = new bool[ContructionSize.X,
                                             ContructionSize.Y];

            for (int X = 0; X < ContructionSize.X; X++)
                for (int Y = 0; Y < ContructionSize.Y; Y++)
                {
                    ArrayConstructionZoneEmpty[X, Y] = true;
                    //Loop through the players to find a Unit or a Construction.
                    for (int P = 0; P < Map.ListPlayer.Count && ArrayConstructionZoneEmpty[X, Y]; P++)
                    {
                        if (!ArrayConstructionZoneEmpty[X, Y])
                            continue;
                        //Find if a current player Construction is under the cursor.
                        int CursorSelect = Map.CheckForConstructionAtPosition(P, Map.CursorPosition, new Vector3(X, Y, 0));

                        //If one was found.
                        if (CursorSelect >= 0)
                            ArrayConstructionZoneEmpty[X, Y] = false;
                        else
                        {
                            //Find if a current player Unit is under the cursor.
                            CursorSelect = Map.CheckForConstructionAtPosition(P, Map.CursorPosition, new Vector3(X, Y, 0));
                            if (CursorSelect >= 0)
                                ArrayConstructionZoneEmpty[X, Y] = false;
                        }
                    }
                }
        }
        
        public override void Draw(CustomSpriteBatch g)
        {
            Map.DrawConstructionMenuInfo(g);
            g.DrawString(Map.fntArial12, "Constructions", new Vector2(Constants.Width - 97, 2), Color.White);

            //Draw Construction choices
            for (int C = 0; C < Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible.Count; C++)
            {
                g.Draw(Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[C].MenuIcon, new Vector2(Constants.Width - 98 + (C % ConstructionPerLine) * 35, Constants.Height - 200 + (C / ConstructionPerLine) * 35), Color.White);
                if (Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[C].Price < Map.ListPlayer[Map.ActivePlayerIndex].EnergyReserve)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 98 + (C % ConstructionPerLine) * 35, Constants.Height - 200 + (C / ConstructionPerLine) * 35,
                                                    Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[C].MenuIcon.Width, Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[C].MenuIcon.Height),
                                                    Color.FromNonPremultiplied(127, 127, 127, 200));
                }
            }

            //Draw Construction cursor
            g.Draw(GameScreen.sprPixel, new Rectangle(Constants.Width - 98 + (ConstructionMenuIndex % ConstructionPerLine) * 35,
                Constants.Height - 200 + (ConstructionMenuIndex / ConstructionPerLine) * 35, 32, 32),
                Color.FromNonPremultiplied(255, 255, 255, 190));

            //Draw build location
            for (int X = 0; X < Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[ConstructionMenuIndex].MapSize.X; X++)
            {
                for (int Y = 0; Y < Map.ListPlayer[Map.ActivePlayerIndex].ListConstructionChoiceVisible[ConstructionMenuIndex].MapSize.Y; Y++)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle((int)(Map.CursorPosition.X + X - Map.CameraPosition.X) * Map.TileSize.X,
                        (int)(Map.CursorPosition.Y + Y - Map.CameraPosition.Y) * Map.TileSize.Y, 32, 32),
                        ArrayConstructionZoneEmpty[X, Y] ? Color.FromNonPremultiplied(0, 128, 0, 190) : Color.FromNonPremultiplied(255, 0, 0, 190));
                }
            }
        }
    }
}
