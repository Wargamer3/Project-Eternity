using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    public partial class AlwaysDodgeBattleBehaviorTests
    {
        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASingleUnitSquadOutCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(13, 5, 0));
            EnemySquad.CurrentLeader.AttackIndex = 0;
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            EnemySquad.CurrentLeader.AttackIndex = 0;
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            EnemySquad.CurrentLeader.AttackIndex = 0;
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "0";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            DummySquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DummySquad.CurrentWingmanA.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;
            DummySquad.CurrentWingmanB.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;

            EnemySquad.CurrentLeader.BattleDefenseChoice = Unit.BattleDefenseChoices.Evade;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
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

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "30000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "0";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "30000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromBothWingmans_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByASingleUnitSquadAtCounterRange_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = -1;
            EnemySquad.CurrentWingmanA.AttackIndex = -1;
            EnemySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanB.AttackIndex);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByASurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;
            EnemySquad.CurrentWingmanA.AttackIndex = 0;
            EnemySquad.CurrentWingmanB.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanB.AttackIndex);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByAnUnsurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;
            EnemySquad.CurrentWingmanA.AttackIndex = 0;
            EnemySquad.CurrentWingmanB.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0));
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0));
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(DummySquad, EnemySquad);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanB.AttackIndex);
        }
    }
}
