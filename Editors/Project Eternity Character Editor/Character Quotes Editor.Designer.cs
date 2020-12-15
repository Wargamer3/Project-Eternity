namespace ProjectEternity.Editors.CharacterEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lvBaseQuotes = new System.Windows.Forms.ListView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemoveQuote = new System.Windows.Forms.Button();
            this.btnAddQuote = new System.Windows.Forms.Button();
            this.lstQuotes = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtPortraitPath = new System.Windows.Forms.TextBox();
            this.btnSelectPortrait = new System.Windows.Forms.Button();
            this.txtQuoteEditor = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lsVersusQuotes = new System.Windows.Forms.ListBox();
            this.btnDeleteVersusQuote = new System.Windows.Forms.Button();
            this.btnAddVersusQuote = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.dgvQuoteSets = new System.Windows.Forms.DataGridView();
            this.clQuoteSetName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuoteSets)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lvBaseQuotes);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(132, 130);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Base quotes";
            // 
            // lvBaseQuotes
            // 
            this.lvBaseQuotes.HideSelection = false;
            this.lvBaseQuotes.Location = new System.Drawing.Point(6, 19);
            this.lvBaseQuotes.MultiSelect = false;
            this.lvBaseQuotes.Name = "lvBaseQuotes";
            this.lvBaseQuotes.Size = new System.Drawing.Size(120, 105);
            this.lvBaseQuotes.TabIndex = 7;
            this.lvBaseQuotes.UseCompatibleStateImageBehavior = false;
            this.lvBaseQuotes.View = System.Windows.Forms.View.List;
            this.lvBaseQuotes.SelectedIndexChanged += new System.EventHandler(this.lvBaseQuotes_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRemoveQuote);
            this.groupBox2.Controls.Add(this.btnAddQuote);
            this.groupBox2.Controls.Add(this.lstQuotes);
            this.groupBox2.Location = new System.Drawing.Point(354, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 201);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Quotes";
            // 
            // btnRemoveQuote
            // 
            this.btnRemoveQuote.Location = new System.Drawing.Point(140, 172);
            this.btnRemoveQuote.Name = "btnRemoveQuote";
            this.btnRemoveQuote.Size = new System.Drawing.Size(120, 23);
            this.btnRemoveQuote.TabIndex = 4;
            this.btnRemoveQuote.Text = "Remove quote";
            this.btnRemoveQuote.UseVisualStyleBackColor = true;
            this.btnRemoveQuote.Click += new System.EventHandler(this.btnRemoveQuote_Click);
            // 
            // btnAddQuote
            // 
            this.btnAddQuote.Location = new System.Drawing.Point(6, 172);
            this.btnAddQuote.Name = "btnAddQuote";
            this.btnAddQuote.Size = new System.Drawing.Size(120, 23);
            this.btnAddQuote.TabIndex = 3;
            this.btnAddQuote.Text = "Add quote";
            this.btnAddQuote.UseVisualStyleBackColor = true;
            this.btnAddQuote.Click += new System.EventHandler(this.btnAddQuote_Click);
            // 
            // lstQuotes
            // 
            this.lstQuotes.FormattingEnabled = true;
            this.lstQuotes.Location = new System.Drawing.Point(6, 19);
            this.lstQuotes.Name = "lstQuotes";
            this.lstQuotes.Size = new System.Drawing.Size(254, 147);
            this.lstQuotes.TabIndex = 0;
            this.lstQuotes.SelectedIndexChanged += new System.EventHandler(this.lstQuotes_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtPortraitPath);
            this.groupBox3.Controls.Add(this.btnSelectPortrait);
            this.groupBox3.Controls.Add(this.txtQuoteEditor);
            this.groupBox3.Location = new System.Drawing.Point(354, 219);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(266, 148);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Quote editor";
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
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lsVersusQuotes);
            this.groupBox4.Controls.Add(this.btnDeleteVersusQuote);
            this.groupBox4.Controls.Add(this.btnAddVersusQuote);
            this.groupBox4.Location = new System.Drawing.Point(12, 142);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(132, 222);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Versus quotes";
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
            this.lsVersusQuotes.SelectedIndexChanged += new System.EventHandler(this.lsVersusQuotes_SelectedIndexChanged);
            // 
            // btnDeleteVersusQuote
            // 
            this.btnDeleteVersusQuote.Location = new System.Drawing.Point(6, 191);
            this.btnDeleteVersusQuote.Name = "btnDeleteVersusQuote";
            this.btnDeleteVersusQuote.Size = new System.Drawing.Size(120, 23);
            this.btnDeleteVersusQuote.TabIndex = 6;
            this.btnDeleteVersusQuote.Text = "Delete versus quote";
            this.btnDeleteVersusQuote.UseVisualStyleBackColor = true;
            this.btnDeleteVersusQuote.Click += new System.EventHandler(this.btnDeleteVersusQuote_Click);
            // 
            // btnAddVersusQuote
            // 
            this.btnAddVersusQuote.Location = new System.Drawing.Point(6, 162);
            this.btnAddVersusQuote.Name = "btnAddVersusQuote";
            this.btnAddVersusQuote.Size = new System.Drawing.Size(120, 23);
            this.btnAddVersusQuote.TabIndex = 5;
            this.btnAddVersusQuote.Text = "Add versus quote";
            this.btnAddVersusQuote.UseVisualStyleBackColor = true;
            this.btnAddVersusQuote.Click += new System.EventHandler(this.btnAddVersusQuote_Click);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.dgvQuoteSets);
            this.groupBox7.Location = new System.Drawing.Point(144, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(204, 352);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Quote sets";
            // 
            // dgvQuoteSets
            // 
            this.dgvQuoteSets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuoteSets.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clQuoteSetName});
            this.dgvQuoteSets.Location = new System.Drawing.Point(6, 19);
            this.dgvQuoteSets.MultiSelect = false;
            this.dgvQuoteSets.Name = "dgvQuoteSets";
            this.dgvQuoteSets.RowHeadersVisible = false;
            this.dgvQuoteSets.Size = new System.Drawing.Size(192, 325);
            this.dgvQuoteSets.TabIndex = 0;
            this.dgvQuoteSets.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvQuoteSets_CellClick);
            this.dgvQuoteSets.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvQuoteSets_UserAddedRow);
            this.dgvQuoteSets.Click += new System.EventHandler(this.dgvQuoteSets_Click);
            // 
            // clQuoteSetName
            // 
            this.clQuoteSetName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clQuoteSetName.HeaderText = "Name";
            this.clQuoteSetName.Name = "clQuoteSetName";
            // 
            // CharacterQuotesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 379);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "CharacterQuotesEditor";
            this.Text = "Character Quotes Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuoteSets)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox lstQuotes;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtQuoteEditor;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnDeleteVersusQuote;
        private System.Windows.Forms.Button btnAddVersusQuote;
        private System.Windows.Forms.Button btnRemoveQuote;
        private System.Windows.Forms.Button btnAddQuote;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.DataGridViewTextBoxColumn clQuoteSetName;
        public System.Windows.Forms.ListView lvBaseQuotes;
        public System.Windows.Forms.DataGridView dgvQuoteSets;
        public System.Windows.Forms.ListBox lsVersusQuotes;
        private System.Windows.Forms.TextBox txtPortraitPath;
        private System.Windows.Forms.Button btnSelectPortrait;
    }
}