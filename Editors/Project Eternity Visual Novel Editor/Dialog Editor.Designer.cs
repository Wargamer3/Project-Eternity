namespace ProjectEternity.Editors.VisualNovelEditor
{
    partial class DialogEditor
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
            this.cutsceneViewer = new ProjectEternity.Editors.CutsceneEditor.CutsceneViewer();
            this.ScriptingContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pgScriptProperties = new System.Windows.Forms.PropertyGrid();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.rbBeforeDialog = new System.Windows.Forms.RadioButton();
            this.rbAfterDialog = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).BeginInit();
            this.ScriptingContainer.Panel1.SuspendLayout();
            this.ScriptingContainer.Panel2.SuspendLayout();
            this.ScriptingContainer.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.cutsceneViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.ScriptingContainer);
            this.splitContainer1.Size = new System.Drawing.Size(984, 553);
            this.splitContainer1.SplitterDistance = 720;
            this.splitContainer1.TabIndex = 2;
            // 
            // cutsceneViewer
            // 
            this.cutsceneViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cutsceneViewer.Location = new System.Drawing.Point(0, 0);
            this.cutsceneViewer.Name = "cutsceneViewer";
            this.cutsceneViewer.Size = new System.Drawing.Size(716, 549);
            this.cutsceneViewer.TabIndex = 0;
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
            // 
            // pgScriptProperties
            // 
            this.pgScriptProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgScriptProperties.Location = new System.Drawing.Point(0, 0);
            this.pgScriptProperties.Name = "pgScriptProperties";
            this.pgScriptProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgScriptProperties.Size = new System.Drawing.Size(256, 272);
            this.pgScriptProperties.TabIndex = 0;
            this.pgScriptProperties.ToolbarVisible = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // rbBeforeDialog
            // 
            this.rbBeforeDialog.AutoSize = true;
            this.rbBeforeDialog.Location = new System.Drawing.Point(12, 3);
            this.rbBeforeDialog.Name = "rbBeforeDialog";
            this.rbBeforeDialog.Size = new System.Drawing.Size(89, 17);
            this.rbBeforeDialog.TabIndex = 4;
            this.rbBeforeDialog.TabStop = true;
            this.rbBeforeDialog.Text = "Before Dialog";
            this.rbBeforeDialog.UseVisualStyleBackColor = true;
            this.rbBeforeDialog.CheckedChanged += new System.EventHandler(this.rbBeforeDialog_CheckedChanged);
            // 
            // rbAfterDialog
            // 
            this.rbAfterDialog.AutoSize = true;
            this.rbAfterDialog.Location = new System.Drawing.Point(103, 3);
            this.rbAfterDialog.Name = "rbAfterDialog";
            this.rbAfterDialog.Size = new System.Drawing.Size(80, 17);
            this.rbAfterDialog.TabIndex = 5;
            this.rbAfterDialog.TabStop = true;
            this.rbAfterDialog.Text = "After Dialog";
            this.rbAfterDialog.UseVisualStyleBackColor = true;
            this.rbAfterDialog.CheckedChanged += new System.EventHandler(this.rbAfterDialog_CheckedChanged);
            // 
            // DialogEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 577);
            this.Controls.Add(this.rbAfterDialog);
            this.Controls.Add(this.rbBeforeDialog);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "DialogEditor";
            this.Text = "Dialog Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ScriptingContainer.Panel1.ResumeLayout(false);
            this.ScriptingContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ScriptingContainer)).EndInit();
            this.ScriptingContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private CutsceneEditor.CutsceneViewer cutsceneViewer;
        private System.Windows.Forms.SplitContainer ScriptingContainer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.PropertyGrid pgScriptProperties;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.RadioButton rbBeforeDialog;
        private System.Windows.Forms.RadioButton rbAfterDialog;
    }
}