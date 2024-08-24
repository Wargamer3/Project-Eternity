using System;
using System.Windows.Forms;

namespace ProjectEternity.Editors.TilesetEditor
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
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmTileEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.gbTileset = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbTilesetType = new System.Windows.Forms.ComboBox();
            this.sclTileWidth = new System.Windows.Forms.HScrollBar();
            this.panTilesetPreview = new System.Windows.Forms.Panel();
            this.sclTileHeight = new System.Windows.Forms.VScrollBar();
            this.txtTilesetName = new System.Windows.Forms.TextBox();
            this.btnAddTile = new System.Windows.Forms.Button();
            this.lblActiveTileset = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.mnuToolBar.SuspendLayout();
            this.gbTileset.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmTileEditor});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(448, 24);
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
            // tsmTileEditor
            // 
            this.tsmTileEditor.Name = "tsmTileEditor";
            this.tsmTileEditor.Size = new System.Drawing.Size(71, 20);
            this.tsmTileEditor.Text = "Tile Editor";
            this.tsmTileEditor.Click += new System.EventHandler(this.tsmTileEditor_Click);
            // 
            // gbTileset
            // 
            this.gbTileset.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTileset.Controls.Add(this.panel1);
            this.gbTileset.Controls.Add(this.label1);
            this.gbTileset.Controls.Add(this.cbTilesetType);
            this.gbTileset.Controls.Add(this.sclTileWidth);
            this.gbTileset.Controls.Add(this.panTilesetPreview);
            this.gbTileset.Controls.Add(this.sclTileHeight);
            this.gbTileset.Controls.Add(this.txtTilesetName);
            this.gbTileset.Controls.Add(this.btnAddTile);
            this.gbTileset.Controls.Add(this.lblActiveTileset);
            this.gbTileset.Location = new System.Drawing.Point(12, 27);
            this.gbTileset.Name = "gbTileset";
            this.gbTileset.Size = new System.Drawing.Size(424, 493);
            this.gbTileset.TabIndex = 19;
            this.gbTileset.TabStop = false;
            this.gbTileset.Text = "Tilesets";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 22;
            this.label1.Text = "Tileset type";
            // 
            // cbTilesetType
            // 
            this.cbTilesetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTilesetType.FormattingEnabled = true;
            this.cbTilesetType.Items.AddRange(new object[] {
            "Normal",
            "Road",
            "Water"});
            this.cbTilesetType.Location = new System.Drawing.Point(6, 71);
            this.cbTilesetType.Name = "cbTilesetType";
            this.cbTilesetType.Size = new System.Drawing.Size(227, 21);
            this.cbTilesetType.TabIndex = 23;
            // 
            // sclTileWidth
            // 
            this.sclTileWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sclTileWidth.Location = new System.Drawing.Point(0, 476);
            this.sclTileWidth.Name = "sclTileWidth";
            this.sclTileWidth.Size = new System.Drawing.Size(401, 17);
            this.sclTileWidth.TabIndex = 8;
            this.sclTileWidth.Scroll += new System.Windows.Forms.ScrollEventHandler(this.sclTileWidth_Scroll);
            // 
            // panTilesetPreview
            // 
            this.panTilesetPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panTilesetPreview.Location = new System.Drawing.Point(0, 127);
            this.panTilesetPreview.Name = "panTilesetPreview";
            this.panTilesetPreview.Size = new System.Drawing.Size(401, 346);
            this.panTilesetPreview.TabIndex = 20;
            this.panTilesetPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbTilePreview_MouseClick);
            // 
            // sclTileHeight
            // 
            this.sclTileHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sclTileHeight.Location = new System.Drawing.Point(404, 127);
            this.sclTileHeight.Name = "sclTileHeight";
            this.sclTileHeight.Size = new System.Drawing.Size(17, 346);
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
            this.btnAddTile.Location = new System.Drawing.Point(0, 98);
            this.btnAddTile.Name = "btnAddTile";
            this.btnAddTile.Size = new System.Drawing.Size(412, 23);
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
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(404, 476);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(20, 16);
            this.panel1.TabIndex = 24;
            // 
            // ProjectEternityTilesetPresetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 531);
            this.Controls.Add(this.gbTileset);
            this.Controls.Add(this.mnuToolBar);
            this.Name = "ProjectEternityTilesetPresetEditor";
            this.Text = "Tile Attributes";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ProjectEternityTilesetPresetEditor_FormClosed);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.gbTileset.ResumeLayout(false);
            this.gbTileset.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbTileset;
        private System.Windows.Forms.Button btnAddTile;
        private System.Windows.Forms.Label lblActiveTileset;
        private Panel panTilesetPreview;
        private TextBox txtTilesetName;
        private ToolStripMenuItem tsmTileEditor;
        private Label label1;
        private ComboBox cbTilesetType;
        private HScrollBar sclTileWidth;
        private VScrollBar sclTileHeight;
        private Panel panel1;
    }
}