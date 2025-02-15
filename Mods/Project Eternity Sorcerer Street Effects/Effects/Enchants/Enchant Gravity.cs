using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//All Players' next die roll yields a 1.
    public sealed class EnchantGravityEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Gravity";

        public EnchantGravityEffect()
            : base(Name, false)
        {
        }

        public EnchantGravityEffect(SorcererStreetBattleParams Params)
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
            foreach (var ActivePlayer in Params.Map.ListPlayer)
            {
                SetDiceValueEffect NewGravityEffect = new SetDiceValueEffect(Params, 1);
                NewGravityEffect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
                NewGravityEffect.Lifetime[0].LifetimeTypeValue = 2;
                ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewGravityEffect, IconHolder.Icons.sprPlayerMovement);
            }
            return "Gravity";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantGravityEffect NewEffect = new EnchantGravityEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
