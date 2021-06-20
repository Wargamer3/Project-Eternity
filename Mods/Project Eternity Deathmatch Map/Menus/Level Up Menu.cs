using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    //https://pastebin.com/bhtdxaba
    public class LevelUpMenu : GameScreen
    {
        protected enum SpiritSelectionTypes { None, Auto, TargetAlly, TargetEnemy };

        #region Ressources

        private SpriteFont fntFinlanderFont;
        private Texture2D sprArrow;

        #endregion

        #region Variables

        public int TotalExpGained;
        public int TotalPPGained;
        public int TotalMoneyGained;

        private bool ShowLevelUp;

        public bool UpdateBattleEventsOnClose;
        private readonly DeathmatchMap Map;
        private readonly Character Pilot;
        private readonly Unit Owner;
        public readonly bool IsHuman;

        private readonly int OriginalPilotLevel;
        private readonly int OriginalPilotSP;
        private readonly bool[] ArrayOriginalPilotSkill;
        private readonly bool[] ArrayOriginalPilotSpirit;
        private readonly int PilotOriginalMEL;
        private readonly int PilotOriginalRNG;
        private readonly int PilotOriginalSKL;
        private readonly int PilotOriginalDEF;
        private readonly int PilotOriginalEVA;
        private readonly int PilotOriginalHIT;

        #endregion

        public LevelUpMenu(DeathmatchMap Map, Character Pilot, Unit Owner, bool IsHuman)
        {
            RequireDrawFocus = true;
            RequireFocus = true;
            TotalExpGained = 500034;

            this.UpdateBattleEventsOnClose = false;
            this.Map = Map;
            this.Pilot = Pilot;
            this.Owner = Owner;
            this.IsHuman = IsHuman;

            OriginalPilotLevel = Pilot.Level;
            OriginalPilotSP = Pilot.MaxSP;

            ArrayOriginalPilotSkill = new bool[Pilot.ArrayPilotSkill.Length];
            for (int i = 0; i < Pilot.ArrayPilotSkill.Length; ++i)
            {
                ArrayOriginalPilotSkill[i] = Pilot.ArrayPilotSkill[i].CurrentLevel <= Pilot.Level;
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
        }

        public void LevelUp()
        {
            //Only 5 levels up allowed.
            if (TotalExpGained > 2500)
            {
                TotalExpGained = 2500;
            }

            Pilot.IncreaseEXP(TotalExpGained);

            while (Pilot.EXP >= Pilot.NextEXP)
            {
                Pilot.LevelUpOnce();
            }
        }

        public override void Load()
        {
            fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");
            sprArrow = Content.Load<Texture2D>("Status Screen/Arrow"); 

            LevelUp();
        }

        public override void Update(GameTime gameTime)
        {
            if (InputHelper.InputConfirmReleased())
            {
                if (Pilot.Level != OriginalPilotLevel && !ShowLevelUp)
                {
                    ShowLevelUp = true;
                }
                else
                {
                    RemoveScreen(this);

                    if (UpdateBattleEventsOnClose)
                    {
                        Map.UpdateMapEvent(BattleMap.EventTypeOnBattle, 1);
                    }
                }
            }
        }

        public override void Draw(CustomSpriteBatch g)
        {
            if (ShowLevelUp)
            {
                DrawLevelUp(g);
            }
            else
            {
                DrawRecap(g);
            }
        }

        public void DrawRecap(CustomSpriteBatch g)
        {
            int Y = 15;
            DrawBox(g, new Vector2(30, Y), 180, 45, Color.Black);
            g.DrawString(fntFinlanderFont, "RESULTS", new Vector2(60, Y += 8), Color.Yellow);

            Y += 37;
            DrawBox(g, new Vector2(30, Y), 580, 110, Color.Green);
            g.Draw(Pilot.sprPortrait, new Rectangle(70, Y + 15, 80, 80), Color.White);
            g.DrawString(fntFinlanderFont, Pilot.Name, new Vector2(190, Y += 3), Color.White);
            g.Draw(Owner.SpriteMap, new Rectangle(190, Y + 25, 32, 32), Color.White);
            g.DrawString(fntFinlanderFont, Owner.FullName, new Vector2(225, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, "Level", new Vector2(190, Y += 25), Color.Yellow);

            if (Pilot.Level != OriginalPilotLevel)
            {
                g.DrawString(fntFinlanderFont, Pilot.Level.ToString(), new Vector2(270, Y), Color.LightGreen);
                g.DrawString(fntFinlanderFont, "Level Up", new Vector2(300, Y), Color.Yellow);
            }
            else
            {

                g.DrawString(fntFinlanderFont, OriginalPilotLevel.ToString(), new Vector2(270, Y), Color.Yellow);
            }

            g.DrawString(fntFinlanderFont, "EXP", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, TotalExpGained.ToString(), new Vector2(250, Y), Color.White);
            g.DrawString(fntFinlanderFont, "PP", new Vector2(320, Y), Color.White);
            g.DrawString(fntFinlanderFont, TotalPPGained.ToString(), new Vector2(360, Y), Color.White);
            g.DrawString(fntFinlanderFont, "Money", new Vector2(420, Y), Color.White);
            g.DrawString(fntFinlanderFont, TotalMoneyGained.ToString(), new Vector2(500, Y), Color.White);

            DrawBox(g, new Vector2(30, Y += 32), 580, 45, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "GAINED PARTS", new Vector2(Constants.Width / 2, Y += 8), Color.Yellow);
            DrawBox(g, new Vector2(30, Y += 36), 580, 90, Color.Green);
            for (int i = 0; i < 4; ++i)
            {
                g.DrawString(fntFinlanderFont, "---------------", new Vector2(50 + (300 * (i / 2)), Y + 10 + (40 * (i % 2))), Color.White);
            }
        }

        public void DrawLevelUp(CustomSpriteBatch g)
        {
            int Y = 15;
            DrawBox(g, new Vector2(30, Y), 180, 45, Color.Black);
            g.DrawString(fntFinlanderFont, "LEVEL UP", new Vector2(60, Y += 8), Color.Yellow);

            Y += 37;
            DrawBox(g, new Vector2(30, Y), 580, 135, Color.Green);
            g.Draw(sprPixel, new Rectangle(70, Y + 30, 80, 80), Color.White);
            DrawRectangle(g, new Vector2(70, Y + 30), new Vector2(70 + 80, Y + 30 + 80), Color.Black);
            g.Draw(Pilot.sprPortrait, new Rectangle(70, Y + 30, 80, 80), Color.White);
            g.DrawString(fntFinlanderFont, Pilot.Name, new Vector2(190, Y += 3), Color.White);
            g.Draw(Owner.SpriteMap, new Rectangle(190, Y + 25, 32, 32), Color.White);
            g.DrawString(fntFinlanderFont, Owner.FullName, new Vector2(225, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, "Level", new Vector2(190, Y += 25), Color.Yellow);

            if (Pilot.Level != OriginalPilotLevel)
            {
                g.DrawString(fntFinlanderFont, OriginalPilotLevel.ToString(), new Vector2(270, Y), Color.LightGreen);
                g.Draw(sprArrow, new Vector2(300, Y + 10), Color.White);
                g.DrawString(fntFinlanderFont, Pilot.Level.ToString(), new Vector2(320, Y), Color.LightGreen);
            }
            else
            {

                g.DrawString(fntFinlanderFont, OriginalPilotLevel.ToString(), new Vector2(270, Y), Color.Yellow);
            }

            g.DrawString(fntFinlanderFont, "EXP", new Vector2(190, Y += 25), Color.White);
            g.DrawString(fntFinlanderFont, Pilot.EXP.ToString(), new Vector2(250, Y), Color.White);
            g.DrawString(fntFinlanderFont, "NEXT " + (Pilot.NextEXP - Pilot.EXP), new Vector2(320, Y), Color.White);
            g.DrawString(fntFinlanderFont, "SP", new Vector2(190, Y += 25), Color.White);
            if (Pilot.MaxSP != OriginalPilotSP)
            {
                g.DrawString(fntFinlanderFont, Pilot.MaxSP.ToString(), new Vector2(250, Y), Color.LightGreen);
                g.DrawString(fntFinlanderFont, "+" + (Pilot.MaxSP - OriginalPilotSP), new Vector2(320, Y), Color.LightGreen);
            }
            else
            {
                g.DrawString(fntFinlanderFont, Pilot.MaxSP.ToString(), new Vector2(250, Y), Color.White);
            }

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
