namespace ProjectEternity.Editors.UnitTransformingEditor
{
    partial class UnitTransformingEditor
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
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lstUnits = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cbTurnLimit = new System.Windows.Forms.CheckBox();
            this.txtTurnLimit = new System.Windows.Forms.NumericUpDown();
            this.cbWillRequirement = new System.Windows.Forms.CheckBox();
            this.txtWillRequirement = new System.Windows.Forms.NumericUpDown();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listBox3 = new System.Windows.Forms.ListBox();
            this.listBox2 = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.txtPermanentTransformation = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTurnLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWillRequirement)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnAdd);
            this.groupBox1.Controls.Add(this.lstUnits);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 240);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Units";
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(96, 211);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 211);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 1;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lstUnits
            // 
            this.lstUnits.FormattingEnabled = true;
            this.lstUnits.Location = new System.Drawing.Point(6, 19);
            this.lstUnits.Name = "lstUnits";
            this.lstUnits.Size = new System.Drawing.Size(165, 186);
            this.lstUnits.TabIndex = 0;
            this.lstUnits.SelectedIndexChanged += new System.EventHandler(this.lstUnits_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPermanentTransformation);
            this.groupBox2.Controls.Add(this.checkBox1);
            this.groupBox2.Controls.Add(this.cbTurnLimit);
            this.groupBox2.Controls.Add(this.txtTurnLimit);
            this.groupBox2.Controls.Add(this.cbWillRequirement);
            this.groupBox2.Controls.Add(this.txtWillRequirement);
            this.groupBox2.Location = new System.Drawing.Point(195, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(230, 114);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Limitations";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(6, 71);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(103, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "Skill requirement";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // cbTurnLimit
            // 
            this.cbTurnLimit.AutoSize = true;
            this.cbTurnLimit.Location = new System.Drawing.Point(6, 48);
            this.cbTurnLimit.Name = "cbTurnLimit";
            this.cbTurnLimit.Size = new System.Drawing.Size(68, 17);
            this.cbTurnLimit.TabIndex = 3;
            this.cbTurnLimit.Text = "Turn limit";
            this.cbTurnLimit.UseVisualStyleBackColor = true;
            this.cbTurnLimit.CheckedChanged += new System.EventHandler(this.cbTurnLimit_CheckedChanged);
            // 
            // txtTurnLimit
            // 
            this.txtTurnLimit.Enabled = false;
            this.txtTurnLimit.Location = new System.Drawing.Point(145, 45);
            this.txtTurnLimit.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.txtTurnLimit.Name = "txtTurnLimit";
            this.txtTurnLimit.Size = new System.Drawing.Size(79, 20);
            this.txtTurnLimit.TabIndex = 2;
            this.txtTurnLimit.ValueChanged += new System.EventHandler(this.txtTurnLimit_ValueChanged);
            // 
            // cbWillRequirement
            // 
            this.cbWillRequirement.AutoSize = true;
            this.cbWillRequirement.Location = new System.Drawing.Point(6, 22);
            this.cbWillRequirement.Name = "cbWillRequirement";
            this.cbWillRequirement.Size = new System.Drawing.Size(101, 17);
            this.cbWillRequirement.TabIndex = 1;
            this.cbWillRequirement.Text = "Will requirement";
            this.cbWillRequirement.UseVisualStyleBackColor = true;
            this.cbWillRequirement.CheckedChanged += new System.EventHandler(this.cbWillRequirement_CheckedChanged);
            // 
            // txtWillRequirement
            // 
            this.txtWillRequirement.Enabled = false;
            this.txtWillRequirement.Location = new System.Drawing.Point(145, 19);
            this.txtWillRequirement.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtWillRequirement.Name = "txtWillRequirement";
            this.txtWillRequirement.Size = new System.Drawing.Size(79, 20);
            this.txtWillRequirement.TabIndex = 0;
            this.txtWillRequirement.ValueChanged += new System.EventHandler(this.txtWillRequirement_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listBox3);
            this.groupBox3.Controls.Add(this.listBox2);
            this.groupBox3.Location = new System.Drawing.Point(195, 147);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(301, 120);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Pilot order";
            // 
            // listBox3
            // 
            this.listBox3.FormattingEnabled = true;
            this.listBox3.Location = new System.Drawing.Point(192, 19);
            this.listBox3.Name = "listBox3";
            this.listBox3.Size = new System.Drawing.Size(103, 95);
            this.listBox3.TabIndex = 1;
            // 
            // listBox2
            // 
            this.listBox2.FormattingEnabled = true;
            this.listBox2.Location = new System.Drawing.Point(6, 19);
            this.listBox2.Name = "listBox2";
            this.listBox2.Size = new System.Drawing.Size(103, 95);
            this.listBox2.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(506, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // txtPermanentTransformation
            // 
            this.txtPermanentTransformation.AutoSize = true;
            this.txtPermanentTransformation.Location = new System.Drawing.Point(6, 94);
            this.txtPermanentTransformation.Name = "txtPermanentTransformation";
            this.txtPermanentTransformation.Size = new System.Drawing.Size(146, 17);
            this.txtPermanentTransformation.TabIndex = 5;
            this.txtPermanentTransformation.Text = "Permanent transformation";
            this.txtPermanentTransformation.UseVisualStyleBackColor = true;
            this.txtPermanentTransformation.CheckedChanged += new System.EventHandler(this.txtPermanentTransformation_CheckedChanged);
            // 
            // UnitTransformingEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 275);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitTransformingEditor";
            this.Text = "Unit Transforming Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTurnLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWillRequirement)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstUnits;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbTurnLimit;
        private System.Windows.Forms.NumericUpDown txtTurnLimit;
        private System.Windows.Forms.CheckBox cbWillRequirement;
        private System.Windows.Forms.NumericUpDown txtWillRequirement;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox listBox3;
        private System.Windows.Forms.ListBox listBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.CheckBox txtPermanentTransformation;
    }
}