using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using static ProjectEternity.GameScreens.SorcererStreetScreen.SorcererStreetBattleContext;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Enchant
    {
        public BaseAutomaticSkill Skill;
        public Texture2D sprIcon;
        public UnitMap3D Unit3DSprite;
        public AnimatedModel Unit3DModel;

        public Enchant(BaseAutomaticSkill Skill)
        {
            this.Skill = Skill;
        }

        public Enchant(BaseAutomaticSkill Skill, Texture2D sprIcon)
        {
            this.Skill = Skill;
            this.sprIcon = sprIcon;
            Unit3DSprite = new UnitMap3D(GameScreen.GraphicsDevice, GameScreen.ContentFallback.Load<Effect>("Shaders/Billboard 3D"), sprIcon, 1);
            Unit3DSprite.SetScale(new Microsoft.Xna.Framework.Vector2(16, 16));
        }
    }

    public static class EnchantHelper
    {
        public static bool CanActivate(ManualSkill Spell)
        {
            Spell.UpdateSkillActivation();
            return Spell.CanActivate;
        }

        public static void ActivateEnchant(SorcererStreetPlayerContext GlobalPlayerContext, ManualSkill Spell)
        {
            for (int E = Spell.ListEffect.Count - 1; E >= 0; --E)
            {
                BaseEffect ActiveEffect = Spell.ListEffect[E].Copy();

                ActiveEffect.ExecuteEffect();
            }

            foreach (BaseSkillActivation Activation in GlobalPlayerContext.ActivePlayer.Enchant.Skill.CurrentSkillLevel.ListActivation)
            {
                for (int E = Activation.ListEffect.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = Activation.ListEffect[E];

                    foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                    {
                        if (ActiveLifetime.LifetimeType == SkillEffect.LifetimeTypeTurns)
                        {
                            ActiveLifetime.LifetimeType = SkillEffect.LifetimeTypeTurns + GlobalPlayerContext.ActivePlayerIndex;
                        }

                        ActiveLifetime.Lifetime = ActiveLifetime.LifetimeTypeValue;
                    }
                }
            }
        }

        public static void ActivateEnchantOnCreature(SorcererStreetBattleContext GlobalContext, ManualSkill Spell, CreatureCard SelfCreature)
        {
            GlobalContext.SelfCreature.Creature = SelfCreature;

            for (int E = Spell.ListEffect.Count - 1; E >= 0; --E)
            {
                BaseEffect ActiveEffect = Spell.ListEffect[E].Copy();

                ActiveEffect.ExecuteEffect();
            }

            foreach (BaseSkillActivation Activation in SelfCreature.Enchant.Skill.CurrentSkillLevel.ListActivation)
            {
                for (int E = Activation.ListEffect.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = Activation.ListEffect[E];

                    foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                    {
                        if (ActiveLifetime.LifetimeType == SkillEffect.LifetimeTypeTurns)
                        {
                            ActiveLifetime.LifetimeType = SkillEffect.LifetimeTypeTurns + GlobalContext.SelfCreature.PlayerIndex;
                        }

                        ActiveLifetime.Lifetime = ActiveLifetime.LifetimeTypeValue;
                    }
                }
            }
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
                BaseEffect ActiveEffect = Spell.ListEffect[E].Copy();

                ActiveEffect.ExecuteEffect();
            }

            foreach (BaseSkillActivation Activation in Invader.Owner.Enchant.Skill.CurrentSkillLevel.ListActivation)
            {
                for (int E = Activation.ListEffect.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = Activation.ListEffect[E];

                    foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                    {
                        if (ActiveLifetime.LifetimeType == SkillEffect.LifetimeTypeTurns)
                        {
                            ActiveLifetime.LifetimeType = SkillEffect.LifetimeTypeTurns + Context.SelfCreature.PlayerIndex;
                        }

                        ActiveLifetime.Lifetime = ActiveLifetime.LifetimeTypeValue;
                    }
                }
            }
        }

        public static void ActivateOnPlayer(SorcererStreetBattleContext Context, Player ActivePlayer, BattleCreatureInfo Invader, BattleCreatureInfo Defender)
        {
            if (ActivePlayer.Enchant == null)
            {
                return;
            }

            Context.SelfCreature = Invader;
            Context.OpponentCreature = Defender;

            List<BaseSkillActivation> DicSkillActivation = ActivePlayer.Enchant.Skill.GetAvailableActivation(ActionPanelBattleEnchantModifierPhase.RequirementName);

            foreach (BaseSkillActivation ActiveSkill in DicSkillActivation)
            {
                ActiveSkill.Activate(ActivePlayer.Enchant.Skill.Name);
            }
        }

        public static void UpdateLifetime(Player ActivePlayer, string LifetimeType)
        {
            if (ActivePlayer.Enchant == null)
            {
                return;
            }

            bool EnchantHasUpdated = false;
            bool EnchantIsAlive = false;
            foreach (BaseSkillActivation Activation in ActivePlayer.Enchant.Skill.CurrentSkillLevel.ListActivation)
            {
                for (int E = Activation.ListEffect.Count - 1; E >= 0; --E)
                {
                    BaseEffect ActiveEffect = Activation.ListEffect[E];

                    foreach (BaseEffectLifetime ActiveLifetime in ActiveEffect.Lifetime)
                    {
                        if (ActiveLifetime.LifetimeType == LifetimeType)
                        {
                            --ActiveLifetime.Lifetime;
                            ActiveEffect.ResetState();

                            if (ActiveLifetime.Lifetime == 0)
                            {
                                EnchantHasUpdated = true;
                                Activation.ListEffect.RemoveAt(E);
                                continue;
                            }

                            EnchantIsAlive = true;
                        }
                    }
                }
            }

            if (EnchantHasUpdated && !EnchantIsAlive)
            {
                ActivePlayer.Enchant = null;
            }
        }

        public static Enchant CreateEnchant(ManualSkill ActiveSkill)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = ActiveSkill.Name;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.Add(new SorcererStreetEnchantPhaseRequirement());
            DefaultActivation.ListEffect.AddRange(ActiveSkill.ListEffect);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return new Enchant(NewEnchant);
        }

        public static Enchant CreatePassiveEnchant(string EnchantName, BaseEffect EffectToUse, Texture2D sprIcon = null)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = EnchantName;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.Add(new SorcererStreetOnCreateRequirement());
            DefaultActivation.ListEffect.Add(EffectToUse);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return new Enchant(NewEnchant, sprIcon);
        }

        public static Enchant CreatePassiveEnchant(string EnchantName, List<BaseEffect> ListEffectToUse, Texture2D sprIcon = null)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = EnchantName;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.Add(new SorcererStreetOnCreateRequirement());
            DefaultActivation.ListEffect.AddRange(ListEffectToUse);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });
            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return new Enchant(NewEnchant, sprIcon);
        }

        public static Enchant CreateBattleEnchant(string EnchantName, BaseEffect EffectToUse, Texture2D sprIcon = null)
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

            return new Enchant(NewEnchant, sprIcon);
        }

        public static Enchant CreateBattleEnchant(string EnchantName, List<BaseEffect> ListEffect, Texture2D sprIcon = null)
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

            return new Enchant(NewEnchant, sprIcon);
        }

        public static Enchant CreateEnchant(string EnchantName, BaseSkillRequirement Requirement, BaseEffect ListEffect, Texture2D sprIcon = null)
        {
            BaseAutomaticSkill NewEnchant = new BaseAutomaticSkill();

            NewEnchant.CurrentLevel = 1;
            NewEnchant.Name = EnchantName;

            BaseSkillLevel DefaultSkillLevel = new BaseSkillLevel();
            NewEnchant.ListSkillLevel.Add(DefaultSkillLevel);
            BaseSkillActivation DefaultActivation = new BaseSkillActivation();
            DefaultSkillLevel.ListActivation.Add(DefaultActivation);

            DefaultActivation.ListRequirement.Add(Requirement);
            DefaultActivation.ListEffect.Add(ListEffect);

            DefaultActivation.ListEffectTarget.Add(new List<string>() { EffectActivationExecuteOnly.Name });

            DefaultActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { new EffectActivationExecuteOnly() });

            return new Enchant(NewEnchant);
        }
    }

    public sealed class SorcererStreetOnCreateRequirement : SorcererStreetRequirement
    {
        public SorcererStreetOnCreateRequirement()
            : this(null)
        {
        }

        public SorcererStreetOnCreateRequirement(SorcererStreetBattleContext GlobalContext)
            : base("Sorcerer Street On Load", GlobalContext)
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
            return true;
        }

        public override BaseSkillRequirement Copy()
        {
            return new SorcererStreetOnCreateRequirement(GlobalContext);
        }

        public override void CopyMembers(BaseSkillRequirement Copy)
        {
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
