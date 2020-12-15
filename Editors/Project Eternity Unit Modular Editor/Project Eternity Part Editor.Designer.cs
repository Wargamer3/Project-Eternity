namespace ProjectEternity.Editors.UnitModularEditor
{
    partial class ProjectEternityPartEditor
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtBaseMovement = new System.Windows.Forms.NumericUpDown();
            this.txtBaseMobility = new System.Windows.Forms.NumericUpDown();
            this.txtBaseArmor = new System.Windows.Forms.NumericUpDown();
            this.txtBaseEN = new System.Windows.Forms.NumericUpDown();
            this.txtBaseHP = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtHIT = new System.Windows.Forms.NumericUpDown();
            this.txtEVA = new System.Windows.Forms.NumericUpDown();
            this.txtDEF = new System.Windows.Forms.NumericUpDown();
            this.txtRNG = new System.Windows.Forms.NumericUpDown();
            this.txtMEL = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtPrice = new System.Windows.Forms.NumericUpDown();
            this.lblPrice = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtSKL = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseMovement)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseMobility)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseArmor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseEN)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseHP)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHIT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEVA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDEF)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRNG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMEL)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSKL)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(568, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBaseMovement);
            this.groupBox1.Controls.Add(this.txtBaseMobility);
            this.groupBox1.Controls.Add(this.txtBaseArmor);
            this.groupBox1.Controls.Add(this.txtBaseEN);
            this.groupBox1.Controls.Add(this.txtBaseHP);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 183);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 152);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Unit boosts";
            // 
            // txtBaseMovement
            // 
            this.txtBaseMovement.Location = new System.Drawing.Point(145, 123);
            this.txtBaseMovement.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtBaseMovement.Name = "txtBaseMovement";
            this.txtBaseMovement.Size = new System.Drawing.Size(120, 20);
            this.txtBaseMovement.TabIndex = 35;
            // 
            // txtBaseMobility
            // 
            this.txtBaseMobility.Location = new System.Drawing.Point(145, 97);
            this.txtBaseMobility.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtBaseMobility.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtBaseMobility.Name = "txtBaseMobility";
            this.txtBaseMobility.Size = new System.Drawing.Size(120, 20);
            this.txtBaseMobility.TabIndex = 34;
            // 
            // txtBaseArmor
            // 
            this.txtBaseArmor.Location = new System.Drawing.Point(145, 71);
            this.txtBaseArmor.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtBaseArmor.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtBaseArmor.Name = "txtBaseArmor";
            this.txtBaseArmor.Size = new System.Drawing.Size(120, 20);
            this.txtBaseArmor.TabIndex = 33;
            // 
            // txtBaseEN
            // 
            this.txtBaseEN.Location = new System.Drawing.Point(145, 45);
            this.txtBaseEN.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtBaseEN.Name = "txtBaseEN";
            this.txtBaseEN.Size = new System.Drawing.Size(120, 20);
            this.txtBaseEN.TabIndex = 32;
            // 
            // txtBaseHP
            // 
            this.txtBaseHP.Location = new System.Drawing.Point(145, 19);
            this.txtBaseHP.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtBaseHP.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
            this.txtBaseHP.Name = "txtBaseHP";
            this.txtBaseHP.Size = new System.Drawing.Size(120, 20);
            this.txtBaseHP.TabIndex = 31;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Base Movement:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Base Mobility:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Base Armor:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Base EN:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Base HP:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtSKL);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.txtHIT);
            this.groupBox2.Controls.Add(this.txtEVA);
            this.groupBox2.Controls.Add(this.txtDEF);
            this.groupBox2.Controls.Add(this.txtRNG);
            this.groupBox2.Controls.Add(this.txtMEL);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(289, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(267, 178);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Pilot boosts";
            // 
            // txtHIT
            // 
            this.txtHIT.Location = new System.Drawing.Point(141, 152);
            this.txtHIT.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtHIT.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtHIT.Name = "txtHIT";
            this.txtHIT.Size = new System.Drawing.Size(120, 20);
            this.txtHIT.TabIndex = 39;
            // 
            // txtEVA
            // 
            this.txtEVA.Location = new System.Drawing.Point(141, 126);
            this.txtEVA.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtEVA.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtEVA.Name = "txtEVA";
            this.txtEVA.Size = new System.Drawing.Size(120, 20);
            this.txtEVA.TabIndex = 38;
            // 
            // txtDEF
            // 
            this.txtDEF.Location = new System.Drawing.Point(141, 71);
            this.txtDEF.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtDEF.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtDEF.Name = "txtDEF";
            this.txtDEF.Size = new System.Drawing.Size(120, 20);
            this.txtDEF.TabIndex = 37;
            // 
            // txtRNG
            // 
            this.txtRNG.Location = new System.Drawing.Point(141, 45);
            this.txtRNG.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtRNG.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtRNG.Name = "txtRNG";
            this.txtRNG.Size = new System.Drawing.Size(120, 20);
            this.txtRNG.TabIndex = 36;
            // 
            // txtMEL
            // 
            this.txtMEL.Location = new System.Drawing.Point(141, 19);
            this.txtMEL.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMEL.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtMEL.Name = "txtMEL";
            this.txtMEL.Size = new System.Drawing.Size(120, 20);
            this.txtMEL.TabIndex = 35;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 154);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "HIT:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 128);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 14;
            this.label9.Text = "EVA:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "DEF:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "RNG:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "MEL:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtPrice);
            this.groupBox3.Controls.Add(this.lblPrice);
            this.groupBox3.Controls.Add(this.txtDescription);
            this.groupBox3.Controls.Add(this.lblDescription);
            this.groupBox3.Location = new System.Drawing.Point(12, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(271, 150);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Item Information";
            // 
            // txtPrice
            // 
            this.txtPrice.Location = new System.Drawing.Point(145, 19);
            this.txtPrice.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.txtPrice.Name = "txtPrice";
            this.txtPrice.Size = new System.Drawing.Size(120, 20);
            this.txtPrice.TabIndex = 27;
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(96, 21);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(34, 13);
            this.lblPrice.TabIndex = 18;
            this.lblPrice.Text = "Price:";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 58);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(256, 86);
            this.txtDescription.TabIndex = 21;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 42);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 20;
            this.lblDescription.Text = "Description:";
            // 
            // txtSKL
            // 
            this.txtSKL.Location = new System.Drawing.Point(141, 97);
            this.txtSKL.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtSKL.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.txtSKL.Name = "txtSKL";
            this.txtSKL.Size = new System.Drawing.Size(120, 20);
            this.txtSKL.TabIndex = 41;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 99);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(30, 13);
            this.label11.TabIndex = 40;
            this.label11.Text = "SKL:";
            // 
            // ProjectEternityPartEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 344);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ProjectEternityPartEditor";
            this.Text = "Project Eternity Part Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseMovement)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseMobility)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseArmor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseEN)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseHP)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHIT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEVA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDEF)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtRNG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMEL)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSKL)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.NumericUpDown txtBaseMovement;
        private System.Windows.Forms.NumericUpDown txtBaseMobility;
        private System.Windows.Forms.NumericUpDown txtBaseArmor;
        private System.Windows.Forms.NumericUpDown txtBaseEN;
        private System.Windows.Forms.NumericUpDown txtBaseHP;
        private System.Windows.Forms.NumericUpDown txtMEL;
        private System.Windows.Forms.NumericUpDown txtRNG;
        private System.Windows.Forms.NumericUpDown txtHIT;
        private System.Windows.Forms.NumericUpDown txtEVA;
        private System.Windows.Forms.NumericUpDown txtDEF;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.NumericUpDown txtPrice;
        private System.Windows.Forms.NumericUpDown txtSKL;
        private System.Windows.Forms.Label label11;

    }
}

