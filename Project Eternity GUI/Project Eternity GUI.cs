using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Units;

namespace ProjectEternity.GUI
{
    public partial class GUI : Form
    {
        private Lookup<string, ItemContainer> ItemsByLogicPath;
        private Dictionary<string, List<ItemContainer>> ListEditor;
        private TreeNode mySelectedNode;
        private string OldItemName;
        private GUIGraphicDevice RootGraphicDevice;
        private string PathSeprator = "/";

        public GUI(bool LoadContent = true)
        {
            if (LoadContent)
            {
                SplashScreen.ShowSplashScreen();
                SplashScreen.SetStatus("Initializing Roslyn");
            }

            InitializeComponent();

            if (LoadContent)
            {
                SplashScreen.SetStatus("Initializing Graphics");
                RootGraphicDevice = new GUIGraphicDevice();
                GameScreens.GameScreen.LoadHelpers(RootGraphicDevice.Content);
                TextHelper.LoadHelpers(RootGraphicDevice.Content);
                GameScreens.GameScreen.GraphicsDevice = RootGraphicDevice.GraphicsDevice;
                UnitAndTerrainValues.Default.Load();

                foreach (KeyValuePair<string, Unit> ActiveUnitType in Unit.LoadAllUnits())
                {
                    Unit.DicDefaultUnitType.Add(ActiveUnitType.Key, ActiveUnitType.Value);
                }
                foreach (KeyValuePair<string, BaseEffect> ActiveEffect in BaseEffect.LoadAllEffects())
                {
                    BaseEffect.DicDefaultEffect.Add(ActiveEffect.Key, ActiveEffect.Value);
                }
                foreach (KeyValuePair<string, BaseSkillRequirement> ActiveRequirement in BaseSkillRequirement.LoadAllRequirements())
                {
                    BaseSkillRequirement.DicDefaultRequirement.Add(ActiveRequirement.Key, ActiveRequirement.Value);
                }
                foreach (KeyValuePair<string, AutomaticSkillTargetType> ActiveAutomaticSkill in AutomaticSkillTargetType.LoadAllTargetTypes())
                {
                    AutomaticSkillTargetType.DicDefaultTarget.Add(ActiveAutomaticSkill.Key, ActiveAutomaticSkill.Value);
                }
                foreach (KeyValuePair<string, ManualSkillTarget> ActiveManualSkill in ManualSkillTarget.LoadAllTargetTypes())
                {
                    ManualSkillTarget.DicDefaultTarget.Add(ActiveManualSkill.Key, ActiveManualSkill.Value);
                }
            }

            SplashScreen.SetStatus("Initializing Editors");
            BaseEditor.GetItemsByRoot = GetItemsByRoot;
            BaseEditor.GetItemByKey = GetItemValueByKey;
            this.SetStyle(ControlStyles.StandardDoubleClick, false);//Disable right double click.

            //Place holder to store values for the lookup.
            ListEditor = new Dictionary<string, List<ItemContainer>>();

            if (Directory.Exists("Editors") && LoadContent)
            {
                tvItems.Nodes.Clear();
                //Look for every dll in the Editors folder.
                String[] Files = Directory.GetFiles("Editors", "*.dll");
                Type EditorType;
                bool EditorIsBaseEditor = true;
                for (int F = 0; F < Files.Count(); F++)
                {//Load a dll.
                    Assembly ass = Assembly.LoadFile(Path.GetFullPath(Files[F]));
                    Type[] types = null;
                    //Get every classes in it.
                    try
                    {
                        types = ass.GetTypes();
                    }
                    catch (Exception ex)
                    {
                        if (ex is ReflectionTypeLoadException)
                        {
                            var typeLoadException = ex as ReflectionTypeLoadException;
                            var loaderExceptions = typeLoadException.LoaderExceptions;

                            MessageBox.Show(ex.Message);
                        }
                        continue;
                    }
                    for (int t = 0; t < types.Count(); t++)
                    {
                        //Look if the class inherit from BaseEditor somewhere.
                        EditorType = types[t];
                        EditorIsBaseEditor = false;
                        while (EditorType != null && !EditorIsBaseEditor)
                        {
                            EditorType = EditorType.BaseType;
                            if (EditorType == typeof(BaseEditor))
                                EditorIsBaseEditor = true;
                        }
                        //If this class is from BaseEditor, load it.
                        if (EditorIsBaseEditor)
                        {
                            BaseEditor instance = null;
                            try
                            {
                                instance = Activator.CreateInstance(types[t]) as BaseEditor;
                                SplashScreen.SetStatus($"Initializing Editors ({instance.Name})");
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.ToString());
                                continue;
                            }

                            //Get the BaseEditor's EditorInfo.
                            EditorInfo[] Info = instance.LoadEditors();
                            if (Info == null)
                                continue;

                            //Loop through the Item paths used to filter the items(if there is multiple items type by editor).
                            for (int P = 0; P < Info.Length; P++)
                            {
                                AddEditor(Info[P]);
                            }
                        }
                    }
                }

                InitGUI();
            }

