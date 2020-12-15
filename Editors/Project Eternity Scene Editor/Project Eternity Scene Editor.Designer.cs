namespace ProjectEternity.Editors.SceneEditor
{
    partial class SceneEditor
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.ScriptingContainer = new System.Windows.Forms.SplitContainer();
            this.ScenePreviewViewer = new ProjectEternity.Editors.SceneEditor.ScenePreviewViewerControl();
            this.tcSceneEvents = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.SceneTimelineViewer = new ProjectEternity.Editors.SceneEditor.SceneTimelineViewerControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSceneEvents = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).BeginInit();
            this.ScriptingContainer.Panel1.SuspendLayout();
            this.ScriptingContainer.Panel2.SuspendLayout();
            this.ScriptingContainer.SuspendLayout();
            this.tcSceneEvents.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.ScriptingContainer);
            this.splitContainer1.Size = new System.Drawing.Size(984, 553);
            this.splitContainer1.SplitterDistance = 720;
            this.splitContainer1.TabIndex = 0;
            // 
            // ScriptingContainer
            // 
            this.ScriptingContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ScriptingContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScriptingContainer.Location = new System.Drawing.Point(0, 0);
            this.ScriptingContainer.Name = "ScriptingContainer";
            this.ScriptingContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ScriptingContainer.Panel1
            // 
            this.ScriptingContainer.Panel1.Controls.Add(this.ScenePreviewViewer);
            // 
            // ScriptingContainer.Panel2
            // 
            this.ScriptingContainer.Panel2.Controls.Add(this.tcSceneEvents);
            this.ScriptingContainer.Size = new System.Drawing.Size(720, 553);
            this.ScriptingContainer.SplitterDistance = 271;
            this.ScriptingContainer.TabIndex = 6;
            // 
            // ScenePreviewViewer
            // 
            this.ScenePreviewViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ScenePreviewViewer.Location = new System.Drawing.Point(0, 0);
            this.ScenePreviewViewer.Name = "ScenePreviewViewer";
            this.ScenePreviewViewer.Size = new System.Drawing.Size(716, 267);
            this.ScenePreviewViewer.TabIndex = 0;
            this.ScenePreviewViewer.Text = "scenePreviewViewerControl1";
            // 
            // tcSceneEvents
            // 
            this.tcSceneEvents.Controls.Add(this.tabMain);
            this.tcSceneEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcSceneEvents.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tcSceneEvents.Location = new System.Drawing.Point(0, 0);
            this.tcSceneEvents.Name = "tcSceneEvents";
            this.tcSceneEvents.SelectedIndex = 0;
            this.tcSceneEvents.Size = new System.Drawing.Size(716, 274);
            this.tcSceneEvents.TabIndex = 0;
            this.tcSceneEvents.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tcSceneEvents_DrawItem);
            this.tcSceneEvents.SelectedIndexChanged += new System.EventHandler(this.tcSceneEvents_SelectedIndexChanged);
            this.tcSceneEvents.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tcSceneEvents_MouseClick);
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.SceneTimelineViewer);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(708, 248);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // SceneTimelineViewer
            // 
            this.SceneTimelineViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SceneTimelineViewer.Location = new System.Drawing.Point(3, 3);
            this.SceneTimelineViewer.Name = "SceneTimelineViewer";
            this.SceneTimelineViewer.Size = new System.Drawing.Size(702, 242);
            this.SceneTimelineViewer.TabIndex = 0;
            this.SceneTimelineViewer.Text = "sceneViewerControl1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmProperties});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
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
            // cmsSceneEvents
            // 
            this.cmsSceneEvents.Name = "cmsSceneEvents";
            this.cmsSceneEvents.Size = new System.Drawing.Size(61, 4);
            // 
            // SceneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 577);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SceneEditor";
            this.Text = "Scene Editor";
            this.Shown += new System.EventHandler(this.SceneEditor_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ScriptingContainer.Panel1.ResumeLayout(false);
            this.ScriptingContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).EndInit();
            this.ScriptingContainer.ResumeLayout(false);
            this.tcSceneEvents.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.SplitContainer ScriptingContainer;
        private System.Windows.Forms.TabPage tabMain;
        private SceneTimelineViewerControl SceneTimelineViewer;
        private ScenePreviewViewerControl ScenePreviewViewer;
        public System.Windows.Forms.ContextMenuStrip cmsSceneEvents;
        private System.Windows.Forms.ToolStripMenuItem tsmProperties;
        public System.Windows.Forms.TabControl tcSceneEvents;
    }
}

