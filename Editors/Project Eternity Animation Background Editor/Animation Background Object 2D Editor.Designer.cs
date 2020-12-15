namespace ProjectEternity.Editors.AnimationBackgroundEditor
{
    partial class BackgroundObject2DEditor
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
            this.gbBackground = new System.Windows.Forms.GroupBox();
            this.BackgroundViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.gbBackgroundLink = new System.Windows.Forms.GroupBox();
            this.BackgroundLinkViewer = new ProjectEternity.Core.Editor.Texture2DViewerControl();
            this.gbLinkedBackgrounds = new System.Windows.Forms.GroupBox();
            this.btnRemoveBackground = new System.Windows.Forms.Button();
            this.btnAddBackground = new System.Windows.Forms.Button();
            this.pgBackgroundLink = new System.Windows.Forms.PropertyGrid();
            this.lstBackgroundChain = new System.Windows.Forms.ListBox();
            this.mnuToolBar = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmSetBackgroundImage = new System.Windows.Forms.ToolStripMenuItem();
            this.gbPreview = new System.Windows.Forms.GroupBox();
            this.BackgroundPreviewViewer = new ProjectEternity.Editors.AnimationBackgroundEditor.AnimationBackground2DViewerControl();
            this.gbBackground.SuspendLayout();
            this.gbBackgroundLink.SuspendLayout();
            this.gbLinkedBackgrounds.SuspendLayout();
            this.mnuToolBar.SuspendLayout();
            this.gbPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbBackground
            // 
            this.gbBackground.Controls.Add(this.BackgroundViewer);
            this.gbBackground.Location = new System.Drawing.Point(12, 27);
            this.gbBackground.Name = "gbBackground";
            this.gbBackground.Size = new System.Drawing.Size(264, 208);
            this.gbBackground.TabIndex = 0;
            this.gbBackground.TabStop = false;
            this.gbBackground.Text = "Background";
            // 
            // BackgroundViewer
            // 
            this.BackgroundViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackgroundViewer.Location = new System.Drawing.Point(6, 19);
            this.BackgroundViewer.Name = "BackgroundViewer";
            this.BackgroundViewer.Size = new System.Drawing.Size(252, 183);
            this.BackgroundViewer.TabIndex = 0;
            this.BackgroundViewer.Text = "texture2DViewerControl1";
            this.BackgroundViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundViewer_MouseMove);
            this.BackgroundViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BackgroundViewer_MouseMove);
            // 
            // gbBackgroundLink
            // 
            this.gbBackgroundLink.Controls.Add(this.BackgroundLinkViewer);
            this.gbBackgroundLink.Location = new System.Drawing.Point(282, 27);
            this.gbBackgroundLink.Name = "gbBackgroundLink";
            this.gbBackgroundLink.Size = new System.Drawing.Size(263, 208);
            this.gbBackgroundLink.TabIndex = 1;
            this.gbBackgroundLink.TabStop = false;
            this.gbBackgroundLink.Text = "Background Link";
            // 
            // BackgroundLinkViewer
            // 
            this.BackgroundLinkViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BackgroundLinkViewer.Location = new System.Drawing.Point(6, 19);
            this.BackgroundLinkViewer.Name = "BackgroundLinkViewer";
            this.BackgroundLinkViewer.Size = new System.Drawing.Size(251, 183);
            this.BackgroundLinkViewer.TabIndex = 0;
            this.BackgroundLinkViewer.Text = "texture2DViewerControl2";
            this.BackgroundLinkViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundLinkViewer_MouseMove);
            this.BackgroundLinkViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BackgroundLinkViewer_MouseMove);
            // 
            // gbLinkedBackgrounds
            // 
            this.gbLinkedBackgrounds.Controls.Add(this.btnRemoveBackground);
            this.gbLinkedBackgrounds.Controls.Add(this.btnAddBackground);
            this.gbLinkedBackgrounds.Controls.Add(this.pgBackgroundLink);
            this.gbLinkedBackgrounds.Controls.Add(this.lstBackgroundChain);
            this.gbLinkedBackgrounds.Location = new System.Drawing.Point(551, 27);
            this.gbLinkedBackgrounds.Name = "gbLinkedBackgrounds";
            this.gbLinkedBackgrounds.Size = new System.Drawing.Size(237, 422);
            this.gbLinkedBackgrounds.TabIndex = 2;
            this.gbLinkedBackgrounds.TabStop = false;
            this.gbLinkedBackgrounds.Text = "Linked Backgrounds";
            // 
            // btnRemoveBackground
            // 
            this.btnRemoveBackground.Location = new System.Drawing.Point(130, 185);
            this.btnRemoveBackground.Name = "btnRemoveBackground";
            this.btnRemoveBackground.Size = new System.Drawing.Size(101, 23);
            this.btnRemoveBackground.TabIndex = 3;
            this.btnRemoveBackground.Text = "Remove";
            this.btnRemoveBackground.UseVisualStyleBackColor = true;
            this.btnRemoveBackground.Click += new System.EventHandler(this.btnRemoveBackground_Click);
            // 
            // btnAddBackground
            // 
            this.btnAddBackground.Location = new System.Drawing.Point(6, 185);
            this.btnAddBackground.Name = "btnAddBackground";
            this.btnAddBackground.Size = new System.Drawing.Size(118, 23);
            this.btnAddBackground.TabIndex = 2;
            this.btnAddBackground.Text = "Add";
            this.btnAddBackground.UseVisualStyleBackColor = true;
            this.btnAddBackground.Click += new System.EventHandler(this.btnAddBackground_Click);
            // 
            // pgBackgroundLink
            // 
            this.pgBackgroundLink.Location = new System.Drawing.Point(6, 214);
            this.pgBackgroundLink.Name = "pgBackgroundLink";
            this.pgBackgroundLink.Size = new System.Drawing.Size(225, 202);
            this.pgBackgroundLink.TabIndex = 1;
            // 
            // lstBackgroundChain
            // 
            this.lstBackgroundChain.FormattingEnabled = true;
            this.lstBackgroundChain.Location = new System.Drawing.Point(6, 19);
            this.lstBackgroundChain.Name = "lstBackgroundChain";
            this.lstBackgroundChain.Size = new System.Drawing.Size(225, 160);
            this.lstBackgroundChain.TabIndex = 0;
            this.lstBackgroundChain.SelectedIndexChanged += new System.EventHandler(this.lstBackgroundChain_SelectedIndexChanged);
            // 
            // mnuToolBar
            // 
            this.mnuToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave,
            this.tsmSetBackgroundImage});
            this.mnuToolBar.Location = new System.Drawing.Point(0, 0);
            this.mnuToolBar.Name = "mnuToolBar";
            this.mnuToolBar.Size = new System.Drawing.Size(800, 24);
            this.mnuToolBar.TabIndex = 3;
            this.mnuToolBar.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // tsmSetBackgroundImage
            // 
            this.tsmSetBackgroundImage.Name = "tsmSetBackgroundImage";
            this.tsmSetBackgroundImage.Size = new System.Drawing.Size(138, 20);
            this.tsmSetBackgroundImage.Text = "Set Background Image";
            this.tsmSetBackgroundImage.Click += new System.EventHandler(this.tsmSetBackgroundImage_Click);
            // 
            // gbPreview
            // 
            this.gbPreview.Controls.Add(this.BackgroundPreviewViewer);
            this.gbPreview.Location = new System.Drawing.Point(12, 241);
            this.gbPreview.Name = "gbPreview";
            this.gbPreview.Size = new System.Drawing.Size(533, 208);
            this.gbPreview.TabIndex = 4;
            this.gbPreview.TabStop = false;
            this.gbPreview.Text = "Preview";
            // 
            // BackgroundPreviewViewer
            // 
            this.BackgroundPreviewViewer.Location = new System.Drawing.Point(6, 19);
            this.BackgroundPreviewViewer.Name = "BackgroundPreviewViewer";
            this.BackgroundPreviewViewer.Size = new System.Drawing.Size(521, 183);
            this.BackgroundPreviewViewer.TabIndex = 0;
            this.BackgroundPreviewViewer.Text = "animationBackground2DViewerControl1";
            this.BackgroundPreviewViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BackgroundPreviewViewer_MouseDown);
            this.BackgroundPreviewViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BackgroundPreviewViewer_MouseMove);
            // 
            // BackgroundObject2DEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 461);
            this.Controls.Add(this.gbPreview);
            this.Controls.Add(this.mnuToolBar);
            this.Controls.Add(this.gbLinkedBackgrounds);
            this.Controls.Add(this.gbBackgroundLink);
            this.Controls.Add(this.gbBackground);
            this.Name = "BackgroundObject2DEditor";
            this.Text = "Background Chains";
            this.gbBackground.ResumeLayout(false);
            this.gbBackgroundLink.ResumeLayout(false);
            this.gbLinkedBackgrounds.ResumeLayout(false);
            this.mnuToolBar.ResumeLayout(false);
            this.mnuToolBar.PerformLayout();
            this.gbPreview.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbBackground;
        private Core.Editor.Texture2DViewerControl BackgroundViewer;
        private System.Windows.Forms.GroupBox gbBackgroundLink;
        private Core.Editor.Texture2DViewerControl BackgroundLinkViewer;
        private System.Windows.Forms.GroupBox gbLinkedBackgrounds;
        private System.Windows.Forms.Button btnRemoveBackground;
        private System.Windows.Forms.Button btnAddBackground;
        private System.Windows.Forms.PropertyGrid pgBackgroundLink;
        private System.Windows.Forms.ListBox lstBackgroundChain;
        private System.Windows.Forms.MenuStrip mnuToolBar;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.ToolStripMenuItem tsmSetBackgroundImage;
        private System.Windows.Forms.GroupBox gbPreview;
        private AnimationBackground2DViewerControl BackgroundPreviewViewer;
    }
}