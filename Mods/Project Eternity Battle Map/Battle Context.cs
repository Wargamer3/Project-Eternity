using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Effects;
using Microsoft.Xna.Framework;
using Roslyn;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    /// <summary>
    /// Used to pass battle information to Skills.
    /// </summary>
    public class BattleContext : UnitEffectContext
    {
        public static readonly BattleContext DefaultBattleContext = new BattleContext();//When you just need a placeholder outside of a game.

        public BattleMap.BattleResult Result;
        public BattleMap.BattleResult EnemyResult;
        public Unit SupportAttack;
        public Unit SupportDefend;
        public MovementAlgorithmTile[] ArrayAttackPosition;
        public List<Vector3> ListMVPoints;
        public List<string> ListAttackPickedUp;

        public BattleContext()
        {
            SupportAttack = null;
            SupportDefend = null;
            ArrayAttackPosition = new MovementAlgorithmTile[0];
            ListMVPoints = new List<Vector3>();
            ListAttackPickedUp = new List<string>();
        }

        public BattleContext(BattleContext GlobalContext)
        {
            Result = GlobalContext.Result;
            EnemyResult = GlobalContext.EnemyResult;
            SupportAttack = GlobalContext.SupportAttack;
            SupportDefend = GlobalContext.SupportDefend;
            ArrayAttackPosition = GlobalContext.ArrayAttackPosition;
            ListMVPoints = GlobalContext.ListMVPoints;
            ListAttackPickedUp = GlobalContext.ListAttackPickedUp;

            SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter,
                 GlobalContext.EffectTargetSquad, GlobalContext.EffectTargetUnit, GlobalContext.EffectTargetCharacter, GlobalContext.ActiveParser);
        }

        public static void LoadDefaultValues()
        {
            foreach (KeyValuePair<string, Unit> ActiveUnitType in Unit.LoadAllUnits())
            {
                Unit.DicDefaultUnitType.Add(ActiveUnitType.Key, ActiveUnitType.Value);
            }

            LoadEffects();
            LoadSkillRequirements();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();

            SystemList.LoadSystemLists();
        }

        private static void LoadEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(SkillEffect), new UnitEffectParams(DefaultBattleContext, UnitQuickLoadEffectContext.DefaultQuickLoadContext)))
            {
                BaseEffect.DicDefaultEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(SkillEffect), new UnitEffectParams(DefaultBattleContext, UnitQuickLoadEffectContext.DefaultQuickLoadContext)))
            {
                BaseEffect.DicDefaultEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(DefaultBattleContext, UnitQuickLoadEffectContext.DefaultQuickLoadContext)))
                {
                    BaseEffect.DicDefaultEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SkillEffect), new UnitEffectParams(DefaultBattleContext, UnitQuickLoadEffectContext.DefaultQuickLoadContext)))
                {
                    BaseEffect.DicDefaultEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }

            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadAllEffects())
            {
                if (!BaseEffect.DicDefaultEffect.ContainsKey(ActiveEffect.Key))
                {
                    BaseEffect.DicDefaultEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        private static void LoadSkillRequirements()
        {
            BaseSkillRequirement.DicDefaultRequirement.Add(BaseSkillRequirement.OnCreatedRequirementName, new OnCreatedRequirement());

            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects", "*.dll"), typeof(UnitSkillRequirement), DefaultBattleContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                BaseSkillRequirement.DicDefaultRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }
            Dictionary<string, BaseSkillRequirement> DicRequirementBattleMap = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Battle Map", "*.dll"), typeof(UnitSkillRequirement), DefaultBattleContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMap)
            {
                BaseSkillRequirement.DicDefaultRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), DefaultBattleContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    BaseSkillRequirement.DicDefaultRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Battle Map", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementBattleMapAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(UnitSkillRequirement), DefaultBattleContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementBattleMapAssembly)
                {
                    BaseSkillRequirement.DicDefaultRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }

            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in BaseSkillRequirement.LoadAllRequirements())
            {
                if (!BaseSkillRequirement.DicDefaultRequirement.ContainsKey(ActiveRequirement.Key))
                {
                    BaseSkillRequirement.DicDefaultRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        private static void LoadAutomaticSkillActivation()
        {
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadAllTargetTypes())
            {
                AutomaticSkillTargetType.DicDefaultTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }
        }

        private static void LoadManualSkillActivation()
        {
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadAllTargetTypes())
            {
                ManualSkillTarget.DicDefaultTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }
        }
    }
}
