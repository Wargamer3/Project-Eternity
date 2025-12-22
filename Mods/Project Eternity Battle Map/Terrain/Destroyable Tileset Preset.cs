using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Attacks;
using static ProjectEternity.Core.Attacks.DestructibleTilesAttackAttributes;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class DestructibleTilesetPreset
    {
        public DestructibleTilesAttackAttributes.DestructibleTypes TilesetType;
        public int HP;
        public DestructibleTilesetPreset Master;
        public int SlaveIndex;
        public TilesetPresetInformation[] ArrayTilesetInformation;
        public List<string> ListBattleBackgroundAnimationPath;
        public string RelativePath;
        private int AnimationFrames => ArrayTilesetInformation[0].AnimationFrames;

        protected DestructibleTilesetPreset(DestructibleTilesetPreset Clone, int Index)
        {
            Master = Clone;
            SlaveIndex = Index;
            ArrayTilesetInformation = new TilesetPresetInformation[1];
            ArrayTilesetInformation[0] = Clone.ArrayTilesetInformation[Index];
            ListBattleBackgroundAnimationPath = new List<string>();
        }

        public DestructibleTilesetPreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            ArrayTilesetInformation = new TilesetPresetInformation[1];
            ArrayTilesetInformation[0] = CreateTerrain(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
            ListBattleBackgroundAnimationPath = new List<string>();
        }

        public DestructibleTilesetPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex, bool LoadBackgroundPaths = true)
        {
            TilesetType = (DestructibleTilesAttackAttributes.DestructibleTypes)BR.ReadByte();
            HP = BR.ReadInt32();
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

        public virtual DestructibleTilesetPreset CreateSlave(int Index)
        {
            DestructibleTilesetPreset Slave = new DestructibleTilesetPreset(this, Index);
            Slave.TilesetType = DestructibleTilesAttackAttributes.DestructibleTypes.Slave;

            return Slave;
        }

        public int GetAnimationFrames()
        {
            return AnimationFrames;
        }

        protected virtual TilesetPresetInformation CreateTerrain(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new TilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected virtual TilesetPresetInformation ReadTerrain(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new TilesetPresetInformation(BR, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected virtual void CreateTerrain(int GridX, int GridY, int LayerIndex, DrawableTile ReplacementTile, Terrain ReplacementTerrain)
        {

        }

        protected virtual DrawableTile? GetTile(int GridX, int GridY, int LayerIndex, out bool IsOutOfBound)
        {
            throw new NotImplementedException();
        }

        protected virtual Terrain GetTerrain(int GridX, int GridY, int LayerIndex)
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter BW)
        {
            BW.Write((byte)TilesetType);
            BW.Write(HP);
            BW.Write((byte)ArrayTilesetInformation.Length);
            for (int i = 0; i < ArrayTilesetInformation.Length; ++i)
            {
                ArrayTilesetInformation[i].Write(BW);
            }
        }

        public static DestructibleTilesetPreset FromFile(string FilePath, string RelativePath, int TilesetIndex = 0)
        {
            FileStream FS = new FileStream("Content/" + FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int TileSizeX = BR.ReadInt32();
            int TileSizeY = BR.ReadInt32();

            DestructibleTilesetPreset NewTilesetPreset = new DestructibleTilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);
            NewTilesetPreset.RelativePath = RelativePath;

            BR.Close();
            FS.Close();

            return NewTilesetPreset;
        }

        internal void DrawPreview(SpriteBatch g, Point Position, Texture2D sprTileset)
        {
            if (sprTileset == null)
            {
                g.Draw(GameScreen.sprPixel, new Rectangle(Position.X, Position.Y, 32, 32), null, Color.Red);
                return;
            }
            switch (TilesetType)
            {
                case DestructibleTilesAttackAttributes.DestructibleTypes.River:
                    g.Draw(sprTileset, new Rectangle(Position.X, Position.Y, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Width, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Height), Color.White);
                    break;
            }
            g.Draw(sprTileset, new Rectangle(Position.X, Position.Y, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Width, ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin.Height), ArrayTilesetInformation[0].ArrayTiles[0, 0].Origin, Color.White);
        }

        public bool UpdateAutotTile(DrawableTile NewTile, Terrain NewTerrain, int GridX, int GridY, int LayerIndex, int TileSizeX, int TileSizeY, List<DestructibleTilesetPreset> ListTilesetPreset)
        {
            List<int> Whitelist = ListTilesetPreset[NewTile.TilesetIndex].ArrayTilesetInformation[0].ListAllowedTerrainTypeIndex;

            DestructibleTypes TilesetType = ListTilesetPreset[NewTile.TilesetIndex].TilesetType;

            bool TileWasReplaced = UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX, GridY, LayerIndex), GridX, GridY, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);

            DrawableTile? LeftTile = GetTile(GridX - 1, GridY, LayerIndex, out _);
            DrawableTile? UpTile = GetTile(GridX, GridY - 1, LayerIndex, out _);
            DrawableTile? RightTile = GetTile(GridX + 1, GridY, LayerIndex, out _);
            DrawableTile? DownTile = GetTile(GridX, GridY + 1, LayerIndex, out _);

            DrawableTile? UpLeftTile = GetTile(GridX - 1, GridY - 1, LayerIndex, out _);
            DrawableTile? UpRightTile = GetTile(GridX + 1, GridY - 1, LayerIndex, out _);
            DrawableTile? DownLeftTile = GetTile(GridX - 1, GridY + 1, LayerIndex, out _);
            DrawableTile? DownRightTile = GetTile(GridX + 1, GridY + 1, LayerIndex, out _);

            //Left
            if (LeftTile.HasValue && ListTilesetPreset[LeftTile.Value.TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX - 1, GridY, LayerIndex), GridX - 1, GridY, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
            }

            //Right
            if (RightTile.HasValue && ListTilesetPreset[RightTile.Value.TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX + 1, GridY, LayerIndex), GridX + 1, GridY, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
            }

            //Up
            if (UpTile.HasValue && ListTilesetPreset[UpTile.Value.TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX, GridY - 1, LayerIndex), GridX, GridY - 1, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);

                //Corner Up Left
                if (UpLeftTile.HasValue && ListTilesetPreset[UpLeftTile.Value.TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX - 1, GridY - 1, LayerIndex), GridX - 1, GridY - 1, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
                }
                //Corner Up Right
                if (UpRightTile.HasValue && ListTilesetPreset[UpRightTile.Value.TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX + 1, GridY - 1, LayerIndex), GridX + 1, GridY - 1, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
                }
            }

            //Down
            if (DownTile .HasValue && ListTilesetPreset[DownTile.Value.TilesetIndex].TilesetType == TilesetType)
            {
                UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX, GridY + 1, LayerIndex), GridX, GridY + 1, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);

                //Corner Down Left
                if (DownLeftTile.HasValue && ListTilesetPreset[DownLeftTile.Value.TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX - 1, GridY + 1, LayerIndex), GridX - 1, GridY + 1, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
                }
                //Corner Down Right
                if (DownRightTile.HasValue && ListTilesetPreset[DownRightTile.Value.TilesetIndex].TilesetType == TilesetType)
                {
                    UpdateAutotTileParse(TilesetType, NewTile, NewTerrain, GetTerrain(GridX + 1, GridY + 1, LayerIndex), GridX + 1, GridY + 1, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
                }
            }

            return TileWasReplaced;
        }

        public bool UpdateAutotTileParse(DestructibleTypes TilesetType, DrawableTile NewTile, Terrain NewTerrain, Terrain CurrentTerrain,  int GridX, int GridY, int LayerIndex,int TileSizeX, int TileSizeY, List<DestructibleTilesetPreset> ListTilesetPreset)
        {
            DestructibleTilesetPreset CurrentPreset = ListTilesetPreset[NewTile.TilesetIndex];

            if (TilesetType == DestructibleTypes.Slave)
            {
                DestructibleTypes MasterType = CurrentPreset.Master.TilesetType;
                if (MasterType == DestructibleTypes.Road)
                {
                    switch (CurrentPreset.SlaveIndex)
                    {
                        case 1:
                            TilesetType = DestructibleTypes.Bridge;//Not placed manually, never used
                            break;
                    }
                }
            }

            switch (TilesetType)
            {
                case DestructibleTypes.Road:
                    return UpdtateSmartTileRoad(NewTile, NewTerrain, CurrentTerrain, GridX, GridY, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
            }

            return true;
        }

        private bool UpdtateSmartTileRoad(DrawableTile NewTile, Terrain NewTerrain, Terrain CurrentTerrain, int GridX, int GridY, int LayerIndex, int TileSizeX, int TileSizeY, List<DestructibleTilesetPreset> ListTilesetPreset)
        {
            DestructibleTilesetPreset Self = ListTilesetPreset[NewTile.TilesetIndex];

            if (ListTilesetPreset.Count >= NewTile.TilesetIndex + 1)
            {
                DrawableTile? SelfTile = GetTile(GridX, GridY, LayerIndex, out _);
                if (Self.ArrayTilesetInformation.Length > 1 && (Self.ArrayTilesetInformation[1].ListAllowedTerrainTypeIndex.Contains(CurrentTerrain.TerrainTypeIndex) || (SelfTile.HasValue && NewTile.TilesetIndex + 1 == SelfTile.Value.TilesetIndex)))
                {
                    NewTile.TilesetIndex = NewTile.TilesetIndex + 1;
                    return UpdtateSmartTileBridge(NewTile, NewTerrain, CurrentTerrain, GridX, GridY, LayerIndex, TileSizeX, TileSizeY, ListTilesetPreset);
                }
            }

            DestructibleTilesetPreset Master = ListTilesetPreset[NewTile.TilesetIndex].Master;

            bool LeftTileValid;
            bool UpTileValid;
            bool RightTileValid;
            bool DownTileValid;

            bool UpLeftTileValid;
            bool UpRightTileValid;
            bool DownLeftTileValid;
            bool DownRightTileValid;

            DrawableTile? LeftTile = GetTile(GridX - 1, GridY, LayerIndex, out LeftTileValid);
            DrawableTile? UpTile = GetTile(GridX, GridY - 1, LayerIndex, out UpTileValid);
            DrawableTile? RightTile = GetTile(GridX + 1, GridY, LayerIndex, out RightTileValid);
            DrawableTile? DownTile = GetTile(GridX, GridY + 1, LayerIndex, out DownTileValid);

            DrawableTile? UpLeftTile = GetTile(GridX - 1, GridY - 1, LayerIndex, out UpLeftTileValid);
            DrawableTile? UpRightTile = GetTile(GridX + 1, GridY - 1, LayerIndex, out UpRightTileValid);
            DrawableTile? DownLeftTile = GetTile(GridX - 1, GridY + 1, LayerIndex, out DownLeftTileValid);
            DrawableTile? DownRightTile = GetTile(GridX + 1, GridY + 1, LayerIndex, out DownRightTileValid);

            int TopHalf = TileSizeY - TileSizeY / 2;
            int BottomHalf = TileSizeY - TopHalf;

            if (LeftTile.HasValue)
            {
                LeftTileValid = ListTilesetPreset[LeftTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[LeftTile.Value.TilesetIndex].Master && ListTilesetPreset[LeftTile.Value.TilesetIndex].SlaveIndex == 1);
            }
            if (UpTile.HasValue)
            {
                UpTileValid = ListTilesetPreset[UpTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[UpTile.Value.TilesetIndex].Master && ListTilesetPreset[UpTile.Value.TilesetIndex].SlaveIndex == 1);

                if (UpLeftTile.HasValue)
                {
                    UpLeftTileValid = ListTilesetPreset[UpLeftTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[UpLeftTile.Value.TilesetIndex].Master && ListTilesetPreset[UpLeftTile.Value.TilesetIndex].SlaveIndex == 1);
                }
                if (UpRightTile.HasValue)
                {
                    UpRightTileValid = ListTilesetPreset[UpRightTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[UpRightTile.Value.TilesetIndex].Master && ListTilesetPreset[UpRightTile.Value.TilesetIndex].SlaveIndex == 1);
                }
            }
            if (RightTile.HasValue)
            {
                RightTileValid = ListTilesetPreset[RightTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[RightTile.Value.TilesetIndex].Master && ListTilesetPreset[RightTile.Value.TilesetIndex].SlaveIndex == 1);
            }
            if (DownTile.HasValue)
            {
                DownTileValid = ListTilesetPreset[DownTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[DownTile.Value.TilesetIndex].Master && ListTilesetPreset[DownTile.Value.TilesetIndex].SlaveIndex == 1);

                if (DownLeftTile.HasValue)
                {
                    DownLeftTileValid = ListTilesetPreset[DownLeftTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[DownLeftTile.Value.TilesetIndex].Master && ListTilesetPreset[DownLeftTile.Value.TilesetIndex].SlaveIndex == 1);
                }
                if (DownRightTile.HasValue)
                {
                    DownRightTileValid = ListTilesetPreset[DownRightTile.Value.TilesetIndex] == Self || (Self == ListTilesetPreset[DownRightTile.Value.TilesetIndex].Master && ListTilesetPreset[DownRightTile.Value.TilesetIndex].SlaveIndex == 1);
                }
            }

            //No corners allowed
            if ((UpTileValid && LeftTileValid && UpLeftTileValid)
                || (UpTileValid && RightTileValid && UpRightTileValid)
                || (DownTileValid && LeftTileValid && DownLeftTileValid)
                || (DownTileValid && RightTileValid && DownRightTileValid))
            {
                return false;
            }


            if (LeftTileValid)
            {
                if (UpTileValid)
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            NewTile.Origin.Location = new Point(0 * TileSizeX, 0 * TileSizeY);
                        }
                        else//Nothing Down
                        {
                            NewTile.ArraySubTile = new Rectangle[]
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

                            NewTile.ArraySubTile = new Rectangle[] { TopLeft, TopRight, BottomLeft, BottomRight };
                        }
                        else//Nothing Down
                        {
                            NewTile.Origin.Location = new Point(2 * TileSizeX, 3 * TileSizeY);
                        }
                    }
                }
                else//Nothing Up
                {
                    if (RightTileValid)
                    {
                        if (DownTileValid)
                        {
                            NewTile.ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(1 * TileSizeX,                    1 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(1 * TileSizeX + TileSizeX / 2,    1 * TileSizeY,              TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    0 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,    0 * TileSizeY + TopHalf,    TileSizeX / 2, BottomHalf),
                            };
                        }
                        else//Nothing Down
                        {
                            NewTile.ArraySubTile = new Rectangle[]
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
                            NewTile.Origin.Location = new Point(2 * TileSizeX, 1 * TileSizeY);
                        }
                        else
                        {
                            NewTile.ArraySubTile = new Rectangle[]
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
                            NewTile.ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                        2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,        3 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                        2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(0 * TileSizeX + TileSizeX / 2,        1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else//Nothing Down
                        {
                            NewTile.Origin.Location = new Point(0, 3 * TileSizeY);
                        }
                    }
                    else//Nothing Up
                    {
                        if (DownTileValid)
                        {
                            NewTile.Origin.Location = new Point(0, 1 * TileSizeY);
                        }
                        else
                        {
                            NewTile.ArraySubTile = new Rectangle[]
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
                            NewTile.ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    2 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            NewTile.ArraySubTile = new Rectangle[]
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
                            NewTile.ArraySubTile = new Rectangle[]
                            {
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY, TileSizeX / 2, TopHalf),
                                    new Rectangle(0 * TileSizeX,                    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                                    new Rectangle(2 * TileSizeX + TileSizeX / 2,    1 * TileSizeY + TopHalf, TileSizeX / 2, BottomHalf),
                            };
                        }
                        else
                        {
                            NewTile.Origin.Location = new Point(0 * TileSizeX, 0 * TileSizeY);
                        }
                    }
                }
            }

            CreateTerrain(GridX, GridY, LayerIndex, NewTile, NewTerrain);

            return true;
        }

        private bool UpdtateSmartTileBridge(DrawableTile NewTile, Terrain NewTerrain, Terrain CurrentTerrain, int GridX, int GridY, int LayerIndex, int TileSizeX, int TileSizeY, List<DestructibleTilesetPreset> ListTilesetPreset)
        {
            DestructibleTilesetPreset Self = ListTilesetPreset[NewTile.TilesetIndex];
            DestructibleTilesetPreset Master = ListTilesetPreset[NewTile.TilesetIndex].Master;

            bool LeftTileValid;
            bool UpTileValid;
            bool RightTileValid;
            bool DownTileValid;

            bool UpLeftTileValid;
            bool UpRightTileValid;
            bool DownLeftTileValid;
            bool DownRightTileValid;

            DrawableTile? LeftTile = GetTile(GridX - 1, GridY, LayerIndex, out LeftTileValid);
            DrawableTile? UpTile = GetTile(GridX, GridY - 1, LayerIndex, out UpTileValid);
            DrawableTile? RightTile = GetTile(GridX + 1, GridY, LayerIndex, out RightTileValid);
            DrawableTile? DownTile = GetTile(GridX, GridY + 1, LayerIndex, out DownTileValid);

            DrawableTile? UpLeftTile = GetTile(GridX - 1, GridY - 1, LayerIndex, out UpLeftTileValid);
            DrawableTile? UpRightTile = GetTile(GridX + 1, GridY - 1, LayerIndex, out UpRightTileValid);
            DrawableTile? DownLeftTile = GetTile(GridX - 1, GridY + 1, LayerIndex, out DownLeftTileValid);
            DrawableTile? DownRightTile = GetTile(GridX + 1, GridY + 1, LayerIndex, out DownRightTileValid);

            int TopHalf = TileSizeY - TileSizeY / 2;
            int BottomHalf = TileSizeY - TopHalf;

            if (LeftTile.HasValue)
            {
                LeftTileValid = ListTilesetPreset[LeftTile.Value.TilesetIndex] == Self || (Master == ListTilesetPreset[LeftTile.Value.TilesetIndex]);
            }
            if (UpTile.HasValue)
            {
                UpTileValid = ListTilesetPreset[UpTile.Value.TilesetIndex] == Self || (Master == ListTilesetPreset[UpTile.Value.TilesetIndex]);

                if (UpLeftTile.HasValue)
                {
                    UpLeftTileValid = ListTilesetPreset[UpLeftTile.Value.TilesetIndex] == Self || Master == ListTilesetPreset[UpLeftTile.Value.TilesetIndex];
                }
                if (UpRightTile.HasValue)
                {
                    UpRightTileValid = ListTilesetPreset[UpRightTile.Value.TilesetIndex] == Self || Master == ListTilesetPreset[UpRightTile.Value.TilesetIndex];
                }
            }
            if (RightTile.HasValue)
            {
                RightTileValid = ListTilesetPreset[RightTile.Value.TilesetIndex] == Self || (Master == ListTilesetPreset[RightTile.Value.TilesetIndex]);
            }
            if (DownTile.HasValue)
            {
                DownTileValid = ListTilesetPreset[DownTile.Value.TilesetIndex] == Self || (Master == ListTilesetPreset[DownTile.Value.TilesetIndex]);

                if (DownLeftTile.HasValue)
                {
                    DownLeftTileValid = ListTilesetPreset[DownLeftTile.Value.TilesetIndex] == Self || Master == ListTilesetPreset[DownLeftTile.Value.TilesetIndex].Master;
                }
                if (DownRightTile.HasValue)
                {
                    DownRightTileValid = ListTilesetPreset[DownRightTile.Value.TilesetIndex] == Self || Master == ListTilesetPreset[DownRightTile.Value.TilesetIndex].Master;
                }
            }

            //No corners allowed
            if ((LeftTileValid && (UpLeftTileValid || DownLeftTileValid))
                || (RightTileValid && (UpRightTileValid || DownRightTileValid))
                || (DownTileValid && (DownLeftTileValid || DownRightTileValid))
                || (UpTileValid && (UpRightTileValid || UpLeftTileValid)))
            {
                return false;
            }

            if (LeftTileValid || RightTileValid)
            {
                NewTile.Origin.Location = new Point(1 * TileSizeX, 1 * TileSizeY);
            }
            else if (UpTileValid || DownTileValid)
            {
                NewTile.Origin.Location = new Point(0 * TileSizeX, 2 * TileSizeY);
            }

            CreateTerrain(GridX, GridY, LayerIndex, NewTile, NewTerrain);

            return true;
        }
    }
}
