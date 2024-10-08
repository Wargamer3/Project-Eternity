﻿using System;
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

        public override void RemoveUnit(int PlayerIndex, object UnitToRemove)
        {
            /*ListPlayer[ActivePlayerIndex].ListSquad.Remove((Squad)UnitToRemove);
            ListPlayer[ActivePlayerIndex].UpdateAliveStatus();*/
        }

        public override void AddUnit(int PlayerIndex, object UnitToAdd, MovementAlgorithmTile NewPosition)
        {
            /*Squad ActiveSquad = (Squad)UnitToAdd;
            for (int U = 0; U < ActiveSquad.UnitsInSquad; ++U)
            {
                ActiveSquad.At(U).ReinitializeMembers(DicUnitType[ActiveSquad.At(U).UnitTypeName]);
            }

            ActiveSquad.ReloadSkills(DicUnitType, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            ListPlayer[PlayerIndex].ListSquad.Add(ActiveSquad);
            ListPlayer[PlayerIndex].UpdateAliveStatus();
            ActiveSquad.SetPosition(new Vector3(NewPosition.WorldPosition.X, NewPosition.WorldPosition.Y, NewPosition.LayerIndex));*/
        }

        public override void ReplaceTile(int X, int Y, int LayerIndex, DrawableTile ActiveTile)
        {
            /*DrawableTile NewTile = new DrawableTile(ActiveTile);

            LayerManager.ListLayer[LayerIndex].LayerGrid.ReplaceTile(X, Y, NewTile);
            LayerManager.LayerHolderDrawable.Reset();*/
        }

        public override void AddPlatform(BattleMapPlatform NewPlatform)
        {
            /*foreach (Player ActivePlayer in ListPlayer)
            {
                NewPlatform.AddLocalPlayer(ActivePlayer);
            }*/

            ListPlatform.Add(NewPlatform);
        }

        public override void SetWorld(Matrix World)
        {
            /*LayerManager.LayerHolderDrawable.SetWorld(World);

            for (int Z = 0; Z < LayerManager.ListLayer.Count; ++Z)
            {
                Vector3[] ArrayNewPosition = new Vector3[MapSize.X * MapSize.Y];
                for (int X = 0; X < MapSize.X; ++X)
                {
                    for (int Y = 0; Y < MapSize.Y; ++Y)
                    {
                        ArrayNewPosition[X + Y * MapSize.X] = new Vector3(X * 32, (LayerManager.ListLayer[Z].ArrayTerrain[X, Y].Height + Z) * 32, Y * 32);
                    }
                }

                Vector3.Transform(ArrayNewPosition, ref World, ArrayNewPosition);

                for (int X = 0; X < MapSize.X; ++X)
                {
                    for (int Y = 0; Y < MapSize.Y; ++Y)
                    {
                        LayerManager.ListLayer[Z].ArrayTerrain[X, Y].Position
                            = new Vector3((float)Math.Round(ArrayNewPosition[X + Y * MapSize.X].X / 32), (float)Math.Round(ArrayNewPosition[X + Y * MapSize.X].Z / 32), ArrayNewPosition[X + Y * MapSize.X].Y / 32);
                    }
                }
            }*/
        }

        public override MovementAlgorithmTile GetNextLayerIndex(MovementAlgorithmTile StartingPosition, int NextX, int NextY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
        {
            ListLayerPossibility = new List<MovementAlgorithmTile>();

            byte CurrentTerrainType = GetTerrain((int)StartingPosition.WorldPosition.X, (int)StartingPosition.WorldPosition.Y, (int)StartingPosition.LayerIndex).TerrainTypeIndex;
            float CurrentZ = StartingPosition.WorldPosition.Z;

            int ClosestLayerIndexDown = -1;
            int ClosestLayerIndexUp = 0;
            float ClosestTerrainDistanceDown = float.MaxValue;
            float ClosestTerrainDistanceUp = float.MinValue;

            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain NextTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                byte NextTerrainType = GetTerrain(NextX, NextY, L).TerrainTypeIndex;
                float NextTerrainZ = NextTerrain.WorldPosition.Z;

                //Check lower or higher neighbors if on solid ground
                if (CurrentTerrainType != UnitStats.TerrainAirIndex && CurrentTerrainType != UnitStats.TerrainVoidIndex)
                {
                    if (NextTerrainType != UnitStats.TerrainAirIndex && NextTerrainType != UnitStats.TerrainVoidIndex)
                    {
                        //Prioritize going downward
                        if (NextTerrainZ <= CurrentZ)
                        {
                            float ZDiff = CurrentZ - NextTerrainZ;
                            if (ZDiff <= ClosestTerrainDistanceDown && HasEnoughClearance(NextTerrainZ, NextX, NextY, L, MaxClearance))
                            {
                                ClosestTerrainDistanceDown = ZDiff;
                                ClosestLayerIndexDown = L;
                                ListLayerPossibility.Add(NextTerrain);
                            }
                        }
                        else
                        {
                            float ZDiff = NextTerrainZ - CurrentZ;
                            if (ZDiff >= ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                            {
                                ClosestTerrainDistanceUp = ZDiff;
                                ClosestLayerIndexUp = L;
                                ListLayerPossibility.Add(NextTerrain);
                            }
                        }
                    }
                }
                //Already in void, check for any neighbors
                else
                {
                    if (NextTerrainZ == StartingPosition.LayerIndex && NextTerrainType == CurrentTerrainType)
                    {
                        return NextTerrain;
                    }
                    //Prioritize going upward
                    else if (NextTerrainZ > StartingPosition.LayerIndex)
                    {
                        float ZDiff = NextTerrainZ - CurrentZ;
                        if (ZDiff < ClosestTerrainDistanceUp && ZDiff <= ClimbValue)
                        {
                            ClosestTerrainDistanceUp = ZDiff;
                            ClosestLayerIndexUp = L;
                            ListLayerPossibility.Add(NextTerrain);
                        }
                    }
                }
            }

            if (ClosestLayerIndexDown >= 0)
            {
                return ListLayer[ClosestLayerIndexDown].ArrayTerrain[NextX, NextY];
            }
            else
            {
                return ListLayer[ClosestLayerIndexUp].ArrayTerrain[NextX, NextY];
            }
        }

        public override MovementAlgorithmTile GetMovementTile(int X, int Y, int LayerIndex)
        {
            if (X < 0 || Y >= MapSize.X || Y < 0 || Y >= MapSize.Y || LayerIndex < 0 || LayerIndex >= ListLayer.Count)
            {
                return null;
            }

            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public override List<MovementAlgorithmTile> GetCampaignEnemySpawnLocations()
        {
            List<MovementAlgorithmTile> ListPossibleSpawnPoint = new List<MovementAlgorithmTile>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.GetCampaignEnemySpawnLocations());
            }

            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListCampaignSpawns.Count; S++)
                {
                    if (ActiveLayer.ListCampaignSpawns[S].Tag == "E")
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ArrayTerrain[(int)ActiveLayer.ListMultiplayerSpawns[S].Position.X, (int)ActiveLayer.ListMultiplayerSpawns[S].Position.Y]);
                    }
                }
            }

            return ListPossibleSpawnPoint;
        }

        public override List<MovementAlgorithmTile> GetMultiplayerSpawnLocations(int Team)
        {
            List<MovementAlgorithmTile> ListPossibleSpawnPoint = new List<MovementAlgorithmTile>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.GetMultiplayerSpawnLocations(Team));
            }

            string PlayerTag = (Team + 1).ToString();
            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (ActiveLayer.ListMultiplayerSpawns[S].Tag == PlayerTag)
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ArrayTerrain[(int)ActiveLayer.ListMultiplayerSpawns[S].Position.X, (int)ActiveLayer.ListMultiplayerSpawns[S].Position.Y]);
                    }
                }
            }

            return ListPossibleSpawnPoint;
        }

        private bool HasEnoughClearance(float CurrentZ, int NextX, int NextY, int StartLayer, float MaxClearance)
        {
            for (int L = StartLayer + 1; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                Terrain ActiveTerrain = ActiveLayer.ArrayTerrain[NextX, NextY];

                byte NextTerrainType = GetTerrain(NextX, NextX, L).TerrainTypeIndex;
                float NextTerrainZ = ActiveTerrain.WorldPosition.Z;

                float ZDiff = NextTerrainZ - CurrentZ;

                if (NextTerrainType != UnitStats.TerrainAirIndex && NextTerrainType != UnitStats.TerrainVoidIndex && ZDiff < MaxClearance)
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

        public override BattleMap GetNewMap(GameModeInfo GameInfo, string ParamsID)
        {
            return new WorldMap(BattleMapPath, GameInfo);
        }

        public override GameScreen GetMultiplayerScreen()
        {
            throw new NotImplementedException();
        }

        public override string GetMapType()
        {
            return "World Map";
        }

        public override Dictionary<string, GameModeInfo> GetAvailableGameModes()
        {
            Dictionary<string, GameModeInfo> DicGameType = new Dictionary<string, GameModeInfo>();

            return DicGameType;
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
