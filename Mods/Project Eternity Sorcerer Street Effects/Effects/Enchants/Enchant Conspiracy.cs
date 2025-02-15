using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//User can summon creatures for no magic cost if creatures of higher cost are already in play. Lasts until user reaches the castle.
    public sealed class EnchantConspiracyEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Conspiracy";

        public EnchantConspiracyEffect()
            : base(Name, false)
        {
        }

        public EnchantConspiracyEffect(SorcererStreetBattleParams Params)
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
            SetCreatureCostEffect NewSetCreatureCostEffect = new SetCreatureCostEffect(Params, 0f);
            NewSetCreatureCostEffect.Lifetime[0].LifetimeType = CastleTerrain.CastleReachedLifetimeType;
            NewSetCreatureCostEffect.Lifetime[0].LifetimeTypeValue = 1;

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewSetCreatureCostEffect, IconHolder.Icons.sprPlayerConspiracy);

            return "Conspiracy";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantConspiracyEffect NewEffect = new EnchantConspiracyEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
