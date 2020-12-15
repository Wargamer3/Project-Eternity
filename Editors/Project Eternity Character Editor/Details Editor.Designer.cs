namespace ProjectEternity.Editors.CharacterEditor
{
    partial class DetailsEditor
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
            this.gbPortraits = new System.Windows.Forms.GroupBox();
            this.gbPortrait = new System.Windows.Forms.GroupBox();
            this.btnSelectPortrait = new System.Windows.Forms.Button();
            this.txtPortrait = new System.Windows.Forms.TextBox();
            this.gbBox = new System.Windows.Forms.GroupBox();
            this.btnRemoveBoxPortrait = new System.Windows.Forms.Button();
            this.btnAddBoxPortrait = new System.Windows.Forms.Button();
            this.lstBox = new System.Windows.Forms.ListBox();
            this.gbBust = new System.Windows.Forms.GroupBox();
            this.btnRemoveBustPortrait = new System.Windows.Forms.Button();
            this.btnAddBustPortrait = new System.Windows.Forms.Button();
            this.lstBust = new System.Windows.Forms.ListBox();
            this.gbTags = new System.Windows.Forms.GroupBox();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.gbPortraits.SuspendLayout();
            this.gbPortrait.SuspendLayout();
            this.gbBox.SuspendLayout();
            this.gbBust.SuspendLayout();
            this.gbTags.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPortraits
            // 
            this.gbPortraits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPortraits.Controls.Add(this.gbPortrait);
            this.gbPortraits.Controls.Add(this.gbBox);
            this.gbPortraits.Controls.Add(this.gbBust);
            this.gbPortraits.Location = new System.Drawing.Point(12, 12);
            this.gbPortraits.Name = "gbPortraits";
            this.gbPortraits.Size = new System.Drawing.Size(314, 352);
            this.gbPortraits.TabIndex = 3;
            this.gbPortraits.TabStop = false;
            this.gbPortraits.Text = "Portraits";
            // 
            // gbPortrait
            // 
            this.gbPortrait.Controls.Add(this.btnSelectPortrait);
            this.gbPortrait.Controls.Add(this.txtPortrait);
            this.gbPortrait.Location = new System.Drawing.Point(6, 19);
            this.gbPortrait.Name = "gbPortrait";
            this.gbPortrait.Size = new System.Drawing.Size(302, 49);
            this.gbPortrait.TabIndex = 4;
            this.gbPortrait.TabStop = false;
            this.gbPortrait.Text = "Portrait";
            // 
            // btnSelectPortrait
            // 
            this.btnSelectPortrait.Location = new System.Drawing.Point(204, 17);
            this.btnSelectPortrait.Name = "btnSelectPortrait";
            this.btnSelectPortrait.Size = new System.Drawing.Size(92, 23);
            this.btnSelectPortrait.TabIndex = 1;
            this.btnSelectPortrait.Text = "Select Portrait";
            this.btnSelectPortrait.UseVisualStyleBackColor = true;
            this.btnSelectPortrait.Click += new System.EventHandler(this.btnSelectPortrait_Click);
            // 
            // txtPortrait
            // 
            this.txtPortrait.Location = new System.Drawing.Point(6, 19);
            this.txtPortrait.Name = "txtPortrait";
            this.txtPortrait.ReadOnly = true;
            this.txtPortrait.Size = new System.Drawing.Size(192, 20);
            this.txtPortrait.TabIndex = 0;
            // 
            // gbBox
            // 
            this.gbBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBox.Controls.Add(this.btnRemoveBoxPortrait);
            this.gbBox.Controls.Add(this.btnAddBoxPortrait);
            this.gbBox.Controls.Add(this.lstBox);
            this.gbBox.Location = new System.Drawing.Point(160, 74);
            this.gbBox.Name = "gbBox";
            this.gbBox.Size = new System.Drawing.Size(148, 272);
            this.gbBox.TabIndex = 3;
            this.gbBox.TabStop = false;
            this.gbBox.Text = "Box";
            // 
            // btnRemoveBoxPortrait
            // 
            this.btnRemoveBoxPortrait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBoxPortrait.Location = new System.Drawing.Point(6, 243);
            this.btnRemoveBoxPortrait.Name = "btnRemoveBoxPortrait";
            this.btnRemoveBoxPortrait.Size = new System.Drawing.Size(136, 23);
            this.btnRemoveBoxPortrait.TabIndex = 2;
            this.btnRemoveBoxPortrait.Text = "Remove Portrait";
            this.btnRemoveBoxPortrait.UseVisualStyleBackColor = true;
            this.btnRemoveBoxPortrait.Click += new System.EventHandler(this.btnRemoveBoxPortrait_Click);
            // 
            // btnAddBoxPortrait
            // 
            this.btnAddBoxPortrait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBoxPortrait.Location = new System.Drawing.Point(6, 214);
            this.btnAddBoxPortrait.Name = "btnAddBoxPortrait";
            this.btnAddBoxPortrait.Size = new System.Drawing.Size(136, 23);
            this.btnAddBoxPortrait.TabIndex = 1;
            this.btnAddBoxPortrait.Text = "Add Portrait";
            this.btnAddBoxPortrait.UseVisualStyleBackColor = true;
            this.btnAddBoxPortrait.Click += new System.EventHandler(this.btnAddBoxPortrait_Click);
            // 
            // lstBox
            // 
            this.lstBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBox.FormattingEnabled = true;
            this.lstBox.Location = new System.Drawing.Point(6, 19);
            this.lstBox.Name = "lstBox";
            this.lstBox.Size = new System.Drawing.Size(136, 186);
            this.lstBox.TabIndex = 0;
            // 
            // gbBust
            // 
            this.gbBust.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbBust.Controls.Add(this.btnRemoveBustPortrait);
            this.gbBust.Controls.Add(this.btnAddBustPortrait);
            this.gbBust.Controls.Add(this.lstBust);
            this.gbBust.Location = new System.Drawing.Point(6, 74);
            this.gbBust.Name = "gbBust";
            this.gbBust.Size = new System.Drawing.Size(148, 272);
            this.gbBust.TabIndex = 1;
            this.gbBust.TabStop = false;
            this.gbBust.Text = "Bust";
            // 
            // btnRemoveBustPortrait
            // 
            this.btnRemoveBustPortrait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveBustPortrait.Location = new System.Drawing.Point(6, 243);
            this.btnRemoveBustPortrait.Name = "btnRemoveBustPortrait";
            this.btnRemoveBustPortrait.Size = new System.Drawing.Size(136, 23);
            this.btnRemoveBustPortrait.TabIndex = 2;
            this.btnRemoveBustPortrait.Text = "Remove Portrait";
            this.btnRemoveBustPortrait.UseVisualStyleBackColor = true;
            this.btnRemoveBustPortrait.Click += new System.EventHandler(this.btnRemoveBustPortrait_Click);
            // 
            // btnAddBustPortrait
            // 
            this.btnAddBustPortrait.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddBustPortrait.Location = new System.Drawing.Point(6, 214);
            this.btnAddBustPortrait.Name = "btnAddBustPortrait";
            this.btnAddBustPortrait.Size = new System.Drawing.Size(136, 23);
            this.btnAddBustPortrait.TabIndex = 1;
            this.btnAddBustPortrait.Text = "Add Portrait";
            this.btnAddBustPortrait.UseVisualStyleBackColor = true;
            this.btnAddBustPortrait.Click += new System.EventHandler(this.btnAddBustPortrait_Click);
            // 
            // lstBust
            // 
            this.lstBust.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstBust.FormattingEnabled = true;
            this.lstBust.Location = new System.Drawing.Point(6, 19);
            this.lstBust.Name = "lstBust";
            this.lstBust.Size = new System.Drawing.Size(136, 186);
            this.lstBust.TabIndex = 0;
            // 
            // gbTags
            // 
            this.gbTags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTags.Controls.Add(this.txtTags);
            this.gbTags.Location = new System.Drawing.Point(12, 370);
            this.gbTags.Name = "gbTags";
            this.gbTags.Size = new System.Drawing.Size(314, 70);
            this.gbTags.TabIndex = 4;
            this.gbTags.TabStop = false;
            this.gbTags.Text = "Tags";
            // 
            // txtTags
            // 
            this.txtTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTags.Location = new System.Drawing.Point(3, 16);
            this.txtTags.Multiline = true;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(308, 51);
            this.txtTags.TabIndex = 0;
            // 
            // DetailsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 445);
            this.Controls.Add(this.gbTags);
            this.Controls.Add(this.gbPortraits);
            this.Name = "DetailsEditor";
            this.Text = "Details";
            this.gbPortraits.ResumeLayout(false);
            this.gbPortrait.ResumeLayout(false);
            this.gbPortrait.PerformLayout();
            this.gbBox.ResumeLayout(false);
            this.gbBust.ResumeLayout(false);
            this.gbTags.ResumeLayout(false);
            this.gbTags.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbPortraits;
        private System.Windows.Forms.GroupBox gbTags;
        public System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.GroupBox gbBust;
        private System.Windows.Forms.Button btnRemoveBustPortrait;
        private System.Windows.Forms.Button btnAddBustPortrait;
        private System.Windows.Forms.GroupBox gbBox;
        private System.Windows.Forms.Button btnRemoveBoxPortrait;
        private System.Windows.Forms.Button btnAddBoxPortrait;
        public System.Windows.Forms.ListBox lstBust;
        public System.Windows.Forms.ListBox lstBox;
        private System.Windows.Forms.GroupBox gbPortrait;
        private System.Windows.Forms.Button btnSelectPortrait;
        public System.Windows.Forms.TextBox txtPortrait;
    }
}