using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class MoveManualEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Move Manual";

        int _MaxDistance;

        public MoveManualEffect()
            : base(Name, false)
        {
        }

        public MoveManualEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }

        protected override void Load(BinaryReader BR)
        {
            _MaxDistance = BR.ReadByte();
        }

        protected override void Save(BinaryWriter BW)
        {
            BW.Write(_MaxDistance);
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
            MoveManualEffect NewEffect = new MoveManualEffect(Params);

            NewEffect._MaxDistance = _MaxDistance;

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
            MoveManualEffect NewEffect = (MoveManualEffect)Copy;

            _MaxDistance = NewEffect._MaxDistance;
        }

        #region Properties

        [CategoryAttribute("Effects"),
        DescriptionAttribute(""),
        DefaultValueAttribute("")]
        public int MaxDistance
        {
            get
            {
                return _MaxDistance;
            }
            set
            {
                _MaxDistance = value;
            }
        }

        #endregion
    }
}
