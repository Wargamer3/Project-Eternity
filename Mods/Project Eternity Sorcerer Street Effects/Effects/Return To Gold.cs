using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class ReturnToGoldEffect : SorcererStreetEffect
    {
        public enum Targets { Self, Opponent, LastDestroyed }

        public static string Name = "Sorcerer Street Return To Gold";

        private Targets _Target;

        public ReturnToGoldEffect()
            : base(Name, false)
        {
        }

        public ReturnToGoldEffect(SorcererStreetBattleParams Params)
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
            return "Return To Gold";
        }

        protected override BaseEffect DoCopy()
        {
            ReturnToGoldEffect NewEffect = new ReturnToGoldEffect(Params);

            NewEffect._Target = _Target;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            ReturnToGoldEffect NewEffect = (ReturnToGoldEffect)Copy;

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
