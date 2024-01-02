using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Parts;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
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
        private RenderTarget2D RenderTargetSkills;
        private RenderTarget2D RenderTargetSpirits;

        #endregion

        #region Variables

        public int TotalExpGained;
        public int TotalPPGained;
        public int TotalMoneyGained;

        private bool ShowLevelUp;

        private bool UpdateBattleEventsOnClose;
        private readonly DeathmatchMap Map;
        private readonly Character Pilot;
        private readonly Unit Owner;
        private readonly Squad OwnerSquad;
        public readonly bool IsHuman;
        private int AttackerSquadPlayerIndex;
        private Squad Attacker;
        private SupportSquadHolder ActiveSquadSupport;
        private int TargetSquadPlayerIndex;
        private Squad TargetSquad;
        private SupportSquadHolder TargetSquadSupport;
        private List<string> ListDropPart;

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

        private bool NeedToScrollSkills;
        private float ScrollTextOffsetSkill;
        private float ScrollTextOffsetMaxSkill;
        private bool NeedToScrollSpirits;
        private float ScrollTextOffsetSpirits;
        private float ScrollTextOffsetMaxSpirits;

        private const int SpiritTextMaxSize = 160;
        private const float ScrollingTextPixelPerSecondSpeed = 10;

        #endregion

        public LevelUpMenu(DeathmatchMap Map, Character Pilot, Unit Owner, Squad OwnerSquad, bool IsHuman)
        {
            RequireDrawFocus = true;
            RequireFocus = true;

            this.UpdateBattleEventsOnClose = false;
            this.Map = Map;
            this.Pilot = Pilot;
            this.Owner = Owner;
            this.OwnerSquad = OwnerSquad;
            this.IsHuman = IsHuman;

            ListDropPart = new List<string>();

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

        public void SetBattleContent(bool UpdateBattleEventsOnClose, int AttackerSquadPlayerIndex, Squad Attacker, SupportSquadHolder ActiveSquadSupport, int TargetSquadPlayerIndex, Squad TargetSquad, SupportSquadHolder TargetSquadSupport)
        {
            this.UpdateBattleEventsOnClose = UpdateBattleEventsOnClose;
            this.AttackerSquadPlayerIndex = AttackerSquadPlayerIndex;
            this.Attacker = Attacker;
            this.ActiveSquadSupport = ActiveSquadSupport;
            this.TargetSquadPlayerIndex = TargetSquadPlayerIndex;
            this.TargetSquad = TargetSquad;
            this.TargetSquadSupport = TargetSquadSupport;

            Squad Enemy;

            if (OwnerSquad == Attacker)
            {
                Enemy = TargetSquad;
            }
            else
            {
                Enemy = Attacker;
            }

            foreach (string PartDropPath in Enemy.ListParthDrop)
            {
                string[] PartByType = PartDropPath.Split('/');
                ListDropPart.Add(PartByType[PartByType.Length - 1]);
                if (SystemList.ListPart.ContainsKey(PartDropPath))
                {
                    SystemList.ListPart[PartDropPath].Quantity++;
                }
                else
                {
                    if (PartByType[0] == "Standard Parts")
                    {
                        SystemList.ListPart.Add(PartDropPath, new UnitStandardPart("Content/Units/" + PartDropPath + ".pep", Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget));
                    }
                    else if (PartByType[0] == "Consumable Parts")
                    {
                        SystemList.ListPart.Add(PartDropPath, new UnitConsumablePart("Content/Units/" + PartDropPath + ".pep", Map.Params.DicRequirement, Map.Params.DicEffect, Map.Params.DicAutomaticSkillTarget, Map.Params.DicManualSkillTarget));
                    }
                }
            }
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
            sprArrow = Content.Load<Texture2D>("Menus/Status Screen/Arrow");

            ScrollTextOffsetMaxSkill = GetMaxOffsetSkills(SpiritTextMaxSize);
            ScrollTextOffsetSkill = 0;
            if (ScrollTextOffsetMaxSkill > 0)
            {
                NeedToScrollSkills = true;
            }

            ScrollTextOffsetMaxSpirits = GetMaxOffsetSpirits(SpiritTextMaxSize);
            ScrollTextOffsetSpirits = 0;
            if (ScrollTextOffsetMaxSpirits > 0)
            {
                NeedToScrollSpirits = true;
            }

            LevelUp();
        }

        public override void Update(GameTime gameTime)
        {
            if (ShowLevelUp)
            {
                if (NeedToScrollSkills)
                {
                    ScrollTextOffsetSkill += (float)(gameTime.ElapsedGameTime.TotalSeconds) * ScrollingTextPixelPerSecondSpeed;

                    if (ScrollTextOffsetSkill > ScrollTextOffsetMaxSkill + 20)
                    {
                        ScrollTextOffsetSkill = -10;
                    }
                }

                if (NeedToScrollSpirits)
                {
                    ScrollTextOffsetSpirits += (float)(gameTime.ElapsedGameTime.TotalSeconds) * ScrollingTextPixelPerSecondSpeed;

                    if (ScrollTextOffsetSkill > ScrollTextOffsetMaxSkill + 20)
                    {
                        ScrollTextOffsetSkill = -10;
                    }
                }
            }

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
                        Map.Params.GlobalContext.SetContext(Attacker, Attacker.CurrentLeader, Attacker.CurrentLeader.Pilot, TargetSquad, TargetSquad.CurrentLeader, TargetSquad.CurrentLeader.Pilot, Map.Params.ActiveParser);

                        Map.UpdateMapEvent(BattleMap.EventTypeOnBattle, 1);

                        Map.Params.GlobalContext.SetContext(null, null, null, null, null, null, Map.Params.ActiveParser);
                    }

                    //Don't update the leader until after the events are processed. (If a battle map event try to read the leader of a dead unit it will crash on a null pointer as dead units have no leader)
                    if (Attacker != null)
                    {
                        Attacker.UpdateSquad();
                        if (ActiveSquadSupport != null && ActiveSquadSupport.ActiveSquadSupport != null)
                            ActiveSquadSupport.ActiveSquadSupport.UpdateSquad();
                        TargetSquad.UpdateSquad();
                        if (TargetSquadSupport != null && TargetSquadSupport.ActiveSquadSupport != null)
                            TargetSquadSupport.ActiveSquadSupport.UpdateSquad();

                        if (Attacker.IsDead)
                        {
                            Map.GameRule.OnSquadDefeated(TargetSquadPlayerIndex, TargetSquad, AttackerSquadPlayerIndex, Attacker);
                        }
                        if (TargetSquad.IsDead)
                        {
                            Map.GameRule.OnSquadDefeated(AttackerSquadPlayerIndex, Attacker, TargetSquadPlayerIndex, TargetSquad);
                        }
                    }
                }
            }
        }

        private float GetMaxOffsetSkills(int MaxTextSize)
        {
            float Offset = 0;

            for (int S = 0; S < OwnerSquad.CurrentLeader.Pilot.ArrayPilotSkill.Length; S++)
            {
                float TextLength = fntFinlanderFont.MeasureString(OwnerSquad.CurrentLeader.Pilot.ArrayPilotSkill[S].Name).X - MaxTextSize;
                if (TextLength > Offset)
                {
                    Offset = TextLength;
                }
            }

            return Offset;
        }

        private float GetMaxOffsetSpirits(int MaxTextSize)
        {
            float Offset = 0;

            for (int S = 0; S < 6; S++)
            {
                if (S < OwnerSquad.CurrentLeader.Pilot.ArrayPilotSpirit.Length && OwnerSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].IsUnlocked)
                {
                    float TextLength = fntFinlanderFont.MeasureString(OwnerSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].Name).X - MaxTextSize;
                    if (TextLength > Offset)
                    {
                        Offset = TextLength;
                    }
                }
            }

            return Offset;
        }

        public override void BeginDraw(CustomSpriteBatch g)
        {
            if (NeedToScrollSkills)
            {
                if (RenderTargetSkills == null)
                {
                    RenderTargetSkills = new RenderTarget2D(
                        g.GraphicsDevice,
                        SpiritTextMaxSize,
                        180);
                }

                g.GraphicsDevice.SetRenderTarget(RenderTargetSkills);
                g.GraphicsDevice.Clear(Color.Transparent);
                g.Begin();
                DrawSkills(g, 0, 0, true);
                g.End();
            }

            if (NeedToScrollSpirits)
            {
                if (RenderTargetSpirits == null)
                {
                    RenderTargetSpirits = new RenderTarget2D(
                        g.GraphicsDevice,
                        SpiritTextMaxSize,
                        180);
                }

                g.GraphicsDevice.SetRenderTarget(RenderTargetSkills);
                g.GraphicsDevice.Clear(Color.Transparent);
                g.Begin();
                DrawSpirits(g, ScrollTextOffsetSpirits, 0, true);
                g.End();
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
            g.DrawString(fntFinlanderFont, Owner.UnitStat.Name, new Vector2(225, Y += 25), Color.White);
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
            g.DrawStringMiddleAligned(fntFinlanderFont, "GAINED PARTS", new Vector2(320, Y += 8), Color.Yellow);
            DrawBox(g, new Vector2(30, Y += 36), 580, 90, Color.Green);
            for (int i = 0; i < 4; ++i)
            {
                if (i < ListDropPart.Count)
                {
                    g.DrawString(fntFinlanderFont, ListDropPart[i], new Vector2(50 + (300 * (i / 2)), Y + 10 + (40 * (i % 2))), Color.White);
                }
                else
                {
                    g.DrawString(fntFinlanderFont, "---------------", new Vector2(50 + (300 * (i / 2)), Y + 10 + (40 * (i % 2))), Color.White);
                }
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
            g.DrawString(fntFinlanderFont, Owner.UnitStat.Name, new Vector2(225, Y += 25), Color.White);
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
            g.DrawString(fntFinlanderFont, "GAINED " + TotalExpGained, new Vector2(450, Y), Color.White);
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

            DrawSkills(g, 40, Y, false);
            DrawSpirits(g, 233, Y, false);

            DrawStat(g, Y, "MEL", PilotOriginalMEL, Pilot.MEL);
            DrawStat(g, Y + 25, "RNG", PilotOriginalRNG, Pilot.RNG);
            DrawStat(g, Y + 50, "SKL", PilotOriginalSKL, Pilot.SKL);
            DrawStat(g, Y + 75, "DEF", PilotOriginalDEF, Pilot.DEF);
            DrawStat(g, Y + 100, "EVA", PilotOriginalEVA, Pilot.EVA);
            DrawStat(g, Y + 125, "HIT", PilotOriginalHIT, Pilot.HIT);
        }

        private void DrawSkills(CustomSpriteBatch g, float X, float Y, bool IsBeginDraw)
        {
            if (NeedToScrollSkills && !IsBeginDraw)
            {
                g.Draw(RenderTargetSkills, new Vector2(X, Y), Color.White);
            }
            else
            {
                for (int S = 0; S < Pilot.ArrayPilotSkill.Length; ++S)
                {
                    if (Pilot.ArrayPilotSkill[S].CurrentLevel >= 0)
                    {
                        Color DrawColor = Color.White;

                        if (!ArrayOriginalPilotSkill[S])
                        {
                            DrawColor = Color.LightGreen;
                        }

                        if (fntFinlanderFont.MeasureString(Pilot.ArrayPilotSkill[S].Name).X > SpiritTextMaxSize)
                        {
                            g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSkill[S].Name,
                                new Vector2(X - Math.Max(0, ScrollTextOffsetSkill), Y + S * 30), Color.White);
                        }
                        else
                        {
                            g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSkill[S].Name, new Vector2(X, Y + S * 25), DrawColor);
                        }
                    }
                    else
                    {
                        g.DrawString(fntFinlanderFont, "---", new Vector2(X, Y + S * 25), Color.White);
                    }
                }
            }
        }

        private void DrawSpirits(CustomSpriteBatch g, float X, float Y, bool IsBeginDraw)
        {
            if (NeedToScrollSpirits && !IsBeginDraw)
            {
                g.Draw(RenderTargetSpirits, new Vector2(X, Y), Color.White);
            }
            else
            {
                for (int i = 0; i < Pilot.ArrayPilotSpirit.Length; ++i)
                {
                    if (Pilot.ArrayPilotSpirit[i].IsUnlocked)
                    {
                        Color DrawColor = Color.White;

                        if (!ArrayOriginalPilotSpirit[i])
                        {
                            DrawColor = Color.LightGreen;
                        }

                        g.DrawString(fntFinlanderFont, Pilot.ArrayPilotSpirit[i].Name, new Vector2(X, Y + i * 25), DrawColor);
                    }
                    else
                    {
                        g.DrawString(fntFinlanderFont, "---", new Vector2(X, Y + i * 25), Color.White);
                    }

                }
            }
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
