using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
	public class WetWeather : BattleMapOverlay
	{
		BattleMap Map;
		Texture2D BumpMap;
		Effect WetEffect;

		public WetWeather(BattleMap Map, ZoneShape Shape)
		{
			this.Map = Map;
		}

		public void Init()
		{
			BumpMap = Map.Content.Load<Texture2D>("Maps/Tilesets/RAD NormalMap");

			WetEffect = Map.Content.Load<Effect>("Shaders/Bump Map");
			Matrix View = Matrix.Identity;

			Matrix Projection = Matrix.CreateOrthographicOffCenter(0, Constants.Width, Constants.Width, 0, 0, 1);
			Matrix HalfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

			Projection = View * (HalfPixelOffset * Projection);

			WetEffect.Parameters["NormalMap"].SetValue(BumpMap);
			WetEffect.Parameters["View"].SetValue(View);
			WetEffect.Parameters["Projection"].SetValue(Projection);
			WetEffect.Parameters["World"].SetValue(Matrix.Identity);
			WetEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Invert(Matrix.Identity));
		}

		public void Reset()
		{
		}

		public void Update(GameTime gameTime)
		{
		}

		public void BeginDraw(CustomSpriteBatch g)
		{
		}

		public void Draw(CustomSpriteBatch g)
		{
		}

		public void EndDraw(CustomSpriteBatch g)
		{
		}

		public void SetCrossfadeValue(double Value)
		{
		}
    }
}
