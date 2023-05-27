using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public sealed partial class DeathmatchCutsceneScriptHolder
    {
        public class ScriptCenterCameraOnSquad : DeathmatchMapScript
        {
            private Point CursorPosition;
            private uint _SquadID;

            public ScriptCenterCameraOnSquad()
                : this(null)
            {
            }

            public ScriptCenterCameraOnSquad(DeathmatchMap Map)
                : base(Map, 150, 50, "Center Camera On Squad", new string[] { "Center" }, new string[] { "Camera Centered" })
            {
            }

            public override void ExecuteTrigger(int Index)
            {
                IsActive = true;

                foreach (Player ActivePlayer in Map.ListPlayer)
                {
                    foreach (Core.Units.Squad ActiveSquad in ActivePlayer.ListSquad)
                    {
                        if (ActiveSquad.ID == _SquadID)
                        {
                            CursorPosition = new Point((int)ActiveSquad.X, (int)ActiveSquad.Y);
                            return;
                        }
                    }
                }
            }

            public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
            {
                bool IsFinished = true;
                if (Map.CursorPosition.X < CursorPosition.X)
                {
                    Map.CursorPosition.X++;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X > CursorPosition.X)
                {
                    Map.CursorPosition.X--;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y < CursorPosition.Y)
                {
                    Map.CursorPosition.Y++;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y > CursorPosition.Y)
                {
                    Map.CursorPosition.Y--;
                    IsFinished = false;
                }

                //Update the camera if needed.
                if (Map.CursorPosition.X - Map.Camera2DPosition.X - 3 < 0 && Map.Camera2DPosition.X > -3)
                {
                    --Map.Camera2DPosition.X;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X - Map.Camera2DPosition.X >= Map.ScreenSize.X / 2 && Map.Camera2DPosition.X + Map.ScreenSize.X < Map.MapSize.X + 3)
                {
                    ++Map.Camera2DPosition.X;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y - Map.Camera2DPosition.Y - 3 < 0 && Map.Camera2DPosition.Y > -3)
                {
                    --Map.Camera2DPosition.Y;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y - Map.Camera2DPosition.Y >= Map.ScreenSize.Y / 2 && Map.Camera2DPosition.Y + Map.ScreenSize.Y < Map.MapSize.Y + 3)
                {
                    ++Map.Camera2DPosition.Y;
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
                _SquadID = BR.ReadUInt32();
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_SquadID);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCenterCameraOnSquad(Map);
            }

            #region Properties

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("Squad to center cursor on."),
            DefaultValueAttribute(0)]
            public uint SquadID
            {
                get
                {
                    return _SquadID;
                }
                set
                {
                    _SquadID = value;
                }
            }

            #endregion
        }
    }
}
