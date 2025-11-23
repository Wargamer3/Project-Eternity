using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelInfo : ActionPanelSorcererStreet
    {
        private int ActivePlayerIndex;
        private Player ActivePlayer;
        private int CursorIndex;

        public ActionPanelInfo(SorcererStreetMap Map)
            : base("Info", Map, false)
        {
        }

        public ActionPanelInfo(SorcererStreetMap Map, int ActivePlayerIndex)
            : base("Info", Map, false)
        {
            this.ActivePlayerIndex = ActivePlayerIndex;
            ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputConfirmPressed())
            {
                if (CursorIndex == 4)
                {
                    RemoveFromPanelList(this);
                }
            }
            else if (InputHelper.InputUpPressed())
            {
                --CursorIndex;
                if (CursorIndex < 0)
                {
                    CursorIndex = 4;
                }
            }
            else if (InputHelper.InputDownPressed())
            {
                ++CursorIndex;
                if (CursorIndex > 4)
                {
                    CursorIndex = 0;
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void DoRead(ByteReader BR)
        {
            ActivePlayerIndex = BR.ReadInt32();
        }

        public override void DoWrite(ByteWriter BW)
        {
            BW.AppendInt32(ActivePlayerIndex);
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelInfo(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int MenuBoxX = 190;
            int MenuBoxY = 110;
            int MenuBoxWidth = 446;
            int MenuBoxHeight = 330;
            int LineHeight = 52;

            MenuHelper.DrawNamedBox(g, "Menu", new Vector2(MenuBoxX, MenuBoxY), MenuBoxWidth, MenuBoxHeight);

            MenuBoxX += 42;
            MenuBoxY += 12;

            g.DrawString(Map.fntMenuText, "Information Menu", new Vector2(MenuBoxX + 20, MenuBoxY), SorcererStreetMap.TextColor);

            MenuBoxX += 66;
            MenuBoxY += 6;

            g.DrawString(Map.fntMenuText, "Hand Info", new Vector2(MenuBoxX, MenuBoxY + LineHeight), CursorIndex == 0 ? Color.Orange : SorcererStreetMap.TextColor);
            g.DrawString(Map.fntMenuText, "Player Info", new Vector2(MenuBoxX, MenuBoxY + LineHeight * 2), CursorIndex == 1 ? Color.Orange : SorcererStreetMap.TextColor);
            g.DrawString(Map.fntMenuText, "Symbol Info", new Vector2(MenuBoxX, MenuBoxY + LineHeight * 3), CursorIndex == 2 ? Color.Orange : SorcererStreetMap.TextColor);
            g.DrawString(Map.fntMenuText, "Land Info", new Vector2(MenuBoxX, MenuBoxY + LineHeight * 4), CursorIndex == 3 ? Color.Orange : SorcererStreetMap.TextColor);
            g.DrawString(Map.fntMenuText, "Return", new Vector2(MenuBoxX, MenuBoxY + LineHeight * 5), CursorIndex == 4 ? Color.Orange : SorcererStreetMap.TextColor);

            int InformationBoxX = 650;
            int InformationBoxY = 110;
            int InformationBoxWidth = 1110;
            int InformationBoxHeight = 724;

            MenuHelper.DrawNamedBox(g, "Information", new Vector2(InformationBoxX, InformationBoxY), InformationBoxWidth, InformationBoxHeight);

            switch (CursorIndex)
            {
                case 0:
                    DrawHandInfo(g, InformationBoxX, InformationBoxY, InformationBoxWidth);
                    break;

                case 1:
                    DrawPlayerInfo(g, InformationBoxX, InformationBoxY, InformationBoxWidth);
                    break;
            }

            int GameInformationBoxX = 650;
            int GameInformationBoxY = 880;
            int GameInformationBoxWidth = 1110;
            int GameInformationBoxHeight = 70;

            int IconWidth = (int)(17 * 1.4f);
            int IconHeight = (int)(32 * 1.4f);

            MenuHelper.DrawNamedBox(g, "Information", new Vector2(GameInformationBoxX, GameInformationBoxY), GameInformationBoxWidth, GameInformationBoxHeight);
            GameInformationBoxX += 66;
            g.DrawString(Map.fntMenuText, "Round: " + Map.GameTurn, new Vector2(GameInformationBoxX, GameInformationBoxY + 16), SorcererStreetMap.TextColor);
            GameInformationBoxX += 246;
            g.DrawString(Map.fntMenuText, "Objective", new Vector2(GameInformationBoxX, GameInformationBoxY + 16), SorcererStreetMap.TextColor);
            GameInformationBoxX += 190;
            g.Draw(Map.Symbols.sprMenuTG, new Rectangle(GameInformationBoxX, GameInformationBoxY + 10, IconWidth, IconHeight), SorcererStreetMap.TextColor);
            GameInformationBoxX += 176;
            g.DrawStringRightAligned(Map.fntMenuText, Map.MagicGoal.ToString(), new Vector2(GameInformationBoxX, GameInformationBoxY + 16), SorcererStreetMap.TextColor);
            GameInformationBoxX = 1358;
            g.DrawString(Map.fntMenuText, "Unlimited Rounds", new Vector2(GameInformationBoxX, GameInformationBoxY + 16), SorcererStreetMap.TextColor);
        }

        private void DrawHandInfo(CustomSpriteBatch g, int X, int Y, int Width)
        {
            int IconWidth = (int)(17 * 1.4f);
            int IconHeight = (int)(32 * 1.4f);

            int LineHeight = Map.fntMenuText.LineSpacing;
            X += 66;
            g.DrawString(Map.fntMenuText, "Hand " + ActivePlayer.ListCardInHand.Count + " Cards", new Vector2(X, Y + 10), SorcererStreetMap.TextColor);
            Y += LineHeight + 10;
            g.DrawLine(GameScreen.sprPixel, new Vector2(X, Y), new Vector2(X + Width - 146, Y), Color.White, 2);
            Y += 8;
            foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
            {
                int Offset = 0;
                foreach (Texture2D ActiveIcon in ActiveCard.GetIcons(Map.Symbols))
                {
                    g.Draw(ActiveIcon, new Rectangle(X + Offset, Y + 6, IconHeight, IconHeight), Color.White);
                    Offset += 50;
                }

                g.DrawString(Map.fntMenuText, ActiveCard.Name, new Vector2(X + Offset, Y), SorcererStreetMap.TextColor);

                g.Draw(Map.Symbols.sprMenuG, new Rectangle(X + 534, Y + 6, IconWidth, IconHeight), Color.White);

                g.DrawStringRightAligned(Map.fntMenuText, ActivePlayer.GetFinalCardCost(ActiveCard).ToString(), new Vector2(X + 654, Y), SorcererStreetMap.TextColor);
                Y += LineHeight;
            }
        }

        private void DrawPlayerInfo(CustomSpriteBatch g, float X, float Y, int Width)
        {
            int LineHeight = 52;
            int IconWidth = (int)(17 * 1.5f);
            int IconHeight = (int)(32 * 1.5f);

            Y += 10;
            X += 60;

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                Player ActivePlayer = Map.ListPlayer[P];
                if (ActivePlayer.TeamIndex < 0)
                {
                    continue;
                }

                g.Draw(Map.sprPlayerBackground, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), ActivePlayer.Color);
                g.DrawStringCentered(Map.fntMenuText, Map.DicTeam[ActivePlayer.TeamIndex].Rank.ToString(), new Vector2(X + 22, Y  + 28), SorcererStreetMap.TextColor);
                g.DrawString(Map.fntMenuText, ActivePlayer.Name, new Vector2(X + IconHeight + 5, Y), Color.White);

                Y += LineHeight;
                g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)X, (int)Y, IconWidth, IconHeight), Color.White);
                g.DrawString(Map.fntMenuText, ActivePlayer.Gold.ToString(), new Vector2(X + IconWidth + 5, Y + 6), SorcererStreetMap.TextColor);

                g.Draw(Map.Symbols.sprMenuTG, new Rectangle((int)X + 210, (int)Y, IconWidth, IconHeight), Color.White);
                g.DrawString(Map.fntMenuText, Map.DicTeam[ActivePlayer.TeamIndex].TotalMagic.ToString(), new Vector2(X + 210 + IconWidth + 5, Y + 6), SorcererStreetMap.TextColor);

                for (int C = 0; C < Map.ListCheckpoint.Count; C++)
                {
                    SorcererStreetMap.Checkpoints ActiveCheckpoint = Map.ListCheckpoint[C];
                    if (ActivePlayer.ListPassedCheckpoint.Contains(ActiveCheckpoint))
                    {
                        switch (ActiveCheckpoint)
                        {
                            case SorcererStreetMap.Checkpoints.North:
                                g.Draw(Map.sprDirectionNorthFilled, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.South:
                                g.Draw(Map.sprDirectionSouthFilled, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.East:
                                g.Draw(Map.sprDirectionEastFilled, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.West:
                                g.Draw(Map.sprDirectionWestFilled, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                        }
                    }
                    else
                    {
                        switch (ActiveCheckpoint)
                        {
                            case SorcererStreetMap.Checkpoints.North:
                                g.Draw(Map.sprDirectionNorth, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.South:
                                g.Draw(Map.sprDirectionSouth, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.East:
                                g.Draw(Map.sprDirectionEast, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.West:
                                g.Draw(Map.sprDirectionWest, new Rectangle((int)X + 412 + C * 34, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                        }
                    }
                }

                Y += LineHeight;

                if (P + 1 < Map.ListPlayer.Count)
                {
                    Y += LineHeight + 5;
                    g.DrawLine(GameScreen.sprPixel, new Vector2(X, Y), new Vector2(X + Width - 146, Y), Color.White, 2);
                }

                Y += 10;
            }
        }

        public override string ToString()
        {
            return "Allows you to view a variety of data, such as your objective value, your current status, the areas, and the players.";
        }
    }
}
