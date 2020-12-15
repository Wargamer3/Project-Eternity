using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Magic;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.Core.ProjectileParams;

namespace ProjectEternity.Units.Magic
{
    public class MagicTimelineEndHitAnimation : MagicTimeline
    {
        private AnimationClass Owner;

        public MagicTimelineEndHitAnimation(AnimationClass Owner, MagicSpell ActiveSpell, ProjectileContext GlobalContext, SharedProjectileParams SharedParams)
            : base(ActiveSpell, GlobalContext, SharedParams)
        {
            this.Owner = Owner;
        }
        
        public override Timeline Copy()
        {
            MagicTimeline NewTimelineEvent = new MagicTimeline();

            NewTimelineEvent.UpdateFrom(this);

            return NewTimelineEvent;
        }

        public override List<VisibleTimeline> CreateNewEditorItem(AnimationClass ActiveAnimation, AnimationClass.AnimationLayer ActiveLayer, int KeyFrame, System.Drawing.Point MousePosition)
        {
            List<VisibleTimeline> ReturnValue = new List<VisibleTimeline>();

            return ReturnValue;
        }

        public override void UpdateAnimationObject(int KeyFrame)
        {
            SharedParams.OwnerPosition = new Vector2();
            base.UpdateAnimationObject(KeyFrame);
        }

        private Rectangle GetEnemyPosition()
        {
            float MinX = float.MaxValue;
            float MinY = float.MaxValue;
            float MaxX = float.MinValue;
            float MaxY = float.MinValue;
            
            AnimationClass EnemyAnimation = Owner.ListAnimationLayer[0].ListActiveMarker[0].AnimationMarker;

            foreach (VisibleTimeline ActiveObject in EnemyAnimation.ListAnimationLayer[0].ListVisibleObject)
            {
                if (ActiveObject.Position.X < MinX)
                    MinX = ActiveObject.Position.X;
                if (ActiveObject.Position.X + ActiveObject.Width > MaxX)
                    MaxX = ActiveObject.Position.X + ActiveObject.Width;

                if (ActiveObject.Position.Y < MinY)
                    MinY = ActiveObject.Position.Y;
                if (ActiveObject.Position.Y + ActiveObject.Height > MaxY)
                    MaxY = ActiveObject.Position.Y + ActiveObject.Height;
            }

            return new Rectangle((int)MinX, (int)MinY, (int)(MaxX - MinX), (int)(MaxY - MinY));
        }
    }
}
