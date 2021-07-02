using Microsoft.Xna.Framework;

namespace ProjectEternity.Core.Units
{
    public class UnitMap : UnitMapComponent
    {
        public readonly Unit ActiveUnit;

        public override int Width { get { return ActiveUnit.SpriteMap.Width; } }
        public override int Height { get { return ActiveUnit.SpriteMap.Height; } }
        public override bool[,] ArrayMapSize { get { return ActiveUnit.UnitStat.ArrayMapSize; } }
        public override bool IsActive { get { return ActiveUnit.HP > 0; } }

        public UnitMap(Unit ActiveUnit)
        {
            this.ActiveUnit = ActiveUnit;
        }

        public override void Draw2DOnMap(CustomSpriteBatch g, Vector3 Position, int SizeX, int SizeY, Color UnitColor)
        {
            g.Draw(ActiveUnit.SpriteMap, new Rectangle((int)Position.X, (int)Position.Y, SizeX, SizeY), UnitColor);
        }

        public override void DrawExtraOnMap(CustomSpriteBatch g, Vector3 Position, Color UnitColor)
        {
        }

        public override void DrawOverlayOnMap(CustomSpriteBatch g, Vector3 Position)
        {
        }

        public override void DrawTimeOfDayOverlayOnMap(CustomSpriteBatch g, Vector3 Position, int TimeOfDay)
        {
        }
    }
}
