﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public enum TerrainBonus { HPRegen, ENRegen, HPRestore, ENRestore, Armor, Accuracy, Evasion };//Regen = %, Restore = #.

    public enum TerrainActivation { OnEveryTurns, OnThisTurn, OnNextTurn, OnEnter, OnLeaved, OnAttack, OnHit, OnMiss, OnDefend, OnHited, OnMissed };

    public class DeathmatchTerrainBonusInfo
    {
        public TerrainActivation[] ListActivation;//Activation type of the bonuses.
        public TerrainBonus[] ListBonus;//Bonuses the terrain can give.
        public int[] ListBonusValue;//Value of the bonuses.
        public byte BattleBackgroundAnimationIndex;
        public byte BattleForegroundAnimationIndex;

        public DeathmatchTerrainBonusInfo()
        {
            ListActivation = new TerrainActivation[0];
            ListBonus = ListBonus = new TerrainBonus[0];
            ListBonusValue = ListBonusValue = new int[0];
            BattleBackgroundAnimationIndex = 0;
            BattleForegroundAnimationIndex = 0;
        }

        public DeathmatchTerrainBonusInfo(DeathmatchTerrainBonusInfo Other)
        {
            ListActivation = (TerrainActivation[])Other.ListActivation.Clone();
            ListBonus = (TerrainBonus[])Other.ListBonus.Clone();
            ListBonusValue = (int[])Other.ListBonusValue.Clone();
            BattleBackgroundAnimationIndex = Other.BattleBackgroundAnimationIndex;
            BattleForegroundAnimationIndex = Other.BattleForegroundAnimationIndex;
        }
    }

    public class Terrain : MovementAlgorithmTile
    {
        public class TilesetPreset
        {
            public enum TilesetTypes { Regular, Road, Water }

            public TilesetTypes TilesetType;
            public string TilesetName;
            public Terrain[,] ArrayTerrain;
            public DrawableTile[,] ArrayTiles;
            public List<string> ListBattleBackgroundAnimationPath;

            public TilesetPreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            {
                this.TilesetName = TilesetName;

                ArrayTerrain = new Terrain[TilesetWidth / TileSizeX, TilesetHeight / TileSizeY];
                ArrayTiles = new DrawableTile[ArrayTerrain.GetLength(0), ArrayTerrain.GetLength(1)];
                ListBattleBackgroundAnimationPath = new List<string>();

                //Tiles
                for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
                {
                    for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                    {
                        Terrain NewTerrain = new Terrain(X, Y, TileSizeX, TileSizeY, 0, 0, 0);
                        DrawableTile NewTile = new DrawableTile(new Rectangle(X * TileSizeX, Y * TileSizeY, TileSizeX, TileSizeY), TilesetIndex);
                        
                        NewTerrain.TerrainTypeIndex = 0;

                        ArrayTerrain[X, Y] = NewTerrain;
                        ArrayTiles[X, Y] = NewTile;
                    }
                }
            }

            public TilesetPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index, bool LoadBackgroundPaths = true)
            {
                TilesetName = BR.ReadString();
                TilesetType = (TilesetTypes)BR.ReadByte();

                ArrayTerrain = new Terrain[BR.ReadInt32(), BR.ReadInt32()];
                ArrayTiles = new DrawableTile[ArrayTerrain.GetLength(0), ArrayTerrain.GetLength(1)];

                //Tiles
                for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
                {
                    for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                    {
                        Terrain NewTerrain = ReadTerrain(BR, X, Y, 0, 0);
                        DrawableTile NewTile = new DrawableTile(new Rectangle(X * TileSizeX, Y * TileSizeY, TileSizeX, TileSizeY), Index);
                        ArrayTerrain[X, Y] = NewTerrain;
                        ArrayTiles[X, Y] = NewTile;
                    }
                }

                if (LoadBackgroundPaths)
                {
                    int ListBattleBackgroundAnimationPathCount = BR.ReadInt32();
                    ListBattleBackgroundAnimationPath = new List<string>(ListBattleBackgroundAnimationPathCount);
                    for (int B = 0; B < ListBattleBackgroundAnimationPathCount; B++)
                    {
                        ListBattleBackgroundAnimationPath.Add(BR.ReadString());
                    }
                }
            }

            protected virtual Terrain ReadTerrain(BinaryReader BR, int X, int Y, int LayerIndex, int LayerDepth)
            {
                return new Terrain(BR, X, Y, 1, 1, LayerIndex, 0, LayerDepth);
            }

            public void Write(BinaryWriter BW)
            {
                BW.Write(TilesetName);
                BW.Write((byte)TilesetType);

                BW.Write(ArrayTerrain.GetLength(0));
                BW.Write(ArrayTerrain.GetLength(1));

                //Tiles
                for (int Y = 0; Y < ArrayTerrain.GetLength(1); Y++)
                {
                    for (int X = 0; X < ArrayTerrain.GetLength(0); X++)
                    {
                        ArrayTerrain[X, Y].Save(BW);
                    }
                }
            }

            public static TilesetPreset FromFile(string FilePath, int Index = 0)
            {
                FileStream FS = new FileStream("Content/Maps/Tileset presets/" + FilePath + ".pet", FileMode.Open, FileAccess.Read);
                BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
                BR.BaseStream.Seek(0, SeekOrigin.Begin);

                int TileSizeX = BR.ReadInt32();
                int TileSizeY = BR.ReadInt32();

                TilesetPreset NewTilesetPreset = new TilesetPreset(BR, TileSizeX, TileSizeY, Index);

                BR.Close();
                FS.Close();

                return NewTilesetPreset;
            }

            internal void DrawPreview(SpriteBatch g, Point Position, Texture2D sprTileset)
            {
                switch(TilesetType)
                {
                    case TilesetTypes.Water:
                        g.Draw(sprTileset, new Rectangle(Position.X, Position.Y, ArrayTiles[0, 0].Origin.Width, ArrayTiles[0, 0].Origin.Height), Color.White);
                        break;
                }
                g.Draw(sprTileset, new Rectangle(Position.X, Position.Y, ArrayTiles[0, 0].Origin.Width, ArrayTiles[0, 0].Origin.Height), ArrayTiles[0, 0].Origin, Color.White);
            }

            public void UpdateSmartTile(int TilesetIndex, int X, int Y, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile)
            {
                switch (TilesetType)
                {
                    case TilesetTypes.Water:
                        UpdtateSmartTileWater(TilesetIndex, X, Y, TileSizeX, TileSizeY, ArrayTile);
                        break;
                }
                if (X > 0 && ArrayTile[X - 1, Y].TilesetIndex == TilesetIndex)
                {
                    switch (TilesetType)
                    {
                        case TilesetTypes.Water:
                            UpdtateSmartTileWater(TilesetIndex, X - 1, Y, TileSizeX, TileSizeY, ArrayTile);
                            break;
                    }
                }
                if (X < ArrayTile.GetLength(0) - 1 && ArrayTile[X + 1, Y].TilesetIndex == TilesetIndex)
                {
                    switch (TilesetType)
                    {
                        case TilesetTypes.Water:
                            UpdtateSmartTileWater(TilesetIndex, X + 1, Y, TileSizeX, TileSizeY, ArrayTile);
                            break;
                    }
                }
                if (Y > 0 && ArrayTile[X, Y - 1].TilesetIndex == TilesetIndex)
                {
                    switch (TilesetType)
                    {
                        case TilesetTypes.Water:
                            UpdtateSmartTileWater(TilesetIndex, X, Y - 1, TileSizeX, TileSizeY, ArrayTile);
                            break;
                    }

                    //Corners
                    if (X > 0 && ArrayTile[X - 1, Y - 1].TilesetIndex == TilesetIndex)
                    {
                        switch (TilesetType)
                        {
                            case TilesetTypes.Water:
                                UpdtateSmartTileWater(TilesetIndex, X - 1, Y - 1, TileSizeX, TileSizeY, ArrayTile);
                                break;
                        }
                    }
                    if (X < ArrayTile.GetLength(0) - 1 && ArrayTile[X + 1, Y - 1].TilesetIndex == TilesetIndex)
                    {
                        switch (TilesetType)
                        {
                            case TilesetTypes.Water:
                                UpdtateSmartTileWater(TilesetIndex, X + 1, Y - 1, TileSizeX, TileSizeY, ArrayTile);
                                break;
                        }
                    }
                }
                if (Y < ArrayTile.GetLength(1) - 1 && ArrayTile[X, Y + 1].TilesetIndex == TilesetIndex)
                {
                    switch (TilesetType)
                    {
                        case TilesetTypes.Water:
                            UpdtateSmartTileWater(TilesetIndex, X, Y + 1, TileSizeX, TileSizeY, ArrayTile);
                            break;
                    }

                    //Corners
                    if (X > 0 && ArrayTile[X - 1, Y + 1].TilesetIndex == TilesetIndex)
                    {
                        switch (TilesetType)
                        {
                            case TilesetTypes.Water:
                                UpdtateSmartTileWater(TilesetIndex, X - 1, Y + 1, TileSizeX, TileSizeY, ArrayTile);
                                break;
                        }
                    }
                    if (X < ArrayTile.GetLength(0) - 1 && ArrayTile[X + 1, Y + 1].TilesetIndex == TilesetIndex)
                    {
                        switch (TilesetType)
                        {
                            case TilesetTypes.Water:
                                UpdtateSmartTileWater(TilesetIndex, X + 1, Y + 1, TileSizeX, TileSizeY, ArrayTile);
                                break;
                        }
                    }
                }
            }

            private void UpdtateSmartTileWater(int TilesetIndex, int X, int Y, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile)
            {
                DrawableTile LeftTile = new DrawableTile(Rectangle.Empty, -1);
                DrawableTile UpTile = new DrawableTile(Rectangle.Empty, -1);
                DrawableTile RightTile = new DrawableTile(Rectangle.Empty, -1);
                DrawableTile DownTile = new DrawableTile(Rectangle.Empty, -1);

                ArrayTile[X, Y].ArraySubTile = new Rectangle[0];

                if (X > 0)
                {
                    LeftTile = ArrayTile[X - 1, Y];
                }
                if (Y > 0)
                {
                    UpTile = ArrayTile[X, Y - 1];
                }
                if (X < ArrayTile.GetLength(0) - 1)
                {
                    RightTile = ArrayTile[X + 1, Y];
                }
                if (Y < ArrayTile.GetLength(1) - 1)
                {
                    DownTile = ArrayTile[X, Y + 1];
                }

                if (TilesetIndex == LeftTile.TilesetIndex)
                {
                    if (TilesetIndex == UpTile.TilesetIndex)
                    {
                        if (TilesetIndex == RightTile.TilesetIndex)
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X - 1, Y - 1].TilesetIndex || TilesetIndex != ArrayTile[X + 1, Y - 1].TilesetIndex || TilesetIndex != ArrayTile[X - 1, Y + 1].TilesetIndex || TilesetIndex != ArrayTile[X + 1, Y + 1].TilesetIndex)
                                {
                                    Rectangle TopLeft = new Rectangle(2 * TileSizeX, 0 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomLeft = new Rectangle(2 * TileSizeX, TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2,  TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    bool Split = false;

                                    if (TilesetIndex == ArrayTile[X - 1, Y - 1].TilesetIndex)
                                    {
                                        Split = true;
                                        TopLeft = new Rectangle(1 * TileSizeX, 2 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X + 1, Y - 1].TilesetIndex)
                                    {
                                        Split = true;
                                        TopRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X - 1, Y + 1].TilesetIndex)
                                    {
                                        Split = true;
                                        BottomLeft = new Rectangle(1 * TileSizeX, 2 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X + 1, Y + 1].TilesetIndex)
                                    {
                                        Split = true;
                                        BottomRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (Split)
                                    {
                                        ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                        {
                                            TopLeft, TopRight, BottomLeft, BottomRight
                                        };
                                    }
                                    else
                                    {
                                        ArrayTile[X, Y].Origin.Location = new Point(2 * TileSizeX, 0 * TileSizeY);
                                    }
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(1 * TileSizeX, 2 * TileSizeY);
                                }
                            }
                            else//Nothing Down
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X - 1, Y - 1].TilesetIndex || TilesetIndex != ArrayTile[X + 1, Y - 1].TilesetIndex)
                                {
                                    Rectangle TopLeft =     new Rectangle(2 * TileSizeX,                    0 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle TopRight =    new Rectangle(2 * TileSizeX + TileSizeX / 2,    0 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomLeft =  new Rectangle(1 * TileSizeX,                    3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomRight = new Rectangle(1 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);

                                    if (TilesetIndex == ArrayTile[X - 1, Y - 1].TilesetIndex)
                                    {
                                        TopLeft = new Rectangle(1 * TileSizeX,                      3 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X + 1, Y - 1].TilesetIndex)
                                    {
                                        TopRight = new Rectangle(1 * TileSizeX + TileSizeX / 2,     3 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    }

                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(1 * TileSizeX, 3 * TileSizeY);
                                }
                            }
                        }
                        else//Nothing Right
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X - 1, Y - 1].TilesetIndex || TilesetIndex != ArrayTile[X - 1, Y + 1].TilesetIndex)
                                {
                                    Rectangle TopLeft =     new Rectangle(2 * TileSizeX,                    0 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle TopRight =    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomLeft =  new Rectangle(2 * TileSizeX,                    0 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);

                                    if (TilesetIndex == ArrayTile[X - 1, Y - 1].TilesetIndex)
                                    {
                                        TopLeft = new Rectangle(2 * TileSizeX,              2 * TileSizeY,                  TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X - 1, Y + 1].TilesetIndex)
                                    {
                                        BottomLeft = new Rectangle(2 * TileSizeX,           2 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2);
                                    }

                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(2 * TileSizeX, 2 * TileSizeY);
                                }
                            }
                            else//Nothing Down
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X - 1, Y - 1].TilesetIndex)
                                {
                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                    {
                                            new Rectangle(2 * TileSizeX,                    0 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX,                    3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(2 * TileSizeX, 3 * TileSizeY);
                                }
                            }
                        }
                    }
                    else//Nothing Up
                    {
                        if (TilesetIndex == RightTile.TilesetIndex)
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X - 1, Y + 1].TilesetIndex || TilesetIndex != ArrayTile[X + 1, Y + 1].TilesetIndex)
                                {
                                    Rectangle TopLeft =     new Rectangle(1 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle TopRight =    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomLeft =  new Rectangle(2 * TileSizeX,                    0 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2,    0 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);

                                    if (TilesetIndex == ArrayTile[X - 1, Y + 1].TilesetIndex)
                                    {
                                        BottomLeft = new Rectangle(1 * TileSizeX,                       1 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X + 1, Y + 1].TilesetIndex)
                                    {
                                        BottomRight = new Rectangle(1 * TileSizeX + TileSizeX / 2,      1 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2);
                                    }

                                    ArrayTile[X, Y].Origin.Location = new Point(1 * TileSizeX, 1 * TileSizeY);
                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(1 * TileSizeX, 1 * TileSizeY);
                                }
                            }
                            else//Nothing Down
                            {
                                ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,                  TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,                  TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(1 * TileSizeX,                    3 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2),
                                };
                            }
                        }
                        else//Nothing Right
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X - 1, Y + 1].TilesetIndex)
                                {
                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                    {
                                            new Rectangle(2 * TileSizeX, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX, 0 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2, 1 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(2 * TileSizeX, 1 * TileSizeY);
                                }
                            }
                            else
                            {
                                ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                {
                                    new Rectangle(2 * TileSizeX, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                };
                            }
                        }
                    }
                }
                else//Nothing Left
                {
                    if (TilesetIndex == RightTile.TilesetIndex)
                    {
                        if (TilesetIndex == UpTile.TilesetIndex)
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X + 1, Y - 1].TilesetIndex || TilesetIndex != ArrayTile[X + 1, Y + 1].TilesetIndex)
                                {
                                    Rectangle TopLeft =     new Rectangle(0 * TileSizeX,                    2 * TileSizeY,                  TileSizeX / 2, TileSizeY / 2);
                                    Rectangle TopRight =    new Rectangle(2 * TileSizeX + TileSizeX / 2,    0 * TileSizeY,                  TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomLeft =  new Rectangle(0 * TileSizeX,                    2 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2);
                                    Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2,    0 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2);

                                    if (TilesetIndex == ArrayTile[X + 1, Y - 1].TilesetIndex)
                                    {
                                        TopRight = new Rectangle(0 * TileSizeX + TileSizeX / 2,     2 * TileSizeY ,                 TileSizeX / 2, TileSizeY / 2);
                                    }
                                    if (TilesetIndex == ArrayTile[X + 1, Y + 1].TilesetIndex)
                                    {
                                        BottomRight = new Rectangle(0 * TileSizeX + TileSizeX / 2,  2 * TileSizeY + TileSizeY / 2,  TileSizeX / 2, TileSizeY / 2);
                                    }

                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(0, 2 * TileSizeY);
                                }
                            }
                            else//Nothing Down
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X + 1, Y - 1].TilesetIndex)
                                {
                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                    {
                                            new Rectangle(0 * TileSizeX, 3 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(0 * TileSizeX, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(0 * TileSizeX + TileSizeX / 2, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(0, 3 * TileSizeY);
                                }
                            }
                        }
                        else//Nothing Up
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                //Corners
                                if (TilesetIndex != ArrayTile[X + 1, Y + 1].TilesetIndex)
                                {
                                    ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                    {
                                            new Rectangle(0 * TileSizeX, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(0 * TileSizeX + TileSizeX / 2, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(0 * TileSizeX, 1 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    };
                                }
                                else
                                {
                                    ArrayTile[X, Y].Origin.Location = new Point(0, 1 * TileSizeY);
                                }
                            }
                            else
                            {
                                ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                {
                                    new Rectangle(0 * TileSizeX, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(0 * TileSizeX, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                };
                            }
                        }
                    }
                    else//Nothing Right
                    {
                        if (TilesetIndex == UpTile.TilesetIndex)
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                {
                                    new Rectangle(0 * TileSizeX, 2 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(0 * TileSizeX, 2 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                };
                            }
                            else
                            {
                                ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                {
                                    new Rectangle(0 * TileSizeX, 3 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 3 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(0 * TileSizeX, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 3 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                };
                            }
                        }
                        else//Nothing Up
                        {
                            if (TilesetIndex == DownTile.TilesetIndex)
                            {
                                ArrayTile[X, Y].ArraySubTile = new Rectangle[]
                                {
                                    new Rectangle(0 * TileSizeX, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 1 * TileSizeY, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(0 * TileSizeX, 1 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2, 1 * TileSizeY + TileSizeY / 2, TileSizeX / 2, TileSizeY / 2),
                                };
                            }
                            else
                            {
                            }
                        }
                    }
                }
            }
        }

        public DeathmatchTerrainBonusInfo BonusInfo;

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public Terrain(Terrain Other, Point Position, int LayerIndex)
            : base(Position.X, Position.Y, LayerIndex, Other.LayerDepth)
        {
            this.Height = Other.Height;
            this.WorldPosition = Other.WorldPosition;
            this.TerrainTypeIndex = Other.TerrainTypeIndex;
            this.PreventLeavingUpward = Other.PreventLeavingUpward;
            this.PreventLeavingDownward = Other.PreventLeavingDownward;
            this.PreventLeavingLeft = Other.PreventLeavingLeft;
            this.PreventLeavingRight = Other.PreventLeavingRight;
            BonusInfo = new DeathmatchTerrainBonusInfo(Other.BonusInfo);
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public Terrain(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(XPos, YPos, LayerIndex, LayerDepth)
        {
            this.WorldPosition = new Vector3(XPos * TileSizeX, YPos * TileSizeY, LayerIndex * LayerHeight);
            this.TerrainTypeIndex = 0;
            BonusInfo = new DeathmatchTerrainBonusInfo();
        }

        /// <summary>
        /// Create a new Tile with the index of the tile set and it's origin in it along with it's attribtues.
        /// </summary>
        /// <param name="TileSet">Which Tile Set to use from the ListTile.</param>
        /// <param name="Origin">X, Y origin from at which the tile is located in the TileSet.</param>
        /// <param name="TerrainTypeIndex">What kind of terrain it is.</param>
        /// <param name="MVEnterCost">How much energy is required to enter in it.</param>
        /// <param name="MVMoveCost">How much energy is required to move in it.</param>
        /// <param name="ListActivation">Activation type of the bonuses.</param>
        /// <param name="ListBonus">Bonuses the terrain can give.</param>
        /// <param name="ListBonusValue">//Value of the bonuses.</param>
        public Terrain(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex,
            TerrainActivation[] ListActivation, TerrainBonus[] ListBonus, int[] ListBonusValue)
            : this(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
            BonusInfo.ListActivation = ListActivation;
            BonusInfo.ListBonus = ListBonus;
            BonusInfo.ListBonusValue = ListBonusValue;
        }

        public Terrain(BinaryReader BR, int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : this(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            TerrainTypeIndex = BR.ReadByte();
            Height = BR.ReadSingle();

            PreventLeavingUpward = BR.ReadBoolean();
            PreventLeavingDownward = BR.ReadBoolean();
            PreventLeavingLeft = BR.ReadBoolean();
            PreventLeavingRight = BR.ReadBoolean();


            int ArrayBonusLength = BR.ReadInt32();
            BonusInfo.ListBonus = new TerrainBonus[ArrayBonusLength];
            BonusInfo.ListActivation = new TerrainActivation[ArrayBonusLength];
            BonusInfo.ListBonusValue = new int[ArrayBonusLength];

            for (int i = 0; i < ArrayBonusLength; i++)
            {
                BonusInfo.ListBonus[i] = (TerrainBonus)BR.ReadInt32();
                BonusInfo.ListActivation[i] = (TerrainActivation)BR.ReadInt32();
                BonusInfo.ListBonusValue[i] = BR.ReadInt32();
            }

            BonusInfo.BattleBackgroundAnimationIndex = BR.ReadByte();
            BonusInfo.BattleForegroundAnimationIndex = BR.ReadByte();
        }

        public virtual void Save(BinaryWriter BW)
        {
            BW.Write(TerrainTypeIndex);
            BW.Write(Height);

            BW.Write(PreventLeavingUpward);
            BW.Write(PreventLeavingDownward);
            BW.Write(PreventLeavingLeft);
            BW.Write(PreventLeavingRight);

            BW.Write(BonusInfo.ListBonus.Length);
            for (int i = 0; i < BonusInfo.ListBonus.Length; i++)
            {
                BW.Write((int)BonusInfo.ListBonus[i]);
                BW.Write((int)BonusInfo.ListActivation[i]);
                BW.Write(BonusInfo.ListBonusValue[i]);
            }

            BW.Write(BonusInfo.BattleBackgroundAnimationIndex);
            BW.Write(BonusInfo.BattleForegroundAnimationIndex);
        }
    }
}
