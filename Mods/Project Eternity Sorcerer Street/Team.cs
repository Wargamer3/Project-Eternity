using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Team
    {
        public int Rank;//Rank in the game between players
        public int TotalMagic;
        public int TeamIndex;
        public List<Player> ListPlayer;

        public bool AllowSameTeamTerritoryCommand;
        public bool AllowSameTeamInfighting;

        public Dictionary<byte, byte> DicCreatureCountByElementType;

        public Team(int TeamIndex)
        {
            this.TeamIndex = TeamIndex;

            ListPlayer = new List<Player>();
            DicCreatureCountByElementType = new Dictionary<byte, byte>();
        }

        public void IncreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicCreatureCountByElementType.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicCreatureCountByElementType.Add(TerrainTypeIndex, 1);
            }
            else
            {
                DicCreatureCountByElementType[TerrainTypeIndex] = (byte)(ChainValue + 1);
            }
        }

        public void DecreaseChainLevels(byte TerrainTypeIndex)
        {
            byte ChainValue;

            if (!DicCreatureCountByElementType.TryGetValue(TerrainTypeIndex, out ChainValue))
            {
                DicCreatureCountByElementType.Add(TerrainTypeIndex, 0);
            }
            else
            {
                DicCreatureCountByElementType[TerrainTypeIndex] = (byte)(ChainValue - 1);
            }
        }
    }
}
