using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class IncreaseSTEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Increase ST";

        public IncreaseSTEffect()
            : base(Name, false)
        {
        }

        public IncreaseSTEffect(SorcererStreetBattleParams Params)
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
            Params.IncreaseSelfST(30);

            return null;
        }

        protected override BaseEffect DoCopy()
        {
            IncreaseSTEffect NewEffect = new IncreaseSTEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
