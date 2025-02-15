using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//User's next lap bonus is increased to 1.5x. Effect ends when user reaches the castle.
    public sealed class EnchantTriumphEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Triumph";

        public EnchantTriumphEffect()
            : base(Name, false)
        {
        }

        public EnchantTriumphEffect(SorcererStreetBattleParams Params)
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
            SetCastleValueEffect NewSetCastleValueEffect = new SetCastleValueEffect(Params, 1.5f);
            NewSetCastleValueEffect.Lifetime[0].LifetimeType = CastleTerrain.CastleReachedLifetimeType;
            NewSetCastleValueEffect.Lifetime[0].LifetimeTypeValue = 1;

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewSetCastleValueEffect, IconHolder.Icons.sprPlayerTriumph);
            return "Triumph";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantTriumphEffect NewEffect = new EnchantTriumphEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
