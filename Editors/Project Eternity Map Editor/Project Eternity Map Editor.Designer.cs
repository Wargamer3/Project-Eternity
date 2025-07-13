namespace ProjectEternity.Editors.MapEditor
{
    partial class ProjectEternityMapEditor
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
            this.tabToolBox = new System.Windows.Forms.TabControl();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmMapProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.BattleMapViewer = new ProjectEternity.GameScreens.BattleMapScreen.BattleMapViewerControl();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tslInformation = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmGlobalEnvironment = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabToolBox
            // 
            this.tabToolBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabToolBox.Location = new System.Drawing.Point(0, 0);
            this.tabToolBox.Name = "tabToolBox";
            this.tabToolBox.SelectedIndex = 0;
            this.tabToolBox.Size = new System.Drawing.Size(333, 523);
            this.tabToolBox.TabIndex = 1;
            this.tabToolBox.SelectedIndexChanged += new System.EventHandler(this.tabToolBox_SelectedIndexChanged);
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmMapProperties
            // 
            this.tsmMapProperties.Name = "tsmMapProperties";
            this.tsmMapProperties.Size = new System.Drawing.Size(99, 20);
            this.tsmMapProperties.Text = "Map properties";
            this.tsmMapProperties.Click += new System.EventHandler(this.tsmMapProperties_Click);
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 24);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.BattleMapViewer);
            this.splitContainer.Panel1MinSize = 664;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tabToolBox);
            this.splitContainer.Panel2MinSize = 252;
            this.splitContainer.Size = new System.Drawing.Size(1247, 527);
            this.splitContainer.SplitterDistance = 906;
            this.splitContainer.TabIndex = 10;
            // 
            // BattleMapViewer
            // 
            this.BattleMapViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BattleMapViewer.Location = new System.Drawing.Point(0, 0);
            this.BattleMapViewer.Name = "BattleMapViewer";
            this.BattleMapViewer.Size = new System.Drawing.Size(902, 523);
            this.BattleMapViewer.TabIndex = 0;
            this.BattleMapViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnMapPreview_MouseDown);
            this.BattleMapViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnMapPreview_MouseMove);
            this.BattleMapViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnMapPreview_MouseUp);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslInformation});
            this.statusStrip.Location = new System.Drawing.Point(0, 551);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1247, 22);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "statusStrip1";
            // 
            // tslInformation
            // 
            this.tslInformation.Name = "tslInformation";
            this.tslInformation.Size = new System.Drawing.Size(0, 17);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmMapProperties,
            this.tsmGlobalEnvironment});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(1247, 24);
            this.mnuToolBar.TabIndex = 9;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmGlobalEnvironment
            // 
            this.tsmGlobalEnvironment.Name = "tsmGlobalEnvironment";
            this.tsmGlobalEnvironment.Size = new System.Drawing.Size(124, 20);
            this.tsmGlobalEnvironment.Text = "Global Environment";
            this.tsmGlobalEnvironment.Click += new System.EventHandler(this.tsmGlobalEnvironment_Click);
            // 
            // ProjectEternityMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1247, 573);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "ProjectEternityMapEditor";
            this.Shown += new System.EventHandler(this.ProjectEternityMapEditor_Shown);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.TabControl tabToolBox;

        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripMenuItem tsmMapProperties;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tslInformation;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        protected GameScreens.BattleMapScreen.BattleMapViewerControl BattleMapViewer;
        private System.Windows.Forms.ToolStripMenuItem tsmGlobalEnvironment;
        public System.Windows.Forms.SplitContainer splitContainer;
    }
}
