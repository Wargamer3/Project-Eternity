﻿using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
    public class SubMapLayer : BaseMapLayer, ISubMapLayer
    {
        public override string ToString()
        {
            return " - Sub Layer";
        }
    }

    public class MapLayer : BaseMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public float Depth;

        public Map2D LayerGrid;
        public TerrainConquest[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private ConquestMap Map;

        public MapLayer(ConquestMap Map, int LayerIndex)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();
            ToggleTimer = StartupDelay;
            IsVisible = StartupDelay > 0;

            //Tiles
            ArrayTerrain = new TerrainConquest[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new TerrainConquest(X, Y, LayerIndex, Depth);
                }
            }

            LayerGrid = new ConquestMap2D(Map);
        }

        public MapLayer(ConquestMap Map, BinaryReader BR, int LayerIndex)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();

            StartupDelay = BR.ReadInt32();
            ToggleDelayOn = BR.ReadInt32();
            ToggleDelayOff = BR.ReadInt32();
            Depth = BR.ReadSingle();

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
                    ArrayTerrain[X, Y] = new TerrainConquest(BR, X, Y, LayerIndex, Depth);
                }
            }

            LayerGrid = new ConquestMap2D(Map, BR);
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
                        ToggleTimer = ToggleDelayOn;
                    }
                    else
                    {
                        ToggleTimer = ToggleDelayOff;
                    }
                }
            }
        }

        public override string ToString()
        {
            return "Layer";
        }
    }
}
