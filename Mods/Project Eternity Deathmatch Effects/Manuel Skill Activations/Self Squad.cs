using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationSelfSquad : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationSelfSquad()
            : this(null)
        {

        }

        public PilotSkillActivationSelfSquad(DeathmatchParams Params)
            : base("Self Squad", false, Params)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            if (Params.GlobalContext.EffectOwnerSquad.CurrentLeader == null || Params.GlobalContext.EffectOwnerSquad.CurrentLeader.Pilot == null || Params.GlobalContext.EffectOwnerSquad.IsDead)
                return false;
            
            for (int U = Params.GlobalContext.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (Params.GlobalContext.EffectOwnerSquad[U] == null || Params.GlobalContext.EffectOwnerSquad[U].Pilot == null)
                    continue;

                Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                    Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerSquad[U].Pilot,
                    Params.ActiveParser);
                
                if (ActiveSkill.CanActivateEffectsOnTarget(Params.GlobalContext.EffectOwnerSquad[U].Pilot.Effects))
                    return true;
            }

            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            for (int U = Params.GlobalContext.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                    Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerSquad[U].Pilot,
                    Params.ActiveParser);

                AddAndExecuteEffect(ActiveSkill, Params.GlobalContext.EffectOwnerSquad[U].Pilot.Effects, SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex);
                Params.GlobalContext.EffectOwnerCharacter.SP -= ActiveSkill.ActivationCost;
            }
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationSelfSquad(Params);
        }
    }
}
