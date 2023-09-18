using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Roslyn;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;
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
            public bool DamageReceivedIgnoreLandBonus;
            public int DamageReceived;
            public int DamageNeutralized;
            public int DamageReflected;
            public int LandHP;
            public int BonusHP;
            public int BonusST;
            public int FinalHP => Creature.CurrentHP + LandHP + BonusHP;
            public int FinalST => Creature.CurrentST + BonusST;

            public BattleCreatureInfo()
            {
            }

            public BattleCreatureInfo(CreatureCard Creature, Player Owner)
            {
                this.Creature = Creature;
                this.Owner = Owner;
            }

            public void ReceiveDamage(int Damage)
            {
                if (DamageReceivedIgnoreLandBonus)
                {
                    LandHP = 0;
                }

                if (Damage < BonusHP)
                {
                    BonusHP -= Damage;
                    return;
                }
                else
                {
                    Damage -= BonusHP;
                    BonusHP = 0;
                }

                if (Damage < LandHP)
                {
                    LandHP -= Damage;
                    return;
                }
                else
                {
                    Damage -= LandHP;
                    LandHP = 0;
                }

                if (Damage < Creature.CurrentHP)
                {
                    Creature.CurrentHP -= Damage;
                }
                else
                {
                    Creature.CurrentHP = 0;
                }
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

        public bool InvaderCreatureTemporaryTransformation;
        public bool DefenderCreatureTemporaryTransformation;
        public CreatureCard OriginalInvaderCreature;
        public CreatureCard OriginalDefenderCreature;

        public BattleCreatureInfo Invader;
        public BattleCreatureInfo Defender;
        public TerrainSorcererStreet DefenderTerrain;

        public List<BaseEffect> ListActivatedEffect;

        public BattleCreatureInfo SelfCreature;
        public BattleCreatureInfo OpponentCreature;

        public bool CanUseEffectsOrAbilities;
        public UnitAndTerrainValues TerrainRestrictions;
        public Dictionary<CreatureCard.ElementalAffinity, byte> DicCreatureCountByElementType;
        public List<TerrainSorcererStreet> ListBoostCreature;
        public int TotalCreaturesDestroyed;

        public ActionPanelHolder ListBattlePanelHolder;

        public AnimationBackground Background;

        public FormulaParser ActiveParser;

        public SorcererStreetBattleContext()
        {
            CanUseEffectsOrAbilities = true;

            Invader = new BattleCreatureInfo();
            Defender = new BattleCreatureInfo();
            ListBoostCreature = new List<TerrainSorcererStreet>();
            ListActivatedEffect = new List<BaseEffect>();
        }

        public SorcererStreetBattleContext(SorcererStreetBattleContext GlobalContext)
            :this()
        {
        }

        public void ActivateSkill(BattleCreatureInfo Invader, BattleCreatureInfo Defender, Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>> DicSkillActivation)
        {
            SelfCreature = Invader;
            OpponentCreature = Defender;

            foreach (KeyValuePair<BaseAutomaticSkill, List<BaseSkillActivation>> ActiveSkill in DicSkillActivation)
            {
                foreach (BaseSkillActivation SkillActivation in ActiveSkill.Value)
                {
                    SkillActivation.Activate(ActiveSkill.Key.Name);
                }
            }
        }

        public Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>> GetAvailableActivation(BattleCreatureInfo Invader, BattleCreatureInfo Defender, string RequirementName)
        {
            SelfCreature = Invader;
            OpponentCreature = Defender;

            Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>> DicSkillActivation = Invader.Creature.GetAvailableActivation(RequirementName);
            if (Invader.Item != null)
            {
                Dictionary<BaseAutomaticSkill, List<BaseSkillActivation>> DicItemSkillActivation = Invader.Item.GetAvailableActivation(RequirementName);

                foreach (KeyValuePair<BaseAutomaticSkill, List<BaseSkillActivation>> ActiveSkill in DicItemSkillActivation)
                {
                    DicSkillActivation.Add(ActiveSkill.Key, ActiveSkill.Value);
                }
            }

            return DicSkillActivation;
        }
    }

    public class SorcererStreetPlayerMovementContext
    {
        public FormulaParser ActiveParser;

        public SorcererStreetPlayerMovementContext()
        {
        }

        public SorcererStreetPlayerMovementContext(SorcererStreetBattleContext GlobalContext)
            : this()
        {
        }

        public void StopPlayer()
        {
        }
    }

    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class SorcererStreetBattleParams : BattleParams
    {
        // This class is shared through every Effects and Requirements used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly new SorcererStreetBattleContext GlobalContext;
        public readonly SorcererStreetPlayerMovementContext GlobalPlayerMovementContext;

        public new SorcererStreetMap Map;//The map is shared and changed as needed.

        public static readonly ConcurrentDictionary<string, SorcererStreetBattleParams> DicParams = new ConcurrentDictionary<string, SorcererStreetBattleParams>();

        public SorcererStreetBattleParams()
            : base()
        {
            GlobalContext = new SorcererStreetBattleContext();
            GlobalPlayerMovementContext = new SorcererStreetPlayerMovementContext();
        }

        public SorcererStreetBattleParams(SorcererStreetBattleContext GlobalContext)
            : base()
        {
            this.GlobalContext = GlobalContext;
            GlobalPlayerMovementContext = new SorcererStreetPlayerMovementContext();

            LoadEffects();
            LoadSkillRequirements();
            LoadAutomaticSkillActivation();
            LoadManualSkillActivation();
            LoadMutators();
        }

        public void ReplaceSelfCreature(CreatureCard TransformationCreature, bool IsTemporary)
        {
            if (GlobalContext.SelfCreature == GlobalContext.Invader)
            {
                GlobalContext.InvaderCreatureTemporaryTransformation = IsTemporary;
                GlobalContext.OriginalInvaderCreature = GlobalContext.Invader.Creature;

                GlobalContext.Invader.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            else
            {
                GlobalContext.DefenderCreatureTemporaryTransformation = IsTemporary;
                GlobalContext.OriginalDefenderCreature = GlobalContext.Defender.Creature;

                GlobalContext.Defender.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
        }

        public void ReplaceOtherCreature(CreatureCard TransformationCreature, bool IsTemporary)
        {
            if (GlobalContext.SelfCreature == GlobalContext.Invader)
            {
                GlobalContext.DefenderCreatureTemporaryTransformation = IsTemporary;
                GlobalContext.OriginalDefenderCreature = GlobalContext.Defender.Creature;

                GlobalContext.Defender.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
            }
            else
            {
                GlobalContext.InvaderCreatureTemporaryTransformation = IsTemporary;
                GlobalContext.OriginalInvaderCreature = GlobalContext.Invader.Creature;

                GlobalContext.Invader.Creature = (CreatureCard)TransformationCreature.Copy(DicRequirement, DicEffect, DicAutomaticSkillTarget);
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
