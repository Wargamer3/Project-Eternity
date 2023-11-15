using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public static class EnchantHelper
    {
        public static bool CanActivate(ManualSkill Spell)
        {
            Spell.UpdateSkillActivation();
            return Spell.CanActivate;
        }

        /// <summary>
        /// Bypass skill activation to not store any effect.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Spell"></param>
        /// <param name="Invader"></param>
        /// <param name="Defender"></param>
        public static void ActivateOnPlayer(SorcererStreetBattleContext Context, ManualSkill Spell, BattleCreatureInfo Invader, BattleCreatureInfo Defender)
        {
            Context.SelfCreature = Invader;
            Context.OpponentCreature = Defender;

            for (int E = Spell.ListEffect.Count - 1; E >= 0; --E)
            {
                BaseEffect ActiveSkillEffect = Spell.ListEffect[E].Copy();

                if (ActiveSkillEffect.LifetimeType == SkillEffect.LifetimeTypeTurns)
                {
                    ActiveSkillEffect.LifetimeType =  SkillEffect.LifetimeTypeTurns + Context.SelfCreature.PlayerIndex;
                }

                ActiveSkillEffect.Lifetime = ActiveSkillEffect.LifetimeTypeValue;
                ActiveSkillEffect.ExecuteEffect();
            }
        }

        public static void UpdateLifetime(BaseAutomaticSkill Enchant, string LifetimeType)
        {
            foreach (BaseSkillActivation Activation in Enchant.CurrentSkillLevel.ListActivation)
            {
                for (int E = Activation.ListEffect.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = Activation.ListEffect[E];

                    if (ActiveEffect.LifetimeType == LifetimeType)
                    {
                        --ActiveEffect.Lifetime;
                        ActiveEffect.ResetState();

                        if (ActiveEffect.Lifetime == 0)
                        {
                            Activation.ListEffect.RemoveAt(E);
                        }
                    }
                }
            }
        }

        public static BaseAutomaticSkill CreateEnchant(string EnchantName, BaseEffect EffectToUse)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = EnchantName;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.Add(new SorcererStreetEnchantPhaseRequirement());
            DefaultActivation.ListEffect.Add(EffectToUse);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return NewEnchant;
        }

        public static BaseAutomaticSkill CreateEnchant(string EnchantName, List<BaseEffect> ListEffect)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = EnchantName;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.Add(new SorcererStreetEnchantPhaseRequirement());
            DefaultActivation.ListEffect.AddRange(ListEffect);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return NewEnchant;
        }

        public static BaseAutomaticSkill CreateEnchant(string EnchantName, List<BaseSkillRequirement> ListRequirement, List<BaseEffect> ListEffect)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = EnchantName;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.AddRange(ListRequirement);
            DefaultActivation.ListEffect.AddRange(ListEffect);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return NewEnchant;
        }
    }

    public sealed class SorcererStreetEnchantPhaseRequirement : SorcererStreetRequirement
    {
        public SorcererStreetEnchantPhaseRequirement()
            : this(null)
        {
        }

        public SorcererStreetEnchantPhaseRequirement(SorcererStreetBattleContext GlobalContext)
            : base(ActionPanelBattleEnchantModifierPhase.RequirementName, GlobalContext)
        {
        }

        protected override void DoSave(BinaryWriter BW)
        {
        }

        protected override void Load(BinaryReader BR)
        {
        }

        public override bool CanActivatePassive()
        {
            return false;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetEnchantPhaseRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
        }
    }
}
