using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.WorldMapScreen
{
    public class SubMapLayer : ISubMapLayer
    {
        public override string ToString()
        {
            return " - Sub Layer";
        }
    }

    public class MapLayer : BaseMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public int StartupDelay;
        public int ToggleDelayOn;
        public int ToggleDelayOff;
        public float Depth;

        public DrawableGrid LayerGrid;
        public readonly WorldMap2D OriginalLayerGrid;
        public Terrain[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private WorldMap Map;

        public MapLayer(WorldMap Map)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();
            ToggleTimer = StartupDelay;
            IsVisible = StartupDelay > 0;

            //Tiles
            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(X, Y);
                }
            }

            LayerGrid = new WorldMap2D(Map);
        }

        public MapLayer(WorldMap Map, BinaryReader BR)
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

            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(BR, X, Y);
                }
            }
            
            LayerGrid = OriginalLayerGrid = new WorldMap2D(Map, BR);
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

            LayerGrid.Update(gameTime);
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            if (IsVisible)
                LayerGrid.BeginDraw(g);
        }

        public void Draw(CustomSpriteBatch g, int LayerIndex)
        {
            if (IsVisible)
            {
                LayerGrid.Draw(g, LayerIndex, ArrayTerrain);
            }
        }

        public void EndDraw(CustomSpriteBatch g)
        {

        }

        public override string ToString()
        {
            return "Layer";
        }
    }
}
