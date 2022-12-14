using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.Core
{
    public abstract class Projectile3D : ICollisionObjectHolder3D<Projectile3D>
    {
        public bool IsAlive;
        private bool HasLifetime;
        public float Damage;
        public double Lifetime;
        public double TimeAlive;
        public Vector3 Speed;
        protected bool AffectedByGravity;
        public double DistanceTravelled;

        CollisionObject3D<Projectile3D> CollisionBox;
        public CollisionObject3D<Projectile3D> Collision => CollisionBox;
        public List<BaseAutomaticSkill> ListActiveSkill;

        public Projectile3D()
        {
            HasLifetime = false;
            IsAlive = true;
            AffectedByGravity = false;
            DistanceTravelled = 0;
            Speed = Vector3.Zero;
            ListActiveSkill = new List<BaseAutomaticSkill>();
            CollisionBox = new CollisionObject3D<Projectile3D>();
        }

        public Projectile3D(double Lifetime)
        {
            this.Lifetime = Lifetime;

            HasLifetime = true;
            IsAlive = true;
            AffectedByGravity = false;
            DistanceTravelled = 0;
            TimeAlive = 0;
            Speed = Vector3.Zero;
            ListActiveSkill = new List<BaseAutomaticSkill>();
            CollisionBox = new CollisionObject3D<Projectile3D>();
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
    }
}
