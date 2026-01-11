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
    class GamePreparationConfigurePlayerPopup : ActionPanel
    {
        private SpriteFont fntText;
        private CardSymbols Symbols;
        private IconHolder Icons;

        private readonly RoomInformations Room;
        private readonly GameScreen Owner;
        private readonly int BoxWidth;
        private readonly int BoxHeight;
        private int DrawY = 120;

        private double HoverProgression;

        private int SelectedMenuIndex;

        private readonly int SelectedPlayerIndex;

        public GamePreparationConfigurePlayerPopup(ActionPanelHolder ListActionMenuChoice, SpriteFont fntText, RoomInformations Room, int SelectedPlayerIndex, GameScreen Owner)
            : base("Configure Player Popup", ListActionMenuChoice, false)
        {
            this.fntText = fntText;
            this.Room = Room;
            this.Owner = Owner;
            this.SelectedPlayerIndex = SelectedPlayerIndex;

            BoxWidth = SorcererStreetGamePreparationScreen.BoxWidth;
            BoxHeight = SorcererStreetGamePreparationScreen.BoxHeight;
        }

        public override void OnSelect()
        {
            Symbols = CardSymbols.Symbols;
            Icons = IconHolder.Icons;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            HoverProgression += gameTime.ElapsedGameTime.TotalSeconds;

            int X = 15 + SelectedPlayerIndex * BoxWidth;

            for (int I = 0; I < 2; ++I)
            {
                int Y = DrawY + 265 + I * 25;
                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                    && MouseHelper.MouseStateCurrent.Y >= Y && MouseHelper.MouseStateCurrent.Y <= Y + 25)
                {
                    SelectedMenuIndex = I;

                    if (MouseHelper.InputLeftButtonPressed())
                    {
                        if (I == 0)
                        {
                            Owner.PushScreen(new CharacterSelectionScreen(Symbols, (Player)Room.ListRoomPlayer[SelectedPlayerIndex], false));
                            RemoveFromPanelList(this);
                        }
                        else if (I == 1)
                        {
                            Owner.PushScreen(new ChooseBookScreen(Symbols, Icons, (Player)Room.ListRoomPlayer[SelectedPlayerIndex]));
                            RemoveFromPanelList(this);
                        }
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
            int NumberOfItems = 3;
            int Y = DrawY + 265;
            g.Draw(GameScreen.sprPixel, new Rectangle(X + 15, Y, BoxWidth - 30, NumberOfItems * 25 + 10), Lobby.BackgroundColor);
            GameScreen.DrawEmptyBox(g, new Vector2(X + 15, Y), BoxWidth - 30, NumberOfItems * 25 + 10, 10, HoverProgression);
            Y += 5;
            g.Draw(GameScreen.sprPixel, new Rectangle(X + 20, Y + SelectedMenuIndex * 25, BoxWidth - 40, 25), Color.FromNonPremultiplied(255, 255, 255, 127));

            Y += 13;
            g.DrawStringCentered(fntText, "Select Character", new Vector2(X + BoxWidth / 2, Y), Color.White);
            Y += 25;
            g.DrawStringCentered(fntText, "Edit Book", new Vector2(X + BoxWidth / 2, Y), Color.White);
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
