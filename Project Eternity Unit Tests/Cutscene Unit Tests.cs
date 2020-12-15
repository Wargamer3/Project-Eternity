using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class CutsceneTests
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
        public void TestReadFromTextCounter()
        {
            Core.Scripts.ScriptingScriptHolder.ScriptReadFromText ReadFromText = new Core.Scripts.ScriptingScriptHolder.ScriptReadFromText();
            ReadFromText.Text = "Counter, 11, 12, 13";
            ReadFromText.ExecuteTrigger(0);
        }

        [TestMethod]
        public void TestReadFromTextUnit()
        {
            Core.Scripts.ScriptingScriptHolder.ScriptReadFromText ReadFromText = new Core.Scripts.ScriptingScriptHolder.ScriptReadFromText();
            ReadFromText.Text = "Unit, Normal/Getter Robo/Saki, 0, 0, 4, Getter Robo/Mechasaurus, True, 1";
            ReadFromText.ExecuteTrigger(0);
        }

        [TestMethod]
        public void TestReadFromTextSpawnUnit()
        {
            Core.Scripts.ScriptingScriptHolder.ScriptReadFromText ReadFromText = new Core.Scripts.ScriptingScriptHolder.ScriptReadFromText();
            ReadFromText.Text = "Spawn Unit, 1, 10, 10, 0, False, True, SRWE Enemy AI, Always Counterattack, Seconds, 1, , 15, ";
            ReadFromText.ExecuteTrigger(0);
        }

        [TestMethod]
        public void TestReadFromTextMultipleSpawnUnit()
        {
            Core.Scripts.ScriptingScriptHolder.ScriptReadFromText ReadFromText = new Core.Scripts.ScriptingScriptHolder.ScriptReadFromText();
            ReadFromText.Text = 
@"Spawn Unit, 2, 10, 26, 1, False, True, SRWE Enemy AI, Always Counterattack,  Seconds, 1, Spawn_strip4, 15, Confirm, Timer Ended
Spawn Unit, 5, 7, 25, 1, False, True, SRWE Enemy AI, Always Counterattack, Seconds, 1, Spawn_strip4, 15, Confirm, Timer Ended
Spawn Unit, 3, 16, 21, 1, False, True, SRWE Enemy AI, Always Counterattack, Seconds, 1, Spawn_strip4, 15, Confirm, Timer Ended";
            ReadFromText.ExecuteTrigger(0);
        }

        [TestMethod]
        public void TestReadFromTextSpawnUnitWithConter()
        {
            Core.Scripts.ScriptingScriptHolder.ScriptReadFromText ReadFromText = new Core.Scripts.ScriptingScriptHolder.ScriptReadFromText();
            ReadFromText.Text =
@"Counter, 11, 12, 13, Counter Updated
Unit, Normal/Getter-1, 0, 0, 1, Bad guy,Brocken,Finlander,Gregor, True, 3
Spawn Squad, 1, 0, 0, 5, 5, 0, False, True, SRWE, Smart Counterattack, Seconds, 1, Spawn_strip4, 15, Spawn";
            ReadFromText.ExecuteTrigger(0);
        }
    }
}
