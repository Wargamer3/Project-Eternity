using System;
using System.ComponentModel;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Die yields only 4-6 when rolled normally.
    public sealed class GlobalAbilityLimitDiceRollEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Global Ability Limit Dice Roll";

        private byte _LowerValue;
        private byte _UpperValue;

        public GlobalAbilityLimitDiceRollEffect()
            : base(Name, false)
        {
            _LowerValue = 4;
            _UpperValue = 6;
        }

        public GlobalAbilityLimitDiceRollEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _LowerValue = 4;
            _UpperValue = 6;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _LowerValue = BR.ReadByte();
            _UpperValue = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_LowerValue);
            BW.Write(_UpperValue);
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
            GlobalAbilityLimitDiceRollEffect NewEffect = new GlobalAbilityLimitDiceRollEffect(Params);

            NewEffect._LowerValue = _LowerValue;
            NewEffect._UpperValue = _UpperValue;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            GlobalAbilityLimitDiceRollEffect NewEffect = (GlobalAbilityLimitDiceRollEffect)Copy;

            _LowerValue = NewEffect._LowerValue;
            _UpperValue = NewEffect._UpperValue;
        }

        #region Properties

        [CategoryAttribute(""),
        DescriptionAttribute("The minimum value of the dice."),
        DefaultValueAttribute(0)]
        public byte LowerValue
        {
            get
            {
                return _LowerValue;
            }
            set
            {
                _LowerValue = value;
            }
        }

        [CategoryAttribute(""),
        DescriptionAttribute("The maximum value of the dice."),
        DefaultValueAttribute(0)]
        public byte UpperValue
        {
            get
            {
                return _UpperValue;
            }
            set
            {
                _UpperValue = value;
            }
        }

        #endregion
    }
}
