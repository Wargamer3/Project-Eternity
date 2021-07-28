using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.Core.Item;
using ProjectEternity.GameScreens.AnimationScreen;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.UnitTests.SorcererStreetTests
{
    [TestClass]
    public class SorcererStreetBattleEffectsTests
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
        public void TestManualActivationHPBonus()
        {
            CreatureCard DummyInvaderCard = new CreatureCard(40, 20);
            CreatureCard DummyDefenderCard = new CreatureCard(40, 20);

            SorcererStreetBattleContext GlobalSorcererStreetBattleContext = new SorcererStreetBattleContext();

            GlobalSorcererStreetBattleContext.Invader = DummyInvaderCard;
            GlobalSorcererStreetBattleContext.Defender = DummyDefenderCard;

            GlobalSorcererStreetBattleContext.Invader.ResetBonuses();
            GlobalSorcererStreetBattleContext.Defender.ResetBonuses();

            GlobalSorcererStreetBattleContext.InvaderFinalHP = GlobalSorcererStreetBattleContext.Invader.CurrentHP;
            GlobalSorcererStreetBattleContext.DefenderFinalHP = GlobalSorcererStreetBattleContext.Defender.CurrentHP;
            GlobalSorcererStreetBattleContext.InvaderFinalST = GlobalSorcererStreetBattleContext.Invader.CurrentST;
            GlobalSorcererStreetBattleContext.DefenderFinalST = GlobalSorcererStreetBattleContext.Defender.CurrentST;

            GlobalSorcererStreetBattleContext.UserCreature = GlobalSorcererStreetBattleContext.Invader;
            GlobalSorcererStreetBattleContext.OpponentCreature = GlobalSorcererStreetBattleContext.Defender;

            SorcererStreetBattleParams BattleParams = new SorcererStreetBattleParams(GlobalSorcererStreetBattleContext);
            BaseEffect SkillEffect = new IncreaseHPEffect(BattleParams);
            BaseSkillRequirement Requirement = new SorcererStreetCreaturePhaseRequirement(GlobalSorcererStreetBattleContext);
            AutomaticSkillTargetType Target = new SorcererStreetSelfTargetType(GlobalSorcererStreetBattleContext);

            BaseAutomaticSkill HPIncreaseSkill = new BaseAutomaticSkill();
            HPIncreaseSkill.Name = "Dummy";
            HPIncreaseSkill.ListSkillLevel.Add(new BaseSkillLevel());
            HPIncreaseSkill.CurrentLevel = 1;

            BaseSkillActivation NewActivation = new BaseSkillActivation();
            HPIncreaseSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            NewActivation.ListRequirement.Add(Requirement);

            NewActivation.ListEffect.Add(SkillEffect);
            NewActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>());
            NewActivation.ListEffectTargetReal[0].Add(Target);

            HPIncreaseSkill.AddSkillEffectsToTarget(Requirement.SkillRequirementName);

            Assert.AreEqual(DummyInvaderCard.CurrentHP + 30, GlobalSorcererStreetBattleContext.InvaderFinalHP);
        }

        [TestMethod]
        public void TestCreatureModifierPhaseSkillActivation()
        {
            Player DummyPlayer1 = new Player("Player 1", "Human", true, false, 0, new Card[0]);
            Player DummyPlayer2 = new Player("Player 2", "Human", true, false, 0, new Card[0]);
            CreatureCard DummyInvaderCard = new CreatureCard(40, 20);
            CreatureCard DummyDefenderCard = new CreatureCard(40, 20);

            SorcererStreetMap DummyMap = CreateDummyMap();
            SorcererStreetBattleParams BattleParams = new SorcererStreetBattleParams(DummyMap.GlobalSorcererStreetBattleContext);

            TerrainSorcererStreet DummyTerrain = DummyMap.GetTerrain(DummyPlayer2.GamePiece);
            DummyTerrain.DefendingCreature = DummyDefenderCard;
            DummyTerrain.Owner = DummyPlayer2;

            ActionPanelBattleStartPhase BattleStartPhase = new ActionPanelBattleStartPhase(DummyMap, DummyPlayer1, DummyInvaderCard);
            BattleStartPhase.Load();

            BaseEffect SkillEffect = new IncreaseHPEffect(BattleParams);
            BaseSkillRequirement Requirement = new SorcererStreetCreaturePhaseRequirement(DummyMap.GlobalSorcererStreetBattleContext);
            AutomaticSkillTargetType Target = new SorcererStreetSelfTargetType(DummyMap.GlobalSorcererStreetBattleContext);

            BaseAutomaticSkill HPIncreaseSkill = new BaseAutomaticSkill();
            HPIncreaseSkill.Name = "Dummy";
            HPIncreaseSkill.ListSkillLevel.Add(new BaseSkillLevel());
            HPIncreaseSkill.CurrentLevel = 1;

            BaseSkillActivation NewActivation = new BaseSkillActivation();
            HPIncreaseSkill.CurrentSkillLevel.ListActivation.Add(NewActivation);

            NewActivation.ListRequirement.Add(Requirement);

            NewActivation.ListEffect.Add(SkillEffect);
            NewActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>());
            NewActivation.ListEffectTargetReal[0].Add(Target);

            DummyInvaderCard.ListSkill.Add(HPIncreaseSkill);

            ActionPanelBattleCreatureModifierPhase CreaturePhase = new ActionPanelBattleCreatureModifierPhase(DummyMap.ListActionMenuChoice, DummyMap);
            CreaturePhase.OnSelect();

            Assert.AreEqual(DummyInvaderCard.CurrentHP + 30, DummyMap.GlobalSorcererStreetBattleContext.InvaderFinalHP);
        }

        private static SorcererStreetMap CreateDummyMap()
        {
            SorcererStreetMap DummyMap = new SorcererStreetMap();
            DummyMap.GameTurn = 1;
            DummyMap.ListLayer.Add(new MapLayer(DummyMap));
            DummyMap.ListGameScreen = new List<GameScreens.GameScreen>();
            DummyMap.ListLayer[0].ArrayTerrain = new TerrainSorcererStreet[20, 20];
            for (int X = 0; X < 20; ++X)
            {
                for (int Y = 0; Y < 20; ++Y)
                {
                    DummyMap.ListLayer[0].ArrayTerrain[X, Y] = new TerrainSorcererStreet(X, Y, 1);
                }
            }

            DummyMap.ListLayer[0].ArrayTerrain[0, 0].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[1, 0].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[2, 0].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[3, 0].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[4, 0].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[4, 1].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[4, 2].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[4, 3].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[4, 4].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[4, 5].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[3, 5].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[2, 5].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[1, 5].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[0, 5].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[0, 4].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[0, 3].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[0, 2].TerrainTypeIndex = 2;
            DummyMap.ListLayer[0].ArrayTerrain[0, 1].TerrainTypeIndex = 2;

            DummyMap.Init();

            return DummyMap;
        }
    }
}
