namespace ProjectEternity.Core.AI
{
    partial class FollowingScriptOrderEditor
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
            this.gbFollowingScripts = new System.Windows.Forms.GroupBox();
            this.gbScriptsOrder = new System.Windows.Forms.GroupBox();
            this.lstFollowingScripts = new System.Windows.Forms.ListBox();
            this.lstScriptsOrder = new System.Windows.Forms.ListBox();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.gbFollowingScripts.SuspendLayout();
            this.gbScriptsOrder.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFollowingScripts
            // 
            this.gbFollowingScripts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbFollowingScripts.Controls.Add(this.lstFollowingScripts);
            this.gbFollowingScripts.Location = new System.Drawing.Point(12, 12);
            this.gbFollowingScripts.Name = "gbFollowingScripts";
            this.gbFollowingScripts.Size = new System.Drawing.Size(199, 203);
            this.gbFollowingScripts.TabIndex = 0;
            this.gbFollowingScripts.TabStop = false;
            this.gbFollowingScripts.Text = "Following Scripts";
            // 
            // gbScriptsOrder
            // 
            this.gbScriptsOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbScriptsOrder.Controls.Add(this.btnMoveDown);
            this.gbScriptsOrder.Controls.Add(this.btnMoveUp);
            this.gbScriptsOrder.Controls.Add(this.lstScriptsOrder);
            this.gbScriptsOrder.Location = new System.Drawing.Point(217, 12);
            this.gbScriptsOrder.Name = "gbScriptsOrder";
            this.gbScriptsOrder.Size = new System.Drawing.Size(183, 203);
            this.gbScriptsOrder.TabIndex = 2;
            this.gbScriptsOrder.TabStop = false;
            this.gbScriptsOrder.Text = "Scripts Order";
            // 
            // lstFollowingScripts
            // 
            this.lstFollowingScripts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFollowingScripts.FormattingEnabled = true;
            this.lstFollowingScripts.Location = new System.Drawing.Point(6, 19);
            this.lstFollowingScripts.Name = "lstFollowingScripts";
            this.lstFollowingScripts.Size = new System.Drawing.Size(187, 173);
            this.lstFollowingScripts.TabIndex = 0;
            this.lstFollowingScripts.SelectedIndexChanged += new System.EventHandler(this.lstFollowingScripts_SelectedIndexChanged);
            // 
            // lstScriptsOrder
            // 
            this.lstScriptsOrder.FormattingEnabled = true;
            this.lstScriptsOrder.Location = new System.Drawing.Point(6, 19);
            this.lstScriptsOrder.Name = "lstScriptsOrder";
            this.lstScriptsOrder.Size = new System.Drawing.Size(171, 147);
            this.lstScriptsOrder.TabIndex = 1;
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(6, 174);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUp.TabIndex = 2;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(102, 174);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDown.TabIndex = 3;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // FollowingScriptOrderEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 227);
            this.Controls.Add(this.gbScriptsOrder);
            this.Controls.Add(this.gbFollowingScripts);
            this.Name = "FollowingScriptOrderEditor";
            this.Text = "Route Editor";
            this.gbFollowingScripts.ResumeLayout(false);
            this.gbScriptsOrder.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFollowingScripts;
        private System.Windows.Forms.GroupBox gbScriptsOrder;
        private System.Windows.Forms.ListBox lstFollowingScripts;
        private System.Windows.Forms.ListBox lstScriptsOrder;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
    }
}