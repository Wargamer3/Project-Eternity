using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class SimpleWeapon : WeaponBase
    {
        private string HoldingAnimationName;
        private string ShootingAnimationName;
        private string ReloadAnimationName;

        public AnimationTypes AnimationType;
        public ComboRotationTypes ComboRotationType;
        public bool InstantActivation;

        bool IsShooting;
        bool IsShootingNext;

        public SimpleWeapon(string OwnerName, string WeaponPath, bool IsCharacterAnimation, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(OwnerName, WeaponPath)
        {
            FileStream FS = new FileStream("Content/Triple Thunder/Weapons/" + WeaponPath + ".ttw", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            Damage = BR.ReadSingle();
            MaxDurability = BR.ReadSingle();
            MinAngle = BR.ReadSingle();
            MaxAngle = BR.ReadSingle();
            bool UseRangedProperties = BR.ReadBoolean();
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

            BR.Close();
            FS.Close();
        }

        public override void Load(ContentManager Content)
        {
            if (Content == null)
            { return; }

            string WeaponPath = "Animations/Sprites/Triple Thunder/Weapons/" + WeaponName;
            if (File.Exists("Content/" + WeaponPath + ".xnb"))
            {
                sprMapIcon = Content.Load<Texture2D>(WeaponPath);
            }

            if (ActiveProjectileInfo != null)
            {
                ActiveProjectileInfo.Load(Content);
            }

            if (NozzleFlashAnimation != null && NozzleFlashAnimation.Path != string.Empty)
            {
                NozzleFlashAnimation.Load(Content, "Animations/Sprites/");
            }
            if (ExplosionAttributes.ExplosionAnimation.Path != string.Empty)
            {
                ExplosionAttributes.ExplosionAnimation.Load(Content, "Animations/Sprites/");
            }
        }

        public override void Shoot(RobotAnimation Owner, Vector2 GunNozzlePosition, float Angle, List<BaseAutomaticSkill> ListFollowingSkill)
        {
            List<AttackBox> ListAttack = new List<AttackBox>(NumberOfProjectiles);
            for (int i = NumberOfProjectiles - 1; i >= 0; --i)
            {
                AttackBox NewAttack;

                if (ProjectileType == ProjectileTypes.Hitscan)
                {
                    NewAttack = new HitscanBox(Damage, ExplosionAttributes, Owner, GunNozzlePosition, Angle);
                }
                else
                {
                    NewAttack = new ProjectileBox(Damage, ExplosionAttributes, Owner, GunNozzlePosition, ProjectileSize, Angle, ActiveProjectileInfo);
                }

                //Clone the following skills so they are not share by every bullets.
                NewAttack.ListActiveSkill = new List<BaseAutomaticSkill>(ListFollowingSkill.Count);
                foreach (BaseAutomaticSkill ActiveFollowingSkill in ListFollowingSkill)
                {
                    NewAttack.ListActiveSkill.Add(new BaseAutomaticSkill(ActiveFollowingSkill));
                }

                ListAttack.Add(NewAttack);
            }

            Owner.CreateAttackBox(WeaponPath, GunNozzlePosition, ListAttack);
        }

        public override string GetAnimationName(string ActiveMovementStance)
        {
            return HoldingAnimationName;
        }

        public override AnimationTypes GetAnimationType(string ActiveMovementStance)
        {
            return AnimationType;
        }

        public override string GetDefaultAnimationName()
        {
            return HoldingAnimationName;
        }

        public override void ResetAnimationToIdle()
        {
        }

        public override void ResetAnimation(string ActiveMovementStance)
        {
        }

        public override bool CanRotateTowardMouse(string ActiveMovementStance)
        {
            return ComboRotationType != ComboRotationTypes.None;
        }

        public override void UpdateWeaponAngle(float Angle, string ActiveMovementStance, VisibleTimeline WeaponSlotTimeline, RobotAnimation Owner)
        {
            if (CurrentAnimation == null)
                return;

            VisibleTimeline WeaponTimeline = CurrentAnimation.AnimationOrigin;
            ComboRotationTypes ActiveComboRotationTypes = ComboRotationTypes.RotateAroundWeaponSlot;

            if (WeaponSlotTimeline != null)
            {
                float TranslationX = WeaponTimeline.Position.X;
                float TranslationY = WeaponTimeline.Position.Y;

                if (ActiveComboRotationTypes == ComboRotationTypes.RotateAroundWeaponSlot)
                {
                    CurrentAnimation.TransformationMatrix =
                        Matrix.CreateTranslation(-TranslationX, -TranslationY, 0)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateTranslation(WeaponSlotTimeline.Position.X,
                                                   WeaponSlotTimeline.Position.Y, 0);
                }
                else if (ActiveComboRotationTypes == ComboRotationTypes.RotateAroundRobot)
                {
                    Vector2 WeaponOffset = WeaponSlotTimeline.Position - Owner.AnimationOrigin.Position;
                    float ExtraAngle = (float)Math.Atan2(WeaponOffset.Y, WeaponOffset.X);
                    float WeaponLength = WeaponOffset.Length();

                    double LenghtDirX = Math.Cos(Angle + ExtraAngle) * WeaponLength;
                    double LenghtDirY = Math.Sin(Angle + ExtraAngle) * WeaponLength;

                    Vector2 RealGunNozzlePosition = Owner.AnimationOrigin.Position
                        + new Vector2((float)(LenghtDirX), (float)(LenghtDirY));

                    CurrentAnimation.TransformationMatrix =
                        Matrix.CreateTranslation(-TranslationX, -TranslationY, 0)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateTranslation(RealGunNozzlePosition.X,
                                                   RealGunNozzlePosition.Y, 0);
                }
                else
                {
                    CurrentAnimation.TransformationMatrix =
                        Matrix.CreateTranslation(-TranslationX, -TranslationY, 0)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateTranslation(WeaponSlotTimeline.Position.X,
                                                   WeaponSlotTimeline.Position.Y, 0);
                }
            }
            else
            {
                CurrentAnimation.TransformationMatrix =
                    Matrix.CreateScale(0f);
            }
        }

        public override void Reload(string ActiveMovementStance, RobotAnimation Owner)
        {
            IsReloading = true;
            AmmoCurrent = AmmoPerMagazine;
            if (CanBeReloaded())
            {
                InitiateFollowingAttack(true, ActiveMovementStance, Owner);
            }
        }

        public override bool CanBeReloaded()
        {
            return true;
        }

        public override void OnPartialAnimationLoopEnd(PartialAnimation ActivePartialAnimation, string ActiveMovementStance, RobotAnimation Owner)
        {
            if (IsReloading)
            {
                if (CanBeReloaded())
                {
                    if (CurrentAnimation.AnimationPath == ReloadAnimationName)
                    {
                        IsReloading = false;
                        Owner.ActivatePartialWeapon(this, GetAnimationName(ActiveMovementStance));
                    }
                    else
                    {
                        InitiateFollowingAttack(true, ActiveMovementStance, Owner);
                    }
                }
                else
                {
                    IsReloading = false;
                }
            }
            else if (CurrentAnimation == ActivePartialAnimation)
            {
                CurrentAnimation = null;
                InitiateFollowingAttack(true, ActiveMovementStance, Owner);
                if (CurrentAnimation == null)
                {
                    Owner.ActivatePartialWeapon(this, GetAnimationName(ActiveMovementStance));
                }
            }
        }

        public override bool InitiateAttack(GameTime gameTime, AttackInputs AttackInput, MovementInputs CurrentMovementInput, string ActiveMovementStance, bool ForceCombo, RobotAnimation Owner)
        {
            if (!IsShootingNext)
            {
                if (IsShooting)
                {
                    IsShootingNext = true;

                    if (InstantActivation)
                    {
                        InitiateFollowingAttack(AnimationType == AnimationTypes.PartialAnimation, ActiveMovementStance, Owner);
                        return true;
                    }
                }
                //First use of a combo, use it immediatly.
                else
                {
                    IsShootingNext = true;
                    IsShooting = true;

                    InitiateFollowingAttack(AnimationType == AnimationTypes.PartialAnimation, ActiveMovementStance, Owner);
                    return true;
                }
            }

            return false;
        }

        public override void InitiateFollowingAttack(bool IsPartialAnimation, string ActiveMovementStance, RobotAnimation Owner)
        {
            if (!CanBeUsed)
            {
                Owner.Reload();
                return;
            }

            bool CanUseNextCombo = false;

            if (IsShooting)
            {
                if (AnimationType == AnimationTypes.PartialAnimation == IsPartialAnimation)
                {
                    CanUseNextCombo = true;
                }
                else
                {
                    if ((AnimationType == AnimationTypes.PartialAnimation) == IsPartialAnimation)
                    {
                        CanUseNextCombo = true;
                    }
                }

                if (CanUseNextCombo)
                {
                    if (AnimationType == AnimationTypes.PartialAnimation)
                    {
                        Owner.RemovePartialAnimation(HoldingAnimationName);
                        Owner.RemovePartialAnimation(ShootingAnimationName);
                        Owner.ActivatePartialWeapon(this, ShootingAnimationName);
                        CurrentAnimation.ActiveKeyFrame++;
                    }
                    else
                    {
                        Owner.LockAnimation = true;
                        Owner.ActivatePartialWeapon(this, ShootingAnimationName);
                    }
                }
            }

            IsShooting = IsShootingNext;
            IsShootingNext = false;
        }

        public override void UpdateSkills(string RequirementName)
        {
            for (int S = 0; S < ListActiveSkill.Count; ++S)
            {
                ListActiveSkill[S].AddSkillEffectsToTarget(RequirementName);
            }
        }
    }
}
