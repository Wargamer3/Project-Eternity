using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core
{
    public abstract class Projectile2D : ICollisionObjectHolder2D<Projectile2D>
    {
        public bool IsAlive;
        private bool HasLifetime;
        public float Damage;
        protected double Lifetime;
        public double TimeAlive;
        public Vector2 Speed;
        protected bool AffectedByGravity;
        public double DistanceTravelled;

        CollisionObject2D<Projectile2D> CollisionBox;
        public CollisionObject2D<Projectile2D> Collision => CollisionBox;
        public List<BaseAutomaticSkill> ListActiveSkill;

        public Projectile2D()
        {
            HasLifetime = false;
            IsAlive = true;
            AffectedByGravity = false;
            DistanceTravelled = 0;
            Speed = Vector2.Zero;
            ListActiveSkill = new List<BaseAutomaticSkill>();
            CollisionBox = new CollisionObject2D<Projectile2D>();
        }

        public Projectile2D(double Lifetime)
        {
            this.Lifetime = Lifetime;

            HasLifetime = true;
            IsAlive = true;
            AffectedByGravity = false;
            DistanceTravelled = 0;
            TimeAlive = 0;
            Speed = Vector2.Zero;
            ListActiveSkill = new List<BaseAutomaticSkill>();
            CollisionBox = new CollisionObject2D<Projectile2D>();
        }

        public void Update(GameTime gameTime)
        {
            TimeAlive += gameTime.ElapsedGameTime.TotalSeconds;
            if (HasLifetime)
            {
                Lifetime -= gameTime.ElapsedGameTime.TotalSeconds;
                if (Lifetime <= 0)
                {
                    IsAlive = false;
                }
            }

            DoUpdate(gameTime);
            UpdateSkills("OnStep");
        }

        public abstract void DoUpdate(GameTime gameTime);

        public void UpdateSkills(string RequirementName)
        {
            for (int S = 0; S < ListActiveSkill.Count; ++S)
            {
                ListActiveSkill[S].AddSkillEffectsToTarget(RequirementName);
            }
        }

        public abstract void Draw(CustomSpriteBatch g);

        public abstract void SetAngle(float Angle);

        public void ToggleGravity(bool AffectedByGravity)
        {
            this.AffectedByGravity = AffectedByGravity;
        }

        public void Offset(Vector2 Translation)
        {
            for(int P = Collision.ListCollisionPolygon.Count - 1; P>=0; --P)
            {
                Collision.ListCollisionPolygon[P].Offset(Translation.X, Translation.Y);
            }
        }

        public static Dictionary<string, BaseEffect> GetCoreProjectileEffects(Projectile2DParams Params)
        {
            Dictionary<string, BaseEffect> DicEffect = new Dictionary<string, BaseEffect>();

            DicEffect.Add(ChangeAttackSpeedEffect.Name, new ChangeAttackSpeedEffect(Params));
            DicEffect.Add(BounceAttackOffGroundEffect.Name, new BounceAttackOffGroundEffect(Params));
            DicEffect.Add(RotateAttackEffect.Name, new RotateAttackEffect(Params));
            DicEffect.Add(MatchTerrainTiltWithAttackEffect.Name, new MatchTerrainTiltWithAttackEffect(Params));
            DicEffect.Add(ReviveAttackEffect.Name, new ReviveAttackEffect(Params));
            DicEffect.Add(DestroyAttackEffect.Name, new DestroyAttackEffect(Params));
            DicEffect.Add(ToggleAttackGravityEffect.Name, new ToggleAttackGravityEffect(Params));

            return DicEffect;
        }
    }
}
