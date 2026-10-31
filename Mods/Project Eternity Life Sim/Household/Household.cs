using System.Collections.Generic;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public enum LifeSkill
    {
        None,
        Cooking,
        Gardening,
        Fishing,
        Hunting,
        Foraging,
        Blacksmithing,
        Alchemy,
        Mining,
    }

    public struct DayBonus
    {
        public int Attack;
        public int Craft;
        public double Until;

        public bool On(double Now)
        {
            return Now < Until && (Attack > 0 || Craft > 0);
        }
    }

    public class AwayWork
    {
        public LifeSkill Skill;
        public double ReturnsAt;
        public bool Busy;
    }

    public class LifeSheet
    {
        public int Cooking;
        public int Gardening;
        public int Fishing;
        public int Hunting;
        public int Foraging;
        public int Blacksmithing;
        public int Alchemy;
        public int Mining;
        public string Tool;
        public DayBonus Food;
        public DayBonus Rest;
        public AwayWork Away = new AwayWork();
        public bool HasOrder;
        public LifeSkill Order;

        public int Rank(LifeSkill Skill)
        {
            if (Skill == LifeSkill.Cooking)
            {
                return Cooking;
            }

            if (Skill == LifeSkill.Gardening)
            {
                return Gardening;
            }

            if (Skill == LifeSkill.Fishing)
            {
                return Fishing;
            }

            if (Skill == LifeSkill.Hunting)
            {
                return Hunting;
            }

            if (Skill == LifeSkill.Foraging)
            {
                return Foraging;
            }

            if (Skill == LifeSkill.Blacksmithing)
            {
                return Blacksmithing;
            }

            if (Skill == LifeSkill.Alchemy)
            {
                return Alchemy;
            }

            if (Skill == LifeSkill.Mining)
            {
                return Mining;
            }

            return 0;
        }

        public void AddRank(LifeSkill Skill)
        {
            int Next = Rank(Skill) + 1;
            if (Next > 10)
            {
                Next = 10;
            }

            Set(Skill, Next);
        }

        public void Set(LifeSkill Skill, int Rank)
        {
            if (Skill == LifeSkill.Cooking)
            {
                Cooking = Rank;
            }
            else if (Skill == LifeSkill.Gardening)
            {
                Gardening = Rank;
            }
            else if (Skill == LifeSkill.Fishing)
            {
                Fishing = Rank;
            }
            else if (Skill == LifeSkill.Hunting)
            {
                Hunting = Rank;
            }
            else if (Skill == LifeSkill.Foraging)
            {
                Foraging = Rank;
            }
            else if (Skill == LifeSkill.Blacksmithing)
            {
                Blacksmithing = Rank;
            }
            else if (Skill == LifeSkill.Alchemy)
            {
                Alchemy = Rank;
            }
            else if (Skill == LifeSkill.Mining)
            {
                Mining = Rank;
            }
        }

        public int AttackNow(double Now)
        {
            int Total = 0;
            if (Food.On(Now))
            {
                Total += Food.Attack;
            }

            if (Rest.On(Now))
            {
                Total += Rest.Attack;
            }

            return Total;
        }

        public int CraftNow(double Now)
        {
            int Total = 0;
            if (Food.On(Now))
            {
                Total += Food.Craft;
            }

            if (Rest.On(Now))
            {
                Total += Rest.Craft;
            }

            return Total;
        }
    }

    public class Recipe
    {
        public string Name;
        public int Level;
        public string[] Ingredients;
        public int Quality;

        public Recipe(string Name, int Level, string[] Ingredients, int Quality)
        {
            this.Name = Name;
            this.Level = Level;
            this.Ingredients = Ingredients == null ? new string[0] : Ingredients;
            this.Quality = Quality;
        }
    }

    public class PreparedMeal
    {
        public string Name;
        public int Quality;
        public bool OnTable;
        public double PlacedAt;
    }

    public static class Cookbook
    {
        public static readonly Recipe Scraps = new Recipe("Scraps", 0, new string[0], 0);
        public static readonly Recipe Stew = new Recipe("Stew", 1, new string[] { "Meat", "Herb" }, 2);
        public static readonly Recipe Feast = new Recipe("Feast", 4, new string[] { "Meat", "Fish", "Herb" }, 4);
        public static readonly Recipe Blade = new Recipe("Blade", 1, new string[] { "Tin" }, 0);
        public static readonly Recipe BronzeBar = new Recipe("Bronze Bar", 1, new string[] { "Tin" }, 0);
        public static readonly Recipe BronzePick = new Recipe("Bronze Pick", 1, new string[] { "Bronze Bar" }, 0);
        public static readonly Recipe IronBar = new Recipe("Iron Bar", 4, new string[] { "Iron" }, 0);
        public static readonly Recipe IronPick = new Recipe("Iron Pick", 4, new string[] { "Iron Bar" }, 0);
        public static readonly Recipe IronBlade = new Recipe("Iron Blade", 4, new string[] { "Iron", "Iron Ingot" }, 0);
        public static readonly Recipe Tonic = new Recipe("Tonic", 1, new string[] { "Herb" }, 0);

        public static readonly Recipe[] Smithing = new Recipe[] { IronPick, IronBlade, IronBar, BronzePick, BronzeBar, Blade };

        public static Recipe BestSmith(LifeSheet Sheet, Dictionary<string, int> Pantry)
        {
            if (Sheet == null)
            {
                return null;
            }

            int Rank = Sheet.Rank(LifeSkill.Blacksmithing);
            for (int i = 0; i < Smithing.Length; i++)
            {
                Recipe Next = Smithing[i];
                if (Kitchen.CanCook(Rank, Next, Pantry))
                {
                    return Next;
                }
            }

            return null;
        }

        public static readonly Recipe[] Meals = new Recipe[] { Scraps, Stew, Feast };
    }

    public static class Kitchen
    {
        public static PreparedMeal FromNothing()
        {
            PreparedMeal Meal = new PreparedMeal();
            Meal.Name = Cookbook.Scraps.Name;
            Meal.Quality = 0;
            return Meal;
        }

        public static bool CanCook(int Rank, Recipe Recipe, Dictionary<string, int> Pantry)
        {
            return Recipe != null && Rank >= Recipe.Level && Has(Pantry, Recipe);
        }

        public static PreparedMeal Cook(int Rank, Recipe Recipe, Dictionary<string, int> Pantry)
        {
            if (Recipe == null)
            {
                return null;
            }

            if (Recipe.Ingredients.Length == 0)
            {
                return FromNothing();
            }

            if (Rank < Recipe.Level || !Has(Pantry, Recipe))
            {
                return null;
            }

            for (int i = 0; i < Recipe.Ingredients.Length; i++)
            {
                Pantry[Recipe.Ingredients[i]]--;
            }

            PreparedMeal Meal = new PreparedMeal();
            Meal.Name = Recipe.Name;
            int Quality = Recipe.Quality + (Rank - Recipe.Level) / 2;
            if (Quality > 5)
            {
                Quality = 5;
            }

            Meal.Quality = Quality;
            return Meal;
        }

        public static void PlaceOnTable(PreparedMeal Meal, double Now)
        {
            if (Meal == null)
            {
                return;
            }

            Meal.OnTable = true;
            Meal.PlacedAt = Now;
        }

        public static void Tick(List<PreparedMeal> Meals, double Now)
        {
            if (Meals == null)
            {
                return;
            }

            for (int i = Meals.Count - 1; i >= 0; i--)
            {
                PreparedMeal Meal = Meals[i];
                if (Meal != null && Meal.OnTable && Now - Meal.PlacedAt >= 12)
                {
                    Meals.RemoveAt(i);
                }
            }
        }

        public static void Eat(LifeSheet Sheet, PreparedMeal Meal, double Now, PlayerCharacter Who)
        {
            if (Who != null)
            {
                Who.Hunger -= Meal != null && Meal.Quality > 0 ? 40 : 25;
                if (Who.Hunger < 0)
                {
                    Who.Hunger = 0;
                }
            }

            if (Sheet == null || Meal == null || Meal.Quality <= 0)
            {
                return;
            }

            DayBonus Bonus = new DayBonus();
            Bonus.Attack = Meal.Quality;
            Bonus.Craft = Meal.Quality >= 3 ? 1 : 0;
            Bonus.Until = Now + 24;
            Sheet.Food = Bonus;
        }

        static bool Has(Dictionary<string, int> Pantry, Recipe Recipe)
        {
            if (Pantry == null)
            {
                return false;
            }

            for (int i = 0; i < Recipe.Ingredients.Length; i++)
            {
                int Have;
                if (!Pantry.TryGetValue(Recipe.Ingredients[i], out Have) || Have <= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public static class Resting
    {
        public static void Sleep(LifeSheet Sheet, int Comfort, double Now, PlayerCharacter Who)
        {
            if (Who != null)
            {
                Who.Sleep -= 30;
                if (Who.Sleep < 0)
                {
                    Who.Sleep = 0;
                }
            }

            if (Sheet == null || Comfort <= 0)
            {
                return;
            }

            DayBonus Bonus = new DayBonus();
            Bonus.Attack = Comfort / 2;
            Bonus.Craft = Comfort / 4;
            if (Bonus.Attack < 1)
            {
                Bonus.Attack = 1;
            }

            Bonus.Until = Now + 24;
            Sheet.Rest = Bonus;
        }
    }

    public class HouseholdStations
    {
        public bool Kitchen = true;
        public bool Garden;
        public bool Fishing;
        public bool Forge;
        public bool Lab;
        public bool Rocks;
        public int Beds;
        public int Residents;
        public bool LarderLow = true;
    }

    public struct TaskPick
    {
        public bool Act;
        public LifeSkill Skill;
    }

    public static class Work
    {
        public static bool StartAway(LifeSheet Sheet, LifeSkill Skill, double Now)
        {
            if (Sheet == null || Sheet.Away.Busy || (Skill != LifeSkill.Hunting && Skill != LifeSkill.Foraging))
            {
                return false;
            }

            Sheet.Away.Busy = true;
            Sheet.Away.Skill = Skill;
            Sheet.Away.ReturnsAt = Now + (Skill == LifeSkill.Hunting ? 4 : 3);
            return true;
        }

        public static string FinishAway(LifeSheet Sheet, double Now, Dictionary<string, int> Pantry)
        {
            if (Sheet == null || !Sheet.Away.Busy || Now < Sheet.Away.ReturnsAt)
            {
                return null;
            }

            int Amount = 1 + Sheet.Rank(Sheet.Away.Skill) / 2 + Sheet.CraftNow(Now);
            string Item = Sheet.Away.Skill == LifeSkill.Hunting ? "Meat" : "Herb";
            Add(Pantry, Item, Amount);
            Sheet.AddRank(Sheet.Away.Skill);
            Sheet.Away.Busy = false;
            return Item;
        }

        public static string Tend(LifeSheet Sheet, LifeSkill Skill, Dictionary<string, int> Pantry)
        {
            if (Sheet == null || (Skill != LifeSkill.Gardening && Skill != LifeSkill.Fishing))
            {
                return null;
            }

            string Item = Skill == LifeSkill.Gardening ? "Herb" : "Fish";
            Add(Pantry, Item, 1 + Sheet.Rank(Skill) / 3);
            Sheet.AddRank(Skill);
            return Item;
        }

        public static string Craft(LifeSheet Sheet, Recipe Recipe, Dictionary<string, int> Pantry)
        {
            return Craft(Sheet, Recipe, Pantry, null);
        }

        public static string Craft(LifeSheet Sheet, Recipe Recipe, Dictionary<string, int> Pantry, Dictionary<string, int> Reserved)
        {
            if (Sheet == null || Recipe == null)
            {
                return null;
            }

            LifeSkill Skill = Recipe == Cookbook.Tonic ? LifeSkill.Alchemy : LifeSkill.Blacksmithing;
            if (Sheet.Rank(Skill) < Recipe.Level)
            {
                return null;
            }

            PreparedMeal Made = Kitchen.Cook(Sheet.Rank(Skill), Recipe, Pantry);
            if (Made == null)
            {
                return null;
            }

            Add(Pantry, Made.Name, 1);
            Stock.Spend(Reserved, Recipe);
            Sheet.AddRank(Skill);
            return Made.Name;
        }

        public static TaskPick Choose(LifeSheet Sheet, bool CriticalHunger, bool CriticalSleep, HouseholdStations Stations)
        {
            TaskPick Pick = new TaskPick();
            if (Sheet == null || CriticalHunger || CriticalSleep)
            {
                return Pick;
            }

            if (Sheet.HasOrder)
            {
                Pick.Act = Sheet.Order != LifeSkill.None;
                Pick.Skill = Sheet.Order;
                return Pick;
            }

            LifeSkill Best = LifeSkill.None;
            int Rank = -1;
            Consider(Sheet, LifeSkill.Cooking, Stations.Kitchen, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Gardening, Stations.Garden, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Fishing, Stations.Fishing, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Hunting, Stations.LarderLow, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Foraging, Stations.LarderLow, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Blacksmithing, Stations.Forge, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Alchemy, Stations.Lab, ref Best, ref Rank);
            Consider(Sheet, LifeSkill.Mining, Stations.Rocks, ref Best, ref Rank);
            Pick.Act = Best != LifeSkill.None;
            Pick.Skill = Best;
            return Pick;
        }

        static void Consider(LifeSheet Sheet, LifeSkill Skill, bool Open, ref LifeSkill Best, ref int Rank)
        {
            if (!Open)
            {
                return;
            }

            int Value = Sheet.Rank(Skill);
            if (Value > Rank)
            {
                Rank = Value;
                Best = Skill;
            }
        }

        public static void Add(Dictionary<string, int> Pantry, string Item, int Amount)
        {
            if (Pantry == null || string.IsNullOrEmpty(Item) || Amount <= 0)
            {
                return;
            }

            int Have;
            Pantry.TryGetValue(Item, out Have);
            Pantry[Item] = Have + Amount;
        }
    }

    public class HouseholdState
    {
        public const string Id = "House";

        public int Funds = 2000;
        public readonly DungeonParty Party = new DungeonParty();
        public readonly Dictionary<string, int> Pantry = new Dictionary<string, int>();
        public readonly List<PreparedMeal> Meals = new List<PreparedMeal>();
        public readonly List<LifeSheet> Workers = new List<LifeSheet>();
        public readonly HouseholdStations Stations = new HouseholdStations();
        public readonly Dictionary<string, int> Reserved = new Dictionary<string, int>();
        public readonly List<GatherNode> Nodes = new List<GatherNode>();
        public readonly List<HouseholdRequest> Requests = new List<HouseholdRequest>();
        public readonly List<SkillRecord> Skills = new List<SkillRecord>();
        public readonly Dictionary<string, int> Pouch = new Dictionary<string, int>();
        public bool RoadOpen;
        public DungeonRun Dungeon;

        public void Track(LifeSheet Sheet)
        {
            if (Sheet != null && !Workers.Contains(Sheet))
            {
                Workers.Add(Sheet);
            }
        }

        public DungeonRun Run()
        {
            if (Dungeon == null)
            {
                Dungeon = new DungeonRun();
                Dungeon.Party = Party;
            }

            return Dungeon;
        }

        public void Tick(double Now)
        {
            Kitchen.Tick(Meals, Now);
            for (int i = 0; i < Workers.Count; i++)
            {
                Work.FinishAway(Workers[i], Now, Pantry);
            }
        }
    }

    public static class PlotSale
    {
        public static bool TryBuy(BuildingZone Zone, string Buyer, ref int Funds)
        {
            if (Zone == null || Zone.Price <= 0 || !string.IsNullOrEmpty(Zone.OwnerId) || string.IsNullOrEmpty(Buyer) || Funds < Zone.Price)
            {
                return false;
            }

            Funds -= Zone.Price;
            Zone.OwnerId = Buyer;
            return true;
        }

        public static bool StandsAnywhere(string Item)
        {
            return Item == "Dungeon Gate";
        }

        public static string Offer(BuildingZone Zone, string Buyer, int Funds)
        {
            if (Zone == null)
            {
                return "No plot here. Mark one in the admin menu and set a price.";
            }

            if (!string.IsNullOrEmpty(Zone.OwnerId))
            {
                return Zone.OwnerId == Buyer ? "Your land" : "This land is owned";
            }

            if (Zone.Price <= 0)
            {
                return "This plot is not for sale";
            }

            if (Funds < Zone.Price)
            {
                return "This land costs " + Zone.Price + ". You have " + Funds + ".";
            }

            return "B buys this land for " + Zone.Price;
        }

        public static bool MayBuild(BuildingZone Zone, string Buyer, bool Admin)
        {
            if (Admin)
            {
                return Zone != null;
            }

            if (Zone == null || string.IsNullOrEmpty(Zone.OwnerId))
            {
                return false;
            }

            return Zone.OwnerId == Buyer;
        }
    }

    public static class HouseholdRooms
    {
        public static int Beds(BuildingZone Zone)
        {
            if (Zone == null)
            {
                return 0;
            }

            int Count = 0;
            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                ZoneFootprint Foot = Zone.Footprints[i];
                if (Foot == null || Foot.Chunk == null || Foot.Chunk.Furniture == null)
                {
                    continue;
                }

                for (int f = 0; f < Foot.Chunk.Furniture.Count; f++)
                {
                    FurnitureItem Box = Foot.Chunk.Furniture[f];
                    if (Box != null && (Box.Interaction == "Bed" || Box.Interaction == "Tent") && Zone.Contains(Foot.Chunk, Box.Position.X, Box.Position.Y))
                    {
                        Count++;
                    }
                }
            }

            return Count;
        }
    }

    public static class LifeSync
    {
        public static HouseholdState Home;
        public static DungeonRun Dungeon;

        public static byte[] Capture()
        {
            ByteWriter Writer = new ByteWriter();
            Write(Writer, Home, Dungeon);
            byte[] Data = Writer.GetBytes();
            Writer.ClearWriteBuffer();
            return Data;
        }

        public static void Restore(byte[] Data)
        {
            if (Data == null || Data.Length == 0)
            {
                return;
            }

            ByteReader Reader = new ByteReader(Data);
            HouseholdState HomeState;
            DungeonRun Run;
            Read(Reader, out HomeState, out Run);
            Home = HomeState;
            Dungeon = Run;
        }

        public static void Write(ByteWriter Writer, HouseholdState Home, DungeonRun Run)
        {
            if (Writer == null)
            {
                return;
            }

            Writer.AppendBoolean(Home != null);
            if (Home == null)
            {
                return;
            }

            Writer.AppendInt32(Home.Funds);
            Writer.AppendBoolean(Home.Party.Started);
            Writer.AppendInt32(Home.Party.Roster.Count);
            for (int i = 0; i < Home.Party.Roster.Count; i++)
            {
                PartyMember Who = Home.Party.Roster[i];
                Writer.AppendString(Who.Id ?? "");
                Writer.AppendString(Who.Name ?? "");
                Writer.AppendInt32(Who.Seat);
                Writer.AppendBoolean(Who.InParty);
                Writer.AppendBoolean(Who.Resident);
                Writer.AppendInt32((int)Who.Trade);
                Writer.AppendString(Who.Tool ?? "");
            }

            Writer.AppendInt32(Home.Pantry.Count);
            foreach (KeyValuePair<string, int> Item in Home.Pantry)
            {
                Writer.AppendString(Item.Key ?? "");
                Writer.AppendInt32(Item.Value);
            }

            Writer.AppendInt32(Home.Meals.Count);
            for (int i = 0; i < Home.Meals.Count; i++)
            {
                PreparedMeal Meal = Home.Meals[i];
                Writer.AppendString(Meal.Name ?? "");
                Writer.AppendInt32(Meal.Quality);
                Writer.AppendBoolean(Meal.OnTable);
                Writer.AppendDouble(Meal.PlacedAt);
            }

            WorkBook.Write(Writer, Home);
            Writer.AppendBoolean(Run != null);
            if (Run == null)
            {
                return;
            }

            Writer.AppendInt32(Run.Seed);
            Writer.AppendInt32(Run.FloorCount);
            Writer.AppendInt32(Run.Floors.Count);
            foreach (KeyValuePair<int, DungeonFloor> Pair in Run.Floors)
            {
                DungeonFloor Floor = Pair.Value;
                Writer.AppendInt32(Floor.Index);
                Writer.AppendInt32(Floor.EntryX);
                Writer.AppendInt32(Floor.EntryY);
                Writer.AppendInt32(Floor.DownX);
                Writer.AppendInt32(Floor.DownY);
                Writer.AppendInt32(Floor.UpX);
                Writer.AppendInt32(Floor.UpY);
            }

            Writer.AppendInt32(Run.FloorOf.Count);
            foreach (KeyValuePair<string, int> Pair in Run.FloorOf)
            {
                Writer.AppendString(Pair.Key ?? "");
                Writer.AppendInt32(Pair.Value);
            }

            Writer.AppendInt32(Run.Wing);
        }

        public static void Read(ByteReader Reader, out HouseholdState Home, out DungeonRun Run)
        {
            Home = null;
            Run = null;
            if (Reader == null || !Reader.ReadBoolean())
            {
                return;
            }

            Home = new HouseholdState();
            Home.Funds = Reader.ReadInt32();
            Home.Party.Started = Reader.ReadBoolean();
            int People = Reader.ReadInt32();
            for (int i = 0; i < People; i++)
            {
                PartyMember Who = new PartyMember();
                Who.Id = Reader.ReadString();
                Who.Name = Reader.ReadString();
                Who.Seat = Reader.ReadInt32();
                Who.InParty = Reader.ReadBoolean();
                Who.Resident = Reader.ReadBoolean();
                Who.Trade = (LifeSkill)Reader.ReadInt32();
                Who.Tool = Reader.ReadString();
                Home.Party.Roster.Add(Who);
            }

            int Pantry = Reader.ReadInt32();
            for (int i = 0; i < Pantry; i++)
            {
                string Key = Reader.ReadString();
                Home.Pantry[Key] = Reader.ReadInt32();
            }

            int Meals = Reader.ReadInt32();
            for (int i = 0; i < Meals; i++)
            {
                PreparedMeal Meal = new PreparedMeal();
                Meal.Name = Reader.ReadString();
                Meal.Quality = Reader.ReadInt32();
                Meal.OnTable = Reader.ReadBoolean();
                Meal.PlacedAt = Reader.ReadDouble();
                Home.Meals.Add(Meal);
            }

            WorkBook.Read(Reader, Home);
            if (!Reader.ReadBoolean())
            {
                return;
            }

            Run = new DungeonRun();
            Run.Party = Home.Party;
            Run.Seed = Reader.ReadInt32();
            Run.FloorCount = Reader.ReadInt32();
            int Floors = Reader.ReadInt32();
            for (int i = 0; i < Floors; i++)
            {
                DungeonFloor Floor = new DungeonFloor();
                Floor.Index = Reader.ReadInt32();
                Floor.EntryX = Reader.ReadInt32();
                Floor.EntryY = Reader.ReadInt32();
                Floor.DownX = Reader.ReadInt32();
                Floor.DownY = Reader.ReadInt32();
                Floor.UpX = Reader.ReadInt32();
                Floor.UpY = Reader.ReadInt32();
                Run.Floors[Floor.Index] = Floor;
            }

            int Standing = Reader.ReadInt32();
            for (int i = 0; i < Standing; i++)
            {
                string Id = Reader.ReadString();
                Run.FloorOf[Id] = Reader.ReadInt32();
            }

            Run.Wing = Reader.ReadInt32();
            Home.Dungeon = Run;
        }
    }

    public static class HouseholdMind
    {
        public static void Act(PlayerCharacter Who, HouseholdState Home, double Now)
        {
            if (Who == null || Home == null)
            {
                return;
            }

            Trades.Apply(Who, Home);
            SkillBook.ReadOnto(Home, Who);
            Dictionary<string, int> Pile = TownLife.Pile(Who, Home.Pantry);
            Stock.Equip(Who.Life, Pile);
            Home.Stations.LarderLow = Stock.Short(Pile);
            Home.Stations.Rocks = Gather.Offers(Home, Who.Life);
            Requests.Consider(Home, Who.Life, Who.Name);
            TaskPick Pick = Work.Choose(Who.Life, Who.Hunger >= 80, Who.Sleep >= 80, Home.Stations);
            if (!Pick.Act)
            {
                return;
            }

            if (Pick.Skill == LifeSkill.Mining)
            {
                WorkJob.Assign(Who, Home, Pick.Skill, Now);
                return;
            }

            if (WorkJob.Assign(Who, Home, Pick.Skill, Now))
            {
                return;
            }

            if (Pick.Skill == LifeSkill.Hunting || Pick.Skill == LifeSkill.Foraging)
            {
                if (Who.CityId >= 0)
                {
                    TownBook.DepositAway(Who.Life, Pick.Skill, Pile);
                }
                else
                {
                    DungeonOrders.SendAway(Who, Pick.Skill, Home, Now);
                }

                return;
            }

            if (Pick.Skill == LifeSkill.Gardening || Pick.Skill == LifeSkill.Fishing)
            {
                Work.Tend(Who.Life, Pick.Skill, Pile);
                return;
            }

            if (Pick.Skill == LifeSkill.Cooking)
            {
                PreparedMeal Meal = Kitchen.Cook(Who.Life.Cooking, Cookbook.Stew, Pile);
                if (Meal == null)
                {
                    Meal = Kitchen.FromNothing();
                }

                Kitchen.PlaceOnTable(Meal, Now);
                Home.Meals.Add(Meal);
                Who.Life.AddRank(LifeSkill.Cooking);
                return;
            }

            if (Pick.Skill == LifeSkill.Blacksmithing)
            {
                Recipe Next = Cookbook.BestSmith(Who.Life, Pile);
                Work.Craft(Who.Life, Next ?? Cookbook.Blade, Pile, Home.Reserved);
            }
            else if (Pick.Skill == LifeSkill.Alchemy)
            {
                Work.Craft(Who.Life, Cookbook.Tonic, Pile, Home.Reserved);
            }
        }
    }
}
