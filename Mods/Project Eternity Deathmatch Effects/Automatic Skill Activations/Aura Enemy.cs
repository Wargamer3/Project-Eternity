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

        public EffectActivationAuraEnemy(DeathmatchContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            for (int P = GlobalContext.Map.ListPlayer.Count - 1; P >= 0; --P)
            {
                if (GlobalContext.Map.ListPlayer[P].Team != GlobalContext.Map.ListPlayer[GlobalContext.Map.ActivePlayerIndex].Team)
                    continue;

                for (int S = GlobalContext.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    float Distance = Math.Abs(GlobalContext.Map.ActiveSquad.X - GlobalContext.Map.ListPlayer[P].ListSquad[S].X) + Math.Abs(GlobalContext.Map.ActiveSquad.Y - GlobalContext.Map.ListPlayer[P].ListSquad[S].Y);
                    if (Distance > ActiveSkillEffect.Range)
                        continue;

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
                if (GlobalContext.Map.ListPlayer[P].Team != GlobalContext.Map.ListPlayer[GlobalContext.Map.ActivePlayerIndex].Team)
                    continue;

                for (int S = GlobalContext.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                {
                    float Distance = Math.Abs(GlobalContext.Map.ActiveSquad.X - GlobalContext.Map.ListPlayer[P].ListSquad[S].X) + Math.Abs(GlobalContext.Map.ActiveSquad.Y - GlobalContext.Map.ListPlayer[P].ListSquad[S].Y);
                    if (Distance > ActiveSkillEffect.Range)
                        continue;

                    for (int U = GlobalContext.Map.ListPlayer[P].ListSquad[S].UnitsAliveInSquad - 1; U >= 0; --U)
                    {
                        GlobalContext.Map.ListPlayer[P].ListSquad[S][U].Pilot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName, LifeType);
                    }
                }
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new EffectActivationAuraEnemy(GlobalContext);
        }
    }
}
