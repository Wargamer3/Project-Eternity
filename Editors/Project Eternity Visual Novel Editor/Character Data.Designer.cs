namespace ProjectEternity.Editors.VisualNovelEditor
{
    partial class CharacterData
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
            this.gbSpeakerPriority = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnAddID = new System.Windows.Forms.Button();
            this.btnAddLocation = new System.Windows.Forms.Button();
            this.btnAddNewCharacter = new System.Windows.Forms.Button();
            this.lstSpeakerPriority = new System.Windows.Forms.ListBox();
            this.gbSpeakerPriority.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSpeakerPriority
            // 
            this.gbSpeakerPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSpeakerPriority.Controls.Add(this.btnDelete);
            this.gbSpeakerPriority.Controls.Add(this.btnMoveDown);
            this.gbSpeakerPriority.Controls.Add(this.btnMoveUp);
            this.gbSpeakerPriority.Controls.Add(this.btnAddID);
            this.gbSpeakerPriority.Controls.Add(this.btnAddLocation);
            this.gbSpeakerPriority.Controls.Add(this.btnAddNewCharacter);
            this.gbSpeakerPriority.Controls.Add(this.lstSpeakerPriority);
            this.gbSpeakerPriority.Location = new System.Drawing.Point(12, 12);
            this.gbSpeakerPriority.Name = "gbSpeakerPriority";
            this.gbSpeakerPriority.Size = new System.Drawing.Size(262, 209);
            this.gbSpeakerPriority.TabIndex = 0;
            this.gbSpeakerPriority.TabStop = false;
            this.gbSpeakerPriority.Text = "Speaker Priority";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(132, 178);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(120, 23);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(132, 149);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(120, 23);
            this.btnMoveDown.TabIndex = 6;
            this.btnMoveDown.Text = "Move down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(132, 120);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(120, 23);
            this.btnMoveUp.TabIndex = 5;
            this.btnMoveUp.Text = "Move up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnAddID
            // 
            this.btnAddID.Location = new System.Drawing.Point(6, 178);
            this.btnAddID.Name = "btnAddID";
            this.btnAddID.Size = new System.Drawing.Size(120, 23);
            this.btnAddID.TabIndex = 4;
            this.btnAddID.Text = "Add ID";
            this.btnAddID.UseVisualStyleBackColor = true;
            this.btnAddID.Click += new System.EventHandler(this.btnAddID_Click);
            // 
            // btnAddLocation
            // 
            this.btnAddLocation.Location = new System.Drawing.Point(6, 149);
            this.btnAddLocation.Name = "btnAddLocation";
            this.btnAddLocation.Size = new System.Drawing.Size(120, 23);
            this.btnAddLocation.TabIndex = 3;
            this.btnAddLocation.Text = "Add location";
            this.btnAddLocation.UseVisualStyleBackColor = true;
            this.btnAddLocation.Click += new System.EventHandler(this.btnAddLocation_Click);
            // 
            // btnAddNewCharacter
            // 
            this.btnAddNewCharacter.Location = new System.Drawing.Point(6, 120);
            this.btnAddNewCharacter.Name = "btnAddNewCharacter";
            this.btnAddNewCharacter.Size = new System.Drawing.Size(120, 23);
            this.btnAddNewCharacter.TabIndex = 2;
            this.btnAddNewCharacter.Text = "Add new character";
            this.btnAddNewCharacter.UseVisualStyleBackColor = true;
            this.btnAddNewCharacter.Click += new System.EventHandler(this.btnAddNewCharacter_Click);
            // 
            // lstSpeakerPriority
            // 
            this.lstSpeakerPriority.FormattingEnabled = true;
            this.lstSpeakerPriority.Location = new System.Drawing.Point(6, 19);
            this.lstSpeakerPriority.Name = "lstSpeakerPriority";
            this.lstSpeakerPriority.Size = new System.Drawing.Size(120, 95);
            this.lstSpeakerPriority.TabIndex = 1;
            // 
            // CharacterData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(286, 233);
            this.Controls.Add(this.gbSpeakerPriority);
            this.Name = "CharacterData";
            this.Text = "Character Data";
            this.gbSpeakerPriority.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSpeakerPriority;
        private System.Windows.Forms.ListBox lstSpeakerPriority;
        private System.Windows.Forms.Button btnAddNewCharacter;
        private System.Windows.Forms.Button btnAddLocation;
        private System.Windows.Forms.Button btnAddID;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnDelete;
    }
}