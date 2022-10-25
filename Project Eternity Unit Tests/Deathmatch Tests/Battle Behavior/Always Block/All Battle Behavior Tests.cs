using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using Microsoft.Xna.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    public partial class AlwaysBlockBattleBehaviorTests
    {
        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASingleUnitSquadAtCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASingleUnitSquadOutCounterRange_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(13, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromASingleUnitSquad_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
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
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad[1].CurrentAttack);
            Assert.AreEqual(null, DummySquad[2].CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad[1].ListAttack[0].PowerFormula = "0";
            DummySquad[2].ListAttack[0].PowerFormula = "20000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad[1].CurrentAttack);
            Assert.AreEqual(null, DummySquad[2].CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByASurvivableLethalAttackFromBothWingmans_ThenTheDefender()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad[1].ListAttack[0].PowerFormula = "10000";
            DummySquad[2].ListAttack[0].PowerFormula = "10000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad[1].CurrentAttack);
            Assert.AreEqual(null, DummySquad[2].CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromWingmanA_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad[1].ListAttack[0].PowerFormula = "30000";
            DummySquad[2].ListAttack[0].PowerFormula = "0";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad[1].CurrentAttack);
            Assert.AreEqual(null, DummySquad[2].CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromWingmanB_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad[1].ListAttack[0].PowerFormula = "0";
            DummySquad[2].ListAttack[0].PowerFormula = "30000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad[1].CurrentAttack);
            Assert.AreEqual(null, DummySquad[2].CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASingleUnitSquad_WhenAllAttackedByAnUnsurvivableLethalAttackFromBothWingmans_ThenTheDefenderDefend()
        {
            Squad DummySquad = CreateDummySquadWithWingmans();
            Squad EnemySquad = CreateDummySquad();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "0";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;
            DummySquad[1].ListAttack[0].PowerFormula = "20000";
            DummySquad[2].ListAttack[0].PowerFormula = "20000";

            DummySquad[1].CurrentAttack = null;
            DummySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Spread;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, DummySquad[2].BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);
            Assert.AreEqual(null, DummySquad[1].CurrentAttack);
            Assert.AreEqual(null, DummySquad[2].CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByASingleUnitSquadAtCounterRange_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "4000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.CurrentLeader.CurrentAttack = null;
            EnemySquad[1].CurrentAttack = null;
            EnemySquad[2].CurrentAttack = null;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad[2].BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNull(EnemySquad[1].CurrentAttack);
            Assert.IsNull(EnemySquad[2].CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByASurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "10000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad[2].BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNull(EnemySquad[1].CurrentAttack);
            Assert.IsNull(EnemySquad[2].CurrentAttack);
        }

        [TestMethod]
        public void GivenASquadWithWingmans_WhenAllAttackedByAnUnsurvivableLethalAttack_ThenTheWingmansDefend()
        {
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquadWithWingmans();

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "20000";
            DummySquad.CurrentLeader.ListAttack[0].Pri = Core.Attacks.WeaponPrimaryProperty.ALL;

            EnemySquad.IsPlayerControlled = false;
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector2(3, 5), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector2(5, 5), 0);
            DummyMap.BattleMenuOffenseFormationChoice = BattleMap.FormationChoices.ALL;
            DummyMap.BattleMenuDefenseFormationChoice = BattleMap.FormationChoices.Focused;
            DummyMap.PrepareSquadsForBattle(0, 0, DummySquad.CurrentLeader.CurrentAttack, 1, 0);

            Assert.AreEqual(Unit.BattleDefenseChoices.Attack, DummySquad.CurrentLeader.BattleDefenseChoice);

            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad.CurrentLeader.BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad[1].BattleDefenseChoice);
            Assert.AreEqual(Unit.BattleDefenseChoices.Defend, EnemySquad[2].BattleDefenseChoice);

            Assert.IsNotNull(DummySquad.CurrentLeader.CurrentAttack);

            Assert.AreEqual(null, EnemySquad.CurrentLeader.CurrentAttack);
            Assert.IsNull(EnemySquad[1].CurrentAttack);
            Assert.IsNull(EnemySquad[2].CurrentAttack);
        }
    }
}
