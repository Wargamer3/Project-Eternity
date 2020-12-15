namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class TilesetOriginEditor
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
            this.TilesetViewer = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.SuspendLayout();
            // 
            // TilesetViewer
            // 
            this.TilesetViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TilesetViewer.Location = new System.Drawing.Point(0, 0);
            this.TilesetViewer.Name = "TilesetViewer";
            this.TilesetViewer.Size = new System.Drawing.Size(305, 298);
            this.TilesetViewer.TabIndex = 0;
            this.TilesetViewer.Text = "tilesetViewerControl1";
            // 
            // TilesetOriginEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(305, 298);
            this.Controls.Add(this.TilesetViewer);
            this.Name = "TilesetOriginEditor";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        public BattleMapScreen.TilesetViewerControl TilesetViewer;
    }
}