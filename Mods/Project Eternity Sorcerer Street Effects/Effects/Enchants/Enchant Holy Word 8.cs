using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord8Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 8";

        public EnchantHolyWord8Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord8Effect(SorcererStreetBattleParams Params)
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
            SetDiceValueEffect NewHolyWord8Effect = new SetDiceValueEffect(Params, 8);
            NewHolyWord8Effect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewHolyWord8Effect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewHolyWord8Effect, IconHolder.Icons.sprPlayerMovement);
            return "Holy Word 8";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord8Effect NewEffect = new EnchantHolyWord8Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
