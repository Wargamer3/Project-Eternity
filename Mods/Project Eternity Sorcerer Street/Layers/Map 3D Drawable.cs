using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Map3DDrawable : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        const float CameraHeight = 700;
        const float CameraDistance = 500;
        public static readonly float LayerHeight = 32;

        private SorcererStreetMap Map;

        private Effect MapEffect;
        private Effect ColorEffect;
        private BasicEffect PolygonEffect;
        private Camera3D Camera => Map.Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private List<Tile3D> ListEditorCursorFace;

        private Dictionary<int, Tile3DHolder> DicTile3DByTileset;
        private Dictionary<int, Dictionary<int, Tile3DHolder>> DicTile3DByLayerByTileset;
        private Dictionary<int, Tile3DHolder> DicHiddenTile3DByTileset = new Dictionary<int, Tile3DHolder>();
        private List<DrawableTile> ListIgnoredTerrain = new List<DrawableTile>();

        private Dictionary<Vector4, List<Tile3D>> DicDrawablePointPerColor;
        private List<Tile3D> ListDrawableArrowPerColor;
        private Dictionary<string, Vector3> DicDamageNumberByPosition;

        public Map3DDrawable(SorcererStreetMap Map, LayerHolderSorcererStreet LayerManager, GraphicsDevice g)
        {
            this.Map = Map;
            sprCursor = Map.sprCursor;
            ListEditorCursorFace = new List<Tile3D>();

            MapEffect = Map.Content.Load<Effect>("Shaders/Default Shader 3D 2");
            ColorEffect = Map.Content.Load<Effect>("Shaders/Color Only");
            ColorEffect.Parameters["t0"].SetValue(GameScreen.sprPixel);

            PolygonEffect = new BasicEffect(g);

            PolygonEffect.TextureEnabled = true;

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            PolygonEffect.Projection = Projection;

            PolygonEffect.World = Matrix.Identity;
            PolygonEffect.View = Matrix.Identity;

            MapEffect.Parameters["World"].SetValue(Matrix.Transpose(PolygonEffect.World));

            Matrix worldInverse = Matrix.Invert(PolygonEffect.World);

            MapEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverse);

            // Key light.
            MapEffect.Parameters["DirLight0Direction"].SetValue(new Vector3(-0.5265408f, -0.5735765f, -0.6275069f));
            MapEffect.Parameters["DirLight0DiffuseColor"].SetValue(new Vector3(1, 0.9607844f, 0.8078432f));
            MapEffect.Parameters["DirLight0SpecularColor"].SetValue(new Vector3(1, 0.9607844f, 0.8078432f));

            // Fill light.
            MapEffect.Parameters["DirLight1Direction"].SetValue(new Vector3(0.7198464f, 0.3420201f, 0.6040227f));
            MapEffect.Parameters["DirLight1DiffuseColor"].SetValue(new Vector3(0.9647059f, 0.7607844f, 0.4078432f));
            MapEffect.Parameters["DirLight1SpecularColor"].SetValue(Vector3.Zero);

            // Back light.
            MapEffect.Parameters["DirLight2Direction"].SetValue(new Vector3(0.4545195f, -0.7660444f, 0.4545195f));
            MapEffect.Parameters["DirLight2DiffuseColor"].SetValue(new Vector3(0.3231373f, 0.3607844f, 0.3937255f));
            MapEffect.Parameters["DirLight2SpecularColor"].SetValue(new Vector3(0.3231373f, 0.3607844f, 0.3937255f));

            Vector3 diffuseColor = Vector3.One;
            Vector3 emissiveColor = Vector3.Zero;
            Vector3 ambientLightColor = new Vector3(0.05333332f, 0.09882354f, 0.1819608f);
            Vector4 diffuse = new Vector4();
            Vector3 emissive = new Vector3();
            float alpha = 1;
            diffuse.X = diffuseColor.X * alpha;
            diffuse.Y = diffuseColor.Y * alpha;
            diffuse.Z = diffuseColor.Z * alpha;
            diffuse.W = alpha;

            emissive.X = (emissiveColor.X + ambientLightColor.X * diffuseColor.X) * alpha;
            emissive.Y = (emissiveColor.Y + ambientLightColor.Y * diffuseColor.Y) * alpha;
            emissive.Z = (emissiveColor.Z + ambientLightColor.Z * diffuseColor.Z) * alpha;

            MapEffect.Parameters["DiffuseColor"].SetValue(diffuse);
            MapEffect.Parameters["EmissiveColor"].SetValue(emissive);
            MapEffect.Parameters["SpecularColor"].SetValue(Vector3.One);
            MapEffect.Parameters["SpecularPower"].SetValue(64);

            MapEffect.Parameters["FogLimits"].SetValue(new Vector2(1200, 2000));
            MapEffect.Parameters["FogColor"].SetValue(new Vector3(0.0f, 0.0f, 0.0f));

            DicDrawablePointPerColor = new Dictionary<Vector4, List<Tile3D>>();
            ListDrawableArrowPerColor = new List<Tile3D>();
            DicDamageNumberByPosition = new Dictionary<string, Vector3>();

            CreateMap(Map, LayerManager);

            float Z = LayerManager.ListLayer[0].ArrayTerrain[0, 0].WorldPosition.Z;
            Map2D GroundLayer = LayerManager.ListLayer[0].LayerGrid;
            DrawableTile ActiveTerrain = GroundLayer.GetTile(0, 0);
            Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
            Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                0, 0, Z, 0, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];

            if (Map.IsEditor)
            {
                Map.CursorPosition.X = Map.MapSize.X / 2;
                Map.CursorPosition.Y = Map.MapSize.Y / 2;
                Map.CursorPosition.Z = (LayerManager.ListLayer.Count - 1) / 2;
            }
        }

        private void CreateMap(SorcererStreetMap Map, LayerHolderSorcererStreet LayerManager)
        {
            DicTile3DByTileset = new Dictionary<int, Tile3DHolder>();
            DicHiddenTile3DByTileset = new Dictionary<int, Tile3DHolder>();
            DicTile3DByLayerByTileset = new Dictionary<int, Dictionary<int, Tile3DHolder>>();

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, LayerManager.ListLayer[L], L);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicHiddenTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
            }

            foreach (KeyValuePair<int, Dictionary<int, Tile3DHolder>> ActiveLayer in DicTile3DByLayerByTileset)
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in ActiveLayer.Value)
                {
                    ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
                }
            }
        }

        protected void CreateMap(SorcererStreetMap Map, MapLayer Owner, int LayerIndex)
        {
            foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
            {
                CreateMap(Map, ActiveSubLayer, LayerIndex);
            }

            Map2D GroundLayer = Owner.LayerGrid;

            if (!DicTile3DByLayerByTileset.ContainsKey(LayerIndex))
            {
                DicTile3DByLayerByTileset.Add(LayerIndex, new Dictionary<int, Tile3DHolder>());
            }

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                    Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                    if (ActiveTerrain3D.TerrainStyle == Terrain3D.TerrainStyles.Invisible)
                    {
                        continue;
                    }

                    float Z = Owner.ArrayTerrain[X, Y].WorldPosition.Z * LayerHeight;
                    float MinZ = Z - Owner.ArrayTerrain[X, Y].Height * LayerHeight;
                    float ZFront = MinZ;
                    float ZBack = MinZ;
                    float ZRight = MinZ;
                    float ZLeft = MinZ;
                    if (Y + 1 < Map.MapSize.Y && ConsiderTerrain(Owner.ArrayTerrain[X, Y + 1].DrawableTile.Terrain3DInfo.TerrainStyle))
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].WorldPosition.Z * LayerHeight;
                    }
                    if (Y - 1 >= 0 && ConsiderTerrain(Owner.ArrayTerrain[X, Y - 1].DrawableTile.Terrain3DInfo.TerrainStyle))
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].WorldPosition.Z * LayerHeight;
                    }
                    if (X - 1 >= 0 && ConsiderTerrain(Owner.ArrayTerrain[X - 1, Y].DrawableTile.Terrain3DInfo.TerrainStyle))
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].WorldPosition.Z * LayerHeight;
                    }
                    if (X + 1 < Map.MapSize.X && ConsiderTerrain(Owner.ArrayTerrain[X + 1, Y].DrawableTile.Terrain3DInfo.TerrainStyle))
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].WorldPosition.Z * LayerHeight;
                    }

                    List<Tile3D> ListNew3DTile = ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                                            X * Map.TileSize.X, Y * Map.TileSize.Y, Z, MinZ,
                                            Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0);

                    Tile3D TopTile = ListNew3DTile[0];
                    if (TopTile.ArrayVertex.Length == 4)
                    {
                        float TextureWidth = Map.TileSize.X / (float)Map.ListTileSet[TopTile.TilesetIndex].Width / 10;
                        float TextureHeight = Map.TileSize.Y / (float)Map.ListTileSet[TopTile.TilesetIndex].Height / 10;

                        TopTile.ArrayVertex[0] = new VertexPositionNormalTexture(TopTile.ArrayVertex[0].Position, TopTile.ArrayVertex[0].Normal, new Vector2(TopTile.ArrayVertex[0].TextureCoordinate.X - TextureWidth, TopTile.ArrayVertex[0].TextureCoordinate.Y - TextureHeight));
                        TopTile.ArrayVertex[1] = new VertexPositionNormalTexture(TopTile.ArrayVertex[1].Position, TopTile.ArrayVertex[1].Normal, new Vector2(TopTile.ArrayVertex[1].TextureCoordinate.X + TextureWidth, TopTile.ArrayVertex[1].TextureCoordinate.Y - TextureHeight));
                        TopTile.ArrayVertex[2] = new VertexPositionNormalTexture(TopTile.ArrayVertex[2].Position, TopTile.ArrayVertex[2].Normal, new Vector2(TopTile.ArrayVertex[2].TextureCoordinate.X - TextureWidth, TopTile.ArrayVertex[2].TextureCoordinate.Y + TextureHeight));
                        TopTile.ArrayVertex[3] = new VertexPositionNormalTexture(TopTile.ArrayVertex[3].Position, TopTile.ArrayVertex[3].Normal, new Vector2(TopTile.ArrayVertex[3].TextureCoordinate.X + TextureWidth, TopTile.ArrayVertex[3].TextureCoordinate.Y + TextureHeight));
                    }

                    if (ListIgnoredTerrain.Contains(ActiveTerrain))
                    {
                        foreach (Tile3D ActiveTile in ListNew3DTile)
                        {
                            if (!DicHiddenTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicHiddenTile3DByTileset.Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex], 0.5f));
                            }

                            if (!DicTile3DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicTile3DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                            }

                            DicHiddenTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTile);
                            DicTile3DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile);
                        }
                    }
                    else
                    {
                        foreach (Tile3D ActiveTile in ListNew3DTile)
                        {
                            if (!DicTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicTile3DByTileset.Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                            }

                            if (!DicTile3DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                            {
                                DicTile3DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]));
                            }

                            DicTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTile);
                            DicTile3DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile);
                        }
                    }
                }
            }

            for (int L = 0; L < Owner.ListSubLayer.Count; L++)
            {
                CreateMap(Map, Owner.ListSubLayer[L], LayerIndex);
            }
        }

        private bool ConsiderTerrain(Terrain3D.TerrainStyles TerrainInfo)
        {
            return TerrainInfo != Terrain3D.TerrainStyles.Invisible
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeBackToFront
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeFrontToBack
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeLeftToRight
                && TerrainInfo != Terrain3D.TerrainStyles.SlopeRightToLeft;
        }

        private void Create3DCursor()
        {
            ListEditorCursorFace.Clear();

            int X = (int)Map.CursorPositionVisible.X;
            int Y = (int)Map.CursorPositionVisible.Y;
            float ZTop = (Map.CursorPosition.Z + 1) * LayerHeight + 0.3f;
            float ZBottom = Map.CursorPosition.Z * LayerHeight + 0.3f;
            Terrain3D Cursor = new Terrain3D();
            Cursor.TerrainStyle = Terrain3D.TerrainStyles.Cube;
            Cursor.FrontFace = new DrawableTile();
            Cursor.FrontFace.TilesetIndex = 0;
            Cursor.FrontFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);
            Cursor.BackFace = new DrawableTile();
            Cursor.BackFace.TilesetIndex = 0;
            Cursor.BackFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);
            Cursor.LeftFace = new DrawableTile();
            Cursor.LeftFace.TilesetIndex = 0;
            Cursor.LeftFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);
            Cursor.RightFace = new DrawableTile();
            Cursor.RightFace.TilesetIndex = 0;
            Cursor.RightFace.Origin = new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y);

            ListEditorCursorFace = Cursor.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, ZTop, ZBottom, Map.TileSize, new List<Texture2D>() { sprCursor }, ZBottom, ZBottom, ZBottom, ZBottom, 0);
            ListEditorCursorFace.Add(Cursor.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, ZBottom, ZBottom, Map.TileSize, new List<Texture2D>() { sprCursor }, ZBottom, ZBottom, ZBottom, ZBottom, 0)[0]);
        }

        public void Update(GameTime gameTime)
        {
            if (Map.ActivePlatform == null && (!Map.IsAPlatform || Map.IsPlatformActive))
            {
                UpdateCamera();

                int X = (int)Map.CursorPositionVisible.X;
                int Y = (int)Map.CursorPositionVisible.Y;
                float Z = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[X, Y].WorldPosition.Z * LayerHeight + 0.3f;
                Map2D GroundLayer = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].LayerGrid;
                DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                    X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Map.CursorPosition.Z * LayerHeight + 0.3f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];
            }

            if (!Map.IsAPlatform && Map.ListPlayer.Count > 0)
            {
                SetTarget(new Vector3(Map.TileSize.X * Map.ListPlayer[Map.ActivePlayerIndex].GamePiece.X, Map.ListPlayer[Map.ActivePlayerIndex].GamePiece.Z * LayerHeight, Map.TileSize.Y * Map.ListPlayer[Map.ActivePlayerIndex].GamePiece.Y));

                Camera.Update(gameTime);
            }

            DicDrawablePointPerColor.Clear();
            ListDrawableArrowPerColor.Clear();
            DicDamageNumberByPosition.Clear();
            if (Map.IsEditor)
            {
                Create3DCursor();
            }

            foreach (Player ActivePlayer in Map.ListPlayer)
            {
                //If it's dead, don't draw it unless it's an event unit.
                if (ActivePlayer.GamePiece.Unit3DModel == null)
                    continue;

                ActivePlayer.GamePiece.Unit3DModel.Update(gameTime);
            }
        }

        public void SetTarget(Vector3 Target)
        {
            Camera.CameraPosition3D = Vector3.Transform(new Vector3(0, 0, 200), Matrix.CreateRotationY(MathHelper.ToRadians(40))) + Target;
            Camera.CameraPosition3D = Vector3.Transform(Camera.CameraPosition3D, Matrix.CreateTranslation(0f, 150, 0f));
            Camera.View = Matrix.CreateLookAt(Camera.CameraPosition3D, Target, Vector3.Up);
        }

        private void UpdateCamera()
        {
            if (Map.IsEditor)
            {
                KeyboardHelper.UpdateKeyboardStatus();
                Map.CursorControl(new KeyboardInput());
            }
            if (KeyboardHelper.KeyPressed(Keys.Q))
            {
                float NextZ = Map.CursorPosition.Z;

                if (KeyboardHelper.KeyHold(Keys.LeftAlt))
                {
                    NextZ += 1f;
                }
                else
                {
                    do
                    {
                        NextZ += 1f;
                    }
                    while (NextZ < Map.LayerManager.ListLayer.Count
                        && Map.LayerManager.ListLayer[(int)NextZ].ArrayTerrain[(int)Map.CursorPosition.X, (int)Map.CursorPosition.Y].TerrainTypeIndex == UnitStats.TerrainVoidIndex);

                    if (NextZ >= Map.LayerManager.ListLayer.Count)
                    {
                        NextZ = Map.CursorPosition.Z + 1f;
                    }
                }

                if (NextZ >= Map.LayerManager.ListLayer.Count)
                {
                    NextZ = Map.LayerManager.ListLayer.Count - 1;
                }

                Map.CursorPosition.Z = NextZ;
            }
            else if (KeyboardHelper.KeyPressed(Keys.E))
            {
                float NextZ = Map.CursorPosition.Z;

                if (KeyboardHelper.KeyHold(Keys.LeftAlt))
                {
                    NextZ -= 1f;
                }
                else
                {
                    do
                    {
                        NextZ -= 1f;
                    }
                    while (NextZ >= 0
                        && Map.LayerManager.ListLayer[(int)NextZ].ArrayTerrain[(int)Map.CursorPosition.X, (int)Map.CursorPosition.Y].TerrainTypeIndex == UnitStats.TerrainVoidIndex);

                    if (NextZ < 0)
                    {
                        NextZ = Map.CursorPosition.Z - 1f;
                    }
                }

                if (NextZ < 0)
                {
                    NextZ = 0;
                }

                Map.CursorPosition.Z = NextZ;
            }

            if (Map.IsEditor)
            {
                KeyboardHelper.PlayerStateLast = Keyboard.GetState();
            }
        }

        public void SetWorld(Matrix NewWorld)
        {
            PolygonEffect.World = NewWorld;

            Matrix worldInverse = Matrix.Invert(NewWorld);

            MapEffect.Parameters["World"].SetValue(Matrix.Transpose(NewWorld));

            MapEffect.Parameters["WorldInverseTranspose"].SetValue(worldInverse);

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                if (Map.CameraOverride != null)
                {
                    ActiveTileSet.Value.SetWorld(NewWorld * Map.CameraOverride.View);
                }
                else
                {
                    ActiveTileSet.Value.SetWorld(NewWorld * Map.Camera.View);
                }
            }
        }

        private bool IsCursorHiddenByWall()
        {
            int CursorX = (int)Map.CursorPosition.X;
            int CursorLayer = (int)Map.CursorPosition.Z;
            for (int Y = (int)Map.CursorPosition.Y; Y < Map.MapSize.Y; ++Y)
            {
                if (CursorLayer < Map.LayerManager.ListLayer.Count && Map.LayerManager.ListLayer[CursorLayer].ArrayTerrain[CursorX, Y].TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    return true;
                }
                ++CursorLayer;
            }

            return false;
        }

        public Point GetVisiblePosition(Vector3 Position)
        {
            Point BaseMenuPosition;

            Vector3 Visible3DPosition = new Vector3(Position.X * Map.TileSize.X, Position.Z * LayerHeight, Position.Y * Map.TileSize.Y);

            if (Map.ActivePlatform !=null)
            {
                Visible3DPosition = new Vector3(Map.ActivePlatform.Map.CursorTerrain.WorldPosition.X * Map.ActivePlatform.Map.TileSize.X,
                    Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Z * LayerHeight,
                    Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Y * Map.ActivePlatform.Map.TileSize.Y);
            }

            Vector3 Position2D = GameScreen.GraphicsDevice.Viewport.Project(Visible3DPosition, PolygonEffect.Projection, PolygonEffect.View, Matrix.Identity);

            BaseMenuPosition.X = (int)Position2D.X;
            BaseMenuPosition.Y = (int)Position2D.Y;

            return BaseMenuPosition;
        }

        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (MovementAlgorithmTile ActivePoint in ListPoint)
            {
                int X = (int)ActivePoint.WorldPosition.X;
                int Y = (int)ActivePoint.WorldPosition.Y;
                float Z = ActivePoint.WorldPosition.Z * LayerHeight + 0.1f;
                float MinZ = Z - ActivePoint.Height * LayerHeight;
                DrawableTile ActiveTerrain = ActivePoint.DrawableTile;
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawablePoint3D.Add(ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * ActivePoint.Owner.TileSize.X, Y * ActivePoint.Owner.TileSize.Y, Z, MinZ, ActivePoint.Owner.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0]);
            }

            DicDrawablePointPerColor.Add(new Vector4(PointColor.R / 255f, PointColor.G / 255f, PointColor.B / 255f, PointColor.A / 255f), ListDrawablePoint3D);
        }

        public void AddDrawablePath(List<MovementAlgorithmTile> ListPoint)
        {
            for (int P = 1; P < ListPoint.Count; P++)
            {
                MovementAlgorithmTile ActivePoint = ListPoint[P];
                MovementAlgorithmTile Previous = ListPoint[P - 1];
                MovementAlgorithmTile Next = null;
                if (P + 1 < ListPoint.Count)
                {
                    Next = ListPoint[P + 1];
                }

                int X = (int)ActivePoint.WorldPosition.X;
                int Y = (int)ActivePoint.WorldPosition.Y;
                float Z = ActivePoint.WorldPosition.Z * LayerHeight + 0.15f;
                float MinZ = Z - ActivePoint.Height * LayerHeight;
                DrawableTile ActiveTerrain = ActivePoint.DrawableTile;
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawableArrowPerColor.Add(ActiveTerrain3D.CreateTile3D(0, GetCursorTextureOffset(Previous, ActivePoint, Next),
                X * ActivePoint.Owner.TileSize.X, Y * ActivePoint.Owner.TileSize.Y, Z, MinZ, ActivePoint.Owner.TileSize, new List<Texture2D>() { Map.sprCursorPath }, Z, Z, Z, Z, 0)[0]);
            }
        }

        public void AddDamageNumber(string Damage, Vector3 Position)
        {
            DicDamageNumberByPosition.Add(Damage, Position);
        }

        private Point GetCursorTextureOffset(MovementAlgorithmTile Previous, MovementAlgorithmTile Current, MovementAlgorithmTile Next)
        {
            const int TextureSize = 32;

            Point TextureOffset = Point.Zero;
            if (Next == null)
            {
                if (Previous != null && Previous.WorldPosition.X < Current.WorldPosition.X)//Right
                {
                    TextureOffset.X = TextureSize * 3;
                }
                else if (Previous != null && Previous.WorldPosition.X > Current.WorldPosition.X)//Left
                {
                    TextureOffset.X = TextureSize * 4;
                }
                else if (Previous != null && Previous.WorldPosition.Y < Current.WorldPosition.Y)//Down
                {
                    TextureOffset.X = TextureSize * 5;
                }
                else if (Previous != null && Previous.WorldPosition.Y > Current.WorldPosition.Y)//Up
                {
                    TextureOffset.X = TextureSize * 2;
                }
            }
            else if (Current.WorldPosition.X < Next.WorldPosition.X)//Going Right
            {
                if (Previous != null && Previous.WorldPosition.Y < Current.WorldPosition.Y)//Down Right
                {
                    TextureOffset.X = TextureSize * 9;
                }
                else if (Previous != null && Previous.WorldPosition.Y > Current.WorldPosition.Y)//Up Right
                {
                    TextureOffset.X = TextureSize * 8;
                }
                else
                {
                    TextureOffset.X = TextureSize * 1;
                }
            }
            else if (Current.WorldPosition.X > Next.WorldPosition.X)//Going Left
            {
                if (Previous != null && Previous.WorldPosition.Y < Current.WorldPosition.Y)//Down Left
                {
                    TextureOffset.X = TextureSize * 7;
                }
                else if (Previous != null && Previous.WorldPosition.Y > Current.WorldPosition.Y)//Up Left
                {
                    TextureOffset.X = TextureSize * 6;
                }
                else
                {
                    TextureOffset.X = TextureSize * 1;
                }
            }
            else if (Current.WorldPosition.Y < Next.WorldPosition.Y)//Going Down
            {
                if (Previous != null && Previous.WorldPosition.X < Current.WorldPosition.X)//Right Down
                {
                    TextureOffset.X = TextureSize * 6;
                }
                else if (Previous != null && Previous.WorldPosition.X > Current.WorldPosition.X)//Left Down
                {
                    TextureOffset.X = TextureSize * 8;
                }
                else
                {
                    TextureOffset.X = TextureSize * 0;
                }
            }
            else if (Current.WorldPosition.Y > Next.WorldPosition.Y)//Going Up
            {
                if (Previous != null && Previous.WorldPosition.X < Current.WorldPosition.X)//Right Up
                {
                    TextureOffset.X = TextureSize * 7;
                }
                else if (Previous != null && Previous.WorldPosition.X > Current.WorldPosition.X)//Left Up
                {
                    TextureOffset.X = TextureSize * 9;
                }
                else
                {
                    TextureOffset.X = TextureSize * 0;
                }
            }

            return TextureOffset;
        }

        public void Reset()
        {
            CreateMap(Map, Map.LayerManager);
        }

        public void CursorMoved()
        {
            FilterTerrainObscuringUnits();
        }

        private void FilterTerrainObscuringUnits()
        {
            ListIgnoredTerrain.Clear();

            foreach (Player ActivePlayer in Map.ListPlayer)
            {
                if (ActivePlayer.GamePiece.Z + 1 < Map.LayerManager.ListLayer.Count)
                {
                    Terrain UpperTerrain = Map.GetTerrain(new Vector3(ActivePlayer.GamePiece.X, ActivePlayer.GamePiece.Z, ActivePlayer.GamePiece.Z + 1));
                    TerrainType UpperTerrainType = Map.TerrainRestrictions.ListTerrainType[UpperTerrain.TerrainTypeIndex];

                    if (UpperTerrainType.ListRestriction.Count > 0 || UpperTerrain.DrawableTile.Terrain3DInfo.TerrainStyle != Terrain3D.TerrainStyles.Invisible
                        && !ListIgnoredTerrain.Contains(UpperTerrain.DrawableTile))
                    {
                        ListIgnoredTerrain.Add(UpperTerrain.DrawableTile);
                        FloodFill(new Vector3(UpperTerrain.InternalPosition.X, UpperTerrain.InternalPosition.Y, UpperTerrain.LayerIndex), UpperTerrain.TerrainTypeIndex);
                    }
                }
            }

            CreateMap(Map, Map.LayerManager);
        }

        private void FloodFill(Vector3 StartingPosition, byte TargetTerrainIndex)
        {
            int Z = (int)StartingPosition.Z;

            Stack<Vector3> ListTerrainPositionToHide = new Stack<Vector3>();

            ListTerrainPositionToHide.Push(StartingPosition);
            while (ListTerrainPositionToHide.Count != 0)
            {
                Vector3 ActivePosition = ListTerrainPositionToHide.Pop();
                float ActiveY = ActivePosition.Y;
                Terrain ActiveTerrain = Map.GetTerrain(new Vector3(ActivePosition.X, ActiveY, Z));

                while (ActiveY >= 0 && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTerrain.DrawableTile))
                {
                    if (--ActiveY >= 0)
                    {
                        ActiveTerrain = Map.GetTerrain(new Vector3(ActivePosition.X, ActiveY, Z));
                    }
                }

                ActiveY++;
                bool spanLeft = false;
                bool spanRight = false;

                while (ActiveY < Map.MapSize.Y && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex)
                {
                    ListIgnoredTerrain.Add(Map.GetTerrain(new Vector3(ActivePosition.X, ActiveY, Z)).DrawableTile);

                    if (ActivePosition.X - 1 >= 0)
                    {
                        ActiveTerrain = Map.GetTerrain(new Vector3(ActivePosition.X - 1, ActiveY, Z));

                        if (!spanLeft && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTerrain.DrawableTile))
                        {
                            ListTerrainPositionToHide.Push(new Vector3(ActivePosition.X - 1, ActiveY, Z));
                            spanLeft = true;
                        }
                        else if (spanLeft && ActivePosition.X - 1 == 0 && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTerrain.DrawableTile))
                        {
                            spanLeft = false;

                        }
                    }

                    if (ActivePosition.X + 1 < Map.MapSize.X)
                    {
                        ActiveTerrain = Map.GetTerrain(new Vector3(ActivePosition.X + 1, ActiveY, Z));

                        if (!spanRight && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTerrain.DrawableTile))
                        {
                            ListTerrainPositionToHide.Push(new Vector3(ActivePosition.X + 1, ActiveY, Z));
                            spanRight = true;
                        }
                        else if (spanRight && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTerrain.DrawableTile))
                        {
                            spanRight = false;
                        }
                    }

                    ActiveY++;
                }
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Matrix View = Map.Camera.View;
            Matrix World = PolygonEffect.World;
            Vector3 CameraPosition = Map.Camera.CameraPosition3D;

            if (Map.CameraOverride != null)
            {
                View = Map.CameraOverride.View;
                CameraPosition = Map.CameraOverride.CameraPosition3D;
            }
            PolygonEffect.View = View;
            Matrix ViewProjection = View * PolygonEffect.Projection;
            Matrix WorldViewProjection = World * ViewProjection;
            ColorEffect.Parameters["ViewProjection"].SetValue(ViewProjection);

            DrawMap(g, View, WorldViewProjection);

            DrawVehicles(g, View);

            DrawDrawablePoints(g);

            if ((Map.ActivePlatform == null && !Map.IsAPlatform) || Map.IsPlatformActive)
            {
                g.GraphicsDevice.DepthStencilState = DepthStencilState.None;
                PolygonEffect.Texture = sprCursor;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                if (Map.IsEditor)
                {
                    g.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                    foreach (Tile3D CursorFace in ListEditorCursorFace)
                    {
                        CursorFace.Draw(g.GraphicsDevice);
                    }
                    g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
                }
                else
                {
                    Cursor.Draw(g.GraphicsDevice);
                }
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            DrawPlayers(g, View);

            DrawCreatures(g, View);

            DrawDelayedAttacks(g);

            DrawPERAttacks(g, View);

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicHiddenTile3DByTileset)
            {
                ActiveTileSet.Value.SetViewMatrix(WorldViewProjection, CameraPosition);

                ActiveTileSet.Value.Draw(g.GraphicsDevice);
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            g.End();
            g.Begin();

            DrawDamageNumbers(g, View);
        }

        private void DrawMap(CustomSpriteBatch g, Matrix View, Matrix WorldViewProjection)
        {
            Vector3 CameraPosition = Map.Camera.CameraPosition3D;
            if (Map.CameraOverride != null)
            {
                CameraPosition = Map.CameraOverride.CameraPosition3D;
            }

            if (Map.ShowLayerIndex == -1)
            {
                bool DrawUpperLayers = !IsCursorHiddenByWall();

                if (DrawUpperLayers || Map.IsEditor)
                {
                    foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
                    {
                        ActiveTileSet.Value.SetViewMatrix(WorldViewProjection, CameraPosition);

                        ActiveTileSet.Value.Draw(g.GraphicsDevice);
                    }

                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                    {
                        DrawItems(g, View, Map.LayerManager.ListLayer[L], false);
                    }
                }
                else
                {
                    int MaxLayerIndex = Map.LayerManager.ListLayer.Count;
                    if (!DrawUpperLayers)
                    {
                        MaxLayerIndex = (int)Map.CursorPosition.Z + 1;
                    }

                    for (int L = 0; L < MaxLayerIndex; L++)
                    {
                        foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.SetViewMatrix(WorldViewProjection, Camera.CameraPosition3D);

                            ActiveTileSet.Value.Draw(g.GraphicsDevice);
                        }

                        DrawItems(g, View, Map.LayerManager.ListLayer[L], false);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByLayerByTileset[Map.ShowLayerIndex])
                {
                    ActiveTileSet.Value.SetViewMatrix(WorldViewProjection, Camera.CameraPosition3D);

                    ActiveTileSet.Value.Draw(g.GraphicsDevice);
                }

                DrawItems(g, View, Map.LayerManager.ListLayer[Map.ShowLayerIndex], false);
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.None;
            for (int L = 0; L < Map.LayerManager.ListLayer.Count; ++L)
            {
                for (int X = 0; X < Map.MapSize.X; ++X)
                {
                    for (int Y = 0; Y < Map.MapSize.Y; ++Y)
                    {
                        float Z = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[X, Y].WorldPosition.Z * LayerHeight + 0.3f;
                        Map2D GroundLayer = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].LayerGrid;
                        DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                        Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                        if (ActiveTerrain3D.TerrainStyle != Terrain3D.TerrainStyles.Invisible)
                        {
                            if (Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[X, Y].DefendingCreature != null)
                            {
                                PolygonEffect.Texture = Map.sprTileBorderColor;
                            }
                            else
                            {
                                PolygonEffect.Texture = Map.sprTileBorderEmpty;
                            }
                            PolygonEffect.CurrentTechnique.Passes[0].Apply();

                            Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Map.CursorPosition.Z * LayerHeight + 0.3f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];

                            Cursor.Draw(g.GraphicsDevice);
                        }
                    }
                }
            }
        }

        public void DrawItems(CustomSpriteBatch g, Matrix View, MapLayer Owner, bool IsSubLayer)
        {
            if (!Owner.IsVisible)
            {
                return;
            }

            if (IsSubLayer)
            {
                return;
            }

            for (int P = 0; P < Owner.ListProp.Count; ++P)
            {
                Owner.ListProp[P].Draw3D(GameScreen.GraphicsDevice,View, g);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.SetViewMatrix(View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListAttackPickup[P].Position.X, (int)Owner.ListAttackPickup[P].Position.Y].WorldPosition.Z;

                Owner.ListAttackPickup[P].Attack3D.SetPosition(
                    (Owner.ListAttackPickup[P].Position.X + 0.5f) * Map.TileSize.X,
                    TerrainZ * LayerHeight,
                    (Owner.ListAttackPickup[P].Position.Y + 0.5f) * Map.TileSize.Y);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.Draw(GameScreen.GraphicsDevice);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.SetViewMatrix(View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListHoldableItem[I].Position.X, (int)Owner.ListHoldableItem[I].Position.Y].WorldPosition.Z;

                Owner.ListHoldableItem[I].Item3D.SetPosition(
                    (Owner.ListHoldableItem[I].Position.X + 0.5f) * Map.TileSize.X,
                    TerrainZ * LayerHeight,
                    (Owner.ListHoldableItem[I].Position.Y + 0.5f) * Map.TileSize.Y);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.Draw(GameScreen.GraphicsDevice);
            }
        }

        private void DrawPlayers(CustomSpriteBatch g, Matrix View)
        {
            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                Player ActivePlayer = Map.ListPlayer[P];
                //If it's dead, don't draw it unless it's an event unit.
                if (ActivePlayer.GamePiece == null)
                    continue;

                if (Map.MovementAnimation.Contains(ActivePlayer.GamePiece))
                {
                    Vector3 CurrentPosition = Map.MovementAnimation.GetPosition(ActivePlayer.GamePiece);

                    if (ActivePlayer.GamePiece.Speed == Vector3.Zero)
                    {
                        CurrentPosition = Map.LayerManager.ListLayer[(int)CurrentPosition.Z].ArrayTerrain[(int)Math.Floor(CurrentPosition.X + 0.5f), (int)Math.Floor(CurrentPosition.Y + 0.5f)].GetRealPosition(CurrentPosition + new Vector3(0.5f, 0.5f, 0));
                    }

                    if (ActivePlayer.GamePiece.ItemHeld != null)
                    {
                        ActivePlayer.GamePiece.ItemHeld.Item3D.SetViewMatrix(View);

                        ActivePlayer.GamePiece.ItemHeld.Item3D.SetPosition(
                            CurrentPosition.X * Map.TileSize.X,
                            (CurrentPosition.Z + 1f) * LayerHeight,
                            (CurrentPosition.Y - 0.5f) * Map.TileSize.Y);

                        ActivePlayer.GamePiece.ItemHeld.Item3D.Draw(GameScreen.GraphicsDevice);
                    }

                    if (ActivePlayer.GamePiece.Unit3DModel == null)
                    {
                        ActivePlayer.GamePiece.Unit3DSprite.SetViewMatrix(View);

                        ActivePlayer.GamePiece.Unit3DSprite.SetPosition(
                            CurrentPosition.X * Map.TileSize.X,
                            (CurrentPosition.Z + 0.5f) * LayerHeight,
                            CurrentPosition.Y * Map.TileSize.Y);

                        ActivePlayer.GamePiece.Unit3DSprite.UnitEffect3D.Parameters["Greyscale"].SetValue(true);

                        ActivePlayer.GamePiece.Unit3DSprite.Draw(GameScreen.GraphicsDevice);
                    }
                    else
                    {
                        ActivePlayer.GamePiece.Unit3DModel.PlayAnimation("Walking");
                        Matrix RotationMatrix = Matrix.Identity;
                        if (ActivePlayer.GamePiece.Direction == UnitMapComponent.Directions.Right)
                        {
                            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(90));
                        }
                        else if (ActivePlayer.GamePiece.Direction == UnitMapComponent.Directions.Up)
                        {
                            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(180));
                        }
                        else if (ActivePlayer.GamePiece.Direction == UnitMapComponent.Directions.Left)
                        {
                            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(270));
                        }

                        ActivePlayer.GamePiece.Unit3DModel.Draw(View, PolygonEffect.Projection, Matrix.CreateScale(0.2f) * RotationMatrix
                            * Matrix.CreateTranslation(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Z * LayerHeight, CurrentPosition.Y * Map.TileSize.Y));
                    }
                }
                else
                {
                    Color UnitColor;
                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        UnitColor = ActivePlayer.Color;
                    else
                        UnitColor = Color.White;

                    Vector3 CurrentPosition = ActivePlayer.GamePiece.Position;

                    if (ActivePlayer.GamePiece.Speed == Vector3.Zero)
                    {
                        CurrentPosition = Map.LayerManager.ListLayer[(int)CurrentPosition.Z].ArrayTerrain[(int)Math.Floor(CurrentPosition.X + 0.5f), (int)Math.Floor(CurrentPosition.Y + 0.5f)].GetRealPosition(CurrentPosition + new Vector3(0.5f, 0.5f, 0));
                    }

                    if (ActivePlayer.GamePiece.ItemHeld != null)
                    {
                        ActivePlayer.GamePiece.ItemHeld.Item3D.SetViewMatrix(View);

                        ActivePlayer.GamePiece.ItemHeld.Item3D.SetPosition(
                            CurrentPosition.X * Map.TileSize.X,
                            (CurrentPosition.Z + 1f) * LayerHeight,
                            (CurrentPosition.Y - 0.5f) * Map.TileSize.Y);

                        ActivePlayer.GamePiece.ItemHeld.Item3D.Draw(GameScreen.GraphicsDevice);
                    }

                    if (ActivePlayer.GamePiece.Unit3DModel == null)
                    {
                        ActivePlayer.GamePiece.Unit3DSprite.SetViewMatrix(View);

                        ActivePlayer.GamePiece.Unit3DSprite.SetPosition(
                            CurrentPosition.X * Map.TileSize.X,
                            (CurrentPosition.Z + 0.5f) * LayerHeight,
                            CurrentPosition.Y * Map.TileSize.Y);

                        ActivePlayer.GamePiece.Unit3DSprite.UnitEffect3D.Parameters["Greyscale"].SetValue(!ActivePlayer.GamePiece.CanMove && P == Map.ActivePlayerIndex);

                        ActivePlayer.GamePiece.Unit3DSprite.Draw(GameScreen.GraphicsDevice);
                    }
                    else
                    {
                        ActivePlayer.GamePiece.Unit3DModel.PlayAnimation("Idle");
                        Matrix RotationMatrix = Matrix.Identity;
                        if (ActivePlayer.GamePiece.Direction == UnitMapComponent.Directions.Right)
                        {
                            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(90));
                        }
                        else if (ActivePlayer.GamePiece.Direction == UnitMapComponent.Directions.Up)
                        {
                            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(180));
                        }
                        else if (ActivePlayer.GamePiece.Direction == UnitMapComponent.Directions.Left)
                        {
                            RotationMatrix = Matrix.CreateRotationY(MathHelper.ToRadians(270));
                        }

                        ActivePlayer.GamePiece.Unit3DModel.Draw(View, PolygonEffect.Projection, Matrix.CreateScale(0.2f) * RotationMatrix
                            * Matrix.CreateTranslation(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Z * LayerHeight, CurrentPosition.Y * Map.TileSize.Y));
                    }
                }
            }
        }

        private void DrawCreatures(CustomSpriteBatch g, Matrix View)
        {
            for (int Z = 0; Z < Map.LayerManager.ListLayer.Count; ++Z)
            {
                for (int X = MapSize.X - 1; X >= 0; --X)
                {
                    for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                    {
                        TerrainSorcererStreet ActiveTerrain = Map.GetTerrain(X, Y, Z);
                        CreatureCard DefendingCreature = ActiveTerrain.DefendingCreature;
                        if (DefendingCreature != null)
                        {
                            DefendingCreature.Map3DModel.Draw(View, PolygonEffect.Projection,  Matrix.CreateTranslation((ActiveTerrain.WorldPosition.X + 0.9f) * Map.TileSize.X, ActiveTerrain.WorldPosition.Z * LayerHeight, (ActiveTerrain.WorldPosition.Y + 0.9f) * Map.TileSize.Y));

                            Vector3 Visible3DPosition = new Vector3(ActiveTerrain.WorldPosition.X + 0.7f, ActiveTerrain.WorldPosition.Z * LayerHeight, ActiveTerrain.WorldPosition.Y + 0.9f);
                            Vector3 Position = new Vector3(Visible3DPosition.X * Map.TileSize.X, Visible3DPosition.Y, Visible3DPosition.Z * Map.TileSize.Y);

                            Vector3 Position2D = g.GraphicsDevice.Viewport.Project(Position, PolygonEffect.Projection, View, Matrix.Identity);
                            g.DrawString(Map.fntNonDemoDamage, ActiveTerrain.CurrentToll.ToString(), new Vector2(Position2D.X, Position2D.Y), Color.White);
                        }
                    }
                } }
        }

        private void DrawDrawablePoints(CustomSpriteBatch g)
        {
            ColorEffect.CurrentTechnique.Passes[0].Apply();

            foreach (KeyValuePair<Vector4, List<Tile3D>> DrawablePointPerColor in DicDrawablePointPerColor)
            {
                ColorEffect.Parameters["Color"].SetValue(DrawablePointPerColor.Key);

                ColorEffect.CurrentTechnique.Passes[0].Apply();

                foreach (Tile3D DrawablePoint in DrawablePointPerColor.Value)
                {
                    DrawablePoint.Draw(g.GraphicsDevice);
                }
            }

            PolygonEffect.Texture = Map.sprCursorPath;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();

            foreach (Tile3D DrawablePoint in ListDrawableArrowPerColor)
            {
                DrawablePoint.Draw(g.GraphicsDevice);
            }
        }

        private void DrawDelayedAttacks(CustomSpriteBatch g)
        {
            /*int BorderX = (int)(TileSize.X * 0.1);
            int BorderY = (int)(TileSize.Y * 0.1);

            foreach (DelayedAttack ActiveAttack in Map.ListDelayedAttack)
            {
                foreach (Vector3 ActivePosition in ActiveAttack.ListAttackPosition)
                {
                    g.Draw(GameScreen.sprPixel,
                        new Rectangle(
                            (int)(ActivePosition.X - CameraPosition.X) * TileSize.X + BorderX,
                            (int)(ActivePosition.Y - CameraPosition.Y) * TileSize.Y + BorderY,
                            TileSize.X - BorderX * 2,
                            TileSize.Y - BorderY * 2), Color.FromNonPremultiplied(139, 0, 0, 190));
                }
            }*/
        }

        private void DrawPERAttacks(CustomSpriteBatch g, Matrix View)
        {
            /*foreach (PERAttack ActiveAttack in Map.ListPERAttack)
            {
                Vector3 CurrentPosition = ActiveAttack.Position;

                if (ActiveAttack.IsOnGround)
                {
                    CurrentPosition = Map.LayerManager.ListLayer[(int)CurrentPosition.Z].ArrayTerrain[(int)Math.Floor(CurrentPosition.X), (int)Math.Floor(CurrentPosition.Y)].GetRealPosition(CurrentPosition);
                }

                if (ActiveAttack.Map3DComponent != null)
                {
                    ActiveAttack.Map3DComponent.SetPosition(CurrentPosition.X * 32, CurrentPosition.Z * 32 + 16, CurrentPosition.Y * 32);
                    ActiveAttack.Map3DComponent.SetViewMatrix(View);
                    ActiveAttack.Map3DComponent.Draw(g.GraphicsDevice);
                }
                else if (ActiveAttack.Unit3DModel != null)
                {
                    ActiveAttack.Unit3DModel.Draw(View, PolygonEffect.Projection, Matrix.CreateTranslation(CurrentPosition.X * Map.TileSize.X, CurrentPosition.Z * LayerHeight, CurrentPosition.Y * Map.TileSize.Y));
                }
            }*/
        }

        private void DrawDamageNumbers(CustomSpriteBatch g, Matrix View)
        {
            foreach (KeyValuePair<string, Vector3> ActiveAttack in DicDamageNumberByPosition)
            {
                Vector3 Visible3DPosition = new Vector3(ActiveAttack.Value.X, ActiveAttack.Value.Z * LayerHeight, ActiveAttack.Value.Y);
                Vector3 Position = new Vector3(Visible3DPosition.X * Map.TileSize.X, Visible3DPosition.Y + 16, Visible3DPosition.Z * Map.TileSize.Y);

                Vector3 Position2D = g.GraphicsDevice.Viewport.Project(Position, PolygonEffect.Projection, View, Matrix.Identity);
                g.DrawString(Map.fntNonDemoDamage, ActiveAttack.Key, new Vector2(Position2D.X, Position2D.Y), Color.White);
            }
        }

        private void DrawVehicles(CustomSpriteBatch g, Matrix View)
        {
            foreach (Vehicle ActiveVehicle in Map.ListVehicle)
            {
                PolygonEffect.World = ActiveVehicle.World;
                PolygonEffect.Texture = ActiveVehicle.sprVehicle;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                ActiveVehicle.Draw3D(g.GraphicsDevice);

                foreach (VehicleSeat ActiveSeat in ActiveVehicle.ListSeat)
                {
                    if (ActiveSeat.User != null)
                    {
                        ActiveSeat.User.Unit3DSprite.SetViewMatrix(View);

                        Vector3 UserPositon = new Vector3(ActiveVehicle.Position.X - ActiveVehicle.sprVehicle.Width / 2 + ActiveSeat.SeatOffset.X,
                            ActiveVehicle.Position.Y, ActiveVehicle.Position.Z - ActiveVehicle.sprVehicle.Height / 2 + ActiveSeat.SeatOffset.Y);
                        var a = Matrix.CreateTranslation(
                            new Vector3(
                                -ActiveVehicle.Position.X,
                                -ActiveVehicle.Position.Y,
                                -ActiveVehicle.Position.Z))
                            * Matrix.CreateRotationY(ActiveVehicle.Yaw) * Matrix.CreateTranslation(ActiveVehicle.Position);
                        Vector3 UserPos2 = Vector3.Transform(UserPositon, a);
                        ActiveSeat.User.Unit3DSprite.SetPosition(
                            UserPos2.X,
                            UserPos2.Y + 8,
                            UserPos2.Z);

                        ActiveSeat.User.Unit3DSprite.Draw(GameScreen.GraphicsDevice);
                    }
                }
            }

            PolygonEffect.World = Map.World;
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }
}
