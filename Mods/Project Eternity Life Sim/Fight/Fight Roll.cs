using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    internal static class FightRoll
    {
        internal static void ApplyConditions(PlayerCharacter Target, StrikeRoll Roll, StrikeDegree Degree, PlayerCharacter Source)
        {
            if (Target == null || Roll == null)
            {
                return;
            }

            for (int i = 0; i < Roll.Conditions.Count; i++)
            {
                RollCondition Condition = Roll.Conditions[i];
                if (!ConditionMatches(Condition.When, Degree))
                {
                    continue;
                }

                if (Condition.Scope == "Sight")
                {
                    Target.SeesConcealed = true;
                }

                if (Condition.Name == "Frightened")
                {
                    if (Condition.Value > Target.Frightened)
                    {
                        Target.Frightened = Condition.Value;
                    }

                    if (Condition.Floor > Target.FrightenedFloor)
                    {
                        Target.FrightenedFloor = Condition.Floor;
                    }
                }
                else if (Condition.Name == "Enfeebled")
                {
                    if (Condition.Value > Target.Enfeebled)
                    {
                        Target.Enfeebled = Condition.Value;
                    }
                }
                else if (Condition.Name == "Off-guard")
                {
                    Target.OffGuard = true;
                    Target.OffGuardFrom = Source;
                }
                else if (Condition.Name == "Prone")
                {
                    Target.Prone = true;
                }
                else if (Condition.Name == "Grabbed")
                {
                    Target.Grabbed = true;
                }
                else if (Condition.Name == "Concealed")
                {
                    Target.Concealed = true;
                }
                else if (Condition.Name == "Muzzled")
                {
                    Target.Muzzled = true;
                }
                else if (Condition.Name == "Charmed")
                {
                    Target.Charmed = true;
                }
                else if (Condition.Name == "Commanded")
                {
                    Target.Commanded = "Halt";
                }
                else if (Condition.Name == "Slowed")
                {
                    Target.SpeedPenaltyFeet = Condition.Value;
                }
            }
        }

        internal static bool ConditionMatches(string When, StrikeDegree Degree)
        {
            if (When == "Always")
            {
                return true;
            }

            if (When == "Hit")
            {
                return Degree == StrikeDegree.Success || Degree == StrikeDegree.CriticalSuccess;
            }

            if (When == "Failure")
            {
                return Degree == StrikeDegree.Failure || Degree == StrikeDegree.CriticalFailure;
            }

            if (When == "CriticalFailure")
            {
                return Degree == StrikeDegree.CriticalFailure;
            }

            if (When == "Critical")
            {
                return Degree == StrikeDegree.CriticalSuccess;
            }

            return false;
        }

        internal static int SumPackets(StrikeRoll Roll, StrikeDegree Degree, int DieFace)
        {
            int Total = SumWhen(Roll, "Hit", DieFace, Degree == StrikeDegree.CriticalSuccess, 2);
            if (Degree == StrikeDegree.CriticalSuccess)
            {
                Total += SumWhen(Roll, "Critical", DieFace, false, 2);
            }

            return Total;
        }

        internal static int SumWhen(StrikeRoll Roll, string When, int DieFace, bool Double, int BasicMode)
        {
            if (Roll == null)
            {
                return 0;
            }

            int Total = 0;
            for (int i = 0; i < Roll.Packets.Count; i++)
            {
                DamagePacket Packet = Roll.Packets[i];
                if (Packet.When != When)
                {
                    continue;
                }

                if (BasicMode == 1 && !Packet.Basic)
                {
                    continue;
                }

                if (BasicMode == 2 && Packet.Basic)
                {
                    continue;
                }

                Total += PacketAmount(Packet, DieFace, Double);
            }

            return Total;
        }

        internal static int PacketAmount(DamagePacket Packet, int DieFace, bool Double)
        {
            if (Packet == null)
            {
                return 0;
            }

            int Total = Packet.Flat;
            int Face = DieFace;
            if (Face < 1)
            {
                Face = Packet.Faces > 0 ? (RandomHelper.Next(Packet.Faces) + 1) : 0;
            }

            if (Packet.Faces > 0 && Face > Packet.Faces)
            {
                Face = Packet.Faces;
            }

            for (int i = 0; i < Packet.Count; i++)
            {
                Total += Face;
            }

            if (Double)
            {
                Total *= 2;
            }

            return Total;
        }

        internal static int BasicAmount(int Amount, StrikeDegree Degree)
        {
            if (Degree == StrikeDegree.CriticalSuccess)
            {
                return 0;
            }

            if (Degree == StrikeDegree.Success)
            {
                return Amount / 2;
            }

            if (Degree == StrikeDegree.CriticalFailure)
            {
                return Amount * 2;
            }

            return Amount;
        }

        internal static bool HasWhen(StrikeRoll Roll, string When)
        {
            if (Roll == null)
            {
                return false;
            }

            for (int i = 0; i < Roll.Packets.Count; i++)
            {
                if (Roll.Packets[i].When == When)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool HasBasic(StrikeRoll Roll)
        {
            for (int i = 0; i < Roll.Packets.Count; i++)
            {
                if (Roll.Packets[i].Basic)
                {
                    return true;
                }
            }

            return false;
        }

        internal static int ApplyResistance(PlayerCharacter Target, string Type, int Damage)
        {
            if (Target == null || Damage <= 0)
            {
                return Damage;
            }

            StrikeRoll Roll = new StrikeRoll();
            Roll.Trigger = Type;
            RunTraits(Target.Feats, Roll);
            if (Roll.GrantResistance > 0 && Roll.ResistanceType == Type && Target.ResistanceFire < Roll.GrantResistance)
            {
                Target.ResistanceFire = Roll.GrantResistance;
            }

            if (Type == "Fire" && Target.ResistanceFire > 0)
            {
                Damage -= Target.ResistanceFire;
            }

            if (Damage < 0)
            {
                return 0;
            }

            return Damage;
        }

        internal static void TryPersistentRecovery(PlayerCharacter Who, string Types, int Flat)
        {
            if (Who == null)
            {
                return;
            }

            int Check = Flat;
            if (Check < 1 || Check > 20)
            {
                Check = RandomHelper.Next(20) + 1;
            }

            if (Check < 15)
            {
                return;
            }

            for (int i = Who.Persistent.Count - 1; i >= 0; i--)
            {
                if (TypeListed(Types, Who.Persistent[i].Type))
                {
                    Who.Persistent.RemoveAt(i);
                }
            }
        }

        internal static bool TypeListed(string Types, string Type)
        {
            if (string.IsNullOrEmpty(Types) || string.IsNullOrEmpty(Type))
            {
                return false;
            }

            string[] Parts = Types.Split(',');
            for (int i = 0; i < Parts.Length; i++)
            {
                if (Parts[i].Trim() == Type)
                {
                    return true;
                }
            }

            return false;
        }

        internal static void RememberPersistent(PlayerCharacter Target, StrikeRoll Roll, bool Attach)
        {
            if (!Attach || Target == null || Roll == null)
            {
                return;
            }

            for (int i = 0; i < Roll.Packets.Count; i++)
            {
                DamagePacket Packet = Roll.Packets[i];
                if (Packet.When != "Persistent")
                {
                    continue;
                }

                int Score = Packet.Count * (Packet.Faces < 1 ? 1 : Packet.Faces) + Packet.Flat;
                int Found = -1;
                for (int p = 0; p < Target.Persistent.Count; p++)
                {
                    if (Target.Persistent[p].Type == Packet.Type)
                    {
                        Found = p;
                        break;
                    }
                }

                if (Found >= 0)
                {
                    DamagePacket Old = Target.Persistent[Found];
                    int OldScore = Old.Count * (Old.Faces < 1 ? 1 : Old.Faces) + Old.Flat;
                    if (Score <= OldScore)
                    {
                        continue;
                    }

                    Target.Persistent.RemoveAt(Found);
                }

                DamagePacket Copy = new DamagePacket();
                Copy.Count = Packet.Count;
                Copy.Faces = Packet.Faces;
                Copy.Flat = Packet.Flat;
                Copy.Type = Packet.Type;
                Copy.When = "Persistent";
                Target.Persistent.Add(Copy);
            }
        }

        internal static void ApplyHurt(PlayerCharacter Target, int Damage)
        {
            if (Target == null || Damage <= 0)
            {
                return;
            }

            int Left = Damage;
            if (Target.TempHP > 0)
            {
                int Soak = Target.TempHP < Left ? Target.TempHP : Left;
                Target.TempHP -= Soak;
                Left -= Soak;
            }

            Target.CurrentHP -= Left;
            if (Target.CurrentHP < 0)
            {
                Target.CurrentHP = 0;
            }
        }

        internal static void RunDamage(List<Trait> Traits, StrikeRoll Roll)
        {
            if (Traits == null)
            {
                return;
            }

            for (int i = 0; i < Traits.Count; i++)
            {
                Trait Trait = Traits[i];
                if (Trait == null || Trait.ListEffect == null)
                {
                    continue;
                }

                for (int e = 0; e < Trait.ListEffect.Count; e++)
                {
                    if (Trait.ListEffect[e] != null)
                    {
                        Trait.ListEffect[e].AlterDamage(Roll);
                    }
                }
            }
        }

        internal static void RunTraits(List<Trait> Traits, StrikeRoll Roll)
        {
            if (Traits == null)
            {
                return;
            }

            for (int i = 0; i < Traits.Count; i++)
            {
                Trait Trait = Traits[i];
                if (Trait == null || Trait.ListEffect == null)
                {
                    continue;
                }

                for (int e = 0; e < Trait.ListEffect.Count; e++)
                {
                    if (Trait.ListEffect[e] != null)
                    {
                        Trait.ListEffect[e].AlterAttack(Roll);
                    }
                }
            }
        }

        internal static SkirmishSide Opposite(SkirmishSide Side)
        {
            return Side == SkirmishSide.Hero ? SkirmishSide.Monster : SkirmishSide.Hero;
        }

        internal static bool Named(PlayerCharacter Who, string Name)
        {
            if (Who == null || Who.Feats == null)
            {
                return false;
            }

            for (int i = 0; i < Who.Feats.Count; i++)
            {
                if (Who.Feats[i] != null && Who.Feats[i].Name == Name)
                {
                    return true;
                }
            }

            return false;
        }

        internal static Weapon CopyWeapon(Weapon Source)
        {
            if (Source == null)
            {
                return null;
            }

            Weapon Copy = new Weapon();
            Copy.Name = Source.Name;
            Copy.Damage = Source.Damage;
            Copy.DamageType = Source.DamageType;
            Copy.WeaponType = Source.WeaponType;
            Copy.GroupName = Source.GroupName;
            Copy.HandType = Source.HandType;
            Copy.FarRange = Source.FarRange;
            Copy.Reload = Source.Reload;
            Copy.ListTraits = Source.ListTraits;
            return Copy;
        }

        internal static List<Weapon> WeaponsOf(PlayerCharacter Who)
        {
            List<Weapon> Hands = new List<Weapon>();
            if (Who != null && Who.ListWeapon != null && Who.ListWeapon.Count > 0)
            {
                Hands.AddRange(Who.ListWeapon);
                return Hands;
            }

            if (Who != null && Who.Weapon != null)
            {
                Hands.Add(Who.Weapon);
            }

            if (Who != null && Who.OtherWeapon != null && Who.OtherWeapon != Who.Weapon)
            {
                Hands.Add(Who.OtherWeapon);
            }

            return Hands;
        }

        internal static string DegreeName(StrikeDegree Degree)
        {
            if (Degree == StrikeDegree.CriticalSuccess)
            {
                return "critical success";
            }

            if (Degree == StrikeDegree.Success)
            {
                return "success";
            }

            if (Degree == StrikeDegree.CriticalFailure)
            {
                return "critical failure";
            }

            return "failure";
        }

    }
}
