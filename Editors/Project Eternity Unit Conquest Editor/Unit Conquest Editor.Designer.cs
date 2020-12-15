namespace ProjectEternity.Editors.UnitConquestEditor
{
    partial class UnitConquestEditor
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
            this.cbArmourType = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtVisionRange = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.txtGazCostPerTurn = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.txtMaterial = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtGaz = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAmmo = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cbMovementType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMovement = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtHP = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtWeapon1MaximumRange = new System.Windows.Forms.NumericUpDown();
            this.txtWeapon1MinimumRange = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.cbWeapon1PostMovement = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtWeapon1Name = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtWeapon2MaximumRange = new System.Windows.Forms.NumericUpDown();
            this.txtWeapon2MinimumRange = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.cbWeapon2PostMovement = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtWeapon2Name = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.clCanTransport = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVisionRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGazCostPerTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaterial)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGaz)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMovement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHP)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon1MaximumRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon1MinimumRange)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon2MaximumRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon2MinimumRange)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbArmourType);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtVisionRange);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtGazCostPerTurn);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtMaterial);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtGaz);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtAmmo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.cbMovementType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtMovement);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtHP);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(239, 264);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Statistics";
            // 
            // cbArmourType
            // 
            this.cbArmourType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbArmourType.FormattingEnabled = true;
            this.cbArmourType.Items.AddRange(new object[] {
            "Infantry",
            "Bazooka",
            "TireA",
            "TireB",
            "Tank",
            "Air",
            "Ship",
            "Transport"});
            this.cbArmourType.Location = new System.Drawing.Point(99, 182);
            this.cbArmourType.Name = "cbArmourType";
            this.cbArmourType.Size = new System.Drawing.Size(121, 21);
            this.cbArmourType.TabIndex = 18;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 185);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "Armour Type:";
            // 
            // txtVisionRange
            // 
            this.txtVisionRange.Location = new System.Drawing.Point(108, 235);
            this.txtVisionRange.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtVisionRange.Name = "txtVisionRange";
            this.txtVisionRange.Size = new System.Drawing.Size(64, 20);
            this.txtVisionRange.TabIndex = 15;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 237);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 13);
            this.label14.TabIndex = 14;
            this.label14.Text = "Vision Range:";
            // 
            // txtGazCostPerTurn
            // 
            this.txtGazCostPerTurn.Location = new System.Drawing.Point(108, 209);
            this.txtGazCostPerTurn.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtGazCostPerTurn.Name = "txtGazCostPerTurn";
            this.txtGazCostPerTurn.Size = new System.Drawing.Size(64, 20);
            this.txtGazCostPerTurn.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 211);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(94, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Gaz cost per turn: ";
            // 
            // txtMaterial
            // 
            this.txtMaterial.Location = new System.Drawing.Point(75, 126);
            this.txtMaterial.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtMaterial.Name = "txtMaterial";
            this.txtMaterial.Size = new System.Drawing.Size(64, 20);
            this.txtMaterial.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Material:";
            // 
            // txtGaz
            // 
            this.txtGaz.Location = new System.Drawing.Point(75, 100);
            this.txtGaz.Name = "txtGaz";
            this.txtGaz.Size = new System.Drawing.Size(64, 20);
            this.txtGaz.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Gaz:";
            // 
            // txtAmmo
            // 
            this.txtAmmo.Location = new System.Drawing.Point(75, 74);
            this.txtAmmo.Name = "txtAmmo";
            this.txtAmmo.Size = new System.Drawing.Size(64, 20);
            this.txtAmmo.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Ammo:";
            // 
            // cbMovementType
            // 
            this.cbMovementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMovementType.FormattingEnabled = true;
            this.cbMovementType.Items.AddRange(new object[] {
            "Infantry",
            "Bazooka",
            "TireA",
            "TireB",
            "Tank",
            "Air",
            "Ship",
            "Transport"});
            this.cbMovementType.Location = new System.Drawing.Point(99, 155);
            this.cbMovementType.Name = "cbMovementType";
            this.cbMovementType.Size = new System.Drawing.Size(121, 21);
            this.cbMovementType.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 158);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Movement Type:";
            // 
            // txtMovement
            // 
            this.txtMovement.Location = new System.Drawing.Point(75, 48);
            this.txtMovement.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtMovement.Name = "txtMovement";
            this.txtMovement.Size = new System.Drawing.Size(64, 20);
            this.txtMovement.TabIndex = 4;
            this.txtMovement.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Movement: ";
            // 
            // txtHP
            // 
            this.txtHP.Location = new System.Drawing.Point(75, 22);
            this.txtHP.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtHP.Name = "txtHP";
            this.txtHP.Size = new System.Drawing.Size(64, 20);
            this.txtHP.TabIndex = 2;
            this.txtHP.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "HP: ";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtWeapon1MaximumRange);
            this.groupBox2.Controls.Add(this.txtWeapon1MinimumRange);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.cbWeapon1PostMovement);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtWeapon1Name);
            this.groupBox2.Location = new System.Drawing.Point(257, 30);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(238, 122);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Weapon 1";
            // 
            // txtWeapon1MaximumRange
            // 
            this.txtWeapon1MaximumRange.Location = new System.Drawing.Point(136, 94);
            this.txtWeapon1MaximumRange.Name = "txtWeapon1MaximumRange";
            this.txtWeapon1MaximumRange.Size = new System.Drawing.Size(58, 20);
            this.txtWeapon1MaximumRange.TabIndex = 38;
            // 
            // txtWeapon1MinimumRange
            // 
            this.txtWeapon1MinimumRange.Location = new System.Drawing.Point(136, 68);
            this.txtWeapon1MinimumRange.Name = "txtWeapon1MinimumRange";
            this.txtWeapon1MinimumRange.Size = new System.Drawing.Size(58, 20);
            this.txtWeapon1MinimumRange.TabIndex = 37;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 97);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 13);
            this.label9.TabIndex = 33;
            this.label9.Text = "Maximum range:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 71);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(81, 13);
            this.label10.TabIndex = 31;
            this.label10.Text = "Minimum range:";
            // 
            // cbWeapon1PostMovement
            // 
            this.cbWeapon1PostMovement.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbWeapon1PostMovement.Location = new System.Drawing.Point(6, 45);
            this.cbWeapon1PostMovement.Name = "cbWeapon1PostMovement";
            this.cbWeapon1PostMovement.Size = new System.Drawing.Size(160, 17);
            this.cbWeapon1PostMovement.TabIndex = 35;
            this.cbWeapon1PostMovement.Text = "Post Movement:";
            this.cbWeapon1PostMovement.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Name: ";
            // 
            // txtWeapon1Name
            // 
            this.txtWeapon1Name.Location = new System.Drawing.Point(64, 19);
            this.txtWeapon1Name.Name = "txtWeapon1Name";
            this.txtWeapon1Name.Size = new System.Drawing.Size(100, 20);
            this.txtWeapon1Name.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtWeapon2MaximumRange);
            this.groupBox3.Controls.Add(this.txtWeapon2MinimumRange);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.cbWeapon2PostMovement);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.txtWeapon2Name);
            this.groupBox3.Location = new System.Drawing.Point(501, 30);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(238, 122);
            this.groupBox3.TabIndex = 16;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Weapon 2";
            // 
            // txtWeapon2MaximumRange
            // 
            this.txtWeapon2MaximumRange.Location = new System.Drawing.Point(136, 94);
            this.txtWeapon2MaximumRange.Name = "txtWeapon2MaximumRange";
            this.txtWeapon2MaximumRange.Size = new System.Drawing.Size(58, 20);
            this.txtWeapon2MaximumRange.TabIndex = 40;
            // 
            // txtWeapon2MinimumRange
            // 
            this.txtWeapon2MinimumRange.Location = new System.Drawing.Point(136, 68);
            this.txtWeapon2MinimumRange.Name = "txtWeapon2MinimumRange";
            this.txtWeapon2MinimumRange.Size = new System.Drawing.Size(58, 20);
            this.txtWeapon2MinimumRange.TabIndex = 39;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 97);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 13);
            this.label12.TabIndex = 37;
            this.label12.Text = "Maximum range:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 71);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(81, 13);
            this.label13.TabIndex = 36;
            this.label13.Text = "Minimum range:";
            // 
            // cbWeapon2PostMovement
            // 
            this.cbWeapon2PostMovement.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cbWeapon2PostMovement.Location = new System.Drawing.Point(6, 45);
            this.cbWeapon2PostMovement.Name = "cbWeapon2PostMovement";
            this.cbWeapon2PostMovement.Size = new System.Drawing.Size(160, 17);
            this.cbWeapon2PostMovement.TabIndex = 39;
            this.cbWeapon2PostMovement.Text = "Post Movement:";
            this.cbWeapon2PostMovement.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Name: ";
            // 
            // txtWeapon2Name
            // 
            this.txtWeapon2Name.Location = new System.Drawing.Point(64, 19);
            this.txtWeapon2Name.Name = "txtWeapon2Name";
            this.txtWeapon2Name.Size = new System.Drawing.Size(100, 20);
            this.txtWeapon2Name.TabIndex = 2;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dataGridView2);
            this.groupBox4.Location = new System.Drawing.Point(257, 158);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(238, 138);
            this.groupBox4.TabIndex = 17;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Transport";
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clCanTransport});
            this.dataGridView2.Location = new System.Drawing.Point(6, 19);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.Size = new System.Drawing.Size(226, 110);
            this.dataGridView2.TabIndex = 37;
            // 
            // clCanTransport
            // 
            this.clCanTransport.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clCanTransport.HeaderText = "Can Transport";
            this.clCanTransport.Items.AddRange(new object[] {
            "Vehicule"});
            this.clCanTransport.Name = "clCanTransport";
            this.clCanTransport.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.clCanTransport.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(752, 24);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // UnitConquestEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 304);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitConquestEditor";
            this.Text = "UnitConquestEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtVisionRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGazCostPerTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaterial)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtGaz)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtAmmo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMovement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHP)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon1MaximumRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon1MinimumRange)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon2MaximumRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWeapon2MinimumRange)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown txtHP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbMovementType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtMovement;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtMaterial;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtGaz;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown txtAmmo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtWeapon1Name;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtWeapon2Name;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox cbWeapon1PostMovement;
        private System.Windows.Forms.NumericUpDown txtGazCostPerTurn;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox cbWeapon2PostMovement;
        private System.Windows.Forms.NumericUpDown txtVisionRange;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cbArmourType;
        private System.Windows.Forms.Label label16;
        public System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.DataGridViewComboBoxColumn clCanTransport;
        private System.Windows.Forms.NumericUpDown txtWeapon1MinimumRange;
        private System.Windows.Forms.NumericUpDown txtWeapon1MaximumRange;
        private System.Windows.Forms.NumericUpDown txtWeapon2MaximumRange;
        private System.Windows.Forms.NumericUpDown txtWeapon2MinimumRange;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
    }
}