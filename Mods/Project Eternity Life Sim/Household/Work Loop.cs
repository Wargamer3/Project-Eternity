using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Online;

namespace ProjectEternity.GameScreens.LifeSimScreen
{
    public class GatherNode
    {
        public string Id;
        public string Resource;
        public LifeSkill Skill;
        public int MinRank = 1;
        public int ToolTier = 1;
        public double ReadyAt;
        public double Respawn = 1;
        public Vector3 Place;
        public string Biome = Biomes.Grass;
        public MapInfo Chunk;
    }

    public class HouseholdRequest
    {
        public string Item;
        public string Who;
        public string Why;
        public bool Open = true;
    }

    public class SkillRecord
    {
        public string Id;
        public int Mining;
        public int Cooking;
        public int Gardening;
        public int Fishing;
        public int Hunting;
        public int Foraging;
        public int Blacksmithing;
        public int Alchemy;
        public string Tool;
        public LifeSkill Trade;

        public int Rank(LifeSkill Skill)
        {
            if (Skill == LifeSkill.Mining)
            {
                return Mining;
            }

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

            return 0;
        }
    }

    public static class Biomes
    {
        public const string Grass = "Grass";
        public const string HighlandName = "Highland";
        public const string Road = "Highland Road";

        public static string Of(MapInfo Chunk)
        {
            if (Chunk == null)
            {
                return Grass;
            }

            if (Chunk.ChunkX == 4 && Chunk.ChunkZ == 0)
            {
                return HighlandName;
            }

            if (Chunk.MapMapContainer != null && Chunk.MapMapContainer.AreaName == HighlandName)
            {
                return HighlandName;
            }

            return Grass;
        }

        public static bool Open(HouseholdState Home, string Biome)
        {
            if (string.IsNullOrEmpty(Biome) || Biome == Grass)
            {
                return true;
            }

            return Home != null && Home.RoadOpen && Biome == HighlandName;
        }

        public static int Wing(MapInfo Chunk, HouseholdState Home)
        {
            if (Of(Chunk) == HighlandName && Home != null && Home.RoadOpen)
            {
                return 1;
            }

            return 0;
        }

        public static void AfterBuy(HouseholdState Home, BuildingZone Zone)
        {
            if (Home == null || Zone == null || !OnHighland(Zone))
            {
                return;
            }

            Home.RoadOpen = true;
            Requests.Finish(Home, Road);
        }

        public static bool OnHighland(BuildingZone Zone)
        {
            if (Zone == null)
            {
                return false;
            }

            if (Zone.Name == Road || Of(Zone.EntryChunk) == HighlandName)
            {
                return true;
            }

            for (int i = 0; i < Zone.Footprints.Count; i++)
            {
                ZoneFootprint Foot = Zone.Footprints[i];
                if (Foot != null && Of(Foot.Chunk) == HighlandName)
                {
                    return true;
                }
            }

            return false;
        }

        public static ChunkHop Gate(MapInfo From, MapInfo To, HouseholdState Home)
        {
            if (From == null || To == null)
            {
                return null;
            }

            if (From.ListLink == null)
            {
                From.ListLink = new List<ChunkHop>();
            }

            if (To.ListLink == null)
            {
                To.ListLink = new List<ChunkHop>();
            }

            bool Open = Home != null && Home.RoadOpen;
            ChunkHop Hop = Find(From, To);
            if (Hop == null)
            {
                Hop = new ChunkHop();
                Hop.Name = Road;
                Hop.FromChunk = From;
                Hop.ToChunk = To;
                Hop.FromPosition = new Vector3(8f, 0f, 0f);
                Hop.ToPosition = new Vector3(8f, 0f, 0f);
                From.ListLink.Add(Hop);
            }

            ChunkHop Back = Find(To, From);
            if (Back == null)
            {
                Back = Hop.Reversed();
                Back.Name = Road;
                To.ListLink.Add(Back);
            }

            Hop.IsOpen = Open;
            Back.IsOpen = Open;
            return Hop;
        }

        static ChunkHop Find(MapInfo From, MapInfo To)
        {
            if (From == null || From.ListLink == null)
            {
                return null;
            }

            for (int i = 0; i < From.ListLink.Count; i++)
            {
                ChunkHop Link = From.ListLink[i];
                if (Link != null && Link.Name == Road && Link.Joins(From, To))
                {
                    return Link;
                }
            }

            return null;
        }
    }

    public static class Trades
    {
        public static string Starter(LifeSkill Skill)
        {
            if (Skill == LifeSkill.Mining)
            {
                return "Bronze Pick";
            }

            return "";
        }

        public static int Tier(string Tool)
        {
            if (Tool == "Iron Pick")
            {
                return 2;
            }

            if (Tool == "Bronze Pick")
            {
                return 1;
            }

            return 0;
        }

