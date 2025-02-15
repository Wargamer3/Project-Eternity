using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord3Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 3";

        public EnchantHolyWord3Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord3Effect(SorcererStreetBattleParams Params)
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
            SetDiceValueEffect NewHolyWord3Effect = new SetDiceValueEffect(Params, 3);
            NewHolyWord3Effect.Lifetime[0].LifetimeType = BattleMapScreen.BattleMap.EventTypeTurn;
            NewHolyWord3Effect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewHolyWord3Effect, IconHolder.Icons.sprPlayerMovement);
            return "Holy Word 3";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord3Effect NewEffect = new EnchantHolyWord3Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
