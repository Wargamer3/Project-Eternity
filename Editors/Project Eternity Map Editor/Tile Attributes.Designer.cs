namespace ProjectEternity.Editors.MapEditor
{
    partial class TileAttributes
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
            this.cboTerrainType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstTerrainBonus = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboTerrainBonusType = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnAddNewBonus = new System.Windows.Forms.Button();
            this.btnRemoveBonus = new System.Windows.Forms.Button();
            this.btnClearBonuses = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cboTerrainBonusActivation = new System.Windows.Forms.ComboBox();
            this.txtMVEnterCost = new System.Windows.Forms.NumericUpDown();
            this.txtMVMoveCost = new System.Windows.Forms.NumericUpDown();
            this.txtBonusValue = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.txtMVEnterCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMVMoveCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).BeginInit();
            this.SuspendLayout();
            // 
            // cboTerrainType
            // 
            this.cboTerrainType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainType.FormattingEnabled = true;
            this.cboTerrainType.Items.AddRange(new object[] {
            "Air",
            "Land",
            "Sea",
            "Space"});
            this.cboTerrainType.Location = new System.Drawing.Point(12, 47);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Terrain type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MV enter cost";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "MV move cost";
            // 
            // lstTerrainBonus
            // 
            this.lstTerrainBonus.FormattingEnabled = true;
            this.lstTerrainBonus.Location = new System.Drawing.Point(171, 47);
            this.lstTerrainBonus.Name = "lstTerrainBonus";
            this.lstTerrainBonus.Size = new System.Drawing.Size(176, 225);
            this.lstTerrainBonus.TabIndex = 5;
            this.lstTerrainBonus.SelectedIndexChanged += new System.EventHandler(this.lstTerrainBonus_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(168, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Terrain bonus";
            // 
            // cboTerrainBonusType
            // 
            this.cboTerrainBonusType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusType.FormattingEnabled = true;
            this.cboTerrainBonusType.Location = new System.Drawing.Point(353, 47);
            this.cboTerrainBonusType.Name = "cboTerrainBonusType";
            this.cboTerrainBonusType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusType.TabIndex = 7;
            this.cboTerrainBonusType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainBonusType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(353, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Terrain bonus type";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(353, 143);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Bonus value";
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(403, 292);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 11;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnAddNewBonus
            // 
            this.btnAddNewBonus.Location = new System.Drawing.Point(356, 191);
            this.btnAddNewBonus.Name = "btnAddNewBonus";
            this.btnAddNewBonus.Size = new System.Drawing.Size(100, 23);
            this.btnAddNewBonus.TabIndex = 12;
            this.btnAddNewBonus.Text = "Add new bonus";
            this.btnAddNewBonus.UseVisualStyleBackColor = true;
            this.btnAddNewBonus.Click += new System.EventHandler(this.btnAddNewBonus_Click);
            // 
            // btnRemoveBonus
            // 
            this.btnRemoveBonus.Location = new System.Drawing.Point(356, 220);
            this.btnRemoveBonus.Name = "btnRemoveBonus";
            this.btnRemoveBonus.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveBonus.TabIndex = 13;
            this.btnRemoveBonus.Text = "Remove bonus";
            this.btnRemoveBonus.UseVisualStyleBackColor = true;
            this.btnRemoveBonus.Click += new System.EventHandler(this.btnRemoveBonus_Click);
            // 
            // btnClearBonuses
            // 
            this.btnClearBonuses.Location = new System.Drawing.Point(356, 249);
            this.btnClearBonuses.Name = "btnClearBonuses";
            this.btnClearBonuses.Size = new System.Drawing.Size(100, 23);
            this.btnClearBonuses.TabIndex = 14;
            this.btnClearBonuses.Text = "Clear bonuses";
            this.btnClearBonuses.UseVisualStyleBackColor = true;
            this.btnClearBonuses.Click += new System.EventHandler(this.btnClearBonuses_Click);
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(353, 87);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Terrain bonus activation";
            // 
            // cboTerrainBonusActivation
            // 
            this.cboTerrainBonusActivation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusActivation.FormattingEnabled = true;
            this.cboTerrainBonusActivation.Location = new System.Drawing.Point(353, 103);
            this.cboTerrainBonusActivation.Name = "cboTerrainBonusActivation";
            this.cboTerrainBonusActivation.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusActivation.TabIndex = 15;
            this.cboTerrainBonusActivation.SelectedIndexChanged += new System.EventHandler(this.cboTerrainBonusActivation_SelectedIndexChanged);
            // 
            // txtMVEnterCost
            // 
            this.txtMVEnterCost.Location = new System.Drawing.Point(15, 103);
            this.txtMVEnterCost.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txtMVEnterCost.Name = "txtMVEnterCost";
            this.txtMVEnterCost.Size = new System.Drawing.Size(100, 20);
            this.txtMVEnterCost.TabIndex = 17;
            this.txtMVEnterCost.ValueChanged += new System.EventHandler(this.txtMVEnterCost_TextChanged);
            // 
            // txtMVMoveCost
            // 
            this.txtMVMoveCost.Location = new System.Drawing.Point(15, 160);
            this.txtMVMoveCost.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txtMVMoveCost.Name = "txtMVMoveCost";
            this.txtMVMoveCost.Size = new System.Drawing.Size(100, 20);
            this.txtMVMoveCost.TabIndex = 18;
            this.txtMVMoveCost.ValueChanged += new System.EventHandler(this.txtMVMoveCost_TextChanged);
            // 
            // txtBonusValue
            // 
            this.txtBonusValue.Location = new System.Drawing.Point(356, 159);
            this.txtBonusValue.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.txtBonusValue.Name = "txtBonusValue";
            this.txtBonusValue.Size = new System.Drawing.Size(100, 20);
            this.txtBonusValue.TabIndex = 19;
            this.txtBonusValue.ValueChanged += new System.EventHandler(this.txtBonusValue_TextChanged);
            // 
            // TileAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 327);
            this.Controls.Add(this.txtBonusValue);
            this.Controls.Add(this.txtMVMoveCost);
            this.Controls.Add(this.txtMVEnterCost);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cboTerrainBonusActivation);
            this.Controls.Add(this.btnClearBonuses);
            this.Controls.Add(this.btnRemoveBonus);
            this.Controls.Add(this.btnAddNewBonus);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cboTerrainBonusType);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstTerrainBonus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboTerrainType);
            this.Name = "TileAttributes";
            this.Text = "Tile Attributes";
            ((System.ComponentModel.ISupportInitialize)(this.txtMVEnterCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMVMoveCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTerrainType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstTerrainBonus;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboTerrainBonusType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnAddNewBonus;
        private System.Windows.Forms.Button btnRemoveBonus;
        private System.Windows.Forms.Button btnClearBonuses;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cboTerrainBonusActivation;
        private System.Windows.Forms.NumericUpDown txtMVEnterCost;
        private System.Windows.Forms.NumericUpDown txtMVMoveCost;
        private System.Windows.Forms.NumericUpDown txtBonusValue;
    }
}