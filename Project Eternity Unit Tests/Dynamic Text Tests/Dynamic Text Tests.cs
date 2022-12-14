using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Item;

namespace ProjectEternity.UnitTests.DynamicTextTests
{
    [TestClass]
    public class DynamicTextTests
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
        public void TestBasicText()
        {
            string TestText = "Testing the image parser {{Icon:fire}}{{Icon:water}}";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new RegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new IconProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new DefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.ParseText(TestText);

            Assert.AreEqual(3, DynamicTextTest.ListTextSection.Count);

            DynamicTextPart FirstImage = DynamicTextTest.ListTextSection[1];
            Assert.AreEqual(250, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);

            DynamicTextPart SecondImage = DynamicTextTest.ListTextSection[2];
            Assert.AreEqual(280, SecondImage.Position.X);
            Assert.AreEqual(0, SecondImage.Position.Y);
        }

        [TestMethod]
        public void TestSizeLimit()
        {
            string TestText = "1234567890{{Icon:fire}}{{Icon:water}}";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 100;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new RegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new IconProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new DefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.ParseText(TestText);

            Assert.AreEqual(3, DynamicTextTest.ListTextSection.Count);

            DynamicTextPart FirstImage = DynamicTextTest.ListTextSection[1];
            Assert.AreEqual(100, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);

            DynamicTextPart SecondImage = DynamicTextTest.ListTextSection[2];
            Assert.AreEqual(30, SecondImage.Position.X);
            Assert.AreEqual(DynamicTextTest.LineHeight, SecondImage.Position.Y);
        }

        [TestMethod]
        public void TestTextLeftColumn()
        {
            string TestText = "{{Text:{Offset:0}{MaxWidth:100}Testing the column parser}} Other text";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new RegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new DefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.ParseText(TestText);

            Assert.AreEqual(2, DynamicTextTest.ListTextSection.Count);

            DynamicTextPart FirstImage = DynamicTextTest.ListTextSection[1];
            Assert.AreEqual(100, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);
        }

        [TestMethod]
        public void TestTextRightColumn()
        {
            string TestText = "{{Text:{Offset:200}{MaxWidth:100}Testing the column parser}}Other text";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new RegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new DefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.ParseText(TestText);

            Assert.AreEqual(2, DynamicTextTest.ListTextSection.Count);

            DynamicTextPart FirstImage = DynamicTextTest.ListTextSection[1];
            Assert.AreEqual(0, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);
        }
    }
}
