using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class MapLayer : BaseMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public float Depth { get { return _Depth; } set { _Depth = value; } }
        private float _Depth;

        public TerrainConquest[,] ArrayTerrain;//Array of every tile on the map.

        public bool IsVisible;
        private int ToggleTimer;
        private ConquestMap Map;
        public List<UnitSpawn> ListUnitSpawn;
        public List<BuildingSpawn> ListBuildingSpawn;

        public MapLayer(ConquestMap Map, int LayerIndex)
        {
            this.Map = Map;

            ListCampaignSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListTeleportPoint = new List<TeleportPoint>();
            ListUnitSpawn = new List<UnitSpawn>();
            ListBuildingSpawn = new List<BuildingSpawn>();

            ListSubLayer = new List<SubMapLayer>();
            ListProp = new List<InteractiveProp>();
            ListHoldableItem = new List<Core.Units.HoldableItem>();
            ListAttackPickup = new List<Core.Attacks.TemporaryAttackPickup>();
            IsVisible = true;

            //Tiles
            ArrayTerrain = new TerrainConquest[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new TerrainConquest(X, Y, Map.TileSize.X, Map.TileSize.Y, LayerIndex, Map.LayerHeight, _Depth, 1);
                    ArrayTerrain[X, Y].Owner = Map;
                    ArrayTerrain[X, Y].WorldPosition.Z = ArrayTerrain[X, Y].Height + LayerIndex;
                }
            }

            ArrayTile = new DrawableTile[Map.MapSize.X, Map.MapSize.Y];

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(new Rectangle(0, 0, Map.TileSize.X, Map.TileSize.Y), 0);
                }
            }
        }

        public MapLayer(ConquestMap Map, BinaryReader BR, int LayerIndex)
        {
            this.Map = Map;

            ListCampaignSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListTeleportPoint = new List<TeleportPoint>();
            ListUnitSpawn = new List<UnitSpawn>();
            ListBuildingSpawn = new List<BuildingSpawn>();

            ListSubLayer = new List<SubMapLayer>();
            ListProp = new List<InteractiveProp>();
            ListHoldableItem = new List<Core.Units.HoldableItem>();
            ListAttackPickup = new List<Core.Attacks.TemporaryAttackPickup>();

            StartupDelay = BR.ReadInt32();
            ToggleDelayOn = BR.ReadInt32();
            ToggleDelayOff = BR.ReadInt32();
            _Depth = BR.ReadSingle();

            if (StartupDelay == 0)
            {
                IsVisible = true;
                ToggleTimer = ToggleDelayOn;
            }
            else
            {
                IsVisible = false;
                ToggleTimer = StartupDelay;
            }

            ArrayTerrain = new TerrainConquest[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new TerrainConquest(BR, X, Y, Map.TileSize.X, Map.TileSize.Y, LayerIndex, Map.LayerHeight, _Depth);
                    ArrayTerrain[X, Y].Owner = Map;
                    ArrayTerrain[X, Y].WorldPosition.Z = ArrayTerrain[X, Y].Height + LayerIndex;
                }
            }


            ArrayTile = new DrawableTile[Map.MapSize.X, Map.MapSize.Y];
            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y] = new DrawableTile(BR, Map.TileSize.X, Map.TileSize.Y);
                }
            }

            int ListSubLayerCount = BR.ReadInt32();
            ListSubLayer = new List<SubMapLayer>(ListSubLayerCount);
            for (int L = 0; L < ListSubLayerCount; L++)
            {
                ListSubLayer.Add(new SubMapLayer(Map, BR, LayerIndex));
            }

            int ListSingleplayerSpawnsCount = BR.ReadInt32();
            ListCampaignSpawns = new List<EventPoint>(ListSingleplayerSpawnsCount);

            for (int S = 0; S < ListSingleplayerSpawnsCount; S++)
            {
                EventPoint NewPoint = new EventPoint(BR);
                NewPoint.ColorRed = Color.Blue.R;
                NewPoint.ColorGreen = Color.Blue.G;
                NewPoint.ColorBlue = Color.Blue.B;

                ListCampaignSpawns.Add(NewPoint);
            }

            int ListMultiplayerSpawnsCount = BR.ReadInt32();
            ListMultiplayerSpawns = new List<EventPoint>(ListMultiplayerSpawnsCount);

            for (int S = 0; S < ListMultiplayerSpawnsCount; S++)
            {
                EventPoint NewPoint = new EventPoint(BR);
                int ColorIndex = Convert.ToInt32(NewPoint.Tag) - 1;
                /*NewPoint.ColorRed = Map.ListMultiplayerColor[ColorIndex].R;
                NewPoint.ColorGreen = Map.ListMultiplayerColor[ColorIndex].G;
                NewPoint.ColorBlue = Map.ListMultiplayerColor[ColorIndex].B;*/
                ListMultiplayerSpawns.Add(NewPoint);
            }

            int ListMapSwitchPointCount = BR.ReadInt32();
            ListMapSwitchPoint = new List<MapSwitchPoint>(ListMapSwitchPointCount);

            for (int S = 0; S < ListMapSwitchPointCount; S++)
            {
                MapSwitchPoint NewMapSwitchPoint = new MapSwitchPoint(BR);
                ListMapSwitchPoint.Add(NewMapSwitchPoint);
                string SwitchMapType = NewMapSwitchPoint.SwitchMapPath.Split(new string[] { "/" }, StringSplitOptions.None)[0];
                if (BattleMap.DicBattmeMapType.Count > 0 && !string.IsNullOrEmpty(NewMapSwitchPoint.SwitchMapPath)
                    && Map.ListSubMap.Find(x => x.BattleMapPath == NewMapSwitchPoint.SwitchMapPath) == null)
                {
                    string ActiveGameModeName = BR.ReadString();
                    GameModeInfo GameInfo = BattleMap.DicBattmeMapType[SwitchMapType].GetAvailableGameModes()[ActiveGameModeName];
                    GameInfo.Load(BR);
                    ConquestMap NewMap = (ConquestMap)BattleMap.DicBattmeMapType[SwitchMapType].GetNewMap(GameInfo, string.Empty);
                    NewMap.BattleMapPath = NewMapSwitchPoint.SwitchMapPath.Substring(SwitchMapType.Length + 1);
                    NewMap.ListGameScreen = Map.ListGameScreen;
                    NewMap.ListSubMap = Map.ListSubMap;
                    NewMap.Load();
                    foreach (Player ActivePlayer in Map.ListAllPlayer)
                    {
                        NewMap.SharePlayer(ActivePlayer, Map.ListLocalPlayer.Contains(ActivePlayer));
                    }
                }
            }

            int ListTeleportPointCount = BR.ReadInt32();
            ListTeleportPoint = new List<TeleportPoint>(ListTeleportPointCount);

            for (int S = 0; S < ListTeleportPointCount; S++)
            {
                TeleportPoint NewTeleportPoint = new TeleportPoint(BR);
                ListTeleportPoint.Add(NewTeleportPoint);
            }

            int ListUnitSpawnsCount = BR.ReadInt32();
            ListUnitSpawn = new List<UnitSpawn>(ListUnitSpawnsCount);

            for (int S = 0; S < ListUnitSpawnsCount; S++)
            {
                UnitSpawn NewUnitspawn = new UnitSpawn(BR);
                ListUnitSpawn.Add(NewUnitspawn);
            }

            int ListBuildingSpawnsCount = BR.ReadInt32();
            ListBuildingSpawn = new List<BuildingSpawn>(ListBuildingSpawnsCount);

            for (int S = 0; S < ListBuildingSpawnsCount; S++)
            {
                BuildingSpawn NewBuildingSpawn = new BuildingSpawn(BR);
                ListBuildingSpawn.Add(NewBuildingSpawn);
            }

            int ListPropCount = BR.ReadInt32();
            ListProp = new List<InteractiveProp>(ListPropCount);
            for (int L = 0; L < ListPropCount; L++)
            {
                ListProp.Add(Map.DicInteractiveProp[BR.ReadString()].LoadCopy(BR, LayerIndex));
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(StartupDelay);
            BW.Write(ToggleDelayOn);
            BW.Write(ToggleDelayOff);
            BW.Write(Depth);

            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y].Save(BW);
                }
            }

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
            {
                for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                {
                    ArrayTile[X, Y].Save(BW);
                }
            }

            BW.Write(ListSubLayer.Count);
            for (int L = 0; L < ListSubLayer.Count; L++)
            {
                ListSubLayer[L].Save(BW);
            }

            BW.Write(ListCampaignSpawns.Count);
            for (int S = 0; S < ListCampaignSpawns.Count; S++)
            {
                ListCampaignSpawns[S].Save(BW);
            }
            BW.Write(ListMultiplayerSpawns.Count);
            for (int S = 0; S < ListMultiplayerSpawns.Count; S++)
            {
                ListMultiplayerSpawns[S].Save(BW);
            }
            BW.Write(ListMapSwitchPoint.Count);
            for (int S = 0; S < ListMapSwitchPoint.Count; S++)
            {
                ListMapSwitchPoint[S].Save(BW);
            }
            BW.Write(ListTeleportPoint.Count);
            for (int T = 0; T < ListTeleportPoint.Count; T++)
            {
                ListTeleportPoint[T].Save(BW);
            }
            BW.Write(ListUnitSpawn.Count);
            for (int S = 0; S < ListUnitSpawn.Count; S++)
            {
                ListUnitSpawn[S].Save(BW);
            }
            BW.Write(ListBuildingSpawn.Count);
            for (int S = 0; S < ListBuildingSpawn.Count; S++)
            {
                ListBuildingSpawn[S].Save(BW);
            }

            BW.Write(ListProp.Count);
            for (int P = 0; P < ListProp.Count; P++)
            {
                ListProp[P].Save(BW);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (ToggleDelayOn > 0)
            {
                ToggleTimer -= gameTime.ElapsedGameTime.Milliseconds;

                if (ToggleTimer <= 0)
                {
                    IsVisible = !IsVisible;
                    if (IsVisible)
                    {
                        ToggleTimer += ToggleDelayOn;
                    }
                    else
                    {
                        ToggleTimer += ToggleDelayOff;
                    }
                }
            }

            for (int P = 0; P < ListProp.Count; ++P)
            {
                ListProp[P].Update(gameTime);
            }

            foreach (SubMapLayer ActiveSubLayer in ListSubLayer)
            {
                ActiveSubLayer.Update(gameTime);
            }
        }

        public override string ToString()
        {
            return "Layer";
        }

        public override MovementAlgorithmTile GetTile(int X, int Y)
        {
            return ArrayTerrain[X, Y];
        }
    }
}
