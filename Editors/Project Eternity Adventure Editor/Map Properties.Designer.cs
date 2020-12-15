namespace ProjectEternity.Editors.AdventureEditor
{
    partial class MapProperties
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
            this.gbCameraLimits = new System.Windows.Forms.GroupBox();
            this.lblBottom = new System.Windows.Forms.Label();
            this.txtBottom = new System.Windows.Forms.NumericUpDown();
            this.lblTop = new System.Windows.Forms.Label();
            this.txtTop = new System.Windows.Forms.NumericUpDown();
            this.lblRight = new System.Windows.Forms.Label();
            this.txtRight = new System.Windows.Forms.NumericUpDown();
            this.lblLeft = new System.Windows.Forms.Label();
            this.txtLeft = new System.Windows.Forms.NumericUpDown();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbCameraLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // gbCameraLimits
            // 
            this.gbCameraLimits.Controls.Add(this.lblBottom);
            this.gbCameraLimits.Controls.Add(this.txtBottom);
            this.gbCameraLimits.Controls.Add(this.lblTop);
            this.gbCameraLimits.Controls.Add(this.txtTop);
            this.gbCameraLimits.Controls.Add(this.lblRight);
            this.gbCameraLimits.Controls.Add(this.txtRight);
            this.gbCameraLimits.Controls.Add(this.lblLeft);
            this.gbCameraLimits.Controls.Add(this.txtLeft);
            this.gbCameraLimits.Location = new System.Drawing.Point(12, 12);
            this.gbCameraLimits.Name = "gbCameraLimits";
            this.gbCameraLimits.Size = new System.Drawing.Size(398, 116);
            this.gbCameraLimits.TabIndex = 0;
            this.gbCameraLimits.TabStop = false;
            this.gbCameraLimits.Text = "Camera Limits";
            // 
            // lblBottom
            // 
            this.lblBottom.AutoSize = true;
            this.lblBottom.Location = new System.Drawing.Point(115, 91);
            this.lblBottom.Name = "lblBottom";
            this.lblBottom.Size = new System.Drawing.Size(40, 13);
            this.lblBottom.TabIndex = 7;
            this.lblBottom.Text = "Bottom";
            // 
            // txtBottom
            // 
            this.txtBottom.Location = new System.Drawing.Point(161, 89);
            this.txtBottom.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.txtBottom.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            -2147483648});
            this.txtBottom.Name = "txtBottom";
            this.txtBottom.Size = new System.Drawing.Size(120, 20);
            this.txtBottom.TabIndex = 6;
            // 
            // lblTop
            // 
            this.lblTop.AutoSize = true;
            this.lblTop.Location = new System.Drawing.Point(129, 21);
            this.lblTop.Name = "lblTop";
            this.lblTop.Size = new System.Drawing.Size(26, 13);
            this.lblTop.TabIndex = 5;
            this.lblTop.Text = "Top";
            // 
            // txtTop
            // 
            this.txtTop.Location = new System.Drawing.Point(161, 19);
            this.txtTop.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.txtTop.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            -2147483648});
            this.txtTop.Name = "txtTop";
            this.txtTop.Size = new System.Drawing.Size(120, 20);
            this.txtTop.TabIndex = 4;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Location = new System.Drawing.Point(221, 56);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(32, 13);
            this.lblRight.TabIndex = 3;
            this.lblRight.Text = "Right";
            // 
            // txtRight
            // 
            this.txtRight.Location = new System.Drawing.Point(259, 54);
            this.txtRight.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.txtRight.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            -2147483648});
            this.txtRight.Name = "txtRight";
            this.txtRight.Size = new System.Drawing.Size(120, 20);
            this.txtRight.TabIndex = 2;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Location = new System.Drawing.Point(6, 56);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(25, 13);
            this.lblLeft.TabIndex = 1;
            this.lblLeft.Text = "Left";
            // 
            // txtLeft
            // 
            this.txtLeft.Location = new System.Drawing.Point(37, 54);
            this.txtLeft.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.txtLeft.Minimum = new decimal(new int[] {
            50000,
            0,
            0,
            -2147483648});
            this.txtLeft.Name = "txtLeft";
            this.txtLeft.Size = new System.Drawing.Size(120, 20);
            this.txtLeft.TabIndex = 0;
            // 
            // btnConfirm
            // 
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(254, 134);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(335, 134);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MapProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 169);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.gbCameraLimits);
            this.Name = "MapProperties";
            this.Text = "Map Properties";
            this.gbCameraLimits.ResumeLayout(false);
            this.gbCameraLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLeft)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCameraLimits;
        private System.Windows.Forms.Label lblBottom;
        private System.Windows.Forms.Label lblTop;
        private System.Windows.Forms.Label lblRight;
        private System.Windows.Forms.Label lblLeft;
        public System.Windows.Forms.NumericUpDown txtBottom;
        public System.Windows.Forms.NumericUpDown txtTop;
        public System.Windows.Forms.NumericUpDown txtRight;
        public System.Windows.Forms.NumericUpDown txtLeft;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
    }
}