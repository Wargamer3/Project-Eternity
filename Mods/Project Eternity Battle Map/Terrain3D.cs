using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class Terrain3D
    {
        public enum TerrainStyles : byte { Flat, Invisible, Cube, FlatWallLateral, FlatWallMedial, FlatWallFront, FlatWallBack, FlatWallLeft, FlatWallRight,
            SlopeLeftToRight, SlopeRightToLeft, SlopeFrontToBack, SlopeBackToFront,
            WedgeLeftToRight, WedgeRightToLeft, WedgeFrontToBack, WedgeBackToFront,
            FrontLeftCorner, FrontRightCorner, BackLeftCorner, BackRightCorner,
            Pyramid, PipeLeftToRight, PipeFrontToBack, PipeUpToDown, }

        public TerrainStyles TerrainStyle;
        public float Transparancy;

        public DrawableTile FrontFace;
        public DrawableTile BackFace;
        public DrawableTile LeftFace;
        public DrawableTile RightFace;

        public Terrain3D()
        {
            TerrainStyle = TerrainStyles.Flat;
            FrontFace = new DrawableTile(new Rectangle[0]);
            BackFace = new DrawableTile(new Rectangle[0]);
            LeftFace = new DrawableTile(new Rectangle[0]);
            RightFace = new DrawableTile(new Rectangle[0]);
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

        public List<Tile3D> CreateTile3D(int TilesetIndex, Point Origin, float X, float Y, float Z, float MinZ, Point TileSize, Point TextureSize, List<Texture2D> ListTileSet,
            float FrontZ, float BackZ, float LeftZ, float RightZ, float Offset)
        {
            Texture2D ActiveTileset = ListTileSet[TilesetIndex];

            switch (TerrainStyle)
            {
                case TerrainStyles.Pyramid:
                    return CreateComplexShape(TilesetIndex, Origin, X, Y, Z, MinZ, TileSize, ActiveTileset.Width, ActiveTileset.Height);
            }

            float TopFrontLeftX = X;
            float TopFrontRightX = X + TileSize.X;
            float TopBackLeftX = X;
            float TopBackRightX = X + TileSize.X;

            float BottomFrontLeftX = X;
            float BottomFrontRightX = X + TileSize.X;
            float BottomBackLeftX = X;
            float BottomBackRightX = X + TileSize.X;

            float TopFrontLeftY = Y + TileSize.Y;
            float TopFrontRightY = Y + TileSize.Y;
            float TopBackLeftY = Y;
            float TopBackRightY = Y;

            float BottomFrontLeftY = Y + TileSize.Y;
            float BottomFrontRightY = Y + TileSize.Y;
            float BottomBackLeftY = Y;
            float BottomBackRightY = Y;

            float TopFrontLeftZ = Z;
            float TopFrontRightZ = Z;
            float TopBackLeftZ = Z;
            float TopBackRightZ = Z;

            float BottomFrontLeftZ = MinZ;
            float BottomFrontRightZ = MinZ;
            float BottomBackLeftZ = MinZ;
            float BottomBackRightZ = MinZ;

            ComputeX(X, TileSize, ref TopFrontLeftX, ref TopFrontRightX, ref TopBackLeftX, ref TopBackRightX, ref BottomFrontLeftX, ref BottomFrontRightX, ref BottomBackLeftX, ref BottomBackRightX);
            ComputeY(Y, TileSize, ref TopFrontLeftY, ref TopFrontRightY, ref TopBackLeftY, ref TopBackRightY, ref BottomFrontLeftY, ref BottomFrontRightY, ref BottomBackLeftY, ref BottomBackRightY);
            ComputeZ(MinZ, ref TopFrontLeftZ, ref TopFrontRightZ, ref TopBackLeftZ, ref TopBackRightZ, ref BottomFrontLeftZ, ref BottomFrontRightZ, ref BottomBackLeftZ, ref BottomBackRightZ);

            List<Tile3D> ListTile3D = new List<Tile3D>();

            Vector3[] ArrayVertexPosition = new Vector3[4];
            ArrayVertexPosition[0] = new Vector3(TopBackLeftX, TopFrontLeftZ, TopBackLeftY);
            ArrayVertexPosition[1] = new Vector3(TopBackRightX, TopFrontRightZ, TopBackRightY);
            ArrayVertexPosition[2] = new Vector3(TopFrontLeftX, TopBackLeftZ, TopFrontLeftY);
            ArrayVertexPosition[3] = new Vector3(TopFrontRightX, TopBackRightZ, TopFrontRightY);

            ListTile3D.Add(CreateTile3D(TilesetIndex, TextureSize, ArrayVertexPosition, Origin, ActiveTileset.Width, ActiveTileset.Height, Offset));

            if (TopFrontRightZ > RightZ || TopBackRightZ > RightZ)
            {
                ActiveTileset = ListTileSet[RightFace.TilesetIndex];
                Vector3[] ArrayVertexPositionRight = new Vector3[4];
                ArrayVertexPositionRight[1] = new Vector3(TopBackRightX, TopFrontRightZ, TopBackRightY);
                ArrayVertexPositionRight[0] = new Vector3(TopFrontRightX, TopBackRightZ, TopFrontRightY);
                ArrayVertexPositionRight[3] = new Vector3(BottomBackRightX, BottomFrontRightZ, BottomBackRightY);
                ArrayVertexPositionRight[2] = new Vector3(BottomFrontRightX, BottomBackRightZ, BottomFrontRightY);
                ListTile3D.Add(CreateTile3D(RightFace.TilesetIndex, TextureSize, ArrayVertexPositionRight, RightFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            if (TopFrontLeftZ > LeftZ || TopBackLeftZ > LeftZ)
            {
                ActiveTileset = ListTileSet[LeftFace.TilesetIndex];
                Vector3[] ArrayVertexPositionRight = new Vector3[4];
                ArrayVertexPositionRight[0] = new Vector3(TopBackLeftX, TopFrontLeftZ, TopBackLeftY);
                ArrayVertexPositionRight[1] = new Vector3(TopFrontLeftX, TopBackLeftZ, TopFrontLeftY);
                ArrayVertexPositionRight[2] = new Vector3(BottomBackLeftX, BottomFrontLeftZ, BottomBackLeftY);
                ArrayVertexPositionRight[3] = new Vector3(BottomFrontLeftX, BottomBackLeftZ, BottomFrontLeftY);
                ListTile3D.Add(CreateTile3D(LeftFace.TilesetIndex, TextureSize, ArrayVertexPositionRight, LeftFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            if (TopFrontLeftZ > FrontZ || TopBackRightZ > FrontZ)
            {
                ActiveTileset = ListTileSet[FrontFace.TilesetIndex];
                Vector3[] ArrayVertexPositionDown = new Vector3[4];
                ArrayVertexPositionDown[0] = new Vector3(TopFrontLeftX, TopBackLeftZ, TopFrontLeftY);
                ArrayVertexPositionDown[1] = new Vector3(TopFrontRightX, TopBackRightZ, TopFrontRightY);
                ArrayVertexPositionDown[2] = new Vector3(BottomFrontLeftX, BottomBackLeftZ, BottomFrontLeftY);
                ArrayVertexPositionDown[3] = new Vector3(BottomFrontRightX, BottomBackRightZ, BottomFrontRightY);
                ListTile3D.Add(CreateTile3D(FrontFace.TilesetIndex, TextureSize, ArrayVertexPositionDown, FrontFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            if (TopBackLeftZ > BackZ || TopBackRightZ > BackZ)
            {
                ActiveTileset = ListTileSet[BackFace.TilesetIndex];
                Vector3[] ArrayVertexPositionDown = new Vector3[4];
                ArrayVertexPositionDown[1] = new Vector3(TopBackLeftX, TopFrontLeftZ, TopBackLeftY);
                ArrayVertexPositionDown[0] = new Vector3(TopBackRightX, TopFrontRightZ, TopBackRightY);
                ArrayVertexPositionDown[3] = new Vector3(BottomBackLeftX, BottomFrontLeftZ, BottomBackLeftY);
                ArrayVertexPositionDown[2] = new Vector3(BottomBackRightX, BottomFrontRightZ, BottomBackRightY);
                ListTile3D.Add(CreateTile3D(BackFace.TilesetIndex, TextureSize, ArrayVertexPositionDown, BackFace.Origin.Location, ActiveTileset.Width, ActiveTileset.Height, Offset));
            }

            return ListTile3D;
        }

        private void ComputeX(float MinX, Point TileSize, ref float TopFrontLeftX, ref float TopFrontRightX, ref float TopBackLeftX, ref float TopBackRightX,
            ref float BottomFrontLeftX, ref float BottomFrontRightX, ref float BottomBackLeftX, ref float BottomBackRightX)
        {
            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                case TerrainStyles.FlatWallLateral:
                    break;
                case TerrainStyles.PipeFrontToBack:
                    TopFrontLeftX = MinX;
                    TopFrontRightX = MinX + TileSize.X / 4;
                    TopBackLeftX = MinX;
                    TopBackRightX = MinX + TileSize.X / 4;

                    BottomFrontLeftX = MinX - TileSize.X / 4;
                    BottomFrontRightX = MinX;
                    BottomBackLeftX = MinX - TileSize.X / 4;
                    BottomBackRightX = MinX;
                    break;

                case TerrainStyles.PipeUpToDown:
                    TopFrontLeftX = MinX;
                    TopFrontRightX = MinX + TileSize.X / 4;
                    TopBackLeftX = MinX - TileSize.X / 4;
                    TopBackRightX = MinX;

                    BottomFrontLeftX = MinX;
                    BottomFrontRightX = MinX + TileSize.X / 4;
                    BottomBackLeftX = MinX - TileSize.X / 4;
                    BottomBackRightX = MinX;
                    break;
            }
        }

        private void ComputeY(float MinY, Point TileSize, ref float TopFrontLeftY, ref float TopFrontRightY, ref float TopBackLeftY, ref float TopBackRightY,
            ref float BottomFrontLeftY, ref float BottomFrontRightY, ref float BottomBackLeftY, ref float BottomBackRightY)
        {
            switch (TerrainStyle)
            {
                case TerrainStyles.Flat:
                    break;

                case TerrainStyles.FlatWallLateral:
                    break;

                case TerrainStyles.PipeUpToDown:
                    TopFrontLeftY = MinY + TileSize.Y / 4 + TileSize.Y / 4;
                    TopFrontRightY = MinY + TileSize.Y / 4;
                    TopBackLeftY = MinY + TileSize.Y / 4;
                    TopBackRightY = MinY - TileSize.Y / 4 + TileSize.Y / 4;

                    BottomFrontLeftY = MinY + TileSize.Y / 4 + TileSize.Y / 4;
                    BottomFrontRightY = MinY + TileSize.Y / 4;
                    BottomBackLeftY = MinY + TileSize.Y / 4;
                    BottomBackRightY = MinY - TileSize.Y / 4 + TileSize.Y / 4;
                    break;
            }
        }

        private void ComputeZ(float MinZ, ref float TopFrontLeftZ, ref float TopFrontRightZ, ref float TopBackLeftZ, ref float TopBackRightZ,
            ref float BottomFrontLeftZ, ref float BottomFrontRightZ, ref float BottomBackLeftZ, ref float BottomBackRightZ)
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

                case TerrainStyles.PipeFrontToBack:
                    TopFrontLeftZ = MinZ + 32;
                    TopFrontRightZ = MinZ + 24;
                    TopBackLeftZ = MinZ + 32;
                    TopBackRightZ = MinZ + 24;

                    BottomFrontLeftZ = MinZ + 24;
                    BottomFrontRightZ = MinZ + 16;
                    BottomBackLeftZ = MinZ + 24;
                    BottomBackRightZ = MinZ + 16;
                    break;

                case TerrainStyles.PipeUpToDown:
                    TopFrontLeftZ = MinZ + 32;
                    TopFrontRightZ = MinZ + 32;
                    TopBackLeftZ = MinZ + 32;
                    TopBackRightZ = MinZ + 32;

                    BottomFrontLeftZ = MinZ;
                    BottomFrontRightZ = MinZ;
                    BottomBackLeftZ = MinZ;
                    BottomBackRightZ = MinZ;
                    break;
            }
        }

        private List<Tile3D> CreateComplexShape(int TilesetIndex, Point Origin, float X, float Y, float Z, float MinZ, Point TileSize, int TextureWidth, int TextureHeight)
        {
            float FrontLeftX = X;
            float FrontRightX = X + TileSize.X;
            float BackLeftX = X;
            float BackRightX = X + TileSize.X;

            float FrontLeftY = Y + TileSize.Y;
            float FrontRightY = Y + TileSize.Y;
            float BackLeftY = Y;
            float BackRightY = Y;

            float TopFrontLeftZ = Z;
            float TopFrontRightZ = Z;
            float TopBackLeftZ = Z;
            float TopBackRightZ = Z;

            List<Tile3D> ListTile3D = new List<Tile3D>();

            switch (TerrainStyle)
            {
                case TerrainStyles.Pyramid:
                    Vector3[] PyramidTop = new Vector3[3];
                    PyramidTop[0] = new Vector3(FrontRightX, MinZ, FrontRightY);
                    PyramidTop[1] = new Vector3(FrontLeftX, MinZ, FrontLeftY);
                    PyramidTop[2] = new Vector3(X + TileSize.X / 2, Z, Y + TileSize.Y / 2);
                    ListTile3D.Add(CreateTriangle(FrontFace.TilesetIndex, TileSize, PyramidTop, FrontFace.Origin.Location, TextureWidth, TextureHeight));

                    PyramidTop[0] = new Vector3(BackRightX, MinZ, BackRightY);
                    PyramidTop[1] = new Vector3(FrontRightX, MinZ, FrontRightY);
                    PyramidTop[2] = new Vector3(X + TileSize.X / 2, Z, Y + TileSize.Y / 2);
                    ListTile3D.Add(CreateTriangle(RightFace.TilesetIndex, TileSize, PyramidTop, RightFace.Origin.Location, TextureWidth, TextureHeight));

                    PyramidTop[0] = new Vector3(FrontLeftX, MinZ, FrontLeftY);
                    PyramidTop[1] = new Vector3(BackLeftX, MinZ, BackLeftY);
                    PyramidTop[2] = new Vector3(X + TileSize.X / 2, Z, Y + TileSize.Y / 2);
                    ListTile3D.Add(CreateTriangle(LeftFace.TilesetIndex, TileSize, PyramidTop, LeftFace.Origin.Location, TextureWidth, TextureHeight));

                    PyramidTop[0] = new Vector3(BackLeftX, MinZ, BackLeftY);
                    PyramidTop[1] = new Vector3(BackRightX, MinZ, BackRightY);
                    PyramidTop[2] = new Vector3(X + TileSize.X / 2, Z, Y + TileSize.Y / 2);
                    ListTile3D.Add(CreateTriangle(BackFace.TilesetIndex, TileSize, PyramidTop, BackFace.Origin.Location, TextureWidth, TextureHeight));
                    break;
            }

            return ListTile3D;
        }

        private static Tile3D CreateTile3D(int TilesetIndex, Point TileSize, Vector3[] ArrayVertexPosition, Point Offset, int TextureWidth, int TextureHeight, float PositionOffset)
        {
            VertexPositionNormalTexture[] ArrayVertex = new VertexPositionNormalTexture[4];
            float UVXValue = Offset.X + 0.5f;
            float UVYValue = Offset.Y + 0.5f;

            Vector3 NormalTriangle = Vector3.Normalize(Vector3.Cross(ArrayVertexPosition[1] - ArrayVertexPosition[0], ArrayVertexPosition[2] - ArrayVertexPosition[0]));

            ArrayVertex[0] = new VertexPositionNormalTexture();
            ArrayVertex[0].Position = new Vector3(ArrayVertexPosition[0].X - TileSize.X / 2f, ArrayVertexPosition[0].Y, ArrayVertexPosition[0].Z - TileSize.Y / 2f);
            ArrayVertex[0].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[0].Normal = NormalTriangle;

            UVXValue = Offset.X + TileSize.X - 0.5f;
            UVYValue = Offset.Y + 0.5f;
            ArrayVertex[1] = new VertexPositionNormalTexture();
            ArrayVertex[1].Position = new Vector3(ArrayVertexPosition[1].X - TileSize.X / 2f, ArrayVertexPosition[1].Y, ArrayVertexPosition[1].Z - TileSize.Y / 2f);
            ArrayVertex[1].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[1].Normal = NormalTriangle;

            UVXValue = Offset.X + 0.5f;
            UVYValue = Offset.Y + TileSize.Y - 0.5f;
            ArrayVertex[2] = new VertexPositionNormalTexture();
            ArrayVertex[2].Position = new Vector3(ArrayVertexPosition[2].X - TileSize.X / 2f, ArrayVertexPosition[2].Y, ArrayVertexPosition[2].Z - TileSize.Y / 2f);
            ArrayVertex[2].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[2].Normal = NormalTriangle;

            UVXValue = Offset.X + TileSize.X - 0.5f;
            UVYValue = Offset.Y + TileSize.Y - 0.5f;
            ArrayVertex[3] = new VertexPositionNormalTexture();
            ArrayVertex[3].Position = new Vector3(ArrayVertexPosition[3].X - TileSize.X / 2f, ArrayVertexPosition[3].Y, ArrayVertexPosition[3].Z - TileSize.Y / 2f);
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

            Matrix TranslationToOriginMatrix = Matrix.CreateTranslation(-PositionOffset, PositionOffset, -PositionOffset);
            Vector3.Transform(ArrayVertexPosition, ref TranslationToOriginMatrix, ArrayTransformedVertexPosition);
            for (int V = ArrayVertexPosition.Length - 1; V >= 0; --V)
            {
                ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            return new Tile3D(TilesetIndex, ArrayVertex, ArrayIndex);
        }

        private Tile3D CreateTriangle(int TilesetIndex, Point TileSize, Vector3[] ArrayVertexPosition, Point Offset, int TextureWidth, int TextureHeight)
        {
            VertexPositionNormalTexture[] ArrayVertex = new VertexPositionNormalTexture[3];

            Vector3 NormalTriangle = Vector3.Normalize(Vector3.Cross(ArrayVertexPosition[1] - ArrayVertexPosition[0], ArrayVertexPosition[2] - ArrayVertexPosition[0]));

            float UVXValue = Offset.X + 0.5f;
            float UVYValue = Offset.Y + TileSize.Y - 0.5f;
            ArrayVertex[0] = new VertexPositionNormalTexture();
            ArrayVertex[0].Position = new Vector3(ArrayVertexPosition[0].X - TileSize.X / 2f, ArrayVertexPosition[0].Y, ArrayVertexPosition[0].Z - TileSize.Y / 2f);
            ArrayVertex[0].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[0].Normal = NormalTriangle;

            UVXValue = Offset.X + TileSize.X - 0.5f;
            UVYValue = Offset.Y + TileSize.Y - 0.5f;
            ArrayVertex[1] = new VertexPositionNormalTexture();
            ArrayVertex[1].Position = new Vector3(ArrayVertexPosition[1].X - TileSize.X / 2f, ArrayVertexPosition[1].Y, ArrayVertexPosition[1].Z - TileSize.Y / 2f);
            ArrayVertex[1].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[1].Normal = NormalTriangle;

            UVXValue = Offset.X + TileSize.X / 2 - 0.5f;
            UVYValue = Offset.Y;
            ArrayVertex[2] = new VertexPositionNormalTexture();
            ArrayVertex[2].Position = new Vector3(ArrayVertexPosition[2].X - TileSize.X / 2f, ArrayVertexPosition[2].Y, ArrayVertexPosition[2].Z - TileSize.Y / 2f);
            ArrayVertex[2].TextureCoordinate = new Vector2(UVXValue / TextureWidth, UVYValue / TextureHeight);
            ArrayVertex[2].Normal = NormalTriangle;

            short[] ArrayIndex = new short[3];
            ArrayIndex[0] = 0;
            ArrayIndex[1] = 1;
            ArrayIndex[2] = 2;

            Vector3[] ArrayTransformedVertexPosition = new Vector3[ArrayVertexPosition.Length];

            Matrix TranslationToOriginMatrix = Matrix.Identity;
            Vector3.Transform(ArrayVertexPosition, ref TranslationToOriginMatrix, ArrayTransformedVertexPosition);
            for (int V = ArrayVertexPosition.Length - 1; V >= 0; --V)
            {
                ArrayVertex[V].Position = ArrayTransformedVertexPosition[V];
            }

            return new Tile3D(TilesetIndex, ArrayVertex, ArrayIndex);
        }

        public float GetZOffset(Vector2 PositionInTile, float Height)
        {
            switch (TerrainStyle)
            {
                case TerrainStyles.SlopeFrontToBack:
                    return Height * (PositionInTile.Y) - Height;

                case TerrainStyles.SlopeBackToFront:
                    break;

                case TerrainStyles.SlopeLeftToRight:
                    break;

                case TerrainStyles.SlopeRightToLeft:

                    return Height * (1 - PositionInTile.X) - Height;

                case TerrainStyles.WedgeFrontToBack:
                    break;

                case TerrainStyles.WedgeBackToFront:
                    break;

                case TerrainStyles.WedgeLeftToRight:
                    break;

                case TerrainStyles.WedgeRightToLeft:
                    return 1 - (1 - Height) * PositionInTile.X - Height;
            }

            return 0;
        }
    }
}
