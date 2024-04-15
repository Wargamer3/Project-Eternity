using System;
using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationAuraEnemy : AutomaticDeathmatchTargetType
    {
        public static string Name = "Aura Enemy";

        public EffectActivationAuraEnemy()
            : this(null)
        {
        }

        public EffectActivationAuraEnemy(DeathmatchParams Params)
            : base(Name, Params)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int P = Params.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (Params.Map.ListPlayer[P].TeamIndex != Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].TeamIndex)
                    continue;

                for (int S = Params.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    float Distance = Math.Abs(Params.Map.ActiveSquad.X - Params.Map.ListPlayer[P].ListSquad[S].X) + Math.Abs(Params.Map.ActiveSquad.Y - Params.Map.ListPlayer[P].ListSquad[S].Y);
                    if (Distance > ActiveSkillEffect.Range)
                        continue;

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

            if (ActiveSkillEffect.LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                LifeType = SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex;
            }

            for (int P = Params.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (Params.Map.ListPlayer[P].TeamIndex != Params.Map.ListPlayer[Params.Map.ActivePlayerIndex].TeamIndex)
                    continue;

                for (int S = Params.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    float Distance = Math.Abs(Params.Map.ActiveSquad.X - Params.Map.ListPlayer[P].ListSquad[S].X) + Math.Abs(Params.Map.ActiveSquad.Y - Params.Map.ListPlayer[P].ListSquad[S].Y);
                    if (Distance > ActiveSkillEffect.Range)
                        continue;

                    for (int U = Params.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        Params.Map.ListPlayer[P].ListSquad[S][U].Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
                    }
                }
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationAuraEnemy(Params);
        }
    }
}
