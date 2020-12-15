namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    partial class BackgroundProperties
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
            this.gbWorldType = new System.Windows.Forms.GroupBox();
            this.rbWorldTypeLooped = new System.Windows.Forms.RadioButton();
            this.rbWorldTypeLimited = new System.Windows.Forms.RadioButton();
            this.rbWorldTypeInfinite = new System.Windows.Forms.RadioButton();
            this.gbWorldSize = new System.Windows.Forms.GroupBox();
            this.lblWorldSizeDepth = new System.Windows.Forms.Label();
            this.txtWorldSizeDepth = new System.Windows.Forms.NumericUpDown();
            this.lblWorldSizeWidth = new System.Windows.Forms.Label();
            this.txtWorldSizeWidth = new System.Windows.Forms.NumericUpDown();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.gbBackgroundStartValues = new System.Windows.Forms.GroupBox();
            this.lblRoll = new System.Windows.Forms.Label();
            this.txtBackgroundRoll = new System.Windows.Forms.NumericUpDown();
            this.lblPitch = new System.Windows.Forms.Label();
            this.txtBackgroundPitch = new System.Windows.Forms.NumericUpDown();
            this.lblYaw = new System.Windows.Forms.Label();
            this.txtBackgroundYaw = new System.Windows.Forms.NumericUpDown();
            this.lblBackgroundStartZ = new System.Windows.Forms.Label();
            this.txtBackgroundStartZ = new System.Windows.Forms.NumericUpDown();
            this.lblBackgroundStartY = new System.Windows.Forms.Label();
            this.txtBackgroundStartY = new System.Windows.Forms.NumericUpDown();
            this.lblBackgroundStartX = new System.Windows.Forms.Label();
            this.txtBackgroundStartX = new System.Windows.Forms.NumericUpDown();
            this.gbWorldType.SuspendLayout();
            this.gbWorldSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorldSizeDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorldSizeWidth)).BeginInit();
            this.gbBackgroundStartValues.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundRoll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundPitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundYaw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundStartZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundStartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundStartX)).BeginInit();
            this.SuspendLayout();
            // 
            // gbWorldType
            // 
            this.gbWorldType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorldType.Controls.Add(this.rbWorldTypeLooped);
            this.gbWorldType.Controls.Add(this.rbWorldTypeLimited);
            this.gbWorldType.Controls.Add(this.rbWorldTypeInfinite);
            this.gbWorldType.Location = new System.Drawing.Point(12, 12);
            this.gbWorldType.Name = "gbWorldType";
            this.gbWorldType.Size = new System.Drawing.Size(250, 42);
            this.gbWorldType.TabIndex = 0;
            this.gbWorldType.TabStop = false;
            this.gbWorldType.Text = "World Type";
            // 
            // rbWorldTypeLooped
            // 
            this.rbWorldTypeLooped.AutoSize = true;
            this.rbWorldTypeLooped.Location = new System.Drawing.Point(183, 19);
            this.rbWorldTypeLooped.Name = "rbWorldTypeLooped";
            this.rbWorldTypeLooped.Size = new System.Drawing.Size(61, 17);
            this.rbWorldTypeLooped.TabIndex = 2;
            this.rbWorldTypeLooped.Text = "Looped";
            this.rbWorldTypeLooped.UseVisualStyleBackColor = true;
            // 
            // rbWorldTypeLimited
            // 
            this.rbWorldTypeLimited.AutoSize = true;
            this.rbWorldTypeLimited.Location = new System.Drawing.Point(88, 19);
            this.rbWorldTypeLimited.Name = "rbWorldTypeLimited";
            this.rbWorldTypeLimited.Size = new System.Drawing.Size(58, 17);
            this.rbWorldTypeLimited.TabIndex = 1;
            this.rbWorldTypeLimited.Text = "Limited";
            this.rbWorldTypeLimited.UseVisualStyleBackColor = true;
            // 
            // rbWorldTypeInfinite
            // 
            this.rbWorldTypeInfinite.AutoSize = true;
            this.rbWorldTypeInfinite.Checked = true;
            this.rbWorldTypeInfinite.Location = new System.Drawing.Point(6, 19);
            this.rbWorldTypeInfinite.Name = "rbWorldTypeInfinite";
            this.rbWorldTypeInfinite.Size = new System.Drawing.Size(56, 17);
            this.rbWorldTypeInfinite.TabIndex = 0;
            this.rbWorldTypeInfinite.TabStop = true;
            this.rbWorldTypeInfinite.Text = "Infinite";
            this.rbWorldTypeInfinite.UseVisualStyleBackColor = true;
            this.rbWorldTypeInfinite.CheckedChanged += new System.EventHandler(this.rbWorldTypeInfinite_CheckedChanged);
            // 
            // gbWorldSize
            // 
            this.gbWorldSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbWorldSize.Controls.Add(this.lblWorldSizeDepth);
            this.gbWorldSize.Controls.Add(this.txtWorldSizeDepth);
            this.gbWorldSize.Controls.Add(this.lblWorldSizeWidth);
            this.gbWorldSize.Controls.Add(this.txtWorldSizeWidth);
            this.gbWorldSize.Location = new System.Drawing.Point(12, 60);
            this.gbWorldSize.Name = "gbWorldSize";
            this.gbWorldSize.Size = new System.Drawing.Size(250, 45);
            this.gbWorldSize.TabIndex = 3;
            this.gbWorldSize.TabStop = false;
            this.gbWorldSize.Text = "World Size";
            // 
            // lblWorldSizeDepth
            // 
            this.lblWorldSizeDepth.AutoSize = true;
            this.lblWorldSizeDepth.Location = new System.Drawing.Point(133, 21);
            this.lblWorldSizeDepth.Name = "lblWorldSizeDepth";
            this.lblWorldSizeDepth.Size = new System.Drawing.Size(39, 13);
            this.lblWorldSizeDepth.TabIndex = 3;
            this.lblWorldSizeDepth.Text = "Depth:";
            // 
            // txtWorldSizeDepth
            // 
            this.txtWorldSizeDepth.Location = new System.Drawing.Point(178, 19);
            this.txtWorldSizeDepth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtWorldSizeDepth.Name = "txtWorldSizeDepth";
            this.txtWorldSizeDepth.Size = new System.Drawing.Size(66, 20);
            this.txtWorldSizeDepth.TabIndex = 2;
            // 
            // lblWorldSizeWidth
            // 
            this.lblWorldSizeWidth.AutoSize = true;
            this.lblWorldSizeWidth.Location = new System.Drawing.Point(6, 21);
            this.lblWorldSizeWidth.Name = "lblWorldSizeWidth";
            this.lblWorldSizeWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWorldSizeWidth.TabIndex = 1;
            this.lblWorldSizeWidth.Text = "Width:";
            // 
            // txtWorldSizeWidth
            // 
            this.txtWorldSizeWidth.Location = new System.Drawing.Point(50, 19);
            this.txtWorldSizeWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtWorldSizeWidth.Name = "txtWorldSizeWidth";
            this.txtWorldSizeWidth.Size = new System.Drawing.Size(66, 20);
            this.txtWorldSizeWidth.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(187, 217);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(106, 217);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 5;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // gbBackgroundStartValues
            // 
            this.gbBackgroundStartValues.Controls.Add(this.lblRoll);
            this.gbBackgroundStartValues.Controls.Add(this.txtBackgroundRoll);
            this.gbBackgroundStartValues.Controls.Add(this.lblPitch);
            this.gbBackgroundStartValues.Controls.Add(this.txtBackgroundPitch);
            this.gbBackgroundStartValues.Controls.Add(this.lblYaw);
            this.gbBackgroundStartValues.Controls.Add(this.txtBackgroundYaw);
            this.gbBackgroundStartValues.Controls.Add(this.lblBackgroundStartZ);
            this.gbBackgroundStartValues.Controls.Add(this.txtBackgroundStartZ);
            this.gbBackgroundStartValues.Controls.Add(this.lblBackgroundStartY);
            this.gbBackgroundStartValues.Controls.Add(this.txtBackgroundStartY);
            this.gbBackgroundStartValues.Controls.Add(this.lblBackgroundStartX);
            this.gbBackgroundStartValues.Controls.Add(this.txtBackgroundStartX);
            this.gbBackgroundStartValues.Location = new System.Drawing.Point(12, 111);
            this.gbBackgroundStartValues.Name = "gbBackgroundStartValues";
            this.gbBackgroundStartValues.Size = new System.Drawing.Size(250, 100);
            this.gbBackgroundStartValues.TabIndex = 6;
            this.gbBackgroundStartValues.TabStop = false;
            this.gbBackgroundStartValues.Text = "Background Start Values";
            // 
            // lblRoll
            // 
            this.lblRoll.AutoSize = true;
            this.lblRoll.Location = new System.Drawing.Point(132, 73);
            this.lblRoll.Name = "lblRoll";
            this.lblRoll.Size = new System.Drawing.Size(28, 13);
            this.lblRoll.TabIndex = 12;
            this.lblRoll.Text = "Roll:";
            // 
            // txtBackgroundRoll
            // 
            this.txtBackgroundRoll.DecimalPlaces = 3;
            this.txtBackgroundRoll.Location = new System.Drawing.Point(180, 71);
            this.txtBackgroundRoll.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtBackgroundRoll.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.txtBackgroundRoll.Name = "txtBackgroundRoll";
            this.txtBackgroundRoll.Size = new System.Drawing.Size(64, 20);
            this.txtBackgroundRoll.TabIndex = 11;
            // 
            // lblPitch
            // 
            this.lblPitch.AutoSize = true;
            this.lblPitch.Location = new System.Drawing.Point(132, 47);
            this.lblPitch.Name = "lblPitch";
            this.lblPitch.Size = new System.Drawing.Size(31, 13);
            this.lblPitch.TabIndex = 10;
            this.lblPitch.Text = "Pitch";
            // 
            // txtBackgroundPitch
            // 
            this.txtBackgroundPitch.DecimalPlaces = 3;
            this.txtBackgroundPitch.Location = new System.Drawing.Point(180, 45);
            this.txtBackgroundPitch.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtBackgroundPitch.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.txtBackgroundPitch.Name = "txtBackgroundPitch";
            this.txtBackgroundPitch.Size = new System.Drawing.Size(64, 20);
            this.txtBackgroundPitch.TabIndex = 9;
            // 
            // lblYaw
            // 
            this.lblYaw.AutoSize = true;
            this.lblYaw.Location = new System.Drawing.Point(132, 21);
            this.lblYaw.Name = "lblYaw";
            this.lblYaw.Size = new System.Drawing.Size(31, 13);
            this.lblYaw.TabIndex = 8;
            this.lblYaw.Text = "Yaw:";
            // 
            // txtBackgroundYaw
            // 
            this.txtBackgroundYaw.DecimalPlaces = 3;
            this.txtBackgroundYaw.Location = new System.Drawing.Point(180, 19);
            this.txtBackgroundYaw.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtBackgroundYaw.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.txtBackgroundYaw.Name = "txtBackgroundYaw";
            this.txtBackgroundYaw.Size = new System.Drawing.Size(64, 20);
            this.txtBackgroundYaw.TabIndex = 7;
            // 
            // lblBackgroundStartZ
            // 
            this.lblBackgroundStartZ.AutoSize = true;
            this.lblBackgroundStartZ.Location = new System.Drawing.Point(4, 73);
            this.lblBackgroundStartZ.Name = "lblBackgroundStartZ";
            this.lblBackgroundStartZ.Size = new System.Drawing.Size(42, 13);
            this.lblBackgroundStartZ.TabIndex = 6;
            this.lblBackgroundStartZ.Text = "Start Z:";
            // 
            // txtBackgroundStartZ
            // 
            this.txtBackgroundStartZ.DecimalPlaces = 3;
            this.txtBackgroundStartZ.Location = new System.Drawing.Point(52, 71);
            this.txtBackgroundStartZ.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtBackgroundStartZ.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.txtBackgroundStartZ.Name = "txtBackgroundStartZ";
            this.txtBackgroundStartZ.Size = new System.Drawing.Size(64, 20);
            this.txtBackgroundStartZ.TabIndex = 5;
            // 
            // lblBackgroundStartY
            // 
            this.lblBackgroundStartY.AutoSize = true;
            this.lblBackgroundStartY.Location = new System.Drawing.Point(4, 47);
            this.lblBackgroundStartY.Name = "lblBackgroundStartY";
            this.lblBackgroundStartY.Size = new System.Drawing.Size(42, 13);
            this.lblBackgroundStartY.TabIndex = 4;
            this.lblBackgroundStartY.Text = "Start Y:";
            // 
            // txtBackgroundStartY
            // 
            this.txtBackgroundStartY.DecimalPlaces = 3;
            this.txtBackgroundStartY.Location = new System.Drawing.Point(52, 45);
            this.txtBackgroundStartY.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtBackgroundStartY.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.txtBackgroundStartY.Name = "txtBackgroundStartY";
            this.txtBackgroundStartY.Size = new System.Drawing.Size(64, 20);
            this.txtBackgroundStartY.TabIndex = 3;
            // 
            // lblBackgroundStartX
            // 
            this.lblBackgroundStartX.AutoSize = true;
            this.lblBackgroundStartX.Location = new System.Drawing.Point(4, 21);
            this.lblBackgroundStartX.Name = "lblBackgroundStartX";
            this.lblBackgroundStartX.Size = new System.Drawing.Size(42, 13);
            this.lblBackgroundStartX.TabIndex = 2;
            this.lblBackgroundStartX.Text = "Start X:";
            // 
            // txtBackgroundStartX
            // 
            this.txtBackgroundStartX.DecimalPlaces = 3;
            this.txtBackgroundStartX.Location = new System.Drawing.Point(52, 19);
            this.txtBackgroundStartX.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.txtBackgroundStartX.Minimum = new decimal(new int[] {
            10000,
            0,
            0,
            -2147483648});
            this.txtBackgroundStartX.Name = "txtBackgroundStartX";
            this.txtBackgroundStartX.Size = new System.Drawing.Size(64, 20);
            this.txtBackgroundStartX.TabIndex = 1;
            // 
            // BackgroundProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 251);
            this.Controls.Add(this.gbBackgroundStartValues);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbWorldSize);
            this.Controls.Add(this.gbWorldType);
            this.Name = "BackgroundProperties";
            this.Text = "Properties";
            this.Shown += new System.EventHandler(this.BackgroundProperties_Shown);
            this.gbWorldType.ResumeLayout(false);
            this.gbWorldType.PerformLayout();
            this.gbWorldSize.ResumeLayout(false);
            this.gbWorldSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorldSizeDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorldSizeWidth)).EndInit();
            this.gbBackgroundStartValues.ResumeLayout(false);
            this.gbBackgroundStartValues.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundRoll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundPitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundYaw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundStartZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundStartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBackgroundStartX)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbWorldType;
        private System.Windows.Forms.RadioButton rbWorldTypeInfinite;
        private System.Windows.Forms.RadioButton rbWorldTypeLimited;
        private System.Windows.Forms.RadioButton rbWorldTypeLooped;
        private System.Windows.Forms.GroupBox gbWorldSize;
        private System.Windows.Forms.NumericUpDown txtWorldSizeWidth;
        private System.Windows.Forms.Label lblWorldSizeWidth;
        private System.Windows.Forms.Label lblWorldSizeDepth;
        private System.Windows.Forms.NumericUpDown txtWorldSizeDepth;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.GroupBox gbBackgroundStartValues;
        private System.Windows.Forms.Label lblRoll;
        public System.Windows.Forms.NumericUpDown txtBackgroundRoll;
        private System.Windows.Forms.Label lblPitch;
        public System.Windows.Forms.NumericUpDown txtBackgroundPitch;
        private System.Windows.Forms.Label lblYaw;
        public System.Windows.Forms.NumericUpDown txtBackgroundYaw;
        private System.Windows.Forms.Label lblBackgroundStartZ;
        public System.Windows.Forms.NumericUpDown txtBackgroundStartZ;
        private System.Windows.Forms.Label lblBackgroundStartY;
        public System.Windows.Forms.NumericUpDown txtBackgroundStartY;
        private System.Windows.Forms.Label lblBackgroundStartX;
        public System.Windows.Forms.NumericUpDown txtBackgroundStartX;
    }
}