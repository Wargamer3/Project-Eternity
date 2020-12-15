using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.AI;
using ProjectEternity.GameScreens.ConquestMapScreen;

namespace ProjectEternity.AI.ConquestMapScreen
{
    public sealed partial class ConquestScriptHolder
    {
        public class TurnReached : ConquestScript, ScriptReference
        {
            private int _Turn;

            public TurnReached()
                : base(150, 50, "Turn Reached Conquest", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return Info.Map.GameTurn >= _Turn;
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);

                _Turn = BR.ReadInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);

                BW.Write(_Turn);
            }

            public override AIScript CopyScript()
            {
                return new TurnReached();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute(""),
            DefaultValueAttribute("")]
            public int Turn
            {
                get
                {
                    return _Turn;
                }
                set
                {
                    _Turn = value;
                }
            }
        }
    }
}
