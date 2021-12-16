using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsGameRulesScreen : GameScreen
    {
        private SpriteFont fntText;

        private BoxNumericUpDown BotNumberTextbox;
        private BoxNumericUpDown GoalScoreTextbox;
        private BoxNumericUpDown TimeLimitTextbox;
        private BoxNumericUpDown TurnLimitTextbox;
        private BoxNumericUpDown MinPlayerTextbox;
        private BoxNumericUpDown MaxPlayerTextbox;
        private BoxNumericUpDown MaxSquadPerPlayerTextbox;

        private IUIElement[] ArrayMenuButton;

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

            BotNumberTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));

            DrawY += 105;
            DrawY += 40;

            GoalScoreTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            TimeLimitTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            TurnLimitTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            MinPlayerTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));
            MaxPlayerTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(PanelWidth - 145, DrawY - 2), new Vector2(140, 20));
            DrawY += 30;
            MaxSquadPerPlayerTextbox = new BoxNumericUpDown(fntText, sprPixel, sprPixel, new Vector2(RightColumnX - 145, DrawY - 2), new Vector2(140, 20));

            ArrayMenuButton = new IUIElement[]
            {
                BotNumberTextbox, GoalScoreTextbox, TimeLimitTextbox, TurnLimitTextbox, MinPlayerTextbox, MaxPlayerTextbox, MaxSquadPerPlayerTextbox,
            };
        }

        public override void Update(GameTime gameTime)
        {
            foreach (IUIElement ActiveButton in ArrayMenuButton)
            {
                ActiveButton.Update(gameTime);
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

            DrawY += 105;
            DrawBox(g, new Vector2(DrawX, DrawY), PanelWidth, PanelHeight + 30, Color.White);
            DrawBox(g, new Vector2(DrawX, DrawY), PanelWidth, 30, Color.White);
            g.DrawString(fntText, "Game", new Vector2(DrawX + 10, DrawY + 8), Color.White);

            DrawY += 40;
            g.DrawString(fntText, "Goal Score", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Time Limit", new Vector2(RightColumnX + 10, DrawY), Color.White);
            DrawY += 30;
            g.DrawString(fntText, "Turn Limit", new Vector2(RightColumnX + 10, DrawY), Color.White);
            DrawY += 30;
            g.DrawString(fntText, "Min Players", new Vector2(DrawX + 10, DrawY), Color.White);
            g.DrawString(fntText, "Max Players", new Vector2(RightColumnX + 10, DrawY), Color.White);
            DrawY += 30;
            g.DrawString(fntText, "Max Squad Per Player", new Vector2(DrawX + 10, DrawY), Color.White);

            foreach (IUIElement ActiveButton in ArrayMenuButton)
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
