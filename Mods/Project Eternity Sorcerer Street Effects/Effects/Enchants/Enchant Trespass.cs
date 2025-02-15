using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//For two rounds caster gains the other Players' castle bonus.
    public sealed class EnchantTrespassEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Trespass";

        public EnchantTrespassEffect()
            : base(Name, false)
        {
        }

        public EnchantTrespassEffect(SorcererStreetBattleParams Params)
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
            NewGainGoldEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewGainGoldEffect.Lifetime[0].LifetimeTypeValue = 2;
            NewGainGoldEffect.Value = "CastleBonusValue";

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewGainGoldEffect, IconHolder.Icons.sprPlayerFairyLight);
            return "Trespass";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantTrespassEffect NewEffect = new EnchantTrespassEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
