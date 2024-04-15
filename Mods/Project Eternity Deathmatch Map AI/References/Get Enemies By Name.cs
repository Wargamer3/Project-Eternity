using System.ComponentModel;
using System.Collections.Generic;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetEnemiesByName : DeathmatchScript, ScriptReference
        {
            private string _EnemyName;

            public GetEnemiesByName()
                : base(100, 50, "Get Enemies By Name", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                List<object> ListEnemy = new List<object>();
                int CurrentTeam = Info.Map.ListPlayer[Info.Map.ActivePlayerIndex].TeamIndex;

                for (int P = Info.Map.ListPlayer.Count - 1; P >= 0; --P)
                {
                    if (Info.Map.ListPlayer[P].TeamIndex != CurrentTeam)
                    {
                        for (int S = Info.Map.ListPlayer[P].ListSquad.Count - 1; S >= 0; --S)
                        {
                            if (Info.Map.ListPlayer[P].ListSquad[S].CurrentLeader.RelativePath == _EnemyName)
                                ListEnemy.Add(Info.Map.ListPlayer[P].ListSquad[S]);
                        }
                    }
                }

                return ListEnemy;
            }

            public override AIScript CopyScript()
            {
                return new GetEnemiesByName();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string EnemyName
            {
                get
                {
                    return _EnemyName;
                }
                set
                {
                    _EnemyName = value;
                }
            }
        }
    }
}
