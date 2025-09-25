using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{/*en gros, le but du jeu ce joue principalement sur la capture de batiment, les ville serve a créé de l'argent, les port a créé des bateau, les caserne a créé des unité de terre etc...
  *et ultimement le but par défaut d'une partie c'est sois éliminé tout les unité ennemis ou sois de capturé le QG ennemis
 bob Le Nolife: c'est assez simpliste comme jeu, après il y a des subtilité comme le choix du générale qui donne des bonus, mais ça je m'en fou carrément, je prend toujours les généraux par défaut*/
    //Captured building give 1000g per turn
    //Any unit in a friendly city will repair 2 HP and deduct 20% of its cost
    //http://www.warsworldnews.com/index.php?page=aw/battlemechanics/index.php
    //http://www.warsworldnews.com/aw/damagechart/index.php
    //http://strategywiki.org/wiki/Advance_Wars:_Days_of_Ruin/Terrain
    //http://strategywiki.org/wiki/Advance_Wars:_Days_of_Ruin/Getting_Started
    //http://www.gamefaqs.com/ds/943675-advance-wars-days-of-ruin/faqs/51639
    //http://www.gamesradar.com/cheats/13550/
    //http://AdvanceWars.wikia.com/wiki/Terrain
    //http://AdvanceWars.wikia.com/wiki/Building
    //http://www.advance-wars-net.com/?g=awdor&a=articles/awdor/Terrain.html
    //http://ca.ign.com/wikis/advance-wars-2-black-hole-rising/Basics
    //http://ticc.uvt.nl/~pspronck/pubs/BNAIC2008Bergsma.pdf
    //http://www.warsworldnews.com/dor/aw4-color.pdf attacks

    public class ConquestTerrainHolder
    {
        public List<string> ListMoveType;
        public List<ConquestTerrainType> ListConquestTerrainType;

        public ConquestTerrainHolder()
        {
            ListMoveType = new List<string>();
            ListConquestTerrainType = new List<ConquestTerrainType>();
        }

        public void LoadData()
        {
            FileStream FS = new FileStream("Content/Conquest Terrains And Movements.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS, Encoding.Unicode);

            int ListMoveTypeCount = BR.ReadInt32();
            ListMoveType = new List<string>(ListMoveTypeCount);
            for (int i = 0; i < ListMoveTypeCount; ++i)
            {
                ListMoveType.Add(BR.ReadString());
            }

            int ListConquestTerrainTypeCount = BR.ReadInt32();
            ListConquestTerrainType = new List<ConquestTerrainType>(ListConquestTerrainTypeCount);
            for (int i = 0; i < ListConquestTerrainTypeCount; ++i)
            {
                ListConquestTerrainType.Add(new ConquestTerrainType(BR));
            }

            BR.Close();
            FS.Close();
        }
    }

    public class ConquestTerrainType
    {
        public string TerrainName;
        public byte DefenceValue;
        public Dictionary<byte, byte> DicMovementCostByMoveType;

        public ConquestTerrainType()
        {
            TerrainName = "New Terrain";
            DefenceValue = 1;

            DicMovementCostByMoveType = new Dictionary<byte, byte>();
        }

        public ConquestTerrainType(BinaryReader BR)
        {
            TerrainName = BR.ReadString();
            DefenceValue = BR.ReadByte();

            int ListeMovementCostByMoveTypeCount = BR.ReadInt32();
            DicMovementCostByMoveType = new Dictionary<byte, byte>(ListeMovementCostByMoveTypeCount);

            for (int i = 0; i < ListeMovementCostByMoveTypeCount; ++i)
            {
                DicMovementCostByMoveType.Add(BR.ReadByte(), BR.ReadByte());
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(TerrainName);
            BW.Write(DefenceValue);

            BW.Write(DicMovementCostByMoveType.Count);

            foreach (KeyValuePair<byte, byte> MovementCostByMoveType in DicMovementCostByMoveType)
            {
                BW.Write(MovementCostByMoveType.Key);
                BW.Write(MovementCostByMoveType.Value);
            }
        }
    }

    public class ConquestTilesetPreset : TilesetPreset
    {
        public class ConquestTilesetPresetInformation : TilesetPresetInformation
        {
            public ConquestTilesetPresetInformation(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
                : base(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
            {
            }

            public ConquestTilesetPresetInformation(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
                : base(BR, TileSizeX, TileSizeY, TilesetIndex)
            {
            }

            public override Terrain CreateTerrain(int X, int Y, int TileSizeX, int TileSizeY)
            {
                return new TerrainConquest(X, Y, TileSizeX, TileSizeY, 0, 0, 0);
            }

            protected override Terrain ReadTerrain(BinaryReader BR, int X, int Y, int LayerIndex, int LayerDepth)
            {
                return new TerrainConquest(BR, X, Y, 0, 0, LayerIndex, 0, LayerDepth);
            }
        }

        public ConquestTilesetPreset(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
            : base(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex)
        {
        }

        public ConquestTilesetPreset(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex, bool LoadBackgroundPaths = true)
            : base(BR, TileSizeX, TileSizeY, TilesetIndex, LoadBackgroundPaths)
        {
        }

        protected override TilesetPresetInformation CreateTerrain(string TilesetName, int TilesetWidth, int TilesetHeight, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new ConquestTilesetPresetInformation(TilesetName, TilesetWidth, TilesetHeight, TileSizeX, TileSizeY, TilesetIndex);
        }

        protected override TilesetPresetInformation ReadTerrain(BinaryReader BR, int TileSizeX, int TileSizeY, int TilesetIndex)
        {
            return new ConquestTilesetPresetInformation(BR, TileSizeX, TileSizeY, TilesetIndex);
        }
    }

    public class TerrainConquest : Terrain
    {
        public TerrainConquest(Terrain Other, Point Position, int LayerIndex)
            : base(Other, Position, LayerIndex)
        {
        }

        /// <summary>
        /// Used to create the empty array of the map.
        /// </summary>
        public TerrainConquest(int GridPosX, int GridPosY, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(GridPosX, GridPosY, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
        }

        public TerrainConquest(int GridPosX, int GridPosY, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth, byte TerrainTypeIndex)
            : base(GridPosX, GridPosY, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
            this.TerrainTypeIndex = TerrainTypeIndex;
        }

        public TerrainConquest(BinaryReader BR, int GridPosX, int GridPosY, int TileSizeX, int TileSizeY, int LayerIndex, int LayerHeight, float LayerDepth)
            : base(BR, GridPosX, GridPosY, TileSizeX, TileSizeY, LayerIndex, LayerHeight, LayerDepth)
        {
        }
    }

    public class DestructibleTerrain
    {
        public TerrainConquest ReplacementTerrain;
        public DrawableTile ReplacementTile;
        public int RemainingHP;
        public int Defense;//Used to determine what can damage it

        public void DamageTile()
        {
            --RemainingHP;

            if (RemainingHP <= 0)
            {
            }
            else
            {
                UpdateTile();
            }
        }

        public void UpdateTile()
        {
            ReplacementTile.Origin.X -= ReplacementTile.Origin.Width;
        }

        public static void UpdateAllTemporaryTerrain(ConquestMap ActiveMap)
        {
            ActiveMap.DicTemporaryTerrain.Clear();
        }
    }
}
