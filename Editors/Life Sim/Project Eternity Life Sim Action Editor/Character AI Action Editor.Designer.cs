namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class AIActionEditor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.gbAIActions = new System.Windows.Forms.GroupBox();
            this.cbAIActionType = new System.Windows.Forms.ComboBox();
            this.lblAIActionType = new System.Windows.Forms.Label();
            this.pgAIAction = new System.Windows.Forms.PropertyGrid();
            this.btnDeleteAIAction = new System.Windows.Forms.Button();
            this.lsAIActions = new System.Windows.Forms.ListBox();
            this.btnAddAIAction = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.gbAIActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(536, 24);
            this.menuStrip1.TabIndex = 66;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbItemInformation
            // 
            this.gbItemInformation.Controls.Add(this.txtDescription);
            this.gbItemInformation.Controls.Add(this.lblDescription);
            this.gbItemInformation.Controls.Add(this.lblName);
            this.gbItemInformation.Controls.Add(this.txtName);
            this.gbItemInformation.Location = new System.Drawing.Point(12, 27);
            this.gbItemInformation.Name = "gbItemInformation";
            this.gbItemInformation.Size = new System.Drawing.Size(395, 145);
            this.gbItemInformation.TabIndex = 67;
            this.gbItemInformation.TabStop = false;
            this.gbItemInformation.Text = "Basic Information";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 58);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(383, 82);
            this.txtDescription.TabIndex = 73;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(6, 42);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 72;
            this.lblDescription.Text = "Description:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 23;
            this.lblName.Text = "Name:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(339, 20);
            this.txtName.TabIndex = 22;
            // 
            // gbAIActions
            // 
            this.gbAIActions.Controls.Add(this.cbAIActionType);
            this.gbAIActions.Controls.Add(this.lblAIActionType);
            this.gbAIActions.Controls.Add(this.pgAIAction);
            this.gbAIActions.Controls.Add(this.btnDeleteAIAction);
            this.gbAIActions.Controls.Add(this.lsAIActions);
            this.gbAIActions.Controls.Add(this.btnAddAIAction);
            this.gbAIActions.Location = new System.Drawing.Point(12, 178);
            this.gbAIActions.Name = "gbAIActions";
            this.gbAIActions.Size = new System.Drawing.Size(509, 293);
            this.gbAIActions.TabIndex = 83;
            this.gbAIActions.TabStop = false;
            this.gbAIActions.Text = "AI Actions";
            // 
            // cbAIActionType
            // 
            this.cbAIActionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAIActionType.FormattingEnabled = true;
            this.cbAIActionType.Location = new System.Drawing.Point(6, 253);
            this.cbAIActionType.Name = "cbAIActionType";
            this.cbAIActionType.Size = new System.Drawing.Size(244, 21);
            this.cbAIActionType.TabIndex = 72;
            this.cbAIActionType.SelectedIndexChanged += new System.EventHandler(this.cbAIActionType_SelectedIndexChanged);
            // 
            // lblAIActionType
            // 
            this.lblAIActionType.AutoSize = true;
            this.lblAIActionType.Location = new System.Drawing.Point(6, 237);
            this.lblAIActionType.Name = "lblAIActionType";
            this.lblAIActionType.Size = new System.Drawing.Size(77, 13);
            this.lblAIActionType.TabIndex = 70;
            this.lblAIActionType.Text = "AI Action Type";
            // 
            // pgAIAction
            // 
            this.pgAIAction.Location = new System.Drawing.Point(256, 19);
            this.pgAIAction.Name = "pgAIAction";
            this.pgAIAction.Size = new System.Drawing.Size(244, 255);
            this.pgAIAction.TabIndex = 69;
            // 
            // btnDeleteAIAction
            // 
            this.btnDeleteAIAction.Location = new System.Drawing.Point(163, 211);
            this.btnDeleteAIAction.Name = "btnDeleteAIAction";
            this.btnDeleteAIAction.Size = new System.Drawing.Size(87, 23);
            this.btnDeleteAIAction.TabIndex = 68;
            this.btnDeleteAIAction.Text = "Delete";
            this.btnDeleteAIAction.UseVisualStyleBackColor = true;
            this.btnDeleteAIAction.Click += new System.EventHandler(this.btnDeleteAIAction_Click);
            // 
            // lsAIActions
            // 
            this.lsAIActions.FormattingEnabled = true;
            this.lsAIActions.Location = new System.Drawing.Point(9, 19);
            this.lsAIActions.Name = "lsAIActions";
            this.lsAIActions.Size = new System.Drawing.Size(241, 186);
            this.lsAIActions.TabIndex = 65;
            this.lsAIActions.SelectedIndexChanged += new System.EventHandler(this.lsAIActions_SelectedIndexChanged);
            // 
            // btnAddAIAction
            // 
            this.btnAddAIAction.Location = new System.Drawing.Point(6, 211);
            this.btnAddAIAction.Name = "btnAddAIAction";
            this.btnAddAIAction.Size = new System.Drawing.Size(87, 23);
            this.btnAddAIAction.TabIndex = 25;
            this.btnAddAIAction.Text = "Add";
            this.btnAddAIAction.UseVisualStyleBackColor = true;
            this.btnAddAIAction.Click += new System.EventHandler(this.btnAddAIAction_Click);
            // 
            // AIActionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 480);
            this.Controls.Add(this.gbAIActions);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "AIActionEditor";
            this.Text = "Character AI Action Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.gbAIActions.ResumeLayout(false);
            this.gbAIActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.GroupBox gbAIActions;
        private System.Windows.Forms.ComboBox cbAIActionType;
        private System.Windows.Forms.Label lblAIActionType;
        private System.Windows.Forms.PropertyGrid pgAIAction;
        private System.Windows.Forms.Button btnDeleteAIAction;
        private System.Windows.Forms.ListBox lsAIActions;
        private System.Windows.Forms.Button btnAddAIAction;
    }
}