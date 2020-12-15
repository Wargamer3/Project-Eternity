namespace ProjectEternity.Editors.UnitNormalEditor
{
    partial class UnitSizeEditor
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
            this.gbCustomisation = new System.Windows.Forms.GroupBox();
            this.txtHeight = new System.Windows.Forms.NumericUpDown();
            this.lblHeight = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.NumericUpDown();
            this.lblWidth = new System.Windows.Forms.Label();
            this.rbCustomSizeBox = new System.Windows.Forms.RadioButton();
            this.rbSizeOnly = new System.Windows.Forms.RadioButton();
            this.rbNone = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbUnitSize = new System.Windows.Forms.PictureBox();
            this.gbCustomisation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWidth)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbUnitSize)).BeginInit();
            this.SuspendLayout();
            // 
            // gbCustomisation
            // 
            this.gbCustomisation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCustomisation.Controls.Add(this.txtHeight);
            this.gbCustomisation.Controls.Add(this.lblHeight);
            this.gbCustomisation.Controls.Add(this.txtWidth);
            this.gbCustomisation.Controls.Add(this.lblWidth);
            this.gbCustomisation.Controls.Add(this.rbCustomSizeBox);
            this.gbCustomisation.Controls.Add(this.rbSizeOnly);
            this.gbCustomisation.Controls.Add(this.rbNone);
            this.gbCustomisation.Location = new System.Drawing.Point(12, 12);
            this.gbCustomisation.Name = "gbCustomisation";
            this.gbCustomisation.Size = new System.Drawing.Size(590, 42);
            this.gbCustomisation.TabIndex = 0;
            this.gbCustomisation.TabStop = false;
            this.gbCustomisation.Text = "Customisation";
            // 
            // txtHeight
            // 
            this.txtHeight.Location = new System.Drawing.Point(528, 16);
            this.txtHeight.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(56, 20);
            this.txtHeight.TabIndex = 7;
            this.txtHeight.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtHeight.ValueChanged += new System.EventHandler(this.txtHeight_ValueChanged);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(481, 21);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(41, 13);
            this.lblHeight.TabIndex = 6;
            this.lblHeight.Text = "Height:";
            // 
            // txtWidth
            // 
            this.txtWidth.Location = new System.Drawing.Point(419, 16);
            this.txtWidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(56, 20);
            this.txtWidth.TabIndex = 5;
            this.txtWidth.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtWidth.ValueChanged += new System.EventHandler(this.txtWidth_ValueChanged);
            // 
            // lblWidth
            // 
            this.lblWidth.AutoSize = true;
            this.lblWidth.Location = new System.Drawing.Point(375, 21);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(38, 13);
            this.lblWidth.TabIndex = 4;
            this.lblWidth.Text = "Width:";
            // 
            // rbCustomSizeBox
            // 
            this.rbCustomSizeBox.AutoSize = true;
            this.rbCustomSizeBox.Location = new System.Drawing.Point(138, 19);
            this.rbCustomSizeBox.Name = "rbCustomSizeBox";
            this.rbCustomSizeBox.Size = new System.Drawing.Size(103, 17);
            this.rbCustomSizeBox.TabIndex = 3;
            this.rbCustomSizeBox.TabStop = true;
            this.rbCustomSizeBox.Text = "Custom Size box";
            this.rbCustomSizeBox.UseVisualStyleBackColor = true;
            this.rbCustomSizeBox.CheckedChanged += new System.EventHandler(this.rbCustomSizeBox_CheckedChanged);
            // 
            // rbSizeOnly
            // 
            this.rbSizeOnly.AutoSize = true;
            this.rbSizeOnly.Location = new System.Drawing.Point(63, 19);
            this.rbSizeOnly.Name = "rbSizeOnly";
            this.rbSizeOnly.Size = new System.Drawing.Size(69, 17);
            this.rbSizeOnly.TabIndex = 2;
            this.rbSizeOnly.TabStop = true;
            this.rbSizeOnly.Text = "Size Only";
            this.rbSizeOnly.UseVisualStyleBackColor = true;
            this.rbSizeOnly.CheckedChanged += new System.EventHandler(this.rbSizeOnly_CheckedChanged);
            // 
            // rbNone
            // 
            this.rbNone.AutoSize = true;
            this.rbNone.Checked = true;
            this.rbNone.Location = new System.Drawing.Point(6, 19);
            this.rbNone.Name = "rbNone";
            this.rbNone.Size = new System.Drawing.Size(51, 17);
            this.rbNone.TabIndex = 2;
            this.rbNone.TabStop = true;
            this.rbNone.Text = "None";
            this.rbNone.UseVisualStyleBackColor = true;
            this.rbNone.CheckedChanged += new System.EventHandler(this.rbNone_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Location = new System.Drawing.Point(12, 60);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(590, 280);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Attack pattern";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pbUnitSize);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(584, 261);
            this.panel1.TabIndex = 0;
            // 
            // pbUnitSize
            // 
            this.pbUnitSize.Location = new System.Drawing.Point(0, 0);
            this.pbUnitSize.Name = "pbUnitSize";
            this.pbUnitSize.Size = new System.Drawing.Size(382, 308);
            this.pbUnitSize.TabIndex = 0;
            this.pbUnitSize.TabStop = false;
            this.pbUnitSize.Paint += new System.Windows.Forms.PaintEventHandler(this.pbUnitSize_Paint);
            this.pbUnitSize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbUnitSize_MouseMove);
            this.pbUnitSize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbUnitSize_MouseMove);
            // 
            // UnitSizeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 352);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.gbCustomisation);
            this.MinimumSize = new System.Drawing.Size(630, 390);
            this.Name = "UnitSizeEditor";
            this.Text = "Unit Size Editor";
            this.Shown += new System.EventHandler(this.UnitSizeEditor_Shown);
            this.gbCustomisation.ResumeLayout(false);
            this.gbCustomisation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWidth)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbUnitSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCustomisation;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbUnitSize;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.Label lblWidth;
        public System.Windows.Forms.RadioButton rbCustomSizeBox;
        public System.Windows.Forms.RadioButton rbSizeOnly;
        public System.Windows.Forms.RadioButton rbNone;
        public System.Windows.Forms.NumericUpDown txtHeight;
        public System.Windows.Forms.NumericUpDown txtWidth;
    }
}