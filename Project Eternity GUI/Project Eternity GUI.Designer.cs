namespace ProjectEternity.GUI
{
    partial class GUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Characters");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Units");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Heads");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Torsos");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Arms");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Legs");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Unit Parts", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Antenas");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Ears");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Eyes");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("CPU");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Head Parts", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9,
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Torso Parts");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Arm Parts");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Legs Parts");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Generic Parts");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Small Parts", new System.Windows.Forms.TreeNode[] {
            treeNode12,
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Scrap Parts");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Blueprints");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Items", new System.Windows.Forms.TreeNode[] {
            treeNode7,
            treeNode17,
            treeNode18,
            treeNode19});
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Weapons");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Visual Novels");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Maps");
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUI));
            this.tvItems = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUnitTester = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRosterEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSystemList = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTerrainAndUnitTypes = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmVariables = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbNew = new System.Windows.Forms.ToolStripButton();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.cmsItemMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmNew = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmNewFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmClone = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmOpenInFileExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.cmsItemMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvItems
            // 
            this.tvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvItems.LabelEdit = true;
            this.tvItems.Location = new System.Drawing.Point(0, 0);
            this.tvItems.Name = "tvItems";
            treeNode1.Name = "tnCharacters";
            treeNode1.Text = "Characters";
            treeNode2.Name = "Node3";
            treeNode2.Text = "Units";
            treeNode3.Name = "Node13";
            treeNode3.Text = "Heads";
            treeNode4.Name = "Node14";
            treeNode4.Text = "Torsos";
            treeNode5.Name = "Node15";
            treeNode5.Text = "Arms";
            treeNode6.Name = "Node16";
            treeNode6.Text = "Legs";
            treeNode7.Name = "Node10";
            treeNode7.Text = "Unit Parts";
            treeNode8.Name = "Node17";
            treeNode8.Text = "Antenas";
            treeNode9.Name = "Node18";
            treeNode9.Text = "Ears";
            treeNode10.Name = "Node19";
            treeNode10.Text = "Eyes";
            treeNode11.Name = "Node23";
            treeNode11.Text = "CPU";
            treeNode12.Name = "Node22";
            treeNode12.Text = "Head Parts";
            treeNode13.Name = "Node20";
            treeNode13.Text = "Torso Parts";
            treeNode14.Name = "Node21";
            treeNode14.Text = "Arm Parts";
            treeNode15.Name = "Node24";
            treeNode15.Text = "Legs Parts";
            treeNode16.Name = "Node25";
            treeNode16.Text = "Generic Parts";
            treeNode17.Name = "Node11";
            treeNode17.Text = "Small Parts";
            treeNode18.Name = "Node28";
            treeNode18.Text = "Scrap Parts";
            treeNode19.Name = "Node27";
            treeNode19.Text = "Blueprints";
            treeNode20.Name = "Node26";
            treeNode20.Text = "Items";
            treeNode21.Name = "Node6";
            treeNode21.Text = "Weapons";
            treeNode22.Name = "Node29";
            treeNode22.Text = "Visual Novels";
            treeNode23.Name = "Node4";
            treeNode23.Text = "Maps";
            this.tvItems.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode20,
            treeNode21,
            treeNode22,
            treeNode23});
            this.tvItems.PathSeparator = "/";
            this.tvItems.Size = new System.Drawing.Size(209, 492);
            this.tvItems.TabIndex = 0;
            this.tvItems.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvItems_BeforeLabelEdit);
            this.tvItems.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvItems_AfterLabelEdit);
            this.tvItems.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvItems_NodeMouseDoubleClick);
            this.tvItems.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvItems_MouseClick);
            this.tvItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvItems_MouseDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.tsmUnitTester,
            this.tsmRosterEditor,
            this.tsmSystemList,
            this.tsmTerrainAndUnitTypes,
            this.tsmVariables});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(758, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tsmUnitTester
            // 
            this.tsmUnitTester.Name = "tsmUnitTester";
            this.tsmUnitTester.Size = new System.Drawing.Size(74, 20);
            this.tsmUnitTester.Text = "Unit Tester";
            this.tsmUnitTester.Click += new System.EventHandler(this.tsmUnitTester_Click);
            // 
            // tsmRosterEditor
            // 
            this.tsmRosterEditor.Name = "tsmRosterEditor";
            this.tsmRosterEditor.Size = new System.Drawing.Size(86, 20);
            this.tsmRosterEditor.Text = "Roster Editor";
            this.tsmRosterEditor.Click += new System.EventHandler(this.tsmRosterEditor_Click);
            // 
            // tsmSystemList
            // 
            this.tsmSystemList.Name = "tsmSystemList";
            this.tsmSystemList.Size = new System.Drawing.Size(78, 20);
            this.tsmSystemList.Text = "System List";
            this.tsmSystemList.Click += new System.EventHandler(this.tsmSystemList_Click);
            // 
            // tsmTerrainAndUnitTypes
            // 
            this.tsmTerrainAndUnitTypes.Name = "tsmTerrainAndUnitTypes";
            this.tsmTerrainAndUnitTypes.Size = new System.Drawing.Size(134, 20);
            this.tsmTerrainAndUnitTypes.Text = "Terrain and Unit Types";
            this.tsmTerrainAndUnitTypes.Click += new System.EventHandler(this.tsmTerrainAndUnitTypes_Click);
            // 
            // tsmVariables
            // 
            this.tsmVariables.Name = "tsmVariables";
            this.tsmVariables.Size = new System.Drawing.Size(65, 20);
            this.tsmVariables.Text = "Variables";
            this.tsmVariables.Click += new System.EventHandler(this.tsmVariables_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbNew,
            this.tsbOpen,
            this.tsbSave,
            this.toolStripSeparator1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(758, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsbNew
            // 
            this.tsbNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbNew.Image = ((System.Drawing.Image)(resources.GetObject("tsbNew.Image")));
            this.tsbNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbNew.Name = "tsbNew";
            this.tsbNew.Size = new System.Drawing.Size(23, 22);
            this.tsbNew.Text = "New";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsbOpen.Image")));
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(23, 22);
            this.tsbOpen.Text = "Open";
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Image = ((System.Drawing.Image)(resources.GetObject("tsbSave.Image")));
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(23, 22);
            this.tsbSave.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 52);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tvItems);
            this.splitContainer1.Size = new System.Drawing.Size(758, 496);
            this.splitContainer1.SplitterDistance = 213;
            this.splitContainer1.TabIndex = 3;
            // 
            // cmsItemMenu
            // 
            this.cmsItemMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmNew,
            this.tsmNewFolder,
            this.tsmClone,
            this.tsmEdit,
            this.tsmDelete,
            this.tsmRename,
            this.tsmProperties,
            this.tsmOpenInFileExplorer});
            this.cmsItemMenu.Name = "cmsItemProperties";
            this.cmsItemMenu.Size = new System.Drawing.Size(182, 202);
            // 
            // tsmNew
            // 
            this.tsmNew.Name = "tsmNew";
            this.tsmNew.Size = new System.Drawing.Size(181, 22);
            this.tsmNew.Text = "New";
            this.tsmNew.Click += new System.EventHandler(this.tsmNew_Click);
            // 
            // tsmNewFolder
            // 
            this.tsmNewFolder.Name = "tsmNewFolder";
            this.tsmNewFolder.Size = new System.Drawing.Size(181, 22);
            this.tsmNewFolder.Text = "New folder";
            this.tsmNewFolder.Click += new System.EventHandler(this.tsmNewFolder_Click);
            // 
            // tsmClone
            // 
            this.tsmClone.Name = "tsmClone";
            this.tsmClone.Size = new System.Drawing.Size(181, 22);
            this.tsmClone.Text = "Clone";
            this.tsmClone.Click += new System.EventHandler(this.tsmClone_Click);
            // 
            // tsmEdit
            // 
            this.tsmEdit.Name = "tsmEdit";
            this.tsmEdit.Size = new System.Drawing.Size(181, 22);
            this.tsmEdit.Text = "Edit";
            this.tsmEdit.Click += new System.EventHandler(this.tsmEdit_Click);
            // 
            // tsmDelete
            // 
            this.tsmDelete.Name = "tsmDelete";
            this.tsmDelete.Size = new System.Drawing.Size(181, 22);
            this.tsmDelete.Text = "Delete";
            this.tsmDelete.Click += new System.EventHandler(this.tsmDelete_Click);
            // 
            // tsmRename
            // 
            this.tsmRename.Name = "tsmRename";
            this.tsmRename.Size = new System.Drawing.Size(181, 22);
            this.tsmRename.Text = "Rename";
            this.tsmRename.Click += new System.EventHandler(this.tsmRename_Click);
            // 
            // tsmProperties
            // 
            this.tsmProperties.Name = "tsmProperties";
            this.tsmProperties.Size = new System.Drawing.Size(181, 22);
            this.tsmProperties.Text = "Properties";
            this.tsmProperties.Click += new System.EventHandler(this.tsmProperties_Click);
            // 
            // tsmOpenInFileExplorer
            // 
            this.tsmOpenInFileExplorer.Name = "tsmOpenInFileExplorer";
            this.tsmOpenInFileExplorer.Size = new System.Drawing.Size(181, 22);
            this.tsmOpenInFileExplorer.Text = "Open in file explorer";
            this.tsmOpenInFileExplorer.Click += new System.EventHandler(this.tsmOpenInFileExplorer_Click);
            // 
            // GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 560);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.splitContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "GUI";
            this.Text = "Project Eternity GUI";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.cmsItemMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripButton tsbNew;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ContextMenuStrip cmsItemMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmNew;
        private System.Windows.Forms.ToolStripMenuItem tsmProperties;
        private System.Windows.Forms.ToolStripMenuItem tsmDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmRename;
        private System.Windows.Forms.ToolStripMenuItem tsmNewFolder;
        private System.Windows.Forms.ToolStripMenuItem tsmUnitTester;
        private System.Windows.Forms.ToolStripMenuItem tsmRosterEditor;
        private System.Windows.Forms.ToolStripMenuItem tsmSystemList;
        public System.Windows.Forms.TreeView tvItems;
        private System.Windows.Forms.ToolStripMenuItem tsmClone;
        private System.Windows.Forms.ToolStripMenuItem tsmTerrainAndUnitTypes;
        private System.Windows.Forms.ToolStripMenuItem tsmVariables;
        private System.Windows.Forms.ToolStripMenuItem tsmOpenInFileExplorer;
    }
}

