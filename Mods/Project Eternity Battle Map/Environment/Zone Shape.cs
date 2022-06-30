using Microsoft.Xna.Framework;
using System.IO;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class ZoneShape
    {
        public enum ZoneShapeTypes { Full, Rectangle, Oval }

        public ZoneShapeTypes ZoneShapeType;
        public Vector2 Position;
        public Vector2 Size;//Used if Rectangle
        public float Radius;//Used if Oval

        public ZoneShape(ZoneShapeTypes ZoneShapeType)
        {
            this.ZoneShapeType = ZoneShapeType;
        }

        public ZoneShape(ZoneShapeTypes ZoneShapeType, Vector2 Position, Vector2 Size, float Radius)
        {
            this.ZoneShapeType = ZoneShapeType;
            this.Position = Position;
            this.Size = Size;
            this.Radius = Radius;
        }

        public ZoneShape(BinaryReader BR)
        {
            ZoneShapeType = (ZoneShapeTypes)BR.ReadByte();

            if (ZoneShapeType == ZoneShapeTypes.Rectangle)
            {
                Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                Size = new Vector2(BR.ReadSingle(), BR.ReadSingle());
            }
            else if (ZoneShapeType == ZoneShapeTypes.Oval)
            {
                Position = new Vector2(BR.ReadSingle(), BR.ReadSingle());
                Radius = BR.ReadSingle();
            }
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write((byte)ZoneShapeType);

            if (ZoneShapeType == ZoneShapeTypes.Rectangle)
            {
                BW.Write(Position.X);
                BW.Write(Position.Y);
                BW.Write(Size.X);
                BW.Write(Size.Y);
            }
            else if (ZoneShapeType == ZoneShapeTypes.Oval)
            {
                BW.Write(Position.X);
                BW.Write(Position.Y);
                BW.Write(Radius);
            }
        }
    }
}
