using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class ChunkHop
    {
        public string Name;
        public MapInfo FromChunk;
        public Vector3 FromPosition;
        public MapInfo ToChunk;
        public Vector3 ToPosition;
        public bool IsOpen;

        public bool IsPortal
        {
            get { return Name != "Border"; }
        }

        public ChunkHop Copy()
        {
            ChunkHop Clone = new ChunkHop();
            Clone.Name = Name;
            Clone.FromChunk = FromChunk;
            Clone.FromPosition = FromPosition;
            Clone.ToChunk = ToChunk;
            Clone.ToPosition = ToPosition;
            Clone.IsOpen = IsOpen;
            return Clone;
        }

        public bool Joins(MapInfo A, MapInfo B)
        {
            return (FromChunk == A && ToChunk == B) || (FromChunk == B && ToChunk == A);
        }

        public ChunkHop Reversed()
        {
            ChunkHop Hop = Copy();
            Hop.FromChunk = ToChunk;
            Hop.ToChunk = FromChunk;
            Hop.FromPosition = ToPosition;
            Hop.ToPosition = FromPosition;
            return Hop;
        }
    }

    public enum WorldMarkerKind
    {
        Object,
        Door,
        Window,
        Bridge,
        Blocker,
        Obstacle,
    }

    public class WorldMarker
    {
        public string Name;
        public string Tag;
        public Vector3 LocalPosition;
        public WorldMarkerKind Kind;
        public PassageState State;
        public string KeyName;
        public ChunkHop Link;
        public float CutRadius;
        public bool HasTraversal;
        public Traversal TraversalKind;
        public Vector3 TraversalEnd;

        public bool IsOpen
        {
            get { return Passage.LetsPeopleThrough(State); }
            set
            {
                if (value)
                {
                    State = PassageState.Open;
                }
                else if (State == PassageState.Open)
                {
                    State = PassageState.Closed;
                }
            }
        }
    }

    public class BelievedObject
    {
        public string Name;
        public string Tag;
        public MapInfo Chunk;
        public Vector3 LocalPosition;
        public WorldMarkerKind Kind;
        public PassageState State;
        public string KeyName;
        public ChunkHop Hop;
        public float CutRadius;
        public bool HasTraversal;
        public Traversal TraversalKind;
        public Vector3 TraversalEnd;

        public bool IsOpen
        {
            get { return Passage.LetsPeopleThrough(State); }
            set
            {
                if (value)
                {
                    State = PassageState.Open;
                }
                else if (State == PassageState.Open)
                {
                    State = PassageState.Closed;
                }
            }
        }
    }

    public class CharacterBelief
    {
        public List<MapInfo> ListKnownArea;
        public List<ChunkHop> ListLink;
        public List<BelievedObject> ListObject;

        public CharacterBelief()
        {
            ListKnownArea = new List<MapInfo>();
            ListLink = new List<ChunkHop>();
            ListObject = new List<BelievedObject>();
        }

        public void LearnArea(MapInfo Area)
        {
            if (Area == null || ListKnownArea.Contains(Area))
            {
                return;
            }

            ListKnownArea.Add(Area);
        }

        public bool KnowsArea(MapInfo Area)
        {
            return ListKnownArea.Contains(Area);
        }

        public void LearnLink(ChunkHop Source)
        {
            if (Source == null)
            {
                return;
            }

            ChunkHop Known = FindKind(Source.FromChunk, Source.ToChunk, Source.IsPortal);
            if (Known == null)
            {
                ListLink.Add(Source.Copy());
                return;
            }

            Known.Name = Source.Name;
            Known.FromPosition = Source.FromChunk == Known.FromChunk ? Source.FromPosition : Source.ToPosition;
            Known.ToPosition = Source.FromChunk == Known.FromChunk ? Source.ToPosition : Source.FromPosition;
            if (Source.IsOpen)
            {
                Known.IsOpen = true;
            }
        }

        public void SetLinkOpen(MapInfo From, MapInfo To, bool IsOpen)
        {
            ChunkHop Known = FindStoredLink(From, To);
            if (Known != null)
            {
                Known.IsOpen = IsOpen;
            }
        }

        public ChunkHop OpenLink(MapInfo From, MapInfo To)
        {
            foreach (ChunkHop Link in ListLink)
            {
                if (!Link.IsOpen)
                {
                    continue;
                }

                if (Link.FromChunk == From && Link.ToChunk == To)
                {
                    return Link;
                }

                if (Link.FromChunk == To && Link.ToChunk == From)
                {
                    return Link.Reversed();
                }
            }

            return null;
        }

        public ChunkHop RouteLink(MapInfo From, MapInfo To)
        {
            ChunkHop Best = null;
            float BestCost = float.MaxValue;
            foreach (ChunkHop Stored in ListLink)
            {
                bool Forward = Stored.FromChunk == From && Stored.ToChunk == To;
                bool Backward = Stored.FromChunk == To && Stored.ToChunk == From;
                if (!Forward && !Backward)
                {
                    continue;
                }

                ChunkHop Link = Forward ? Stored : Stored.Reversed();
                if (Rejected(Link))
                {
                    continue;
                }

                float Cost = Link.IsPortal ? InterMapMovementAlgorithm.PortalCost : 1f;
                if (Cost < BestCost)
                {
                    Best = Link;
                    BestCost = Cost;
                }
            }

            return Best;
        }

        bool Rejected(ChunkHop Link)
        {
            foreach (BelievedObject Object in ListObject)
            {
                if (Object.Hop == null || !Object.Hop.Joins(Link.FromChunk, Link.ToChunk))
                {
                    continue;
                }

                if (Object.Kind != WorldMarkerKind.Door && Object.Kind != WorldMarkerKind.Window)
                {
                    continue;
                }

                if (Object.Hop.IsPortal != Link.IsPortal)
                {
                    continue;
                }

                return Object.State == PassageState.Locked || !Link.IsOpen;
            }

            return !Link.IsOpen;
        }

        public void LearnThatItIsLocked(BelievedObject Door, ChunkHop Hop)
        {
            if (Door != null)
            {
                Door.State = PassageState.Locked;
            }

            ChunkHop Known = Hop;
            if (Door != null && Door.Hop != null)
            {
                Known = Door.Hop;
            }

            if (Known == null)
            {
                return;
            }

            SetLinkOpen(Known.FromChunk, Known.ToChunk, false);
            if (Door != null)
            {
                Door.Hop = FindStoredLink(Known.FromChunk, Known.ToChunk);
            }
        }

        public BelievedObject FindObject(string Name)
        {
            foreach (BelievedObject Object in ListObject)
            {
                if (Object.Name == Name)
                {
                    return Object;
                }
            }

            return null;
        }

        public BelievedObject FindTagged(string Name)
        {
            BelievedObject ByName = FindObject(Name);
            if (ByName != null)
            {
                return ByName;
            }

            foreach (BelievedObject Object in ListObject)
            {
                if (Object.Tag == Name)
                {
                    return Object;
                }
            }

            return null;
        }

        public MapInfo FindAreaNamed(string Name)
        {
            foreach (MapInfo Area in ListKnownArea)
            {
                if (Area.MapMapContainer != null && Area.MapMapContainer.AreaName == Name)
                {
                    return Area;
                }
            }

            return null;
        }

        public bool HasCategory(string Category)
        {
            foreach (MapInfo Area in ListKnownArea)
            {
                if (Area.MapMapContainer != null && Area.MapMapContainer.AreaName == Category)
                {
                    return true;
                }
            }

            foreach (BelievedObject Object in ListObject)
            {
                if (Object.Name == Category || Object.Tag == Category)
                {
                    return true;
                }
            }

            return false;
        }

        public void Perceive(MapInfo Chunk, Vector3 ViewerPosition, float Radius)
        {
            if (Chunk == null)
            {
                return;
            }

            LearnArea(Chunk);

            foreach (ChunkHop Link in Chunk.ListLink)
            {
                bool StartsHere = Link.FromChunk == Chunk && Vector3.Distance(ViewerPosition, Link.FromPosition) <= Radius;
                bool EndsHere = Link.ToChunk == Chunk && Vector3.Distance(ViewerPosition, Link.ToPosition) <= Radius;
                if (StartsHere || EndsHere)
                {
                    LearnLink(Link);
                }
            }

            foreach (WorldMarker Marker in Chunk.ListMarker)
            {
                if (Vector3.Distance(ViewerPosition, Marker.LocalPosition) > Radius)
                {
                    continue;
                }

                LearnObject(Chunk, Marker);
                if (Marker.Link != null)
                {
                    LearnLink(Marker.Link);
                    BelievedObject Known = FindObject(Marker.Name);
                    if (Known != null)
                    {
                        Known.Hop = FindStoredLink(Marker.Link.FromChunk, Marker.Link.ToChunk);
                    }

                    if (Marker.Kind == WorldMarkerKind.Door || Marker.Kind == WorldMarkerKind.Window)
                    {
                        RememberVisiblePassage(Known);
                    }
                    else
                    {
                        SetLinkOpen(Marker.Link.FromChunk, Marker.Link.ToChunk, Passage.LetsPeopleThrough(Marker.State));
                    }
                }
            }
        }

        void RememberVisiblePassage(BelievedObject Known)
        {
            if (Known == null || Known.Hop == null)
            {
                return;
            }

            if (Known.State == PassageState.Open || Known.State == PassageState.Broken)
            {
                SetLinkOpen(Known.Hop.FromChunk, Known.Hop.ToChunk, true);
            }
            else if (Known.State == PassageState.Locked)
            {
                SetLinkOpen(Known.Hop.FromChunk, Known.Hop.ToChunk, false);
            }
        }

        void LearnObject(MapInfo Chunk, WorldMarker Marker)
        {
            BelievedObject Known = FindObject(Marker.Name);
            bool AlreadyKnown = Known != null;
            PassageState Believed = AlreadyKnown ? Known.State : PassageState.Closed;
            if (Known == null)
            {
                Known = new BelievedObject();
                ListObject.Add(Known);
            }

            Known.Name = Marker.Name;
            Known.Tag = Marker.Tag;
            Known.Chunk = Chunk;
            Known.LocalPosition = Marker.LocalPosition;
            Known.Kind = Marker.Kind;
            Known.KeyName = Marker.KeyName;
            Known.CutRadius = Marker.CutRadius;
            Known.HasTraversal = Marker.HasTraversal;
            Known.TraversalKind = Marker.TraversalKind;
            Known.TraversalEnd = Marker.TraversalEnd;
            if (Marker.Kind == WorldMarkerKind.Door || Marker.Kind == WorldMarkerKind.Window)
            {
                Known.State = Passage.AsSeen(Believed, Marker.State, AlreadyKnown);
            }
            else
            {
                Known.State = Marker.State;
            }
        }

        ChunkHop FindKind(MapInfo From, MapInfo To, bool Portal)
        {
            foreach (ChunkHop Link in ListLink)
            {
                bool Forward = Link.FromChunk == From && Link.ToChunk == To;
                bool Backward = Link.FromChunk == To && Link.ToChunk == From;
                if ((Forward || Backward) && Link.IsPortal == Portal)
                {
                    return Link;
                }
            }

            return null;
        }

        ChunkHop FindStoredLink(MapInfo From, MapInfo To)
        {
            foreach (ChunkHop Link in ListLink)
            {
                bool Forward = Link.FromChunk == From && Link.ToChunk == To;
                bool Backward = Link.FromChunk == To && Link.ToChunk == From;
                if (Forward || Backward)
                {
                    return Link;
                }
            }

            return null;
        }
    }
}