        public static string Station(LifeSkill Skill)
        {
            if (Skill == LifeSkill.Blacksmithing)
            {
                return "Forge";
            }

            if (Skill == LifeSkill.Alchemy)
            {
                return "Alchemy Bench";
            }

            if (Skill == LifeSkill.Gardening)
            {
                return "Garden Plot";
            }

            if (Skill == LifeSkill.Fishing)
            {
                return "Fishing Spot";
            }

            return null;
        }

        public static LifeSkill StationSkill(string Name)
        {
            if (Name == "Forge")
            {
                return LifeSkill.Blacksmithing;
            }

            if (Name == "Alchemy Bench")
            {
                return LifeSkill.Alchemy;
            }

            if (Name == "Garden Plot")
            {
                return LifeSkill.Gardening;
            }

            if (Name == "Fishing Spot")
            {
                return LifeSkill.Fishing;
            }

            return LifeSkill.None;
        }

        public static bool Offered(string Name, HouseholdState Home)
        {
            LifeSkill Need = StationSkill(Name);
            if (Need == LifeSkill.None || Home == null)
            {
                return true;
            }

            return Lives(Home, Need);
        }

        public static bool Lives(HouseholdState Home, LifeSkill Skill)
        {
            if (Home == null || Skill == LifeSkill.None)
            {
                return false;
            }

            for (int i = 0; i < Home.Party.Roster.Count; i++)
            {
                PartyMember Who = Home.Party.Roster[i];
                if (Who != null && Who.Resident && Who.Trade == Skill)
                {
                    return true;
                }
            }

            for (int i = 0; i < Home.Skills.Count; i++)
            {
                SkillRecord Record = Home.Skills[i];
                if (Record != null && (Record.Trade == Skill || Record.Rank(Skill) > 0))
                {
                    return true;
                }
            }

            return false;
        }

        public static void Give(PlayerCharacter Who, LifeSkill Skill)
        {
            if (Who == null || Skill == LifeSkill.None)
            {
                return;
            }

            if (Who.Life.Rank(Skill) < 1)
            {
                Who.Life.Set(Skill, 1);
            }

            string Tool = Starter(Skill);
            if (string.IsNullOrEmpty(Tool))
            {
                return;
            }

            if (string.IsNullOrEmpty(Who.Life.Tool))
            {
                Who.Life.Tool = Tool;
            }

            if (!Who.IsCarrying(Tool))
            {
                Item Gear = new Item();
                Gear.Name = Tool;
                Gear.ArrayTags = new string[] { Tool };
                Who.AddItem(new ItemInfo(Gear, 1));
            }
        }

        public static void Apply(PlayerCharacter Who, HouseholdState Home)
        {
            if (Who == null || Home == null)
            {
                return;
            }

            PartyMember Member = Match(Home, Who);
            if (Member == null || Member.Trade == LifeSkill.None)
            {
                return;
            }

            Give(Who, Member.Trade);
            if (string.IsNullOrEmpty(Member.Tool))
            {
                Member.Tool = Who.Life.Tool;
            }
            else if (string.IsNullOrEmpty(Who.Life.Tool))
            {
                Who.Life.Tool = Member.Tool;
            }
        }

        public static PartyMember Match(HouseholdState Home, PlayerCharacter Who)
        {
            if (Home == null || Who == null)
            {
                return null;
            }

            PartyMember Found = Home.Party.Find(Who.PartyId);
            if (Found == null)
            {
                Found = Home.Party.Find(Who.Name);
            }

            return Found;
        }

        public static LifeSkill ByFloor(int Index, int n)
        {
            LifeSkill[] Tradeset = new LifeSkill[]
            {
                LifeSkill.Mining,
                LifeSkill.Blacksmithing,
                LifeSkill.Alchemy,
                LifeSkill.Cooking,
                LifeSkill.Gardening,
                LifeSkill.Fishing,
                LifeSkill.Hunting,
                LifeSkill.Foraging,
            };

            int At = Index + n;
            if (At < 0)
            {
                At = 0;
            }

            return Tradeset[At % Tradeset.Length];
        }
    }

    public static class Prices
    {
        public static int Of(string Item)
        {
            if (Item == "Tin" || Item == "Ore" || Item == "Copper")
            {
                return 5;
            }

            if (Item == "Iron")
            {
                return 15;
            }

            if (Item == "Bronze Bar")
            {
                return 12;
            }

            if (Item == "Iron Bar")
            {
                return 30;
            }

            if (Item == "Blade")
            {
                return 20;
            }

            if (Item == "Iron Ingot")
            {
                return 25;
            }

            if (Item == "Meat")
            {
                return 4;
            }

            if (Item == "Herb")
            {
                return 2;
            }

            if (Item == "Fish")
            {
                return 4;
            }

            if (Item == "Tonic")
            {
                return 8;
            }

            return 0;
        }
    }

    public static class Ores
    {
        public static bool Metal(string Item)
        {
            return Item == "Tin" || Item == "Copper" || Item == "Ore" || Item == "Iron";
        }
    }

