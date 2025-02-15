using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature is destroyed at Battle End.
    public sealed class EnchantSenilityEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Senility";

        public EnchantSenilityEffect()
            : base(Name, false)
        {
        }

        public EnchantSenilityEffect(SorcererStreetBattleParams Params)
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
            DestroyCreatureEffect NewDestroyCreatureEffect = new DestroyCreatureEffect(Params);
            NewDestroyCreatureEffect.Target = DestroyCreatureEffect.Targets.Opponent;
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, NewDestroyCreatureEffect, IconHolder.Icons.sprCreatureSenility);
            return null;
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSenilityEffect NewEffect = new EnchantSenilityEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
