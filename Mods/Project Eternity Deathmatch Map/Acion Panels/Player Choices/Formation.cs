using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class ActionPanelFormation : ActionPanelDeathmatch
    {
        private int ActionMenuSwitchSquadCursor;
        private Squad ActiveSquad;

        public ActionPanelFormation(DeathmatchMap Map, Squad ActiveSquad)
            : base("Formation", Map)
        {
            this.ActiveSquad = ActiveSquad;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            //Move up or down in the menu.
            if (InputHelper.InputUpPressed())
            {
                --ActionMenuSwitchSquadCursor;
                if (ActiveSquad.UnitsAliveInSquad == 2)
                {
                    if (ActionMenuSwitchSquadCursor < 0)
                        ActionMenuSwitchSquadCursor = 1;
                }
                if (ActiveSquad.UnitsAliveInSquad == 3)
                {
                    if (ActionMenuSwitchSquadCursor < 0)
                        ActionMenuSwitchSquadCursor = 5;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                ++ActionMenuSwitchSquadCursor;
                if (ActiveSquad.UnitsAliveInSquad == 2)
                {
                    if (ActionMenuSwitchSquadCursor >= 2)
                        ActionMenuSwitchSquadCursor = 0;
                }
                if (ActiveSquad.UnitsAliveInSquad == 3)
                {
                    if (ActionMenuSwitchSquadCursor >= 6)
                        ActionMenuSwitchSquadCursor = 0;
                }
            }
            if (InputHelper.InputConfirmPressed())
            {
                int OldLeader = 0;
                int OldWingmanA = 1;
                int OldWingmanB = 2;

                if (ActiveSquad.UnitsAliveInSquad == 2)
                {
                    if (ActiveSquad.At(OldLeader).HP <= 0)
                    {
                        OldLeader = OldWingmanA;
                        OldWingmanA = OldWingmanB;
                    }
                    else if (ActiveSquad.At(OldWingmanA).HP <= 0)
                    {
                        OldWingmanA = OldWingmanB;
                    }

                    if (ActionMenuSwitchSquadCursor == 0)
                    {
                        ActiveSquad.SetLeader(OldLeader);
                        ActiveSquad.SetWingmanA(OldWingmanA);
                    }
                    else if (ActionMenuSwitchSquadCursor == 1)
                    {
                        ActiveSquad.SetLeader(OldWingmanA);
                        ActiveSquad.SetWingmanA(OldLeader);
                    }
                }
                else if (ActiveSquad.UnitsAliveInSquad == 3)
                {
                    switch (ActionMenuSwitchSquadCursor)
                    {
                        case 0:
                            ActiveSquad.SetLeader(OldLeader);
                            ActiveSquad.SetWingmanA(OldWingmanA);
                            ActiveSquad.SetWingmanB(OldWingmanB);
                            break;

                        case 1:
                            ActiveSquad.SetLeader(OldLeader);
                            ActiveSquad.SetWingmanA(OldWingmanB);
                            ActiveSquad.SetWingmanB(OldWingmanA);
                            break;

                        case 2:
                            ActiveSquad.SetLeader(OldWingmanA);
                            ActiveSquad.SetWingmanA(OldLeader);
                            ActiveSquad.SetWingmanB(OldWingmanB);
                            break;

                        case 3:
                            ActiveSquad.SetLeader(OldWingmanA);
                            ActiveSquad.SetWingmanA(OldWingmanB);
                            ActiveSquad.SetWingmanB(OldLeader);
                            break;

                        case 4:
                            ActiveSquad.SetLeader(OldWingmanB);
                            ActiveSquad.SetWingmanA(OldLeader);
                            ActiveSquad.SetWingmanB(OldWingmanA);
                            break;

                        case 5:
                            ActiveSquad.SetLeader(OldWingmanB);
                            ActiveSquad.SetWingmanA(OldWingmanA);
                            ActiveSquad.SetWingmanB(OldLeader);
                            break;
                    }
                }
            }
            if (InputHelper.InputCancelPressed())
            {
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int X, Y;

            g.Draw(GameScreen.sprPixel, new Vector2((ActiveSquad.X - Map.CameraPosition.X) * Map.TileSize.X, (ActiveSquad.Y - Map.CameraPosition.Y) * Map.TileSize.Y), Color.FromNonPremultiplied(255, 255, 255, 200));

            #region 3 Unit Squad

            if (ActiveSquad.UnitsAliveInSquad == 3)
            {
                int MenuCursorHeight = Map.TileSize.Y + 2 + 2;
                X = (Constants.Width - MinActionMenuWidth) / 2;
                Y = (Constants.Height - MinActionMenuWidth) / 2;
                int ColumnWidth = MinActionMenuWidth / 3;
                int Col1X = X + 2;
                int Col2X = X + 2 + ColumnWidth;
                int Col3X = X + 2 + ColumnWidth + ColumnWidth;

                GameScreen.DrawBox(g, new Vector2(X, Y), MinActionMenuWidth, MenuCursorHeight * 5, Color.White);

                //1, 2, 3
                g.Draw(ActiveSquad.At(0).SpriteMap, new Vector2(Col1X, Y + 2), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(0).RelativePath, new Vector2(Col1X + 40, Y + 10), Color.White);
                g.Draw(ActiveSquad.At(1).SpriteMap, new Vector2(Col2X, Y + 2), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(1).RelativePath, new Vector2(Col2X + 40, Y + 10), Color.White);
                g.Draw(ActiveSquad.At(2).SpriteMap, new Vector2(Col3X, Y + 2), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(2).RelativePath, new Vector2(Col3X + 40, Y + 10), Color.White);
                //1, 3, 2
                g.Draw(ActiveSquad.At(0).SpriteMap, new Vector2(Col1X, Y + 2 + MenuCursorHeight), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(0).RelativePath, new Vector2(Col1X + 40, Y + 10 + MenuCursorHeight), Color.White);
                g.Draw(ActiveSquad.At(2).SpriteMap, new Vector2(Col2X, Y + 2 + MenuCursorHeight), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(2).RelativePath, new Vector2(Col2X + 40, Y + 10 + MenuCursorHeight), Color.White);
                g.Draw(ActiveSquad.At(1).SpriteMap, new Vector2(Col3X, Y + 2 + MenuCursorHeight), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(1).RelativePath, new Vector2(Col3X + 40, Y + 10 + MenuCursorHeight), Color.White);
                //2, 1, 3
                g.Draw(ActiveSquad.At(1).SpriteMap, new Vector2(Col1X, Y + 2 + MenuCursorHeight * 2), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(1).RelativePath, new Vector2(Col1X + 40, Y + 10 + MenuCursorHeight * 2), Color.White);
                g.Draw(ActiveSquad.At(0).SpriteMap, new Vector2(Col2X, Y + 2 + MenuCursorHeight * 2), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(0).RelativePath, new Vector2(Col2X + 40, Y + 10 + MenuCursorHeight * 2), Color.White);
                g.Draw(ActiveSquad.At(2).SpriteMap, new Vector2(Col3X, Y + 2 + MenuCursorHeight * 2), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(2).RelativePath, new Vector2(Col3X + 40, Y + 10 + MenuCursorHeight * 2), Color.White);
                //2, 3, 1
                g.Draw(ActiveSquad.At(1).SpriteMap, new Vector2(Col1X, Y + 2 + MenuCursorHeight * 3), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(1).RelativePath, new Vector2(Col1X + 40, Y + 10 + MenuCursorHeight * 3), Color.White);
                g.Draw(ActiveSquad.At(2).SpriteMap, new Vector2(Col2X, Y + 2 + MenuCursorHeight * 3), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(2).RelativePath, new Vector2(Col2X + 40, Y + 10 + MenuCursorHeight * 3), Color.White);
                g.Draw(ActiveSquad.At(0).SpriteMap, new Vector2(Col3X, Y + 2 + MenuCursorHeight * 3), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(0).RelativePath, new Vector2(Col3X + 40, Y + 10 + MenuCursorHeight * 3), Color.White);
                //3, 1, 2
                g.Draw(ActiveSquad.At(2).SpriteMap, new Vector2(Col1X, Y + 2 + MenuCursorHeight * 4), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(2).RelativePath, new Vector2(Col1X + 40, Y + 10 + MenuCursorHeight * 4), Color.White);
                g.Draw(ActiveSquad.At(0).SpriteMap, new Vector2(Col2X, Y + 2 + MenuCursorHeight * 4), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(0).RelativePath, new Vector2(Col2X + 40, Y + 10 + MenuCursorHeight * 4), Color.White);
                g.Draw(ActiveSquad.At(1).SpriteMap, new Vector2(Col3X, Y + 2 + MenuCursorHeight * 4), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(1).RelativePath, new Vector2(Col3X + 40, Y + 10 + MenuCursorHeight * 4), Color.White);
                //3, 2, 1
                g.Draw(ActiveSquad.At(2).SpriteMap, new Vector2(Col1X, Y + 2 + MenuCursorHeight * 5), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(2).RelativePath, new Vector2(Col1X + 40, Y + 10 + MenuCursorHeight * 5), Color.White);
                g.Draw(ActiveSquad.At(1).SpriteMap, new Vector2(Col2X, Y + 2 + MenuCursorHeight * 5), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(1).RelativePath, new Vector2(Col2X + 40, Y + 10 + MenuCursorHeight * 5), Color.White);
                g.Draw(ActiveSquad.At(0).SpriteMap, new Vector2(Col3X, Y + 2 + MenuCursorHeight * 5), Color.White);
                TextHelper.DrawText(g, ActiveSquad.At(0).RelativePath, new Vector2(Col3X + 40, Y + 10 + MenuCursorHeight * 5), Color.White);

                g.Draw(GameScreen.sprPixel, new Rectangle(X, Y + ActionMenuSwitchSquadCursor * MenuCursorHeight, MinActionMenuWidth, MenuCursorHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
            }

            #endregion

            else if (ActiveSquad.UnitsAliveInSquad == 2)
            {
                int MenuCursorHeight = Map.TileSize.Y + 2 + 2;
                X = (Constants.Width - MinActionMenuWidth) / 2;
                Y = (Constants.Height - MinActionMenuWidth) / 2;

                int ColumnWidth = MinActionMenuWidth / 2;
                int Col1X = X + 2;
                int Col2X = X + 2 + ColumnWidth;

                GameScreen.DrawBox(g, new Vector2(X, Y), MinActionMenuWidth, MenuCursorHeight * 5, Color.White);
                Unit Unit1 = ActiveSquad.At(0);
                Unit Unit2 = ActiveSquad.At(1);
                if (Unit1.HP <= 0)
                {
                    Unit1 = Unit2;
                    Unit2 = ActiveSquad.At(2);
                }
                else if (Unit2.HP <= 0)
                {
                    Unit2 = ActiveSquad.At(2);
                }
                //1, 2
                g.Draw(Unit1.SpriteMap, new Vector2(Col1X, Y + 2), Color.White);
                TextHelper.DrawText(g, Unit1.RelativePath, new Vector2(Col1X + 40, Y + 10), Color.White);
                g.Draw(Unit2.SpriteMap, new Vector2(Col2X, Y + 2), Color.White);
                TextHelper.DrawText(g, Unit2.RelativePath, new Vector2(Col2X + 40, Y + 10), Color.White);
                //2, 0
                g.Draw(Unit2.SpriteMap, new Vector2(Col1X, Y + 2 + MenuCursorHeight), Color.White);
                TextHelper.DrawText(g, Unit2.RelativePath, new Vector2(Col1X + 40, Y + 10 + MenuCursorHeight), Color.White);
                g.Draw(Unit1.SpriteMap, new Vector2(Col2X, Y + 2 + MenuCursorHeight), Color.White);
                TextHelper.DrawText(g, Unit1.RelativePath, new Vector2(Col2X + 40, Y + 10 + MenuCursorHeight), Color.White);

                g.Draw(GameScreen.sprPixel, new Rectangle(X, Y + ActionMenuSwitchSquadCursor * MenuCursorHeight, MinActionMenuWidth, MenuCursorHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
            }
        }
    }
}
