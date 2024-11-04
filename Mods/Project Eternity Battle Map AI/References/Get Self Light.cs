using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using System.Collections.Generic;

namespace ProjectEternity.AI.BattleMapScreen
{
    public sealed partial class BattleLightScriptHolder
    {
        public class GetSelfLight : BattleScript, ScriptReference
        {
            public GetSelfLight()
                : base(100, 50, "Get Self Light", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return Info.ActiveLight;
            }

            public override AIScript CopyScript()
            {
                return new GetSelfLight();
            }
        }
    }
}
