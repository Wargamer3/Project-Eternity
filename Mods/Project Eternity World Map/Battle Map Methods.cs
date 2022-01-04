using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public partial class WorldMap
    {
        public Terrain GetTerrain(int X, int Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public string GetTerrainType(float PosX, float PosY, int LayerIndex)
        {
            return GetTerrainType(GetTerrain((int)PosX, (int)PosY, LayerIndex));
        }

        public string GetTerrainType(MovementAlgorithmTile ActiveTerrain)
        {
            return ListTerrainType[ActiveTerrain.TerrainTypeIndex];
        }

        public override int GetNextLayerIndex(Vector3 CurrentPosition, int NextX, int NextY, float MaxClearance, float ClimbValue, out List<int> ListLayerPossibility)
        {
            ListLayerPossibility = new List<int>();

            Terrain CurrentTerrain = ListLayer[(int)CurrentPosition.Z].ArrayTerrain[(int)CurrentPosition.X, (int)CurrentPosition.Y];
            string CurrentTerrainType = GetTerrainType(CurrentPosition.X, CurrentPosition.Y, (int)CurrentPosition.Z);
            float CurrentZ = CurrentTerrain.Position.Z + CurrentPosition.Z;

            int ClosestLayerIndexDown = -1;
            int ClosestLayerIndexUp = 0;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain NextTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                string NextTerrainType = GetTerrainType(NextX, NextY, L);
                float NextTerrainZ = NextTerrain.Position.Z + L;

                //Check lower or higher neighbors if on solid ground
                if (CurrentTerrainType != UnitStats.TerrainAir && CurrentTerrainType != UnitStats.TerrainVoid)
                {
                    if (NextTerrainType != UnitStats.TerrainAir && NextTerrainType != UnitStats.TerrainVoid)
                    {
                        //Prioritize going downward
                        if (NextTerrainZ <= CurrentZ)
                        {
                            float ZDiff = CurrentZ - NextTerrainZ;
                            if (ZDiff <= ClosestTerrainDistanceDown && HasEnoughClearance(NextTerrainZ, NextX, NextY, L, MaxClearance))
                            {
                                ClosestTerrainDistanceDown = ZDiff;
                                ClosestLayerIndexDown = L;
                                ListLayerPossibility.Add(L);
                            }
                        }
                        else
                        {
                            float ZDiff = NextTerrainZ - CurrentZ;
                            if (ZDiff >= ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                            {
                                ClosestTerrainDistanceUp = ZDiff;
                                ClosestLayerIndexUp = L;
                                ListLayerPossibility.Add(L);
                            }
                        }
                    }
                }
                //Already in void, check for any neighbors
                else
                {
                    if (NextTerrainZ == CurrentPosition.Z && NextTerrainType == CurrentTerrainType)
                    {
                        return L;
                    }
                    //Prioritize going upward
                    else if (NextTerrainZ > CurrentPosition.Z)
                    {
                        float ZDiff = NextTerrainZ - CurrentZ;
                        if (ZDiff < ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistanceUp = ZDiff;
                            ClosestLayerIndexUp = L;
                            ListLayerPossibility.Add(L);
                        }
                    }
                }
            }

            if (ClosestLayerIndexDown >= 0)
            {
                return ClosestLayerIndexDown;
            }
            else
            {
                return ClosestLayerIndexUp;
            }
        }

        private bool HasEnoughClearance(float CurrentZ, int NextX, int NextY, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain ActiveTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                string NextTerrainType = GetTerrainType(NextX, NextX, L);
                float NextTerrainZ = ActiveTerrain.Position.Z + L;

                float ZDiff = NextTerrainZ - CurrentZ;

                if (NextTerrainType != UnitStats.TerrainAir && NextTerrainType != UnitStats.TerrainVoid && ZDiff < MaxClearance)
                {
                    return false;
                }
            }

            return true;
        }


        public override void SaveTemporaryMap()
        {
            throw new NotImplementedException();
        }

        public override BattleMap LoadTemporaryMap(BinaryReader BR)
        {
            throw new NotImplementedException();
        }

        public override BattleMap GetNewMap(string GameMode)
        {
            return new WorldMap(BattleMapPath, GameMode, DicSpawnSquadByPlayer);
        }

        public override GameScreen GetMultiplayerScreen()
        {
            throw new NotImplementedException();
        }

        public override string GetMapType()
        {
            return "World Map";
        }

        public override Dictionary<string, ActionPanel> GetOnlineActionPanel()
        {
            Dictionary<string, ActionPanel> DicActionPanel = new Dictionary<string, ActionPanel>();

            Assembly ActiveAssembly = Assembly.LoadFile(Path.GetFullPath("Mods/Project Eternity World Map.dll"));
            Dictionary<string, BattleMapActionPanel> DicActionPanelMap = BattleMapActionPanel.LoadFromAssembly(ActiveAssembly, typeof(ActionPanelWorldMap), this);
            foreach (KeyValuePair<string, BattleMapActionPanel> ActiveRequirement in DicActionPanelMap)
            {
                DicActionPanel.Add(ActiveRequirement.Key, ActiveRequirement.Value);
            }

            return DicActionPanel;
        }
    }
}
