
using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class GainGoldEffect : SorcererStreetEffect
    {
        public enum Targets { Self, Opponent }

        public static string Name = "Sorcerer Street Gain Gold";

        private Targets _Target;
        private string _Value;

        public GainGoldEffect()
            : base(Name, false)
        {
            _Value = string.Empty;
        }

        public GainGoldEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Value = string.Empty;
        }
        
        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
            _Value = BR.ReadString();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
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
            GainGoldEffect NewEffect = new GainGoldEffect(Params);

            NewEffect._Target = _Target;
            NewEffect._Value = _Value;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            GainGoldEffect NewEffect = (GainGoldEffect)Copy;

            _Target = NewEffect._Target;
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
        DescriptionAttribute("Number of symbols to gain."),
        DefaultValueAttribute(0)]
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
