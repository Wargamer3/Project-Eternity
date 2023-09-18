using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ReturnToHandEffect : SorcererStreetEffect
    {
        public enum Targets { Self, Opponent }

        public static string Name = "Sorcerer Street Return To Hand";

        private Targets _Target;

        public ReturnToHandEffect()
            : base(Name, false)
        {
        }

        public ReturnToHandEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
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
            return "Return To Hand";
        }

        protected override BaseEffect DoCopy()
        {
            ReturnToHandEffect NewEffect = new ReturnToHandEffect(Params);

            NewEffect._Target = _Target;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ReturnToHandEffect NewEffect = (ReturnToHandEffect)Copy;

            _Target = NewEffect._Target;
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

        #endregion
    }
}
