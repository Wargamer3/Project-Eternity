using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    [TestClass]
    public partial class AlwaysCounterattackBattleBehaviorTests
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
            DummyMap.ListPlayer.Clear();
        }

        private Squad CreateDummySquad()
        {
            Squad DummySquad = DeathmatchMapHelper.CreateDummySquad(GlobalDeathmatchContext);
            DummySquad.SquadDefenseBattleBehavior = "Always Counterattack";
            return DummySquad;
        }

        private Squad CreateDummySquadWithWingmans()
        {
            Squad DummySquad = DeathmatchMapHelper.CreateDummySquadWithWingmans(GlobalDeathmatchContext);
            DummySquad.SquadDefenseBattleBehavior = "Always Counterattack";
            return DummySquad;
        }
    }
}
