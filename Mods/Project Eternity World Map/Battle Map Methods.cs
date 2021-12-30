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

        public override int GetNextLayerIndex(Vector3 CurrentPosition, int NextX, int NextY)
        {
            string CurrentTerrainType = GetTerrainType(CurrentPosition.X, CurrentPosition.Y, (int)CurrentPosition.Z);
            float MaxClearance = 1f;
            float ClimbValue = 1f;

            int ClosestLayerIndex = -1;
            float ClosestTerrainDistance = float.MaxValue;

            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain ActiveTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                string NextTerrainType = GetTerrainType(NextX, NextX, L);
                float NextTerrainZ = ActiveTerrain.Position.Z + L;

                //Check lower or higher neighbors if on solid ground
                if (CurrentTerrainType != UnitStats.TerrainAir && CurrentTerrainType != UnitStats.TerrainVoid)
                {
                    float ZDiff = NextTerrainZ - CurrentPosition.Z;
                    //Prioritize going downward
                    if (NextTerrainZ <= CurrentPosition.Z)
                    {
                        if (ZDiff < ClosestTerrainDistance && HasEnoughClearance(NextTerrainZ, NextX, NextY, L, MaxClearance))
                        {
                            ClosestTerrainDistance = ZDiff;
                            ClosestLayerIndex = L;
                        }
                    }
                    else
                    {
                        if (ZDiff < ClosestTerrainDistance && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistance = ZDiff;
                            ClosestLayerIndex = L;
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
                        float ZDiff = NextTerrainZ - CurrentPosition.Z;
                        if (ZDiff < ClosestTerrainDistance && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistance = ZDiff;
                            ClosestLayerIndex = L;
                        }
                    }
                }
            }

            return ClosestLayerIndex;
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
