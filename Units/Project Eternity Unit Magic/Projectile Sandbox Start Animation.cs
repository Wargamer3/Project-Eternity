using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Attacks;

namespace ProjectEternity.Units.Magic
{
    public class MagicProjectileSandboxStartAnimation : IProjectileSandbox
    {
        public List<Projectile> ListProjectile;
        public float TotalDamage;
        public float TotalManaCost;
        private Polygon SanboxCollisionBox;
        public double TotalSimulationTime;
        public int TotalSimulationFrames;

        public MagicProjectileSandboxStartAnimation()
        {
            TotalSimulationTime = 100000 * 60;
            ListProjectile = new List<Projectile>();

            Vector2[] SanboxPoints = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(0, Constants.Height),
                new Vector2(Constants.Width, Constants.Height),
                new Vector2(Constants.Width, 0),
            };
            SanboxCollisionBox = new Polygon(SanboxPoints, Constants.Width, Constants.Height);
        }

        public void Reset()
        {
            ListProjectile.Clear();
            TotalDamage = 0;
            TotalManaCost = 0;
        }
        
        public void AddProjectile(Projectile ActiveProjectile)
        {
            TotalDamage += ActiveProjectile.Damage;
            ListProjectile.Add(ActiveProjectile);
        }

        public List<Projectile> SimulateAttack(Attack ActiveAttack)
        {
            bool AttackHasStarted = false;
            double OriginalGametime = Constants.TotalGameTime;
            for (int A = 0; A < ActiveAttack.ArrayAttackAttributes.Length; ++A)
            {
                ActiveAttack.ArrayAttackAttributes[A].AddSkillEffectsToTarget("");
            }

            int MaxNumberOfExecutions = 100000;
            int NumberOfExecutions = 0;
            List<Projectile> ListOutputProjectile = new List<Projectile>();

            while (NumberOfExecutions++ < MaxNumberOfExecutions && (!AttackHasStarted || ListProjectile.Count  != ListOutputProjectile.Count))
            {
                GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, NumberOfExecutions * 16), new TimeSpan(0, 0, 0, 0, 16));
                TotalSimulationTime = gameTime.TotalGameTime.TotalSeconds;
                TotalSimulationFrames = NumberOfExecutions;
                Constants.TotalGameTime = OriginalGametime + gameTime.TotalGameTime.TotalSeconds;

                for (int A = 0; A < ActiveAttack.ArrayAttackAttributes.Length; ++A)
                {
                    ActiveAttack.ArrayAttackAttributes[A].AddSkillEffectsToTarget("");
                }

                if (ListProjectile.Count > 0)
                {
                    AttackHasStarted = true;
                }

                for (int P = 0; P < ListProjectile.Count; P++)
                {
                    Projectile ActiveProjectile = ListProjectile[P];

                    if (ActiveProjectile.IsAlive)
                    {
                        ActiveProjectile.Update(gameTime);
                        foreach (Polygon ProjectileCollision in ActiveProjectile.ListCollisionPolygon)
                        {
                            //Out of bound
                            if (!ListOutputProjectile.Contains(ActiveProjectile) && Polygon.PolygonCollisionSAT(SanboxCollisionBox, ProjectileCollision, Vector2.Zero).Distance < 0)
                            {
                                ListOutputProjectile.Add(ActiveProjectile);
                                ActiveProjectile.IsAlive = false;
                            }
                        }
                    }
                }
            }

            Constants.TotalGameTime = OriginalGametime;

            return ListOutputProjectile;
        }
    }
}
