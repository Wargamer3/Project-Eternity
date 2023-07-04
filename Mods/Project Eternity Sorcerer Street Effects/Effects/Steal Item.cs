using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class StealItemEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Steal Item";

        public StealItemEffect()
            : base(Name, false)
        {
        }

        public StealItemEffect(SorcererStreetBattleParams Params)
            : base(Name, false, Params)
        {
        }
        
        protected override void Load(BinaryReader BR)
        {
        }

        protected override void Save(BinaryWriter BW)
        {
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
            StealItemEffect NewEffect = new StealItemEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
