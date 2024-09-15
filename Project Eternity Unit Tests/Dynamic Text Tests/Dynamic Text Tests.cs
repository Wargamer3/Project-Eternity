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
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new ImagelessIconProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            DynamicTextPart FirstImage = DynamicTextTest.Root.ListSubTextSection[1];
            Assert.AreEqual(250, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);

            DynamicTextPart SecondImage = DynamicTextTest.Root.ListSubTextSection[2];
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
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new ImagelessIconProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            DynamicTextPart FirstImage = DynamicTextTest.Root.ListSubTextSection[1];
            Assert.AreEqual(100, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);

            DynamicTextPart SecondImage = DynamicTextTest.Root.ListSubTextSection[2];
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
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new ImagelessIconProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            DynamicTextPart FirstImage = DynamicTextTest.Root.ListSubTextSection[1];
            Assert.AreEqual(100, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);
        }

        [TestMethod]
        public void TestTextLeftColumnWithImage()
        {
            string TestText = "{{Text:{Font:16}{MaxWidth:100}super long text {{Icon:Fire}} More super long text}} other text{{Icon:Fire}}";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new ImagelessIconProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            Assert.AreEqual(3, DynamicTextTest.Root.ListSubTextSection.Count);

            DynamicTextPart FirstText = DynamicTextTest.Root.ListSubTextSection[0];
            Assert.AreEqual(0, FirstText.Position.X);
            Assert.AreEqual(0, FirstText.Position.Y);
            Assert.AreEqual(3, FirstText.ListSubTextSection.Count);

            DynamicTextPart FirstSubText = DynamicTextTest.Root.ListSubTextSection[0].ListSubTextSection[0];
            Assert.AreEqual(0, FirstSubText.Position.X);
            Assert.AreEqual(0, FirstSubText.Position.Y);

            DynamicTextPart SecondSubText = DynamicTextTest.Root.ListSubTextSection[0].ListSubTextSection[2];
            Assert.AreEqual(94, SecondSubText.Position.X);
            Assert.AreEqual(40, SecondSubText.Position.Y);

            DynamicTextPart SecondText = DynamicTextTest.Root.ListSubTextSection[1];
            Assert.AreEqual(" other text", SecondText.OriginalText);
            Assert.AreEqual(16, FirstText.ListSubTextSection[0].OriginalText.Length);
            Assert.AreEqual(21, FirstText.ListSubTextSection[2].OriginalText.Length);
            Assert.AreEqual(FirstText.ListSubTextSection[0].UpdatePosition().X, FirstText.ListSubTextSection[1].Position.X);
            Assert.AreEqual(FirstText.ListSubTextSection[1].Position.X + ImagelessIconPart.ImageSizeDefault, FirstText.ListSubTextSection[2].Position.X);

            Assert.AreEqual(100, SecondText.Position.X);
            Assert.AreEqual(0, SecondText.Position.Y);
        }

        [TestMethod]
        public void TestTextRightColumn()
        {
            string TestText = "{{Text:{Offset:200}{MaxWidth:100}Testing the column parser}}Other text";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            Assert.AreEqual(2, DynamicTextTest.Root.ListSubTextSection.Count);

            DynamicTextPart FirstImage = DynamicTextTest.Root.ListSubTextSection[1];
            Assert.AreEqual(0, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);
        }

        [TestMethod]
        public void TestTextRainbow()
        {
            string TestText = "{{Text:{Font:16}{MaxWidth:100}{Rainbow}{Wave}super long text {{Icon:Fire}}More super long text}}other text{{Icon:Fire}} with other icon";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            DynamicTextPart FirstImage = DynamicTextTest.Root.ListSubTextSection[0];
            Assert.AreEqual(0, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);
            Assert.AreEqual(4, FirstImage.DicSubTag.Count);
        }

        [TestMethod]
        public void TestTextRainbowPlayerName()
        {
            string TestText = "{{Text:{Font:16}{MaxWidth:100}{Rainbow}{Wave}{{Player:Self}}{{Icon:Fire}}More super long text}}{{Player:Ally}}{{Icon:Fire}} with other icon";
            DynamicText DynamicTextTest = new DynamicText();
            DynamicTextTest.TextMaxWidthInPixel = 300;
            DynamicTextTest.LineHeight = 20;
            DynamicTextTest.ListProcessor.Add(new PlayerNameTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessRegularTextProcessor(DynamicTextTest));
            DynamicTextTest.ListProcessor.Add(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.SetDefaultProcessor(new FontlessDefaultTextProcessor(DynamicTextTest));
            DynamicTextTest.Load(null);
            DynamicTextTest.ParseText(TestText);

            DynamicTextPart FirstImage = DynamicTextTest.Root.ListSubTextSection[0];
            Assert.AreEqual(0, FirstImage.Position.X);
            Assert.AreEqual(0, FirstImage.Position.Y);
            Assert.AreEqual(4, FirstImage.DicSubTag.Count);
            Assert.AreEqual("Success", FirstImage.ListSubTextSection[0].OriginalText);
        }
    }
}
