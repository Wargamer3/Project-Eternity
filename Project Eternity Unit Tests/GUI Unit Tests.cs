using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.UnitNormalEditor;

namespace ProjectEternity.UnitTests
{
    [TestClass]
    public class GUIUnitTests
    {
        private static GUI.GUI TestGui;
        private static string RessourcePath;
        private static string ItemExtention;

        private static string NewFolderName;
        private static string NewFolderFilePath;

        private static string NewItemName;
        private static string NewItemFilePath;
        private static string NewItemInFolderLogicName;
        private static string NewItemInFolderFilePath;

        private static string RenamedItemName;
        private static string RenamedItemPath;
        private static string RenamedItemInFolderLogicName;
        private static string RenamedItemInFolderFilePath;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            RessourcePath = "Units/Normal/";
            ItemExtention = ".peu";

            NewFolderName = "New Folder";
            NewFolderFilePath = "Content/" + RessourcePath + NewFolderName + "/";

            NewItemName = "New Item";
            NewItemFilePath = "Content/" + RessourcePath + NewItemName + ItemExtention;
            NewItemInFolderLogicName = NewFolderName + "/" + NewItemName;
            NewItemInFolderFilePath = NewFolderFilePath + NewItemName + ItemExtention;

            RenamedItemName = "Renamed Item";
            RenamedItemPath = "Content/" + RessourcePath + RenamedItemName + ItemExtention;
            RenamedItemInFolderLogicName = NewFolderName + "/" + RenamedItemName;
            RenamedItemInFolderFilePath = NewFolderFilePath + RenamedItemName + ItemExtention;
        }

        [TestInitialize()]
        public void Initialize()
        {
            TestGui = new GUI.GUI(false);
            TestGui.tvItems.Nodes.Clear();

            if (File.Exists(NewItemFilePath))
            {
                File.Delete(NewItemFilePath);
            }
            if (File.Exists(RenamedItemPath))
            {
                File.Delete(RenamedItemPath);
            }
            if (File.Exists(NewItemInFolderFilePath))
            {
                File.Delete(NewItemInFolderFilePath);
            }
            if (File.Exists(RenamedItemInFolderFilePath))
            {
                File.Delete(RenamedItemInFolderFilePath);
            }
            if (Directory.Exists(NewFolderFilePath))
            {
                Directory.Delete(NewFolderFilePath);
            }
            if (!Directory.Exists("Content/" + RessourcePath))
            {
                Directory.CreateDirectory("Content/" + RessourcePath);
            }

            TestGui.AddEditor(new EditorInfo(new string[] { BaseEditor.GUIRootPathUnitsNormal, BaseEditor.GUIRootPathUnits }, RessourcePath, new string[] { ItemExtention },
                typeof(UnitNormalEditor)));

            TestGui.InitGUI();
        }

