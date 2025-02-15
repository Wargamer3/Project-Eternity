using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Destroys a single card at random from hand of target creature's owner at Battle End
    public sealed class EnchantBulimiaEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Bulimia";

        public EnchantBulimiaEffect()
            : base(Name, false)
        {
        }

        public EnchantBulimiaEffect(SorcererStreetBattleParams Params)
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
            DestroyCardsEffect NewDestroyCardEffect = new DestroyCardsEffect(Params);
            NewDestroyCardEffect.CardDestroyType = DestroyCardsEffect.CardDestroyTypes.Random;
            NewDestroyCardEffect.NumberOfCards = 1;
            NewDestroyCardEffect.Target = DestroyCardsEffect.Targets.Self;

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateEnchant(Name, new SorcererStreetBattleEndRequirement(), NewDestroyCardEffect, IconHolder.Icons.sprCreatureBulimia);
            return "Bulimia";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantBulimiaEffect NewEffect = new EnchantBulimiaEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
