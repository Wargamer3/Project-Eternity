using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
	public class DefaultWeather : BattleMapOverlay
	{
		protected readonly ConquestMap Map;

		protected readonly Dictionary<int, Tile2DHolder> DicTile2DByTileset;
        protected readonly Dictionary<int, Dictionary<int, Tile2DHolder>> DicTile2DByLayerByTileset;

        public DefaultWeather(ConquestMap Map, ZoneShape Shape)
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

		public void CreateMap(ConquestMap Map, MapLayer Owner, int LayerIndex, Effect WetEffect)
        {
            foreach (SubMapLayer ActiveSubLayer in Owner.ListSubLayer)
            {
                CreateMap(Map, ActiveSubLayer, LayerIndex, WetEffect);
            }

            if (!DicTile2DByLayerByTileset.ContainsKey(LayerIndex))
            {
                DicTile2DByLayerByTileset.Add(LayerIndex, new Dictionary<int, Tile2DHolder>());
            }

            for (int X = Map.MapSize.X - 1; X >= 0; --X)
			{
				for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
				{
					Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
                    DrawableTile ActiveTile = Owner.LayerGrid.ArrayTile[X, Y];

                    if (!DicTile2DByTileset.ContainsKey(ActiveTile.TilesetIndex))
					{
						DicTile2DByTileset.Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetName, Map.Content, WetEffect));
                    }
                    if (!DicTile2DByLayerByTileset[LayerIndex].ContainsKey(ActiveTile.TilesetIndex))
                    {
                        DicTile2DByLayerByTileset[LayerIndex].Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetName, Map.Content, WetEffect));
                    }

                    DicTile2DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTile.Origin, ActiveTerrain.WorldPosition);
                    DicTile2DByLayerByTileset[LayerIndex][ActiveTile.TilesetIndex].AddTile(ActiveTile.Origin, ActiveTerrain.WorldPosition);
                }
			}
		}

        public void Reset()
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
                            ActiveTileSet.Value.Draw(g, Map, L);
                        }
                    }
                }
                else
                {
                    int MaxLayerIndex = Map.LayerManager.ListLayer.Count;

                    for (int L = 0; L < MaxLayerIndex; L++)
                    {
                        MapLayer ActiveLayer = Map.LayerManager.ListLayer[L];
                        if (!ActiveLayer.IsVisible)
                        {
                            continue;
                        }

                        foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[L])
                        {
                            ActiveTileSet.Value.Draw(g, Map, L);
                        }
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<int, Tile2DHolder> ActiveTileSet in DicTile2DByLayerByTileset[Map.ShowLayerIndex])
                {
                    ActiveTileSet.Value.Draw(g, Map, Map.ShowLayerIndex);
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
