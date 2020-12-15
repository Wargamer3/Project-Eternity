namespace ProjectEternity.Editors.AttackEditor
{
    partial class QuoteEditor
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvQuotes = new System.Windows.Forms.DataGridView();
            this.cmsDataGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmInsertLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDeleteLine = new System.Windows.Forms.ToolStripMenuItem();
            this.clQuoteSet = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuotes)).BeginInit();
            this.cmsDataGridView.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.dgvQuotes);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(592, 267);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Quotes";
            // 
            // dgvQuotes
            // 
            this.dgvQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvQuotes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuotes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clQuoteSet});
            this.dgvQuotes.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvQuotes.Location = new System.Drawing.Point(6, 19);
            this.dgvQuotes.MultiSelect = false;
            this.dgvQuotes.Name = "dgvQuotes";
            this.dgvQuotes.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvQuotes.Size = new System.Drawing.Size(580, 242);
            this.dgvQuotes.TabIndex = 3;
            this.dgvQuotes.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvQuotes_RowHeaderMouseClick);
            // 
            // cmsDataGridView
            // 
            this.cmsDataGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmInsertLine,
            this.tsmDeleteLine});
            this.cmsDataGridView.Name = "cmsDataGridView";
            this.cmsDataGridView.Size = new System.Drawing.Size(130, 48);
            this.cmsDataGridView.Opening += new System.ComponentModel.CancelEventHandler(this.cmsDataGridView_Opening);
            // 
            // tsmInsertLine
            // 
            this.tsmInsertLine.Name = "tsmInsertLine";
            this.tsmInsertLine.Size = new System.Drawing.Size(129, 22);
            this.tsmInsertLine.Text = "Insert line";
            this.tsmInsertLine.Click += new System.EventHandler(this.tsmInsertLine_Click);
            // 
            // tsmDeleteLine
            // 
            this.tsmDeleteLine.Name = "tsmDeleteLine";
            this.tsmDeleteLine.Size = new System.Drawing.Size(129, 22);
            this.tsmDeleteLine.Text = "Delete line";
            this.tsmDeleteLine.Click += new System.EventHandler(this.tsmDeleteLine_Click);
            // 
            // clQuoteSet
            // 
            this.clQuoteSet.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clQuoteSet.HeaderText = "Quote set";
            this.clQuoteSet.Name = "clQuoteSet";
            // 
            // QuoteEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(616, 291);
            this.Controls.Add(this.groupBox1);
            this.Name = "QuoteEditor";
            this.Text = "Quote Editor";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuotes)).EndInit();
            this.cmsDataGridView.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip cmsDataGridView;
        private System.Windows.Forms.ToolStripMenuItem tsmInsertLine;
        private System.Windows.Forms.ToolStripMenuItem tsmDeleteLine;
        public System.Windows.Forms.DataGridView dgvQuotes;
        private System.Windows.Forms.DataGridViewTextBoxColumn clQuoteSet;
    }
}