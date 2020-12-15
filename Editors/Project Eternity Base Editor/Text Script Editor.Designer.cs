namespace ProjectEternity.Core.Editor
{
    partial class TextScriptEditor
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
            this.scTextScript = new System.Windows.Forms.SplitContainer();
            this.txtTextScript = new System.Windows.Forms.TextBox();
            this.txtTextScriptHelper = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.scTextScript)).BeginInit();
            this.scTextScript.Panel1.SuspendLayout();
            this.scTextScript.Panel2.SuspendLayout();
            this.scTextScript.SuspendLayout();
            this.SuspendLayout();
            // 
            // scTextScript
            // 
            this.scTextScript.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.scTextScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scTextScript.Location = new System.Drawing.Point(0, 0);
            this.scTextScript.Name = "scTextScript";
            this.scTextScript.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scTextScript.Panel1
            // 
            this.scTextScript.Panel1.Controls.Add(this.txtTextScript);
            // 
            // scTextScript.Panel2
            // 
            this.scTextScript.Panel2.Controls.Add(this.txtTextScriptHelper);
            this.scTextScript.Size = new System.Drawing.Size(412, 174);
            this.scTextScript.SplitterDistance = 137;
            this.scTextScript.TabIndex = 0;
            // 
            // txtTextScript
            // 
            this.txtTextScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTextScript.Location = new System.Drawing.Point(0, 0);
            this.txtTextScript.Multiline = true;
            this.txtTextScript.Name = "txtTextScript";
            this.txtTextScript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTextScript.Size = new System.Drawing.Size(408, 133);
            this.txtTextScript.TabIndex = 0;
            this.txtTextScript.WordWrap = false;
            this.txtTextScript.TextChanged += new System.EventHandler(this.txtTextScript_TextChanged);
            // 
            // txtTextScriptHelper
            // 
            this.txtTextScriptHelper.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTextScriptHelper.Location = new System.Drawing.Point(0, 0);
            this.txtTextScriptHelper.Multiline = true;
            this.txtTextScriptHelper.Name = "txtTextScriptHelper";
            this.txtTextScriptHelper.ReadOnly = true;
            this.txtTextScriptHelper.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTextScriptHelper.Size = new System.Drawing.Size(408, 29);
            this.txtTextScriptHelper.TabIndex = 0;
            // 
            // TextScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 174);
            this.Controls.Add(this.scTextScript);
            this.Name = "TextScriptEditor";
            this.Text = "Text Script Editor";
            this.scTextScript.Panel1.ResumeLayout(false);
            this.scTextScript.Panel1.PerformLayout();
            this.scTextScript.Panel2.ResumeLayout(false);
            this.scTextScript.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scTextScript)).EndInit();
            this.scTextScript.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer scTextScript;
        private System.Windows.Forms.TextBox txtTextScriptHelper;
        public System.Windows.Forms.TextBox txtTextScript;
    }
}