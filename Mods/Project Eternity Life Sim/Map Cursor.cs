using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class MapCursor
    {
        public const float Speed = 160f;

        public static float TileOf(LifeSimMap Map)
        {
            if (Map != null && Map.TileSize.X > 0)
            {
                return Map.TileSize.X;
            }

            return 32f;
        }

        public static Vector3 Slide(Vector3 Cursor, float Delta, bool Up, bool Down, bool Left, bool Right)
        {
            float Step = Speed * Delta;
            if (Up)
            {
                Cursor.Z -= Step;
            }
            else if (Down)
            {
                Cursor.Z += Step;
            }

            if (Left)
            {
                Cursor.X -= Step;
            }
            else if (Right)
            {
                Cursor.X += Step;
            }

            return Cursor;
        }

        public static Vector3 Clamp(Vector3 Ground, LifeSimMap Map, float Tile)
        {
            if (Map == null)
            {
                return Ground;
            }

            float MaxX = Map.MapSize.X > 0 ? Map.MapSize.X * Tile - 0.01f : 0f;
            float MaxY = Map.MapSize.Y > 0 ? Map.MapSize.Y * Tile - 0.01f : 0f;
            if (Ground.X < 0f)
            {
                Ground.X = 0f;
            }
            else if (Ground.X > MaxX)
            {
                Ground.X = MaxX;
            }

            if (Ground.Y < 0f)
            {
                Ground.Y = 0f;
            }
            else if (Ground.Y > MaxY)
            {
                Ground.Y = MaxY;
            }

            return Ground;
        }

        public static void ShowTile(LifeSimMap Map, Vector3 Ground, float Tile)
        {
            if (Map == null)
            {
                return;
            }

            Vector3 Origin = new Vector3(Ground.X - Tile * 0.5f, Ground.Y - Tile * 0.5f, 0f);
            Map.CursorPosition = Origin;
            Map.CursorPositionVisible = Origin;
        }
    }
}
