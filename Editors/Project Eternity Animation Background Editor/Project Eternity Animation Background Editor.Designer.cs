namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    partial class AnimationBackgroundEditor
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
            this.AnimationBackgroundViewer = new ProjectEternity.Editors.AnimationBackgroundEditor.AnimationBackgroundViewerControl();
            this.pgAnimationProperties = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCreateNewProp = new System.Windows.Forms.Button();
            this.lstItemChoices = new System.Windows.Forms.TreeView();
            this.btnRemoveSprite = new System.Windows.Forms.Button();
            this.btnLoadNewBackgroundType = new System.Windows.Forms.Button();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsmProperties = new System.Windows.Forms.ToolStripMenuItem();
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
            this.splitContainer1.Size = new System.Drawing.Size(768, 477);
            this.splitContainer1.SplitterDistance = 573;
            this.splitContainer1.TabIndex = 1;
            // 
            // AnimationBackgroundViewer
            // 
            this.AnimationBackgroundViewer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AnimationBackgroundViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AnimationBackgroundViewer.Location = new System.Drawing.Point(0, 0);
            this.AnimationBackgroundViewer.Name = "AnimationBackgroundViewer";
            this.AnimationBackgroundViewer.Size = new System.Drawing.Size(569, 473);
            this.AnimationBackgroundViewer.TabIndex = 0;
            this.AnimationBackgroundViewer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AnimationBackgroundViewer_KeyDown);
            this.AnimationBackgroundViewer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.AnimationBackgroundViewer_KeyUp);
            this.AnimationBackgroundViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseDown);
            this.AnimationBackgroundViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseMove);
            this.AnimationBackgroundViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseUp);
            this.AnimationBackgroundViewer.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.AnimationBackgroundViewer_MouseWheel);
            // 
            // pgAnimationProperties
            // 
            this.pgAnimationProperties.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pgAnimationProperties.Location = new System.Drawing.Point(0, 255);
            this.pgAnimationProperties.Name = "pgAnimationProperties";
            this.pgAnimationProperties.Size = new System.Drawing.Size(187, 218);
            this.pgAnimationProperties.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCreateNewProp);
            this.groupBox1.Controls.Add(this.lstItemChoices);
            this.groupBox1.Controls.Add(this.btnRemoveSprite);
            this.groupBox1.Controls.Add(this.btnLoadNewBackgroundType);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(181, 246);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Item Choices";
            // 
            // btnCreateNewProp
            // 
            this.btnCreateNewProp.Location = new System.Drawing.Point(6, 188);
            this.btnCreateNewProp.Name = "btnCreateNewProp";
            this.btnCreateNewProp.Size = new System.Drawing.Size(168, 23);
            this.btnCreateNewProp.TabIndex = 6;
            this.btnCreateNewProp.Text = "Create New Prop";
            this.btnCreateNewProp.UseVisualStyleBackColor = true;
            this.btnCreateNewProp.Click += new System.EventHandler(this.btnCreateNewProp_Click);
            // 
            // lstItemChoices
            // 
            this.lstItemChoices.LabelEdit = true;
            this.lstItemChoices.Location = new System.Drawing.Point(6, 19);
            this.lstItemChoices.Name = "lstItemChoices";
            this.lstItemChoices.Size = new System.Drawing.Size(168, 134);
            this.lstItemChoices.TabIndex = 5;
            this.lstItemChoices.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.lstItemChoices_AfterSelect);
            // 
            // btnRemoveSprite
            // 
            this.btnRemoveSprite.Location = new System.Drawing.Point(6, 217);
            this.btnRemoveSprite.Name = "btnRemoveSprite";
            this.btnRemoveSprite.Size = new System.Drawing.Size(168, 23);
            this.btnRemoveSprite.TabIndex = 4;
            this.btnRemoveSprite.Text = "Remove Item";
            this.btnRemoveSprite.UseVisualStyleBackColor = true;
            this.btnRemoveSprite.Click += new System.EventHandler(this.btnRemoveItem_Click);
            // 
            // btnLoadNewBackgroundType
            // 
            this.btnLoadNewBackgroundType.Location = new System.Drawing.Point(6, 159);
            this.btnLoadNewBackgroundType.Name = "btnLoadNewBackgroundType";
            this.btnLoadNewBackgroundType.Size = new System.Drawing.Size(168, 23);
            this.btnLoadNewBackgroundType.TabIndex = 3;
            this.btnLoadNewBackgroundType.Text = "Load New Background Type";
            this.btnLoadNewBackgroundType.UseVisualStyleBackColor = true;
            this.btnLoadNewBackgroundType.Click += new System.EventHandler(this.btnLoadNewBackgroundType_Click);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmProperties});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(768, 24);
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 501);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(768, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // tsmProperties
            // 
            this.tsmProperties.Name = "tsmProperties";
            this.tsmProperties.Size = new System.Drawing.Size(72, 20);
            this.tsmProperties.Text = "Properties";
            this.tsmProperties.Click += new System.EventHandler(this.tsmProperties_Click);
            // 
            // AnimationBackgroundEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 523);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuToolBar);
            this.Controls.Add(this.statusStrip1);
            this.MainMenuStrip = this.mnuToolBar;
            this.Name = "AnimationBackgroundEditor";
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

        private AnimationBackgroundViewerControl AnimationBackgroundViewer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.PropertyGrid pgAnimationProperties;
        private System.Windows.Forms.Button btnRemoveSprite;
        private System.Windows.Forms.Button btnLoadNewBackgroundType;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TreeView lstItemChoices;
        private System.Windows.Forms.Button btnCreateNewProp;
        private System.Windows.Forms.ToolStripMenuItem tsmProperties;
    }
}

