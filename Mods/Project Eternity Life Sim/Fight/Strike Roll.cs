using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum StrikeDegree
    {
        CriticalFailure,
        Failure,
        Success,
        CriticalSuccess,
    }

    public struct DamageExpression
    {
        public int Count;
        public int Faces;
        public int Bonus;
    }

    public class DamagePacket
    {
        public int Count;
        public int Faces;
        public int Flat;
        public string Type = "";
        public string When = "Hit";
        public bool Nonlethal;
        public bool Basic;
    }

    public class RollCondition
    {
        public string Name = "";
        public int Value = 1;
        public int Floor;
        public string When = "Hit";
        public string Scope = "Target";
    }

    public class StrikeRoll
    {
        public PlayerCharacter Who;
        public Weapon Weapon;
        public int StrikesAlreadyMade;
        public int StrikesWithThisWeapon;
        public bool MissedWithThisWeapon;
        public int UntypedPenalty;
        public int CircumstanceBonus;
        public int CircumstancePenalty;
        public int AttackAbility;
        public int DamageAbility;
        public int DiceCount;
        public int DiceFaces;
        public int FlatBonus;
        public int ExtraDamage;
        public int ExtraDiceCount;
        public int ExtraFaces;
        public bool Critical;
        public bool Hit;
        public float Distance;
        public bool OffGuard;
        public int Potency;
        public bool FatalDieAdded;
        public string RequestedType;
        public string VersatileType;
        public string DamageType;
        public List<DamagePacket> Packets = new List<DamagePacket>();
        public List<RollCondition> Conditions = new List<RollCondition>();
        public List<string> Activities = new List<string>();
        public int IgnoreCircumstance;
        public int SpellRank = 1;
        public bool HoldPenalty;
        public string Save = "";
        public bool Basic;
        public bool Automatic;
        public int AutomaticCount = 1;
        public string Activity = "";
        public bool TripIsCritical;
        public int GrantArmor;
        public int GrantHardness;
        public bool RaiseShield;
        public int GrantResistance;
        public string ResistanceType = "";
        public int HealAmount;
        public bool TemporaryHeal;
        public string RecoverPersistent = "";
        public int GrantImages;
        public bool IsSpellAttack;
        public bool IsZone;
        public float ZoneFeet;
        public float RangeFeet;
        public float StepFeet;
        public bool SpendReaction;
        public bool FreeStrike;
        public bool StrikeBack;
        public string SummonEntry = "";
        public string FormName = "";
        public string Trigger = "";
        public string TargetTrait = "";
        public int AimPenalty;
        public int AimDamage;
        public int NextDamage;
        public string NextDamageGroup = "";
        public bool Sacrifice;
        public int LeapFeet;
        public bool ClearDying;
        public float AreaFeet;
        public int PerAction;
        public bool ReactionAvailable;
        public int SpeedPenaltyFeet;
        public int GrantItem;
        public bool HalveHeal;
        public bool UseSustain;

        public void RaiseCircumstanceBonus(int Amount)
        {
            if (Amount > CircumstanceBonus)
            {
                CircumstanceBonus = Amount;
            }
        }

        public void RaiseCircumstancePenalty(int Amount)
        {
            if (Amount > CircumstancePenalty)
            {
                CircumstancePenalty = Amount;
            }
        }
    }
}
