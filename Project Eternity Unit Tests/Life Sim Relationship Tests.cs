using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class LifeSimRelationshipTests
    {
        [TestMethod]
        public void ANewBondIsAnEmptyNeutralBar()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Ben = new PlayerCharacter(null, null, Vector3.Zero);
            Ada.Name = "Ada";
            Ben.Name = "Ben";

            CharacterRelationship Bond = Ada.RelationshipWith(Ben);

            Assert.AreSame(Ben, Bond.Other);
            Assert.AreEqual(0f, Bond.Progress, 0.001f);
            Assert.AreEqual(0f, Bond.Bar, 0.001f);
            Assert.AreEqual(RelationshipStatus.Neutral, Bond.Status);
            Assert.AreEqual(RelationshipMood.Neutral, Bond.Mood);
            Assert.AreEqual(0, Ada.ArrayRelationshipBonus.Length);
        }

        [TestMethod]
        public void ProgressFillsTheBarAndStopsAtBothEnds()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Ben = new PlayerCharacter(null, null, Vector3.Zero);
            CharacterRelationship Bond = Ada.RelationshipWith(Ben);

            Bond.AddProgress(40f);
            Assert.AreEqual(40f, Bond.Progress, 0.001f);
            Assert.AreEqual(0.4f, Bond.Bar, 0.001f);

            Bond.AddProgress(80f);
            Assert.AreEqual(CharacterRelationship.FullBar, Bond.Progress, 0.001f);

            Bond.AddProgress(-250f);
            Assert.AreEqual(0f, Bond.Progress, 0.001f);
            Assert.AreEqual(RelationshipStatus.Neutral, Bond.Status);
        }

        [TestMethod]
        public void StatusAndMoodStayPutWhenTheBarMoves()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Ben = new PlayerCharacter(null, null, Vector3.Zero);
            CharacterRelationship Bond = Ada.RelationshipWith(Ben);

            Bond.Status = RelationshipStatus.Married;
            Bond.Mood = RelationshipMood.Angry;
            Bond.AddProgress(25f);

            Assert.AreEqual(RelationshipStatus.Married, Bond.Status);
            Assert.AreEqual(RelationshipMood.Angry, Bond.Mood);
            Assert.AreEqual(25f, Bond.Progress, 0.001f);

            Bond.Mood = RelationshipMood.LoveyDovey;
            Assert.AreEqual(25f, Bond.Progress, 0.001f);
            Assert.AreEqual(RelationshipStatus.Married, Bond.Status);
        }

        [TestMethod]
        public void EachCharacterKeepsTheirOwnViewOfTheOther()
        {
            PlayerCharacter Ada = new PlayerCharacter(null, null, Vector3.Zero);
            PlayerCharacter Ben = new PlayerCharacter(null, null, Vector3.Zero);

            CharacterRelationship AdaOnBen = Ada.RelationshipWith(Ben);
            AdaOnBen.Status = RelationshipStatus.BestFriend;
            AdaOnBen.Mood = RelationshipMood.LoveyDovey;
            AdaOnBen.AddProgress(70f);

            CharacterRelationship BenOnAda = Ben.RelationshipWith(Ada);
            Assert.AreEqual(RelationshipStatus.Neutral, BenOnAda.Status);
            Assert.AreEqual(RelationshipMood.Neutral, BenOnAda.Mood);
            Assert.AreEqual(0f, BenOnAda.Progress, 0.001f);
            Assert.AreSame(AdaOnBen, Ada.RelationshipWith(Ben));
            Assert.IsNull(Ada.RelationshipWith(null));
        }
    }
}
