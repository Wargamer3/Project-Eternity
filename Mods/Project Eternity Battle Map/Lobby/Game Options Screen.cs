using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    class GameOptionsScreen : GameScreen
    {
        private SpriteFont fntText;

        private readonly RoomInformations Room;

        private GameScreen ActiveTab;
        private GameScreen[] ArrayOptionTab;

        public GameOptionsScreen(RoomInformations Room)
        {
            this.Room = Room;
        }

        public override void Load()
        {
            fntText = Content.Load<SpriteFont>("Fonts/Arial10");

            ArrayOptionTab = new GameScreen[5];
            ArrayOptionTab[0] = new GameOptionsGametypeScreen(Room);
            ArrayOptionTab[1] = new GameOptionsSelectMapScreen(Room);
            ArrayOptionTab[2] = new GameOptionsGameRulesScreen(Room);
            ArrayOptionTab[3] = new GameOptionsMutatorsScreen(Room);
            ArrayOptionTab[4] = new GameOptionsBotConfigScreen(Room);

            for (int T = 0; T < ArrayOptionTab.Length; ++T)
            {
                ArrayOptionTab[T].Load();
            }

            ActiveTab = ArrayOptionTab[0];
        }

        public override void Update(GameTime gameTime)
        {
            ActiveTab.Update(gameTime);
        }

        private void OnGametypePressed()
        {

        }

        private void OnSelectMapPressed()
        {

        }

        private void OnGameRulePressed()
        {

        }

        private void OnMutatorPressed()
        {

        }

        private void OnBotsConfigPressed()
        {

        }

        public override void Draw(CustomSpriteBatch g)
        {
            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.White);
            int HeaderHeight = (int)(Constants.Height * 0.05);
            int TabSectionY = HeaderHeight;
            int TabSectionHeight = (int)(Constants.Height * 0.06);
            int TabY = TabSectionY + (int)(Constants.Height * 0.005);
            int TabWidth = (int)(Constants.Height * 0.15);
            int TabHeight = (int)(Constants.Height * 0.05);
            int TabOffset = (int)(Constants.Height * 0.05);
            DrawBox(g, new Vector2(), Constants.Width, Constants.Height, Color.White);
            DrawBox(g, new Vector2(), Constants.Width, HeaderHeight, Color.White);
            DrawBox(g, new Vector2(0, TabSectionY), Constants.Width, TabSectionHeight, Color.White);

            for (int T = 0; T < ArrayOptionTab.Length; ++T)
            {
                float X = 10 + T * (TabWidth + TabOffset);
                DrawBox(g, new Vector2(X, TabY), TabWidth, TabHeight, Color.White);
                g.DrawStringCentered(fntText, ArrayOptionTab[T].ToString(), new Vector2(X + TabWidth / 2, TabY + TabHeight / 2), Color.White);
            }

            int CloseButtonX = (int)(Constants.Width * 0.86);
            int CloseButtonY = (int)(Constants.Height * 0.92);
            int CloseButtonWidth = (int)(Constants.Width * 0.12);
            int CloseButtonHeight = (int)(Constants.Height * 0.06);
            DrawBox(g, new Vector2(CloseButtonX, CloseButtonY), CloseButtonWidth, CloseButtonHeight, Color.White);
            g.DrawStringCentered(fntText, "Close", new Vector2(CloseButtonX + CloseButtonWidth / 2, CloseButtonY + CloseButtonHeight / 2), Color.White);
            ActiveTab.Draw(g);
        }
    }
}
