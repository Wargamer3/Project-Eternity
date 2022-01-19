using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    public partial class AlwaysBlockBattleBehaviorTests
    {
        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASingleUnitSquadOutCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(13, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "0";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            DummySquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DummySquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DummySquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;

            EnemySquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromBothWingmans_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "10000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "30000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "0";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "30000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromBothWingmans_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            EnemySquad.CurrentLeader.CurrentAttack = null;
            EnemySquad.CurrentWingmanA.CurrentAttack = null;
            EnemySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNull(EnemySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNull(EnemySquad.CurrentWingmanB.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByASurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNull(EnemySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNull(EnemySquad.CurrentWingmanB.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByAnUnsurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNull(EnemySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNull(EnemySquad.CurrentWingmanB.CurrentAttack);
        }
    }
}
