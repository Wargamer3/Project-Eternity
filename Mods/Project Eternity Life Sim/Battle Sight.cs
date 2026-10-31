using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class BattleSight
    {
        public const float HalfCone = 50f * 3.14159265f / 180f;

        public static bool CanSee(Vector3 From, Vector3 Facing, Vector3 Target, IList<Point> Walls, float Tile)
        {
            if (Tile <= 0f)
            {
                Tile = 32f;
            }

            Vector3 To = Target - From;
            To.Z = 0f;
            if (To.LengthSquared() < 0.01f)
            {
                return true;
            }

            Vector3 Direction = Vector3.Normalize(To);
            Vector3 Face = Facing;
            Face.Z = 0f;
            if (Face.LengthSquared() < 0.01f)
            {
                Face = Vector3.UnitY;
            }
            else
            {
                Face.Normalize();
            }

            if (Vector3.Dot(Face, Direction) < (float)System.Math.Cos(HalfCone))
            {
                return false;
            }

            if (Walls == null)
            {
                return true;
            }

            int X0 = (int)(From.X / Tile);
            int Y0 = (int)(From.Y / Tile);
            int X1 = (int)(Target.X / Tile);
            int Y1 = (int)(Target.Y / Tile);
            int Steps = System.Math.Max(System.Math.Abs(X1 - X0), System.Math.Abs(Y1 - Y0));
            for (int i = 1; i <= Steps; i++)
            {
                int X = X0 + (X1 - X0) * i / Steps;
                int Y = Y0 + (Y1 - Y0) * i / Steps;
                for (int w = 0; w < Walls.Count; w++)
                {
                    if (Walls[w].X == X && Walls[w].Y == Y)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static void Apply(PlayerCharacter Viewer, IList<PlayerCharacter> Others, IList<Point> Walls, float Tile, Vector3 Facing)
        {
            if (Viewer != null)
            {
                Viewer.HiddenFromBattle = false;
            }

            if (Others == null)
            {
                return;
            }

            for (int i = 0; i < Others.Count; i++)
            {
                PlayerCharacter Other = Others[i];
                if (Other == null || Other == Viewer)
                {
                    continue;
                }

                Other.HiddenFromBattle = Viewer == null || !CanSee(Viewer.Position, Facing, Other.Position, Walls, Tile);
            }
        }
    }
}