    public static class Stock
    {
        public static bool Short(Dictionary<string, int> Pantry)
        {
            if (Pantry == null)
            {
                return true;
            }

            int Meat;
            int Herb;
            Pantry.TryGetValue("Meat", out Meat);
            Pantry.TryGetValue("Herb", out Herb);
            return Meat < 4 || Herb < 4;
        }

        public static int Count(Dictionary<string, int> Pantry, string Item)
        {
            if (Pantry == null || string.IsNullOrEmpty(Item))
            {
                return 0;
            }

            int Have;
            Pantry.TryGetValue(Item, out Have);
            return Have;
        }

        public static bool Needs(HouseholdState Home, string Item)
        {
            if (Home == null || !Ores.Metal(Item))
            {
                return false;
            }

            return Trades.Lives(Home, LifeSkill.Blacksmithing);
        }

        public static void Receive(HouseholdState Home, string Item, int Amount)
        {
            if (Home == null || string.IsNullOrEmpty(Item) || Amount <= 0)
            {
                return;
            }

            if (Needs(Home, Item))
            {
                Work.Add(Home.Pantry, Item, Amount);
                Work.Add(Home.Reserved, Item, Amount);
                return;
            }

            int Price = Prices.Of(Item);
            if (Price > 0)
            {
                Home.Funds += Price * Amount;
                return;
            }

            Work.Add(Home.Pantry, Item, Amount);
        }

        public static void Spend(Dictionary<string, int> Reserved, Recipe Recipe)
        {
            if (Reserved == null || Recipe == null)
            {
                return;
            }

            for (int i = 0; i < Recipe.Ingredients.Length; i++)
            {
                string Item = Recipe.Ingredients[i];
                int Have;
                if (!Reserved.TryGetValue(Item, out Have) || Have <= 0)
                {
                    continue;
                }

                Reserved[Item] = Have - 1;
            }
        }

        public static void Equip(LifeSheet Sheet, Dictionary<string, int> Pantry)
        {
            if (Sheet == null)
            {
                return;
            }

            string Best = null;
            if (Sheet.Rank(LifeSkill.Mining) >= 4 && Count(Pantry, "Iron Pick") > 0)
            {
                Best = "Iron Pick";
            }
            else if (Count(Pantry, "Bronze Pick") > 0 && Trades.Tier(Sheet.Tool) < 1)
            {
                Best = "Bronze Pick";
            }

            if (Best == null || Sheet.Tool == Best)
            {
                return;
            }

            int Have = Count(Pantry, Best);
            Pantry[Best] = Have - 1;
            if (!string.IsNullOrEmpty(Sheet.Tool))
            {
                Work.Add(Pantry, Sheet.Tool, 1);
            }

            Sheet.Tool = Best;
        }
    }

    public static class Gather
    {
        public static bool CanMine(LifeSheet Sheet, GatherNode Node, HouseholdState Home, double Now)
        {
            if (Sheet == null || Node == null || Now < Node.ReadyAt)
            {
                return false;
            }

            if (!Biomes.Open(Home, Node.Biome))
            {
                return false;
            }

            if (Sheet.Rank(Node.Skill) < Node.MinRank)
            {
                return false;
            }

            if (Node.Skill == LifeSkill.Mining && Trades.Tier(Sheet.Tool) < Node.ToolTier)
            {
                return false;
            }

            return true;
        }

        public static bool Offers(HouseholdState Home, LifeSheet Sheet)
        {
            if (Home == null || Sheet == null)
            {
                return false;
            }

            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                GatherNode Node = Home.Nodes[i];
                if (Node == null || Node.Skill != LifeSkill.Mining || !Biomes.Open(Home, Node.Biome))
                {
                    continue;
                }

                if (Sheet.Rank(LifeSkill.Mining) >= Node.MinRank && Trades.Tier(Sheet.Tool) >= Node.ToolTier)
                {
                    return true;
                }
            }

            return false;
        }

        public static GatherNode ReadyNode(HouseholdState Home, LifeSheet Sheet, double Now)
        {
            if (Home == null)
            {
                return null;
            }

            GatherNode Best = null;
            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                GatherNode Node = Home.Nodes[i];
                if (!CanMine(Sheet, Node, Home, Now))
                {
                    continue;
                }

                if (Node.Skill != LifeSkill.Mining)
                {
                    continue;
                }

                if (Best == null || Node.MinRank < Best.MinRank)
                {
                    Best = Node;
                }
            }

