using System;
using System.Collections.Generic;
using FMOD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class StatusMenuScreen : GameScreen
    {
        public enum StatusPannels { Summary, Pilot, Unit, Attacks }

        #region Ressources

        private Texture2D sprLand;
        private Texture2D sprSea;
        private Texture2D sprSky;
        private Texture2D sprSpace;

        private Texture2D sprBarExtraLargeBackground;
        private Texture2D sprBarExtraLargeEN;
        private Texture2D sprBarExtraLargeHP;

        private FMODSound sndConfirm;
        private FMODSound sndSelection;
        private FMODSound sndDeny;
        private FMODSound sndCancel;

        private Texture2D sprBackground;
        private RenderTarget2D RenderTargetSkills;
        private RenderTarget2D RenderTargetSpirits;

        private SpriteFont fntFinlanderFont;

        #endregion

        public Squad ActiveSquad;
        private List<Attack> ListAttack;
        private BattleMap Map;

        private bool IsLoaded;
        public StatusPannels StatusPannel;
        private int SecondaryMenuIndex;
        private int SelectedPilotSkillIndex;
        private int SelectedPilotSpiritIndex;
        private int SelectedUnitAbilityIndex;
        private int AttackCursorIndex;
        private int AttackAttributeIndex;

        private bool NeedToScrollSkills;
        private float ScrollTextOffsetSkill;
        private float ScrollTextOffsetMaxSkill;
        private bool NeedToScrollSpirits;
        private float ScrollTextOffsetSpirits;
        private float ScrollTextOffsetMaxSpirits;

        private const int SpiritTextMaxSize = 160;
        private const float ScrollingTextPixelPerSecondSpeed = 10;

        public StatusMenuScreen(BattleMap Map)
        {
            this.Map = Map;
            if (Map != null)
                ListGameScreen = Map.ListGameScreen;
        }

        public void OpenStatusMenuScreen(Squad ActiveSquad)
        {
            this.ActiveSquad = ActiveSquad;
            ListAttack = ActiveSquad.CurrentLeader.ListAttack;
            StatusPannel = StatusPannels.Summary;
            SecondaryMenuIndex = 0;
            SelectedPilotSkillIndex = -1;
            SelectedPilotSpiritIndex = -1;
            AttackCursorIndex = 0;
            AttackAttributeIndex = -1;
            Alive = true;

            if (IsLoaded)
            {
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
            }

            PushScreen(this);
        }

        public override void Load()
        {
            if (!IsLoaded)
            {
                IsLoaded = true;

                sprLand = Content.Load<Texture2D>("Status Screen/Ground");
                sprSea = Content.Load<Texture2D>("Status Screen/Sea");
                sprSky = Content.Load<Texture2D>("Status Screen/Sky");
                sprSpace = Content.Load<Texture2D>("Status Screen/Space");

                sprBarExtraLargeBackground = Content.Load<Texture2D>("Battle/Bars/Extra Long Bar");
                sprBarExtraLargeEN = Content.Load<Texture2D>("Battle/Bars/Extra Long Energy");
                sprBarExtraLargeHP = Content.Load<Texture2D>("Battle/Bars/Extra Long Health");

                sndConfirm = new FMODSound(FMODSystem, "Content/SFX/Confirm.mp3");
                sndDeny = new FMODSound(FMODSystem, "Content/SFX/Deny.mp3");
                sndSelection = new FMODSound(FMODSystem, "Content/SFX/Selection.mp3");
                sndCancel = new FMODSound(FMODSystem, "Content/SFX/Cancel.mp3");

                sprBackground = Content.Load<Texture2D>("Status Screen/Background Black");

                fntFinlanderFont = Content.Load<SpriteFont>("Fonts/Finlander Font");

                if (ActiveSquad != null)
                {
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
                }
            }
        }

        public override void Update(GameTime gameTime)
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

            if (SecondaryMenuIndex == 0)
            {
                if (InputHelper.InputLeftPressed())
                {
                    --StatusPannel;
                    if (StatusPannel < StatusPannels.Summary)
                        StatusPannel = StatusPannels.Attacks;
                    sndSelection.Play();
                }
                else if (InputHelper.InputRightPressed())
                {
                    ++StatusPannel;
                    if (StatusPannel > StatusPannels.Attacks)
                        StatusPannel = StatusPannels.Summary;
                    sndSelection.Play();
                }
                else if (InputHelper.InputCancelPressed())
                {
                    RemoveWithoutUnloading(this);
                    sndCancel.Play();
                }
            }

            #region Pilot Pannel

            if (StatusPannel == StatusPannels.Pilot)
            {
                if (SecondaryMenuIndex == 0)
                {
                    if (InputHelper.InputDownPressed() || InputHelper.InputConfirmPressed())
                    {
                        if (ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill.Length > 0)
                            SelectedPilotSkillIndex = 0;
                        else if (ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit.Length > 0)
                            SelectedPilotSpiritIndex = 0;
                        else
                            return;

                        SecondaryMenuIndex = 1;

                        sndSelection.Play();
                    }
                }
                else
                {
                    if (InputHelper.InputLeftPressed() || InputHelper.InputRightPressed())
                    {
                        if (SelectedPilotSkillIndex >= 0 && ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit.Length > 0)
                        {
                            SelectedPilotSpiritIndex = SelectedPilotSkillIndex;
                            SelectedPilotSkillIndex = -1;
                            sndSelection.Play();
                        }
                        else if (SelectedPilotSpiritIndex >= 0 && ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill.Length > 0)
                        {
                            SelectedPilotSkillIndex = SelectedPilotSpiritIndex;
                            SelectedPilotSpiritIndex = -1;
                            sndSelection.Play();
                        }
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        if (SelectedPilotSkillIndex >= 0)
                        {
                            ++SelectedPilotSkillIndex;
                            if (SelectedPilotSkillIndex >= ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill.Length)
                                SelectedPilotSkillIndex = 0;
                            sndSelection.Play();
                        }
                        else if (SelectedPilotSpiritIndex >= 0)
                        {
                            ++SelectedPilotSpiritIndex;
                            if (SelectedPilotSpiritIndex >= ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit.Length)
                                SelectedPilotSpiritIndex = 0;
                            sndSelection.Play();
                        }
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        if (SelectedPilotSkillIndex >= 0)
                        {
                            --SelectedPilotSkillIndex;
                            if (SelectedPilotSkillIndex < 0)
                                SecondaryMenuIndex = 0;

                            sndSelection.Play();
                        }
                        else if (SelectedPilotSpiritIndex >= 0)
                        {
                            --SelectedPilotSpiritIndex;
                            if (SelectedPilotSpiritIndex < 0)
                                SecondaryMenuIndex = 0;

                            sndSelection.Play();
                        }
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        SecondaryMenuIndex = 0;
                        sndCancel.Play();
                    }
                }
            }

            #endregion

            #region Unit Pannel

            else if (StatusPannel == StatusPannels.Unit)
            {
                if (SecondaryMenuIndex == 0)
                {
                    if (InputHelper.InputDownPressed() || InputHelper.InputConfirmPressed())
                    {
                        if (ActiveSquad.CurrentLeader.ArrayUnitAbility.Length > 0)
                            SelectedUnitAbilityIndex = 0;
                        else
                            return;

                        SecondaryMenuIndex = 1;
                        sndSelection.Play();
                    }
                }
                else
                {
                    if (InputHelper.InputLeftPressed() || InputHelper.InputRightPressed())
                    {
                        if (SelectedUnitAbilityIndex < 4 && SelectedUnitAbilityIndex + 4 < ActiveSquad.CurrentLeader.ArrayUnitAbility.Length)
                        {
                            SelectedUnitAbilityIndex += 4;
                            sndSelection.Play();
                        }
                        else if (SelectedUnitAbilityIndex >= 4 && SelectedUnitAbilityIndex - 4 >= 0)
                        {
                            SelectedUnitAbilityIndex -= 4;
                            sndSelection.Play();
                        }
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ++SelectedUnitAbilityIndex;
                        if (SelectedUnitAbilityIndex >= ActiveSquad.CurrentLeader.ArrayUnitAbility.Length)
                            SelectedUnitAbilityIndex = 0;

                        sndSelection.Play();
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        --SelectedUnitAbilityIndex;
                        if (SelectedUnitAbilityIndex < 0)
                            SecondaryMenuIndex = 0;

                        sndSelection.Play();
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        SecondaryMenuIndex = 0;
                        sndCancel.Play();
                    }
                }
            }

            #endregion

            #region Attacks Pannel

            else if (StatusPannel == StatusPannels.Attacks)
            {
                if (SecondaryMenuIndex == 0)
                {
                    if (InputHelper.InputUpPressed())
                    {
                        --AttackCursorIndex;
                        if (AttackCursorIndex < 0)
                            AttackCursorIndex = 0;

                        sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ++AttackCursorIndex;
                        if (AttackCursorIndex >= ListAttack.Count)
                            AttackCursorIndex = 0;

                        sndSelection.Play();
                    }
                    else if (InputHelper.InputConfirmPressed())
                    {
                        if (ListAttack[AttackCursorIndex].ArrayAttackAttributes.Length > 0)
                        {
                            SecondaryMenuIndex = 1;
                            AttackAttributeIndex = 0;

                            sndConfirm.Play();
                        }
                    }
                }
                else
                {
                    if (InputHelper.InputLeftPressed() || InputHelper.InputRightPressed())
                    {
                        if (AttackAttributeIndex < 2 && AttackAttributeIndex + 2 < ListAttack[AttackCursorIndex].ArrayAttackAttributes.Length)
                        {
                            AttackAttributeIndex += 2;
                            sndSelection.Play();
                        }
                        else if (AttackAttributeIndex >= 2 && AttackAttributeIndex - 2 >= 0)
                        {
                            AttackAttributeIndex -= 2;
                            sndSelection.Play();
                        }
                    }
                    else if (InputHelper.InputUpPressed())
                    {
                        --AttackAttributeIndex;
                        if (AttackAttributeIndex < 0)
                            AttackAttributeIndex = ListAttack[AttackCursorIndex].ArrayAttackAttributes.Length - 1;

                        sndSelection.Play();
                    }
                    else if (InputHelper.InputDownPressed())
                    {
                        ++AttackAttributeIndex;
                        if (AttackAttributeIndex >= ListAttack[AttackCursorIndex].ArrayAttackAttributes.Length)
                            AttackAttributeIndex = 0;

                        sndSelection.Play();
                    }
                    else if (InputHelper.InputCancelPressed())
                    {
                        SecondaryMenuIndex = 0;
                        AttackAttributeIndex = -1;
                        sndCancel.Play();
                    }
                }
            }

            #endregion
        }

        private float GetMaxOffsetSkills(int MaxTextSize)
        {
            float Offset = 0;

            for (int S = 0; S < ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill.Length; S++)
            {
                float TextLength = fntFinlanderFont.MeasureString(ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill[S].Name).X - MaxTextSize;
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
                if (S < ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit.Length && ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].IsUnlocked)
                {
                    float TextLength = fntFinlanderFont.MeasureString(ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].Name).X - MaxTextSize;
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
            g.Draw(sprBackground, new Rectangle(0, 0, Constants.Width, Constants.Height), Color.White);
            DrawTop(g);

            switch (StatusPannel)
            {
                case StatusPannels.Summary:
                    DrawSummary(g);
                    break;

                case StatusPannels.Pilot:
                    DrawPilot(g);
                    break;

                case StatusPannels.Unit:
                    DrawUnit(g);
                    break;

                case StatusPannels.Attacks:
                    Map.DrawAttackPanel(g, fntFinlanderFont, ActiveSquad.CurrentLeader, ListAttack, AttackCursorIndex);
                    break;
            }
        }

        public void DrawSummary(CustomSpriteBatch g)
        {
            Unit ActiveUnit = ActiveSquad.CurrentLeader;
            g.Draw(ActiveUnit.SpriteMap, new Vector2(20, 50), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.UnitStat.Name, new Vector2(60, 50), Color.White);
            g.Draw(sprPixel, new Rectangle(60, 75, (int)fntFinlanderFont.MeasureString(ActiveUnit.UnitStat.Name).X, 1), Color.FromNonPremultiplied(173, 216, 230, 190));
            g.Draw(ActiveUnit.SpriteUnit, new Vector2(240, 280 - ActiveUnit.SpriteUnit.Height), Color.White);

            int BottomWidth = 504 - 100;
            int BottomPositionY = Constants.Height - 192;
            int BottomHeight = 182;

            int RightWidth = (Constants.Width - 10) / 5;
            int RightPosY = 50;
            int RightPosX = Constants.Width - 131;
            int RightHeight = Constants.Height - RightPosY - 10;

            int MiddlePosX = 5 + BottomWidth;
            int MiddleWidth = RightPosX - MiddlePosX;

            DrawBox(g, new Vector2(5, BottomPositionY), BottomWidth, BottomHeight, Color.White);

            int DistanceBetweenText = 16;
            int MenuOffset = (int)(DistanceBetweenText * 0.5);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);

            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP,
                new Vector2(55, BottomPositionY - MenuOffset + 20 + DistanceBetweenText), ActiveUnit.HP, ActiveUnit.MaxHP);

            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.HP + "/" + ActiveUnit.MaxHP, new Vector2(222, BottomPositionY - MenuOffset + 6 + DistanceBetweenText), Color.White);

            g.DrawString(fntFinlanderFont, "EN", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);

            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN,
                new Vector2(55, BottomPositionY - MenuOffset + 20 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), ActiveUnit.EN, ActiveUnit.MaxEN);

            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.EN + "/" + ActiveUnit.MaxEN, new Vector2(222, BottomPositionY - MenuOffset + 6 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);

            g.DrawString(fntFinlanderFont, "Armor", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.Armor.ToString(), new Vector2(95, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.White);
            g.DrawString(fntFinlanderFont, "Mobility", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.Mobility.ToString(), new Vector2(115, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.White);

            g.DrawString(fntFinlanderFont, "MV", new Vector2(235, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxMovement.ToString(), new Vector2(325, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "Size", new Vector2(235, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.Size, new Vector2(325, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, "Move Type", new Vector2(235, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);

            int CurrentX = 235;

            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
            {
                g.Draw(sprSky, new Vector2(CurrentX, Constants.Height - 60), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
            {
                g.Draw(sprLand, new Vector2(CurrentX, Constants.Height - 60), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
            {
                g.Draw(sprSea, new Vector2(CurrentX, Constants.Height - 60), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
            {
                g.Draw(sprSpace, new Vector2(CurrentX, Constants.Height - 60), Color.White);
                CurrentX += 50;
            }


            int CurrentY = Constants.Height - 174;

            DrawBox(g, new Vector2(MiddlePosX, BottomPositionY), MiddleWidth, BottomHeight, Color.White);

            g.Draw(sprSky, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveSquad.CurrentMovement == UnitStats.TerrainAir)
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainAir) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeAir, new Vector2(MiddlePosX + 34, CurrentY), Color.Green);
            else
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainAir) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeAir, new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;
            g.Draw(sprLand, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveSquad.CurrentMovement == UnitStats.TerrainLand)
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainLand) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeLand, new Vector2(MiddlePosX + 34, CurrentY), Color.Green);
            else
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainLand) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeLand, new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;
            g.Draw(sprSea, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveSquad.CurrentMovement == UnitStats.TerrainSea)
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSea) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeSea, new Vector2(MiddlePosX + 34, CurrentY), Color.Green);
            else
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSea) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeSea, new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;
            g.Draw(sprSpace, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            if (ActiveSquad.CurrentMovement == UnitStats.TerrainSpace)
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSpace) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeSpace, new Vector2(MiddlePosX + 34, CurrentY), Color.Green);
            else
                g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSpace) + " + " + ActiveUnit.Pilot.TerrainGrade.TerrainGradeSpace, new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            DrawBox(g, new Vector2(RightPosX, RightPosY), RightWidth, RightHeight, Color.White);

            g.Draw(sprPixel, new Rectangle(RightPosX + 20, RightPosY + 13, 84, 84), Color.Gray);
            g.Draw(sprPixel, new Rectangle(RightPosX + 22, RightPosY + 15, 80, 80), Color.White);
            g.Draw(ActiveUnit.Pilot.sprPortrait, new Vector2(RightPosX + 22, RightPosY + 15), Color.White);
            TextHelper.DrawTextMultiline(g, fntFinlanderFont, TextHelper.FitToWidth(fntFinlanderFont, ActiveUnit.PilotName, 100), TextHelper.TextAligns.Center, RightPosX + 62, RightPosY + 100, 100);

            CurrentY = 266;

            g.DrawString(fntFinlanderFont, "Lvl", new Vector2(RightPosX + 16, CurrentY), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotLevel.ToString(), new Vector2(RightPosX + 111, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;

            g.DrawString(fntFinlanderFont, "Will", new Vector2(RightPosX + 16, CurrentY), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotMorale.ToString(), new Vector2(RightPosX + 111, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;

            g.DrawString(fntFinlanderFont, "SP", new Vector2(RightPosX + 16, CurrentY), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotSP.ToString(), new Vector2(RightPosX + 111, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;

            g.DrawString(fntFinlanderFont, "PP", new Vector2(RightPosX + 16, CurrentY), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotPilotPoints.ToString(), new Vector2(RightPosX + 111, CurrentY), Color.White);

            CurrentY += DistanceBetweenText + fntFinlanderFont.LineSpacing;

            g.DrawString(fntFinlanderFont, "Next", new Vector2(RightPosX + 16, CurrentY), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.PilotNextEXP.ToString(), new Vector2(RightPosX + 111, CurrentY), Color.White);
        }

        public void DrawPilot(CustomSpriteBatch g)
        {
            Unit ActiveUnit = ActiveSquad.CurrentLeader;
            g.Draw(sprPixel, new Rectangle(20, 56, 84, 84), Color.Gray);
            g.Draw(sprPixel, new Rectangle(22, 58, 80, 80), Color.White);
            g.Draw(ActiveSquad.CurrentLeader.Pilot.sprPortrait, new Vector2(22, 58), Color.White);

            g.DrawString(fntFinlanderFont, ActiveUnit.PilotName, new Vector2(110, 50), Color.White);
            g.Draw(sprPixel, new Rectangle(110, 75, (int)fntFinlanderFont.MeasureString(ActiveUnit.PilotName).X, 1), Color.FromNonPremultiplied(173, 216, 230, 190));

            g.DrawString(fntFinlanderFont, "Lvl", new Vector2(110, 80), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotLevel.ToString(), new Vector2(240, 80), Color.White);

            g.DrawString(fntFinlanderFont, "SP", new Vector2(110, 110), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotSP.ToString(), new Vector2(240, 110), Color.White);

            g.DrawString(fntFinlanderFont, "NEXT", new Vector2(260, 80), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotNextEXP.ToString(), new Vector2(390, 80), Color.White);

            g.DrawString(fntFinlanderFont, "Will", new Vector2(260, 110), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotMorale.ToString(), new Vector2(390, 110), Color.White);

            g.DrawString(fntFinlanderFont, "Kills", new Vector2(410, 80), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotKills.ToString(), new Vector2(540, 80), Color.White);

            g.DrawString(fntFinlanderFont, "PP", new Vector2(410, 110), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotPilotPoints.ToString(), new Vector2(540, 110), Color.White);

            g.Draw(sprPixel, new Rectangle(110, 105, 440, 1), Color.FromNonPremultiplied(173, 216, 230, 190));
            g.Draw(sprPixel, new Rectangle(110, 135, 440, 1), Color.FromNonPremultiplied(173, 216, 230, 190));

            DrawBox(g, new Vector2(20, 160), 180, 40, Color.Gray);
            DrawBox(g, new Vector2(20, 200), 180, 180, Color.White);
            g.DrawString(fntFinlanderFont, "Skills", new Vector2(70, 165), Color.Yellow);
            DrawSkills(g, 30, 200, false);

            DrawBox(g, new Vector2(200, 160), 180, 40, Color.Gray);
            DrawBox(g, new Vector2(200, 200), 180, 180, Color.White);
            g.DrawString(fntFinlanderFont, "Spirits", new Vector2(245, 165), Color.Yellow);
            DrawSpirits(g, 210, 200, false);

            DrawBox(g, new Vector2(380, 160), 240, 40, Color.Gray);
            DrawBox(g, new Vector2(380, 200), 240, 120, Color.White);
            g.DrawString(fntFinlanderFont, "Stats", new Vector2(475, 165), Color.Yellow);
            g.DrawString(fntFinlanderFont, "MEL", new Vector2(390, 200), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotMEL.ToString(), new Vector2(490, 200), Color.White);
            g.DrawString(fntFinlanderFont, "SKL", new Vector2(390, 240), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotSKL.ToString(), new Vector2(490, 240), Color.White);
            g.DrawString(fntFinlanderFont, "EVA", new Vector2(390, 280), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotEVA.ToString(), new Vector2(490, 280), Color.White);
            g.DrawString(fntFinlanderFont, "RNG", new Vector2(500, 200), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotRNG.ToString(), new Vector2(605, 200), Color.White);
            g.DrawString(fntFinlanderFont, "DEF", new Vector2(500, 240), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotDEF.ToString(), new Vector2(605, 240), Color.White);
            g.DrawString(fntFinlanderFont, "HIT", new Vector2(500, 280), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.PilotHIT.ToString(), new Vector2(605, 280), Color.White);

            DrawBox(g, new Vector2(380, 320), 240, 40, Color.Gray);
            DrawBox(g, new Vector2(380, 360), 240, 60, Color.White);
            g.DrawString(fntFinlanderFont, "Terrain", new Vector2(460, 325), Color.Yellow);
            g.Draw(sprSky, new Vector2(390, 380), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.Pilot.TerrainGrade.TerrainGradeAir.ToString(), new Vector2(420, 378), Color.White);
            g.Draw(sprLand, new Vector2(446, 380), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.Pilot.TerrainGrade.TerrainGradeLand.ToString(), new Vector2(476, 378), Color.White);
            g.Draw(sprSea, new Vector2(502, 380), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.Pilot.TerrainGrade.TerrainGradeSea.ToString(), new Vector2(532, 378), Color.White);
            g.Draw(sprSpace, new Vector2(558, 380), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.Pilot.TerrainGrade.TerrainGradeSpace.ToString(), new Vector2(588, 378), Color.White);

            DrawBox(g, new Vector2(20, 380), 360, 40, Color.Gray);
            DrawBox(g, new Vector2(20, 420), 360, 40, Color.White);
            g.DrawString(fntFinlanderFont, "Ace Bonus", new Vector2(30, 385), Color.Yellow);
            g.DrawString(fntFinlanderFont, "------------------", new Vector2(30, 425), Color.White);

            if (SecondaryMenuIndex == 1)
            {
                string Description = "";

                if (SelectedPilotSkillIndex >= 0)
                {
                    BaseAutomaticSkill ActivePilotSkill = ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill[SelectedPilotSkillIndex];
                    Description = ActivePilotSkill.Description;

                    g.Draw(sprPixel, new Rectangle(30, 200 + SelectedPilotSkillIndex * 30, 100, 30), Color.FromNonPremultiplied(255, 255, 255, 127));
                }
                else
                {
                    ManualSkill ActivePilotSpirit = ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[SelectedPilotSpiritIndex];
                    Description = ActivePilotSpirit.Description;
                    g.Draw(sprPixel, new Rectangle(210, 200 + SelectedPilotSpiritIndex * 30, 100, 30), Color.FromNonPremultiplied(255, 255, 255, 127));
                }

                if (string.IsNullOrEmpty(Description))
                    Description = "No description available for this skill";

                float MaxBoxWidth = Constants.Width * 0.75f;

                string Output = FitTextToWidth(fntFinlanderFont, Description, MaxBoxWidth);

                Vector2 BoxPosition = new Vector2((Constants.Width - MaxBoxWidth) / 2, (Constants.Height - (int)fntFinlanderFont.MeasureString(Output).Y - 20) / 2);
                DrawBox(g, new Vector2(BoxPosition.X - 10, BoxPosition.Y - 10), (int)MaxBoxWidth + 20, (int)fntFinlanderFont.MeasureString(Output).Y + 20, Color.White);
                g.DrawString(fntFinlanderFont, Output, BoxPosition, Color.White);
            }
        }

        private void DrawSkills(CustomSpriteBatch g, float X, float Y, bool IsBeginDraw)
        {
            if (NeedToScrollSkills && !IsBeginDraw)
            {
                g.Draw(RenderTargetSkills, new Vector2(X, Y), Color.White);
            }
            else
            {
                for (int S = 0; S < 6; S++)
                {
                    if (S < ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill.Length)
                    {
                        if (fntFinlanderFont.MeasureString(ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill[S].Name).X > SpiritTextMaxSize)
                        {
                            g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill[S].Name,
                                new Vector2(X - Math.Max(0,   ScrollTextOffsetSkill), Y + S * 30), Color.White);
                        }
                        else
                        {
                            g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill[S].Name, new Vector2(X, Y + S * 30), Color.White);
                        }
                    }
                    else
                        g.DrawString(fntFinlanderFont, "---", new Vector2(X, Y + S * 30), Color.White);
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
                for (int S = 0; S < 6; S++)
                {
                    if (S < ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit.Length && ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].IsUnlocked)
                    {
                        if (fntFinlanderFont.MeasureString(ActiveSquad.CurrentLeader.Pilot.ArrayPilotSkill[S].Name).X > SpiritTextMaxSize)
                        {
                            g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].Name,
                                new Vector2(X - Math.Max(0, ScrollTextOffsetSpirits), Y + S * 30), Color.White);
                        }
                        else
                        {
                            g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].Name, new Vector2(X, Y + S * 30), Color.White);
                        }
                        g.DrawStringRightAligned(fntFinlanderFont, ActiveSquad.CurrentLeader.Pilot.ArrayPilotSpirit[S].SPCost.ToString(), new Vector2(X + 160, Y + S * 30), Color.White);
                    }
                    else
                        g.DrawString(fntFinlanderFont, "---", new Vector2(X, Y + S * 30), Color.White);
                }
            }
        }

        public void DrawUnit(CustomSpriteBatch g)
        {
            Unit ActiveUnit = ActiveSquad.CurrentLeader;
            g.Draw(ActiveSquad.CurrentLeader.SpriteMap, new Vector2(20, 50), Color.White);
            g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.UnitStat.Name, new Vector2(60, 50), Color.White);
            g.Draw(sprPixel, new Rectangle(60, 75, (int)fntFinlanderFont.MeasureString(ActiveSquad.CurrentLeader.UnitStat.Name).X, 1), Color.FromNonPremultiplied(173, 216, 230, 190));
            g.Draw(ActiveSquad.CurrentLeader.SpriteUnit, new Vector2(250 - ActiveSquad.CurrentLeader.SpriteUnit.Width, 280 - ActiveSquad.CurrentLeader.SpriteUnit.Height), Color.White);

            DrawBox(g, new Vector2(355, 88), 260, 40, Color.Gray);
            DrawBox(g, new Vector2(355, 128), 260, 160, Color.White);
            g.DrawString(fntFinlanderFont, "Ability", new Vector2(445, 92), Color.Yellow);
            for (int A = 0; A < 4; A++)
            {
                if (A < ActiveSquad.CurrentLeader.ArrayUnitAbility.Length)
                    g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.ArrayUnitAbility[A].Name, new Vector2(370, 130 + A * 40), Color.White);
                else
                    g.DrawString(fntFinlanderFont, "---------", new Vector2(370, 130 + A * 40), Color.White);
            }
            for (int A = 0; A < 4; A++)
            {
                if (A + 4 < ActiveSquad.CurrentLeader.ArrayUnitAbility.Length)
                    g.DrawString(fntFinlanderFont, ActiveSquad.CurrentLeader.ArrayUnitAbility[A + 4].Name, new Vector2(490, 130 + A * 40), Color.White);
                else
                    g.DrawString(fntFinlanderFont, "---------", new Vector2(490, 130 + A * 40), Color.White);
            }

            int BottomWidth = 504;
            int BottomPositionY = 288;
            int BottomHeight = 182;
            DrawBox(g, new Vector2(5, BottomPositionY), BottomWidth, BottomHeight, Color.White);

            int DistanceBetweenText = 16;
            int MenuOffset = (int)(DistanceBetweenText * 0.5);
            g.DrawString(fntFinlanderFont, "HP", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);

            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP,
                new Vector2(55, BottomPositionY - MenuOffset + 20 + DistanceBetweenText), ActiveUnit.HP, ActiveUnit.MaxHP);

            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.HP + "/" + ActiveUnit.MaxHP, new Vector2(222, BottomPositionY - MenuOffset + 6 + DistanceBetweenText), Color.White);

            g.DrawString(fntFinlanderFont, "EN", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);

            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN,
                new Vector2(55, BottomPositionY - MenuOffset + 20 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), ActiveUnit.EN, ActiveUnit.MaxEN);

            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.EN + "/" + ActiveUnit.MaxEN, new Vector2(222, BottomPositionY - MenuOffset + 6 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);

            g.DrawString(fntFinlanderFont, "Armor", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.Armor.ToString(), new Vector2(95, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.White);
            g.DrawString(fntFinlanderFont, "Mobility", new Vector2(15, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.Yellow);
            g.DrawString(fntFinlanderFont, ActiveUnit.Mobility.ToString(), new Vector2(115, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 4 + fntFinlanderFont.LineSpacing * 3), Color.White);

            g.DrawString(fntFinlanderFont, "MV", new Vector2(235, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.MaxMovement.ToString(), new Vector2(325, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.White);
            g.DrawString(fntFinlanderFont, "Size", new Vector2(235, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.Size, new Vector2(325, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 2 + fntFinlanderFont.LineSpacing), Color.White);
            g.DrawString(fntFinlanderFont, "Move Type", new Vector2(235, BottomPositionY - MenuOffset + 10 + DistanceBetweenText * 3 + fntFinlanderFont.LineSpacing * 2), Color.Yellow);

            g.DrawString(fntFinlanderFont, "Cost", new Vector2(355, 306), Color.Yellow);
            g.DrawStringRightAligned(fntFinlanderFont, ActiveUnit.Price + "$", new Vector2(495, BottomPositionY - MenuOffset + 10 + DistanceBetweenText), Color.White);

            int CurrentX = 235;

            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainAir))
            {
                g.Draw(sprSky, new Vector2(CurrentX, 420), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainLand))
            {
                g.Draw(sprLand, new Vector2(CurrentX, 420), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainSea))
            {
                g.Draw(sprSea, new Vector2(CurrentX, 420), Color.White);
                CurrentX += 50;
            }
            if (ActiveUnit.ListTerrainChoices.Contains(UnitStats.TerrainSpace))
            {
                g.Draw(sprSpace, new Vector2(CurrentX, 420), Color.White);
                CurrentX += 50;
            }

            int MiddlePosX = 520;

            int CurrentY = 340;

            DrawBox(g, new Vector2(509, BottomPositionY), 106, 40, Color.Gray);
            DrawBox(g, new Vector2(509, BottomPositionY + 40), 106, BottomHeight - 40, Color.White);
            g.DrawString(fntFinlanderFont, "Terrain", new Vector2(523, 292), Color.Yellow);

            g.Draw(sprSky, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainAir).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprLand, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainLand).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprSea, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSea).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            CurrentY += fntFinlanderFont.LineSpacing + 6;
            g.Draw(sprSpace, new Vector2(MiddlePosX + 10, CurrentY + 2), Color.White);
            g.DrawString(fntFinlanderFont, ActiveUnit.TerrainLetterAttribute(UnitStats.TerrainSpace).ToString(), new Vector2(MiddlePosX + 34, CurrentY), Color.White);

            if (SecondaryMenuIndex == 1)
            {
                BaseAutomaticSkill ActivePilotSkill = ActiveSquad.CurrentLeader.ArrayUnitAbility[SelectedUnitAbilityIndex];
                string Description = ActivePilotSkill.Description;

                if (string.IsNullOrEmpty(Description))
                    Description = "No description available for this skill";

                if (SelectedUnitAbilityIndex < 4)
                    g.Draw(sprPixel, new Rectangle(370, 130 + SelectedUnitAbilityIndex * 40, 100, 30), Color.FromNonPremultiplied(255, 255, 255, 127));
                else
                    g.Draw(sprPixel, new Rectangle(490, 130 + SelectedUnitAbilityIndex * 40, 100, 30), Color.FromNonPremultiplied(255, 255, 255, 127));

                float MaxBoxWidth = Constants.Width * 0.75f;

                string Output = FitTextToWidth(fntFinlanderFont, Description, MaxBoxWidth);

                Vector2 BoxPosition = new Vector2((Constants.Width - MaxBoxWidth) / 2, (Constants.Height - (int)fntFinlanderFont.MeasureString(Output).Y - 20) / 2);
                DrawBox(g, new Vector2(BoxPosition.X - 10, BoxPosition.Y - 10), (int)MaxBoxWidth + 20, (int)fntFinlanderFont.MeasureString(Output).Y + 20, Color.White);
                g.DrawString(fntFinlanderFont, Output, BoxPosition, Color.White);
            }
        }

        public void DrawTop(CustomSpriteBatch g)
        {
            int MenuWith = 130;
            int MenuHeight = 40;
            int DistanceBetweenMenu = (Constants.Width - MenuWith * 4) / 4;
            int MenuOffset = (int)(DistanceBetweenMenu * 0.5);

            int CurrentX = DistanceBetweenMenu - MenuOffset;

            DrawBox(g, new Vector2(CurrentX, 5), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Summary", new Vector2(DistanceBetweenMenu - MenuOffset + MenuWith / 2, 8), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, 5), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Pilot", new Vector2(DistanceBetweenMenu * 2 - MenuOffset + MenuWith + MenuWith / 2, 8), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, 5), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Unit", new Vector2(DistanceBetweenMenu * 3 - MenuOffset + MenuWith * 2 + MenuWith / 2, 8), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, 5), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Weapon", new Vector2(DistanceBetweenMenu * 4 - MenuOffset + MenuWith * 3 + MenuWith / 2, 8), Color.White);

            g.Draw(sprPixel, new Rectangle(DistanceBetweenMenu - MenuOffset + (DistanceBetweenMenu + MenuWith) * (int)StatusPannel, 5, MenuWith, MenuHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
        }
    }
}
