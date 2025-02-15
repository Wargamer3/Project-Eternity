using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class ActionPanelViewMap : ActionPanelSorcererStreet
    {
        private Vector3 CursorPosition;
        private Vector3 CameraPosition;
        private Camera3D Camera;
        protected TerrainSorcererStreet ActiveTerrain;

        public ActionPanelViewMap(SorcererStreetMap Map)
            : base("View Map", Map, true)
        {
        }

        public override void OnSelect()
        {
            Camera = new DefaultCamera(GameScreen.GraphicsDevice);
            Map.Camera3DOverride = Camera;

            CursorPosition = new Vector3(Map.ListPlayer[Map.ActivePlayerIndex].GamePiece.X, Map.ListPlayer[Map.ActivePlayerIndex].GamePiece.Z, Map.ListPlayer[Map.ActivePlayerIndex].GamePiece.Y);
            CameraPosition = CursorPosition;
        }

        public override void DoUpdate(GameTime gameTime)
        {
            float MaxCursorSpeed = 5;
            int BorderWidth = Constants.Width / 10;
            int BorderHeight = Constants.Height / 10;

            if (CursorPosition.X > 0 && CameraPosition.X > 0 && CursorPosition.X < BorderWidth)
            {
                float FinalSpeed = ((BorderWidth - CursorPosition.X) / BorderWidth) * MaxCursorSpeed;
                CameraPosition.X -= FinalSpeed;
            }
            else if (CursorPosition.X < Constants.Width && CameraPosition.X < (Map.MapSize.X - 3) * Map.TileSize.X && CursorPosition.X > Constants.Width - BorderWidth)
            {
                float FinalSpeed = ((CursorPosition.X - (Constants.Width - BorderWidth)) / BorderWidth) * MaxCursorSpeed;
                CameraPosition.X += FinalSpeed;
            }

            if (CursorPosition.Y > 0 && CameraPosition.Y > 0 && CursorPosition.Y < BorderHeight)
            {
                float FinalSpeed = ((BorderHeight - CursorPosition.Y) / BorderHeight) * MaxCursorSpeed;
                CameraPosition.Y -= FinalSpeed;
            }
            else if (CursorPosition.Y < Constants.Height && CameraPosition.Y < (Map.MapSize.Y - 3) * Map.TileSize.Y && CursorPosition.Y > Constants.Height - BorderHeight)
            {
                float FinalSpeed = ((CursorPosition.Y - (Constants.Height - BorderHeight)) / BorderHeight) * MaxCursorSpeed;
                CameraPosition.Y += FinalSpeed;
            }

            if (MouseHelper.MouseMoved())
            {
                ActiveTerrain = null;

                CursorPosition.X = MouseHelper.MouseStateCurrent.X;
                CursorPosition.Y = MouseHelper.MouseStateCurrent.Y;

                foreach (MapLayer ActiveLayer in Map.LayerManager.ListLayer)
                {
                    for (int X = 0; X < Map.MapSize.X; ++X)
                    {
                        for (int Y = 0; Y < Map.MapSize.Y; ++Y)
                        {
                            if (ActiveLayer.ArrayTerrain[X, Y].TerrainTypeIndex != 0)
                            {
                                Vector3 TilePosition = new Vector3(ActiveLayer.ArrayTerrain[X, Y].WorldPosition.X, ActiveLayer.ArrayTerrain[X, Y].WorldPosition.Z, ActiveLayer.ArrayTerrain[X, Y].WorldPosition.Y);

                                Vector3 Tile3DPositionIn2DTopLeft = GameScreen.GraphicsDevice.Viewport.Project(TilePosition, Camera.Projection, Camera.View, Matrix.Identity);
                                Vector3 Tile3DPositionIn2DBottomRight = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(Map.TileSize.X, 0, Map.TileSize.Y), Camera.Projection, Camera.View, Matrix.Identity);
                                Vector3 Tile3DPositionIn2DTopRight = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(Map.TileSize.X, 0, 0), Camera.Projection, Camera.View, Matrix.Identity);
                                Vector3 Tile3DPositionIn2DBottomLeft = GameScreen.GraphicsDevice.Viewport.Project(TilePosition + new Vector3(0, 0, Map.TileSize.Y), Camera.Projection, Camera.View, Matrix.Identity);

                                if (CursorPosition.X >= Tile3DPositionIn2DBottomLeft.X - Map.TileSize.X * 1.5f && CursorPosition.X <= Tile3DPositionIn2DTopRight.X + Map.TileSize.X * 1.5f
                                    && CursorPosition.Y >= Tile3DPositionIn2DTopLeft.Y - Map.TileSize.X * 1.5f && CursorPosition.Y <= Tile3DPositionIn2DBottomRight.Y + Map.TileSize.Y * 1.5f)
                                {
                                    Vector2 Axis1 = new Vector2(Tile3DPositionIn2DTopRight.X - Tile3DPositionIn2DTopLeft.X, Tile3DPositionIn2DTopRight.Y - Tile3DPositionIn2DTopLeft.Y);
                                    Vector2 Axis2 = new Vector2(Tile3DPositionIn2DBottomLeft.X - Tile3DPositionIn2DTopLeft.X, Tile3DPositionIn2DBottomLeft.Y - Tile3DPositionIn2DTopLeft.Y);
                                    Axis1.Normalize();
                                    Axis2.Normalize();

                                    float MouseValue1 = Vector2.Dot(Axis1, new Vector2(CursorPosition.X, CursorPosition.Y));
                                    float TopLeft1 = Vector2.Dot(Axis1, new Vector2(Tile3DPositionIn2DTopLeft.X, Tile3DPositionIn2DTopLeft.Y));
                                    float TopRight = Vector2.Dot(Axis1, new Vector2(Tile3DPositionIn2DTopRight.X, Tile3DPositionIn2DTopRight.Y));

                                    float MouseValue2 = Vector2.Dot(Axis2, new Vector2(CursorPosition.X, CursorPosition.Y));
                                    float TopLeft2 = Vector2.Dot(Axis2, new Vector2(Tile3DPositionIn2DTopLeft.X, Tile3DPositionIn2DTopLeft.Y));
                                    float BottomLeft = Vector2.Dot(Axis2, new Vector2(Tile3DPositionIn2DBottomLeft.X, Tile3DPositionIn2DBottomLeft.Y));

                                    if (MouseValue1 >= TopLeft1 && MouseValue1 <= TopRight
                                        && MouseValue2 >= TopLeft2 && MouseValue2 <= BottomLeft)
                                    {
                                        ActiveTerrain = ActiveLayer.ArrayTerrain[X, Y];
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SetTarget();
        }

        public void SetTarget()
        {
            Vector3 Target = new Vector3(CameraPosition.X, CameraPosition.Z, CameraPosition.Y);

            float YawRotation = MathHelper.ToRadians(Map.Camera3DYawAngle);
            float PitchRotation = MathHelper.ToRadians(Map.Camera3DPitchAngle);
            float RollRotation = 0;

            Matrix FinalMatrix = Matrix.CreateTranslation(0, Map.Camera3DDistance, 0) * Matrix.CreateFromYawPitchRoll(YawRotation, PitchRotation, RollRotation);
            Camera.CameraPosition3D = FinalMatrix.Translation + Target;

            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Target, Vector3.Up);
        }

        protected override void OnCancelPanel()
        {
            Map.Camera3DOverride = null;
        }

        public override void DoRead(ByteReader BR)
        {
        }

        public override void DoWrite(ByteWriter BW)
        {
        }

        protected override ActionPanel Copy()
        {
            return new ActionPanelViewMap(Map);
        }

        public override void Draw(CustomSpriteBatch g)
        {
            MenuHelper.DrawFingerIcon(g, new Vector2(CursorPosition.X - 84, CursorPosition.Y  - 18));

            if (ActiveTerrain != null)
            {
                if (CursorPosition.X < 880 && CursorPosition.Y < 400)
                {
                    ActionPanelPlayerDefault.DrawLandInformationBottom(g, Map, ActiveTerrain);
                }
                else
                {
                    ActionPanelPlayerDefault.DrawLandInformationTop(g, Map, ActiveTerrain);
                }
            }
        }

        public override string ToString()
        {
            return "Look over the entire map.";
        }
    }
}
