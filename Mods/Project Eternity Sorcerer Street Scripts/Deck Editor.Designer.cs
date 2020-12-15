namespace ProjectEternity.GameScreens.SorcererStreetScreen
{
    partial class DeckEditor
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
            this.gbCards = new System.Windows.Forms.GroupBox();
            this.lstCards = new System.Windows.Forms.ListBox();
            this.btnAddCard = new System.Windows.Forms.Button();
            this.btnRemoveCard = new System.Windows.Forms.Button();
            this.gbCards.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCards
            // 
            this.gbCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbCards.Controls.Add(this.btnRemoveCard);
            this.gbCards.Controls.Add(this.btnAddCard);
            this.gbCards.Controls.Add(this.lstCards);
            this.gbCards.Location = new System.Drawing.Point(12, 12);
            this.gbCards.Name = "gbCards";
            this.gbCards.Size = new System.Drawing.Size(173, 260);
            this.gbCards.TabIndex = 0;
            this.gbCards.TabStop = false;
            this.gbCards.Text = "Cards";
            // 
            // lstCards
            // 
            this.lstCards.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCards.FormattingEnabled = true;
            this.lstCards.Location = new System.Drawing.Point(6, 19);
            this.lstCards.Name = "lstCards";
            this.lstCards.Size = new System.Drawing.Size(161, 173);
            this.lstCards.TabIndex = 0;
            // 
            // btnAddCard
            // 
            this.btnAddCard.Location = new System.Drawing.Point(6, 198);
            this.btnAddCard.Name = "btnAddCard";
            this.btnAddCard.Size = new System.Drawing.Size(161, 23);
            this.btnAddCard.TabIndex = 1;
            this.btnAddCard.Text = "Add Card";
            this.btnAddCard.UseVisualStyleBackColor = true;
            this.btnAddCard.Click += new System.EventHandler(this.btnAddCard_Click);
            // 
            // btnRemoveCard
            // 
            this.btnRemoveCard.Location = new System.Drawing.Point(6, 231);
            this.btnRemoveCard.Name = "btnRemoveCard";
            this.btnRemoveCard.Size = new System.Drawing.Size(161, 23);
            this.btnRemoveCard.TabIndex = 2;
            this.btnRemoveCard.Text = "Remove Card";
            this.btnRemoveCard.UseVisualStyleBackColor = true;
            this.btnRemoveCard.Click += new System.EventHandler(this.btnRemoveCard_Click);
            // 
            // DeckEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(197, 284);
            this.Controls.Add(this.gbCards);
            this.Name = "DeckEditor";
            this.Text = "Deck Editor";
            this.gbCards.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCards;
        private System.Windows.Forms.Button btnRemoveCard;
        private System.Windows.Forms.Button btnAddCard;
        private System.Windows.Forms.ListBox lstCards;
    }
}