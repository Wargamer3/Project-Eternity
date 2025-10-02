using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class DeathmatchMap
    {
        public enum BattleMenuStages { Default, ChooseAttack, ChooseDefense, ChooseFormation, ChooseSquadMember, ChooseSquadMemberDefense, ChooseSupport, ChooseSupportAttack }

        public enum BattleMenuChoices { Start, Action, Formation, Support, Demo }

        #region Variables

        public BattleMenuStages BattleMenuStage;
        public BattleMenuChoices BattleMenuCursorIndex;
        public int BattleMenuCursorIndexSecond;
        public int BattleMenuCursorIndexThird;

        public FormationChoices BattleMenuDefenseFormationChoice;
        public FormationChoices BattleMenuOffenseFormationChoice;
        
        protected Vector2 BattleMenuMapCursor;
        public Texture2D sprBattleMenuBackground;

        #endregion

        protected void LoadPreBattleMenu()
        {
            sprBattleMenuBackground = Content.Load<Texture2D>("Deathmatch/Status Screen/Background Black");
        }

        private void BattleSumaryDraw(CustomSpriteBatch g, Squad RightSquad)
        {
            int MenuWith = 130;
            int MenuHeight = 30;
            int MenuPositionY = Constants.Height - 30;
            int DistanceBetweenMenu = (Constants.Width - MenuWith * 5) / 5;
            int MenuOffset = (int)(DistanceBetweenMenu * 0.5);

            int CurrentX = DistanceBetweenMenu - MenuOffset;

            DrawBox(g, new Vector2(CurrentX, MenuPositionY), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Start", new Vector2(DistanceBetweenMenu - MenuOffset + MenuWith / 2, MenuPositionY), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, MenuPositionY), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Action", new Vector2(DistanceBetweenMenu * 2 - MenuOffset + MenuWith + MenuWith / 2, MenuPositionY), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, MenuPositionY), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Formation", new Vector2(DistanceBetweenMenu * 3 - MenuOffset + MenuWith * 2 + MenuWith / 2, MenuPositionY), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, MenuPositionY), MenuWith, MenuHeight, Color.Black);
            g.DrawStringMiddleAligned(fntFinlanderFont, "Support", new Vector2(DistanceBetweenMenu * 4 - MenuOffset + MenuWith * 3 + MenuWith / 2, MenuPositionY), Color.White);

            CurrentX += DistanceBetweenMenu + MenuWith;

            DrawBox(g, new Vector2(CurrentX, MenuPositionY), MenuWith, MenuHeight, Color.Black);
            if (Constants.ShowAnimation)
                g.DrawStringMiddleAligned(fntFinlanderFont, "Demo: ON", new Vector2(DistanceBetweenMenu * 5 - MenuOffset + MenuWith * 4 + MenuWith / 2, MenuPositionY), Color.White);
            else
                g.DrawStringMiddleAligned(fntFinlanderFont, "Demo: OFF", new Vector2(DistanceBetweenMenu * 5 - MenuOffset + MenuWith * 4 + MenuWith / 2, MenuPositionY), Color.White);

            g.Draw(sprPixel, new Rectangle(DistanceBetweenMenu - MenuOffset + (DistanceBetweenMenu + MenuWith) * (int)BattleMenuCursorIndex, MenuPositionY, MenuWith, MenuHeight), Color.FromNonPremultiplied(255, 255, 255, 127));

            switch (BattleMenuStage)
            {
                case BattleMenuStages.ChooseAttack:
                    DrawAttackPanel(g, fntFinlanderFont);
                    break;

                case BattleMenuStages.ChooseDefense:
                    CurrentX = Constants.Width - 415;
                    DrawBox(g, new Vector2(CurrentX, 45), 100, 30, Color.Black);
                    g.DrawString(fntFinlanderFont, "Attack", new Vector2(CurrentX + 10, 45), Color.White);
                    DrawBox(g, new Vector2(CurrentX, 75), 100, 30, Color.Black);
                    g.DrawString(fntFinlanderFont, "Defend", new Vector2(CurrentX + 10, 75), Color.White);
                    DrawBox(g, new Vector2(CurrentX, 105), 100, 30, Color.Black);
                    g.DrawString(fntFinlanderFont, "Evade", new Vector2(CurrentX + 10, 105), Color.White);
                    g.Draw(sprPixel, new Rectangle(CurrentX, 45 + BattleMenuCursorIndexSecond * 30, 100, 30), Color.FromNonPremultiplied(255, 255, 255, 127));
                    break;

                case BattleMenuStages.ChooseSquadMemberDefense:
                    CurrentX = Constants.Width - 415;
                    switch (BattleMenuCursorIndexSecond)
                    {
                        case 0:
                            g.Draw(sprPixel, new Rectangle(CurrentX + 100, 45, 310, 80), Color.FromNonPremultiplied(255, 255, 255, 127));
                            DrawBox(g, new Vector2(CurrentX, 45), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Attack", new Vector2(CurrentX + 10, 45), Color.White);
                            DrawBox(g, new Vector2(CurrentX, 75), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Defend", new Vector2(CurrentX + 10, 75), Color.White);
                            DrawBox(g, new Vector2(CurrentX, 105), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Evade", new Vector2(CurrentX + 10, 105), Color.White);
                            break;

                        case 1:
                            g.Draw(sprPixel, new Rectangle(CurrentX + 100, 230, 310, 65), Color.FromNonPremultiplied(255, 255, 255, 127));
                            DrawBox(g, new Vector2(CurrentX, 230), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Attack", new Vector2(CurrentX + 10, 230), Color.White);
                            DrawBox(g, new Vector2(CurrentX, 260), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Defend", new Vector2(CurrentX + 10, 260), Color.White);
                            DrawBox(g, new Vector2(CurrentX, 290), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Evade", new Vector2(CurrentX + 10, 290), Color.White);
                            break;

                        case 2:
                            g.Draw(sprPixel, new Rectangle(CurrentX + 100, 300, 310, 65), Color.FromNonPremultiplied(255, 255, 255, 127));
                            DrawBox(g, new Vector2(CurrentX, 300), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Attack", new Vector2(CurrentX + 10, 300), Color.White);
                            DrawBox(g, new Vector2(CurrentX, 330), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Defend", new Vector2(CurrentX + 10, 330), Color.White);
                            DrawBox(g, new Vector2(CurrentX, 360), 100, 30, Color.Black);
                            g.DrawString(fntFinlanderFont, "Evade", new Vector2(CurrentX + 10, 360), Color.White);
                            break;
                    }
                    break;

                case BattleMenuStages.ChooseSquadMember:
                    CurrentX = Constants.Width - 315;
                    switch (BattleMenuCursorIndexSecond)
                    {
                        case 0:
                            g.Draw(sprPixel, new Rectangle(CurrentX, 45, 310, 80), Color.FromNonPremultiplied(255, 255, 255, 127));
                            break;

                        case 1:
                            g.Draw(sprPixel, new Rectangle(CurrentX, 230, 310, 65), Color.FromNonPremultiplied(255, 255, 255, 127));
                            break;

                        case 2:
                            g.Draw(sprPixel, new Rectangle(CurrentX, 300, 310, 65), Color.FromNonPremultiplied(255, 255, 255, 127));
                            break;
                    }
                    break;

                case BattleMenuStages.ChooseFormation:
                    CurrentX = Constants.Width - 385;
                    DrawBox(g, new Vector2(CurrentX, 390), MenuWith, MenuHeight, Color.Black);
                    g.DrawString(fntFinlanderFont, "Focused", new Vector2(CurrentX + 10, 390), Color.White);
                    DrawBox(g, new Vector2(CurrentX, 420), MenuWith, MenuHeight, Color.Black);
                    g.DrawString(fntFinlanderFont, "Spread", new Vector2(CurrentX + 10, 420), Color.White);
                    g.Draw(sprPixel, new Rectangle(CurrentX, 390 + BattleMenuCursorIndexSecond * 30, MenuWith, MenuHeight), Color.FromNonPremultiplied(255, 255, 255, 127));
                    break;
            }
        }

        public void BattleSumaryAttackDraw(CustomSpriteBatch g,
            int DefendingPlayerIndex, int DefendingSquadIndex, SupportSquadHolder TargetSquadSupport,
            int AttackingPlayerIndex, int AttackingSquadIndex, SupportSquadHolder AttackingSupport)
        {
            Squad AttackingSquad = ListPlayer[AttackingPlayerIndex].ListSquad[AttackingSquadIndex];
            Squad DefendingSquad = ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            g.Draw(sprBattleMenuBackground, new Rectangle(0, 0, Constants.Width, Constants.Height), null, Color.White);
            BattleSumaryDrawSquadLeft(g, DefendingSquad, BattleMenuDefenseFormationChoice, TargetSquadSupport.ActiveSquadSupport);

            BattleSumaryDrawSquadRight(g, AttackingSquad, BattleMenuOffenseFormationChoice);

            if (AttackingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                int AttackDamage = DamageFormula(AttackingSquad.CurrentLeader, AttackingSquad.CurrentLeader.CurrentAttack, AttackingSquad, 1f,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    DefendingSquad.CurrentLeader.BattleDefenseChoice, false)
                    .AttackDamage;

                g.DrawString(fntFinlanderFont, AttackDamage.ToString() + "( " + (int)(AttackDamage * 1.2) + " )",
                new Vector2(Constants.Width - 300, 230), Color.White);
            }

            if (DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                int AttackDamage = DamageFormula(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad, 1f,
                    AttackingPlayerIndex, AttackingSquadIndex, 0,
                    AttackingSquad.CurrentLeader.BattleDefenseChoice, false)
                    .AttackDamage;

                g.DrawString(fntFinlanderFont, AttackDamage.ToString() + "( " + (int)(AttackDamage * 1.2) + " )",
                new Vector2(20, 230), Color.White);
            }

            if (AttackingSupport.ActiveSquadSupport != null)
            {
                DrawBox(g, new Vector2(325, 370), 70, 35, Color.Blue);
                DrawBox(g, new Vector2(325, 370 + 30), 70, 35, Color.Blue);
                DrawBox(g, new Vector2(395, 370), 240, 65, Color.Blue);
                g.DrawString(fntFinlanderFont, AttackingSupport.ActiveSquadSupport.CurrentLeader.UnitStat.Name, new Vector2(405, 375), Color.White);
                if (AttackingSupport.ActiveSquadSupport.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
                {
                    g.DrawString(fntFinlanderFont, AttackingSupport.ActiveSquadSupport.CurrentLeader.CurrentAttack.ItemName, new Vector2(405, 397), Color.White);
                    //Draw remaining supports
                    g.DrawStringMiddleAligned(fntFinlanderFont, AttackingSupport.ActiveSquadSupport.CurrentLeader.Boosts.SupportAttackModifier + "/" + AttackingSupport.ActiveSquadSupport.CurrentLeader.Boosts.SupportAttackModifierMax,
                        new Vector2(359, 372), Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
                    //Draw the hit rate %.
                    g.DrawStringMiddleAligned(fntFinlanderFont, AttackingSupport.ActiveSquadSupport.CurrentLeader.AttackAccuracy,
                        new Vector2(359, 401), Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
                }
            }

            //Player side is always on the right.
            BattleSumaryDraw(g, AttackingSquad);

            switch (BattleMenuStage)
            {
                case BattleMenuStages.ChooseSupport:
                    DrawBox(g, new Vector2(0, 0), Constants.Width, 30, Color.Black);
                    g.DrawStringMiddleAligned(fntFinlanderFont, "Select Support", new Vector2(Constants.Width / 2, 0), Color.Yellow);
                    DrawBox(g, new Vector2(0, 30), Constants.Width, Constants.Height - 30, Color.White);
                    DrawBox(g, new Vector2(45, 40), 550, 40, Color.Blue);
                    g.DrawStringMiddleAligned(fntFinlanderFont, "No Support", new Vector2(Constants.Width / 2, 45), Color.White);

                    int StartY = 90;
                    for (int S = 0; S < AttackingSupport.Count; S++)
                    {
                        Unit ActiveSupportUnit = AttackingSupport[S].CurrentLeader;
                        DrawBox(g, new Vector2(35, StartY), 580, 75, Color.Blue);
                        g.DrawString(fntFinlanderFont, "Support", new Vector2(45, StartY + 5), Color.White);
                        g.DrawString(fntFinlanderFont, ActiveSupportUnit.Boosts.SupportAttackModifier + "/" + ActiveSupportUnit.Boosts.SupportAttackModifierMax, new Vector2(45, StartY + 35), Color.White);
                        g.Draw(ActiveSupportUnit.SpriteMap, new Vector2(200, StartY + 10), Color.White);
                        g.DrawString(fntFinlanderFont, ActiveSupportUnit.UnitStat.Name, new Vector2(245, StartY + 5), Color.White);
                        g.DrawString(fntFinlanderFont, ActiveSupportUnit.PilotName, new Vector2(245, StartY + 35), Color.White);
                        g.DrawString(fntFinlanderFont, "LVL", new Vector2(475, StartY + 5), Color.White);
                        g.DrawString(fntFinlanderFont, "Will", new Vector2(475, StartY + 35), Color.White);

                        g.DrawStringRightAligned(fntFinlanderFont, ActiveSupportUnit.PilotLevel.ToString(), new Vector2(600, StartY + 5), Color.White);
                        g.DrawStringRightAligned(fntFinlanderFont, ActiveSupportUnit.PilotMorale.ToString(), new Vector2(600, StartY + 35), Color.White);

                        StartY += 90;
                    }
                    if (AttackingSupport.ActiveSquadSupportIndex >= 0)
                        g.Draw(sprPixel, new Rectangle(35, 90 + 90 * AttackingSupport.ActiveSquadSupportIndex, 580, 75), Color.FromNonPremultiplied(255, 255, 255, 127));
                    else
                        g.Draw(sprPixel, new Rectangle(45, 40, 550, 40), Color.FromNonPremultiplied(255, 255, 255, 127));
                    break;

                case BattleMenuStages.ChooseSupportAttack:
                    DrawAttackPanel(g, fntFinlanderFont);
                    break;
            }
        }

        public void BattleSumaryDefenceDraw(CustomSpriteBatch g,
            int AttackingPlayerIndex, int AttackingSquadIndex, SupportSquadHolder ActiveSquadSupport,
            int DefendingPlayerIndex, int DefendingSquadIndex, SupportSquadHolder TargetSquadSupport)
        {
            Squad AttackingSquad = ListPlayer[AttackingPlayerIndex].ListSquad[AttackingSquadIndex];
            Squad DefendingSquad = ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            g.Draw(sprBattleMenuBackground, new Rectangle(0,0, Constants.Width, Constants.Height), null, Color.White);
            BattleSumaryDrawSquadLeft(g, AttackingSquad, BattleMenuOffenseFormationChoice, ActiveSquadSupport.ActiveSquadSupport);
            
            BattleSumaryDrawSquadRight(g, DefendingSquad, BattleMenuDefenseFormationChoice);
            
            if (DefendingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                int AttackDamage = DamageFormula(DefendingSquad.CurrentLeader, DefendingSquad.CurrentLeader.CurrentAttack, DefendingSquad, 1f,
                    AttackingPlayerIndex, AttackingSquadIndex, 0,
                    AttackingSquad.CurrentLeader.BattleDefenseChoice, false)
                    .AttackDamage;

                g.DrawString(fntFinlanderFont, AttackDamage.ToString() + "( " + (int)(AttackDamage * 1.2) + " )",
                new Vector2(Constants.Width - 300, 230),Color.White);
            }

            if (AttackingSquad.CurrentLeader.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                int AttackDamage = DamageFormula(AttackingSquad.CurrentLeader, AttackingSquad.CurrentLeader.CurrentAttack, AttackingSquad, 1f,
                    DefendingPlayerIndex, DefendingSquadIndex, 0,
                    DefendingSquad.CurrentLeader.BattleDefenseChoice, false)
                    .AttackDamage;

                g.DrawString(fntFinlanderFont, AttackDamage.ToString() + "( " + (int)(AttackDamage * 1.2) + " )",
                new Vector2(20, 230), Color.White);
            }

            if (TargetSquadSupport.ActiveSquadSupport != null)
            {
                DrawBox(g, new Vector2(325, 370), 70, 65, Color.Blue);
                DrawBox(g, new Vector2(395, 370), 240, 65, Color.Blue);
                g.DrawString(fntFinlanderFont, TargetSquadSupport.ActiveSquadSupport.CurrentLeader.UnitStat.Name, new Vector2(405, 375), Color.White);
                g.DrawString(fntFinlanderFont, "Support Defense", new Vector2(405, 397), Color.White);
                //Draw remaining supports
                g.DrawStringMiddleAligned(fntFinlanderFont, TargetSquadSupport.ActiveSquadSupport.CurrentLeader.Boosts.SupportDefendModifier + "/" + TargetSquadSupport.ActiveSquadSupport.CurrentLeader.Boosts.SupportDefendModifierMax,
                    new Vector2(353, 390), Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
            }

            //Player side is always on the right.
            BattleSumaryDraw(g, DefendingSquad);

            switch (BattleMenuStage)
            {
                case BattleMenuStages.ChooseSupport:
                    DrawBox(g, new Vector2(0, 0), Constants.Width, 30, Color.Black);
                    g.DrawStringMiddleAligned(fntFinlanderFont, "Select Support", new Vector2(Constants.Width / 2, 0), Color.Yellow);
                    DrawBox(g, new Vector2(0, 30), Constants.Width, Constants.Height - 30, Color.White);
                    DrawBox(g, new Vector2(45, 40), 550, 40, Color.Blue);
                    g.DrawStringMiddleAligned(fntFinlanderFont, "No Support", new Vector2(Constants.Width / 2, 45), Color.White);

                    int StartY = 90;
                    for (int S = 0; S < TargetSquadSupport.Count; S++)
                    {
                        Unit ActiveSupportUnit = TargetSquadSupport[S].CurrentLeader;
                        DrawBox(g, new Vector2(35, StartY), 580, 75, Color.Blue);
                        g.DrawString(fntFinlanderFont, "Support", new Vector2(45, StartY + 5), Color.White);
                        g.DrawString(fntFinlanderFont, ActiveSupportUnit.Boosts.SupportDefendModifier + "/" + ActiveSupportUnit.Boosts.SupportDefendModifierMax, new Vector2(45, StartY + 35), Color.White);
                        g.Draw(ActiveSupportUnit.SpriteMap, new Vector2(200, StartY + 10), Color.White);
                        g.DrawString(fntFinlanderFont, ActiveSupportUnit.UnitStat.Name, new Vector2(245, StartY + 5), Color.White);
                        g.DrawString(fntFinlanderFont, ActiveSupportUnit.PilotName, new Vector2(245, StartY + 35), Color.White);
                        g.DrawString(fntFinlanderFont, "LVL", new Vector2(475, StartY + 5), Color.White);
                        g.DrawString(fntFinlanderFont, "Will", new Vector2(475, StartY + 35), Color.White);

                        g.DrawStringRightAligned(fntFinlanderFont, ActiveSupportUnit.PilotLevel.ToString(), new Vector2(600, StartY + 5), Color.White);
                        g.DrawStringRightAligned(fntFinlanderFont, ActiveSupportUnit.PilotMorale.ToString(), new Vector2(600, StartY + 35), Color.White);

                        StartY += 90;
                    }
                    if (TargetSquadSupport.ActiveSquadSupportIndex >= 0)
                        g.Draw(sprPixel, new Rectangle(35, 90 + 90 * TargetSquadSupport.ActiveSquadSupportIndex, 580, 75), Color.FromNonPremultiplied(255, 255, 255, 127));
                    else
                        g.Draw(sprPixel, new Rectangle(45, 40, 550, 40), Color.FromNonPremultiplied(255, 255, 255, 127));
                    break;
            }
        }

        protected void BattleSumaryDrawSquadLeft(CustomSpriteBatch g, Squad SquadLeft, FormationChoices LeftFormation, Squad Support)
        {
            int DrawX = 5;
            int DrawY = 45;
            bool ShowStats = ListPlayer[TargetPlayerIndex].IsPlayerControlled || SquadLeft.ListAttackedTeam.Contains(ListPlayer[ActivePlayerIndex].TeamIndex);

            #region Leader

            //Draw the hit rate %.
            if (LeftFormation == FormationChoices.ALL)
            {
                BattleSumaryDrawLeaderLeft(g, DrawX, DrawY, SquadLeft.CurrentLeader, Color.FromNonPremultiplied(0xff, 0x00, 0x00, 255), ShowStats);
                for (int A = 0; A < SquadLeft.CurrentLeader.ListMAPAttackAccuracy.Count; A++)
                {
                    g.DrawStringMiddleAligned(fntAccuracySmall, SquadLeft.CurrentLeader.ListMAPAttackAccuracy[A], new Vector2(DrawX + 212, DrawY + 35 + A * 12), Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
                }
            }
            else
                BattleSumaryDrawLeaderLeft(g, DrawX, DrawY, SquadLeft.CurrentLeader, Color.FromNonPremultiplied(0xff, 0x00, 0x00, 255), ShowStats);

            #endregion

            for (int U = SquadLeft.UnitsAliveInSquad - 1; U >= 1; --U)
            {
                float Y = 160 + U * 70;

                BattleSumaryDrawWingmanLeft(g, 5, 230, SquadLeft[U], Color.FromNonPremultiplied(0xff, 0x00, 0x00, 255), ShowStats);
                DrawBox(g, new Vector2(5, Y), 240, 65, Color.Red);
                DrawBox(g, new Vector2(245, Y), 70, 65, Color.Red);
                g.DrawString(fntFinlanderFont, SquadLeft[U].UnitStat.Name, new Vector2(15, Y + 5), Color.White);
                g.DrawString(fntFinlanderFont, SquadLeft[U].HP.ToString(), new Vector2(15, Y + 27), Color.White);
            }

            if (Support != null)
            {
                DrawBox(g, new Vector2(5, 370), 240, 65, Color.Red);
                DrawBox(g, new Vector2(245, 370), 70, 35, Color.Red);
                DrawBox(g, new Vector2(245, 370 + 30), 70, 35, Color.Red);
                g.DrawString(fntFinlanderFont, Support.CurrentLeader.UnitStat.Name, new Vector2(15, 370 + 5), Color.White);

                //Support defend won't have an attack
                if (Support.CurrentLeader.CurrentAttack != null)
                {
                    g.DrawString(fntFinlanderFont, Support.CurrentLeader.CurrentAttack.ToString(), new Vector2(15, 370 + 27), Color.White);//Draw remaining supports
                    g.DrawStringMiddleAligned(fntFinlanderFont, Support.CurrentLeader.Boosts.SupportAttackModifier + "/" + Support.CurrentLeader.Boosts.SupportAttackModifierMax,
                        new Vector2(280, 372), Color.FromNonPremultiplied(0xff, 0x00, 0x00, 255));
                    //Draw the hit rate %.
                    g.DrawStringMiddleAligned(fntFinlanderFont, Support.CurrentLeader.AttackAccuracy,
                        new Vector2(280, 401), Color.FromNonPremultiplied(0xff, 0x00, 0x00, 255));
                }
            }
        }

        protected void BattleSumaryDrawLeaderLeft(CustomSpriteBatch g, int DrawX, int DrawY, Unit UnitToDraw, Color HitRateColor, bool ShowStats)
        {
            DrawBox(g, new Vector2(DrawX + 240, DrawY), 70, 80, Color.Red);
            DrawBox(g, new Vector2(DrawX, DrawY), 240, 80, Color.Red);
            DrawBox(g, new Vector2(DrawX, DrawY + 80), 310, 40, Color.Black);
            DrawBox(g, new Vector2(DrawX, DrawY + 120), 310, 60, Color.Red);
            g.Draw(UnitToDraw.SpriteMap, new Vector2(DrawX + 6, DrawY + 5), Color.White);
            TextHelper.DrawText(g, UnitToDraw.PilotName, new Vector2(DrawX + 41, DrawY + 3), Color.White);
            TextHelper.DrawText(g, UnitToDraw.UnitStat.Name, new Vector2(DrawX + 41, DrawY + 25), Color.White);
            TextHelper.DrawTextRightAligned(g, "LV: " + UnitToDraw.PilotLevel, new Vector2(DrawX + 230, DrawY + 3), Color.White);
            TextHelper.DrawTextRightAligned(g, "Will: " + UnitToDraw.PilotMorale, new Vector2(DrawX + 230, DrawY + 22), Color.White);

            TextHelper.DrawTextRightAligned(g, "HP", new Vector2(DrawX + 60, DrawY + 43), Color.Yellow);
            TextHelper.DrawTextRightAligned(g, "EN", new Vector2(DrawX + 60, DrawY + 59), Color.Yellow);
            //Draw HP bar
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(DrawX + 60, DrawY + 46), UnitToDraw.HP, UnitToDraw.MaxHP);
            //Draw EN bar.
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(DrawX + 60, DrawY + 62), UnitToDraw.EN, UnitToDraw.MaxEN);

            if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                //Draw the hit rate %.
                g.DrawStringMiddleAligned(fntFinlanderFont, UnitToDraw.AttackAccuracy, new Vector2(DrawX + 275, DrawY + 26), HitRateColor);
                //Draw the Weapon name.
                g.DrawString(fntFinlanderFont, UnitToDraw.CurrentAttack.ItemName, new Vector2(DrawX + 10, DrawY + 85), Color.White);
                //Draw the EN cost.
                if (UnitToDraw.CurrentAttack.ENCost > 0)
                {
                    DrawENRemaining(g, DrawX, DrawY, UnitToDraw);
                }
            }
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Defend)
                g.DrawString(fntFinlanderFont, "DEF", new Vector2(DrawX + 245, DrawY + 27), Color.White);
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Evade)
                g.DrawString(fntFinlanderFont, "EVA", new Vector2(DrawX + 245, DrawY + 27), Color.White);

            //Draw HP number.
            TextHelper.DrawTextRightAligned(g, UnitToDraw.HP + "/" + UnitToDraw.MaxHP, new Vector2(DrawX + 229, DrawY + 41), Color.White);
            //Draw EN number.
            TextHelper.DrawTextRightAligned(g, UnitToDraw.EN + "/" + UnitToDraw.MaxEN, new Vector2(DrawX + 229, DrawY + 57), Color.White);

            for (int A = 0; A < 4; A++)
            {
                string TextToDraw = "------";
                if (UnitToDraw.CurrentAttack != null && A < UnitToDraw.CurrentAttack.ArrayAttackAttributes.Length)
                    TextToDraw = UnitToDraw.CurrentAttack.ArrayAttackAttributes[A].Name;
                g.DrawString(fntFinlanderFont, TextToDraw, new Vector2(DrawX + 10 + (A / 2) * 150, DrawY + 125 + (A % 2) * fntFinlanderFont.LineSpacing), Color.White);
            }
        }

        protected void BattleSumaryDrawWingmanLeft(CustomSpriteBatch g, int DrawX, int DrawY, Unit UnitToDraw, Color HitRateColor, bool ShowStats)
        {
            DrawBox(g, new Vector2(DrawX, DrawY), 240, 65, Color.Red);
            DrawBox(g, new Vector2(DrawX + 240, DrawY), 70, 65, Color.Red);
            g.DrawString(fntFinlanderFont, UnitToDraw.UnitStat.Name, new Vector2(DrawX + 10, DrawY + 5), Color.White);
            g.DrawString(fntFinlanderFont, UnitToDraw.HP.ToString(), new Vector2(DrawX + 10, DrawY + 27), Color.White);

            if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                //Draw the hit rate %.
                g.DrawStringMiddleAligned(fntFinlanderFont, UnitToDraw.AttackAccuracy, new Vector2(DrawX + 275, DrawY + 26), HitRateColor);
                //Draw the EN cost.
                if (UnitToDraw.CurrentAttack.ENCost > 0)
                {
                    DrawENRemaining(g, DrawX, DrawY, UnitToDraw);
                }
            }
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Defend)
                g.DrawString(fntFinlanderFont, "DEF", new Vector2(DrawX + 245, DrawY + 17), Color.White);
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Evade)
                g.DrawString(fntFinlanderFont, "EVA", new Vector2(DrawX + 245, DrawY + 17), Color.White);
        }

        protected void BattleSumaryDrawSquadRight(CustomSpriteBatch g, Squad SquadRight, FormationChoices RightFormation)
        {
            int DrawX = Constants.Width - 315;
            int DrawY = 45;

            #region Leader

            if (RightFormation == FormationChoices.ALL)
            {
                BattleSumaryDrawLeaderRight(g, DrawX, DrawY, SquadRight.CurrentLeader, Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
                for (int A = 0; A < SquadRight.CurrentLeader.ListMAPAttackAccuracy.Count; A++)
                {
                    g.DrawStringMiddleAligned(fntAccuracySmall, SquadRight.CurrentLeader.ListMAPAttackAccuracy[A], new Vector2(DrawX + 25, DrawY + 12 + 12 * A), Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
                }
            }
            else
                BattleSumaryDrawLeaderRight(g, DrawX, DrawY, SquadRight.CurrentLeader, Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));

            #endregion

            for (int U = SquadRight.UnitsAliveInSquad - 1; U >= 1; --U)
            {
                float Y = 160 + U * 70;
                BattleSumaryDrawWingmanRight(g, 325, 230, SquadRight[U], Color.FromNonPremultiplied(0x00, 0xc6, 0xff, 255));
            }
        }

        protected void BattleSumaryDrawLeaderRight(CustomSpriteBatch g, int DrawX, int DrawY, Unit UnitToDraw, Color HitRateColor)
        {
            DrawBox(g, new Vector2(DrawX, DrawY), 70, 80, Color.Blue);
            DrawBox(g, new Vector2(DrawX + 70, DrawY), 240, 80, Color.Blue);
            DrawBox(g, new Vector2(DrawX, DrawY + 80), 310, 40, Color.Black);
            DrawBox(g, new Vector2(DrawX, DrawY + 120), 310, 60, Color.Blue);
            g.Draw(UnitToDraw.SpriteMap, new Vector2(DrawX + 76, DrawY + 5), Color.White);
            TextHelper.DrawText(g, UnitToDraw.PilotName, new Vector2(DrawX + 111, DrawY + 3), Color.White);
            TextHelper.DrawText(g, UnitToDraw.UnitStat.Name, new Vector2(DrawX + 111, DrawY + 25), Color.White);
            TextHelper.DrawTextRightAligned(g, "LV: " + UnitToDraw.PilotLevel, new Vector2(DrawX + 300, DrawY + 3), Color.White);
            TextHelper.DrawTextRightAligned(g, "Will: " + UnitToDraw.PilotMorale, new Vector2(DrawX + 300, DrawY + 22), Color.White);

            TextHelper.DrawTextRightAligned(g, "HP", new Vector2(DrawX + 130, DrawY + 43), Color.Yellow);
            TextHelper.DrawTextRightAligned(g, "EN", new Vector2(DrawX + 130, DrawY + 59), Color.Yellow);
            //Draw HP bar
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeHP, new Vector2(DrawX + 130, DrawY + 46), UnitToDraw.HP, UnitToDraw.MaxHP);
            //Draw EN bar.
            DrawBar(g, sprBarExtraLargeBackground, sprBarExtraLargeEN, new Vector2(DrawX + 130, DrawY + 62), UnitToDraw.EN, UnitToDraw.MaxEN);

            if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                //Draw the hit rate %.
                g.DrawStringMiddleAligned(fntFinlanderFont, UnitToDraw.AttackAccuracy,
                    new Vector2(DrawX + 35, DrawY + 26), HitRateColor);
                //Draw the Weapon name.
                g.DrawString(fntFinlanderFont, UnitToDraw.CurrentAttack.ItemName, new Vector2(DrawX + 10, DrawY + 85), Color.White);
                //Draw the EN cost.
                if (UnitToDraw.CurrentAttack.ENCost > 0)
                {
                    DrawENRemaining(g, DrawX + 70, DrawY, UnitToDraw);
                }
            }
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Defend)
                g.DrawString(fntFinlanderFont, "DEF", new Vector2(DrawX + 9, DrawY + 27), Color.White);
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Evade)
                g.DrawString(fntFinlanderFont, "EVA", new Vector2(DrawX + 5, DrawY + 27), Color.White);

            //Draw HP number.
            TextHelper.DrawTextRightAligned(g, UnitToDraw.HP + "/" + UnitToDraw.MaxHP, new Vector2(DrawX + 299, DrawY + 41), Color.White);
            //Draw EN number.
            TextHelper.DrawTextRightAligned(g, UnitToDraw.EN + "/" + UnitToDraw.MaxEN, new Vector2(DrawX + 299, DrawY + 57), Color.White);

            for (int A = 0; A < 4; A++)
            {
                string TextToDraw = "------";
                if (UnitToDraw.CurrentAttack != null && A < UnitToDraw.CurrentAttack.ArrayAttackAttributes.Length)
                    TextToDraw = UnitToDraw.CurrentAttack.ArrayAttackAttributes[A].Name;
                g.DrawString(fntFinlanderFont, TextToDraw, new Vector2(DrawX + 10 + (A / 2) * 150, DrawY + 125 + (A % 2) * fntFinlanderFont.LineSpacing), Color.White);
            }
        }

        protected void BattleSumaryDrawWingmanRight(CustomSpriteBatch g, int DrawX, int DrawY, Unit UnitToDraw, Color HitRateColor)
        {
            DrawBox(g, new Vector2(DrawX, DrawY), 70, 65, Color.Blue);
            DrawBox(g, new Vector2(DrawX + 70, DrawY), 240, 65, Color.Blue);
            g.DrawString(fntFinlanderFont, UnitToDraw.UnitStat.Name, new Vector2(DrawX + 80, DrawY + 5), Color.White);
            g.DrawString(fntFinlanderFont, UnitToDraw.HP.ToString(), new Vector2(DrawX + 80, DrawY + 27), Color.White);

            if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Attack)
            {
                //Draw the hit rate %.
                g.DrawString(fntFinlanderFont, UnitToDraw.AttackAccuracy, new Vector2(DrawX + 35, DrawY + 19), HitRateColor);
                //Draw the EN cost.
                if (UnitToDraw.CurrentAttack.ENCost > 0)
                {
                    DrawENRemaining(g, DrawX + 70, DrawY, UnitToDraw);
                }
            }
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Defend)
                g.DrawString(fntFinlanderFont, "DEF", new Vector2(DrawX + 10, DrawY + 17), Color.White);
            else if (UnitToDraw.BattleDefenseChoice == Unit.BattleDefenseChoices.Evade)
                g.DrawString(fntFinlanderFont, "EVA", new Vector2(DrawX + 10, DrawY + 17), Color.White);
        }

        private void DrawENRemaining(CustomSpriteBatch g, int DrawX, int DrawY, Unit UnitToDraw)
        {
            float ENBarRatio = sprBarExtraLargeEN.Width / (float)UnitToDraw.MaxEN;
            g.Draw(sprBarExtraLargeEN,
                new Vector2(DrawX + 60 + (int)(ENBarRatio * UnitToDraw.EN - ENBarRatio * UnitToDraw.CurrentAttack.ENCost),
                    DrawY + 62),
                new Rectangle((int)(ENBarRatio * UnitToDraw.EN - ENBarRatio * UnitToDraw.CurrentAttack.ENCost),
                    0,
                    (int)(Math.Ceiling(ENBarRatio * UnitToDraw.CurrentAttack.ENCost)),
                    sprBarExtraLargeEN.Height),
                Color.Red);
        }
    }
}
