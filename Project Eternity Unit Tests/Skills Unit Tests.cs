using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units.Normal;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class SkillsUnitTests
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
        public void TestLoadAll()
        {
            BaseEffect.LoadAllEffects();
            BaseSkillRequirement.LoadAllRequirements();
            AutomaticSkillTargetType.LoadAllTargetTypes();
        }

        [TestMethod]
        public void TestSkillCreation()
        {
            BaseAutomaticSkill NewPilotSkill = new BaseAutomaticSkill();

            Character DummyCharacter = new Character();
            DummyCharacter.Level = 1;
            DummyCharacter.ArrayLevelMEL = new int[1] { 100 };
            DummyCharacter.ArrayLevelRNG = new int[1] { 100 };
            DummyCharacter.ArrayLevelDEF = new int[1] { 100 };
            DummyCharacter.ArrayLevelSKL = new int[1] { 100 };
            DummyCharacter.ArrayLevelEVA = new int[1] { 100 };
            DummyCharacter.ArrayLevelHIT = new int[1] { 100 };
            DummyCharacter.Init();

            Unit DummyUnit = new UnitNormal();
            Attack DummyAttack = new Attack("Dummy Attack", string.Empty, 0, "10000", 0, 5, WeaponPrimaryProperty.None, WeaponSecondaryProperty.None, 10, 5, 0, 1, 100, "Laser", new System.Collections.Generic.Dictionary<string, char>());
            DummyAttack.PostMovementLevel = 1;
            DummyUnit.ArrayCharacterActive = new Character[1] { DummyCharacter };
            DummyUnit.ListAttack.Add(DummyAttack);
            DummyUnit.CurrentAttack = DummyAttack;
            Squad DummySquad = new Squad("Dummy", DummyUnit);

            Assert.AreEqual(190, DeathmatchMap.Accuracy(DummyUnit, 0));
        }
    }
}