            if (LoadContent)
            {
                SplashScreen.CloseForm();
            }
        }

        public void InitGUI()
        {
            //Convert the place editor back to the Lookup.
            ItemsByLogicPath = (Lookup<string, ItemContainer>)ListEditor.SelectMany(pair => pair.Value,
                                   (pair, Value) => new { pair.Key, Value })
                       .ToLookup(pair => pair.Key, pair => pair.Value);
        }

        public void AddEditor(EditorInfo Info)
        {//Split the path to get each word.
            string[] ItemSubPaths = Info.ItemContainer.ContainerGUIPath.Split(new string[] { PathSeprator }, StringSplitOptions.RemoveEmptyEntries);
            int SubPathIndex = 0;
            string CurrentPath = "";
            TreeNode CurrentNode;
            TreeNode LastNode;
            TreeNodeCollection LastNodeCollection;

            #region Load editor

            //Search for the first filter.
            LastNode = SearchNodeByPath(tvItems.Nodes, ItemSubPaths[0]);
            if (LastNode == null)//No node found, start from the base of the TreeView.
                LastNodeCollection = tvItems.Nodes;
            else
            {
                LastNodeCollection = LastNode.Nodes;//Filter found, start from its node.
                CurrentPath += ItemSubPaths[0] + PathSeprator;//Skip the first step of the loop as it's already been processed
                ++SubPathIndex;
            }
            while (SubPathIndex < ItemSubPaths.Length)
            {//Loop through every filter word
                CurrentPath += ItemSubPaths[SubPathIndex];
                CurrentNode = SearchNodeByPath(tvItems.Nodes, CurrentPath);

                if (CurrentNode == null)
                {//Add a node for the missing filter.
                    LastNode = LastNodeCollection.Add(ItemSubPaths[SubPathIndex]);
                    LastNode.Tag = null;//Make sure the Tag is null.
                    LastNode.Name = ItemSubPaths[SubPathIndex];
                    LastNodeCollection = LastNode.Nodes;
                }
                else
                {
                    LastNode = CurrentNode;
                    LastNodeCollection = LastNode.Nodes;
                }

                CurrentPath += PathSeprator;
                ++SubPathIndex;
            }
            //Asign the EditorInfo the the final container node.
            LastNode.Tag = Info;

            #endregion

            #region Load items

            TreeNode NewItemNode;
            TreeNode ParentItemNode;
            //Add the items to the node.
            foreach (KeyValuePair<string, string> Item in Info.ItemContainer.ListItem)
            {
                //Don't add Items with no text.
                if (String.IsNullOrWhiteSpace(Item.Value))
                    continue;

                ParentItemNode = LastNode;
                LastNodeCollection = ParentItemNode.Nodes;
                CurrentPath = Info.ItemContainer.ContainerGUIPath;
                ItemSubPaths = Item.Key.Split(new string[] { PathSeprator }, StringSplitOptions.None);
                SubPathIndex = 0;

                if (ItemSubPaths.Length > 1)
                {
                    while (SubPathIndex + 1 < ItemSubPaths.Length)
                    {//Loop through every filter word
                        CurrentPath += ItemSubPaths[SubPathIndex];
                        CurrentNode = SearchFolderNodeByPath(tvItems.Nodes, CurrentPath);

                        if (CurrentNode == null)
                        {//Add a node for the missing filter.
                            ParentItemNode = LastNodeCollection.Add(ItemSubPaths[SubPathIndex]);
                            ParentItemNode.Tag = null;//Make sure the Tag is null.
                            ParentItemNode.Name = ItemSubPaths[SubPathIndex];
                            LastNodeCollection = ParentItemNode.Nodes;

                            //Asign the EditorInfo the the final container node.
                            EditorInfo NewEditor = new EditorInfo(Info);
                            for (int i = 0; i < ItemSubPaths.Length - 1; i++)
                                NewEditor.FolderPath += ItemSubPaths[i] + PathSeprator;

                            NewEditor.IsFolder = true;

                            ParentItemNode.Tag = NewEditor;
                        }
                        else
                        {
                            ParentItemNode = CurrentNode;
                            LastNodeCollection = ParentItemNode.Nodes;
                        }
                        CurrentPath += PathSeprator;
                        ++SubPathIndex;
                    }
                }

                NewItemNode = new TreeNode(ItemSubPaths.Last());
                NewItemNode.Name = NewItemNode.Text;

                ParentItemNode.Nodes.Add(NewItemNode);
            }

            #endregion

            //Add the root path filters.
            for (int R = 0; R < Info.ArrayLogicPath.Count(); R++)
            {
                if (!ListEditor.ContainsKey(Info.ArrayLogicPath[R]))
                {
                    ListEditor.Add(Info.ArrayLogicPath[R], new List<ItemContainer>());
                }
                ListEditor[Info.ArrayLogicPath[R]].Add(new ItemContainer(Info.ItemContainer));
            }
        }

