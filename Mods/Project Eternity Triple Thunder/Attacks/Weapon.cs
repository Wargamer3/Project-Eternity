using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.AnimationScreen;
using FMOD;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public class Weapon
    {
        public struct ExplosionOptions
        {
            public float ExplosionRadius;
            public float ExplosionWindPowerAtCenter;
            public float ExplosionWindPowerAtEdge;
            public float ExplosionWindPowerToSelfMultiplier;
            public float ExplosionDamageAtCenter;
            public float ExplosionDamageAtEdge;
            public float ExplosionDamageToSelfMultiplier;
            public string sndExplosionPath;
            public FMODSound sndExplosion;

            public SimpleAnimation ExplosionAnimation;

            public ExplosionOptions(ExplosionOptions Clone)
            {
                ExplosionRadius = Clone.ExplosionRadius;
                ExplosionWindPowerAtCenter = Clone.ExplosionWindPowerAtCenter;
                ExplosionWindPowerAtEdge = Clone.ExplosionWindPowerAtEdge;
                ExplosionWindPowerToSelfMultiplier = Clone.ExplosionWindPowerToSelfMultiplier;
                ExplosionDamageAtCenter = Clone.ExplosionDamageAtCenter;
                ExplosionDamageAtEdge = Clone.ExplosionDamageAtEdge;
                ExplosionDamageToSelfMultiplier = Clone.ExplosionDamageToSelfMultiplier;
                sndExplosionPath = Clone.sndExplosionPath;
                sndExplosion = Clone.sndExplosion;

                ExplosionAnimation = Clone.ExplosionAnimation.Copy();
            }

            public ExplosionOptions(BinaryReader BR)
            {
                ExplosionRadius = BR.ReadSingle();
                ExplosionWindPowerAtCenter = BR.ReadSingle();
                ExplosionWindPowerAtEdge = BR.ReadSingle();
                ExplosionWindPowerToSelfMultiplier = BR.ReadSingle();
                ExplosionDamageAtCenter = BR.ReadSingle();
                ExplosionDamageAtEdge = BR.ReadSingle();
                ExplosionDamageToSelfMultiplier = BR.ReadSingle();
                sndExplosionPath = BR.ReadString();
                
                if (!string.IsNullOrEmpty(sndExplosionPath))
                {
                    sndExplosion = new FMODSound(GameScreen.FMODSystem, "Content/SFX/" + sndExplosionPath);
                }
                else
                {
                    sndExplosion = null;
                }

                ExplosionAnimation = new SimpleAnimation(BR, true);
            }

            public void Save(BinaryWriter BW)
            {
                BW.Write(ExplosionRadius);
                BW.Write(ExplosionWindPowerAtCenter);
                BW.Write(ExplosionWindPowerAtEdge);
                BW.Write(ExplosionWindPowerToSelfMultiplier);
                BW.Write(ExplosionDamageAtCenter);
                BW.Write(ExplosionDamageAtEdge);
                BW.Write(ExplosionDamageToSelfMultiplier);
                BW.Write(sndExplosionPath ?? string.Empty);

                ExplosionAnimation.Save(BW);
            }
        }

        public enum ProjectileTypes { Hitscan, Projectile }

        public readonly string Name;
        public bool IsExtra;

        public Combo NoneCombo;
        public Combo MovingCombo;
        public Combo RunningCombo;
        public Combo DashCombo;
        public Combo AirborneCombo;

        //Weapon attributes
        private float MinAngle;
        private float MaxAngle;
        private float MaxDurability;
        private string MapImagePath;
        public float WeaponAngle;
        public Combo ActiveCombo;
        public Combo NextCombo;

        public ExplosionOptions ExplosionAttributes;

        //Ranged attributes
        private float Damage;
        public float AmmoCurrent;
        public float AmmoPerMagazine;
        private float AmmoRegen;
        public bool IsReloading;
        private float Recoil;
        private float MaxRecoil;
        private float RecoilRecoverySpeed;
        private int NumberOfProjectiles;
        private Vector2 ProjectileSize;
        private ProjectileTypes ProjectileType;
        public Combo ReloadCombo;
        public SimpleAnimation NozzleFlashAnimation;

        private ProjectileInfo[] ArrayProjectileInfo;
        public ProjectileInfo ActiveProjectileInfo;

        public AnimationClass CurrentAnimation;
        private List<BaseAutomaticSkill> ListActiveSkill;
        public bool HasSkills => ListActiveSkill.Count > 0;
        public bool CanBeUsed { get { return AmmoPerMagazine == 0 || (AmmoPerMagazine > 0 && AmmoCurrent > 0); } }

        public Combo ComboByName(string ComboName)
        {
            switch (ComboName)
            {
                case "None":
                    return NoneCombo;

                case "Moving":
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

        /// <summary>
        /// Used by tests
        /// </summary>
        /// <param name="Damage"></param>
        /// <param name="AffectedByGravity"></param>
        /// <param name="ProjectileSpeed"></param>
        public Weapon(float Damage, bool AffectedByGravity, float ProjectileSpeed)
        {
            this.Damage = Damage;
            ActiveProjectileInfo = new ProjectileInfo();
            ActiveProjectileInfo.AffectedByGravity = AffectedByGravity;
            ActiveProjectileInfo.ProjectileSpeed = ProjectileSpeed;
            NumberOfProjectiles = 1;
        }

        public Weapon(string Path, Dictionary<string, BaseSkillRequirement> DicRequirement, Dictionary<string, BaseEffect> DicEffects)
        {
            this.Name = Path;
            FileStream FS = new FileStream("Content/Triple Thunder/Weapons/" + Path + ".ttw", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.UTF8);

            string NoneComboName = BR.ReadString();
            string MovingComboName = BR.ReadString();
            string RunningComboName = BR.ReadString();
            string DashComboName = BR.ReadString();
            string AirborneComboName = BR.ReadString();

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
                    BaseAutomaticSkill ActiveSkill = new BaseAutomaticSkill(BRSkillChain, DicRequirement, DicEffects);

                    InitSkillChainTarget(ActiveSkill);

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
                string ReloadAnimation = BR.ReadString();

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

                if (!string.IsNullOrEmpty(ReloadAnimation))
                    ReloadCombo = new Combo(ReloadAnimation);
            }

            if (!string.IsNullOrEmpty(NoneComboName))
                NoneCombo = new Combo(NoneComboName);

            if (!string.IsNullOrEmpty(MovingComboName))
                MovingCombo = new Combo(MovingComboName);

            if (!string.IsNullOrEmpty(RunningComboName))
                RunningCombo = new Combo(RunningComboName);

            if (!string.IsNullOrEmpty(DashComboName))
                DashCombo = new Combo(DashComboName);

            if (!string.IsNullOrEmpty(AirborneComboName))
                AirborneCombo = new Combo(AirborneComboName);

            BR.Close();
            FS.Close();
        }

        public void Load(ContentManager Content)
        {
            if (Content == null)
            { return; }

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

        public void Shoot(RobotAnimation Owner, Vector2 GunNozzlePosition, float Angle, List<BaseAutomaticSkill> ListFollowingSkill)
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

            Owner.CreateAttackBox(Name, GunNozzlePosition, ListAttack);
        }

        private void InitSkillChainTarget(BaseAutomaticSkill ActiveSkill)
        {
            foreach (BaseSkillLevel ActiveSkillLevel in ActiveSkill.ListSkillLevel)
            {
                foreach (BaseSkillActivation ActiveSkillActivation in ActiveSkillLevel.ListActivation)
                {
                    for (int E = 0; E < ActiveSkillActivation.ListEffect.Count; ++E)
                    {
                        if (ActiveSkillActivation.ListEffect[E] is TripleThunderAttackEffect)
                        {
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(AutomaticSkillTargetType.DicTargetType["Self Attack"]);
                        }
                        else if (ActiveSkillActivation.ListEffect[E] is TripleThunderRobotEffect)
                        {
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(AutomaticSkillTargetType.DicTargetType["Self Robot"]);
                        }
                        else if (ActiveSkillActivation.ListEffect[E] is ProjectileEffect)
                        {
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(AutomaticSkillTargetType.DicTargetType["Self Attack"]);
                        }

                        foreach(BaseAutomaticSkill ActiveFollowingSkill in ActiveSkillActivation.ListEffect[E].ListFollowingSkill)
                        {
                            InitSkillChainTarget(ActiveFollowingSkill);
                        }
                    }
                }
            }
        }

        public Combo ActiveWeaponCombo(string ActiveMovementStance)
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
                    return AirborneCombo;

                default:
                    return NoneCombo;
            }
        }

        public void UpdateSkills(string RequirementName)
        {
            for (int S = 0; S < ListActiveSkill.Count; ++S)
            {
                ListActiveSkill[S].AddSkillEffectsToTarget(RequirementName);
            }
        }
    }
}
