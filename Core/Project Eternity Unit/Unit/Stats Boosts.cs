using System;
using System.IO;
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

        public void QuickSave(BinaryWriter BW)
        {
            BW.Write(PostMovementModifier.Attack);
            BW.Write(PostMovementModifier.Move);
            BW.Write(PostMovementModifier.Spirit);
            BW.Write(PostMovementModifier.Transform);

            BW.Write(PostAttackModifier.Attack);
            BW.Write(PostAttackModifier.Move);
            BW.Write(PostAttackModifier.Spirit);
            BW.Write(PostAttackModifier.Transform);

            BW.Write(AttackFirstModifier);
            BW.Write(AutoDodgeModifier);
            BW.Write(IgnoreDefenseBonusModifier);
            BW.Write(NullifyDamageModifier);
            BW.Write(RepairModifier);
            BW.Write(ResupplyModifier);
            BW.Write(ShieldModifier);
            BW.Write(SwordCutModifier);
            BW.Write(ShootDownModifier);
            BW.Write(SupportAttackModifier);
            BW.Write(SupportAttackModifierMax);
            BW.Write(SupportDefendModifier);
            BW.Write(SupportDefendModifierMax);

            BW.Write(ParryModifier.Count);
            for (int P = 0; P < ParryModifier.Count; ++P)
            {
                BW.Write(ParryModifier[P]);
            }

            BW.Write(NullifyAttackModifier.Count);
            for (int N = 0; N < NullifyAttackModifier.Count; ++N)
            {
                BW.Write(NullifyAttackModifier[N]);
            }

            BW.Write(MovementModifier);
            BW.Write(RangeModifier);
            BW.Write(ExtraActionsPerTurn);

            BW.Write(FinalDamageModifier);
            BW.Write(FinalDamageMultiplier);
            BW.Write(FinalDamageTakenFixedModifier);
            BW.Write(BaseDamageModifier);
            BW.Write(BaseDamageMultiplier);
            BW.Write(BaseDamageTakenReductionMultiplier);

            BW.Write(CriticalHitRateModifier);
            BW.Write(CriticalAlwaysSucceed);
            BW.Write(CriticalAlwaysFail);

            BW.Write(CriticalFinalDamageModifier);
            BW.Write(CriticalFinalDamageMultiplier);
            BW.Write(CriticalBaseDamageModifier);
            BW.Write(CriticalBaseDamageMultiplier);
            BW.Write(CriticalDamageTakenFixedModifier);
            BW.Write(CriticalDamageTakenReductionMultiplier);

            BW.Write(EXPMultiplier);
            BW.Write(PPMultiplier);
            BW.Write(MoneyMultiplier);
            BW.Write(HPMinModifier);
            BW.Write(MoraleModifier);
            BW.Write(ENCostModifier);
            BW.Write(AmmoMaxModifier);

            BW.Write(HPMaxModifier);
            BW.Write(ENMaxModifier);
            BW.Write(ArmorModifier);
            BW.Write(MobilityModifier);
            BW.Write(MVMaxModifier);
            BW.Write(HPMaxMultiplier);
            BW.Write(ENMaxMultiplier);
            BW.Write(ArmorMultiplier);
            BW.Write(MobilityMultiplier);
            BW.Write(MVMaxMultiplier);

            BW.Write(DicTerrainLetterAttributeModifier.Count);
            foreach (KeyValuePair<string, int> ActiveTerrain in DicTerrainLetterAttributeModifier)
            {
                BW.Write(ActiveTerrain.Key);
                BW.Write(ActiveTerrain.Value);
            }

            BW.Write(AccuracyModifier);
            BW.Write(AccuracyFixedModifier);
            BW.Write(AccuracyMultiplier);
            BW.Write(EvasionModifier);
            BW.Write(EvasionFixedModifier);
        }

        public void QuickLoad(BinaryReader BR)
        {
            PostMovementModifier.Attack = BR.ReadBoolean();
            PostMovementModifier.Move = BR.ReadBoolean();
            PostMovementModifier.Spirit = BR.ReadBoolean();
            PostMovementModifier.Transform = BR.ReadBoolean();

            PostAttackModifier.Attack = BR.ReadBoolean();
            PostAttackModifier.Move = BR.ReadBoolean();
            PostAttackModifier.Spirit = BR.ReadBoolean();
            PostAttackModifier.Transform = BR.ReadBoolean();

            AttackFirstModifier = BR.ReadBoolean();
            AutoDodgeModifier = BR.ReadBoolean();
            IgnoreDefenseBonusModifier = BR.ReadBoolean();
            NullifyDamageModifier = BR.ReadBoolean();
            RepairModifier = BR.ReadBoolean();
            ResupplyModifier = BR.ReadBoolean();
            ShieldModifier = BR.ReadBoolean();
            SwordCutModifier = BR.ReadBoolean();
            ShootDownModifier = BR.ReadBoolean();
            SupportAttackModifier = BR.ReadInt32();
            SupportAttackModifierMax = BR.ReadInt32();
            SupportDefendModifier = BR.ReadInt32();
            SupportDefendModifierMax = BR.ReadInt32();

            int ParryModifierCount = BR.ReadInt32();
            ParryModifier = new List<string>(ParryModifierCount);
            for (int P = 0; P < ParryModifierCount; ++P)
            {
                ParryModifier.Add(BR.ReadString());
            }

            int NullifyAttackModifierCount = BR.ReadInt32();
            NullifyAttackModifier = new List<string>(NullifyAttackModifierCount);
            for (int N = 0; N < NullifyAttackModifierCount; ++N)
            {
                NullifyAttackModifier.Add(BR.ReadString());
            }

            MovementModifier = BR.ReadInt32();
            RangeModifier = BR.ReadInt32();
            ExtraActionsPerTurn = BR.ReadInt32();

            FinalDamageModifier = BR.ReadInt32();
            FinalDamageMultiplier = BR.ReadSingle();
            FinalDamageTakenFixedModifier = BR.ReadInt32();
            BaseDamageModifier = BR.ReadInt32();
            BaseDamageMultiplier = BR.ReadSingle();
            BaseDamageTakenReductionMultiplier = BR.ReadSingle();

            CriticalHitRateModifier = BR.ReadInt32();
            CriticalAlwaysSucceed = BR.ReadBoolean();
            CriticalAlwaysFail = BR.ReadBoolean();

            CriticalFinalDamageModifier = BR.ReadInt32();
            CriticalFinalDamageMultiplier = BR.ReadSingle();
            CriticalBaseDamageModifier = BR.ReadInt32();
            CriticalBaseDamageMultiplier = BR.ReadSingle();
            CriticalDamageTakenFixedModifier = BR.ReadInt32();
            CriticalDamageTakenReductionMultiplier = BR.ReadSingle();

            EXPMultiplier = BR.ReadSingle();
            PPMultiplier = BR.ReadSingle();
            MoneyMultiplier = BR.ReadSingle();
            HPMinModifier = BR.ReadInt32();
            MoraleModifier = BR.ReadInt32();
            ENCostModifier = BR.ReadInt32();
            AmmoMaxModifier = BR.ReadInt32();

            HPMaxModifier = BR.ReadInt32();
            ENMaxModifier = BR.ReadInt32();
            ArmorModifier = BR.ReadInt32();
            MobilityModifier = BR.ReadInt32();
            MVMaxModifier = BR.ReadInt32();
            HPMaxMultiplier = BR.ReadSingle();
            ENMaxMultiplier = BR.ReadSingle();
            ArmorMultiplier = BR.ReadSingle();
            MobilityMultiplier = BR.ReadSingle();
            MVMaxMultiplier = BR.ReadSingle();

            int DicTerrainLetterAttributeModifierCount = BR.ReadInt32();
            DicTerrainLetterAttributeModifier = new Dictionary<string, int>(DicTerrainLetterAttributeModifierCount);
            for (int T = 0; T < DicTerrainLetterAttributeModifierCount; ++T)
            {
                DicTerrainLetterAttributeModifier.Add(BR.ReadString(), BR.ReadInt32());
            }

            AccuracyModifier = BR.ReadInt32();
            AccuracyFixedModifier = BR.ReadInt32();
            AccuracyMultiplier = BR.ReadSingle();
            EvasionModifier = BR.ReadInt32();
            EvasionFixedModifier = BR.ReadInt32();
        }
    }
}