        private IEnumerable<ItemContainer> GetItemsByRoot(string RootPath)
        {
            if (ItemsByLogicPath.Contains(RootPath))
                return ItemsByLogicPath[RootPath];
            return null;
        }

        private ItemContainer GetItemContainerByPath(string RootPath, string GUIPath)
        {
            if (ItemsByLogicPath.Contains(RootPath))
            {
                IEnumerable<ItemContainer> Items = GetItemsByRoot(RootPath);
                foreach (ItemContainer Container in Items)
                {
                    if (Container.ContainerGUIPath == GUIPath)
                        return Container;
                }
            }
            return new ItemContainer();
        }

        private TreeNode SearchNodeByPath(TreeNodeCollection SearchingNode, string SearchPath)
        {
            TreeNode ReturnNode = null;

            if (SearchPath != string.Empty)
            {
                for (int N = 0; N < SearchingNode.Count && ReturnNode == null; N++)
                {
                    if (SearchingNode[N].FullPath == SearchPath)
                    {
                        return SearchingNode[N];
                    }
                    else
                    {
                        ReturnNode = SearchNodeByPath(SearchingNode[N].Nodes, SearchPath);
                    }
                }
            }
            else
                MessageBox.Show("Enter Name");

            return ReturnNode;
        }

        private TreeNode SearchFolderNodeByPath(TreeNodeCollection SearchingNode, string SearchPath)
        {
            TreeNode ReturnNode = null;

            if (SearchPath != string.Empty)
            {
                for (int N = 0; N < SearchingNode.Count && ReturnNode == null; N++)
                {
                    if (SearchingNode[N].FullPath == SearchPath && SearchingNode[N].Nodes.Count > 0)
                    {
                        return SearchingNode[N];
                    }
                    else
                    {
                        ReturnNode = SearchFolderNodeByPath(SearchingNode[N].Nodes, SearchPath);
                    }
                }
            }
            else
                MessageBox.Show("Enter Name");

            return ReturnNode;
        }

