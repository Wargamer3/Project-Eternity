using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class TilesetPreset
    {
        public enum TilesetTypes { Regular, Road, River, Ocean, Pipes, Bridge, Shoal, Waterfall, Slave /*Only used during loading to fill the tilesets created by multi tileset presets*/ }

        public string RelativePath;
        public TilesetTypes TilesetType;
        public TilesetPreset Master;
        public int SlaveIndex;
        public TilesetPresetInformation[] ArrayTilesetInformation;
        private int AnimationFrames => ArrayTilesetInformation[0].AnimationFrames;
        public List<string> ListBattleBackgroundAnimationPath;

        private TilesetPreset(TilesetPreset Clone, int Index)
        {
            Master = Clone;
            SlaveIndex = Index;
            ArrayTilesetInformation = new TilesetPresetInformation[1];
            ArrayTilesetInformation[0] = Clone.ArrayTilesetInformation[Index];
            ListBattleBackgroundAnimationPath = new List<string>();
        }

        public TilesetPreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            ArrayTilesetInformation = new TilesetPresetInformation[1];
            ArrayTilesetInformation[0] = CreateTerrain(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            ListBattleBackgroundAnimationPath = new List<string>();
        }

        public TilesetPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex, bool LoadBackgroundPaths = true)
        {
            TilesetType = (TilesetTypes)BR.ReadByte();
            byte ArrayTilesetInformationLength = BR.ReadByte();
            ArrayTilesetInformation = new TilesetPresetInformation[ArrayTilesetInformationLength];

            for (byte i = 0; i < ArrayTilesetInformationLength; ++i)
            {
                ArrayTilesetInformation[i] = ReadTerrain(BR, TileSizeX, TileSizeY, TilesetIndex + i);
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

        public virtual TilesetPreset CreateSlave(int Index)
        {
            TilesetPreset Slave = new TilesetPreset(this, Index);
            Slave.TilesetType = TilesetTypes.Slave;

            return Slave;
        }

        public int GetAnimationFrames()
        {
            if (TilesetType == TilesetTypes.River)
            {
                return 4;
            }

            return 0;
        }

        protected virtual TilesetPresetInformation CreateTerrain(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new TilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected virtual TilesetPresetInformation ReadTerrain(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new TilesetPresetInformation(BR, TileSizeX, TileSizeY, TilesetIndex);
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write((byte)TilesetType);
            BW.Write((byte)ArrayTilesetInformation.Length);
            for (int i = 0; i < ArrayTilesetInformation.Length; ++i)
            {
                ArrayTilesetInformation[i].Write(BW);
            }
        }

        public static TilesetPreset FromFile(string Folder, string FilePath, int TilesetIndex = 0)
        {
            FileStream FS = new FileStream("Content/" + Folder + "/Tilesets presets/" + FilePath + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int TileSizeX = BR.ReadInt32();
            int TileSizeY = BR.ReadInt32();

            TilesetPreset NewTilesetPreset = new TilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);
            NewTilesetPreset.RelativePath = FilePath;

            BR.Close();
            FS.Close();

            return NewTilesetPreset;
        }

        public void DrawPreview(SpriteBatch g, Point Position, Texture2D sprTileset)
        {
            if (sprTileset == null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(Position.X, Position.Y, 32, 32), null, Color.Red);
                return;
            }
            switch (TilesetType)
            {
                case TilesetTypes.River:
                    g.Draw(sprTileset, new Rectangle(Position.X, Position.Y, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Width, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Height), Color.White);
                    break;
            }
            g.Draw(sprTileset, new Rectangle(Position.X, Position.Y, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Width, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Height), ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin, Color.White);
        }

        public void UpdateAutotTile(DrawableTile NewTile, Terrain CurrentTerrain, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            List<int> Whitelist = ListTilesetPreset[NewTile.TilesetIndex].ArrayTilesetInformation[0].ListAllowedTerrainTypeIndex;

            if (Whitelist.Count > 0 && !Whitelist.Contains(CurrentTerrain.TerrainTypeIndex))
            {
                return;
            }

            TilesetTypes TilesetType = ListTilesetPreset[NewTile.TilesetIndex].TilesetType;

            UpdateAutotTileParse(TilesetType, NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);

            //Left
            if (GridX > 0 && ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, GridX - 1, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
            }

            //Right
            if (GridX < ArrayTile.GetLength(0) - 1 && ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, GridX + 1, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
            }

            //Up
            if (GridY > 0 && ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, GridX, GridY - 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);

                //Corner Up Left
                if (GridX > 0 && ListTilesetPreset[ArrayTile[GridX - 1, GridY - 1].TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, GridX - 1, GridY - 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                }
                //Corner Up Right
                if (GridX < ArrayTile.GetLength(0) - 1 && ListTilesetPreset[ArrayTile[GridX + 1, GridY - 1].TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, GridX + 1, GridY - 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                }
            }

            //Down
            if (GridY < ArrayTile.GetLength(1) - 1 && ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, GridX, GridY + 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);

                //Corner Down Left
                if (GridX > 0 && ListTilesetPreset[ArrayTile[GridX - 1, GridY + 1].TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, GridX - 1, GridY + 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                }
                //Corner Down Right
                if (GridX < ArrayTile.GetLength(0) - 1 && ListTilesetPreset[ArrayTile[GridX + 1, GridY + 1].TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, GridX + 1, GridY + 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                }
            }
        }

        public void UpdateAutotTileParse(TilesetTypes TilesetType, DrawableTile NewTile, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            TilesetPreset CurrentPreset = ListTilesetPreset[NewTile.TilesetIndex];

            if (TilesetType == TilesetTypes.Slave)
            {
                TilesetTypes MasterType = CurrentPreset.Master.TilesetType;
                if (MasterType == TilesetTypes.Road)
                {
                    switch (CurrentPreset.SlaveIndex)
                    {
                        case 1:
                            TilesetType = TilesetTypes.Bridge;//Not placed manually, never used
                            break;
                    }
                }
                else if (MasterType == TilesetTypes.Ocean)
                {
                    switch (CurrentPreset.SlaveIndex)
                    {
                        case 1:
                            TilesetType = TilesetTypes.River;
                            break;
                        case 2:
                            TilesetType = TilesetTypes.Shoal;
                            break;
                        case 3:
                            TilesetType = TilesetTypes.Waterfall;//Not placed manually, never used
                            break;
                    }
                }
            }

            switch (TilesetType)
            {
                case TilesetTypes.Road:
                case TilesetTypes.Bridge:
                    UpdtateSmartTileRoad(NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                    break;

                case TilesetTypes.Ocean:
                    ArrayTile[GridX, GridY] = NewTile;
                    UpdtateSmartTileWater(NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                    UpdtateSmartTileWaterfall(NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                    break;

                case TilesetTypes.River:
                    UpdtateSmartTileRiver(NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                    UpdtateSmartTileWaterfall(NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                    break;

                case TilesetTypes.Shoal:
                    UpdateSmartTileShoal(NewTile, GridX, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
                    break;
            }
        }

        private void UpdtateSmartTileRoad(DrawableTile NewTile, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            TilesetPreset Self = ListTilesetPreset[NewTile.TilesetIndex];
            TilesetPreset Master = ListTilesetPreset[NewTile.TilesetIndex].Master;

            bool LeftTileValid = false;
            bool UpTileValid = false;
            bool RightTileValid = false;
            bool DownTileValid = false;

            bool UpLeftTileValid = false;
            bool UpRightTileValid = false;
            bool DownLeftTileValid = false;
            bool DownRightTileValid = false;

            int TopHalf = TileSizeY - TileSizeY / 2;
            int BottomHalf = TileSizeY - TopHalf;

            if (GridX > 0)
            {
                LeftTileValid = ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex] == Self || (Master == ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].SlaveIndex == 1);
            }
            if (GridY > 0)
            {
                UpTileValid = ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex] == Self || (Master == ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].SlaveIndex == 1);
            }
            if (GridX < ArrayTile.GetLength(0) - 1)
            {
                RightTileValid = ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex] == Self || (Master == ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].SlaveIndex == 1);
            }
            if (GridY < ArrayTile.GetLength(1) - 1)
            {
                DownTileValid = ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex] == Self || (Master == ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].SlaveIndex == 1);
            }

            //No corners allowed
            if ((UpTileValid && LeftTileValid && UpLeftTileValid)
                || (UpTileValid && RightTileValid && UpRightTileValid)
                || (DownTileValid && LeftTileValid && DownLeftTileValid)
                || (DownTileValid && RightTileValid && DownRightTileValid))
            {
                return;
            }

            ArrayTile[GridX, GridY] = NewTile;

            if (LeftTileValid)
            {
                if (UpTileValid)
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0 * TileSizeX, 0 * TileSizeY);
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    0 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,    0 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Right
                    {
                        if (DownTileValid)
                        {
                            Rectangle TopLeft = new Rectangle(0 * TileSizeX, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                            Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                            Rectangle BottomLeft = new Rectangle(0 * TileSizeX, 0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                            Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);

                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 3 * TileSizeY);
                        }
                    }
                }
                else//Nothing Up
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    0 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,    0 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,                  TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,                  TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX,                    3 * TileSizeY + TopHalf,  TileSizeX / 2, BottomHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf,  TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Right
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 1 * TileSizeY);
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(2 * TileSizeX,                        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX,                        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
            }
            else//Nothing Left
            {
                if (RightTileValid)
                {
                    if (UpTileValid)
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0, 3 * TileSizeY);
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0, 1 * TileSizeY);
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                            1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,            1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                            3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,            3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
                else//Nothing Right
                {
                    if (UpTileValid)//Something Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0 * TileSizeX, 0 * TileSizeY);
                        }
                    }
                }
            }
        }

        private void UpdtateSmartTileWater(DrawableTile NewTile, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            TilesetTypes TilesetType = ListTilesetPreset[NewTile.TilesetIndex].TilesetType;

            bool LeftTileValid = true;
            bool UpTileValid = true;
            bool RightTileValid = true;
            bool DownTileValid = true;

            bool UpLeftTileValid = true;
            bool UpRightTileValid = true;
            bool DownLeftTileValid = true;
            bool DownRightTileValid = true;

            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[0];

            int TopHalf = TileSizeY - TileSizeY / 4;
            int BottomHalf = TileSizeY - TopHalf;

            if (GridX > 0)
            {
                LeftTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX - 1, GridY].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX - 1, GridY].TilesetIndex);
            }
            if (GridY > 0)
            {
                UpTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX, GridY - 1].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX, GridY - 1].TilesetIndex);

                if (GridX > 0)
                {
                    UpLeftTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX - 1, GridY - 1].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY - 1].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX - 1, GridY - 1].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY - 1].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX - 1, GridY - 1].TilesetIndex);
                }
                if (GridX < ArrayTile.GetLength(0) - 1)
                {
                    UpRightTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX + 1, GridY - 1].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX + 1, GridY - 1].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX + 1, GridY - 1].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX + 1, GridY - 1].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX + 1, GridY - 1].TilesetIndex);
                }
            }
            if (GridX < ArrayTile.GetLength(0) - 1)
            {
                RightTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].TilesetType
                     || (ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX + 1, GridY].TilesetIndex)
                     || (ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX + 1, GridY].TilesetIndex);
            }
            if (GridY < ArrayTile.GetLength(1) - 1)
            {
                DownTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX, GridY + 1].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX, GridY + 1].TilesetIndex);

                if (GridX > 0)
                {
                    DownLeftTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX - 1, GridY + 1].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY + 1].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX - 1, GridY + 1].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY + 1].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX - 1, GridY + 1].TilesetIndex);
                }
                if (GridX < ArrayTile.GetLength(0) - 1)
                {
                    DownRightTileValid = TilesetType == ListTilesetPreset[ArrayTile[GridX + 1, GridY + 1].TilesetIndex].TilesetType
                    || (ListTilesetPreset[ArrayTile[GridX + 1, GridY + 1].TilesetIndex].SlaveIndex == 2 && NewTile.TilesetIndex + 2 == ArrayTile[GridX + 1, GridY + 1].TilesetIndex)
                    || (ListTilesetPreset[ArrayTile[GridX + 1, GridY + 1].TilesetIndex].SlaveIndex == 3 && NewTile.TilesetIndex + 3 == ArrayTile[GridX + 1, GridY + 1].TilesetIndex);
                }
            }

            if (LeftTileValid)
            {
                if (UpTileValid)
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            //Corners
                            if (!UpLeftTileValid || !UpRightTileValid || !DownLeftTileValid ||!DownRightTileValid)
                            {
                                Rectangle TopLeft = new Rectangle(2 * TileSizeX, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle BottomLeft = new Rectangle(2 * TileSizeX, TopHalf, TileSizeX / 2, BottomHalf);
                                Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, TopHalf, TileSizeX / 2, BottomHalf);
                                bool Split = false;

                                if (UpLeftTileValid)
                                {
                                    Split = true;
                                    TopLeft = new Rectangle(1 * TileSizeX, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                                }
                                if (UpRightTileValid)
                                {
                                    Split = true;
                                    TopRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                                }
                                if (DownLeftTileValid)
                                {
                                    Split = true;
                                    BottomLeft = new Rectangle(1 * TileSizeX, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                }
                                if (DownRightTileValid)
                                {
                                    Split = true;
                                    BottomRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                }
                                if (Split)
                                {
                                    ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                                    {
                                            TopLeft, TopRight, BottomLeft, BottomRight
                                    };
                                }
                                else
                                {
                                    ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 0 * TileSizeY);
                                }
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(1 * TileSizeX, 2 * TileSizeY);
                            }
                        }
                        else//Nothing Down
                        {
                            //Corners
                            if (!UpLeftTileValid || !UpRightTileValid)
                            {
                                Rectangle TopLeft = new Rectangle(2 * TileSizeX, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle BottomLeft = new Rectangle(1 * TileSizeX, 3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                Rectangle BottomRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);

                                if (UpLeftTileValid)
                                {
                                    TopLeft = new Rectangle(1 * TileSizeX, 3 * TileSizeY, TileSizeX / 2, TopHalf);
                                }
                                if (UpRightTileValid)
                                {
                                    TopRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 3 * TileSizeY, TileSizeX / 2, TopHalf);
                                }

                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(1 * TileSizeX, 3 * TileSizeY);
                            }
                        }
                    }
                    else//Nothing Right
                    {
                        if (DownTileValid)
                        {
                            //Corners
                            if (!UpLeftTileValid || !DownLeftTileValid)
                            {
                                Rectangle TopLeft = new Rectangle(2 * TileSizeX, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle BottomLeft = new Rectangle(2 * TileSizeX, 0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);

                                if (UpLeftTileValid)
                                {
                                    TopLeft = new Rectangle(2 * TileSizeX, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                                }
                                if (DownLeftTileValid)
                                {
                                    BottomLeft = new Rectangle(2 * TileSizeX, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                }

                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 2 * TileSizeY);
                            }
                        }
                        else//Nothing Down
                        {
                            //Corners
                            if (!UpLeftTileValid)
                            {
                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                                {
                                            new Rectangle(2 * TileSizeX,                    0 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(2 * TileSizeX,                    3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 3 * TileSizeY);
                            }
                        }
                    }
                }
                else//Nothing Up
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            //Corners
                            if (!DownLeftTileValid ||!DownRightTileValid)
                            {
                                Rectangle TopLeft = new Rectangle(1 * TileSizeX, 1 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle TopRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 1 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle BottomLeft = new Rectangle(2 * TileSizeX, 0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);

                                if (DownLeftTileValid)
                                {
                                    BottomLeft = new Rectangle(1 * TileSizeX, 1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                }
                                if (DownRightTileValid)
                                {
                                    BottomRight = new Rectangle(1 * TileSizeX + TileSizeX / 2, 1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                }

                                ArrayTile[GridX, GridY].Origin.Location = new Point(1 * TileSizeX, 1 * TileSizeY);
                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(1 * TileSizeX, 1 * TileSizeY);
                            }
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,                  TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,                  TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX,                    3 * TileSizeY + TopHalf,  TileSizeX / 2, BottomHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf,  TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Right
                    {
                        if (DownTileValid)
                        {
                            //Corners
                            if (!DownLeftTileValid)
                            {
                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                                {
                                            new Rectangle(2 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(2 * TileSizeX,                    0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 1 * TileSizeY);
                            }
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(2 * TileSizeX,                        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX,                        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
            }
            else//Nothing Left
            {
                if (RightTileValid)
                {
                    if (UpTileValid)
                    {
                        if (DownTileValid)
                        {
                            //Corners
                            if (!UpRightTileValid ||!DownRightTileValid)
                            {
                                Rectangle TopLeft = new Rectangle(0 * TileSizeX, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                                Rectangle BottomLeft = new Rectangle(0 * TileSizeX, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);

                                if (UpRightTileValid)
                                {
                                    TopRight = new Rectangle(0 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                                }
                                if (DownRightTileValid)
                                {
                                    BottomRight = new Rectangle(0 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                                }

                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(0, 2 * TileSizeY);
                            }
                        }
                        else//Nothing Down
                        {
                            //Corners
                            if (!UpRightTileValid)
                            {
                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                                {
                                            new Rectangle(0 * TileSizeX,                    3 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    0 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(0 * TileSizeX,                    3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                            new Rectangle(0 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(0, 3 * TileSizeY);
                            }
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            //Corners
                            if (!DownRightTileValid)
                            {
                                ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                                {
                                            new Rectangle(0 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(0 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                            new Rectangle(0 * TileSizeX,                    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                            new Rectangle(2 * TileSizeX + TileSizeX / 2,    0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                };
                            }
                            else
                            {
                                ArrayTile[GridX, GridY].Origin.Location = new Point(0, 1 * TileSizeY);
                            }
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                            1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,            1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                            3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,            3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
                else//Nothing Right
                {
                    if (UpTileValid)//Something Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        private void UpdtateSmartTileRiver(DrawableTile NewTile, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            TilesetTypes TilesetType = ListTilesetPreset[NewTile.TilesetIndex].TilesetType;
            TilesetPreset Master = ListTilesetPreset[NewTile.TilesetIndex].Master;

            bool LeftTileValid = false;
            bool UpTileValid = false;
            bool RightTileValid = false;
            bool DownTileValid = false;

            bool UpLeftTileValid = false;
            bool UpRightTileValid = false;
            bool DownLeftTileValid = false;
            bool DownRightTileValid = false;

            int TopHalf = TileSizeY - TileSizeY / 4;
            int BottomHalf = TileSizeY - TopHalf;

            if (GridX > 0)
            {
                LeftTileValid = (Master == ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].SlaveIndex == 1)
                    || (ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].SlaveIndex == 3 && (NewTile.TilesetIndex + 2 == ArrayTile[GridX - 1, GridY].TilesetIndex));
            }
            if (GridY > 0)
            {
                UpTileValid = Master == ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].SlaveIndex == 1
                    || (ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].SlaveIndex == 3 && (NewTile.TilesetIndex + 2 == ArrayTile[GridX, GridY - 1].TilesetIndex));

                if (GridX > 0)
                {
                    UpLeftTileValid = Master == ListTilesetPreset[ArrayTile[GridX - 1, GridY - 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX - 1, GridY - 1].TilesetIndex].SlaveIndex == 1;
                }
                if (GridX < ArrayTile.GetLength(0) - 1)
                {
                    UpRightTileValid = Master == ListTilesetPreset[ArrayTile[GridX + 1, GridY - 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX + 1, GridY - 1].TilesetIndex].SlaveIndex == 1;
                }
            }
            if (GridX < ArrayTile.GetLength(0) - 1)
            {
                RightTileValid = Master == ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].SlaveIndex == 1
                    || (ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].SlaveIndex == 3 && (NewTile.TilesetIndex + 2 == ArrayTile[GridX + 1, GridY].TilesetIndex));
            }
            if (GridY < ArrayTile.GetLength(1) - 1)
            {
                DownTileValid = Master == ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].SlaveIndex == 1
                    || (ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].SlaveIndex == 3 && (NewTile.TilesetIndex + 2 == ArrayTile[GridX, GridY + 1].TilesetIndex));

                if (GridX > 0)
                {
                    DownLeftTileValid = Master == ListTilesetPreset[ArrayTile[GridX - 1, GridY + 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX - 1, GridY + 1].TilesetIndex].SlaveIndex == 1;
                }
                if (GridX < ArrayTile.GetLength(0) - 1)
                {
                    DownRightTileValid = Master == ListTilesetPreset[ArrayTile[GridX + 1, GridY + 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX + 1, GridY + 1].TilesetIndex].SlaveIndex == 1;
                }
            }

            //No corners allowed
            if ((UpTileValid && LeftTileValid && UpLeftTileValid)
                || (UpTileValid && RightTileValid && UpRightTileValid)
                || (DownTileValid && LeftTileValid && DownLeftTileValid)
                || (DownTileValid && RightTileValid && DownRightTileValid))
            {
                return;
            }

            ArrayTile[GridX, GridY] = NewTile;

            if (LeftTileValid)
            {
                if (UpTileValid)
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0 * TileSizeX, 0 * TileSizeY);
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    0 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,    0 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Right
                    {
                        if (DownTileValid)
                        {
                            Rectangle TopLeft = new Rectangle(0 * TileSizeX, 0 * TileSizeY, TileSizeX / 2, TopHalf);
                            Rectangle TopRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY, TileSizeX / 2, TopHalf);
                            Rectangle BottomLeft = new Rectangle(0 * TileSizeX, 0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);
                            Rectangle BottomRight = new Rectangle(2 * TileSizeX + TileSizeX / 2, 2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf);

                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 3 * TileSizeY);
                        }
                    }
                }
                else//Nothing Up
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    0 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,    0 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,                  TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,                  TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX,                    3 * TileSizeY + TopHalf,  TileSizeX / 2, BottomHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf,  TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Right
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 1 * TileSizeY);
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(2 * TileSizeX,                        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX,                        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,        3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
            }
            else//Nothing Left
            {
                if (RightTileValid)
                {
                    if (UpTileValid)
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                        2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,        0 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                        2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,        0 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else//Nothing Down
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0, 3 * TileSizeY);
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].Origin.Location = new Point(0, 1 * TileSizeY);
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                            1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,            1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                            3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,            3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
                else//Nothing Right
                {
                    if (UpTileValid)//Something Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            ArrayTile[GridX, GridY].ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    3 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                    }
                }
            }
        }

        private void UpdateSmartTileShoal(DrawableTile NewTile, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            if (ListTilesetPreset[ArrayTile[GridX, GridY].TilesetIndex] == ListTilesetPreset[NewTile.TilesetIndex].Master)//Ocean type master
            {
                DrawableTile MasterTile = ArrayTile[GridX, GridY];
                //Left Size
                if (MasterTile.Origin.X != TileSizeX * 1 || MasterTile.Origin.Y != TileSizeY * 2)
                {
                    ArrayTile[GridX, GridY] = NewTile;
                    ArrayTile[GridX, GridY].Origin = MasterTile.Origin;
                    ArrayTile[GridX, GridY].ArraySubTile = MasterTile.ArraySubTile;
                }
            }
        }

        private void UpdtateSmartTileWaterfall(DrawableTile NewTile, int GridX, int GridY, int TileSizeX, int TileSizeY, DrawableTile[,] ArrayTile, List<TilesetPreset> ListTilesetPreset)
        {
            //River type
            bool SelfTileValid = ListTilesetPreset[ArrayTile[GridX, GridY].TilesetIndex].TilesetType == TilesetTypes.Slave && ListTilesetPreset[ArrayTile[GridX, GridY].TilesetIndex].SlaveIndex == 1;

            if (!SelfTileValid)
            {
                return;
            }

            TilesetPreset Master = ListTilesetPreset[ArrayTile[GridX, GridY].TilesetIndex].Master;
            NewTile = Master.ArrayTilesetInformation[3].ArrayTiles[0, 0];

            bool LeftRiverTileValid = false;
            bool UpRiverTileValid = false;
            bool RightRiverTileValid = false;
            bool DownRiverTileValid = false;

            bool LeftOceanTileValid = false;
            bool UpOceanTileValid = false;
            bool RightOceanTileValid = false;
            bool DownOceanTileValid = false;

            //Find River tile behind
            if (GridX > 0)
            {
                LeftRiverTileValid = Master == ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex].SlaveIndex == 1;
            }
            if (GridY > 0)
            {
                UpRiverTileValid = Master == ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex].SlaveIndex == 1;
            }
            if (GridX < ArrayTile.GetLength(0) - 1)
            {
                RightRiverTileValid = Master == ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex].SlaveIndex == 1;
            }
            if (GridY < ArrayTile.GetLength(1) - 1)
            {
                DownRiverTileValid = Master == ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].Master && ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex].SlaveIndex == 1;
            }

            //Find Ocean tile
            if (GridX > 0)
            {
                LeftOceanTileValid = Master == ListTilesetPreset[ArrayTile[GridX - 1, GridY].TilesetIndex];
            }
            if (GridY > 0)
            {
                UpOceanTileValid = Master == ListTilesetPreset[ArrayTile[GridX, GridY - 1].TilesetIndex];
            }
            if (GridX < ArrayTile.GetLength(0) - 1)
            {
                RightOceanTileValid = Master == ListTilesetPreset[ArrayTile[GridX + 1, GridY].TilesetIndex];
            }
            if (GridY < ArrayTile.GetLength(1) - 1)
            {
                DownOceanTileValid = Master == ListTilesetPreset[ArrayTile[GridX, GridY + 1].TilesetIndex];
            }

            if (LeftRiverTileValid && RightOceanTileValid && UpOceanTileValid && DownOceanTileValid)
            {
                ArrayTile[GridX, GridY] = NewTile;
                ArrayTile[GridX, GridY].Origin.Location = new Point(2 * TileSizeX, 2 * TileSizeY);
                DrawableTile NewOceanTile = Master.ArrayTilesetInformation[0].ArrayTiles[0, 0];
                UpdtateSmartTileWater(NewOceanTile, GridX + 1, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
            }
            else if (UpRiverTileValid && DownOceanTileValid && LeftOceanTileValid && RightOceanTileValid)
            {
                ArrayTile[GridX, GridY] = NewTile;
                ArrayTile[GridX, GridY].Origin.Location = new Point(1 * TileSizeX, 3 * TileSizeY);
                DrawableTile NewOceanTile = Master.ArrayTilesetInformation[0].ArrayTiles[0, 0];
                UpdtateSmartTileWater(NewOceanTile, GridX, GridY + 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
            }
            else if (RightRiverTileValid && LeftOceanTileValid && UpOceanTileValid && DownOceanTileValid)
            {
                ArrayTile[GridX, GridY] = NewTile;
                ArrayTile[GridX, GridY].Origin.Location = new Point(0 * TileSizeX, 2 * TileSizeY);
                DrawableTile NewOceanTile = Master.ArrayTilesetInformation[0].ArrayTiles[0, 0];
                UpdtateSmartTileWater(NewOceanTile, GridX - 1, GridY, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
            }
            else if (DownRiverTileValid && UpOceanTileValid && LeftOceanTileValid && RightOceanTileValid)
            {
                ArrayTile[GridX, GridY] = NewTile;
                ArrayTile[GridX, GridY].Origin.Location = new Point(1 * TileSizeX, 1 * TileSizeY);
                DrawableTile NewOceanTile = Master.ArrayTilesetInformation[0].ArrayTiles[0, 0];
                UpdtateSmartTileWater(NewOceanTile, GridX, GridY - 1, TileSizeX, TileSizeY, ArrayTile, ListTilesetPreset);
            }
        }
    }
}
