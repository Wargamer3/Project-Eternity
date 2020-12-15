namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    partial class AnimationBackgroundEditor2D
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.AnimationBackgroundViewer = new ProjectEternity.Editors.AnimationBackgroundEditor.AnimationBackground2DViewerControl();
            this.pgAnimationProperties = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddBackground = new System.Windows.Forms.Button();
            this.lstItemChoices = new System.Windows.Forms.TreeView();
            this.btnRemoveBackground = new System.Windows.Forms.Button();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.statusStrip1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.AnimationBackgroundViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pgAnimationProperties);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(839, 484);
            this.splitContainer1.SplitterDistance = 644;
            this.splitContainer1.TabIndex = 1;
            // 
            // AnimationBackgroundViewer
            // 
            this.AnimationBackgroundViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnimationBackgroundViewer.Location = new System.Drawing.Point(0, 0);
            this.AnimationBackgroundViewer.Name = "AnimationBackgroundViewer";
            this.AnimationBackgroundViewer.Size = new System.Drawing.Size(640, 480);
            this.AnimationBackgroundViewer.TabIndex = 0;
            this.AnimationBackgroundViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseDown);
            this.AnimationBackgroundViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseMove);
            this.AnimationBackgroundViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseUp);
            // 
            // pgAnimationProperties
            // 
            this.pgAnimationProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgAnimationProperties.Location = new System.Drawing.Point(0, 262);
            this.pgAnimationProperties.Name = "pgAnimationProperties";
            this.pgAnimationProperties.Size = new System.Drawing.Size(187, 218);
            this.pgAnimationProperties.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnAddBackground);
            this.groupBox1.Controls.Add(this.lstItemChoices);
            this.groupBox1.Controls.Add(this.btnRemoveBackground);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 246);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Backgrounds";
            // 
            // btnAddBackground
            // 
            this.btnAddBackground.Location = new System.Drawing.Point(6, 188);
            this.btnAddBackground.Name = "btnAddBackground";
            this.btnAddBackground.Size = new System.Drawing.Size(168, 23);
            this.btnAddBackground.TabIndex = 6;
            this.btnAddBackground.Text = "Add Background";
            this.btnAddBackground.UseVisualStyleBackColor = true;
            this.btnAddBackground.Click += new System.EventHandler(this.btnAddBackground_Click);
            // 
            // lstItemChoices
            // 
            this.lstItemChoices.LabelEdit = true;
            this.lstItemChoices.Location = new System.Drawing.Point(6, 19);
            this.lstItemChoices.Name = "lstItemChoices";
            this.lstItemChoices.Size = new System.Drawing.Size(168, 163);
            this.lstItemChoices.TabIndex = 5;
            this.lstItemChoices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.lstItemChoices_AfterSelect);
            // 
            // btnRemoveBackground
            // 
            this.btnRemoveBackground.Location = new System.Drawing.Point(6, 217);
            this.btnRemoveBackground.Name = "btnRemoveBackground";
            this.btnRemoveBackground.Size = new System.Drawing.Size(168, 23);
            this.btnRemoveBackground.TabIndex = 4;
            this.btnRemoveBackground.Text = "Remove Background";
            this.btnRemoveBackground.UseVisualStyleBackColor = true;
            this.btnRemoveBackground.Click += new System.EventHandler(this.btnRemoveSprite_Click);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(839, 24);
            this.mnuToolBar.TabIndex = 2;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 508);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(839, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // AnimationBackgroundEditor2D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 530);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuToolBar);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.mnuToolBar;
            this.Name = "AnimationBackgroundEditor2D";
            this.Text = "Project Eternity Animation Background Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.PropertyGrid pgAnimationProperties;
        private System.Windows.Forms.Button btnRemoveBackground;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TreeView lstItemChoices;
        private System.Windows.Forms.Button btnAddBackground;
        private AnimationBackground2DViewerControl AnimationBackgroundViewer;
    }
}

