namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    partial class CharacterQuotesEditor
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
            this.gbQuotes = new System.Windows.Forms.GroupBox();
            this.btnRemoveQuote = new System.Windows.Forms.Button();
            this.btnAddQuote = new System.Windows.Forms.Button();
            this.lstQuotes = new System.Windows.Forms.ListBox();
            this.gbQuoteEditor = new System.Windows.Forms.GroupBox();
            this.txtPortraitPath = new System.Windows.Forms.TextBox();
            this.btnSelectPortrait = new System.Windows.Forms.Button();
            this.txtQuoteEditor = new System.Windows.Forms.TextBox();
            this.gbBaseQuotes = new System.Windows.Forms.GroupBox();
            this.lvBaseQuotes = new System.Windows.Forms.ListView();
            this.gbVersusQuotes = new System.Windows.Forms.GroupBox();
            this.lsVersusQuotes = new System.Windows.Forms.ListBox();
            this.btnDeleteVersusQuote = new System.Windows.Forms.Button();
            this.btnAddVersusQuote = new System.Windows.Forms.Button();
            this.gbMapQuotes = new System.Windows.Forms.GroupBox();
            this.lsMapQuotes = new System.Windows.Forms.ListBox();
            this.btnAddBatchQuote = new System.Windows.Forms.Button();
            this.gbQuotes.SuspendLayout();
            this.gbQuoteEditor.SuspendLayout();
            this.gbBaseQuotes.SuspendLayout();
            this.gbVersusQuotes.SuspendLayout();
            this.gbMapQuotes.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbQuotes
            // 
            this.gbQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbQuotes.Controls.Add(this.btnAddBatchQuote);
            this.gbQuotes.Controls.Add(this.btnRemoveQuote);
            this.gbQuotes.Controls.Add(this.btnAddQuote);
            this.gbQuotes.Controls.Add(this.lstQuotes);
            this.gbQuotes.Location = new System.Drawing.Point(360, 12);
            this.gbQuotes.Name = "gbQuotes";
            this.gbQuotes.Size = new System.Drawing.Size(260, 201);
            this.gbQuotes.TabIndex = 2;
            this.gbQuotes.TabStop = false;
            this.gbQuotes.Text = "Quotes";
            // 
            // btnRemoveQuote
            // 
            this.btnRemoveQuote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveQuote.Location = new System.Drawing.Point(171, 172);
            this.btnRemoveQuote.Name = "btnRemoveQuote";
            this.btnRemoveQuote.Size = new System.Drawing.Size(89, 23);
            this.btnRemoveQuote.TabIndex = 4;
            this.btnRemoveQuote.Text = "Remove quote";
            this.btnRemoveQuote.UseVisualStyleBackColor = true;
            this.btnRemoveQuote.Click += new System.EventHandler(this.btnRemoveQuote_Click);
            // 
            // btnAddQuote
            // 
            this.btnAddQuote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddQuote.Location = new System.Drawing.Point(6, 172);
            this.btnAddQuote.Name = "btnAddQuote";
            this.btnAddQuote.Size = new System.Drawing.Size(72, 23);
            this.btnAddQuote.TabIndex = 3;
            this.btnAddQuote.Text = "Add quote";
            this.btnAddQuote.UseVisualStyleBackColor = true;
            this.btnAddQuote.Click += new System.EventHandler(this.btnAddQuote_Click);
            // 
            // lstQuotes
            // 
            this.lstQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstQuotes.FormattingEnabled = true;
            this.lstQuotes.Location = new System.Drawing.Point(6, 19);
            this.lstQuotes.Name = "lstQuotes";
            this.lstQuotes.Size = new System.Drawing.Size(254, 147);
            this.lstQuotes.TabIndex = 0;
            this.lstQuotes.SelectedIndexChanged += new System.EventHandler(this.lstQuotes_SelectedIndexChanged);
            // 
            // gbQuoteEditor
            // 
            this.gbQuoteEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbQuoteEditor.Controls.Add(this.txtPortraitPath);
            this.gbQuoteEditor.Controls.Add(this.btnSelectPortrait);
            this.gbQuoteEditor.Controls.Add(this.txtQuoteEditor);
            this.gbQuoteEditor.Location = new System.Drawing.Point(360, 219);
            this.gbQuoteEditor.Name = "gbQuoteEditor";
            this.gbQuoteEditor.Size = new System.Drawing.Size(260, 148);
            this.gbQuoteEditor.TabIndex = 3;
            this.gbQuoteEditor.TabStop = false;
            this.gbQuoteEditor.Text = "Quote editor";
            // 
            // txtPortraitPath
            // 
            this.txtPortraitPath.Location = new System.Drawing.Point(106, 122);
            this.txtPortraitPath.Name = "txtPortraitPath";
            this.txtPortraitPath.ReadOnly = true;
            this.txtPortraitPath.Size = new System.Drawing.Size(154, 20);
            this.txtPortraitPath.TabIndex = 11;
            // 
            // btnSelectPortrait
            // 
            this.btnSelectPortrait.Location = new System.Drawing.Point(6, 119);
            this.btnSelectPortrait.Name = "btnSelectPortrait";
            this.btnSelectPortrait.Size = new System.Drawing.Size(94, 23);
            this.btnSelectPortrait.TabIndex = 10;
            this.btnSelectPortrait.Text = "Select Portait";
            this.btnSelectPortrait.UseVisualStyleBackColor = true;
            this.btnSelectPortrait.Click += new System.EventHandler(this.btnSelectPortrait_Click);
            // 
            // txtQuoteEditor
            // 
            this.txtQuoteEditor.Enabled = false;
            this.txtQuoteEditor.Location = new System.Drawing.Point(6, 19);
            this.txtQuoteEditor.Multiline = true;
            this.txtQuoteEditor.Name = "txtQuoteEditor";
            this.txtQuoteEditor.Size = new System.Drawing.Size(254, 97);
            this.txtQuoteEditor.TabIndex = 4;
            this.txtQuoteEditor.TextChanged += new System.EventHandler(this.txtQuoteEditor_TextChanged);
            // 
            // gbBaseQuotes
            // 
            this.gbBaseQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbBaseQuotes.Controls.Add(this.lvBaseQuotes);
            this.gbBaseQuotes.Location = new System.Drawing.Point(12, 12);
            this.gbBaseQuotes.Name = "gbBaseQuotes";
            this.gbBaseQuotes.Size = new System.Drawing.Size(204, 355);
            this.gbBaseQuotes.TabIndex = 8;
            this.gbBaseQuotes.TabStop = false;
            this.gbBaseQuotes.Text = "Base Quotes";
            // 
            // lvBaseQuotes
            // 
            this.lvBaseQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lvBaseQuotes.HideSelection = false;
            this.lvBaseQuotes.Location = new System.Drawing.Point(6, 19);
            this.lvBaseQuotes.MultiSelect = false;
            this.lvBaseQuotes.Name = "lvBaseQuotes";
            this.lvBaseQuotes.Size = new System.Drawing.Size(192, 330);
            this.lvBaseQuotes.TabIndex = 7;
            this.lvBaseQuotes.UseCompatibleStateImageBehavior = false;
            this.lvBaseQuotes.View = System.Windows.Forms.View.List;
            this.lvBaseQuotes.SelectedIndexChanged += new System.EventHandler(this.lvBaseQuotes_SelectedIndexChanged);
            // 
            // gbVersusQuotes
            // 
            this.gbVersusQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gbVersusQuotes.Controls.Add(this.lsVersusQuotes);
            this.gbVersusQuotes.Controls.Add(this.btnDeleteVersusQuote);
            this.gbVersusQuotes.Controls.Add(this.btnAddVersusQuote);
            this.gbVersusQuotes.Location = new System.Drawing.Point(222, 142);
            this.gbVersusQuotes.Name = "gbVersusQuotes";
            this.gbVersusQuotes.Size = new System.Drawing.Size(132, 225);
            this.gbVersusQuotes.TabIndex = 10;
            this.gbVersusQuotes.TabStop = false;
            this.gbVersusQuotes.Text = "Versus Quotes";
            // 
            // lsVersusQuotes
            // 
            this.lsVersusQuotes.FormattingEnabled = true;
            this.lsVersusQuotes.Items.AddRange(new object[] {
            "None"});
            this.lsVersusQuotes.Location = new System.Drawing.Point(6, 19);
            this.lsVersusQuotes.Name = "lsVersusQuotes";
            this.lsVersusQuotes.Size = new System.Drawing.Size(120, 134);
            this.lsVersusQuotes.TabIndex = 7;
            // 
            // btnDeleteVersusQuote
            // 
            this.btnDeleteVersusQuote.Location = new System.Drawing.Point(6, 191);
            this.btnDeleteVersusQuote.Name = "btnDeleteVersusQuote";
            this.btnDeleteVersusQuote.Size = new System.Drawing.Size(120, 23);
            this.btnDeleteVersusQuote.TabIndex = 6;
            this.btnDeleteVersusQuote.Text = "Delete versus quote";
            this.btnDeleteVersusQuote.UseVisualStyleBackColor = true;
            // 
            // btnAddVersusQuote
            // 
            this.btnAddVersusQuote.Location = new System.Drawing.Point(6, 162);
            this.btnAddVersusQuote.Name = "btnAddVersusQuote";
            this.btnAddVersusQuote.Size = new System.Drawing.Size(120, 23);
            this.btnAddVersusQuote.TabIndex = 5;
            this.btnAddVersusQuote.Text = "Add versus quote";
            this.btnAddVersusQuote.UseVisualStyleBackColor = true;
            this.btnAddVersusQuote.Click += new System.EventHandler(this.btnAddVersusQuote_Click_1);
            // 
            // gbMapQuotes
            // 
            this.gbMapQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbMapQuotes.Controls.Add(this.lsMapQuotes);
            this.gbMapQuotes.Location = new System.Drawing.Point(222, 12);
            this.gbMapQuotes.Name = "gbMapQuotes";
            this.gbMapQuotes.Size = new System.Drawing.Size(132, 130);
            this.gbMapQuotes.TabIndex = 9;
            this.gbMapQuotes.TabStop = false;
            this.gbMapQuotes.Text = "Map Quotes";
            // 
            // lsMapQuotes
            // 
            this.lsMapQuotes.FormattingEnabled = true;
            this.lsMapQuotes.Items.AddRange(new object[] {
            "Any"});
            this.lsMapQuotes.Location = new System.Drawing.Point(6, 19);
            this.lsMapQuotes.Name = "lsMapQuotes";
            this.lsMapQuotes.Size = new System.Drawing.Size(120, 108);
            this.lsMapQuotes.TabIndex = 8;
            // 
            // btnAddBatchQuote
            // 
            this.btnAddBatchQuote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddBatchQuote.Location = new System.Drawing.Point(84, 172);
            this.btnAddBatchQuote.Name = "btnAddBatchQuote";
            this.btnAddBatchQuote.Size = new System.Drawing.Size(81, 23);
            this.btnAddBatchQuote.TabIndex = 5;
            this.btnAddBatchQuote.Text = "Add batch";
            this.btnAddBatchQuote.UseVisualStyleBackColor = true;
            this.btnAddBatchQuote.Click += new System.EventHandler(this.btnAddBatchQuote_Click);
            // 
            // CharacterQuotesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 379);
            this.Controls.Add(this.gbVersusQuotes);
            this.Controls.Add(this.gbMapQuotes);
            this.Controls.Add(this.gbBaseQuotes);
            this.Controls.Add(this.gbQuoteEditor);
            this.Controls.Add(this.gbQuotes);
            this.Name = "CharacterQuotesEditor";
            this.Text = "Character Quotes Editor";
            this.gbQuotes.ResumeLayout(false);
            this.gbQuoteEditor.ResumeLayout(false);
            this.gbQuoteEditor.PerformLayout();
            this.gbBaseQuotes.ResumeLayout(false);
            this.gbVersusQuotes.ResumeLayout(false);
            this.gbMapQuotes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbQuotes;
        private System.Windows.Forms.ListBox lstQuotes;
        private System.Windows.Forms.GroupBox gbQuoteEditor;
        private System.Windows.Forms.TextBox txtQuoteEditor;
        private System.Windows.Forms.Button btnRemoveQuote;
        private System.Windows.Forms.Button btnAddQuote;
        private System.Windows.Forms.GroupBox gbBaseQuotes;
        private System.Windows.Forms.TextBox txtPortraitPath;
        private System.Windows.Forms.Button btnSelectPortrait;
        public System.Windows.Forms.ListView lvBaseQuotes;
        private System.Windows.Forms.GroupBox gbVersusQuotes;
        public System.Windows.Forms.ListBox lsVersusQuotes;
        private System.Windows.Forms.Button btnDeleteVersusQuote;
        private System.Windows.Forms.Button btnAddVersusQuote;
        private System.Windows.Forms.GroupBox gbMapQuotes;
        public System.Windows.Forms.ListBox lsMapQuotes;
        private System.Windows.Forms.Button btnAddBatchQuote;
    }
}