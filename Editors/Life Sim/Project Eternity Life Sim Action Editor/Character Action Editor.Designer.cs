namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class ActionEditor
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
            this.txtActionCost = new System.Windows.Forms.NumericUpDown();
            this.lblActionCost = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.tabControlActions = new System.Windows.Forms.TabControl();
            this.gbTraits = new System.Windows.Forms.GroupBox();
            this.btnRemoveTrait = new System.Windows.Forms.Button();
            this.btnAddTrait = new System.Windows.Forms.Button();
            this.lsTraits = new System.Windows.Forms.ListBox();
            this.btnAddExtraAction = new System.Windows.Forms.Button();
            this.gbAIActions = new System.Windows.Forms.GroupBox();
            this.btnRemoveAIAction = new System.Windows.Forms.Button();
            this.btnAddAIAction = new System.Windows.Forms.Button();
            this.lsAIActions = new System.Windows.Forms.ListBox();
            this.gbActions = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtActionCost)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            this.gbTraits.SuspendLayout();
            this.gbAIActions.SuspendLayout();
            this.gbActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtActionCost
            // 
            this.txtActionCost.Location = new System.Drawing.Point(61, 19);
            this.txtActionCost.Name = "txtActionCost";
            this.txtActionCost.Size = new System.Drawing.Size(70, 20);
            this.txtActionCost.TabIndex = 67;
            // 
            // lblActionCost
            // 
            this.lblActionCost.AutoSize = true;
            this.lblActionCost.Location = new System.Drawing.Point(6, 21);
            this.lblActionCost.Name = "lblActionCost";
            this.lblActionCost.Size = new System.Drawing.Size(49, 13);
            this.lblActionCost.TabIndex = 27;
            this.lblActionCost.Text = "Use cost";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(692, 24);
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
            // tabControlActions
            // 
            this.tabControlActions.Location = new System.Drawing.Point(413, 27);
            this.tabControlActions.Name = "tabControlActions";
            this.tabControlActions.SelectedIndex = 0;
            this.tabControlActions.Size = new System.Drawing.Size(275, 383);
            this.tabControlActions.TabIndex = 71;
            // 
            // gbTraits
            // 
            this.gbTraits.Controls.Add(this.btnRemoveTrait);
            this.gbTraits.Controls.Add(this.btnAddTrait);
            this.gbTraits.Controls.Add(this.lsTraits);
            this.gbTraits.Location = new System.Drawing.Point(12, 178);
            this.gbTraits.Name = "gbTraits";
            this.gbTraits.Size = new System.Drawing.Size(395, 85);
            this.gbTraits.TabIndex = 75;
            this.gbTraits.TabStop = false;
            this.gbTraits.Text = "Traits";
            // 
            // btnRemoveTrait
            // 
            this.btnRemoveTrait.Location = new System.Drawing.Point(273, 48);
            this.btnRemoveTrait.Name = "btnRemoveTrait";
            this.btnRemoveTrait.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveTrait.TabIndex = 75;
            this.btnRemoveTrait.Text = "Remove Trait";
            this.btnRemoveTrait.UseVisualStyleBackColor = true;
            this.btnRemoveTrait.Click += new System.EventHandler(this.btnRemoveTrait_Click);
            // 
            // btnAddTrait
            // 
            this.btnAddTrait.Location = new System.Drawing.Point(273, 19);
            this.btnAddTrait.Name = "btnAddTrait";
            this.btnAddTrait.Size = new System.Drawing.Size(116, 23);
            this.btnAddTrait.TabIndex = 74;
            this.btnAddTrait.Text = "Add Trait";
            this.btnAddTrait.UseVisualStyleBackColor = true;
            this.btnAddTrait.Click += new System.EventHandler(this.btnAddTrait_Click);
            // 
            // lsTraits
            // 
            this.lsTraits.FormattingEnabled = true;
            this.lsTraits.Location = new System.Drawing.Point(6, 19);
            this.lsTraits.Name = "lsTraits";
            this.lsTraits.Size = new System.Drawing.Size(261, 56);
            this.lsTraits.TabIndex = 73;
            // 
            // btnAddExtraAction
            // 
            this.btnAddExtraAction.Location = new System.Drawing.Point(148, 19);
            this.btnAddExtraAction.Name = "btnAddExtraAction";
            this.btnAddExtraAction.Size = new System.Drawing.Size(119, 23);
            this.btnAddExtraAction.TabIndex = 1;
            this.btnAddExtraAction.Text = "Add Extra Action";
            this.btnAddExtraAction.UseVisualStyleBackColor = true;
            this.btnAddExtraAction.Click += new System.EventHandler(this.btnAddExtraAction_Click);
            // 
            // gbAIActions
            // 
            this.gbAIActions.Controls.Add(this.btnRemoveAIAction);
            this.gbAIActions.Controls.Add(this.btnAddAIAction);
            this.gbAIActions.Controls.Add(this.lsAIActions);
            this.gbAIActions.Location = new System.Drawing.Point(12, 317);
            this.gbAIActions.Name = "gbAIActions";
            this.gbAIActions.Size = new System.Drawing.Size(395, 85);
            this.gbAIActions.TabIndex = 76;
            this.gbAIActions.TabStop = false;
            this.gbAIActions.Text = "AI Actions";
            // 
            // btnRemoveAIAction
            // 
            this.btnRemoveAIAction.Location = new System.Drawing.Point(273, 48);
            this.btnRemoveAIAction.Name = "btnRemoveAIAction";
            this.btnRemoveAIAction.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveAIAction.TabIndex = 75;
            this.btnRemoveAIAction.Text = "Remove AI Action";
            this.btnRemoveAIAction.UseVisualStyleBackColor = true;
            this.btnRemoveAIAction.Click += new System.EventHandler(this.btnRemoveAIAction_Click);
            // 
            // btnAddAIAction
            // 
            this.btnAddAIAction.Location = new System.Drawing.Point(273, 19);
            this.btnAddAIAction.Name = "btnAddAIAction";
            this.btnAddAIAction.Size = new System.Drawing.Size(116, 23);
            this.btnAddAIAction.TabIndex = 74;
            this.btnAddAIAction.Text = "Add AI Action";
            this.btnAddAIAction.UseVisualStyleBackColor = true;
            this.btnAddAIAction.Click += new System.EventHandler(this.btnAddAIAction_Click);
            // 
            // lsAIActions
            // 
            this.lsAIActions.FormattingEnabled = true;
            this.lsAIActions.Location = new System.Drawing.Point(6, 19);
            this.lsAIActions.Name = "lsAIActions";
            this.lsAIActions.Size = new System.Drawing.Size(261, 56);
            this.lsAIActions.TabIndex = 73;
            // 
            // gbActions
            // 
            this.gbActions.Controls.Add(this.btnAddExtraAction);
            this.gbActions.Controls.Add(this.txtActionCost);
            this.gbActions.Controls.Add(this.lblActionCost);
            this.gbActions.Location = new System.Drawing.Point(12, 269);
            this.gbActions.Name = "gbActions";
            this.gbActions.Size = new System.Drawing.Size(395, 50);
            this.gbActions.TabIndex = 77;
            this.gbActions.TabStop = false;
            this.gbActions.Text = "Actions";
            // 
            // ActionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 414);
            this.Controls.Add(this.gbActions);
            this.Controls.Add(this.gbAIActions);
            this.Controls.Add(this.gbTraits);
            this.Controls.Add(this.tabControlActions);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "ActionEditor";
            this.Text = "Action Editor";
            ((System.ComponentModel.ISupportInitialize)(this.txtActionCost)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.gbTraits.ResumeLayout(false);
            this.gbAIActions.ResumeLayout(false);
            this.gbActions.ResumeLayout(false);
            this.gbActions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblActionCost;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbItemInformation;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.NumericUpDown txtActionCost;
        private System.Windows.Forms.TabControl tabControlActions;
        private System.Windows.Forms.GroupBox gbTraits;
        private System.Windows.Forms.Button btnRemoveTrait;
        private System.Windows.Forms.Button btnAddTrait;
        private System.Windows.Forms.ListBox lsTraits;
        private System.Windows.Forms.Button btnAddExtraAction;
        private System.Windows.Forms.GroupBox gbAIActions;
        private System.Windows.Forms.Button btnRemoveAIAction;
        private System.Windows.Forms.Button btnAddAIAction;
        private System.Windows.Forms.ListBox lsAIActions;
        private System.Windows.Forms.GroupBox gbActions;
    }
}