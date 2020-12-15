using System;
using System.Collections.Generic;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units.Conquest;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    partial class ConquestMap
    {
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
            if (ActiveWeapon.FullName == "" || !DicUnitDamageWeapon.ContainsKey(UnitName))
                return new List<Tuple<int, int>>();

            List<Tuple<int, int>> ListSquadFound = new List<Tuple<int, int>>();
            float Distance;

            for (int P = 0; P < ListPlayer.Count; P++)
            {//Only check for enemies, can move through allies, can't move through ennemies.
                if (ListPlayer[P].Team == ListPlayer[ActivePlayerIndex].Team)
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
