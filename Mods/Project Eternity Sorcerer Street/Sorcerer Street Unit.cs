using System;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    public class SorcererStreetUnit : UnitMapComponent
    {
        public string SpriteMapPath;
        public Texture2D SpriteMap;
        public UnitMap3D Unit3DSprite;
        public string Model3DPath;
        public AnimatedModel Unit3DModel;
        internal UnitStats UnitStat;

        public override int Width => 32;

        public override int Height => 32;

        public override bool IsActive => throw new System.NotImplementedException();

        public override bool[,] ArrayMapSize { get { return new bool[,] { { true } }; } }

        public SorcererStreetUnit()
        {
            Direction = DirectionDown;
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 PositionY, Color UnitColor)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
            throw new System.NotImplementedException();
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
            throw new System.NotImplementedException();
        }

        public override void Draw3DOnMap(GraphicsDevice GraphicsDevice, Matrix View, Matrix Projection)
        {
            throw new NotImplementedException();
        }

        internal void KillUnit()
        {
            throw new NotImplementedException();
        }
    }
}
