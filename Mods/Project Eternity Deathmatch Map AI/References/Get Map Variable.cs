using System.ComponentModel;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.AI.DeathmatchMapScreen
{
    public sealed partial class DeathmatchScriptHolder
    {
        public class GetMapVariable : DeathmatchScript, ScriptReference
        {
            private string _VariableName;

            public GetMapVariable()
                : base(100, 50, "Get Map Variable", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return BattleMap.DicMapVariables[_VariableName];
            }

            public override AIScript CopyScript()
            {
                return new GetMapVariable();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public string VariableName
            {
                get
                {
                    return _VariableName;
                }
                set
                {
                    _VariableName = value;
                }
            }
        }
    }
}
