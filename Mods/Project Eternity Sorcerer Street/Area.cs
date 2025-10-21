using System;
using System.Collections.Generic;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class Area
    {
        public string Name;
        public List<TerrainSorcererStreet> ListTerrainInArea = new List<TerrainSorcererStreet>();

        public Area(string Name)
        {
            this.Name = Name;
        }

        public TerrainSorcererStreet FindExistingTerrain(int GridX, int GridY, int LayerHeight)
        {
            TerrainSorcererStreet FoundTerrain = null;

            foreach (TerrainSorcererStreet ActiveTerrain in ListTerrainInArea)
            {
                if (ActiveTerrain.GridPosition.X == GridX
                    && ActiveTerrain.GridPosition.Y == GridY
                     && ActiveTerrain.LayerIndex == LayerHeight)
                {
                    FoundTerrain = ActiveTerrain;
                    break;
                }
            }

            return FoundTerrain;
        }
    }
}
