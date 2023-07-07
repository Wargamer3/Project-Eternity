using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Roslyn;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetBattleContext
    {
        public class BattleCreatureInfo
        {
            public CreatureCard Creature;
            public SimpleAnimation Animation;
            public int PlayerIndex;
            public Player Owner;
            public Card Item;
            public int FinalHP;
            public int FinalST;
            public int DamageReceived;

            public BattleCreatureInfo()
            {
            }

            public BattleCreatureInfo(CreatureCard Creature, Player Owner)
            {
                this.Creature = Creature;
                this.Owner = Owner;
            }

            public List<string> GetAttackAnimationPaths()
            {
                List<string> ListAttackAnimationPath = new List<string>();
                if (Item == null)
                {
                    ListAttackAnimationPath.Add(Creature.AttackAnimationPath);
                }
                else if (Item is CreatureCard)
                {
                    ListAttackAnimationPath.Add(((CreatureCard)Item).AttackAnimationPath);
                }
                else
                {
                    ListAttackAnimationPath.Add(((ItemCard)Item).ItemActivationAnimationPath);
                }

                return ListAttackAnimationPath;
            }
        }

        public BattleCreatureInfo Invader;
        public BattleCreatureInfo Defender;
        public TerrainSorcererStreet DefenderTerrain;

        public BaseEffect ActivatedEffect;

        public BattleCreatureInfo SelfCreature;
        public BattleCreatureInfo OpponentCreature;

        public List<TerrainSorcererStreet> ListBoostCreature;

        public ActionPanelHolder ListBattlePanelHolder;

        public AnimationBackground Background;

        public FormulaParser ActiveParser;

        public SorcererStreetBattleContext()
        {
            Invader = new BattleCreatureInfo();
            Defender = new BattleCreatureInfo();
            ListBoostCreature = new List<TerrainSorcererStreet>();
        }

        public SorcererStreetBattleContext(SorcererStreetBattleContext GlobalContext)
            :this()
        {
        }

        internal void ActivateSkill(BattleCreatureInfo Invader, BattleCreatureInfo Defender, string RequirementName)
        {
            SelfCreature = Invader;
            OpponentCreature = Defender;

            SelfCreature.Creature.ActivateSkill(RequirementName);
        }

        public bool CanActivateSkillCreature(BattleCreatureInfo Invader, BattleCreatureInfo Defender, string RequirementName)
        {
            SelfCreature = Invader;
            OpponentCreature = Defender;

            return Invader.Creature.CanActivateSkill(RequirementName);
        }

        public bool CanActivateSkillItem(BattleCreatureInfo Invader, BattleCreatureInfo Defender, string RequirementName)
        {
            SelfCreature = Invader;
            OpponentCreature = Defender;

            return Invader.Item.CanActivateSkill(RequirementName);
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
        }

        public SorcererStreetBattleParams(SorcererStreetBattleContext GlobalContext)
            : base()
        {
            this.GlobalContext = GlobalContext;

            LoadEffects();
            LoadSkillRequirements();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();
            LoadMutators();
        }

        public void ReplaceSelfCreature(CreatureCard TransformationCreature)
        {
            if (GlobalContext.SelfCreature == GlobalContext.Invader)
            {
                GlobalContext.Invader.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                GlobalContext.Invader.FinalST = TransformationCreature.CurrentST;
                GlobalContext.Invader.FinalHP = TransformationCreature.MaxHP;
            }
            else
            {
                GlobalContext.Defender.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                GlobalContext.Defender.FinalST = TransformationCreature.CurrentST;
                GlobalContext.Defender.FinalHP = TransformationCreature.MaxHP;
            }
        }

        public void ReplaceOtherCreature(CreatureCard TransformationCreature)
        {
            if (GlobalContext.SelfCreature == GlobalContext.Invader)
            {
                GlobalContext.Defender.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                GlobalContext.Defender.FinalST = TransformationCreature.CurrentST;
                GlobalContext.Defender.FinalHP = TransformationCreature.MaxHP;
            }
            else
            {
                GlobalContext.Invader.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
                GlobalContext.Invader.FinalST = TransformationCreature.CurrentST;
                GlobalContext.Invader.FinalHP = TransformationCreature.MaxHP;
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
