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
    public class ComboWeapon : WeaponBase
    {
        private Combo NoneCombo;
        private Combo MovingCombo;
        private Combo RunningCombo;
        private Combo DashCombo;
        private Combo AirborneCombo;
        private Combo ActiveCombo;
        private Combo NextCombo;
        private Combo ReloadCombo;

        public override bool IsShooting => ActiveCombo != null;

        /// <summary>
        /// Used by tests
        /// </summary>
        /// <param name="Damage"></param>
        /// <param name="AffectedByGravity"></param>
        /// <param name="ProjectileSpeed"></param>
        public ComboWeapon(float Damage, bool AffectedByGravity, float ProjectileSpeed)
            : base(null, null)
        {
            this.Damage = Damage;
            ActiveProjectileInfo = new ProjectileInfo();
            ActiveProjectileInfo.AffectedByGravity = AffectedByGravity;
            ActiveProjectileInfo.ProjectileSpeed = ProjectileSpeed;
            NumberOfProjectiles = 1;
        }

        public ComboWeapon(BinaryReader BR, string OwnerName, string WeaponPath, bool IsCharacterAnimation, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
            : base(BR, OwnerName, WeaponPath, DicRequirement, DicEffects, DicAutomaticSkillTarget)
        {
            if (UseRangedProperties)
            {
                string ReloadAnimation = WeaponPath + "/Reloading";

                if (File.Exists("Content/Triple Thunder/Combos/" + ReloadAnimation + ".ttc"))
                    ReloadCombo = new Combo(ReloadAnimation);
            }

            string NoneComboName;
            string MovingComboName;
            string RunningComboName;
            string DashComboName;
            string AirborneComboName;

            if (IsCharacterAnimation)
            {
                NoneComboName = WeaponPath + "/Idle";
                MovingComboName = WeaponPath + "/Move";
                RunningComboName = WeaponPath + "/Move";
                DashComboName = WeaponPath + "/Move";
                AirborneComboName = WeaponPath + "/Idle";
            }
            else
            {
                NoneComboName = WeaponPath + "/Holding";
                MovingComboName = WeaponPath + "/Holding";
                RunningComboName = WeaponPath + "/Holding";
                DashComboName = WeaponPath + "/Holding";
                AirborneComboName = WeaponPath + "/Holding";
            }

            if (!string.IsNullOrEmpty(NoneComboName) && File.Exists("Content/Triple Thunder/Combos/" + NoneComboName + ".ttc"))
                NoneCombo = new Combo(NoneComboName);

            if (!string.IsNullOrEmpty(MovingComboName) && File.Exists("Content/Triple Thunder/Combos/" + MovingComboName + ".ttc"))
                MovingCombo = new Combo(MovingComboName);

            if (!string.IsNullOrEmpty(RunningComboName) && File.Exists("Content/Triple Thunder/Combos/" + RunningComboName + ".ttc"))
                RunningCombo = new Combo(RunningComboName);

            if (!string.IsNullOrEmpty(DashComboName) && File.Exists("Content/Triple Thunder/Combos/" + DashComboName + ".ttc"))
                DashCombo = new Combo(DashComboName);

            if (!string.IsNullOrEmpty(AirborneComboName) && File.Exists("Content/Triple Thunder/Combos/" + AirborneComboName + ".ttc"))
                AirborneCombo = new Combo(AirborneComboName);
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

        public Combo GetActiveWeaponCombo(string ActiveMovementStance)
        {
            switch (ActiveMovementStance)
            {
                case "None":
                    return NoneCombo;

                case "Walking":
                    return MovingCombo;

                case "Running":
                    return RunningCombo;

                case "Dash":
                    return DashCombo;

                case "Airborne":
                    if (AirborneCombo == null)
                        return NoneCombo;
                    return AirborneCombo;

                default:
                    return NoneCombo;
            }
        }

        public override string GetAnimationName(string ActiveMovementStance)
        {
            switch (ActiveMovementStance)
            {
                case "None":
                    return NoneCombo.AnimationName;

                case "Moving":
                    return MovingCombo.AnimationName;

                case "Running":
                    return RunningCombo.AnimationName;

                case "Dash":
                    return DashCombo.AnimationName;

                case "Airborne":
                    if (AirborneCombo == null)
                        return NoneCombo.AnimationName;
                    return AirborneCombo.AnimationName;

                default:
                    return NoneCombo.AnimationName;
            }
        }

        public override AnimationTypes GetAnimationType(string ActiveMovementStance)
        {
            if (ActiveCombo == null)
            {
                return AnimationTypes.Null;
            }

            return GetActiveWeaponCombo(ActiveMovementStance).AnimationType;
        }

        public override string GetDefaultAnimationName()
        {
            return NoneCombo.AnimationName;
        }

        public override void ResetAnimationToIdle()
        {
            ActiveCombo = NoneCombo;
        }

        public override void ResetAnimation(string ActiveMovementStance)
        {
            Combo ActiveWeaponCombo = GetActiveWeaponCombo(ActiveMovementStance);
            ActiveWeaponCombo.Reset();
        }

        public override bool CanRotateTowardMouse(string ActiveMovementStance)
        {
            bool RotateTowardMouse = true;

            if (GetActiveWeaponCombo(ActiveMovementStance) != null)
            {
                RotateTowardMouse = GetActiveWeaponCombo(ActiveMovementStance).ComboRotationType != ComboRotationTypes.None;
            }
            if (ActiveCombo != null)
            {
                RotateTowardMouse = ActiveCombo.ComboRotationType != ComboRotationTypes.None;
            }

            return RotateTowardMouse;
        }

        public override void UpdateWeaponAngle(float Angle, string ActiveMovementStance, VisibleTimeline WeaponSlotTimeline, RobotAnimation Owner)
        {
            if (CurrentAnimation == null)
                return;

            VisibleTimeline WeaponTimeline = CurrentAnimation.AnimationOrigin;
            Combo ActiveWeaponCombo = GetActiveWeaponCombo(ActiveMovementStance);

            if (ActiveWeaponCombo != null && WeaponSlotTimeline != null)
            {
                float TranslationX = WeaponTimeline.Position.X;
                float TranslationY = WeaponTimeline.Position.Y;

                if (ActiveWeaponCombo.ComboRotationType == ComboRotationTypes.RotateAroundWeaponSlot)
                {
                    CurrentAnimation.TransformationMatrix2D =
                        Matrix.CreateTranslation(-TranslationX, -TranslationY, 0)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateTranslation(WeaponSlotTimeline.Position.X,
                                                   WeaponSlotTimeline.Position.Y, 0);
                }
                else if (ActiveWeaponCombo.ComboRotationType == ComboRotationTypes.RotateAroundRobot)
                {
                    Vector2 WeaponOffset = WeaponSlotTimeline.Position - Owner.AnimationOrigin.Position;
                    float ExtraAngle = (float)Math.Atan2(WeaponOffset.Y, WeaponOffset.X);
                    float WeaponLength = WeaponOffset.Length();

                    double LenghtDirX = Math.Cos(Angle + ExtraAngle) * WeaponLength;
                    double LenghtDirY = Math.Sin(Angle + ExtraAngle) * WeaponLength;

                    Vector2 RealGunNozzlePosition = Owner.AnimationOrigin.Position
                        + new Vector2((float)(LenghtDirX), (float)(LenghtDirY));

                    CurrentAnimation.TransformationMatrix2D =
                        Matrix.CreateTranslation(-TranslationX, -TranslationY, 0)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateTranslation(RealGunNozzlePosition.X,
                                                   RealGunNozzlePosition.Y, 0);
                }
                else
                {
                    CurrentAnimation.TransformationMatrix2D =
                        Matrix.CreateTranslation(-TranslationX, -TranslationY, 0)
                        * Matrix.CreateRotationZ(Angle)
                        * Matrix.CreateTranslation(WeaponSlotTimeline.Position.X,
                                                   WeaponSlotTimeline.Position.Y, 0);
                }
            }
            else
            {
                CurrentAnimation.TransformationMatrix2D =
                    Matrix.CreateScale(0f);
            }
        }

        public override void Reload(string ActiveMovementStance, RobotAnimation Owner)
        {
            IsReloading = true;
            AmmoCurrent = AmmoPerMagazine;
            if (ReloadCombo != null)
            {
                NextCombo = ReloadCombo;
                InitiateFollowingAttack(true, ActiveMovementStance, Owner);
            }
        }

        public override bool CanBeReloaded()
        {
            return ReloadCombo != null;
        }

        public override void OnPartialAnimationLoopEnd(PartialAnimation ActivePartialAnimation, string ActiveMovementStance, RobotAnimation Owner)
        {
            if (IsReloading)
            {
                if (ReloadCombo != null)
                {
                    if (CurrentAnimation.AnimationPath == ReloadCombo.AnimationName)
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
            else
            {

            }
        }

        public override bool InitiateAttack(GameTime gameTime, AttackInputs AttackInput, MovementInputs CurrentMovementInput, string ActiveMovementStance, bool ForceCombo, RobotAnimation Owner)
        {
            //Only get the next combo if it is not set to avoid overriding it.
            if (NextCombo == null)
            {
                // Already using a combo, fetch the next combo.
                if (ActiveCombo != null)
                {
                    NextCombo = ActiveCombo.GetNextCombo(AttackInput, CurrentMovementInput, gameTime, ForceCombo, Owner);

                    if (NextCombo != null && NextCombo.InstantActivation)
                    {
                        InitiateFollowingAttack(NextCombo.AnimationType == AnimationTypes.PartialAnimation, ActiveMovementStance, Owner);
                    }
                }
                //First use of a combo, use it immediatly.
                else
                {
                    Combo ActiveWeaponCombo = GetActiveWeaponCombo(ActiveMovementStance);

                    if (ActiveWeaponCombo != null)
                    {
                        NextCombo = ActiveWeaponCombo.GetNextCombo(AttackInput, CurrentMovementInput, gameTime, ForceCombo, Owner);
                        if (NextCombo != null)
                        {
                            ActiveCombo = ActiveWeaponCombo;
                            InitiateFollowingAttack(NextCombo.AnimationType == AnimationTypes.PartialAnimation, ActiveMovementStance, Owner);
                            return true;
                        }
                    }
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

            if (ActiveCombo != null)
            {
                if (ActiveCombo.AnimationType == AnimationTypes.PartialAnimation == IsPartialAnimation)
                {
                    CanUseNextCombo = true;
                }
                else
                {
                    Combo ActiveWeaponCombo = GetActiveWeaponCombo(ActiveMovementStance);

                    if (ActiveWeaponCombo != null)
                    {
                        if ((ActiveCombo.AnimationType == AnimationTypes.PartialAnimation) == IsPartialAnimation)
                        {
                            CanUseNextCombo = true;
                            ActiveWeaponCombo.Reset();
                        }
                    }
                }

                if (CanUseNextCombo && NextCombo != null)
                {
                    if (NextCombo.AnimationType == AnimationTypes.PartialAnimation)
                    {
                        Owner.RemovePartialAnimation(ActiveCombo.AnimationName);
                        Owner.ActivatePartialWeapon(this, NextCombo.AnimationName);
                        CurrentAnimation.ActiveKeyFrame++;
                    }
                    else
                    {
                        Owner.LockAnimation = true;
                        Owner.ActivatePartialWeapon(this, NextCombo.AnimationName);
                    }
                }
            }

            ActiveCombo = NextCombo;
            NextCombo = null;
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
