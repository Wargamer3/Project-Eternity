using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantHolyWord2Effect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Holy Word 2";

        public EnchantHolyWord2Effect()
            : base(Name, false)
        {
        }

        public EnchantHolyWord2Effect(SorcererStreetBattleParams Params)
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
            SetDiceValueEffect NewHolyWord2Effect = new SetDiceValueEffect(Params, 2);
            NewHolyWord2Effect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewHolyWord2Effect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewHolyWord2Effect, IconHolder.Icons.sprPlayerMovement);
            return "Holy Word 2";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantHolyWord2Effect NewEffect = new EnchantHolyWord2Effect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
