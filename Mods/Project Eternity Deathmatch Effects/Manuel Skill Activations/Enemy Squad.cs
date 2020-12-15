using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationEnemySquad : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationEnemySquad()
            : this(null)
        {

        }

        public PilotSkillActivationEnemySquad(DeathmatchContext Context)
            : base("Enemy Squad", true, Context)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            for (int P = Context.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (Context.Map.ListPlayer[P].Team == Context.Map.ListPlayer[Context.Map.ActivePlayerIndex].Team)
                    continue;

                for (int Squad = Context.Map.ListPlayer[P].ListSquad.Count - 1; Squad >= 0; --Squad)
                {
                    if (Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader == null || Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader.Pilot == null || Context.Map.ListPlayer[P].ListSquad[Squad].IsDead)
                        continue;
                    
                    for (int U = Context.Map.ListPlayer[P].ListSquad[Squad].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Context.Map.ListPlayer[P].ListSquad[Squad][U] == null || Context.Map.ListPlayer[P].ListSquad[Squad][U].Pilot == null)
                            continue;

                        Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter,
                            Context.Map.ListPlayer[P].ListSquad[Squad], Context.Map.ListPlayer[P].ListSquad[Squad][U], Context.Map.ListPlayer[P].ListSquad[Squad][U].Pilot);
                        
                        if (ActiveSkill.CanActivateEffectsOnTarget(Context.Map.ListPlayer[P].ListSquad[Squad][U].Pilot.Effects))
                            return true;
                    }
                }
            }

            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Context.Map.PushScreen(new SelectSkillTargetScreen(Context.Map, ActiveSkill, Context.EffectOwnerCharacter, Context.EffectOwnerUnit, Context.EffectOwnerSquad, Context, this));
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationEnemySquad(Context);
        }
    }
}
