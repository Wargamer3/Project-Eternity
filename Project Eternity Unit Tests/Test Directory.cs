using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class TestDirectory
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            // vstest keeps the process in bin\test\Debug. Plugin scans use the working directory.
            string TestDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string Configuration = new DirectoryInfo(TestDirectory).Name;
            string GameBin = Path.GetFullPath(Path.Combine(
                TestDirectory,
                "..", "..", "..", "..",
                "Project Eternity",
                "Project Eternity",
                "bin",
                Configuration));

            if (!Directory.Exists(Path.Combine(GameBin, "Units")))
            {
                throw new DirectoryNotFoundException("Unit tests need the game bin at " + GameBin);
            }

            Directory.SetCurrentDirectory(GameBin);
        }
    }
}
