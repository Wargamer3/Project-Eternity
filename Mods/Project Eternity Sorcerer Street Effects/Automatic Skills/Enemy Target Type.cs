using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public sealed class SorcererStreetEnemyTargetType : SorcererStreetBattleTargetType
    {
        public static string Name = "Sorcerer Street Enemy";

        public SorcererStreetEnemyTargetType()
            : this(null)
        {
        }

        public SorcererStreetEnemyTargetType(SorcererStreetBattleContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            return ActiveSkillEffect.CanActivate();
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            if (GlobalContext.SelfCreature == GlobalContext.Invader)
            {
                GlobalContext.OpponentCreature = GlobalContext.Invader;
            }
            else if (GlobalContext.SelfCreature == GlobalContext.Defender)
            {
                GlobalContext.OpponentCreature = GlobalContext.Defender;
            }

            GlobalContext.OpponentCreature.Creature.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
        }

        public override AutomaticSkillTargetType Copy()
        {
            return new SorcererStreetEnemyTargetType(GlobalContext);
        }
    }
}
