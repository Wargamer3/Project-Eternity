using System;
using System.IO;
using System.ComponentModel;
using ProjectEternity.Core.Scripts;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public sealed partial class ExtraBattleMapCutsceneScriptHolder
    {
        public class ScriptCameraOverride : BattleMapScript
        {
            private enum CameraMovements { }

            private Vector3 _StartPosition;
            private Vector3 _EndPosition;
            private Vector3 _Speed;
            private Vector3 _Rotation;

            private Camera3D Camera;
            private Vector3 RealEndPosition;

            public ScriptCameraOverride()
                : this(null)
            {
            }

            public ScriptCameraOverride(BattleMap Map)
                : base(Map, 150, 100, "Camera Override", new string[] { "Create Camera", "Grab Existing Camera", "Reset Position", "Move", "Move towards End", "Stop Override" }, new string[0])
            {
                Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            }

            public override void ExecuteTrigger(int Index)
            {
                if (Map == null)
                {
                    return;
                }

                RealEndPosition = new Vector3(_EndPosition.X * Map.TileSize.X, _EndPosition.Z * 32, _EndPosition.Y * Map.TileSize.Y);

                switch (Index)
                {
                    case 0://Create Camera
                        Camera = new DefaultCamera(GameScreen.GraphicsDevice);
                        Map.CameraOverride = Camera;
                        Camera.CameraPosition3D = new Vector3(_StartPosition.X * Map.TileSize.X, _StartPosition.Z * 32, _StartPosition.Y * Map.TileSize.Y);
                        break;

                    case 1://Grab Existing Camera
                        Camera = Map.CameraOverride;
                        break;

                    case 2://Reset Position
                        Camera.CameraPosition3D = new Vector3(_StartPosition.X * Map.TileSize.X, _StartPosition.Z * 32, _StartPosition.Y * Map.TileSize.Y);
                        break;

                    case 3://Move
                        Camera.CameraPosition3D += new Vector3(_Speed.X, -_Speed.Z, _Speed.Y);
                        break;

                    case 4://Move Towards End
                        Move(RealEndPosition);
                        break;

                    case 5://Stop Override
                        Map.CameraOverride = null;
                        break;
                }

                Map.CameraOverride = Camera;
                Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Camera.CameraPosition3D + Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationX(MathHelper.ToRadians(170))), Vector3.Up);
            }

            public override void ExecuteUpdate(GameTime gameTime, int Index)
            {
                if (Map == null)
                {
                    return;
                }

                switch (Index)
                {
                    case 0://Create Camera
                        Camera = new DefaultCamera(GameScreen.GraphicsDevice);
                        Map.CameraOverride = Camera;
                        Camera.CameraPosition3D = new Vector3(_StartPosition.X * Map.TileSize.X, _StartPosition.Z * 32, _StartPosition.Y * Map.TileSize.Y);
                        break;

                    case 1://Grab Existing Camera
                        Camera = Map.CameraOverride;
                        break;

                    case 2://Reset Position
                        Camera.CameraPosition3D = new Vector3(_StartPosition.X * Map.TileSize.X, _StartPosition.Z * 32, _StartPosition.Y * Map.TileSize.Y);
                        break;

                    case 3://Move
                        Camera.CameraPosition3D += new Vector3(_Speed.X, -_Speed.Z, _Speed.Y);
                        break;

                    case 4://Move Towards End
                        Move(RealEndPosition);
                        break;

                    case 5://Stop Override
                        Map.CameraOverride = null;
                        break;
                }

                Map.CameraOverride = Camera;
                Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Camera.CameraPosition3D + Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationX(MathHelper.ToRadians(170))), Vector3.Up);
            }

            public override void Update(GameTime gameTime)
            {
                Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Camera.CameraPosition3D + Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationX(MathHelper.ToRadians(170))), Vector3.Up);
            }

            private void Move(Vector3 Target)
            {
                if (Speed.X > 0 && Camera.CameraPosition3D.X + Speed.X < Target.X)
                {
                    Camera.CameraPosition3D.X += Speed.X;
                    if (Camera.CameraPosition3D.X >= Target.X)
                    {
                        Camera.CameraPosition3D.X = Target.X;
                    }
                }
                else if (Speed.X < 0 && Camera.CameraPosition3D.X + Speed.X > Target.X)
                {
                    Camera.CameraPosition3D.X += Speed.X;
                    if (Camera.CameraPosition3D.X <= Target.X)
                    {
                        Camera.CameraPosition3D.X = Target.X;
                    }
                }
                if (Speed.X > 0 && Camera.CameraPosition3D.Y + Speed.Y < Target.Y)
                {
                    Camera.CameraPosition3D.Y += Speed.Y;
                    if (Camera.CameraPosition3D.Y >= Target.Y)
                    {
                        Camera.CameraPosition3D.Y = Target.Y;
                    }
                }
                else if (Speed.X < 0 && Camera.CameraPosition3D.Y + Speed.Y > Target.Y)
                {
                    Camera.CameraPosition3D.Y += Speed.Y;
                    if (Camera.CameraPosition3D.Y <= Target.Y)
                    {
                        Camera.CameraPosition3D.Y = Target.Y;
                    }
                }
                if (Speed.X > 0 && Camera.CameraPosition3D.Z + Speed.Z < Target.Z)
                {
                    Camera.CameraPosition3D.Z += Speed.Z;
                    if (Camera.CameraPosition3D.Z >= Target.Z)
                    {
                        Camera.CameraPosition3D.Z = Target.Z;
                    }
                }
                else if (Speed.X < 0 && Camera.CameraPosition3D.Z + Speed.Z > Target.Z)
                {
                    Camera.CameraPosition3D.Z += Speed.Z;
                    if (Camera.CameraPosition3D.Z <= Target.Z)
                    {
                        Camera.CameraPosition3D.Z = Target.Z;
                    }
                }
            }

            public override void Draw(CustomSpriteBatch g)
            {
                throw new NotImplementedException();
            }

            public override void Load(BinaryReader BR)
            {
                _StartPosition = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                _EndPosition = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                _Speed = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
                _Rotation = new Vector3(BR.ReadSingle(), BR.ReadSingle(), BR.ReadSingle());
            }

            public override void Save(BinaryWriter BW)
            {
                BW.Write(_StartPosition.X);
                BW.Write(_StartPosition.Y);
                BW.Write(_StartPosition.Z);

                BW.Write(_EndPosition.X);
                BW.Write(_EndPosition.Y);
                BW.Write(_EndPosition.Z);

                BW.Write(_Speed.X);
                BW.Write(_Speed.Y);
                BW.Write(_Speed.Z);

                BW.Write(_Rotation.X);
                BW.Write(_Rotation.Y);
                BW.Write(_Rotation.Z);
            }

            protected override CutsceneScript DoCopyScript()
            {
                return new ScriptCameraOverride(Map);
            }

            #region Properties

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("Start Position."),
            DefaultValueAttribute(0)]
            public Vector3 StartPosition
            {
                get
                {
                    return _StartPosition;
                }
                set
                {
                    _StartPosition = value;
                }
            }

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("End Position."),
            DefaultValueAttribute(0)]
            public Vector3 EndPosition
            {
                get
                {
                    return _EndPosition;
                }
                set
                {
                    _EndPosition = value;
                }
            }

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("Speed."),
            DefaultValueAttribute(0)]
            public Vector3 Speed
            {
                get
                {
                    return _Speed;
                }
                set
                {
                    _Speed = value;
                }
            }

            [CategoryAttribute("Camera Attributes"),
            DescriptionAttribute("Rotation."),
            DefaultValueAttribute(0)]
            public Vector3 Rotation
            {
                get
                {
                    return _Rotation;
                }
                set
                {
                    _Rotation = value;
                }
            }

            #endregion
        }
    }
}
