using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
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

    public class ConquestTerrainTypeAttributes
    {
        public string TerrainName;
        public byte DefenceValue;
        public Dictionary<byte, byte> DicMovementCostByMoveType;

        public ConquestTerrainTypeAttributes()
        {
            TerrainName = "New Terrain";
            DefenceValue = 1;

            DicMovementCostByMoveType = new Dictionary<byte, byte>();
        }

        public ConquestTerrainTypeAttributes(BinaryReader BR)
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
}