        private void RenameFolderNode(TreeNode ActiveNode, string NewKey)
        {
            EditorInfo Info = (EditorInfo)ActiveNode.Tag;

            string OldPath = Info.ItemContainer.ContainerRootPath + Info.FolderPath;
            Info.FolderPath = ((EditorInfo)ActiveNode.Parent.Tag).FolderPath + NewKey + PathSeprator;

            Directory.Move(Path.GetFullPath(OldPath), Path.GetFullPath(Info.ItemContainer.ContainerRootPath + Info.FolderPath));

            ActiveNode.Tag = Info;

            for (int N = 0; N < ActiveNode.Nodes.Count; N++)
            {
                if (ActiveNode.Nodes[N].Tag is EditorInfo)
                {
                }
            }
        }
        
        private string GetItemPath(string RootPath, string ItemLogicName)
        {
            if (ItemsByLogicPath.Contains(RootPath))
            {
                foreach (ItemContainer Item in ItemsByLogicPath[RootPath])
                {
                    if (Item.ListItem.ContainsKey(ItemLogicName))
                        return Item.ListItem[ItemLogicName];
                }
            }
            return null;
        }

        private ItemInfo GetItemValueByKey(string RootPath, string Key)
        {
            if (ItemsByLogicPath.Contains(RootPath))
            {
                foreach (ItemContainer Item in ItemsByLogicPath[RootPath])
                {
                    if (Item.ListItem.ContainsKey(Key))
                        return new ItemInfo(Item.ListItem[Key], Key);
                }
            }
            return new ItemInfo();
        }

        #region Helper methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="ActiveNode"></param>
        /// <param name="NewName"></param>
        /// <returns>An error message if the rename failed, null if succesful</returns>
        public string RenameNode(TreeNode ActiveNode, string NewName)
        {
            string OldName = ActiveNode.Text;
            TreeNode EditorNode = ActiveNode.Parent;
            EditorInfo Editor = (EditorInfo)EditorNode.Tag;

            if (EditorNode == null)
            {
                return "Unable to find a corresponding editor to save the name modfication.";
            }

            if (string.IsNullOrEmpty(NewName) || EditorNode.Nodes.ContainsKey(NewName))
            {
                return "An error occured";
            }

            try
            {
                string OldPath = GetFilePathForItem(Editor, OldName);
                string OldLogicName = Editor.FolderPath + OldName;
                string NewPath = GetFilePathForItem(Editor, NewName);
                string NewLogicName = Editor.FolderPath + NewName;

                File.Move(OldPath, NewPath);

                ActiveNode.Name = NewName;

                foreach (string GUIPath in Editor.ArrayLogicPath)
                {
                    ItemContainer Container = GetItemContainerByPath(GUIPath, Editor.ItemContainer.ContainerGUIPath);

                    Container.ListItem.Add(NewLogicName, NewPath);
                    Container.ListItem.Remove(OldLogicName);
                }
            }
            catch (Exception E)
            {
                return E.Message;
            }

            return null;
        }

