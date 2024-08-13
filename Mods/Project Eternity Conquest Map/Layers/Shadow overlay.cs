using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core.Graphics;
using ProjectEternity.GameScreens.BattleMapScreen;

namespace ProjectEternity.GameScreens.ConquestMapScreen
{
	public class ShadowOverlay
	{
		protected readonly ConquestMap Map;

		protected readonly Dictionary<int, Tile2DHolder> DicTile2DByTileset;

		RenderTarget2D screenShadows;
		private QuadRenderComponent quadRender;
		private ShadowmapResolver shadowmapResolver;

		public ShadowOverlay(ConquestMap Map)
		{
			this.Map = Map;
			DicTile2DByTileset = new Dictionary<int, Tile2DHolder>();
			screenShadows = new RenderTarget2D(GameScreen.GraphicsDevice, GameScreen.GraphicsDevice.Viewport.Width, GameScreen.GraphicsDevice.Viewport.Height);

			quadRender = new QuadRenderComponent(GameScreen.GraphicsDevice);
			quadRender.LoadContent();
			shadowmapResolver = new ShadowmapResolver(GameScreen.GraphicsDevice, quadRender, ShadowmapSize.Size256, ShadowmapSize.Size1024);
			shadowmapResolver.LoadContent(Map.Content);

			for (int L = 0; L < Map.LayerManager.ListLayer.Count; L++)
			{
				CreateMap(Map, Map.LayerManager.ListLayer[L], null);
			}
		}

		protected void CreateMap(ConquestMap Map, MapLayer Owner, Effect WetEffect)
		{
			for (int X = Map.MapSize.X - 1; X >= 0; --X)
			{
				for (int Y = Map.MapSize.Y - 1; Y >= 0; --Y)
				{
					Terrain ActiveTerrain = Owner.ArrayTerrain[X, Y];
					DrawableTile ActiveTile = Owner.ArrayTerrain[X, Y].DrawableTile;

					if (!DicTile2DByTileset.ContainsKey(ActiveTile.TilesetIndex))
					{
						DicTile2DByTileset.Add(ActiveTile.TilesetIndex, new Tile2DHolder(Map.ListTilesetPreset[ActiveTile.TilesetIndex].TilesetName + " WallMap", Map.Content, WetEffect));
					}

					DicTile2DByTileset[ActiveTile.TilesetIndex].AddTile(ActiveTerrain);
				}
			}
		}

        public void BeginDraw(CustomSpriteBatch g)
		{
			foreach (BattleMapLight ActiveLight in Map.ListLight)
			{
				ActiveLight.BeginDrawingShadowCasters(g);

				Vector2 Offset = -new Vector2(ActiveLight.Position.X - ActiveLight.Size * 0.5f, ActiveLight.Position.Y - ActiveLight.Size * 0.5f);

				foreach (Tile2DHolder ActiveTileset in DicTile2DByTileset.Values)
				{
					ActiveTileset.Draw(g, Offset);
				}

				ActiveLight.EndDrawingShadowCasters(g);
				shadowmapResolver.ResolveShadows(ActiveLight.RenderTarget, ActiveLight.RenderTarget);
			}

			g.GraphicsDevice.SetRenderTarget(screenShadows);
			g.GraphicsDevice.Clear(Color.Black);
			g.Begin(SpriteSortMode.Deferred, BlendState.Additive);
			foreach (BattleMapLight ActiveLight in Map.ListLight)
			{
				g.Draw(ActiveLight.RenderTarget, new Vector2(ActiveLight.Position.X - ActiveLight.Size * 0.5f, ActiveLight.Position.Y - ActiveLight.Size * 0.5f), ActiveLight.PrimaryColor);
			}
			g.End();
			g.GraphicsDevice.SetRenderTarget(null);
		}

		public void Draw(CustomSpriteBatch g)
		{
			BlendState blendState = new BlendState();
			blendState.ColorSourceBlend = Blend.DestinationColor;
			blendState.ColorDestinationBlend = Blend.SourceColor;

			g.Begin(SpriteSortMode.Immediate, BlendState.Additive);
			foreach (BattleMapLight ActiveLight in Map.ListLight)
			{
				g.Draw(screenShadows, new Vector2(), Color.FromNonPremultiplied(255, 255, 255, 127));
			}
			g.End();
		}
    }
}
