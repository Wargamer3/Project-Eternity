using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeTollEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Toll";

        private SignOperators _SignOperator;
        private string _Value;

        public ChangeTollEffect()
            : base(Name, false)
        {
            _SignOperator = SignOperators.PlusEqual;
            _Value = string.Empty;
        }

        public ChangeTollEffect(SorcererStreetBattleParams Params)
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

            Params.GlobalContext.SelfCreature.Creature.GetCurrentAbilities(SorcererStreetBattleContext.EffectActivationPhases.Enchant).TollOverride = int.Parse(EvaluationResult);

            return "ST+" + EvaluationResult;
        }

        protected override BaseEffect DoCopy()
        {
            ChangeTollEffect NewEffect = new ChangeTollEffect(Params);

            NewEffect._SignOperator = _SignOperator;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeTollEffect NewEffect = (ChangeTollEffect)Copy;

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
