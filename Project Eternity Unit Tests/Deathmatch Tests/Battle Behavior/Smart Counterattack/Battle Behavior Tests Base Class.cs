using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    [TestClass]
    public partial class SmartCounterattackBattleBehaviorTests
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
        }

        private Squad CreateDummySquad()
        {
            return DeathmatchMapHelper.CreateDummySquad(GlobalDeathmatchContext);
        }

        private Squad CreateDummySquadWithWingmans()
        {
            return DeathmatchMapHelper.CreateDummySquadWithWingmans(GlobalDeathmatchContext);
        }
    }
}
