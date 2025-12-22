using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Attacks;
using System.Text;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class ConquestDestructibleTilesetPreset : DestructibleTilesetPreset
    {
        private readonly ConquestMap ActiveMap;
        private int Defense;

        private ConquestDestructibleTilesetPreset(DestructibleTilesetPreset Clone, int Index)
            : base(Clone, Index)
        {
        }

        public ConquestDestructibleTilesetPreset(ConquestMap Map, BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex, bool LoadBackgroundPaths = true)
            : base(BR, TileSizeX, TileSizeY, TilesetIndex, LoadBackgroundPaths)
        {
            this.ActiveMap = Map;
        }

        public ConquestDestructibleTilesetPreset(ConquestMap Map, string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            : base(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
        {
            this.ActiveMap = Map;
        }

        public override DestructibleTilesetPreset CreateSlave(int Index)
        {
            DestructibleTilesetPreset Slave = new ConquestDestructibleTilesetPreset(this, Index);
            Slave.TilesetType = DestructibleTilesAttackAttributes.DestructibleTypes.Slave;

            return Slave;
        }

        protected override void CreateTerrain(int GridX, int GridY, int LayerIndex, DrawableTile ReplacementTile, Terrain ReplacementTerrain)
        {
            TerrainConquest NewTerrain = new TerrainConquest(ReplacementTerrain, new Point(GridX, GridY), LayerIndex);
            NewTerrain.Owner = ActiveMap;
            NewTerrain.WorldPosition = new Vector3(GridX * ActiveMap.TileSize.X, GridY * ActiveMap.TileSize.Y, (LayerIndex + NewTerrain.Height) * ActiveMap.LayerHeight);

            Vector3 Position = new Vector3(GridX, GridY, LayerIndex);
            DestructibleTerrain NewDestructibleTerrain = new DestructibleTerrain();
            NewDestructibleTerrain.ReplacementTile = ReplacementTile;
            NewDestructibleTerrain.ReplacementTerrain = (TerrainConquest)NewTerrain;
            NewDestructibleTerrain.RemainingHP = HP;
            NewDestructibleTerrain.Defense = Defense;

            if (ActiveMap.DicTemporaryTerrain.ContainsKey(Position))
            {
                ActiveMap.DicTemporaryTerrain[Position] = NewDestructibleTerrain;
            }
            else
            {
                ActiveMap.DicTemporaryTerrain.Add(Position, NewDestructibleTerrain);
            }
        }

        protected override TilesetPresetInformation CreateTerrain(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new ConquestTilesetPreset.ConquestTilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected override DrawableTile? GetTile(int GridX, int GridY, int LayerIndex, out bool IsOutOfBound)
        {
            IsOutOfBound = false;
            DestructibleTerrain FoundTerrain;
            if (ActiveMap.DicTemporaryTerrain.TryGetValue(new Vector3(GridX, GridY, LayerIndex), out FoundTerrain))
            {
                return FoundTerrain.ReplacementTile;
            }

            if (GridX < 0 || GridY < 0 || GridX >= ActiveMap.MapSize.X || GridY >= ActiveMap.MapSize.Y)
            {
                IsOutOfBound = true;
            }
            return new DrawableTile?();
        }

        protected override Terrain GetTerrain(int GridX, int GridY, int LayerIndex)
        {
            DestructibleTerrain FoundTerrain;
            if (ActiveMap.DicTemporaryTerrain.TryGetValue(new Vector3(GridX, GridY, LayerIndex), out FoundTerrain))
            {
                return FoundTerrain.ReplacementTerrain;
            }

            return null;
        }

        protected override TilesetPresetInformation ReadTerrain(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new ConquestTilesetPreset.ConquestTilesetPresetInformation(BR, TileSizeX, TileSizeY, TilesetIndex);
        }

        public static DestructibleTilesetPreset FromFile(ConquestMap ActiveMap, string FilePath, string RelativePath, int TilesetIndex = 0)
        {
            FileStream FS = new FileStream("Content/" + FilePath, FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int TileSizeX = BR.ReadInt32();
            int TileSizeY = BR.ReadInt32();

            ConquestDestructibleTilesetPreset NewTilesetPreset = new ConquestDestructibleTilesetPreset(ActiveMap, BR, TileSizeX, TileSizeY, TilesetIndex);
            NewTilesetPreset.RelativePath = RelativePath;

            BR.Close();
            FS.Close();

            return NewTilesetPreset;
        }

    }
}