        public TreeNode CreateNewItem(TreeNode EditorNode)
        {
            if (EditorNode.Tag == null)
                EditorNode = EditorNode.Parent;

            EditorInfo Editor = (EditorInfo)EditorNode.Tag;

            string NewItemName = "New Item";
            if (EditorNode.Nodes.ContainsKey(NewItemName))
            {
                int SameItemCount = 2;
                while (EditorNode.Nodes.ContainsKey(NewItemName + " (" + SameItemCount + ")"))
                    SameItemCount++;

                NewItemName += " (" + SameItemCount + ")";
            }

            string NewItemPath = GetFilePathForItem(Editor, NewItemName);
            TreeNode NewItem = new TreeNode(NewItemName);
            NewItem.Name = NewItem.Text;
            EditorNode.Nodes.Add(NewItem);

            foreach (string GUIPath in Editor.ArrayLogicPath)
            {
                ItemContainer Container = GetItemContainerByPath(GUIPath, Editor.ItemContainer.ContainerGUIPath);

                Container.ListItem.Add(Editor.FolderPath + NewItemName, NewItemPath);
            }

            //Force the editor to access the item and create the file.
            BaseEditor instance = Activator.CreateInstance(Editor.EditorType, NewItemPath, Editor.InitParams) as BaseEditor;

            return NewItem;
        }

        public TreeNode CloneItem(TreeNode EditorNode)
        {
            TreeNode ActiveNode = EditorNode;

            if (EditorNode.Tag == null)
                EditorNode = EditorNode.Parent;

            EditorInfo Editor = (EditorInfo)EditorNode.Tag;

            string NewItemName = ActiveNode.Text;
            if (EditorNode.Nodes.ContainsKey(NewItemName))
            {
                int SameItemCount = 1;
                while (EditorNode.Nodes.ContainsKey(NewItemName + " (" + SameItemCount + ")"))
                    SameItemCount++;

                NewItemName += " (" + SameItemCount + ")";
            }

            string NewItemPath = GetFilePathForItem(Editor, NewItemName);
            TreeNode NewItem = new TreeNode(NewItemName);
            NewItem.Name = NewItem.Text;
            EditorNode.Nodes.Add(NewItem);

            foreach (string GUIPath in Editor.ArrayLogicPath)
            {
                ItemContainer Container = GetItemContainerByPath(GUIPath, Editor.ItemContainer.ContainerGUIPath);

                Container.ListItem.Add(Editor.FolderPath + NewItemName, NewItemPath);
            }

            string FilePath = GetFilePathForItemNode(ActiveNode);
            File.Copy(FilePath, NewItemPath);

            return NewItem;
        }

        public TreeNode CreateNewFolder(TreeNode EditorNode)
        {
            if (EditorNode.Tag == null)
                EditorNode = EditorNode.Parent;

            string NewItemName = "New Folder";
            if (EditorNode.Nodes.ContainsKey(NewItemName))
            {
                int SameItemCount = 2;
                while (EditorNode.Nodes.ContainsKey(NewItemName + " (" + SameItemCount + ")"))
                    SameItemCount++;

                NewItemName += " (" + SameItemCount + ")";
            }

            EditorInfo Info = new EditorInfo((EditorInfo)EditorNode.Tag);
            Info.FolderPath += NewItemName + PathSeprator;
            Info.IsFolder = true;

            Directory.CreateDirectory(Info.ItemContainer.ContainerRootPath + Info.FolderPath);

            TreeNode NewItem = new TreeNode(NewItemName);
            NewItem.Tag = Info;
            NewItem.Name = NewItemName;
            EditorNode.Nodes.Add(NewItem);

            return NewItem;
        }

        public void DeleteNode(TreeNode ActiveNode)
        {
            // Item
            if (ActiveNode.Tag == null)
            {
                TreeNode EditorNode = ActiveNode.Parent;
                EditorInfo Editor = (EditorInfo)EditorNode.Tag;

                try
                {
                    string FilePath = GetFilePathForItemNode(ActiveNode);
                    File.Delete(FilePath);
                    EditorNode.Nodes.Remove(ActiveNode);
                    foreach (string GUIPath in Editor.ArrayLogicPath)
                    {
                        ItemContainer Container = GetItemContainerByPath(GUIPath, Editor.ItemContainer.ContainerGUIPath);

                        Container.ListItem.Remove(Editor.FolderPath + ActiveNode.Text);
                    }
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                }
            }
            // Folder
            else if (ActiveNode.Tag is EditorInfo)
            {
                foreach (TreeNode Node in ActiveNode.Nodes)
                {
                    DeleteNode(Node);
                }
            }
        }

