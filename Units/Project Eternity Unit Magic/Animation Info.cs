using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Magic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.GameScreens.AnimationScreen;
using static ProjectEternity.Core.Projectile2DParams;

namespace ProjectEternity.Units.Magic
{
    public class MagicAttackAnimationInfo : AnimationInfo
    {
        private MagicSpell Owner;
        private Projectile2DContext GlobalContext;
        private SharedProjectileParams SharedParams;

        public MagicAttackAnimationInfo(string AnimationName, MagicSpell Owner, Projectile2DContext GlobalContext, SharedProjectileParams SharedParams)
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
            Vector2 OriginalPosition = SharedParams.OwnerPosition;
            Rectangle EnemyBounds = GetEnemyPosition(NewAnimation);

            MagicProjectileSandboxEndAnimation AnimationEndSanbox = new MagicProjectileSandboxEndAnimation(EnemyBounds);

            SharedParams.OwnerPosition = OriginalPosition;
            SharedParams.OwnerAngle = 0;
            List<Projectile2D> ListProjectileAfterStartAnimationEnded;
            Attack SpellAttack = GetAnimationStartSandbox(Owner, OriginalPosition, out ListProjectileAfterStartAnimationEnded);

            foreach (Projectile2D ActiveProjectile in ListProjectileAfterStartAnimationEnded)
            {
                AnimationEndSanbox.AddProjectile(ActiveProjectile);
            }

            Dictionary<int, Timeline> DicExtraTimeline = new Dictionary<int, Timeline>();

            GlobalContext.OwnerSandbox = AnimationEndSanbox;
            SharedParams.OwnerPosition = Vector2.Zero;
            SharedParams.OwnerAngle = 0;

            AnimationEndSanbox.SimulateAttack(SpellAttack);

            Owner = new MagicSpell(Owner, Owner.GlobalContext.ActiveTarget);
            MagicTimeline NewTimeline = new MagicTimeline(Owner, GlobalContext, SharedParams);

            SharedParams.OwnerPosition = OriginalPosition;
            SharedParams.OwnerAngle = 0;
            GetAnimationStartSandbox(Owner, OriginalPosition, out ListProjectileAfterStartAnimationEnded);

            foreach (Projectile2D ActiveProjectile in ListProjectileAfterStartAnimationEnded)
            {
                NewTimeline.AddProjectile(ActiveProjectile);
            }

            NewTimeline.DeathFrame = AnimationEndSanbox.TotalSimulationFrames;
            NewAnimation.LoopEnd = Math.Max(NewAnimation.LoopEnd, NewTimeline.DeathFrame);
            DicExtraTimeline.Add(0, NewTimeline);

            SharedParams.OwnerPosition = OriginalPosition;
            SharedParams.OwnerAngle = 0;

            return DicExtraTimeline;
        }

        private Attack GetAnimationStartSandbox(MagicSpell ActiveSpell, Vector2 AnimationOrigin, out List<Projectile2D> ListOutputProjectile)
        {
            MagicProjectileSandboxStartAnimation AnimationStartSandbox = new MagicProjectileSandboxStartAnimation();

            GlobalContext.OwnerSandbox = AnimationStartSandbox;
            SharedParams.OwnerPosition = AnimationOrigin;
            SharedParams.OwnerAngle = 0;

            List<BaseAutomaticSkill> ListAttackAttribute = ActiveSpell.ComputeSpell();
            Attack SpellAttack = new Attack("Dummy Attack");
            SpellAttack.ArrayAttackAttributes = ListAttackAttribute.ToArray();

            ListOutputProjectile = AnimationStartSandbox.SimulateAttack(SpellAttack);

            foreach (Projectile2D ActiveProjectile in ListOutputProjectile)
            {
                float MinX = float.MaxValue;
                float MinY = float.MaxValue;
                float MaxX = float.MinValue;
                float MaxY = float.MinValue;

                for (int P = ActiveProjectile.Collision.ListCollisionPolygon.Count - 1; P >= 0; --P)
                {
                    for (int V = ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex.Length - 1; V >= 0; --V)
                    {
                        if (ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].X < MinX)
                            MinX = ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].X;
                        if (ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].X > MaxX)
                            MaxX = ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].X;

                        if (ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].Y < MinY)
                            MinY = ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].Y;
                        if (ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].Y > MaxY)
                            MaxY = ActiveProjectile.Collision.ListCollisionPolygon[P].ArrayVertex[V].Y;
                    }
                }

                //Offset almost completely outside screen
                ActiveProjectile.Offset(new Vector2(Constants.Width + MaxX - 1, 0));
                ActiveProjectile.IsAlive = true;
            }

            return SpellAttack;
        }

        private Rectangle GetEnemyPosition(AnimationClass NewAnimation)
        {
            float MinX = float.MaxValue;
            float MinY = float.MaxValue;
            float MaxX = float.MinValue;
            float MaxY = float.MinValue;

            Vector2 MarkerPosition = NewAnimation.ListAnimationLayer[0].ListActiveMarker[0].Position;
            Vector2 MarkerOriginPosition = NewAnimation.ListAnimationLayer[0].ListActiveMarker[0].AnimationMarker.AnimationOrigin.Position;
            AnimationClass EnemyAnimation = NewAnimation.ListAnimationLayer[0].ListActiveMarker[0].AnimationMarker;

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

            float FinalX = MinX - MarkerOriginPosition.X + MarkerPosition.X;
            float FinalY = MinY - MarkerOriginPosition.Y + MarkerPosition.Y;
            return new Rectangle((int)FinalX, (int)FinalY, (int)(MaxX - MinX), (int)(MaxY - MinY));
        }
    }
}
