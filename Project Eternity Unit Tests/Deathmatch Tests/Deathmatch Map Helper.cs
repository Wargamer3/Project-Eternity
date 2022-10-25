using System.Collections.Generic;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Units.Normal;
using ProjectEternity.GameScreens.BattleMapScreen;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    class DeathmatchMapHelper
    {
        public static DeathmatchMap CreateDummyMap(BattleContext GlobalDeathmatchContext)
        {
            DeathmatchMap DummyMap = new DeathmatchMap(new DeathmatchParams(GlobalDeathmatchContext));
            DummyMap.LayerManager.ListLayer.Add(new MapLayer(DummyMap, 0));
            DummyMap.LoadDeathmatchAIScripts();

            DummyMap.NonDemoScreen = new NonDemoScreen(DummyMap);
            DummyMap.ListGameScreen = new List<GameScreens.GameScreen>();
            DummyMap.TargetPlayerIndex = 1;

            return DummyMap;
        }

        public static Squad CreateDummySquad(BattleContext GlobalDeathmatchContext)
        {
            Unit DummyLeader = CreateDummyUnit();
            Squad DummySquad = new Squad("Dummy", DummyLeader);
            DummySquad.Init(GlobalDeathmatchContext);

            return DummySquad;
        }

        public static Squad CreateDummySquadWithWingmans(BattleContext GlobalDeathmatchContext)
        {
            Unit DummyLeader = CreateDummyUnit();
            Unit DummyWingmanA = CreateDummyUnit();
            Unit DummyWingmanB = CreateDummyUnit();

            DummyWingmanA.UnitStat.ListAttack[0].Pri = WeaponPrimaryProperty.PLA;
            DummyWingmanB.UnitStat.ListAttack[0].Pri = WeaponPrimaryProperty.PLA;

            Squad DummySquad = new Squad("Dummy", DummyLeader, DummyWingmanA, DummyWingmanB);
            DummySquad.Init(GlobalDeathmatchContext);

            return DummySquad;
        }

        public static Unit CreateDummyUnit()
        {
            Character DummyCharacter = new Character();
            DummyCharacter.Name = "Dummy Pilot";
            DummyCharacter.Level = 1;
            DummyCharacter.ArrayLevelMEL = new int[1] { 100 };
            DummyCharacter.ArrayLevelRNG = new int[1] { 100 };
            DummyCharacter.ArrayLevelDEF = new int[1] { 100 };
            DummyCharacter.ArrayLevelSKL = new int[1] { 100 };
            DummyCharacter.ArrayLevelEVA = new int[1] { 100 };
            DummyCharacter.ArrayLevelHIT = new int[1] { 200 };
            DummyCharacter.ArrayLevelMaxSP = new int[1] { 50 };
            DummyCharacter.DicRankByMovement.Add(0, 1);
            DummyCharacter.DicRankByMovement.Add(1, 1);
            DummyCharacter.DicRankByMovement.Add(2, 1);
            DummyCharacter.DicRankByMovement.Add(3, 1);
            DummyCharacter.PostMVLevel = 1;
            DummyCharacter.Init();

            Unit DummyUnit = new UnitNormal("Dummy Unit");
            DummyUnit.MaxHP = 10000;
            DummyUnit.MaxEN = 200;
            DummyUnit.Armor = 100;
            DummyUnit.Mobility = 50;
            DummyUnit.MaxMovement = 5;
            DummyUnit.UnitStat.PostMVLevel = 1;

            Attack DummyAttack = new Attack("Dummy Attack", string.Empty, 0, "10000", 0, 5, WeaponPrimaryProperty.None,
                WeaponSecondaryProperty.None, 10, 0, 6, 1, 100, "Laser",
                new Dictionary<byte, byte>() { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } });

            DummyAttack.PostMovementLevel = 1;

            DummyUnit.ArrayCharacterActive = new Character[1] { DummyCharacter };
            DummyUnit.UnitStat.ListAttack.Add(DummyAttack);
            DummyUnit.CurrentAttack = DummyAttack;
            DummyUnit.UnitStat.DicRankByMovement.Add(0, 1);
            DummyUnit.UnitStat.DicRankByMovement.Add(1, 1);
            DummyUnit.UnitStat.DicRankByMovement.Add(2, 1);
            DummyUnit.UnitStat.DicRankByMovement.Add(3, 1);

            return DummyUnit;
        }
    }
}
