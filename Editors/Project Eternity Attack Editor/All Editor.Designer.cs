namespace ProjectEternity.Editors.AttackEditor
{
    partial class ALLAttackEditor
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
            this.gbALLAttributes = new System.Windows.Forms.GroupBox();
            this.lblLevel = new System.Windows.Forms.Label();
            this.txtLevel = new System.Windows.Forms.NumericUpDown();
            this.gbALLAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // gbALLAttributes
            // 
            this.gbALLAttributes.Controls.Add(this.lblLevel);
            this.gbALLAttributes.Controls.Add(this.txtLevel);
            this.gbALLAttributes.Location = new System.Drawing.Point(12, 12);
            this.gbALLAttributes.Name = "gbALLAttributes";
            this.gbALLAttributes.Size = new System.Drawing.Size(185, 46);
            this.gbALLAttributes.TabIndex = 1;
            this.gbALLAttributes.TabStop = false;
            this.gbALLAttributes.Text = "ALL Attributes";
            // 
            // lblLevel
            // 
            this.lblLevel.AutoSize = true;
            this.lblLevel.Location = new System.Drawing.Point(6, 21);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(33, 13);
            this.lblLevel.TabIndex = 47;
            this.lblLevel.Text = "Level";
            // 
            // txtLevel
            // 
            this.txtLevel.Location = new System.Drawing.Point(108, 19);
            this.txtLevel.Name = "txtLevel";
            this.txtLevel.Size = new System.Drawing.Size(71, 20);
            this.txtLevel.TabIndex = 46;
            // 
            // ALLAttackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 60);
            this.Controls.Add(this.gbALLAttributes);
            this.Name = "ALLAttackEditor";
            this.Text = "ALL Attack Editor";
            this.gbALLAttributes.ResumeLayout(false);
            this.gbALLAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtLevel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbALLAttributes;
        private System.Windows.Forms.Label lblLevel;
        public System.Windows.Forms.NumericUpDown txtLevel;
    }
}