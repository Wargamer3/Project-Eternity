namespace ProjectEternity.Editors.AttackEditor
{
    partial class DashAttackEditor
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
            this.bgDashAttributes = new System.Windows.Forms.GroupBox();
            this.lblMaxDashReach = new System.Windows.Forms.Label();
            this.txtMaxDashReach = new System.Windows.Forms.NumericUpDown();
            this.gbEnemyKnockback = new System.Windows.Forms.GroupBox();
            this.lblSelfKnockback = new System.Windows.Forms.Label();
            this.txtSelfKnockback = new System.Windows.Forms.NumericUpDown();
            this.lblEnemyKnockback = new System.Windows.Forms.Label();
            this.txtEnemyKnockback = new System.Windows.Forms.NumericUpDown();
            this.bgDashAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxDashReach)).BeginInit();
            this.gbEnemyKnockback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelfKnockback)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEnemyKnockback)).BeginInit();
            this.SuspendLayout();
            // 
            // bgDashAttributes
            // 
            this.bgDashAttributes.Controls.Add(this.lblMaxDashReach);
            this.bgDashAttributes.Controls.Add(this.txtMaxDashReach);
            this.bgDashAttributes.Location = new System.Drawing.Point(12, 12);
            this.bgDashAttributes.Name = "bgDashAttributes";
            this.bgDashAttributes.Size = new System.Drawing.Size(185, 46);
            this.bgDashAttributes.TabIndex = 1;
            this.bgDashAttributes.TabStop = false;
            this.bgDashAttributes.Text = "Dash Attributes";
            // 
            // lblMaxDashReach
            // 
            this.lblMaxDashReach.AutoSize = true;
            this.lblMaxDashReach.Location = new System.Drawing.Point(6, 21);
            this.lblMaxDashReach.Name = "lblMaxDashReach";
            this.lblMaxDashReach.Size = new System.Drawing.Size(83, 13);
            this.lblMaxDashReach.TabIndex = 47;
            this.lblMaxDashReach.Text = "Max dash reach";
            // 
            // txtMaxDashReach
            // 
            this.txtMaxDashReach.Location = new System.Drawing.Point(108, 19);
            this.txtMaxDashReach.Name = "txtMaxDashReach";
            this.txtMaxDashReach.Size = new System.Drawing.Size(71, 20);
            this.txtMaxDashReach.TabIndex = 46;
            // 
            // gbEnemyKnockback
            // 
            this.gbEnemyKnockback.Controls.Add(this.lblSelfKnockback);
            this.gbEnemyKnockback.Controls.Add(this.txtSelfKnockback);
            this.gbEnemyKnockback.Controls.Add(this.lblEnemyKnockback);
            this.gbEnemyKnockback.Controls.Add(this.txtEnemyKnockback);
            this.gbEnemyKnockback.Location = new System.Drawing.Point(12, 64);
            this.gbEnemyKnockback.Name = "gbEnemyKnockback";
            this.gbEnemyKnockback.Size = new System.Drawing.Size(185, 80);
            this.gbEnemyKnockback.TabIndex = 48;
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
            // DashAttackEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(207, 156);
            this.Controls.Add(this.gbEnemyKnockback);
            this.Controls.Add(this.bgDashAttributes);
            this.Name = "DashAttackEditor";
            this.Text = "Knockback Editor";
            this.bgDashAttributes.ResumeLayout(false);
            this.bgDashAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxDashReach)).EndInit();
            this.gbEnemyKnockback.ResumeLayout(false);
            this.gbEnemyKnockback.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSelfKnockback)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEnemyKnockback)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox bgDashAttributes;
        private System.Windows.Forms.Label lblMaxDashReach;
        public System.Windows.Forms.NumericUpDown txtMaxDashReach;
        private System.Windows.Forms.GroupBox gbEnemyKnockback;
        private System.Windows.Forms.Label lblSelfKnockback;
        public System.Windows.Forms.NumericUpDown txtSelfKnockback;
        private System.Windows.Forms.Label lblEnemyKnockback;
        public System.Windows.Forms.NumericUpDown txtEnemyKnockback;
    }
}