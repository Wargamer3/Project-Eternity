using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Magic;

namespace ProjectEternity.Units.Magic
{
    public class MagicProjectileSandbox : IProjectileSandbox
    {
        public List<Projectile> ListProjectile;
        public float TotalDamage;
        public float TotalManaCost;

        public MagicProjectileSandbox()
        {
            ListProjectile = new List<Projectile>();
        }

        public void Reset()
        {
            ListProjectile.Clear();
            TotalDamage = 0;
            TotalManaCost = 0;
        }

        public void Update(GameTime gameTime)
        {
            for (int M = 0; M < ListProjectile.Count; M++)
            {
                Projectile ActiveMagic = ListProjectile[M];

                if (ActiveMagic.IsAlive)
                {
                    ActiveMagic.Update(gameTime);
                }
            }
        }

        public void AddProjectile(Projectile ActiveMagic)
        {
            TotalDamage += ActiveMagic.Damage;
            ListProjectile.Add(ActiveMagic);
        }

        public void SimulateAttack(List<Core.Item.BaseAutomaticSkill> ListAttackAttribute)
        {
            double OriginalGametime = Constants.TotalGameTime;
            for (int A = 0; A < ListAttackAttribute.Count; ++A)
            {
                ListAttackAttribute[A].AddSkillEffectsToTarget("");
            }

            int MaxNumberOfExecutions = 100000;
            int NumberOfExecutions = 0;

            while (NumberOfExecutions++ < MaxNumberOfExecutions && ListProjectile.Count > 0)
            {
                GameTime gameTime = new GameTime(new TimeSpan(0, 0, 0, 0, NumberOfExecutions * 16), new TimeSpan(0, 0, 0, 0, 16));
                Constants.TotalGameTime = OriginalGametime + gameTime.TotalGameTime.TotalSeconds;
                Update(gameTime);
            }

            Constants.TotalGameTime = OriginalGametime;
        }
    }
}
