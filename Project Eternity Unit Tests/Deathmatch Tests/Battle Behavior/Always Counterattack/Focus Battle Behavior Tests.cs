using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    public partial class AlwaysCounterattackBattleBehaviorTests
    {
        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASingleUnitSquadOutCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(13, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.DoNothing, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad[1].ListAttack[0].PowerFormula = "20000";
            DummySquad[2].ListAttack[0].PowerFormula = "0";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            DummySquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DummySquad[1].BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DummySquad[2].BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;

            EnemySquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad[1].CurrentAttack);
            Assert.IsNotNull(DummySquad[2].CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromWingmanB_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad[1].ListAttack[0].PowerFormula = "0";
            DummySquad[2].ListAttack[0].PowerFormula = "20000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad[1].CurrentAttack);
            Assert.IsNotNull(DummySquad[2].CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromBothWingmans_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad[1].ListAttack[0].PowerFormula = "10000";
            DummySquad[2].ListAttack[0].PowerFormula = "10000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad[1].CurrentAttack);
            Assert.IsNotNull(DummySquad[2].CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromWingmanA_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad[1].ListAttack[0].PowerFormula = "30000";
            DummySquad[2].ListAttack[0].PowerFormula = "0";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad[1].CurrentAttack);
            Assert.IsNotNull(DummySquad[2].CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromWingmanB_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad[1].ListAttack[0].PowerFormula = "0";
            DummySquad[2].ListAttack[0].PowerFormula = "30000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad[1].CurrentAttack);
            Assert.IsNotNull(DummySquad[2].CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromBothWingmans_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad[1].ListAttack[0].PowerFormula = "20000";
            DummySquad[2].ListAttack[0].PowerFormula = "20000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(DummySquad[1].CurrentAttack);
            Assert.IsNotNull(DummySquad[2].CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            EnemySquad.CurrentLeader.CurrentAttack = null;
            EnemySquad[1].CurrentAttack = null;
            EnemySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad[2].BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(EnemySquad[1].CurrentAttack);
            Assert.IsNotNull(EnemySquad[2].CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByASurvivableLethalAttack_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad[2].BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(EnemySquad[1].CurrentAttack);
            Assert.IsNotNull(EnemySquad[2].CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByAnUnsurvivableLethalAttack_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad[2].BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(EnemySquad[1].CurrentAttack);
            Assert.IsNotNull(EnemySquad[2].CurrentAttack);
        }
    }
}
