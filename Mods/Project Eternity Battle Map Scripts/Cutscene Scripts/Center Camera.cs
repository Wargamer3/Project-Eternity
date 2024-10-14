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
                Map.UpdateCursorVisiblePosition(gameTime);

                bool IsFinished = true;
                if (Map.CursorPosition.X < _CursorPosition.X * Map.TileSize.X)
                {
                    float CursorSpeed = Map.TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.CursorPosition.X += Math.Min(CursorSpeed, _CursorPosition.X * Map.TileSize.X - Map.CursorPosition.X);
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X > _CursorPosition.X * Map.TileSize.X)
                {
                    float CursorSpeed = Map.TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.CursorPosition.X -= Math.Min(CursorSpeed, Map.CursorPosition.X -_CursorPosition.X * Map.TileSize.X);
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y < _CursorPosition.Y * Map.TileSize.Y)
                {
                    float CursorSpeed = Map.TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.CursorPosition.Y += Math.Min(CursorSpeed, _CursorPosition.Y * Map.TileSize.Y - Map.CursorPosition.Y);
                    IsFinished = false;
                }
                else if (Map.CursorPosition.Y > _CursorPosition.Y * Map.TileSize.Y)
                {
                    float CursorSpeed = Map.TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.CursorPosition.Y -= Math.Min(CursorSpeed, Map.CursorPosition.Y -_CursorPosition.Y * Map.TileSize.Y);
                    IsFinished = false;
                }

                int MapWidth = Map.MapSize.X * Map.TileSize.X;
                int MapHeight = Map.MapSize.Y * Map.TileSize.Y;

                //Update the camera if needed.
                if (Map.CursorPosition.X - Map.Camera2DPosition.X - 3 * Map.TileSize.X < 0 && Map.Camera2DPosition.X > -3 * Map.TileSize.X)
                {
                    float CursorSpeed = Map.TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.Camera2DPosition.X -= CursorSpeed;
                    IsFinished = false;
                }
                else if (Map.CursorPosition.X - Map.Camera2DPosition.X + 3 * Map.TileSize.X >= Map.ScreenSize.X * Map.TileSize.X && Map.Camera2DPosition.X + Map.ScreenSize.X * Map.TileSize.X < (Map.MapSize.X + 3) * Map.TileSize.X)
                {
                    float CursorSpeed = Map.TileSize.X * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.Camera2DPosition.X += CursorSpeed;
                    IsFinished = false;
                }

                if (Map.CursorPosition.Y - Map.Camera2DPosition.Y - 3 * Map.TileSize.Y * Map.TileSize.Y < 0 && Map.Camera2DPosition.Y > -3 * Map.TileSize.Y)
                {
                    float CursorSpeed = Map.TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.Camera2DPosition.Y -= CursorSpeed;
                    IsFinished = false;
                }
                else if (CursorPosition.Y - Map.Camera2DPosition.Y + 3 * Map.TileSize.Y >= Map.ScreenSize.Y * Map.TileSize.Y && Map.Camera2DPosition.Y + Map.ScreenSize.Y * Map.TileSize.Y < (Map.MapSize.Y + 3) * Map.TileSize.Y)
                {
                    float CursorSpeed = Map.TileSize.Y * gameTime.ElapsedGameTime.Milliseconds * 0.01f;
                    Map.Camera2DPosition.Y += CursorSpeed;
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
