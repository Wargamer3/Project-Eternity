using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Strike
    {
        readonly Skirmish Field;

        public Strike(Skirmish Field)
        {
            this.Field = Field;
        }

        public int Make(PlayerCharacter Actor, PlayerCharacter Target, Weapon Attack, int Natural, int DieFace)
        {
            return Make(Actor, Target, Attack, Natural, DieFace, true, "", false);
        }

        public int Make(PlayerCharacter Actor, PlayerCharacter Target, Weapon Attack, int Natural, int DieFace, bool Spend, string Activity, bool Hold)
        {
            Field.LastDamage = 0;
            Field.LastPenalty = 0;
            if (Actor == null || Target == null || Attack == null || Actor.CurrentHP <= 0 || Target.CurrentHP <= 0)
            {
                Field.LastResult = "No target.";
                return 0;
            }

            if (Spend && Actor.ActionsLeft < 1)
            {
                Field.LastResult = "No actions left.";
                return 0;
            }

            if (Attack.Name == "Jaws" && Actor.Muzzled)
            {
                Field.LastResult = "Muzzled.";
                return 0;
            }

            bool Ranged = Attack.WeaponType == Weapon.WeaponTypes.Range;
            float ReachFeet = Attack.FarRange / PracticeMath.FeetToMeters;
            float Distance = Vector3.Distance(Actor.Position, Target.Position);
            if (!Ranged && Distance > PracticeMath.ToUnits(ReachFeet + Actor.ReachBonusFeet))
            {
                Field.LastResult = "Out of reach.";
                return 0;
            }

            if (Ranged && Attack.Reload > 0 && !Actor.Loaded)
            {
                Field.LastResult = "Not loaded.";
                return 0;
            }

            if (Spend)
            {
                Actor.ActionsLeft--;
            }

            PlayerCharacter Who = Actor;
            if (Actor.Form != null && Attack == Actor.Form.Strike && Actor.Form.Who != null)
            {
                Who = Actor.Form.Who;
            }

            StrikeRoll Roll = new StrikeRoll();
            Roll.Who = Who;
            Roll.Weapon = Attack;
            Roll.Activity = Activity;
            Roll.Trigger = "Strike";
            Roll.TargetTrait = Target.CreatureTrait;
            Roll.StrikesAlreadyMade = Actor.StrikesThisTurn;
            Roll.OffGuard = Field.TargetOffGuard(Actor, Target, Ranged);
            if (Who != null)
            {
                StrikeRoll Prepared = PracticeMath.Prepare(Who, Attack, Actor.StrikesThisTurn, Actor.StrikesThisTurn, false, Distance);
                Roll.AttackAbility = Prepared.AttackAbility;
                Roll.DamageAbility = Prepared.DamageAbility;
                Roll.DiceCount = Prepared.DiceCount;
                Roll.DiceFaces = Prepared.DiceFaces;
                Roll.FlatBonus = Prepared.FlatBonus;
                Roll.UntypedPenalty = Prepared.UntypedPenalty;
            }
            else
            {
                DamageExpression Dice = PracticeMath.ParseDamage(Attack.Damage);
                Roll.DiceCount = Dice.Count;
                Roll.DiceFaces = Dice.Faces;
                Roll.FlatBonus = Dice.Bonus;
                Roll.UntypedPenalty = PracticeMath.MapPenalty(Actor.StrikesThisTurn);
            }

            FightRoll.RunTraits(Attack.ListTraits, Roll);
            FightRoll.RunTraits(Actor.Feats, Roll);
            if (Actor.Enfeebled > 0)
            {
                Roll.UntypedPenalty += Actor.Enfeebled;
            }

            if (Actor.Frightened > 0)
            {
                Roll.UntypedPenalty += Actor.Frightened;
            }

            if (Field.Actions.HasActivity(Actor, "Far Shot"))
            {
                Actor.FarShot = true;
            }

            if (Ranged && ReachFeet > 0f)
            {
                int Band = PracticeMath.RangeCircumstancePenalty(Distance, ReachFeet, Actor.FarShot);
                Roll.RaiseCircumstancePenalty(Band);
            }

            if (Roll.AimPenalty > 0)
            {
                Roll.RaiseCircumstancePenalty(Roll.AimPenalty);
            }

            int Defense = Field.OfferDefense(Target, Ranged ? "Ranged" : "Targeted");
            int Armor = Field.ArmorOf(Target, Roll.IgnoreCircumstance, Defense);
            int Bonus = 0;
            if (Who != null)
            {
                Bonus = PracticeMath.AttackBonus(Who, Attack, Roll.AttackAbility);
            }

            Bonus += Actor.DayAttackBonus;

            string Group = string.IsNullOrEmpty(Attack.GroupName) ? Attack.WeaponGroup.ToString() : Attack.GroupName;
            if (!string.IsNullOrEmpty(Actor.NextDamageGroup) && Group == Actor.NextDamageGroup)
            {
                Roll.FlatBonus += Actor.NextDamage;
                Actor.NextDamage = 0;
            }

            Roll.FlatBonus += Roll.AimDamage;
            Field.LastPenalty = Roll.UntypedPenalty;
            StrikeDegree Degree = PracticeMath.Degree(Natural, Bonus + Roll.CircumstanceBonus, Field.LastPenalty + Roll.CircumstancePenalty, Armor);
            bool Hit = Degree == StrikeDegree.Success || Degree == StrikeDegree.CriticalSuccess;
            if (Hit && Target.Images > 0)
            {
                Target.Images--;
                Hit = false;
                Degree = StrikeDegree.Failure;
                Field.LastResult = "An image takes the blow.";
            }

            Roll.Critical = Degree == StrikeDegree.CriticalSuccess;
            Roll.Hit = Hit;
            int Damage = 0;
            if (Hit)
            {
                FightRoll.RunDamage(Attack.ListTraits, Roll);
                FightRoll.RunDamage(Actor.Feats, Roll);
                if (Roll.Packets.Count > 0)
                {
                    Damage = FightRoll.SumPackets(Roll, Degree, DieFace);
                }
                else
                {
                    int[] Faces = DieFace > 0 ? new int[] { DieFace } : null;
                    Damage = PracticeMath.ResolveDamage(Roll, Faces, null);
                }
            }
            else if (Degree == StrikeDegree.Failure)
            {
                Damage = FightRoll.SumWhen(Roll, "Failure", DieFace, false, 2);
            }

            Damage = FightRoll.ApplyResistance(Target, Attack.DamageType, Damage);
            Damage = Field.OfferBlock(Target, Damage, Actor);
            FightRoll.ApplyHurt(Target, Damage);
            FightRoll.ApplyConditions(Target, Roll, Degree, Actor);
            if (Ranged && Attack.Reload > 0)
            {
                Actor.Loaded = false;
            }

            if (!Hold && !Roll.HoldPenalty)
            {
                Actor.StrikesThisTurn++;
            }

            Actor.Attacked = true;
            if (Degree == StrikeDegree.CriticalFailure)
            {
                Field.OfferStrikeBack(Target, Actor, DieFace);
            }

            Field.LastDamage = Damage;
            Field.LastResult = Actor.Name + " " + Attack.Name + " vs " + Target.Name + ". " + FightRoll.DegreeName(Degree) + ". " + Damage + " damage. " + Target.CurrentHP + " HP.";
            return Damage;
        }

    }
}
