using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;
using static ProjectEternity.GameScreens.SorcererStreetScreen.ActionPanelBattleAttackPhase;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ReflectDamageEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Reflect Damage";

        private AttackTypes[] _ArrayReflectType;
        private NumberTypes _SignOperator;
        private string _Value;

        public ReflectDamageEffect()
            : base(Name, false)
        {
            _ArrayReflectType = new AttackTypes[0];
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }

        public ReflectDamageEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _ArrayReflectType = new AttackTypes[0];
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }
        
        protected override void Load(BinaryReader BR)
        {
            int ReflectTypeLength = BR.ReadByte();
            _ArrayReflectType = new AttackTypes[ReflectTypeLength];
            for (int A = 0; A < ReflectTypeLength; ++A)
            {
                _ArrayReflectType[A] = (AttackTypes)BR.ReadByte();
            }
            _SignOperator = (NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_ArrayReflectType.Length);
            for (int A = 0; A < _ArrayReflectType.Length; ++A)
            {
                BW.Write((byte)_ArrayReflectType[A]);
            }
            BW.Write((byte)_SignOperator);
            BW.Write(_Value);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.ListReflectType.AddRange(_ArrayReflectType);
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.ReflectSignOperator = _SignOperator;
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.ReflectValue = _Value;

            return "Reflect " + _Value + "% Damage";
        }

        protected override BaseEffect DoCopy()
        {
            ReflectDamageEffect NewEffect = new ReflectDamageEffect(Params);

            NewEffect._ArrayReflectType = _ArrayReflectType;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ReflectDamageEffect NewEffect = (ReflectDamageEffect)Copy;

            _ArrayReflectType = NewEffect._ArrayReflectType;
            _SignOperator = NewEffect._SignOperator;
            _Value = NewEffect._Value;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public AttackTypes[] ReflectionTypes
        {
            get
            {
                return _ArrayReflectType;
            }
            set
            {
                _ArrayReflectType = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public NumberTypes SignOperator
        {
            get
            {
                return _SignOperator;
            }
            set
            {
                _SignOperator = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public string Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
            }
        }

        #endregion
    }
}
