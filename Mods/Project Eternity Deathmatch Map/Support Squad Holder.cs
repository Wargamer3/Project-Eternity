using System;
using System.Collections.Generic;
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

                    //Can't support if the attacking unit is flying and the support can't fly.
                    if (SquadIndex >= 0 && ActivePlayer.ListSquad[SquadIndex].CurrentLeader.Boosts.SupportAttackModifier > 0
                        && (!AttackingSquad.IsFlying
                            || ActivePlayer.ListSquad[SquadIndex].CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainAir)))
                    {
                        if (!ActivePlayer.ListSquad[SquadIndex].CanMove || ActivePlayer.ListSquad[SquadIndex] == AttackingSquad)
                            continue;

                        Squad AttackerSupportSquad = ActivePlayer.ListSquad[SquadIndex];
                        Unit AttackerSupportUnit = AttackerSupportSquad.CurrentLeader;
                        Unit Defender = DefendingSquad.CurrentLeader;

                        AttackerSupportUnit.DisableAllAttacks();
                        AttackerSupportUnit.UpdateAllAttacks(AttackerSupportSquad.Position, DefendingSquad.Position, DefendingSquad.ArrayMapSize, DefendingSquad.CurrentMovement, true);

                        if (AttackerSupportUnit.CanAttack)
                        {
                            AttackerSupportUnit.BattleDefenseChoice = Unit.BattleDefenseChoices.Attack;
                            int BestDamage = 0;
                            int BestAttackIndex = -1;

                            for (AttackerSupportUnit.AttackIndex = 0; AttackerSupportUnit.AttackIndex < AttackerSupportUnit.ListAttack.Count; ++AttackerSupportUnit.AttackIndex)
                            {
                                if (!AttackerSupportUnit.CurrentAttack.CanAttack)
                                    continue;

                                int Accuracy = Map.CalculateHitRate(AttackerSupportUnit, AttackerSupportSquad, DefendingSquad.CurrentLeader, DefendingSquad, DefendingSquad.CurrentLeader.BattleDefenseChoice);
                                AttackerSupportUnit.AttackAccuracy = Accuracy + "%";

                                if (Accuracy > 0)
                                {
                                    BattleMap.BattleResult Result = Map.DamageFormula(AttackerSupportUnit, ActivePlayer.ListSquad[SquadIndex], 1, DefendingPlayerIndex, DefendingSquadIndex, 0, Defender.BattleDefenseChoice, true);

                                    if (Result.AttackDamage > BestDamage)
                                    {
                                        BestDamage = Result.AttackDamage;
                                        BestAttackIndex = AttackerSupportUnit.AttackIndex;
                                    }
                                }
                            }
                            if (BestAttackIndex >= 0)
                            {
                                AttackerSupportUnit.AttackIndex = BestAttackIndex;
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

                        //Can't support if the defending unit is flying and the support can't fly.
                        if (SquadIndex >= 0 && ActivePlayer.ListSquad[SquadIndex].CurrentLeader.Boosts.SupportDefendModifier > 0
                            && (!DefendingSquad.IsFlying
                                || ActivePlayer.ListSquad[SquadIndex].CurrentLeader.ListTerrainChoices.Contains(UnitStats.TerrainAir)))
                        {
                            ActivePlayer.ListSquad[SquadIndex].CurrentLeader.AttackIndex = -1;
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
