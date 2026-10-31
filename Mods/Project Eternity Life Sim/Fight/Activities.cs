using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Activities
    {
        readonly Skirmish Field;

        public Activities(Skirmish Field)
        {
            this.Field = Field;
        }

        public void SuddenCharge(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Sudden Charge") || Actor.ActionsLeft < 2 || !TryFlourish(Actor))
            {
                return;
            }

            if (Target != null)
            {
                Field.Move(Actor, Target.Position, (Actor.SpeedFeet - Actor.SpeedPenaltyFeet) * 2f);
                Field.Scuttle(Actor);
            }

            Actor.ActionsLeft -= 2;
            Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, false, "Sudden Charge", false);
        }

        public void FlingingCharge(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Flinging Charge") || Actor.ActionsLeft < 2 || !TryFlourish(Actor))
            {
                return;
            }

            if (Target != null)
            {
                Field.Move(Actor, Target.Position, (Actor.SpeedFeet - Actor.SpeedPenaltyFeet) * 2f);
            }

            Actor.ActionsLeft -= 2;
            Weapon Thrown = FightRoll.CopyWeapon(Actor.CurrentWeapon());
            if (Thrown != null)
            {
                Thrown.WeaponType = Weapon.WeaponTypes.Range;
                Thrown.FarRange = 10f * PracticeMath.FeetToMeters;
            }

            int Damage = Field.Strikes.Make(Actor, Target, Thrown, Natural, DieFace, false, "Flinging Charge", false);
            if (Damage > 0 && Target != null)
            {
                Target.OffGuardMelee = true;
                Target.OffGuardMeleeFrom = Actor;
            }
        }

        public void StepAndStrike(PlayerCharacter Actor, PlayerCharacter Target, Vector3 StepTo, int Natural, int DieFace, string Activity)
        {
            if (!HasActivity(Actor, Activity) || Actor.ActionsLeft < 1 || !TryFlourish(Actor))
            {
                return;
            }

            Field.Step(Actor, StepTo);
            Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, true, Activity, false);
        }

        public void Swipe(PlayerCharacter Actor, PlayerCharacter First, PlayerCharacter Second, int FirstNatural, int SecondNatural, int DieFace)
        {
            if (!HasActivity(Actor, "Swipe") || Actor.ActionsLeft < 1 || !TryFlourish(Actor))
            {
                return;
            }

            Weapon Attack = Actor.CurrentWeapon();
            Field.Strikes.Make(Actor, First, Attack, FirstNatural, DieFace, true, "Swipe", true);
            Field.Strikes.Make(Actor, Second, Attack, SecondNatural, DieFace, false, "Swipe", true);
            Actor.StrikesThisTurn += 2;
        }

        public void SlamDown(PlayerCharacter Actor, PlayerCharacter Target, int StrikeNatural, int TripNatural, int DieFace)
        {
            if (!HasActivity(Actor, "Slam Down") || Actor.ActionsLeft < 1 || !TryFlourish(Actor))
            {
                return;
            }

            int Damage = Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), StrikeNatural, DieFace, true, "Slam Down", true);
            Actor.StrikesThisTurn++;
            if (Damage <= 0 || Target == null)
            {
                return;
            }

            StrikeRoll Probe = new StrikeRoll();
            Probe.Activity = "Slam Down";
            Probe.Trigger = "Strike";
            FightRoll.RunTraits(Actor.Feats, Probe);
            bool Crit = Probe.TripIsCritical;
            if (!Crit)
            {
                int DC = 10 + Target.Reflex;
                StrikeDegree Trip = PracticeMath.Degree(TripNatural, Actor.Athletics, 0, DC);
                Crit = Trip == StrikeDegree.CriticalSuccess;
                if (Trip != StrikeDegree.Success && Trip != StrikeDegree.CriticalSuccess)
                {
                    Field.LastResult = "Trip fails.";
                    Actor.StrikesThisTurn++;
                    return;
                }
            }

            Target.Prone = true;
            Target.OffGuard = true;
            if (Crit)
            {
                DamageExpression Dice = PracticeMath.ParseDamage(Actor.CurrentWeapon() == null ? "1d6" : Actor.CurrentWeapon().Damage);
                int Extra = Dice.Faces < 1 ? 6 : Dice.Faces;
                FightRoll.ApplyHurt(Target, Extra);
                Field.LastDamage += Extra;
            }

            Actor.StrikesThisTurn++;
            Field.LastResult = "Trip. " + Target.Name + " is prone.";
        }

        public void ViciousSwing(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Vicious Swing") || Actor.ActionsLeft < 2)
            {
                Field.LastResult = "No actions left.";
                return;
            }

            Actor.ActionsLeft--;
            Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, true, "Vicious Swing", false);
        }

        public void CombatGrab(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Combat Grab"))
            {
                return;
            }

            int Damage = Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, true, "Combat Grab", false);
            if (Damage > 0 && Actor.HandsFree > 0 && Target != null)
            {
                Target.Grabbed = true;
                Target.OffGuard = true;
            }
        }

        public void Lunge(PlayerCharacter Actor)
        {
            if (!HasActivity(Actor, "Lunge") || Actor.ActionsLeft < 1)
            {
                return;
            }

            Actor.ActionsLeft--;
            Actor.ReachBonusFeet = 5;
            Field.LastResult = "Lunge.";
        }

        public void SnaggingStrike(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Snagging Strike"))
            {
                return;
            }

            int Damage = Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, true, "Snagging Strike", false);
            if (Damage > 0 && Target != null)
            {
                Target.OffGuard = true;
                Target.OffGuardFrom = Actor;
            }
        }

        public void LightningSwap(PlayerCharacter Actor)
        {
            if (!HasActivity(Actor, "Lightning Swap") || Actor.Swapped || Actor.OtherWeapon == null)
            {
                return;
            }

            Weapon Held = Actor.Weapon;
            Actor.Weapon = Actor.OtherWeapon;
            Actor.OtherWeapon = Held;
            Actor.Swapped = true;
            Field.LastResult = "Swapped to " + (Actor.Weapon == null ? "empty" : Actor.Weapon.Name) + ".";
        }

        public void Reload(PlayerCharacter Actor)
        {
            if (Actor == null || Actor.ActionsLeft < 1)
            {
                return;
            }

            Actor.ActionsLeft--;
            Actor.Loaded = true;
            Field.Note(Actor, "Reload");
            Field.LastResult = "Reloaded.";
        }

        public void RunningReload(PlayerCharacter Actor, Vector3 Dest)
        {
            if (!HasActivity(Actor, "Running Reload") || Actor.ActionsLeft < 1)
            {
                return;
            }

            Field.Move(Actor, Dest, Actor.SpeedFeet);
            Actor.ActionsLeft--;
            Actor.Loaded = true;
            Field.Note(Actor, "Reload");
            Field.LastResult = "Running reload.";
        }

        public void HuntPrey(PlayerCharacter Actor, PlayerCharacter Prey)
        {
            if (!HasActivity(Actor, "Hunt Prey") || Actor.ActionsLeft < 1)
            {
                return;
            }

            Actor.ActionsLeft--;
            Actor.Prey = Prey;
            Field.Note(Actor, "Hunt Prey");
            Field.LastResult = "Hunting " + (Prey == null ? "nothing" : Prey.Name) + ".";
        }

        public void HuntersAim(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Hunter's Aim") || Actor.ActionsLeft < 2 || Actor.Prey != Target)
            {
                Field.LastResult = "No prey.";
                return;
            }

            Actor.ActionsLeft--;
            Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, true, "Hunter's Aim", false);
        }

        public void DeadlyAim(PlayerCharacter Actor, PlayerCharacter Target, int Natural, int DieFace)
        {
            if (!HasActivity(Actor, "Deadly Aim") || Actor.ActionsLeft < 1 || Actor.Prey != Target || !Actor.Attacked)
            {
                Field.LastResult = "Press needs an attack first.";
                return;
            }

            Field.Strikes.Make(Actor, Target, Actor.CurrentWeapon(), Natural, DieFace, true, "Deadly Aim", false);
        }

        public void Command(PlayerCharacter Ranger)
        {
            if (Ranger == null || Ranger.Companion == null || Ranger.ActionsLeft < 1)
            {
                return;
            }

            Ranger.ActionsLeft--;
            Ranger.Companion.ActionsLeft = 2;
            Field.LastResult = "Command " + Ranger.Companion.Name + ".";
        }

        public void Hide(PlayerCharacter Who)
        {
            if (HasActivity(Who, "Camouflage"))
            {
                Who.Concealed = true;
                Field.LastResult = "Hidden.";
            }
        }

        public bool HasActivity(PlayerCharacter Who, string Name)
        {
            if (Who == null || string.IsNullOrEmpty(Name))
            {
                return false;
            }

            StrikeRoll Roll = new StrikeRoll();
            Roll.Trigger = "List";
            FightRoll.RunTraits(Who.Feats, Roll);
            for (int i = 0; i < Roll.Activities.Count; i++)
            {
                if (Roll.Activities[i] == Name)
                {
                    return true;
                }
            }

            return false;
        }

        bool TryFlourish(PlayerCharacter Who)
        {
            if (Who == null || Who.FlourishUsed)
            {
                Field.LastResult = "Flourish already used.";
                return false;
            }

            Who.FlourishUsed = true;
            return true;
        }

    }
}
