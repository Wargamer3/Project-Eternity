using System.Collections.Generic;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetEnemies : DeathmatchScript, ScriptReference
        {
            public GetEnemies()
                : base(100, 50, "Get Enemies Deathmatch", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                List<object> ListEnemy = new List<object>();
                int CurrentTeam = Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].Team;

                for (int P = Info.Map.ListPlayer.Count - 1; P >=0; --P)
                {
                    if (Info.Map.ListPlayer[P].Team != CurrentTeam)
                    {
                        for (int S = Info.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                        {
                            ListEnemy.Add(Info.Map.ListPlayer[P].ListSquad[S]);
                        }
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
