using System;
using System.Collections.Generic;
using System.IO;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{//Raises target territory's toll value to 1.5x. / In Battle: HP-10
    public sealed class EnchantGreedEffect : SorcererStreetEffect
    {
        public static string Name = "Sorcerer Street Enchant Greed";

        public EnchantGreedEffect()
            : base(Name, false)
        {
        }

        public EnchantGreedEffect(SorcererStreetBattleParams Params)
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
            List<BaseEffect> ListEffect = new List<BaseEffect>();

            ChangeTollEffect NewChangeTollEffect = new ChangeTollEffect(Params);
            NewChangeTollEffect.SignOperator = Core.Operators.SignOperators.MultiplicatedEqual;
            NewChangeTollEffect.Value = "1.5";

            DealDamageEffect NewDealDamageEffect = new DealDamageEffect(Params);
            NewDealDamageEffect.DamageToDeal = "10";

            ListEffect.Add(NewChangeTollEffect);
            ListEffect.Add(NewDealDamageEffect);
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = Name;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation Activation1 = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(Activation1);
            BaseSkillActivation Activation2 = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(Activation2);

            Activation1.ListRequirement.Add(new SorcererStreetOnCreateRequirement());
            Activation1.ListEffect.Add(NewChangeTollEffect);
            Activation1.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            Activation1.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            Activation2.ListRequirement.Add(new SorcererStreetEnchantPhaseRequirement());
            Activation2.ListEffect.Add(NewDealDamageEffect);
            Activation2.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            Activation2.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            Params.GlobalContext.SelfCreature.Creature.Enchant = new Enchant(NewEnchant, IconHolder.Icons.sprCreatureGreed);
            return "Greed";
        }

        protected override BaseEffect DoCopy()
        {
            EnchantGreedEffect NewEffect = new EnchantGreedEffect(Params);

            return NewEffect;
        }

        protected override void DoCopyMembers(BaseEffect Copy)
        {
        }
    }
}
