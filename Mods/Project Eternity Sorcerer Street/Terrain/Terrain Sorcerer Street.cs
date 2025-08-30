using System;
using System.IO;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetTilesetPreset : TilesetPreset
    {
        public class SorcererStreetTilesetPresetInformation : TilesetPresetInformation
        {
            public SorcererStreetTilesetPresetInformation(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
                : base(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
            {
            }

            public SorcererStreetTilesetPresetInformation(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
                : base(BR, TileSizeX, TileSizeY, TilesetIndex)
            {
            }

            public override Terrain CreateTerrain(int X, int Y, int TileSizeX, int TileSizeY)
            {
                return new TerrainSorcererStreet(X, Y, TileSizeX, TileSizeY, 0, 0, 0);
            }

            protected override Terrain ReadTerrain(BinaryReader BR, int X, int Y, int LayerIndex, int LayerDepth)
            {
                return new TerrainSorcererStreet(BR, X, Y, 0, 0, LayerIndex, 0, LayerDepth);
            }
        }

        public SorcererStreetTilesetPreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            :base (TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
        {
        }

        public SorcererStreetTilesetPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int Index, bool LoadBackgroundPaths = true)
            : base(BR, TileSizeX, TileSizeY, Index, LoadBackgroundPaths)
        {
        }

        protected override TilesetPresetInformation CreateTerrain(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new SorcererStreetTilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected override TilesetPresetInformation ReadTerrain(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new SorcererStreetTilesetPresetInformation(BR, TileSizeX, TileSizeY, TilesetIndex);
        }
    }

    public class TerrainSorcererStreet : Terrain
    {
        public const string Castle = "Castle";
        public const string FireElement = "Fire";
        public const string WaterElement = "Water";
        public const string EarthElement = "Earth";
        public const string AirElement = "Air";
        public const string NeutralElement = "Neutral";
        public const string MorphElement = "Morph";
        public const string MultiElement = "Multi";
        public const string EastTower = "East Gate";
        public const string WestTower = "Weast Gate";
        public const string SouthTower = "South Gate";
        public const string NorthTower = "North Gate";
        public const string Shrine = "Shrine";
        public const string FortuneTeller = "Fortune Teller";

        public CreatureCard DefendingCreature;
        public Player PlayerOwner;
        public int BaseValue;//80,100,120
        public int CurrentToll;
        public int CurrentValue;
        public float TollMultiplier;
        public int TollModifier;
        public int LandLevel;

        public TerrainSorcererStreet(Terrain Other, Point Position, int LayerIndex)
            : base(Other, Position, LayerIndex)
        {
            TerrainTypeIndex = Other.TerrainTypeIndex;
            TollMultiplier = 1;
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public TerrainSorcererStreet(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            TerrainTypeIndex = 0;
            TollMultiplier = 1;
            BaseValue = 100;
            LandLevel = 1;
        }
        
        public TerrainSorcererStreet(int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
            TollMultiplier = 1;
            BaseValue = 100;
            LandLevel = 1;
        }

        public TerrainSorcererStreet(BinaryReader BR, int XPos, int YPos, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(XPos, YPos, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            TerrainTypeIndex = BR.ReadByte();
            Height = BR.ReadSingle();
        }

        public override void Save(BinaryWriter BW)
        {
            BW.Write(TerrainTypeIndex);
            BW.Write(Height);
        }

        public void UpdateValue(int ChainLevel, CreatureCard Defender)
        {
            CurrentToll = (int)(BaseValue / 5 * Math.Pow(2, LandLevel - 1) * GetChainMultiplier(ChainLevel) * TollMultiplier * Defender.TollMultiplier) + TollModifier;
            CurrentValue = (int)(BaseValue * Math.Pow(2, LandLevel - 1) * GetChainMultiplier(ChainLevel));
        }

        private float GetChainMultiplier(int ChainLevel)
        {
            if (ChainLevel >= 5)
            {
                return 2.2f;
            }
            else if (ChainLevel == 4)
            {
                return 2f;
            }
            else if (ChainLevel == 3)
            {
                return 1.8f;
            }
            else if (ChainLevel == 2)
            {
                return 1.5f;
            }
            return 1;
        }

        public virtual void OnReached(SorcererStreetMap Map, int ActivePlayerIndex, int MovementRemaining)
        {
        }
    }
}
