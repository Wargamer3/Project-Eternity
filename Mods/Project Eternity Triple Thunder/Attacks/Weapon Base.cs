using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.AnimationScreen;
using System.IO;
using System.Text;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class WeaponBase
    {
        public enum ProjectileTypes { Hitscan, Projectile }

        public readonly string OwnerName;
        public readonly string WeaponPath;
        public string WeaponName;
        public bool IsExtra;
        public float WeaponAngle;
        public abstract bool IsShooting { get; }

        public ExplosionOptions ExplosionAttributes;
        public float AmmoCurrent;
        public float AmmoPerMagazine;
        public bool IsReloading;
        public SimpleAnimation NozzleFlashAnimation;
        public ProjectileInfo ActiveProjectileInfo;

        public AnimationClass CurrentAnimation;
        public Texture2D sprMapIcon;

        //Weapon attributes
        protected float MinAngle;
        protected float MaxAngle;
        protected float MaxDurability;
        protected bool UseRangedProperties;

        //Ranged attributes
        protected float Damage;
        protected float AmmoRegen;
        protected float Recoil;
        protected float MaxRecoil;
        protected float RecoilRecoverySpeed;
        protected int NumberOfProjectiles;
        protected Vector2 ProjectileSize;
        protected ProjectileTypes ProjectileType;

        protected ProjectileInfo[] ArrayProjectileInfo;
        protected List<BaseAutomaticSkill> ListActiveSkill;

        public bool HasSkills => ListActiveSkill.Count > 0;
        public bool CanBeUsed { get { return AmmoPerMagazine == 0 || (AmmoPerMagazine > 0 && AmmoCurrent > 0); } }

        public WeaponBase(string OwnerName, string WeaponPath)
        {
            this.OwnerName = OwnerName;
            this.WeaponPath = WeaponPath;
        }

        public WeaponBase(BinaryReader BR, string OwnerName, string WeaponPath, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : this(OwnerName, WeaponPath)
        {
            Damage = BR.ReadSingle();
            MaxDurability = BR.ReadSingle();
            MinAngle = BR.ReadSingle();
            MaxAngle = BR.ReadSingle();
            UseRangedProperties = BR.ReadBoolean();
            string SkillChainName = BR.ReadString();

            if (!string.IsNullOrWhiteSpace(SkillChainName) && DicRequirement != null)
            {
                FileStream FSSkillChain = new FileStream("Content/Triple Thunder/Skill Chains/" + SkillChainName + ".pesc", FileMode.Open, FileAccess.Read);
                BinaryReader BRSkillChain = new BinaryReader(FSSkillChain, Encoding.UTF8);
                BRSkillChain.BaseStream.Seek(0, SeekOrigin.Begin);

                int tvSkillsNodesCount = BRSkillChain.ReadInt32();
                ListActiveSkill = new List<BaseAutomaticSkill>(tvSkillsNodesCount);
                for (int N = 0; N < tvSkillsNodesCount; ++N)
                {
                    BaseAutomaticSkill ActiveSkill = new BaseAutomaticSkill(BRSkillChain, DicRequirement, DicEffects, DicAutomaticSkillTarget);

                    InitSkillChainTarget(ActiveSkill, DicAutomaticSkillTarget);

                    ListActiveSkill.Add(ActiveSkill);
                }

                BRSkillChain.Close();
                FSSkillChain.Close();
            }
            else
            {
                ListActiveSkill = new List<BaseAutomaticSkill>();
            }

            ExplosionAttributes = new ExplosionOptions(BR);

            if (UseRangedProperties)
            {
                AmmoPerMagazine = BR.ReadSingle();

                AmmoCurrent = AmmoPerMagazine;

                AmmoRegen = BR.ReadSingle();
                Recoil = BR.ReadSingle();
                MaxRecoil = BR.ReadSingle();
                RecoilRecoverySpeed = BR.ReadSingle();
                NumberOfProjectiles = BR.ReadInt32();
                ProjectileType = (ProjectileTypes)BR.ReadInt32();

                if (ProjectileType == ProjectileTypes.Projectile)
                {
                    ProjectileSize = new Vector2(5, 2);
                    ActiveProjectileInfo = new ProjectileInfo(BR);
                    ArrayProjectileInfo = new ProjectileInfo[1] { ActiveProjectileInfo };

                    NozzleFlashAnimation = new SimpleAnimation();
                    NozzleFlashAnimation.Name = "Nozzle Flash";
                    NozzleFlashAnimation.Path = "Fire 1_strip5";
                    NozzleFlashAnimation.IsLooped = false;
                }
            }
        }

        protected void InitSkillChainTarget(BaseAutomaticSkill ActiveSkill, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            foreach (BaseSkillLevel ActiveSkillLevel in ActiveSkill.ListSkillLevel)
            {
                foreach (BaseSkillActivation ActiveSkillActivation in ActiveSkillLevel.ListActivation)
                {
                    for (int E = 0; E < ActiveSkillActivation.ListEffect.Count; ++E)
                    {
                        if (ActiveSkillActivation.ListEffect[E] is TripleThunderAttackEffect)
                        {
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(DicAutomaticSkillTarget["Self Attack"]);
                        }
                        else if (ActiveSkillActivation.ListEffect[E] is TripleThunderRobotEffect)
                        {
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(DicAutomaticSkillTarget["Self Robot"]);
                        }
                        else if (ActiveSkillActivation.ListEffect[E] is ProjectileEffect)
                        {
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(DicAutomaticSkillTarget["Self Attack"]);
                        }

                        foreach (BaseAutomaticSkill ActiveFollowingSkill in ActiveSkillActivation.ListEffect[E].ListFollowingSkill)
                        {
                            InitSkillChainTarget(ActiveFollowingSkill, DicAutomaticSkillTarget);
                        }
                    }
                }
            }
        }

        public abstract void Load(ContentManager Content);
        public abstract bool CanBeReloaded();
        public abstract bool CanRotateTowardMouse(string ActiveMovementStance);
        public abstract string GetAnimationName(string ActiveMovementStance);
        public abstract AnimationTypes GetAnimationType(string ActiveMovementStance);
        public abstract string GetDefaultAnimationName();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="AttackInput"></param>
        /// <param name="ActiveWeapon"></param>
        /// <returns>Returns true if the attack was used.</returns>
        public abstract bool InitiateAttack(GameTime gameTime, AttackInputs AttackInput, MovementInputs CurrentMovementInput, string ActiveMovementStance, bool ForceCombo, RobotAnimation Owner);
        public abstract void InitiateFollowingAttack(bool IsPartialAnimation, string ActiveMovementStance, RobotAnimation Owner);
        public abstract void OnPartialAnimationLoopEnd(PartialAnimation ActivePartialAnimation, string ActiveMovementStance, RobotAnimation Owner);

        public abstract void Reload(string ActiveMovementStance, RobotAnimation Owner);

        public abstract void ResetAnimation(string ActiveMovementStance);
        public abstract void ResetAnimationToIdle();
        public abstract void Shoot(RobotAnimation Owner, Vector2 GunNozzlePosition, float Angle, List<BaseAutomaticSkill> ListFollowingSkill);
        public abstract void UpdateSkills(string RequirementName);
        public abstract void UpdateWeaponAngle(float Angle, string ActiveMovementStance, VisibleTimeline WeaponSlotTimeline, RobotAnimation Owner);

        public static WeaponBase CreateFromFile(string OwnerName, string WeaponPath, bool IsCharacterAnimation, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            FileStream FS = new FileStream("Content/Triple Thunder/Weapons/" + WeaponPath + ".ttw", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            WeaponBase NewWeapon = null;

            int WeaponType = BR.ReadInt32();

            if (WeaponType == 0)
            {
                NewWeapon = new ComboWeapon(BR, OwnerName, WeaponPath, IsCharacterAnimation, DicRequirement, DicEffects, DicAutomaticSkillTarget);
            }
            else if (WeaponType == 1)
            {
                NewWeapon = new SimpleWeapon(BR, OwnerName, WeaponPath, IsCharacterAnimation, DicRequirement, DicEffects, DicAutomaticSkillTarget);
            }

            BR.Close();
            FS.Close();

            return NewWeapon;
        }
    }
}