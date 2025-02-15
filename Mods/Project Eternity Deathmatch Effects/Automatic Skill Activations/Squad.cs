using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationSquad : AutomaticDeathmatchTargetType
    {
        public static string Name = "Squad";

        public EffectActivationSquad()
            : this(null)
        {
        }

        public EffectActivationSquad(DeathmatchParams Params)
            : base(Name, Params)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int U = Params.GlobalContext.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
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

            for (int U = Params.GlobalContext.EffectOwnerSquad.UnitsAliveInSquad - 1; U >= 0; --U)
            {
                Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                    Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerSquad[U], Params.GlobalContext.EffectTargetUnit.Pilot, Params.GlobalContext.ActiveParser);

                Params.GlobalContext.EffectTargetUnit.Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSquad(Params);
        }
    }
}
