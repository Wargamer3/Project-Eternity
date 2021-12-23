using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class Map3D : DrawableGrid
    {
        protected Point MapSize { get { return Map.MapSize; } }

        public List<Tile3D> ListTile3D;
        protected BasicEffect PolygonEffect;
        protected float Radius;
        private DeathmatchMap Map;
        protected int LayerIndex;
        protected MapLayer Owner;
        public DefaultCamera Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private Dictionary<Color, List<Tile3D>> DicDrawablePointPerColor;
        private Random Random;

        public Map3D(DeathmatchMap Map, int LayerIndex, MapLayer Owner, GraphicsDevice g)
        {
            this.Map = Map;
            this.LayerIndex = LayerIndex;
            this.Owner = Owner;
            Random = new Random();
            sprCursor = Map.sprCursor;
            Camera = new DefaultCamera(g);
            Radius = (Map.MapSize.X * Map.TileSize.X) / 2;

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

            DicDrawablePointPerColor = new Dictionary<Color, List<Tile3D>>();
            ListTile3D = new List<Tile3D>();

            CreateMap(Map);

            Cursor = CreateCursor(Map, Map.CursorPositionVisible.X, Map.CursorPositionVisible.Y, LayerIndex, sprCursor.Width, sprCursor.Height, Radius);
        }

        public void Save(BinaryWriter BW)
        {
            for (int X = MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = MapSize.Y - 1; Y >= 0; --Y)
                {
                    Owner.OriginalLayerGrid.GetTile(X, Y).Save(BW);
                }
            }
        }

        public void Load(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        protected virtual void CreateMap(DeathmatchMap Map)
        {
            ListTile3D.Clear();
            Map2D GroundLayer = Owner.OriginalLayerGrid;

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                    Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
                    float Z = Owner.ArrayTerrain[X, Y].Position.Z * 32 + (LayerIndex * 64);
                    float ZFront = Z;
                    float ZBack = Z;
                    float ZRight = Z;
                    float ZLeft = Z;
                    if (Y + 1 < Map.MapSize.Y)
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].Position.Z * 32 + (LayerIndex * 64);
                    }
                    if (Y - 1 >= 0)
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].Position.Z * 32 + (LayerIndex * 64);
                    }
                    if (X - 1 >= 0)
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].Position.Z * 32 + (LayerIndex * 64);
                    }
                    if (X + 1 < Map.MapSize.X)
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].Position.Z * 32 + (LayerIndex * 64);
                    }

                    ListTile3D.AddRange(ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                        X * Map.TileSize.X, Y * Map.TileSize.Y, Z, (LayerIndex * 64), Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0));
                }
            }
        }
        
        private static Tile3D CreateCursor(DeathmatchMap Map, float X, float Y, int LayerIndex, int TextureWidth, int TextureHeight, float Radius)
        {
            float Z = Map.GetTerrain(Math.Max(0, X), Math.Max(0, Y), LayerIndex).Position.Z;

            Vector3[] ArrayVertexPosition = new Vector3[4];
            ArrayVertexPosition[0] = new Vector3(X * Map.TileSize.X, Z, Y * Map.TileSize.Y);
            ArrayVertexPosition[1] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y);
            ArrayVertexPosition[2] = new Vector3(X * Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);
            ArrayVertexPosition[3] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);

            return CreateTile3D(Map, ArrayVertexPosition, 0, 0, TextureWidth, TextureHeight, Radius);
        }

        private static Tile3D CreateTile3D(DeathmatchMap Map, Vector3[] ArrayVertexPosition, float OffsetX, float OffsetY, int TextureWidth, int TextureHeight, float Radius)
        {
            VertexPositionNormalTexture[] ArrayVertex = new VertexPositionNormalTexture[4];
            float UVXValue = OffsetX + 0.5f;
            float UVYValue = OffsetY + 0.5f;

            Vector3 NormalTriangle = Vector3.Normalize(Vector3.Cross(ArrayVertexPosition[1] - ArrayVertexPosition[0], ArrayVertexPosition[2] - ArrayVertexPosition[0]));

            ArrayVertex[0] = new VertexPositionNormalTexture();
            ArrayVertex[0].Position = new Vector3(ArrayVertexPosition[0].X - Map.TileSize.X / 2f, ArrayVertexPosition[0].Y, ArrayVertexPosition[0].Z - Map.TileSize.Y / 2f);
            ArrayVertex[0].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[0].Normal = NormalTriangle;

            UVXValue = OffsetX + Map.TileSize.X - 0.5f;
            UVYValue = OffsetY + 0.5f;
            ArrayVertex[1] = new VertexPositionNormalTexture();
            ArrayVertex[1].Position = new Vector3(ArrayVertexPosition[1].X - Map.TileSize.X / 2f, ArrayVertexPosition[1].Y, ArrayVertexPosition[1].Z - Map.TileSize.Y / 2f);
            ArrayVertex[1].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[1].Normal = NormalTriangle;

            UVXValue = OffsetX + 0.5f;
            UVYValue = OffsetY + Map.TileSize.Y - 0.5f;
            ArrayVertex[2] = new VertexPositionNormalTexture();
            ArrayVertex[2].Position = new Vector3(ArrayVertexPosition[2].X - Map.TileSize.X / 2f, ArrayVertexPosition[2].Y, ArrayVertexPosition[2].Z - Map.TileSize.Y / 2f);
            ArrayVertex[2].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[2].Normal = NormalTriangle;

            UVXValue = OffsetX + Map.TileSize.X - 0.5f;
            UVYValue = OffsetY + Map.TileSize.Y - 0.5f;
            ArrayVertex[3] = new VertexPositionNormalTexture();
            ArrayVertex[3].Position = new Vector3(ArrayVertexPosition[3].X - Map.TileSize.X / 2f, ArrayVertexPosition[3].Y, ArrayVertexPosition[3].Z - Map.TileSize.Y / 2f);
            ArrayVertex[3].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[3].Normal = NormalTriangle;

            short[] ArrayIndex = new short[6];
            ArrayIndex[0] = 0;
            ArrayIndex[1] = 1;
            ArrayIndex[2] = 3;
            ArrayIndex[3] = 0;
            ArrayIndex[4] = 3;
            ArrayIndex[5] = 2;

            Vector3[] ArrayTransformedVertexPosition = new Vector3[ArrayVertexPosition.Length];

            Matrix TranslationToOriginMatrix = Matrix.CreateTranslation(-Radius, Radius, -Radius);
            Vector3.Transform(ArrayVertexPosition, ref TranslationToOriginMatrix, ArrayTransformedVertexPosition);
            for (int V = ArrayVertexPosition.Length - 1; V >= 0; --V)
            {
                ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            return new Tile3D(0, ArrayVertex, ArrayIndex);
        }

        public void TogglePreview(bool UsePreview)
        {
        }

        public void RemoveTileset(int TilesetIndex)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            Cursor = CreateCursor(Map, Map.CursorPositionVisible.X, Map.CursorPositionVisible.Y, LayerIndex, sprCursor.Width, sprCursor.Height, Radius);
            Camera.SetTarget(new Vector3(Map.TileSize.X * Map.MapSize.X / 2, LayerIndex * 32, Map.TileSize.Y * Map.MapSize.Y / 2));
            Camera.Update(gameTime);

            DicDrawablePointPerColor.Clear();
        }
        
        public void BeginDraw(CustomSpriteBatch g)
        {
        }

        public void Draw(CustomSpriteBatch g, int LayerIndex, bool IsSubLayer, MovementAlgorithmTile[,] ArrayTerrain)
        {
            PolygonEffect.View = Camera.View;
            g.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            g.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            g.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            foreach (Tile3D ActiveTile in ListTile3D)
            {
                PolygonEffect.Texture = Map.ListTileSet[ActiveTile.TilesetIndex];
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                ActiveTile.Draw(g.GraphicsDevice);
            }

            PolygonEffect.Texture = sprCursor;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();

            Cursor.Draw(g.GraphicsDevice);

            g.End();
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                {//If it's dead, don't draw it unless it's an event unit.
                    if ((Map.ListPlayer[P].ListSquad[U].CurrentLeader == null && !Map.ListPlayer[P].ListSquad[U].IsEventSquad) || Map.ListPlayer[P].ListSquad[U].IsDead)
                        continue;

                    Color UnitColor;
                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        UnitColor = Map.ListPlayer[P].Color;
                    else
                        UnitColor = Color.White;

                    Map.ListPlayer[P].ListSquad[U].Unit3D.SetViewMatrix(Camera.View);

                    Map.ListPlayer[P].ListSquad[U].Unit3D.SetPosition(
                        -Map.MapSize.X / 2 + 0.5f + Map.ListPlayer[P].ListSquad[U].Position.X,
                        Radius,
                        -Map.MapSize.Y / 2 + 0.5f + Map.ListPlayer[P].ListSquad[U].Y);

                    Map.ListPlayer[P].ListSquad[U].Unit3D.Draw(GameScreen.GraphicsDevice);
                }
            }
            g.Begin();
        }
        
        public void AddDrawablePoints(List<Vector3> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (Vector3 ActivePoint in ListPoint)
            {
                ListDrawablePoint3D.Add(CreateCursor(Map, ActivePoint.X, ActivePoint.Y, LayerIndex, 32, 32, Radius));
            }

            DicDrawablePointPerColor.Add(PointColor, ListDrawablePoint3D);
        }

        public void Reset()
        {
            CreateMap(Map);
        }
    }
}
