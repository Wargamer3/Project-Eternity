using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using ProjectEternity.Core;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.GameScreens.BattleMapScreen
{

    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptCenterCamera : BattleMapScript
        {
            private Point _CursorPosition;

            public ScriptCenterCamera()
                : this(null)
            {
            }

            public ScriptCenterCamera(BattleMap Map)
                : base(Map, 100, 50, "Center Camera", new string[] { "Center" }, new string[] { "Camera Centered" })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                bool IsFinished = true;
                if (Map.CursorPosition.X < _CursorPosition.X)
                {
                    Map.CursorPosition.X++;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X > _CursorPosition.X)
                {
                    Map.CursorPosition.X--;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y < _CursorPosition.Y)
                {
                    Map.CursorPosition.Y++;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y > _CursorPosition.Y)
                {
                    Map.CursorPosition.Y--;
                    IsFinished = false;
                }

                //Update the camera if needed.
                if (Map.CursorPosition.X - Map.CameraPosition.X - 3 < 0 && Map.CameraPosition.X > -3)
                {
                    --Map.CameraPosition.X;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X - Map.CameraPosition.X >= Map.ScreenSize.X / 2 && Map.CameraPosition.X + Map.ScreenSize.X < Map.MapSize.X + 3)
                {
                    ++Map.CameraPosition.X;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y - Map.CameraPosition.Y - 3 < 0 && Map.CameraPosition.Y > -3)
                {
                    --Map.CameraPosition.Y;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y - Map.CameraPosition.Y >= Map.ScreenSize.Y / 2 && Map.CameraPosition.Y + Map.ScreenSize.Y < Map.MapSize.Y + 3)
                {
                    ++Map.CameraPosition.Y;
                    IsFinished = false;
                }

                if (IsFinished)
                {
                    ExecuteEvent(this, 0);
                    IsEnded = true;
                }
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
                return new ScriptCenterCamera(Map);
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
