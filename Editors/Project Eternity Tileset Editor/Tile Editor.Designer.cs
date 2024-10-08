﻿
namespace ProjectEternity.Editors.TilesetEditor
{
    partial class TileEditor
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
            this.gbTileInformation = new System.Windows.Forms.GroupBox();
            this.lblBattleAnimationBackground = new System.Windows.Forms.Label();
            this.cboBattleAnimationBackground = new System.Windows.Forms.ComboBox();
            this.btnDeleteBattleAnimationBackground = new System.Windows.Forms.Button();
            this.btnNewBattleAnimationBackground = new System.Windows.Forms.Button();
            this.txtBonusValue = new System.Windows.Forms.NumericUpDown();
            this.lblTerrainType = new System.Windows.Forms.Label();
            this.cboTerrainType = new System.Windows.Forms.ComboBox();
            this.lblTerrainBonusActivation = new System.Windows.Forms.Label();
            this.cboTerrainBonusActivation = new System.Windows.Forms.ComboBox();
            this.btnClearBonuses = new System.Windows.Forms.Button();
            this.btnRemoveBonus = new System.Windows.Forms.Button();
            this.btnAddNewBonus = new System.Windows.Forms.Button();
            this.lstTerrainBonus = new System.Windows.Forms.ListBox();
            this.lblTerrainBonus = new System.Windows.Forms.Label();
            this.lblBonusValue = new System.Windows.Forms.Label();
            this.cboTerrainBonusType = new System.Windows.Forms.ComboBox();
            this.lblTerrainBonusType = new System.Windows.Forms.Label();
            this.gbTileInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).BeginInit();
            this.SuspendLayout();
            // 
            // gbTileInformation
            // 
            this.gbTileInformation.Controls.Add(this.lblBattleAnimationBackground);
            this.gbTileInformation.Controls.Add(this.cboBattleAnimationBackground);
            this.gbTileInformation.Controls.Add(this.btnDeleteBattleAnimationBackground);
            this.gbTileInformation.Controls.Add(this.btnNewBattleAnimationBackground);
            this.gbTileInformation.Controls.Add(this.txtBonusValue);
            this.gbTileInformation.Controls.Add(this.lblTerrainType);
            this.gbTileInformation.Controls.Add(this.cboTerrainType);
            this.gbTileInformation.Controls.Add(this.lblTerrainBonusActivation);
            this.gbTileInformation.Controls.Add(this.cboTerrainBonusActivation);
            this.gbTileInformation.Controls.Add(this.btnClearBonuses);
            this.gbTileInformation.Controls.Add(this.btnRemoveBonus);
            this.gbTileInformation.Controls.Add(this.btnAddNewBonus);
            this.gbTileInformation.Controls.Add(this.lstTerrainBonus);
            this.gbTileInformation.Controls.Add(this.lblTerrainBonus);
            this.gbTileInformation.Controls.Add(this.lblBonusValue);
            this.gbTileInformation.Controls.Add(this.cboTerrainBonusType);
            this.gbTileInformation.Controls.Add(this.lblTerrainBonusType);
            this.gbTileInformation.Location = new System.Drawing.Point(12, 12);
            this.gbTileInformation.Name = "gbTileInformation";
            this.gbTileInformation.Size = new System.Drawing.Size(397, 244);
            this.gbTileInformation.TabIndex = 19;
            this.gbTileInformation.TabStop = false;
            this.gbTileInformation.Text = "Tile Information";
            // 
            // lblBattleAnimationBackground
            // 
            this.lblBattleAnimationBackground.AutoSize = true;
            this.lblBattleAnimationBackground.Location = new System.Drawing.Point(6, 170);
            this.lblBattleAnimationBackground.Name = "lblBattleAnimationBackground";
            this.lblBattleAnimationBackground.Size = new System.Drawing.Size(142, 13);
            this.lblBattleAnimationBackground.TabIndex = 21;
            this.lblBattleAnimationBackground.Text = "Battle animation background";
            // 
            // cboBattleAnimationBackground
            // 
            this.cboBattleAnimationBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationBackground.FormattingEnabled = true;
            this.cboBattleAnimationBackground.Location = new System.Drawing.Point(6, 186);
            this.cboBattleAnimationBackground.Name = "cboBattleAnimationBackground";
            this.cboBattleAnimationBackground.Size = new System.Drawing.Size(163, 21);
            this.cboBattleAnimationBackground.TabIndex = 8;
            this.cboBattleAnimationBackground.SelectedIndexChanged += new System.EventHandler(this.cboBattleAnimationBackground_SelectedIndexChanged);
            // 
            // btnDeleteBattleAnimationBackground
            // 
            this.btnDeleteBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteBattleAnimationBackground.Location = new System.Drawing.Point(94, 214);
            this.btnDeleteBattleAnimationBackground.Name = "btnDeleteBattleAnimationBackground";
            this.btnDeleteBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnDeleteBattleAnimationBackground.TabIndex = 2;
            this.btnDeleteBattleAnimationBackground.Text = "Delete";
            this.btnDeleteBattleAnimationBackground.UseVisualStyleBackColor = true;
            this.btnDeleteBattleAnimationBackground.Click += new System.EventHandler(this.btnDeleteBattleAnimationBackground_Click);
            // 
            // btnNewBattleAnimationBackground
            // 
            this.btnNewBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewBattleAnimationBackground.Location = new System.Drawing.Point(6, 214);
            this.btnNewBattleAnimationBackground.Name = "btnNewBattleAnimationBackground";
            this.btnNewBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnNewBattleAnimationBackground.TabIndex = 1;
            this.btnNewBattleAnimationBackground.Text = "New";
            this.btnNewBattleAnimationBackground.UseVisualStyleBackColor = true;
            this.btnNewBattleAnimationBackground.Click += new System.EventHandler(this.btnNewBattleAnimationBackground_Click);
            // 
            // txtBonusValue
            // 
            this.txtBonusValue.Location = new System.Drawing.Point(266, 213);
            this.txtBonusValue.Name = "txtBonusValue";
            this.txtBonusValue.Size = new System.Drawing.Size(121, 20);
            this.txtBonusValue.TabIndex = 18;
            this.txtBonusValue.Click += new System.EventHandler(this.txtBonusValue_TextChanged);
            // 
            // lblTerrainType
            // 
            this.lblTerrainType.Location = new System.Drawing.Point(6, 16);
            this.lblTerrainType.Name = "lblTerrainType";
            this.lblTerrainType.Size = new System.Drawing.Size(71, 13);
            this.lblTerrainType.TabIndex = 0;
            this.lblTerrainType.Text = "Terrain type";
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
            this.cboTerrainType.Location = new System.Drawing.Point(6, 32);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainType_SelectedIndexChanged);
            // 
            // lblTerrainBonusActivation
            // 
            this.lblTerrainBonusActivation.Location = new System.Drawing.Point(266, 157);
            this.lblTerrainBonusActivation.Name = "lblTerrainBonusActivation";
            this.lblTerrainBonusActivation.Size = new System.Drawing.Size(121, 13);
            this.lblTerrainBonusActivation.TabIndex = 16;
            this.lblTerrainBonusActivation.Text = "Terrain bonus activation";
            // 
            // cboTerrainBonusActivation
            // 
            this.cboTerrainBonusActivation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusActivation.FormattingEnabled = true;
            this.cboTerrainBonusActivation.Location = new System.Drawing.Point(266, 173);
            this.cboTerrainBonusActivation.Name = "cboTerrainBonusActivation";
            this.cboTerrainBonusActivation.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusActivation.TabIndex = 15;
            this.cboTerrainBonusActivation.Click += new System.EventHandler(this.cboTerrainBonusActivation_SelectedIndexChanged);
            // 
            // btnClearBonuses
            // 
            this.btnClearBonuses.Location = new System.Drawing.Point(161, 87);
            this.btnClearBonuses.Name = "btnClearBonuses";
            this.btnClearBonuses.Size = new System.Drawing.Size(100, 23);
            this.btnClearBonuses.TabIndex = 14;
            this.btnClearBonuses.Text = "Clear bonuses";
            this.btnClearBonuses.UseVisualStyleBackColor = true;
            this.btnClearBonuses.Click += new System.EventHandler(this.btnClearBonuses_Click);
            // 
            // btnRemoveBonus
            // 
            this.btnRemoveBonus.Location = new System.Drawing.Point(161, 58);
            this.btnRemoveBonus.Name = "btnRemoveBonus";
            this.btnRemoveBonus.Size = new System.Drawing.Size(100, 23);
            this.btnRemoveBonus.TabIndex = 13;
            this.btnRemoveBonus.Text = "Remove bonus";
            this.btnRemoveBonus.UseVisualStyleBackColor = true;
            this.btnRemoveBonus.Click += new System.EventHandler(this.btnRemoveBonus_Click);
            // 
            // btnAddNewBonus
            // 
            this.btnAddNewBonus.Location = new System.Drawing.Point(161, 29);
            this.btnAddNewBonus.Name = "btnAddNewBonus";
            this.btnAddNewBonus.Size = new System.Drawing.Size(100, 23);
            this.btnAddNewBonus.TabIndex = 12;
            this.btnAddNewBonus.Text = "Add new bonus";
            this.btnAddNewBonus.UseVisualStyleBackColor = true;
            this.btnAddNewBonus.Click += new System.EventHandler(this.btnAddNewBonus_Click);
            // 
            // lstTerrainBonus
            // 
            this.lstTerrainBonus.FormattingEnabled = true;
            this.lstTerrainBonus.Location = new System.Drawing.Point(267, 32);
            this.lstTerrainBonus.Name = "lstTerrainBonus";
            this.lstTerrainBonus.Size = new System.Drawing.Size(124, 82);
            this.lstTerrainBonus.TabIndex = 5;
            this.lstTerrainBonus.Click += new System.EventHandler(this.lstTerrainBonus_SelectedIndexChanged);
            // 
            // lblTerrainBonus
            // 
            this.lblTerrainBonus.AutoSize = true;
            this.lblTerrainBonus.Location = new System.Drawing.Point(266, 16);
            this.lblTerrainBonus.Name = "lblTerrainBonus";
            this.lblTerrainBonus.Size = new System.Drawing.Size(72, 13);
            this.lblTerrainBonus.TabIndex = 6;
            this.lblTerrainBonus.Text = "Terrain bonus";
            // 
            // lblBonusValue
            // 
            this.lblBonusValue.AutoSize = true;
            this.lblBonusValue.Location = new System.Drawing.Point(266, 197);
            this.lblBonusValue.Name = "lblBonusValue";
            this.lblBonusValue.Size = new System.Drawing.Size(66, 13);
            this.lblBonusValue.TabIndex = 10;
            this.lblBonusValue.Text = "Bonus value";
            // 
            // cboTerrainBonusType
            // 
            this.cboTerrainBonusType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusType.FormattingEnabled = true;
            this.cboTerrainBonusType.Location = new System.Drawing.Point(266, 133);
            this.cboTerrainBonusType.Name = "cboTerrainBonusType";
            this.cboTerrainBonusType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusType.TabIndex = 7;
            this.cboTerrainBonusType.Click += new System.EventHandler(this.cboTerrainBonusType_SelectedIndexChanged);
            // 
            // lblTerrainBonusType
            // 
            this.lblTerrainBonusType.Location = new System.Drawing.Point(266, 117);
            this.lblTerrainBonusType.Name = "lblTerrainBonusType";
            this.lblTerrainBonusType.Size = new System.Drawing.Size(100, 13);
            this.lblTerrainBonusType.TabIndex = 8;
            this.lblTerrainBonusType.Text = "Terrain bonus type";
            // 
            // TileEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(417, 261);
            this.Controls.Add(this.gbTileInformation);
            this.Name = "TileEditor";
            this.Text = "Tile_Editor";
            this.gbTileInformation.ResumeLayout(false);
            this.gbTileInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTileInformation;
        private System.Windows.Forms.Label lblBattleAnimationBackground;
        private System.Windows.Forms.ComboBox cboBattleAnimationBackground;
        private System.Windows.Forms.Button btnDeleteBattleAnimationBackground;
        private System.Windows.Forms.Button btnNewBattleAnimationBackground;
        private System.Windows.Forms.NumericUpDown txtBonusValue;
        private System.Windows.Forms.Label lblTerrainType;
        private System.Windows.Forms.ComboBox cboTerrainType;
        private System.Windows.Forms.Label lblTerrainBonusActivation;
        private System.Windows.Forms.ComboBox cboTerrainBonusActivation;
        private System.Windows.Forms.Button btnClearBonuses;
        private System.Windows.Forms.Button btnRemoveBonus;
        private System.Windows.Forms.Button btnAddNewBonus;
        private System.Windows.Forms.ListBox lstTerrainBonus;
        private System.Windows.Forms.Label lblTerrainBonus;
        private System.Windows.Forms.Label lblBonusValue;
        private System.Windows.Forms.ComboBox cboTerrainBonusType;
        private System.Windows.Forms.Label lblTerrainBonusType;
    }
}