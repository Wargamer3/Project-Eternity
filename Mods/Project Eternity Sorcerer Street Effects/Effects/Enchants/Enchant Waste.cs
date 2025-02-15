using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target enemy Player's creature and item cards cost double magic until target reaches the castle.
    public sealed class EnchantWasteEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Waste";

        public EnchantWasteEffect()
            : base(Name, false)
        {
        }

        public EnchantWasteEffect(SorcererStreetBattleParams Params)
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
            SetItemCostEffect NewSetItemCostEffect = new SetItemCostEffect(Params, 2f);
            NewSetItemCostEffect.Lifetime[0].LifetimeType = CastleTerrain.CastleReachedLifetimeType;
            NewSetItemCostEffect.Lifetime[0].LifetimeTypeValue = 1;

            SetCreatureCostEffect NewSetCreatureCostEffect = new SetCreatureCostEffect(Params, 2f);
            NewSetCreatureCostEffect.Lifetime[0].LifetimeType = CastleTerrain.CastleReachedLifetimeType;
            NewSetCreatureCostEffect.Lifetime[0].LifetimeTypeValue = 1;

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, new List<BaseEffect>() { NewSetItemCostEffect, NewSetCreatureCostEffect }, IconHolder.Icons.sprPlayerWaste);
            return "Waste";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantWasteEffect NewEffect = new EnchantWasteEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
