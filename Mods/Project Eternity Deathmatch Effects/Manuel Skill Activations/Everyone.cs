using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{

    public class PilotSkillActivationEveryone : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationEveryone()
            : this(null)
        {

        }

        public PilotSkillActivationEveryone(DeathmatchContext Context)
            : base("Everyone", false, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            for (int P = Context.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int Squad = Context.Map.ListPlayer[P].ListSquad.Count - 1; Squad >= 0; --Squad)
                {
                    if (Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader == null || Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader.Pilot == null || Context.Map.ListPlayer[P].ListSquad[Squad].IsDead)
                        continue;

                    for (int U = Context.Map.ListPlayer[P].ListSquad[Squad].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Context.Map.ListPlayer[P].ListSquad[Squad][U] == null || Context.Map.ListPlayer[P].ListSquad[Squad][U].Pilot == null)
                            continue;

                        Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter,
                            Context.Map.ListPlayer[P].ListSquad[Squad], Context.Map.ListPlayer[P].ListSquad[Squad][U], Context.Map.ListPlayer[P].ListSquad[Squad][U].Pilot, Context.Map.ActiveParser);

                        if (ActiveSkill.CanActivateEffectsOnTarget(Context.Map.ListPlayer[P].ListSquad[Squad][U].Pilot.Effects))
                            return true;
                    }
                }
            }

            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            for (int P = Context.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                for (int S = Context.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = Context.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter,
                            Context.Map.ListPlayer[P].ListSquad[S], Context.Map.ListPlayer[P].ListSquad[S][U], Context.Map.ListPlayer[P].ListSquad[S][U].Pilot, Context.Map.ActiveParser);

                        AddAndExecuteEffect(ActiveSkill, Context.Map.ListPlayer[P].ListSquad[S][U].Pilot.Effects);
                    }
                }
            }
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationEveryone(Context);
        }
    }
}
