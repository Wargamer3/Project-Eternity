namespace ProjectEternity.Editors.AIEditor
{
    partial class AIViewer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsScriptMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmDeleteScript = new System.Windows.Forms.ToolStripMenuItem();
            this.sclScriptHeight = new System.Windows.Forms.VScrollBar();
            this.sclScriptWidth = new System.Windows.Forms.HScrollBar();
            this.panDrawingSurface = new System.Windows.Forms.Panel();
            this.cmsScriptMenu.SuspendLayout();
            this.panDrawingSurface.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsScriptMenu
            // 
            this.cmsScriptMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmDeleteScript});
            this.cmsScriptMenu.Name = "cmsScriptMenu";
            this.cmsScriptMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // tsmDeleteScript
            // 
            this.tsmDeleteScript.Name = "tsmDeleteScript";
            this.tsmDeleteScript.Size = new System.Drawing.Size(152, 22);
            this.tsmDeleteScript.Text = "DeleteScript";
            // 
            // sclScriptHeight
            // 
            this.sclScriptHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sclScriptHeight.Location = new System.Drawing.Point(133, 0);
            this.sclScriptHeight.Name = "sclScriptHeight";
            this.sclScriptHeight.Size = new System.Drawing.Size(17, 133);
            this.sclScriptHeight.TabIndex = 0;
            this.sclScriptHeight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclScriptHeight_Scroll);
            // 
            // sclScriptWidth
            // 
            this.sclScriptWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sclScriptWidth.Location = new System.Drawing.Point(0, 133);
            this.sclScriptWidth.Name = "sclScriptWidth";
            this.sclScriptWidth.Size = new System.Drawing.Size(133, 17);
            this.sclScriptWidth.TabIndex = 1;
            this.sclScriptWidth.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclScriptWidth_Scroll);
            // 
            // panDrawingSurface
            // 
            this.panDrawingSurface.Controls.Add(this.sclScriptHeight);
            this.panDrawingSurface.Controls.Add(this.sclScriptWidth);
            this.panDrawingSurface.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panDrawingSurface.Location = new System.Drawing.Point(0, 0);
            this.panDrawingSurface.Name = "panDrawingSurface";
            this.panDrawingSurface.Size = new System.Drawing.Size(150, 150);
            this.panDrawingSurface.TabIndex = 1;
            this.panDrawingSurface.Paint += new System.Windows.Forms.PaintEventHandler(this.panDrawingSurface_Paint);
            this.panDrawingSurface.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panDrawingSurface_MouseDown);
            this.panDrawingSurface.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panDrawingSurface_MouseMove);
            this.panDrawingSurface.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panDrawingSurface_MouseUp);
            this.panDrawingSurface.Resize += new System.EventHandler(this.panDrawingSurface_Resize);
            // 
            // AIViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panDrawingSurface);
            this.Name = "AIViewer";
            this.cmsScriptMenu.ResumeLayout(false);
            this.panDrawingSurface.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar sclScriptHeight;
        private System.Windows.Forms.HScrollBar sclScriptWidth;
        private System.Windows.Forms.Panel panDrawingSurface;
        public System.Windows.Forms.ToolStripMenuItem tsmDeleteScript;
        private System.Windows.Forms.ContextMenuStrip cmsScriptMenu;
    }
}
