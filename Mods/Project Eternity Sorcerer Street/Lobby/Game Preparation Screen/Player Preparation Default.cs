using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    class PlayerPreparationDefaultActionPanel : ActionPanel
    {
        private SpriteFont fntText;

        private Texture2D sprArrowDown;
        private Texture2D sprIconHuman;
        private Texture2D sprIconBot;

        protected readonly RoomInformations Room;
        private readonly int BoxWidth;
        private readonly int BoxHeight;
        private int DrawY = 120;
        private int SelectedPlayerIndex = -1;

        public PlayerPreparationDefaultActionPanel(ActionPanelHolder ListActionMenuChoice, SpriteFont fntText, RoomInformations Room)
            : base("Player Preparation Default", ListActionMenuChoice, false)
        {
            this.fntText = fntText;
            this.Room = Room;

            sprArrowDown = SorcererStreetGamePreparationScreen.sprArrowDown;
            sprIconHuman = SorcererStreetGamePreparationScreen.sprIconHuman;
            sprIconBot = SorcererStreetGamePreparationScreen.sprIconBot;

            BoxWidth = SorcererStreetGamePreparationScreen.BoxWidth;
            BoxHeight = SorcererStreetGamePreparationScreen.BoxHeight;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            SelectedPlayerIndex = -1;
            for (int P = 0; P < Math.Max(Room.ListRoomPlayer.Count, Room.MaxNumberOfPlayer); P++)
            {
                int X = 15 + P * BoxWidth;

                if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                    && MouseHelper.MouseStateCurrent.Y >= DrawY + 215 && MouseHelper.MouseStateCurrent.Y <= DrawY + 240)
                {
                    SelectedPlayerIndex = P;

                    if (MouseHelper.InputLeftButtonPressed() && Room.ListRoomBot.Count > 0)
                    {
                        ListActionMenuChoice.AddToPanelListAndSelect(new GamePreparationChangePlayerTypePopup(ListActionMenuChoice, fntText, Room, SelectedPlayerIndex));
                    }
                }
                else if (MouseHelper.MouseStateCurrent.X >= X && MouseHelper.MouseStateCurrent.X <= X + BoxWidth - 20
                    && MouseHelper.MouseStateCurrent.Y >= DrawY + 260 && MouseHelper.MouseStateCurrent.Y <= DrawY + 385)
                {
                    SelectedPlayerIndex = P;

                    if (MouseHelper.InputLeftButtonPressed())
                    {//Configure player
                    }
                }
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
            if (SelectedPlayerIndex >= 0 && Room.ListRoomBot.Count > 0)
            {
                if (MouseHelper.MouseStateCurrent.Y >= DrawY + 260)
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle(15 + SelectedPlayerIndex * BoxWidth, DrawY + 260, BoxWidth - 20, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                else
                {
                    g.Draw(GameScreen.sprPixel, new Rectangle(15 + SelectedPlayerIndex * BoxWidth, DrawY + 215, BoxWidth - 20, 25), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
            }
        }

        protected override ActionPanel Copy()
        {
            return new PlayerPreparationDefaultActionPanel(ListActionMenuChoice, fntText, Room);
        }

        protected override void OnCancelPanel()
        {
        }
    }
}
