using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Attacks;

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

        private DestructibleTilesetPreset(DestructibleTilesetPreset Clone, int Index)
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

        public static DestructibleTilesetPreset FromFile(string FilePath, int TilesetIndex = 0)
        {
            FileStream FS = new FileStream("Content/Maps/Tilesets presets/" + FilePath + ".pet", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);
            BR.BaseStream.Seek(0, SeekOrigin.Begin);

            int TileSizeX = BR.ReadInt32();
            int TileSizeY = BR.ReadInt32();

            DestructibleTilesetPreset NewTilesetPreset = new DestructibleTilesetPreset(BR, TileSizeX, TileSizeY, TilesetIndex);

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
    }
}
