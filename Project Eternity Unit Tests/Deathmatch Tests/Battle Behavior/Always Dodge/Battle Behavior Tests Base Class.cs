using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    [TestClass]
    public partial class AlwaysDodgeBattleBehaviorTests
    {
        private static DeathmatchMap DummyMap;
        private static DeathmatchContext GlobalDeathmatchContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            AutomaticSkillTargetType.DicTargetType.Clear();

            GlobalDeathmatchContext = new DeathmatchContext();
            DummyMap = DeathmatchMapHelper.CreateDummyMap(GlobalDeathmatchContext);
        }

        [TestInitialize()]
        public void Initialize()
        {
        }

        private Squad CreateDummySquad()
        {
            Squad DummySquad = DeathmatchMapHelper.CreateDummySquad(GlobalDeathmatchContext);
            DummySquad.SquadDefenseBattleBehavior = "Always Dodge";
            return DummySquad;
        }

        private Squad CreateDummySquadWithWingmans()
        {
            Squad DummySquad = DeathmatchMapHelper.CreateDummySquadWithWingmans(GlobalDeathmatchContext);
            DummySquad.SquadDefenseBattleBehavior = "Always Dodge";
            return DummySquad;
        }
    }
}
