using System;
using System.Collections.Generic;
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
            TollImmunityEffect NewTollImmunityEffect = new TollImmunityEffect(Params);
            NewTollImmunityEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewTollImmunityEffect.Lifetime[0].LifetimeTypeValue = 4;

            InvasionLimitEffect NewInvasionLimitEffect = new InvasionLimitEffect(Params);
            NewInvasionLimitEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewInvasionLimitEffect.Lifetime[0].LifetimeTypeValue = 4;

            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, new List<BaseEffect>() { NewTollImmunityEffect, NewInvasionLimitEffect }, IconHolder.Icons.sprPlayerInnocence);
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
