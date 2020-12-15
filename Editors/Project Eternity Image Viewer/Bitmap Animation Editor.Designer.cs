namespace ProjectEternity.Editors.BitmapAnimationEditor
{
    partial class ProjectEternityBitmapAnimationEditor
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
            this.panPictureContainer = new System.Windows.Forms.Panel();
            this.txtImageHeight = new System.Windows.Forms.TextBox();
            this.txtImageWidth = new System.Windows.Forms.TextBox();
            this.txtNumberOfImages = new System.Windows.Forms.TextBox();
            this.lblImageHeight = new System.Windows.Forms.Label();
            this.lblImageWidth = new System.Windows.Forms.Label();
            this.lblNumberOfImages = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.panPictureContainer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtImageHeight);
            this.splitContainer1.Panel2.Controls.Add(this.txtImageWidth);
            this.splitContainer1.Panel2.Controls.Add(this.txtNumberOfImages);
            this.splitContainer1.Panel2.Controls.Add(this.lblImageHeight);
            this.splitContainer1.Panel2.Controls.Add(this.lblImageWidth);
            this.splitContainer1.Panel2.Controls.Add(this.lblNumberOfImages);
            this.splitContainer1.Size = new System.Drawing.Size(614, 197);
            this.splitContainer1.SplitterDistance = 377;
            this.splitContainer1.TabIndex = 0;
            // 
            // panPictureContainer
            // 
            this.panPictureContainer.AutoScroll = true;
            this.panPictureContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panPictureContainer.Location = new System.Drawing.Point(0, 0);
            this.panPictureContainer.Name = "panPictureContainer";
            this.panPictureContainer.Size = new System.Drawing.Size(373, 193);
            this.panPictureContainer.TabIndex = 1;
            // 
            // txtImageHeight
            // 
            this.txtImageHeight.Location = new System.Drawing.Point(109, 98);
            this.txtImageHeight.Name = "txtImageHeight";
            this.txtImageHeight.ReadOnly = true;
            this.txtImageHeight.Size = new System.Drawing.Size(110, 20);
            this.txtImageHeight.TabIndex = 7;
            // 
            // txtImageWidth
            // 
            this.txtImageWidth.Location = new System.Drawing.Point(109, 72);
            this.txtImageWidth.Name = "txtImageWidth";
            this.txtImageWidth.ReadOnly = true;
            this.txtImageWidth.Size = new System.Drawing.Size(110, 20);
            this.txtImageWidth.TabIndex = 6;
            // 
            // txtNumberOfImages
            // 
            this.txtNumberOfImages.Location = new System.Drawing.Point(109, 20);
            this.txtNumberOfImages.Name = "txtNumberOfImages";
            this.txtNumberOfImages.ReadOnly = true;
            this.txtNumberOfImages.Size = new System.Drawing.Size(110, 20);
            this.txtNumberOfImages.TabIndex = 4;
            // 
            // lblImageHeight
            // 
            this.lblImageHeight.AutoSize = true;
            this.lblImageHeight.Location = new System.Drawing.Point(3, 101);
            this.lblImageHeight.Name = "lblImageHeight";
            this.lblImageHeight.Size = new System.Drawing.Size(68, 13);
            this.lblImageHeight.TabIndex = 3;
            this.lblImageHeight.Text = "Image height";
            // 
            // lblImageWidth
            // 
            this.lblImageWidth.AutoSize = true;
            this.lblImageWidth.Location = new System.Drawing.Point(3, 75);
            this.lblImageWidth.Name = "lblImageWidth";
            this.lblImageWidth.Size = new System.Drawing.Size(67, 13);
            this.lblImageWidth.TabIndex = 2;
            this.lblImageWidth.Text = "Image width:";
            // 
            // lblNumberOfImages
            // 
            this.lblNumberOfImages.AutoSize = true;
            this.lblNumberOfImages.Location = new System.Drawing.Point(3, 23);
            this.lblNumberOfImages.Name = "lblNumberOfImages";
            this.lblNumberOfImages.Size = new System.Drawing.Size(95, 13);
            this.lblNumberOfImages.TabIndex = 0;
            this.lblNumberOfImages.Text = "Number of images:";
            // 
            // ProjectEternityBitmapAnimationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 197);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ProjectEternityBitmapAnimationEditor";
            this.Text = "Project Eternity Bitmap Animation Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtImageHeight;
        private System.Windows.Forms.TextBox txtImageWidth;
        private System.Windows.Forms.TextBox txtNumberOfImages;
        private System.Windows.Forms.Label lblImageHeight;
        private System.Windows.Forms.Label lblImageWidth;
        private System.Windows.Forms.Label lblNumberOfImages;
        private System.Windows.Forms.Panel panPictureContainer;
    }
}

