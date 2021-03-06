﻿using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationSquadEnemy : AutomaticDeathmatchTargetType
    {
        public static string Name = "Squad Enemy";

        public EffectActivationSquadEnemy()
            : this(null)
        {
        }

        public EffectActivationSquadEnemy(DeathmatchContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int U = GlobalContext.EffectTargetSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (ActiveSkillEffect.CanActivate())
                    return true;
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

            for (int U = GlobalContext.EffectTargetSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                GlobalContext.Map.ActiveSquad[U].Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSquadEnemy(GlobalContext);
        }
    }
}
