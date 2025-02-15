using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Every round, target Player gains (number of cards in hand x10G) magic. Effect ends when target reaches the castle.
    public sealed class EnchantFairyLightEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Fairy Light";

        public EnchantFairyLightEffect()
            : base(Name, false)
        {
        }

        public EnchantFairyLightEffect(SorcererStreetBattleParams Params)
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
            GainGoldEffect NewGainGoldEffect = new GainGoldEffect(Params);
            NewGainGoldEffect.Lifetime[0].LifetimeType = CastleTerrain.CastleReachedLifetimeType;
            NewGainGoldEffect.Lifetime[0].LifetimeTypeValue = 1;
            NewGainGoldEffect.Value = "selfplayer.cardsinhand*10";

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetOnLapBonusRequirement(), NewGainGoldEffect, IconHolder.Icons.sprPlayerFairyLight);
            return "Fairy Light";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantFairyLightEffect NewEffect = new EnchantFairyLightEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
