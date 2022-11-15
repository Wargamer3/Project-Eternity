using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetAllTargetType : SorcererStreetBattleTargetType
    {
        public static string Name = "Sorcerer Street All";

        public SorcererStreetAllTargetType()
            : this(null)
        {
        }

        public SorcererStreetAllTargetType(SorcererStreetBattleContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            GlobalContext.OpponentCreature = GlobalContext.Invader;
            if (ActiveSkillEffect.CanActivate())
            {
                return true;
            }

            GlobalContext.OpponentCreature = GlobalContext.Defender;
            if (ActiveSkillEffect.CanActivate())
            {
                return true;
            }

            return false;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            GlobalContext.OpponentCreature = GlobalContext.Invader;
            if (ActiveSkillEffect.CanActivate())
            {
                GlobalContext.OpponentCreature.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
            }

            GlobalContext.OpponentCreature = GlobalContext.Defender;
            if (ActiveSkillEffect.CanActivate())
            {
                GlobalContext.OpponentCreature.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new SorcererStreetAllTargetType(GlobalContext);
        }
    }
}
