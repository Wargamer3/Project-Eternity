namespace ProjectEternity.Editors.CharacterEditor
{
    partial class RelationshipEditor
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
            this.gbCharacters = new System.Windows.Forms.GroupBox();
            this.btnRemoveCharacter = new System.Windows.Forms.Button();
            this.btnAddCharacter = new System.Windows.Forms.Button();
            this.lstCharacters = new System.Windows.Forms.ListBox();
            this.gbRelationship = new System.Windows.Forms.GroupBox();
            this.lblRelationshipLevel = new System.Windows.Forms.Label();
            this.txtRelationshipLevel = new System.Windows.Forms.NumericUpDown();
            this.btnSelectRelationshipBonus = new System.Windows.Forms.Button();
            this.lblRelationshipBonus = new System.Windows.Forms.Label();
            this.txtRelationshipBonus = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.gbRequirement = new System.Windows.Forms.GroupBox();
            this.cboRequirementType = new System.Windows.Forms.ComboBox();
            this.pgRequirement = new System.Windows.Forms.PropertyGrid();
            this.gbCharacters.SuspendLayout();
            this.gbRelationship.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelationshipLevel)).BeginInit();
            this.gbRequirement.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbCharacters
            // 
            this.gbCharacters.Controls.Add(this.btnRemoveCharacter);
            this.gbCharacters.Controls.Add(this.btnAddCharacter);
            this.gbCharacters.Controls.Add(this.lstCharacters);
            this.gbCharacters.Location = new System.Drawing.Point(12, 12);
            this.gbCharacters.Name = "gbCharacters";
            this.gbCharacters.Size = new System.Drawing.Size(168, 177);
            this.gbCharacters.TabIndex = 0;
            this.gbCharacters.TabStop = false;
            this.gbCharacters.Text = "Characters";
            // 
            // btnRemoveCharacter
            // 
            this.btnRemoveCharacter.Location = new System.Drawing.Point(87, 146);
            this.btnRemoveCharacter.Name = "btnRemoveCharacter";
            this.btnRemoveCharacter.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveCharacter.TabIndex = 2;
            this.btnRemoveCharacter.Text = "Remove";
            this.btnRemoveCharacter.UseVisualStyleBackColor = true;
            this.btnRemoveCharacter.Click += new System.EventHandler(this.btnRemoveCharacter_Click);
            // 
            // btnAddCharacter
            // 
            this.btnAddCharacter.Location = new System.Drawing.Point(6, 146);
            this.btnAddCharacter.Name = "btnAddCharacter";
            this.btnAddCharacter.Size = new System.Drawing.Size(75, 23);
            this.btnAddCharacter.TabIndex = 1;
            this.btnAddCharacter.Text = "Add";
            this.btnAddCharacter.UseVisualStyleBackColor = true;
            this.btnAddCharacter.Click += new System.EventHandler(this.btnAddCharacter_Click);
            // 
            // lstCharacters
            // 
            this.lstCharacters.FormattingEnabled = true;
            this.lstCharacters.Location = new System.Drawing.Point(6, 19);
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.Size = new System.Drawing.Size(156, 121);
            this.lstCharacters.TabIndex = 1;
            this.lstCharacters.SelectedIndexChanged += new System.EventHandler(this.lstCharacters_SelectedIndexChanged);
            // 
            // gbRelationship
            // 
            this.gbRelationship.Controls.Add(this.lblRelationshipLevel);
            this.gbRelationship.Controls.Add(this.txtRelationshipLevel);
            this.gbRelationship.Controls.Add(this.btnSelectRelationshipBonus);
            this.gbRelationship.Controls.Add(this.lblRelationshipBonus);
            this.gbRelationship.Controls.Add(this.txtRelationshipBonus);
            this.gbRelationship.Enabled = false;
            this.gbRelationship.Location = new System.Drawing.Point(186, 12);
            this.gbRelationship.Name = "gbRelationship";
            this.gbRelationship.Size = new System.Drawing.Size(200, 140);
            this.gbRelationship.TabIndex = 1;
            this.gbRelationship.TabStop = false;
            this.gbRelationship.Text = "Relationship";
            // 
            // lblRelationshipLevel
            // 
            this.lblRelationshipLevel.AutoSize = true;
            this.lblRelationshipLevel.Location = new System.Drawing.Point(6, 92);
            this.lblRelationshipLevel.Name = "lblRelationshipLevel";
            this.lblRelationshipLevel.Size = new System.Drawing.Size(97, 13);
            this.lblRelationshipLevel.TabIndex = 4;
            this.lblRelationshipLevel.Text = "Relationship Level:";
            // 
            // txtRelationshipLevel
            // 
            this.txtRelationshipLevel.Location = new System.Drawing.Point(6, 108);
            this.txtRelationshipLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtRelationshipLevel.Name = "txtRelationshipLevel";
            this.txtRelationshipLevel.Size = new System.Drawing.Size(120, 20);
            this.txtRelationshipLevel.TabIndex = 3;
            this.txtRelationshipLevel.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtRelationshipLevel.ValueChanged += new System.EventHandler(this.txtRelationshipLevel_ValueChanged);
            // 
            // btnSelectRelationshipBonus
            // 
            this.btnSelectRelationshipBonus.Location = new System.Drawing.Point(6, 66);
            this.btnSelectRelationshipBonus.Name = "btnSelectRelationshipBonus";
            this.btnSelectRelationshipBonus.Size = new System.Drawing.Size(188, 23);
            this.btnSelectRelationshipBonus.TabIndex = 2;
            this.btnSelectRelationshipBonus.Text = "Select bonus";
            this.btnSelectRelationshipBonus.UseVisualStyleBackColor = true;
            this.btnSelectRelationshipBonus.Click += new System.EventHandler(this.btnSelectRelationshipBonus_Click);
            // 
            // lblRelationshipBonus
            // 
            this.lblRelationshipBonus.AutoSize = true;
            this.lblRelationshipBonus.Location = new System.Drawing.Point(6, 19);
            this.lblRelationshipBonus.Name = "lblRelationshipBonus";
            this.lblRelationshipBonus.Size = new System.Drawing.Size(101, 13);
            this.lblRelationshipBonus.TabIndex = 1;
            this.lblRelationshipBonus.Text = "Relationship Bonus:";
            // 
            // txtRelationshipBonus
            // 
            this.txtRelationshipBonus.Location = new System.Drawing.Point(6, 40);
            this.txtRelationshipBonus.Name = "txtRelationshipBonus";
            this.txtRelationshipBonus.ReadOnly = true;
            this.txtRelationshipBonus.Size = new System.Drawing.Size(188, 20);
            this.txtRelationshipBonus.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(500, 195);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(419, 195);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 3;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // gbRequirement
            // 
            this.gbRequirement.Controls.Add(this.cboRequirementType);
            this.gbRequirement.Controls.Add(this.pgRequirement);
            this.gbRequirement.Enabled = false;
            this.gbRequirement.Location = new System.Drawing.Point(392, 12);
            this.gbRequirement.Name = "gbRequirement";
            this.gbRequirement.Size = new System.Drawing.Size(183, 177);
            this.gbRequirement.TabIndex = 17;
            this.gbRequirement.TabStop = false;
            this.gbRequirement.Text = "Requirement";
            // 
            // cboRequirementType
            // 
            this.cboRequirementType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRequirementType.FormattingEnabled = true;
            this.cboRequirementType.Location = new System.Drawing.Point(6, 19);
            this.cboRequirementType.Name = "cboRequirementType";
            this.cboRequirementType.Size = new System.Drawing.Size(171, 21);
            this.cboRequirementType.TabIndex = 3;
            this.cboRequirementType.SelectedIndexChanged += new System.EventHandler(this.cboRequirementType_SelectedIndexChanged);
            // 
            // pgRequirement
            // 
            this.pgRequirement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgRequirement.Location = new System.Drawing.Point(6, 46);
            this.pgRequirement.Name = "pgRequirement";
            this.pgRequirement.Size = new System.Drawing.Size(171, 125);
            this.pgRequirement.TabIndex = 2;
            this.pgRequirement.ToolbarVisible = false;
            // 
            // RelationshipEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 227);
            this.Controls.Add(this.gbRequirement);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbRelationship);
            this.Controls.Add(this.gbCharacters);
            this.Name = "RelationshipEditor";
            this.Text = "Relationship Editor";
            this.gbCharacters.ResumeLayout(false);
            this.gbRelationship.ResumeLayout(false);
            this.gbRelationship.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRelationshipLevel)).EndInit();
            this.gbRequirement.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbCharacters;
        private System.Windows.Forms.Button btnRemoveCharacter;
        private System.Windows.Forms.Button btnAddCharacter;
        private System.Windows.Forms.ListBox lstCharacters;
        private System.Windows.Forms.GroupBox gbRelationship;
        private System.Windows.Forms.Label lblRelationshipLevel;
        private System.Windows.Forms.NumericUpDown txtRelationshipLevel;
        private System.Windows.Forms.Button btnSelectRelationshipBonus;
        private System.Windows.Forms.Label lblRelationshipBonus;
        private System.Windows.Forms.TextBox txtRelationshipBonus;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.GroupBox gbRequirement;
        private System.Windows.Forms.ComboBox cboRequirementType;
        private System.Windows.Forms.PropertyGrid pgRequirement;
    }
}