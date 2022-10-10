﻿using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.BattleMapScreen;
using System;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class MapLayer : BaseMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public float Depth { get { return _Depth; } set { _Depth = value; if (LayerGrid != null) LayerGrid.Depth = value; } }
        private float _Depth;

        public readonly SorcererStreetMap2D LayerGrid;
        public TerrainSorcererStreet[,] ArrayTerrain;//Array of every tile on the map.

        public bool IsVisible;
        private int ToggleTimer;
        private SorcererStreetMap Map;

        public MapLayer(SorcererStreetMap Map, int LayerIndex)
        {
            this.Map = Map;

            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListTeleportPoint = new List<TeleportPoint>();

            ListSubLayer = new List<SubMapLayer>();
            ListProp = new List<InteractiveProp>();
            ListHoldableItem = new List<Core.Units.HoldableItem>();
            ListAttackPickup = new List<Core.Attacks.TemporaryAttackPickup>();
            IsVisible = true;

            //Tiles
            ArrayTerrain = new TerrainSorcererStreet[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, LayerIndex, _Depth);
                    ArrayTerrain[X, Y].Owner = Map;
                    ArrayTerrain[X, Y].WorldPosition.Z = ArrayTerrain[X, Y].Height + LayerIndex;
                }
            }

            LayerGrid = new SorcererStreetMap2D(Map, this);
            _Depth = LayerGrid.Depth;
        }

        public MapLayer(SorcererStreetMap Map, BinaryReader BR, int LayerIndex)
        {
            this.Map = Map;

            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();
            ListTeleportPoint = new List<TeleportPoint>();

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

            ArrayTerrain = new TerrainSorcererStreet[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    byte TerrainTypeIndex = BR.ReadByte();
                    float Height = BR.ReadSingle();

                    switch (Map.ListTerrainType[TerrainTypeIndex])
                    {
                        case TerrainSorcererStreet.FireElement:
                        case TerrainSorcererStreet.WaterElement:
                        case TerrainSorcererStreet.EarthElement:
                        case TerrainSorcererStreet.AirElement:
                            ArrayTerrain[X, Y] = new ElementalTerrain(X, Y, LayerIndex, Depth, TerrainTypeIndex);
                            break;

                        case TerrainSorcererStreet.EastGate:
                        case TerrainSorcererStreet.WestGate:
                        case TerrainSorcererStreet.SouthGate:
                        case TerrainSorcererStreet.NorthGate:
                            ArrayTerrain[X, Y] = new GateTerrain(X, Y, LayerIndex, Depth, TerrainTypeIndex);
                            break;

                        default:
                            ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, LayerIndex, Depth, TerrainTypeIndex);
                            break;
                    }

                    ArrayTerrain[X, Y].Owner = Map;
                    ArrayTerrain[X, Y].Height = Height;
                    ArrayTerrain[X, Y].WorldPosition.Z = ArrayTerrain[X, Y].Height + LayerIndex;
                }
            }

            LayerGrid = new SorcererStreetMap2D(Map, this, BR);
            int ListSubLayerCount = BR.ReadInt32();
            ListSubLayer = new List<SubMapLayer>(ListSubLayerCount);
            for (int L = 0; L < ListSubLayerCount; L++)
            {
                ListSubLayer.Add(new SubMapLayer(Map, BR, LayerIndex));
            }

            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y].DrawableTile = LayerGrid.ArrayTile[X, Y];
                }
            }

            int ListSingleplayerSpawnsCount = BR.ReadInt32();
            ListSingleplayerSpawns = new List<EventPoint>(ListSingleplayerSpawnsCount);

            for (int S = 0; S < ListSingleplayerSpawnsCount; S++)
            {
                EventPoint NewPoint = new EventPoint(BR);
                NewPoint.ColorRed = Color.Blue.R;
                NewPoint.ColorGreen = Color.Blue.G;
                NewPoint.ColorBlue = Color.Blue.B;

                ListSingleplayerSpawns.Add(NewPoint);
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
                    SorcererStreetMap NewMap = (SorcererStreetMap)BattleMap.DicBattmeMapType[SwitchMapType].GetNewMap(string.Empty, string.Empty);
                    NewMap.BattleMapPath = NewMapSwitchPoint.SwitchMapPath.Substring(SwitchMapType.Length + 1);
                    NewMap.ListGameScreen = Map.ListGameScreen;
                    NewMap.ListSubMap = Map.ListSubMap;
                    NewMap.Load();
                }
            }

            int ListTeleportPointCount = BR.ReadInt32();
            ListTeleportPoint = new List<TeleportPoint>(ListTeleportPointCount);

            for (int S = 0; S < ListTeleportPointCount; S++)
            {
                TeleportPoint NewTeleportPoint = new TeleportPoint(BR);
                ListTeleportPoint.Add(NewTeleportPoint);
            }

            int ListPropCount = BR.ReadInt32();
            ListProp = new List<InteractiveProp>(ListPropCount);
            for (int L = 0; L < ListPropCount; L++)
            {
                ListProp.Add(Map.DicInteractiveProp[BR.ReadString()].LoadCopy(BR, LayerIndex));
            }

            LayerGrid.Depth = Depth;
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

            LayerGrid.Save(BW);
            BW.Write(ListSubLayer.Count);
            for (int L = 0; L < ListSubLayer.Count; L++)
            {
                ListSubLayer[L].Save(BW);
            }

            BW.Write(ListSingleplayerSpawns.Count);
            for (int S = 0; S < ListSingleplayerSpawns.Count; S++)
            {
                ListSingleplayerSpawns[S].Save(BW);
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
    }
}
