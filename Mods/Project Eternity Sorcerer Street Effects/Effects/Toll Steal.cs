using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class TollStealEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Toll Steal";

        private SignOperators _SignOperator;
        private string _Value;

        public TollStealEffect()
            : base(Name, false)
        {
            _SignOperator = SignOperators.PlusEqual;
            _Value = string.Empty;
        }

        public TollStealEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _SignOperator = SignOperators.PlusEqual;
            _Value = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _SignOperator = (SignOperators)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_SignOperator);
            BW.Write(_Value);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            string EvaluationResult = Params.ActiveParser.Evaluate(_Value);

            Params.GlobalPlayerContext.ActivePlayer.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).TollStealMultiplier = float.Parse(EvaluationResult) / 100f;

            return "Toll Steal: " + EvaluationResult + "%";
        }

        protected override BaseEffect DoCopy()
        {
            TollStealEffect NewEffect = new TollStealEffect(Params);

            NewEffect._SignOperator = _SignOperator;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            TollStealEffect NewEffect = (TollStealEffect)Copy;

            _SignOperator = NewEffect._SignOperator;
            _Value = NewEffect._Value;
        }

        #region Properties

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
