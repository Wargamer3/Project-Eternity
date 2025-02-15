using ProjectEternity.Core.Effects;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class EffectActivationSelf : AutomaticDeathmatchTargetType
    {
        public static string Name = "Self";

        public EffectActivationSelf()
            : this(null)
        {
        }

        public EffectActivationSelf(DeathmatchParams Params)
            : base(Name, Params)
        {
        }
        
        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter, Params.GlobalContext.ActiveParser);

            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            string LifeType = "";

            if (ActiveSkillEffect.Lifetime[0].LifetimeType == SkillEffect.LifetimeTypeTurns)
            {
                LifeType = SkillEffect.LifetimeTypeTurns + Params.Map.ActivePlayerIndex;
            }

            Params.GlobalContext.SetContext(Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter,
                Params.GlobalContext.EffectOwnerSquad, Params.GlobalContext.EffectOwnerUnit, Params.GlobalContext.EffectOwnerCharacter, Params.GlobalContext.ActiveParser);

            Params.GlobalContext.EffectTargetUnit.Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationSelf(Params);
        }
    }
}
