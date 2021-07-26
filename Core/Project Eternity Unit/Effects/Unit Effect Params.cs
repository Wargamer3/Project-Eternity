namespace ProjectEternity.Core.Effects
{
    /// <summary>
    /// Local parameters used by Effects.
    /// </summary>
    public class UnitEffectParams
    {
        // This class is shared through every SkillEffect used to temporary pass variables to effects.
        // Because it is shared through all effect, its variables will constantly change and must be kept as a member after being activated.
        // There should never be more than one instance of the global context.
        public readonly UnitEffectContext GlobalContext;
        // When an effect is copied to be activated, the global context is copied into the local context.
        // This context is local and can't be changed.
        public readonly UnitEffectContext LocalContext;

        // Used to feed Squads, Units and Character when quick loading.
        public readonly UnitQuickLoadEffectContext GlobalQuickLoadContext;

        public UnitEffectParams(UnitEffectContext GlobalContext, UnitQuickLoadEffectContext GlobalQuickLoadContext)
        {
            this.GlobalContext = GlobalContext;
            this.GlobalQuickLoadContext = GlobalQuickLoadContext;
            LocalContext = new UnitEffectContext();
        }

        public UnitEffectParams(UnitEffectParams Clone)
            : this(Clone.GlobalContext, Clone.GlobalQuickLoadContext)
        {
        }

        internal void CopyGlobalIntoLocal()
        {
            LocalContext.SetContext(GlobalContext.EffectOwnerSquad, GlobalContext.EffectOwnerUnit, GlobalContext.EffectOwnerCharacter,
                 GlobalContext.EffectTargetSquad, GlobalContext.EffectTargetUnit, GlobalContext.EffectTargetCharacter);
        }
    }
}
