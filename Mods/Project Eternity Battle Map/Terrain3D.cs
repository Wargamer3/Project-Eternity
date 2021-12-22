using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Terrain3D
    {
        public enum TerrainStyles : byte { Flat, Cube, FlatWallLateral, FlatWallMedial, FlatWallFront, FlatWallBack, FlatWallLeft, FlatWallRight,
            SlopeLeftToRight, SlopeRightToLeft, SlopeFrontToBack, SlopeBackToFront,
            WedgeLeftToRight, WedgeRightToLeft, WedgeFrontToBack, WedgeBackToFront,
            FrontLeftCorner, FrontRightCorner, BackLeftCorner, BackRightCorner }

        public TerrainStyles TerrainStyle;
        public float Transparancy;

        public DrawableTile FrontFace;
        public DrawableTile BackFace;
        public DrawableTile LeftFace;
        public DrawableTile RightFace;

        public Terrain3D()
        {
            TerrainStyle = TerrainStyles.Flat;
        }

        public Terrain3D(Terrain3D Other)
        {
            TerrainStyle = Other.TerrainStyle;
            Transparancy = Other.Transparancy;

            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                default:
                    FrontFace = new DrawableTile(Other.FrontFace);
                    BackFace = new DrawableTile(Other.BackFace);
                    LeftFace = new DrawableTile(Other.LeftFace);
                    RightFace = new DrawableTile(Other.RightFace);
                    break;
            }
        }

        public Terrain3D(BinaryReader BR, int TileWidth, int TileHeight)
        {
            TerrainStyle = (TerrainStyles)BR.ReadByte();

            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                default:
                    FrontFace = new DrawableTile(BR, TileWidth, TileHeight);
                    BackFace = new DrawableTile(BR, TileWidth, TileHeight);
                    LeftFace = new DrawableTile(BR, TileWidth, TileHeight);
                    RightFace = new DrawableTile(BR, TileWidth, TileHeight);
                    break;
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write((byte)TerrainStyle);

            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                default:
                    FrontFace.Save(BW);
                    BackFace.Save(BW);
                    LeftFace.Save(BW);
                    RightFace.Save(BW);
                    break;
            }
        }

        public List<Tile3D> CreateTile3D(int TilesetIndex, Point Origin, float X, float Y, float Z, float MinZ, Point TileSize, List<Texture2D> ListTileSet,
            float FrontZ, float BackZ, float LeftZ, float RightZ, float Offset)
        {
            float TopFrontLeftX = X;
            float TopFrontRightX = X + TileSize.X;
            float TopBackLeftX = X;
            float TopBackRightX = X + TileSize.X;

            float TopFrontLeftY = Y + TileSize.Y;
            float TopFrontRightY = Y + TileSize.Y;
            float TopBackLeftY = Y;
            float TopBackRightY = Y;

            float TopFrontLeftZ = Z;
            float TopFrontRightZ = Z;
            float TopBackLeftZ = Z;
            float TopBackRightZ = Z;

            float BottomFrontLeftZ = MinZ;
            float BottomFrontRightZ = MinZ;
            float BottomBackLeftZ = MinZ;
            float BottomBackRightZ = MinZ;

            ComputeX(X, ref TopFrontLeftX, ref TopFrontRightX, ref TopBackLeftX, ref TopBackRightX);
            ComputeY(Y, ref TopFrontLeftY, ref TopFrontRightY, ref TopBackLeftY, ref TopBackRightY);
            ComputeZ(MinZ, ref TopFrontLeftZ, ref TopFrontRightZ, ref TopBackLeftZ, ref TopBackRightZ);

            List<Tile3D> ListTile3D = new List<Tile3D>();

            Vector3[] ArrayVertexPosition = new Vector3[4];
            ArrayVertexPosition[0] = new Vector3(TopBackLeftX, TopFrontLeftZ, TopBackLeftY);
            ArrayVertexPosition[1] = new Vector3(TopBackRightX, TopFrontRightZ, TopBackRightY);
            ArrayVertexPosition[2] = new Vector3(TopFrontLeftX, TopBackLeftZ, TopFrontLeftY);
            ArrayVertexPosition[3] = new Vector3(TopFrontRightX, TopBackRightZ, TopFrontRightY);

            Texture2D ActiveTileset = ListTileSet[TilesetIndex];

            ListTile3D.Add(CreateTile3D(TilesetIndex, TileSize, ArrayVertexPosition, Origin, ActiveTileset.Width, ActiveTileset.Height, Offset));

            if (Z > RightZ)
            {
                ActiveTileset = ListTileSet[RightFace.TilesetIndex];
                Vector3[] ArrayVertexPositionRight = new Vector3[4];
                ArrayVertexPositionRight[1] = new Vector3(TopBackRightX, TopFrontRightZ, TopBackRightY);
                ArrayVertexPositionRight[0] = new Vector3(TopFrontRightX, TopBackRightZ, TopFrontRightY);
                ArrayVertexPositionRight[3] = new Vector3(TopBackRightX, BottomFrontRightZ, TopBackRightY);
                ArrayVertexPositionRight[2] = new Vector3(TopFrontRightX, BottomBackRightZ, TopFrontRightY);
                ListTile3D.Add(CreateTile3D(RightFace.TilesetIndex, TileSize, ArrayVertexPositionRight, RightFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            if (Z > LeftZ)
            {
                ActiveTileset = ListTileSet[LeftFace.TilesetIndex];
                Vector3[] ArrayVertexPositionRight = new Vector3[4];
                ArrayVertexPositionRight[0] = new Vector3(TopBackLeftX, TopFrontLeftZ, TopBackLeftY);
                ArrayVertexPositionRight[1] = new Vector3(TopFrontLeftX, TopBackLeftZ, TopFrontLeftY);
                ArrayVertexPositionRight[2] = new Vector3(TopBackLeftX, BottomFrontLeftZ, TopBackLeftY);
                ArrayVertexPositionRight[3] = new Vector3(TopFrontLeftX, BottomBackLeftZ, TopFrontLeftY);
                ListTile3D.Add(CreateTile3D(LeftFace.TilesetIndex, TileSize, ArrayVertexPositionRight, LeftFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            if (Z > FrontZ)
            {
                ActiveTileset = ListTileSet[FrontFace.TilesetIndex];
                Vector3[] ArrayVertexPositionDown = new Vector3[4];
                ArrayVertexPositionDown[0] = new Vector3(TopFrontLeftX, TopBackLeftZ, TopFrontLeftY);
                ArrayVertexPositionDown[1] = new Vector3(TopFrontRightX, TopBackRightZ, TopFrontRightY);
                ArrayVertexPositionDown[2] = new Vector3(TopFrontLeftX, BottomBackLeftZ, TopFrontLeftY);
                ArrayVertexPositionDown[3] = new Vector3(TopFrontRightX, BottomBackRightZ, TopFrontRightY);
                ListTile3D.Add(CreateTile3D(FrontFace.TilesetIndex, TileSize, ArrayVertexPositionDown, FrontFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            if (Z > BackZ)
            {
                ActiveTileset = ListTileSet[BackFace.TilesetIndex];
                Vector3[] ArrayVertexPositionDown = new Vector3[4];
                ArrayVertexPositionDown[1] = new Vector3(TopBackLeftX, TopFrontLeftZ, TopBackLeftY);
                ArrayVertexPositionDown[0] = new Vector3(TopBackRightX, TopFrontRightZ, TopBackRightY);
                ArrayVertexPositionDown[3] = new Vector3(TopBackLeftX, BottomFrontLeftZ, TopBackLeftY);
                ArrayVertexPositionDown[2] = new Vector3(TopBackRightX, BottomFrontRightZ, TopBackRightY);
                ListTile3D.Add(CreateTile3D(BackFace.TilesetIndex, TileSize, ArrayVertexPositionDown, BackFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            return ListTile3D;
        }

        private void ComputeX(float MinX, ref float TopFrontLeftX, ref float TopFrontRightX, ref float TopBackLeftX, ref float TopBackRightX)
        {
            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                case TerrainStyles.FlatWallLateral:
                    break;
            }
        }

        private void ComputeY(float MinY, ref float TopFrontLeftY, ref float TopFrontRightY, ref float TopBackLeftY, ref float TopBackRightY)
        {
            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                case TerrainStyles.FlatWallLateral:
                    break;
            }
        }

        private void ComputeZ(float MinZ, ref float TopFrontLeftZ, ref float TopFrontRightZ, ref float TopBackLeftZ, ref float TopBackRightZ)
        {
            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    TopFrontLeftZ = MinZ;
                    TopFrontRightZ = MinZ;
                    TopBackLeftZ = MinZ;
                    TopBackRightZ = MinZ;
                    break;

                case TerrainStyles.Cube:
                    break;

                case TerrainStyles.SlopeFrontToBack:
                    TopFrontLeftZ = MinZ;
                    TopFrontRightZ = MinZ;
                    break;

                case TerrainStyles.SlopeBackToFront:
                    TopBackLeftZ = MinZ;
                    TopBackRightZ = MinZ;
                    break;

                case TerrainStyles.SlopeLeftToRight:
                    TopFrontLeftZ = MinZ;
                    TopBackLeftZ = MinZ;
                    break;

                case TerrainStyles.SlopeRightToLeft:
                    TopFrontRightZ = MinZ;
                    TopBackRightZ = MinZ;
                    break;

                case TerrainStyles.WedgeFrontToBack:
                    TopBackLeftZ = MinZ + 32;
                    TopBackRightZ = MinZ + 32;
                    break;

                case TerrainStyles.WedgeBackToFront:
                    TopFrontLeftZ = MinZ + 32;
                    TopFrontRightZ = MinZ + 32;
                    break;

                case TerrainStyles.WedgeLeftToRight:
                    TopFrontRightZ = MinZ + 32;
                    TopBackRightZ = MinZ + 32;
                    break;

                case TerrainStyles.WedgeRightToLeft:
                    TopFrontLeftZ = MinZ + 32;
                    TopBackLeftZ = MinZ + 32;
                    break;

                case TerrainStyles.FrontLeftCorner:
                    TopFrontLeftZ = MinZ;
                    TopFrontRightZ = MinZ;
                    TopBackLeftZ = MinZ;
                    break;

                case TerrainStyles.FrontRightCorner:
                    TopFrontLeftZ = MinZ;
                    TopFrontRightZ = MinZ;
                    TopBackRightZ = MinZ;
                    break;

                case TerrainStyles.BackLeftCorner:
                    TopFrontLeftZ = MinZ;
                    TopBackLeftZ = MinZ;
                    TopBackRightZ = MinZ;
                    break;

                case TerrainStyles.BackRightCorner:
                    TopFrontRightZ = MinZ;
                    TopBackLeftZ = MinZ;
                    TopBackRightZ = MinZ;
                    break;

                case TerrainStyles.FlatWallLateral:
                case TerrainStyles.FlatWallMedial:
                case TerrainStyles.FlatWallFront:
                case TerrainStyles.FlatWallBack:
                case TerrainStyles.FlatWallLeft:
                case TerrainStyles.FlatWallRight:
                    TopFrontLeftZ = MinZ + 32;
                    TopFrontRightZ = MinZ + 32;
                    TopBackLeftZ = MinZ + 32;
                    TopBackRightZ = MinZ + 32;
                    break;
            }
        }

        private static Tile3D CreateTile3D(int TilesetIndex, Point TileSize, Vector3[] ArrayVertexPosition, Point Offset, int TextureWidth, int TextureHeight, float PositionOffset)
        {
            VertexPositionColorTexture[] ArrayVertex = new VertexPositionColorTexture[4];
            float UVXValue = Offset.X + 0.5f;
            float UVYValue = Offset.Y + 0.5f;

            ArrayVertex[0] = new VertexPositionColorTexture();
            ArrayVertex[0].Position = new Vector3(ArrayVertexPosition[0].X - TileSize.X / 2f, ArrayVertexPosition[0].Y, ArrayVertexPosition[0].Z - TileSize.Y / 2f);
            ArrayVertex[0].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[0].Color = Color.White;

            UVXValue = Offset.X + TileSize.X - 0.5f;
            UVYValue = Offset.Y + 0.5f;
            ArrayVertex[1] = new VertexPositionColorTexture();
            ArrayVertex[1].Position = new Vector3(ArrayVertexPosition[1].X - TileSize.X / 2f, ArrayVertexPosition[1].Y, ArrayVertexPosition[1].Z - TileSize.Y / 2f);
            ArrayVertex[1].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[1].Color = Color.White;

            UVXValue = Offset.X + 0.5f;
            UVYValue = Offset.Y + TileSize.Y - 0.5f;
            ArrayVertex[2] = new VertexPositionColorTexture();
            ArrayVertex[2].Position = new Vector3(ArrayVertexPosition[2].X - TileSize.X / 2f, ArrayVertexPosition[2].Y, ArrayVertexPosition[2].Z - TileSize.Y / 2f);
            ArrayVertex[2].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[2].Color = Color.White;

            UVXValue = Offset.X + TileSize.X - 0.5f;
            UVYValue = Offset.Y + TileSize.Y - 0.5f;
            ArrayVertex[3] = new VertexPositionColorTexture();
            ArrayVertex[3].Position = new Vector3(ArrayVertexPosition[3].X - TileSize.X / 2f, ArrayVertexPosition[3].Y, ArrayVertexPosition[3].Z - TileSize.Y / 2f);
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

            Matrix TranslationToOriginMatrix = Matrix.CreateTranslation(-PositionOffset, PositionOffset, -PositionOffset);
            Vector3.Transform(ArrayVertexPosition, ref TranslationToOriginMatrix, ArrayTransformedVertexPosition);
            for (int V = ArrayVertexPosition.Length - 1; V >= 0; --V)
            {
                ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            return new Tile3D(TilesetIndex, ArrayVertex, ArrayIndex);
        }
    }
}
