using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class PracticeMath
    {
        public const float FeetToMeters = 0.3048f;
        public const float UnitsPerMeter = 32f;

        public static int MapPenalty(int StrikesThisTurn)
        {
            if (StrikesThisTurn <= 0)
            {
                return 0;
            }

            if (StrikesThisTurn == 1)
            {
                return 5;
            }

            return 10;
        }

        public static int RankBonus(Proficiency.ProficiencyRanks Rank)
        {
            switch (Rank)
            {
                case Proficiency.ProficiencyRanks.Trained:
                    return 2;

                case Proficiency.ProficiencyRanks.Expert:
                    return 4;

                case Proficiency.ProficiencyRanks.Master:
                    return 6;

                case Proficiency.ProficiencyRanks.Legendary:
                    return 8;
            }

            return 0;
        }

        public static int LandSpeedFeet(PlayerCharacter Who)
        {
            if (Who == null || Who.Ancestry == null || Who.Ancestry.Speed == 0)
            {
                return 25;
            }

            return Who.Ancestry.Speed;
        }

        public static float StrideMeters(PlayerCharacter Who)
        {
            return LandSpeedFeet(Who) * FeetToMeters;
        }

        public static float StrideUnits(PlayerCharacter Who)
        {
            return StrideMeters(Who) * UnitsPerMeter;
        }

        public static bool StrideFits(float PathMeters, float BudgetMeters)
        {
            return PathMeters <= BudgetMeters + 0.001f;
        }

        public static float PathLength(IList<AutomatedAction> Steps)
        {
            float Total = 0f;
            if (Steps == null)
            {
                return Total;
            }

            for (int i = 0; i < Steps.Count; i++)
            {
                FollowPath Path = Steps[i] as FollowPath;
                if (Path == null || Path.Segments == null)
                {
                    continue;
                }

                for (int s = 0; s < Path.Segments.Count; s++)
                {
                    Total += LengthOf(Path.Segments[s].Polyline);
                }
            }

            return Total;
        }

        public static float LengthOf(IList<Vector3> Polyline)
        {
            float Total = 0f;
            if (Polyline == null)
            {
                return Total;
            }

            for (int i = 1; i < Polyline.Count; i++)
            {
                Total += Vector3.Distance(Polyline[i - 1], Polyline[i]);
            }

            return Total;
        }

        public static int AttackBonus(PlayerCharacter Who, Weapon Weapon)
        {
            return AttackBonus(Who, Weapon, AbilityModifier(Who, Weapon));
        }

        public static int AttackBonus(PlayerCharacter Who, Weapon Weapon, int Ability)
        {
            if (Who == null)
            {
                return Ability;
            }

            Proficiency.ProficiencyRanks Rank = BestRank(Who, Weapon);
            if (Rank == Proficiency.ProficiencyRanks.None)
            {
                return Ability;
            }

            return Ability + Who.Level + RankBonus(Rank);
        }

        public static int AbilityModifier(PlayerCharacter Who, Weapon Weapon)
        {
            if (Who == null)
            {
                return 0;
            }

            if (Weapon != null && Weapon.WeaponType == Weapon.WeaponTypes.Range)
            {
                return Who.DEX;
            }

            return Who.STR;
        }

        public static StrikeDegree Degree(int Natural, int Bonus, int Penalty, int ArmorClass)
        {
            int Delta = Natural + Bonus - Penalty - ArmorClass;
            StrikeDegree Result;
            if (Delta >= 10)
            {
                Result = StrikeDegree.CriticalSuccess;
            }
            else if (Delta >= 0)
            {
                Result = StrikeDegree.Success;
            }
            else if (Delta <= -10)
            {
                Result = StrikeDegree.CriticalFailure;
            }
            else
            {
                Result = StrikeDegree.Failure;
            }

            if (Natural == 20)
            {
                return Improve(Result);
            }

            if (Natural == 1)
            {
                return Worsen(Result);
            }

            return Result;
        }

        public static float WeaponReach(Weapon Weapon)
        {
            float Meters = 2f;
            if (Weapon != null && Weapon.FarRange > 0f)
            {
                Meters = Weapon.FarRange;
            }

            return Meters * UnitsPerMeter;
        }

        public static float DistanceToEdge(Vector3 Who, FurnitureItem Box)
        {
            if (Box == null)
            {
                return float.MaxValue;
            }

            float Along;
            float Across;
            Box.MapToLocal(Who.X, Who.Y, out Along, out Across);
            float HalfW = Box.Width * 0.5f;
            float HalfL = Box.Length * 0.5f;
            float DX = 0f;
            if (Along > HalfW)
            {
                DX = Along - HalfW;
            }
            else if (Along < -HalfW)
            {
                DX = Along + HalfW;
            }

            float DY = 0f;
            if (Across > HalfL)
            {
                DY = Across - HalfL;
            }
            else if (Across < -HalfL)
            {
                DY = Across + HalfL;
            }

            return (float)System.Math.Sqrt(DX * DX + DY * DY);
        }

        public static bool InReach(PlayerCharacter Who, FurnitureItem Dummy, Weapon Weapon)
        {
            if (Who == null)
            {
                return false;
            }

            return DistanceToEdge(Who.Position, Dummy) <= WeaponReach(Weapon);
        }

        public static bool Busy(PlayerCharacter Who)
        {
            if (Who == null || Who.ListAIObjective == null)
            {
                return false;
            }

            for (int i = 0; i < Who.ListAIObjective.Count; i++)
            {
                AIObjective Order = Who.ListAIObjective[i];
                if (Order != null && Order.ListRemainingStep != null && Order.ListRemainingStep.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static DamageExpression ParseDamage(string Text)
        {
            DamageExpression Made;
            Made.Count = 1;
            Made.Faces = 4;
            Made.Bonus = 0;
            if (string.IsNullOrEmpty(Text))
            {
                return Made;
            }

            string Body = Text.Replace(" ", "");
            if (Body.Length == 0)
            {
                return Made;
            }

            int SignAt = Body.IndexOf('+');
            if (SignAt < 0)
            {
                SignAt = Body.IndexOf('-');
            }

            if (SignAt > 0)
            {
                int Bonus;
                if (int.TryParse(Body.Substring(SignAt + 1), out Bonus))
                {
                    Made.Bonus = Body[SignAt] == '-' ? -Bonus : Bonus;
                }

                Body = Body.Substring(0, SignAt);
            }

            int Dice = Body.IndexOf('d');
            if (Dice < 0)
            {
                int Flat;
                if (int.TryParse(Body, out Flat) && Flat > 0)
                {
                    Made.Count = 0;
                    Made.Faces = 0;
                    Made.Bonus += Flat;
                }

                return Made;
            }

            if (Dice > 0)
            {
                int Count;
                if (int.TryParse(Body.Substring(0, Dice), out Count) && Count > 0)
                {
                    Made.Count = Count;
                }
            }

            int Faces;
            if (int.TryParse(Body.Substring(Dice + 1), out Faces) && Faces > 0)
            {
                Made.Faces = Faces;
            }

            return Made;
        }

        public static int Damage(string Text, int Ability, bool AddAbility, bool Critical, int[] Rolls)
        {
            DamageExpression Dice = ParseDamage(Text);
            int Total = Dice.Bonus;
            for (int i = 0; i < Dice.Count; i++)
            {
                int Face;
                if (Rolls != null && i < Rolls.Length)
                {
                    Face = Rolls[i];
                }
                else if (Dice.Faces < 1)
                {
                    Face = 0;
                }
                else
                {
                    Face = RandomHelper.Next(Dice.Faces) + 1;
                }

                if (Dice.Faces > 0)
                {
                    if (Face < 1)
                    {
                        Face = 1;
                    }
                    else if (Face > Dice.Faces)
                    {
                        Face = Dice.Faces;
                    }
                }

                Total += Face;
            }

            if (AddAbility)
            {
                Total += Ability;
            }

            if (Critical)
            {
                Total *= 2;
            }

            if (Total < 0)
            {
                return 0;
            }

            return Total;
        }

        public static StrikeRoll Prepare(PlayerCharacter Who, Weapon Weapon, int StrikesAlreadyMade, int StrikesWithThisWeapon, bool MissedWithThisWeapon, float Distance)
        {
            StrikeRoll Roll = new StrikeRoll();
            Roll.Who = Who;
            Roll.Weapon = Weapon;
            Roll.StrikesAlreadyMade = StrikesAlreadyMade;
            Roll.StrikesWithThisWeapon = StrikesWithThisWeapon;
            Roll.MissedWithThisWeapon = MissedWithThisWeapon;
            Roll.Distance = Distance;
            Roll.UntypedPenalty = MapPenalty(StrikesAlreadyMade);
            bool Melee = Weapon == null || Weapon.WeaponType != Weapon.WeaponTypes.Range;
            int Strength = Who == null ? 0 : Who.STR;
            int Dexterity = Who == null ? 0 : Who.DEX;
            Roll.AttackAbility = Melee ? Strength : Dexterity;
            Roll.DamageAbility = Melee ? Strength : 0;
            DamageExpression Dice = ParseDamage(Weapon == null ? null : Weapon.Damage);
            Roll.DiceCount = Dice.Count;
            Roll.DiceFaces = Dice.Faces;
            Roll.FlatBonus = Dice.Bonus;
            return Roll;
        }

        public static void AlterAttack(StrikeRoll Roll)
        {
            EachEffect(Roll, true);
        }

        public static void AlterDamage(StrikeRoll Roll)
        {
            EachEffect(Roll, false);
        }

        public static int ResolveDamage(StrikeRoll Roll, int[] Rolls, int[] ExtraRolls)
        {
            if (Roll == null)
            {
                return 0;
            }

            int Total = Roll.FlatBonus + Roll.DamageAbility + Roll.ExtraDamage;
            for (int i = 0; i < Roll.DiceCount; i++)
            {
                Total += FaceOf(Roll.DiceFaces, i, Rolls);
            }

            if (Roll.Critical)
            {
                Total *= 2;
            }

            for (int i = 0; i < Roll.ExtraDiceCount; i++)
            {
                Total += FaceOf(Roll.ExtraFaces, i, ExtraRolls);
            }

            if (Total < 0)
            {
                return 0;
            }

            return Total;
        }

        public static string Equation(int Natural, int Bonus, int Penalty, int CircumstanceBonus, int CircumstancePenalty)
        {
            string Text = "d20 " + Natural + " + " + Bonus;
            if (CircumstanceBonus != 0)
            {
                Text += " + " + CircumstanceBonus;
            }

            Text += " - " + Penalty;
            if (CircumstancePenalty != 0)
            {
                Text += " - " + CircumstancePenalty;
            }

            int Total = Natural + Bonus + CircumstanceBonus - Penalty - CircumstancePenalty;
            return Text + " = " + Total;
        }

        public static string Describe(int Natural, int Bonus, int Penalty, int ArmorClass, StrikeDegree Degree, int Damage, int RemainingHP)
        {
            return Describe(Natural, Bonus, Penalty, ArmorClass, Degree, Damage, RemainingHP, 0, 0);
        }

        public static string Describe(int Natural, int Bonus, int Penalty, int ArmorClass, StrikeDegree Degree, int Damage, int RemainingHP, int CircumstanceBonus, int CircumstancePenalty)
        {
            return Describe(Natural, Bonus, Penalty, ArmorClass, Degree, Damage, RemainingHP, CircumstanceBonus, CircumstancePenalty, null);
        }

        public static string Describe(int Natural, int Bonus, int Penalty, int ArmorClass, StrikeDegree Degree, int Damage, int RemainingHP, int CircumstanceBonus, int CircumstancePenalty, string DamageType)
        {
            string Kind = "damage";
            if (!string.IsNullOrEmpty(DamageType))
            {
                Kind = DamageType + " damage";
            }

            string EquationText = Equation(Natural, Bonus, Penalty, CircumstanceBonus, CircumstancePenalty);
            return EquationText + " vs AC " + ArmorClass + ". " + DegreeName(Degree) + ". " + Damage + " " + Kind + ". Dummy " + RemainingHP + " HP.";
        }

        public static int LevelHitPoints(int Ancestry, int ClassHitPoints, int Constitution, int Level)
        {
            if (Level < 1)
            {
                Level = 1;
            }

            return Ancestry + Level * (ClassHitPoints + Constitution);
        }

        public static int GearArmorClass(int Dexterity, int DexCap, int ItemBonus, int ProficiencyBonus, int Circumstance)
        {
            int Dex = Dexterity;
            if (DexCap >= 0 && Dex > DexCap)
            {
                Dex = DexCap;
            }

            int Total = 10 + Dex + ItemBonus + ProficiencyBonus + Circumstance;
            if (Total < 0)
            {
                return 0;
            }

            return Total;
        }

        public static float ToUnits(float Feet)
        {
            return Feet * FeetToMeters * UnitsPerMeter;
        }

        public static int RangeCircumstancePenalty(float DistanceUnits, float IncrementFeet, bool FarShot)
        {
            if (IncrementFeet <= 0f || DistanceUnits <= 0f)
            {
                return 0;
            }

            float Increment = ToUnits(IncrementFeet);
            if (FarShot)
            {
                Increment *= 2f;
            }

            if (Increment <= 0f || DistanceUnits <= Increment)
            {
                return 0;
            }

            int Bands = (int)((DistanceUnits - 0.001f) / Increment);
            return Bands * 2;
        }

        public static int CantripRank(int Level)
        {
            if (Level < 1)
            {
                return 1;
            }

            return (Level + 1) / 2;
        }

        public static int ArmorClass(PlayerCharacter Who, int Circumstance)
        {
            int Dexterity = Who == null ? 0 : Who.DEX;
            int Total = 10 + Dexterity + Circumstance;
            if (Total < 0)
            {
                return 0;
            }

            return Total;
        }

        public static string Incoming(int Natural, int Bonus, int ArmorClass, StrikeDegree Degree, int Damage, string DamageType)
        {
            string Kind = "damage";
            if (!string.IsNullOrEmpty(DamageType))
            {
                Kind = DamageType + " damage";
            }

            string EquationText = Equation(Natural, Bonus, 0, 0, 0);
            return EquationText + " vs AC " + ArmorClass + ". " + DegreeName(Degree) + ". " + Damage + " " + Kind + ". You take 0.";
        }

        static string DegreeName(StrikeDegree Degree)
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

        public static string VersatileOffer(Weapon Weapon)
        {
            if (Weapon == null)
            {
                return null;
            }

            Weapon.EnsureTraits();
            StrikeRoll Roll = Prepare(null, Weapon, 0, 0, false, 0f);
            AlterAttack(Roll);
            return Roll.VersatileType;
        }

        static void EachEffect(StrikeRoll Roll, bool Attack)
        {
            if (Roll == null || Roll.Weapon == null || Roll.Weapon.ListTraits == null)
            {
                return;
            }

            for (int i = 0; i < Roll.Weapon.ListTraits.Count; i++)
            {
                Trait Trait = Roll.Weapon.ListTraits[i];
                if (Trait == null || Trait.ListEffect == null)
                {
                    continue;
                }

                for (int e = 0; e < Trait.ListEffect.Count; e++)
                {
                    TraitEffect Effect = Trait.ListEffect[e];
                    if (Effect == null)
                    {
                        continue;
                    }

                    if (Attack)
                    {
                        Effect.AlterAttack(Roll);
                    }
                    else
                    {
                        Effect.AlterDamage(Roll);
                    }
                }
            }
        }

        static int FaceOf(int Faces, int Index, int[] Rolls)
        {
            int Face;
            if (Rolls != null && Index < Rolls.Length)
            {
                Face = Rolls[Index];
            }
            else if (Faces < 1)
            {
                Face = 0;
            }
            else
            {
                Face = RandomHelper.Next(Faces) + 1;
            }

            if (Faces > 0)
            {
                if (Face < 1)
                {
                    Face = 1;
                }
                else if (Face > Faces)
                {
                    Face = Faces;
                }
            }

            return Face;
        }

        static StrikeDegree Improve(StrikeDegree Degree)
        {
            if (Degree == StrikeDegree.CriticalSuccess)
            {
                return Degree;
            }

            return (StrikeDegree)((int)Degree + 1);
        }

        static StrikeDegree Worsen(StrikeDegree Degree)
        {
            if (Degree == StrikeDegree.CriticalFailure)
            {
                return Degree;
            }

            return (StrikeDegree)((int)Degree - 1);
        }

        static Proficiency.ProficiencyRanks BestRank(PlayerCharacter Who, Weapon Weapon)
        {
            Proficiency.ProficiencyRanks Best = Proficiency.ProficiencyRanks.None;
            if (Who == null || Who.DicProficiencyLevelByName == null || Weapon == null)
            {
                return Best;
            }

            foreach (KeyValuePair<string, ProficiencyLink> Pair in Who.DicProficiencyLevelByName)
            {
                if (!Matches(Pair.Key, Pair.Value, Weapon))
                {
                    continue;
                }

                if (Pair.Value != null && Pair.Value.ProficiencyRank > Best)
                {
                    Best = Pair.Value.ProficiencyRank;
                }
            }

            return Best;
        }

        static bool Matches(string Path, ProficiencyLink Link, Weapon Weapon)
        {
            string Group = Weapon.WeaponGroup.ToString();
            if (!string.IsNullOrEmpty(Weapon.GroupName))
            {
                Group = Weapon.GroupName;
            }
            if (NamesMatch(Path, Weapon.Name) || NamesMatch(Path, Group))
            {
                return true;
            }

            if (Link == null || Link.LinkedProficiency == null)
            {
                return false;
            }

            return NamesMatch(Link.LinkedProficiency.Name, Weapon.Name) || NamesMatch(Link.LinkedProficiency.Name, Group);
        }

        static bool NamesMatch(string Have, string Want)
        {
            if (string.IsNullOrEmpty(Have) || string.IsNullOrEmpty(Want))
            {
                return false;
            }

            if (string.Equals(Have, Want, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            string File = Have.Replace('\\', '/');
            int Slash = File.LastIndexOf('/');
            string Tail = Slash >= 0 ? File.Substring(Slash + 1) : File;
            return string.Equals(Tail, Want, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
