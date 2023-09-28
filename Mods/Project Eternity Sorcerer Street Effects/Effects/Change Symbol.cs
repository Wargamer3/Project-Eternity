using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;
using static ProjectEternity.GameScreens.SorcererStreetScreen.CreatureCard;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeSymbolsEffect : SorcererStreetEffect
    {
        public enum Targets { Self, Opponent }
        public enum SymbolTypeTypes { Terrain, Random, TargetMostNumerous, Neutral, Fire, Water, Earth, Air }

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
            _Value = string.Empty;
        }

        public ChangeSymbolsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _SignOperator = SignOperators.PlusEqual;
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
            string EvaluationResult = Params.ActiveParser.Evaluate(_Value);

            Player Owner;

            if (_Target == Targets.Self)
            {
                Owner = Params.GlobalContext.SelfCreature.Owner;
            }
            else
            {
                Owner = Params.GlobalContext.OpponentCreature.Owner;
            }

            ElementalAffinity ElementToChange = ElementalAffinity.Neutral;

            if (_SymbolTypeType == SymbolTypeTypes.TargetMostNumerous)
            {
                int MostNumerousSymbolValue = 0;
                foreach (System.Collections.Generic.KeyValuePair<ElementalAffinity, int> ActiveSymbol in Owner.DicOwnedSymbols)
                {
                    if (ActiveSymbol.Value > MostNumerousSymbolValue)
                    {
                        ElementToChange = ActiveSymbol.Key;
                        MostNumerousSymbolValue = ActiveSymbol.Value;
                    }
                }
            }

            int FinalValue;
            if (_NumberType == NumberTypes.Absolute)
            {
                FinalValue = int.Parse(EvaluationResult);
            }
            else
            {
                FinalValue = Owner.DicOwnedSymbols[ElementToChange] * int.Parse(EvaluationResult);
            }

            switch (_SignOperator)
            {
                case SignOperators.Equal:
                    Owner.DicOwnedSymbols[ElementToChange] = FinalValue;

                    if (_Target == Targets.Opponent)
                    {
                        return "Opponent Own " + FinalValue + " " + ElementToChange + " symbols";
                    }
                    else
                    {
                        return "Own " + FinalValue + " " + ElementToChange + " symbols";
                    }

                case SignOperators.PlusEqual:
                    Owner.DicOwnedSymbols[ElementToChange] += FinalValue;
                    if (_Target == Targets.Opponent)
                    {
                        return "Opponent gained " + FinalValue + " " + ElementToChange + " symbols";
                    }
                    else
                    {
                        return "Gained " + FinalValue + " " + ElementToChange + " symbols";
                    }

                case SignOperators.MinusEqual:
                    Owner.DicOwnedSymbols[ElementToChange] -= FinalValue;
                    if (_Target == Targets.Opponent)
                    {
                        return "Opponent lost " + FinalValue + " " + ElementToChange + " symbols";
                    }
                    else
                    {
                        return "Lost " + FinalValue + " " + ElementToChange + " symbols";
                    }

                case SignOperators.MultiplicatedEqual:
                    Owner.DicOwnedSymbols[ElementToChange] *= FinalValue;
                    break;

                case SignOperators.DividedEqual:
                    Owner.DicOwnedSymbols[ElementToChange] /= FinalValue;
                    break;

                case SignOperators.ModuloEqual:
                    Owner.DicOwnedSymbols[ElementToChange] %= FinalValue;
                    break;
            }

            return "Own " + Owner.DicOwnedSymbols[ElementToChange] + " " + ElementToChange + " symbols";
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
        public NumberTypes Element
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
