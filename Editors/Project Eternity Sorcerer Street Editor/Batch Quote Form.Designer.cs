namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    partial class BatchQuoteForm
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
            this.gbQuotesToAdd = new System.Windows.Forms.GroupBox();
            this.txtQuotesToAdd = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.gbQuotesToAdd.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbQuotesToAdd
            // 
            this.gbQuotesToAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbQuotesToAdd.Controls.Add(this.txtQuotesToAdd);
            this.gbQuotesToAdd.Location = new System.Drawing.Point(12, 12);
            this.gbQuotesToAdd.Name = "gbQuotesToAdd";
            this.gbQuotesToAdd.Size = new System.Drawing.Size(583, 235);
            this.gbQuotesToAdd.TabIndex = 0;
            this.gbQuotesToAdd.TabStop = false;
            this.gbQuotesToAdd.Text = "Quotes to add (1 per line)";
            // 
            // txtQuotesToAdd
            // 
            this.txtQuotesToAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuotesToAdd.Location = new System.Drawing.Point(6, 19);
            this.txtQuotesToAdd.Multiline = true;
            this.txtQuotesToAdd.Name = "txtQuotesToAdd";
            this.txtQuotesToAdd.Size = new System.Drawing.Size(571, 210);
            this.txtQuotesToAdd.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(520, 253);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnConfirm
            // 
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(439, 253);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            // 
            // BatchQuoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 288);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbQuotesToAdd);
            this.Name = "BatchQuoteForm";
            this.Text = "Batch Quote";
            this.gbQuotesToAdd.ResumeLayout(false);
            this.gbQuotesToAdd.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbQuotesToAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        public System.Windows.Forms.TextBox txtQuotesToAdd;
    }
}