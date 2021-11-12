using System;
using System.IO;
using ProjectEternity.Core.Item;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class TripleThunderRobotParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        private readonly TripleThunderRobotContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly TripleThunderRobotContext LocalContext;
        
        public TripleThunderRobotParams(TripleThunderRobotContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;
            LocalContext = new TripleThunderRobotContext();
        }

        public TripleThunderRobotParams(TripleThunderRobotParams Clone)
            : this(Clone.GlobalContext)
        {
            LocalContext.Map = GlobalContext.Map;
            LocalContext.ActiveLayer = GlobalContext.ActiveLayer;
            LocalContext.Target = GlobalContext.Target;
            LocalContext.TargetWeapon = GlobalContext.TargetWeapon;
            LocalContext.TargetWeaponAngle = GlobalContext.TargetWeaponAngle;
            LocalContext.TargetWeaponPosition = GlobalContext.TargetWeaponPosition;
        }
    }

    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class TripleThunderAttackParams : ProjectileParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        private readonly new TripleThunderAttackContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly new TripleThunderAttackContext LocalContext;

        public TripleThunderAttackParams(TripleThunderAttackContext GlobalContext)
            : this(GlobalContext, new TripleThunderAttackContext(), new SharedProjectileParams())
        {
        }

        private TripleThunderAttackParams(TripleThunderAttackContext GlobalContext, TripleThunderAttackContext LocalContext, SharedProjectileParams SharedParams)
            : base(GlobalContext, LocalContext, SharedParams)
        {
            this.GlobalContext = GlobalContext;
            this.LocalContext = LocalContext;
        }

        public TripleThunderAttackParams(TripleThunderAttackParams Clone)
            : this(Clone.GlobalContext, new TripleThunderAttackContext(), Clone.SharedParams)
        {
            LocalContext.Owner = GlobalContext.Owner;
        }
    }

    public class TripleThunderRobotContext
    {
        public FightingZone Map;
        public Layer ActiveLayer;
        public RobotAnimation Target;
        public WeaponBase TargetWeapon;
        public float TargetWeaponAngle;
        public Vector2 TargetWeaponPosition;

        public TripleThunderRobotContext()
        {
        }

        public void SetRobotContext(Layer ActiveLayer, RobotAnimation ActiveRobotAnimation, WeaponBase ActiveWeapon, float Angle, Vector2 Position)
        {
            this.ActiveLayer = ActiveLayer;
            Target = ActiveRobotAnimation;
            TargetWeapon = ActiveWeapon;
            TargetWeaponAngle = Angle;
            TargetWeaponPosition = Position;
        }

        public void SetRobotContext(Layer ActiveLayer, RobotAnimation ActiveRobotAnimation)
        {
            this.ActiveLayer = ActiveLayer;
            Target = ActiveRobotAnimation;
        }
    }

    public class TripleThunderAttackContext : ProjectileContext
    {
        public RobotAnimation Owner;
    }

    public abstract class TripleThunderRobotEffect : BaseEffect
    {
        protected readonly TripleThunderRobotParams Params;

        public TripleThunderRobotEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        /// <summary>
        /// Used for reflection
        /// </summary>
        /// <param name="EffectTypeName"></param>
        /// <param name="EffectContext"></param>
        public TripleThunderRobotEffect(string EffectTypeName, bool IsPassive, TripleThunderRobotParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new TripleThunderRobotParams(Params);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TripleThunderAttackEffect : BaseEffect
    {
        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        protected readonly TripleThunderAttackParams Params;

        public TripleThunderAttackEffect(string EffectTypeName, bool IsPassive)
            : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        public TripleThunderAttackEffect(string EffectTypeName, bool IsPassive, TripleThunderAttackParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
            {
                this.Params = new TripleThunderAttackParams(Params);
            }
        }

        protected override void DoQuickLoad(BinaryReader BR)
        {
        }

        protected override void DoQuickSave(BinaryWriter BW)
        {
        }

        protected override void ReactivateEffect()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class TripleThunderAttackRequirement : BaseSkillRequirement
    {
        public static string OnGroundCollisionAttackName = "On Ground Collision";

        protected readonly TripleThunderAttackContext GlobalContext;

        public TripleThunderAttackRequirement(string EffectTypeName, TripleThunderAttackContext GlobalContext)
            : base(EffectTypeName)
        {
            this.GlobalContext = GlobalContext;
        }
    }

    public abstract class TripleThunderRobotRequirement : BaseSkillRequirement
    {
        public static string OnGroundCollisionName = "On Ground Collision Robot";
        public static string OnWallCollisionName = "On Wall Collision Robot";
        public static string OnCeilingCollisionName = "On Ceiling Collision Robot";
        public static string OnDestroyedName = "On Destroyed Robot";
        public static string OnStepName = "On Step";

        protected readonly TripleThunderRobotContext GlobalContext;

        public TripleThunderRobotRequirement(string EffectTypeName, TripleThunderRobotContext GlobalContext)
            : base(EffectTypeName)
        {
            this.GlobalContext = GlobalContext;
        }
    }

    public abstract class RobotTargetType : AutomaticSkillTargetType
    {
        protected readonly TripleThunderRobotContext GlobalContext;

        public RobotTargetType(string TargetType, TripleThunderRobotContext GlobalContext)
            : base(TargetType)
        {
            this.GlobalContext = GlobalContext;
        }
    }

    public abstract class AttackTargetType : AutomaticSkillTargetType
    {
        protected readonly TripleThunderAttackContext GlobalContext;

        public AttackTargetType(string TargetType, TripleThunderAttackContext GlobalContext)
            : base(TargetType)
        {
            this.GlobalContext = GlobalContext;
        }
    }
}
