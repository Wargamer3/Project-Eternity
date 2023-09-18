using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class DisableEffectsEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Disable Effects";

        public DisableEffectsEffect()
            : base(Name, false)
        {
        }

        public DisableEffectsEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.CanUseEffectsOrAbilities = false;
            return "No effects or abilities can be used during battle";
        }

        protected override BaseEffect DoCopy()
        {
            DisableEffectsEffect NewEffect = new DisableEffectsEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