        [TestMethod]
        public void TestGUICreateItemFromRoot()
        {
            TreeNode UnitsNode = TestGui.tvItems.Nodes.Find("Units", false)[0];
            TreeNode NormalNode = UnitsNode.Nodes.Find("Normal", false)[0];
            TreeNode NewNode = TestGui.CreateNewItem(NormalNode); // Create New Item
            Assert.IsTrue(File.Exists(NewItemFilePath));

            TreeNode NewItemNode = NormalNode.Nodes.Find(NewItemName, false)[0];

            Assert.AreEqual(NewNode, NewItemNode);
            Assert.AreEqual(NewItemName, NewItemNode.Text);

            List<ItemContainer> ListContainerUnits = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnits).ToList();
            List<ItemContainer> ListContainerUnitsNormal = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnitsNormal).ToList();

            Assert.AreEqual(1, ListContainerUnits.Count);
            Assert.AreEqual(1, ListContainerUnitsNormal.Count);

            ItemContainer ContainerUnits = ListContainerUnits[0];
            ItemContainer ContainerUnitsNormal = ListContainerUnitsNormal[0];

            Assert.IsTrue(ContainerUnits.ListItem.ContainsKey(NewItemName));
            Assert.IsTrue(ContainerUnitsNormal.ListItem.ContainsKey(NewItemName));

            Assert.AreEqual(NewItemFilePath, ContainerUnits.ListItem[NewItemName]);
            Assert.AreEqual(NewItemFilePath, ContainerUnitsNormal.ListItem[NewItemName]);
        }

        [TestMethod]
        public void TestGUIRenameFromRoot()
        {
            TreeNode UnitsNode = TestGui.tvItems.Nodes.Find("Units", false)[0];
            TreeNode NormalNode = UnitsNode.Nodes.Find("Normal", false)[0];
            TreeNode NewItemNode = TestGui.CreateNewItem(NormalNode); // Create New Item

            ItemContainer ContainerUnits = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnits).First();
            ItemContainer ContainerUnitsNormal = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnitsNormal).First();

            TestGui.RenameNode(NewItemNode, RenamedItemName);

            Assert.IsTrue(File.Exists(RenamedItemPath));

            Assert.IsTrue(ContainerUnits.ListItem.ContainsKey(RenamedItemName));
            Assert.IsTrue(ContainerUnitsNormal.ListItem.ContainsKey(RenamedItemName));

            Assert.AreEqual(RenamedItemPath, ContainerUnits.ListItem[RenamedItemName]);
            Assert.AreEqual(RenamedItemPath, ContainerUnitsNormal.ListItem[RenamedItemName]);
        }

        [TestMethod]
        public void TestGUICreateFolderFromRoot()
        {
            TreeNode UnitsNode = TestGui.tvItems.Nodes.Find("Units", false)[0];
            TreeNode NormalNode = UnitsNode.Nodes.Find("Normal", false)[0];
            TreeNode NewNode = TestGui.CreateNewFolder(NormalNode); // Create New Folder
            Assert.IsTrue(Directory.Exists(NewFolderFilePath));

            TreeNode NewItemNode = NormalNode.Nodes.Find(NewFolderName, false)[0];

            Assert.AreEqual(NewNode, NewItemNode);
            Assert.AreEqual(NewFolderName, NewItemNode.Text);
        }

        [TestMethod]
        public void TestGUICreateNewItemFromFolder()
        {
            TreeNode UnitsNode = TestGui.tvItems.Nodes.Find("Units", false)[0];
            TreeNode NormalNode = UnitsNode.Nodes.Find("Normal", false)[0];
            TreeNode NewFolderNode = TestGui.CreateNewFolder(NormalNode); // Create New Folder

            TreeNode NewNode = TestGui.CreateNewItem(NewFolderNode); // Create New Item
            Assert.IsTrue(File.Exists(NewItemInFolderFilePath));

            TreeNode NewItemNode = NewFolderNode.Nodes.Find(NewItemName, false)[0];

            Assert.AreEqual(NewNode, NewItemNode);
            Assert.AreEqual(NewItemName, NewItemNode.Text);

            List<ItemContainer> ListContainerUnits = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnits).ToList();
            List<ItemContainer> ListContainerUnitsNormal = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnitsNormal).ToList();

            Assert.AreEqual(1, ListContainerUnits.Count);
            Assert.AreEqual(1, ListContainerUnitsNormal.Count);

            ItemContainer ContainerUnits = ListContainerUnits[0];
            ItemContainer ContainerUnitsNormal = ListContainerUnitsNormal[0];

            Assert.IsTrue(ContainerUnits.ListItem.ContainsKey(NewItemInFolderLogicName));
            Assert.IsTrue(ContainerUnitsNormal.ListItem.ContainsKey(NewItemInFolderLogicName));

            Assert.AreEqual(NewItemInFolderFilePath, ContainerUnits.ListItem[NewItemInFolderLogicName]);
            Assert.AreEqual(NewItemInFolderFilePath, ContainerUnitsNormal.ListItem[NewItemInFolderLogicName]);
        }

        [TestMethod]
        public void TestGUIRenameFromFolder()
        {
            TreeNode UnitsNode = TestGui.tvItems.Nodes.Find("Units", false)[0];
            TreeNode NormalNode = UnitsNode.Nodes.Find("Normal", false)[0];
            TreeNode NewFolderNode = TestGui.CreateNewFolder(NormalNode); // Create New Folder

            TreeNode NewNode = TestGui.CreateNewItem(NewFolderNode); // Create New Item

            TreeNode NewItemNode = NewFolderNode.Nodes.Find(NewItemName, false)[0];

            List<ItemContainer> ListContainerUnits = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnits).ToList();
            List<ItemContainer> ListContainerUnitsNormal = BaseEditor.GetItemsByRoot(BaseEditor.GUIRootPathUnitsNormal).ToList();
            ItemContainer ContainerUnits = ListContainerUnits[0];
            ItemContainer ContainerUnitsNormal = ListContainerUnitsNormal[0];

            TestGui.RenameNode(NewItemNode, RenamedItemName);

            Assert.IsTrue(File.Exists(RenamedItemInFolderFilePath));

            Assert.IsTrue(ContainerUnits.ListItem.ContainsKey(RenamedItemInFolderLogicName));
            Assert.IsTrue(ContainerUnitsNormal.ListItem.ContainsKey(RenamedItemInFolderLogicName));

            Assert.AreEqual(RenamedItemInFolderFilePath, ContainerUnits.ListItem[RenamedItemInFolderLogicName]);
            Assert.AreEqual(RenamedItemInFolderFilePath, ContainerUnitsNormal.ListItem[RenamedItemInFolderLogicName]);
        }
    }
}
