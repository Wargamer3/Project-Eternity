namespace ProjectEternity.Editors.AttackEditor
{
    partial class MAPAttackEditor
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
            this.txtAttackDelay = new System.Windows.Forms.NumericUpDown();
            this.lblAttackDelay = new System.Windows.Forms.Label();
            this.txtAttackHeight = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAttackWidth = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.rbTargeted = new System.Windows.Forms.RadioButton();
            this.rbDirection = new System.Windows.Forms.RadioButton();
            this.rbSpread = new System.Windows.Forms.RadioButton();
            this.ckFriendlyFire = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbMAPArea = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMAPArea)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtAttackDelay);
            this.groupBox1.Controls.Add(this.lblAttackDelay);
            this.groupBox1.Controls.Add(this.txtAttackHeight);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtAttackWidth);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.rbTargeted);
            this.groupBox1.Controls.Add(this.rbDirection);
            this.groupBox1.Controls.Add(this.rbSpread);
            this.groupBox1.Controls.Add(this.ckFriendlyFire);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(510, 69);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Attack Information";
            // 
            // txtAttackDelay
            // 
            this.txtAttackDelay.Location = new System.Drawing.Point(166, 42);
            this.txtAttackDelay.Name = "txtAttackDelay";
            this.txtAttackDelay.Size = new System.Drawing.Size(56, 20);
            this.txtAttackDelay.TabIndex = 9;
            // 
            // lblAttackDelay
            // 
            this.lblAttackDelay.AutoSize = true;
            this.lblAttackDelay.Location = new System.Drawing.Point(91, 44);
            this.lblAttackDelay.Name = "lblAttackDelay";
            this.lblAttackDelay.Size = new System.Drawing.Size(69, 13);
            this.lblAttackDelay.TabIndex = 8;
            this.lblAttackDelay.Text = "Attack delay:";
            // 
            // txtAttackHeight
            // 
            this.txtAttackHeight.Location = new System.Drawing.Point(434, 16);
            this.txtAttackHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtAttackHeight.Name = "txtAttackHeight";
            this.txtAttackHeight.Size = new System.Drawing.Size(56, 20);
            this.txtAttackHeight.TabIndex = 7;
            this.txtAttackHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtAttackHeight.ValueChanged += new System.EventHandler(this.txtAttackHeight_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(355, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Attack height:";
            // 
            // txtAttackWidth
            // 
            this.txtAttackWidth.Location = new System.Drawing.Point(293, 16);
            this.txtAttackWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtAttackWidth.Name = "txtAttackWidth";
            this.txtAttackWidth.Size = new System.Drawing.Size(56, 20);
            this.txtAttackWidth.TabIndex = 5;
            this.txtAttackWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtAttackWidth.ValueChanged += new System.EventHandler(this.txtAttackWidth_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(218, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Attack width:";
            // 
            // rbTargeted
            // 
            this.rbTargeted.AutoSize = true;
            this.rbTargeted.Location = new System.Drawing.Point(144, 19);
            this.rbTargeted.Name = "rbTargeted";
            this.rbTargeted.Size = new System.Drawing.Size(68, 17);
            this.rbTargeted.TabIndex = 3;
            this.rbTargeted.TabStop = true;
            this.rbTargeted.Text = "Targeted";
            this.rbTargeted.UseVisualStyleBackColor = true;
            // 
            // rbDirection
            // 
            this.rbDirection.AutoSize = true;
            this.rbDirection.Location = new System.Drawing.Point(71, 19);
            this.rbDirection.Name = "rbDirection";
            this.rbDirection.Size = new System.Drawing.Size(67, 17);
            this.rbDirection.TabIndex = 2;
            this.rbDirection.TabStop = true;
            this.rbDirection.Text = "Direction";
            this.rbDirection.UseVisualStyleBackColor = true;
            // 
            // rbSpread
            // 
            this.rbSpread.AutoSize = true;
            this.rbSpread.Checked = true;
            this.rbSpread.Location = new System.Drawing.Point(6, 19);
            this.rbSpread.Name = "rbSpread";
            this.rbSpread.Size = new System.Drawing.Size(59, 17);
            this.rbSpread.TabIndex = 2;
            this.rbSpread.TabStop = true;
            this.rbSpread.Text = "Spread";
            this.rbSpread.UseVisualStyleBackColor = true;
            // 
            // ckFriendlyFire
            // 
            this.ckFriendlyFire.AutoSize = true;
            this.ckFriendlyFire.Location = new System.Drawing.Point(6, 43);
            this.ckFriendlyFire.Name = "ckFriendlyFire";
            this.ckFriendlyFire.Size = new System.Drawing.Size(79, 17);
            this.ckFriendlyFire.TabIndex = 1;
            this.ckFriendlyFire.Text = "Friendly fire";
            this.ckFriendlyFire.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(12, 87);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(510, 276);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Attack pattern";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pbMAPArea);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(504, 257);
            this.panel1.TabIndex = 0;
            // 
            // pbMAPArea
            // 
            this.pbMAPArea.Location = new System.Drawing.Point(0, 0);
            this.pbMAPArea.Name = "pbMAPArea";
            this.pbMAPArea.Size = new System.Drawing.Size(382, 312);
            this.pbMAPArea.TabIndex = 0;
            this.pbMAPArea.TabStop = false;
            this.pbMAPArea.Paint += new System.Windows.Forms.PaintEventHandler(this.pbMAPArea_Paint);
            this.pbMAPArea.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbMAPArea_MouseMove);
            this.pbMAPArea.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbMAPArea_MouseMove);
            // 
            // MAPAttackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 375);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(550, 390);
            this.Name = "MAPAttackEditor";
            this.Text = "MAP Attack Editor";
            this.Shown += new System.EventHandler(this.MAPAttackEditor_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAttackWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbMAPArea)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbMAPArea;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox ckFriendlyFire;
        public System.Windows.Forms.RadioButton rbTargeted;
        public System.Windows.Forms.RadioButton rbDirection;
        public System.Windows.Forms.RadioButton rbSpread;
        public System.Windows.Forms.NumericUpDown txtAttackHeight;
        public System.Windows.Forms.NumericUpDown txtAttackWidth;
        public System.Windows.Forms.NumericUpDown txtAttackDelay;
        private System.Windows.Forms.Label lblAttackDelay;
    }
}