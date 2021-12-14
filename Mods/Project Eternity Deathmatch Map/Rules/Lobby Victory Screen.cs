using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Parts;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class LobbyVictoryScreen : GameScreen
    {
        public class PlayerGains
        {
            public int Exp;
            public int Money;
            public List<UnitPart> ListEquipment;
            public List<object> ListConsumable;

            public PlayerGains()
            {
                Exp = 0;
                Money = 0;
                ListEquipment = new List<UnitPart>();
                ListConsumable = new List<object>();
            }
        }

        private SpriteFont fntArial12;

        DeathmatchMap Owner;
        private List<PlayerGains> ListPlayerGains;

        public LobbyVictoryScreen(DeathmatchMap Owner, List<PlayerGains> ListPlayerGains)
        {
            this.Owner = Owner;
            this.ListPlayerGains = ListPlayerGains;
        }

        public override void Load()
        {
            fntArial12 = Content.Load<SpriteFont>("Fonts/Arial12");
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(CustomSpriteBatch g)
        {
            int ScreenWidth = (int)(Constants.Width * 0.9);
            int ScreenHeight = (int)(Constants.Height * 0.8);
            float StartX = (Constants.Width - ScreenWidth) / 2;
            float StartY = (Constants.Height - ScreenHeight) / 2;
            float DrawX = StartX;
            float DrawY = StartY;

            DrawBox(g, new Vector2(DrawX, DrawY), ScreenWidth, ScreenHeight, Color.White);
            DrawX += 100;
            DrawBox(g, new Vector2(DrawX, DrawY), 100, 30, Color.Black);
            DrawBox(g, new Vector2(DrawX, DrawY + 30), 100, ScreenHeight - 30, Color.Blue);
            g.DrawStringCentered(fntArial12, "Level", new Vector2(DrawX + 50, DrawY + 15), Color.White);

            DrawX += 100;
            DrawBox(g, new Vector2(DrawX, DrawY), 200, 30, Color.Black);
            g.DrawStringCentered(fntArial12, "Name", new Vector2(DrawX + 100, DrawY + 15), Color.White);

            DrawX += 200;
            DrawBox(g, new Vector2(DrawX, DrawY), 100, 30, Color.Black);
            DrawBox(g, new Vector2(DrawX, DrawY + 30), 100, ScreenHeight - 30, Color.Blue);
            g.DrawStringCentered(fntArial12, "Points", new Vector2(DrawX + 50, DrawY + 15), Color.White);

            DrawX += 100;
            DrawBox(g, new Vector2(DrawX, DrawY), 100, 30, Color.Black);
            g.DrawStringCentered(fntArial12, "EXP", new Vector2(DrawX + 50, DrawY + 15), Color.White);

            DrawX += 100;
            DrawBox(g, new Vector2(DrawX, DrawY), (int)StartX + ScreenWidth - (int)DrawX, 30, Color.Black);
            DrawBox(g, new Vector2(DrawX, DrawY + 30), (int)StartX + ScreenWidth - (int)DrawX, ScreenHeight - 30, Color.Blue);
            g.DrawStringCentered(fntArial12, "Money", new Vector2(DrawX + 50, DrawY + 15), Color.White);

            DrawX = (Constants.Width - ScreenWidth) / 2;
            DrawY += 30;

            for (int P = 0; P < Owner.ListAllPlayer.Count; P++)
            {
                Player ActivePlayer = Owner.ListAllPlayer[P];
                if (ActivePlayer.Team < 0 || ActivePlayer.Team >= 10)
                {
                    continue;
                }

                g.DrawStringCentered(fntArial12, (P + 1).ToString(), new Vector2(DrawX + 50, DrawY + 15), Color.White);
                g.DrawStringCentered(fntArial12, "LV." + ActivePlayer.Level, new Vector2(DrawX + 150, DrawY + 15), Color.White);
                g.DrawStringCentered(fntArial12, ActivePlayer.Name, new Vector2(DrawX + 300, DrawY + 15), Color.White);
                g.DrawStringCentered(fntArial12, ListPlayerGains[P].Exp.ToString(), new Vector2(DrawX + 550, DrawY + 15), Color.White);
                g.DrawStringCentered(fntArial12, ListPlayerGains[P].Money.ToString(), new Vector2(DrawX + 650, DrawY + 15), Color.White);

                if (Owner.ListLocalPlayer.Contains(ActivePlayer))
                {

                }
                else
                {

                }

                DrawY += 25;
            }
        }
    }
}
