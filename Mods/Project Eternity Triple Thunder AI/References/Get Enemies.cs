using System.Collections.Generic;
using ProjectEternity.Core.AI;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class GetEnemies : TripleThunderScript, ScriptReference
        {
            private List<object> ListEnemy;

            public GetEnemies()
                : base(100, 50, "Get Enemies", new string[0], new string[0])
            {
                ListEnemy = new List<object>();
            }

            public object GetContent()
            {
                if (Info.OwnerLayer.DicRobot.Count != ListEnemy.Count)
                {
                    ListEnemy.Clear();

                    foreach (RobotAnimation ActiveRobot in Info.OwnerLayer.DicRobot.Values)
                    {
                        if (ActiveRobot == Info.Owner || ActiveRobot.Team == Info.Owner.Team)
                            continue;

                        ListEnemy.Add(ActiveRobot);
                    }
                }
                return ListEnemy;
            }

            public override AIScript CopyScript()
            {
                return new GetEnemies();
            }
        }
    }
}
