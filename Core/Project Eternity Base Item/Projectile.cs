using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.Core
{
    public interface IProjectileSandbox
    {
        void AddProjectile(Projectile NewProjectile);
    }

    public abstract class Projectile
    {
        public bool IsAlive;
        private bool HasLifetime;
        public float Damage;
        protected double Lifetime;
        public double TimeAlive;
        public Vector2 Speed;
        protected bool AffectedByGravity;
        public double DistanceTravelled;
        public List<Polygon> ListCollisionPolygon;
        public List<BaseAutomaticSkill> ListActiveSkill;

        public Projectile()
        {
            HasLifetime = false;
            IsAlive = true;
            AffectedByGravity = false;
            DistanceTravelled = 0;
            Speed = Vector2.Zero;
            ListActiveSkill = new List<BaseAutomaticSkill>();
        }

        public Projectile(double Lifetime)
        {
            this.Lifetime = Lifetime;

            HasLifetime = true;
            IsAlive = true;
            AffectedByGravity = false;
            DistanceTravelled = 0;
            TimeAlive = 0;
            Speed = Vector2.Zero;
            ListActiveSkill = new List<BaseAutomaticSkill>();
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
            for(int P = ListCollisionPolygon.Count - 1; P>=0; --P)
            {
                ListCollisionPolygon[P].Offset(Translation.X, Translation.Y);
            }
        }

        public static Dictionary<string, BaseEffect> GetCoreProjectileEffects(ProjectileParams Params)
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


    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class ProjectileParams
    {
        public class SharedProjectileParams
        {
            public Vector2 OwnerPosition;
            public float OwnerAngle;
            public ContentManager Content;
        }

        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        protected readonly ProjectileContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly ProjectileContext LocalContext;

        //Global params that can't be cached in the LocalContext.
        public readonly SharedProjectileParams SharedParams;

        public ProjectileParams(ProjectileContext GlobalContext)
            : this(GlobalContext, new ProjectileContext(), new SharedProjectileParams())
        {
        }

        public ProjectileParams(ProjectileParams Clone)
            : this(Clone.GlobalContext)
        {
            SharedParams = Clone.SharedParams;
        }

        protected ProjectileParams(ProjectileContext GlobalContext, ProjectileContext LocalContext, SharedProjectileParams SharedParams)
        {
            this.GlobalContext = GlobalContext;
            this.LocalContext = LocalContext;
            this.SharedParams = SharedParams;

            LocalContext.OwnerSandbox = GlobalContext.OwnerSandbox;
            LocalContext.OwnerProjectile = GlobalContext.OwnerProjectile;
        }
    }

    public class ProjectileContext
    {
        public IProjectileSandbox OwnerSandbox;
        public Projectile OwnerProjectile;
    }
}
