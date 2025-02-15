using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord1Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 1";

        public EnchantHolyWord1Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord1Effect(SorcererStreetBattleParams Params)
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
            //Play Activation Animation

            //Enchant Player
            SetDiceValueEffect NewHolyWord1Effect = new SetDiceValueEffect(Params, 1);
            NewHolyWord1Effect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewHolyWord1Effect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewHolyWord1Effect, IconHolder.Icons.sprPlayerMovement);
            return "Holy Word 1";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord1Effect NewEffect = new EnchantHolyWord1Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
