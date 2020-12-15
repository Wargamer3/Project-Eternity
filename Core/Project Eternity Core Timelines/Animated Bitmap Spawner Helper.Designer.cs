namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class AnimatedBitmapSpawnerHelper
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
            this.SpawnViewer = new ProjectEternity.GameScreens.AnimationScreen.SpawnerViewerControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtOriginY = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOriginX = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSetSprite = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            //
            // splitContainer1
            //
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            //
            // splitContainer1.Panel1
            //
            this.splitContainer1.Panel1.Controls.Add(this.SpawnViewer);
            //
            // splitContainer1.Panel2
            //
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.btnSetSprite);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnConfirm);
            this.splitContainer1.Size = new System.Drawing.Size(450, 377);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 1;
            //
            // SpawnViewer
            //
            this.SpawnViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpawnViewer.Location = new System.Drawing.Point(0, 0);
            this.SpawnViewer.Name = "SpawnViewer";
            this.SpawnViewer.Size = new System.Drawing.Size(271, 377);
            this.SpawnViewer.TabIndex = 0;
            this.SpawnViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SpawnViewer_MouseMove);
            this.SpawnViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SpawnViewer_MouseMove);
            //
            // panel1
            //
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(12, 132);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(154, 188);
            this.panel1.TabIndex = 4;
            //
            // groupBox1
            //
            this.groupBox1.Controls.Add(this.txtOriginY);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtOriginX);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 72);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Origin";
            //
            // txtOriginY
            //
            this.txtOriginY.Location = new System.Drawing.Point(63, 45);
            this.txtOriginY.Name = "txtOriginY";
            this.txtOriginY.Size = new System.Drawing.Size(100, 20);
            this.txtOriginY.TabIndex = 3;
            this.txtOriginY.Text = "0";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Origin Y";
            //
            // txtOriginX
            //
            this.txtOriginX.Location = new System.Drawing.Point(63, 19);
            this.txtOriginX.Name = "txtOriginX";
            this.txtOriginX.Size = new System.Drawing.Size(100, 20);
            this.txtOriginX.TabIndex = 1;
            this.txtOriginX.Text = "0";
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Origin X";
            //
            // btnSetSprite
            //
            this.btnSetSprite.Location = new System.Drawing.Point(3, 12);
            this.btnSetSprite.Name = "btnSetSprite";
            this.btnSetSprite.Size = new System.Drawing.Size(75, 23);
            this.btnSetSprite.TabIndex = 2;
            this.btnSetSprite.Text = "Set sprite";
            this.btnSetSprite.UseVisualStyleBackColor = true;
            this.btnSetSprite.Click += new System.EventHandler(this.btnSetSprite_Click);
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(97, 351);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            //
            // btnConfirm
            //
            this.btnConfirm.Location = new System.Drawing.Point(3, 351);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            //
            // AnimatedBitmapSpawnerHelper
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 377);
            this.Controls.Add(this.splitContainer1);
            this.Name = "AnimatedBitmapSpawnerHelper";
            this.Text = "Animated Bitmap Spawner Helper";
            this.Shown += new System.EventHandler(this.AnimatedBitmapSpawnerHelper_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnSetSprite;
        public SpawnerViewerControl SpawnViewer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtOriginX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOriginY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
    }
}
