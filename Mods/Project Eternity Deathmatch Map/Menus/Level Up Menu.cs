using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    //https://pastebin.com/bhtdxaba
    public class LevelUpMenu : GameScreen
    {
        protected enum SpiritSelectionTypes { None, Auto, TargetAlly, TargetEnemy };

        #region Ressources

        private SpriteFont fntFinlanderFont;

        #endregion

        #region Variables

        Character Pilot;
        int OriginalPilotLevel;
        int OriginalPilotSP;
        bool[] ArrayOriginalPilotSkill;
        bool[] ArrayOriginalPilotSpirit;
        int PilotOriginalMEL;
        int PilotOriginalRNG;
        int PilotOriginalSKL;
        int PilotOriginalDEF;
        int PilotOriginalEVA;
        int PilotOriginalHIT;

        #endregion

        public LevelUpMenu(Character Pilot)
        {
            Pilot.EXP = 500034;
            this.Pilot = Pilot;
            OriginalPilotLevel = Pilot.Level;
            OriginalPilotSP = Pilot.MaxSP;

            ArrayOriginalPilotSkill = new bool[Pilot.ArrayPilotSkill.Length];
            for (int i = 0; i < Pilot.ArrayPilotSkill.Length; ++i)
            {
                ArrayOriginalPilotSkill[i] = Pilot.ArrayPilotSkill[i].CurrentLevel == 0;
            }

            ArrayOriginalPilotSpirit = new bool[Pilot.ArrayPilotSpirit.Length];
            for (int i = 0; i < Pilot.ArrayPilotSpirit.Length; ++i)
            {
                ArrayOriginalPilotSpirit[i] = Pilot.ArrayPilotSpirit[i].IsUnlocked;
            }

            PilotOriginalMEL = Pilot.MEL;
            PilotOriginalRNG = Pilot.RNG;
            PilotOriginalSKL = Pilot.SKL;
            PilotOriginalDEF = Pilot.DEF;
            PilotOriginalEVA = Pilot.EVA;
            PilotOriginalHIT = Pilot.HIT;

            LevelUp();
        }

        private void LevelUp()
        {
            //Only 5 levels up allowed.
            if (Pilot.EXP > 2500)
            {
                Pilot.EXP = 2500;
            }

            while (Pilot.EXP >= 500)
            {
                Pilot.LevelUp();
            }
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
            int Y = 15;
            DrawBox(g, new Vector2(30, Y), 180, 45, Color.Black);
            g.DrawString(fntFinlanderFont, "LEVEL UP", new Vector2(60, Y += 8), Color.White);

            Y += 37;
            DrawBox(g, new Vector2(30, Y), 580, 135, Color.Green);
            g.DrawString(fntFinlanderFont, Pilot.Name, new Vector2(190, Y += 3), Color.White);
            g.DrawString(fntFinlanderFont, "Unit Name", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, "Level", new Vector2(190, Y += 25), Color.Yellow);
            if (Pilot.Level != OriginalPilotLevel)
            {
                g.DrawString(fntFinlanderFont, OriginalPilotLevel.ToString(), new Vector2(270, Y), Color.LightGreen);
                g.DrawString(fntFinlanderFont, Pilot.Level.ToString(), new Vector2(320, Y), Color.LightGreen);
            }
            else
            {

                g.DrawString(fntFinlanderFont, OriginalPilotLevel.ToString(), new Vector2(270, Y), Color.Yellow);
            }

            g.DrawString(fntFinlanderFont, "EXP", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, Pilot.EXP.ToString(), new Vector2(250, Y), Color.White);
            g.DrawString(fntFinlanderFont, "NEXT " + Pilot.NextEXP, new Vector2(320, Y), Color.White);
            g.DrawString(fntFinlanderFont, "SP", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, Pilot.MaxSP.ToString(), new Vector2(250, Y), Color.White);
            g.DrawString(fntFinlanderFont, "+" + (Pilot.MaxSP), new Vector2(320, Y), Color.White);

            DrawBox(g, new Vector2(30, Y += 32), 193, 45, Color.Black);
            DrawBox(g, new Vector2(223, Y), 193, 45, Color.Black);
            DrawBox(g, new Vector2(416, Y), 194, 45, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Skills", new Vector2(120, Y + 7), Color.Yellow);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Spirits", new Vector2(313, Y + 7), Color.Yellow);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Stats", new Vector2(506, Y + 7), Color.Yellow);

            DrawBox(g, new Vector2(30, Y += 45), 193, 160, Color.Green);
            DrawBox(g, new Vector2(223, Y), 193, 160, Color.Green);
            DrawBox(g, new Vector2(416, Y), 194, 160, Color.Green);
            Y += 2;

            for (int i = 0; i < Pilot.ArrayPilotSkill.Length; ++i)
            {
                if (Pilot.ArrayPilotSkill[i].CurrentLevel >= 0)
                {
                    if (!ArrayOriginalPilotSkill[i])
                    {
                        g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSkill[i].Name, new Vector2(40, Y + i * 25), Color.LightGreen);
                    }
                    else
                    {
                        g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSkill[i].Name, new Vector2(40, Y + i * 25), Color.White);
                    }
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "---", new Vector2(40, Y + i * 25), Color.White);
                }
            }

            for (int i = 0; i < Pilot.ArrayPilotSpirit.Length; ++i)
            {
                if (Pilot.ArrayPilotSpirit[i].IsUnlocked)
                {
                    if (!ArrayOriginalPilotSpirit[i])
                    {
                        g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSpirit[i].Name, new Vector2(233, Y + i * 25), Color.LightGreen);
                    }
                    else
                    {
                        g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSpirit[i].Name, new Vector2(233, Y + i * 25), Color.White);
                    }
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "---", new Vector2(233, Y + i * 25), Color.White);
                }
            }
            
            DrawStat(g, Y, "MEL", PilotOriginalMEL, Pilot.MEL);
            DrawStat(g, Y + 25, "RNG", PilotOriginalRNG, Pilot.RNG);
            DrawStat(g, Y + 50, "SKL", PilotOriginalSKL, Pilot.SKL);
            DrawStat(g, Y + 75, "DEF", PilotOriginalDEF, Pilot.DEF);
            DrawStat(g, Y + 100, "EVA", PilotOriginalEVA, Pilot.EVA);
            DrawStat(g, Y + 125, "HIT", PilotOriginalHIT, Pilot.HIT);
        }

        private void DrawStat(CustomSpriteBatch g, int Y, string StatName, int OriginalStat, int NewStat)
        {
            g.DrawString(fntFinlanderFont, StatName, new Vector2(426, Y), Color.Yellow);

            if (OriginalStat != NewStat)
            {
                g.DrawStringRightAligned(fntFinlanderFont, NewStat.ToString(), new Vector2(550, Y), Color.LightGreen);
                g.DrawStringRightAligned(fntFinlanderFont, "+" + (NewStat - OriginalStat).ToString(), new Vector2(600, Y), Color.LightGreen);
            }
            else
            {
                g.DrawStringRightAligned(fntFinlanderFont, NewStat.ToString(), new Vector2(550, Y), Color.White);
            }
        }
    }
}
