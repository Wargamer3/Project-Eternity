namespace ProjectEternity.Editors.AnimationEditor
{
    partial class ProjectEternityAnimationEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.AnimationViewer = new ProjectEternity.GameScreens.AnimationScreen.AnimationViewerControl();
            this.panTimelineViewer = new ProjectEternity.GameScreens.AnimationScreen.AnimationTimelineViewer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.vsbAnimationLayer = new System.Windows.Forms.VScrollBar();
            this.panAnimationLayers = new System.Windows.Forms.Panel();
            this.btnRemoveLayer = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnAddLayer = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.pgAnimationProperties = new System.Windows.Forms.PropertyGrid();
            this.tmrAnimation = new System.Windows.Forms.Timer(this.components);
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmUndo = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.pgAnimationProperties);
            this.splitContainer1.Size = new System.Drawing.Size(974, 680);
            this.splitContainer1.SplitterDistance = 720;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.AnimationViewer);
            this.splitContainer2.Panel1MinSize = 498;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panTimelineViewer);
            this.splitContainer2.Size = new System.Drawing.Size(716, 676);
            this.splitContainer2.SplitterDistance = 498;
            this.splitContainer2.TabIndex = 0;
            // 
            // AnimationViewer
            // 
            this.AnimationViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnimationViewer.Location = new System.Drawing.Point(0, 0);
            this.AnimationViewer.Name = "AnimationViewer";
            this.AnimationViewer.Size = new System.Drawing.Size(716, 498);
            this.AnimationViewer.TabIndex = 0;
            this.AnimationViewer.TimelineChanged += new ProjectEternity.GameScreens.AnimationScreen.AnimationViewerControl.TimelineChangedHandler(this.AnimationViewer_TimelineChanged);
            this.AnimationViewer.TimelineSelectionChanged += new ProjectEternity.GameScreens.AnimationScreen.AnimationViewerControl.TimelineChangedHandler(this.AnimationViewer_TimelineSelectionChanged);
            this.AnimationViewer.LayersChanged += new ProjectEternity.GameScreens.AnimationScreen.AnimationViewerControl.LayersChangedHandler(this.AnimationViewer_LayersChanged);
            this.AnimationViewer.TimelineSelected += new ProjectEternity.GameScreens.AnimationScreen.AnimationViewerControl.TimelineSelectedHandler(this.AnimationViewer_TimelineSelected);
            // 
            // panTimelineViewer
            // 
            this.panTimelineViewer.AllowDrop = true;
            this.panTimelineViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panTimelineViewer.Location = new System.Drawing.Point(0, 0);
            this.panTimelineViewer.Name = "panTimelineViewer";
            this.panTimelineViewer.Size = new System.Drawing.Size(716, 174);
            this.panTimelineViewer.TabIndex = 3;
            this.panTimelineViewer.TimelineChanged += new ProjectEternity.GameScreens.AnimationScreen.AnimationTimelineViewer.TimelineChangedHandler(this.OnTimelineSelectionChange);
            this.panTimelineViewer.TimelineSelected += new ProjectEternity.GameScreens.AnimationScreen.AnimationTimelineViewer.TimelineSelectedHandler(this.OnTimelineSelection);
            this.panTimelineViewer.KeyFrameSelected += new ProjectEternity.GameScreens.AnimationScreen.AnimationTimelineViewer.KeyFrameSelectedHandler(this.OnKeyFrameSelected);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.vsbAnimationLayer);
            this.groupBox1.Controls.Add(this.panAnimationLayers);
            this.groupBox1.Controls.Add(this.btnRemoveLayer);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.btnAddLayer);
            this.groupBox1.Controls.Add(this.btnStop);
            this.groupBox1.Controls.Add(this.btnPause);
            this.groupBox1.Controls.Add(this.btnPlay);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 415);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Animation Layers";
            // 
            // vsbAnimationLayer
            // 
            this.vsbAnimationLayer.Location = new System.Drawing.Point(220, 19);
            this.vsbAnimationLayer.Name = "vsbAnimationLayer";
            this.vsbAnimationLayer.Size = new System.Drawing.Size(17, 313);
            this.vsbAnimationLayer.TabIndex = 8;
            this.vsbAnimationLayer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsbAnimationLayer_Scroll);
            // 
            // panAnimationLayers
            // 
            this.panAnimationLayers.AllowDrop = true;
            this.panAnimationLayers.Location = new System.Drawing.Point(6, 19);
            this.panAnimationLayers.Name = "panAnimationLayers";
            this.panAnimationLayers.Size = new System.Drawing.Size(213, 313);
            this.panAnimationLayers.TabIndex = 7;
            this.panAnimationLayers.DragDrop += new System.Windows.Forms.DragEventHandler(this.panAnimationLayers_DragDrop);
            this.panAnimationLayers.DragEnter += new System.Windows.Forms.DragEventHandler(this.panAnimationLayers_DragEnter);
            this.panAnimationLayers.DragOver += new System.Windows.Forms.DragEventHandler(this.panAnimationLayers_DragOver);
            this.panAnimationLayers.Paint += new System.Windows.Forms.PaintEventHandler(this.panAnimationLayers_Paint);
            this.panAnimationLayers.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panAnimationLayers_MouseMove);
            this.panAnimationLayers.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panAnimationLayers_MouseUp);
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Location = new System.Drawing.Point(164, 338);
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(73, 23);
            this.btnRemoveLayer.TabIndex = 6;
            this.btnRemoveLayer.Text = "Remove";
            this.btnRemoveLayer.UseVisualStyleBackColor = true;
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(85, 338);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(73, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Hide Layer";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnAddLayer
            // 
            this.btnAddLayer.Location = new System.Drawing.Point(6, 338);
            this.btnAddLayer.Name = "btnAddLayer";
            this.btnAddLayer.Size = new System.Drawing.Size(73, 23);
            this.btnAddLayer.TabIndex = 4;
            this.btnAddLayer.Text = "Add Layer";
            this.btnAddLayer.UseVisualStyleBackColor = true;
            this.btnAddLayer.Click += new System.EventHandler(this.btnAddLayer_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStop.Location = new System.Drawing.Point(164, 386);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(73, 23);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPause.Location = new System.Drawing.Point(85, 386);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(73, 23);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPlay.Location = new System.Drawing.Point(6, 386);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(73, 23);
            this.btnPlay.TabIndex = 1;
            this.btnPlay.Text = "Play";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // pgAnimationProperties
            // 
            this.pgAnimationProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgAnimationProperties.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgAnimationProperties.Location = new System.Drawing.Point(0, 415);
            this.pgAnimationProperties.Name = "pgAnimationProperties";
            this.pgAnimationProperties.Size = new System.Drawing.Size(246, 261);
            this.pgAnimationProperties.TabIndex = 1;
            this.pgAnimationProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgAnimationProperties_PropertyValueChanged);
            // 
            // tmrAnimation
            // 
            this.tmrAnimation.Interval = 15;
            this.tmrAnimation.Tick += new System.EventHandler(this.tmrAnimation_Tick);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmProperties,
            this.tsmUndo});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(974, 24);
            this.mnuToolBar.TabIndex = 0;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmProperties
            // 
            this.tsmProperties.Name = "tsmProperties";
            this.tsmProperties.Size = new System.Drawing.Size(72, 20);
            this.tsmProperties.Text = "Properties";
            this.tsmProperties.Click += new System.EventHandler(this.tsmProperties_Click);
            // 
            // tsmUndo
            // 
            this.tsmUndo.Enabled = false;
            this.tsmUndo.Name = "tsmUndo";
            this.tsmUndo.Size = new System.Drawing.Size(48, 20);
            this.tsmUndo.Text = "Undo";
            this.tsmUndo.Click += new System.EventHandler(this.tsmUndo_Click);
            // 
            // ProjectEternityAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 704);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuToolBar);
            this.MainMenuStrip = this.mnuToolBar;
            this.MinimumSize = new System.Drawing.Size(990, 650);
            this.Name = "ProjectEternityAnimationEditor";
            this.Text = "Project Eternity Animation Editor";
            this.Shown += new System.EventHandler(this.ProjectEternityAnimationEditor_Shown);
            this.Resize += new System.EventHandler(this.ProjectEternityAnimationEditor_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.SplitContainer splitContainer1;
        internal System.Windows.Forms.SplitContainer splitContainer2;
        internal System.Windows.Forms.GroupBox groupBox1;
        internal System.Windows.Forms.PropertyGrid pgAnimationProperties;
        internal GameScreens.AnimationScreen.AnimationViewerControl AnimationViewer;
        internal System.Windows.Forms.Button btnPlay;
        internal System.Windows.Forms.Button btnPause;
        internal System.Windows.Forms.Timer tmrAnimation;
        internal System.Windows.Forms.Button btnStop;
        internal System.Windows.Forms.MenuStrip mnuToolBar;
        internal System.Windows.Forms.ToolStripMenuItem tsmSave;
        internal System.Windows.Forms.ToolStripMenuItem tsmProperties;
        internal System.Windows.Forms.Button btnAddLayer;
        internal System.Windows.Forms.Button btnRemoveLayer;
        internal System.Windows.Forms.Button button2;
        internal System.Windows.Forms.Panel panAnimationLayers;
        internal System.Windows.Forms.VScrollBar vsbAnimationLayer;
        public System.Windows.Forms.ToolStripMenuItem tsmUndo;
        public GameScreens.AnimationScreen.AnimationTimelineViewer panTimelineViewer;
    }
}

