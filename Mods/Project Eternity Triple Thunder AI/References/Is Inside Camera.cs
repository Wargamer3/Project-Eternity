using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.AI;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.TripleThunderScreen
{

    public sealed partial class GameScriptHolder
    {
        public class IsInsideCamera : TripleThunderScript, ScriptReference
        {
            private Vector2 _Size;

            public IsInsideCamera()
                : base(100, 50, "Is Inside Camera", new string[0], new string[0])
            {
            }

            public object GetContent()
            {
                return Info.OwnerMap.IsInsideCamera(Info.Owner.Position, _Size);
            }

            public override void Load(BinaryReader BR)
            {
                base.Load(BR);
                _Size = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }

            public override void Save(BinaryWriter BW)
            {
                base.Save(BW);
                BW.Write(_Size.X);
                BW.Write(_Size.Y);
            }

            public override AIScript CopyScript()
            {
                return new IsInsideCamera();
            }

            [CategoryAttribute("Script Attributes"),
            DescriptionAttribute("")]
            public Vector2 Size
            {
                get
                {
                    return _Size;
                }
                set
                {
                    _Size = value;
                }
            }
        }
    }
}
