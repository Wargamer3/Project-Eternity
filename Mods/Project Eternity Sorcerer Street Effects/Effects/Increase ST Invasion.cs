using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class IncreaseInvaderSTEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Increase Invader ST";

        private int _STIncrease;

        public IncreaseInvaderSTEffect()
            : base(Name, false)
        {
        }

        public IncreaseInvaderSTEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
            _STIncrease = BR.ReadInt32();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_STIncrease);
        }

        public override bool CanActivate()
        {
            return true;
        }

        protected override string DoExecuteEffect()
        {
            Params.GlobalContext.InvaderFinalST += _STIncrease;

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            IncreaseInvaderSTEffect NewEffect = new IncreaseInvaderSTEffect(Params);

            _STIncrease = NewEffect._STIncrease;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }

        #region Properties

        [CategoryAttribute("Tileset"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public int ST
        {
            get
            {
                return _STIncrease;
            }
            set
            {
                _STIncrease = value;
            }
        }

        #endregion
    }
}
