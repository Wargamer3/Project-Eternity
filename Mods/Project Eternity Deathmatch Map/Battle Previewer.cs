using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units;
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

        private int UnitDisplayCounter;

        public BattlePreviewer(DeathmatchMap Map, Squad ActiveSquad, Attack ActiveAttack)
        {
            this.Map = Map;
            this.ActiveSquad = ActiveSquad;
            this.ActiveAttack = ActiveAttack;
            if (ActiveAttack != null)
            {
                this.ActiveSquad.CurrentLeader.AttackIndex = this.ActiveSquad.CurrentLeader.ListAttack.IndexOf(ActiveAttack);
            }
        }

        public void UpdateUnitDisplay()
        {
            if (UnitDisplayCounter < 70)
            {
                ++UnitDisplayCounter;

                if (UnitDisplayCounter == 70)
                {
                    UnitDisplayCounter = 0;
                    int UnitIndex = -1;

                    for (int P = 0; P < Map.ListPlayer.Count && UnitIndex == -1; P++)
                    {
                        UnitIndex = Map.CheckForSquadAtPosition(P, Map.CursorPosition, Vector3.Zero);
                        if (UnitIndex >= 0)
                        {
                            if (P == Map.ActivePlayerIndex)
                            {
                                BoxColor = Color.Green;
                            }
                            else
                            {
                                BoxColor = Color.Red;
                            }

                            Squad DefendingSquad = Map.ListPlayer[P].ListSquad[UnitIndex];

                            if (ActiveAttack != null)
                            {
                                SupportSquadHolder ActiveSquadSupport = new SupportSquadHolder();
                                SupportSquadHolder TargetSquadSupport = new SupportSquadHolder();

                                ActiveSquadSupport.PrepareAttackSupport(Map, Map.ActivePlayerIndex, ActiveSquad, DefendingSquad);
                                TargetSquadSupport.PrepareDefenceSupport(Map, P, DefendingSquad);

                                BattleResult = Map.CalculateFinalHP(ActiveSquad, ActiveSquadSupport.ActiveSquadSupport, Map.ActivePlayerIndex,
                                                                        FormationChoices.Focused,
                                                                        DefendingSquad, TargetSquadSupport.ActiveSquadSupport, P, true, false);
                            }
                            else
                            {
                                BattleResult.ArrayResult = new BattleResult[DefendingSquad.UnitsAliveInSquad];
                                for(int U = 0; U < DefendingSquad.UnitsAliveInSquad; ++U)
                                {
                                    BattleResult.ArrayResult[U] = new BattleResult();
                                    BattleResult.ArrayResult[U].Target = DefendingSquad[U];
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
