using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Attacks;
using System.Collections.Generic;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    public partial class SmartCounterattackBattleBehaviorTests
    {
        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderCounter()
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

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(0, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderCounterWithNonPostMovementAttack()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            Attack DummyAttack = new Attack("Dummy Attack", string.Empty, 0, "40000", 0, 5, WeaponPrimaryProperty.None,
                WeaponSecondaryProperty.None, 10, 0, 6, 1, 100, "Laser",
                new Dictionary<string, char>() { { "Air", 'S' }, { "Land", 'S' }, { "Sea", 'S' }, { "Space", 'S' } });
            EnemySquad.CurrentLeader.ListAttack.Add(DummyAttack);

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(1, EnemySquad.CurrentLeader.AttackIndex);
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
            EnemySquad.CurrentLeader.AttackIndex = 0;
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
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
            EnemySquad.CurrentLeader.AttackIndex = 0;
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderEvade()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            EnemySquad.CurrentLeader.AttackIndex = 0;
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
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
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

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

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByASurvivableLethalAttackFromBothWingmans_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "10000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

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

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromWingmanA_ThenTheDefenderEvade()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "30000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "0";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromWingmanB_ThenTheDefenderEvade()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "30000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenFocusAttackedByAnUnsurvivableLethalAttackFromBothWingmans_ThenTheDefenderEvade()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentWingmanA.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentWingmanB.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentLeader.AttackIndex = 0;
            DummySquad.CurrentWingmanA.AttackIndex = -1;
            DummySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, DummySquad.CurrentWingmanB.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByASingleUnitSquadAtCounterRange_ThenTheWingmansCounter()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = -1;
            EnemySquad.CurrentWingmanA.AttackIndex = -1;
            EnemySquad.CurrentWingmanB.AttackIndex = -1;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);

            Assert.AreEqual(0, EnemySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(0, EnemySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(0, EnemySquad.CurrentWingmanB.AttackIndex);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByASurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;
            EnemySquad.CurrentWingmanA.AttackIndex = 0;
            EnemySquad.CurrentWingmanB.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanB.AttackIndex);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenFocusAttackedByAnUnsurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";

            DummySquad.CurrentLeader.AttackIndex = 0;

            EnemySquad.CurrentLeader.AttackIndex = 0;
            EnemySquad.CurrentWingmanA.AttackIndex = 0;
            EnemySquad.CurrentWingmanB.AttackIndex = 0;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(5, 5, 0), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Evade, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanA.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentWingmanB.BattleDefenseChoice);

            Assert.AreEqual(0, DummySquad.CurrentLeader.AttackIndex);

            Assert.AreEqual(-1, EnemySquad.CurrentLeader.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanA.AttackIndex);
            Assert.AreEqual(-1, EnemySquad.CurrentWingmanB.AttackIndex);
        }
    }
}
