using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class MoveCursorToAttackEnemy : DeathmatchScript, ScriptEvaluator
        {
            public MoveCursorToAttackEnemy()
                : base(150, 50, "Move Cursor To Attack Enemy", new string[0], new string[1] { "Enemy" })
            {
            }

            public void Evaluate(GameTime gameTime, object Input, out bool IsCompleted, out List<object> Result)
            {
                Tuple<int, int> Target = (Tuple<int, int>)ArrayReferences[0].ReferencedScript.GetContent();
                Info.Map.TargetPlayerIndex = Target.Item1;

                Info.Map.ListActionMenuChoice.AddToPanelListAndSelect(new ActionPanelAIAttackBehavior(
                    Info.Map,
                    Info.Map.ActivePlayerIndex,
                    Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].ListSquad.IndexOf(Info.ActiveSquad),
                    Target));

                IsCompleted = false;
                Result = new List<object>() { "break" };
            }

            public override AIScript CopyScript()
            {
                return new MoveCursorToAttackEnemy();
            }
        }
    }
}
