using System;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{
    public sealed class AllTargetType : RobotTargetType
    {
        public static string Name = "All Robot";

        public AllTargetType()
            : this(null)
        {
        }

        public AllTargetType(TripleThunderRobotContext GlobalContext)
            : base(Name, GlobalContext)
        {
        }

        public override bool CanExecuteEffectOnTarget(BaseEffect ActiveSkillEffect)
        {
            foreach (Layer ActiveLayer in GlobalContext.Map.ListLayer)
            {
                foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                {
                    GlobalContext.Target = ActiveRobot;
                    if (ActiveSkillEffect.CanActivate())
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void ExecuteAndAddEffectToTarget(BaseEffect ActiveSkillEffect, string SkillName)
        {
            foreach (Layer ActiveLayer in GlobalContext.Map.ListLayer)
            {
                foreach (RobotAnimation ActiveRobot in ActiveLayer.DicRobot.Values)
                {
                    GlobalContext.Target = ActiveRobot;
                    if (ActiveSkillEffect.CanActivate())
                    {
                        ActiveRobot.Effects.AddAndExecuteEffect(ActiveSkillEffect, SkillName);
                    }
                }
            }
        }

        public override AutomaticSkillTargetType Copy()
        {
            throw new NotImplementedException();
        }
    }
}
