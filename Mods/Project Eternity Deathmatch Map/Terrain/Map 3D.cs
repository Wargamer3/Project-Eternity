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

        public Dictionary<int, List<Tile3D>> DicTile3DByTileset;
        protected BasicEffect PolygonEffect;
        protected float Radius;
        private DeathmatchMap Map;
        protected int LayerIndex;
        protected MapLayer Owner;
        public DefaultCamera Camera;
        private Texture2D sprCursor;
        private Tile3D Cursor;
        private Dictionary<Color, List<Tile3D>> DicDrawablePointPerColor;
        private List<Tile3D> ListDrawableArrowPerColor;
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
            DicTile3DByTileset = new Dictionary<int, List<Tile3D>>();
            ListDrawableArrowPerColor = new List<Tile3D>();

            CreateMap(Map);

            int X = (int)Map.CursorPositionVisible.X;
            int Y = (int)Map.CursorPositionVisible.Y;
            float Z = Owner.ArrayTerrain[X, Y].Position.Z * 32 + (LayerIndex * 64);
            Map2D GroundLayer = Owner.OriginalLayerGrid;
            DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
            Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
            Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, LayerIndex * 64, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];
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
            DicTile3DByTileset.Clear();
            Map2D GroundLayer = Owner.OriginalLayerGrid;

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

                    float Z = Owner.ArrayTerrain[X, Y].Position.Z * 32 + (LayerIndex * 32);
                    float ZFront = Z;
                    float ZBack = Z;
                    float ZRight = Z;
                    float ZLeft = Z;
                    if (Y + 1 < Map.MapSize.Y)
                    {
                        ZFront = Owner.ArrayTerrain[X, Y + 1].Position.Z * 32 + (LayerIndex * 32);
                    }
                    if (Y - 1 >= 0)
                    {
                        ZBack = Owner.ArrayTerrain[X, Y - 1].Position.Z * 32 + (LayerIndex * 32);
                    }
                    if (X - 1 >= 0)
                    {
                        ZLeft = Owner.ArrayTerrain[X - 1, Y].Position.Z * 32 + (LayerIndex * 32);
                    }
                    if (X + 1 < Map.MapSize.X)
                    {
                        ZRight = Owner.ArrayTerrain[X + 1, Y].Position.Z * 32 + (LayerIndex * 32);
                    }

                    List<Tile3D> ListNew3DTile = ActiveTerrain3D.CreateTile3D(ActiveTerrain.TilesetIndex, ActiveTerrain.Origin.Location,
                                            X * Map.TileSize.X, Y * Map.TileSize.Y, Z, (LayerIndex * 32), Map.TileSize, Map.ListTileSet, ZFront, ZBack, ZLeft, ZRight, 0);

                    foreach (Tile3D ActiveTile in ListNew3DTile)
                    {
                        if (!DicTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                        {
                            DicTile3DByTileset.Add(ActiveTile.TilesetIndex, new List<Tile3D>());
                        }
                        DicTile3DByTileset[ActiveTile.TilesetIndex].Add(ActiveTile);
                    }
                }
            }
        }
        
        public void RemoveTileset(int TilesetIndex)
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            if (Map.CursorPositionVisible.X < 0)
            {
                Camera.CameraHeight = 600;
                Camera.CameraDistance = 500;
                Camera.SetTarget(new Vector3(Map.TileSize.X * Map.MapSize.X / 2, LayerIndex * 32, Map.TileSize.Y * Map.MapSize.Y / 2));
                Camera.Update(gameTime);
                return;
            }

            Camera.CameraHeight = 400;
            Camera.CameraDistance = 300;
            int X = (int)Map.CursorPositionVisible.X;
            int Y = (int)Map.CursorPositionVisible.Y;
            float Z = Owner.ArrayTerrain[X, Y].Position.Z * 32 + (LayerIndex * 32) + 0.3f;
            Map2D GroundLayer = Owner.OriginalLayerGrid;
            DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
            Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;
            Cursor = ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, LayerIndex * 32 + 0.3f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0];

            Camera.SetTarget(new Vector3(Map.TileSize.X * Map.CursorPositionVisible.X, Map.CursorPosition.Z * 32, Map.TileSize.Y * Map.CursorPositionVisible.Y));
            Camera.Update(gameTime);

            DicDrawablePointPerColor.Clear();
            ListDrawableArrowPerColor.Clear();
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

            foreach (KeyValuePair<int, List<Tile3D>> ActiveTileSet in DicTile3DByTileset)
            {
                PolygonEffect.Texture = Map.ListTileSet[ActiveTileSet.Key];
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                foreach (Tile3D ActiveTile in ActiveTileSet.Value)
                {
                    ActiveTile.Draw(g.GraphicsDevice);
                }
            }

            if (IsSubLayer)
            {
                return;
            }

            g.End();
            GameScreen.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            for (int P = 0; P < Map.ListPlayer.Count; P++)
            {
                //If the selected unit have the order to move, draw the possible positions it can go to.
                for (int U = 0; U < Map.ListPlayer[P].ListSquad.Count; U++)
                {
                    //If it's dead, don't draw it unless it's an event unit.
                    if ((int)Map.ListPlayer[P].ListSquad[U].Position.Z != LayerIndex
                        || (Map.ListPlayer[P].ListSquad[U].CurrentLeader == null && !Map.ListPlayer[P].ListSquad[U].IsEventSquad)
                        || Map.ListPlayer[P].ListSquad[U].IsDead)
                        continue;

                    Color UnitColor;
                    if (Constants.UnitRepresentationState == Constants.UnitRepresentationStates.Colored)
                        UnitColor = Map.ListPlayer[P].Color;
                    else
                        UnitColor = Color.White;

                    Map.ListPlayer[P].ListSquad[U].Unit3D.SetViewMatrix(Camera.View);
                    float TerrainZ = Owner.ArrayTerrain[(int)Map.ListPlayer[P].ListSquad[U].Position.X, (int)Map.ListPlayer[P].ListSquad[U].Position.Y].Position.Z;

                    Map.ListPlayer[P].ListSquad[U].Unit3D.SetPosition(
                        Map.ListPlayer[P].ListSquad[U].Position.X + 0.5f,
                        Map.ListPlayer[P].ListSquad[U].Position.Z * 32 + TerrainZ * 32,
                        Map.ListPlayer[P].ListSquad[U].Position.Y + 0.5f);

                    Map.ListPlayer[P].ListSquad[U].Unit3D.Draw(GameScreen.GraphicsDevice);
                }
            }

            for (int P = 0; P < Owner.ListProp.Count; ++P)
            {
                Owner.ListProp[P].Unit3D.SetViewMatrix(Camera.View);
                float TerrainZ = Owner.ArrayTerrain[(int)Owner.ListProp[P].Position.X, (int)Owner.ListProp[P].Position.Y].Position.Z;

                Owner.ListProp[P].Unit3D.SetPosition(
                    Owner.ListProp[P].Position.X + 0.5f,
                    Owner.ListProp[P].Position.Z * 32 + TerrainZ * 32,
                    Owner.ListProp[P].Position.Y + 0.5f);

                Owner.ListProp[P].Draw3D(GameScreen.GraphicsDevice);
            }

            DrawDrawablePoints(g);

            if ((int)Map.CursorPosition.Z == LayerIndex)
            {
                PolygonEffect.Texture = sprCursor;
                PolygonEffect.CurrentTechnique.Passes[0].Apply();

                Cursor.Draw(g.GraphicsDevice);
            }

            g.Begin();
        }
        
        public void AddDrawablePoints(List<MovementAlgorithmTile> ListPoint, Color PointColor)
        {
            List<Tile3D> ListDrawablePoint3D = new List<Tile3D>(ListPoint.Count);

            foreach (MovementAlgorithmTile ActivePoint in ListPoint)
            {
                int X = (int)ActivePoint.Position.X;
                int Y = (int)ActivePoint.Position.Y;
                float Z = Map.ListLayer[(int)ActivePoint.LayerIndex].ArrayTerrain[X, Y].Position.Z * 32 + (ActivePoint.LayerIndex * 32) + 0.1f;
                Map2D GroundLayer = Owner.OriginalLayerGrid;
                DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawablePoint3D.Add(ActiveTerrain3D.CreateTile3D(0, Point.Zero,
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Z + 0.1f, Map.TileSize, new List<Texture2D>() { sprCursor }, Z, Z, Z, Z, 0)[0]);
            }

            DicDrawablePointPerColor.Add(PointColor, ListDrawablePoint3D);
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
                float Z = Map.ListLayer[(int)ActivePoint.LayerIndex].ArrayTerrain[X, Y].Position.Z * 32 + (ActivePoint.LayerIndex * 32) + 0.1f;
                Map2D GroundLayer = Owner.OriginalLayerGrid;
                DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                Terrain3D ActiveTerrain3D = ActiveTerrain.Terrain3DInfo;

                ListDrawableArrowPerColor.Add(ActiveTerrain3D.CreateTile3D(0, GetCursorTextureOffset(Previous, ActivePoint, Next),
                X * Map.TileSize.X, Y * Map.TileSize.Y, Z, Z + 0.15f, Map.TileSize, new List<Texture2D>() { Map.sprCursorPath }, Z, Z, Z, Z, 0)[0]);
            }
        }

        private void DrawDrawablePoints(CustomSpriteBatch g)
        {
            PolygonEffect.Texture = GameScreen.sprPixel;
            PolygonEffect.CurrentTechnique.Passes[0].Apply();

            foreach (KeyValuePair<Color, List<Tile3D>> DrawablePointPerColor in DicDrawablePointPerColor)
            {
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
            CreateMap(Map);
        }
    }
}
