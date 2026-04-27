using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class GenericAIAttackAction : LifeSimAIAction
    {
        BaseEffect Effecti;

        bool RandomActionCriticalSuccess;
        bool RandomActionSuccess;
        bool RandomActionFailure;
        bool RandomActionCriticalFailure;

        List<BaseEffect> ListEffectCriticalSuccess;
        List<BaseEffect> ListEffectSuccess;
        List<BaseEffect> ListEffectFailure;
        List<BaseEffect> ListEffectCriticalFailure;

        public GenericAIAttackAction(string Goal, PlayerCharacter Owner)
            : base(Goal, Owner)
        {
        }

        public override bool Execute(GameTime gameTime, NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override ActionPanelLifeSim GetActionPanel()
        {
            throw new NotImplementedException();
        }

        public override List<AIAction> GetAIExecutionPlan(NavMapGameManager Map)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePrecondition(string Event, NavMapGameManager Map)
        {
            if (Effecti.CanActivate())
            {
            }
            throw new NotImplementedException();
        }
    }
}