            return Best;
        }

        public static GatherNode Prop(HouseholdState Home, LifeSkill Skill)
        {
            if (Home == null)
            {
                return null;
            }

            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                GatherNode Node = Home.Nodes[i];
                if (Node != null && Node.Skill == Skill && Biomes.Open(Home, Node.Biome))
                {
                    return Node;
                }
            }

            return null;
        }

        public static bool Produces(HouseholdState Home, string Item)
        {
            if (Home == null || string.IsNullOrEmpty(Item))
            {
                return false;
            }

            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                GatherNode Node = Home.Nodes[i];
                if (Node != null && Node.Resource == Item && Biomes.Open(Home, Node.Biome))
                {
                    return true;
                }
            }

            return false;
        }

        public static string Take(LifeSheet Sheet, GatherNode Node, HouseholdState Home, double Now)
        {
            if (!CanMine(Sheet, Node, Home, Now))
            {
                return null;
            }

            int Amount = 1 + Sheet.Rank(Node.Skill) / 3;
            Node.ReadyAt = Now + (Node.Respawn <= 0 ? 1 : Node.Respawn);
            if (Ores.Metal(Node.Resource))
            {
                Stock.Receive(Home, Node.Resource, Amount);
            }
            else
            {
                Work.Add(Home.Pantry, Node.Resource, Amount);
            }

            Sheet.AddRank(Node.Skill);
            return Node.Resource;
        }
    }

    public static class Requests
    {
        public static HouseholdRequest Open(HouseholdState Home, string Item)
        {
            if (Home == null)
            {
                return null;
            }

            for (int i = 0; i < Home.Requests.Count; i++)
            {
                HouseholdRequest Note = Home.Requests[i];
                if (Note != null && Note.Open && Note.Item == Item)
                {
                    return Note;
                }
            }

            return null;
        }

        public static void Post(HouseholdState Home, string Item, string Who, string Why)
        {
            if (Home == null || string.IsNullOrEmpty(Item) || Open(Home, Item) != null)
            {
                return;
            }

            HouseholdRequest Note = new HouseholdRequest();
            Note.Item = Item;
            Note.Who = Who;
            Note.Why = Why;
            Home.Requests.Add(Note);
        }

        public static void Finish(HouseholdState Home, string Item)
        {
            HouseholdRequest Note = Open(Home, Item);
            if (Note == null)
            {
                return;
            }

            Note.Open = false;
            if (Item == Biomes.Road && Home != null)
            {
                Home.RoadOpen = true;
            }
        }

        public static void Consider(HouseholdState Home, LifeSheet Sheet, string Who)
        {
            if (Home == null || Sheet == null)
            {
                return;
            }

            OreField.Ensure(Home);
            string Name = string.IsNullOrEmpty(Who) ? "Smith" : Who;
            if (Sheet.Rank(LifeSkill.Blacksmithing) >= 1 && !Gather.Produces(Home, "Iron Ingot") && Stock.Count(Home.Pantry, "Iron Ingot") <= 0)
            {
                Post(Home, "Iron Ingot", Name, "The forge needs an ingot from the dungeon.");
            }

            if (Sheet.Rank(LifeSkill.Blacksmithing) >= Cookbook.IronBar.Level && !Gather.Produces(Home, "Iron"))
            {
                Post(Home, Biomes.Road, Name, "Iron is past the highland road.");
            }
        }

        public static void Deliver(HouseholdState Home)
        {
            if (Home == null)
            {
                return;
            }

            for (int i = 0; i < Home.Requests.Count; i++)
            {
                HouseholdRequest Note = Home.Requests[i];
                if (Note == null || !Note.Open)
                {
                    continue;
                }

                if (Note.Item == Biomes.Road && Home.RoadOpen)
                {
                    Note.Open = false;
                    continue;
                }

                if (Stock.Count(Home.Pantry, Note.Item) > 0)
                {
                    Note.Open = false;
                }
            }
        }
    }

    public static class SkillBook
    {
        public static SkillRecord Find(HouseholdState Home, string Id)
        {
            if (Home == null || string.IsNullOrEmpty(Id))
            {
                return null;
            }

            for (int i = 0; i < Home.Skills.Count; i++)
            {
                if (Home.Skills[i] != null && Home.Skills[i].Id == Id)
                {
                    return Home.Skills[i];
                }
            }

            return null;
        }

        public static void Store(HouseholdState Home, string Id, LifeSheet Sheet, LifeSkill Trade)
        {
            if (Home == null || Sheet == null || string.IsNullOrEmpty(Id))
            {
                return;
            }

            SkillRecord Record = Find(Home, Id);
            if (Record == null)
            {
                Record = new SkillRecord();
                Record.Id = Id;
                Home.Skills.Add(Record);
            }

            Record.Mining = Sheet.Mining;
            Record.Cooking = Sheet.Cooking;
            Record.Gardening = Sheet.Gardening;
            Record.Fishing = Sheet.Fishing;
            Record.Hunting = Sheet.Hunting;
            Record.Foraging = Sheet.Foraging;
            Record.Blacksmithing = Sheet.Blacksmithing;
            Record.Alchemy = Sheet.Alchemy;
            Record.Tool = Sheet.Tool;
            if (Trade != LifeSkill.None)
            {
                Record.Trade = Trade;
            }
        }

        public static void ReadOnto(HouseholdState Home, PlayerCharacter Who)
        {
            if (Who == null)
            {
                return;
            }

            string Id = !string.IsNullOrEmpty(Who.PartyId) ? Who.PartyId : Who.Name;
            SkillRecord Record = Find(Home, Id);
            if (Record == null)
            {
                return;
            }

            CopyUp(Who.Life, Record);
            if (string.IsNullOrEmpty(Who.Life.Tool) && !string.IsNullOrEmpty(Record.Tool))
            {
                Who.Life.Tool = Record.Tool;
            }
        }

        static void CopyUp(LifeSheet Sheet, SkillRecord Record)
        {
            Raise(Sheet, LifeSkill.Mining, Record.Mining);
            Raise(Sheet, LifeSkill.Cooking, Record.Cooking);
            Raise(Sheet, LifeSkill.Gardening, Record.Gardening);
            Raise(Sheet, LifeSkill.Fishing, Record.Fishing);
            Raise(Sheet, LifeSkill.Hunting, Record.Hunting);
            Raise(Sheet, LifeSkill.Foraging, Record.Foraging);
            Raise(Sheet, LifeSkill.Blacksmithing, Record.Blacksmithing);
            Raise(Sheet, LifeSkill.Alchemy, Record.Alchemy);
        }

        static void Raise(LifeSheet Sheet, LifeSkill Skill, int Rank)
        {
            if (Sheet.Rank(Skill) < Rank)
            {
                Sheet.Set(Skill, Rank);
            }
        }
    }

    public static class WorkBook
    {
        public static void Write(ByteWriter Writer, HouseholdState Home)
        {
            if (Writer == null)
            {
                return;
            }

            int Reserved = Home == null || Home.Reserved == null ? 0 : Home.Reserved.Count;
            Writer.AppendInt32(Reserved);
            if (Home != null)
            {
                foreach (KeyValuePair<string, int> Item in Home.Reserved)
                {
                    Writer.AppendString(Item.Key ?? "");
                    Writer.AppendInt32(Item.Value);
                }
            }

            int Skills = Home == null ? 0 : Home.Skills.Count;
            Writer.AppendInt32(Skills);
            if (Home != null)
            {
                for (int i = 0; i < Home.Skills.Count; i++)
                {
                    SkillRecord Record = Home.Skills[i];
                    Writer.AppendString(Record.Id ?? "");
                    Writer.AppendInt32(Record.Mining);
                    Writer.AppendInt32(Record.Cooking);
                    Writer.AppendInt32(Record.Gardening);
                    Writer.AppendInt32(Record.Fishing);
                    Writer.AppendInt32(Record.Hunting);
                    Writer.AppendInt32(Record.Foraging);
                    Writer.AppendInt32(Record.Blacksmithing);
                    Writer.AppendInt32(Record.Alchemy);
                    Writer.AppendString(Record.Tool ?? "");
                    Writer.AppendInt32((int)Record.Trade);
                }
            }

            int Nodes = Home == null ? 0 : Home.Nodes.Count;
            Writer.AppendInt32(Nodes);
            if (Home != null)
            {
                for (int i = 0; i < Home.Nodes.Count; i++)
                {
                    GatherNode Node = Home.Nodes[i];
                    Writer.AppendString(Node.Id ?? "");
                    Writer.AppendString(Node.Resource ?? "");
                    Writer.AppendInt32((int)Node.Skill);
                    Writer.AppendInt32(Node.MinRank);
                    Writer.AppendInt32(Node.ToolTier);
                    Writer.AppendDouble(Node.ReadyAt);
                    Writer.AppendDouble(Node.Respawn);
                    Writer.AppendDouble(Node.Place.X);
                    Writer.AppendDouble(Node.Place.Y);
                    Writer.AppendDouble(Node.Place.Z);
                    Writer.AppendString(Node.Biome ?? "");
                }
            }

            int Notes = Home == null ? 0 : Home.Requests.Count;
            Writer.AppendInt32(Notes);
            if (Home != null)
            {
                for (int i = 0; i < Home.Requests.Count; i++)
                {
                    HouseholdRequest Note = Home.Requests[i];
                    Writer.AppendString(Note.Item ?? "");
                    Writer.AppendString(Note.Who ?? "");
                    Writer.AppendString(Note.Why ?? "");
                    Writer.AppendBoolean(Note.Open);
                }
            }

            Writer.AppendBoolean(Home != null && Home.RoadOpen);
            int Pouch = Home == null || Home.Pouch == null ? 0 : Home.Pouch.Count;
            Writer.AppendInt32(Pouch);
            if (Home != null)
            {
                foreach (KeyValuePair<string, int> Item in Home.Pouch)
                {
                    Writer.AppendString(Item.Key ?? "");
                    Writer.AppendInt32(Item.Value);
                }
            }
        }

        public static void Read(ByteReader Reader, HouseholdState Home)
        {
            if (Reader == null || Home == null)
            {
                return;
            }

            int Reserved = Reader.ReadInt32();
            for (int i = 0; i < Reserved; i++)
            {
                string Key = Reader.ReadString();
                Home.Reserved[Key] = Reader.ReadInt32();
            }

            int Skills = Reader.ReadInt32();
            for (int i = 0; i < Skills; i++)
            {
                SkillRecord Record = new SkillRecord();
                Record.Id = Reader.ReadString();
                Record.Mining = Reader.ReadInt32();
                Record.Cooking = Reader.ReadInt32();
                Record.Gardening = Reader.ReadInt32();
                Record.Fishing = Reader.ReadInt32();
                Record.Hunting = Reader.ReadInt32();
                Record.Foraging = Reader.ReadInt32();
                Record.Blacksmithing = Reader.ReadInt32();
                Record.Alchemy = Reader.ReadInt32();
                Record.Tool = Reader.ReadString();
                Record.Trade = (LifeSkill)Reader.ReadInt32();
                Home.Skills.Add(Record);
            }

            int Nodes = Reader.ReadInt32();
            for (int i = 0; i < Nodes; i++)
            {
                GatherNode Node = new GatherNode();
                Node.Id = Reader.ReadString();
                Node.Resource = Reader.ReadString();
                Node.Skill = (LifeSkill)Reader.ReadInt32();
                Node.MinRank = Reader.ReadInt32();
                Node.ToolTier = Reader.ReadInt32();
                Node.ReadyAt = Reader.ReadDouble();
                Node.Respawn = Reader.ReadDouble();
                float X = (float)Reader.ReadDouble();
                float Y = (float)Reader.ReadDouble();
                float Z = (float)Reader.ReadDouble();
                Node.Place = new Vector3(X, Y, Z);
                Node.Biome = Reader.ReadString();
                Home.Nodes.Add(Node);
            }

            int Notes = Reader.ReadInt32();
            for (int i = 0; i < Notes; i++)
            {
                HouseholdRequest Note = new HouseholdRequest();
                Note.Item = Reader.ReadString();
                Note.Who = Reader.ReadString();
                Note.Why = Reader.ReadString();
                Note.Open = Reader.ReadBoolean();
                Home.Requests.Add(Note);
            }

            Home.RoadOpen = Reader.ReadBoolean();
            int Pouch = Reader.ReadInt32();
            for (int i = 0; i < Pouch; i++)
            {
                string Key = Reader.ReadString();
                Home.Pouch[Key] = Reader.ReadInt32();
            }
        }
    }

    public static class OreField
    {
        public static void Ensure(HouseholdState Home)
        {
            if (Home == null)
            {
                return;
            }

            if (!Has(Home, "Tin"))
            {
                Add(Home, "Tin-0", "Tin", LifeSkill.Mining, 1, 1, Biomes.Grass, new Vector3(40f, 24f, 0f));
                Add(Home, "Tin-1", "Tin", LifeSkill.Mining, 1, 1, Biomes.Grass, new Vector3(72f, 24f, 0f));
                Add(Home, "Tin-2", "Tin", LifeSkill.Mining, 1, 1, Biomes.Grass, new Vector3(40f, 64f, 0f));
            }

            if (!Has(Home, "Iron"))
            {
                Add(Home, "Iron-0", "Iron", LifeSkill.Mining, 4, 2, Biomes.HighlandName, new Vector3(40f, 24f, 0f));
                Add(Home, "Iron-1", "Iron", LifeSkill.Mining, 4, 2, Biomes.HighlandName, new Vector3(72f, 40f, 0f));
            }
        }

        public static void Tend(NavMapGameManager Manager)
        {
            if (Manager == null || Manager.Home == null || WorldStream.Slot == null)
            {
                return;
            }

            Ensure(Manager.Home);
            MapInfo Home = WorldStream.ChunkAt(0, 0, false);
            MapInfo Highland = WorldStream.ChunkAt(4, 0, false);
            Biomes.Gate(Home, Highland, Manager.Home);
            Place(Home, Manager.Home);
            Place(Highland, Manager.Home);
        }

        public static void Place(MapInfo Chunk, HouseholdState Home)
        {
            if (Chunk == null || Home == null)
            {
                return;
            }

            if (Chunk.Furniture == null)
            {
                Chunk.Furniture = new List<FurnitureItem>();
            }

            string Biome = Biomes.Of(Chunk);
            int Have = 0;
            for (int i = 0; i < Chunk.Furniture.Count; i++)
            {
                FurnitureItem Box = Chunk.Furniture[i];
                if (Box != null && Box.Interaction == "Ore Rock")
                {
                    Have++;
                }
            }

            int Need = 0;
            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                GatherNode Node = Home.Nodes[i];
                if (Node != null && Node.Biome == Biome)
                {
                    Need++;
                    if (Node.Chunk == null)
                    {
                        Node.Chunk = Chunk;
                    }
                }
            }

            if (Have >= Need)
            {
                return;
            }

            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                GatherNode Node = Home.Nodes[i];
                if (Node == null || Node.Biome != Biome)
                {
                    continue;
                }

                Node.Chunk = Chunk;
                if (Have >= Need)
                {
                    break;
                }

                FurnitureItem Rock = new FurnitureItem();
                Rock.Body.Position = Node.Place;
                Rock.Body.Width = 12f;
                Rock.Body.Height = 1f;
                Rock.Body.Length = 12f;
                Rock.Interaction = "Ore Rock";
                Rock.Swatch = Node.Resource == "Iron" ? new Vector3(0.45f, 0.38f, 0.4f) : new Vector3(0.55f, 0.4f, 0.25f);
                Chunk.Furniture.Add(Rock);
                Have++;
            }
        }

        static bool Has(HouseholdState Home, string Resource)
        {
            for (int i = 0; i < Home.Nodes.Count; i++)
            {
                if (Home.Nodes[i] != null && Home.Nodes[i].Resource == Resource)
                {
                    return true;
                }
            }

            return false;
        }

        static void Add(HouseholdState Home, string Id, string Resource, LifeSkill Skill, int Rank, int Tier, string Biome, Vector3 Place)
        {
            GatherNode Node = new GatherNode();
            Node.Id = Id;
            Node.Resource = Resource;
            Node.Skill = Skill;
            Node.MinRank = Rank;
            Node.ToolTier = Tier;
            Node.Biome = Biome;
            Node.Place = Place;
            Home.Nodes.Add(Node);
        }
    }

    public static class DungeonPay
    {
        public static void Claim(HouseholdState Home, DungeonSpawn Foe)
        {
            if (Home == null || Foe == null || Foe.Paid || Foe.Kind != DungeonPersonKind.Monster)
            {
                return;
            }

            bool Dead = Foe.HP <= 0 || (Foe.Body != null && Foe.Body.CurrentHP <= 0);
            if (!Dead)
            {
                return;
            }

            Foe.Paid = true;
            int Purse = Foe.Purse > 0 ? Foe.Purse : 8;
            Home.Funds += Purse;
            if (!string.IsNullOrEmpty(Foe.Drop))
            {
                Work.Add(Home.Pouch, Foe.Drop, 1);
            }
        }

        public static void Sweep(DungeonRun Run, HouseholdState Home)
        {
            if (Run == null || Home == null)
            {
                return;
            }

            foreach (KeyValuePair<int, DungeonFloor> Pair in Run.Floors)
            {
                DungeonFloor Floor = Pair.Value;
                if (Floor == null)
                {
                    continue;
                }

                for (int i = 0; i < Floor.Spawns.Count; i++)
                {
                    Claim(Home, Floor.Spawns[i]);
                }
            }
        }

        public static void CashOut(HouseholdState Home)
        {
            if (Home == null)
            {
                return;
            }

            foreach (KeyValuePair<string, int> Item in new List<KeyValuePair<string, int>>(Home.Pouch))
            {
                Work.Add(Home.Pantry, Item.Key, Item.Value);
            }

            Home.Pouch.Clear();
            Requests.Deliver(Home);
        }
    }

    public static class WorkJob
    {
        public static bool Assign(PlayerCharacter Who, HouseholdState Home, LifeSkill Skill, double Now)
        {
            if (Who == null || Home == null || Who.ListAIObjective.Count > 0)
            {
                return false;
            }

            List<AutomatedAction> Steps = StepsFor(Who, Home, Skill, Now);
            if (Steps.Count == 0)
            {
                return false;
            }

            AIObjective Job = new AIObjective();
            Job.GoalName = Skill.ToString();
            Job.IsPlayerOrder = true;
            Job.ListRemainingStep = Steps;
            Who.CommitBottom(Job);
            if (Job.ListRemainingStep.Count > 0)
            {
                Job.ListRemainingStep[0].OnStarted();
            }

            return Who.ListAIObjective.Count > 0;
        }

        public static List<AutomatedAction> StepsFor(PlayerCharacter Who, HouseholdState Home, LifeSkill Skill, double Now)
        {
            List<AutomatedAction> Steps = new List<AutomatedAction>();
            if (Who == null || Home == null)
            {
                return Steps;
            }

            if (Skill == LifeSkill.Mining)
            {
                GatherNode Node = Gather.ReadyNode(Home, Who.Life, Now);
                if (Node == null)
                {
                    if (Gather.Offers(Home, Who.Life))
                    {
                        Steps.Add(new WaitAction(Who, 2));
                    }

                    return Steps;
                }

                if (!Travel(Who, Node, Steps))
                {
                    return new List<AutomatedAction>();
                }

                Steps.Add(new MineStep(Who, Node, Home));
                return Steps;
            }

            GatherNode Field = Gather.Prop(Home, Skill);
            if (Field != null && (Skill == LifeSkill.Hunting || Skill == LifeSkill.Foraging))
            {
                if (!Travel(Who, Field, Steps))
                {
                    return new List<AutomatedAction>();
                }

                Steps.Add(new MineStep(Who, Field, Home));
                return Steps;
            }

            string Prop = Skill == LifeSkill.Cooking ? "Counter" : Trades.Station(Skill);
            FurnitureItem Box = Find(Who, Prop);
            if (Box == null)
            {
                return Steps;
            }

            MapInfo Here = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
            Steps.AddRange(LifePlan.GoTo(Who, Here, Box.Beside()));
            Steps.Add(new StationStep(Who, Home, Skill));
            return Steps;
        }

        static bool Travel(PlayerCharacter Who, GatherNode Node, List<AutomatedAction> Steps)
        {
            if (Node.Chunk == null)
            {
                return true;
            }

            MapInfo Here = Who.SharedMapContex == null ? null : Who.SharedMapContex.CurrentMapInfo;
            bool Elsewhere = Here != null && Node.Chunk != Here;
            List<AutomatedAction> Walk = LifePlan.GoTo(Who, Node.Chunk, Node.Place);
            if (Elsewhere && Walk.Count == 0)
            {
                return false;
            }

            Steps.AddRange(Walk);
            return true;
        }

        static FurnitureItem Find(PlayerCharacter Who, string Interaction)
        {
            if (Who == null || Who.SharedMapContex == null || string.IsNullOrEmpty(Interaction))
            {
                return null;
            }

            MapInfo Here = Who.SharedMapContex.CurrentMapInfo;
            if (Here == null || Here.Furniture == null)
            {
                return null;
            }

            for (int i = 0; i < Here.Furniture.Count; i++)
            {
                FurnitureItem Box = Here.Furniture[i];
                if (Box != null && Box.Interaction == Interaction)
                {
                    return Box;
                }
            }

            return null;
        }
    }

    public class MineStep : LifeSimStep
    {
        readonly GatherNode Node;
        readonly HouseholdState Home;

        public MineStep(PlayerCharacter Owner, GatherNode Node, HouseholdState Home)
            : base("Mine", Owner, 0.5)
        {
            this.Node = Node;
            this.Home = Home;
        }

        protected override void OnFinished()
        {
            Gather.Take(Owner.Life, Node, Home, Owner.SimHour);
            string Id = !string.IsNullOrEmpty(Owner.PartyId) ? Owner.PartyId : Owner.Name;
            LifeSkill Trade = LifeSkill.None;
            PartyMember Member = Trades.Match(Home, Owner);
            if (Member != null)
            {
                Trade = Member.Trade;
            }

            SkillBook.Store(Home, Id, Owner.Life, Trade);
            Requests.Consider(Home, Owner.Life, Owner.Name);
        }

        protected override AutomatedAction Copy()
        {
            return new MineStep(Owner, Node, Home);
        }
    }

    public class StationStep : LifeSimStep
    {
        readonly HouseholdState Home;
        readonly LifeSkill Skill;

        public StationStep(PlayerCharacter Owner, HouseholdState Home, LifeSkill Skill)
            : base(Skill.ToString(), Owner, 0.5)
        {
            this.Home = Home;
            this.Skill = Skill;
        }

        protected override void OnFinished()
        {
            if (Home == null || Owner == null)
            {
                return;
            }

            Dictionary<string, int> Pile = TownLife.Pile(Owner, Home.Pantry);
            if (Skill == LifeSkill.Gardening || Skill == LifeSkill.Fishing)
            {
                Work.Tend(Owner.Life, Skill, Pile);
            }
            else if (Skill == LifeSkill.Cooking)
            {
                PreparedMeal Meal = Kitchen.Cook(Owner.Life.Cooking, Cookbook.Stew, Pile);
                if (Meal == null)
                {
                    Meal = Kitchen.FromNothing();
                }

                Kitchen.PlaceOnTable(Meal, Owner.SimHour);
                Home.Meals.Add(Meal);
                Owner.Life.AddRank(LifeSkill.Cooking);
            }
            else if (Skill == LifeSkill.Blacksmithing)
            {
                Recipe Next = Cookbook.BestSmith(Owner.Life, Pile);
                Work.Craft(Owner.Life, Next ?? Cookbook.Blade, Pile, Home.Reserved);
            }
            else if (Skill == LifeSkill.Alchemy)
            {
                Work.Craft(Owner.Life, Cookbook.Tonic, Pile, Home.Reserved);
            }

            string Id = !string.IsNullOrEmpty(Owner.PartyId) ? Owner.PartyId : Owner.Name;
            PartyMember Member = Trades.Match(Home, Owner);
            SkillBook.Store(Home, Id, Owner.Life, Member == null ? LifeSkill.None : Member.Trade);
            Requests.Consider(Home, Owner.Life, Owner.Name);
        }

        protected override AutomatedAction Copy()
        {
            return new StationStep(Owner, Home, Skill);
        }
    }
}
