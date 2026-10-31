using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Spellcast
    {
        readonly Skirmish Field;

        public Spellcast(Skirmish Field)
        {
            this.Field = Field;
        }

        public static PlayerCharacter Unlisted(string Name)
        {
            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Name = Name;
            Who.Level = 1;
            CharacterAncestry Ancestry = new CharacterAncestry();
            Ancestry.Name = Name;
            Ancestry.BaseHP = 6;
            Ancestry.Speed = 25;
            Trait Kind = new Trait();
            Kind.Name = Name;
            Ancestry.ListTraits.Add(Kind);
            Who.Ancestry = Ancestry;
            CharacterClass Class = new CharacterClass();
            Class.Name = Name;
            Class.BaseHP = 2;
            Who.Class = Class;
            Weapon Strike = new Weapon();
            Strike.Name = "Strike";
            Strike.Damage = "1d6";
            Strike.ListTraits = new List<Trait>();
            Strike.FarRange = 5f * PracticeMath.FeetToMeters;
            Who.Equip(Strike);
            return Who.PrepareForFight();
        }

        public int Cast(PlayerCharacter Caster, SpellCard Card, PlayerCharacter Target, int Natural, int Rank, int DieFace)
        {
            return Cast(Caster, Card, Target, null, Natural, Rank, DieFace);
        }

        public int Cast(PlayerCharacter Caster, SpellCard Card, PlayerCharacter Target, PlayerCharacter Second, int Natural, int Rank, int DieFace)
        {
            Field.LastDamage = 0;
            if (Caster == null || Card == null || Card.Spell == null)
            {
                return 0;
            }

            int Use = Rank < Card.BaseRank ? Card.BaseRank : Rank;
            if (Card.Cantrip)
            {
                Use = PracticeMath.CantripRank(Caster.Level);
            }

            if (Caster.ActionsLeft < Card.Actions)
            {
                Field.LastResult = "No actions left.";
                return 0;
            }

            if (!Card.Cantrip)
            {
                if (Use < 0 || Use >= Caster.Slots.Length || Caster.Slots[Use] <= 0)
                {
                    Field.LastResult = "No slot.";
                    return 0;
                }
            }

            StrikeRoll Roll = new StrikeRoll();
            Roll.Who = Caster;
            Roll.SpellRank = Use;
            Roll.Trigger = "Cast";
            Roll.TargetTrait = Target == null ? "" : Target.CreatureTrait;
            FightRoll.RunTraits(new List<Trait> { Card.Spell }, Roll);
            Caster.ActionsLeft -= Card.Actions;
            if (!Card.Cantrip)
            {
                Caster.Slots[Use]--;
            }

            if (Roll.ClearDying && Target != null)
            {
                Target.Dying = false;
            }

            if (Roll.LeapFeet > 0)
            {
                Caster.LeapFeet += Roll.LeapFeet;
            }

            if (!string.IsNullOrEmpty(Roll.SummonEntry))
            {
                Summon(Caster, Roll.SummonEntry, Target);
            }

            if (Roll.Sacrifice)
            {
                Sacrifice(Caster, Target, Roll, DieFace);
            }

            if (!string.IsNullOrEmpty(Roll.FormName))
            {
                for (int i = 0; i < Field.Forms.Count; i++)
                {
                    if (Field.Forms[i].Name == Roll.FormName)
                    {
                        Caster.Form = Field.Forms[i];
                    }
                }
            }

            ApplyGrants(Caster, Target, Roll);
            if (Roll.IsZone)
            {
                SkirmishZone Zone = new SkirmishZone();
                Zone.Owner = Caster;
                Zone.Position = Target == null ? Caster.Position : Target.Position;
                Zone.Feet = Roll.ZoneFeet;
                Zone.Effect = Card.Spell;
                Zone.Rank = Use;
                Zone.HalveHeal = Roll.HalveHeal;
                Zone.Renewed = true;
                Field.Zones.Add(Zone);
                AffectZone(Zone, DieFace, Natural, false);
            }

            int Damage = 0;
            if (!string.IsNullOrEmpty(Roll.Save) && Target != null && !Roll.IsZone)
            {
                Damage += ResolveSave(Caster, Target, Roll, Natural, DieFace);
            }

            if (Roll.Automatic && Target != null)
            {
                if (Roll.PerAction > 0)
                {
                    Roll.AutomaticCount = Card.Actions;
                }

                Damage += FightRoll.SumWhen(Roll, "Hit", DieFace, false, 2) * (Roll.AutomaticCount < 1 ? 1 : Roll.AutomaticCount);
                FightRoll.ApplyHurt(Target, Damage);
            }

            if (Roll.IsSpellAttack && Target != null)
            {
                float Range = Roll.RangeFeet <= 0f ? 30f : Roll.RangeFeet;
                int First = SpellStrike(Caster, Target, Range, Roll, Natural, DieFace, Second == null);
                Damage += First;
                if (Second != null && Roll.HoldPenalty && Caster.HandsFree >= 2)
                {
                    Damage += SpellStrike(Caster, Second, Range, Roll, Natural, DieFace, true);
                    Caster.StrikesThisTurn++;
                }
            }

            if (Damage > 0)
            {
                Field.LastDamage = Damage;
            }

            if (Roll.HealAmount > 0 && Target != null)
            {
                int Heal = Roll.HealAmount;
                if (Target.HealHalved)
                {
                    Heal = Heal / 2;
                }

                if (Roll.TemporaryHeal)
                {
                    Target.TempHP += Heal;
                }
                else
                {
                    Target.CurrentHP += Heal;
                    if (Target.CurrentHP > Target.MaxHP)
                    {
                        Target.CurrentHP = Target.MaxHP;
                    }
                }

                Field.LastDamage = Heal;
            }

            if (!string.IsNullOrEmpty(Roll.RecoverPersistent) && Target != null)
            {
                FightRoll.TryPersistentRecovery(Target, Roll.RecoverPersistent, Natural);
            }

            Field.LastResult = Caster.Name + " casts " + Card.Name + ".";
            return Damage;
        }

        public void Sustain(PlayerCharacter Caster, int Natural, int DieFace)
        {
            if (Caster == null || Caster.ActionsLeft < 1)
            {
                return;
            }

            Caster.ActionsLeft--;
            for (int i = 0; i < Field.Zones.Count; i++)
            {
                if (Field.Zones[i].Owner == Caster)
                {
                    Field.Zones[i].Renewed = true;
                    AffectZone(Field.Zones[i], DieFace, Natural, true);
                }
            }
        }

        public void DismissGlee(PlayerCharacter Caster, PlayerCharacter Ally, PlayerCharacter Target, int DieFace)
        {
            if (Caster == null || Ally == null || !Field.SpendReaction(Caster, "Dismiss"))
            {
                return;
            }

            int Heal = 4;
            if (DieFace > 0)
            {
                Heal = DieFace;
            }

            Ally.CurrentHP += Heal;
            if (Target != null)
            {
                Target.FrightenedFloor = 0;
                Target.Frightened = 0;
            }

            Field.LastResult = "Dismiss. " + Heal + " healed.";
        }

        int SpellStrike(PlayerCharacter Caster, PlayerCharacter Target, float RangeFeet, StrikeRoll Roll, int Natural, int DieFace, bool Count)
        {
            float Distance = Vector3.Distance(Caster.Position, Target.Position);
            if (RangeFeet > 0f && Distance > PracticeMath.ToUnits(RangeFeet))
            {
                Field.LastResult = "Out of reach.";
                return 0;
            }

            int Defense = Field.OfferDefense(Target, "Ranged");
            int Armor = Field.ArmorOf(Target, Roll.IgnoreCircumstance, Defense);
            int Bonus = SpellBonus(Caster) - Caster.Frightened;
            StrikeDegree Degree = PracticeMath.Degree(Natural, Bonus + Roll.CircumstanceBonus, Caster.StrikesThisTurn == 0 ? 0 : PracticeMath.MapPenalty(Caster.StrikesThisTurn), Armor);
            Roll.Critical = Degree == StrikeDegree.CriticalSuccess;
            Roll.Hit = Degree == StrikeDegree.Success || Degree == StrikeDegree.CriticalSuccess;
            int Damage = 0;
            if (Roll.Hit)
            {
                Damage = FightRoll.SumPackets(Roll, Degree, DieFace);
            }
            else if (Degree == StrikeDegree.Failure)
            {
                Damage = FightRoll.SumWhen(Roll, "Failure", DieFace, false, 2);
            }

            string Type = Roll.Packets.Count > 0 ? Roll.Packets[0].Type : "";
            Damage = FightRoll.ApplyResistance(Target, Type, Damage);
            FightRoll.ApplyHurt(Target, Damage);
            FightRoll.RememberPersistent(Target, Roll, Degree == StrikeDegree.CriticalSuccess);
            FightRoll.ApplyConditions(Target, Roll, Degree, Caster);
            if (Count)
            {
                Caster.StrikesThisTurn++;
            }

            Field.LastDamage = Damage;
            Field.LastResult = Caster.Name + " spell vs " + Target.Name + ". " + FightRoll.DegreeName(Degree) + ". " + Damage + ".";
            return Damage;
        }

        int ResolveSave(PlayerCharacter Caster, PlayerCharacter Target, StrikeRoll Roll, int Natural, int DieFace)
        {
            int Bonus = SaveBonus(Target, Roll.Save);
            int DC = 10 + SpellBonus(Caster);
            StrikeDegree Degree = PracticeMath.Degree(Natural, Bonus, 0, DC);
            string PacketWhen = "Hit";
            if (Roll.UseSustain && FightRoll.HasWhen(Roll, "Sustain"))
            {
                PacketWhen = "Sustain";
            }

            int Damage = FightRoll.SumWhen(Roll, PacketWhen, DieFace, false, 1);
            if (Roll.Basic || FightRoll.HasBasic(Roll))
            {
                Damage = FightRoll.BasicAmount(Damage, Degree);
            }
            else if (Degree == StrikeDegree.Success || Degree == StrikeDegree.CriticalSuccess)
            {
                Damage = 0;
            }
            else if (Degree == StrikeDegree.CriticalFailure)
            {
                Damage *= 2;
            }

            FightRoll.ApplyHurt(Target, Damage);
            FightRoll.RememberPersistent(Target, Roll, Degree == StrikeDegree.CriticalFailure);
            FightRoll.ApplyConditions(Target, Roll, Degree, Caster);
            Field.LastDamage = Damage;
            Field.LastResult = Target.Name + " " + Roll.Save + " " + FightRoll.DegreeName(Degree) + ". " + Damage + ".";
            return Damage;
        }

        void Summon(PlayerCharacter Caster, string Name, PlayerCharacter Near)
        {
            PlayerCharacter Template = null;
            for (int i = 0; i < Field.Summoned.Count; i++)
            {
                if (Field.Summoned[i].Name == Name || Field.Summoned[i].CreatureTrait == Name)
                {
                    Template = Field.Summoned[i];
                }
            }

            PlayerCharacter Body = Template == null ? Unlisted(Name) : Template.Copy();
            Body.Side = Caster.Side;
            Body.Owner = Caster;
            Body.SkipTurn = true;
            Body.Position = Near == null ? Caster.Position : Near.Position;
            Field.Add(Body);
            Field.LastResult = "Summoned " + Body.Name + ".";
        }

        void Sacrifice(PlayerCharacter Caster, PlayerCharacter Target, StrikeRoll Roll, int DieFace)
        {
            PlayerCharacter Minion = null;
            for (int i = 0; i < Field.Bodies.Count; i++)
            {
                if (Field.Bodies[i].Owner == Caster && Field.Bodies[i].CurrentHP > 0)
                {
                    Minion = Field.Bodies[i];
                }
            }

            if (Minion == null)
            {
                return;
            }

            Minion.CurrentHP = 0;
            if (Target != null)
            {
                int Damage = FightRoll.SumWhen(Roll, "Hit", DieFace, false, 1);
                int Bonus = Target.Reflex;
                StrikeDegree Degree = PracticeMath.Degree(10, Bonus, 0, 10 + SpellBonus(Caster));
                Damage = FightRoll.BasicAmount(Damage, Degree);
                FightRoll.ApplyHurt(Target, Damage);
                Field.LastDamage = Damage;
            }
        }

        void ApplyGrants(PlayerCharacter Caster, PlayerCharacter Target, StrikeRoll Roll)
        {
            PlayerCharacter Who = Target == null ? Caster : Target;
            if (Roll.GrantItem > 0)
            {
                Who.ItemArmor += Roll.GrantItem;
                Who.PrintedArmor = false;
            }

            if (Roll.GrantArmor > 0)
            {
                if (Who.CircumstanceArmor < Roll.GrantArmor)
                {
                    Who.CircumstanceArmor = Roll.GrantArmor;
                }

                Who.RaisedShield = true;
            }

            if (Roll.GrantHardness > 0)
            {
                Who.BlockHardness = Roll.GrantHardness;
                Who.ShieldHardness = Roll.GrantHardness;
            }

            if (Roll.GrantImages > 0)
            {
                Who.Images = Roll.GrantImages;
            }

            if (Roll.GrantResistance > 0 && Roll.ResistanceType == "Fire")
            {
                Who.ResistanceFire = Roll.GrantResistance;
            }

            for (int i = 0; i < Roll.Packets.Count; i++)
            {
                if (Roll.Packets[i].When == "Break")
                {
                    Who.BreakPackets.Add(Roll.Packets[i]);
                }
            }

            if (Roll.HalveHeal)
            {
                Who.HealHalved = true;
            }
        }

        void AffectZone(SkirmishZone Zone, int DieFace, int Natural, bool Sustaining)
        {
            if (Zone == null || Zone.Effect == null)
            {
                return;
            }

            for (int i = 0; i < Field.Bodies.Count; i++)
            {
                PlayerCharacter Body = Field.Bodies[i];
                if (Body.CurrentHP <= 0 || Body == Zone.Owner)
                {
                    continue;
                }

                if (Vector3.Distance(Body.Position, Zone.Position) > PracticeMath.ToUnits(Zone.Feet))
                {
                    continue;
                }

                StrikeRoll Roll = new StrikeRoll();
                Roll.SpellRank = Zone.Rank;
                Roll.Trigger = "Cast";
                Roll.UseSustain = Sustaining;
                FightRoll.RunTraits(new List<Trait> { Zone.Effect }, Roll);
                if (Zone.HalveHeal || Roll.HalveHeal)
                {
                    Body.HealHalved = true;
                }

                if (!string.IsNullOrEmpty(Roll.Save))
                {
                    ResolveSave(Zone.Owner, Body, Roll, Natural, DieFace);
                }
                else
                {
                    FightRoll.ApplyConditions(Body, Roll, StrikeDegree.Success, Zone.Owner);
                }
                if (Roll.Conditions.Count == 0 && Roll.SpeedPenaltyFeet > 0)
                {
                    Body.SpeedPenaltyFeet = Roll.SpeedPenaltyFeet;
                }
            }
        }

        internal void TickZones(PlayerCharacter Who)
        {
            for (int i = 0; i < Field.Zones.Count; i++)
            {
                if (Vector3.Distance(Who.Position, Field.Zones[i].Position) <= PracticeMath.ToUnits(Field.Zones[i].Feet))
                {
                    if (Field.Zones[i].HalveHeal)
                    {
                        Who.HealHalved = true;
                    }
                }
            }
        }

        internal int SpellBonus(PlayerCharacter Caster)
        {
            if (Caster == null)
            {
                return 0;
            }

            return Caster.CastingAbility + Caster.Level + Caster.SpellProficiency;
        }

        int SaveBonus(PlayerCharacter Target, string Save)
        {
            if (Target == null)
            {
                return 0;
            }

            int Bonus = Target.Fortitude;
            if (Save == "Reflex")
            {
                Bonus = Target.Reflex;
                if (Target.RaisedShield && Field.Actions.HasActivity(Target, "Reflexive Shield"))
                {
                    Bonus += Target.ShieldBonus;
                }
            }
            else if (Save == "Will")
            {
                Bonus = Target.Will;
            }

            Bonus -= Target.Frightened;
            return Bonus;
        }

    }
}
