using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Map3DDrawable : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        const float CameraHeight = 700;
        const float CameraDistance = 500;

        private DeathmatchMap Map;

        private Effect MapEffect;
        private Effect ColorEffect;
        private BasicEffect PolygonEffect;
        private Camera3D Camera => Map.Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private List<Tile3D> ListEditorCursorFace;
        private Dictionary<int, Tile3DHolder> DicTile3DByTileset;
        private Dictionary<int, Dictionary<int, Tile3DHolder>> DicTile3DByLayerByTileset;
        private Dictionary<Vector4, List<Tile3D>> DicDrawablePointPerColor;
        private List<Tile3D> ListDrawableArrowPerColor;
        private Dictionary<string, Vector3> DicDamageNumberByPosition;

        UnitMap3D TestUnit;
        public Map3DDrawable(DeathmatchMap Map, LayerHolderDeathmatch LayerManager, GraphicsDevice g)
        {
            this.Map = Map;
            sprCursor = Map.sprCursor;
            ListEditorCursorFace = new List<Tile3D>();

            TestUnit = new UnitMap3D(GameScreen.GraphicsDevice, Map.Content.Load<Effect>("Shaders/Billboard 3D 2"), Map.Content.Load<Texture2D>("Units/Normal/Map Sprite/Mazinger"), 1);
            MapEffect = Map.Content.Load<Effect>("Shaders/Default Shader 3D 2");
            ColorEffect = Map.Content.Load<Effect>("Shaders/Color Only");
            ColorEffect.Parameters["t0"].SetValue(GameScreen.sprPixel);

            PolygonEffect = new BasicEffect(g);

            PolygonEffect.TextureEnabled = true;
            PolygonEffect.EnableDefaultLighting();

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            PolygonEffect.Projection = Projection;

            PolygonEffect.World = Matrix.Identity;
            PolygonEffect.View = Matrix.Identity;

            MapEffect.Parameters["World"].SetValue(Matrix.Transpose(PolygonEffect.World));
            TestUnit.UnitEffect3D.Parameters["World"].SetValue(Matrix.Transpose(PolygonEffect.World));

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


            DicDrawablePointPerColor = new Dictionary<Vector4, List<Tile3D>>();
            DicTile3DByTileset = new Dictionary<int, Tile3DHolder>();
            DicTile3DByLayerByTileset = new Dictionary<int, Dictionary<int, Tile3DHolder>>();
            ListDrawableArrowPerColor = new List<Tile3D>();
            DicDamageNumberByPosition = new Dictionary<string, Vector3>();

            for (int L = 0; L < LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, LayerManager.ListLayer[L], L);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
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

            float Z = LayerManager.ListLayer[0].ArrayTerrain[0, 0].Position.Z;
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

        protected void CreateMap(DeathmatchMap Map, MapLayer Owner, int LayerIndex)
        {
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

                    float Z = Owner.ArrayTerrain[X, Y].Position.Z * 32;
                    float ZFront = Z;
                    float ZBack = Z;
                    float ZRight = Z;
                    float ZLeft = Z;
                    if (Y + 1 < Map.MapSize.Y)
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].Position.Z * 32;
                    }
                    if (Y - 1 >= 0)
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].Position.Z * 32;
                    }
                    if (X - 1 >= 0)
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].Position.Z * 32;
                    }
                    if (X + 1 < Map.MapSize.X)
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].Position.Z * 32;
                    }

                    List<Tile3D> ListNew3DTile = ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                                            X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Z - Owner.ArrayTerrain[X, Y].Height * 32,
                                            Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0);

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

            for (int L = 0; L < Owner.ListSubLayer.Count; L++)
            {
                CreateMap(Map, Owner.ListSubLayer[L], LayerIndex);
            }
        }

        private void Create3DCursor()
        {
            ListEditorCursorFace.Clear();

            int X = (int)Map.CursorPositionVisible.X;
            int Y = (int)Map.CursorPositionVisible.Y;
            float ZTop = (Map.CursorPosition.Z + 1) * 32 + 0.3f;
            float ZBottom = Map.CursorPosition.Z * 32 + 0.3f;
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
                float Z = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].ArrayTerrain[X, Y].Position.Z * 32 + 0.3f;
                Map2D GroundLayer = Map.LayerManager.ListLayer[(int)Map.CursorPosition.Z].LayerGrid;
                DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                    X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Map.CursorPosition.Z * 32 + 0.3f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];
            }

            if (!Map.IsAPlatform)
            {
                SetTarget(new Vector3(Map.TileSize.X * Map.CursorPositionVisible.X, Map.CursorPosition.Z * 32, Map.TileSize.Y * Map.CursorPositionVisible.Y));
                Camera.Update(gameTime);
            }

            DicDrawablePointPerColor.Clear();
            ListDrawableArrowPerColor.Clear();
            DicDamageNumberByPosition.Clear();
            if (Map.IsEditor)
            {
                Create3DCursor();
            }
        }

        public void SetTarget(Vector3 Target)
        {
            Camera.CameraPosition3D = Vector3.Transform(new Vector3(0, 0, CameraDistance), Matrix.CreateRotationY(0.2f)) + Target;
            Camera.CameraPosition3D = Vector3.Transform(Camera.CameraPosition3D, Matrix.CreateTranslation(0f, CameraHeight, 0f));
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
                ActiveTileSet.Value.SetWorld(NewWorld);
            }
        }

        private bool IsCursorHiddenByWall()
        {
            int CursorX = (int)Map.CursorPosition.X;
            int CursorLayer = (int)Map.CursorPosition.Z;
            for (int Y = (int)Map.CursorPosition.Y; Y < Map.MapSize.Y; ++Y)
            {
                if (Map.LayerManager.ListLayer[CursorLayer].ArrayTerrain[CursorX, Y].TerrainTypeIndex == UnitStats.TerrainWallIndex)
                {
                    return true;
                }
            }

            return false;
        }

        public Point GetMenuPosition()
        {
            Point BaseMenuPosition;

            Vector3 Visible3DPosition = new Vector3(Map.CursorPosition.X * Map.TileSize.X, Map.CursorPosition.Z * 32, Map.CursorPosition.Y * Map.TileSize.Y);

            Vector3 Position2D = GameScreen.GraphicsDevice.Viewport.Project(Visible3DPosition, PolygonEffect.Projection, PolygonEffect.View, Matrix.Identity);

            BaseMenuPosition.X = (int)Position2D.X + Map.TileSize.X;
            BaseMenuPosition.Y = (int)Position2D.Y;

            return BaseMenuPosition;
        }

        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (MovementAlgorithmTile ActivePoint in ListPoint)
            {
                int X = (int)ActivePoint.Position.X;
                int Y = (int)ActivePoint.Position.Y;
                float Z = ActivePoint.Position.Z * 32 + 0.1f;
                DrawableTile ActiveTerrain = ActivePoint.DrawableTile;
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawablePoint3D.Add(ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * ActivePoint.Owner.TileSize.X, Y * ActivePoint.Owner.TileSize.Y, Z, Z + 0.1f, ActivePoint.Owner.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0]);
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

                int X = (int)ActivePoint.Position.X;
                int Y = (int)ActivePoint.Position.Y;
                float Z = ActivePoint.Position.Z * 32 + 0.1f;
                DrawableTile ActiveTerrain = ActivePoint.DrawableTile;
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawableArrowPerColor.Add(ActiveTerrain3D.CreateTile3D(0, GetCursorTextureOffset(Previous, ActivePoint, Next),
                X * ActivePoint.Owner.TileSize.X, Y * ActivePoint.Owner.TileSize.Y, Z, Z + 0.15f, ActivePoint.Owner.TileSize, new List<Texture2D>() { Map.sprCursorPath }, Z, Z, Z, Z, 0)[0]);
            }
        }

        public void AddDamageNumber(string Damage, Vector3 Position)
        {
            DicDamageNumberByPosition.Add(Damage, Position);
        }

        private Point GetCursorTextureOffset(MovementAlgorithmTile Previous, MovementAlgorithmTile Current, MovementAlgorithmTile Next)
        {
            Point TextureOffset = Point.Zero;
            if (Next == null)
            {
                if (Previous != null && Previous.Position.X < Current.Position.X)//Right
                {
                    TextureOffset.X = 32 * 3;
                }
                else if (Previous != null && Previous.Position.X > Current.Position.X)//Left
                {
                    TextureOffset.X = 32 * 4;
                }
                else if (Previous != null && Previous.Position.Y < Current.Position.Y)//Down
                {
                    TextureOffset.X = 32 * 5;
                }
                else if (Previous != null && Previous.Position.Y > Current.Position.Y)//Up
                {
                    TextureOffset.X = 32 * 2;
                }
            }
            else if (Current.Position.X < Next.Position.X)//Going Right
            {
                if (Previous != null && Previous.Position.Y < Current.Position.Y)//Down Right
                {
                    TextureOffset.X = 32 * 9;
                }
                else if (Previous != null && Previous.Position.Y > Current.Position.Y)//Up Right
                {
                    TextureOffset.X = 32 * 8;
                }
                else
                {
                    TextureOffset.X = 32 * 1;
                }
            }
            else if (Current.Position.X > Next.Position.X)//Going Left
            {
                if (Previous != null && Previous.Position.Y < Current.Position.Y)//Down Left
                {
                    TextureOffset.X = 32 * 7;
                }
                else if (Previous != null && Previous.Position.Y > Current.Position.Y)//Up Left
                {
                    TextureOffset.X = 32 * 6;
                }
                else
                {
                    TextureOffset.X = 32 * 1;
                }
            }
            else if (Current.Position.Y < Next.Position.Y)//Going Down
            {
                if (Previous != null && Previous.Position.X < Current.Position.X)//Right Down
                {
                    TextureOffset.X = 32 * 6;
                }
                else if (Previous != null && Previous.Position.X > Current.Position.X)//Left Down
                {
                    TextureOffset.X = 32 * 8;
                }
                else
                {
                    TextureOffset.X = 32 * 0;
                }
            }
            else if (Current.Position.Y > Next.Position.Y)//Going Up
            {
                if (Previous != null && Previous.Position.X < Current.Position.X)//Right Up
                {
                    TextureOffset.X = 32 * 7;
                }
                else if (Previous != null && Previous.Position.X > Current.Position.X)//Left Up
                {
                    TextureOffset.X = 32 * 9;
                }
                else
                {
                    TextureOffset.X = 32 * 0;
                }
            }

            return TextureOffset;
        }

        public void Reset()
        {
            DicTile3DByTileset.Clear();
            DicTile3DByLayerByTileset.Clear();

            for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, Map.LayerManager.ListLayer[L], L);
            }

            foreach (KeyValuePair<int, Dictionary<int, Tile3DHolder>> ActiveLayer in DicTile3DByLayerByTileset)
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in ActiveLayer.Value)
                {
                    ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
                }
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                ActiveTileSet.Value.Finish(GameScreen.GraphicsDevice);
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
            PolygonEffect.View = Camera.View;
            Matrix ViewProjection = Camera.View * PolygonEffect.Projection;
            Matrix WorldViewProjection = PolygonEffect.World * ViewProjection;
            ColorEffect.Parameters["ViewProjection"].SetValue(ViewProjection);
            TestUnit.UnitEffect3D.Parameters["World"].SetValue(PolygonEffect.World);

            if (Map.ShowLayerIndex == -1)
            {
                bool DrawUpperLayers = !IsCursorHiddenByWall();

                if (DrawUpperLayers || Map.IsEditor)
                {
                    foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
                    {
                        ActiveTileSet.Value.SetViewMatrix(WorldViewProjection, Camera.CameraPosition3D);

                        ActiveTileSet.Value.Draw(g.GraphicsDevice);
                    }

                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                    {
                        Draw(g, Map.LayerManager.ListLayer[L], false);
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
                            ActiveTileSet.Value.SetViewMatrix(ViewProjection, Camera.CameraPosition3D);

                            ActiveTileSet.Value.Draw(g.GraphicsDevice);
                        }

                        Draw(g, Map.LayerManager.ListLayer[L], false);
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByLayerByTileset[Map.ShowLayerIndex])
                {
                    ActiveTileSet.Value.SetViewMatrix(ViewProjection, Camera.CameraPosition3D);

                    ActiveTileSet.Value.Draw(g.GraphicsDevice);
                }

                Draw(g, Map.LayerManager.ListLayer[Map.ShowLayerIndex], false);
            }

            DrawDrawablePoints(g);

            if (Map.ActivePlatform == null && (!Map.IsAPlatform || Map.IsPlatformActive))
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

            if (Map.IsAPlatform)
            {
                TestUnit.SetViewMatrix(Camera.View);
                TestUnit.SetPosition(15 , 32 * 7, 6);
                TestUnit.Draw(GameScreen.GraphicsDevice);
            }

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                foreach (Squad ActiveSquad in Map.ListPlayer[P].ListSquad)
                {
                    //If it's dead, don't draw it unless it's an event unit.
                    if ((ActiveSquad.CurrentLeader == null && !ActiveSquad.IsEventSquad)
                        || ActiveSquad.IsDead)
                        continue;

                    ActiveSquad.Unit3D.SetViewMatrix(Camera.View);

                    if (Map.MovementAnimation.Contains(ActiveSquad))
                    {
                        Vector3 CurrentPosition = Map.MovementAnimation.GetPosition(ActiveSquad);

                        float TerrainZ = 0;
                        if (ActiveSquad.Speed == Vector3.Zero)
                        {
                            TerrainZ = Map.LayerManager.ListLayer[(int)CurrentPosition.Z].ArrayTerrain[(int)CurrentPosition.X, (int)CurrentPosition.Y].Position.Z;
                        }

                        if (ActiveSquad.ItemHeld != null)
                        {
                            ActiveSquad.ItemHeld.Item3D.SetViewMatrix(Camera.View);

                            ActiveSquad.ItemHeld.Item3D.SetPosition(
                                CurrentPosition.X + 0.5f,
                                TerrainZ + 0.5f,
                                CurrentPosition.Y);

                            ActiveSquad.ItemHeld.Item3D.Draw(GameScreen.GraphicsDevice);
                        }

                        ActiveSquad.Unit3D.SetPosition(
                            CurrentPosition.X + 0.5f,
                            TerrainZ,
                            CurrentPosition.Y + 0.5f);

                        ActiveSquad.Unit3D.UnitEffect3D.Parameters["Greyscale"].SetValue(true);

                        ActiveSquad.Unit3D.Draw(GameScreen.GraphicsDevice);
                    }
                    else
                    {
                        Color UnitColor;
                        if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                            UnitColor = Map.ListPlayer[P].Color;
                        else
                            UnitColor = Color.White;

                        float TerrainZ = 0;
                        if (ActiveSquad.Speed == Vector3.Zero)
                        {
                            TerrainZ = Map.LayerManager.ListLayer[(int)ActiveSquad.Z].ArrayTerrain[(int)ActiveSquad.Position.X, (int)ActiveSquad.Position.Y].Position.Z;
                        }

                        if (ActiveSquad.ItemHeld != null)
                        {
                            ActiveSquad.ItemHeld.Item3D.SetViewMatrix(Camera.View);

                            ActiveSquad.ItemHeld.Item3D.SetPosition(
                                ActiveSquad.Position.X + 0.5f,
                                TerrainZ * 32 + 0.5f,
                                ActiveSquad.Position.Y);

                            ActiveSquad.ItemHeld.Item3D.Draw(GameScreen.GraphicsDevice);
                        }

                        ActiveSquad.Unit3D.SetPosition(
                            ActiveSquad.Position.X + 0.5f,
                            TerrainZ,
                            ActiveSquad.Position.Y + 0.5f);

                        ActiveSquad.Unit3D.UnitEffect3D.Parameters["Greyscale"].SetValue(!ActiveSquad.CanMove && P == Map.ActivePlayerIndex);

                        ActiveSquad.Unit3D.Draw(GameScreen.GraphicsDevice);
                    }
                }
            }

            DrawDelayedAttacks(g);

            DrawPERAttacks(g);

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            g.End();
            g.Begin();

            DrawDamageNumbers(g);
        }

        public void Draw(CustomSpriteBatch g, MapLayer Owner, bool IsSubLayer)
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
                Owner.ListProp[P].Unit3D.SetViewMatrix(Camera.View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListProp[P].Position.X, (int)Owner.ListProp[P].Position.Y].Position.Z;

                Owner.ListProp[P].Unit3D.SetPosition(
                    Owner.ListProp[P].Position.X + 0.5f,
                    TerrainZ,
                    Owner.ListProp[P].Position.Y + 0.5f);
            }

            for (int P = 0; P < Owner.ListProp.Count; ++P)
            {
                Owner.ListProp[P].Draw3D(GameScreen.GraphicsDevice, g);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.SetViewMatrix(Camera.View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListAttackPickup[P].Position.X, (int)Owner.ListAttackPickup[P].Position.Y].Position.Z;

                Owner.ListAttackPickup[P].Attack3D.SetPosition(
                    Owner.ListAttackPickup[P].Position.X + 0.5f,
                    TerrainZ,
                    Owner.ListAttackPickup[P].Position.Y + 0.5f);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.Draw(GameScreen.GraphicsDevice);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.SetViewMatrix(Camera.View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListHoldableItem[I].Position.X, (int)Owner.ListHoldableItem[I].Position.Y].Position.Z;

                Owner.ListHoldableItem[I].Item3D.SetPosition(
                    Owner.ListHoldableItem[I].Position.X + 0.5f,
                    TerrainZ,
                    Owner.ListHoldableItem[I].Position.Y + 0.5f);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.Draw(GameScreen.GraphicsDevice);
            }
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

        private void DrawPERAttacks(CustomSpriteBatch g)
        {
            foreach (PERAttack ActiveAttack in Map.ListPERAttack)
            {
                ActiveAttack.Map3DComponent.SetViewMatrix(Camera.View);
                ActiveAttack.Map3DComponent.Draw(g.GraphicsDevice);
            }
        }

        private void DrawDamageNumbers(CustomSpriteBatch g)
        {
            foreach (KeyValuePair<string, Vector3> ActiveAttack in DicDamageNumberByPosition)
            {
                Vector3 Visible3DPosition = new Vector3(ActiveAttack.Value.X, ActiveAttack.Value.Z * 32, ActiveAttack.Value.Y);
                Vector3 Position = new Vector3(Visible3DPosition.X * 32, Visible3DPosition.Y + 16, Visible3DPosition.Z * 32);

                Vector3 Position2D = g.GraphicsDevice.Viewport.Project(Position, PolygonEffect.Projection, PolygonEffect.View, Matrix.Identity);
                g.DrawString(Map.fntNonDemoDamage, ActiveAttack.Key, new Vector2(Position2D.X, Position2D.Y), Color.White);
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }
}
