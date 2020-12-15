using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    //https://pastebin.com/bhtdxaba
    public class ResultMenu : GameScreen
    {
        protected enum SpiritSelectionTypes { None, Auto, TargetAlly, TargetEnemy };

        #region Ressources

        private SpriteFont fntFinlanderFont;

        #endregion

        #region Variables


        #endregion

        public ResultMenu()
        {
        }

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int Y = 115;
            DrawBox(g, new Vector2(30, Y), 180, 45, Color.Black);
            g.DrawString(fntFinlanderFont, "RESULTS", new Vector2(40, Y+= 8), Color.White);

            Y += 37;
            DrawBox(g, new Vector2(30, Y), 580, 230, Color.Green);
            g.DrawString(fntFinlanderFont, "Pilot Name", new Vector2(190, Y += 3), Color.White);
            g.DrawString(fntFinlanderFont, "Unit Name", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, "Level", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, "EXP", new Vector2(190, Y += 25), Color.White);

            DrawBox(g, new Vector2(30, Y += 32), 580, 45, Color.Black);

            Y += 50;

            for (int i = 0; i < 4; ++i)
            {
                if (i < 2)
                {
                    g.DrawString(fntFinlanderFont, "--------------", new Vector2(40, Y + i * 30), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "--------------", new Vector2(340, Y + (i - 2) * 30), Color.White);
                }
            }
        }
    }
}
