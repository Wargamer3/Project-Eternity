using ProjectEternity.Core.Effects;
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

        public EffectActivationAura(DeathmatchParams Params)
            : base(Name, Params)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int P = Params.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (Params.Map.ListPlayer[P].TeamIndex == Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].TeamIndex)
                    continue;

                for (int S = Params.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = Params.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
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

            if (ActiveSkillEffect.Lifetime[0].LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                LifeType = SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex;
            }

            for (int P = Params.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (Params.Map.ListPlayer[P].TeamIndex == Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].TeamIndex)
                    continue;

                for (int S = Params.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    for (int U = Params.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        Params.Map.ListPlayer[P].ListSquad[S][U].Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
                    }
                }
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationAura(Params);
        }
    }
}
