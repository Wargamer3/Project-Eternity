using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MapLayer : IMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public int StartupDelay;
        public int ToggleDelayOn;
        public int ToggleDelayOff;
        public float Depth { get { return _Depth; } set { _Depth = value; if (OriginalLayerGrid != null) OriginalLayerGrid.Depth = value; } }
        private float _Depth;

        public DrawableGrid LayerGrid;
        public readonly DeathmatchMap2D OriginalLayerGrid;
        public Terrain[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private DeathmatchMap Map;

        public MapLayer(DeathmatchMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();
            IsVisible = true;

            //Tiles
            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(X, Y);
                }
            }

            LayerGrid = OriginalLayerGrid = new DeathmatchMap2D(Map, ListBackgrounds, ListForegrounds);
            _Depth = OriginalLayerGrid.Depth;
        }

        public MapLayer(DeathmatchMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds, BinaryReader BR)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();

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

            ArrayTerrain = new Terrain[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new Terrain(BR, X, Y);
                }
            }

            LayerGrid = OriginalLayerGrid = new DeathmatchMap2D(Map, ListBackgrounds, ListForegrounds, BR);
            OriginalLayerGrid.Depth = Depth;
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
                        ToggleTimer += ToggleDelayOn;
                    }
                    else
                    {
                        ToggleTimer += ToggleDelayOff;
                    }
                }
            }

            LayerGrid.Update(gameTime);
        }

        public void ResetGrid()
        {
            LayerGrid.Reset();
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            if (IsVisible)
                LayerGrid.BeginDraw(g);
        }

        public void Draw(CustomSpriteBatch g)
        {
            if (IsVisible)
                LayerGrid.Draw(g);
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
