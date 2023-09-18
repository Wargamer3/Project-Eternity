using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SwapHPAndSTEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Swap HP and ST";
        public enum Targets { Self, Opponent }

        private Targets _Target;


        public SwapHPAndSTEffect()
            : base(Name, false)
        {
            _Target = Targets.Opponent;
        }

        public SwapHPAndSTEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
            _Target = Targets.Opponent;
        }

        protected override void Load(BinaryReader BR)
        {
            _Target = (Targets)BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write((byte)_Target);
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
            SwapHPAndSTEffect NewEffect = new SwapHPAndSTEffect(Params);

            NewEffect._Target = _Target;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            SwapHPAndSTEffect CopyRequirement = (SwapHPAndSTEffect)Copy;

            _Target = CopyRequirement._Target;
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

        #endregion
    }
}
