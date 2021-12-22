namespace ProjectEternity.GameScreens.BattleMapScreen
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
            this.lblBattleAnimationBackground = new System.Windows.Forms.Label();
            this.cboBattleAnimationBackground = new System.Windows.Forms.ComboBox();
            this.btnDeleteBattleAnimationBackground = new System.Windows.Forms.Button();
            this.btnNewBattleAnimationBackground = new System.Windows.Forms.Button();
            this.lblBattleAnimationForeground = new System.Windows.Forms.Label();
            this.cboBattleAnimationForeground = new System.Windows.Forms.ComboBox();
            this.txtHeight = new System.Windows.Forms.NumericUpDown();
            this.lblHeight = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txtMVEnterCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMVMoveCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).BeginInit();
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
            "Space",
            "Wall",
            "Void"});
            this.cboTerrainType.Location = new System.Drawing.Point(12, 25);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Terrain type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MV enter cost";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(100, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "MV move cost";
            // 
            // lstTerrainBonus
            // 
            this.lstTerrainBonus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lstTerrainBonus.FormattingEnabled = true;
            this.lstTerrainBonus.Location = new System.Drawing.Point(320, 25);
            this.lstTerrainBonus.Name = "lstTerrainBonus";
            this.lstTerrainBonus.Size = new System.Drawing.Size(117, 82);
            this.lstTerrainBonus.TabIndex = 5;
            this.lstTerrainBonus.SelectedIndexChanged += new System.EventHandler(this.lstTerrainBonus_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(317, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Terrain bonus";
            // 
            // cboTerrainBonusType
            // 
            this.cboTerrainBonusType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTerrainBonusType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusType.FormattingEnabled = true;
            this.cboTerrainBonusType.Location = new System.Drawing.Point(317, 126);
            this.cboTerrainBonusType.Name = "cboTerrainBonusType";
            this.cboTerrainBonusType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusType.TabIndex = 7;
            this.cboTerrainBonusType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainBonusType_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.Location = new System.Drawing.Point(317, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Terrain bonus type";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(317, 190);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Bonus value";
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(362, 244);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 11;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            // 
            // btnAddNewBonus
            // 
            this.btnAddNewBonus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNewBonus.Location = new System.Drawing.Point(214, 25);
            this.btnAddNewBonus.Name = "btnAddNewBonus";
            this.btnAddNewBonus.Size = new System.Drawing.Size(100, 23);
            this.btnAddNewBonus.TabIndex = 12;
            this.btnAddNewBonus.Text = "Add new bonus";
            this.btnAddNewBonus.UseVisualStyleBackColor = true;
            this.btnAddNewBonus.Click += new System.EventHandler(this.btnAddNewBonus_Click);
            // 
            // btnRemoveBonus
            // 
            this.btnRemoveBonus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBonus.Location = new System.Drawing.Point(214, 54);
            this.btnRemoveBonus.Name = "btnRemoveBonus";
            this.btnRemoveBonus.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveBonus.TabIndex = 13;
            this.btnRemoveBonus.Text = "Remove bonus";
            this.btnRemoveBonus.UseVisualStyleBackColor = true;
            this.btnRemoveBonus.Click += new System.EventHandler(this.btnRemoveBonus_Click);
            // 
            // btnClearBonuses
            // 
            this.btnClearBonuses.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearBonuses.Location = new System.Drawing.Point(214, 83);
            this.btnClearBonuses.Name = "btnClearBonuses";
            this.btnClearBonuses.Size = new System.Drawing.Size(100, 23);
            this.btnClearBonuses.TabIndex = 14;
            this.btnClearBonuses.Text = "Clear bonuses";
            this.btnClearBonuses.UseVisualStyleBackColor = true;
            this.btnClearBonuses.Click += new System.EventHandler(this.btnClearBonuses_Click);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.Location = new System.Drawing.Point(317, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(121, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Terrain bonus activation";
            // 
            // cboTerrainBonusActivation
            // 
            this.cboTerrainBonusActivation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboTerrainBonusActivation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusActivation.FormattingEnabled = true;
            this.cboTerrainBonusActivation.Location = new System.Drawing.Point(317, 166);
            this.cboTerrainBonusActivation.Name = "cboTerrainBonusActivation";
            this.cboTerrainBonusActivation.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusActivation.TabIndex = 15;
            this.cboTerrainBonusActivation.SelectedIndexChanged += new System.EventHandler(this.cboTerrainBonusActivation_SelectedIndexChanged);
            // 
            // txtMVEnterCost
            // 
            this.txtMVEnterCost.Location = new System.Drawing.Point(15, 65);
            this.txtMVEnterCost.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txtMVEnterCost.Name = "txtMVEnterCost";
            this.txtMVEnterCost.Size = new System.Drawing.Size(72, 20);
            this.txtMVEnterCost.TabIndex = 17;
            this.txtMVEnterCost.ValueChanged += new System.EventHandler(this.txtMVEnterCost_TextChanged);
            // 
            // txtMVMoveCost
            // 
            this.txtMVMoveCost.Location = new System.Drawing.Point(103, 66);
            this.txtMVMoveCost.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.txtMVMoveCost.Name = "txtMVMoveCost";
            this.txtMVMoveCost.Size = new System.Drawing.Size(72, 20);
            this.txtMVMoveCost.TabIndex = 18;
            this.txtMVMoveCost.ValueChanged += new System.EventHandler(this.txtMVMoveCost_TextChanged);
            // 
            // txtBonusValue
            // 
            this.txtBonusValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBonusValue.Location = new System.Drawing.Point(320, 206);
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
            // lblBattleAnimationBackground
            // 
            this.lblBattleAnimationBackground.AutoSize = true;
            this.lblBattleAnimationBackground.Location = new System.Drawing.Point(12, 134);
            this.lblBattleAnimationBackground.Name = "lblBattleAnimationBackground";
            this.lblBattleAnimationBackground.Size = new System.Drawing.Size(142, 13);
            this.lblBattleAnimationBackground.TabIndex = 25;
            this.lblBattleAnimationBackground.Text = "Battle animation background";
            // 
            // cboBattleAnimationBackground
            // 
            this.cboBattleAnimationBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationBackground.FormattingEnabled = true;
            this.cboBattleAnimationBackground.Location = new System.Drawing.Point(12, 150);
            this.cboBattleAnimationBackground.Name = "cboBattleAnimationBackground";
            this.cboBattleAnimationBackground.Size = new System.Drawing.Size(163, 21);
            this.cboBattleAnimationBackground.TabIndex = 24;
            this.cboBattleAnimationBackground.SelectedIndexChanged += new System.EventHandler(this.cboBattleAnimationBackground_SelectedIndexChanged);
            // 
            // btnDeleteBattleAnimationBackground
            // 
            this.btnDeleteBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteBattleAnimationBackground.Location = new System.Drawing.Point(96, 218);
            this.btnDeleteBattleAnimationBackground.Name = "btnDeleteBattleAnimationBackground";
            this.btnDeleteBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnDeleteBattleAnimationBackground.TabIndex = 23;
            this.btnDeleteBattleAnimationBackground.Text = "Delete";
            this.btnDeleteBattleAnimationBackground.UseVisualStyleBackColor = true;
            this.btnDeleteBattleAnimationBackground.Click += new System.EventHandler(this.btnDeleteBattleAnimationBackground_Click);
            // 
            // btnNewBattleAnimationBackground
            // 
            this.btnNewBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewBattleAnimationBackground.Location = new System.Drawing.Point(8, 218);
            this.btnNewBattleAnimationBackground.Name = "btnNewBattleAnimationBackground";
            this.btnNewBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnNewBattleAnimationBackground.TabIndex = 22;
            this.btnNewBattleAnimationBackground.Text = "New";
            this.btnNewBattleAnimationBackground.UseVisualStyleBackColor = true;
            this.btnNewBattleAnimationBackground.Click += new System.EventHandler(this.btnNewBattleAnimationBackground_Click);
            // 
            // lblBattleAnimationForeground
            // 
            this.lblBattleAnimationForeground.AutoSize = true;
            this.lblBattleAnimationForeground.Location = new System.Drawing.Point(12, 174);
            this.lblBattleAnimationForeground.Name = "lblBattleAnimationForeground";
            this.lblBattleAnimationForeground.Size = new System.Drawing.Size(136, 13);
            this.lblBattleAnimationForeground.TabIndex = 27;
            this.lblBattleAnimationForeground.Text = "Battle animation foreground";
            // 
            // cboBattleAnimationForeground
            // 
            this.cboBattleAnimationForeground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationForeground.FormattingEnabled = true;
            this.cboBattleAnimationForeground.Location = new System.Drawing.Point(12, 190);
            this.cboBattleAnimationForeground.Name = "cboBattleAnimationForeground";
            this.cboBattleAnimationForeground.Size = new System.Drawing.Size(163, 21);
            this.cboBattleAnimationForeground.TabIndex = 26;
            this.cboBattleAnimationForeground.SelectedIndexChanged += new System.EventHandler(this.cboBattleAnimationForeground_SelectedIndexChanged);
            // 
            // txtHeight
            // 
            this.txtHeight.DecimalPlaces = 2;
            this.txtHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.txtHeight.Location = new System.Drawing.Point(15, 105);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(72, 20);
            this.txtHeight.TabIndex = 29;
            this.txtHeight.ValueChanged += new System.EventHandler(this.txtHeight_ValueChanged);
            // 
            // lblHeight
            // 
            this.lblHeight.AutoSize = true;
            this.lblHeight.Location = new System.Drawing.Point(12, 88);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(38, 13);
            this.lblHeight.TabIndex = 28;
            this.lblHeight.Text = "Height";
            // 
            // TileAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 279);
            this.Controls.Add(this.txtHeight);
            this.Controls.Add(this.lblHeight);
            this.Controls.Add(this.lblBattleAnimationForeground);
            this.Controls.Add(this.cboBattleAnimationForeground);
            this.Controls.Add(this.lblBattleAnimationBackground);
            this.Controls.Add(this.cboBattleAnimationBackground);
            this.Controls.Add(this.btnDeleteBattleAnimationBackground);
            this.Controls.Add(this.btnNewBattleAnimationBackground);
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
            ((System.ComponentModel.ISupportInitialize)(this.txtHeight)).EndInit();
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
        private System.Windows.Forms.Label lblBattleAnimationBackground;
        private System.Windows.Forms.ComboBox cboBattleAnimationBackground;
        private System.Windows.Forms.Button btnDeleteBattleAnimationBackground;
        private System.Windows.Forms.Button btnNewBattleAnimationBackground;
        private System.Windows.Forms.Label lblBattleAnimationForeground;
        private System.Windows.Forms.ComboBox cboBattleAnimationForeground;
        private System.Windows.Forms.NumericUpDown txtHeight;
        private System.Windows.Forms.Label lblHeight;
    }
}