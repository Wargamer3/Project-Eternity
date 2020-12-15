namespace ProjectEternity.Editors.VisualNovelEditor
{
    partial class TextEdit
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
            this.txtEditbox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtEditbox
            // 
            this.txtEditbox.Location = new System.Drawing.Point(12, 12);
            this.txtEditbox.Multiline = true;
            this.txtEditbox.Name = "txtEditbox";
            this.txtEditbox.Size = new System.Drawing.Size(260, 238);
            this.txtEditbox.TabIndex = 0;
            this.txtEditbox.TextChanged += new System.EventHandler(this.txtEditbox_TextChanged);
            this.txtEditbox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtEditbox_KeyPress);
            // 
            // TextEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.txtEditbox);
            this.Name = "TextEdit";
            this.Text = "TextEdit";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEditbox;
    }
}