using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class LifePlan
    {
        public static List<AutomatedAction> EatJerky(PlayerCharacter Owner)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Owner == null)
            {
                return Steps;
            }

            if (Owner.IsCarrying("Jerky"))
            {
                Steps.Add(new EatMealAction(Owner, true));
                return StanceSequence.Splice(Owner.CurrentPose, Steps);
            }

            BelievedObject Drawer = Owner.Belief.FindTagged("Jerky");
            if (Drawer == null)
            {
                return Steps;
            }

            bool Elsewhere = Drawer.Chunk != null && Drawer.Chunk != Owner.SharedMapContex.CurrentMapInfo;
            List<AutomatedAction> Travel = GoTo(Owner, Drawer.Chunk, Drawer.LocalPosition);
            if (Elsewhere && Travel.Count == 0)
            {
                return Steps;
            }

            Steps.AddRange(Travel);
            Steps.Add(new OpenContainerAction(Owner));
            Steps.Add(new TakeItemAction(Owner, Drawer, "Jerky"));
            Steps.Add(new EatMealAction(Owner, true));
            return StanceSequence.Splice(Owner.CurrentPose, Steps);
        }

        public static List<AutomatedAction> Restaurant(PlayerCharacter Owner)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Owner == null || !KnowsRestaurant(Owner, out MapInfo Chunk, out Vector3 Place))
            {
                return Steps;
            }

            bool Elsewhere = Chunk != null && Chunk != Owner.SharedMapContex.CurrentMapInfo;
            List<AutomatedAction> Travel = GoTo(Owner, Chunk, Place);
            if (Elsewhere && Travel.Count == 0)
            {
                return Steps;
            }

            Steps.AddRange(Travel);
            Steps.Add(new EatMealAction(Owner, false));
            return StanceSequence.Splice(Owner.CurrentPose, Steps);
        }

        public static bool KnowsDoor(PlayerCharacter Owner, string DoorName)
        {
            return FindDoor(Owner, DoorName) != null;
        }

        public static bool CanOpen(PlayerCharacter Owner, string DoorName)
        {
            BelievedObject Door = FindDoor(Owner, DoorName);
            return Door != null && Door.State == PassageState.Closed;
        }

        public static bool CanLock(PlayerCharacter Owner, string DoorName)
        {
            BelievedObject Door = FindDoor(Owner, DoorName);
            return Door != null && Door.State != PassageState.Broken && Door.State != PassageState.Locked;
        }

        public static bool CanUnlock(PlayerCharacter Owner, string DoorName)
        {
            BelievedObject Door = FindDoor(Owner, DoorName);
            return Door != null && Door.State == PassageState.Locked && DoorOpening.CanUnlock(Owner, Door);
        }

        public static bool CanBreak(PlayerCharacter Owner, string DoorName)
        {
            BelievedObject Door = FindDoor(Owner, DoorName);
            return Door != null && Door.State != PassageState.Broken;
        }

        public static List<AutomatedAction> UseDoor(PlayerCharacter Owner, string DoorName, string Interaction)
        {
            return UseDoor(Owner, DoorName, Interaction, DoorOpenManner.Normally);
        }

        public static List<AutomatedAction> UseDoor(PlayerCharacter Owner, string DoorName, string Interaction, DoorOpenManner Manner)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            BelievedObject Door = FindDoor(Owner, DoorName);
            if (Door == null || !Allowed(Owner, Door, Interaction))
            {
                return Steps;
            }

            bool Elsewhere = Door.Chunk != null && Owner.SharedMapContex != null && Door.Chunk != Owner.SharedMapContex.CurrentMapInfo;
            List<AutomatedAction> Travel = GoTo(Owner, Door.Chunk, Door.LocalPosition);
            if (Elsewhere && Travel.Count == 0)
            {
                return Steps;
            }

            Steps.AddRange(Travel);
            bool Running = DoorOpening.ArrivingAtARun(Travel);
            string StepName = Interaction;
            PassageState Result = PassageState.Open;
            double Seconds = 0.3;
            if (Interaction == "Lock Door")
            {
                Result = PassageState.Locked;
            }
            else if (Interaction == "Unlock Door")
            {
                Result = PassageState.Closed;
            }
            else if (Interaction == "Break Door")
            {
                Result = PassageState.Broken;
                Seconds = DoorOpening.Seconds(DoorOpenManner.Bash, Running);
            }
            else
            {
                StepName = DoorOpening.StepName(Manner);
                Result = PassageState.Open;
                Seconds = DoorOpening.Seconds(Manner, Running);
            }

            Steps.Add(new DoorInteractionAction(Owner, Door, StepName, Result, Seconds));
            return StanceSequence.Splice(Owner.CurrentPose, Steps);
        }

        static bool Allowed(PlayerCharacter Owner, BelievedObject Door, string Interaction)
        {
            if (Interaction == "Lock Door")
            {
                return Door.State != PassageState.Broken && Door.State != PassageState.Locked;
            }

            if (Interaction == "Unlock Door")
            {
                return Door.State == PassageState.Locked && DoorOpening.CanUnlock(Owner, Door);
            }

            if (Interaction == "Break Door")
            {
                return Door.State != PassageState.Broken;
            }

            return Door.State == PassageState.Closed;
        }

        public static List<AutomatedAction> GoTo(PlayerCharacter Owner, MapInfo TargetChunk, Vector3 TargetPosition)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            MapInfo Here = Owner.SharedMapContex.CurrentMapInfo;
            List<ChunkHop> Hops = new List<ChunkHop>();
            if (TargetChunk != null && Here != null && TargetChunk != Here && TargetChunk.MapMapContainer != null)
            {
                BuildingZone Stay = BuildingZones.Shared(Here, Owner.Position.X, Owner.Position.Y, TargetChunk, TargetPosition.X, TargetPosition.Y);
                if (Stay != null)
                {
                    Hops = BuildingZones.Walk(Stay, Here, TargetChunk);
                }
                else
                {
                    Hops = new InterMapMovementAlgorithm().FindHops(Here, TargetChunk.MapMapContainer.AreaName, Owner.Belief);
                }
            }

            Owner.ListPlannedHop = Hops;
            Owner.PlannedHopIndex = 0;

            if (Hops.Count > 0)
            {
                if (LifeOrder.AppendJourney(Owner, Hops, TargetPosition, LoadedMap(TargetChunk), Steps))
                {
                    return Steps;
                }

                return new List<AutomatedAction>();
            }

            MapInfo Local = TargetChunk ?? Here;
            if (Local == null || (TargetChunk != null && Here != null && TargetChunk != Here))
            {
                return Steps;
            }

            FollowPath Path = Owner.PlanPath(Local, TargetPosition, false);
            if (Path.Segments.Count > 0)
            {
                Steps.Add(Path);
            }

            return Steps;
        }

        public static List<AutomatedAction> Drive(PlayerCharacter Owner, Vector3 Goal)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Owner == null || Owner.Belief == null)
            {
                return Steps;
            }

            BelievedObject Car = Owner.Belief.FindTagged("Car");
            if (Car == null)
            {
                return Steps;
            }

            Steps.AddRange(GoTo(Owner, Car.Chunk, Car.LocalPosition));
            Steps.Add(new DriveLegAction(Owner, Goal));
            return StanceSequence.Splice(Owner.CurrentPose, Steps);
        }

        static BelievedObject FindDoor(PlayerCharacter Owner, string DoorName)
        {
            if (Owner == null || Owner.Belief == null || string.IsNullOrEmpty(DoorName))
            {
                return null;
            }

            BelievedObject Door = Owner.Belief.FindObject(DoorName);
            if (Door == null)
            {
                Door = Owner.Belief.FindTagged(DoorName);
            }

            return Door;
        }

        public static bool KnowsRestaurant(PlayerCharacter Owner, out MapInfo Chunk, out Vector3 Place)
        {
            Chunk = null;
            Place = Vector3.Zero;
            if (Owner == null || Owner.Belief == null)
            {
                return false;
            }

            BuildingZone Zone = BuildingZones.Named(Owner, "Restaurant");
            if (Zone != null && Zone.EntryChunk != null)
            {
                Chunk = Zone.EntryChunk;
                Place = Zone.Entry;
                return true;
            }

            BelievedObject Spot = Owner.Belief.FindTagged("Restaurant");
            if (Spot != null)
            {
                Chunk = Spot.Chunk;
                Place = Spot.LocalPosition;
                return true;
            }

            Chunk = Owner.Belief.FindAreaNamed("Restaurant");
            return Chunk != null;
        }

        static LifeSimMap LoadedMap(MapInfo Chunk)
        {
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return null;
            }

            return Chunk.MapMapContainer.ActiveMap;
        }
    }
}
