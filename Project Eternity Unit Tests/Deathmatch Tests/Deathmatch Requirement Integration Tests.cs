using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Effects;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public partial class DeathmatchTests
    {
        private static DeathmatchMap DummyMap;
        private static DeathmatchContext GlobalDeathmatchContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            GlobalDeathmatchContext = new DeathmatchContext();
            DummyMap = DeathmatchMapHelper.CreateDummyMap(GlobalDeathmatchContext);
        }

        [TestInitialize()]
        public void Initialize()
        {
            if (DummyMap.ListPlayer.Count > 0)
                DummyMap.ListPlayer[0].ListSquad.Clear();
            if (DummyMap.ListPlayer.Count > 1)
                DummyMap.ListPlayer[1].ListSquad.Clear();
        }

        private Squad CreateDummySquad()
        {
            return DeathmatchMapHelper.CreateDummySquad(GlobalDeathmatchContext);
        }

        private BaseAutomaticSkill CreateDummySkill(BaseSkillRequirement Requirement)
        {
            BaseAutomaticSkill TestSkill = new BaseAutomaticSkill();
            TestSkill.CurrentLevel = 1;
            TestSkill.Name = "Test";
            BaseSkillLevel TestSkillLevel = new BaseSkillLevel();
            BaseSkillActivation TestSkillActivation = new BaseSkillActivation();

            TestSkillActivation.ListEffectTarget.Add(new List<string>() { "Self" });
            TestSkillActivation.ListEffectTargetReal.Add(new List<AutomaticSkillTargetType>() { DummyMap.DicAutomaticSkillTarget[EffectActivationSelf.Name].Copy() });
            TestSkillActivation.ActivationPercentage = 100;
            TestSkillActivation.ListRequirement.Add(Requirement);

            FinalDamageEffect NewEffect = (FinalDamageEffect)DummyMap.DicEffect[FinalDamageEffect.Name].Copy();
            NewEffect.FinalDamageValue = "100";
            NewEffect.NumberType = Core.Operators.NumberTypes.Absolute;
            NewEffect.LifetimeType = SkillEffect.LifetimeTypePermanent;

            TestSkillActivation.ListEffect.Add(NewEffect);
            TestSkillLevel.ListActivation.Add(TestSkillActivation);
            TestSkill.ListSkillLevel.Add(TestSkillLevel);

            return TestSkill;
        }

        [TestMethod]
        public void TestBattle()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;
            Unit DummyDefenderUnit = DummyDefenderSquad.CurrentLeader;
            
            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);

            Assert.AreEqual(100, DummyMap.CalculateHitRate(DummyUnit, DummySquad, DummyDefenderUnit, DummyDefenderSquad, Unit.BattleDefenseChoices.Defend));

            Assert.AreEqual(11880, Result.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestPassiveRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;

            BaseAutomaticSkill TestSkill = CreateDummySkill(new PassiveRequirement());
            DummyUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);
            
            Assert.AreEqual(11980, Result.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestBattleStartRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BattleStartRequirement(GlobalDeathmatchContext));
            DummyUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };
            
            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);
            
            Assert.AreEqual(11980, Result.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestBeforeAttackRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BeforeAttackRequirement(GlobalDeathmatchContext));
            DummyUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);

            Assert.AreEqual(11980, Result.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestBeforeGettingAttackedRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BeforeAttackRequirement(GlobalDeathmatchContext));
            DummyDefenderSquad.CurrentLeader.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);
            var ResultCounter = DummyMap.CalculateFinalHP(DummyDefenderSquad, null, 0, BattleMap.FormationChoices.Focused, DummySquad, null, 1, true, false);

            Assert.AreEqual(11880, Result.ArrayResult[0].AttackDamage);
            Assert.AreEqual(11980, ResultCounter.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestBeforeHitRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BeforeHitRequirement(GlobalDeathmatchContext));
            DummyUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);

            Assert.AreEqual(11980, Result.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestBeforeGettingHitRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            DummySquad.SquadName = "Attacker";
            Squad DummyDefenderSquad = CreateDummySquad();
            DummyDefenderSquad.SquadName = "Defender";

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BeforeGettingHitRequirement(GlobalDeathmatchContext));
            DummyDefenderSquad.CurrentLeader.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);
            var ResultCounter = DummyMap.CalculateFinalHP(DummyDefenderSquad, null, 0, BattleMap.FormationChoices.Focused, DummySquad, null, 1, true, false);

            Assert.AreEqual(11880, Result.ArrayResult[0].AttackDamage);
            Assert.AreEqual(11980, ResultCounter.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void TestBeforeMissRequirement()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;
            Unit DummyDefenderUnit = DummyDefenderSquad.CurrentLeader;
            DummyUnit.Pilot.ArrayLevelHIT[0] = 0;
            DummyUnit.Pilot.Init();
            DummyDefenderUnit.Pilot.ArrayLevelEVA[0] = 200;
            DummyDefenderUnit.Pilot.Init();

            Assert.AreEqual(0, DummyMap.CalculateHitRate(DummyUnit, DummySquad, DummyDefenderUnit, DummyDefenderSquad, Unit.BattleDefenseChoices.Defend));

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BeforeMissRequirement(GlobalDeathmatchContext));
            DummyUnit.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };
            DummyUnit.Pilot.ArrayPilotSkillLevels = new Core.Characters.Character.SkillLevels[1] { new Core.Characters.Character.SkillLevels() };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);

            Assert.AreEqual(0, Result.ArrayResult[0].AttackDamage);
            Assert.AreEqual(true, Result.ArrayResult[0].AttackMissed);

            DummyUnit.Pilot.ArrayLevelHIT[0] = 200;
            DummyUnit.Pilot.Init();

            Assert.AreEqual(100, DummyMap.CalculateHitRate(DummyUnit, DummySquad, DummyDefenderUnit, DummyDefenderSquad, Unit.BattleDefenseChoices.Defend));

            Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);

            Assert.AreEqual(11980, Result.ArrayResult[0].AttackDamage);
        }

        [TestMethod]
        public void GivenBeforeGettingMissedRequirement_WhenActivatedDuringABattle_ThenFinalDamageChanged()
        {
            Squad DummySquad = CreateDummySquad();
            Squad DummyDefenderSquad = CreateDummySquad();
            Unit DummyUnit = DummySquad.CurrentLeader;
            Unit DummyDefenderUnit = DummyDefenderSquad.CurrentLeader;
            DummyUnit.Pilot.ArrayLevelHIT[0] = 0;
            DummyUnit.Pilot.Init();
            DummyDefenderUnit.Pilot.ArrayLevelEVA[0] = 200;
            DummyDefenderUnit.Pilot.Init();

            Assert.AreEqual(0, DummyMap.CalculateHitRate(DummyUnit, DummySquad, DummyDefenderUnit, DummyDefenderSquad, Unit.BattleDefenseChoices.Defend));

            BaseAutomaticSkill TestSkill = CreateDummySkill(new BeforeGettingMissedRequirement(GlobalDeathmatchContext));
            DummyDefenderSquad.CurrentLeader.Pilot.ArrayPilotSkill = new BaseAutomaticSkill[1] { TestSkill };

            var Result = DummyMap.CalculateFinalHP(DummySquad, null, 0, BattleMap.FormationChoices.Focused, DummyDefenderSquad, null, 1, true, false);
            var ResultCounter = DummyMap.CalculateFinalHP(DummyDefenderSquad, null, 0, BattleMap.FormationChoices.Focused, DummySquad, null, 1, true, false);
            
            Assert.AreEqual(0, Result.ArrayResult[0].AttackDamage);
            Assert.AreEqual(true, Result.ArrayResult[0].AttackMissed);

            Assert.AreEqual(11980, ResultCounter.ArrayResult[0].AttackDamage);
        }
    }
}
