namespace ProjectEternity.Editors.RacingMapEditor
{
    partial class ProjectEternityRacingMapEditor
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
            this.tabAITunnels = new System.Windows.Forms.TabPage();
            this.gbAITunnelProperties = new System.Windows.Forms.GroupBox();
            this.pgAITunnel = new System.Windows.Forms.PropertyGrid();
            this.gbAITunnels = new System.Windows.Forms.GroupBox();
            this.lvAItunnels = new System.Windows.Forms.ListView();
            this.btnRemoveAITunnel = new System.Windows.Forms.Button();
            this.btnAddAITunnel = new System.Windows.Forms.Button();
            this.tabCollisionBoxes = new System.Windows.Forms.TabPage();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.RacingMapViewer = new ProjectEternity.Editors.RacingMapEditor.RacingMapViewerControl();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tslInformation = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.gbCollisionBoxProperties = new System.Windows.Forms.GroupBox();
            this.pgCollisionBox = new System.Windows.Forms.PropertyGrid();
            this.gbCollisionBoxes = new System.Windows.Forms.GroupBox();
            this.lvCollisionsBoxes = new System.Windows.Forms.ListView();
            this.btnRemoveCollisionBox = new System.Windows.Forms.Button();
            this.btnAddCollisionBox = new System.Windows.Forms.Button();
            this.tabToolBox.SuspendLayout();
            this.tabAITunnels.SuspendLayout();
            this.gbAITunnelProperties.SuspendLayout();
            this.gbAITunnels.SuspendLayout();
            this.tabCollisionBoxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.gbCollisionBoxProperties.SuspendLayout();
            this.gbCollisionBoxes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabToolBox
            // 
            this.tabToolBox.Controls.Add(this.tabAITunnels);
            this.tabToolBox.Controls.Add(this.tabCollisionBoxes);
            this.tabToolBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabToolBox.Location = new System.Drawing.Point(0, 0);
            this.tabToolBox.Name = "tabToolBox";
            this.tabToolBox.SelectedIndex = 0;
            this.tabToolBox.Size = new System.Drawing.Size(248, 523);
            this.tabToolBox.TabIndex = 1;
            // 
            // tabAITunnels
            // 
            this.tabAITunnels.Controls.Add(this.gbAITunnelProperties);
            this.tabAITunnels.Controls.Add(this.gbAITunnels);
            this.tabAITunnels.Location = new System.Drawing.Point(4, 22);
            this.tabAITunnels.Name = "tabAITunnels";
            this.tabAITunnels.Padding = new System.Windows.Forms.Padding(3);
            this.tabAITunnels.Size = new System.Drawing.Size(240, 497);
            this.tabAITunnels.TabIndex = 2;
            this.tabAITunnels.Text = "AI Tunnels";
            this.tabAITunnels.UseVisualStyleBackColor = true;
            // 
            // gbAITunnelProperties
            // 
            this.gbAITunnelProperties.Controls.Add(this.pgAITunnel);
            this.gbAITunnelProperties.Location = new System.Drawing.Point(0, 262);
            this.gbAITunnelProperties.Name = "gbAITunnelProperties";
            this.gbAITunnelProperties.Size = new System.Drawing.Size(237, 235);
            this.gbAITunnelProperties.TabIndex = 5;
            this.gbAITunnelProperties.TabStop = false;
            this.gbAITunnelProperties.Text = "AI Tunnel Properties";
            // 
            // pgAITunnel
            // 
            this.pgAITunnel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgAITunnel.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgAITunnel.Location = new System.Drawing.Point(3, 16);
            this.pgAITunnel.Name = "pgAITunnel";
            this.pgAITunnel.Size = new System.Drawing.Size(231, 216);
            this.pgAITunnel.TabIndex = 1;
            // 
            // gbAITunnels
            // 
            this.gbAITunnels.Controls.Add(this.lvAItunnels);
            this.gbAITunnels.Controls.Add(this.btnRemoveAITunnel);
            this.gbAITunnels.Controls.Add(this.btnAddAITunnel);
            this.gbAITunnels.Location = new System.Drawing.Point(0, 0);
            this.gbAITunnels.Name = "gbAITunnels";
            this.gbAITunnels.Size = new System.Drawing.Size(240, 256);
            this.gbAITunnels.TabIndex = 4;
            this.gbAITunnels.TabStop = false;
            this.gbAITunnels.Text = "AI Tunnels";
            // 
            // lvAItunnels
            // 
            this.lvAItunnels.Location = new System.Drawing.Point(6, 19);
            this.lvAItunnels.Name = "lvAItunnels";
            this.lvAItunnels.Size = new System.Drawing.Size(228, 202);
            this.lvAItunnels.TabIndex = 4;
            this.lvAItunnels.UseCompatibleStateImageBehavior = false;
            this.lvAItunnels.View = System.Windows.Forms.View.List;
            this.lvAItunnels.SelectedIndexChanged += new System.EventHandler(this.lvAItunnels_SelectedIndexChanged);
            // 
            // btnRemoveAITunnel
            // 
            this.btnRemoveAITunnel.Location = new System.Drawing.Point(128, 227);
            this.btnRemoveAITunnel.Name = "btnRemoveAITunnel";
            this.btnRemoveAITunnel.Size = new System.Drawing.Size(106, 23);
            this.btnRemoveAITunnel.TabIndex = 3;
            this.btnRemoveAITunnel.Text = "Remove AI Tunnel";
            this.btnRemoveAITunnel.UseVisualStyleBackColor = true;
            this.btnRemoveAITunnel.Click += new System.EventHandler(this.btnRemoveAITunnel_Click);
            // 
            // btnAddAITunnel
            // 
            this.btnAddAITunnel.Location = new System.Drawing.Point(6, 227);
            this.btnAddAITunnel.Name = "btnAddAITunnel";
            this.btnAddAITunnel.Size = new System.Drawing.Size(106, 23);
            this.btnAddAITunnel.TabIndex = 2;
            this.btnAddAITunnel.Text = "Add AI Tunnel";
            this.btnAddAITunnel.UseVisualStyleBackColor = true;
            this.btnAddAITunnel.Click += new System.EventHandler(this.btnAddAITunnel_Click);
            // 
            // tabCollisionBoxes
            // 
            this.tabCollisionBoxes.Controls.Add(this.gbCollisionBoxProperties);
            this.tabCollisionBoxes.Controls.Add(this.gbCollisionBoxes);
            this.tabCollisionBoxes.Cursor = System.Windows.Forms.Cursors.Default;
            this.tabCollisionBoxes.Location = new System.Drawing.Point(4, 22);
            this.tabCollisionBoxes.Name = "tabCollisionBoxes";
            this.tabCollisionBoxes.Size = new System.Drawing.Size(240, 497);
            this.tabCollisionBoxes.TabIndex = 3;
            this.tabCollisionBoxes.Text = "Collision Boxes";
            this.tabCollisionBoxes.UseVisualStyleBackColor = true;
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
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
            this.splitContainer1.Panel1.Controls.Add(this.RacingMapViewer);
            this.splitContainer1.Panel1MinSize = 664;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabToolBox);
            this.splitContainer1.Panel2MinSize = 252;
            this.splitContainer1.Size = new System.Drawing.Size(928, 527);
            this.splitContainer1.SplitterDistance = 672;
            this.splitContainer1.TabIndex = 10;
            // 
            // RacingMapViewer
            // 
            this.RacingMapViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RacingMapViewer.Location = new System.Drawing.Point(0, 0);
            this.RacingMapViewer.Name = "RacingMapViewer";
            this.RacingMapViewer.Size = new System.Drawing.Size(668, 523);
            this.RacingMapViewer.TabIndex = 0;
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(928, 24);
            this.mnuToolBar.TabIndex = 9;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tslInformation
            // 
            this.tslInformation.Name = "tslInformation";
            this.tslInformation.Size = new System.Drawing.Size(0, 17);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tslInformation});
            this.statusStrip1.Location = new System.Drawing.Point(0, 551);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(928, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // gbCollisionBoxProperties
            // 
            this.gbCollisionBoxProperties.Controls.Add(this.pgCollisionBox);
            this.gbCollisionBoxProperties.Location = new System.Drawing.Point(0, 262);
            this.gbCollisionBoxProperties.Name = "gbCollisionBoxProperties";
            this.gbCollisionBoxProperties.Size = new System.Drawing.Size(237, 235);
            this.gbCollisionBoxProperties.TabIndex = 7;
            this.gbCollisionBoxProperties.TabStop = false;
            this.gbCollisionBoxProperties.Text = "Collision Box Properties";
            // 
            // pgCollisionBox
            // 
            this.pgCollisionBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgCollisionBox.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgCollisionBox.Location = new System.Drawing.Point(3, 16);
            this.pgCollisionBox.Name = "pgCollisionBox";
            this.pgCollisionBox.Size = new System.Drawing.Size(231, 216);
            this.pgCollisionBox.TabIndex = 1;
            // 
            // gbCollisionBoxes
            // 
            this.gbCollisionBoxes.Controls.Add(this.lvCollisionsBoxes);
            this.gbCollisionBoxes.Controls.Add(this.btnRemoveCollisionBox);
            this.gbCollisionBoxes.Controls.Add(this.btnAddCollisionBox);
            this.gbCollisionBoxes.Location = new System.Drawing.Point(0, 0);
            this.gbCollisionBoxes.Name = "gbCollisionBoxes";
            this.gbCollisionBoxes.Size = new System.Drawing.Size(240, 256);
            this.gbCollisionBoxes.TabIndex = 6;
            this.gbCollisionBoxes.TabStop = false;
            this.gbCollisionBoxes.Text = "Collision Boxes";
            // 
            // lvCollisionsBoxes
            // 
            this.lvCollisionsBoxes.Location = new System.Drawing.Point(6, 19);
            this.lvCollisionsBoxes.Name = "lvCollisionsBoxes";
            this.lvCollisionsBoxes.Size = new System.Drawing.Size(228, 202);
            this.lvCollisionsBoxes.TabIndex = 4;
            this.lvCollisionsBoxes.UseCompatibleStateImageBehavior = false;
            this.lvCollisionsBoxes.View = System.Windows.Forms.View.List;
            this.lvCollisionsBoxes.SelectedIndexChanged += new System.EventHandler(this.lvCollisionsBoxes_SelectedIndexChanged);
            // 
            // btnRemoveCollisionBox
            // 
            this.btnRemoveCollisionBox.Location = new System.Drawing.Point(128, 227);
            this.btnRemoveCollisionBox.Name = "btnRemoveCollisionBox";
            this.btnRemoveCollisionBox.Size = new System.Drawing.Size(106, 23);
            this.btnRemoveCollisionBox.TabIndex = 3;
            this.btnRemoveCollisionBox.Text = "Remove Box";
            this.btnRemoveCollisionBox.UseVisualStyleBackColor = true;
            this.btnRemoveCollisionBox.Click += new System.EventHandler(this.btnRemoveCollisionBox_Click);
            // 
            // btnAddCollisionBox
            // 
            this.btnAddCollisionBox.Location = new System.Drawing.Point(6, 227);
            this.btnAddCollisionBox.Name = "btnAddCollisionBox";
            this.btnAddCollisionBox.Size = new System.Drawing.Size(106, 23);
            this.btnAddCollisionBox.TabIndex = 2;
            this.btnAddCollisionBox.Text = "Add Collision Box";
            this.btnAddCollisionBox.UseVisualStyleBackColor = true;
            this.btnAddCollisionBox.Click += new System.EventHandler(this.btnAddCollisionBox_Click);
            // 
            // ProjectEternityRacingMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(928, 573);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "ProjectEternityRacingMapEditor";
            this.Shown += new System.EventHandler(this.ProjectEternityRacingEditor_Shown);
            this.tabToolBox.ResumeLayout(false);
            this.tabAITunnels.ResumeLayout(false);
            this.gbAITunnelProperties.ResumeLayout(false);
            this.gbAITunnels.ResumeLayout(false);
            this.tabCollisionBoxes.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbCollisionBoxProperties.ResumeLayout(false);
            this.gbCollisionBoxes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabToolBox;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private RacingMapViewerControl RacingMapViewer;
        private System.Windows.Forms.TabPage tabAITunnels;
        private System.Windows.Forms.ToolStripStatusLabel tslInformation;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.GroupBox gbAITunnelProperties;
        private System.Windows.Forms.PropertyGrid pgAITunnel;
        private System.Windows.Forms.GroupBox gbAITunnels;
        private System.Windows.Forms.Button btnRemoveAITunnel;
        private System.Windows.Forms.Button btnAddAITunnel;
        private System.Windows.Forms.TabPage tabCollisionBoxes;
        private System.Windows.Forms.ListView lvAItunnels;
        private System.Windows.Forms.GroupBox gbCollisionBoxProperties;
        private System.Windows.Forms.PropertyGrid pgCollisionBox;
        private System.Windows.Forms.GroupBox gbCollisionBoxes;
        private System.Windows.Forms.ListView lvCollisionsBoxes;
        private System.Windows.Forms.Button btnRemoveCollisionBox;
        private System.Windows.Forms.Button btnAddCollisionBox;
    }
}