using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;
using static ProjectEternity.Core.Operators;
using static ProjectEternity.GameScreens.SorcererStreetScreen.ActionPanelBattleAttackPhase;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class NeutralizeDamageEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Neutralize Damage";

        public AttackTypes[] ArrayNeutralizeType;
        private NumberTypes _SignOperator;
        private string _Value;

        public NeutralizeDamageEffect()
            : base(Name, false)
        {
            ArrayNeutralizeType = new AttackTypes[0];
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }

        public NeutralizeDamageEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            ArrayNeutralizeType = new AttackTypes[0];
            _SignOperator = NumberTypes.Relative;
            _Value = "100";
        }
        
        protected override void Load(BinaryReader BR)
        {
            int ArrayAffinityLength = BR.ReadByte();
            ArrayNeutralizeType = new AttackTypes[ArrayAffinityLength];
            for (int A = 0; A < ArrayAffinityLength; ++A)
            {
                ArrayNeutralizeType[A] = (AttackTypes)BR.ReadByte();
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
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.ListNeutralizeType.AddRange(ArrayNeutralizeType);
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.NeutralizeSignOperator = _SignOperator;
            Params.GlobalContext.SelfCreature.Creature.BattleAbilities.NeutralizeValue = Params.ActiveParser.Evaluate(_Value);

            return "Neutralize " + _Value + "% Damage";
        }

        protected override BaseEffect DoCopy()
        {
            NeutralizeDamageEffect NewEffect = new NeutralizeDamageEffect(Params);

            NewEffect.ArrayNeutralizeType = ArrayNeutralizeType;
            NewEffect._SignOperator = _SignOperator;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            NeutralizeDamageEffect NewEffect = (NeutralizeDamageEffect)Copy;

            ArrayNeutralizeType = NewEffect.ArrayNeutralizeType;
            _SignOperator = NewEffect._SignOperator;
            _Value = NewEffect._Value;
        }

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public AttackTypes[] Lands
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
