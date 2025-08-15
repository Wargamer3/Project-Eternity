namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    partial class SorcererStreetMapStatistics
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblHigestDieRoll = new System.Windows.Forms.Label();
            this.txtHighestDieRoll = new System.Windows.Forms.NumericUpDown();
            this.lblMagicGoal = new System.Windows.Forms.Label();
            this.txtMagicGoal = new System.Windows.Forms.NumericUpDown();
            this.lblMagicPerTower = new System.Windows.Forms.Label();
            this.txtMagicPerTower = new System.Windows.Forms.NumericUpDown();
            this.lblMagicPerLap = new System.Windows.Forms.Label();
            this.txtMagicPerLap = new System.Windows.Forms.NumericUpDown();
            this.lblMagicAtStart = new System.Windows.Forms.Label();
            this.txtMagicAtStart = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoursInDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtlblTimeMultiplier)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderNumber)).BeginInit();
            this.gbDescription.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighestDieRoll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicGoal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicPerTower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicPerLap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicAtStart)).BeginInit();
            this.SuspendLayout();
            // 
            // txtDescription
            // 
            this.txtDescription.Size = new System.Drawing.Size(211, 373);
            // 
            // gbDescription
            // 
            this.gbDescription.Size = new System.Drawing.Size(223, 427);
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(536, 445);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(626, 445);
            // 
            // btnOpenTranslationFile
            // 
            this.btnOpenTranslationFile.Location = new System.Drawing.Point(72, 398);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblHigestDieRoll);
            this.groupBox1.Controls.Add(this.txtHighestDieRoll);
            this.groupBox1.Controls.Add(this.lblMagicGoal);
            this.groupBox1.Controls.Add(this.txtMagicGoal);
            this.groupBox1.Controls.Add(this.lblMagicPerTower);
            this.groupBox1.Controls.Add(this.txtMagicPerTower);
            this.groupBox1.Controls.Add(this.lblMagicPerLap);
            this.groupBox1.Controls.Add(this.txtMagicPerLap);
            this.groupBox1.Controls.Add(this.lblMagicAtStart);
            this.groupBox1.Controls.Add(this.txtMagicAtStart);
            this.groupBox1.Location = new System.Drawing.Point(12, 386);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 76);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Sorcerer Street";
            // 
            // lblHigestDieRoll
            // 
            this.lblHigestDieRoll.AutoSize = true;
            this.lblHigestDieRoll.Location = new System.Drawing.Point(305, 49);
            this.lblHigestDieRoll.Name = "lblHigestDieRoll";
            this.lblHigestDieRoll.Size = new System.Drawing.Size(83, 13);
            this.lblHigestDieRoll.TabIndex = 9;
            this.lblHigestDieRoll.Text = "Highest Die Roll";
            // 
            // txtHighestDieRoll
            // 
            this.txtHighestDieRoll.Location = new System.Drawing.Point(394, 47);
            this.txtHighestDieRoll.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.txtHighestDieRoll.Name = "txtHighestDieRoll";
            this.txtHighestDieRoll.Size = new System.Drawing.Size(60, 20);
            this.txtHighestDieRoll.TabIndex = 8;
            // 
            // lblMagicGoal
            // 
            this.lblMagicGoal.AutoSize = true;
            this.lblMagicGoal.Location = new System.Drawing.Point(6, 47);
            this.lblMagicGoal.Name = "lblMagicGoal";
            this.lblMagicGoal.Size = new System.Drawing.Size(61, 13);
            this.lblMagicGoal.TabIndex = 7;
            this.lblMagicGoal.Text = "Magic Goal";
            // 
            // txtMagicGoal
            // 
            this.txtMagicGoal.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMagicGoal.Location = new System.Drawing.Point(73, 45);
            this.txtMagicGoal.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtMagicGoal.Name = "txtMagicGoal";
            this.txtMagicGoal.Size = new System.Drawing.Size(83, 20);
            this.txtMagicGoal.TabIndex = 6;
            // 
            // lblMagicPerTower
            // 
            this.lblMagicPerTower.AutoSize = true;
            this.lblMagicPerTower.Location = new System.Drawing.Point(300, 22);
            this.lblMagicPerTower.Name = "lblMagicPerTower";
            this.lblMagicPerTower.Size = new System.Drawing.Size(88, 13);
            this.lblMagicPerTower.TabIndex = 5;
            this.lblMagicPerTower.Text = "Magic Per Tower";
            // 
            // txtMagicPerTower
            // 
            this.txtMagicPerTower.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txtMagicPerTower.Location = new System.Drawing.Point(394, 19);
            this.txtMagicPerTower.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMagicPerTower.Name = "txtMagicPerTower";
            this.txtMagicPerTower.Size = new System.Drawing.Size(60, 20);
            this.txtMagicPerTower.TabIndex = 4;
            // 
            // lblMagicPerLap
            // 
            this.lblMagicPerLap.AutoSize = true;
            this.lblMagicPerLap.Location = new System.Drawing.Point(152, 21);
            this.lblMagicPerLap.Name = "lblMagicPerLap";
            this.lblMagicPerLap.Size = new System.Drawing.Size(76, 13);
            this.lblMagicPerLap.TabIndex = 3;
            this.lblMagicPerLap.Text = "Magic Per Lap";
            // 
            // txtMagicPerLap
            // 
            this.txtMagicPerLap.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txtMagicPerLap.Location = new System.Drawing.Point(234, 19);
            this.txtMagicPerLap.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMagicPerLap.Name = "txtMagicPerLap";
            this.txtMagicPerLap.Size = new System.Drawing.Size(60, 20);
            this.txtMagicPerLap.TabIndex = 2;
            // 
            // lblMagicAtStart
            // 
            this.lblMagicAtStart.AutoSize = true;
            this.lblMagicAtStart.Location = new System.Drawing.Point(6, 21);
            this.lblMagicAtStart.Name = "lblMagicAtStart";
            this.lblMagicAtStart.Size = new System.Drawing.Size(74, 13);
            this.lblMagicAtStart.TabIndex = 1;
            this.lblMagicAtStart.Text = "Magic At Start";
            // 
            // txtMagicAtStart
            // 
            this.txtMagicAtStart.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txtMagicAtStart.Location = new System.Drawing.Point(86, 19);
            this.txtMagicAtStart.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMagicAtStart.Name = "txtMagicAtStart";
            this.txtMagicAtStart.Size = new System.Drawing.Size(60, 20);
            this.txtMagicAtStart.TabIndex = 0;
            // 
            // SorcererStreetMapStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 478);
            this.Controls.Add(this.groupBox1);
            this.Name = "SorcererStreetMapStatistics";
            this.Text = "Map statistics";
            this.Controls.SetChildIndex(this.btnAccept, 0);
            this.Controls.SetChildIndex(this.btnClose, 0);
            this.Controls.SetChildIndex(this.gbDescription, 0);
            this.Controls.SetChildIndex(this.txtMapName, 0);
            this.Controls.SetChildIndex(this.txtOrderNumber, 0);
            this.Controls.SetChildIndex(this.txtTileWidth, 0);
            this.Controls.SetChildIndex(this.txtTileHeight, 0);
            this.Controls.SetChildIndex(this.txtMapWidth, 0);
            this.Controls.SetChildIndex(this.txtMapHeight, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.txtTileWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoursInDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtlblTimeMultiplier)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderNumber)).EndInit();
            this.gbDescription.ResumeLayout(false);
            this.gbDescription.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHighestDieRoll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicGoal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicPerTower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicPerLap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMagicAtStart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMagicAtStart;
        private System.Windows.Forms.Label lblMagicPerLap;
        private System.Windows.Forms.Label lblMagicPerTower;
        private System.Windows.Forms.Label lblMagicGoal;
        private System.Windows.Forms.Label lblHigestDieRoll;
        public System.Windows.Forms.NumericUpDown txtMagicAtStart;
        public System.Windows.Forms.NumericUpDown txtMagicPerLap;
        public System.Windows.Forms.NumericUpDown txtMagicPerTower;
        public System.Windows.Forms.NumericUpDown txtMagicGoal;
        public System.Windows.Forms.NumericUpDown txtHighestDieRoll;
    }
}