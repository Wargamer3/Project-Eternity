namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class AnimationTimelineViewer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cmsTimelineOptions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmInsertKeyFrame = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDeleteKeyFrame = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCreateGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vsbTimeline = new System.Windows.Forms.VScrollBar();
            this.cmsTimelineOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsTimelineOptions
            // 
            this.cmsTimelineOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmInsertKeyFrame,
            this.tsmDeleteKeyFrame,
            this.tsmCreateGroup,
            this.tsmDeleteItem});
            this.cmsTimelineOptions.Name = "cmsTimelineOptiions";
            this.cmsTimelineOptions.Size = new System.Drawing.Size(166, 92);
            // 
            // tsmInsertKeyFrame
            // 
            this.tsmInsertKeyFrame.Name = "tsmInsertKeyFrame";
            this.tsmInsertKeyFrame.Size = new System.Drawing.Size(165, 22);
            this.tsmInsertKeyFrame.Text = "Insert Key Frame";
            this.tsmInsertKeyFrame.Click += new System.EventHandler(this.tsmInsertKeyFrame_Click);
            // 
            // tsmDeleteKeyFrame
            // 
            this.tsmDeleteKeyFrame.Name = "tsmDeleteKeyFrame";
            this.tsmDeleteKeyFrame.Size = new System.Drawing.Size(165, 22);
            this.tsmDeleteKeyFrame.Text = "Delete Key Frame";
            this.tsmDeleteKeyFrame.Click += new System.EventHandler(this.tsmDeleteKeyFrame_Click);
            // 
            // tsmCreateGroup
            // 
            this.tsmCreateGroup.Name = "tsmCreateGroup";
            this.tsmCreateGroup.Size = new System.Drawing.Size(165, 22);
            this.tsmCreateGroup.Text = "Create Group";
            this.tsmCreateGroup.Click += new System.EventHandler(this.tsmCreateGroup_Click);
            // 
            // tsmDeleteItem
            // 
            this.tsmDeleteItem.Name = "tsmDeleteItem";
            this.tsmDeleteItem.Size = new System.Drawing.Size(165, 22);
            this.tsmDeleteItem.Text = "Delete Item";
            this.tsmDeleteItem.Click += new System.EventHandler(this.tsmDeleteItem_Click);
            // 
            // vsbTimeline
            // 
            this.vsbTimeline.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.vsbTimeline.LargeChange = 1;
            this.vsbTimeline.Location = new System.Drawing.Point(133, 0);
            this.vsbTimeline.Maximum = 5;
            this.vsbTimeline.Name = "vsbTimeline";
            this.vsbTimeline.Size = new System.Drawing.Size(17, 150);
            this.vsbTimeline.TabIndex = 2;
            this.vsbTimeline.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsbTimeline_Scroll);
            // 
            // AnimationTimelineViewer
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.vsbTimeline);
            this.Name = "AnimationTimelineViewer";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.panTimelineViewer_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.panTimelineViewer_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.panTimelineViewer_DragOver);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panTimelineViewer_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panTimelineViewer_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panTimelineViewer_MouseUp);
            this.Resize += new System.EventHandler(this.panTimelineViewer_Resize);
            this.cmsTimelineOptions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        internal System.Windows.Forms.ContextMenuStrip cmsTimelineOptions;
        internal System.Windows.Forms.ToolStripMenuItem tsmInsertKeyFrame;
        internal System.Windows.Forms.ToolStripMenuItem tsmDeleteKeyFrame;
        internal System.Windows.Forms.ToolStripMenuItem tsmCreateGroup;
        internal System.Windows.Forms.ToolStripMenuItem tsmDeleteItem;
        private System.Windows.Forms.VScrollBar vsbTimeline;
    }
}
