using ProjectEternity.Core.Effects;
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

        public EffectActivationSquadEnemy(DeathmatchParams Params)
            : base(Name, Params)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int U = Params.GlobalContext.EffectTargetSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                if (ActiveSkillEffect.CanActivate())
                    return true;
            }

            return false;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            string LifeType = "";

            if (ActiveSkillEffect.Lifetime[0].LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                LifeType = SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex;
            }

            for (int U = Params.GlobalContext.EffectTargetSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                Params.Map.ActiveSquad[U].Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSquadEnemy(Params);
        }
    }
}
