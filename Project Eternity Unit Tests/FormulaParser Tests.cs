using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.GameScreens.DeathmatchMapScreen;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class FormulaParaserTests
    {
        private static DeathmatchFormulaParser Parser;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Parser = new DeathmatchFormulaParser(null);
        }

        [TestInitialize()]
        public void Initialize()
        {
        }

        [TestMethod]
        public void TestRandom()
        {
            Assert.AreNotEqual("70", Parser.Evaluate("Random.70"));
            Assert.AreNotEqual("70", Parser.Evaluate("Random.60 + 10"));
        }
    }
}
