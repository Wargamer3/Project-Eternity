using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimTownTests
    {
        [TestMethod]
        public void ATownListsEveryPersonFiledThere()
        {
            TownBook Book = TwoTowns();
            for (int i = 0; i < 25; i++)
            {
                Book.Admit("Girl " + i.ToString(), 0);
            }

            List<TownPerson> Here = Book.Group(0);
            List<string> Lines = Book.Lines();

            Assert.AreEqual(25, Here.Count);
            Assert.AreEqual(25, Lines.Count);
            Assert.AreEqual("Millford: Girl 0", Lines[0]);
            Assert.AreEqual("Millford: Girl 24", Lines[24]);
        }

        [TestMethod]
        public void MailArrivesOnItsHourAndLeavesPeopleWhereTheyAre()
        {
            TownBook Book = TwoTowns();
            Book.Admit("Ada", 0);
            Book.Admit("Bea", 0);
            Book.Admit("Cora", 1);
            Dictionary<string, int> Home = new Dictionary<string, int>();
            Home["Herb"] = 4;

            Assert.IsTrue(Book.SendParcel(0, 1, "Herb", 3, 10, Home));
            Assert.AreEqual(1, Home["Herb"]);
            double Arrive = 10 + Book.TripHours(0, 1);
            Assert.IsTrue(Arrive >= 11);

            Assert.IsFalse(Book.Deliver(Arrive - 0.01, Home));
            Assert.AreEqual(0, Book.CountOf(1, "Herb", Home));
            Assert.IsTrue(Book.Deliver(Arrive, Home));
            Assert.AreEqual(3, Book.CountOf(1, "Herb", Home));
            Assert.AreEqual(0, Book.Mail.Count);
            Assert.AreEqual(2, Book.Group(0).Count);
            Assert.AreEqual("Ada", Book.Group(0)[0].Name);
            Assert.AreEqual("Bea", Book.Group(0)[1].Name);
            Assert.AreEqual(1, Book.Group(1).Count);
            Assert.AreEqual("Cora", Book.Group(1)[0].Name);
        }

        [TestMethod]
        public void ACarrierJoinsTheDestinationAndTheOthersStay()
        {
            TownBook Book = TwoTowns();
            TownPerson Ada = Book.Admit("Ada", 0);
            Book.Admit("Bea", 0);
            Dictionary<string, int> Home = new Dictionary<string, int>();
            Home["Meat"] = 5;

            Assert.IsTrue(Book.Carry(Ada.Id, 1, "Meat", 2, Home));
            Assert.AreEqual(3, Home["Meat"]);
            Assert.AreEqual(1, Book.Group(0).Count);
            Assert.AreEqual("Bea", Book.Group(0)[0].Name);
            Assert.AreEqual(1, Book.OnTheRoad().Count);
            Assert.AreEqual("Traveling: Ada", Book.Lines()[Book.Lines().Count - 1]);

            Assert.IsTrue(Book.Step(Book.TripHours(0, 1), Home));
            Assert.AreEqual(1, Ada.Town);
            Assert.AreEqual(0, Ada.Bag.Count);
            Assert.IsFalse(Ada.Traveling);
            Assert.AreEqual(2, Book.CountOf(1, "Meat", Home));
            Assert.AreEqual(1, Book.Group(0).Count);
            Assert.AreEqual("Bea", Book.Group(0)[0].Name);
            Assert.AreEqual(1, Book.Group(1).Count);
            Assert.AreEqual("Ada", Book.Group(1)[0].Name);
        }

        [TestMethod]
        public void TheBookRoundTripsTownsPeopleAndParcels()
        {
            TownBook Book = TwoTowns();
            Book.WorldIndex = 4;
            Book.Admit("Ada", 0);
            Book.Admit("Bea", 0);
            Dictionary<string, int> Stock = Book.Pile(1, null);
            Stock["Fish"] = 2;
            Assert.IsTrue(Book.SendParcel(1, 0, "Fish", 2, 3, null));

            MemoryStream Stream = new MemoryStream();
            Book.Save(Stream);
            Stream.Position = 0;
            TownBook Loaded = TownBook.Load(Stream);

            Assert.AreEqual(4, Loaded.WorldIndex);
            Assert.AreEqual(2, Loaded.Towns.Count);
            Assert.AreEqual("Ashwell", Loaded.Towns[1].Name);
            Assert.AreEqual(1, Loaded.Towns[0].Roads.Count);
            Assert.AreEqual(1, Loaded.Towns[0].Roads[0]);
            Assert.AreEqual(2, Loaded.Group(0).Count);
            Assert.AreEqual(1, Loaded.Mail.Count);
            Assert.AreEqual("Fish", Loaded.Mail[0].Item);
            Assert.AreEqual(2, Loaded.Mail[0].Count);
            Assert.AreEqual(0, Loaded.Mail[0].Destination);
        }

        [TestMethod]
        public void SomeoneStandingInATownHouseStillCountsAsThere()
        {
            TownBook Book = new TownBook();
            Book.AddTown("Millford", 0f, 0f, 8f, 0, 0);

            Assert.AreEqual(0, Book.Nearest(180f, 0f, TownLife.TownReach));
            Assert.AreEqual(-1, Book.Nearest(180f, 0f, 120f));
        }

        static TownBook TwoTowns()
        {
            TownBook Book = new TownBook();
            Book.WorldIndex = 1;
            Book.AddTown("Millford", 0f, 0f, 8f, 0, 0);
            Book.AddTown("Ashwell", 80f, 0f, 8f, 0, 0);
            Book.Link(0, 1);
            return Book;
        }
    }
}
