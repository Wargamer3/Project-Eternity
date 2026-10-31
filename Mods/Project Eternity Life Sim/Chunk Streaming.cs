using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class ChunkStreaming
    {
        public static void KeepEdge(HashSet<MapInfo> Keep, MapInfo Here, float LocalX, float LocalY, float LoadDistance)
        {
            if (Keep == null || Here == null || Here.MapMapContainer == null || Here.ListLink == null)
            {
                return;
            }

            Keep.Add(Here);
            float SpanX = Here.MapMapContainer.SpanX;
            float SpanY = Here.MapMapContainer.SpanY;
            if (SpanX <= 0f || SpanY <= 0f)
            {
                return;
            }

            for (int i = 0; i < Here.ListLink.Count; i++)
            {
                ChunkHop Link = Here.ListLink[i];
                if (Link == null)
                {
                    continue;
                }

                MapInfo Other = Link.FromChunk == Here ? Link.ToChunk : Link.ToChunk == Here ? Link.FromChunk : null;
                if (Other == null || Other.MapMapContainer == null)
                {
                    continue;
                }

                float DX = Other.MapMapContainer.OffsetX - Here.MapMapContainer.OffsetX;
                float DY = Other.MapMapContainer.OffsetY - Here.MapMapContainer.OffsetY;
                bool Close = false;
                if (DX > 1f)
                {
                    Close = LocalX >= SpanX - LoadDistance;
                }
                else if (DX < -1f)
                {
                    Close = LocalX <= LoadDistance;
                }
                else if (DY > 1f)
                {
                    Close = LocalY >= SpanY - LoadDistance;
                }
                else if (DY < -1f)
                {
                    Close = LocalY <= LoadDistance;
                }

                if (Close)
                {
                    Keep.Add(Other);
                }
            }
        }

        public static void KeepAround(HashSet<MapInfo> Keep, IList<MapInfo> Maps, float WorldX, float WorldY, float Radius, MapInfo Skip)
        {
            if (Keep == null || Maps == null || Radius < 0f)
            {
                return;
            }

            float RadiusSq = Radius * Radius;
            for (int i = 0; i < Maps.Count; i++)
            {
                MapInfo Map = Maps[i];
                if (Map == null || Map == Skip || Map.MapMapContainer == null)
                {
                    continue;
                }

                float SpanX = Map.MapMapContainer.SpanX;
                float SpanY = Map.MapMapContainer.SpanY;
                if (SpanX <= 0f || SpanY <= 0f)
                {
                    continue;
                }

                float MinX = Map.MapMapContainer.OffsetX;
                float MinY = Map.MapMapContainer.OffsetY;
                float NearX = WorldX < MinX ? MinX : WorldX > MinX + SpanX ? MinX + SpanX : WorldX;
                float NearY = WorldY < MinY ? MinY : WorldY > MinY + SpanY ? MinY + SpanY : WorldY;
                float DX = WorldX - NearX;
                float DY = WorldY - NearY;
                if (DX * DX + DY * DY <= RadiusSq)
                {
                    Keep.Add(Map);
                }
            }
        }

        public static HashSet<MapInfo> Needed(IList<MapInfo> Cameras, IList<PlayerCharacter> Characters, float LoadDistance)
        {
            HashSet<MapInfo> Keep = new HashSet<MapInfo>();
            if (Cameras != null)
            {
                for (int i = 0; i < Cameras.Count; i++)
                {
                    if (Cameras[i] != null)
                    {
                        Keep.Add(Cameras[i]);
                    }
                }
            }

            if (Characters == null)
            {
                return Keep;
            }

            for (int i = 0; i < Characters.Count; i++)
            {
                PlayerCharacter Character = Characters[i];
                if (Character == null || Character.SharedMapContex == null)
                {
                    continue;
                }

                MapInfo Here = Character.SharedMapContex.CurrentMapInfo;
                if (Here != null)
                {
                    Keep.Add(Here);
                    KeepEdge(Keep, Here, Character.Position.X, Character.Position.Y, LoadDistance);
                }

                ChunkHop Next = Character.NextHop();
                if (Next == null)
                {
                    continue;
                }

                MapInfo Destination = Next.ToChunk;
                Vector3 Portal = Next.FromPosition;
                if (Here != null && Next.FromChunk != Here)
                {
                    Destination = Next.FromChunk;
                    Portal = Next.ToPosition;
                }

                if (Destination != null && Vector3.Distance(Character.Position, Portal) <= LoadDistance)
                {
                    Keep.Add(Destination);
                }
            }

            return Keep;
        }
    }
}
