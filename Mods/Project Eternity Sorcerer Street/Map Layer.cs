using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.AnimationScreen;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
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
        public TerrainSorcererStreet[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private SorcererStreetMap Map;
        public Map2D OriginalLayerGrid;

        public MapLayer(SorcererStreetMap Map)
        {
            this.Map = Map;

            ListSubLayer = new List<SubMapLayer>();
            ToggleTimer = StartupDelay;
            IsVisible = StartupDelay > 0;

            //Tiles
            ArrayTerrain = new TerrainSorcererStreet[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y);
                }
            }

            OriginalLayerGrid = new SorcererStreetMap2D(Map);
            LayerGrid = new Map3D(Map, this, OriginalLayerGrid, GameScreen.GraphicsDevice);
        }

        public MapLayer(SorcererStreetMap Map, List<AnimationBackground> ListBackgrounds, List<AnimationBackground> ListForegrounds, BinaryReader BR)
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

            ArrayTerrain = new TerrainSorcererStreet[Map.MapSize.X, Map.MapSize.Y];
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    int TerrainTypeIndex = BR.ReadInt32();

                    switch (Map.ListTerrainType[TerrainTypeIndex])
                    {
                        case TerrainSorcererStreet.FireElement:
                        case TerrainSorcererStreet.WaterElement:
                        case TerrainSorcererStreet.EarthElement:
                        case TerrainSorcererStreet.AirElement:
                            ArrayTerrain[X, Y] = new ElementalTerrain(X, Y, TerrainTypeIndex);
                            break;

                        case TerrainSorcererStreet.EastGate:
                        case TerrainSorcererStreet.WestGate:
                        case TerrainSorcererStreet.SouthGate:
                        case TerrainSorcererStreet.NorthGate:
                            ArrayTerrain[X, Y] = new GateTerrain(X, Y, TerrainTypeIndex);
                            break;

                        default:
                            ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, TerrainTypeIndex);
                            break;
                    }
                }
            }

            OriginalLayerGrid = new SorcererStreetMap2D(Map, BR);
            LayerGrid = new Map3D(Map, this, OriginalLayerGrid, GameScreen.GraphicsDevice);
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
