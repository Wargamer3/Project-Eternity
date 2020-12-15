using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationSelf : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationSelf()
            : this(null)
        {

        }

        public PilotSkillActivationSelf(DeathmatchContext Context)
            : base("Self", false, Context)
        {

        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            if (Context.EffectOwnerSquad.CurrentLeader == null || Context.EffectOwnerSquad.CurrentLeader.Pilot == null || Context.EffectOwnerSquad.IsDead)
                return false;

            Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter, Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter);
            
            return ActiveSkill.CanActivateEffectsOnTarget(Context.EffectOwnerCharacter.Effects);
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter, Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter);

            AddAndExecuteEffect(ActiveSkill, Context.EffectOwnerCharacter.Effects);
            Context.EffectOwnerCharacter.SP -= ActiveSkill.SPCost;
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationSelf(Context);
        }
    }
}
