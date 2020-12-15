using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelTerrainLevelUpCommands : ActionPanelSorcererStreet
    {
        private enum CreatureCommands { LevelLand, CreatureMovement, TerrainChange }

        private readonly Player ActivePlayer;

        private int CursorIndex;

        public ActionPanelTerrainLevelUpCommands(SorcererStreetMap Map, Player ActivePlayer)
            : base("Terrain Commands", Map, false)
        {
            this.ActivePlayer = ActivePlayer;
        }

        public override void OnSelect()
        {
        }

        public override void DoUpdate(GameTime gameTime)
        {
            if (InputHelper.InputUpPressed())
            {
                if (--CursorIndex < 0)
                {
                    CursorIndex = 5;
                }
            }
            if (InputHelper.InputDownPressed())
            {
                if (++CursorIndex > 5)
                {
                    CursorIndex = 0;
                }
            }
        }

        protected override void OnCancelPanel()
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            float X = 20;
            float Y = 20;

            GameScreen.DrawBox(g, new Vector2(X, Y), 100, 80, Color.White);

            g.DrawString(Map.fntArial12, "Level 1", new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, "50", new Vector2(X + 100, Y), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 110, (int)Y, 20, 20), Color.White);

            Y += 20;

            g.DrawString(Map.fntArial12, "Level 2", new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, "100", new Vector2(X + 100, Y), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 110, (int)Y, 20, 20), Color.White);

            Y += 20;

            g.DrawString(Map.fntArial12, "Level 3", new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, "150", new Vector2(X + 100, Y), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 110, (int)Y, 20, 20), Color.White);

            Y += 20;

            g.DrawString(Map.fntArial12, "Level 4", new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, "250", new Vector2(X + 100, Y), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 110, (int)Y, 20, 20), Color.White);

            Y += 20;

            g.DrawString(Map.fntArial12, "Level 5", new Vector2(X, Y), Color.White);
            g.DrawStringRightAligned(Map.fntArial12, "400", new Vector2(X + 100, Y), Color.White);
            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 110, (int)Y, 20, 20), Color.White);

            g.Draw(GameScreen.sprPixel, new Rectangle((int)X + 5, (int)Y + 5 + CursorIndex * 20, 95, 18), Color.FromNonPremultiplied(255, 255, 255, 127));
        }
    }
}
