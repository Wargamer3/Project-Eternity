using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class PracticeEncounter
    {
        public const string Interaction = "Combat Dummy";
        public const int ArmorClass = 15;
        public const int StartingHP = 30;
        public const int ActionsPerTurn = 3;
        public const int DummyAttackBonus = 5;
        public const int DummyDamage = 6;

        public readonly PlayerCharacter Fighter;
        public readonly FurnitureItem Dummy;
        public int HP;
        public int ActionsLeft;
        public int StrikesThisTurn;
        public int CircumstanceArmor;
        public int LastIncoming;
        public string LastResult = "";
        readonly Dictionary<string, int> StrikesByWeapon = new Dictionary<string, int>();
        readonly Dictionary<string, bool> MissedByWeapon = new Dictionary<string, bool>();

        public PracticeEncounter(PlayerCharacter Fighter, FurnitureItem Dummy)
        {
            this.Fighter = Fighter;
            this.Dummy = Dummy;
            HP = StartingHP;
            ActionsLeft = ActionsPerTurn;
        }

        public void EndTurn()
        {
            EndTurn(RandomHelper.Next(20) + 1);
        }

        public void EndTurn(int Natural)
        {
            string Swing = DummySwing(Natural);
            ActionsLeft = ActionsPerTurn;
            StrikesThisTurn = 0;
            StrikesByWeapon.Clear();
            MissedByWeapon.Clear();
            CircumstanceArmor = 0;
            if (string.IsNullOrEmpty(Swing))
            {
                LastResult = "Turn ended. 3 actions.";
            }
            else
            {
                LastResult = Swing;
            }
        }

        string DummySwing(int Natural)
        {
            LastIncoming = 0;
            if (Fighter == null || Dummy == null)
            {
                return null;
            }

            if (!PracticeMath.InReach(Fighter, Dummy, null))
            {
                return "The dummy is out of reach. Turn ended. 3 actions.";
            }

            int Armor = PracticeMath.ArmorClass(Fighter, CircumstanceArmor);
            StrikeDegree Result = PracticeMath.Degree(Natural, DummyAttackBonus, 0, Armor);
            int Shown = 0;
            if (Result == StrikeDegree.Success)
            {
                Shown = DummyDamage;
            }
            else if (Result == StrikeDegree.CriticalSuccess)
            {
                Shown = DummyDamage * 2;
            }

            LastIncoming = Shown;
            string Phrase = PracticeMath.Incoming(Natural, DummyAttackBonus, Armor, Result, Shown, "Bludgeoning");
            if (Fighter.Logs != null)
            {
                string Equation = PracticeMath.Equation(Natural, DummyAttackBonus, 0, 0, 0);
                string Entry = "Dummy\n\r{{DiceRoll:" + Equation + "}}{{DiceResult:" + Shown + "}}";
                Entry += Phrase;
                Fighter.Logs.AddLog(Entry);
            }

            return Phrase;
        }

        public bool Strike(int Natural)
        {
            return Strike(Natural, null);
        }

        public bool Strike(int Natural, string RequestedType)
        {
            if (Fighter == null)
            {
                LastResult = "Nobody is practicing.";
                return false;
            }

            if (PracticeMath.Busy(Fighter))
            {
                LastResult = "Still moving.";
                return false;
            }

            if (ActionsLeft <= 0)
            {
                LastResult = "No actions left.";
                return false;
            }

            Weapon Weapon = Fighter.EquippedWeapon();
            if (!PracticeMath.InReach(Fighter, Dummy, Weapon))
            {
                LastResult = "Out of reach.";
                return false;
            }

            ActionsLeft--;
            if (Weapon != null)
            {
                Weapon.EnsureTraits();
            }

            string Key = WeaponKey(Weapon);
            int WithThis;
            StrikesByWeapon.TryGetValue(Key, out WithThis);
            bool Missed;
            MissedByWeapon.TryGetValue(Key, out Missed);
            float Distance = PracticeMath.DistanceToEdge(Fighter.Position, Dummy);
            StrikeRoll Roll = PracticeMath.Prepare(Fighter, Weapon, StrikesThisTurn, WithThis, Missed, Distance);
            Roll.RequestedType = RequestedType;
            PracticeMath.AlterAttack(Roll);
            MissedByWeapon[Key] = false;
            StrikesThisTurn++;
            StrikesByWeapon[Key] = WithThis + 1;

            int Bonus = PracticeMath.AttackBonus(Fighter, Weapon, Roll.AttackAbility);
            int Penalty = Roll.UntypedPenalty;
            StrikeDegree Result = PracticeMath.Degree(Natural, Bonus + Roll.CircumstanceBonus, Penalty + Roll.CircumstancePenalty, ArmorClass);
            Roll.Critical = Result == StrikeDegree.CriticalSuccess;
            Roll.Hit = Roll.Critical || Result == StrikeDegree.Success;
            if (Roll.Hit)
            {
                PracticeMath.AlterDamage(Roll);
            }
            else
            {
                MissedByWeapon[Key] = true;
            }

            int Dealt = Roll.Hit ? PracticeMath.ResolveDamage(Roll, null, null) : 0;
            HP -= Dealt;
            if (HP < 0)
            {
                HP = 0;
            }

            LastResult = PracticeMath.Describe(Natural, Bonus, Penalty, ArmorClass, Result, Dealt, HP, Roll.CircumstanceBonus, Roll.CircumstancePenalty, Roll.DamageType);
            if (Fighter.Logs != null)
            {
                string Equation = PracticeMath.Equation(Natural, Bonus, Penalty, Roll.CircumstanceBonus, Roll.CircumstancePenalty);
                string Entry = "Strike\n\r{{DiceRoll:" + Equation + "}}{{DiceResult:" + Dealt + "}}";
                Entry += LastResult;
                Fighter.Logs.AddLog(Entry);
            }

            if (HP <= 0)
            {
                LastResult += " Practice over.";
                return true;
            }

            return false;
        }

        static string WeaponKey(Weapon Weapon)
        {
            if (Weapon == null || string.IsNullOrEmpty(Weapon.Name))
            {
                return string.Empty;
            }

            return Weapon.Name;
        }
    }

    public class BeginPracticeStep : LifeSimStep
    {
        private readonly PlaySession Session;
        private readonly FurnitureItem Dummy;

        public BeginPracticeStep(PlayerCharacter Owner, PlaySession Session, FurnitureItem Dummy)
            : base("Practice", Owner, 0.2)
        {
            this.Session = Session;
            this.Dummy = Dummy;
        }

        protected override void OnFinished()
        {
            if (Session == null)
            {
                return;
            }

            Session.BeginPractice(Owner, Dummy);
        }

        protected override AutomatedAction Copy()
        {
            return new BeginPracticeStep(Owner, Session, Dummy);
        }
    }
}
