using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Skill;
using ProjectEternity.GameScreens.BattleMapScreen;
using Roslyn;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetBattleContext
    {
        public SorcererStreetMap Map;

        public CreatureCard Invader;
        public CreatureCard Defender;

        public SimpleAnimation InvaderCard;
        public SimpleAnimation DefenderCard;

        public Player InvaderPlayer;
        public Player DefenderPlayer;

        public int InvaderFinalHP;
        public int DefenderFinalHP;

        public int InvaderFinalST;
        public int DefenderFinalST;

        public Card InvaderItem;
        public Card DefenderItem;

        public CreatureCard UserCreature;
        public CreatureCard OpponentCreature;

        public AnimationBackground Background;

        public SorcererStreetBattleContext()
        {
        }

        public SorcererStreetBattleContext(SorcererStreetBattleContext GlobalContext)
        {
        }
    }

    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class SorcererStreetBattleParams : BattleParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly new SorcererStreetBattleContext GlobalContext;

        public new SorcererStreetMap Map;//The map is shared and changed as needed.

        public static readonly ConcurrentDictionary<string, SorcererStreetBattleParams> DicParams = new ConcurrentDictionary<string, SorcererStreetBattleParams>();

        public SorcererStreetBattleParams()
            : base()
        {
            GlobalContext = new SorcererStreetBattleContext();

            LoadUnits();
            LoadEffects();
            LoadSkillRequirements();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();
            LoadMutators();
        }

        public SorcererStreetBattleParams(SorcererStreetBattleContext GlobalContext)
            : base()
        {
            this.GlobalContext = GlobalContext;
        }

        public void IncreaseSelfHP(int Value)
        {
            if (GlobalContext.UserCreature == GlobalContext.Invader)
            {
                GlobalContext.InvaderFinalHP += Value;
            }
            else if (GlobalContext.UserCreature == GlobalContext.Defender)
            {
                GlobalContext.DefenderFinalHP += Value;
            }
        }

        public void IncreaseOtherHP(int Value)
        {
            if (GlobalContext.UserCreature == GlobalContext.Invader)
            {
                GlobalContext.DefenderFinalHP += Value;
            }
            else if (GlobalContext.UserCreature == GlobalContext.Defender)
            {
                GlobalContext.InvaderFinalHP += Value;
            }
        }

        public void IncreaseSelfST(int Value)
        {
            if (GlobalContext.OpponentCreature == GlobalContext.Invader)
            {
                GlobalContext.InvaderFinalST += Value;
            }
            else if (GlobalContext.OpponentCreature == GlobalContext.Defender)
            {
                GlobalContext.DefenderFinalST += Value;
            }
        }

        public void IncreaseOtherST(int Value)
        {
            if (GlobalContext.OpponentCreature == GlobalContext.Invader)
            {
                GlobalContext.DefenderFinalST += Value;
            }
            else if (GlobalContext.OpponentCreature == GlobalContext.Defender)
            {
                GlobalContext.InvaderFinalST += Value;
            }
        }

        protected override void LoadEffects()
        {
            foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Sorcerer Street", "*.dll"), typeof(SorcererStreetEffect), this))
            {
                DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Sorcerer Street", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadFromAssembly(ActiveAssembly, typeof(SorcererStreetEffect), this))
                {
                    DicEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
            }
        }

        protected override void LoadSkillRequirements()
        {
            Dictionary<string, BaseSkillRequirement> DicRequirementCore = BaseSkillRequirement.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Sorcerer Street", "*.dll"), typeof(SorcererStreetRequirement), GlobalContext);
            foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCore)
            {
                DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Sorcerer Street", "*.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                Dictionary<string, BaseSkillRequirement> DicRequirementCoreAssembly = BaseSkillRequirement.LoadFromAssembly(ActiveAssembly, typeof(SorcererStreetRequirement), GlobalContext);
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in DicRequirementCoreAssembly)
                {
                    DicRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
            }
        }

        protected override void LoadAutomaticSkillActivation()
        {
            foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Sorcerer Street", "*.dll"), typeof(SorcererStreetBattleTargetType), GlobalContext))
            {
                DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Sorcerer Street", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadFromAssembly(ActiveAssembly, typeof(SorcererStreetBattleTargetType), GlobalContext))
                {
                    DicAutomaticSkillTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }
        }

        protected override void LoadManualSkillActivation()
        {
            foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssemblyFiles(Directory.GetFiles("Effects/Sorcerer Street", "*.dll"), GlobalContext))
            {
                DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Effects/Sorcerer Street", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadFromAssembly(ActiveAssembly, GlobalContext))
                {
                    DicManualSkillTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }
        }

        protected override void LoadMutators()
        {
            /*foreach (KeyValuePair<string, Mutator> ActiveAutomaticSkill in Mutator.LoadFromAssemblyFiles(Directory.GetFiles("Mutators/Sorcerer Street", "*.dll"), this))
            {
                DicMutator.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
            }

            List<Assembly> ListAssembly = RoslynWrapper.GetCompiledAssembliesFromFolder("Mutators/Sorcerer Street", " *.csx", SearchOption.TopDirectoryOnly);
            foreach (Assembly ActiveAssembly in ListAssembly)
            {
                foreach (KeyValuePair<string, Mutator> ActiveAutomaticSkill in Mutator.LoadFromAssembly(ActiveAssembly, this))
                {
                    DicMutator.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
            }*/
        }
    }
}
