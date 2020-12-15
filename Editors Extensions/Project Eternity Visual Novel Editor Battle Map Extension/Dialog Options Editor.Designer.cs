namespace ProjectEternity.EditorsExtensions.VisualNovelEditorExtension
{
    partial class DialogOptionsEditor
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
            this.gbSoundOptions = new System.Windows.Forms.GroupBox();
            this.lblSFX = new System.Windows.Forms.Label();
            this.txtSFX = new System.Windows.Forms.TextBox();
            this.btnChangeSFX = new System.Windows.Forms.Button();
            this.lblBGM = new System.Windows.Forms.Label();
            this.txtBGM = new System.Windows.Forms.TextBox();
            this.btnChangeBGM = new System.Windows.Forms.Button();
            this.gbSoundOptions.SuspendLayout();
            this.txtSFX.SuspendLayout();
            this.txtBGM.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSoundOptions
            // 
            this.gbSoundOptions.Controls.Add(this.lblSFX);
            this.gbSoundOptions.Controls.Add(this.txtSFX);
            this.gbSoundOptions.Controls.Add(this.lblBGM);
            this.gbSoundOptions.Controls.Add(this.txtBGM);
            this.gbSoundOptions.Enabled = false;
            this.gbSoundOptions.Location = new System.Drawing.Point(12, 12);
            this.gbSoundOptions.Name = "gbSoundOptions";
            this.gbSoundOptions.Size = new System.Drawing.Size(200, 77);
            this.gbSoundOptions.TabIndex = 0;
            this.gbSoundOptions.TabStop = false;
            this.gbSoundOptions.Text = "Sound Options";
            // 
            // lblSFX
            // 
            this.lblSFX.AutoSize = true;
            this.lblSFX.Location = new System.Drawing.Point(6, 48);
            this.lblSFX.Name = "lblSFX";
            this.lblSFX.Size = new System.Drawing.Size(27, 13);
            this.lblSFX.TabIndex = 2;
            this.lblSFX.Text = "SFX";
            // 
            // txtSFX
            // 
            this.txtSFX.Controls.Add(this.btnChangeSFX);
            this.txtSFX.Location = new System.Drawing.Point(56, 45);
            this.txtSFX.Name = "txtSFX";
            this.txtSFX.ReadOnly = true;
            this.txtSFX.Size = new System.Drawing.Size(138, 20);
            this.txtSFX.TabIndex = 3;
            // 
            // btnChangeSFX
            // 
            this.btnChangeSFX.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnChangeSFX.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnChangeSFX.Location = new System.Drawing.Point(113, 0);
            this.btnChangeSFX.Margin = new System.Windows.Forms.Padding(0);
            this.btnChangeSFX.Name = "btnChangeSFX";
            this.btnChangeSFX.Size = new System.Drawing.Size(21, 16);
            this.btnChangeSFX.TabIndex = 35;
            this.btnChangeSFX.TabStop = false;
            this.btnChangeSFX.Text = "...";
            this.btnChangeSFX.UseVisualStyleBackColor = true;
            this.btnChangeSFX.Click += new System.EventHandler(this.btnChangeSFX_Click);
            // 
            // lblBGM
            // 
            this.lblBGM.AutoSize = true;
            this.lblBGM.Location = new System.Drawing.Point(6, 22);
            this.lblBGM.Name = "lblBGM";
            this.lblBGM.Size = new System.Drawing.Size(31, 13);
            this.lblBGM.TabIndex = 1;
            this.lblBGM.Text = "BGM";
            // 
            // txtBGM
            // 
            this.txtBGM.Controls.Add(this.btnChangeBGM);
            this.txtBGM.Location = new System.Drawing.Point(56, 19);
            this.txtBGM.Name = "txtBGM";
            this.txtBGM.ReadOnly = true;
            this.txtBGM.Size = new System.Drawing.Size(138, 20);
            this.txtBGM.TabIndex = 1;
            // 
            // btnChangeBGM
            // 
            this.btnChangeBGM.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnChangeBGM.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnChangeBGM.Location = new System.Drawing.Point(113, 0);
            this.btnChangeBGM.Margin = new System.Windows.Forms.Padding(0);
            this.btnChangeBGM.Name = "btnChangeBGM";
            this.btnChangeBGM.Size = new System.Drawing.Size(21, 16);
            this.btnChangeBGM.TabIndex = 34;
            this.btnChangeBGM.TabStop = false;
            this.btnChangeBGM.Text = "...";
            this.btnChangeBGM.UseVisualStyleBackColor = true;
            this.btnChangeBGM.Click += new System.EventHandler(this.btnChangeBGM_Click);
            // 
            // DialogOptionsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 103);
            this.Controls.Add(this.gbSoundOptions);
            this.Name = "DialogOptionsEditor";
            this.Text = "Dialog Options";
            this.gbSoundOptions.ResumeLayout(false);
            this.gbSoundOptions.PerformLayout();
            this.txtSFX.ResumeLayout(false);
            this.txtBGM.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSoundOptions;
        private System.Windows.Forms.TextBox txtBGM;
        private System.Windows.Forms.Label lblSFX;
        private System.Windows.Forms.TextBox txtSFX;
        private System.Windows.Forms.Label lblBGM;
        private System.Windows.Forms.Button btnChangeBGM;
        private System.Windows.Forms.Button btnChangeSFX;
    }
}