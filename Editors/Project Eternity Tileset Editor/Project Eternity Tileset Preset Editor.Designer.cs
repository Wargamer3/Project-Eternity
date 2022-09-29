using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.MapEditor
{
    partial class ProjectEternityTilesetPresetEditor
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
            this.lblTerrainType = new System.Windows.Forms.Label();
            this.lstTerrainBonus = new System.Windows.Forms.ListBox();
            this.lblTerrainBonus = new System.Windows.Forms.Label();
            this.cboTerrainBonusType = new System.Windows.Forms.ComboBox();
            this.lblTerrainBonusType = new System.Windows.Forms.Label();
            this.lblBonusValue = new System.Windows.Forms.Label();
            this.btnAddNewBonus = new System.Windows.Forms.Button();
            this.btnRemoveBonus = new System.Windows.Forms.Button();
            this.btnClearBonuses = new System.Windows.Forms.Button();
            this.lblTerrainBonusActivation = new System.Windows.Forms.Label();
            this.cboTerrainBonusActivation = new System.Windows.Forms.ComboBox();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbTileInformation = new System.Windows.Forms.GroupBox();
            this.lblBattleAnimationBackground = new System.Windows.Forms.Label();
            this.cboBattleAnimationBackground = new System.Windows.Forms.ComboBox();
            this.btnDeleteBattleAnimationBackground = new System.Windows.Forms.Button();
            this.btnNewBattleAnimationBackground = new System.Windows.Forms.Button();
            this.txtBonusValue = new System.Windows.Forms.NumericUpDown();
            this.gbTileset = new System.Windows.Forms.GroupBox();
            this.sclTileWidth = new System.Windows.Forms.HScrollBar();
            this.panTilesetPreview = new System.Windows.Forms.Panel();
            this.sclTileHeight = new System.Windows.Forms.VScrollBar();
            this.txtTilesetName = new System.Windows.Forms.TextBox();
            this.btnAddTile = new System.Windows.Forms.Button();
            this.lblActiveTileset = new System.Windows.Forms.Label();
            this.mnuToolBar.SuspendLayout();
            this.gbTileInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).BeginInit();
            this.gbTileset.SuspendLayout();
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
            this.cboTerrainType.Location = new System.Drawing.Point(6, 32);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainType_SelectedIndexChanged);
            // 
            // lblTerrainType
            // 
            this.lblTerrainType.Location = new System.Drawing.Point(6, 16);
            this.lblTerrainType.Name = "lblTerrainType";
            this.lblTerrainType.Size = new System.Drawing.Size(71, 13);
            this.lblTerrainType.TabIndex = 0;
            this.lblTerrainType.Text = "Terrain type";
            // 
            // lstTerrainBonus
            // 
            this.lstTerrainBonus.FormattingEnabled = true;
            this.lstTerrainBonus.Location = new System.Drawing.Point(267, 32);
            this.lstTerrainBonus.Name = "lstTerrainBonus";
            this.lstTerrainBonus.Size = new System.Drawing.Size(124, 82);
            this.lstTerrainBonus.TabIndex = 5;
            this.lstTerrainBonus.SelectedIndexChanged += new System.EventHandler(this.lstTerrainBonus_SelectedIndexChanged);
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
            // cboTerrainBonusType
            // 
            this.cboTerrainBonusType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainBonusType.FormattingEnabled = true;
            this.cboTerrainBonusType.Location = new System.Drawing.Point(266, 133);
            this.cboTerrainBonusType.Name = "cboTerrainBonusType";
            this.cboTerrainBonusType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainBonusType.TabIndex = 7;
            this.cboTerrainBonusType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainBonusType_SelectedIndexChanged);
            // 
            // lblTerrainBonusType
            // 
            this.lblTerrainBonusType.Location = new System.Drawing.Point(266, 117);
            this.lblTerrainBonusType.Name = "lblTerrainBonusType";
            this.lblTerrainBonusType.Size = new System.Drawing.Size(100, 13);
            this.lblTerrainBonusType.TabIndex = 8;
            this.lblTerrainBonusType.Text = "Terrain bonus type";
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
            this.cboTerrainBonusActivation.SelectedIndexChanged += new System.EventHandler(this.cboTerrainBonusActivation_SelectedIndexChanged);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(665, 24);
            this.mnuToolBar.TabIndex = 17;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
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
            this.gbTileInformation.Location = new System.Drawing.Point(257, 27);
            this.gbTileInformation.Name = "gbTileInformation";
            this.gbTileInformation.Size = new System.Drawing.Size(397, 244);
            this.gbTileInformation.TabIndex = 18;
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
            this.txtBonusValue.ValueChanged += new System.EventHandler(this.txtBonusValue_TextChanged);
            // 
            // gbTileset
            // 
            this.gbTileset.Controls.Add(this.sclTileWidth);
            this.gbTileset.Controls.Add(this.panTilesetPreview);
            this.gbTileset.Controls.Add(this.sclTileHeight);
            this.gbTileset.Controls.Add(this.txtTilesetName);
            this.gbTileset.Controls.Add(this.btnAddTile);
            this.gbTileset.Controls.Add(this.lblActiveTileset);
            this.gbTileset.Location = new System.Drawing.Point(12, 27);
            this.gbTileset.Name = "gbTileset";
            this.gbTileset.Size = new System.Drawing.Size(239, 398);
            this.gbTileset.TabIndex = 19;
            this.gbTileset.TabStop = false;
            this.gbTileset.Text = "Tilesets";
            // 
            // sclTileWidth
            // 
            this.sclTileWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sclTileWidth.Location = new System.Drawing.Point(0, 381);
            this.sclTileWidth.Name = "sclTileWidth";
            this.sclTileWidth.Size = new System.Drawing.Size(222, 17);
            this.sclTileWidth.TabIndex = 8;
            this.sclTileWidth.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileWidth_Scroll);
            // 
            // panTilesetPreview
            // 
            this.panTilesetPreview.Location = new System.Drawing.Point(6, 91);
            this.panTilesetPreview.Name = "panTilesetPreview";
            this.panTilesetPreview.Size = new System.Drawing.Size(216, 290);
            this.panTilesetPreview.TabIndex = 20;
            this.panTilesetPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbTilePreview_MouseClick);
            // 
            // sclTileHeight
            // 
            this.sclTileHeight.Location = new System.Drawing.Point(222, 91);
            this.sclTileHeight.Name = "sclTileHeight";
            this.sclTileHeight.Size = new System.Drawing.Size(17, 290);
            this.sclTileHeight.TabIndex = 7;
            this.sclTileHeight.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileHeight_Scroll);
            // 
            // txtTilesetName
            // 
            this.txtTilesetName.Location = new System.Drawing.Point(6, 32);
            this.txtTilesetName.Name = "txtTilesetName";
            this.txtTilesetName.ReadOnly = true;
            this.txtTilesetName.Size = new System.Drawing.Size(227, 20);
            this.txtTilesetName.TabIndex = 20;
            // 
            // btnAddTile
            // 
            this.btnAddTile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTile.Location = new System.Drawing.Point(6, 62);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(227, 23);
            this.btnAddTile.TabIndex = 10;
            this.btnAddTile.Text = "Select tileset";
            this.btnAddTile.UseVisualStyleBackColor = true;
            this.btnAddTile.Click += new System.EventHandler(this.btnAddTile_Click);
            // 
            // lblActiveTileset
            // 
            this.lblActiveTileset.AutoSize = true;
            this.lblActiveTileset.Location = new System.Drawing.Point(6, 16);
            this.lblActiveTileset.Name = "lblActiveTileset";
            this.lblActiveTileset.Size = new System.Drawing.Size(67, 13);
            this.lblActiveTileset.TabIndex = 8;
            this.lblActiveTileset.Text = "Active tileset";
            // 
            // ProjectEternityTilesetPresetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 433);
            this.Controls.Add(this.gbTileset);
            this.Controls.Add(this.gbTileInformation);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "ProjectEternityTilesetPresetEditor";
            this.Text = "Tile Attributes";
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.gbTileInformation.ResumeLayout(false);
            this.gbTileInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBonusValue)).EndInit();
            this.gbTileset.ResumeLayout(false);
            this.gbTileset.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTerrainType;
        private System.Windows.Forms.Label lblTerrainType;
        private System.Windows.Forms.ListBox lstTerrainBonus;
        private System.Windows.Forms.Label lblTerrainBonus;
        private System.Windows.Forms.ComboBox cboTerrainBonusType;
        private System.Windows.Forms.Label lblTerrainBonusType;
        private System.Windows.Forms.Label lblBonusValue;
        private System.Windows.Forms.Button btnAddNewBonus;
        private System.Windows.Forms.Button btnRemoveBonus;
        private System.Windows.Forms.Button btnClearBonuses;
        private System.Windows.Forms.Label lblTerrainBonusActivation;
        private System.Windows.Forms.ComboBox cboTerrainBonusActivation;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbTileInformation;
        private System.Windows.Forms.GroupBox gbTileset;
        private System.Windows.Forms.Button btnAddTile;
        private System.Windows.Forms.Label lblActiveTileset;
        private Panel panTilesetPreview;
        private TextBox txtTilesetName;
        private NumericUpDown txtBonusValue;
        private HScrollBar sclTileWidth;
        private VScrollBar sclTileHeight;
        private Button btnDeleteBattleAnimationBackground;
        private Button btnNewBattleAnimationBackground;
        private Label lblBattleAnimationBackground;
        private ComboBox cboBattleAnimationBackground;
    }
}