namespace ProjectEternity.Editors.AnimationEditor
{
    partial class AnimationProperties
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
            this.gbScreenSize = new System.Windows.Forms.GroupBox();
            this.txtScreenHeight = new System.Windows.Forms.NumericUpDown();
            this.txtScreenWidth = new System.Windows.Forms.NumericUpDown();
            this.lblScreenHeight = new System.Windows.Forms.Label();
            this.lblScreenWidth = new System.Windows.Forms.Label();
            this.gbLoop = new System.Windows.Forms.GroupBox();
            this.lblLoopEnd = new System.Windows.Forms.Label();
            this.txtLoopEnd = new System.Windows.Forms.NumericUpDown();
            this.lblLoopStart = new System.Windows.Forms.Label();
            this.txtLoopStart = new System.Windows.Forms.NumericUpDown();
            this.gbBackgroundPreview = new System.Windows.Forms.GroupBox();
            this.btnSelectBackgroundPreview = new System.Windows.Forms.Button();
            this.gbScreenSize.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtScreenHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScreenWidth)).BeginInit();
            this.gbLoop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoopEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoopStart)).BeginInit();
            this.gbBackgroundPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbScreenSize
            // 
            this.gbScreenSize.Controls.Add(this.txtScreenHeight);
            this.gbScreenSize.Controls.Add(this.txtScreenWidth);
            this.gbScreenSize.Controls.Add(this.lblScreenHeight);
            this.gbScreenSize.Controls.Add(this.lblScreenWidth);
            this.gbScreenSize.Location = new System.Drawing.Point(12, 12);
            this.gbScreenSize.Name = "gbScreenSize";
            this.gbScreenSize.Size = new System.Drawing.Size(120, 73);
            this.gbScreenSize.TabIndex = 0;
            this.gbScreenSize.TabStop = false;
            this.gbScreenSize.Text = "Screen Size";
            // 
            // txtScreenHeight
            // 
            this.txtScreenHeight.Location = new System.Drawing.Point(56, 45);
            this.txtScreenHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtScreenHeight.Name = "txtScreenHeight";
            this.txtScreenHeight.Size = new System.Drawing.Size(58, 20);
            this.txtScreenHeight.TabIndex = 5;
            // 
            // txtScreenWidth
            // 
            this.txtScreenWidth.Location = new System.Drawing.Point(56, 19);
            this.txtScreenWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtScreenWidth.Name = "txtScreenWidth";
            this.txtScreenWidth.Size = new System.Drawing.Size(58, 20);
            this.txtScreenWidth.TabIndex = 4;
            // 
            // lblScreenHeight
            // 
            this.lblScreenHeight.AutoSize = true;
            this.lblScreenHeight.Location = new System.Drawing.Point(6, 47);
            this.lblScreenHeight.Name = "lblScreenHeight";
            this.lblScreenHeight.Size = new System.Drawing.Size(41, 13);
            this.lblScreenHeight.TabIndex = 3;
            this.lblScreenHeight.Text = "Height:";
            // 
            // lblScreenWidth
            // 
            this.lblScreenWidth.AutoSize = true;
            this.lblScreenWidth.Location = new System.Drawing.Point(6, 21);
            this.lblScreenWidth.Name = "lblScreenWidth";
            this.lblScreenWidth.Size = new System.Drawing.Size(41, 13);
            this.lblScreenWidth.TabIndex = 1;
            this.lblScreenWidth.Text = "Width: ";
            // 
            // gbLoop
            // 
            this.gbLoop.Controls.Add(this.lblLoopEnd);
            this.gbLoop.Controls.Add(this.txtLoopEnd);
            this.gbLoop.Controls.Add(this.lblLoopStart);
            this.gbLoop.Controls.Add(this.txtLoopStart);
            this.gbLoop.Location = new System.Drawing.Point(138, 12);
            this.gbLoop.Name = "gbLoop";
            this.gbLoop.Size = new System.Drawing.Size(120, 73);
            this.gbLoop.TabIndex = 1;
            this.gbLoop.TabStop = false;
            this.gbLoop.Text = "Loop";
            // 
            // lblLoopEnd
            // 
            this.lblLoopEnd.AutoSize = true;
            this.lblLoopEnd.Location = new System.Drawing.Point(6, 47);
            this.lblLoopEnd.Name = "lblLoopEnd";
            this.lblLoopEnd.Size = new System.Drawing.Size(29, 13);
            this.lblLoopEnd.TabIndex = 3;
            this.lblLoopEnd.Text = "End:";
            // 
            // txtLoopEnd
            // 
            this.txtLoopEnd.Location = new System.Drawing.Point(56, 45);
            this.txtLoopEnd.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtLoopEnd.Name = "txtLoopEnd";
            this.txtLoopEnd.Size = new System.Drawing.Size(58, 20);
            this.txtLoopEnd.TabIndex = 2;
            this.txtLoopEnd.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // lblLoopStart
            // 
            this.lblLoopStart.AutoSize = true;
            this.lblLoopStart.Location = new System.Drawing.Point(6, 21);
            this.lblLoopStart.Name = "lblLoopStart";
            this.lblLoopStart.Size = new System.Drawing.Size(32, 13);
            this.lblLoopStart.TabIndex = 1;
            this.lblLoopStart.Text = "Start:";
            // 
            // txtLoopStart
            // 
            this.txtLoopStart.Location = new System.Drawing.Point(56, 19);
            this.txtLoopStart.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtLoopStart.Name = "txtLoopStart";
            this.txtLoopStart.Size = new System.Drawing.Size(58, 20);
            this.txtLoopStart.TabIndex = 0;
            // 
            // gbBackgroundPreview
            // 
            this.gbBackgroundPreview.Controls.Add(this.btnSelectBackgroundPreview);
            this.gbBackgroundPreview.Location = new System.Drawing.Point(12, 91);
            this.gbBackgroundPreview.Name = "gbBackgroundPreview";
            this.gbBackgroundPreview.Size = new System.Drawing.Size(120, 52);
            this.gbBackgroundPreview.TabIndex = 4;
            this.gbBackgroundPreview.TabStop = false;
            this.gbBackgroundPreview.Text = "Background Preview";
            // 
            // btnSelectBackgroundPreview
            // 
            this.btnSelectBackgroundPreview.Location = new System.Drawing.Point(6, 19);
            this.btnSelectBackgroundPreview.Name = "btnSelectBackgroundPreview";
            this.btnSelectBackgroundPreview.Size = new System.Drawing.Size(108, 23);
            this.btnSelectBackgroundPreview.TabIndex = 0;
            this.btnSelectBackgroundPreview.Text = "Select";
            this.btnSelectBackgroundPreview.UseVisualStyleBackColor = true;
            this.btnSelectBackgroundPreview.Click += new System.EventHandler(this.btnSelectBackgroundPreview_Click);
            // 
            // AnimationProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 155);
            this.Controls.Add(this.gbBackgroundPreview);
            this.Controls.Add(this.gbLoop);
            this.Controls.Add(this.gbScreenSize);
            this.Name = "AnimationProperties";
            this.Text = "Properties";
            this.gbScreenSize.ResumeLayout(false);
            this.gbScreenSize.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtScreenHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtScreenWidth)).EndInit();
            this.gbLoop.ResumeLayout(false);
            this.gbLoop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoopEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtLoopStart)).EndInit();
            this.gbBackgroundPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbScreenSize;
        private System.Windows.Forms.Label lblScreenWidth;
        private System.Windows.Forms.Label lblScreenHeight;
        private System.Windows.Forms.GroupBox gbLoop;
        private System.Windows.Forms.Label lblLoopEnd;
        private System.Windows.Forms.Label lblLoopStart;
        public System.Windows.Forms.NumericUpDown txtLoopEnd;
        public System.Windows.Forms.NumericUpDown txtLoopStart;
        public System.Windows.Forms.NumericUpDown txtScreenHeight;
        public System.Windows.Forms.NumericUpDown txtScreenWidth;
        private System.Windows.Forms.GroupBox gbBackgroundPreview;
        private System.Windows.Forms.Button btnSelectBackgroundPreview;
    }
}