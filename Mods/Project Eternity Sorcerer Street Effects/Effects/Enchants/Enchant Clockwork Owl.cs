using System;
using System.IO;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class EnchantClockworkOwlEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Clockwork Owl";

        public EnchantClockworkOwlEffect()
            : base(Name, false)
        {
        }

        public EnchantClockworkOwlEffect(SorcererStreetBattleParams Params)
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
            SetPlayerDirectionEffect NewHolyWord0Effect = new SetPlayerDirectionEffect(Params, SetPlayerDirectionEffect.PlayerDirections.None);
            NewHolyWord0Effect.Lifetime[0].LifetimeType = SkillEffect.LifetimeTypeTurns;
            NewHolyWord0Effect.Lifetime[0].LifetimeTypeValue = 1;
            Params.GlobalPlayerContext.ActivePlayer.Enchant = EnchantHelper.CreatePassiveEnchant(Name, NewHolyWord0Effect, IconHolder.Icons.sprPlayerMovement);
            return "Holy Word 0";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantClockworkOwlEffect NewEffect = new EnchantClockworkOwlEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
