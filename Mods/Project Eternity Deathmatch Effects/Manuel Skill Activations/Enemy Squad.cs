using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationEnemySquad : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationEnemySquad()
            : this(null)
        {

        }

        public PilotSkillActivationEnemySquad(DeathmatchParams Params)
            : base("Enemy Squad", true, Params)
        {
        }

        public override bool CanActivateOnTarget(ManualSkill ActiveSkill)
        {
            for (int P = Params.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (Params.Map.ListPlayer[P].TeamIndex == Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].TeamIndex)
                    continue;

                for (int Squad = Params.Map.ListPlayer[P].ListSquad.Count - 1; Squad >= 0; --Squad)
                {
                    if (Params.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader == null || Params.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader.Pilot == null || Params.Map.ListPlayer[P].ListSquad[Squad].IsDead)
                        continue;
                    
                    for (int U = Params.Map.ListPlayer[P].ListSquad[Squad].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (Params.Map.ListPlayer[P].ListSquad[Squad][U] == null || Params.Map.ListPlayer[P].ListSquad[Squad][U].Pilot == null)
                            continue;

                        Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                            Params.Map.ListPlayer[P].ListSquad[Squad], Params.Map.ListPlayer[P].ListSquad[Squad][U], Params.Map.ListPlayer[P].ListSquad[Squad][U].Pilot, Params.ActiveParser);
                        
                        if (ActiveSkill.CanActivateEffectsOnTarget(Params.Map.ListPlayer[P].ListSquad[Squad][U].Pilot.Effects))
                            return true;
                    }
                }
            }

            return false;
        }

        public override void ActivateSkillFromMenu(ManualSkill ActiveSkill)
        {
            Params.Map.PushScreen(new SelectSkillTargetScreen(Params.Map, ActiveSkill, Params.GlobalContext.EffectOwnerCharacter, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerSquad, Params, this));
        }

        public override ManualSkillTarget Copy()
        {
            return new PilotSkillActivationEnemySquad(Params);
        }

        public override void CopyMembers(ManualSkillTarget Copy)
        {
        }
    }
}
