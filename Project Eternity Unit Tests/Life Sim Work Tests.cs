using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimWorkTests
    {
        [TestMethod]
        public void AMinerPathsToTheRockAndAddsOre()
        {
            LifeSimMap Map = NewMap(1, 4);
            Fill(Map, UnitStats.TerrainLandIndex);
            MapInfo Room = new MapInfo(new MapContainer("Room", Map), null, null);
            Room.Kind = GroundKind.Grid;
            PlayerCharacter Miner = new PlayerCharacter(null, Room, Center(Map, 0, 0));
            Miner.Name = "Dwarf";
            Miner.Life.Mining = 1;
            Miner.Life.Tool = "Bronze Pick";
            HouseholdState Home = new HouseholdState();
            PartyMember Smith = new PartyMember();
            Smith.Id = "Smith";
            Smith.Resident = true;
            Smith.Trade = LifeSkill.Blacksmithing;
            Home.Party.Roster.Add(Smith);
            GatherNode Rock = new GatherNode();
            Rock.Id = "Tin-test";
            Rock.Resource = "Tin";
            Rock.Skill = LifeSkill.Mining;
            Rock.MinRank = 1;
            Rock.ToolTier = 1;
            Rock.Biome = Biomes.Grass;
            Rock.Chunk = Room;
            Rock.Place = Center(Map, 0, 3);
            Home.Nodes.Add(Rock);

            List<AutomatedAction> Steps = WorkJob.StepsFor(Miner, Home, LifeSkill.Mining, 0);
            Assert.AreEqual("Follow Path", Steps[0].ActionName);
            Assert.AreEqual("Mine", Steps[Steps.Count - 1].ActionName);
            Assert.IsFalse(Steps[Steps.Count - 1].Update(new GameTime(TimeSpan.Zero, TimeSpan.FromSeconds(1))));
            Assert.AreEqual(1, Home.Pantry["Tin"]);
            Assert.AreEqual(1, Home.Reserved["Tin"]);
            Assert.AreEqual(2000, Home.Funds);
            Assert.AreEqual(2, Miner.Life.Mining);
        }

        [TestMethod]
        public void OreSellsWhenNoSmithLivesThere()
        {
            HouseholdState Home = new HouseholdState();
            LifeSheet Sheet = new LifeSheet();
            Sheet.Mining = 1;
            Sheet.Tool = "Bronze Pick";
            GatherNode Rock = Tin(null);
            Home.Nodes.Add(Rock);
            Assert.AreEqual("Tin", Gather.Take(Sheet, Rock, Home, 0));
            Assert.IsFalse(Home.Pantry.ContainsKey("Tin"));
            Assert.AreEqual(2005, Home.Funds);
        }

        [TestMethod]
        public void RankOneCannotTakeTheNextOre()
        {
            HouseholdState Home = new HouseholdState();
            Home.RoadOpen = true;
            LifeSheet Sheet = new LifeSheet();
            Sheet.Mining = 1;
            Sheet.Tool = "Bronze Pick";
            GatherNode Iron = new GatherNode();
            Iron.Resource = "Iron";
            Iron.Skill = LifeSkill.Mining;
            Iron.MinRank = 4;
            Iron.ToolTier = 2;
            Iron.Biome = Biomes.HighlandName;
            Assert.IsFalse(Gather.CanMine(Sheet, Iron, Home, 0));
            Sheet.Mining = 4;
            Assert.IsFalse(Gather.CanMine(Sheet, Iron, Home, 0));
            Sheet.Tool = "Iron Pick";
            Assert.IsTrue(Gather.CanMine(Sheet, Iron, Home, 0));
            Home.RoadOpen = false;
            Assert.IsFalse(Gather.CanMine(Sheet, Iron, Home, 0));
        }

        [TestMethod]
        public void AForgeCraftSpendsTheReservedStack()
        {
            LifeSheet Sheet = new LifeSheet();
            Sheet.Blacksmithing = 1;
            Dictionary<string, int> Pantry = new Dictionary<string, int>();
            Dictionary<string, int> Reserved = new Dictionary<string, int>();
            Pantry["Tin"] = 1;
            Reserved["Tin"] = 1;
            Assert.AreEqual("Blade", Work.Craft(Sheet, Cookbook.Blade, Pantry, Reserved));
            Assert.AreEqual(0, Pantry["Tin"]);
            Assert.AreEqual(0, Reserved["Tin"]);
            Assert.AreEqual(1, Pantry["Blade"]);
            Assert.AreEqual(2, Sheet.Blacksmithing);
        }

        [TestMethod]
        public void ASaveRoundTripKeepsTheRankThePickAndTheRock()
        {
            HouseholdState Home = new HouseholdState();
            LifeSheet Sheet = new LifeSheet();
            Sheet.Mining = 3;
            Sheet.Tool = "Bronze Pick";
            SkillBook.Store(Home, "Dwarf", Sheet, LifeSkill.Mining);
            GatherNode Rock = Tin(null);
            Rock.ReadyAt = 5;
            Home.Nodes.Add(Rock);
            Home.Reserved["Tin"] = 2;
            ByteWriter Writer = new ByteWriter();
            LifeSync.Write(Writer, Home, null);
            HouseholdState Loaded;
            DungeonRun Run;
            LifeSync.Read(new ByteReader(Writer.GetBytes()), out Loaded, out Run);
            Assert.AreEqual(3, Loaded.Skills[0].Mining);
            Assert.AreEqual("Bronze Pick", Loaded.Skills[0].Tool);
            Assert.AreEqual(LifeSkill.Mining, Loaded.Skills[0].Trade);
            Assert.AreEqual(5, Loaded.Nodes[0].ReadyAt);
            Assert.AreEqual(2, Loaded.Reserved["Tin"]);

            PlayerCharacter Who = new PlayerCharacter(null, null, Vector3.Zero);
            Who.Name = "Dwarf";
            SkillBook.ReadOnto(Loaded, Who);
            Assert.AreEqual(3, Who.Life.Mining);
            Assert.AreEqual("Bronze Pick", Who.Life.Tool);
        }

        [TestMethod]
        public void ATradeUnlocksItsStationAndGrantsTheStarterTool()
        {
            HouseholdState Home = new HouseholdState();
            Assert.IsFalse(Trades.Offered("Forge", Home));
            Assert.IsFalse(Named(BuildCatalog.In(CatalogPage.Hobby, Home), "Forge"));
            Assert.IsTrue(Named(BuildCatalog.In(CatalogPage.Comfort, Home), "Bed"));

            bool SentHome;
            Home.Party.Recruit("Dwarf", "Dwarf", LifeSkill.Mining, out SentHome);
            Home.Party.Recruit("Smith", "Smith", LifeSkill.Blacksmithing, out SentHome);
            Assert.AreEqual("Bronze Pick", Home.Party.Find("Dwarf").Tool);
            Assert.AreEqual(LifeSkill.Blacksmithing, Home.Party.Find("Smith").Trade);
            Assert.IsTrue(Trades.Offered("Forge", Home));
            Assert.IsFalse(Trades.Offered("Alchemy Bench", Home));
            Assert.IsTrue(Named(BuildCatalog.In(CatalogPage.Hobby, Home), "Forge"));

            PlayerCharacter Body = new PlayerCharacter(null, null, Vector3.Zero);
            Trades.Give(Body, LifeSkill.Mining);
            Assert.AreEqual(1, Body.Life.Mining);
            Assert.AreEqual("Bronze Pick", Body.Life.Tool);
            Assert.IsTrue(Body.IsCarrying("Bronze Pick"));
        }

        [TestMethod]
        public void AKillPaysAndResidentsStayOutUntilTaken()
        {
            HouseholdState Home = new HouseholdState();
            DungeonSpawn Foe = new DungeonSpawn();
            Foe.Kind = DungeonPersonKind.Monster;
            Foe.HP = 0;
            Foe.Purse = 11;
            Foe.Drop = "Iron Ingot";
            DungeonPay.Claim(Home, Foe);
            DungeonPay.Claim(Home, Foe);
            Assert.AreEqual(2011, Home.Funds);
            Assert.AreEqual(1, Home.Pouch["Iron Ingot"]);
            DungeonPay.CashOut(Home);
            Assert.AreEqual(1, Home.Pantry["Iron Ingot"]);
            Assert.AreEqual(0, Home.Pouch.Count);

            DungeonRun Run = new DungeonRun();
            Run.FloorCount = 1;
            Run.Width = 16;
            Run.Height = 16;
            Run.Party.Pick(0, "Hero", "Hero");
            Run.Party.Note(1);
            bool SentHome;
            Run.Party.Recruit("Dwarf", "Dwarf", LifeSkill.Mining, out SentHome);
            Assert.IsTrue(Run.Enter("Hero"));
            Assert.IsTrue(Run.FloorOf.ContainsKey("Hero"));
            Assert.IsFalse(Run.FloorOf.ContainsKey("Dwarf"));
            Assert.IsTrue(Run.Enter("Dwarf"));
            Assert.IsTrue(Run.FloorOf.ContainsKey("Dwarf"));
        }

        [TestMethod]
        public void ASmithAsksForTheIngotAndTheRoadTheMinerDoesNotAskForTin()
        {
            HouseholdState Home = new HouseholdState();
            LifeSheet Miner = new LifeSheet();
            Miner.Mining = 1;
            Requests.Consider(Home, Miner, "Dwarf");
            Assert.IsNull(Requests.Open(Home, "Tin"));
            Assert.IsNull(Requests.Open(Home, "Iron Ingot"));

            LifeSheet Smith = new LifeSheet();
            Smith.Blacksmithing = 1;
            Requests.Consider(Home, Smith, "Smith");
            Assert.IsNotNull(Requests.Open(Home, "Iron Ingot"));
            Assert.IsNull(Requests.Open(Home, Biomes.Road));
            Smith.Blacksmithing = 4;
            Requests.Consider(Home, Smith, "Smith");
            Assert.IsNotNull(Requests.Open(Home, Biomes.Road));

            Home.Pantry["Iron Ingot"] = 1;
            Requests.Deliver(Home);
            Assert.IsNull(Requests.Open(Home, "Iron Ingot"));
            Assert.IsNotNull(Requests.Open(Home, Biomes.Road));

            BuildingZone Zone = new BuildingZone();
            Zone.Name = Biomes.Road;
            Zone.Price = 100;
            int Funds = Home.Funds;
            Assert.IsTrue(PlotSale.TryBuy(Zone, HouseholdState.Id, ref Funds));
            Home.Funds = Funds;
            Biomes.AfterBuy(Home, Zone);
            Assert.IsTrue(Home.RoadOpen);
            Assert.IsNull(Requests.Open(Home, Biomes.Road));
        }

        [TestMethod]
        public void TheBestPickAndAFullLarderAndTheHighlandGate()
        {
            LifeSheet Sheet = new LifeSheet();
            Sheet.Mining = 1;
            Sheet.Tool = "Bronze Pick";
            Dictionary<string, int> Pantry = new Dictionary<string, int>();
            Pantry["Iron Pick"] = 1;
            Stock.Equip(Sheet, Pantry);
            Assert.AreEqual("Bronze Pick", Sheet.Tool);
            Assert.AreEqual(1, Pantry["Iron Pick"]);
            Sheet.Mining = 4;
            Stock.Equip(Sheet, Pantry);
            Assert.AreEqual("Iron Pick", Sheet.Tool);
            Assert.AreEqual(1, Pantry["Bronze Pick"]);
            Assert.AreEqual(0, Pantry["Iron Pick"]);

            Assert.IsTrue(Stock.Short(new Dictionary<string, int>()));
            Dictionary<string, int> Full = new Dictionary<string, int>();
            Full["Meat"] = 4;
            Full["Herb"] = 4;
            Assert.IsFalse(Stock.Short(Full));

            MapInfo From = new MapInfo(new MapContainer("Grass", null), null, null);
            MapInfo To = new MapInfo(new MapContainer("Highland", null), null, null);
            To.ChunkX = 4;
            HouseholdState Home = new HouseholdState();
            ChunkHop Shut = Biomes.Gate(From, To, Home);
            Assert.IsFalse(Shut.IsOpen);
            Assert.AreEqual(Biomes.HighlandName, Biomes.Of(To));
            Home.RoadOpen = true;
            ChunkHop Open = Biomes.Gate(From, To, Home);
            Assert.IsTrue(Open.IsOpen);
            Assert.AreSame(Shut, Open);
        }

        [TestMethod]
        public void TheHighlandWingHitsHarderAndCaptivesCarryATrade()
        {
            DungeonRun Low = new DungeonRun();
            Low.Seed = 3;
            Low.FloorCount = 1;
            Low.Width = 16;
            Low.Height = 16;
            DungeonFloor Calm = Low.Visit(0);
            DungeonRun High = new DungeonRun();
            High.Seed = 3;
            High.Wing = 1;
            High.FloorCount = 1;
            High.Width = 16;
            High.Height = 16;
            DungeonFloor Wild = High.Visit(0);
            Assert.IsTrue(MonsterHp(Wild) > MonsterHp(Calm));
            Assert.IsTrue(MonsterPurse(Wild) > MonsterPurse(Calm));
            Assert.AreNotEqual(LifeSkill.None, CaptiveTrade(Calm));
        }

        static GatherNode Tin(MapInfo Chunk)
        {
            GatherNode Rock = new GatherNode();
            Rock.Resource = "Tin";
            Rock.Skill = LifeSkill.Mining;
            Rock.MinRank = 1;
            Rock.ToolTier = 1;
            Rock.Biome = Biomes.Grass;
            Rock.Chunk = Chunk;
            return Rock;
        }

        static int MonsterHp(DungeonFloor Floor)
        {
            for (int i = 0; i < Floor.Spawns.Count; i++)
            {
                if (Floor.Spawns[i].Kind == DungeonPersonKind.Monster)
                {
                    return Floor.Spawns[i].HP;
                }
            }

            return 0;
        }

        static int MonsterPurse(DungeonFloor Floor)
        {
            for (int i = 0; i < Floor.Spawns.Count; i++)
            {
                if (Floor.Spawns[i].Kind == DungeonPersonKind.Monster)
                {
                    return Floor.Spawns[i].Purse;
                }
            }

            return 0;
        }

        static LifeSkill CaptiveTrade(DungeonFloor Floor)
        {
            for (int i = 0; i < Floor.Spawns.Count; i++)
            {
                if (Floor.Spawns[i].Kind == DungeonPersonKind.Captive)
                {
                    return Floor.Spawns[i].Trade;
                }
            }

            return LifeSkill.None;
        }

        static bool Named(List<CatalogItem> Cards, string Name)
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                if (Cards[i].Name == Name)
                {
                    return true;
                }
            }

            return false;
        }

        static LifeSimMap NewMap(int Width, int Height)
        {
            LifeSimMap Map = new LifeSimMap();
            Map.MapSize = new Point(Width, Height);
            Map.LayerManager.ListLayer.Add(new MapLayer(Map, 0));
            return Map;
        }

        static void Fill(LifeSimMap Map, byte TerrainIndex)
        {
            for (int Y = 0; Y < Map.MapSize.Y; Y++)
            {
                for (int X = 0; X < Map.MapSize.X; X++)
                {
                    Terrain Tile = Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y];
                    Tile.TerrainTypeIndex = TerrainIndex;
                    Tile.Height = 0;
                    Tile.WorldPosition.Z = 0;
                    Tile.MovementCost = 1;
                }
            }
        }

        static Vector3 Center(LifeSimMap Map, int X, int Y)
        {
            return GridNavGeometry.CellCenter(Map, Map.LayerManager.ListLayer[0].ArrayTerrain[X, Y]);
        }
    }
}
