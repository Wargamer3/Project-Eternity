using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeSymbolsValueEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Symbols Value";

        public enum Targets { Terrain, Fire, Water, Earth, Air, All }

        private Targets _Target;
        private SignOperators _SignOperator;
        private string _SymbolsValue;

        public ChangeSymbolsValueEffect()
            : base(Name, false)
        {
            _Target = Targets.Terrain;
            _SignOperator = SignOperators.PlusEqual;
            _SymbolsValue = string.Empty;
        }

        public ChangeSymbolsValueEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Terrain;
            _SignOperator = SignOperators.PlusEqual;
            _SymbolsValue = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _SignOperator = (SignOperators)BR.ReadByte();
            _SymbolsValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_SignOperator);
            BW.Write(_SymbolsValue);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_SymbolsValue);

            if (_Target == Targets.Terrain)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        break;

                    case SignOperators.PlusEqual:
                        break;

                    case SignOperators.DividedEqual:
                        break;

                    case SignOperators.MinusEqual:
                        break;

                    case SignOperators.MultiplicatedEqual:
                        break;

                    case SignOperators.ModuloEqual:
                        break;
                }
            }

            return "Symbol Value+" + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeSymbolsValueEffect NewEffect = new ChangeSymbolsValueEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._SymbolsValue = _SymbolsValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeSymbolsValueEffect NewEffect = (ChangeSymbolsValueEffect)Copy;

            _Target = NewEffect._Target;
            _SignOperator = NewEffect._SignOperator;
            _SymbolsValue = NewEffect._SymbolsValue;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
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
        public string Value
        {
            get
            {
                return _SymbolsValue;
            }
            set
            {
                _SymbolsValue = value;
            }
        }

        #endregion
    }
}
