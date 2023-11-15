using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Destroys a single card at random from hand of target creature's owner at Battle End
    public sealed class EnchantBulimiaEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Bulimia";

        public EnchantBulimiaEffect()
            : base(Name, false)
        {
        }

        public EnchantBulimiaEffect(SorcererStreetBattleParams Params)
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
            return "Bulimia";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantBulimiaEffect NewEffect = new EnchantBulimiaEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
