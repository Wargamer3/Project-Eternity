namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class AnimatedChainSpawnerHelper
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
            this.ChainLinkViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.gbChainLink = new System.Windows.Forms.GroupBox();
            this.btnSetChainLinkSprite = new System.Windows.Forms.Button();
            this.lblChainLinkOriginY = new System.Windows.Forms.Label();
            this.txtChainLinkOriginY = new System.Windows.Forms.NumericUpDown();
            this.lblChainLinkOriginX = new System.Windows.Forms.Label();
            this.txtChainLinkOriginX = new System.Windows.Forms.NumericUpDown();
            this.gbChainEnd = new System.Windows.Forms.GroupBox();
            this.btnSetChainEndSprite = new System.Windows.Forms.Button();
            this.lblChainEndOriginY = new System.Windows.Forms.Label();
            this.txtChainEndOriginY = new System.Windows.Forms.NumericUpDown();
            this.lblChainEndOriginX = new System.Windows.Forms.Label();
            this.txtChainEndOriginX = new System.Windows.Forms.NumericUpDown();
            this.ChainEndViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.gbChainStart = new System.Windows.Forms.GroupBox();
            this.btnSetChainStartSprite = new System.Windows.Forms.Button();
            this.lblChainStartOriginY = new System.Windows.Forms.Label();
            this.txtChainStartOriginY = new System.Windows.Forms.NumericUpDown();
            this.lblChainStartOriginX = new System.Windows.Forms.Label();
            this.txtChainStartOriginX = new System.Windows.Forms.NumericUpDown();
            this.ChainStartViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.gbChainLink.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainLinkOriginY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainLinkOriginX)).BeginInit();
            this.gbChainEnd.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainEndOriginY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainEndOriginX)).BeginInit();
            this.gbChainStart.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainStartOriginY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainStartOriginX)).BeginInit();
            this.SuspendLayout();
            // 
            // ChainLinkViewer
            // 
            this.ChainLinkViewer.Location = new System.Drawing.Point(9, 100);
            this.ChainLinkViewer.Name = "ChainLinkViewer";
            this.ChainLinkViewer.Size = new System.Drawing.Size(137, 136);
            this.ChainLinkViewer.TabIndex = 0;
            this.ChainLinkViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChainLinkViewer_MouseMove);
            this.ChainLinkViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChainLinkViewer_MouseMove);
            // 
            // gbChainLink
            // 
            this.gbChainLink.Controls.Add(this.btnSetChainLinkSprite);
            this.gbChainLink.Controls.Add(this.lblChainLinkOriginY);
            this.gbChainLink.Controls.Add(this.txtChainLinkOriginY);
            this.gbChainLink.Controls.Add(this.lblChainLinkOriginX);
            this.gbChainLink.Controls.Add(this.txtChainLinkOriginX);
            this.gbChainLink.Controls.Add(this.ChainLinkViewer);
            this.gbChainLink.Location = new System.Drawing.Point(12, 12);
            this.gbChainLink.Name = "gbChainLink";
            this.gbChainLink.Size = new System.Drawing.Size(152, 242);
            this.gbChainLink.TabIndex = 1;
            this.gbChainLink.TabStop = false;
            this.gbChainLink.Text = "Chain Link";
            // 
            // btnSetChainLinkSprite
            // 
            this.btnSetChainLinkSprite.Location = new System.Drawing.Point(6, 19);
            this.btnSetChainLinkSprite.Name = "btnSetChainLinkSprite";
            this.btnSetChainLinkSprite.Size = new System.Drawing.Size(75, 23);
            this.btnSetChainLinkSprite.TabIndex = 6;
            this.btnSetChainLinkSprite.Text = "Set Sprite";
            this.btnSetChainLinkSprite.UseVisualStyleBackColor = true;
            this.btnSetChainLinkSprite.Click += new System.EventHandler(this.btnSetChainLinkSprite_Click);
            // 
            // lblChainLinkOriginY
            // 
            this.lblChainLinkOriginY.AutoSize = true;
            this.lblChainLinkOriginY.Location = new System.Drawing.Point(6, 76);
            this.lblChainLinkOriginY.Name = "lblChainLinkOriginY";
            this.lblChainLinkOriginY.Size = new System.Drawing.Size(44, 13);
            this.lblChainLinkOriginY.TabIndex = 5;
            this.lblChainLinkOriginY.Text = "Origin Y";
            // 
            // txtChainLinkOriginY
            // 
            this.txtChainLinkOriginY.Location = new System.Drawing.Point(64, 74);
            this.txtChainLinkOriginY.Name = "txtChainLinkOriginY";
            this.txtChainLinkOriginY.Size = new System.Drawing.Size(82, 20);
            this.txtChainLinkOriginY.TabIndex = 4;
            // 
            // lblChainLinkOriginX
            // 
            this.lblChainLinkOriginX.AutoSize = true;
            this.lblChainLinkOriginX.Location = new System.Drawing.Point(6, 50);
            this.lblChainLinkOriginX.Name = "lblChainLinkOriginX";
            this.lblChainLinkOriginX.Size = new System.Drawing.Size(44, 13);
            this.lblChainLinkOriginX.TabIndex = 3;
            this.lblChainLinkOriginX.Text = "Origin X";
            // 
            // txtChainLinkOriginX
            // 
            this.txtChainLinkOriginX.Location = new System.Drawing.Point(64, 48);
            this.txtChainLinkOriginX.Name = "txtChainLinkOriginX";
            this.txtChainLinkOriginX.Size = new System.Drawing.Size(82, 20);
            this.txtChainLinkOriginX.TabIndex = 2;
            // 
            // gbChainEnd
            // 
            this.gbChainEnd.Controls.Add(this.btnSetChainEndSprite);
            this.gbChainEnd.Controls.Add(this.lblChainEndOriginY);
            this.gbChainEnd.Controls.Add(this.txtChainEndOriginY);
            this.gbChainEnd.Controls.Add(this.lblChainEndOriginX);
            this.gbChainEnd.Controls.Add(this.txtChainEndOriginX);
            this.gbChainEnd.Controls.Add(this.ChainEndViewer);
            this.gbChainEnd.Location = new System.Drawing.Point(170, 12);
            this.gbChainEnd.Name = "gbChainEnd";
            this.gbChainEnd.Size = new System.Drawing.Size(152, 242);
            this.gbChainEnd.TabIndex = 7;
            this.gbChainEnd.TabStop = false;
            this.gbChainEnd.Text = "Chain End";
            // 
            // btnSetChainEndSprite
            // 
            this.btnSetChainEndSprite.Location = new System.Drawing.Point(6, 19);
            this.btnSetChainEndSprite.Name = "btnSetChainEndSprite";
            this.btnSetChainEndSprite.Size = new System.Drawing.Size(75, 23);
            this.btnSetChainEndSprite.TabIndex = 6;
            this.btnSetChainEndSprite.Text = "Set Sprite";
            this.btnSetChainEndSprite.UseVisualStyleBackColor = true;
            this.btnSetChainEndSprite.Click += new System.EventHandler(this.btnSetChainEndSprite_Click);
            // 
            // lblChainEndOriginY
            // 
            this.lblChainEndOriginY.AutoSize = true;
            this.lblChainEndOriginY.Location = new System.Drawing.Point(6, 76);
            this.lblChainEndOriginY.Name = "lblChainEndOriginY";
            this.lblChainEndOriginY.Size = new System.Drawing.Size(44, 13);
            this.lblChainEndOriginY.TabIndex = 5;
            this.lblChainEndOriginY.Text = "Origin Y";
            // 
            // txtChainEndOriginY
            // 
            this.txtChainEndOriginY.Location = new System.Drawing.Point(64, 74);
            this.txtChainEndOriginY.Name = "txtChainEndOriginY";
            this.txtChainEndOriginY.Size = new System.Drawing.Size(82, 20);
            this.txtChainEndOriginY.TabIndex = 4;
            // 
            // lblChainEndOriginX
            // 
            this.lblChainEndOriginX.AutoSize = true;
            this.lblChainEndOriginX.Location = new System.Drawing.Point(6, 50);
            this.lblChainEndOriginX.Name = "lblChainEndOriginX";
            this.lblChainEndOriginX.Size = new System.Drawing.Size(44, 13);
            this.lblChainEndOriginX.TabIndex = 3;
            this.lblChainEndOriginX.Text = "Origin X";
            // 
            // txtChainEndOriginX
            // 
            this.txtChainEndOriginX.Location = new System.Drawing.Point(64, 48);
            this.txtChainEndOriginX.Name = "txtChainEndOriginX";
            this.txtChainEndOriginX.Size = new System.Drawing.Size(82, 20);
            this.txtChainEndOriginX.TabIndex = 2;
            // 
            // ChainEndViewer
            // 
            this.ChainEndViewer.Location = new System.Drawing.Point(9, 100);
            this.ChainEndViewer.Name = "ChainEndViewer";
            this.ChainEndViewer.Size = new System.Drawing.Size(137, 136);
            this.ChainEndViewer.TabIndex = 0;
            this.ChainEndViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChainEndViewer_MouseMove);
            this.ChainEndViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChainEndViewer_MouseMove);
            // 
            // gbChainStart
            // 
            this.gbChainStart.Controls.Add(this.btnSetChainStartSprite);
            this.gbChainStart.Controls.Add(this.lblChainStartOriginY);
            this.gbChainStart.Controls.Add(this.txtChainStartOriginY);
            this.gbChainStart.Controls.Add(this.lblChainStartOriginX);
            this.gbChainStart.Controls.Add(this.txtChainStartOriginX);
            this.gbChainStart.Controls.Add(this.ChainStartViewer);
            this.gbChainStart.Location = new System.Drawing.Point(328, 12);
            this.gbChainStart.Name = "gbChainStart";
            this.gbChainStart.Size = new System.Drawing.Size(152, 242);
            this.gbChainStart.TabIndex = 8;
            this.gbChainStart.TabStop = false;
            this.gbChainStart.Text = "Chain Start";
            // 
            // btnSetChainStartSprite
            // 
            this.btnSetChainStartSprite.Location = new System.Drawing.Point(6, 19);
            this.btnSetChainStartSprite.Name = "btnSetChainStartSprite";
            this.btnSetChainStartSprite.Size = new System.Drawing.Size(75, 23);
            this.btnSetChainStartSprite.TabIndex = 6;
            this.btnSetChainStartSprite.Text = "Set Sprite";
            this.btnSetChainStartSprite.UseVisualStyleBackColor = true;
            this.btnSetChainStartSprite.Click += new System.EventHandler(this.btnSetChainStartSprite_Click);
            // 
            // lblChainStartOriginY
            // 
            this.lblChainStartOriginY.AutoSize = true;
            this.lblChainStartOriginY.Location = new System.Drawing.Point(6, 76);
            this.lblChainStartOriginY.Name = "lblChainStartOriginY";
            this.lblChainStartOriginY.Size = new System.Drawing.Size(44, 13);
            this.lblChainStartOriginY.TabIndex = 5;
            this.lblChainStartOriginY.Text = "Origin Y";
            // 
            // txtChainStartOriginY
            // 
            this.txtChainStartOriginY.Location = new System.Drawing.Point(64, 74);
            this.txtChainStartOriginY.Name = "txtChainStartOriginY";
            this.txtChainStartOriginY.Size = new System.Drawing.Size(82, 20);
            this.txtChainStartOriginY.TabIndex = 4;
            // 
            // lblChainStartOriginX
            // 
            this.lblChainStartOriginX.AutoSize = true;
            this.lblChainStartOriginX.Location = new System.Drawing.Point(6, 50);
            this.lblChainStartOriginX.Name = "lblChainStartOriginX";
            this.lblChainStartOriginX.Size = new System.Drawing.Size(44, 13);
            this.lblChainStartOriginX.TabIndex = 3;
            this.lblChainStartOriginX.Text = "Origin X";
            // 
            // txtChainStartOriginX
            // 
            this.txtChainStartOriginX.Location = new System.Drawing.Point(64, 48);
            this.txtChainStartOriginX.Name = "txtChainStartOriginX";
            this.txtChainStartOriginX.Size = new System.Drawing.Size(82, 20);
            this.txtChainStartOriginX.TabIndex = 2;
            // 
            // ChainStartViewer
            // 
            this.ChainStartViewer.Location = new System.Drawing.Point(9, 100);
            this.ChainStartViewer.Name = "ChainStartViewer";
            this.ChainStartViewer.Size = new System.Drawing.Size(137, 136);
            this.ChainStartViewer.TabIndex = 0;
            this.ChainStartViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChainStartViewer_MouseMove);
            this.ChainStartViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChainStartViewer_MouseMove);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(399, 260);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnConfirm
            // 
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(305, 260);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 9;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            // 
            // AnimatedChainSpawnerHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 286);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.gbChainStart);
            this.Controls.Add(this.gbChainEnd);
            this.Controls.Add(this.gbChainLink);
            this.Name = "AnimatedChainSpawnerHelper";
            this.Text = "Animated Chain Spawner Helper";
            this.Shown += new System.EventHandler(this.AnimatedChainSpawnerHelper_Shown);
            this.gbChainLink.ResumeLayout(false);
            this.gbChainLink.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainLinkOriginY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainLinkOriginX)).EndInit();
            this.gbChainEnd.ResumeLayout(false);
            this.gbChainEnd.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainEndOriginY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainEndOriginX)).EndInit();
            this.gbChainStart.ResumeLayout(false);
            this.gbChainStart.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainStartOriginY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtChainStartOriginX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbChainLink;
        private System.Windows.Forms.Label lblChainLinkOriginY;
        private System.Windows.Forms.Label lblChainLinkOriginX;
        private System.Windows.Forms.Button btnSetChainLinkSprite;
        private System.Windows.Forms.GroupBox gbChainEnd;
        private System.Windows.Forms.Button btnSetChainEndSprite;
        private System.Windows.Forms.Label lblChainEndOriginY;
        private System.Windows.Forms.Label lblChainEndOriginX;
        private System.Windows.Forms.GroupBox gbChainStart;
        private System.Windows.Forms.Button btnSetChainStartSprite;
        private System.Windows.Forms.Label lblChainStartOriginY;
        private System.Windows.Forms.Label lblChainStartOriginX;
        public Core.Editor.Texture2DViewerControl ChainLinkViewer;
        public Core.Editor.Texture2DViewerControl ChainEndViewer;
        public Core.Editor.Texture2DViewerControl ChainStartViewer;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        public System.Windows.Forms.NumericUpDown txtChainLinkOriginY;
        public System.Windows.Forms.NumericUpDown txtChainLinkOriginX;
        public System.Windows.Forms.NumericUpDown txtChainEndOriginY;
        public System.Windows.Forms.NumericUpDown txtChainEndOriginX;
        public System.Windows.Forms.NumericUpDown txtChainStartOriginY;
        public System.Windows.Forms.NumericUpDown txtChainStartOriginX;
    }
}
