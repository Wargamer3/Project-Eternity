using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.SorcererStreetScreen;
using static ProjectEternity.Core.Units.UnitMapComponent;

namespace ProjectEternity.UnitTests.SorcererStreetTests
{
    [TestClass]
    public class SorcererStreetMapTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
        }

        [TestInitialize()]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestMapCreation()
        {
            CreateDummyMap();
        }

        [TestMethod]
        public void TestGainPhase()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];
            ActionPanelCastlePhase GainPhase = new ActionPanelCastlePhase(DummyMap, 0, 1);

            Assert.AreEqual(0, DummyPlayer.Magic);
            GainPhase.OnSelect();
            Assert.AreEqual(20, DummyPlayer.Magic);
        }

        [TestMethod]
        public void TestDrawCardPhase()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];
            ActionPanelDrawCardPhase DrawCardPhase = new ActionPanelDrawCardPhase(DummyMap, 0);

            Assert.AreEqual(3, DummyPlayer.ListRemainingCardInDeck.Count);
            Assert.AreEqual(0, DummyPlayer.ListCardInHand.Count);
            DrawCardPhase.OnSelect();
            Assert.AreEqual(2, DummyPlayer.ListRemainingCardInDeck.Count);
            Assert.AreEqual(1, DummyPlayer.ListCardInHand.Count);
        }

        [TestMethod]
        public void TestRefillDeckPhase()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];
            ActionPanelRefillDeckPhase RefillDeckPhase = new ActionPanelRefillDeckPhase(DummyMap, 0);

            DummyPlayer.ListRemainingCardInDeck.Clear();
            Assert.AreEqual(0, DummyPlayer.ListRemainingCardInDeck.Count);
            RefillDeckPhase.OnSelect();
            Assert.AreEqual(3, DummyPlayer.ListRemainingCardInDeck.Count);
        }

        [TestMethod]
        public void TestSpellSelectionPhase()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];
            ActionPanelCardSelectionPhase SpellSelectionPhase = new ActionPanelSpellCardSelectionPhase(DummyMap, 0);

            DummyPlayer.ListCardInHand.Add(new CreatureCard(20, 20));
            DummyPlayer.ListCardInHand.Add(new CreatureCard(20, 20));
            DummyPlayer.ListCardInHand.Add(new CreatureCard(20, 20));
            DummyPlayer.ListCardInHand.Add(new CreatureCard(20, 20));

            SpellSelectionPhase.OnSelect();
        }

        [TestMethod]
        public void TestRollDicePhaseGoingRight()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];
            DummyPlayer.GamePiece.Direction = DirectionRight;
            ActionPanelRollDicePhase RollDicePhase = new ActionPanelRollDicePhase(DummyMap, 0);

            RollDicePhase.RollDice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelMovementPhase));
        }

        [TestMethod]
        public void TestRollDicePhaseWithoutDirection()
        {
            Player DummyPlayer = new Player("Player 1", "Human", "Host", false, 0, true, Color.Blue, new List<Card>());
            SorcererStreetMap DummyMap = CreateDummyMap();
            DummyMap.AddPlayer(DummyPlayer);
            ActionPanelRollDicePhase RollDicePhase = new ActionPanelRollDicePhase(DummyMap, 0);

            RollDicePhase.RollDice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelChooseDirection));
        }

        [TestMethod]
        public void TestPlayerMovementPhase()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];
            DummyPlayer.GamePiece.Direction = DirectionRight;
            ActionPanelMovementPhase MovementPhase = new ActionPanelMovementPhase(DummyMap, 0, 3);

            DummyMap.ListActionMenuChoice.AddToPanelListAndSelect(MovementPhase);

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasMainPanel);
            Assert.AreEqual(1, DummyPlayer.GamePiece.Position.X);

            for (int i = 0; i < 6; ++i)
            {
                DummyMap.Update(new GameTime());
            }

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasSubPanels);
            Assert.AreEqual(2, DummyPlayer.GamePiece.Position.X);

            for (int i = 0; i < 6; ++i)
            {
                DummyMap.Update(new GameTime());
            }

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasSubPanels);
            Assert.AreEqual(3, DummyPlayer.GamePiece.Position.X);
        }

        [TestMethod]
        public void TestDiscardCardPhase()
        {
            Player DummyPlayer = new Player("Player 1", "Human", "Host", false, 0, true, Color.Blue, new List<Card>());
            SorcererStreetMap DummyMap = CreateDummyMap();
            DummyMap.AddPlayer(DummyPlayer);
            ActionPanelDiscardCardPhase DiscardCardPhase = new ActionPanelDiscardCardPhase(DummyMap, 0, 6);
            
            DiscardCardPhase.OnSelect();
        }

        [TestMethod]
        public void TestMapBehavior()
        {
            SorcererStreetMap DummyMap = CreateDummyMap();
            Player DummyPlayer = DummyMap.ListAllPlayer[0];

            DummyMap.Update(new GameTime());

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelPlayerDefault));
            Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            ActionPanelPlayerDefault PlayerDefault = (ActionPanelPlayerDefault)DummyMap.ListActionMenuChoice.Last();
            PlayerDefault.ConfirmStartOfTurn();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelDrawCardPhase));
            Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            ActionPanelDrawCardPhase DrawCardPhase = (ActionPanelDrawCardPhase)DummyMap.ListActionMenuChoice.Last();
            DrawCardPhase.FinishPhase();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelCardSelectionPhase));
            Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            ActionPanelCardSelectionPhase CardSelectionPhase = (ActionPanelCardSelectionPhase)DummyMap.ListActionMenuChoice.Last();
            CardSelectionPhase.PrepareToRollDice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelRollDicePhase));
            Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            ActionPanelRollDicePhase RollDicePhase = (ActionPanelRollDicePhase)DummyMap.ListActionMenuChoice.Last();
            RollDicePhase.RollDice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelChooseDirection));
            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasSubPanels);

            ActionPanelChooseDirection ChooseDirection = (ActionPanelChooseDirection)DummyMap.ListActionMenuChoice.Last();
            DummyPlayer.GamePiece.Direction = DirectionRight;
            ChooseDirection.FinalizeChoice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelMovementPhase));
            Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            while (DummyMap.ListActionMenuChoice.Last() is ActionPanelMovementPhase)
            {
                DummyMap.Update(new GameTime());
            }

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelCardSelectionPhase));
            Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            ActionPanelCardSelectionPhase CreatureCardSelectionPhase = (ActionPanelCardSelectionPhase)DummyMap.ListActionMenuChoice.Last();
        }

        private static SorcererStreetMap CreateDummyMap()
        {
            SorcererStreetMap DummyMap = new SorcererStreetMap();
            DummyMap.LayerManager.ListLayer.Add(new MapLayer(DummyMap, 0));
            DummyMap.ListGameScreen = new List<GameScreens.GameScreen>();
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain = new TerrainSorcererStreet[20, 20];
            for (int X = 0; X < 20; ++X)
            {
                for (int Y = 0; Y < 20; ++Y)
                {
                    DummyMap.LayerManager.ListLayer[0].ArrayTerrain[X, Y] = new ElementalTerrain(X, Y, 0, 0, 0);
                }
            }

            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 0].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[1, 0].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[2, 0].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[3, 0].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 0].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 1].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 2].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 3].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 4].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 5].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[3, 5].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[2, 5].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[1, 5].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 5].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 4].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 3].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 2].TerrainTypeIndex = 1;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 1].TerrainTypeIndex = 1;

            Player DummyPlayer = new Player("Player 1", "Human", "Host", false, 0, true, Color.Blue, new List<Card>() { new CreatureCard(20, 20), new CreatureCard(20, 20), new CreatureCard(20, 20) });
            DummyMap.AddPlayer(DummyPlayer);

            DummyMap.Init();
            DummyMap.ListActionMenuChoice.Add(new ActionPanelPlayerDefault(DummyMap));

            return DummyMap;
        }
    }
}