        public string GetFilePathForItemNode(TreeNode Node)
        {
            EditorInfo Editor = (EditorInfo)Node.Parent.Tag;
            return GetFilePathForItem(Editor, Node.Text);
        }

        public string GetFilePathForItem(EditorInfo Editor, string ItemName)
        {
            return Editor.ItemContainer.ContainerRootPath + Editor.FolderPath + ItemName + Editor.ArrayFileExtention[0];
        }

        #endregion

        #region Events

        private void tvItems_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode NodeClicked = tvItems.GetNodeAt(e.X, e.Y);

            if (tvItems.SelectedNode == null || NodeClicked == null || NodeClicked != tvItems.SelectedNode)
                return;

            TreeNode ActiveNode = tvItems.SelectedNode;
            TreeNode EditorNode = ActiveNode.Parent;
            if (ActiveNode.Nodes.Count > 0 || EditorNode == null || EditorNode.Tag == null)
                return;
            EditorInfo Info = (EditorInfo)EditorNode.Tag;

            try
            {
                BaseEditor instance = Activator.CreateInstance(Info.EditorType, GetFilePathForItemNode(ActiveNode), Info.InitParams) as BaseEditor;
                instance.Show();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }

            if (EditorNode == null)
                MessageBox.Show("Unable to find a corresponding editor to open.");
        }

        private void tsmEdit_Click(object sender, EventArgs e)
        {
            if (tvItems.SelectedNode == null)
                return;

            TreeNode ActiveNode = tvItems.SelectedNode;
            TreeNode EditorNode = ActiveNode.Parent;
            if (ActiveNode.Nodes.Count > 0 || EditorNode == null || EditorNode.Tag == null)
                return;
            EditorInfo Info = (EditorInfo)EditorNode.Tag;

            try
            {
                BaseEditor instance = Activator.CreateInstance(Info.EditorType, GetFilePathForItemNode(ActiveNode), Info.InitParams) as BaseEditor;
                instance.Show();
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }

            if (EditorNode == null)
                MessageBox.Show("Unable to find a corresponding editor to open.");
        }

        private void tvItems_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            OldItemName = e.Node.Text;
        }

        private void tvItems_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            //e.Label = New Item Name or null if unchanged
            //e.Node.Text = Old Item Name.
            TreeNode ActiveNode = tvItems.SelectedNode;

            #region Item selection

            if (ActiveNode.Tag == null && e.Label != null)
            {
                string ErrorMessage = RenameNode(ActiveNode, e.Label);
                if (ErrorMessage != null)
                {
                    MessageBox.Show(ErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.CancelEdit = true;
                }
            }

            #endregion

            #region Folder selection

            else if (ActiveNode.Tag is EditorInfo)
            {
                if (String.IsNullOrEmpty(e.Label))
                {
                    e.CancelEdit = true;
                    return;
                }
                if (OldItemName != e.Label)
                {
                    RenameFolderNode(ActiveNode, e.Label);
                }
            }

            #endregion
        }

        private void tvItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mySelectedNode = tvItems.GetNodeAt(e.X, e.Y);
                if (mySelectedNode == null)
                {
                    tvItems.SelectedNode = null;
                    tvItems.LabelEdit = false;
                }
            }
            else
            {
                mySelectedNode = tvItems.GetNodeAt(e.X, e.Y);
                tvItems.SelectedNode = mySelectedNode;
            }
        }

        private void tvItems_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mySelectedNode != null && mySelectedNode.Tag == null)
                {
                    tvItems.LabelEdit = true;
                }
                else
                    tvItems.LabelEdit = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (mySelectedNode != null)
                {
                    if (mySelectedNode.Tag == null)
                    {
                        tsmEdit.Visible = true;
                        tsmClone.Visible = true;
                        tsmDelete.Visible = true;
                        tsmRename.Visible = true;
                        tsmProperties.Visible = true;
                        if (mySelectedNode.Parent != null && mySelectedNode.Parent.Tag != null && ((EditorInfo)mySelectedNode.Parent.Tag).CanCreateNewItems)
                            tsmNew.Visible = true;
                        else
                            tsmNew.Visible = false;
                    }
                    else
                    {
                        tsmEdit.Visible = false;
                        tsmClone.Visible = false;
                        if (((EditorInfo)mySelectedNode.Tag).IsFolder)
                        {
                            tsmDelete.Visible = true;
                            tsmRename.Visible = true;
                        }
                        else
                        {
                            tsmDelete.Visible = false;
                            tsmRename.Visible = false;
                        }
                        tsmProperties.Visible = false;
                        if (((EditorInfo)mySelectedNode.Tag).CanCreateNewItems)
                            tsmNew.Visible = true;
                        else
                            tsmNew.Visible = false;
                    }

                    cmsItemMenu.Show(tvItems, e.Location);
                }
            }
        }

        private void tsmNew_Click(object sender, EventArgs e)
        {
            TreeNode NewItem = CreateNewItem(tvItems.SelectedNode);
            tvItems.LabelEdit = true;
            tvItems.SelectedNode = NewItem;

            NewItem.BeginEdit();
        }

        private void tsmNewFolder_Click(object sender, EventArgs e)
        {
            TreeNode NewItem = CreateNewFolder(tvItems.SelectedNode);
            tvItems.LabelEdit = true;
            tvItems.SelectedNode = NewItem;

            NewItem.BeginEdit();
        }

        #endregion

        private void tsmClone_Click(object sender, EventArgs e)
        {
            TreeNode NewItem = CloneItem(tvItems.SelectedNode);
            tvItems.LabelEdit = true;
            tvItems.SelectedNode = NewItem;

            NewItem.BeginEdit();
        }

        private void tsmDelete_Click(object sender, EventArgs e)
        {
            DeleteNode(tvItems.SelectedNode);
        }

        private void tsmRename_Click(object sender, EventArgs e)
        {
            TreeNode ActiveNode = tvItems.SelectedNode;

            tvItems.LabelEdit = true;

            ActiveNode.BeginEdit();
        }

        private void tsmProperties_Click(object sender, EventArgs e)
        {
            TreeNode EditorNode = tvItems.SelectedNode;

            PropertiesForm pf = new PropertiesForm(EditorNode.Name, (string)EditorNode.Tag);
            pf.Show();
        }

        private void tsmUnitTester_Click(object sender, EventArgs e)
        {
            Editors.UnitTester.UnitTester NewTester = new Editors.UnitTester.UnitTester();
            NewTester.Show();
        }

        private void tsmRosterEditor_Click(object sender, EventArgs e)
        {
            Editors.RosterEditor.ProjectEternityRosterEditor NewTester = new Editors.RosterEditor.ProjectEternityRosterEditor();
            NewTester.LoadRoster();
            NewTester.Show();
        }

        private void tsmSystemList_Click(object sender, EventArgs e)
        {
            Editors.SystemListEditor.SystemListEditor NewTester = new Editors.SystemListEditor.SystemListEditor();
            NewTester.LoadData();
            NewTester.Show();
        }

        private void tsmTerrainAndUnitTypes_Click(object sender, EventArgs e)
        {
            Editors.SystemListEditor.TerrainAndUnitTypesEditor NewTester = new Editors.SystemListEditor.TerrainAndUnitTypesEditor();
            NewTester.Show();
        }

        private void tsmVariables_Click(object sender, EventArgs e)
        {
            Editors.SystemListEditor.VariablesListEditor NewTester = new Editors.SystemListEditor.VariablesListEditor();
            NewTester.Show();
        }
    }
}
