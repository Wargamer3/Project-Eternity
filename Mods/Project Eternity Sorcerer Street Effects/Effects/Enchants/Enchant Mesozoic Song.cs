using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Protection: Transforms into Tyrannosaurus for 3 rounds.
    public sealed class EnchantMesozoicSongEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Mesozoic Song";

        public EnchantMesozoicSongEffect()
            : base(Name, false)
        {
        }

        public EnchantMesozoicSongEffect(SorcererStreetBattleParams Params)
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
            return "Mesozoic Song";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantMesozoicSongEffect NewEffect = new EnchantMesozoicSongEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
