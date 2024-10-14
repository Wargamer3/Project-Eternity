using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units.Conquest;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    partial class ConquestMap
    {
        public int CheckForSquadAtPosition(int PlayerIndex, Vector3 Position, Vector3 Displacement)
        {
            if (ListPlayer[PlayerIndex].ListUnit.Count == 0)
                return -1;

            Vector3 FinalPosition = Position + Displacement;

            if (FinalPosition.X < 0 || FinalPosition.X >= MapSize.X || FinalPosition.Y < 0 || FinalPosition.Y >= MapSize.Y)
                return -1;

            int S = 0;
            bool SquadFound = false;
            //Check if there's a Construction.
            while (S < ListPlayer[PlayerIndex].ListUnit.Count && !SquadFound)
            {
                if (ListPlayer[PlayerIndex].ListUnit[S].HP == 0)
                {
                    ++S;
                    continue;
                }
                if (ListPlayer[PlayerIndex].ListUnit[S].IsUnitAtPosition(FinalPosition, TileSize))
                    SquadFound = true;
                else
                    ++S;
            }
            //If a Unit was founded.
            if (SquadFound)
                return S;

            return -1;
        }

        public Tuple<int, int> CheckForEnemies(int ActivePlayerIndex, Vector3 PositionToCheck, bool FriendlyFire)
        {
            for (int P = 0; P < ListPlayer.Count; P++)
            {
                int TargetIndex = CheckForSquadAtPosition(P, PositionToCheck, Vector3.Zero);

                if (TargetIndex >= 0 && (FriendlyFire || ListPlayer[ActivePlayerIndex].TeamIndex != ListPlayer[P].TeamIndex))
                {
                    return new Tuple<int, int>(P, TargetIndex);
                }
            }

            return null;
        }

        public Stack<Tuple<int, int>> GetEnemies(bool FriendlyFire, List<MovementAlgorithmTile> ListAttackPosition)
        {
            Stack<Tuple<int, int>> ListMAPAttackTarget = new Stack<Tuple<int, int>>();//Player index, Squad index.

            for (int P = 0; P < ListPlayer.Count; P++)
            {
                for (int i = 0; i < ListAttackPosition.Count; i++)
                {
                    //Find if a Unit is under the cursor.
                    int TargetIndex = CheckForSquadAtPosition(P, ListAttackPosition[i].WorldPosition, Vector3.Zero);
                    //If one was found.
                    if (TargetIndex >= 0 && (FriendlyFire ||
                                                ListPlayer[ActivePlayerIndex].TeamIndex != ListPlayer[P].TeamIndex))
                    {
                        ListMAPAttackTarget.Push(new Tuple<int, int>(P, TargetIndex));
                    }
                }
            }

            return ListMAPAttackTarget;
        }

        public void AttackWithExplosion(int ActivePlayerIndex, UnitConquest Owner, Attack ActiveAttack, Vector3 Position)
        {
            Stack<Tuple<int, int>> ListAttackTarget = new Stack<Tuple<int, int>>();

            for (float OffsetX = -ActiveAttack.ExplosionOption.ExplosionRadius; OffsetX < ActiveAttack.ExplosionOption.ExplosionRadius; ++OffsetX)
            {
                for (float OffsetY = -ActiveAttack.ExplosionOption.ExplosionRadius; OffsetY < ActiveAttack.ExplosionOption.ExplosionRadius; ++OffsetY)
                {
                    for (float OffsetZ = -ActiveAttack.ExplosionOption.ExplosionRadius; OffsetZ < ActiveAttack.ExplosionOption.ExplosionRadius; ++OffsetZ)
                    {
                        if (Math.Abs(OffsetX) + Math.Abs(OffsetY) + Math.Abs(OffsetZ) < ActiveAttack.ExplosionOption.ExplosionRadius)
                        {
                            Tuple<int, int> ActiveTarget = CheckForEnemies(ActivePlayerIndex, new Vector3(Position.X + OffsetX, Position.Y + OffsetY, Position.Z + OffsetZ), true);

                            if (ActiveTarget != null)
                            {
                                ListAttackTarget.Push(ActiveTarget);
                            }
                        }
                    }
                }
            }

            if (ListAttackTarget.Count > 0)
            {
                foreach (Tuple<int, int> ActiveTarget in ListAttackTarget)
                {
                    UnitConquest SquadToKill = ListPlayer[ActiveTarget.Item1].ListUnit[ActiveTarget.Item2];

                    TerrainConquest SquadTerrain = GetTerrain(SquadToKill.Components);

                    Vector3 FinalSpeed = new Vector3(SquadToKill.Position.X - Position.X, SquadToKill.Position.Y - Position.Y, SquadTerrain.WorldPosition.Z - Position.Z);

                    float DiffTotal = FinalSpeed.Length() / 3f;

                    if (DiffTotal < ActiveAttack.ExplosionOption.ExplosionRadius)
                    {
                        float WindForce = 1 - (DiffTotal / ActiveAttack.ExplosionOption.ExplosionRadius);
                        float WindValue = ActiveAttack.ExplosionOption.ExplosionWindPowerAtEdge + WindForce * (ActiveAttack.ExplosionOption.ExplosionWindPowerAtCenter - ActiveAttack.ExplosionOption.ExplosionWindPowerAtEdge);

                        FinalSpeed.Normalize();

                        FinalSpeed *= WindValue;
                        SquadToKill.Components.Speed = FinalSpeed;

                        if (SquadToKill.Components.IsOnGround)
                        {
                            SquadToKill.SetPosition(SquadTerrain.WorldPosition + new Vector3(0.5f, 0.5f, 0f));
                            if (FinalSpeed.Z < 0)
                            {
                                SquadToKill.Components.IsOnGround = false;
                            }
                        }
                    }
                }

                ActionPanelUseMAPAttack.AttackWithMAPAttack(this, ActivePlayerIndex, ListPlayer[ActivePlayerIndex].ListUnit.IndexOf(Owner), ActiveAttack, new List<Vector3>(), ListAttackTarget);
            }
        }

        public List<Tuple<int, int>> CanSquadAttackWeapon1(int PosX, int PosY, int ActivePlayerIndex, string UnitName, Attack ActiveWeapon)
        {
            return CanSquadAttackWeapon(PosX, PosY, ActivePlayerIndex, UnitName, ActiveWeapon, DicUnitDamageWeapon1);
        }

        public List<Tuple<int, int>> CanSquadAttackWeapon2(int PosX, int PosY, int ActivePlayerIndex, string UnitName, Attack ActiveWeapon)
        {
            return CanSquadAttackWeapon(PosX, PosY, ActivePlayerIndex, UnitName, ActiveWeapon, DicUnitDamageWeapon2);
        }

        public List<Tuple<int, int>> CanSquadAttackWeapon(int PosX, int PosY, int ActivePlayerIndex, string UnitName, Attack ActiveWeapon, Dictionary<string, Dictionary<string, int>> DicUnitDamageWeapon)
        {
            if (ActiveWeapon.RelativePath == "" || !DicUnitDamageWeapon.ContainsKey(UnitName))
                return new List<Tuple<int, int>>();

            List<Tuple<int, int>> ListSquadFound = new List<Tuple<int, int>>();
            float Distance;

            for (int P = 0; P < ListPlayer.Count; P++)
            {//Only check for enemies, can move through allies, can't move through ennemies.
                if (ListPlayer[P].TeamIndex == ListPlayer[ActivePlayerIndex].TeamIndex)
                    continue;

                for (int TargetSelect = 0; TargetSelect < ListPlayer[P].ListUnit.Count; TargetSelect++)
                {
                    //Calculate is the Target is in attack range.
                    Distance = Math.Abs(PosX - ListPlayer[P].ListUnit[TargetSelect].X) + Math.Abs(PosY - ListPlayer[P].ListUnit[TargetSelect].Y);
                    if (Distance >= ActiveWeapon.RangeMinimum && Distance <= ActiveWeapon.RangeMaximum)
                    {
                        if (DicUnitDamageWeapon[UnitName].ContainsKey(ListPlayer[P].ListUnit[TargetSelect].ArmourType))
                            ListSquadFound.Add(new Tuple<int, int>(P, TargetSelect));
                    }
                }
            }

            return ListSquadFound;
        }

        public int GetAttackDamageWithWeapon(UnitConquest Attacker, UnitConquest Defender, int AttackerHP)
        {
            //Attack Formula
            float ChartDamage = 55; //Dommage par defaut provenant du table comparatif
            float UnitATK = 100; //Bonus du CO attaquant spécifique à l'unité
            float UniversalATK = 100; //Bonus du CO attaquant universel à toute ces unité
            float Luck = 5; //Bonus allant de 0 à 9, certain CO on d'autre valeur plancher et plafond

            ChartDamage = ChartDamage / 10;

            UnitATK = UnitATK / 100;
            UniversalATK = UniversalATK / 100;

            float AttackValue = ChartDamage * UnitATK * UniversalATK;
            AttackValue += Luck;

            //Defense Formula
            float DefenderHP = Defender.HP / 10f; //Point de vie de l'unité qui défend, allant de 10 à 1 
            float TerrainStars = 3; //Point de défense octroyé par le terrain ou ce trouve l'unité qui défend
            float UnitDEF = 100; //Bonus du CO défendant spécifique à l'unité
            float UniversalDEF = 100; //Bonus du CO défendant universel à toute ces unité

            TerrainStars = TerrainStars * DefenderHP;
            TerrainStars = 100 - TerrainStars;
            TerrainStars = TerrainStars / 100;

            UnitDEF = 200 - UnitDEF;
            UnitDEF = UnitDEF / 100;

            UniversalDEF = 200 - UniversalDEF;
            UniversalDEF = UniversalDEF / 100;

            float DefenseValue = TerrainStars * UnitDEF * UniversalDEF;


            float TotalDamage = (AttackerHP / 10f) * AttackValue * DefenseValue;
            return (int)Math.Floor(TotalDamage);
        }
        public int GetAttackDamageWithWeapon1(UnitConquest Attacker, UnitConquest Defender, int AttackerHP)
        {
            int ActualHP = (int)Math.Ceiling(AttackerHP / 10d);
            int DamageDealt = DicUnitDamageWeapon1[Attacker.ArmourType][Defender.ArmourType];

            int Offense = 100;
            float Defense = 100;
            return (int)Math.Floor(DamageDealt * (Offense / Defense) * (ActualHP / 10d));
        }

        public int GetAttackDamageWithWeapon2(UnitConquest Attacker, UnitConquest Defender, int AttackerHP)
        {
            int ActualHP = (int)Math.Ceiling(AttackerHP / 10d);
            int DamageDealt = DicUnitDamageWeapon2[Attacker.ArmourType][Defender.ArmourType];

            int Offense = 100;
            float Defense = 100;
            return (int)Math.Floor(DamageDealt * (Offense / Defense) * (ActualHP / 10d));
        }
    }
}
