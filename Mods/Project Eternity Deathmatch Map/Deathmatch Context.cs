using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.BattleMapScreen;
using System.Collections.Generic;
using System.IO;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    /// <summary>
    /// Used to pass battle information to Skills.
    /// </summary>
    public class DeathmatchContext : BattleContext
    {
        public DeathmatchMap Map;

        public DeathmatchContext()
            : base()
        {
            Map = null;
        }
    }

    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class DeathmatchParams
    {
        // This class is shared through every RobotEffects used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly DeathmatchContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly DeathmatchContext LocalContext;

        public DeathmatchParams(DeathmatchContext GlobalContext)
        {
            this.GlobalContext = GlobalContext;
            LocalContext = new DeathmatchContext();
        }

        public DeathmatchParams(DeathmatchParams Clone)
            : this(Clone.GlobalContext)
        {
        }

        internal void CopyGlobalIntoLocal()
        {
            LocalContext.Map = GlobalContext.Map;
            LocalContext.SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter,
                GlobalContext.EffectTargetSquad, GlobalContext.EffectTargetUnit, GlobalContext.EffectTargetCharacter);

            LocalContext.Result = GlobalContext.Result;
            LocalContext.EnemyResult = GlobalContext.EnemyResult;
            LocalContext.SupportAttack = GlobalContext.SupportAttack;
            LocalContext.SupportDefend = GlobalContext.SupportDefend;
            LocalContext.ArrayAttackPosition = new Microsoft.Xna.Framework.Vector3[GlobalContext.ArrayAttackPosition.Length];
            GlobalContext.ArrayAttackPosition.CopyTo(LocalContext.ArrayAttackPosition, 0);
        }
    }
    public abstract class DeathmatchEffect : BaseEffect
    {
        /// <summary>
        /// Should only use the Local Context when inside the DoExecuteEffect method.
        /// Should only use the Global Context when inside the CanActivate method.
        /// </summary>
        protected readonly DeathmatchParams Params;

        public DeathmatchEffect(string EffectTypeName, bool IsPassive)
           : base(EffectTypeName, IsPassive)
        {
            Params = null;
        }

        public DeathmatchEffect(string EffectTypeName, bool IsPassive, DeathmatchParams Params)
            : base(EffectTypeName, IsPassive)
        {
            if (Params != null)
                this.Params = new DeathmatchParams(Params);
        }

        /// <summary>
        /// Used for Copy.
        /// </summary>
        /// <param name="EffectTypeName"></param>
        /// <param name="Copy"></param>
        protected DeathmatchEffect(string EffectTypeName, bool IsPassive, DeathmatchEffect Copy)
            : this(EffectTypeName, IsPassive, Copy.Params)
        {
            if (Params != null)
            {
                Params.CopyGlobalIntoLocal();
                if (Params.LocalContext.EffectOwnerUnit != null && GameScreen.Debug != null)
                {
                    List<string> ListDebugText = new List<string>();
                    ListDebugText.Add("The context used was:");

                    if (this.Params.LocalContext.EffectOwnerSquad != null && !string.IsNullOrEmpty(this.Params.LocalContext.EffectOwnerSquad.SquadName))
                        ListDebugText.Add("Owner Squad: " + this.Params.LocalContext.EffectOwnerSquad.SquadName);
                    if (this.Params.LocalContext.EffectOwnerUnit != null)
                        ListDebugText.Add("Owner Unit: " + this.Params.LocalContext.EffectOwnerUnit.RelativePath);
                    if (this.Params.LocalContext.EffectOwnerCharacter != null)
                        ListDebugText.Add("Owner Pilot: " + this.Params.LocalContext.EffectOwnerCharacter.FullName);

                    if (this.Params.LocalContext.EffectTargetSquad != null && !string.IsNullOrEmpty(this.Params.LocalContext.EffectTargetSquad.SquadName))
                        ListDebugText.Add("Target Squad: " + this.Params.LocalContext.EffectTargetSquad.SquadName);
                    if (this.Params.LocalContext.EffectTargetUnit != null)
                        ListDebugText.Add("Target Unit: " + this.Params.LocalContext.EffectTargetUnit.RelativePath);
                    if (this.Params.LocalContext.EffectTargetCharacter != null)
                        ListDebugText.Add("Target Pilot: " + this.Params.LocalContext.EffectTargetCharacter.FullName);

                    GameScreens.GameScreen.Debug.AddDebugEffect(this, ListDebugText);
                }
            }
        }

        protected override void Load(BinaryReader BR)
        {
            if (LifetimeType == Core.Effects.SkillEffect.LifetimeTypePermanent)
                LifetimeTypeValue = -1;
        }

        public override bool CanActivate()
        {
            return true;
        }
    }
}
