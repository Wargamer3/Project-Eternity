using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsGameRulesScreen : GameScreen
    {
        private SpriteFont fntText;

        private BoxNumericUpDown MaxBotNumberTextbox;
        private BoxNumericUpDown MaxSquadPerBotNumberTextbox;
        private BoxNumericUpDown GoalScoreTextbox;
        private BoxNumericUpDown TimeLimitTextbox;
        private BoxNumericUpDown TurnLimitTextbox;
        private BoxNumericUpDown MinPlayerTextbox;
        private BoxNumericUpDown MaxPlayerTextbox;
        private BoxNumericUpDown MaxSquadPerPlayerTextbox;

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

            MaxBotNumberTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20), OnMaxBotsChanged);
            MaxSquadPerBotNumberTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20), OnMaxSquadPerBotChanged);
            MaxBotNumberTextbox.SetText(Room.MaxNumberOfBots.ToString());
            MaxSquadPerBotNumberTextbox.SetText(Room.MaxSquadsPerBot.ToString());

            DrawY += 105;
            DrawY += 40;

            MinPlayerTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            MaxPlayerTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            MaxSquadPerPlayerTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            GoalScoreTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            TimeLimitTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            TurnLimitTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));

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
                    Type ObjectType = ActiveParameter.Value.GetType();
                    if (typeof(int) == ObjectType)
                    {
                        ListMenuButton.Add(new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(DrawX, DrawY), new Vector2(140, 20), (InputMessage) => { UpdateValue(InputMessage, ActiveParameter); }));
                    }
                    else if (typeof(bool) == ObjectType)
                    {
                        ListMenuButton.Add(new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(DrawX, DrawY), new Vector2(140, 20), (InputMessage) => { UpdateValue(InputMessage, ActiveParameter); }));
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
                }
            }
        }

        private void OnMaxBotsChanged(string InputMessage)
        {
            Room.MaxNumberOfBots = int.Parse(InputMessage);

            while (Room.ListRoomBot.Count < Room.MaxNumberOfBots)
            {
                BattleMapPlayer NewPlayer = new BattleMapPlayer(PlayerManager.OnlinePlayerID, "Bot", OnlinePlayerBase.PlayerTypes.Player, false, 0, false, Color.Blue);
                NewPlayer.InitFirstTimeInventory();
                NewPlayer.FillLoadout(Room.MaxSquadsPerBot);
                Room.ListRoomBot.Add(NewPlayer);
            }

            while (Room.ListRoomBot.Count > Room.MaxNumberOfBots)
            {
                Room.ListRoomBot.RemoveAt(Room.ListRoomBot.Count - 1);
            }
        }

        private void OnMaxSquadPerBotChanged(string InputMessage)
        {
            Room.MaxSquadsPerBot = int.Parse(InputMessage);

            foreach (BattleMapPlayer ActivePlayer in Room.ListRoomBot)
            {
                ActivePlayer.FillLoadout(Room.MaxSquadsPerBot);
            }
        }

        private void UpdateValue(string InputMessage, GameModeInfo.GameModeParameter Sender)
        {
            Type ObjectType = Sender.Value.GetType();
            if (typeof(int) == ObjectType)
            {
                Sender.Value = int.Parse(InputMessage);
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int DrawX = PanelX;
            int DrawY = PanelY;

            DrawBox(g, new Vector2(DrawX, DrawY), PanelWidth, PanelHeight, Color.White);
            DrawBox(g, new Vector2(DrawX, DrawY), PanelWidth, 30, Color.White);
            g.DrawString(fntText, "Bots", new Vector2(DrawX + 10, DrawY + 8), Color.White);

            DrawY += 40;
            g.DrawString(fntText, "Number of Bots", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Max Squads Per Bots", new Vector2(RightColumnX + 10, DrawY), Color.White);

            DrawY += 105;
            DrawBox(g, new Vector2(DrawX, DrawY), PanelWidth, PanelHeight + 30, Color.White);
            DrawBox(g, new Vector2(DrawX, DrawY), PanelWidth, 30, Color.White);
            g.DrawString(fntText, "Game", new Vector2(DrawX + 10, DrawY + 8), Color.White);

            DrawY += 40;
            g.DrawString(fntText, "Min Players", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Max Players", new Vector2(RightColumnX + 10, DrawY), Color.White);
            DrawY += 30;
            g.DrawString(fntText, "Max Squad Per Player", new Vector2(DrawX + 10, DrawY), Color.White);
            /*DrawY += 30;
            g.DrawString(fntText, "Goal Score", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Time Limit", new Vector2(RightColumnX + 10, DrawY), Color.White);
            DrawY += 30;
            g.DrawString(fntText, "Turn Limit", new Vector2(RightColumnX + 10, DrawY), Color.White);*/

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
