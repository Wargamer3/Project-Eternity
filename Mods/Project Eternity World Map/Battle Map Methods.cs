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
        /// <summary>
        /// Move the cursor on the map.
        /// </summary>
        /// <returns>Returns true if the cursor was moved</returns>
        public override bool CursorControl(GameTime gameTime, PlayerInput ActiveInputManager)
        {
            Point Offset;
            BattleMap ActiveMap;

            bool CursorMoved = CursorControlGrid(gameTime, ActiveInputManager, out Offset, out ActiveMap);

            if (CursorMoved)
            {
                MovementAlgorithmTile NextTerrain = GetNextLayerTile(ActiveMap.CursorTerrain, Offset.X, Offset.Y, 1f, 15f, out _);
                if (NextTerrain == ActiveMap.CursorTerrain)//Force movement
                {
                    ActiveMap.CursorPosition.Z = NextTerrain.LayerIndex;
                    ActiveMap.CursorPosition.X = Math.Max(0, Math.Min(ActiveMap.MapSize.X - 1, NextTerrain.GridPosition.X + Offset.X));
                    ActiveMap.CursorPosition.Y = Math.Max(0, Math.Min(ActiveMap.MapSize.Y - 1, NextTerrain.GridPosition.Y + Offset.Y));
                }
                else
                {
                    ActiveMap.CursorPosition.Z = NextTerrain.LayerIndex;
                    ActiveMap.CursorPosition.X = NextTerrain.GridPosition.X;
                    ActiveMap.CursorPosition.Y = NextTerrain.GridPosition.Y;
                }
            }

            return CursorMoved;
        }

        public override Vector3 GetFinalPosition(Vector3 WorldPosition)
        {
            int GridX = (int)WorldPosition.X / TileSize.X;
            int GridY = (int)WorldPosition.X / TileSize.Y;
            int LayerIndex = (int)WorldPosition.Z / LayerHeight;

            Terrain ActiveTerrain = ListLayer[LayerIndex].ArrayTerrain[GridX, GridY];
            DrawableTile ActiveTile = ListLayer[LayerIndex].ArrayTile[GridX, GridY];

            Vector2 PositionInTile = new Vector2(WorldPosition.X - ActiveTerrain.WorldPosition.X, WorldPosition.Y - ActiveTerrain.WorldPosition.Y);

            return WorldPosition + new Vector3(PositionInTile, ActiveTile.Terrain3DInfo.GetZOffset(PositionInTile, ActiveTerrain.Height));
        }

        public override Vector3 GetNextPosition(Vector3 WorldPosition, Vector3 Movement)
        {
            return GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y, GetNextLayerTile(GetTerrain(WorldPosition), (int)(Movement.X / TileSize.X), (int)(Movement.Y / TileSize.Y), 1f, 15f, out _).WorldPosition.Z));
        }

        public override Tile3D CreateTile3D(int TilesetIndex, Vector3 WorldPosition, Point Origin, Point TileSize, Point TextureSize, float PositionOffset)
        {
            Vector3 TopFrontLeft = GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y + TileSize.Y, WorldPosition.Z));
            Vector3 TopFrontRight = GetFinalPosition(new Vector3(WorldPosition.X + TileSize.X, WorldPosition.Y + TileSize.Y, WorldPosition.Z));
            Vector3 TopBackLeft = GetFinalPosition(new Vector3(WorldPosition.X, WorldPosition.Y, WorldPosition.Z));
            Vector3 TopBackRight = GetFinalPosition(new Vector3(WorldPosition.X + TileSize.X, WorldPosition.Y, WorldPosition.Z));

            return Terrain3D.CreateTile3D(TilesetIndex, TopFrontLeft, TopFrontRight, TopBackLeft, TopBackRight, TileSize, Origin, TextureSize.X, TextureSize.Y, PositionOffset);
        }

        public Terrain GetTerrain(Vector3 Position)
        {
            Position = new Vector3((float)Math.Floor(Position.X / TileSize.X), (float)Math.Floor(Position.Y / TileSize.Y), (float)Math.Floor(Position.Z / LayerHeight));

            if (Position.X < 0 || Position.X >= MapSize.X || Position.Y < 0 || Position.Y >= MapSize.Y || Position.Z < 0 || Position.Z >= ListLayer.Count)
            {
                return null;
            }

            return ListLayer[(int)Position.Z].ArrayTerrain[(int)Position.X, (int)Position.Y];
        }

        public Terrain GetTerrain(int X, int Y, int LayerIndex)
        {
            return ListLayer[LayerIndex].ArrayTerrain[X, Y];
        }

        public override void RemoveUnit(int PlayerIndex, object UnitToRemove)
        {
            /*ListPlayer[ActivePlayerIndex].ListSquad.Remove((Squad)UnitToRemove);
            ListPlayer[ActivePlayerIndex].UpdateAliveStatus();*/
        }

        public override void AddUnit(int PlayerIndex, object UnitToAdd, Vector3 NewPosition)
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

        public MovementAlgorithmTile GetNextLayerTile(MovementAlgorithmTile StartingPosition, int NextX, int NextY, float MaxClearance, float ClimbValue, out List<MovementAlgorithmTile> ListLayerPossibility)
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

        public override List<Vector3> GetCampaignEnemySpawnLocations()
        {
            List<Vector3> ListPossibleSpawnPoint = new List<Vector3>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.Map.GetCampaignEnemySpawnLocations());
            }

            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListCampaignSpawns.Count; S++)
                {
                    if (ActiveLayer.ListCampaignSpawns[S].Tag == "E")
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ArrayTerrain[(int)ActiveLayer.ListMultiplayerSpawns[S].Position.X, (int)ActiveLayer.ListMultiplayerSpawns[S].Position.Y].WorldPosition);
                    }
                }
            }

            return ListPossibleSpawnPoint;
        }

        public override List<Vector3> GetMultiplayerSpawnLocations(int Team)
        {
            List<Vector3> ListPossibleSpawnPoint = new List<Vector3>();

            foreach (BattleMapPlatform ActivePlatform in ListPlatform)
            {
                ListPossibleSpawnPoint.AddRange(ActivePlatform.Map.GetMultiplayerSpawnLocations(Team));
            }

            string PlayerTag = (Team + 1).ToString();
            for (int L = 0; L < ListLayer.Count; L++)
            {
                MapLayer ActiveLayer = ListLayer[L];
                for (int S = 0; S < ActiveLayer.ListMultiplayerSpawns.Count; S++)
                {
                    if (ActiveLayer.ListMultiplayerSpawns[S].Tag == PlayerTag)
                    {
                        ListPossibleSpawnPoint.Add(ActiveLayer.ArrayTerrain[(int)ActiveLayer.ListMultiplayerSpawns[S].Position.X, (int)ActiveLayer.ListMultiplayerSpawns[S].Position.Y].WorldPosition);
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
