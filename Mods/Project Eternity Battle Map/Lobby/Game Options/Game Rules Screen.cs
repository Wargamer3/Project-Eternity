using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsGameRulesScreen : GameScreen
    {
        private SpriteFont fntText;

        private EmptyNumericUpDown MaxBotNumberTextbox;
        private EmptyNumericUpDown MaxSquadPerBotNumberTextbox;
        private EmptyNumericUpDown GoalScoreTextbox;
        private EmptyNumericUpDown TimeLimitTextbox;
        private EmptyNumericUpDown TurnLimitTextbox;
        private EmptyNumericUpDown MinPlayerTextbox;
        private EmptyNumericUpDown MaxPlayerTextbox;
        private EmptyNumericUpDown MaxSquadPerPlayerTextbox;

        private List<IUIElement> ListMenuButton;

        private readonly RoomInformations Room;

        int PanelX;
        int PanelY;
        int PanelWidth;
        int PanelHeight;
        int RightColumnX;

        public GameOptionsGameRulesScreen(RoomInformations Room)
        {
            this.Room = Room;

            PanelX = (int)(Constants.Width * 0.03);
            PanelY = (int)(Constants.Height * 0.15);
            PanelWidth = (int)(Constants.Width * 0.94);
            PanelHeight = (int)(Constants.Height * 0.22);
            RightColumnX = (int)(PanelWidth / 2);
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            int DrawY = PanelY;
            DrawY += 40;

            MaxBotNumberTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20), OnMaxBotsChanged);
            MaxSquadPerBotNumberTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20), OnMaxSquadPerBotChanged);
            MaxBotNumberTextbox.SetText(Room.MaxNumberOfBots.ToString());
            MaxSquadPerBotNumberTextbox.SetText(Room.MaxSquadsPerBot.ToString());

            DrawY += 105;
            DrawY += 40;

            MinPlayerTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            MaxPlayerTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            MaxSquadPerPlayerTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            GoalScoreTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            TimeLimitTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            TurnLimitTextbox = new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));

            ListMenuButton = new List<IUIElement>()
            {
                MaxBotNumberTextbox, MaxSquadPerBotNumberTextbox, MinPlayerTextbox, MaxPlayerTextbox, MaxSquadPerPlayerTextbox,// GoalScoreTextbox, TimeLimitTextbox, TurnLimitTextbox,
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ListMenuButton)
            {
                ActiveButton.Update(gameTime);
            }
        }

        public void UpdateGameParameters()
        {
            ListMenuButton.Clear();
            ListMenuButton.Add(MaxBotNumberTextbox);
            ListMenuButton.Add(MaxSquadPerBotNumberTextbox);
            ListMenuButton.Add(MinPlayerTextbox);
            ListMenuButton.Add(MaxPlayerTextbox);
            ListMenuButton.Add(MaxSquadPerPlayerTextbox);

            int DrawY = PanelY + 255;

            Dictionary<string, List<GameModeInfo.GameModeParameter>> DicGameModeParametersByCategory = Room.GameInfo.GetDescriptionFromEnumValue();
            foreach (KeyValuePair<string, List<GameModeInfo.GameModeParameter>> ActiveCategory in DicGameModeParametersByCategory)
            {
                int DrawX = RightColumnX - 145;
                bool IsFirstColumn = true;
                foreach (GameModeInfo.GameModeParameter ActiveParameter in ActiveCategory.Value)
                {
                    if (!ActiveParameter.IsVisible)
                    {
                        continue;
                    }

                    Type ObjectType = ActiveParameter.Value.PropertyType;
                    if (typeof(int) == ObjectType)
                    {
                        ListMenuButton.Add(new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(DrawX, DrawY), new Vector2(140, 20), (Sender, InputMessage) => { UpdateValue(Sender, InputMessage, ActiveParameter); }, ActiveParameter.Value.GetValue(ActiveParameter.Owner).ToString()));
                    }
                    else if (typeof(bool) == ObjectType)
                    {
                        ListMenuButton.Add(new EmptyNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(DrawX, DrawY), new Vector2(140, 20), (Sender, InputMessage) => { UpdateValue(Sender, InputMessage, ActiveParameter); }, ActiveParameter.Value.GetValue(ActiveParameter.Owner).ToString()));
                    }

                    if (IsFirstColumn)
                    {
                        ListMenuButton.Add(new UIText(Content, ActiveParameter.Name, new Vector2(PanelX + 10, DrawY), 200));
                        DrawX = PanelWidth - 145;
                    }
                    else
                    {
                        ListMenuButton.Add(new UIText(Content, ActiveParameter.Name, new Vector2(RightColumnX + 10, DrawY), 200));
                        DrawX = RightColumnX - 145;
                    }

                    IsFirstColumn = !IsFirstColumn;
                }
            }
        }

        private void OnMaxBotsChanged(TextInput SenderInput, string InputMessage)
        {
            Room.MaxNumberOfBots = int.Parse(InputMessage);

            Room.GameInfo.OnBotChanged(Room);
        }

        private void OnMaxSquadPerBotChanged(TextInput SenderInput, string InputMessage)
        {
            Room.MaxSquadsPerBot = int.Parse(InputMessage);

            foreach (BattleMapPlayer ActivePlayer in Room.ListRoomBot)
            {
                ActivePlayer.FillLoadout(Room.MaxSquadsPerBot);
            }
        }

        private void UpdateValue(TextInput SenderInput, string InputMessage, GameModeInfo.GameModeParameter Sender)
        {
            Type ObjectType = Sender.Value.PropertyType;
            if (typeof(int) == ObjectType)
            {
                Sender.Value.SetValue(Sender.Owner, int.Parse(InputMessage));
                SenderInput.SetText(Sender.Value.GetValue(Sender.Owner).ToString());
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int DrawX = PanelX;
            int DrawY = PanelY;
            Color NewBackgroundColor = Color.FromNonPremultiplied((int)(Lobby.BackgroundColor.R * 0.8f), (int)(Lobby.BackgroundColor.G * 0.8f), (int)(Lobby.BackgroundColor.B * 0.8f), 150);

            DrawEmptyBox(g, new Vector2(DrawX, DrawY), PanelWidth, 30);
            g.Draw(GameScreen.sprPixel, new Rectangle(DrawX, DrawY, PanelWidth, 30), NewBackgroundColor);
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), PanelWidth, PanelHeight);
            g.Draw(GameScreen.sprPixel, new Rectangle(DrawX, DrawY, PanelWidth, PanelHeight - 100), NewBackgroundColor);

            g.DrawString(fntText, "Bots", new Vector2(DrawX + 10, DrawY + 8), Color.White);

            DrawY += 40;
            g.DrawString(fntText, "Number of Bots", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Max Squads Per Bots", new Vector2(RightColumnX + 10, DrawY), Color.White);

            DrawY += 105;
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), PanelWidth, 30);
            g.Draw(GameScreen.sprPixel, new Rectangle(DrawX, DrawY, PanelWidth, 30), NewBackgroundColor);
            DrawEmptyBox(g, new Vector2(DrawX, DrawY), PanelWidth, PanelHeight + 30);
            g.Draw(GameScreen.sprPixel, new Rectangle(DrawX, DrawY, PanelWidth, PanelHeight), NewBackgroundColor);

            g.DrawString(fntText, "Game", new Vector2(DrawX + 10, DrawY + 8), Color.White);

            DrawY += 40;
            g.DrawString(fntText, "Min Players", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Max Players", new Vector2(RightColumnX + 10, DrawY), Color.White);
            DrawY += 30;
            g.DrawString(fntText, "Max Squad Per Player", new Vector2(DrawX + 10, DrawY), Color.White);

            foreach (IUIElement ActiveButton in ListMenuButton)
            {
                ActiveButton.Draw(g);
            }
        }

        public override string ToString()
        {
            return "Game Rules";
        }
    }
}
