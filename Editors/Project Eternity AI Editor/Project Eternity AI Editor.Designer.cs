﻿namespace ProjectEternity.Editors.AIEditor
{
    partial class AIEditor
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
            this.aiViewer = new ProjectEternity.Editors.AIEditor.AIViewer();
            this.ScriptingContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pgScriptProperties = new System.Windows.Forms.PropertyGrid();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).BeginInit();
            this.ScriptingContainer.Panel1.SuspendLayout();
            this.ScriptingContainer.Panel2.SuspendLayout();
            this.ScriptingContainer.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.aiViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ScriptingContainer);
            this.splitContainer1.Size = new System.Drawing.Size(984, 553);
            this.splitContainer1.SplitterDistance = 720;
            this.splitContainer1.TabIndex = 0;
            // 
            // aiViewer
            // 
            this.aiViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.aiViewer.Location = new System.Drawing.Point(0, 0);
            this.aiViewer.Name = "aiViewer";
            this.aiViewer.Size = new System.Drawing.Size(716, 549);
            this.aiViewer.TabIndex = 0;
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
            this.ScriptingContainer.Panel1.Controls.Add(this.tabControl1);
            // 
            // ScriptingContainer.Panel2
            // 
            this.ScriptingContainer.Panel2.Controls.Add(this.pgScriptProperties);
            this.ScriptingContainer.Size = new System.Drawing.Size(260, 553);
            this.ScriptingContainer.SplitterDistance = 273;
            this.ScriptingContainer.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(256, 269);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DoubleClick += new System.EventHandler(this.lstChoices_DoubleClick);
            // 
            // pgScriptProperties
            // 
            this.pgScriptProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgScriptProperties.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgScriptProperties.Location = new System.Drawing.Point(0, 0);
            this.pgScriptProperties.Name = "pgScriptProperties";
            this.pgScriptProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgScriptProperties.Size = new System.Drawing.Size(256, 272);
            this.pgScriptProperties.TabIndex = 0;
            this.pgScriptProperties.ToolbarVisible = false;
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(984, 24);
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
            // AIEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 577);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuToolBar);
            this.MainMenuStrip = this.mnuToolBar;
            this.Name = "AIEditor";
            this.Text = "AI Editor";
            this.Shown += new System.EventHandler(this.AIEditor_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ScriptingContainer.Panel1.ResumeLayout(false);
            this.ScriptingContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).EndInit();
            this.ScriptingContainer.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.SplitContainer ScriptingContainer;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.PropertyGrid pgScriptProperties;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private AIViewer aiViewer;
    }
}

