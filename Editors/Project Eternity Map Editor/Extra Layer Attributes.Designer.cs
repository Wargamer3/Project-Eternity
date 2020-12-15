namespace ProjectEternity.Editors.MapEditor
{
    partial class ExtraLayerAttributes
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
            this.gbAnimation = new System.Windows.Forms.GroupBox();
            this.txtDepth = new System.Windows.Forms.NumericUpDown();
            this.lblDepth = new System.Windows.Forms.Label();
            this.txtAnimationToggleDelayOn = new System.Windows.Forms.NumericUpDown();
            this.txtAnimationStartupDelay = new System.Windows.Forms.NumericUpDown();
            this.lblAnimationStartupDelay = new System.Windows.Forms.Label();
            this.lblAnimationToggleDelayOn = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.txtAnimationToggleDelayOff = new System.Windows.Forms.NumericUpDown();
            this.lblAnimationToggleDelayOff = new System.Windows.Forms.Label();
            this.gbAnimation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnimationToggleDelayOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnimationStartupDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnimationToggleDelayOff)).BeginInit();
            this.SuspendLayout();
            // 
            // gbAnimation
            // 
            this.gbAnimation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAnimation.Controls.Add(this.txtAnimationToggleDelayOff);
            this.gbAnimation.Controls.Add(this.lblAnimationToggleDelayOff);
            this.gbAnimation.Controls.Add(this.txtDepth);
            this.gbAnimation.Controls.Add(this.lblDepth);
            this.gbAnimation.Controls.Add(this.txtAnimationToggleDelayOn);
            this.gbAnimation.Controls.Add(this.txtAnimationStartupDelay);
            this.gbAnimation.Controls.Add(this.lblAnimationStartupDelay);
            this.gbAnimation.Controls.Add(this.lblAnimationToggleDelayOn);
            this.gbAnimation.Location = new System.Drawing.Point(12, 12);
            this.gbAnimation.Name = "gbAnimation";
            this.gbAnimation.Size = new System.Drawing.Size(274, 125);
            this.gbAnimation.TabIndex = 0;
            this.gbAnimation.TabStop = false;
            this.gbAnimation.Text = "Animation";
            // 
            // txtDepth
            // 
            this.txtDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDepth.DecimalPlaces = 2;
            this.txtDepth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txtDepth.Location = new System.Drawing.Point(170, 97);
            this.txtDepth.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtDepth.Name = "txtDepth";
            this.txtDepth.Size = new System.Drawing.Size(98, 20);
            this.txtDepth.TabIndex = 5;
            // 
            // lblDepth
            // 
            this.lblDepth.AutoSize = true;
            this.lblDepth.Location = new System.Drawing.Point(6, 98);
            this.lblDepth.Name = "lblDepth";
            this.lblDepth.Size = new System.Drawing.Size(36, 13);
            this.lblDepth.TabIndex = 4;
            this.lblDepth.Text = "Depth";
            // 
            // txtAnimationToggleDelayOn
            // 
            this.txtAnimationToggleDelayOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnimationToggleDelayOn.Location = new System.Drawing.Point(170, 45);
            this.txtAnimationToggleDelayOn.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtAnimationToggleDelayOn.Name = "txtAnimationToggleDelayOn";
            this.txtAnimationToggleDelayOn.Size = new System.Drawing.Size(98, 20);
            this.txtAnimationToggleDelayOn.TabIndex = 3;
            // 
            // txtAnimationStartupDelay
            // 
            this.txtAnimationStartupDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnimationStartupDelay.Location = new System.Drawing.Point(170, 19);
            this.txtAnimationStartupDelay.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtAnimationStartupDelay.Name = "txtAnimationStartupDelay";
            this.txtAnimationStartupDelay.Size = new System.Drawing.Size(98, 20);
            this.txtAnimationStartupDelay.TabIndex = 2;
            // 
            // lblAnimationStartupDelay
            // 
            this.lblAnimationStartupDelay.AutoSize = true;
            this.lblAnimationStartupDelay.Location = new System.Drawing.Point(6, 21);
            this.lblAnimationStartupDelay.Name = "lblAnimationStartupDelay";
            this.lblAnimationStartupDelay.Size = new System.Drawing.Size(142, 13);
            this.lblAnimationStartupDelay.TabIndex = 1;
            this.lblAnimationStartupDelay.Text = "Animation Startup Delay (ms)";
            // 
            // lblAnimationToggleDelayOn
            // 
            this.lblAnimationToggleDelayOn.AutoSize = true;
            this.lblAnimationToggleDelayOn.Location = new System.Drawing.Point(6, 47);
            this.lblAnimationToggleDelayOn.Name = "lblAnimationToggleDelayOn";
            this.lblAnimationToggleDelayOn.Size = new System.Drawing.Size(158, 13);
            this.lblAnimationToggleDelayOn.TabIndex = 0;
            this.lblAnimationToggleDelayOn.Text = "Animation Toggle Delay On (ms)";
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(205, 145);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 12;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            // 
            // txtAnimationToggleDelayOff
            // 
            this.txtAnimationToggleDelayOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnimationToggleDelayOff.Location = new System.Drawing.Point(170, 71);
            this.txtAnimationToggleDelayOff.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtAnimationToggleDelayOff.Name = "txtAnimationToggleDelayOff";
            this.txtAnimationToggleDelayOff.Size = new System.Drawing.Size(98, 20);
            this.txtAnimationToggleDelayOff.TabIndex = 7;
            // 
            // lblAnimationToggleDelayOff
            // 
            this.lblAnimationToggleDelayOff.AutoSize = true;
            this.lblAnimationToggleDelayOff.Location = new System.Drawing.Point(6, 73);
            this.lblAnimationToggleDelayOff.Name = "lblAnimationToggleDelayOff";
            this.lblAnimationToggleDelayOff.Size = new System.Drawing.Size(158, 13);
            this.lblAnimationToggleDelayOff.TabIndex = 6;
            this.lblAnimationToggleDelayOff.Text = "Animation Toggle Delay Off (ms)";
            // 
            // ExtraLayerAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(298, 180);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.gbAnimation);
            this.Name = "ExtraLayerAttributes";
            this.Text = "ExtraLayerAttributes";
            this.gbAnimation.ResumeLayout(false);
            this.gbAnimation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnimationToggleDelayOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnimationStartupDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAnimationToggleDelayOff)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbAnimation;
        private System.Windows.Forms.Label lblAnimationStartupDelay;
        private System.Windows.Forms.Label lblAnimationToggleDelayOn;
        private System.Windows.Forms.Button btnAccept;
        public System.Windows.Forms.NumericUpDown txtAnimationToggleDelayOn;
        public System.Windows.Forms.NumericUpDown txtAnimationStartupDelay;
        public System.Windows.Forms.NumericUpDown txtDepth;
        private System.Windows.Forms.Label lblDepth;
        public System.Windows.Forms.NumericUpDown txtAnimationToggleDelayOff;
        private System.Windows.Forms.Label lblAnimationToggleDelayOff;
    }
}