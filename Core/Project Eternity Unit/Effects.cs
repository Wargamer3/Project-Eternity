using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Characters;
using System.Collections.Generic;

namespace ProjectEternity.Core.Effects
{
    public enum StatusTypes : byte { MEL = 0, RNG = 1, DEF = 2, SKL = 3, EVA = 4, HIT = 5 }

    public enum UnitStats : byte { MaxHP, MaxEN, Armor, Mobility, MaxMV }
    
    public class StatusTypeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(
                          ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                 GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "MEL", "RNG", "DEF", "SKL", "EVA", "HIT" });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                              Type destinationType)
        {
            if (destinationType == typeof(StatusTypes))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                           CultureInfo culture,
                           object value,
                           Type destinationType)
        {
            if (value.GetType() == typeof(string))
            {
                return value;
            }
            else if (value.GetType() == typeof(StatusTypes))
            {
                return value.ToString();
            }
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
                          Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                          CultureInfo culture, object value)
        {
            string StatusType = (string)value;

            switch (StatusType)
            {
                case "MEL":
                    return StatusTypes.MEL;

                case "RNG":
                    return StatusTypes.RNG;

                case "DEF":
                    return StatusTypes.DEF;

                case "SKL":
                    return StatusTypes.SKL;

                case "EVA":
                    return StatusTypes.EVA;

                case "HIT":
                    return StatusTypes.HIT;
            }
            return StatusTypes.MEL;
        }
    }

    public class UnitStatsConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(
                          ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
                 GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "Max HP", "Max EN", "Armor", "Mobility", "Max MV" });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context,
                              Type destinationType)
        {
            if (destinationType == typeof(UnitStats))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
                           CultureInfo culture,
                           object value,
                           Type destinationType)
        {
            if (value.GetType() == typeof(string))
            {
                return value;
            }
            else if (value.GetType() == typeof(UnitStats))
            {
                return value.ToString();
            }
            return null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
                          Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
                          CultureInfo culture, object value)
        {
            string StatusType = (string)value;

            switch (StatusType)
            {
                case "Max HP":
                    return UnitStats.MaxHP;

                case "Max EN":
                    return UnitStats.MaxEN;

                case "Armor":
                    return UnitStats.Armor;

                case "Mobility":
                    return UnitStats.Mobility;

                case "Max MV":
                    return UnitStats.MaxMV;
            }
            return UnitStats.MaxHP;
        }
    }

    public class CsvConverter : TypeConverter
    {
        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return string.Join(",", (string[])value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class UnitEffectParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly UnitEffectContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly UnitEffectContext LocalContext;

        public UnitEffectParams(UnitEffectContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;
            LocalContext = new UnitEffectContext();
        }

        public UnitEffectParams(UnitEffectParams Clone)
            : this(Clone.GlobalContext)
        {
        }

        internal void CopyGlobalIntoLocal()
        {
            LocalContext.SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter,
                 GlobalContext.EffectTargetSquad, GlobalContext.EffectTargetUnit, GlobalContext.EffectTargetCharacter);
        }
    }
    
    public class UnitEffectContext
    {
        private Squad _EffectOwnerSquad;
        private Unit _EffectOwnerUnit;
        private Character _EffectOwnerCharacter;

        private Squad _EffectTargetSquad;
        private Unit _EffectTargetUnit;
        private Character _EffectTargetCharacter;

        public Squad EffectOwnerSquad { get { return _EffectOwnerSquad; } }
        public Unit EffectOwnerUnit { get { return _EffectOwnerUnit; } }
        public Character EffectOwnerCharacter { get { return _EffectOwnerCharacter; } }

        public Squad EffectTargetSquad { get { return _EffectTargetSquad; } }
        public Unit EffectTargetUnit { get { return _EffectTargetUnit; } }
        public Character EffectTargetCharacter { get { return _EffectTargetCharacter; } }

        public void SetContext(Squad EffectOwnerSquad, Unit EffectOwnerUnit, Character EffectOwnerCharacter,
            Squad EffectTargetSquad, Unit EffectTargetUnit, Character EffectTargetCharacter)
        {
            _EffectOwnerSquad = EffectOwnerSquad;
            _EffectOwnerUnit = EffectOwnerUnit;
            _EffectOwnerCharacter = EffectOwnerCharacter;

            _EffectTargetSquad = EffectTargetSquad;
            _EffectTargetUnit = EffectTargetUnit;
            _EffectTargetCharacter = EffectTargetCharacter;
        }
    }

    public abstract class SkillEffect : BaseEffect
    {
        public const string LifetimeTypePermanent = "Permanent";
        public const string LifetimeTypeTurns = "Turns";
        public const string LifetimeTypeBattle = "Battle";
        public const string LifetimeTypeOnHit = "OnHit";
        public const string LifetimeTypeOnEnemyHit = "OnEnemyHit";
        public const string LifetimeTypeOnAttack = "OnAttack";
        public const string LifetimeTypeOnEnemyAttack = "OnEnemyAttack";
        public const string LifetimeTypeOnAction = "OnAction";

        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        protected readonly UnitEffectParams Params;

        public SkillEffect(string EffectTypeName, bool IsPassive)
           : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        public SkillEffect(string EffectTypeName, bool IsPassive, UnitEffectParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new UnitEffectParams(Params);
                this.Params.CopyGlobalIntoLocal();
                if (this.Params.LocalContext.EffectOwnerUnit != null && GameScreens.GameScreen.Debug != null)
                {
                    List<string> ListDebugText = new List<string>();
                    ListDebugText.Add("The context used was:");

                    if (this.Params.LocalContext.EffectOwnerSquad != null && !string.IsNullOrEmpty(this.Params.LocalContext.EffectOwnerSquad.SquadName))
                        ListDebugText.Add("Owner Squad: " + this.Params.LocalContext.EffectOwnerSquad.SquadName);
                    if (this.Params.LocalContext.EffectOwnerUnit != null)
                        ListDebugText.Add("Owner Unit: " + this.Params.LocalContext.EffectOwnerUnit.FullName);
                    if (this.Params.LocalContext.EffectOwnerCharacter != null)
                        ListDebugText.Add("Owner Pilot: " + this.Params.LocalContext.EffectOwnerCharacter.FullName);

                    if (this.Params.LocalContext.EffectTargetSquad != null && !string.IsNullOrEmpty(this.Params.LocalContext.EffectTargetSquad.SquadName))
                        ListDebugText.Add("Target Squad: " + this.Params.LocalContext.EffectTargetSquad.SquadName);
                    if (this.Params.LocalContext.EffectTargetUnit != null)
                        ListDebugText.Add("Target Unit: " + this.Params.LocalContext.EffectTargetUnit.FullName);
                    if (this.Params.LocalContext.EffectTargetCharacter != null)
                        ListDebugText.Add("Target Pilot: " + this.Params.LocalContext.EffectTargetCharacter.FullName);

                    GameScreens.GameScreen.Debug.AddDebugEffect(this, ListDebugText);
                }
            }
        }

        protected override void Load(BinaryReader BR)
        {
            if (LifetimeType == LifetimeTypePermanent)
                LifetimeTypeValue = -1;
        }

        public override bool CanActivate()
        {
            return true;
        }
    }
}
