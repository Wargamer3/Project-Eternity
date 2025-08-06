namespace ProjectEternity.Editors.ConquestMapEditor
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnEditTerrainTypes = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblBattleAnimationForeground = new System.Windows.Forms.Label();
            this.cboBattleAnimationForeground = new System.Windows.Forms.ComboBox();
            this.lblBattleAnimationBackground = new System.Windows.Forms.Label();
            this.cboBattleAnimationBackground = new System.Windows.Forms.ComboBox();
            this.btnNewBattleAnimationBackground = new System.Windows.Forms.Button();
            this.btnDeleteBattleAnimationBackground = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboTerrainType
            // 
            this.cboTerrainType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTerrainType.FormattingEnabled = true;
            this.cboTerrainType.Items.AddRange(new object[] {
            "Plain",
            "Road",
            "Wood",
            "Mountain",
            "Wasteland",
            "Ruins",
            "Sea",
            "Bridge",
            "River",
            "Beach",
            "Rough Sea",
            "Mist ",
            "Reef",
            "HQ",
            "City",
            "Factory",
            "Airport",
            "Port",
            "Com Tower",
            "Radar",
            "Temp Airport",
            "Temp Port",
            "Missile Silo"});
            this.cboTerrainType.Location = new System.Drawing.Point(12, 25);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(142, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.SelectedIndexChanged += new System.EventHandler(this.cboTerrainType_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Terrain type";
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.Location = new System.Drawing.Point(154, 210);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 11;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnEditTerrainTypes
            // 
            this.btnEditTerrainTypes.Location = new System.Drawing.Point(160, 26);
            this.btnEditTerrainTypes.Name = "btnEditTerrainTypes";
            this.btnEditTerrainTypes.Size = new System.Drawing.Size(66, 24);
            this.btnEditTerrainTypes.TabIndex = 26;
            this.btnEditTerrainTypes.Text = "Edit";
            this.btnEditTerrainTypes.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblBattleAnimationForeground);
            this.groupBox1.Controls.Add(this.cboBattleAnimationForeground);
            this.groupBox1.Controls.Add(this.lblBattleAnimationBackground);
            this.groupBox1.Controls.Add(this.cboBattleAnimationBackground);
            this.groupBox1.Controls.Add(this.btnNewBattleAnimationBackground);
            this.groupBox1.Controls.Add(this.btnDeleteBattleAnimationBackground);
            this.groupBox1.Location = new System.Drawing.Point(12, 55);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(214, 141);
            this.groupBox1.TabIndex = 25;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Battle Background";
            // 
            // lblBattleAnimationForeground
            // 
            this.lblBattleAnimationForeground.AutoSize = true;
            this.lblBattleAnimationForeground.Location = new System.Drawing.Point(6, 60);
            this.lblBattleAnimationForeground.Name = "lblBattleAnimationForeground";
            this.lblBattleAnimationForeground.Size = new System.Drawing.Size(136, 13);
            this.lblBattleAnimationForeground.TabIndex = 33;
            this.lblBattleAnimationForeground.Text = "Battle animation foreground";
            // 
            // cboBattleAnimationForeground
            // 
            this.cboBattleAnimationForeground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationForeground.FormattingEnabled = true;
            this.cboBattleAnimationForeground.Location = new System.Drawing.Point(6, 76);
            this.cboBattleAnimationForeground.Name = "cboBattleAnimationForeground";
            this.cboBattleAnimationForeground.Size = new System.Drawing.Size(202, 21);
            this.cboBattleAnimationForeground.TabIndex = 32;
            // 
            // lblBattleAnimationBackground
            // 
            this.lblBattleAnimationBackground.AutoSize = true;
            this.lblBattleAnimationBackground.Location = new System.Drawing.Point(6, 20);
            this.lblBattleAnimationBackground.Name = "lblBattleAnimationBackground";
            this.lblBattleAnimationBackground.Size = new System.Drawing.Size(142, 13);
            this.lblBattleAnimationBackground.TabIndex = 31;
            this.lblBattleAnimationBackground.Text = "Battle animation background";
            // 
            // cboBattleAnimationBackground
            // 
            this.cboBattleAnimationBackground.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboBattleAnimationBackground.FormattingEnabled = true;
            this.cboBattleAnimationBackground.Location = new System.Drawing.Point(6, 36);
            this.cboBattleAnimationBackground.Name = "cboBattleAnimationBackground";
            this.cboBattleAnimationBackground.Size = new System.Drawing.Size(202, 21);
            this.cboBattleAnimationBackground.TabIndex = 30;
            // 
            // btnNewBattleAnimationBackground
            // 
            this.btnNewBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewBattleAnimationBackground.Location = new System.Drawing.Point(6, 111);
            this.btnNewBattleAnimationBackground.Name = "btnNewBattleAnimationBackground";
            this.btnNewBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnNewBattleAnimationBackground.TabIndex = 1;
            this.btnNewBattleAnimationBackground.Text = "New";
            this.btnNewBattleAnimationBackground.UseVisualStyleBackColor = true;
            // 
            // btnDeleteBattleAnimationBackground
            // 
            this.btnDeleteBattleAnimationBackground.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteBattleAnimationBackground.Location = new System.Drawing.Point(87, 111);
            this.btnDeleteBattleAnimationBackground.Name = "btnDeleteBattleAnimationBackground";
            this.btnDeleteBattleAnimationBackground.Size = new System.Drawing.Size(75, 24);
            this.btnDeleteBattleAnimationBackground.TabIndex = 2;
            this.btnDeleteBattleAnimationBackground.Text = "Delete";
            this.btnDeleteBattleAnimationBackground.UseVisualStyleBackColor = true;
            // 
            // TileAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 245);
            this.Controls.Add(this.btnEditTerrainTypes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboTerrainType);
            this.Name = "TileAttributes";
            this.Text = "Tile Attributes";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTerrainType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnEditTerrainTypes;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblBattleAnimationForeground;
        private System.Windows.Forms.ComboBox cboBattleAnimationForeground;
        private System.Windows.Forms.Label lblBattleAnimationBackground;
        private System.Windows.Forms.ComboBox cboBattleAnimationBackground;
        private System.Windows.Forms.Button btnNewBattleAnimationBackground;
        private System.Windows.Forms.Button btnDeleteBattleAnimationBackground;
    }
}