namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class MarkerHelper
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.MarkerViewer = new ProjectEternity.GameScreens.AnimationScreen.MarkerViewerControl();
            this.btnSetTexture = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnNoPlaceholder = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.MarkerViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnNoPlaceholder);
            this.splitContainer1.Panel2.Controls.Add(this.btnSetTexture);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnConfirm);
            this.splitContainer1.Size = new System.Drawing.Size(450, 377);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 1;
            // 
            // MarkerViewer
            // 
            this.MarkerViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MarkerViewer.Location = new System.Drawing.Point(0, 0);
            this.MarkerViewer.Name = "MarkerViewer";
            this.MarkerViewer.Size = new System.Drawing.Size(271, 377);
            this.MarkerViewer.TabIndex = 0;
            // 
            // btnSetTexture
            // 
            this.btnSetTexture.Location = new System.Drawing.Point(3, 12);
            this.btnSetTexture.Name = "btnSetTexture";
            this.btnSetTexture.Size = new System.Drawing.Size(142, 23);
            this.btnSetTexture.TabIndex = 2;
            this.btnSetTexture.Text = "Set Placeholder Sprite";
            this.btnSetTexture.UseVisualStyleBackColor = true;
            this.btnSetTexture.Click += new System.EventHandler(this.btnSetTexture_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(97, 351);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(3, 351);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // btnNoPlaceholder
            // 
            this.btnNoPlaceholder.Location = new System.Drawing.Point(2, 41);
            this.btnNoPlaceholder.Name = "btnNoPlaceholder";
            this.btnNoPlaceholder.Size = new System.Drawing.Size(142, 23);
            this.btnNoPlaceholder.TabIndex = 3;
            this.btnNoPlaceholder.Text = "Don\'t use Placeholder";
            this.btnNoPlaceholder.UseVisualStyleBackColor = true;
            this.btnNoPlaceholder.Click += new System.EventHandler(this.btnNoPlaceholder_Click);
            // 
            // MarkerHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 377);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MarkerHelper";
            this.Text = "Animated Bitmap Spawner Helper";
            this.Shown += new System.EventHandler(this.AnimatedBitmapSpawnerHelper_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnSetTexture;
        public MarkerViewerControl MarkerViewer;
        private System.Windows.Forms.Button btnNoPlaceholder;
    }
}