using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    public partial class AlwaysCounterattackBattleBehaviorTests
    {
        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASingleUnitSquadOutCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(13, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.DoNothing, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromWingmanA_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
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
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromWingmanB_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromBothWingmans_ThenTheDefender()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "10000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromWingmanA_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "30000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "0";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromWingmanB_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "30000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromBothWingmans_ThenTheDefenderCounter()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentWingmanA.CurrentAttack = null;
            DummySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanA.CurrentAttack);
            Assert.AreEqual(null, DummySquad.CurrentWingmanB.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByASingleUnitSquadAtCounterRange_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.CurrentLeader.CurrentAttack = null;
            EnemySquad.CurrentWingmanA.CurrentAttack = null;
            EnemySquad.CurrentWingmanB.CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(EnemySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(EnemySquad.CurrentWingmanB.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByASurvivableLethalAttack_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(EnemySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(EnemySquad.CurrentWingmanB.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByAnUnsurvivableLethalAttack_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.IsNotNull(EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNotNull(EnemySquad.CurrentWingmanA.CurrentAttack);
            Assert.IsNotNull(EnemySquad.CurrentWingmanB.CurrentAttack);
        }
    }
}
