using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ChangeDamageEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Change Damage";
        public enum Targets { Self, Opponent }

        private Targets _Target;
        private NumberTypes _SignOperator;
        private string _Value;

        public ChangeDamageEffect()
            : base(Name, false)
        {
            _Target = Targets.Self;
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }

        public ChangeDamageEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Self;
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _SignOperator = (NumberTypes)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
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

            CreatureCard FinalTarget;
            if (_Target == Targets.Self)
            {
                FinalTarget = Params.GlobalContext.SelfCreature.Creature;
            }
            else
            {
                FinalTarget = Params.GlobalContext.OpponentCreature.Creature;
            }

            if (_SignOperator == NumberTypes.Relative)
            {
                FinalTarget.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).DamageMultiplier = float.Parse(EvaluationResult) / 100f;
                return "Damage changed by " + EvaluationResult + "%";
            }
            else
            {
                FinalTarget.GetCurrentAbilities(Params.GlobalContext.EffectActivationPhase).DamageModifier = int.Parse(EvaluationResult);
                return "Damage is " + EvaluationResult;
            }
        }

        protected override BaseEffect DoCopy()
        {
            ChangeDamageEffect NewEffect = new ChangeDamageEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ChangeDamageEffect NewEffect = (ChangeDamageEffect)Copy;

            _Target = NewEffect._Target;
            _SignOperator = NewEffect._SignOperator;
            _Value = NewEffect._Value;
        }


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
