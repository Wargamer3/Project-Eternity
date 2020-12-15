namespace ProjectEternity.Editors.TripleThunderEditor
{
    partial class TripleThunderMapEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LayerViewer = new ProjectEternity.Editors.AnimationEditor.LayerViewerControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLayers = new System.Windows.Forms.TabPage();
            this.gbGroundLevel = new System.Windows.Forms.GroupBox();
            this.txtGroundLevelPoints = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gbLayers = new System.Windows.Forms.GroupBox();
            this.btnMoveDownLayer = new System.Windows.Forms.Button();
            this.btnMoveUpLayer = new System.Windows.Forms.Button();
            this.lstLayers = new System.Windows.Forms.ListBox();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.tabCollisions = new System.Windows.Forms.TabPage();
            this.pgCollisionBox = new System.Windows.Forms.PropertyGrid();
            this.gbCollisionBoxes = new System.Windows.Forms.GroupBox();
            this.lstCollisionBox = new System.Windows.Forms.ListBox();
            this.btnAddCollisionBox = new System.Windows.Forms.Button();
            this.btnRemoveCollisionBox = new System.Windows.Forms.Button();
            this.tabImages = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabBackgroundPreview = new System.Windows.Forms.TabPage();
            this.pnBackgroundPreview = new System.Windows.Forms.Panel();
            this.BackgroundViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.tabBackgroundProperties = new System.Windows.Forms.TabPage();
            this.pgBackground = new System.Windows.Forms.PropertyGrid();
            this.gbBackgrounds = new System.Windows.Forms.GroupBox();
            this.lstBackgrounds = new System.Windows.Forms.ListBox();
            this.btnAddBackground = new System.Windows.Forms.Button();
            this.btnRemoveBackground = new System.Windows.Forms.Button();
            this.tabProps = new System.Windows.Forms.TabPage();
            this.gbProps = new System.Windows.Forms.GroupBox();
            this.lstProps = new System.Windows.Forms.ListBox();
            this.pgProps = new System.Windows.Forms.PropertyGrid();
            this.tabSpawns = new System.Windows.Forms.TabPage();
            this.gbSpawns = new System.Windows.Forms.GroupBox();
            this.btnAddNoTeamSpawn = new System.Windows.Forms.Button();
            this.btnAddVehicleSpawn = new System.Windows.Forms.Button();
            this.lstSpawns = new System.Windows.Forms.ListBox();
            this.btnAddSPSpawn = new System.Windows.Forms.Button();
            this.btnRemoveSpawn = new System.Windows.Forms.Button();
            this.pgSpawn = new System.Windows.Forms.PropertyGrid();
            this.tbScripts = new System.Windows.Forms.TabPage();
            this.ScriptingContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tbEvents = new System.Windows.Forms.TabPage();
            this.lstEvents = new System.Windows.Forms.ListBox();
            this.tbConditions = new System.Windows.Forms.TabPage();
            this.lstConditions = new System.Windows.Forms.ListBox();
            this.tbTriggers = new System.Windows.Forms.TabPage();
            this.lstTriggers = new System.Windows.Forms.ListBox();
            this.pgScriptProperties = new System.Windows.Forms.PropertyGrid();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmProperties = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLayers.SuspendLayout();
            this.gbGroundLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGroundLevelPoints)).BeginInit();
            this.gbLayers.SuspendLayout();
            this.tabCollisions.SuspendLayout();
            this.gbCollisionBoxes.SuspendLayout();
            this.tabImages.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabBackgroundPreview.SuspendLayout();
            this.pnBackgroundPreview.SuspendLayout();
            this.tabBackgroundProperties.SuspendLayout();
            this.gbBackgrounds.SuspendLayout();
            this.tabProps.SuspendLayout();
            this.gbProps.SuspendLayout();
            this.tabSpawns.SuspendLayout();
            this.gbSpawns.SuspendLayout();
            this.tbScripts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).BeginInit();
            this.ScriptingContainer.Panel1.SuspendLayout();
            this.ScriptingContainer.Panel2.SuspendLayout();
            this.ScriptingContainer.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tbEvents.SuspendLayout();
            this.tbConditions.SuspendLayout();
            this.tbTriggers.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LayerViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(866, 434);
            this.splitContainer1.SplitterDistance = 568;
            this.splitContainer1.TabIndex = 0;
            // 
            // LayerViewer
            // 
            this.LayerViewer.AllowDrop = true;
            this.LayerViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayerViewer.Location = new System.Drawing.Point(0, 0);
            this.LayerViewer.Name = "LayerViewer";
            this.LayerViewer.Size = new System.Drawing.Size(564, 430);
            this.LayerViewer.TabIndex = 0;
            this.LayerViewer.DragDrop += new System.Windows.Forms.DragEventHandler(this.LayerViewer_DragDrop);
            this.LayerViewer.DragEnter += new System.Windows.Forms.DragEventHandler(this.LayerViewer_DragEnter);
            this.LayerViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LayerViewer_MouseDown);
            this.LayerViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LayerViewer_MouseMove);
            this.LayerViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LayerViewer_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLayers);
            this.tabControl1.Controls.Add(this.tabCollisions);
            this.tabControl1.Controls.Add(this.tabImages);
            this.tabControl1.Controls.Add(this.tabProps);
            this.tabControl1.Controls.Add(this.tabSpawns);
            this.tabControl1.Controls.Add(this.tbScripts);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(290, 430);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabLayers
            // 
            this.tabLayers.Controls.Add(this.gbGroundLevel);
            this.tabLayers.Controls.Add(this.gbLayers);
            this.tabLayers.Location = new System.Drawing.Point(4, 22);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayers.Size = new System.Drawing.Size(282, 404);
            this.tabLayers.TabIndex = 4;
            this.tabLayers.Text = "Layers";
            this.tabLayers.UseVisualStyleBackColor = true;
            // 
            // gbGroundLevel
            // 
            this.gbGroundLevel.Controls.Add(this.txtGroundLevelPoints);
            this.gbGroundLevel.Controls.Add(this.label2);
            this.gbGroundLevel.Location = new System.Drawing.Point(6, 165);
            this.gbGroundLevel.Name = "gbGroundLevel";
            this.gbGroundLevel.Size = new System.Drawing.Size(259, 49);
            this.gbGroundLevel.TabIndex = 11;
            this.gbGroundLevel.TabStop = false;
            this.gbGroundLevel.Text = "Layer ground level";
            // 
            // txtGroundLevelPoints
            // 
            this.txtGroundLevelPoints.Location = new System.Drawing.Point(158, 19);
            this.txtGroundLevelPoints.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.txtGroundLevelPoints.Name = "txtGroundLevelPoints";
            this.txtGroundLevelPoints.Size = new System.Drawing.Size(95, 20);
            this.txtGroundLevelPoints.TabIndex = 8;
            this.txtGroundLevelPoints.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.txtGroundLevelPoints.ValueChanged += new System.EventHandler(this.txtGroundLevelPoints_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Number of points";
            // 
            // gbLayers
            // 
            this.gbLayers.Controls.Add(this.btnMoveDownLayer);
            this.gbLayers.Controls.Add(this.btnMoveUpLayer);
            this.gbLayers.Controls.Add(this.lstLayers);
            this.gbLayers.Controls.Add(this.btnAddLayer);
            this.gbLayers.Controls.Add(this.btnRemoveLayer);
            this.gbLayers.Location = new System.Drawing.Point(6, 6);
            this.gbLayers.Name = "gbLayers";
            this.gbLayers.Size = new System.Drawing.Size(259, 153);
            this.gbLayers.TabIndex = 10;
            this.gbLayers.TabStop = false;
            this.gbLayers.Text = "Layers";
            // 
            // btnMoveDownLayer
            // 
            this.btnMoveDownLayer.Location = new System.Drawing.Point(133, 123);
            this.btnMoveDownLayer.Name = "btnMoveDownLayer";
            this.btnMoveDownLayer.Size = new System.Drawing.Size(120, 23);
            this.btnMoveDownLayer.TabIndex = 7;
            this.btnMoveDownLayer.Text = "Move Down";
            this.btnMoveDownLayer.UseVisualStyleBackColor = true;
            this.btnMoveDownLayer.Click += new System.EventHandler(this.btnMoveDownLayer_Click);
            // 
            // btnMoveUpLayer
            // 
            this.btnMoveUpLayer.Location = new System.Drawing.Point(6, 123);
            this.btnMoveUpLayer.Name = "btnMoveUpLayer";
            this.btnMoveUpLayer.Size = new System.Drawing.Size(120, 23);
            this.btnMoveUpLayer.TabIndex = 6;
            this.btnMoveUpLayer.Text = "Move Up";
            this.btnMoveUpLayer.UseVisualStyleBackColor = true;
            this.btnMoveUpLayer.Click += new System.EventHandler(this.btnMoveUpLayer_Click);
            // 
            // lstLayers
            // 
            this.lstLayers.FormattingEnabled = true;
            this.lstLayers.Location = new System.Drawing.Point(6, 19);
            this.lstLayers.Name = "lstLayers";
            this.lstLayers.Size = new System.Drawing.Size(247, 69);
            this.lstLayers.TabIndex = 5;
            this.lstLayers.SelectedIndexChanged += new System.EventHandler(this.lstLayers_SelectedIndexChanged);
            // 
            // btnAddLayer
            // 
            this.btnAddLayer.Location = new System.Drawing.Point(6, 94);
            this.btnAddLayer.Name = "btnAddLayer";
            this.btnAddLayer.Size = new System.Drawing.Size(120, 23);
            this.btnAddLayer.TabIndex = 1;
            this.btnAddLayer.Text = "Add Layer";
            this.btnAddLayer.UseVisualStyleBackColor = true;
            this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Location = new System.Drawing.Point(133, 94);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveLayer.TabIndex = 2;
            this.btnRemoveLayer.Text = "Remove Layer";
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // tabCollisions
            // 
            this.tabCollisions.Controls.Add(this.pgCollisionBox);
            this.tabCollisions.Controls.Add(this.gbCollisionBoxes);
            this.tabCollisions.Location = new System.Drawing.Point(4, 22);
            this.tabCollisions.Name = "tabCollisions";
            this.tabCollisions.Padding = new System.Windows.Forms.Padding(3);
            this.tabCollisions.Size = new System.Drawing.Size(282, 404);
            this.tabCollisions.TabIndex = 6;
            this.tabCollisions.Text = "Collisions";
            this.tabCollisions.UseVisualStyleBackColor = true;
            // 
            // pgCollisionBox
            // 
            this.pgCollisionBox.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgCollisionBox.Location = new System.Drawing.Point(6, 190);
            this.pgCollisionBox.Name = "pgCollisionBox";
            this.pgCollisionBox.Size = new System.Drawing.Size(259, 208);
            this.pgCollisionBox.TabIndex = 14;
            // 
            // gbCollisionBoxes
            // 
            this.gbCollisionBoxes.Controls.Add(this.lstCollisionBox);
            this.gbCollisionBoxes.Controls.Add(this.btnAddCollisionBox);
            this.gbCollisionBoxes.Controls.Add(this.btnRemoveCollisionBox);
            this.gbCollisionBoxes.Location = new System.Drawing.Point(6, 6);
            this.gbCollisionBoxes.Name = "gbCollisionBoxes";
            this.gbCollisionBoxes.Size = new System.Drawing.Size(259, 178);
            this.gbCollisionBoxes.TabIndex = 13;
            this.gbCollisionBoxes.TabStop = false;
            this.gbCollisionBoxes.Text = "Collision Boxes";
            // 
            // lstCollisionBox
            // 
            this.lstCollisionBox.FormattingEnabled = true;
            this.lstCollisionBox.Location = new System.Drawing.Point(6, 19);
            this.lstCollisionBox.Name = "lstCollisionBox";
            this.lstCollisionBox.Size = new System.Drawing.Size(247, 121);
            this.lstCollisionBox.TabIndex = 5;
            this.lstCollisionBox.SelectedIndexChanged += new System.EventHandler(this.lstCollisionBox_SelectedIndexChanged);
            // 
            // btnAddCollisionBox
            // 
            this.btnAddCollisionBox.Location = new System.Drawing.Point(6, 149);
            this.btnAddCollisionBox.Name = "btnAddCollisionBox";
            this.btnAddCollisionBox.Size = new System.Drawing.Size(120, 23);
            this.btnAddCollisionBox.TabIndex = 1;
            this.btnAddCollisionBox.Text = "Add collision box";
            this.btnAddCollisionBox.UseVisualStyleBackColor = true;
            this.btnAddCollisionBox.Click += new System.EventHandler(this.btnAddCollisionBox_Click);
            // 
            // btnRemoveCollisionBox
            // 
            this.btnRemoveCollisionBox.Location = new System.Drawing.Point(133, 149);
            this.btnRemoveCollisionBox.Name = "btnRemoveCollisionBox";
            this.btnRemoveCollisionBox.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveCollisionBox.TabIndex = 2;
            this.btnRemoveCollisionBox.Text = "Remove collision box";
            this.btnRemoveCollisionBox.UseVisualStyleBackColor = true;
            this.btnRemoveCollisionBox.Click += new System.EventHandler(this.btnRemoveCollisionBox_Click);
            // 
            // tabImages
            // 
            this.tabImages.Controls.Add(this.tabControl2);
            this.tabImages.Controls.Add(this.gbBackgrounds);
            this.tabImages.Location = new System.Drawing.Point(4, 22);
            this.tabImages.Name = "tabImages";
            this.tabImages.Padding = new System.Windows.Forms.Padding(3);
            this.tabImages.Size = new System.Drawing.Size(282, 404);
            this.tabImages.TabIndex = 1;
            this.tabImages.Text = "Images";
            this.tabImages.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabBackgroundPreview);
            this.tabControl2.Controls.Add(this.tabBackgroundProperties);
            this.tabControl2.Location = new System.Drawing.Point(0, 190);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(271, 214);
            this.tabControl2.TabIndex = 1;
            // 
            // tabBackgroundPreview
            // 
            this.tabBackgroundPreview.Controls.Add(this.pnBackgroundPreview);
            this.tabBackgroundPreview.Location = new System.Drawing.Point(4, 22);
            this.tabBackgroundPreview.Name = "tabBackgroundPreview";
            this.tabBackgroundPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabBackgroundPreview.Size = new System.Drawing.Size(263, 188);
            this.tabBackgroundPreview.TabIndex = 0;
            this.tabBackgroundPreview.Text = "Background Preview";
            this.tabBackgroundPreview.UseVisualStyleBackColor = true;
            // 
            // pnBackgroundPreview
            // 
            this.pnBackgroundPreview.AutoScroll = true;
            this.pnBackgroundPreview.Controls.Add(this.BackgroundViewer);
            this.pnBackgroundPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBackgroundPreview.Location = new System.Drawing.Point(3, 3);
            this.pnBackgroundPreview.Name = "pnBackgroundPreview";
            this.pnBackgroundPreview.Size = new System.Drawing.Size(257, 182);
            this.pnBackgroundPreview.TabIndex = 1;
            // 
            // BackgroundViewer
            // 
            this.BackgroundViewer.Location = new System.Drawing.Point(0, 0);
            this.BackgroundViewer.Name = "BackgroundViewer";
            this.BackgroundViewer.Size = new System.Drawing.Size(243, 179);
            this.BackgroundViewer.TabIndex = 0;
            // 
            // tabBackgroundProperties
            // 
            this.tabBackgroundProperties.Controls.Add(this.pgBackground);
            this.tabBackgroundProperties.Location = new System.Drawing.Point(4, 22);
            this.tabBackgroundProperties.Name = "tabBackgroundProperties";
            this.tabBackgroundProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabBackgroundProperties.Size = new System.Drawing.Size(263, 188);
            this.tabBackgroundProperties.TabIndex = 1;
            this.tabBackgroundProperties.Text = "Background Properties";
            this.tabBackgroundProperties.UseVisualStyleBackColor = true;
            // 
            // pgBackground
            // 
            this.pgBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgBackground.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgBackground.Location = new System.Drawing.Point(3, 3);
            this.pgBackground.Name = "pgBackground";
            this.pgBackground.Size = new System.Drawing.Size(257, 182);
            this.pgBackground.TabIndex = 1;
            // 
            // gbBackgrounds
            // 
            this.gbBackgrounds.Controls.Add(this.lstBackgrounds);
            this.gbBackgrounds.Controls.Add(this.btnAddBackground);
            this.gbBackgrounds.Controls.Add(this.btnRemoveBackground);
            this.gbBackgrounds.Location = new System.Drawing.Point(6, 6);
            this.gbBackgrounds.Name = "gbBackgrounds";
            this.gbBackgrounds.Size = new System.Drawing.Size(259, 178);
            this.gbBackgrounds.TabIndex = 9;
            this.gbBackgrounds.TabStop = false;
            this.gbBackgrounds.Text = "Backgrounds";
            // 
            // lstBackgrounds
            // 
            this.lstBackgrounds.FormattingEnabled = true;
            this.lstBackgrounds.Location = new System.Drawing.Point(6, 19);
            this.lstBackgrounds.Name = "lstBackgrounds";
            this.lstBackgrounds.Size = new System.Drawing.Size(247, 121);
            this.lstBackgrounds.TabIndex = 5;
            this.lstBackgrounds.SelectedIndexChanged += new System.EventHandler(this.lstBackgrounds_SelectedIndexChanged);
            this.lstBackgrounds.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstImages_MouseDown);
            // 
            // btnAddBackground
            // 
            this.btnAddBackground.Location = new System.Drawing.Point(6, 146);
            this.btnAddBackground.Name = "btnAddBackground";
            this.btnAddBackground.Size = new System.Drawing.Size(120, 23);
            this.btnAddBackground.TabIndex = 1;
            this.btnAddBackground.Text = "Add background";
            this.btnAddBackground.UseVisualStyleBackColor = true;
            this.btnAddBackground.Click += new System.EventHandler(this.btnAddImage_Click);
            // 
            // btnRemoveBackground
            // 
            this.btnRemoveBackground.Location = new System.Drawing.Point(133, 146);
            this.btnRemoveBackground.Name = "btnRemoveBackground";
            this.btnRemoveBackground.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveBackground.TabIndex = 2;
            this.btnRemoveBackground.Text = "Remove background";
            this.btnRemoveBackground.UseVisualStyleBackColor = true;
            this.btnRemoveBackground.Click += new System.EventHandler(this.btnRemoveImage_Click);
            // 
            // tabProps
            // 
            this.tabProps.Controls.Add(this.gbProps);
            this.tabProps.Controls.Add(this.pgProps);
            this.tabProps.Location = new System.Drawing.Point(4, 22);
            this.tabProps.Name = "tabProps";
            this.tabProps.Size = new System.Drawing.Size(282, 404);
            this.tabProps.TabIndex = 3;
            this.tabProps.Text = "Props";
            this.tabProps.UseVisualStyleBackColor = true;
            // 
            // gbProps
            // 
            this.gbProps.Controls.Add(this.lstProps);
            this.gbProps.Location = new System.Drawing.Point(6, 6);
            this.gbProps.Name = "gbProps";
            this.gbProps.Size = new System.Drawing.Size(259, 178);
            this.gbProps.TabIndex = 10;
            this.gbProps.TabStop = false;
            this.gbProps.Text = "Props";
            // 
            // lstProps
            // 
            this.lstProps.FormattingEnabled = true;
            this.lstProps.Location = new System.Drawing.Point(6, 19);
            this.lstProps.Name = "lstProps";
            this.lstProps.Size = new System.Drawing.Size(247, 147);
            this.lstProps.TabIndex = 5;
            this.lstProps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstProps_MouseDown);
            // 
            // pgProps
            // 
            this.pgProps.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgProps.Location = new System.Drawing.Point(6, 190);
            this.pgProps.Name = "pgProps";
            this.pgProps.Size = new System.Drawing.Size(259, 208);
            this.pgProps.TabIndex = 0;
            // 
            // tabSpawns
            // 
            this.tabSpawns.Controls.Add(this.gbSpawns);
            this.tabSpawns.Controls.Add(this.pgSpawn);
            this.tabSpawns.Location = new System.Drawing.Point(4, 22);
            this.tabSpawns.Name = "tabSpawns";
            this.tabSpawns.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpawns.Size = new System.Drawing.Size(282, 404);
            this.tabSpawns.TabIndex = 5;
            this.tabSpawns.Text = "Spawns";
            this.tabSpawns.UseVisualStyleBackColor = true;
            // 
            // gbSpawns
            // 
            this.gbSpawns.Controls.Add(this.btnAddNoTeamSpawn);
            this.gbSpawns.Controls.Add(this.btnAddVehicleSpawn);
            this.gbSpawns.Controls.Add(this.lstSpawns);
            this.gbSpawns.Controls.Add(this.btnAddSPSpawn);
            this.gbSpawns.Controls.Add(this.btnRemoveSpawn);
            this.gbSpawns.Location = new System.Drawing.Point(6, 6);
            this.gbSpawns.Name = "gbSpawns";
            this.gbSpawns.Size = new System.Drawing.Size(259, 178);
            this.gbSpawns.TabIndex = 12;
            this.gbSpawns.TabStop = false;
            this.gbSpawns.Text = "Spawns";
            // 
            // btnAddNoTeamSpawn
            // 
            this.btnAddNoTeamSpawn.Location = new System.Drawing.Point(133, 120);
            this.btnAddNoTeamSpawn.Name = "btnAddNoTeamSpawn";
            this.btnAddNoTeamSpawn.Size = new System.Drawing.Size(120, 23);
            this.btnAddNoTeamSpawn.TabIndex = 7;
            this.btnAddNoTeamSpawn.Text = "Add No Team spawn";
            this.btnAddNoTeamSpawn.UseVisualStyleBackColor = true;
            this.btnAddNoTeamSpawn.Click += new System.EventHandler(this.btnAddNoTeamSpawn_Click);
            // 
            // btnAddVehicleSpawn
            // 
            this.btnAddVehicleSpawn.Location = new System.Drawing.Point(6, 149);
            this.btnAddVehicleSpawn.Name = "btnAddVehicleSpawn";
            this.btnAddVehicleSpawn.Size = new System.Drawing.Size(120, 23);
            this.btnAddVehicleSpawn.TabIndex = 6;
            this.btnAddVehicleSpawn.Text = "Add vehicle spawn";
            this.btnAddVehicleSpawn.UseVisualStyleBackColor = true;
            this.btnAddVehicleSpawn.Click += new System.EventHandler(this.btnAddVehicleSpawn_Click);
            // 
            // lstSpawns
            // 
            this.lstSpawns.FormattingEnabled = true;
            this.lstSpawns.Location = new System.Drawing.Point(6, 19);
            this.lstSpawns.Name = "lstSpawns";
            this.lstSpawns.Size = new System.Drawing.Size(247, 95);
            this.lstSpawns.TabIndex = 5;
            this.lstSpawns.SelectedIndexChanged += new System.EventHandler(this.lstSpawns_SelectedIndexChanged);
            // 
            // btnAddSPSpawn
            // 
            this.btnAddSPSpawn.Location = new System.Drawing.Point(6, 120);
            this.btnAddSPSpawn.Name = "btnAddSPSpawn";
            this.btnAddSPSpawn.Size = new System.Drawing.Size(120, 23);
            this.btnAddSPSpawn.TabIndex = 1;
            this.btnAddSPSpawn.Text = "Add Team spawn";
            this.btnAddSPSpawn.UseVisualStyleBackColor = true;
            this.btnAddSPSpawn.Click += new System.EventHandler(this.btnAddTeamSpawn_Click);
            // 
            // btnRemoveSpawn
            // 
            this.btnRemoveSpawn.Location = new System.Drawing.Point(132, 149);
            this.btnRemoveSpawn.Name = "btnRemoveSpawn";
            this.btnRemoveSpawn.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveSpawn.TabIndex = 2;
            this.btnRemoveSpawn.Text = "Remove spawn";
            this.btnRemoveSpawn.UseVisualStyleBackColor = true;
            this.btnRemoveSpawn.Click += new System.EventHandler(this.btnRemoveSpawn_Click);
            // 
            // pgSpawn
            // 
            this.pgSpawn.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgSpawn.Location = new System.Drawing.Point(6, 190);
            this.pgSpawn.Name = "pgSpawn";
            this.pgSpawn.Size = new System.Drawing.Size(259, 208);
            this.pgSpawn.TabIndex = 11;
            // 
            // tbScripts
            // 
            this.tbScripts.Controls.Add(this.ScriptingContainer);
            this.tbScripts.Location = new System.Drawing.Point(4, 22);
            this.tbScripts.Name = "tbScripts";
            this.tbScripts.Padding = new System.Windows.Forms.Padding(3);
            this.tbScripts.Size = new System.Drawing.Size(282, 404);
            this.tbScripts.TabIndex = 7;
            this.tbScripts.Text = "Scripts";
            this.tbScripts.UseVisualStyleBackColor = true;
            // 
            // ScriptingContainer
            // 
            this.ScriptingContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ScriptingContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScriptingContainer.Location = new System.Drawing.Point(3, 3);
            this.ScriptingContainer.Name = "ScriptingContainer";
            this.ScriptingContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ScriptingContainer.Panel1
            // 
            this.ScriptingContainer.Panel1.Controls.Add(this.tabControl3);
            // 
            // ScriptingContainer.Panel2
            // 
            this.ScriptingContainer.Panel2.Controls.Add(this.pgScriptProperties);
            this.ScriptingContainer.Size = new System.Drawing.Size(276, 398);
            this.ScriptingContainer.SplitterDistance = 195;
            this.ScriptingContainer.TabIndex = 8;
            // 
            // tabControl3
            // 
            this.tabControl3.Controls.Add(this.tbEvents);
            this.tabControl3.Controls.Add(this.tbConditions);
            this.tabControl3.Controls.Add(this.tbTriggers);
            this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl3.Location = new System.Drawing.Point(0, 0);
            this.tabControl3.Multiline = true;
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(272, 191);
            this.tabControl3.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl3.TabIndex = 0;
            // 
            // tbEvents
            // 
            this.tbEvents.Controls.Add(this.lstEvents);
            this.tbEvents.Location = new System.Drawing.Point(4, 22);
            this.tbEvents.Name = "tbEvents";
            this.tbEvents.Size = new System.Drawing.Size(264, 165);
            this.tbEvents.TabIndex = 2;
            this.tbEvents.Text = "Events";
            this.tbEvents.UseVisualStyleBackColor = true;
            // 
            // lstEvents
            // 
            this.lstEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.Location = new System.Drawing.Point(0, 0);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(264, 165);
            this.lstEvents.TabIndex = 0;
            this.lstEvents.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);
            // 
            // tbConditions
            // 
            this.tbConditions.Controls.Add(this.lstConditions);
            this.tbConditions.Location = new System.Drawing.Point(4, 22);
            this.tbConditions.Name = "tbConditions";
            this.tbConditions.Size = new System.Drawing.Size(264, 165);
            this.tbConditions.TabIndex = 0;
            this.tbConditions.Text = "Conditions";
            this.tbConditions.UseVisualStyleBackColor = true;
            // 
            // lstConditions
            // 
            this.lstConditions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstConditions.FormattingEnabled = true;
            this.lstConditions.Location = new System.Drawing.Point(0, 0);
            this.lstConditions.Name = "lstConditions";
            this.lstConditions.Size = new System.Drawing.Size(264, 165);
            this.lstConditions.TabIndex = 0;
            this.lstConditions.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);
            // 
            // tbTriggers
            // 
            this.tbTriggers.Controls.Add(this.lstTriggers);
            this.tbTriggers.Location = new System.Drawing.Point(4, 22);
            this.tbTriggers.Name = "tbTriggers";
            this.tbTriggers.Size = new System.Drawing.Size(264, 165);
            this.tbTriggers.TabIndex = 1;
            this.tbTriggers.Text = "Triggers";
            this.tbTriggers.UseVisualStyleBackColor = true;
            // 
            // lstTriggers
            // 
            this.lstTriggers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTriggers.FormattingEnabled = true;
            this.lstTriggers.Location = new System.Drawing.Point(0, 0);
            this.lstTriggers.Name = "lstTriggers";
            this.lstTriggers.Size = new System.Drawing.Size(264, 165);
            this.lstTriggers.TabIndex = 0;
            this.lstTriggers.DoubleClick += new System.EventHandler(this.lstScriptChoices_DoubleClick);
            // 
            // pgScriptProperties
            // 
            this.pgScriptProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgScriptProperties.Location = new System.Drawing.Point(0, 0);
            this.pgScriptProperties.Name = "pgScriptProperties";
            this.pgScriptProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgScriptProperties.Size = new System.Drawing.Size(272, 195);
            this.pgScriptProperties.TabIndex = 0;
            this.pgScriptProperties.ToolbarVisible = false;
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmBackground,
            this.tsmProperties});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(866, 24);
            this.mnuToolBar.TabIndex = 1;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmBackground
            // 
            this.tsmBackground.Name = "tsmBackground";
            this.tsmBackground.Size = new System.Drawing.Size(83, 20);
            this.tsmBackground.Text = "Background";
            this.tsmBackground.Click += new System.EventHandler(this.tsmBackground_Click);
            // 
            // tsmProperties
            // 
            this.tsmProperties.Name = "tsmProperties";
            this.tsmProperties.Size = new System.Drawing.Size(72, 20);
            this.tsmProperties.Text = "Properties";
            this.tsmProperties.Click += new System.EventHandler(this.tsmProperties_Click);
            // 
            // TripleThunderMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 458);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuToolBar);
            this.MainMenuStrip = this.mnuToolBar;
            this.Name = "TripleThunderMapEditor";
            this.Text = "Triple Thunder Map Editor";
            this.Shown += new System.EventHandler(this.TripleThunderMapEditor_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabLayers.ResumeLayout(false);
            this.gbGroundLevel.ResumeLayout(false);
            this.gbGroundLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtGroundLevelPoints)).EndInit();
            this.gbLayers.ResumeLayout(false);
            this.tabCollisions.ResumeLayout(false);
            this.gbCollisionBoxes.ResumeLayout(false);
            this.tabImages.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabBackgroundPreview.ResumeLayout(false);
            this.pnBackgroundPreview.ResumeLayout(false);
            this.tabBackgroundProperties.ResumeLayout(false);
            this.gbBackgrounds.ResumeLayout(false);
            this.tabProps.ResumeLayout(false);
            this.gbProps.ResumeLayout(false);
            this.tabSpawns.ResumeLayout(false);
            this.gbSpawns.ResumeLayout(false);
            this.tbScripts.ResumeLayout(false);
            this.ScriptingContainer.Panel1.ResumeLayout(false);
            this.ScriptingContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).EndInit();
            this.ScriptingContainer.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tbEvents.ResumeLayout(false);
            this.tbConditions.ResumeLayout(false);
            this.tbTriggers.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabImages;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private AnimationEditor.LayerViewerControl LayerViewer;
        private System.Windows.Forms.TabPage tabProps;
        private System.Windows.Forms.GroupBox gbBackgrounds;
        private System.Windows.Forms.ListBox lstBackgrounds;
        private System.Windows.Forms.Button btnAddBackground;
        private System.Windows.Forms.Button btnRemoveBackground;
        private System.Windows.Forms.TabPage tabLayers;
        private System.Windows.Forms.GroupBox gbLayers;
        private System.Windows.Forms.ListBox lstLayers;
        private System.Windows.Forms.Button btnAddLayer;
        private System.Windows.Forms.Button btnRemoveLayer;
        private System.Windows.Forms.GroupBox gbGroundLevel;
        private System.Windows.Forms.NumericUpDown txtGroundLevelPoints;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnMoveDownLayer;
        private System.Windows.Forms.Button btnMoveUpLayer;
        private System.Windows.Forms.ToolStripMenuItem tsmBackground;
        private System.Windows.Forms.PropertyGrid pgProps;
        private System.Windows.Forms.GroupBox gbProps;
        private System.Windows.Forms.ListBox lstProps;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabBackgroundPreview;
        private System.Windows.Forms.TabPage tabBackgroundProperties;
        private System.Windows.Forms.Panel pnBackgroundPreview;
        private Core.Editor.Texture2DViewerControl BackgroundViewer;
        private System.Windows.Forms.PropertyGrid pgBackground;
        private System.Windows.Forms.ToolStripMenuItem tsmProperties;
        private System.Windows.Forms.TabPage tabSpawns;
        private System.Windows.Forms.GroupBox gbSpawns;
        private System.Windows.Forms.ListBox lstSpawns;
        private System.Windows.Forms.Button btnAddSPSpawn;
        private System.Windows.Forms.Button btnRemoveSpawn;
        private System.Windows.Forms.PropertyGrid pgSpawn;
        private System.Windows.Forms.TabPage tabCollisions;
        private System.Windows.Forms.PropertyGrid pgCollisionBox;
        private System.Windows.Forms.GroupBox gbCollisionBoxes;
        private System.Windows.Forms.ListBox lstCollisionBox;
        private System.Windows.Forms.Button btnAddCollisionBox;
        private System.Windows.Forms.Button btnRemoveCollisionBox;
        private System.Windows.Forms.TabPage tbScripts;
        private System.Windows.Forms.SplitContainer ScriptingContainer;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tbEvents;
        private System.Windows.Forms.ListBox lstEvents;
        private System.Windows.Forms.TabPage tbConditions;
        private System.Windows.Forms.ListBox lstConditions;
        private System.Windows.Forms.TabPage tbTriggers;
        private System.Windows.Forms.ListBox lstTriggers;
        private System.Windows.Forms.PropertyGrid pgScriptProperties;
        private System.Windows.Forms.Button btnAddVehicleSpawn;
        private System.Windows.Forms.Button btnAddNoTeamSpawn;
    }
}
