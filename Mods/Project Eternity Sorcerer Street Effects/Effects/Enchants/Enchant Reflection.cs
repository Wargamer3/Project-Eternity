using System;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature reflects all non-scroll attack damage during battle.
    public sealed class EnchantReflectionEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Reflection";

        public EnchantReflectionEffect()
            : base(Name, false)
        {
        }

        public EnchantReflectionEffect(SorcererStreetBattleParams Params)
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
            ReflectDamageEffect NewReflectDamageEffect = new ReflectDamageEffect(Params);
            NewReflectDamageEffect.ReflectionTypes = new ActionPanelBattleAttackPhase.AttackTypes[] {ActionPanelBattleAttackPhase.AttackTypes.NonScrolls };
            NewReflectDamageEffect.SignOperator = Core.Operators.NumberTypes.Relative;
            NewReflectDamageEffect.Value = "100";

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, NewReflectDamageEffect, IconHolder.Icons.sprCreatureReflect);
            return "Reflection";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantReflectionEffect NewEffect = new EnchantReflectionEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
