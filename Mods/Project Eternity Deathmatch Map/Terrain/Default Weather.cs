using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
	public class DefaultWeather : BattleMapOverlay
	{
		protected readonly DeathmatchMap Map;

		protected readonly Dictionary<int, Tile2DHolder> DicTile2DByTileset;

		public DefaultWeather(DeathmatchMap Map, ZoneShape Shape)
		{
			this.Map = Map;
			DicTile2DByTileset = new Dictionary<int, Tile2DHolder>();

			if (Map.ListTilesetPreset.Count > 0)
			{
				for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
				{
					CreateMap(Map, Map.LayerManager.ListLayer[L], null);
				}
			}
		}

		public void CreateMap(DeathmatchMap Map, MapLayer Owner, Effect WetEffect)
		{
			for (int X = Map.MapSize.X - 1; X >= 0; --X)
			{
				for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
				{
					Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
					DrawableTile ActiveTile = Owner.ArrayTerrain[X, Y].DrawableTile;

					if (!DicTile2DByTileset.ContainsKey(ActiveTile.TilesetIndex))
					{
						DicTile2DByTileset.Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetName, Map.Content, WetEffect));
					}

					DicTile2DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTerrain);
				}
			}
		}

        public void Reset()
        {
            DicTile2DByTileset.Clear();

            for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
            {
                CreateMap(Map, Map.LayerManager.ListLayer[L], null);
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
            foreach (Tile2DHolder ActiveTileset in DicTile2DByTileset.Values)
            {
                ActiveTileset.Draw(g);
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
