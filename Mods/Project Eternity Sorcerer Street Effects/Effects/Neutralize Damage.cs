using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class NeutralizeDamageEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Neutralize Damage";

        public enum NeutralizeTypes { NonScrolls, Scrolls, Penetrate, All, Neutral, Fire, Water, Earth, Air }

        public NeutralizeTypes[] ArrayNeutralizeType;
        private NumberTypes _SignOperator;
        private string _Value;

        public NeutralizeDamageEffect()
            : base(Name, false)
        {
            ArrayNeutralizeType = new NeutralizeTypes[0];
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }

        public NeutralizeDamageEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            ArrayNeutralizeType = new NeutralizeTypes[0];
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }
        
        protected override void Load(BinaryReader BR)
        {
            int ArrayAffinityLength = BR.ReadByte();
            ArrayNeutralizeType = new NeutralizeTypes[ArrayAffinityLength];
            for (int A = 0; A < ArrayAffinityLength; ++A)
            {
                ArrayNeutralizeType[A] = (NeutralizeTypes)BR.ReadByte();
            }
            _SignOperator = (NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)ArrayNeutralizeType.Length);
            for (int A = 0; A < ArrayNeutralizeType.Length; ++A)
            {
                BW.Write((byte)ArrayNeutralizeType[A]);
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
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            NeutralizeDamageEffect NewEffect = new NeutralizeDamageEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public NeutralizeTypes[] Lands
        {
            get
            {
                return ArrayNeutralizeType;
            }
            set
            {
                ArrayNeutralizeType = value;
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
