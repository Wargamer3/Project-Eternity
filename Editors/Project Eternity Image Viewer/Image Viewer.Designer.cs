namespace ProjectEternity.Editors.ImageViewer
{
    partial class ProjectEternityImageViewer
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
            this.pbImageViewer = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbImageViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // pbImageViewer
            // 
            this.pbImageViewer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbImageViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImageViewer.Location = new System.Drawing.Point(0, 0);
            this.pbImageViewer.Name = "pbImageViewer";
            this.pbImageViewer.Size = new System.Drawing.Size(396, 335);
            this.pbImageViewer.TabIndex = 0;
            this.pbImageViewer.TabStop = false;
            // 
            // ImageViewerEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 335);
            this.Controls.Add(this.pbImageViewer);
            this.Name = "ImageViewerEditor";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pbImageViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImageViewer;
    }
}

