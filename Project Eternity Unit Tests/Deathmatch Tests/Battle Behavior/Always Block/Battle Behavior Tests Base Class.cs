﻿using ProjectEternity.Core.Units;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests.BattleBehavior
{
    [TestClass]
    public partial class AlwaysBlockBattleBehaviorTests
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
            Squad DummySquad = DeathmatchMapHelper.CreateDummySquad(GlobalDeathmatchContext);
            DummySquad.SquadDefenseBattleBehavior = "Always Block";
            return DummySquad;
        }

        private Squad CreateDummySquadWithWingmans()
        {
            Squad DummySquad = DeathmatchMapHelper.CreateDummySquadWithWingmans(GlobalDeathmatchContext);
            DummySquad.SquadDefenseBattleBehavior = "Always Block";
            return DummySquad;
        }
    }
}
