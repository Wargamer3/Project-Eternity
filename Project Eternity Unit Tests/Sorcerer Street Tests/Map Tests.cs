﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;

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
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelGainPhase GainPhase = new ActionPanelGainPhase(DummyMap, DummyPlayer);

            Assert.AreEqual(0, DummyPlayer.Magic);
            GainPhase.OnSelect();
            Assert.AreEqual(20, DummyPlayer.Magic);
        }

        [TestMethod]
        public void TestDrawCardPhase()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[] { new Card(), new Card(), new Card() });
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelDrawCardPhase DrawCardPhase = new ActionPanelDrawCardPhase(DummyMap, DummyPlayer);

            Assert.AreEqual(3, DummyPlayer.ListRemainingCardInDeck.Count);
            Assert.AreEqual(0, DummyPlayer.ListCardInHand.Count);
            DrawCardPhase.OnSelect();
            Assert.AreEqual(2, DummyPlayer.ListRemainingCardInDeck.Count);
            Assert.AreEqual(1, DummyPlayer.ListCardInHand.Count);
        }

        [TestMethod]
        public void TestRefillDeckPhase()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[] { new Card(), new Card(), new Card() });
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelRefillDeckPhase RefillDeckPhase = new ActionPanelRefillDeckPhase(DummyMap, DummyPlayer);

            DummyPlayer.ListRemainingCardInDeck.Clear();
            Assert.AreEqual(0, DummyPlayer.ListRemainingCardInDeck.Count);
            RefillDeckPhase.OnSelect();
            Assert.AreEqual(3, DummyPlayer.ListRemainingCardInDeck.Count);
        }

        [TestMethod]
        public void TestSpellSelectionPhase()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelCardSelectionPhase SpellSelectionPhase = new ActionPanelSpellCardSelectionPhase(DummyMap, DummyPlayer);

            DummyPlayer.ListCardInHand.Add(new Card());
            DummyPlayer.ListCardInHand.Add(new Card());
            DummyPlayer.ListCardInHand.Add(new Card());
            DummyPlayer.ListCardInHand.Add(new Card());

            SpellSelectionPhase.OnSelect();
        }

        [TestMethod]
        public void TestRollDicePhaseGoingRight()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            DummyPlayer.CurrentDirection = Player.Directions.Right;
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelRollDicePhase RollDicePhase = new ActionPanelRollDicePhase(DummyMap, DummyPlayer);

            RollDicePhase.RollDice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelMovementPhase));
        }

        [TestMethod]
        public void TestRollDicePhaseWithoutDirection()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelRollDicePhase RollDicePhase = new ActionPanelRollDicePhase(DummyMap, DummyPlayer);

            RollDicePhase.RollDice();

            Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelChooseDirection));
        }

        [TestMethod]
        public void TestPlayerMovementPhase()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            DummyPlayer.CurrentDirection = Player.Directions.Right;
            SorcererStreetMap DummyMap = CreateDummyMap();
            DummyMap.ListPlayer.Add(DummyPlayer);
            ActionPanelMovementPhase MovementPhase = new ActionPanelMovementPhase(DummyMap, DummyPlayer, 3);

            DummyMap.ListActionMenuChoice.AddToPanelListAndSelect(MovementPhase);

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasMainPanel);
            Assert.AreEqual(1, DummyPlayer.GamePiece.Position.X);

            for (int i = 0; i < 6; ++i)
            {
                DummyMap.Update(new GameTime());
            }

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasMainPanel);
            Assert.AreEqual(2, DummyPlayer.GamePiece.Position.X);

            for (int i = 0; i < 6; ++i)
            {
                DummyMap.Update(new GameTime());
            }

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasMainPanel);
            Assert.AreEqual(3, DummyPlayer.GamePiece.Position.X);

            for (int i = 0; i < 6; ++i)
            {
                DummyMap.Update(new GameTime());
            }

            Assert.IsTrue(DummyMap.ListActionMenuChoice.HasMainPanel);
        }

        [TestMethod]
        public void TestDiscardCardPhase()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            SorcererStreetMap DummyMap = CreateDummyMap();
            ActionPanelDiscardCardPhase DiscardCardPhase = new ActionPanelDiscardCardPhase(DummyMap, DummyPlayer, 6);
            
            DiscardCardPhase.OnSelect();
        }

        [TestMethod]
        public void TestMapBehavior()
        {
            Player DummyPlayer = new Player("Player 1", "Human", true, false, 0, new Card[] { new Card(), new Card(), new Card() });
            SorcererStreetMap DummyMap = CreateDummyMap();

            DummyMap.ListPlayer.Add(DummyPlayer);

            DummyMap.Update(new GameTime());

            //Assert.IsInstanceOfType(DummyMap.ListActionMenuChoice.Last(), typeof(ActionPanelGainPhase));
            //Assert.IsFalse(DummyMap.ListActionMenuChoice.HasSubPanels);

            //ActionPanelGainPhase GainPhase = (ActionPanelGainPhase)DummyMap.ListActionMenuChoice.Last();
            //GainPhase.FinishPhase();

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
            DummyPlayer.CurrentDirection = Player.Directions.Right;
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
            DummyMap.GameTurn = 1;
            DummyMap.ListLayer.Add(new MapLayer(DummyMap, new List<AnimationBackground>(), new List<AnimationBackground>()));
            DummyMap.ListGameScreen = new List<GameScreens.GameScreen>();
            DummyMap.ListLayer[0].ArrayTerrain = new TerrainSorcererStreet[20, 20];
            for (int X = 0; X < 20; ++X)
            {
                for (int Y = 0; Y < 20; ++Y)
                {
                    DummyMap.ListLayer[0].ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, 1);
                }
            }

            DummyMap.ListLayer[0].ArrayTerrain[0, 0].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[1, 0].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[2, 0].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[3, 0].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[4, 0].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[4, 1].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[4, 2].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[4, 3].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[4, 4].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[4, 5].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[3, 5].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[2, 5].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[1, 5].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[0, 5].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[0, 4].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[0, 3].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[0, 2].TerrainTypeIndex = 1;
            DummyMap.ListLayer[0].ArrayTerrain[0, 1].TerrainTypeIndex = 1;

            DummyMap.Init();

            return DummyMap;
        }
    }
}
