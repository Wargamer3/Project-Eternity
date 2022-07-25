using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    [TestClass]
    public partial class SmartCounterattackBattleBehaviorTests
    {
        private static DeathmatchMap DummyMap;
        private static BattleContext GlobalDeathmatchContext;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            GlobalDeathmatchContext = new BattleContext();
            DummyMap = DeathmatchMapHelper.CreateDummyMap(GlobalDeathmatchContext);
        }

        [TestInitialize()]
        public void Initialize()
        {
            DummyMap.ListPlayer.Clear();
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
