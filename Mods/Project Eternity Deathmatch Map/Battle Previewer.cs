﻿using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.BattleMapScreen.BattleMap;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class BattlePreviewer
    {
        private SquadBattleResult BattleResult;
        private Color BoxColor;
        private readonly DeathmatchMap Map;
        private readonly Squad ActiveSquad;
        private readonly Attack ActiveAttack;
        public readonly int PlayerIndex;
        public readonly int SquadIndex;

        private int UnitDisplayCounter;
        private const int TimeToDisplay = 20;

        public BattlePreviewer(DeathmatchMap Map, int PlayerIndex, int SquadIndex, Attack ActiveAttack)
        {
            this.Map = Map;
            this.PlayerIndex = PlayerIndex;
            this.SquadIndex = SquadIndex;
            this.ActiveAttack = ActiveAttack;

            ActiveSquad = Map.ListPlayer[PlayerIndex].ListSquad[SquadIndex];

            if (ActiveAttack != null)
            {
                ActiveSquad.CurrentLeader.CurrentAttack = ActiveAttack;
            }
        }

        public void UpdateUnitDisplay()
        {
            if (UnitDisplayCounter < TimeToDisplay)
            {
                ++UnitDisplayCounter;

                if (UnitDisplayCounter == TimeToDisplay)
                {
                    UnitDisplayCounter = 0;
                    int SquadIndex = -1;

                    for (int P = 0; P < Map.ListPlayer.Count && SquadIndex == -1; P++)
                    {
                        SquadIndex = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);
                        if (SquadIndex >= 0)
                        {
                            if (P == Map.ActivePlayerIndex)
                            {
                                BoxColor = Color.Green;
                            }
                            else
                            {
                                BoxColor = Color.Red;
                            }

                            Squad DefendingSquad = Map.ListPlayer[P].ListSquad[SquadIndex];
                            Map.TargetPlayerIndex = P;
                            Map.TargetSquadIndex = SquadIndex;

                            if (ActiveAttack != null)
                            {
                                SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
                                SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();

                                ActiveSquadSupport.PrepareAttackSupport(Map, PlayerIndex, ActiveSquad, P, SquadIndex);
                                TargetSquadSupport.PrepareDefenceSupport(Map, P, DefendingSquad);

                                BattleResult = Map.CalculateFinalHP(ActiveSquad, ActiveAttack, ActiveSquadSupport.ActiveSquadSupport, Map.ActivePlayerIndex,
                                                                        FormationChoices.Focused,
                                                                        DefendingSquad, TargetSquadSupport.ActiveSquadSupport,
                                                                        P, SquadIndex, true, false);
                            }
                            else
                            {
                                BattleResult.ArrayResult = new BattleResult[DefendingSquad.UnitsAliveInSquad];
                                for(int U = 0; U < DefendingSquad.UnitsAliveInSquad; ++U)
                                {
                                    BattleResult.ArrayResult[U] = new BattleResult();
                                    BattleResult.ArrayResult[U].SetTarget(P, SquadIndex, U, DefendingSquad[U]);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawDisplayUnit(CustomSpriteBatch g)
        {
            int X = 0;
            int Y = Constants.Height - 80;
            if (BattleResult.ArrayResult == null || BattleResult.ArrayResult.Length == 0)
                return;

            DrawDisplayUnit(g, BattleResult.ArrayResult[0], X, Y, BoxColor);

            if (BattleResult.ArrayResult.Length > 1)
            {
                DrawDisplayUnit(g, BattleResult.ArrayResult[1], X + 128, Y, BoxColor);
            }

            if (BattleResult.ArrayResult.Length > 2)
            {
                DrawDisplayUnit(g, BattleResult.ArrayResult[2], X + 256, Y, BoxColor);
            }
        }

        public void DrawDisplayUnit(CustomSpriteBatch g, BattleResult TargetUnitResult, int X, int Y, Color BoxColor)
        {
            if (TargetUnitResult.Target == null)
                return;

            GameScreen.DrawBox(g, new Vector2(X, Y), 200, 80, BoxColor);

            TextHelper.DrawText(g, TargetUnitResult.Target.UnitStat.Name, new Vector2(X + 45, Y + 6), Color.White);
            if (TargetUnitResult.Target.Pilot != null)
            {
                TextHelper.DrawText(g, TargetUnitResult.Target.Pilot.Name, new Vector2(X + 45, Y + 24), Color.White);
            }

            g.Draw(TargetUnitResult.Target.SpriteMap, new Rectangle(X + 6, Y + 8, 32, 32), Color.White);

            GameScreen.DrawBar(g, Map.sprBarSmallBackground, Map.sprBarSmallHP, new Vector2(X + 140, Y + 13), TargetUnitResult.Target.HP, TargetUnitResult.Target.MaxHP);
            GameScreen.DrawBar(g, Map.sprBarSmallBackground, Map.sprBarSmallEN, new Vector2(X + 140, Y + 30), TargetUnitResult.Target.EN, TargetUnitResult.Target.MaxEN);

            //Draw numbers.
            g.DrawStringRightAligned(Map.fntBattleNumberSmall, TargetUnitResult.Target.HP.ToString(), new Vector2(X + 191, Constants.Height - 78), Color.White);
            g.DrawStringRightAligned(Map.fntBattleNumberSmall, TargetUnitResult.Target.EN.ToString(), new Vector2(X + 191, Constants.Height - 58), Color.White);

            if (ActiveAttack != null)
            {
                if (!TargetUnitResult.AttackMissed)
                {
                    TextHelper.DrawText(g, "Hit " + TargetUnitResult.Accuracy.ToString(), new Vector2(X + 6, Y + 42), Color.White);
                }

                if (TargetUnitResult.AttackDamage != 0)
                {
                    TextHelper.DrawText(g, "DMG " + TargetUnitResult.AttackDamage + "(" + (int)(TargetUnitResult.AttackDamage * 1.2f) + ")", new Vector2(X + 6, Y + 58), Color.White);
                }
            }
        }
    }
}
