using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class InterMapMovementAlgorithm
    {
        public const float PortalCost = 16f;

        public List<ChunkHop> FindHops(MapInfo Start, string DestinationName, CharacterBelief Belief)
        {
            List<ChunkHop> Empty = new List<ChunkHop>();
            if (Start == null || Belief == null || string.IsNullOrEmpty(DestinationName))
            {
                return Empty;
            }

            if (AreaName(Start) == DestinationName)
            {
                return Empty;
            }

            Dictionary<MapInfo, float> Cost = new Dictionary<MapInfo, float>();
            Dictionary<MapInfo, MapInfo> Parent = new Dictionary<MapInfo, MapInfo>();
            Dictionary<MapInfo, ChunkHop> EnteredBy = new Dictionary<MapInfo, ChunkHop>();
            List<MapInfo> Open = new List<MapInfo>();
            List<MapInfo> Closed = new List<MapInfo>();

            Cost.Add(Start, 0);
            Open.Add(Start);

            MapInfo Found = null;
            while (Open.Count > 0)
            {
                MapInfo Current = TakeLowest(Open, Cost);
                if (Closed.Contains(Current))
                {
                    continue;
                }

                Closed.Add(Current);
                if (AreaName(Current) == DestinationName)
                {
                    Found = Current;
                    break;
                }

                foreach (MapInfo Next in Successors(Current))
                {
                    ChunkHop Door = Belief.RouteLink(Current, Next);
                    if (Door == null || Closed.Contains(Next))
                    {
                        continue;
                    }

                    float NextCost = Cost[Current] + (Door.IsPortal ? PortalCost : 1f);
                    float KnownCost;
                    if (Cost.TryGetValue(Next, out KnownCost) && NextCost >= KnownCost)
                    {
                        continue;
                    }

                    Cost[Next] = NextCost;
                    Parent[Next] = Current;
                    EnteredBy[Next] = Door;
                    if (!Open.Contains(Next))
                    {
                        Open.Add(Next);
                    }
                }
            }

            if (Found == null)
            {
                return Empty;
            }

            List<ChunkHop> Path = new List<ChunkHop>();
            MapInfo Step = Found;
            while (Parent.ContainsKey(Step))
            {
                Path.Add(EnteredBy[Step]);
                Step = Parent[Step];
            }

            Path.Reverse();
            return Path;
        }

        public List<ChunkHop> FindOpenBorders(MapInfo Start, MapInfo Goal)
        {
            List<ChunkHop> Empty = new List<ChunkHop>();
            if (Start == null || Goal == null || Start == Goal)
            {
                return Empty;
            }

            Dictionary<MapInfo, MapInfo> Parent = new Dictionary<MapInfo, MapInfo>();
            Dictionary<MapInfo, ChunkHop> EnteredBy = new Dictionary<MapInfo, ChunkHop>();
            List<MapInfo> Open = new List<MapInfo>();
            List<MapInfo> Closed = new List<MapInfo>();
            Open.Add(Start);
            MapInfo Found = null;
            while (Open.Count > 0)
            {
                MapInfo Current = Open[0];
                Open.RemoveAt(0);
                if (Closed.Contains(Current))
                {
                    continue;
                }

                Closed.Add(Current);
                if (Current == Goal)
                {
                    Found = Current;
                    break;
                }

                if (Current.ListLink == null)
                {
                    continue;
                }

                for (int i = 0; i < Current.ListLink.Count; i++)
                {
                    ChunkHop Link = Current.ListLink[i];
                    if (Link == null || !Link.IsOpen)
                    {
                        continue;
                    }

                    ChunkHop Forward = null;
                    if (Link.FromChunk == Current)
                    {
                        Forward = Link;
                    }
                    else if (Link.ToChunk == Current)
                    {
                        Forward = Link.Reversed();
                    }

                    if (Forward == null || Forward.ToChunk == null || Closed.Contains(Forward.ToChunk) || EnteredBy.ContainsKey(Forward.ToChunk))
                    {
                        continue;
                    }

                    EnteredBy[Forward.ToChunk] = Forward;
                    Parent[Forward.ToChunk] = Current;
                    Open.Add(Forward.ToChunk);
                }
            }

            if (Found == null)
            {
                return Empty;
            }

            List<ChunkHop> Path = new List<ChunkHop>();
            MapInfo Step = Found;
            while (Parent.ContainsKey(Step))
            {
                Path.Add(EnteredBy[Step]);
                Step = Parent[Step];
            }

            Path.Reverse();
            return Path;
        }

        static string AreaName(MapInfo Chunk)
        {
            if (Chunk.MapMapContainer == null)
            {
                return string.Empty;
            }

            return Chunk.MapMapContainer.AreaName;
        }

        static MapInfo TakeLowest(List<MapInfo> Open, Dictionary<MapInfo, float> Cost)
        {
            int LowestIndex = 0;
            float LowestCost = Cost[Open[0]];
            for (int i = 1; i < Open.Count; i++)
            {
                float CandidateCost = Cost[Open[i]];
                if (CandidateCost < LowestCost)
                {
                    LowestCost = CandidateCost;
                    LowestIndex = i;
                }
            }

            MapInfo Chosen = Open[LowestIndex];
            Open.RemoveAt(LowestIndex);
            return Chosen;
        }

        static List<MapInfo> Successors(MapInfo Node)
        {
            List<MapInfo> Next = new List<MapInfo>();
            foreach (MapInfo Child in Node.DicNestedMapByName.Values)
            {
                Next.Add(Child);
            }

            foreach (MapInfo Neighbour in Node.DicNeighbourByName.Values)
            {
                Next.Add(Neighbour);
            }

            if (Node.HierarchicParent != null)
            {
                Next.Add(Node.HierarchicParent);
            }

            return Next;
        }
    }
}
