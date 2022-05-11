using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
	public abstract class DefaultWeather : BattleMapOverlay
	{
		protected readonly DeathmatchMap Map;

		protected readonly Dictionary<int, Tile2DHolder> DicTile3DByTileset;

		public DefaultWeather(DeathmatchMap Map, ZoneShape Shape)
		{
			this.Map = Map;
			DicTile3DByTileset = new Dictionary<int, Tile2DHolder>();

			for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
			{
				CreateMap(Map, Map.LayerManager.ListLayer[L], null);
			}
		}

		protected void CreateMap(DeathmatchMap Map, MapLayer Owner, Effect WetEffect)
		{
			for (int X = Map.MapSize.X - 1; X >= 0; --X)
			{
				for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
				{
					Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
					DrawableTile ActiveTile = Owner.ArrayTerrain[X, Y].DrawableTile;

					if (!DicTile3DByTileset.ContainsKey(ActiveTile.TilesetIndex))
					{
						DicTile3DByTileset.Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetName, Map.Content, WetEffect));
					}

					DicTile3DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTerrain);
				}
			}
		}

		public void DrawMap(CustomSpriteBatch g)
		{
			g.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			g.GraphicsDevice.SetRenderTarget(Map.MapRenderTarget);
			g.GraphicsDevice.Clear(Color.Black);

			foreach (BattleMapPlatform ActivePlatform in Map.ListPlatform)
			{
				ActivePlatform.Draw(g);
			}

			foreach (Tile2DHolder ActiveTileset in DicTile3DByTileset.Values)
			{
				ActiveTileset.Draw(g);
			}

			g.End();
		}

        public abstract void Update(GameTime gameTime);
        public abstract void BeginDraw(CustomSpriteBatch g);
        public abstract void Draw(CustomSpriteBatch g);
        public abstract void EndDraw(CustomSpriteBatch g);
		public abstract void SetCrossfadeValue(double Value);
    }
}
