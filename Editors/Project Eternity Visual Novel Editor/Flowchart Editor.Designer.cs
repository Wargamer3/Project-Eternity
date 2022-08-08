
namespace ProjectEternity.Editors.VisualNovelEditor
{
    partial class FlowchartEditor
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
            this.VisualNovelViewer = new ProjectEternity.Editors.VisualNovelEditor.VisualNovelViewerControl();
            this.cmsScriptEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dfsfToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmsNewDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsScriptEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // VisualNovelViewer
            // 
            this.VisualNovelViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisualNovelViewer.Location = new System.Drawing.Point(0, 0);
            this.VisualNovelViewer.Name = "VisualNovelViewer";
            this.VisualNovelViewer.Size = new System.Drawing.Size(800, 450);
            this.VisualNovelViewer.TabIndex = 2;
            this.VisualNovelViewer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbVisualNovelPreview_MouseClick);
            this.VisualNovelViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbVisualNovelPreview_MouseDown);
            this.VisualNovelViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbVisualNovelPreview_MouseMove);
            // 
            // cmsScriptEditor
            // 
            this.cmsScriptEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dfsfToolStripMenuItem,
            this.tsmEdit,
            this.tsmDelete});
            this.cmsScriptEditor.Name = "contextMenuStrip1";
            this.cmsScriptEditor.Size = new System.Drawing.Size(108, 70);
            // 
            // dfsfToolStripMenuItem
            // 
            this.dfsfToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmsNewDialog});
            this.dfsfToolStripMenuItem.Name = "dfsfToolStripMenuItem";
            this.dfsfToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.dfsfToolStripMenuItem.Text = "New";
            // 
            // tmsNewDialog
            // 
            this.tmsNewDialog.Name = "tmsNewDialog";
            this.tmsNewDialog.Size = new System.Drawing.Size(108, 22);
            this.tmsNewDialog.Text = "Dialog";
            // 
            // tsmEdit
            // 
            this.tsmEdit.Name = "tsmEdit";
            this.tsmEdit.Size = new System.Drawing.Size(107, 22);
            this.tsmEdit.Text = "Edit";
            // 
            // tsmDelete
            // 
            this.tsmDelete.Name = "tsmDelete";
            this.tsmDelete.Size = new System.Drawing.Size(107, 22);
            this.tsmDelete.Text = "Delete";
            // 
            // FlowchartEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.VisualNovelViewer);
            this.Name = "FlowchartEditor";
            this.Text = "Flowchart Editor";
            this.cmsScriptEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private VisualNovelViewerControl VisualNovelViewer;
        private System.Windows.Forms.ContextMenuStrip cmsScriptEditor;
        private System.Windows.Forms.ToolStripMenuItem dfsfToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tmsNewDialog;
        private System.Windows.Forms.ToolStripMenuItem tsmEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmDelete;
    }
}