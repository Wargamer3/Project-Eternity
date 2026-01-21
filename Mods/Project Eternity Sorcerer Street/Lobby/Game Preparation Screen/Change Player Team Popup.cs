using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    class GamePreparationChangePlayerTeamPopup : ActionPanel
    {
        private SpriteFont fntText;

        private readonly RoomInformations Room;
        private readonly int BoxWidth;
        private readonly int BoxHeight;
        private int DrawY = 120;

        private double HoverProgression;

        private int SelectedMenuIndex;

        private readonly int SelectedPlayerIndex;
        private int NumberOfTeams => 4;

        public GamePreparationChangePlayerTeamPopup(ActionPanelHolder ListActionMenuChoice, SpriteFont fntText, RoomInformations Room, int SelectedPlayerIndex)
            : base("Change Player Team Popup", ListActionMenuChoice, false)
        {
            this.fntText = fntText;
            this.Room = Room;
            this.SelectedPlayerIndex = SelectedPlayerIndex;

            BoxWidth = SorcererStreetGamePreparationScreen.BoxWidth;
            BoxHeight = SorcererStreetGamePreparationScreen.BoxHeight;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HoverProgression += gameTime.ElapsedGameTime.TotalSeconds;

            int X = 15 + SelectedPlayerIndex * BoxWidth;

            for (int I = 0; I < NumberOfTeams; ++I)
            {
                int Y = DrawY + 243 + I * 25;
                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + 25)
                {
                    SelectedMenuIndex = I;

                    if (MouseHelper.InputLeftButtonPressed())
                    {
                        Room.ListRoomPlayer[SelectedPlayerIndex].TeamIndex = SelectedMenuIndex;
                        RemoveFromPanelList(this);
                        return;
                    }
                }
            }
            if (MouseHelper.InputLeftButtonPressed())
            {
                RemoveFromPanelList(this);
            }
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int X = 5 + SelectedPlayerIndex * BoxWidth;
            int Y = DrawY + 238;
            g.Draw(GameScreen.sprPixel, new Rectangle(X + 15, Y, BoxWidth - 30, NumberOfTeams * 25 + 10), Lobby.BackgroundColor);
            GameScreen.DrawEmptyBox(g, new Vector2(X + 15, Y), BoxWidth - 30, NumberOfTeams * 25 + 10, 10, HoverProgression);
            Y += 5;
            g.Draw(GameScreen.sprPixel, new Rectangle(X + 20, Y + SelectedMenuIndex * 25, BoxWidth - 40, 25), Color.FromNonPremultiplied(255, 255, 255, 127));

            Y += 13;
            g.DrawStringCentered(fntText, "Red", new Vector2(X + BoxWidth / 2, Y), Color.White);
            Y += 25;
            g.DrawStringCentered(fntText, "Blue", new Vector2(X + BoxWidth / 2, Y), Color.White);
            Y += 25;
            g.DrawStringCentered(fntText, "Yellow", new Vector2(X + BoxWidth / 2, Y), Color.White);
            Y += 25;
            g.DrawStringCentered(fntText, "Green", new Vector2(X + BoxWidth / 2, Y), Color.White);
            Y += 25;
        }

        protected override ActionPanel Copy()
        {
            return new GamePreparationChangePlayerTypePopup(ListActionMenuChoice, fntText, Room, SelectedPlayerIndex);
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
