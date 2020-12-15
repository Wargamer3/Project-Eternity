namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class AnimationViewerControl
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
            this.tsmAnimation = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEditOrigin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEditPolygon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmEditPolygonCutterOrigin = new System.Windows.Forms.ToolStripMenuItem();
            this.spawnNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.particlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsAnimationViewerOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsAnimationViewerOptions.SuspendLayout();
            this.SuspendLayout();
            //
            // cmsAnimationViewerOptions
            //
            this.cmsAnimationViewerOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spawnNewToolStripMenuItem,
            this.tsmEditOrigin,
            this.tsmEditPolygon,
            this.tsmEditPolygonCutterOrigin});
            this.cmsAnimationViewerOptions.Name = "contextMenuStrip1";
            this.cmsAnimationViewerOptions.Size = new System.Drawing.Size(214, 92);
            //
            // spawnNewToolStripMenuItem
            //
            this.spawnNewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.particlesToolStripMenuItem,
            this.tsmAnimation});
            this.spawnNewToolStripMenuItem.Name = "spawnNewToolStripMenuItem";
            this.spawnNewToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.spawnNewToolStripMenuItem.Text = "Spawn New";
            //
            // particlesToolStripMenuItem
            //
            this.particlesToolStripMenuItem.Name = "particlesToolStripMenuItem";
            this.particlesToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.particlesToolStripMenuItem.Text = "Particles";
            //
            // tsmAnimation
            //
            this.tsmAnimation.Name = "tsmAnimation";
            this.tsmAnimation.Size = new System.Drawing.Size(154, 22);
            this.tsmAnimation.Text = "Animation";
            this.tsmAnimation.Click += new System.EventHandler(this.tsmAnimation_Click);
            //
            // tsmEditOrigin
            //
            this.tsmEditOrigin.Name = "tsmEditOrigin";
            this.tsmEditOrigin.Size = new System.Drawing.Size(213, 22);
            this.tsmEditOrigin.Text = "Edit Origin";
            //
            // tsmEditPolygon
            //
            this.tsmEditPolygon.Name = "tsmEditPolygon";
            this.tsmEditPolygon.Size = new System.Drawing.Size(213, 22);
            this.tsmEditPolygon.Text = "Edit Polygon";
            this.tsmEditPolygon.Click += new System.EventHandler(this.tsmEditPolygon_Click);
            //
            // tsmEditPolygonCutterOrigin
            //
            this.tsmEditPolygonCutterOrigin.Name = "tsmEditPolygonCutterOrigin";
            this.tsmEditPolygonCutterOrigin.Size = new System.Drawing.Size(213, 22);
            this.tsmEditPolygonCutterOrigin.Text = "Edit Polygon Cutter Origin";
            this.tsmEditPolygonCutterOrigin.Click += new System.EventHandler(this.tsmEditPolygonCutterOrigin_Click);

            this.cmsAnimationViewerOptions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.ContextMenuStrip cmsAnimationViewerOptions;
        public System.Windows.Forms.ToolStripMenuItem spawnNewToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem particlesToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem tsmAnimation;
        internal System.Windows.Forms.ToolStripMenuItem tsmEditOrigin;
        internal System.Windows.Forms.ToolStripMenuItem tsmEditPolygon;
        internal System.Windows.Forms.ToolStripMenuItem tsmEditPolygonCutterOrigin;
    }
}
