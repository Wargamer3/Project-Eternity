namespace ProjectEternity.Editors.AdventureEditor
{
    partial class AdventureMapEditor
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.LayerViewer = new ProjectEternity.Editors.AdventureEditor.MapViewerControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLayers = new System.Windows.Forms.TabPage();
            this.gbCollisionBoxes = new System.Windows.Forms.GroupBox();
            this.lstCollisionBox = new System.Windows.Forms.ListBox();
            this.btnAddCollisionBox = new System.Windows.Forms.Button();
            this.btnRemoveCollisionBox = new System.Windows.Forms.Button();
            this.tabImages = new System.Windows.Forms.TabPage();
            this.tcImage = new System.Windows.Forms.TabControl();
            this.tabImagePreview = new System.Windows.Forms.TabPage();
            this.pnBackgroundPreview = new System.Windows.Forms.Panel();
            this.ImageViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.tabImageProperties = new System.Windows.Forms.TabPage();
            this.pgImage = new System.Windows.Forms.PropertyGrid();
            this.gbImages = new System.Windows.Forms.GroupBox();
            this.lstImages = new System.Windows.Forms.ListBox();
            this.btnAddImage = new System.Windows.Forms.Button();
            this.btnRemoveImage = new System.Windows.Forms.Button();
            this.tabProps = new System.Windows.Forms.TabPage();
            this.gbProps = new System.Windows.Forms.GroupBox();
            this.lstProps = new System.Windows.Forms.ListBox();
            this.pgProps = new System.Windows.Forms.PropertyGrid();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmProperties = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLayers.SuspendLayout();
            this.gbCollisionBoxes.SuspendLayout();
            this.tabImages.SuspendLayout();
            this.tcImage.SuspendLayout();
            this.tabImagePreview.SuspendLayout();
            this.pnBackgroundPreview.SuspendLayout();
            this.tabImageProperties.SuspendLayout();
            this.gbImages.SuspendLayout();
            this.tabProps.SuspendLayout();
            this.gbProps.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 24);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LayerViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(866, 434);
            this.splitContainer1.SplitterDistance = 568;
            this.splitContainer1.TabIndex = 0;
            // 
            // LayerViewer
            // 
            this.LayerViewer.AllowDrop = true;
            this.LayerViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayerViewer.Location = new System.Drawing.Point(0, 0);
            this.LayerViewer.Name = "LayerViewer";
            this.LayerViewer.Size = new System.Drawing.Size(564, 430);
            this.LayerViewer.TabIndex = 0;
            this.LayerViewer.DragDrop += new System.Windows.Forms.DragEventHandler(this.LayerViewer_DragDrop);
            this.LayerViewer.DragEnter += new System.Windows.Forms.DragEventHandler(this.LayerViewer_DragEnter);
            this.LayerViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LayerViewer_MouseDown);
            this.LayerViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LayerViewer_MouseMove);
            this.LayerViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.LayerViewer_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLayers);
            this.tabControl1.Controls.Add(this.tabImages);
            this.tabControl1.Controls.Add(this.tabProps);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(290, 430);
            this.tabControl1.TabIndex = 0;
            // 
            // tabLayers
            // 
            this.tabLayers.Controls.Add(this.gbCollisionBoxes);
            this.tabLayers.Location = new System.Drawing.Point(4, 22);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayers.Size = new System.Drawing.Size(282, 404);
            this.tabLayers.TabIndex = 4;
            this.tabLayers.Text = "Layers";
            this.tabLayers.UseVisualStyleBackColor = true;
            // 
            // gbCollisionBoxes
            // 
            this.gbCollisionBoxes.Controls.Add(this.lstCollisionBox);
            this.gbCollisionBoxes.Controls.Add(this.btnAddCollisionBox);
            this.gbCollisionBoxes.Controls.Add(this.btnRemoveCollisionBox);
            this.gbCollisionBoxes.Location = new System.Drawing.Point(6, 220);
            this.gbCollisionBoxes.Name = "gbCollisionBoxes";
            this.gbCollisionBoxes.Size = new System.Drawing.Size(259, 178);
            this.gbCollisionBoxes.TabIndex = 12;
            this.gbCollisionBoxes.TabStop = false;
            this.gbCollisionBoxes.Text = "Collision Boxes";
            // 
            // lstCollisionBox
            // 
            this.lstCollisionBox.FormattingEnabled = true;
            this.lstCollisionBox.Location = new System.Drawing.Point(6, 19);
            this.lstCollisionBox.Name = "lstCollisionBox";
            this.lstCollisionBox.Size = new System.Drawing.Size(247, 121);
            this.lstCollisionBox.TabIndex = 5;
            this.lstCollisionBox.SelectedIndexChanged += new System.EventHandler(this.lstCollisionBox_SelectedIndexChanged);
            // 
            // btnAddCollisionBox
            // 
            this.btnAddCollisionBox.Location = new System.Drawing.Point(6, 149);
            this.btnAddCollisionBox.Name = "btnAddCollisionBox";
            this.btnAddCollisionBox.Size = new System.Drawing.Size(120, 23);
            this.btnAddCollisionBox.TabIndex = 1;
            this.btnAddCollisionBox.Text = "Add collision box";
            this.btnAddCollisionBox.UseVisualStyleBackColor = true;
            this.btnAddCollisionBox.Click += new System.EventHandler(this.btnAddCollisionBox_Click);
            // 
            // btnRemoveCollisionBox
            // 
            this.btnRemoveCollisionBox.Location = new System.Drawing.Point(133, 149);
            this.btnRemoveCollisionBox.Name = "btnRemoveCollisionBox";
            this.btnRemoveCollisionBox.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveCollisionBox.TabIndex = 2;
            this.btnRemoveCollisionBox.Text = "Remove collision box";
            this.btnRemoveCollisionBox.UseVisualStyleBackColor = true;
            this.btnRemoveCollisionBox.Click += new System.EventHandler(this.btnRemoveCollisionBox_Click);
            // 
            // tabImages
            // 
            this.tabImages.Controls.Add(this.tcImage);
            this.tabImages.Controls.Add(this.gbImages);
            this.tabImages.Location = new System.Drawing.Point(4, 22);
            this.tabImages.Name = "tabImages";
            this.tabImages.Padding = new System.Windows.Forms.Padding(3);
            this.tabImages.Size = new System.Drawing.Size(282, 404);
            this.tabImages.TabIndex = 1;
            this.tabImages.Text = "Images";
            this.tabImages.UseVisualStyleBackColor = true;
            // 
            // tcImage
            // 
            this.tcImage.Controls.Add(this.tabImagePreview);
            this.tcImage.Controls.Add(this.tabImageProperties);
            this.tcImage.Location = new System.Drawing.Point(0, 190);
            this.tcImage.Name = "tcImage";
            this.tcImage.SelectedIndex = 0;
            this.tcImage.Size = new System.Drawing.Size(271, 214);
            this.tcImage.TabIndex = 1;
            // 
            // tabImagePreview
            // 
            this.tabImagePreview.Controls.Add(this.pnBackgroundPreview);
            this.tabImagePreview.Location = new System.Drawing.Point(4, 22);
            this.tabImagePreview.Name = "tabImagePreview";
            this.tabImagePreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabImagePreview.Size = new System.Drawing.Size(263, 188);
            this.tabImagePreview.TabIndex = 0;
            this.tabImagePreview.Text = "Image Preview";
            this.tabImagePreview.UseVisualStyleBackColor = true;
            // 
            // pnBackgroundPreview
            // 
            this.pnBackgroundPreview.AutoScroll = true;
            this.pnBackgroundPreview.Controls.Add(this.ImageViewer);
            this.pnBackgroundPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBackgroundPreview.Location = new System.Drawing.Point(3, 3);
            this.pnBackgroundPreview.Name = "pnBackgroundPreview";
            this.pnBackgroundPreview.Size = new System.Drawing.Size(257, 182);
            this.pnBackgroundPreview.TabIndex = 1;
            // 
            // ImageViewer
            // 
            this.ImageViewer.Location = new System.Drawing.Point(0, 0);
            this.ImageViewer.Name = "ImageViewer";
            this.ImageViewer.Size = new System.Drawing.Size(243, 179);
            this.ImageViewer.TabIndex = 0;
            // 
            // tabImageProperties
            // 
            this.tabImageProperties.Controls.Add(this.pgImage);
            this.tabImageProperties.Location = new System.Drawing.Point(4, 22);
            this.tabImageProperties.Name = "tabImageProperties";
            this.tabImageProperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabImageProperties.Size = new System.Drawing.Size(263, 188);
            this.tabImageProperties.TabIndex = 1;
            this.tabImageProperties.Text = "Image Properties";
            this.tabImageProperties.UseVisualStyleBackColor = true;
            // 
            // pgBackground
            // 
            this.pgImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgImage.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgImage.Location = new System.Drawing.Point(3, 3);
            this.pgImage.Name = "pgBackground";
            this.pgImage.Size = new System.Drawing.Size(257, 182);
            this.pgImage.TabIndex = 1;
            // 
            // gbImages
            // 
            this.gbImages.Controls.Add(this.lstImages);
            this.gbImages.Controls.Add(this.btnAddImage);
            this.gbImages.Controls.Add(this.btnRemoveImage);
            this.gbImages.Location = new System.Drawing.Point(6, 6);
            this.gbImages.Name = "gbImages";
            this.gbImages.Size = new System.Drawing.Size(259, 178);
            this.gbImages.TabIndex = 9;
            this.gbImages.TabStop = false;
            this.gbImages.Text = "Images";
            // 
            // lstImages
            // 
            this.lstImages.FormattingEnabled = true;
            this.lstImages.Location = new System.Drawing.Point(6, 19);
            this.lstImages.Name = "lstImages";
            this.lstImages.Size = new System.Drawing.Size(247, 121);
            this.lstImages.TabIndex = 5;
            this.lstImages.SelectedIndexChanged += new System.EventHandler(this.lstImages_SelectedIndexChanged);
            this.lstImages.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstImages_MouseDown);
            // 
            // btnAddImage
            // 
            this.btnAddImage.Location = new System.Drawing.Point(6, 146);
            this.btnAddImage.Name = "btnAddImage";
            this.btnAddImage.Size = new System.Drawing.Size(120, 23);
            this.btnAddImage.TabIndex = 1;
            this.btnAddImage.Text = "Add background";
            this.btnAddImage.UseVisualStyleBackColor = true;
            this.btnAddImage.Click += new System.EventHandler(this.btnAddImage_Click);
            // 
            // btnRemoveImage
            // 
            this.btnRemoveImage.Location = new System.Drawing.Point(133, 146);
            this.btnRemoveImage.Name = "btnRemoveImage";
            this.btnRemoveImage.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveImage.TabIndex = 2;
            this.btnRemoveImage.Text = "Remove background";
            this.btnRemoveImage.UseVisualStyleBackColor = true;
            this.btnRemoveImage.Click += new System.EventHandler(this.btnRemoveImage_Click);
            // 
            // tabProps
            // 
            this.tabProps.Controls.Add(this.gbProps);
            this.tabProps.Controls.Add(this.pgProps);
            this.tabProps.Location = new System.Drawing.Point(4, 22);
            this.tabProps.Name = "tabProps";
            this.tabProps.Size = new System.Drawing.Size(282, 404);
            this.tabProps.TabIndex = 3;
            this.tabProps.Text = "Props";
            this.tabProps.UseVisualStyleBackColor = true;
            // 
            // gbProps
            // 
            this.gbProps.Controls.Add(this.lstProps);
            this.gbProps.Location = new System.Drawing.Point(6, 6);
            this.gbProps.Name = "gbProps";
            this.gbProps.Size = new System.Drawing.Size(259, 178);
            this.gbProps.TabIndex = 10;
            this.gbProps.TabStop = false;
            this.gbProps.Text = "Props";
            // 
            // lstProps
            // 
            this.lstProps.FormattingEnabled = true;
            this.lstProps.Location = new System.Drawing.Point(6, 19);
            this.lstProps.Name = "lstProps";
            this.lstProps.Size = new System.Drawing.Size(247, 147);
            this.lstProps.TabIndex = 5;
            this.lstProps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstProps_MouseDown);
            // 
            // pgProps
            // 
            this.pgProps.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgProps.Location = new System.Drawing.Point(6, 190);
            this.pgProps.Name = "pgProps";
            this.pgProps.Size = new System.Drawing.Size(259, 208);
            this.pgProps.TabIndex = 0;
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmProperties});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(866, 24);
            this.mnuToolBar.TabIndex = 1;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmProperties
            // 
            this.tsmProperties.Name = "tsmProperties";
            this.tsmProperties.Size = new System.Drawing.Size(72, 20);
            this.tsmProperties.Text = "Properties";
            this.tsmProperties.Click += new System.EventHandler(this.tsmProperties_Click);
            // 
            // AdventureMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(866, 458);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.mnuToolBar);
            this.MainMenuStrip = this.mnuToolBar;
            this.Name = "AdventureMapEditor";
            this.Text = "Triple Thunder Map Editor";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabLayers.ResumeLayout(false);
            this.gbCollisionBoxes.ResumeLayout(false);
            this.tabImages.ResumeLayout(false);
            this.tcImage.ResumeLayout(false);
            this.tabImagePreview.ResumeLayout(false);
            this.pnBackgroundPreview.ResumeLayout(false);
            this.tabImageProperties.ResumeLayout(false);
            this.gbImages.ResumeLayout(false);
            this.tabProps.ResumeLayout(false);
            this.gbProps.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabImages;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private ProjectEternity.Editors.AdventureEditor.MapViewerControl LayerViewer;
        private System.Windows.Forms.TabPage tabProps;
        private System.Windows.Forms.GroupBox gbImages;
        private System.Windows.Forms.ListBox lstImages;
        private System.Windows.Forms.Button btnAddImage;
        private System.Windows.Forms.Button btnRemoveImage;
        private System.Windows.Forms.TabPage tabLayers;
        private System.Windows.Forms.GroupBox gbCollisionBoxes;
        private System.Windows.Forms.ListBox lstCollisionBox;
        private System.Windows.Forms.Button btnAddCollisionBox;
        private System.Windows.Forms.Button btnRemoveCollisionBox;
        private System.Windows.Forms.PropertyGrid pgProps;
        private System.Windows.Forms.GroupBox gbProps;
        private System.Windows.Forms.ListBox lstProps;
        private System.Windows.Forms.TabControl tcImage;
        private System.Windows.Forms.TabPage tabImagePreview;
        private System.Windows.Forms.TabPage tabImageProperties;
        private System.Windows.Forms.Panel pnBackgroundPreview;
        private Core.Editor.Texture2DViewerControl ImageViewer;
        private System.Windows.Forms.PropertyGrid pgImage;
        private System.Windows.Forms.ToolStripMenuItem tsmProperties;
    }
}
