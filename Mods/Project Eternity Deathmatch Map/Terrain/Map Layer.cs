using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    public class MapLayer : BaseMapLayer
    {
        public List<SubMapLayer> ListSubLayer;
        public float Depth { get { return _Depth; } set { _Depth = value; if (OriginalLayerGrid != null) OriginalLayerGrid.Depth = value; } }
        private float _Depth;

        public DrawableGrid LayerGrid;
        public readonly DeathmatchMap2D OriginalLayerGrid;
        public Terrain[,] ArrayTerrain;//Array of every tile on the map.

        private bool IsVisible;
        private int ToggleTimer;
        private DeathmatchMap Map;

        public MapLayer(DeathmatchMap Map)
        {
            this.Map = Map;

            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();

            ListSubLayer = new List<SubMapLayer>();
            ListProp = new List<InteractiveProp>();
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

            LayerGrid = OriginalLayerGrid = new DeathmatchMap2D(Map);
            _Depth = OriginalLayerGrid.Depth;
        }

        public MapLayer(DeathmatchMap Map, BinaryReader BR)
        {
            this.Map = Map;

            ListSingleplayerSpawns = new List<EventPoint>();
            ListMultiplayerSpawns = new List<EventPoint>();
            ListMapSwitchPoint = new List<MapSwitchPoint>();

            ListSubLayer = new List<SubMapLayer>();
            ListProp = new List<InteractiveProp>();

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
                if (BattleMap.DicBattmeMapType.Count > 0 && !string.IsNullOrEmpty(NewMapSwitchPoint.SwitchMapPath)
                    && Map.ListSubMap.Find(x => x.BattleMapPath == NewMapSwitchPoint.SwitchMapPath) == null)
                {
                    BattleMap NewMap = BattleMap.DicBattmeMapType[NewMapSwitchPoint.SwitchMapType].GetNewMap(string.Empty);
                    NewMap.BattleMapPath = NewMapSwitchPoint.SwitchMapPath;
                    NewMap.ListGameScreen = Map.ListGameScreen;
                    NewMap.ListSubMap = Map.ListSubMap;
                    NewMap.Load();
                }
            }

            int ListPropCount = BR.ReadInt32();
            ListProp = new List<InteractiveProp>(ListPropCount);
            for (int L = 0; L < ListPropCount; L++)
            {
                ListProp.Add(Map.DicInteractiveProp[BR.ReadString()].LoadCopy(BR));
            }

            LayerGrid = OriginalLayerGrid = new DeathmatchMap2D(Map, BR);
            int ListSubLayerCount = BR.ReadInt32();
            ListSubLayer = new List<SubMapLayer>(ListSubLayerCount);
            for (int L = 0; L < ListSubLayerCount; L++)
            {
                ListSubLayer.Add(new SubMapLayer(Map, BR));
            }
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

            BW.Write(ListProp.Count);
            for (int P = 0; P < ListProp.Count; P++)
            {
                ListProp[P].Save(BW);
            }

            LayerGrid.Save(BW);
            BW.Write(ListSubLayer.Count);
            for (int L = 0; L < ListSubLayer.Count; L++)
            {
                ListSubLayer[L].Save(BW);
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
            LayerGrid.Update(gameTime);
            foreach (SubMapLayer ActiveSubLayer in ListSubLayer)
            {
                ActiveSubLayer.Update(gameTime);
            }
        }

        public void ResetGrid()
        {
            LayerGrid.Reset();
        }

        public void BeginDraw(CustomSpriteBatch g)
        {
            if (IsVisible)
            {
                LayerGrid.BeginDraw(g);
                foreach (SubMapLayer ActiveSubLayer in ListSubLayer)
                {
                    ActiveSubLayer.BeginDraw(g);
                }
            }
        }

        public void Draw(CustomSpriteBatch g, int LayerIndex, bool IsSubLayer)
        {
            if (IsVisible)
            {
                LayerGrid.Draw(g, LayerIndex, IsSubLayer, ArrayTerrain);

                if (Map.ShowTerrainType)
                {
                    int IndexOfLayer = LayerIndex;
                    if (Map.ShowLayerIndex >= 0 && IndexOfLayer != -1)
                    {
                        IndexOfLayer = 0;
                    }
                    else if (IndexOfLayer == -1)
                    {
                        IndexOfLayer = 3;
                    }
                    float XOffset = (IndexOfLayer % 3) * Map.TileSize.X / 3;
                    float YOffset = (IndexOfLayer / 3) * Map.TileSize.Y / 3;
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                    {
                        for (int X = 0; X < Map.MapSize.X; X++)
                        {
                            Color TextColor = Color.White;
                            switch (ArrayTerrain[X, Y].TerrainTypeIndex)
                            {
                                case 0:
                                    TextColor = Color.DeepSkyBlue;
                                    break;
                                case 1:
                                    TextColor = Color.White;
                                    break;
                                case 2:
                                    TextColor = Color.Navy;
                                    break;
                                case 3:
                                    TextColor = Color.DarkGray;
                                    break;
                                case 4:
                                    TextColor = Color.Red;
                                    break;
                                case 5:
                                    TextColor = Color.Yellow;
                                    break;
                            }
                            TextHelper.DrawText(g, ArrayTerrain[X, Y].TerrainTypeIndex.ToString(),
                                new Vector2((X - Map.CameraPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.CameraPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
                        }
                    }
                }

                if (Map.ShowTerrainHeight)
                {
                    int IndexOfLayer = Map.ListLayer.IndexOf(this);
                    if (Map.ShowLayerIndex >= 0)
                    {
                        IndexOfLayer = 0;
                    }
                    float XOffset = (IndexOfLayer % 3) * Map.TileSize.X / 3;
                    float YOffset = (IndexOfLayer / 3) * Map.TileSize.Y / 3;
                    for (int Y = 0; Y < Map.MapSize.Y; Y++)
                    {
                        for (int X = 0; X < Map.MapSize.X; X++)
                        {
                            Color TextColor = Color.White;
                            if (ArrayTerrain[X, Y].Position.Z >= 2)
                            {
                                TextColor = Color.Red;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z >= 1)
                            {
                                TextColor = Color.Orange;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z >= 0.75)
                            {
                                TextColor = Color.Yellow;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z >= 0.5)
                            {
                                TextColor = Color.Green;
                            }
                            else if (ArrayTerrain[X, Y].Position.Z > 0)
                            {
                                TextColor = Color.SkyBlue;
                            }

                            TextHelper.DrawText(g, ArrayTerrain[X, Y].Position.Z.ToString(),
                                new Vector2((X - Map.CameraPosition.X) * Map.TileSize.X + XOffset,
                                (Y - Map.CameraPosition.Y) * Map.TileSize.Y + YOffset), TextColor);
                        }
                    }
                }

                foreach (SubMapLayer ActiveSubLayer in ListSubLayer)
                {
                    ActiveSubLayer.Draw(g, LayerIndex, true);
                }

                for (int P = 0; P < ListProp.Count; ++P)
                {
                    ListProp[P].Draw(g);
                }

                if (!Map.ShowUnits)
                {
                    Color BrushPlayer = Color.FromNonPremultiplied(30, 144, 255, 180);
                    Color BrushEnemy = Color.FromNonPremultiplied(255, 0, 0, 180);
                    Color BrushNeutral = Color.FromNonPremultiplied(255, 255, 0, 180);
                    Color BrushAlly = Color.FromNonPremultiplied(191, 255, 0, 180);
                    Color BrushMapSwitchEventPoint = Color.FromNonPremultiplied(191, 255, 0, 180);

                    for (int i = 0; i < ListSingleplayerSpawns.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(ListSingleplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                      (int)(ListSingleplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y),
                                                      null,
                                        BrushPlayer, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, ListSingleplayerSpawns[i].Tag,
                            new Vector2((ListSingleplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (ListSingleplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int i = 0; i < ListMultiplayerSpawns.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(ListMultiplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                      (int)(ListMultiplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushPlayer, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, ListMultiplayerSpawns[i].Tag,
                            new Vector2((ListMultiplayerSpawns[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (ListMultiplayerSpawns[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }

                    for (int i = 0; i < ListMapSwitchPoint.Count; i++)
                    {
                        g.Draw(GameScreen.sprPixel, new Rectangle((int)(ListMapSwitchPoint[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X,
                                                       (int)(ListMapSwitchPoint[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y,
                                                       Map.TileSize.X, Map.TileSize.Y), null,
                                        BrushMapSwitchEventPoint, 0f, Vector2.Zero, SpriteEffects.None, 0.001f);

                        g.DrawString(Map.fntArial9, ListMapSwitchPoint[i].Tag,
                            new Vector2((ListMapSwitchPoint[i].Position.X - Map.CameraPosition.X) * Map.TileSize.X + 10,
                                        (ListMapSwitchPoint[i].Position.Y - Map.CameraPosition.Y) * Map.TileSize.Y + 10),
                            Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                    }
                }
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
