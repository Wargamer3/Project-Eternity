using System;
using System.Collections.Generic;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class SupportSquadHolder
    {
        private List<Squad> ListSquadSupport;
        public int ActiveSquadSupportIndex;

        public Squad ActiveSquadSupport { get { return ActiveSquadSupportIndex < 0 || ActiveSquadSupportIndex >= ListSquadSupport.Count ? null : ListSquadSupport[ActiveSquadSupportIndex]; } }

        public int Count { get { return ListSquadSupport.Count; } }

        public SupportSquadHolder()
        {
            ListSquadSupport = new List<Squad>();
            ActiveSquadSupportIndex = -1;
        }

        public Squad this[int i]
        {
            get
            {
                return ListSquadSupport[i];
            }
        }

        private void AddSupportSquad(Squad NewSupportSquad)
        {
            ListSquadSupport.Add(NewSupportSquad);
        }

        private void Clear()
        {
            ActiveSquadSupportIndex = -1;
            ListSquadSupport.Clear();
        }

        public void PrepareAttackSupport(DeathmatchMap Map, int ActivePlayerIndex, Squad AttackingSquad, int DefendingPlayerIndex, int DefendingSquadIndex)
        {
            Squad DefendingSquad = Map.ListPlayer[DefendingPlayerIndex].ListSquad[DefendingSquadIndex];

            Clear();

            Player ActivePlayer = Map.ListPlayer[ActivePlayerIndex];
            
            for (int X = -1; X <= 1; X++)
            {
                for (int Y = -1; Y <= 1; Y++)
                {
                    if ((AttackingSquad.X + X == AttackingSquad.X && AttackingSquad.Y + Y == AttackingSquad.Y) || (Math.Abs(X) + Math.Abs(Y) > 1))
                        continue;

                    int SquadIndex = Map.CheckForSquadAtPosition(ActivePlayerIndex, AttackingSquad.Position, new Microsoft.Xna.Framework.Vector3(X, Y, 0));

                    //Can't support if the support can't enter the same terrain as the attacker, supporter will be visible in the battle animation.
                    if (SquadIndex >= 0 && ActivePlayer.ListSquad[SquadIndex].CurrentLeader.Boosts.SupportAttackModifier > 0
                        && Map.TerrainRestrictions.CanMove(ActivePlayer.ListSquad[SquadIndex], ActivePlayer.ListSquad[SquadIndex].CurrentLeader.UnitStat, AttackingSquad.CurrentTerrainIndex))
                    {
                        if (!ActivePlayer.ListSquad[SquadIndex].CanMove || ActivePlayer.ListSquad[SquadIndex] == AttackingSquad)
                            continue;

                        Squad AttackerSupportSquad = ActivePlayer.ListSquad[SquadIndex];
                        Unit AttackerSupportUnit = AttackerSupportSquad.CurrentLeader;
                        Unit Defender = DefendingSquad.CurrentLeader;

                        AttackerSupportUnit.DisableAllAttacks();
                        AttackerSupportUnit.UpdateAllAttacks(AttackerSupportSquad.Position, Map.ListPlayer[ActivePlayerIndex].Team, DefendingSquad.Position, Map.ListPlayer[DefendingPlayerIndex].Team, DefendingSquad.ArrayMapSize, DefendingSquad.CurrentTerrainIndex, true);

                        if (AttackerSupportUnit.CanAttack)
                        {
                            AttackerSupportUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                            int BestDamage = 0;
                            Attack BestAttack = null;

                            foreach (Attack ActiveAttack in AttackerSupportUnit.ListAttack)
                            {
                                AttackerSupportUnit.CurrentAttack = ActiveAttack;
                                if (!AttackerSupportUnit.CurrentAttack.CanAttack)
                                    continue;

                                int Accuracy = Map.CalculateHitRate(AttackerSupportUnit, AttackerSupportUnit.CurrentAttack, AttackerSupportSquad, DefendingSquad.CurrentLeader, DefendingSquad, DefendingSquad.CurrentLeader.BattleDefenseChoice);
                                AttackerSupportUnit.AttackAccuracy = Accuracy + "%";

                                if (Accuracy > 0)
                                {
                                    BattleMap.BattleResult Result = Map.DamageFormula(AttackerSupportUnit, AttackerSupportUnit.CurrentAttack, ActivePlayer.ListSquad[SquadIndex], 1, DefendingPlayerIndex, DefendingSquadIndex, 0, Defender.BattleDefenseChoice, true);

                                    if (Result.AttackDamage > BestDamage)
                                    {
                                        BestDamage = Result.AttackDamage;
                                        BestAttack = ActiveAttack;
                                    }
                                }
                            }
                            if (BestAttack != null)
                            {
                                AttackerSupportUnit.CurrentAttack = BestAttack;
                                AddSupportSquad(AttackerSupportSquad);
                                ActiveSquadSupportIndex = Count - 1;
                            }
                        }
                    }
                }
            }
        }

        public void PrepareDefenceSupport(DeathmatchMap Map, int DefendingPlayerIndex, Squad DefendingSquad)
        {
            Clear();

            Player ActivePlayer = Map.ListPlayer[DefendingPlayerIndex];

            if (Map.BattleMenuOffenseFormationChoice != BattleMap.FormationChoices.ALL)
            {
                for (int X = -1; X <= 1; X++)
                {
                    for (int Y = -1; Y <= 1; Y++)
                    {
                        if ((DefendingSquad.X + X == DefendingSquad.X && DefendingSquad.Y + Y == DefendingSquad.Y) || (Math.Abs(X) + Math.Abs(Y) > 1))
                            continue;

                        int SquadIndex = Map.CheckForSquadAtPosition(DefendingPlayerIndex, DefendingSquad.Position, new Microsoft.Xna.Framework.Vector3(X, Y, 0));

                        //Can't support if the support can't enter the same terrain as the defending, supporter will be visible in the battle animation.
                        if (SquadIndex >= 0 && ActivePlayer.ListSquad[SquadIndex].CurrentLeader.Boosts.SupportDefendModifier > 0
                            && Map.TerrainRestrictions.CanMove(ActivePlayer.ListSquad[SquadIndex], ActivePlayer.ListSquad[SquadIndex].CurrentLeader.UnitStat, DefendingSquad.CurrentTerrainIndex))
                        {
                            ActivePlayer.ListSquad[SquadIndex].CurrentLeader.CurrentAttack = null;
                            AddSupportSquad(ActivePlayer.ListSquad[SquadIndex]);

                            if (ActiveSquadSupport == null || ActivePlayer.ListSquad[SquadIndex].CurrentLeader.HP > ActiveSquadSupport.CurrentLeader.HP)
                            {
                                ActiveSquadSupportIndex = Count - 1;
                            }
                        }
                    }
                }
            }
        }
    }
}
