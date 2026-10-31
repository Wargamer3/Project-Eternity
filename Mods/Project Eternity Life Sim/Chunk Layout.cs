using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class ChunkOrigin
    {
        public static Vector3 Shift(MapInfo Map)
        {
            if (Map == null || Map.MapMapContainer == null)
            {
                return Vector3.Zero;
            }

            return new Vector3(Map.MapMapContainer.OffsetX, 0f, Map.MapMapContainer.OffsetY);
        }

        public static Vector3 Camera(MapInfo Map, Vector3 RenderPosition)
        {
            return RenderPosition + Shift(Map);
        }

        public static Vector3 ModelWorld(Vector3 Position, Vector3 Shift)
        {
            return new Vector3(Position.X + Shift.X, Position.Z + Shift.Y, Position.Y + Shift.Z);
        }
    }

    public static class ChunkCross
    {
        public static List<MapInfo> Maps;

        public static MapInfo At(float WorldX, float WorldY)
        {
            if (Maps == null)
            {
                return null;
            }

            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Map = Maps[i];
                if (Map == null || Map.MapMapContainer == null || Map.MapMapContainer.SpanX <= 0f || Map.MapMapContainer.SpanY <= 0f)
                {
                    continue;
                }

                float X = Map.MapMapContainer.OffsetX;
                float Y = Map.MapMapContainer.OffsetY;
                if (WorldX >= X && WorldY >= Y && WorldX < X + Map.MapMapContainer.SpanX && WorldY < Y + Map.MapMapContainer.SpanY)
                {
                    return Map;
                }
            }

            return null;
        }

        public static bool Outside(MapInfo Here, float LocalX, float LocalY)
        {
            if (Here == null || Here.MapMapContainer == null || Here.MapMapContainer.SpanX <= 0f)
            {
                return false;
            }

            return LocalX < 0f || LocalY < 0f || LocalX >= Here.MapMapContainer.SpanX || LocalY >= Here.MapMapContainer.SpanY;
        }

        public static bool Settle(PlayerCharacter Who, float WorldX, float WorldY, float Height)
        {
            if (Who == null || Who.SharedMapContex == null)
            {
                return false;
            }

            MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
            MapInfo Next = At(WorldX, WorldY);
            if (Next == null)
            {
                Next = Here;
            }

            if (Next != null && Next != Here && Next.Kind == GroundKind.Grid && (Next.MapMapContainer == null || Next.MapMapContainer.ActiveMap == null))
            {
                Next = Here;
            }

            if (Next != Here && Next != null)
            {
                if (Here != null)
                {
                    Here.Drop(Who);
                }

                Next.Adopt(Who);
                Who.SharedMapContex.CurrentMapInfo = Next;
            }

            MapInfo Landed = Who.SharedMapContex.CurrentMapInfo;
            float OriginX = Landed == null || Landed.MapMapContainer == null ? 0f : Landed.MapMapContainer.OffsetX;
            float OriginY = Landed == null || Landed.MapMapContainer == null ? 0f : Landed.MapMapContainer.OffsetY;
            Who.Position = new Vector3(WorldX - OriginX, WorldY - OriginY, Height);
            return Next != Here && Who.SharedMapContex.CurrentMapInfo == Next;
        }

        public static bool TryMoveCursor(PlayerCharacter Cursor, float LocalX, float LocalY, float Height)
        {
            if (Cursor == null || Cursor.SharedMapContex == null)
            {
                return false;
            }

            MapInfo Here = Cursor.SharedMapContex.CurrentMapInfo;
            if (!Outside(Here, LocalX, LocalY))
            {
                return false;
            }

            float OriginX = Here == null || Here.MapMapContainer == null ? 0f : Here.MapMapContainer.OffsetX;
            float OriginY = Here == null || Here.MapMapContainer == null ? 0f : Here.MapMapContainer.OffsetY;
            Vector3 Before = Cursor.Position;
            bool Moved = Settle(Cursor, LocalX + OriginX, LocalY + OriginY, Height);
            if (!Moved)
            {
                Cursor.SetPosition(Before);
                return false;
            }

            Vector3 Local = Cursor.Position;
            Cursor.SetPosition(new Vector3(Local.X, Height, Local.Y));
            return true;
        }

        public static bool MoveCursor(PlayerCharacter Cursor, float WorldX, float WorldY, float Height)
        {
            if (Cursor == null || Cursor.SharedMapContex == null)
            {
                return false;
            }

            MapInfo Next = At(WorldX, WorldY);
            if (Next == null || Next.MapMapContainer == null)
            {
                return false;
            }

            MapInfo Here = Cursor.SharedMapContex.CurrentMapInfo;
            if (Here != Next)
            {
                if (Next.Kind == GroundKind.Grid && Next.MapMapContainer.ActiveMap == null)
                {
                    return false;
                }

                if (Here != null)
                {
                    Here.Drop(Cursor);
                }

                Next.Adopt(Cursor);
                Cursor.SharedMapContex.CurrentMapInfo = Next;
            }

            Cursor.SetPosition(new Vector3(WorldX - Next.MapMapContainer.OffsetX, Height, WorldY - Next.MapMapContainer.OffsetY));
            return true;
        }
    }

    public static class ChunkRing
    {
        public static void Around(MapInfo Center, float SpanX, float SpanY, out List<MapInfo> All, out MapInfo Far)
        {
            All = new List<MapInfo>();
            if (Center == null || Center.MapMapContainer == null)
            {
                Far = null;
                return;
            }

            Center.MapMapContainer.OffsetX = 0;
            Center.MapMapContainer.OffsetY = 0;
            Center.MapMapContainer.SpanX = SpanX;
            Center.MapMapContainer.SpanY = SpanY;
            MapInfo[,] Grid = new MapInfo[3, 3];
            Grid[1, 1] = Center;
            All.Add(Center);
            for (int Y = -1; Y <= 1; Y++)
            {
                for (int X = -1; X <= 1; X++)
                {
                    if (X == 0 && Y == 0)
                    {
                        continue;
                    }

                    MapInfo Shell = ShellAt("Grass " + (X + 1) + " " + (Y + 1), X * SpanX, Y * SpanY, SpanX, SpanY);
                    Grid[X + 1, Y + 1] = Shell;
                    All.Add(Shell);
                }
            }

            for (int Y = 0; Y < 3; Y++)
            {
                for (int X = 0; X < 3; X++)
                {
                    if (X + 1 < 3)
                    {
                        Join(Grid[X, Y], Grid[X + 1, Y], new Vector3(SpanX, SpanY * 0.5f, 0f), new Vector3(0f, SpanY * 0.5f, 0f));
                    }

                    if (Y + 1 < 3)
                    {
                        Join(Grid[X, Y], Grid[X, Y + 1], new Vector3(SpanX * 0.5f, SpanY, 0f), new Vector3(SpanX * 0.5f, 0f, 0f));
                    }
                }
            }

            Far = ShellAt("Grass Far", 10000f, 0f, SpanX, SpanY);
            All.Add(Far);
        }

        static MapInfo ShellAt(string Name, float X, float Y, float SpanX, float SpanY)
        {
            MapInfo Shell = new MapInfo(new MapContainer(Name, null), null, null);
            Shell.MapMapContainer.OffsetX = (long)Math.Round(X);
            Shell.MapMapContainer.OffsetY = (long)Math.Round(Y);
            Shell.MapMapContainer.SpanX = SpanX;
            Shell.MapMapContainer.SpanY = SpanY;
            return Shell;
        }

        public static void Border(MapInfo A, MapInfo B)
        {
            if (A == null || B == null || A == B || A.MapMapContainer == null || B.MapMapContainer == null)
            {
                return;
            }

            int DX = B.ChunkX - A.ChunkX;
            int DZ = B.ChunkZ - A.ChunkZ;
            if (Math.Abs(DX) + Math.Abs(DZ) != 1)
            {
                return;
            }

            if (A.ListLink != null)
            {
                for (int i = 0; i < A.ListLink.Count; i++)
                {
                    ChunkHop Link = A.ListLink[i];
                    if (Link != null && Link.Joins(A, B))
                    {
                        return;
                    }
                }
            }

            float SpanX = A.MapMapContainer.SpanX;
            float SpanY = A.MapMapContainer.SpanY;
            float OtherX = B.MapMapContainer.SpanX;
            float OtherY = B.MapMapContainer.SpanY;
            if (SpanX <= 1f || SpanY <= 1f || OtherX <= 1f || OtherY <= 1f)
            {
                return;
            }

            if (DX == 1)
            {
                Join(A, B, new Vector3(SpanX, SpanY * 0.5f, 0f), new Vector3(0f, OtherY * 0.5f, 0f));
            }
            else if (DX == -1)
            {
                Join(B, A, new Vector3(OtherX, OtherY * 0.5f, 0f), new Vector3(0f, SpanY * 0.5f, 0f));
            }
            else if (DZ == 1)
            {
                Join(A, B, new Vector3(SpanX * 0.5f, SpanY, 0f), new Vector3(OtherX * 0.5f, 0f, 0f));
            }
            else
            {
                Join(B, A, new Vector3(OtherX * 0.5f, OtherY, 0f), new Vector3(SpanX * 0.5f, 0f, 0f));
            }
        }

        static void Join(MapInfo A, MapInfo B, Vector3 From, Vector3 To)
        {
            if (A == null || B == null)
            {
                return;
            }

            ChunkHop Hop = new ChunkHop();
            Hop.Name = "Border";
            Hop.FromChunk = A;
            Hop.ToChunk = B;
            Hop.FromPosition = From;
            Hop.ToPosition = To;
            Hop.IsOpen = true;
            A.ListLink.Add(Hop);
            B.ListLink.Add(Hop.Reversed());
            A.AddNeighbour(B);
            B.AddNeighbour(A);
        }
    }
}
