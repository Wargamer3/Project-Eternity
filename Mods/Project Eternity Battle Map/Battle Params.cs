using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using Roslyn;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class BattleParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly BattleContext GlobalContext;

        public readonly UnitQuickLoadEffectContext GlobalQuickLoadContext;

        public BattleMap Map;

        public Dictionary<string, Unit> DicUnitType;
        public Dictionary<string, BaseEffect> DicEffect;
        public Dictionary<string, BaseSkillRequirement> DicRequirement;
        public Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        public Dictionary<string, ManualSkillTarget> DicManualSkillTarget;
        public Dictionary<string, Mutator> DicMutator;

        public BattleParams()
        {
            GlobalContext = new BattleContext();

            GlobalQuickLoadContext = new UnitQuickLoadEffectContext();

            DicUnitType = new Dictionary<string, Unit>();
            DicEffect = new Dictionary<string, BaseEffect>();
            DicRequirement = new Dictionary<string, BaseSkillRequirement>();
            DicAutomaticSkillTarget = new Dictionary<string, AutomaticSkillTargetType>();
            DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();
            DicMutator = new Dictionary<string, Mutator>();
        }

        public BattleParams(BattleContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;

            GlobalQuickLoadContext = new UnitQuickLoadEffectContext();

            DicUnitType = new Dictionary<string, Unit>();
            DicEffect = new Dictionary<string, BaseEffect>();
            DicRequirement = new Dictionary<string, BaseSkillRequirement>();
            DicAutomaticSkillTarget = new Dictionary<string, AutomaticSkillTargetType>();
            DicManualSkillTarget = new Dictionary<string, ManualSkillTarget>();
            DicMutator = new Dictionary<string, Mutator>();

            LoadUnits();
            LoadEffects();
            LoadSkillRequirements();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();
            LoadMutators();
        }

        public void Reload(BattleParams Clone, string ParamsID)
        {
            foreach (KeyValuePair<string, Unit> ActiveUnit in Clone.DicUnitType)
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in Clone.DicEffect)
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value.CopyAndReload(ParamsID));
            }

            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in Clone.DicRequirement)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value.CopyAndReload(ParamsID));
            }

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in Clone.DicAutomaticSkillTarget)
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value.CopyAndReload(ParamsID));
            }

            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in Clone.DicManualSkillTarget)
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value.CopyAndReload(ParamsID));
            }
        }

        protected virtual void LoadUnits()
        {
            foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssemblyFiles(Directory.GetFiles("Units", "*.dll")))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            foreach (Assembly ActiveAssembly in RoslynWrapper.GetCompiledAssembliesFromFolder("Units", "*.csx", SearchOption.TopDirectoryOnly))
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssembly(ActiveAssembly))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }

            foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssemblyFiles(Directory.GetFiles("Units/Battle Map", "*.dll"), this))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            foreach (Assembly ActiveAssembly in RoslynWrapper.GetCompiledAssembliesFromFolder("Units/Battle Map", " *.csx", SearchOption.TopDirectoryOnly))
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }
        }

        protected virtual void LoadEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(SkillEffect), new UnitEffectParams(GlobalContext, GlobalQuickLoadContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(SkillEffect), new UnitEffectParams(GlobalContext, GlobalQuickLoadContext)))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(GlobalContext, GlobalQuickLoadContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(GlobalContext, GlobalQuickLoadContext)))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        protected virtual void LoadSkillRequirements()
        {
            DicRequirement.Add(BaseSkillRequirement.OnCreatedRequirementName, new OnCreatedRequirement());

            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(UnitSkillRequirement), GlobalContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }
            Dictionary<string, BaseSkillRequirement> DicRequirementBattleMap = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(UnitSkillRequirement), GlobalContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMap)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementBattleMapAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), GlobalContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMapAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        protected virtual void LoadAutomaticSkillActivation()
        {
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(AutomaticSkillTargetType)))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType)))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(AutomaticSkillTargetType), GlobalContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }
            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticSkillTargetType), GlobalContext))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        protected virtual void LoadManualSkillActivation()
        {
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll")))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        protected virtual void LoadMutators()
        {
            /*foreach (KeyValuePair<string, Mutator> ActiveMuator in Mutator.LoadFromAssemblyFiles(Directory.GetFiles("Mutators", "*.dll")))
            {
                DicMutator.Add(ActiveMuator.Key, ActiveMuator.Value);
            }

            foreach (KeyValuePair<string, Mutator> ActiveMuator in Mutator.LoadFromAssemblyFiles(Directory.GetFiles("Mutators/Battle Map", "*.dll")))
            {
                DicMutator.Add(ActiveMuator.Key, ActiveMuator.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Mutators", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Mutator> ActiveMuator in Mutator.LoadFromAssembly(ActiveAssembly))
                {
                    DicMutator.Add(ActiveMuator.Key, ActiveMuator.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Mutators/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Mutator> ActiveMuator in Mutator.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicMutator.Add(ActiveMuator.Key, ActiveMuator.Value);
                }
            }*/
        }
    }
}
