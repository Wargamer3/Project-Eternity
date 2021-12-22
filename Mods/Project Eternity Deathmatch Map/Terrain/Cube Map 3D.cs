using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class CubeMap3D : Map3D
    {
        protected virtual bool Spherical { get { return false; } }

        public CubeMap3D(DeathmatchMap Map, int LayerIndex, MapLayer Owner, GraphicsDevice g)
            : base(Map, LayerIndex, Owner, g)
        {
        }

        protected override void CreateMap(DeathmatchMap Map)
        {
            ListTile3D.Clear();

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    Vector3[] ArrayVertexPosition = new Vector3[4];
                    ArrayVertexPosition[0] = new Vector3(X * Map.TileSize.X, 0, Y * Map.TileSize.Y);
                    ArrayVertexPosition[1] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, 0, Y * Map.TileSize.Y);
                    ArrayVertexPosition[2] = new Vector3(X * Map.TileSize.X, 0, Y * Map.TileSize.Y + Map.TileSize.Y);
                    ArrayVertexPosition[3] = new Vector3(X * Map.TileSize.X + Map.TileSize.X, 0, Y * Map.TileSize.Y + Map.TileSize.Y);

                    Vector3[] ArrayTransformedVertexPosition = new Vector3[ArrayVertexPosition.Length];

                    Matrix TranslationToOriginMatrix = Matrix.CreateTranslation(-Radius, Radius, -Radius);
                    Vector3.Transform(ArrayVertexPosition, ref TranslationToOriginMatrix, ArrayTransformedVertexPosition);
                    for (int V = ArrayVertexPosition.Length - 1; V >= 0; --V)
                    {
                        ArrayVertexPosition[V] = ArrayTransformedVertexPosition[V];
                    }

                    Map2D GroundLayer = Map.ListLayer[0].OriginalLayerGrid;
                    DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
                    Texture2D ActiveTileset = Map.ListTileSet[ActiveTerrain.TilesetIndex];
                    Tile3D NewTile = CreateTile(Map, ActiveTileset, ArrayVertexPosition, X, Y);

                    if (Spherical)
                    {
                        MoveToSphericalCoordinates(NewTile, Radius);
                        CreateSphericalElevation(Map, ActiveTileset, NewTile, X, Y);
                    }
                    else
                    {
                        CreateFlatElevation(Map, ActiveTileset, NewTile, X, Y);
                    }
                    CreateCubicTile(Map, NewTile);
                }
            }
        }

        private Tile3D CreateTile(DeathmatchMap Map, Texture2D ActiveTileset, Vector3[] ArrayVertexPosition, int X, int Y)
        {
            //Add and remove a half pixel offset to avoid texture bleeding.
            VertexPositionColorTexture[] ArrayVertex = new VertexPositionColorTexture[4];
            Map2D GroundLayer = Map.ListLayer[0].OriginalLayerGrid;
            DrawableTile ActiveTerrain = GroundLayer.GetTile(X, Y);
            float UVXValue = ActiveTerrain.Origin.X + 0.5f;
            float UVYValue = ActiveTerrain.Origin.Y + 0.5f;
            Vector2 TextureSize = new Vector2(ActiveTileset.Width, ActiveTileset.Height);

            ArrayVertex[0] = new VertexPositionColorTexture();
            ArrayVertex[0].Position = ArrayVertexPosition[0];
            ArrayVertex[0].TextureCoordinate = new Vector2(UVXValue / TextureSize.X, UVYValue / TextureSize.Y);
            ArrayVertex[0].Color = Color.White;

            UVXValue = ActiveTerrain.Origin.X + Map.TileSize.X - 0.5f;
            UVYValue = ActiveTerrain.Origin.Y + 0.5f;
            ArrayVertex[1] = new VertexPositionColorTexture();
            ArrayVertex[1].Position = ArrayVertexPosition[1];
            ArrayVertex[1].TextureCoordinate = new Vector2(UVXValue / TextureSize.X, UVYValue / TextureSize.Y);
            ArrayVertex[1].Color = Color.White;

            UVXValue = ActiveTerrain.Origin.X + 0.5f;
            UVYValue = ActiveTerrain.Origin.Y + Map.TileSize.Y - 0.5f;
            ArrayVertex[2] = new VertexPositionColorTexture();
            ArrayVertex[2].Position = ArrayVertexPosition[2];
            ArrayVertex[2].TextureCoordinate = new Vector2(UVXValue / TextureSize.X, UVYValue / TextureSize.Y);
            ArrayVertex[2].Color = Color.White;

            UVXValue =ActiveTerrain.Origin.X + Map.TileSize.X - 0.5f;
            UVYValue = ActiveTerrain.Origin.Y + Map.TileSize.Y - 0.5f;
            ArrayVertex[3] = new VertexPositionColorTexture();
            ArrayVertex[3].Position = ArrayVertexPosition[3];
            ArrayVertex[3].TextureCoordinate = new Vector2(UVXValue / TextureSize.X, UVYValue / TextureSize.Y);
            ArrayVertex[3].Color = Color.White;

            short[] ArrayIndex = new short[6];
            ArrayIndex[0] = 0;
            ArrayIndex[1] = 1;
            ArrayIndex[2] = 3;
            ArrayIndex[3] = 0;
            ArrayIndex[4] = 3;
            ArrayIndex[5] = 2;

            Tile3D NewTile = new Tile3D(0, ArrayVertex, ArrayIndex);

            return NewTile;
        }

        private void CreateCubicTile(DeathmatchMap Map, Tile3D Original)
        {
            const float DegToRag = 0.0174532925f;
            float Radius = (Map.MapSize.X * Map.TileSize.X) / 2f;

            Vector3[] ArrayOriginalVertexPosition = new Vector3[Original.ArrayVertex.Length];
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                ArrayOriginalVertexPosition[V] = Original.ArrayVertex[V].Position;
            }
            Vector3[] ArrayTransformedVertexPosition = new Vector3[Original.ArrayVertex.Length];
            Matrix TranslationToOriginMatrix = Matrix.CreateTranslation(0f, -Radius, 0f);
            Matrix RotationMatrix;
            Matrix TranslationMatrix;
            Matrix FinalMatrix;
            Tile3D NewTile;

            //Top
            TranslationMatrix = Matrix.CreateTranslation(0f, Radius, 0f);
            FinalMatrix = TranslationToOriginMatrix * TranslationMatrix;

            Vector3.Transform(ArrayOriginalVertexPosition, ref FinalMatrix, ArrayTransformedVertexPosition);
            NewTile = new Tile3D(Original);
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                NewTile.ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            ListTile3D.Add(NewTile);

            //Front
            RotationMatrix = Matrix.CreateRotationX(90 * DegToRag);
            TranslationMatrix = Matrix.CreateTranslation(0f, 0f, Radius);
            FinalMatrix = TranslationToOriginMatrix * RotationMatrix * TranslationMatrix;

            Vector3.Transform(ArrayOriginalVertexPosition, ref FinalMatrix, ArrayTransformedVertexPosition);
            NewTile = new Tile3D(Original);
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                NewTile.ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            ListTile3D.Add(NewTile);

            //Back
            RotationMatrix = Matrix.CreateRotationX(270 * DegToRag);
            TranslationMatrix = Matrix.CreateTranslation(0f, 0f, -Radius);
            FinalMatrix = TranslationToOriginMatrix * RotationMatrix * TranslationMatrix;

            Vector3.Transform(ArrayOriginalVertexPosition, ref FinalMatrix, ArrayTransformedVertexPosition);
            NewTile = new Tile3D(Original);
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                NewTile.ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            ListTile3D.Add(NewTile);

            //Left
            RotationMatrix = Matrix.CreateRotationX(90 * DegToRag) * Matrix.CreateRotationY(90 * DegToRag);
            TranslationMatrix = Matrix.CreateTranslation(Radius, 0f, 0f);
            FinalMatrix = TranslationToOriginMatrix * RotationMatrix * TranslationMatrix;

            Vector3.Transform(ArrayOriginalVertexPosition, ref FinalMatrix, ArrayTransformedVertexPosition);
            NewTile = new Tile3D(Original);
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                NewTile.ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            ListTile3D.Add(NewTile);

            //Right
            RotationMatrix = Matrix.CreateRotationX(90 * DegToRag) * Matrix.CreateRotationY(270 * DegToRag);
            TranslationMatrix = Matrix.CreateTranslation(-Radius, 0f, 0f);
            FinalMatrix = TranslationToOriginMatrix * RotationMatrix * TranslationMatrix;

            Vector3.Transform(ArrayOriginalVertexPosition, ref FinalMatrix, ArrayTransformedVertexPosition);
            NewTile = new Tile3D(Original);
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                NewTile.ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            ListTile3D.Add(NewTile);

            //Bottom
            RotationMatrix = Matrix.CreateRotationX(180 * DegToRag);
            TranslationMatrix = Matrix.CreateTranslation(0f, -Radius, 0f);
            FinalMatrix = TranslationToOriginMatrix * RotationMatrix * TranslationMatrix;

            Vector3.Transform(ArrayOriginalVertexPosition, ref FinalMatrix, ArrayTransformedVertexPosition);
            NewTile = new Tile3D(Original);
            for (int V = Original.ArrayVertex.Length - 1; V >= 0; --V)
            {
                NewTile.ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            ListTile3D.Add(NewTile);
        }

        private void MoveToSphericalCoordinates(Tile3D ActiveTile, float Radius)
        {
            for (int V = ActiveTile.ArrayVertex.Length - 1; V >= 0; --V)
            {
                double X = ActiveTile.ArrayVertex[V].Position.X / Radius;
                double Y = ActiveTile.ArrayVertex[V].Position.Y / Radius;
                double Z = ActiveTile.ArrayVertex[V].Position.Z / Radius;

                double TransformedX = X * Math.Sqrt(1.0 - (Y * Y * 0.5) - (Z * Z * 0.5) + (Y * Y * Z * Z / 3.0));
                double TransformedY = Y * Math.Sqrt(1.0 - (Z * Z * 0.5) - (X * X * 0.5) + (Z * Z * X * X / 3.0));
                double TransformedZ = Z * Math.Sqrt(1.0 - (X * X * 0.5) - (Y * Y * 0.5) + (X * X * Y * Y / 3.0));

                ActiveTile.ArrayVertex[V].Position.X = (float)(TransformedX * Radius);
                ActiveTile.ArrayVertex[V].Position.Y = (float)(TransformedY * Radius);
                ActiveTile.ArrayVertex[V].Position.Z = (float)(TransformedZ * Radius);
            }
        }

        private void CreateFlatElevation(DeathmatchMap Map, Texture2D TileSet, Tile3D ActiveTile, int X, int Y)
        {
            Vector3 A = ActiveTile.ArrayVertex[0].Position;
            Vector3 B = ActiveTile.ArrayVertex[1].Position;
            Vector3 C = ActiveTile.ArrayVertex[2].Position;
            Vector3 D = ActiveTile.ArrayVertex[3].Position;

            Vector3 Normal = Vector3.Cross(C - B, B - A);
            Normal.Normalize();

            float Z = Map.GetTerrain(X, Y, LayerIndex).Position.Z;

            for (int V = ActiveTile.ArrayVertex.Length - 1; V >= 0; --V)
            {
                ActiveTile.ArrayVertex[V].Position += Normal * Z;
            }

            //Create slope right
            if (X + 1 < Map.MapSize.X)
            {
                float ZRight = Map.GetTerrain(X + 1, Y, LayerIndex).Position.Z;

                if (Z != ZRight)
                {
                    Vector3[] ArrayVertexPositionRight = new Vector3[4];
                    ArrayVertexPositionRight[0] = ActiveTile.ArrayVertex[1].Position;
                    ArrayVertexPositionRight[2] = ActiveTile.ArrayVertex[3].Position;
                    ArrayVertexPositionRight[1] = B + Normal * ZRight;
                    ArrayVertexPositionRight[3] = D + Normal * ZRight;

                    Tile3D NewTile = CreateTile(Map, TileSet, ArrayVertexPositionRight, X, Y);
                    CreateCubicTile(Map, NewTile);
                }
            }

            //Create slope down
            if (Y + 1 < Map.MapSize.Y)
            {
                float ZDown = Map.GetTerrain(X, Y + 1, LayerIndex).Position.Z;

                if (Z != ZDown)
                {
                    Vector3[] ArrayVertexPositionDown = new Vector3[4];
                    ArrayVertexPositionDown[0] = ActiveTile.ArrayVertex[2].Position;
                    ArrayVertexPositionDown[1] = ActiveTile.ArrayVertex[3].Position;
                    ArrayVertexPositionDown[2] = C + Normal * ZDown;
                    ArrayVertexPositionDown[3] = D + Normal * ZDown;

                    Tile3D NewTile = CreateTile(Map, TileSet, ArrayVertexPositionDown, X, Y);
                    CreateCubicTile(Map, NewTile);
                }
            }
        }

        private void CreateSphericalElevation(DeathmatchMap Map, Texture2D TileSet, Tile3D ActiveTile, int X, int Y)
        {
            Vector3 A = ActiveTile.ArrayVertex[0].Position;
            Vector3 B = ActiveTile.ArrayVertex[1].Position;
            Vector3 C = ActiveTile.ArrayVertex[2].Position;
            Vector3 D = ActiveTile.ArrayVertex[3].Position;

            float Z = Map.GetTerrain(X, Y, LayerIndex).Position.Z;

            for (int V = ActiveTile.ArrayVertex.Length - 1; V >= 0; --V)
            {
                ActiveTile.ArrayVertex[V].Position += Vector3.Normalize(ActiveTile.ArrayVertex[V].Position) * Z;
            }

            //Create slopes (right)
            if (X + 1 < Map.MapSize.X)
            {
                float ZRight = Map.GetTerrain(X + 1, Y, LayerIndex).Position.Z;

                if (Z != ZRight)
                {
                    //Create slope right
                    Vector3[] ArrayVertexPositionRight = new Vector3[4];
                    ArrayVertexPositionRight[0] = ActiveTile.ArrayVertex[1].Position;
                    ArrayVertexPositionRight[2] = ActiveTile.ArrayVertex[3].Position;
                    ArrayVertexPositionRight[1] = B + Vector3.Normalize(B) * ZRight;
                    ArrayVertexPositionRight[3] = D + Vector3.Normalize(D) * ZRight;

                    Tile3D NewTile = CreateTile(Map, TileSet, ArrayVertexPositionRight, X, Y);
                    CreateCubicTile(Map, NewTile);
                }
            }

            //Create slope down
            if (Y + 1 < Map.MapSize.Y)
            {
                float ZDown = Map.GetTerrain(X, Y + 1, LayerIndex).Position.Z;

                if (Z != ZDown)
                {
                    Vector3[] ArrayVertexPositionDown = new Vector3[4];
                    ArrayVertexPositionDown[0] = ActiveTile.ArrayVertex[2].Position;
                    ArrayVertexPositionDown[1] = ActiveTile.ArrayVertex[3].Position;
                    ArrayVertexPositionDown[2] = C + Vector3.Normalize(C) * ZDown;
                    ArrayVertexPositionDown[3] = D + Vector3.Normalize(D) * ZDown;

                    Tile3D NewTile = CreateTile(Map, TileSet, ArrayVertexPositionDown, X, Y);
                    CreateCubicTile(Map, NewTile);
                }
            }
        }
    }
}
