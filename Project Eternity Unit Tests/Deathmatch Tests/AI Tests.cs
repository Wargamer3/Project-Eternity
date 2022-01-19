﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using ProjectEternity.AI.DeathmatchMapScreen;
using ProjectEternity.Core.AI;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units;

namespace ProjectEternity.UnitTests
{
    public partial class DeathmatchTests
    {
        [TestMethod]
        public void TestGetAttacksOrderedByPower()
        {
            DeathmatchScriptHolder.GetAttacksOrderedByPower Test = new DeathmatchScriptHolder.GetAttacksOrderedByPower();
            Squad DummySquad = CreateDummySquad();

            Attack DummyAttack = new Attack("Dummy Attack", string.Empty, 0, "8000", 0, 5, WeaponPrimaryProperty.None,
                WeaponSecondaryProperty.None, 10, 0, 6, 1, 100, "Laser",
                new Dictionary<string, char>() { { "Air", 'S' }, { "Land", 'S' }, { "Sea", 'S' }, { "Space", 'S' } });
            DummyAttack.PostMovementLevel = 1;
            DummySquad.CurrentLeader.ListAttack.Add(DummyAttack);

            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            List<object> ListAttack = (List<object>)Test.GetContent();
            Assert.AreEqual(2, ListAttack.Count);
            Assert.AreEqual(10000, ((Attack)ListAttack[0]).GetPower(DummySquad.CurrentLeader, null));

            DummySquad.CurrentLeader.ListAttack[0].PowerFormula = "1000";
            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            ListAttack = (List<object>)Test.GetContent();
            Assert.AreEqual(2, ListAttack.Count);
            Assert.AreEqual(8000, ((Attack)ListAttack[0]).GetPower(DummySquad.CurrentLeader, null));
        }

        [TestMethod]
        public void TestGetEnemies()
        {
            DeathmatchScriptHolder.GetEnemies Test = new DeathmatchScriptHolder.GetEnemies();
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(7, 8, 0), 0);
            
            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            List<object> ListEnemies = (List<object>)Test.GetContent();
            Assert.AreEqual(1, ListEnemies.Count);
        }

        [TestMethod]
        public void TestGetTargetsFail()
        {
            DeathmatchScriptHolder.GetTargets Test = new DeathmatchScriptHolder.GetTargets();
            CoreScriptHolder.SetVariable Attack = new CoreScriptHolder.SetVariable();
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(7, 8, 0), 0);
            bool IsCompleted;
            List<object> ListResult;
            Attack.Evaluate(null, DummySquad.CurrentLeader.ListAttack[0], out IsCompleted, out ListResult);
            Test.ArrayReferences[0].ReferencedScript = Attack;

            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            List<object> ListEnemies = (List<object>)Test.GetContent();
            Assert.AreEqual(0, ListEnemies.Count);
        }

        [TestMethod]
        public void TestGetTargetsSuccess()
        {
            DeathmatchScriptHolder.GetTargets Test = new DeathmatchScriptHolder.GetTargets();
            CoreScriptHolder.SetVariable Attack = new CoreScriptHolder.SetVariable();
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(3, 8, 0), 0);
            bool IsCompleted;
            List<object> ListResult;
            Attack.Evaluate(null, DummySquad.CurrentLeader.ListAttack[0], out IsCompleted, out ListResult);
            Test.ArrayReferences[0].ReferencedScript = Attack;

            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            List<object> ListEnemies = (List<object>)Test.GetContent();
            Assert.AreEqual(1, ListEnemies.Count);
        }

        [TestMethod]
        public void TestGetTargetsMoveFail()
        {
            DeathmatchScriptHolder.GetTargets Test = new DeathmatchScriptHolder.GetTargets();
            Test.AddMovementToRange = true;
            CoreScriptHolder.SetVariable Attack = new CoreScriptHolder.SetVariable();
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(18, 8, 0), 0);
            bool IsCompleted;
            List<object> ListResult;
            Attack.Evaluate(null, DummySquad.CurrentLeader.ListAttack[0], out IsCompleted, out ListResult);
            Test.ArrayReferences[0].ReferencedScript = Attack;

            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            List<object> ListEnemies = (List<object>)Test.GetContent();
            Assert.AreEqual(0, ListEnemies.Count);
        }

        [TestMethod]
        public void TestGetTargetsMoveSuccess()
        {
            DeathmatchScriptHolder.GetTargets Test = new DeathmatchScriptHolder.GetTargets();
            Test.AddMovementToRange = true;
            CoreScriptHolder.SetVariable Attack = new CoreScriptHolder.SetVariable();
            Squad DummySquad = CreateDummySquad();
            Squad EnemySquad = CreateDummySquad();
            DummyMap.SpawnSquad(0, DummySquad, 1, new Vector3(3, 5, 0), 0);
            DummyMap.SpawnSquad(1, EnemySquad, 2, new Vector3(8, 8, 0), 0);
            bool IsCompleted;
            List<object> ListResult;
            Attack.Evaluate(null, DummySquad.CurrentLeader.ListAttack[0], out IsCompleted, out ListResult);
            Test.ArrayReferences[0].ReferencedScript = Attack;

            Test.Info = new GameScreens.DeathmatchMapScreen.DeathmatchAIInfo(DummyMap, DummySquad);
            List<object> ListEnemies = (List<object>)Test.GetContent();
            Assert.AreEqual(1, ListEnemies.Count);
        }
    }
}
