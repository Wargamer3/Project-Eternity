using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationSelfSquad : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationSelfSquad()
            : this(null)
        {

        }

        public PilotSkillActivationSelfSquad(DeathmatchContext Context)
            : base("Self Squad", false, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            if (Context.EffectOwnerSquad.CurrentLeader == null || Context.EffectOwnerSquad.CurrentLeader.Pilot == null || Context.EffectOwnerSquad.IsDead)
                return false;
            
            for (int U = Context.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (Context.EffectOwnerSquad[U] == null || Context.EffectOwnerSquad[U].Pilot == null)
                    continue;

                Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter, Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerSquad[U].Pilot);
                
                if (ActiveSkill.CanActivateEffectsOnTarget(Context.EffectOwnerSquad[U].Pilot.Effects))
                    return true;
            }

            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            for (int U = Context.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter, Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerSquad[U].Pilot);

                AddAndExecuteEffect(ActiveSkill, Context.EffectOwnerSquad[U].Pilot.Effects);
                Context.EffectOwnerCharacter.SP -= ActiveSkill.SPCost;
            }
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationSelfSquad(Context);
        }
    }
}
