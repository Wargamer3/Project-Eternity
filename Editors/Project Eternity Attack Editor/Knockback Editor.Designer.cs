namespace ProjectEternity.Editors.AttackEditor
{
    partial class KnockbackAttackEditor
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
            this.gbEnemyKnockback = new System.Windows.Forms.GroupBox();
            this.lblSelfKnockback = new System.Windows.Forms.Label();
            this.txtSelfKnockback = new System.Windows.Forms.NumericUpDown();
            this.lblEnemyKnockback = new System.Windows.Forms.Label();
            this.txtEnemyKnockback = new System.Windows.Forms.NumericUpDown();
            this.gbEnemyKnockback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelfKnockback)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEnemyKnockback)).BeginInit();
            this.SuspendLayout();
            // 
            // gbEnemyKnockback
            // 
            this.gbEnemyKnockback.Controls.Add(this.lblSelfKnockback);
            this.gbEnemyKnockback.Controls.Add(this.txtSelfKnockback);
            this.gbEnemyKnockback.Controls.Add(this.lblEnemyKnockback);
            this.gbEnemyKnockback.Controls.Add(this.txtEnemyKnockback);
            this.gbEnemyKnockback.Location = new System.Drawing.Point(12, 12);
            this.gbEnemyKnockback.Name = "gbEnemyKnockback";
            this.gbEnemyKnockback.Size = new System.Drawing.Size(185, 80);
            this.gbEnemyKnockback.TabIndex = 0;
            this.gbEnemyKnockback.TabStop = false;
            this.gbEnemyKnockback.Text = "Kockback Attributes";
            // 
            // lblSelfKnockback
            // 
            this.lblSelfKnockback.AutoSize = true;
            this.lblSelfKnockback.Location = new System.Drawing.Point(6, 47);
            this.lblSelfKnockback.Name = "lblSelfKnockback";
            this.lblSelfKnockback.Size = new System.Drawing.Size(82, 13);
            this.lblSelfKnockback.TabIndex = 49;
            this.lblSelfKnockback.Text = "Self knockback";
            // 
            // txtSelfKnockback
            // 
            this.txtSelfKnockback.Location = new System.Drawing.Point(108, 45);
            this.txtSelfKnockback.Name = "txtSelfKnockback";
            this.txtSelfKnockback.Size = new System.Drawing.Size(71, 20);
            this.txtSelfKnockback.TabIndex = 48;
            // 
            // lblEnemyKnockback
            // 
            this.lblEnemyKnockback.AutoSize = true;
            this.lblEnemyKnockback.Location = new System.Drawing.Point(6, 21);
            this.lblEnemyKnockback.Name = "lblEnemyKnockback";
            this.lblEnemyKnockback.Size = new System.Drawing.Size(96, 13);
            this.lblEnemyKnockback.TabIndex = 47;
            this.lblEnemyKnockback.Text = "Enemy knockback";
            // 
            // txtEnemyKnockback
            // 
            this.txtEnemyKnockback.Location = new System.Drawing.Point(108, 19);
            this.txtEnemyKnockback.Name = "txtEnemyKnockback";
            this.txtEnemyKnockback.Size = new System.Drawing.Size(71, 20);
            this.txtEnemyKnockback.TabIndex = 46;
            // 
            // KnockbackAttackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 103);
            this.Controls.Add(this.gbEnemyKnockback);
            this.Name = "KnockbackAttackEditor";
            this.Text = "Knockback Editor";
            this.gbEnemyKnockback.ResumeLayout(false);
            this.gbEnemyKnockback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelfKnockback)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEnemyKnockback)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbEnemyKnockback;
        private System.Windows.Forms.Label lblEnemyKnockback;
        public System.Windows.Forms.NumericUpDown txtEnemyKnockback;
        private System.Windows.Forms.Label lblSelfKnockback;
        public System.Windows.Forms.NumericUpDown txtSelfKnockback;
    }
}