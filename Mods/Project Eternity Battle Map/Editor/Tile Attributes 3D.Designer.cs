
namespace ProjectEternity.GameScreens.BattleMapScreen
{
    partial class TileAttributesEditor3D
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
            this.TilesetViewerFront = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.cboTilesFront = new System.Windows.Forms.ComboBox();
            this.gbPreview = new System.Windows.Forms.GroupBox();
            this.TileViewer3D = new ProjectEternity.GameScreens.BattleMapScreen.TileViewer3DControl();
            this.gbTopTile = new System.Windows.Forms.GroupBox();
            this.tc3DSides = new System.Windows.Forms.TabControl();
            this.tabFrontTile = new System.Windows.Forms.TabPage();
            this.panTilesFront = new System.Windows.Forms.Panel();
            this.tabBackTile = new System.Windows.Forms.TabPage();
            this.panTilesBack = new System.Windows.Forms.Panel();
            this.TilesetViewerBack = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.cboTilesBack = new System.Windows.Forms.ComboBox();
            this.tabLeftTile = new System.Windows.Forms.TabPage();
            this.panTilesLeft = new System.Windows.Forms.Panel();
            this.TilesetViewerLeft = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.cboTilesLeft = new System.Windows.Forms.ComboBox();
            this.tabRightTile = new System.Windows.Forms.TabPage();
            this.panTilesRight = new System.Windows.Forms.Panel();
            this.TilesetViewerRight = new ProjectEternity.GameScreens.BattleMapScreen.TilesetViewerControl();
            this.cboTilesRight = new System.Windows.Forms.ComboBox();
            this.gb3DAttributes = new System.Windows.Forms.GroupBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.lblTransparancy = new System.Windows.Forms.Label();
            this.lbl3DStyle = new System.Windows.Forms.Label();
            this.cbo3DStyle = new System.Windows.Forms.ComboBox();
            this.gbPreview.SuspendLayout();
            this.tc3DSides.SuspendLayout();
            this.tabFrontTile.SuspendLayout();
            this.panTilesFront.SuspendLayout();
            this.tabBackTile.SuspendLayout();
            this.panTilesBack.SuspendLayout();
            this.tabLeftTile.SuspendLayout();
            this.panTilesLeft.SuspendLayout();
            this.tabRightTile.SuspendLayout();
            this.panTilesRight.SuspendLayout();
            this.gb3DAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // TilesetViewerFront
            // 
            this.TilesetViewerFront.Location = new System.Drawing.Point(3, 3);
            this.TilesetViewerFront.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.TilesetViewerFront.Name = "TilesetViewerFront";
            this.TilesetViewerFront.Size = new System.Drawing.Size(175, 276);
            this.TilesetViewerFront.TabIndex = 9;
            this.TilesetViewerFront.Click += new System.EventHandler(this.TileViewerFront_Click);
            // 
            // cboTilesFront
            // 
            this.cboTilesFront.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTilesFront.FormattingEnabled = true;
            this.cboTilesFront.Location = new System.Drawing.Point(5, 6);
            this.cboTilesFront.Name = "cboTilesFront";
            this.cboTilesFront.Size = new System.Drawing.Size(227, 21);
            this.cboTilesFront.TabIndex = 8;
            this.cboTilesFront.SelectedIndexChanged += new System.EventHandler(this.cboTilesFront_SelectedIndexChanged);
            // 
            // gbPreview
            // 
            this.gbPreview.Controls.Add(this.TileViewer3D);
            this.gbPreview.Location = new System.Drawing.Point(12, 106);
            this.gbPreview.Name = "gbPreview";
            this.gbPreview.Size = new System.Drawing.Size(200, 163);
            this.gbPreview.TabIndex = 11;
            this.gbPreview.TabStop = false;
            this.gbPreview.Text = "Preview";
            // 
            // TileViewer3D
            // 
            this.TileViewer3D.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TileViewer3D.Location = new System.Drawing.Point(6, 19);
            this.TileViewer3D.Name = "TileViewer3D";
            this.TileViewer3D.Size = new System.Drawing.Size(188, 138);
            this.TileViewer3D.TabIndex = 15;
            this.TileViewer3D.Text = "tileViewer3DControl1";
            // 
            // gbTopTile
            // 
            this.gbTopTile.Location = new System.Drawing.Point(12, 12);
            this.gbTopTile.Name = "gbTopTile";
            this.gbTopTile.Size = new System.Drawing.Size(113, 88);
            this.gbTopTile.TabIndex = 12;
            this.gbTopTile.TabStop = false;
            this.gbTopTile.Text = "Top Tile";
            // 
            // tc3DSides
            // 
            this.tc3DSides.Controls.Add(this.tabFrontTile);
            this.tc3DSides.Controls.Add(this.tabBackTile);
            this.tc3DSides.Controls.Add(this.tabLeftTile);
            this.tc3DSides.Controls.Add(this.tabRightTile);
            this.tc3DSides.Location = new System.Drawing.Point(338, 12);
            this.tc3DSides.Name = "tc3DSides";
            this.tc3DSides.SelectedIndex = 0;
            this.tc3DSides.Size = new System.Drawing.Size(244, 376);
            this.tc3DSides.TabIndex = 13;
            // 
            // tabFrontTile
            // 
            this.tabFrontTile.Controls.Add(this.panTilesFront);
            this.tabFrontTile.Controls.Add(this.cboTilesFront);
            this.tabFrontTile.Location = new System.Drawing.Point(4, 22);
            this.tabFrontTile.Name = "tabFrontTile";
            this.tabFrontTile.Padding = new System.Windows.Forms.Padding(3);
            this.tabFrontTile.Size = new System.Drawing.Size(236, 350);
            this.tabFrontTile.TabIndex = 0;
            this.tabFrontTile.Text = "Front Tile";
            this.tabFrontTile.UseVisualStyleBackColor = true;
            // 
            // panTilesFront
            // 
            this.panTilesFront.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panTilesFront.AutoScroll = true;
            this.panTilesFront.Controls.Add(this.TilesetViewerFront);
            this.panTilesFront.Location = new System.Drawing.Point(6, 33);
            this.panTilesFront.Name = "panTilesFront";
            this.panTilesFront.Size = new System.Drawing.Size(223, 311);
            this.panTilesFront.TabIndex = 15;
            // 
            // tabBackTile
            // 
            this.tabBackTile.Controls.Add(this.panTilesBack);
            this.tabBackTile.Controls.Add(this.cboTilesBack);
            this.tabBackTile.Location = new System.Drawing.Point(4, 22);
            this.tabBackTile.Name = "tabBackTile";
            this.tabBackTile.Padding = new System.Windows.Forms.Padding(3);
            this.tabBackTile.Size = new System.Drawing.Size(236, 350);
            this.tabBackTile.TabIndex = 1;
            this.tabBackTile.Text = "Back Tile";
            this.tabBackTile.UseVisualStyleBackColor = true;
            // 
            // panTilesBack
            // 
            this.panTilesBack.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panTilesBack.AutoScroll = true;
            this.panTilesBack.Controls.Add(this.TilesetViewerBack);
            this.panTilesBack.Location = new System.Drawing.Point(6, 33);
            this.panTilesBack.Name = "panTilesBack";
            this.panTilesBack.Size = new System.Drawing.Size(223, 311);
            this.panTilesBack.TabIndex = 15;
            // 
            // TilesetViewerBack
            // 
            this.TilesetViewerBack.Location = new System.Drawing.Point(3, 3);
            this.TilesetViewerBack.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.TilesetViewerBack.Name = "TilesetViewerBack";
            this.TilesetViewerBack.Size = new System.Drawing.Size(175, 276);
            this.TilesetViewerBack.TabIndex = 11;
            this.TilesetViewerBack.Click += new System.EventHandler(this.TilesetViewerBack_Click);
            // 
            // cboTilesBack
            // 
            this.cboTilesBack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTilesBack.FormattingEnabled = true;
            this.cboTilesBack.Location = new System.Drawing.Point(5, 6);
            this.cboTilesBack.Name = "cboTilesBack";
            this.cboTilesBack.Size = new System.Drawing.Size(227, 21);
            this.cboTilesBack.TabIndex = 10;
            this.cboTilesBack.SelectedIndexChanged += new System.EventHandler(this.cboTilesBack_SelectedIndexChanged);
            // 
            // tabLeftTile
            // 
            this.tabLeftTile.Controls.Add(this.panTilesLeft);
            this.tabLeftTile.Controls.Add(this.cboTilesLeft);
            this.tabLeftTile.Location = new System.Drawing.Point(4, 22);
            this.tabLeftTile.Name = "tabLeftTile";
            this.tabLeftTile.Size = new System.Drawing.Size(236, 350);
            this.tabLeftTile.TabIndex = 2;
            this.tabLeftTile.Text = "Left Tile";
            this.tabLeftTile.UseVisualStyleBackColor = true;
            // 
            // panTilesLeft
            // 
            this.panTilesLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panTilesLeft.AutoScroll = true;
            this.panTilesLeft.Controls.Add(this.TilesetViewerLeft);
            this.panTilesLeft.Location = new System.Drawing.Point(6, 33);
            this.panTilesLeft.Name = "panTilesLeft";
            this.panTilesLeft.Size = new System.Drawing.Size(223, 311);
            this.panTilesLeft.TabIndex = 15;
            // 
            // TilesetViewerLeft
            // 
            this.TilesetViewerLeft.Location = new System.Drawing.Point(3, 3);
            this.TilesetViewerLeft.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.TilesetViewerLeft.Name = "TilesetViewerLeft";
            this.TilesetViewerLeft.Size = new System.Drawing.Size(175, 276);
            this.TilesetViewerLeft.TabIndex = 11;
            this.TilesetViewerLeft.Click += new System.EventHandler(this.TilesetViewerLeft_Click);
            // 
            // cboTilesLeft
            // 
            this.cboTilesLeft.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTilesLeft.FormattingEnabled = true;
            this.cboTilesLeft.Location = new System.Drawing.Point(5, 6);
            this.cboTilesLeft.Name = "cboTilesLeft";
            this.cboTilesLeft.Size = new System.Drawing.Size(227, 21);
            this.cboTilesLeft.TabIndex = 10;
            this.cboTilesLeft.SelectedIndexChanged += new System.EventHandler(this.cboTilesLeft_SelectedIndexChanged);
            // 
            // tabRightTile
            // 
            this.tabRightTile.Controls.Add(this.panTilesRight);
            this.tabRightTile.Controls.Add(this.cboTilesRight);
            this.tabRightTile.Location = new System.Drawing.Point(4, 22);
            this.tabRightTile.Name = "tabRightTile";
            this.tabRightTile.Size = new System.Drawing.Size(236, 350);
            this.tabRightTile.TabIndex = 3;
            this.tabRightTile.Text = "Right Tile";
            this.tabRightTile.UseVisualStyleBackColor = true;
            // 
            // panTilesRight
            // 
            this.panTilesRight.AutoScroll = true;
            this.panTilesRight.Controls.Add(this.TilesetViewerRight);
            this.panTilesRight.Location = new System.Drawing.Point(6, 33);
            this.panTilesRight.Name = "panTilesRight";
            this.panTilesRight.Size = new System.Drawing.Size(223, 311);
            this.panTilesRight.TabIndex = 15;
            // 
            // TilesetViewerRight
            // 
            this.TilesetViewerRight.Location = new System.Drawing.Point(3, 3);
            this.TilesetViewerRight.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.TilesetViewerRight.Name = "TilesetViewerRight";
            this.TilesetViewerRight.Size = new System.Drawing.Size(175, 276);
            this.TilesetViewerRight.TabIndex = 11;
            this.TilesetViewerRight.Click += new System.EventHandler(this.TilesetViewerRight_Click);
            // 
            // cboTilesRight
            // 
            this.cboTilesRight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTilesRight.FormattingEnabled = true;
            this.cboTilesRight.Location = new System.Drawing.Point(5, 6);
            this.cboTilesRight.Name = "cboTilesRight";
            this.cboTilesRight.Size = new System.Drawing.Size(227, 21);
            this.cboTilesRight.TabIndex = 10;
            this.cboTilesRight.SelectedIndexChanged += new System.EventHandler(this.cboTilesRight_SelectedIndexChanged);
            // 
            // gb3DAttributes
            // 
            this.gb3DAttributes.Controls.Add(this.numericUpDown1);
            this.gb3DAttributes.Controls.Add(this.lblTransparancy);
            this.gb3DAttributes.Controls.Add(this.lbl3DStyle);
            this.gb3DAttributes.Controls.Add(this.cbo3DStyle);
            this.gb3DAttributes.Location = new System.Drawing.Point(131, 12);
            this.gb3DAttributes.Name = "gb3DAttributes";
            this.gb3DAttributes.Size = new System.Drawing.Size(201, 88);
            this.gb3DAttributes.TabIndex = 14;
            this.gb3DAttributes.TabStop = false;
            this.gb3DAttributes.Text = "3D Attributes";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(134, 53);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(61, 20);
            this.numericUpDown1.TabIndex = 17;
            // 
            // lblTransparancy
            // 
            this.lblTransparancy.AutoSize = true;
            this.lblTransparancy.Location = new System.Drawing.Point(6, 55);
            this.lblTransparancy.Name = "lblTransparancy";
            this.lblTransparancy.Size = new System.Drawing.Size(75, 13);
            this.lblTransparancy.TabIndex = 16;
            this.lblTransparancy.Text = "Transparancy:";
            // 
            // lbl3DStyle
            // 
            this.lbl3DStyle.AutoSize = true;
            this.lbl3DStyle.Location = new System.Drawing.Point(6, 22);
            this.lbl3DStyle.Name = "lbl3DStyle";
            this.lbl3DStyle.Size = new System.Drawing.Size(50, 13);
            this.lbl3DStyle.TabIndex = 15;
            this.lbl3DStyle.Text = "3D Style:";
            // 
            // cbo3DStyle
            // 
            this.cbo3DStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbo3DStyle.FormattingEnabled = true;
            this.cbo3DStyle.Items.AddRange(new object[] {
            "Flat",
            "Invisible",
            "Cube",
            "Flat Wall Lateral",
            "Flat Wall Medial",
            "Flat Wall Front",
            "Flat Wall Back",
            "Flat Wall Left",
            "Flat Wall Right",
            "Slope Left To Right",
            "Slope Right To Left",
            "Slope Front To Back",
            "Slope Back To Front,",
            "Wedge Left To Right",
            "Wedge Right To Left",
            "Wedge Front To Back",
            "Wedge Back To Front,",
            "FrontLeftCorner",
            "FrontRightCorner",
            "BackLeftCorner",
            "BackRightCorner",
            "Pyramid",
            "Pipe Left To Right",
            "Pipe Front To Back",
            "Pipe Up To Down"});
            this.cbo3DStyle.Location = new System.Drawing.Point(102, 19);
            this.cbo3DStyle.Name = "cbo3DStyle";
            this.cbo3DStyle.Size = new System.Drawing.Size(93, 21);
            this.cbo3DStyle.TabIndex = 9;
            this.cbo3DStyle.SelectedIndexChanged += new System.EventHandler(this.cbo3DStyle_SelectedIndexChanged);
            // 
            // TileAttributesEditor3D
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 392);
            this.Controls.Add(this.gb3DAttributes);
            this.Controls.Add(this.tc3DSides);
            this.Controls.Add(this.gbTopTile);
            this.Controls.Add(this.gbPreview);
            this.Name = "TileAttributesEditor3D";
            this.Text = "3D_Tile_Attributes";
            this.gbPreview.ResumeLayout(false);
            this.tc3DSides.ResumeLayout(false);
            this.tabFrontTile.ResumeLayout(false);
            this.panTilesFront.ResumeLayout(false);
            this.tabBackTile.ResumeLayout(false);
            this.panTilesBack.ResumeLayout(false);
            this.tabLeftTile.ResumeLayout(false);
            this.panTilesLeft.ResumeLayout(false);
            this.tabRightTile.ResumeLayout(false);
            this.panTilesRight.ResumeLayout(false);
            this.gb3DAttributes.ResumeLayout(false);
            this.gb3DAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected TilesetViewerControl TilesetViewerFront;
        protected System.Windows.Forms.ComboBox cboTilesFront;
        private System.Windows.Forms.GroupBox gbPreview;
        private System.Windows.Forms.GroupBox gbTopTile;
        private System.Windows.Forms.TabControl tc3DSides;
        private System.Windows.Forms.TabPage tabFrontTile;
        private System.Windows.Forms.TabPage tabBackTile;
        private System.Windows.Forms.TabPage tabLeftTile;
        private System.Windows.Forms.TabPage tabRightTile;
        private System.Windows.Forms.GroupBox gb3DAttributes;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label lblTransparancy;
        private System.Windows.Forms.Label lbl3DStyle;
        protected System.Windows.Forms.ComboBox cbo3DStyle;
        protected TilesetViewerControl TilesetViewerBack;
        protected System.Windows.Forms.ComboBox cboTilesBack;
        protected TilesetViewerControl TilesetViewerLeft;
        protected System.Windows.Forms.ComboBox cboTilesLeft;
        protected TilesetViewerControl TilesetViewerRight;
        protected System.Windows.Forms.ComboBox cboTilesRight;
        private System.Windows.Forms.Panel panTilesFront;
        private TileViewer3DControl TileViewer3D;
        private System.Windows.Forms.Panel panTilesBack;
        private System.Windows.Forms.Panel panTilesLeft;
        private System.Windows.Forms.Panel panTilesRight;
    }
}