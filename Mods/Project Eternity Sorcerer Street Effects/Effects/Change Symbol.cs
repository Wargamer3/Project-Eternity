using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeSymbolsEffect : SorcererStreetEffect
    {
        public enum Targets { Self, Opponent }
        public enum SymbolTypeTypes { Terrain, Random, Specific, TargetMostNumerous }

        public static string Name = "Sorcerer Street Change Symbols";

        private Targets _Target;
        private SymbolTypeTypes _SymbolTypeType;
        private SignOperators _SignOperator;
        private NumberTypes _NumberType;
        private string _Value;

        public ChangeSymbolsEffect()
            : base(Name, false)
        {
            _SignOperator = SignOperators.PlusEqual;
            _NumberType = NumberTypes.Absolute;
            _Value = string.Empty;
        }

        public ChangeSymbolsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _SignOperator = SignOperators.PlusEqual;
            _NumberType = NumberTypes.Absolute;
            _Value = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _SymbolTypeType = (SymbolTypeTypes)BR.ReadByte();
            _SignOperator = (SignOperators)BR.ReadByte();
            _NumberType = (NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_SymbolTypeType);
            BW.Write((byte)_SignOperator);
            BW.Write((byte)_NumberType);
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
            ChangeSymbolsEffect NewEffect = new ChangeSymbolsEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._SymbolTypeType = _SymbolTypeType;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._NumberType = _NumberType;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeSymbolsEffect NewEffect = (ChangeSymbolsEffect)Copy;

            _Target = NewEffect._Target;
            _SymbolTypeType = NewEffect._SymbolTypeType;
            _SignOperator = NewEffect._SignOperator;
            _NumberType = NewEffect._NumberType;
            _Value = NewEffect._Value;
        }

        #region Properties

        [CategoryAttribute(""),
        DescriptionAttribute("The Target."),
        DefaultValueAttribute(0)]
        public Targets Target
        {
            get
            {
                return _Target;
            }
            set
            {
                _Target = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("How to destroy cards."),
        DefaultValueAttribute(0)]
        public SymbolTypeTypes SymbolTypeType
        {
            get
            {
                return _SymbolTypeType;
            }
            set
            {
                _SymbolTypeType = value;
            }
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public SignOperators SignOperator
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
        public NumberTypes NumberType
        {
            get
            {
                return _NumberType;
            }
            set
            {
                _NumberType = value;
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
