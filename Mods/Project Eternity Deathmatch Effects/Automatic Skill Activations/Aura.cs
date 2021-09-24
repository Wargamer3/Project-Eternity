﻿using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationAura : AutomaticDeathmatchTargetType
    {
        public static string Name = "Aura";

        public EffectActivationAura()
            : this(null)
        {
        }

        public EffectActivationAura(DeathmatchContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int P = GlobalContext.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (GlobalContext.Map.ListPlayer[P].Team == GlobalContext.Map.ListPlayer[GlobalContext.Map.ActivePlayerIndex].Team)
                    continue;

                for (int S = GlobalContext.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = GlobalContext.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        if (ActiveSkillEffect.CanActivate())
                            return true;
                    }
                }
            }

            return false;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            string LifeType = "";

            if (ActiveSkillEffect.LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                LifeType = SkillEffect.LifetimeTypeTurns + GlobalContext.Map.ActivePlayerIndex;
            }

            for (int P = GlobalContext.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (GlobalContext.Map.ListPlayer[P].Team == GlobalContext.Map.ListPlayer[GlobalContext.Map.ActivePlayerIndex].Team)
                    continue;

                for (int S = GlobalContext.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = GlobalContext.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        GlobalContext.Map.ListPlayer[P].ListSquad[S][U].Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
                    }
                }
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationAura(GlobalContext);
        }
    }
}
