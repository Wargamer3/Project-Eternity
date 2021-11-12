using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;
using System.Collections.Generic;
using static ProjectEternity.GameScreens.TripleThunderScreen.ComboWeapon;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public abstract class ComboWeaponBase
    {
        public readonly string OwnerName;
        public readonly string WeaponPath;
        public string WeaponName;
        public bool IsExtra;
        public float WeaponAngle;

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

        public ComboWeaponBase(string OwnerName, string WeaponPath)
        {
            this.OwnerName = OwnerName;
            this.WeaponPath = WeaponPath;
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
        public abstract Combo GetActiveWeaponCombo(string ActiveMovementStance);
        public abstract string GetAnimationName(string ActiveMovementStance);
        public abstract AnimationTypes GetAnimationType(string ActiveMovementStance);
        public abstract string GetDefaultAnimationName();
        public abstract bool InitiateAttack(GameTime gameTime, AttackInputs AttackInput, MovementInputs CurrentMovementInput, string ActiveMovementStance, bool ForceCombo, RobotAnimation Owner);
        public abstract void InitiateFollowingAttack(bool IsPartialAnimation, string ActiveMovementStance, RobotAnimation Owner);
        public abstract void OnPartialAnimationLoopEnd(PartialAnimation ActivePartialAnimation, string ActiveMovementStance, RobotAnimation Owner);

        public abstract void Reload(string ActiveMovementStance, RobotAnimation Owner);

        public abstract void ResetAnimation(string ActiveMovementStance);
        public abstract void ResetAnimationToIdle();
        public abstract void Shoot(RobotAnimation Owner, Vector2 GunNozzlePosition, float Angle, List<BaseAutomaticSkill> ListFollowingSkill);
        public abstract void UpdateSkills(string RequirementName);
        public abstract void UpdateWeaponAngle(float Angle, string ActiveMovementStance, VisibleTimeline WeaponSlotTimeline, RobotAnimation Owner);
    }
}