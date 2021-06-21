using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.Core.ProjectileParams;

namespace ProjectEternity.Units.Magic
{
    public class MagicAttackAnimationStartInfo : AnimationInfo
    {
        private MagicSpell Owner;
        private ProjectileContext GlobalContext;
        private SharedProjectileParams SharedParams;

        public MagicAttackAnimationStartInfo(string AnimationName, MagicSpell Owner, ProjectileContext GlobalContext, SharedProjectileParams SharedParams)
            : base(AnimationName)
        {
            this.Owner = Owner;
            this.GlobalContext = GlobalContext;
            this.SharedParams = SharedParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Timelines by Key Frames</returns>
        public override Dictionary<int, Timeline> GetExtraTimelines(AnimationClass NewAnimation)
        {
            MagicProjectileSandboxStartAnimation AnimationStartSandbox = GetAnimationStartSandbox(NewAnimation.AnimationOrigin.Position);
            
            Dictionary<int, Timeline> DicExtraTimeline = new Dictionary<int, Timeline>();
            MagicTimeline NewTimeline = new MagicTimeline(new MagicSpell(Owner, Owner.GlobalContext.ActiveTarget), GlobalContext, SharedParams);
            NewTimeline.DeathFrame = AnimationStartSandbox.TotalSimulationFrames;
            NewAnimation.LoopEnd = Math.Max(NewAnimation.LoopEnd, NewTimeline.DeathFrame);

            for(int i = 0; i < NewAnimation.ListAnimationLayer.BasicLayerCount; ++i)
            {
                foreach (KeyValuePair<int, List<Timeline>> Timelines in NewAnimation.ListAnimationLayer[i].DicTimelineEvent)
                {
                    foreach (Timeline ActiveTimeline in Timelines.Value)
                    {
                        ActiveTimeline.DeathFrame = Math.Max(ActiveTimeline.DeathFrame, NewTimeline.DeathFrame);
                    }
                }
            }
            DicExtraTimeline.Add(0, NewTimeline);

            SharedParams.OwnerPosition = NewAnimation.AnimationOrigin.Position;
            SharedParams.OwnerAngle = 0;

            return DicExtraTimeline;
        }

        protected MagicProjectileSandboxStartAnimation GetAnimationStartSandbox(Vector2 AnimationOrigin)
        {
            MagicProjectileSandboxStartAnimation AnimationStartSandbox = new MagicProjectileSandboxStartAnimation();

            GlobalContext.OwnerSandbox = AnimationStartSandbox;
            SharedParams.OwnerPosition = AnimationOrigin;
            SharedParams.OwnerAngle = 0;

            List<BaseAutomaticSkill> ListAttackAttribute = new MagicSpell(Owner, Owner.GlobalContext.ActiveTarget).ComputeSpell();
            Attack SpellAttack = new Attack("Dummy Attack");
            SpellAttack.ArrayAttackAttributes = ListAttackAttribute.ToArray();

            AnimationStartSandbox.SimulateAttack(SpellAttack);

            return AnimationStartSandbox;
        }
    }
}
