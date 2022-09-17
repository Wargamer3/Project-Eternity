using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Skill;
using ProjectEternity.GameScreens.BattleMapScreen;
using Roslyn;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class DeathmatchParams : BattleParams
    {
        public new DeathmatchMap Map;//The map is shared and changed as needed.
        public SquadPERContext GlobalSquadContext;
        public SquadPERParams SquadParams;
        public AttackPERContext GlobalAttackContext;
        public AttackPERParams AttackParams;

        public static readonly ConcurrentDictionary<string, DeathmatchParams> DicParams = new ConcurrentDictionary<string, DeathmatchParams>();

        public DeathmatchParams()
            : base()
        {
        }

        public DeathmatchParams(BattleContext GlobalContext)
            : base(GlobalContext)
        {
        }

        protected override void LoadUnits()
        {
            base.LoadUnits();

            foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssemblyFiles(Directory.GetFiles("Units/Deathmatch Map", "*.dll"), this))
            {
                DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
            }

            var ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Units/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Unit> ActiveUnit in Unit.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicUnitType.Add(ActiveUnit.Key, ActiveUnit.Value);
                }
            }
        }

        protected override void LoadEffects()
        {
            GlobalSquadContext = new SquadPERContext();
            SquadParams = new SquadPERParams(GlobalSquadContext);

            GlobalAttackContext = new AttackPERContext();
            AttackParams = new AttackPERParams(GlobalAttackContext);

            base.LoadEffects();

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(DeathmatchEffect), this))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(DeathmatchEffect), this))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(DeathmatchAttackPEREffect), AttackParams))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(DeathmatchAttackPEREffect), AttackParams))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(DeathmatchSquadPEREffect), SquadParams))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(DeathmatchSquadPEREffect), SquadParams))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        protected override void LoadSkillRequirements()
        {
            base.LoadSkillRequirements();

            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(UnitSkillRequirement), this);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), this);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(AttackPERRequirement), AttackParams);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(AttackPERRequirement), AttackParams);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(SquadPERRequirement), SquadParams);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(SquadPERRequirement), SquadParams);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        protected override void LoadAutomaticSkillActivation()
        {
            base.LoadAutomaticSkillActivation();

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(AutomaticDeathmatchTargetType), this))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AutomaticDeathmatchTargetType), this))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(AttackTargetType), AttackParams))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(AttackTargetType), AttackParams))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }

            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), typeof(SquadTargetType), SquadParams))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(SquadTargetType), SquadParams))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        protected override void LoadManualSkillActivation()
        {
            base.LoadManualSkillActivation();

            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Deathmatch Map", "*.dll"), this))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        protected override void LoadMutators()
        {
            base.LoadMutators();

            foreach (KeyValuePair<string, Mutator> ActiveAutomaticSkill in Mutator.LoadFromAssemblyFiles(Directory.GetFiles("Mutators/Deathmatch Map", "*.dll"), this))
            {
                DicMutator.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Mutators/Deathmatch Map", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Mutator> ActiveAutomaticSkill in Mutator.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicMutator.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }
    }
}
