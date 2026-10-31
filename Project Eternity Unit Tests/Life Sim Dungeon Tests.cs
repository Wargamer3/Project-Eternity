using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Online;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimDungeonTests
    {
        [TestMethod]
        public void AFloorHasAWalkablePathFromTheEntryToTheStairs()
        {
            DungeonFloor Floor = DungeonGenerator.Build(3, 0, 4, 24, 24, -1, -1);
            Assert.IsTrue(Floor.EntryLeaves);
            Assert.IsTrue(Floor.Reaches(Floor.EntryX, Floor.EntryY, Floor.DownX, Floor.DownY));
            Assert.IsTrue(DungeonTiles.Allows(Floor.At(Floor.EntryX, Floor.EntryY)));
            Assert.IsTrue(DungeonTiles.Allows(Floor.At(Floor.DownX, Floor.DownY)));
        }

        [TestMethod]
        public void StairwellsLineUpAndSocketsAgree()
        {
            DungeonRun Run = new DungeonRun();
            Run.Seed = 9;
            Run.FloorCount = 4;
            DungeonFloor First = Run.Visit(0);
            DungeonFloor Second = Run.Visit(1);
            Assert.AreEqual(First.DownX, Second.UpX);
            Assert.AreEqual(First.DownY, Second.UpY);
            Assert.IsTrue(Second.Reaches(Second.EntryX, Second.EntryY, Second.DownX, Second.DownY));

            DungeonFloor Again = DungeonGenerator.Build(Run.Seed, 0, Run.FloorCount, Run.Width, Run.Height, -1, -1);
            Assert.AreEqual(First.DownX, Again.DownX);
            Assert.AreEqual(First.DownY, Again.DownY);
            Agree(First);
        }

        [TestMethod]
        public void ARaisedRoomIsReachedByAStep()
        {
            bool Found = false;
            for (int Seed = 1; Seed < 12 && !Found; Seed++)
            {
                DungeonFloor Floor = DungeonGenerator.Build(Seed, 0, 4, 24, 24, -1, -1);
                for (int i = 0; i < Floor.Cells.Length; i++)
                {
                    DungeonCell Cell = Floor.Cells[i];
                    if (Cell.Role != DungeonRole.Step)
                    {
                        continue;
                    }

                    DungeonCell East = Floor.At(Cell.X + 1, Cell.Y);
                    DungeonCell West = Floor.At(Cell.X - 1, Cell.Y);
                    DungeonCell South = Floor.At(Cell.X, Cell.Y + 1);
                    DungeonCell North = Floor.At(Cell.X, Cell.Y - 1);
                    Found = Raised(Cell, East) || Raised(Cell, West) || Raised(Cell, South) || Raised(Cell, North);
                }
            }

            Assert.IsTrue(Found);
        }

        [TestMethod]
        public void OnlySeatedPlayersKeepAFloorAwake()
        {
            DungeonRun Run = PartyOfThree();
            Run.Enter("Lead");
            Run.Enter("Coop");
            Assert.IsTrue(Run.OrderMove("Lead", Run.Floors[0].DownX, Run.Floors[0].DownY));
            Assert.IsTrue(Run.Climb("Lead"));
            Assert.AreEqual(1, Run.FloorOf["Lead"]);
            Assert.AreEqual(1, Run.FloorOf["Pal"]);
            Assert.AreEqual(0, Run.FloorOf["Coop"]);
            HashSet<int> Awake = Run.AwakeFloors();
            Assert.IsTrue(Awake.Contains(0));
            Assert.IsTrue(Awake.Contains(1));
            Assert.AreEqual(2, Awake.Count);
        }

        [TestMethod]
        public void ANewCoopGamePicksCharactersAndALatePlayerTakesOne()
        {
            DungeonParty Party = new DungeonParty();
            Assert.IsNotNull(Party.Pick(0, "Ada#0", "Ada"));
            Assert.IsNotNull(Party.Pick(1, "Bea#1", "Bea"));
            Party.Note(2);
            Assert.IsTrue(Party.Started);
            Assert.IsNull(Party.Pick(2, "Cia#2", "Cia"));
            Assert.AreEqual(JoinKind.Wait, DungeonParty.Kind(true, false, 0));

            bool SentHome;
            Assert.IsTrue(Party.Recruit("Pal", "Pal", out SentHome));
            Assert.IsFalse(SentHome);
            Assert.AreEqual(JoinKind.TakeExisting, DungeonParty.Kind(true, false, Party.Uncontrolled()));
            Assert.IsTrue(Party.LateJoin(2, "Pal"));
            Assert.AreEqual(2, Party.Find("Pal").Seat);
            Assert.AreEqual(3, Party.PartyCount());

            Assert.IsTrue(Party.Recruit("Dot", "Dot", out SentHome));
            Assert.IsFalse(SentHome);
            Assert.IsFalse(Party.Recruit("Eve", "Eve", out SentHome));
            Assert.IsTrue(SentHome);
            Assert.IsFalse(Party.Find("Eve").InParty);
            Party.Leave(2);
            Assert.AreEqual(-1, Party.Find("Pal").Seat);
            Assert.IsTrue(Party.Find("Pal").InParty);
        }

        [TestMethod]
        public void SwingsAndSpellsWaitOnTheAnimation()
        {
            PlayerCharacter Hero = Spellcast.Unlisted("Hero");
            PlayerCharacter Foe = Spellcast.Unlisted("Foe");
            Hero.ActionsLeft = 0;
            Assert.IsTrue(DungeonCombat.BeginStrike(Hero, Foe, 0.45));
            Assert.IsTrue(Hero.SwingRemaining > 0);
            Assert.AreEqual(0, Hero.ActionsLeft);
            Assert.IsFalse(DungeonCombat.BeginStrike(Hero, Foe, 0.45));
            DungeonCombat.Tick(Hero, 1);
            Assert.AreEqual(0, Hero.SwingRemaining, 0.001);

            SpellCard Light = new SpellCard();
            Light.Name = "Light";
            Light.Cantrip = true;
            Light.Actions = 1;
            Light.Spell = new Trait();
            Light.Spell.Name = "Light";
            SpellCard Heavy = new SpellCard();
            Heavy.Name = "Heavy";
            Heavy.Cantrip = true;
            Heavy.Actions = 3;
            Heavy.Spell = new Trait();
            Heavy.Spell.Name = "Heavy";
            Assert.IsTrue(DungeonCombat.BeginSpell(Hero, Foe, Light));
            double Short = Hero.SwingRemaining;
            DungeonCombat.Tick(Hero, 5);
            Assert.IsTrue(DungeonCombat.BeginSpell(Hero, Foe, Heavy));
            Assert.IsTrue(Hero.SwingRemaining > Short);
            Assert.AreEqual(0, Hero.ActionsLeft);
        }

        [TestMethod]
        public void FoodExpiresOnTheTableAndRestLastsADay()
        {
            List<PreparedMeal> Meals = new List<PreparedMeal>();
            PreparedMeal Table = Kitchen.Cook(3, Cookbook.Stew, Pantry());
            Kitchen.PlaceOnTable(Table, 1);
            Meals.Add(Table);
            PreparedMeal Cold = Kitchen.FromNothing();
            Meals.Add(Cold);
            Kitchen.Tick(Meals, 12);
            Assert.AreEqual(2, Meals.Count);
            Kitchen.Tick(Meals, 13);
            Assert.AreEqual(1, Meals.Count);
            Assert.AreEqual("Scraps", Meals[0].Name);

            PlayerCharacter Cook = new PlayerCharacter(null, null, Vector3.Zero);
            Cook.Hunger = 50;
            Cook.SimHour = 2;
            Kitchen.Eat(Cook.Life, Table, 2, Cook);
            Kitchen.Eat(Cook.Life, Cold, 2, Cook);
            Assert.AreEqual(0, Cook.Hunger);
            Assert.IsTrue(Cook.Life.Food.On(2));
            Assert.IsFalse(Cook.Life.Food.On(26));
            Resting.Sleep(Cook.Life, 8, 2, Cook);
            Assert.IsTrue(Cook.Life.AttackNow(2) > Cook.Life.Food.Attack);
            Resting.Sleep(Cook.Life, 0, 2, Cook);
            Assert.IsTrue(Cook.Life.Rest.On(2));
        }

        [TestMethod]
        public void BuyingAPlotSpendsTheSharedPurseAndABedIsRequired()
        {
            BuildingZone Zone = new BuildingZone();
            Zone.Price = 500;
            int Funds = 800;
            Assert.IsTrue(PlotSale.TryBuy(Zone, HouseholdState.Id, ref Funds));
            Assert.AreEqual(300, Funds);
            Assert.AreEqual(HouseholdState.Id, Zone.OwnerId);
            Assert.IsFalse(PlotSale.TryBuy(Zone, "Other", ref Funds));
            Assert.IsTrue(PlotSale.MayBuild(Zone, HouseholdState.Id, false));
            Assert.IsFalse(PlotSale.MayBuild(Zone, "Other", false));

            BuildingZone Open = new BuildingZone();
            Assert.IsFalse(PlotSale.MayBuild(Open, HouseholdState.Id, false));
            Assert.IsTrue(PlotSale.MayBuild(Open, HouseholdState.Id, true));
            Assert.IsTrue(Social.CanInvite(RelationshipStatus.BestFriend, 2, 1));
            Assert.IsFalse(Social.CanInvite(RelationshipStatus.BestFriend, 1, 1));
            Assert.IsFalse(Social.CanInvite(RelationshipStatus.Friendly, 2, 0));
        }

        [TestMethod]
        public void HuntingComesHomeWithMeatAndTheBestHandWorks()
        {
            LifeSheet Sheet = new LifeSheet();
            Sheet.Hunting = 4;
            Sheet.Cooking = 1;
            Dictionary<string, int> Pantry = new Dictionary<string, int>();
            Assert.IsTrue(Work.StartAway(Sheet, LifeSkill.Hunting, 1));
            Assert.IsNull(Work.FinishAway(Sheet, 4, Pantry));
            Assert.AreEqual("Meat", Work.FinishAway(Sheet, 5, Pantry));
            Assert.IsTrue(Pantry["Meat"] > 1);
            Assert.IsFalse(Sheet.Away.Busy);

            HouseholdStations Stations = new HouseholdStations();
            Stations.LarderLow = true;
            Stations.Kitchen = true;
            TaskPick Pick = Work.Choose(Sheet, false, false, Stations);
            Assert.AreEqual(LifeSkill.Hunting, Pick.Skill);
            Pick = Work.Choose(Sheet, true, false, Stations);
            Assert.IsFalse(Pick.Act);
            Sheet.HasOrder = true;
            Sheet.Order = LifeSkill.Cooking;
            Pick = Work.Choose(Sheet, false, false, Stations);
            Assert.AreEqual(LifeSkill.Cooking, Pick.Skill);
        }

        [TestMethod]
        public void TalkingPromotesABondAndRescueFillsAnEmptySlot()
        {
            PlayerCharacter Hero = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Pal = new PlayerCharacter(null, null, Vector3.Zero);
            CharacterRelationship Bond = Hero.RelationshipWith(Pal);
            Social.Talk(Bond, 15f);
            Assert.AreEqual(RelationshipStatus.Neutral, Bond.Status);
            Social.Talk(Bond, 30f);
            Assert.AreEqual(RelationshipStatus.Friendly, Bond.Status);
            Assert.IsTrue(Social.CanVisit(Bond.Status));
            Social.Talk(Bond, 30f);
            Assert.AreEqual(RelationshipStatus.BestFriend, Bond.Status);

            DungeonParty Party = new DungeonParty();
            Party.Pick(0, "Hero", "Hero");
            Party.Note(1);
            DungeonSpawn Captive = new DungeonSpawn();
            Captive.Kind = DungeonPersonKind.Captive;
            Captive.Name = "Lost";
            Captive.Body = Pal;
            bool SentHome;
            Assert.IsTrue(Rescue.Free(Party, Captive, Hero, out SentHome));
            Assert.IsFalse(SentHome);
            Assert.IsTrue(Party.Find("Lost").InParty);
        }

        [TestMethod]
        public void ASnapshotRestoresThePartyThePurseAndTheFloor()
        {
            HouseholdState Home = new HouseholdState();
            Home.Funds = 640;
            Home.Party.Pick(0, "Ada#0", "Ada");
            Home.Party.Note(1);
            Home.Pantry["Herb"] = 3;
            DungeonRun Run = Home.Run();
            Run.Enter("Ada#0");
            PreparedMeal Meal = Kitchen.FromNothing();
            Kitchen.PlaceOnTable(Meal, 4);
            Home.Meals.Add(Meal);
            ByteWriter Writer = new ByteWriter();
            LifeSync.Write(Writer, Home, Run);
            byte[] Data = Writer.GetBytes();
            HouseholdState LoadedHome;
            DungeonRun LoadedRun;
            LifeSync.Read(new ByteReader(Data), out LoadedHome, out LoadedRun);
            Assert.AreEqual(640, LoadedHome.Funds);
            Assert.AreEqual("Ada", LoadedHome.Party.Find("Ada#0").Name);
            Assert.AreEqual(0, LoadedHome.Party.Find("Ada#0").Seat);
            Assert.AreEqual(3, LoadedHome.Pantry["Herb"]);
            Assert.AreEqual(1, LoadedHome.Meals.Count);
            Assert.AreEqual(Run.Floors[0].DownX, LoadedRun.Floors[0].DownX);
            Assert.AreEqual(0, LoadedRun.FloorOf["Ada#0"]);
            LifeSync.Home = Home;
            LifeSync.Dungeon = Run;
            byte[] Captured = LifeSync.Capture();
            LifeSync.Restore(Captured);
            Assert.AreEqual(640, LifeSync.Home.Funds);
        }

        static DungeonRun PartyOfThree()
        {
            DungeonRun Run = new DungeonRun();
            Run.FloorCount = 3;
            Run.Party.Pick(0, "Lead", "Lead");
            Run.Party.Pick(1, "Coop", "Coop");
            Run.Party.Note(2);
            bool SentHome;
            Run.Party.Recruit("Pal", "Pal", out SentHome);
            return Run;
        }

        static Dictionary<string, int> Pantry()
        {
            Dictionary<string, int> Stock = new Dictionary<string, int>();
            Stock["Meat"] = 2;
            Stock["Herb"] = 2;
            Stock["Fish"] = 2;
            return Stock;
        }

        static bool Raised(DungeonCell Step, DungeonCell Other)
        {
            return Other != null && Other.Height == 1 && DungeonFloor.CanCross(Step, Other);
        }

        static void Agree(DungeonFloor Floor)
        {
            for (int i = 0; i < Floor.Cells.Length; i++)
            {
                DungeonCell Cell = Floor.Cells[i];
                if (Cell.Role == DungeonRole.Empty)
                {
                    continue;
                }

                Assert.IsFalse(string.IsNullOrEmpty(Cell.Model));
                Assert.IsTrue(DungeonTiles.Allows(Cell));
                DungeonCell East = Floor.At(Cell.X + 1, Cell.Y);
                if (East != null && East.Role != DungeonRole.Empty)
                {
                    Assert.AreEqual(Cell.East, East.West);
                }

                DungeonCell South = Floor.At(Cell.X, Cell.Y + 1);
                if (South != null && South.Role != DungeonRole.Empty)
                {
                    Assert.AreEqual(Cell.South, South.North);
                }
            }
        }
    }
}
