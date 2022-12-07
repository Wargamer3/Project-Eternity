using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.UnitTests.SorcererStreetTests
{
    [TestClass]
    public class SorcererStreetBattleTests
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
        public void TestBattlePhaseLoad()
        {
            CreatureCard DummyInvaderCard = new CreatureCard(40, 20);
            CreatureCard DummyDefenderCard = new CreatureCard(40, 20);
            Player DummyPlayer1 = new Player("Player 1", "Human", "Host", false, 0, true, Color.Blue, new List<Card>() { DummyInvaderCard });
            Player DummyPlayer2 = new Player("Player 2", "Human", "Host", false, 0, true, Color.Blue, new List<Card>() { DummyDefenderCard });

            SorcererStreetMap DummyMap = CreateDummyMap();
            DummyMap.AddPlayer(DummyPlayer1);
            DummyMap.AddPlayer(DummyPlayer2);

            TerrainSorcererStreet DummyTerrain = DummyMap.GetTerrain(DummyPlayer2.GamePiece);
            DummyTerrain.DefendingCreature = DummyDefenderCard;
            DummyTerrain.PlayerOwner = DummyPlayer2;

            ActionPanelBattleStartPhase BattleStartPhase = new ActionPanelBattleStartPhase(DummyMap, 0, DummyInvaderCard);
            BattleStartPhase.OnSelect();

            Assert.AreEqual(DummyInvaderCard, DummyMap.GlobalSorcererStreetBattleContext.Invader);
            Assert.AreEqual(DummyDefenderCard, DummyMap.GlobalSorcererStreetBattleContext.Defender);

            Assert.AreEqual(DummyInvaderCard.CurrentHP, DummyMap.GlobalSorcererStreetBattleContext.InvaderFinalHP);
            Assert.AreEqual(DummyDefenderCard.CurrentHP, DummyMap.GlobalSorcererStreetBattleContext.DefenderFinalHP);

            Assert.AreEqual(DummyInvaderCard.CurrentST, DummyMap.GlobalSorcererStreetBattleContext.InvaderFinalST);
            Assert.AreEqual(DummyDefenderCard.CurrentST, DummyMap.GlobalSorcererStreetBattleContext.DefenderFinalST);
        }

        [TestMethod]
        public void TestBattlePhaseWithoutBonuses()
        {
            CreatureCard DummyInvaderCard = new CreatureCard(40, 20);
            CreatureCard DummyDefenderCard = new CreatureCard(40, 20);
            Player DummyPlayer1 = new Player("Player 1", "Human", "Host", false, 0, true, Color.Blue, new List<Card>() { DummyInvaderCard });
            Player DummyPlayer2 = new Player("Player 2", "Human", "Host", false, 0, true, Color.Blue, new List<Card>() { DummyDefenderCard });

            SorcererStreetMap DummyMap = CreateDummyMap();
            DummyMap.AddPlayer(DummyPlayer1);
            DummyMap.AddPlayer(DummyPlayer2);

            TerrainSorcererStreet DummyTerrain = DummyMap.GetTerrain(DummyPlayer2.GamePiece);
            DummyTerrain.DefendingCreature = DummyDefenderCard;
            DummyTerrain.PlayerOwner = DummyPlayer2;

            ActionPanelBattleStartPhase BattleStartPhase = new ActionPanelBattleStartPhase(DummyMap, 0, DummyInvaderCard);
            BattleStartPhase.OnSelect();

            ActionPanelBattleAttackPhase BattleAttackPhase = new ActionPanelBattleAttackPhase(DummyMap);
            BattleAttackPhase.OnSelect();

            Assert.AreEqual(DummyInvaderCard, BattleAttackPhase.FirstAttacker);
            Assert.AreEqual(DummyDefenderCard, BattleAttackPhase.SecondAttacker);

            BattleAttackPhase.ExecuteFirstAttack();

            Assert.AreEqual(40, BattleAttackPhase.FirstAttacker.CurrentHP);
            Assert.AreEqual(20, BattleAttackPhase.SecondAttacker.CurrentHP);

            BattleAttackPhase.ExecuteSecondAttack();

            Assert.AreEqual(20, BattleAttackPhase.FirstAttacker.CurrentHP);
            Assert.AreEqual(20, BattleAttackPhase.SecondAttacker.CurrentHP);
        }

        private static SorcererStreetMap CreateDummyMap()
        {
            SorcererStreetMap DummyMap = new SorcererStreetMap();
            DummyMap.GameTurn = 1;
            DummyMap.LayerManager.ListLayer.Add(new MapLayer(DummyMap, 0));
            DummyMap.ListGameScreen = new List<GameScreens.GameScreen>();
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain = new TerrainSorcererStreet[20, 20];
            for (int X = 0; X < 20; ++X)
            {
                for (int Y = 0; Y < 20; ++Y)
                {
                    DummyMap.LayerManager.ListLayer[0].ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, 0, 0, 1);
                }
            }

            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 0].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[1, 0].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[2, 0].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[3, 0].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 0].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 1].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 2].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 3].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 4].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[4, 5].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[3, 5].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[2, 5].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[1, 5].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 5].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 4].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 3].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 2].TerrainTypeIndex = 2;
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain[0, 1].TerrainTypeIndex = 2;

            DummyMap.Init();

            return DummyMap;
        }
    }
}
