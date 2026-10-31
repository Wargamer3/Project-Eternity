using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using ProjectEternity.Core;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public static class LifeOrder
    {
        public const float Reach = 28f;

        static Dictionary<string, LifeInteraction> ByName;
        static readonly HashSet<Assembly> Scanned = new HashSet<Assembly>();

        public static IReadOnlyDictionary<string, LifeInteraction> Interactions
        {
            get
            {
                Ensure();
                return ByName;
            }
        }

        public static void Load(Assembly Source)
        {
            Ensure();
            Take(Source);
        }

        static void Ensure()
        {
            if (ByName != null)
            {
                return;
            }

            ByName = new Dictionary<string, LifeInteraction>();
            Take(typeof(LifeOrder).Assembly);
            Assembly[] Loaded = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < Loaded.Length; i++)
            {
                Take(Loaded[i]);
            }

            string Folder = null;
            try
            {
                Folder = Path.GetDirectoryName(typeof(LifeOrder).Assembly.Location);
            }
            catch (System.Exception)
            {
                Folder = null;
            }

            if (string.IsNullOrEmpty(Folder) || !Directory.Exists(Folder))
            {
                return;
            }

            string[] Files;
            try
            {
                Files = Directory.GetFiles(Folder, "*Interaction*.dll");
            }
            catch (System.Exception)
            {
                return;
            }

            for (int i = 0; i < Files.Length; i++)
            {
                Take(ReflectionHelper.LoadPluginAssembly(Files[i]));
            }
        }

        static void Take(Assembly Source)
        {
            if (Source == null || Scanned.Contains(Source))
            {
                return;
            }

            Scanned.Add(Source);
            Type[] Types;
            try
            {
                Types = Source.GetTypes();
            }
            catch (ReflectionTypeLoadException Broken)
            {
                Types = Broken.Types;
            }
            catch (System.Exception)
            {
                return;
            }

            if (Types == null)
            {
                return;
            }

            List<Type> Concrete = new List<Type>();
            for (int i = 0; i < Types.Length; i++)
            {
                if (Types[i] != null)
                {
                    Concrete.Add(Types[i]);
                }
            }

            List<LifeInteraction> Found = ReflectionHelper.GetObjectsFromBaseTypes<LifeInteraction>(typeof(LifeInteraction), Concrete.ToArray());
            for (int i = 0; i < Found.Count; i++)
            {
                LifeInteraction Item = Found[i];
                if (Item != null && !string.IsNullOrEmpty(Item.Name))
                {
                    ByName[Item.Name] = Item;
                }
            }
        }

        public static List<InteractionChoice> Choices(InteractionContext Context)
        {
            Ensure();
            List<InteractionChoice> Found = new List<InteractionChoice>();
            foreach (LifeInteraction Item in ByName.Values)
            {
                Item.Offer(Context, Found);
            }

            Found.Sort(CompareChoice);
            return Found;
        }

        static int CompareChoice(InteractionChoice A, InteractionChoice B)
        {
            int Order = A.Order.CompareTo(B.Order);
            if (Order != 0)
            {
                return Order;
            }

            return string.Compare(A.Label, B.Label, StringComparison.Ordinal);
        }

        public static List<InteractTarget> Nearby(PlayerCharacter Actor, Vector3 Cursor, int Floor, IList<FurnitureItem> Room, IList<PlayerCharacter> People, InteractionContext Shared)
        {
            List<InteractTarget> Found = new List<InteractTarget>();
            if (Room != null)
            {
                for (int i = 0; i < Room.Count; i++)
                {
                    FurnitureItem Box = Room[i];
                    if (!Near(Box, Cursor, Floor))
                    {
                        continue;
                    }

                    InteractionContext One = Shared == null ? new InteractionContext() : Shared.Copy();
                    One.Actor = Actor;
                    One.Box = Box;
                    One.Other = null;
                    One.Room = Room;
                    One.People = People;
                    List<InteractionChoice> Offered = Choices(One);
                    if (Offered.Count == 0)
                    {
                        continue;
                    }

                    InteractTarget Target = new InteractTarget();
                    Target.Label = string.IsNullOrEmpty(Box.Interaction) ? "Furniture" : Box.Interaction;
                    Target.Box = Box;
                    Target.Choices = Offered;
                    Found.Add(Target);
                }
            }

            if (People != null)
            {
                for (int i = 0; i < People.Count; i++)
                {
                    PlayerCharacter Who = People[i];
                    if (!Near(Who, Cursor, Floor) || Who == Actor)
                    {
                        continue;
                    }

                    InteractionContext One = Shared == null ? new InteractionContext() : Shared.Copy();
                    One.Actor = Actor;
                    One.Other = Who;
                    One.Box = null;
                    One.Room = Room;
                    One.People = People;
                    List<InteractionChoice> Offered = Choices(One);
                    if (Offered.Count == 0)
                    {
                        continue;
                    }

                    InteractTarget Target = new InteractTarget();
                    Target.Label = string.IsNullOrEmpty(Who.Name) ? "Someone" : Who.Name;
                    Target.Person = Who;
                    Target.Choices = Offered;
                    Found.Add(Target);
                }
            }

            return Found;
        }

        public static bool Near(FurnitureItem Box, Vector3 Cursor, int Floor)
        {
            if (Box == null || Box.Floor != Floor)
            {
                return false;
            }

            float DX = System.Math.Abs(Cursor.X - Box.Position.X);
            float DY = System.Math.Abs(Cursor.Y - Box.Position.Y);
            return DX <= Box.Width * 0.5f + Reach && DY <= Box.Length * 0.5f + Reach;
        }

        public static bool Near(PlayerCharacter Who, Vector3 Cursor, int Floor)
        {
            if (Who == null || BuildFloor.Index(Who.Position.Z) != Floor)
            {
                return false;
            }

            float DX = Who.Position.X - Cursor.X;
            float DY = Who.Position.Y - Cursor.Y;
            return DX * DX + DY * DY <= 40f * 40f;
        }

        public static bool IsBed(FurnitureItem Box)
        {
            if (Box == null || string.IsNullOrEmpty(Box.Interaction))
            {
                return false;
            }

            return Box.Interaction == "Tent" || Box.Interaction.IndexOf("Bed", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool IsSeat(FurnitureItem Box)
        {
            if (Box == null)
            {
                return false;
            }

            CatalogItem Card = BuildCatalog.Find(Box.Interaction);
            return Card != null && Card.Seat;
        }

        public static bool Intimate(CharacterRelationship Bond)
        {
            if (Bond == null)
            {
                return false;
            }

            return Bond.Status == RelationshipStatus.Lover || Bond.Status == RelationshipStatus.Married || Bond.Mood == RelationshipMood.LoveyDovey;
        }

        public static bool SharingBed(PlayerCharacter A, PlayerCharacter B, IList<FurnitureItem> Room)
        {
            if (A == null || B == null || Room == null)
            {
                return false;
            }

            for (int i = 0; i < Room.Count; i++)
            {
                FurnitureItem Box = Room[i];
                if (IsBed(Box) && On(A, Box) && On(B, Box))
                {
                    return true;
                }
            }

            return false;
        }

        static bool On(PlayerCharacter Who, FurnitureItem Box)
        {
            if (Who == null || Box == null || BuildFloor.Index(Who.Position.Z) != Box.Floor)
            {
                return false;
            }

            float DX = System.Math.Abs(Who.Position.X - Box.Position.X);
            float DY = System.Math.Abs(Who.Position.Y - Box.Position.Y);
            return DX <= Box.Width * 0.5f + 8f && DY <= Box.Length * 0.5f + 8f;
        }

        public static void Fridge(PlayerCharacter Who, FurnitureItem Box, LifeSimMap Map)
        {
            Order(Who, Box, Map, "Eat", new EatMealAction(Who, false));
        }

        public static void Bed(PlayerCharacter Who, FurnitureItem Box, LifeSimMap Map)
        {
            CatalogItem Card = BuildCatalog.Find(Box == null ? "" : Box.Interaction);
            int Comfort = Card == null ? 0 : Card.Comfort;
            Order(Who, Box, Map, "Sleep", new SleepInBedAction(Who, Comfort));
        }

        public static bool Watch(PlayerCharacter Who, FurnitureItem Screen, LifeSimMap Map)
        {
            if (Who == null || Screen == null)
            {
                return false;
            }

            float Tile = MapCursor.TileOf(Map);
            MapInfo Home = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
            IList<FurnitureItem> Room = Home != null && Home.Furniture != null ? Home.Furniture : (Map == null ? null : Map.Furniture);
            IList<PlayerCharacter> People = Map == null ? null : Map.ListCharacter;
            List<UseSpot> Spots = FurnitureUse.Choices(Screen, Room, People, Who, Tile);
            for (int i = 0; i < Spots.Count; i++)
            {
                UseSpot Spot = Spots[i];
                List<AutomatedAction> Steps = new List<AutomatedAction>();
                if (Map != null && !Travel(Who, Map, Spot.Point, Steps, null, OpenSeat(Map == null ? null : Map.Furniture, Spot.Seat)))
                {
                    continue;
                }

                Steps.Add(new WatchShowAction(Who, Spot.Sitting));
                Who.GiveOrder("Watch", StanceSequence.Splice(Who.CurrentPose, Steps));
                return true;
            }

            return false;
        }

        public static bool Sit(PlayerCharacter Who, FurnitureItem Box, LifeSimMap Map)
        {
            if (Who == null || Box == null)
            {
                return false;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Map != null && !Travel(Who, Map, Box.Position, Steps, null, OpenSeat(Map.Furniture, Box)))
            {
                return false;
            }

            Steps.Add(new SitAction(Who));
            Who.GiveOrder("Sit", StanceSequence.Splice(Who.CurrentPose, Steps));
            return true;
        }

        public static bool Power(PlayerCharacter Who, FurnitureItem Box, LifeSimMap Map, bool Open)
        {
            if (Who == null || Box == null)
            {
                return false;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Map != null && !Travel(Who, Map, Box.Beside(), Steps, CutsFor(Box)))
            {
                return false;
            }

            Steps.Add(new ScreenPowerAction(Who, Box, Open));
            Who.GiveOrder(Open ? "Open TV" : "Close TV", Steps);
            return true;
        }

        public static bool Meet(PlayerCharacter Who, PlayerCharacter Other, LifeSimMap Map, string Goal, SocialKind Kind)
        {
            if (Who == null || Other == null)
            {
                return false;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Kind != SocialKind.Cuddle && Kind != SocialKind.More && Map != null)
            {
                Vector3 Spot = new Vector3(Other.Position.X + 20f, Other.Position.Y, Other.Position.Z);
                if (!Travel(Who, Map, Spot, Steps))
                {
                    return false;
                }
            }

            Steps.Add(new SocialStep(Who, Other, Kind));
            Who.GiveOrder(Goal, StanceSequence.Splice(Who.CurrentPose, Steps));
            return true;
        }

        public static void Cook(PlayerCharacter Who, HouseholdState Home)
        {
            if (Who == null || Home == null)
            {
                return;
            }

            PreparedMeal Meal = Kitchen.Cook(Who.Life.Cooking, Cookbook.Feast, Home.Pantry);
            if (Meal == null)
            {
                Meal = Kitchen.Cook(Who.Life.Cooking, Cookbook.Stew, Home.Pantry);
            }

            if (Meal == null)
            {
                Meal = Kitchen.FromNothing();
            }

            Kitchen.PlaceOnTable(Meal, Who.SimHour);
            Home.Meals.Add(Meal);
            Who.Life.AddRank(LifeSkill.Cooking);
        }

        static IList<FurnitureItem> OpenSeat(IList<FurnitureItem> Furniture, FurnitureItem Seat)
        {
            if (Furniture == null || Seat == null)
            {
                return Furniture;
            }

            List<FurnitureItem> Open = new List<FurnitureItem>();
            for (int i = 0; i < Furniture.Count; i++)
            {
                if (Furniture[i] != Seat)
                {
                    Open.Add(Furniture[i]);
                }
            }

            return Open;
        }

        public static bool Practice(PlayerCharacter Who, FurnitureItem Box, LifeSimMap Map, PlaySession Session)
        {
            if (Who == null || Box == null || Session == null || Session.Practice != null)
            {
                return false;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Map != null && !Travel(Who, Map, Box.Beside(), Steps, CutsFor(Box)))
            {
                return false;
            }

            Steps.Add(new BeginPracticeStep(Who, Session, Box));
            Who.GiveOrder("Practice", Steps);
            return true;
        }

        public static bool Stride(PlayerCharacter Who, Vector3 Spot, LifeSimMap Map, FurnitureItem Dummy)
        {
            if (Who == null || Map == null)
            {
                return false;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (!Travel(Who, Map, Spot, Steps, CutsFor(Dummy)))
            {
                return false;
            }

            if (!PracticeMath.StrideFits(PracticeMath.PathLength(Steps), PracticeMath.StrideUnits(Who)))
            {
                return false;
            }

            Who.GiveOrder("Stride", Steps);
            return true;
        }

        static List<NavCut> CutsFor(FurnitureItem Box)
        {
            if (Box == null)
            {
                return null;
            }

            List<NavCut> Cuts = new List<NavCut>();
            NavCut Cut = new NavCut();
            Cut.Center = Box.Position;
            Cut.Radius = Box.Width * 0.45f;
            Cuts.Add(Cut);
            return Cuts;
        }

        public static bool Walk(PlayerCharacter Who, Vector3 Spot, LifeSimMap Map)
        {
            if (Who == null || Map == null)
            {
                return false;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (!Travel(Who, Map, Spot, Steps))
            {
                return false;
            }

            Who.GiveOrder("Move", Steps);
            return true;
        }

        static void Order(PlayerCharacter Who, FurnitureItem Box, LifeSimMap Map, string Goal, AutomatedAction Use)
        {
            if (Who == null || Box == null)
            {
                return;
            }

            List<AutomatedAction> Steps = new List<AutomatedAction>();
            Vector3 Spot = Box.Beside();
            if (Map != null)
            {
                List<NavCut> Cuts = new List<NavCut>();
                NavCut Cut = new NavCut();
                Cut.Center = Box.Position;
                Cut.Radius = Box.Width * 0.45f;
                Cuts.Add(Cut);
                if (!Travel(Who, Map, Spot, Steps, Cuts))
                {
                    return;
                }
            }

            Steps.Add(Use);
            Who.GiveOrder(Goal, Steps);
        }

        static bool Travel(PlayerCharacter Who, LifeSimMap Map, Vector3 Spot, List<AutomatedAction> Steps)
        {
            return Travel(Who, Map, Spot, Steps, null);
        }

        static bool Travel(PlayerCharacter Who, LifeSimMap Map, Vector3 Spot, List<AutomatedAction> Steps, IList<NavCut> Cuts)
        {
            return Travel(Who, Map, Spot, Steps, Cuts, null);
        }

        static bool Travel(PlayerCharacter Who, LifeSimMap Map, Vector3 Spot, List<AutomatedAction> Steps, IList<NavCut> Cuts, IList<FurnitureItem> Solids)
        {
            MapInfo Home = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
            if (Home == null || Map.Chunk == null || Home == Map.Chunk || (Home.MapMapContainer != null && Home.MapMapContainer.ActiveMap == Map))
            {
                List<PathSegment> Here = Route(Map, Who.Position, Spot, Who.Body, Cuts, Solids);
                if (Here.Count == 0)
                {
                    return false;
                }

                Steps.Add(new FollowPath(Who, Here));
                return true;
            }

            List<ChunkHop> Hops = new InterMapMovementAlgorithm().FindOpenBorders(Home, Map.Chunk);
            if (Hops.Count == 0)
            {
                return false;
            }

            Vector3 From = Who.Position;
            MapInfo Leg = Home;
            for (int i = 0; i < Hops.Count; i++)
            {
                ChunkHop Hop = Hops[i];
                if (Hop.FromChunk != Leg)
                {
                    Hop = Hop.Reversed();
                }

                LifeSimMap LegMap = Active(Leg);
                Vector3 Exit = Hop.FromPosition;
                List<PathSegment> ToEdge = Route(LegMap, From, Inside(Leg, Exit), Who.Body, null);
                if (ToEdge.Count == 0)
                {
                    return false;
                }

                StampBorder(ToEdge, Exit);
                Steps.Add(new FollowPath(Who, ToEdge));
                ChunkHop Cross = Hop.Copy();
                Steps.Add(new CrossPortalAction(Who, Cross, 0));
                From = Inside(Hop.ToChunk, Hop.ToPosition);
                Leg = Hop.ToChunk;
            }

            List<PathSegment> ToSpot = Route(Map, From, Spot, Who.Body, Cuts, Solids);
            if (ToSpot.Count == 0)
            {
                return false;
            }

            Steps.Add(new FollowPath(Who, ToSpot));
            return true;
        }

        public static bool AppendJourney(PlayerCharacter Who, List<ChunkHop> Hops, Vector3 Spot, LifeSimMap GoalMap, List<AutomatedAction> Steps)
        {
            if (Who == null || Hops == null || Steps == null || Hops.Count == 0)
            {
                return false;
            }

            MapInfo Home = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
            Vector3 From = Who.Position;
            MapInfo Leg = Home;
            for (int i = 0; i < Hops.Count; i++)
            {
                ChunkHop Hop = Hops[i];
                if (Hop == null)
                {
                    return false;
                }

                if (Hop.FromChunk != Leg && Hop.ToChunk == Leg)
                {
                    Hop = Hop.Reversed();
                }

                if (Hop.IsPortal)
                {
                    List<PathSegment> ToPortal = Route(Active(Leg), Leg, From, Hop.FromPosition, Who.Body);
                    if (ToPortal.Count > 0)
                    {
                        Steps.Add(new FollowPath(Who, ToPortal));
                    }

                    Steps.Add(new CrossPortalAction(Who, Hop.Copy()));
                    From = Hop.ToPosition;
                    Leg = Hop.ToChunk;
                    continue;
                }

                Vector3 Exit = Hop.FromPosition;
                List<PathSegment> ToEdge = Route(Active(Leg), Leg, From, Inside(Leg, Exit), Who.Body);
                if (ToEdge.Count == 0)
                {
                    return false;
                }

                StampBorder(ToEdge, Exit);
                Steps.Add(new FollowPath(Who, ToEdge));
                Steps.Add(new BorderCrossAction(Who, Hop.Copy()));
                From = Inside(Hop.ToChunk, Hop.ToPosition);
                Leg = Hop.ToChunk;
            }

            MapInfo SpotChunk = GoalMap != null && GoalMap.Chunk != null ? GoalMap.Chunk : Leg;
            List<PathSegment> ToSpot = Route(GoalMap, SpotChunk, From, Spot, Who.Body);
            if (ToSpot.Count == 0)
            {
                return GoalMap == null || Leg == GoalMap.Chunk;
            }

            Steps.Add(new FollowPath(Who, ToSpot));
            return true;
        }

        static void StampBorder(List<PathSegment> Path, Vector3 Border)
        {
            PathSegment Last = Path[Path.Count - 1];
            Vector3 End = Last.Polyline.Count > 0 ? Last.Polyline[Last.Polyline.Count - 1] : Border;
            Vector3 Point = new Vector3(Border.X, Border.Y, End.Z);
            float DX = Point.X - End.X;
            float DY = Point.Y - End.Y;
            if (DX * DX + DY * DY < 0.25f)
            {
                return;
            }

            if (Last.Kind == Traversal.Walk && Last.Polyline.Count > 0)
            {
                Last.Polyline.Add(Point);
                return;
            }

            PathSegment Straight = new PathSegment();
            Straight.Polyline.Add(End);
            Straight.Polyline.Add(Point);
            Path.Add(Straight);
        }

        static LifeSimMap Active(MapInfo Chunk)
        {
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return null;
            }

            return Chunk.MapMapContainer.ActiveMap as LifeSimMap;
        }

        static Vector3 Inside(MapInfo Chunk, Vector3 Border)
        {
            if (Chunk == null || Chunk.MapMapContainer == null)
            {
                return Border;
            }

            float Margin = 8f;
            float X = Border.X;
            float Y = Border.Y;
            float SpanX = Chunk.MapMapContainer.SpanX;
            float SpanY = Chunk.MapMapContainer.SpanY;
            if (SpanX > Margin * 2f)
            {
                if (X <= 0f)
                {
                    X = Margin;
                }
                else if (X >= SpanX)
                {
                    X = SpanX - Margin;
                }
            }

            if (SpanY > Margin * 2f)
            {
                if (Y <= 0f)
                {
                    Y = Margin;
                }
                else if (Y >= SpanY)
                {
                    Y = SpanY - Margin;
                }
            }

            return new Vector3(X, Y, Border.Z);
        }

        static List<PathSegment> Route(LifeSimMap Map, Vector3 From, Vector3 To, TraversalCapabilities Body, IList<NavCut> Cuts)
        {
            return Route(Map, From, To, Body, Cuts, null);
        }

        static List<PathSegment> Route(LifeSimMap Map, Vector3 From, Vector3 To, TraversalCapabilities Body, IList<NavCut> Cuts, IList<FurnitureItem> Solids)
        {
            if (Map == null)
            {
                return new List<PathSegment>();
            }

            return Route(Map, Map.Chunk, From, To, Body, Cuts, Solids);
        }

        static List<PathSegment> Route(LifeSimMap Map, MapInfo Chunk, Vector3 From, Vector3 To, TraversalCapabilities Body)
        {
            return Route(Map, Chunk, From, To, Body, null);
        }

        static List<PathSegment> Route(LifeSimMap Map, MapInfo Chunk, Vector3 From, Vector3 To, TraversalCapabilities Body, IList<NavCut> Cuts)
        {
            return Route(Map, Chunk, From, To, Body, Cuts, null);
        }

        static List<PathSegment> Route(LifeSimMap Map, MapInfo Chunk, Vector3 From, Vector3 To, TraversalCapabilities Body, IList<NavCut> Cuts, IList<FurnitureItem> Solids)
        {
            if (Map == null && Chunk == null)
            {
                return new List<PathSegment>();
            }

            ZoneFootprint Foot = null;
            if (Chunk != null && Chunk.Zones != null)
            {
                for (int i = 0; i < Chunk.Zones.Count; i++)
                {
                    BuildingZone Zone = Chunk.Zones[i];
                    if (Zone == null || !Zone.Contains(Chunk, From.X, From.Y) || !Zone.Contains(Chunk, To.X, To.Y))
                    {
                        continue;
                    }

                    Foot = Zone.Foot(Chunk);
                    break;
                }
            }

            if (Foot != null && Foot.Interior != null && Foot.Interior.Triangles.Count > 0)
            {
                List<PathSegment> InsidePath = LocalPathQuery.Find(Foot.Interior, From, To, Body, RouteStyle.Direct);
                if (InsidePath.Count > 0)
                {
                    return InsidePath;
                }
            }

            IList<FurnitureItem> Furniture = Solids ?? (Map != null ? Map.Furniture : Chunk.Furniture);
            IList<BuildingItem> Buildings = Map != null ? Map.Buildings : Chunk.Buildings;
            List<PathSegment> Found = Map != null
                ? LocalPathQuery.Find(Map, From, To, Body, RouteStyle.Direct, false, Cuts, null, Furniture, Buildings)
                : LocalPathQuery.Find(Chunk, From, To, Body, RouteStyle.Direct, false, Cuts, null, Furniture, Buildings);
            if (Found.Count > 0)
            {
                return Found;
            }

            return AlongGround(Chunk, From, To);
        }

        static List<PathSegment> AlongGround(MapInfo Chunk, Vector3 From, Vector3 To)
        {
            List<PathSegment> Empty = new List<PathSegment>();
            HeightField Field = Chunk == null ? null : Chunk.Heights;
            if (Field == null)
            {
                return Empty;
            }

            float DX = To.X - From.X;
            float DY = To.Y - From.Y;
            float Distance = (float)Math.Sqrt(DX * DX + DY * DY);
            if (Distance < 1f)
            {
                return Empty;
            }

            int Steps = Math.Max(1, (int)Math.Ceiling(Distance / 8f));
            PathSegment Segment = new PathSegment();
            for (int i = 0; i <= Steps; i++)
            {
                float T = i / (float)Steps;
                float X = From.X + DX * T;
                float Y = From.Y + DY * T;
                float Height;
                float SlopeX;
                float SlopeY;
                if (!Field.Sample(X, Y, out Height, out SlopeX, out SlopeY))
                {
                    Height = From.Z + (To.Z - From.Z) * T;
                }

                Segment.Polyline.Add(new Vector3(X, Y, Height));
            }

            Empty.Add(Segment);
            return Empty;
        }
    }
}
