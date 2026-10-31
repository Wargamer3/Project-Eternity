using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectEternity.Core;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Vehicle;
using ProjectEternity.Core.Graphics;
using ProjectEternity.Core.ControlHelper;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class Map3DDrawable : ILayerHolderDrawable
    {
        protected Point MapSize { get { return Map.MapSize; } }

        const float CameraHeight = 200;
        const float CameraDistance = 300;
        const float CameraYaw = 0.2f;
        const float RealDistance = 255;

        private LifeSimMap Map;

        private Effect MapEffect;
        private Effect ColorEffect;
        private BasicEffect PolygonEffect;
        private Camera3D Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private List<Tile3D> ListEditorCursorFace;
        private List<Tile3D> ListAllTile;

        private Dictionary<int, Tile3DHolder> DicTileBorderPerPlayer;
        private Dictionary<int, Tile3DHolder> DicTileShadowPerPlayer;
        private Dictionary<int, Tile3DHolder> DicTile3DByTileset;
        private Dictionary<int, Dictionary<int, Tile3DHolder>> DicTile3DByLayerByTileset;
        private Dictionary<int, Tile3DHolder> DicHiddenTile3DByTileset = new Dictionary<int, Tile3DHolder>();
        private List<DrawableTile> ListIgnoredTerrain = new List<DrawableTile>();

        private Dictionary<Vector4, List<Tile3D>> DicDrawablePointPerColor;
        private List<Tile3D> ListDrawableArrowPerColor;
        private Dictionary<string, Vector3> DicDamageNumberByPosition;

        public Map3DDrawable(LifeSimMap Map, LayerHolderLifeSim LayerManager, GraphicsDevice g)
        {
            this.Map = Map;
            sprCursor = Map.sprCursor;
            ListEditorCursorFace = new List<Tile3D>();

            if (!Map.IsServer)
            {
                MapEffect = Map.Content.Load<Effect>("Shaders/Default Shader 3D 2");
                ColorEffect = Map.Content.Load<Effect>("Shaders/Color Only");
                ColorEffect.Parameters["t0"].SetValue(GameScreen.sprPixel);

                PolygonEffect = new BasicEffect(g);

                PolygonEffect.TextureEnabled = true;
                PolygonEffect.EnableDefaultLighting();

                float AspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

                Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                        AspectRatio,
                                                                        1, 10000);
                PolygonEffect.Projection = Projection;

                PolygonEffect.World = Matrix.Identity;
                PolygonEffect.View = Matrix.Identity;

                MapEffect.Parameters["TextureAlpha"].SetValue(1);
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

                CreateMap(Map, LayerManager);
                /*
                float Z = LayerManager.ListLayer[0].ArrayTerrain[0, 0].WorldPosition.Z;
                DrawableTile ActiveTerrain = LayerManager.ListLayer[0].ArrayTile[0, 0];
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                    0, 0, 0, 0, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];*/
            }

            DicDrawablePointPerColor = new Dictionary<Vector4, List<Tile3D>>();
            ListDrawableArrowPerColor = new List<Tile3D>();
            DicDamageNumberByPosition = new Dictionary<string, Vector3>();

            if (Map.IsEditor)
            {
                Map.CursorPosition.X = Map.MapSize.X / 2 * Map.TileSize.X;
                Map.CursorPosition.Y = Map.MapSize.Y / 2 * Map.TileSize.Y;
                Map.CursorPosition.Z = (LayerManager.ListLayer.Count - 1) / 2;
            }
        }

        private void CreateMap(LifeSimMap Map, LayerHolderLifeSim LayerManager)
        {
            ListAllTile = new List<Tile3D>();
            DicTileBorderPerPlayer = new Dictionary<int, Tile3DHolder>();
            DicTileShadowPerPlayer = new Dictionary<int, Tile3DHolder>();
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

            SubmitChunkPhysics();

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

        protected void CreateMap(LifeSimMap Map, MapLayer Owner, int LayerIndex)
        {
            foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
            {
                CreateMap(Map, ActiveSubLayer, LayerIndex);
            }

            if (!DicTile3DByLayerByTileset.ContainsKey(LayerIndex))
            {
                DicTile3DByLayerByTileset.Add(LayerIndex, new Dictionary<int, Tile3DHolder>());
            }

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    DrawableTile ActiveTerrain = Owner.ArrayTile[X, Y];
                    Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                    if (ActiveTerrain3D.TerrainStyle == Terrain3D.TerrainStyles.Invisible)
                    {
                        continue;
                    }

                    float Z = Owner.ArrayTerrain[X, Y].WorldPosition.Z;
                    float MinZ = Z - Owner.ArrayTerrain[X, Y].Height * Map.LayerHeight;
                    float ZFront = MinZ;
                    float ZBack = MinZ;
                    float ZRight = MinZ;
                    float ZLeft = MinZ;
                    if (Y + 1 < Map.MapSize.Y && ConsiderTerrain(Owner.ArrayTile[X, Y + 1].Terrain3DInfo.TerrainStyle))
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].WorldPosition.Z;
                    }
                    if (Y - 1 >= 0 && ConsiderTerrain(Owner.ArrayTile[X, Y - 1].Terrain3DInfo.TerrainStyle))
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].WorldPosition.Z;
                    }
                    if (X - 1 >= 0 && ConsiderTerrain(Owner.ArrayTile[X - 1, Y].Terrain3DInfo.TerrainStyle))
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].WorldPosition.Z;
                    }
                    if (X + 1 < Map.MapSize.X && ConsiderTerrain(Owner.ArrayTile[X + 1, Y].Terrain3DInfo.TerrainStyle))
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].WorldPosition.Z;
                    }

                    List<Tile3D> ListNew3DTile = ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                                            X * Map.TileSize.X, Y * Map.TileSize.Y, Z, MinZ,
                                            Map.TileSize, Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0);

                    if (ListIgnoredTerrain.Contains(ActiveTerrain))
                    {
                        foreach (Tile3D ActiveTile in ListNew3DTile)
                        {
                            ListAllTile.Add(ActiveTile);
                            if (!DicHiddenTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                            {
                                Tile3DHolder NewTile3DHolder = new Tile3DHolder(MapEffect, Map.ListTileSet[ActiveTile.TilesetIndex]);
                                NewTile3DHolder.Effect3D.Parameters["TextureAlpha"].SetValue(0.5f);
                                DicHiddenTile3DByTileset.Add(ActiveTile.TilesetIndex, NewTile3DHolder);
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
                            ListAllTile.Add(ActiveTile);
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
            float ZTop = (Map.CursorPosition.Z + 1) * Map.LayerHeight + 0.3f;
            float ZBottom = Map.CursorPosition.Z * Map.LayerHeight + 0.3f;
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
                X, Y, ZTop, ZBottom, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, ZBottom, ZBottom, ZBottom, ZBottom, 0);
            ListEditorCursorFace.Add(Cursor.CreateTile3D(0, Point.Zero,
                X, Y, ZBottom, ZBottom, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, ZBottom, ZBottom, ZBottom, ZBottom, 0)[0]);
        }

        void EnsureCursorTexture()
        {
            if (sprCursor != null || Map.Content == null)
            {
                return;
            }

            sprCursor = Map.Content.Load<Texture2D>("Editors/Reticle");
            Map.sprCursor = sprCursor;
        }

        bool CursorHeight(float X, float Y, out float Z)
        {
            Z = 0f;
            HeightField Field = Map.Chunk == null ? null : Map.Chunk.Heights;
            if (Field == null)
            {
                return false;
            }

            float SlopeX;
            float SlopeY;
            return Field.Sample(X, Y, out Z, out SlopeX, out SlopeY);
        }

        void PlaceCursor(float X, float Y, float Z)
        {
            Terrain3D Marker = new Terrain3D();
            List<Tile3D> CursorTiles = Marker.CreateTile3D(0, Point.Zero,
                X, Y, Z, Z, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0);
            if (CursorTiles.Count > 0)
            {
                Cursor = CursorTiles[0];
            }
        }

        void RefreshCursor()
        {
            EnsureCursorTexture();
            if (Map.ActivePlatform != null || (Map.IsAPlatform && !Map.IsPlatformActive) || Map.IsServer
                || sprCursor == null || Map.TileSize.X <= 0 || Map.TileSize.Y <= 0)
            {
                return;
            }

            float MarkX = Map.CursorPositionVisible.X;
            float MarkY = Map.CursorPositionVisible.Y;
            float GroundZ;
            if (CursorHeight(MarkX + Map.TileSize.X * 0.5f, MarkY + Map.TileSize.Y * 0.5f, out GroundZ))
            {
                PlaceCursor(MarkX, MarkY, GroundZ + 0.3f);
                return;
            }

            if (Map.LayerManager == null || Map.LayerManager.ListLayer.Count == 0 || Map.MapSize.X <= 0 || Map.MapSize.Y <= 0)
            {
                return;
            }

            int Layer = (int)Map.CursorPosition.Z;
            if (Layer < 0 || Layer >= Map.LayerManager.ListLayer.Count)
            {
                Layer = 0;
            }

            int X = (int)System.Math.Floor(Map.CursorPositionVisible.X / Map.TileSize.X);
            int Y = (int)System.Math.Floor(Map.CursorPositionVisible.Y / Map.TileSize.Y);
            if (X < 0)
            {
                X = 0;
            }
            else if (X >= Map.MapSize.X)
            {
                X = Map.MapSize.X - 1;
            }

            if (Y < 0)
            {
                Y = 0;
            }
            else if (Y >= Map.MapSize.Y)
            {
                Y = Map.MapSize.Y - 1;
            }

            Terrain[,] Ground = Map.LayerManager.ListLayer[Layer].ArrayTerrain;
            if (Ground == null || X >= Ground.GetLength(0) || Y >= Ground.GetLength(1))
            {
                return;
            }

            float Z = Ground[X, Y].WorldPosition.Z + 0.3f;
            DrawableTile ActiveTerrain = Map.LayerManager.ListLayer[Layer].ArrayTile[X, Y];
            Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
            if (ActiveTerrain3D == null)
            {
                return;
            }

            List<Tile3D> CursorTiles = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                Map.CursorPositionVisible.X, Map.CursorPositionVisible.Y, Z, Map.CursorPosition.Z + 0.3f, Map.TileSize, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0);
            if (CursorTiles.Count > 0)
            {
                Cursor = CursorTiles[0];
            }
        }

        public void Update(GameTime gameTime)
        {
            RefreshCursor();

            DicDrawablePointPerColor.Clear();
            ListDrawableArrowPerColor.Clear();
            DicDamageNumberByPosition.Clear();
            if (Map.IsEditor)
            {
                Create3DCursor();
            }

            Vector3 Shift = ChunkOrigin.Shift(Map.Chunk);
            foreach (PlayerCharacter ActiveCharacter in Map.ListCharacter)
            {
                if (ActiveCharacter.BodyModel != null && !ActiveCharacter.DungeonPose)
                {
                    if (ActiveCharacter.BodySprings != null)
                    {
                        ActiveCharacter.BodySprings.World = ActiveCharacter.BodyDrawWorld(Shift);
                        ActiveCharacter.BodySprings.Seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    ActiveCharacter.BodyModel.Update(gameTime);
                    FootPlant.Apply(ActiveCharacter, Shift);
                    if (ActiveCharacter.BodySprings != null)
                    {
                        ActiveCharacter.BodySprings.Pose(ActiveCharacter.BodyModel);
                    }
                }

                if (ActiveCharacter.Unit3DModel != null)
                {
                    ActiveCharacter.Unit3DModel.Update(gameTime);
                }
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
                Matrix World;

                if (Map.Camera3DOverride != null && !Map.IsEditor)
                {
                    World = NewWorld * Map.Camera3DOverride.View;
                }
                else
                {
                    World = NewWorld * Camera.View;
                }

                worldInverse = Matrix.Invert(World);

                ActiveTileSet.Value.Effect3D.Parameters["World"].SetValue(Matrix.Transpose(NewWorld));
                ActiveTileSet.Value.Effect3D.Parameters["WorldInverseTranspose"].SetValue(worldInverse);
            }
        }

        public void SetCamera(Camera3D Camera)
        {
            this.Camera = Camera;
        }

        void BoostLights()
        {
            float Boost = WeatherDraw.Light;
            Tint(MapEffect, Boost);
            Tint(DicTile3DByTileset, Boost);
            Tint(DicHiddenTile3DByTileset, Boost);
            if (DicTile3DByLayerByTileset == null)
            {
                return;
            }

            foreach (KeyValuePair<int, Dictionary<int, Tile3DHolder>> Layer in DicTile3DByLayerByTileset)
            {
                Tint(Layer.Value, Boost);
            }
        }

        static void Tint(Dictionary<int, Tile3DHolder> Tiles, float Boost)
        {
            if (Tiles == null)
            {
                return;
            }

            foreach (KeyValuePair<int, Tile3DHolder> Tile in Tiles)
            {
                if (Tile.Value != null)
                {
                    Tint(Tile.Value.Effect3D, Boost);
                }
            }
        }

        static void Tint(Effect Effect, float Boost)
        {
            if (Effect == null)
            {
                return;
            }

            Effect.Parameters["DirLight0Direction"].SetValue(-SkyDraw.KeyDirection);
            Effect.Parameters["DirLight0DiffuseColor"].SetValue(SkyDraw.KeyColor);
            Effect.Parameters["DirLight0SpecularColor"].SetValue(SkyDraw.KeyColor);
            Effect.Parameters["DirLight1Direction"].SetValue(-SkyDraw.FillDirection);
            Effect.Parameters["DirLight1DiffuseColor"].SetValue(SkyDraw.FillColor);
            Effect.Parameters["DirLight2Direction"].SetValue(SkyDraw.FillDirection);
            Effect.Parameters["DirLight2DiffuseColor"].SetValue(SkyDraw.FillColor * 0.35f);
            Effect.Parameters["DirLight2SpecularColor"].SetValue(SkyDraw.FillColor * 0.2f);
            EffectParameter Emissive = Effect.Parameters["EmissiveColor"];
            if (Emissive != null)
            {
                Emissive.SetValue(SkyDraw.SkyAmbient);
            }
        }

        private bool IsCursorHiddenByWall()
        {
            if (!Map.IsInsideMap(Map.CursorPosition))
            {
                return false;
            }

            int CursorX = (int)Map.CursorPosition.X / Map.TileSize.X;
            int CursorLayer = (int)Map.CursorPosition.Z;
            for (int Y = (int)Map.CursorPosition.Y / Map.TileSize.Y; Y < Map.MapSize.Y; ++Y)
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

            Vector3 Visible3DPosition = new Vector3(Position.X, Position.Y, Position.Z);

            if (Map.ActivePlatform != null)
            {
                Visible3DPosition = new Vector3(Map.ActivePlatform.Map.CursorTerrain.WorldPosition.X,
                    Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Z,
                    Map.ActivePlatform.Map.CursorTerrain.WorldPosition.Y);
            }

            Vector3 Position2D = GameScreen.GraphicsDevice.Viewport.Project(Visible3DPosition, PolygonEffect.Projection, PolygonEffect.View, Matrix.Identity);

            BaseMenuPosition.X = (int)Position2D.X;
            BaseMenuPosition.Y = (int)Position2D.Y;

            return BaseMenuPosition;
        }

        public Vector3 Get3DPositionFromMouse(int MouseX, int MouseY)
        {
            Vector3 NearPoint = new Vector3(MouseX, MouseY, 0);
            Vector3 FarPoint = new Vector3(MouseX, MouseY, 1);

            NearPoint = GameScreen.GraphicsDevice.Viewport.Unproject(NearPoint, Camera.Projection, Camera.View, Matrix.Identity);
            FarPoint = GameScreen.GraphicsDevice.Viewport.Unproject(FarPoint, Camera.Projection, Camera.View, Matrix.Identity);

            Vector3 Direction = FarPoint - NearPoint;
            Direction.Normalize();

            Ray MouseRay = new Ray(NearPoint, Direction);

            Tile3D FoundTile = null;
            float NearestDistance = 0f;
            foreach (Tile3D ActiveTile in ListAllTile)
            {
                float? checkIntersectDistance = MouseRay.Intersects(ActiveTile.CollisionBox);

                if (checkIntersectDistance != null)
                {
                    if (checkIntersectDistance.Value < NearestDistance || NearestDistance == 0)
                    {
                        NearestDistance = checkIntersectDistance.Value;
                        FoundTile = ActiveTile;
                    }
                }
            }

            if (FoundTile != null)
            {
                return FoundTile.CollisionBox.Min;
            }

            return Vector3.Zero;
        }

        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (MovementAlgorithmTile ActivePoint in ListPoint)
            {
                ListDrawableArrowPerColor.Add(Map.CreateTile3D(0, ActivePoint.WorldPosition, Point.Zero, ActivePoint.Owner.TileSize, ActivePoint.Owner.TileSize, 0));
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

                ListDrawableArrowPerColor.Add(Map.CreateTile3D(0, ActivePoint.WorldPosition, MovementAlgorithm.GetMovementArrowTextureOffset(Previous, ActivePoint, Next), ActivePoint.Owner.TileSize, ActivePoint.Owner.TileSize, 0));
            }
        }

        public void AddDamageNumber(string Damage, Vector3 Position)
        {
            DicDamageNumberByPosition.Add(Damage, Position);
        }

        public void Reset()
        {
            CreateMap(Map, Map.LayerManager);
        }

        public void CursorMoved()
        {
            FilterTerrainObscuringUnits();
        }

        public void UnitMoved(int PlayerIndex)
        {
        }

        public void UnitKilled(int PlayerIndex)
        {
        }

        private void FilterTerrainObscuringUnits()
        {
            ListIgnoredTerrain.Clear();

            foreach (PlayerCharacter ActivePlayer in Map.ListCharacter)
            {
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
                int PosX = (int)ActivePosition.X;
                int PosY = (int)ActivePosition.Y;
                int ActiveY = PosY;

                MapLayer ActiveLayer = Map.LayerManager.ListLayer[Z];
                Terrain ActiveTerrain = ActiveLayer.ArrayTerrain[PosX, PosY];
                DrawableTile ActiveTile = ActiveLayer.ArrayTile[PosX, PosY];

                while (ActiveY >= 0 && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                {
                    if (--ActiveY >= 0)
                    {
                        ActiveTerrain = Map.GetTerrain(new Vector3(PosX, ActiveY, Z));
                    }
                }

                ActiveY++;
                bool spanLeft = false;
                bool spanRight = false;

                while (ActiveY < Map.MapSize.Y && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex)
                {
                    ActiveTile = ActiveLayer.ArrayTile[PosX, ActiveY];
                    ListIgnoredTerrain.Add(ActiveTile);

                    if (PosX - 1 >= 0)
                    {
                        ActiveTerrain = ActiveLayer.ArrayTerrain[PosX - 1, PosY];
                        ActiveTile = ActiveLayer.ArrayTile[PosX - 1, PosY];

                        if (!spanLeft && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            ListTerrainPositionToHide.Push(new Vector3(PosX - 1, ActiveY, Z));
                            spanLeft = true;
                        }
                        else if (spanLeft && PosX - 1 == 0 && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            spanLeft = false;

                        }
                    }

                    if (PosX + 1 < Map.MapSize.X)
                    {
                        ActiveTerrain = ActiveLayer.ArrayTerrain[PosX + 1, PosY];
                        ActiveTile = ActiveLayer.ArrayTile[PosX + 1, PosY];

                        if (!spanRight && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            ListTerrainPositionToHide.Push(new Vector3(PosX + 1, ActiveY, Z));
                            spanRight = true;
                        }
                        else if (spanRight && ActiveTerrain.TerrainTypeIndex == TargetTerrainIndex && !ListIgnoredTerrain.Contains(ActiveTile))
                        {
                            spanRight = false;
                        }
                    }

                    ActiveY++;
                }
            }
        }

        public void SetChunkShift(Vector3 RenderOffset)
        {
            if (PolygonEffect != null)
            {
                PolygonEffect.World = Matrix.CreateTranslation(RenderOffset);
            }
        }

        public void SubmitChunkPhysics()
        {
            if (LifeSimMap.PhysicsManager == null)
            {
                return;
            }

            Vector3 Shift = ChunkOrigin.Shift(Map.Chunk);
            if (Map.Chunk == null)
            {
                LifeSimMap.PhysicsManager.ClearMapMeshes();
            }
            else
            {
                LifeSimMap.PhysicsManager.ClearChunkMeshes(Map.Chunk);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
            {
                if (Map.Chunk == null)
                {
                    LifeSimMap.PhysicsManager.AddMapMesh(ActiveTileSet.Value.ArrayTriangle, ActiveTileSet.Value.ArrayIndices);
                }
                else
                {
                    LifeSimMap.PhysicsManager.AddChunkMesh(Map.Chunk, ActiveTileSet.Value.ArrayTriangle, ActiveTileSet.Value.ArrayIndices, Shift);
                }
            }

            if (Map.Chunk != null && Map.Chunk.Heights != null && Map.Chunk.Heights.PhysicsIndices != null && Map.Chunk.Heights.PhysicsIndices.Length >= 3)
            {
                LifeSimMap.PhysicsManager.AddChunkMesh(Map.Chunk, Map.Chunk.Heights.PhysicsVertices, Map.Chunk.Heights.PhysicsIndices, Shift);
            }

            if (Map.Chunk != null && Map.Chunk.MeshIndices != null && Map.Chunk.MeshIndices.Length >= 3)
            {
                LifeSimMap.PhysicsManager.AddChunkMesh(Map.Chunk, Map.Chunk.MeshVertices, Map.Chunk.MeshIndices, Shift);
            }
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void DrawCursor(CustomSpriteBatch g)
        {
            EnsureCursorTexture();
            if (Camera == null || PolygonEffect == null || sprCursor == null || Map.IsEditor)
            {
                return;
            }

            if ((Map.ActivePlatform != null || Map.IsAPlatform) && !Map.IsPlatformActive)
            {
                return;
            }

            RefreshCursor();
            if (Cursor == null)
            {
                return;
            }

            GraphicsDevice Device = g.GraphicsDevice;
            BlendState PreviousBlend = Device.BlendState;
            DepthStencilState PreviousDepth = Device.DepthStencilState;
            RasterizerState PreviousRaster = Device.RasterizerState;
            Viewport Pane = Device.Viewport;
            if (Pane.Height > 0)
            {
                PolygonEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Pane.Width / (float)Pane.Height, 1f, 10000f);
            }

            Device.BlendState = BlendState.AlphaBlend;
            Device.DepthStencilState = DepthStencilState.DepthRead;
            Device.RasterizerState = RasterizerState.CullCounterClockwise;
            Matrix View = Camera.View;
            if (Map.Camera3DOverride != null)
            {
                View = Map.Camera3DOverride.View;
            }

            PolygonEffect.World = Matrix.CreateTranslation(ChunkOrigin.Shift(Map.Chunk));
            PolygonEffect.View = View;
            PolygonEffect.Texture = sprCursor;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();
            Cursor.Draw(Device);
            Device.BlendState = PreviousBlend;
            Device.DepthStencilState = PreviousDepth;
            Device.RasterizerState = PreviousRaster;
        }

        public void DrawWater(CustomSpriteBatch g)
        {
            if (g == null || Camera == null || Map == null)
            {
                return;
            }

            Viewport Pane = g.GraphicsDevice.Viewport;
            if (Pane.Height <= 0)
            {
                return;
            }

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, Pane.Width / (float)Pane.Height, 1f, 10000f);
            Matrix View = Camera.View;
            Vector3 CameraPosition = Camera.CameraPosition3D;
            if (Map.Camera3DOverride != null && !Map.IsEditor)
            {
                View = Map.Camera3DOverride.View;
                CameraPosition = Map.Camera3DOverride.CameraPosition3D;
            }

            IList<WaterVolume> Volumes = Map.Chunk != null && Map.Chunk.Water != null ? Map.Chunk.Water : Map.Water;
            WaterTracks Tracks = Map.Chunk == null ? null : Map.Chunk.Tracks;
            HeightField Ground = Map.Chunk == null ? null : Map.Chunk.Heights;
            WaterDraw.All(g.GraphicsDevice, Map.Content, Volumes, null, Tracks, View, Projection, ChunkOrigin.Shift(Map.Chunk), CameraPosition, Ground, Map.Chunk);
        }

        public void Draw(CustomSpriteBatch g)
        {
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            Viewport Pane = g.GraphicsDevice.Viewport;
            if (PolygonEffect != null && Pane.Height > 0)
            {
                float AspectRatio = Pane.Width / (float)Pane.Height;
                PolygonEffect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, AspectRatio, 1f, 10000f);
            }
            if (Camera == null || PolygonEffect == null)
            {
                return;
            }

            Matrix View = Camera.View;
            Matrix World = PolygonEffect.World;
            Vector3 CameraPosition = Camera.CameraPosition3D;

            if (Map.Camera3DOverride != null && !Map.IsEditor)
            {
                View = Map.Camera3DOverride.View;
                CameraPosition = Map.Camera3DOverride.CameraPosition3D;
            }
            PolygonEffect.View = View;
            BoostLights();
            Matrix ViewProjection = View * PolygonEffect.Projection;
            Matrix WorldViewProjection = World * ViewProjection;
            ColorEffect.Parameters["ViewProjection"].SetValue(ViewProjection);

            bool DrawUpperLayers = !IsCursorHiddenByWall();

            HeightDraw.Draw(g.GraphicsDevice, Map.Chunk, View, PolygonEffect.Projection);
            ForestScatter.Draw(g.GraphicsDevice, Map.Chunk, View, PolygonEffect.Projection);
            GrassDraw.All(g.GraphicsDevice, Map.Chunk, View, PolygonEffect.Projection, CameraPosition);
            LandmarkDraw.Draw(g.GraphicsDevice, Map.Chunk, View, PolygonEffect.Projection);
            MeshChunk.Draw(g.GraphicsDevice, Map.Chunk, View, PolygonEffect.Projection);
            DrawMap(g, View, WorldViewProjection, DrawUpperLayers);

            DrawVehicles(g, View);

            DrawDrawablePoints(g);

            if (Map.IsEditor && Map.ShowCursor && ((Map.ActivePlatform == null && !Map.IsAPlatform) || Map.IsPlatformActive))
            {
                RefreshCursor();
                PolygonEffect.World = World;
                PolygonEffect.View = View;
                PolygonEffect.Texture = sprCursor;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();
                g.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                foreach (Tile3D CursorFace in ListEditorCursorFace)
                {
                    CursorFace.Draw(g.GraphicsDevice);
                }

                g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            bool Watching = Camera != null && Vector3.DistanceSquared(Camera.LookAtTarget, Camera.CameraPosition3D) > 4f;
            PlacedBoxDraw.All(g.GraphicsDevice, Map.Furniture, Map.Buildings, View, PolygonEffect.Projection, Map.ShownFloor, Map.HideFloorsAbove, ChunkOrigin.Shift(Map.Chunk), Map.Chunk == null ? null : Map.Chunk.Heights, Watching ? Camera.LookAtTarget : Vector3.Zero, Watching);
            IList<WaterVolume> Volumes = Map.Chunk != null && Map.Chunk.Water != null ? Map.Chunk.Water : Map.Water;
            WaterTracks Tracks = Map.Chunk == null ? null : Map.Chunk.Tracks;
            HeightField Ground = Map.Chunk == null ? null : Map.Chunk.Heights;
            WaterDraw.All(g.GraphicsDevice, Map.Content, Volumes, WaterSession.Preview, Tracks, View, PolygonEffect.Projection, ChunkOrigin.Shift(Map.Chunk), CameraPosition, Ground, Map.Chunk);
            DrawPlayers(g, View, DrawUpperLayers);

            DrawDelayedAttacks(g, ViewProjection);

            DrawPERAttacks(g, View);

            foreach (Tile3DHolder ActivePlayerBorder in DicTileBorderPerPlayer.Values)
            {
                ActivePlayerBorder.Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
                ActivePlayerBorder.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);
                ActivePlayerBorder.Draw(g.GraphicsDevice);
            }

            foreach (Tile3DHolder ActivePlayerBorder in DicTileShadowPerPlayer.Values)
            {
                ActivePlayerBorder.Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
                ActivePlayerBorder.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);
                ActivePlayerBorder.Draw(g.GraphicsDevice);
            }

            foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicHiddenTile3DByTileset)
            {
                ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(ViewProjection);
                ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

                ActiveTileSet.Value.Draw(g.GraphicsDevice);
            }

            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            g.End();
            g.Begin();

            DrawDamageNumbers(g, View);
        }

        private void DrawMap(CustomSpriteBatch g, Matrix View, Matrix WorldViewProjection, bool DrawUpperLayers)
        {
            Vector3 CameraPosition = Camera.CameraPosition3D;
            if (Map.Camera3DOverride != null && !Map.IsEditor)
            {
                CameraPosition = Map.Camera3DOverride.CameraPosition3D;
            }

            if (Map.ShowLayerIndex == -1)
            {
                if (DrawUpperLayers || Map.IsEditor)
                {
                    foreach (KeyValuePair<int, Tile3DHolder> ActiveTileSet in DicTile3DByTileset)
                    {
                        ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(WorldViewProjection);
                        ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

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
                            ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(WorldViewProjection);
                            ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

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
                    ActiveTileSet.Value.Effect3D.Parameters["WorldViewProj"].SetValue(WorldViewProjection);
                    ActiveTileSet.Value.Effect3D.Parameters["CameraPosition"].SetValue(CameraPosition);

                    ActiveTileSet.Value.Draw(g.GraphicsDevice);
                }

                DrawItems(g, View, Map.LayerManager.ListLayer[Map.ShowLayerIndex], false);
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
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListProp[P].Position.X, (int)Owner.ListProp[P].Position.Y].WorldPosition.Z;

                if (Owner.ListProp[P].Unit3D != null)
                {
                    Owner.ListProp[P].Unit3D.SetPosition(
                        (Owner.ListProp[P].Position.X + 0.5f) * Map.TileSize.X,
                        (TerrainZ + 0.5f) * Map.LayerHeight,
                        (Owner.ListProp[P].Position.Y + 0.5f) * Map.TileSize.Y);
                }
                Owner.ListProp[P].Draw3D(GameScreen.GraphicsDevice, View, g);
            }

            for (int P = 0; P < Owner.ListAttackPickup.Count; ++P)
            {
                Owner.ListAttackPickup[P].Attack3D.SetViewMatrix(View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListAttackPickup[P].Position.X, (int)Owner.ListAttackPickup[P].Position.Y].WorldPosition.Z;

                Owner.ListAttackPickup[P].Attack3D.SetPosition(
                    (Owner.ListAttackPickup[P].Position.X + 0.5f) * Map.TileSize.X,
                    TerrainZ * Map.LayerHeight,
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
                    TerrainZ * Map.LayerHeight,
                    (Owner.ListHoldableItem[I].Position.Y + 0.5f) * Map.TileSize.Y);
            }

            for (int I = 0; I < Owner.ListHoldableItem.Count; ++I)
            {
                Owner.ListHoldableItem[I].Item3D.Draw(GameScreen.GraphicsDevice);
            }
        }

        private void DrawPlayers(CustomSpriteBatch g, Matrix View, bool DrawUpperLayers)
        {
            for (int P = 0; P < Map.ListCharacter.Count; P++)
            {
                PlayerCharacter ActiveCharacter = Map.ListCharacter[P];
                if (ActiveCharacter.HiddenFromBattle)
                {
                    continue;
                }

                if (Map.HideFloorsAbove && BuildFloor.Index(ActiveCharacter.Position.Z) > Map.ShownFloor)
                {
                    continue;
                }

                Color UnitColor;
                if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                    UnitColor = Map.ListCharacter[P].Color;
                else
                    UnitColor = Color.White;

                if (ActiveCharacter.BodyModel == null && ActiveCharacter.Unit3DSprite == null && ActiveCharacter.Unit3DModel == null)
                {
                    continue;
                }

                Vector3 Shift = ChunkOrigin.Shift(Map.Chunk);
                if (ActiveCharacter.Unit3DSprite != null)
                {
                    ActiveCharacter.Unit3DSprite.SetViewMatrix(View);

                    ActiveCharacter.Unit3DSprite.SetPosition(
                        ActiveCharacter.Position.X + Shift.X,
                        ActiveCharacter.Position.Z + 16f + Shift.Y,
                        ActiveCharacter.Position.Y + Shift.Z);

                    ActiveCharacter.Unit3DSprite.UnitEffect3D.Parameters["World"].SetValue(Matrix.Identity);
                    ActiveCharacter.Unit3DSprite.UnitEffect3D.Parameters["Greyscale"].SetValue(false);
                    ActiveCharacter.Unit3DSprite.UnitEffect3D.Parameters["Scale"].SetValue(0.35f);
                }
                else if (ActiveCharacter.Unit3DModel != null)
                {
                    ActiveCharacter.Unit3DModel.PlayAnimation("Idle");
                }

                ActiveCharacter.Draw3DOnMap(GameScreen.GraphicsDevice, View, PolygonEffect.Projection, Shift);
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

        private void DrawDelayedAttacks(CustomSpriteBatch g, Matrix ViewProjection)
        {
        }

        private void DrawPERAttacks(CustomSpriteBatch g, Matrix View)
        {
        }

        private void DrawDamageNumbers(CustomSpriteBatch g, Matrix View)
        {
            foreach (KeyValuePair<string, Vector3> ActiveAttack in DicDamageNumberByPosition)
            {
                Vector3 Visible3DPosition = new Vector3(ActiveAttack.Value.X, ActiveAttack.Value.Z * Map.LayerHeight, ActiveAttack.Value.Y);
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

                ActiveVehicle.DrawVehicle(g.GraphicsDevice, View, PolygonEffect.Projection);
            }

            PolygonEffect.World = Map.World;
        }

        public void EndDraw(CustomSpriteBatch g)
        {
        }
    }
}
