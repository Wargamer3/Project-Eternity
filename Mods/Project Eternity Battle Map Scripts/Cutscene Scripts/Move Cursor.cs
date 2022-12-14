using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptMoveCursor : BattleMapScript
        {
            private Point _CursorPosition;

            public ScriptMoveCursor()
                : this(null)
            {
            }

            public ScriptMoveCursor(BattleMap Map)
                : base(Map, 100, 50, "Move Cursor", new string[] { "Move" }, new string[0])
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                Map.CursorPosition = new Microsoft.Xna.Framework.Vector3(CursorPosition.X, CursorPosition.Y, Map.CursorPosition.Z);
                Map.CursorPositionVisible = Map.CursorPosition;
                IsEnded = true;
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                CursorPosition = new Point(BR.ReadInt32(), BR.ReadInt32());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(CursorPosition.X);
                BW.Write(CursorPosition.Y);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptMoveCursor(Map);
            }

            #region Properties

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("Position to center cursor on."),
            DefaultValueAttribute(0)]
            public Point CursorPosition
            {
                get
                {
                    return _CursorPosition;
                }
                set
                {
                    _CursorPosition = value;
                }
            }

            #endregion
        }
    }
}
