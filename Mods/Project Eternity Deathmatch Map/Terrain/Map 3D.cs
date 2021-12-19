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
        public Dictionary<Texture2D, List<Tile3D>> DicTile3D;
        protected BasicEffect PolygonEffect;
        protected float Radius;
        private DeathmatchMap Map;
        protected int LayerIndex;
        public DeathmatchCamera Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private Dictionary<Color, List<Tile3D>> DicDrawablePointPerColor;
        private Random Random;

        public Map3D(DeathmatchMap Map, int LayerIndex, GraphicsDevice g)
        {
            this.Map = Map;
            this.LayerIndex = LayerIndex;
            Random = new Random();
            sprCursor = Map.sprCursor;
            Camera = new DeathmatchCamera(g);
            Radius = (Map.MapSize.X * Map.TileSize.X) / 2;

            PolygonEffect = new BasicEffect(g);

            PolygonEffect.VertexColorEnabled = true;
            PolygonEffect.TextureEnabled = true;

            float aspectRatio = g.Viewport.Width / (float)g.Viewport.Height;

            Matrix Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    1, 10000);
            PolygonEffect.Projection = Projection;

            PolygonEffect.World = Matrix.Identity;
            PolygonEffect.View = Matrix.Identity;

            DicDrawablePointPerColor = new Dictionary<Color, List<Tile3D>>();
            DicTile3D = new Dictionary<Texture2D, List<Tile3D>>();

            CreateMap(Map);

            Cursor = CreateCursor(Map, Map.CursorPositionVisible.X, Map.CursorPositionVisible.Y, LayerIndex, sprCursor.Width, sprCursor.Height, Radius);
        }

        public void Save(BinaryWriter BW)
        {
            throw new NotImplementedException();
        }

        public void Load(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        protected virtual void CreateMap(DeathmatchMap Map)
        {
            DicTile3D.Clear();

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    float Z = (-Map.GetTerrain(X, Y, LayerIndex).Position.Z - (LayerIndex * 3)) *- 32;
                    Vector3[] ArrayVertexPosition = new Vector3[4];
                    ArrayVertexPosition[0] = new Vector3(X * Map.TileSize.X, Z, Y * Map.TileSize.Y);
                    ArrayVertexPosition[1] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y);
                    ArrayVertexPosition[2] = new Vector3(X * Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);
                    ArrayVertexPosition[3] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);

                    Map2D GroundLayer = Map.ListLayer[LayerIndex].OriginalLayerGrid;
                    DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                    Texture2D ActiveTileset = Map.ListTileSet[ActiveTerrain.Tileset];
                    if (!DicTile3D.ContainsKey(ActiveTileset))
                    {
                        DicTile3D.Add(ActiveTileset, new List<Tile3D>());
                    }
                    DicTile3D[ActiveTileset].Add(CreateTile3D(Map, ArrayVertexPosition, ActiveTerrain.Origin.X, ActiveTerrain.Origin.Y, X, Y, ActiveTileset.Width, ActiveTileset.Height, Radius));

                    //Create slope right
                    if (X + 1 < Map.MapSize.X)
                    {
                        float ZRight = Map.GetTerrain(X + 1, Y, LayerIndex).Position.Z;
                        if (Z != ZRight)
                        {
                            Vector3[] ArrayVertexPositionRight = new Vector3[4];
                            ArrayVertexPositionRight[0] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y);
                            ArrayVertexPositionRight[2] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);
                            ArrayVertexPositionRight[1] = new Vector3((X + 1) * Map.TileSize.X, ZRight, Y * Map.TileSize.Y);
                            ArrayVertexPositionRight[3] = new Vector3((X + 1) * Map.TileSize.X, ZRight, Y * Map.TileSize.Y + Map.TileSize.Y);
                            DicTile3D[ActiveTileset].Add(CreateTile3D(Map, ArrayVertexPositionRight, ActiveTerrain.Origin.X, ActiveTerrain.Origin.Y, X, Y, ActiveTileset.Width, ActiveTileset.Height, Radius));
                        }
                    }

                    //Create slope down
                    if (Y + 1 < Map.MapSize.Y)
                    {
                        float ZDown = Map.GetTerrain(X, Y + 1, LayerIndex).Position.Z;
                        if (Z != ZDown)
                        {
                            Vector3[] ArrayVertexPositionDown = new Vector3[4];
                            ArrayVertexPositionDown[0] = new Vector3(X * Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);
                            ArrayVertexPositionDown[1] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, Z, Y * Map.TileSize.Y + Map.TileSize.Y);
                            ArrayVertexPositionDown[2] = new Vector3(X * Map.TileSize.X, ZDown, (Y + 1) * Map.TileSize.Y);
                            ArrayVertexPositionDown[3] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, ZDown, (Y + 1) * Map.TileSize.Y);
                            DicTile3D[ActiveTileset].Add(CreateTile3D(Map, ArrayVertexPositionDown, ActiveTerrain.Origin.X, ActiveTerrain.Origin.Y, X, Y, ActiveTileset.Width, ActiveTileset.Height, Radius));
                        }
                    }
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

            return CreateTile3D(Map, ArrayVertexPosition, 0, 0, X, Y, TextureWidth, TextureHeight, Radius);
        }

        private static Tile3D CreateTile3D(DeathmatchMap Map, Vector3[] ArrayVertexPosition, float OffsetX, float OffsetY, float X, float Y, int TextureWidth, int TextureHeight, float Radius)
        {
            VertexPositionColorTexture[] ArrayVertex = new VertexPositionColorTexture[4];
            float UVXValue = OffsetX + 0.5f;
            float UVYValue = OffsetY + 0.5f;

            ArrayVertex[0] = new VertexPositionColorTexture();
            ArrayVertex[0].Position = new Vector3(ArrayVertexPosition[0].X - Map.TileSize.X / 2f, ArrayVertexPosition[0].Y, ArrayVertexPosition[0].Z - Map.TileSize.Y / 2f);
            ArrayVertex[0].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[0].Color = Color.White;

            UVXValue = OffsetX + Map.TileSize.X - 0.5f;
            UVYValue = OffsetY + 0.5f;
            ArrayVertex[1] = new VertexPositionColorTexture();
            ArrayVertex[1].Position = new Vector3(ArrayVertexPosition[1].X - Map.TileSize.X / 2f, ArrayVertexPosition[1].Y, ArrayVertexPosition[1].Z - Map.TileSize.Y / 2f);
            ArrayVertex[1].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[1].Color = Color.White;

            UVXValue = OffsetX + 0.5f;
            UVYValue = OffsetY + Map.TileSize.Y - 0.5f;
            ArrayVertex[2] = new VertexPositionColorTexture();
            ArrayVertex[2].Position = new Vector3(ArrayVertexPosition[2].X - Map.TileSize.X / 2f, ArrayVertexPosition[2].Y, ArrayVertexPosition[2].Z - Map.TileSize.Y / 2f);
            ArrayVertex[2].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[2].Color = Color.White;

            UVXValue = OffsetX + Map.TileSize.X - 0.5f;
            UVYValue = OffsetY + Map.TileSize.Y - 0.5f;
            ArrayVertex[3] = new VertexPositionColorTexture();
            ArrayVertex[3].Position = new Vector3(ArrayVertexPosition[3].X - Map.TileSize.X / 2f, ArrayVertexPosition[3].Y, ArrayVertexPosition[3].Z - Map.TileSize.Y / 2f);
            ArrayVertex[3].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[3].Color = Color.White;

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

            return new Tile3D(ArrayVertex, ArrayIndex);
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
            Camera.TeleportCamera(new Vector3(Map.CursorPosition.X * Map.TileSize.X - Radius, Radius * 1.2f + 200, 200 - Radius * 0.9f + Map.CursorPosition.Y * Map.TileSize.Y));
            Camera.SetRotation(0, (float)-Math.PI / 4f, 0f);
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

            foreach (KeyValuePair<Texture2D, List<Tile3D>> ActiveTileSet in DicTile3D)
            {
                PolygonEffect.Texture = ActiveTileSet.Key;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                foreach (Tile3D ActiveTile in ActiveTileSet.Value)
                {
                    ActiveTile.Draw(g.GraphicsDevice);
                }
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
