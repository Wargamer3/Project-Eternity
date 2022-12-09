using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
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
            int MenuBoxX = Constants.Width / 12;
            int MenuBoxY = Constants.Height / 10;
            int MenuBoxWidth = Constants.Width / 6;
            int MenuBoxHeight = Constants.Height / 5;
            int LineHeight = Constants.Height / 35;

            MenuHelper.DrawNamedBox(g, "Menu", new Vector2(MenuBoxX, MenuBoxY), MenuBoxWidth, MenuBoxHeight);

            g.DrawString(Map.fntArial12, "Information Menu", new Vector2(MenuBoxX + 20, MenuBoxY + 10), Color.White);
            g.DrawString(Map.fntArial12, "Hand Info", new Vector2(MenuBoxX + 40, MenuBoxY + 10 + LineHeight), CursorIndex == 0 ? Color.Orange : Color.White);
            g.DrawString(Map.fntArial12, "Cepter Info", new Vector2(MenuBoxX + 40, MenuBoxY + 10 + LineHeight * 2), CursorIndex == 1 ? Color.Orange : Color.White);
            g.DrawString(Map.fntArial12, "Symbol Info", new Vector2(MenuBoxX + 40, MenuBoxY + 10 + LineHeight * 3), CursorIndex == 2 ? Color.Orange : Color.White);
            g.DrawString(Map.fntArial12, "Land Info", new Vector2(MenuBoxX + 40, MenuBoxY + 10 + LineHeight * 4), CursorIndex == 3 ? Color.Orange : Color.White);
            g.DrawString(Map.fntArial12, "Return", new Vector2(MenuBoxX + 40, MenuBoxY + 10 + LineHeight * 5), CursorIndex == 4 ? Color.Orange : Color.White);

            int InformationBoxX = Constants.Width / 3;
            int InformationBoxY = Constants.Height / 9;
            int InformationBoxWidth = Constants.Width / 2;
            int InformationBoxHeight = (int)(Constants.Height / 1.5f);

            MenuHelper.DrawNamedBox(g, "Information", new Vector2(InformationBoxX, InformationBoxY), InformationBoxWidth, InformationBoxHeight);

            switch (CursorIndex)
            {
                case 0:
                    DrawHandInfo(g, InformationBoxX, InformationBoxY, InformationBoxWidth);
                    break;

                case 1:
                    DrawCepterInfo(g, InformationBoxX, InformationBoxY, InformationBoxWidth);
                    break;
            }

            int GameInformationBoxX = Constants.Width / 3;
            int GameInformationBoxY = InformationBoxY + InformationBoxHeight + 54;
            int GameInformationBoxWidth = Constants.Width / 2;
            int GameInformationBoxHeight = Constants.Height / 20;

            int IconWidth = Constants.Width / 112;
            int IconHeight = Constants.Width / 60;
            MenuHelper.DrawNamedBox(g, "Information", new Vector2(GameInformationBoxX, GameInformationBoxY), GameInformationBoxWidth, GameInformationBoxHeight);
            g.DrawStringVerticallyAligned(Map.fntArial12, "Round: " + Map.GameTurn, new Vector2(GameInformationBoxX + 40, GameInformationBoxY + GameInformationBoxHeight / 2), Color.White);
            g.DrawStringVerticallyAligned(Map.fntArial12, "Objective", new Vector2(GameInformationBoxX + 150, GameInformationBoxY + GameInformationBoxHeight / 2), Color.White);
            g.Draw(Map.Symbols.sprMenuTG, new Rectangle(GameInformationBoxX + 240, GameInformationBoxY + GameInformationBoxHeight / 2 - Map.Symbols.sprMenuTG.Height / 2, IconWidth, IconHeight), Color.White);
            g.DrawStringVerticallyAligned(Map.fntArial12, Map.MagicGoal.ToString(), new Vector2(GameInformationBoxX + 280, GameInformationBoxY + GameInformationBoxHeight / 2), Color.White);
            g.DrawStringVerticallyAligned(Map.fntArial12, "Unlimited Rounds", new Vector2(GameInformationBoxX + 380, GameInformationBoxY + GameInformationBoxHeight / 2), Color.White);
        }

        private void DrawHandInfo(CustomSpriteBatch g, float X, float Y, int Width)
        {
            int LineHeight = Constants.Height / 35;

            g.DrawString(Map.fntArial12, "Hand " + ActivePlayer.ListCardInHand.Count + " Cards", new Vector2(X + 20, Y + 10), Color.White);
            Y += LineHeight + 5;
            g.DrawLine(GameScreen.sprPixel, new Vector2(X + 20, Y), new Vector2(X + Width - 40, Y), Color.White);
            foreach (Card ActiveCard in ActivePlayer.ListCardInHand)
            {
                int Offset = 30;
                foreach (Texture2D ActiveIcon in ActiveCard.GetIcons(Map.Symbols))
                {
                    g.Draw(ActiveIcon, new Vector2(X + Offset, Y + 5), Color.White);
                    Offset += ActiveIcon.Width + 10;
                }

                g.DrawString(Map.fntArial12, ActiveCard.Name, new Vector2(X + Offset, Y + 10), Color.White);
                Y += LineHeight;
            }
        }

        private void DrawCepterInfo(CustomSpriteBatch g, float X, float Y, int Width)
        {
            int LineHeight = Constants.Height / 30;
            int IconWidth = Constants.Width / 112;
            int IconHeight = Constants.Width / 60;

            Y += 10;
            X += 30;

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                Player ActivePlayer = Map.ListPlayer[P];
                g.Draw(Map.sprPlayerBackground, new Rectangle((int)X, (int)Y, IconHeight, IconHeight), ActivePlayer.Color);
                g.DrawStringCentered(Map.fntArial12, ActivePlayer.Rank.ToString(), new Vector2(X + IconHeight / 2, Y + IconHeight / 2), Color.White);
                g.DrawString(Map.fntArial12, ActivePlayer.Name, new Vector2(X + IconHeight + 5, Y), Color.White);

                Y += LineHeight;
                g.Draw(Map.Symbols.sprMenuG, new Rectangle((int)X, (int)Y, IconWidth, IconHeight), Color.White);
                g.DrawString(Map.fntArial12, ActivePlayer.Magic.ToString(), new Vector2(X + IconWidth + 5, Y), Color.White);

                g.Draw(Map.Symbols.sprMenuTG, new Rectangle((int)X + 100, (int)Y, IconWidth, IconHeight), Color.White);
                g.DrawString(Map.fntArial12, ActivePlayer.TotalMagic.ToString(), new Vector2(X + 100 + IconWidth + 5, Y), Color.White);

                for (int C = 0; C < Map.ListCheckpoint.Count; C++)
                {
                    SorcererStreetMap.Checkpoints ActiveCheckpoint = Map.ListCheckpoint[C];
                    if (ActivePlayer.ListPassedCheckpoint.Contains(ActiveCheckpoint))
                    {
                        switch (ActiveCheckpoint)
                        {
                            case SorcererStreetMap.Checkpoints.North:
                                g.Draw(Map.sprDirectionNorthFilled, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.South:
                                g.Draw(Map.sprDirectionSouthFilled, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.East:
                                g.Draw(Map.sprDirectionEastFilled, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.West:
                                g.Draw(Map.sprDirectionWestFilled, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                        }
                    }
                    else
                    {
                        switch (ActiveCheckpoint)
                        {
                            case SorcererStreetMap.Checkpoints.North:
                                g.Draw(Map.sprDirectionNorth, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.South:
                                g.Draw(Map.sprDirectionSouth, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.East:
                                g.Draw(Map.sprDirectionEast, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                            case SorcererStreetMap.Checkpoints.West:
                                g.Draw(Map.sprDirectionWest, new Rectangle((int)X + 200 + C * 20, (int)Y, IconHeight, IconHeight), Color.White);
                                break;
                        }
                    }
                }

                Y += LineHeight;

                if (P + 1 < Map.ListPlayer.Count)
                {
                    Y += LineHeight + 5;
                    g.DrawLine(GameScreen.sprPixel, new Vector2(X + 20, Y), new Vector2(X + Width - 40, Y), Color.White);
                }

                Y += 10;
            }
        }

        public override string ToString()
        {
            return "Allows you to view a variety of data, such as your objective value, your current status, the areas, and the Cepters.";
        }
    }
}
