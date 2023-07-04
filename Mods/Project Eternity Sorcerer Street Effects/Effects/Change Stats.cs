using System;
using System.IO;
using System.Globalization;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeStatsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Stats";

        public enum Targets { Self, Opponent }
        public enum Stats { FinalST, BaseST, FinalHP, BaseHP, MaxHP, }

        private Targets _Target;
        private Stats _Stat;
        private SignOperators _SignOperator;
        private string _STValue;

        public ChangeStatsEffect()
            : base(Name, false)
        {
            _Target = Targets.Self;
            _Stat = Stats.FinalST;
            _SignOperator = SignOperators.PlusEqual;
            _STValue = string.Empty;
        }

        public ChangeStatsEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Self;
            _Stat = Stats.FinalST;
            _SignOperator = SignOperators.PlusEqual;
            _STValue = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _Stat = (Stats)BR.ReadByte();
            _SignOperator = (SignOperators)BR.ReadByte();
            _STValue = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
            BW.Write((byte)_Stat);
            BW.Write((byte)_SignOperator);
            BW.Write(_STValue);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_STValue);

            if (_Stat == Stats.FinalST)
            {
                switch (_SignOperator)
                {
                    case SignOperators.Equal:
                        Params.GlobalContext.SelfCreature.FinalST = int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.PlusEqual:
                        Params.GlobalContext.SelfCreature.FinalST += int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.DividedEqual:
                        Params.GlobalContext.SelfCreature.FinalST /= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.MinusEqual:
                        Params.GlobalContext.SelfCreature.FinalST /= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.MultiplicatedEqual:
                        Params.GlobalContext.SelfCreature.FinalST *= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;

                    case SignOperators.ModuloEqual:
                        Params.GlobalContext.SelfCreature.FinalST %= int.Parse(EvaluationResult, CultureInfo.InvariantCulture);
                        break;
                }
            }

            return "ST+" + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeStatsEffect NewEffect = new ChangeStatsEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._Stat = _Stat;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._STValue = _STValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeStatsEffect NewEffect = (ChangeStatsEffect)Copy;

            _Target = NewEffect._Target;
            _Stat = NewEffect._Stat;
            _SignOperator = NewEffect._SignOperator;
            _STValue = NewEffect._STValue;
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
        public Stats Stat
        {
            get
            {
                return _Stat;
            }
            set
            {
                _Stat = value;
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
        public string ST
        {
            get
            {
                return _STValue;
            }
            set
            {
                _STValue = value;
            }
        }

        #endregion
    }
}
