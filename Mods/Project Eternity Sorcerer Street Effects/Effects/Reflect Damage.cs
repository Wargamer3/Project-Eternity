using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ReflectDamageEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Reflect Damage";

        public enum ReflectTypes { NonScrolls, Scrolls }

        private ReflectTypes _ReflectType;
        private NumberTypes _SignOperator;
        private string _Value;

        public ReflectDamageEffect()
            : base(Name, false)
        {
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }

        public ReflectDamageEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }
        
        protected override void Load(BinaryReader BR)
        {
            _ReflectType = (ReflectTypes)BR.ReadByte();
            _SignOperator = (NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_ReflectType);
            BW.Write((byte)_SignOperator);
            BW.Write(_Value);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            ReflectDamageEffect NewEffect = new ReflectDamageEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public ReflectTypes NumberType
        {
            get
            {
                return _ReflectType;
            }
            set
            {
                _ReflectType = value;
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
    }
}
