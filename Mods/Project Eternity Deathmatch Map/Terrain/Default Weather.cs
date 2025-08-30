using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;
using static ProjectEternity.GameScreens.BattleMapScreen.Terrain;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
	public class DefaultWeather : BattleMapOverlay
	{
		protected readonly DeathmatchMap Map;

		protected readonly Dictionary<int, Tile2DHolder> DicTile2DByTileset;
        protected readonly Dictionary<int, Dictionary<int, Tile2DHolder>> DicTile2DByLayerByTileset;

        public DefaultWeather(DeathmatchMap Map, ZoneShape Shape)
		{
			this.Map = Map;
			DicTile2DByTileset = new Dictionary<int, Tile2DHolder>();
            DicTile2DByLayerByTileset = new Dictionary<int, Dictionary<int, Tile2DHolder>>();

            if (Map.ListTilesetPreset.Count > 0)
			{
				for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
				{
					CreateMap(Map, Map.LayerManager.ListLayer[L], L, null);
				}
			}
		}

		public void CreateMap(DeathmatchMap Map, MapLayer Owner, int LayerIndex, Effect WetEffect)
        {
            foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
            {
                CreateMap(Map, ActiveSubLayer, LayerIndex, WetEffect);
            }

            if (!DicTile2DByLayerByTileset.ContainsKey(LayerIndex))
            {
                DicTile2DByLayerByTileset.Add(LayerIndex, new Dictionary<int, Tile2DHolder>());
            }

            if (Map.ListTileSet.Count > 0)
            {
                for (int X = Map.MapSize.X - 1; X >= 0; --X)
                {
                    for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
                    {
                        Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
                        DrawableTile ActiveTile = Owner.ArrayTile[X, Y];

                        if (!DicTile2DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                        {
                            if (Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetType == TilesetPreset.TilesetTypes.Regular)
                            {
                                DicTile2DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex], Map.Content, WetEffect, "Tilesets/" + Map.ListTilesetPreset[ActiveTile.TilesetIndex].ArrayTilesetInformation[0].TilesetName));
                            }
                            else
                            {
                                DicTile2DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex], Map.Content, WetEffect, "Autotiles/" + Map.ListTilesetPreset[ActiveTile.TilesetIndex].ArrayTilesetInformation[0].TilesetName));
                            }
                        }

                        if (!DicTile2DByTileset.ContainsKey(ActiveTile.TilesetIndex))
                        {
                            DicTile2DByTileset.Add(ActiveTile.TilesetIndex, DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex]);
                        }

                        if (ActiveTile.ArraySubTile.Length == 0)
                        {
                            DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile.Origin, ActiveTerrain.WorldPosition);
                        }
                        else
                        {
                            float OffsetSize = 1f / (ActiveTile.ArraySubTile.Length / 2f);
                            for (int T = 0; T < ActiveTile.ArraySubTile.Length; T++)
                            {
                                Vector3 TilePosition = ActiveTerrain.WorldPosition;
                                TilePosition.X += OffsetSize * (T % (ActiveTile.ArraySubTile.Length / 2)) * Map.TileSize.X;
                                TilePosition.Y += OffsetSize * (T / (ActiveTile.ArraySubTile.Length / 2)) * Map.TileSize.Y;
                                DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile.ArraySubTile[T], TilePosition);
                            }
                        }
                    }
                }
            }
        }

        public virtual void Reset()
        {
            DicTile2DByTileset.Clear();
            DicTile2DByLayerByTileset.Clear();

            for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, Map.LayerManager.ListLayer[L], L, null);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByTileset)
            {
                ActiveTileSet.Value.Update(gameTime);
            }
        }

        public virtual void BeginDraw(CustomSpriteBatch g)
        {
            g.End();

            g.GraphicsDevice.SetRenderTarget(Map.MapRenderTarget);
            g.GraphicsDevice.Clear(Color.White);

            if (Map.ShowLayerIndex == -1)
            {
                if (Map.IsEditor)
                {
                    for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
                    {
                        foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.Draw(g, Map, Map.LayerManager.ListLayer[L].Depth);
                        }
                    }
                }
                else
                {
                    int MaxLayerIndex = Map.LayerManager.ListLayer.Count;

                    for (int L = 0; L < MaxLayerIndex; L++)
                    {
                        foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.Draw(g, Map, Map.LayerManager.ListLayer[L].Depth);
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[Map.ShowLayerIndex])
                {
                    ActiveTileSet.Value.Draw(g, Map, 0);
                }
            }

            g.Begin();
        }

        public virtual void Draw(CustomSpriteBatch g)
		{
			g.Draw(Map.MapRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
		}

        public virtual void EndDraw(CustomSpriteBatch g)
        {

        }

		public virtual void SetCrossfadeValue(double Value)
        {

        }
    }
}
