using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.GameScreens.BattleMapScreen;
using Roslyn;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class LifeSimBattleContext
    {
        public PlayerCharacter Self;
        public PlayerCharacter Opponent;
    }

    public class EffectActivationContext
    {
        public PlayerCharacter User;
        public PlayerCharacter Target;

        public EffectActivationContext()
        {
        }

        public EffectActivationContext(EffectActivationContext Clone)
        {
            User = Clone.User;
            Target = Clone.User;
        }
    }

    public class LifeSimParams
    {
        public readonly PlayerCharacter Owner;

        public readonly LifeSimBattleContext BattleContext;
        public readonly EffectActivationContext ActivationContext;
        public LifeSimMap ActiveMap;

        public static readonly ConcurrentDictionary<string, LifeSimParams> DicParams = new ConcurrentDictionary<string, LifeSimParams>();

        public static Dictionary<string, BaseEffect> DicEffect = new Dictionary<string, BaseEffect>();
        public static Dictionary<string, BaseSkillRequirement> DicRequirement = new Dictionary<string, BaseSkillRequirement>();
        public static Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget = new Dictionary<string, AutomaticSkillTargetType>();
        public static Dictionary<string, ManualSkillTarget> DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();
        public static Dictionary<string, Mutator> DicMutator = new Dictionary<string, Mutator>();
        public static Dictionary<string, ActionEffect> DicActionEffect = new Dictionary<string, ActionEffect>();
        public static Dictionary<string, UnlcokableItemType> DicUnlockableItemTypeByName = new Dictionary<string, UnlcokableItemType>();
        public static Dictionary<string, UnlockRequirementEvaluator> DicRequirementByName = new Dictionary<string, UnlockRequirementEvaluator>();

        public LifeSimParams(PlayerCharacter Owner)
        {
            this.Owner = Owner;

            BattleContext = new LifeSimBattleContext();
            ActivationContext = new EffectActivationContext();
        }

        public LifeSimParams(LifeSimParams Clone)
        {
            this.Owner = Clone.Owner;
            this.ActiveMap = Clone.ActiveMap;

            BattleContext = Clone.BattleContext;
            ActivationContext = new EffectActivationContext(Clone.ActivationContext);
        }

        public static void Init()
        {
            if (DicEffect.Count == 0)
            {
                LoadEffects();
                LoadSkillRequirements();
                LoadAutomaticSkillActivation();
                LoadManualSkillActivation();
                LoadMutators();
                LoadActionEffect();
                LoadUnlcokableItemTypes();
                LoadUnlockRequirementEvaluators();
            }
        }

        protected static void LoadEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll"), typeof(LifeSimEffect)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(LifeSimEffect)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        protected static void LoadSkillRequirements()
        {
            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll"), typeof(LifeSimRequirement));
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(LifeSimRequirement));
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        protected static void LoadAutomaticSkillActivation()
        {
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll"), typeof(LifeSimAutomaticTargetType)))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(LifeSimAutomaticTargetType)))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        protected static void LoadManualSkillActivation()
        {
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        protected static void LoadMutators()
        {
            /*foreach (KeyValuePair<string, Mutator> ActiveAutomaticSkill in Mutator.LoadFromAssemblyFiles(Directory.GetFiles("Mutators/Life Sim", "*.dll"), this))
            {
                DicMutator.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Mutators/Life Sim", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Mutator> ActiveAutomaticSkill in Mutator.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicMutator.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }*/
        }

        protected static void LoadActionEffect()
        {
            foreach (KeyValuePair<string, ActionEffect> ActiveEffect in ActionEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll"), typeof(ActionEffect)))
            {
                DicActionEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ActionEffect> ActiveEffect in ActionEffect.LoadFromAssembly(ActiveAssembly, typeof(ActionEffect)))
                {
                    DicActionEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        protected static void LoadUnlcokableItemTypes()
        {
            foreach (KeyValuePair<string, UnlcokableItemType> ActiveEffect in UnlcokableItemType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll"), typeof(UnlcokableItemType)))
            {
                DicUnlockableItemTypeByName.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, UnlcokableItemType> ActiveEffect in UnlcokableItemType.LoadFromAssembly(ActiveAssembly, typeof(UnlcokableItemType)))
                {
                    DicUnlockableItemTypeByName.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        protected static void LoadUnlockRequirementEvaluators()
        {
            foreach (KeyValuePair<string, UnlockRequirementEvaluator> ActiveEffect in UnlockRequirementEvaluator.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Life Sim", "*.dll"), typeof(UnlockRequirementEvaluator)))
            {
                DicRequirementByName.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Life Sim", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, UnlockRequirementEvaluator> ActiveEffect in UnlockRequirementEvaluator.LoadFromAssembly(ActiveAssembly, typeof(LifeSimEffect)))
                {
                    DicRequirementByName.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }
    }
}
