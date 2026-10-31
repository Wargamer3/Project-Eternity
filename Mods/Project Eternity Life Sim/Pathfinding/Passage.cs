using System.Collections.Generic;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum PassageState
    {
        Closed,
        Open,
        Locked,
        Broken,
    }

    public enum DoorOpenManner
    {
        Slowly,
        Normally,
        Quickly,
        Bash,
    }

    public static class Passage
    {
        public static bool LetsPeopleThrough(PassageState State)
        {
            return State == PassageState.Open || State == PassageState.Broken;
        }

        public static void Apply(PassageState State, ref bool IsClosed, ref bool IsLocked, ref int HP)
        {
            switch (State)
            {
                case PassageState.Open:
                    IsClosed = false;
                    IsLocked = false;
                    if (HP <= 0)
                    {
                        HP = 1;
                    }

                    break;
                case PassageState.Locked:
                    IsClosed = true;
                    IsLocked = true;
                    if (HP <= 0)
                    {
                        HP = 1;
                    }

                    break;
                case PassageState.Broken:
                    IsClosed = false;
                    IsLocked = false;
                    HP = 0;
                    break;
                default:
                    IsClosed = true;
                    IsLocked = false;
                    if (HP <= 0)
                    {
                        HP = 1;
                    }

                    break;
            }
        }

        public static PassageState AsSeen(PassageState Believed, PassageState World, bool AlreadyKnown)
        {
            if (World == PassageState.Open || World == PassageState.Broken)
            {
                return World;
            }

            if (AlreadyKnown && Believed == PassageState.Locked)
            {
                return PassageState.Locked;
            }

            return PassageState.Closed;
        }

        public static WorldMarker Find(ChunkHop Hop)
        {
            WorldMarker OnStart = On(Hop, Hop == null ? null : Hop.FromChunk);
            if (OnStart != null)
            {
                return OnStart;
            }

            return On(Hop, Hop == null ? null : Hop.ToChunk);
        }

        static WorldMarker On(ChunkHop Hop, MapInfo Chunk)
        {
            if (Hop == null || Chunk == null)
            {
                return null;
            }

            foreach (WorldMarker Marker in Chunk.ListMarker)
            {
                if (Marker.Link == null)
                {
                    continue;
                }

                if (Marker.Kind != WorldMarkerKind.Door && Marker.Kind != WorldMarkerKind.Window)
                {
                    continue;
                }

                if (Marker.Link.Joins(Hop.FromChunk, Hop.ToChunk))
                {
                    return Marker;
                }
            }

            return null;
        }

        public static PassageState Read(bool IsClosed, bool IsLocked, int HP)
        {
            if (HP <= 0)
            {
                return PassageState.Broken;
            }

            if (IsLocked)
            {
                return PassageState.Locked;
            }

            if (IsClosed)
            {
                return PassageState.Closed;
            }

            return PassageState.Open;
        }
    }

    public static class DoorOpening
    {
        public static double Seconds(DoorOpenManner Manner, bool Running)
        {
            switch (Manner)
            {
                case DoorOpenManner.Slowly:
                    return 1.2;
                case DoorOpenManner.Quickly:
                    return 0.25;
                case DoorOpenManner.Bash:
                    return Running ? 0.2 : 0.8;
                default:
                    return 0.6;
            }
        }

        public static string StepName(DoorOpenManner Manner)
        {
            switch (Manner)
            {
                case DoorOpenManner.Slowly:
                    return "Open Door Slowly";
                case DoorOpenManner.Quickly:
                    return "Open Door Quickly";
                case DoorOpenManner.Bash:
                    return "Bash Door";
                default:
                    return "Open Door";
            }
        }

        public static bool ArrivingAtARun(IList<AutomatedAction> Steps)
        {
            if (Steps == null)
            {
                return false;
            }

            for (int i = Steps.Count - 1; i >= 0; i--)
            {
                FollowPath Path = Steps[i] as FollowPath;
                if (Path == null)
                {
                    continue;
                }

                foreach (PathSegment Segment in Path.Segments)
                {
                    if (Segment.Kind == Traversal.Run)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        public static bool KnowsKey(PlayerCharacter Owner, BelievedObject Door)
        {
            if (Owner == null || Door == null || string.IsNullOrEmpty(Door.KeyName))
            {
                return false;
            }

            return Owner.HasKnowledge(Door.KeyName) && Owner.IsCarrying(Door.KeyName);
        }

        public static bool KnowsLockpick(PlayerCharacter Owner)
        {
            return Owner != null && Owner.HasKnowledge("Lockpick") && Owner.IsCarrying("Lockpick");
        }

        public static bool CanUnlock(PlayerCharacter Owner, BelievedObject Door)
        {
            return KnowsKey(Owner, Door) || KnowsLockpick(Owner);
        }
    }
}
