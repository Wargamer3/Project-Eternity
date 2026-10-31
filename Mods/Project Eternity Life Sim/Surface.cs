using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum SurfaceKind
    {
        None,
        Grass,
        Dirt,
        Rock,
        Snow,
        Water,
    }

    public struct GroundCover
    {
        public bool Hit;
        public MapInfo Chunk;
        public float Height;
        public float Slope;
        public Vector4 Blend;
        public SurfaceKind Kind;
        public float Step;
    }

    public static class SurfaceData
    {
        public static GroundCover At(float WorldX, float WorldZ)
        {
            GroundCover Cover = new GroundCover();
            Cover.Kind = SurfaceKind.Dirt;
            Cover.Step = 1f;
            MapInfo Info = ChunkCross.At(WorldX, WorldZ);
            if (Info == null || Info.Heights == null)
            {
                return Cover;
            }

            Vector3 Shift = ChunkOrigin.Shift(Info);
            float LocalX = WorldX - Shift.X;
            float LocalZ = WorldZ - Shift.Z;
            float Height;
            float SlopeX;
            float SlopeY;
            if (!Info.Heights.Sample(LocalX, LocalZ, out Height, out SlopeX, out SlopeY))
            {
                return Cover;
            }

            Cover.Hit = true;
            Cover.Chunk = Info;
            Cover.Height = Height;
            Cover.Slope = (float)System.Math.Sqrt(SlopeX * SlopeX + SlopeY * SlopeY);
            Cover.Blend = Info.Heights.ReadBlend(LocalX, LocalZ);
            if (WaterPlace.Over(Info, LocalX, LocalZ) || Ocean.Covers(Height))
            {
                Cover.Kind = SurfaceKind.Water;
            }
            else
            {
                Cover.Kind = Dominant(Cover.Blend);
            }

            Cover.Step = StepOf(Cover.Kind);
            return Cover;
        }

        public static SurfaceKind Dominant(Vector4 Blend)
        {
            if (Blend.W >= Blend.X && Blend.W >= Blend.Y && Blend.W >= Blend.Z && Blend.W > 0.35f)
            {
                return SurfaceKind.Snow;
            }

            if (Blend.Z >= Blend.X && Blend.Z >= Blend.Y && Blend.Z > 0.35f)
            {
                return SurfaceKind.Rock;
            }

            if (Blend.Y >= Blend.X)
            {
                return SurfaceKind.Dirt;
            }

            return SurfaceKind.Grass;
        }

        public static float StepOf(SurfaceKind Kind)
        {
            switch (Kind)
            {
                case SurfaceKind.Grass:
                    return 0.72f;
                case SurfaceKind.Dirt:
                    return 0.9f;
                case SurfaceKind.Rock:
                    return 1.2f;
                case SurfaceKind.Snow:
                    return 0.42f;
                case SurfaceKind.Water:
                    return 0.3f;
                default:
                    return 1f;
            }
        }
    }
}
