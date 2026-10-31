using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimInteractionTests
    {
        static List<string> Labels(List<InteractionChoice> Choices)
        {
            List<string> Found = new List<string>();
            for (int i = 0; i < Choices.Count; i++)
            {
                Found.Add(Choices[i].Label);
            }

            return Found;
        }

        static FurnitureItem Piece(string Name, float X, float Y)
        {
            FurnitureItem Box = new FurnitureItem();
            Box.Interaction = Name;
            Box.Width = 24f;
            Box.Length = 24f;
            Box.Height = 16f;
            Box.Position = new Vector3(X, Y, 0f);
            return Box;
        }

        [TestMethod]
        public void ReflectionRegistersSleepAndWatch()
        {
            Assert.IsTrue(LifeOrder.Interactions.ContainsKey("SleepInBedAction"));
            Assert.IsTrue(LifeOrder.Interactions.ContainsKey("WatchShowAction"));
            Assert.IsTrue(LifeOrder.Interactions.ContainsKey("Sit"));
        }

        [TestMethod]
        public void AClosedTelevisionOffersOpenAndAnOpenOneOffersWatchAndClose()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            FurnitureItem Screen = Piece("Television", 16f, 16f);
            InteractionContext Context = new InteractionContext();
            Context.Actor = Ada;
            Context.Box = Screen;

            List<string> Closed = Labels(LifeOrder.Choices(Context));
            Assert.IsTrue(Closed.Contains("Open TV"));
            Assert.IsFalse(Closed.Contains("Watch"));
            Assert.IsFalse(Closed.Contains("Close TV"));

            Screen.Open = true;
            List<string> Open = Labels(LifeOrder.Choices(Context));
            Assert.IsFalse(Open.Contains("Open TV"));
            Assert.IsTrue(Open.Contains("Watch"));
            Assert.IsTrue(Open.Contains("Close TV"));
        }

        [TestMethod]
        public void ChairsAndCouchesOfferSitAndBedsOfferSleep()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            InteractionContext Context = new InteractionContext();
            Context.Actor = Ada;
            Context.Box = Piece("Chair", 0f, 0f);
            Assert.IsTrue(Labels(LifeOrder.Choices(Context)).Contains("Sit"));

            Context.Box = Piece("Large Couch 1", 0f, 0f);
            Assert.IsTrue(Labels(LifeOrder.Choices(Context)).Contains("Sit"));

            Context.Box = Piece("King Bed", 0f, 0f);
            List<string> Bed = Labels(LifeOrder.Choices(Context));
            Assert.IsTrue(Bed.Contains("Sleep"));
            Assert.IsFalse(Bed.Contains("Sit"));
        }

        [TestMethod]
        public void SeveralPiecesNearTheCursorAreAllChoices()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            List<FurnitureItem> Room = new List<FurnitureItem>();
            Room.Add(Piece("Chair", 16f, 16f));
            Room.Add(Piece("Television", 70f, 16f));
            List<InteractTarget> Near = LifeOrder.Nearby(Ada, new Vector3(40f, 16f, 0f), 0, Room, null, null);
            Assert.AreEqual(2, Near.Count);
            List<InteractTarget> Far = LifeOrder.Nearby(Ada, new Vector3(40f, 16f, 0f), 0, new List<FurnitureItem> { Piece("Chair", 200f, 16f) }, null, null);
            Assert.AreEqual(0, Far.Count);
        }

        [TestMethod]
        public void IntimateCharactersInTheSameBedCanCuddle()
        {
            FurnitureItem Bed = Piece("King Bed", 16f, 16f);
            Bed.Width = 48f;
            Bed.Length = 48f;
            PlayerCharacter Ada = new PlayerCharacter(null, null, new Vector3(16f, 16f, 0f));
            PlayerCharacter Bea = new PlayerCharacter(null, null, new Vector3(24f, 16f, 0f));
            Ada.Name = "Ada";
            Bea.Name = "Bea";
            InteractionContext Context = new InteractionContext();
            Context.Actor = Ada;
            Context.Other = Bea;
            Context.Room = new List<FurnitureItem> { Bed };

            List<string> Strangers = Labels(LifeOrder.Choices(Context));
            Assert.IsTrue(Strangers.Contains("Talk"));
            Assert.IsTrue(Strangers.Contains("Kiss"));
            Assert.IsFalse(Strangers.Contains("Cuddle"));
            Assert.IsFalse(Strangers.Contains("More"));

            Ada.RelationshipWith(Bea).Status = RelationshipStatus.Lover;
            List<string> Close = Labels(LifeOrder.Choices(Context));
            Assert.IsTrue(Close.Contains("Cuddle"));
            Assert.IsTrue(Close.Contains("More"));

            Bea.SetPosition(new Vector3(200f, 16f, 0f));
            List<string> Apart = Labels(LifeOrder.Choices(Context));
            Assert.IsFalse(Apart.Contains("Cuddle"));
        }
    }
}
