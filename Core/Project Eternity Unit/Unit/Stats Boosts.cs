using System.Collections.Generic;

namespace ProjectEternity.Core.Units
{
    public class StatsBoosts
    {
        /// <summary>
        /// Used to add actions choices after another action.
        /// </summary>
        public struct PostActionValue
        {
            public bool Attack;
            public bool Transform;
            public bool Spirit;
            public bool Move;
        }
        
        public PostActionValue PostMovementModifier;
        public PostActionValue PostAttackModifier;
        public bool AttackFirstModifier;
        public bool AutoDodgeModifier;
        public bool IgnoreDefenseBonusModifier;
        public bool NullifyDamageModifier;
        public bool RepairModifier;
        public bool ResupplyModifier;
        public bool ShieldModifier;
        public bool SwordCutModifier;
        public bool ShootDownModifier;
        public int SupportAttackModifier;
        public int SupportAttackModifierMax;
        public int SupportDefendModifier;
        public int SupportDefendModifierMax;
        public List<string> ParryModifier;
        public List<string> NullifyAttackModifier;

        public int MovementModifier;
        public int RangeModifier;
        public int ExtraActionsPerTurn;

        public int FinalDamageModifier;
        public float FinalDamageMultiplier;
        public int FinalDamageTakenFixedModifier;
        public int BaseDamageModifier;
        public float BaseDamageMultiplier;
        public float BaseDamageTakenReductionMultiplier;

        public int CriticalHitRateModifier;
        public bool CriticalAlwaysSucceed;
        public bool CriticalAlwaysFail;

        public int   CriticalFinalDamageModifier;
        public float CriticalFinalDamageMultiplier;
        public int   CriticalBaseDamageModifier;
        public float CriticalBaseDamageMultiplier;
        public int   CriticalDamageTakenFixedModifier;
        public float CriticalDamageTakenReductionMultiplier;

        public float EXPMultiplier;
        public float PPMultiplier;
        public float MoneyMultiplier;
        public int HPMinModifier;
        public int MoraleModifier;
        public int ENCostModifier;
        public int AmmoMaxModifier;

        public int HPMaxModifier;
        public int ENMaxModifier;
        public int ArmorModifier;
        public int MobilityModifier;
        public int MVMaxModifier;
        public float HPMaxMultiplier;
        public float ENMaxMultiplier;
        public float ArmorMultiplier;
        public float MobilityMultiplier;
        public float MVMaxMultiplier;
        public Dictionary<string, int> DicTerrainLetterAttributeModifier;

        public int AccuracyModifier;
        public int AccuracyFixedModifier;//Give a fixed accuracy value.
        public float AccuracyMultiplier;
        public int EvasionModifier;
        public int EvasionFixedModifier;//Give a fixed evasion value.

        public StatsBoosts()
        {
            PostMovementModifier = new PostActionValue();
            PostAttackModifier = new PostActionValue();
            ParryModifier = new List<string>();
            NullifyAttackModifier = new List<string>();
            DicTerrainLetterAttributeModifier = new Dictionary<string, int>();
            SupportAttackModifier = 0;
            SupportDefendModifier = 0;
            Reset();
        }

        public void Reset()
        {
            PostMovementModifier = new PostActionValue();
            PostAttackModifier = new PostActionValue();
            AttackFirstModifier = false;
            AutoDodgeModifier = false;
            IgnoreDefenseBonusModifier = false;
            NullifyDamageModifier = false;
            RepairModifier = false;
            ResupplyModifier = false;
            ShieldModifier = false;
            SwordCutModifier = false;
            ShootDownModifier = false;
            ParryModifier.Clear();
            NullifyAttackModifier.Clear();

            MovementModifier = 0;
            RangeModifier = 0;
            ExtraActionsPerTurn = 0;
            FinalDamageModifier = 0;
            FinalDamageMultiplier = 1;
            FinalDamageTakenFixedModifier = 0;
            BaseDamageModifier = 0;
            BaseDamageMultiplier = 1;
            BaseDamageTakenReductionMultiplier = 1;
            CriticalHitRateModifier = 0;
            CriticalAlwaysSucceed = false;
            CriticalAlwaysFail = false;

            CriticalFinalDamageModifier = 0;
            CriticalFinalDamageMultiplier = 1;
            CriticalBaseDamageModifier = 0;
            CriticalBaseDamageMultiplier = 0;
            CriticalDamageTakenFixedModifier = 0;
            CriticalDamageTakenReductionMultiplier = 1;

            EXPMultiplier = 1;
            PPMultiplier = 1;
            MoneyMultiplier = 1;
            HPMinModifier = 0;
            MoraleModifier = 0;
            ENCostModifier = 0;
            AmmoMaxModifier = 0;

            HPMaxModifier = 0;
            ENMaxModifier = 0;
            ArmorModifier = 0;
            MobilityModifier = 0;
            MVMaxModifier = 0;

            HPMaxMultiplier = 1;
            ENMaxMultiplier = 1;
            ArmorMultiplier = 1;
            MobilityMultiplier = 1;
            MVMaxMultiplier = 1;

            Dictionary<string, int> DicTerrainLetterAttributeModifierCopy = new Dictionary<string, int>(DicTerrainLetterAttributeModifier.Count);

            foreach (var Terrain in DicTerrainLetterAttributeModifier)
            {
                DicTerrainLetterAttributeModifierCopy.Add(Terrain.Key, 0);
            }

            DicTerrainLetterAttributeModifier = DicTerrainLetterAttributeModifierCopy;

            AccuracyModifier = 0;
            AccuracyFixedModifier = 0;//Give a fixed accuracy value.
            AccuracyMultiplier = 1;
            EvasionModifier = 0;
            EvasionFixedModifier = 0;//Give a fixed evasion value.

            SupportAttackModifierMax = 0;
            SupportDefendModifierMax = 0;
        }
    }
}
