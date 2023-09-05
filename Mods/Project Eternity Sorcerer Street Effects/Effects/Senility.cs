using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SenilityEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Senility";

        public SenilityEffect()
            : base(Name, false)
        {
        }

        public SenilityEffect(SorcererStreetBattleParams Params)
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
            SenilityEffect NewEffect = new SenilityEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
