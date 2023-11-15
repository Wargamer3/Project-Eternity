using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Protection: HP+20 for 4 rounds.
    public sealed class EnchantToughSongEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Tough Song";

        public EnchantToughSongEffect()
            : base(Name, false)
        {
        }

        public EnchantToughSongEffect(SorcererStreetBattleParams Params)
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
            return "Tough Song";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantToughSongEffect NewEffect = new EnchantToughSongEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
