using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using System.Collections.Generic;

namespace ProjectEternity.AI.BattleMapScreen
{
    public sealed partial class BattleLightScriptHolder
    {
        public class GetLight : BattleScript, ScriptReference
        {
            public GetLight()
                : base(100, 50, "Get Light", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return Info.ActiveLight;
            }

            public override AIScript CopyScript()
            {
                return new GetLight();
            }
        }
    }
}
