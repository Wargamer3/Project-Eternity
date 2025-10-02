using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.Core.Attacks
{
    public struct PERAttackAttributes
    {
        public enum GroundCollisions { DestroySelf, Stop, Bounce }
        public enum AttackTypes { Shoot, Throw, Kick, Hold }

        public float ProjectileSpeed;
        public bool AffectedByGravity;
        public bool CanBeShotDown;
        public bool Homing;
        public byte MaxLifetime;
        public AttackTypes AttackType;

        public SimpleAnimation ProjectileAnimation;
        public string Projectile3DModelPath;
        public AnimatedModel Projectile3DModel;

        public byte NumberOfProjectiles;
        public float MaxLateralSpread;
        public float MaxForwardSpread;
        public float MaxUpwardSpread;

        public string SkillChainName;
        public List<BaseAutomaticSkill> ListActiveSkill;
        public bool HasSkills => ListActiveSkill.Count > 0;

        public GroundCollisions GroundCollision;
        public byte BounceLimit;

        public PERAttackAttributes(BinaryReader BR, ContentManager Content, Dictionary<string, BaseSkillRequirement> DicRequirement,
            Dictionary<string, BaseEffect> DicEffects, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            ProjectileSpeed = BR.ReadSingle();
            AffectedByGravity = BR.ReadBoolean();
            CanBeShotDown = BR.ReadBoolean();
            Homing = BR.ReadBoolean();
            MaxLifetime = BR.ReadByte();
            AttackType = (AttackTypes)BR.ReadByte();

            ProjectileAnimation = new SimpleAnimation(BR, false);
            Projectile3DModelPath = BR.ReadString();
            Projectile3DModel = null;
            if (Content != null)
            {
                if (!string.IsNullOrEmpty(ProjectileAnimation.Path))
                {
                    ProjectileAnimation.Load(Content, "Animations/Sprites/");
                }

                if (!string.IsNullOrEmpty(Projectile3DModelPath))
                {
                    Projectile3DModel = new AnimatedModel("Deathmatch/Attacks/Models/" + Projectile3DModelPath);
                    Projectile3DModel.LoadContent(Content);
                }
            }

            NumberOfProjectiles = BR.ReadByte();
            MaxLateralSpread = BR.ReadSingle();
            MaxForwardSpread = BR.ReadSingle();
            MaxUpwardSpread = BR.ReadSingle();

            SkillChainName = BR.ReadString();

            GroundCollision = (GroundCollisions)BR.ReadByte();
            if (GroundCollision == GroundCollisions.Bounce)
            {
                BounceLimit = BR.ReadByte();
            }
            else
            {
                BounceLimit = 0;
            }

            if (!string.IsNullOrWhiteSpace(SkillChainName) && DicRequirement != null)
            {
                FileStream FSSkillChain = new FileStream("Content/Deathmatch/Attacks/Skill Chains/" + SkillChainName + ".pesc", FileMode.Open, FileAccess.Read);
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
        }

        private void InitSkillChainTarget(BaseAutomaticSkill ActiveSkill, Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget)
        {
            foreach (BaseSkillLevel ActiveSkillLevel in ActiveSkill.ListSkillLevel)
            {
                foreach (BaseSkillActivation ActiveSkillActivation in ActiveSkillLevel.ListActivation)
                {
                    for (int E = 0; E < ActiveSkillActivation.ListEffect.Count; ++E)
                    {
                        if (ActiveSkillActivation.ListEffect[E] is AttackPEREffect)
                        {
                            ActiveSkillActivation.ListEffectTarget[E].Add("Attack PER");
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(DicAutomaticSkillTarget["Attack PER"]);
                        }
                        else if (ActiveSkillActivation.ListEffect[E] is SquadPEREffect)
                        {
                            ActiveSkillActivation.ListEffectTarget[E].Add("Squad PER");
                            ActiveSkillActivation.ListEffectTargetReal[E].Add(DicAutomaticSkillTarget["Squad PER"]);
                        }

                        foreach (BaseAutomaticSkill ActiveFollowingSkill in ActiveSkillActivation.ListEffect[E].ListFollowingSkill)
                        {
                            InitSkillChainTarget(ActiveFollowingSkill, DicAutomaticSkillTarget);
                        }
                    }
                }
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
