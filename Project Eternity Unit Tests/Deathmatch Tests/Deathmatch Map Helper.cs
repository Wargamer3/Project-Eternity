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
        public static DeathmatchMap CreateDummyMap(DeathmatchContext GlobalDeathmatchContext)
        {
            DeathmatchMap DummyMap = new DeathmatchMap(GlobalDeathmatchContext);
            DummyMap.LayerManager.ListLayer.Add(new MapLayer(DummyMap));
            DummyMap.LoadEffects();
            DummyMap.LoadSkillRequirements();
            DummyMap.LoadAutomaticSkillActivation();
            DummyMap.LoadDeathmatchAIScripts();

            DummyMap.NonDemoScreen = new NonDemoScreen(DummyMap);
            DummyMap.ListGameScreen = new List<GameScreens.GameScreen>();

            DummyMap.LayerManager.ListLayer.Add(new MapLayer(DummyMap));
            DummyMap.LayerManager.ListLayer[0].ArrayTerrain = new Terrain[20, 20];
            for (int X = 0; X < 20; ++X)
            {
                for (int Y = 0; Y < 20; ++Y)
                {
                    DummyMap.LayerManager.ListLayer[0].ArrayTerrain[X, Y] = new Terrain(X, Y, 1, 0, 1, new TerrainActivation[0], new TerrainBonus[0], new int[0]);
                }
            }

            return DummyMap;
        }

        public static Squad CreateDummySquad(DeathmatchContext GlobalDeathmatchContext)
        {
            Unit DummyLeader = CreateDummyUnit();
            Squad DummySquad = new Squad("Dummy", DummyLeader);
            DummySquad.Init(GlobalDeathmatchContext);

            return DummySquad;
        }

        public static Squad CreateDummySquadWithWingmans(DeathmatchContext GlobalDeathmatchContext)
        {
            Unit DummyLeader = CreateDummyUnit();
            Unit DummyWingmanA = CreateDummyUnit();
            Unit DummyWingmanB = CreateDummyUnit();

            DummyWingmanA.ListAttack[0].Pri = WeaponPrimaryProperty.PLA;
            DummyWingmanB.ListAttack[0].Pri = WeaponPrimaryProperty.PLA;

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
            DummyCharacter.Init();

            Unit DummyUnit = new UnitNormal("Dummy Unit");
            DummyUnit.MaxHP = 10000;
            DummyUnit.MaxEN = 200;
            DummyUnit.Armor = 100;
            DummyUnit.Mobility = 50;
            DummyUnit.MaxMovement = 5;
            Attack DummyAttack = new Attack("Dummy Attack", string.Empty, 0, "10000", 0, 5, WeaponPrimaryProperty.None,
                WeaponSecondaryProperty.PostMovement, 10, 0, 6, 1, 100, "Laser",
                new Dictionary<string, char>() { { "Air", 'S' }, { "Land", 'S' }, { "Sea", 'S' }, { "Space", 'S' } });

            DummyUnit.ArrayCharacterActive = new Character[1] { DummyCharacter };
            DummyUnit.ListAttack.Add(DummyAttack);
            DummyUnit.AttackIndex = 0;
            DummyUnit.UnitStat.DicTerrainValue.Add("Air", 1);
            DummyUnit.UnitStat.DicTerrainValue.Add("Land", 1);
            DummyUnit.UnitStat.DicTerrainValue.Add("Sea", 1);
            DummyUnit.UnitStat.DicTerrainValue.Add("Space", 1);

            return DummyUnit;
        }
    }
}
