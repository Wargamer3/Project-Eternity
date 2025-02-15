using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Target creature neutralizes all non-scroll attack damage in battle. User loses (amount of neutralized damage x3G) magic.
    public sealed class EnchantSimulacrumEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Simulacrum";

        public EnchantSimulacrumEffect()
            : base(Name, false)
        {
        }

        public EnchantSimulacrumEffect(SorcererStreetBattleParams Params)
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
            NeutralizeDamageEffect NewNeutralizeDamageEffect = new NeutralizeDamageEffect(Params);
            NewNeutralizeDamageEffect.ArrayNeutralizeType = new ActionPanelBattleAttackPhase.AttackTypes[] { ActionPanelBattleAttackPhase.AttackTypes.NonScrolls };
            NewNeutralizeDamageEffect.SignOperator = Core.Operators.NumberTypes.Relative;
            NewNeutralizeDamageEffect.Value = "100";

            GainGoldEffect NewLoseMoneyEffect = new GainGoldEffect();
            NewLoseMoneyEffect.Value = "-opponent.damagereceived*3";

            Params.GlobalContext.SelfCreature.Creature.Enchant = EnchantHelper.CreateBattleEnchant(Name, new List<BaseEffect>() { NewNeutralizeDamageEffect, NewLoseMoneyEffect }, IconHolder.Icons.sprCreatureSimulacrum);
            return "Simulacrum";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantSimulacrumEffect NewEffect = new EnchantSimulacrumEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
