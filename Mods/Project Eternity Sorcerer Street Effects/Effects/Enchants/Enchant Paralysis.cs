using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Creature is unable to use items and abilities. Note: Creature does still receive Boost and Global Ability effects from other creatures.
    public sealed class EnchantParalysisEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Paralysis";

        public EnchantParalysisEffect()
            : base(Name, false)
        {
        }

        public EnchantParalysisEffect(SorcererStreetBattleParams Params)
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
            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, new ParalysisEffect(Params), IconHolder.Icons.sprCreatureParalysis);
            return "Paralysis";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantParalysisEffect NewEffect = new EnchantParalysisEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
