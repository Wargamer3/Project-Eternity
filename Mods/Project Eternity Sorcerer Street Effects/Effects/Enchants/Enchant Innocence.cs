using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target Player is exempt from tolls but cannot invade enemy territories for 2 rounds.
    public sealed class EnchantInnocenceEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Innocence";

        public EnchantInnocenceEffect()
            : base(Name, false)
        {
        }

        public EnchantInnocenceEffect(SorcererStreetBattleParams Params)
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
            return "Innocence";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantInnocenceEffect NewEffect = new EnchantInnocenceEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
