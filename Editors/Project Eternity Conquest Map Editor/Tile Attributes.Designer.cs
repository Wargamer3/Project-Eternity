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
            this.SuspendLayout();
            // 
            // cboTerrainType
            // 
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
            this.cboTerrainType.Location = new System.Drawing.Point(12, 47);
            this.cboTerrainType.Name = "cboTerrainType";
            this.cboTerrainType.Size = new System.Drawing.Size(121, 21);
            this.cboTerrainType.TabIndex = 0;
            this.cboTerrainType.Text = "Plains";
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
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.Location = new System.Drawing.Point(135, 191);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 11;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // TileAttributes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(222, 226);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cboTerrainType);
            this.Name = "TileAttributes";
            this.Text = "Tile Attributes";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cboTerrainType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAccept;
    }
}