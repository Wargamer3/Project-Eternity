﻿using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.Core.Skill
{
    public class PilotSkillActivationEnemy : ManualSkillActivationDeathmatch
    {
        public PilotSkillActivationEnemy()
            : this(null)
        {

        }

        public PilotSkillActivationEnemy(DeathmatchContext Context)
            : base("Enemy", true, Context)
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

                    Context.SetContext(Context.EffectOwnerSquad, Context.EffectOwnerUnit, Context.EffectOwnerCharacter,
                        Context.Map.ListPlayer[P].ListSquad[Squad], Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader, Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader.Pilot, Context.Map.ActiveParser);
                    
                    if (ActiveSkill.CanActivateEffectsOnTarget(Context.Map.ListPlayer[P].ListSquad[Squad].CurrentLeader.Pilot.Effects))
                        return true;
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
            return new PilotSkillActivationEnemy(Context);
        }
    }
}
