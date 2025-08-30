﻿namespace ProjectEternity.Editors.UnitHubEditor
{
    partial class UnitBuilderEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.bgSpritePreview = new System.Windows.Forms.GroupBox();
            this.viewerBattleSprite = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.btnImportMenuSprite = new System.Windows.Forms.Button();
            this.viewerMapSprite = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.btnImportMapSprite = new System.Windows.Forms.Button();
            this.gbOption = new System.Windows.Forms.GroupBox();
            this.ckResupply = new System.Windows.Forms.CheckBox();
            this.lblHealth = new System.Windows.Forms.Label();
            this.txtHealth = new System.Windows.Forms.NumericUpDown();
            this.txtVision = new System.Windows.Forms.NumericUpDown();
            this.lblVision = new System.Windows.Forms.Label();
            this.ckCapture = new System.Windows.Forms.CheckBox();
            this.lblTerrainType = new System.Windows.Forms.Label();
            this.cboTerrainType = new System.Windows.Forms.ComboBox();
            this.lblCreditPerTurn = new System.Windows.Forms.Label();
            this.txtCreditPerTurn = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.bgSpritePreview.SuspendLayout();
            this.gbOption.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHealth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVision)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreditPerTurn)).BeginInit();
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
            this.groupBox1.Text = "Units to build";
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
            // bgSpritePreview
            // 
            this.bgSpritePreview.Controls.Add(this.viewerBattleSprite);
            this.bgSpritePreview.Controls.Add(this.btnImportMenuSprite);
            this.bgSpritePreview.Controls.Add(this.viewerMapSprite);
            this.bgSpritePreview.Controls.Add(this.btnImportMapSprite);
            this.bgSpritePreview.Location = new System.Drawing.Point(195, 27);
            this.bgSpritePreview.Name = "bgSpritePreview";
            this.bgSpritePreview.Size = new System.Drawing.Size(237, 132);
            this.bgSpritePreview.TabIndex = 20;
            this.bgSpritePreview.TabStop = false;
            this.bgSpritePreview.Text = "Sprite Preview";
            // 
            // viewerBattleSprite
            // 
            this.viewerBattleSprite.Location = new System.Drawing.Point(124, 48);
            this.viewerBattleSprite.Name = "viewerBattleSprite";
            this.viewerBattleSprite.Size = new System.Drawing.Size(107, 75);
            this.viewerBattleSprite.TabIndex = 3;
            this.viewerBattleSprite.Text = "texture2DViewerControl2";
            // 
            // btnImportMenuSprite
            // 
            this.btnImportMenuSprite.Location = new System.Drawing.Point(124, 19);
            this.btnImportMenuSprite.Name = "btnImportMenuSprite";
            this.btnImportMenuSprite.Size = new System.Drawing.Size(107, 23);
            this.btnImportMenuSprite.TabIndex = 2;
            this.btnImportMenuSprite.Text = "Import Menu Sprite";
            this.btnImportMenuSprite.UseVisualStyleBackColor = true;
            this.btnImportMenuSprite.Click += new System.EventHandler(this.btnImportMenuSprite_Click);
            // 
            // viewerMapSprite
            // 
            this.viewerMapSprite.Location = new System.Drawing.Point(7, 48);
            this.viewerMapSprite.Name = "viewerMapSprite";
            this.viewerMapSprite.Size = new System.Drawing.Size(111, 75);
            this.viewerMapSprite.TabIndex = 1;
            this.viewerMapSprite.Text = "texture2DViewerControl1";
            // 
            // btnImportMapSprite
            // 
            this.btnImportMapSprite.Location = new System.Drawing.Point(6, 19);
            this.btnImportMapSprite.Name = "btnImportMapSprite";
            this.btnImportMapSprite.Size = new System.Drawing.Size(112, 23);
            this.btnImportMapSprite.TabIndex = 0;
            this.btnImportMapSprite.Text = "Import Map Sprite";
            this.btnImportMapSprite.UseVisualStyleBackColor = true;
            this.btnImportMapSprite.Click += new System.EventHandler(this.btnImportMapSprite_Click);
            // 
            // gbOption
            // 
            this.gbOption.Controls.Add(this.lblCreditPerTurn);
            this.gbOption.Controls.Add(this.txtCreditPerTurn);
            this.gbOption.Controls.Add(this.ckResupply);
            this.gbOption.Controls.Add(this.lblHealth);
            this.gbOption.Controls.Add(this.txtHealth);
            this.gbOption.Controls.Add(this.txtVision);
            this.gbOption.Controls.Add(this.lblVision);
            this.gbOption.Controls.Add(this.ckCapture);
            this.gbOption.Controls.Add(this.lblTerrainType);
            this.gbOption.Controls.Add(this.cboTerrainType);
            this.gbOption.Location = new System.Drawing.Point(196, 166);
            this.gbOption.Name = "gbOption";
            this.gbOption.Size = new System.Drawing.Size(298, 211);
            this.gbOption.TabIndex = 21;
            this.gbOption.TabStop = false;
            this.gbOption.Text = "Option";
            // 
            // ckResupply
            // 
            this.ckResupply.AutoSize = true;
            this.ckResupply.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.ckResupply.Location = new System.Drawing.Point(24, 124);
            this.ckResupply.Name = "ckResupply";
            this.ckResupply.Size = new System.Drawing.Size(70, 17);
            this.ckResupply.TabIndex = 8;
            this.ckResupply.Text = "Resupply";
            this.ckResupply.UseVisualStyleBackColor = true;
            // 
            // lblHealth
            // 
            this.lblHealth.AutoSize = true;
            this.lblHealth.Location = new System.Drawing.Point(36, 77);
            this.lblHealth.Name = "lblHealth";
            this.lblHealth.Size = new System.Drawing.Size(38, 13);
            this.lblHealth.TabIndex = 6;
            this.lblHealth.Text = "Health";
            // 
            // txtHealth
            // 
            this.txtHealth.Location = new System.Drawing.Point(80, 72);
            this.txtHealth.Name = "txtHealth";
            this.txtHealth.Size = new System.Drawing.Size(77, 20);
            this.txtHealth.TabIndex = 5;
            this.txtHealth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // txtVision
            // 
            this.txtVision.Location = new System.Drawing.Point(80, 45);
            this.txtVision.Name = "txtVision";
            this.txtVision.Size = new System.Drawing.Size(77, 20);
            this.txtVision.TabIndex = 4;
            // 
            // lblVision
            // 
            this.lblVision.AutoSize = true;
            this.lblVision.Location = new System.Drawing.Point(39, 47);
            this.lblVision.Name = "lblVision";
            this.lblVision.Size = new System.Drawing.Size(35, 13);
            this.lblVision.TabIndex = 3;
            this.lblVision.Text = "Vision";
            // 
            // ckCapture
            // 
            this.ckCapture.AutoSize = true;
            this.ckCapture.Checked = true;
            this.ckCapture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckCapture.Location = new System.Drawing.Point(229, 19);
            this.ckCapture.Name = "ckCapture";
            this.ckCapture.Size = new System.Drawing.Size(63, 17);
            this.ckCapture.TabIndex = 2;
            this.ckCapture.Text = "Capture";
            this.ckCapture.UseVisualStyleBackColor = true;
            // 
            // lblTerrainType
            // 
            this.lblTerrainType.AutoSize = true;
            this.lblTerrainType.Location = new System.Drawing.Point(7, 20);
            this.lblTerrainType.Name = "lblTerrainType";
            this.lblTerrainType.Size = new System.Drawing.Size(67, 13);
            this.lblTerrainType.TabIndex = 1;
            this.lblTerrainType.Text = "Terrain Type";
            // 
            // cboTerrainType
            // 
            this.cboTerrainType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainType.FormattingEnabled = true;
            this.cboTerrainType.Items.AddRange(new object[] {
            "Empty"});
            this.cboTerrainType.Location = new System.Drawing.Point(80, 17);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(143, 21);
            this.cboTerrainType.TabIndex = 0;
            // 
            // lblCreditPerTurn
            // 
            this.lblCreditPerTurn.AutoSize = true;
            this.lblCreditPerTurn.Location = new System.Drawing.Point(3, 100);
            this.lblCreditPerTurn.Name = "lblCreditPerTurn";
            this.lblCreditPerTurn.Size = new System.Drawing.Size(74, 13);
            this.lblCreditPerTurn.TabIndex = 10;
            this.lblCreditPerTurn.Text = "Credit Per turn";
            // 
            // txtCreditPerTurn
            // 
            this.txtCreditPerTurn.Location = new System.Drawing.Point(80, 98);
            this.txtCreditPerTurn.Name = "txtCreditPerTurn";
            this.txtCreditPerTurn.Size = new System.Drawing.Size(77, 20);
            this.txtCreditPerTurn.TabIndex = 9;
            // 
            // UnitBuilderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 389);
            this.Controls.Add(this.gbOption);
            this.Controls.Add(this.bgSpritePreview);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "UnitBuilderEditor";
            this.Text = "Building Editor";
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.bgSpritePreview.ResumeLayout(false);
            this.gbOption.ResumeLayout(false);
            this.gbOption.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHealth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtVision)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreditPerTurn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox lstUnits;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox bgSpritePreview;
        private Core.Editor.Texture2DViewerControl viewerBattleSprite;
        private System.Windows.Forms.Button btnImportMenuSprite;
        private Core.Editor.Texture2DViewerControl viewerMapSprite;
        private System.Windows.Forms.Button btnImportMapSprite;
        private System.Windows.Forms.GroupBox gbOption;
        private System.Windows.Forms.Label lblTerrainType;
        private System.Windows.Forms.ComboBox cboTerrainType;
        private System.Windows.Forms.CheckBox ckResupply;
        private System.Windows.Forms.Label lblHealth;
        private System.Windows.Forms.NumericUpDown txtHealth;
        private System.Windows.Forms.NumericUpDown txtVision;
        private System.Windows.Forms.Label lblVision;
        private System.Windows.Forms.CheckBox ckCapture;
        private System.Windows.Forms.Label lblCreditPerTurn;
        private System.Windows.Forms.NumericUpDown txtCreditPerTurn;
    }
}