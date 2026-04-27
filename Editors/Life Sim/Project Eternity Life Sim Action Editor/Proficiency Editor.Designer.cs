namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    partial class ProficiencyEditor
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
            this.lblBaseValue = new System.Windows.Forms.Label();
            this.txtBaseValue = new System.Windows.Forms.NumericUpDown();
            this.gbBonusStats = new System.Windows.Forms.GroupBox();
            this.ckCHA = new System.Windows.Forms.CheckBox();
            this.ckWIS = new System.Windows.Forms.CheckBox();
            this.ckINT = new System.Windows.Forms.CheckBox();
            this.ckCON = new System.Windows.Forms.CheckBox();
            this.ckDEX = new System.Windows.Forms.CheckBox();
            this.ckSTR = new System.Windows.Forms.CheckBox();
            this.gbBaseActions = new System.Windows.Forms.GroupBox();
            this.btnRemoveBaseAction = new System.Windows.Forms.Button();
            this.btnAddBaseAction = new System.Windows.Forms.Button();
            this.lsBaseActions = new System.Windows.Forms.ListBox();
            this.gbTrainedActions = new System.Windows.Forms.GroupBox();
            this.btnRemoveTrainedAction = new System.Windows.Forms.Button();
            this.btnAddTrainedAction = new System.Windows.Forms.Button();
            this.lsTrainedActions = new System.Windows.Forms.ListBox();
            this.gbExpertActions = new System.Windows.Forms.GroupBox();
            this.btnRemoveExpertAction = new System.Windows.Forms.Button();
            this.btnAddExpertAction = new System.Windows.Forms.Button();
            this.lsExpertActions = new System.Windows.Forms.ListBox();
            this.gbLegendaryActions = new System.Windows.Forms.GroupBox();
            this.btnRemoveLegendaryAction = new System.Windows.Forms.Button();
            this.btnAddLegendaryAction = new System.Windows.Forms.Button();
            this.lsLegendaryActions = new System.Windows.Forms.ListBox();
            this.gbMasterActions = new System.Windows.Forms.GroupBox();
            this.btnRemoveMasterAction = new System.Windows.Forms.Button();
            this.btnAddMasterAction = new System.Windows.Forms.Button();
            this.lsMasterActions = new System.Windows.Forms.ListBox();
            this.gbDiceAttributes = new System.Windows.Forms.GroupBox();
            this.lblDiceValue = new System.Windows.Forms.Label();
            this.txtDiceValue = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.gbItemInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseValue)).BeginInit();
            this.gbBonusStats.SuspendLayout();
            this.gbBaseActions.SuspendLayout();
            this.gbTrainedActions.SuspendLayout();
            this.gbExpertActions.SuspendLayout();
            this.gbLegendaryActions.SuspendLayout();
            this.gbMasterActions.SuspendLayout();
            this.gbDiceAttributes.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(965, 24);
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
            this.txtName.Size = new System.Drawing.Size(193, 20);
            this.txtName.TabIndex = 22;
            // 
            // lblBaseValue
            // 
            this.lblBaseValue.AutoSize = true;
            this.lblBaseValue.Location = new System.Drawing.Point(6, 20);
            this.lblBaseValue.Name = "lblBaseValue";
            this.lblBaseValue.Size = new System.Drawing.Size(64, 13);
            this.lblBaseValue.TabIndex = 83;
            this.lblBaseValue.Text = "Base Value:";
            // 
            // txtBaseValue
            // 
            this.txtBaseValue.Location = new System.Drawing.Point(191, 16);
            this.txtBaseValue.Name = "txtBaseValue";
            this.txtBaseValue.Size = new System.Drawing.Size(70, 20);
            this.txtBaseValue.TabIndex = 82;
            // 
            // gbBonusStats
            // 
            this.gbBonusStats.Controls.Add(this.ckCHA);
            this.gbBonusStats.Controls.Add(this.ckWIS);
            this.gbBonusStats.Controls.Add(this.ckINT);
            this.gbBonusStats.Controls.Add(this.ckCON);
            this.gbBonusStats.Controls.Add(this.ckDEX);
            this.gbBonusStats.Controls.Add(this.ckSTR);
            this.gbBonusStats.Location = new System.Drawing.Point(285, 178);
            this.gbBonusStats.Name = "gbBonusStats";
            this.gbBonusStats.Size = new System.Drawing.Size(122, 116);
            this.gbBonusStats.TabIndex = 77;
            this.gbBonusStats.TabStop = false;
            this.gbBonusStats.Text = "Bonus Stats";
            // 
            // ckCHA
            // 
            this.ckCHA.AutoSize = true;
            this.ckCHA.Location = new System.Drawing.Point(60, 72);
            this.ckCHA.Name = "ckCHA";
            this.ckCHA.Size = new System.Drawing.Size(48, 17);
            this.ckCHA.TabIndex = 79;
            this.ckCHA.Text = "CHA";
            this.ckCHA.UseVisualStyleBackColor = true;
            // 
            // ckWIS
            // 
            this.ckWIS.AutoSize = true;
            this.ckWIS.Location = new System.Drawing.Point(6, 72);
            this.ckWIS.Name = "ckWIS";
            this.ckWIS.Size = new System.Drawing.Size(47, 17);
            this.ckWIS.TabIndex = 78;
            this.ckWIS.Text = "WIS";
            this.ckWIS.UseVisualStyleBackColor = true;
            // 
            // ckINT
            // 
            this.ckINT.AutoSize = true;
            this.ckINT.Location = new System.Drawing.Point(60, 46);
            this.ckINT.Name = "ckINT";
            this.ckINT.Size = new System.Drawing.Size(44, 17);
            this.ckINT.TabIndex = 76;
            this.ckINT.Text = "INT";
            this.ckINT.UseVisualStyleBackColor = true;
            // 
            // ckCON
            // 
            this.ckCON.AutoSize = true;
            this.ckCON.Location = new System.Drawing.Point(6, 46);
            this.ckCON.Name = "ckCON";
            this.ckCON.Size = new System.Drawing.Size(49, 17);
            this.ckCON.TabIndex = 77;
            this.ckCON.Text = "CON";
            this.ckCON.UseVisualStyleBackColor = true;
            // 
            // ckDEX
            // 
            this.ckDEX.AutoSize = true;
            this.ckDEX.Location = new System.Drawing.Point(60, 19);
            this.ckDEX.Name = "ckDEX";
            this.ckDEX.Size = new System.Drawing.Size(48, 17);
            this.ckDEX.TabIndex = 76;
            this.ckDEX.Text = "DEX";
            this.ckDEX.UseVisualStyleBackColor = true;
            // 
            // ckSTR
            // 
            this.ckSTR.AutoSize = true;
            this.ckSTR.Location = new System.Drawing.Point(6, 19);
            this.ckSTR.Name = "ckSTR";
            this.ckSTR.Size = new System.Drawing.Size(48, 17);
            this.ckSTR.TabIndex = 75;
            this.ckSTR.Text = "STR";
            this.ckSTR.UseVisualStyleBackColor = true;
            // 
            // gbBaseActions
            // 
            this.gbBaseActions.Controls.Add(this.btnRemoveBaseAction);
            this.gbBaseActions.Controls.Add(this.btnAddBaseAction);
            this.gbBaseActions.Controls.Add(this.lsBaseActions);
            this.gbBaseActions.Location = new System.Drawing.Point(413, 27);
            this.gbBaseActions.Name = "gbBaseActions";
            this.gbBaseActions.Size = new System.Drawing.Size(267, 85);
            this.gbBaseActions.TabIndex = 78;
            this.gbBaseActions.TabStop = false;
            this.gbBaseActions.Text = "Base Actions";
            // 
            // btnRemoveBaseAction
            // 
            this.btnRemoveBaseAction.Location = new System.Drawing.Point(167, 48);
            this.btnRemoveBaseAction.Name = "btnRemoveBaseAction";
            this.btnRemoveBaseAction.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveBaseAction.TabIndex = 75;
            this.btnRemoveBaseAction.Text = "Remove Action";
            this.btnRemoveBaseAction.UseVisualStyleBackColor = true;
            this.btnRemoveBaseAction.Click += new System.EventHandler(this.btnRemoveBaseAction_Click);
            // 
            // btnAddBaseAction
            // 
            this.btnAddBaseAction.Location = new System.Drawing.Point(167, 19);
            this.btnAddBaseAction.Name = "btnAddBaseAction";
            this.btnAddBaseAction.Size = new System.Drawing.Size(94, 23);
            this.btnAddBaseAction.TabIndex = 74;
            this.btnAddBaseAction.Text = "Add Action";
            this.btnAddBaseAction.UseVisualStyleBackColor = true;
            this.btnAddBaseAction.Click += new System.EventHandler(this.btnAddBaseAction_Click);
            // 
            // lsBaseActions
            // 
            this.lsBaseActions.FormattingEnabled = true;
            this.lsBaseActions.Location = new System.Drawing.Point(6, 19);
            this.lsBaseActions.Name = "lsBaseActions";
            this.lsBaseActions.Size = new System.Drawing.Size(155, 56);
            this.lsBaseActions.TabIndex = 73;
            // 
            // gbTrainedActions
            // 
            this.gbTrainedActions.Controls.Add(this.btnRemoveTrainedAction);
            this.gbTrainedActions.Controls.Add(this.btnAddTrainedAction);
            this.gbTrainedActions.Controls.Add(this.lsTrainedActions);
            this.gbTrainedActions.Location = new System.Drawing.Point(413, 118);
            this.gbTrainedActions.Name = "gbTrainedActions";
            this.gbTrainedActions.Size = new System.Drawing.Size(267, 85);
            this.gbTrainedActions.TabIndex = 79;
            this.gbTrainedActions.TabStop = false;
            this.gbTrainedActions.Text = "Trained Actions";
            // 
            // btnRemoveTrainedAction
            // 
            this.btnRemoveTrainedAction.Location = new System.Drawing.Point(167, 48);
            this.btnRemoveTrainedAction.Name = "btnRemoveTrainedAction";
            this.btnRemoveTrainedAction.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveTrainedAction.TabIndex = 75;
            this.btnRemoveTrainedAction.Text = "Remove Action";
            this.btnRemoveTrainedAction.UseVisualStyleBackColor = true;
            this.btnRemoveTrainedAction.Click += new System.EventHandler(this.btnRemoveTrainedAction_Click);
            // 
            // btnAddTrainedAction
            // 
            this.btnAddTrainedAction.Location = new System.Drawing.Point(167, 19);
            this.btnAddTrainedAction.Name = "btnAddTrainedAction";
            this.btnAddTrainedAction.Size = new System.Drawing.Size(94, 23);
            this.btnAddTrainedAction.TabIndex = 74;
            this.btnAddTrainedAction.Text = "Add Action";
            this.btnAddTrainedAction.UseVisualStyleBackColor = true;
            this.btnAddTrainedAction.Click += new System.EventHandler(this.btnAddTrainedAction_Click);
            // 
            // lsTrainedActions
            // 
            this.lsTrainedActions.FormattingEnabled = true;
            this.lsTrainedActions.Location = new System.Drawing.Point(6, 19);
            this.lsTrainedActions.Name = "lsTrainedActions";
            this.lsTrainedActions.Size = new System.Drawing.Size(155, 56);
            this.lsTrainedActions.TabIndex = 73;
            // 
            // gbExpertActions
            // 
            this.gbExpertActions.Controls.Add(this.btnRemoveExpertAction);
            this.gbExpertActions.Controls.Add(this.btnAddExpertAction);
            this.gbExpertActions.Controls.Add(this.lsExpertActions);
            this.gbExpertActions.Location = new System.Drawing.Point(686, 118);
            this.gbExpertActions.Name = "gbExpertActions";
            this.gbExpertActions.Size = new System.Drawing.Size(267, 85);
            this.gbExpertActions.TabIndex = 80;
            this.gbExpertActions.TabStop = false;
            this.gbExpertActions.Text = "Expert Actions";
            // 
            // btnRemoveExpertAction
            // 
            this.btnRemoveExpertAction.Location = new System.Drawing.Point(167, 48);
            this.btnRemoveExpertAction.Name = "btnRemoveExpertAction";
            this.btnRemoveExpertAction.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveExpertAction.TabIndex = 75;
            this.btnRemoveExpertAction.Text = "Remove Action";
            this.btnRemoveExpertAction.UseVisualStyleBackColor = true;
            this.btnRemoveExpertAction.Click += new System.EventHandler(this.btnRemoveExpertAction_Click);
            // 
            // btnAddExpertAction
            // 
            this.btnAddExpertAction.Location = new System.Drawing.Point(167, 19);
            this.btnAddExpertAction.Name = "btnAddExpertAction";
            this.btnAddExpertAction.Size = new System.Drawing.Size(94, 23);
            this.btnAddExpertAction.TabIndex = 74;
            this.btnAddExpertAction.Text = "Add Action";
            this.btnAddExpertAction.UseVisualStyleBackColor = true;
            this.btnAddExpertAction.Click += new System.EventHandler(this.btnAddExpertAction_Click);
            // 
            // lsExpertActions
            // 
            this.lsExpertActions.FormattingEnabled = true;
            this.lsExpertActions.Location = new System.Drawing.Point(6, 19);
            this.lsExpertActions.Name = "lsExpertActions";
            this.lsExpertActions.Size = new System.Drawing.Size(155, 56);
            this.lsExpertActions.TabIndex = 73;
            // 
            // gbLegendaryActions
            // 
            this.gbLegendaryActions.Controls.Add(this.btnRemoveLegendaryAction);
            this.gbLegendaryActions.Controls.Add(this.btnAddLegendaryAction);
            this.gbLegendaryActions.Controls.Add(this.lsLegendaryActions);
            this.gbLegendaryActions.Location = new System.Drawing.Point(686, 209);
            this.gbLegendaryActions.Name = "gbLegendaryActions";
            this.gbLegendaryActions.Size = new System.Drawing.Size(267, 85);
            this.gbLegendaryActions.TabIndex = 79;
            this.gbLegendaryActions.TabStop = false;
            this.gbLegendaryActions.Text = "Legendary Actions";
            // 
            // btnRemoveLegendaryAction
            // 
            this.btnRemoveLegendaryAction.Location = new System.Drawing.Point(167, 48);
            this.btnRemoveLegendaryAction.Name = "btnRemoveLegendaryAction";
            this.btnRemoveLegendaryAction.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveLegendaryAction.TabIndex = 75;
            this.btnRemoveLegendaryAction.Text = "Remove Action";
            this.btnRemoveLegendaryAction.UseVisualStyleBackColor = true;
            this.btnRemoveLegendaryAction.Click += new System.EventHandler(this.btnRemoveLegendaryAction_Click);
            // 
            // btnAddLegendaryAction
            // 
            this.btnAddLegendaryAction.Location = new System.Drawing.Point(167, 19);
            this.btnAddLegendaryAction.Name = "btnAddLegendaryAction";
            this.btnAddLegendaryAction.Size = new System.Drawing.Size(94, 23);
            this.btnAddLegendaryAction.TabIndex = 74;
            this.btnAddLegendaryAction.Text = "Add Action";
            this.btnAddLegendaryAction.UseVisualStyleBackColor = true;
            this.btnAddLegendaryAction.Click += new System.EventHandler(this.btnAddLegendaryAction_Click);
            // 
            // lsLegendaryActions
            // 
            this.lsLegendaryActions.FormattingEnabled = true;
            this.lsLegendaryActions.Location = new System.Drawing.Point(6, 19);
            this.lsLegendaryActions.Name = "lsLegendaryActions";
            this.lsLegendaryActions.Size = new System.Drawing.Size(155, 56);
            this.lsLegendaryActions.TabIndex = 73;
            // 
            // gbMasterActions
            // 
            this.gbMasterActions.Controls.Add(this.btnRemoveMasterAction);
            this.gbMasterActions.Controls.Add(this.btnAddMasterAction);
            this.gbMasterActions.Controls.Add(this.lsMasterActions);
            this.gbMasterActions.Location = new System.Drawing.Point(413, 209);
            this.gbMasterActions.Name = "gbMasterActions";
            this.gbMasterActions.Size = new System.Drawing.Size(267, 85);
            this.gbMasterActions.TabIndex = 81;
            this.gbMasterActions.TabStop = false;
            this.gbMasterActions.Text = "Master Actions";
            // 
            // btnRemoveMasterAction
            // 
            this.btnRemoveMasterAction.Location = new System.Drawing.Point(167, 48);
            this.btnRemoveMasterAction.Name = "btnRemoveMasterAction";
            this.btnRemoveMasterAction.Size = new System.Drawing.Size(94, 23);
            this.btnRemoveMasterAction.TabIndex = 75;
            this.btnRemoveMasterAction.Text = "Remove Action";
            this.btnRemoveMasterAction.UseVisualStyleBackColor = true;
            this.btnRemoveMasterAction.Click += new System.EventHandler(this.btnRemoveMasterAction_Click);
            // 
            // btnAddMasterAction
            // 
            this.btnAddMasterAction.Location = new System.Drawing.Point(167, 19);
            this.btnAddMasterAction.Name = "btnAddMasterAction";
            this.btnAddMasterAction.Size = new System.Drawing.Size(94, 23);
            this.btnAddMasterAction.TabIndex = 74;
            this.btnAddMasterAction.Text = "Add Action";
            this.btnAddMasterAction.UseVisualStyleBackColor = true;
            this.btnAddMasterAction.Click += new System.EventHandler(this.btnAddMasterAction_Click);
            // 
            // lsMasterActions
            // 
            this.lsMasterActions.FormattingEnabled = true;
            this.lsMasterActions.Location = new System.Drawing.Point(6, 19);
            this.lsMasterActions.Name = "lsMasterActions";
            this.lsMasterActions.Size = new System.Drawing.Size(155, 56);
            this.lsMasterActions.TabIndex = 73;
            // 
            // gbDiceAttributes
            // 
            this.gbDiceAttributes.Controls.Add(this.txtDiceValue);
            this.gbDiceAttributes.Controls.Add(this.lblDiceValue);
            this.gbDiceAttributes.Controls.Add(this.lblBaseValue);
            this.gbDiceAttributes.Controls.Add(this.txtBaseValue);
            this.gbDiceAttributes.Location = new System.Drawing.Point(12, 178);
            this.gbDiceAttributes.Name = "gbDiceAttributes";
            this.gbDiceAttributes.Size = new System.Drawing.Size(267, 116);
            this.gbDiceAttributes.TabIndex = 82;
            this.gbDiceAttributes.TabStop = false;
            this.gbDiceAttributes.Text = "Dice Attributes";
            // 
            // lblDiceValue
            // 
            this.lblDiceValue.AutoSize = true;
            this.lblDiceValue.Location = new System.Drawing.Point(6, 45);
            this.lblDiceValue.Name = "lblDiceValue";
            this.lblDiceValue.Size = new System.Drawing.Size(62, 13);
            this.lblDiceValue.TabIndex = 85;
            this.lblDiceValue.Text = "Dice Value:";
            // 
            // txtDiceValue
            // 
            this.txtDiceValue.Location = new System.Drawing.Point(115, 42);
            this.txtDiceValue.Name = "txtDiceValue";
            this.txtDiceValue.Size = new System.Drawing.Size(146, 20);
            this.txtDiceValue.TabIndex = 86;
            // 
            // ProficiencyEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 306);
            this.Controls.Add(this.gbDiceAttributes);
            this.Controls.Add(this.gbMasterActions);
            this.Controls.Add(this.gbLegendaryActions);
            this.Controls.Add(this.gbExpertActions);
            this.Controls.Add(this.gbTrainedActions);
            this.Controls.Add(this.gbBaseActions);
            this.Controls.Add(this.gbBonusStats);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "ProficiencyEditor";
            this.Text = "Proficiency Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtBaseValue)).EndInit();
            this.gbBonusStats.ResumeLayout(false);
            this.gbBonusStats.PerformLayout();
            this.gbBaseActions.ResumeLayout(false);
            this.gbTrainedActions.ResumeLayout(false);
            this.gbExpertActions.ResumeLayout(false);
            this.gbLegendaryActions.ResumeLayout(false);
            this.gbMasterActions.ResumeLayout(false);
            this.gbDiceAttributes.ResumeLayout(false);
            this.gbDiceAttributes.PerformLayout();
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
        private System.Windows.Forms.GroupBox gbBonusStats;
        private System.Windows.Forms.CheckBox ckDEX;
        private System.Windows.Forms.CheckBox ckSTR;
        private System.Windows.Forms.CheckBox ckCHA;
        private System.Windows.Forms.CheckBox ckWIS;
        private System.Windows.Forms.CheckBox ckINT;
        private System.Windows.Forms.CheckBox ckCON;
        private System.Windows.Forms.GroupBox gbBaseActions;
        private System.Windows.Forms.Button btnRemoveBaseAction;
        private System.Windows.Forms.Button btnAddBaseAction;
        private System.Windows.Forms.ListBox lsBaseActions;
        private System.Windows.Forms.GroupBox gbTrainedActions;
        private System.Windows.Forms.Button btnRemoveTrainedAction;
        private System.Windows.Forms.Button btnAddTrainedAction;
        private System.Windows.Forms.ListBox lsTrainedActions;
        private System.Windows.Forms.GroupBox gbExpertActions;
        private System.Windows.Forms.Button btnRemoveExpertAction;
        private System.Windows.Forms.Button btnAddExpertAction;
        private System.Windows.Forms.ListBox lsExpertActions;
        private System.Windows.Forms.GroupBox gbLegendaryActions;
        private System.Windows.Forms.Button btnRemoveLegendaryAction;
        private System.Windows.Forms.Button btnAddLegendaryAction;
        private System.Windows.Forms.ListBox lsLegendaryActions;
        private System.Windows.Forms.GroupBox gbMasterActions;
        private System.Windows.Forms.Button btnRemoveMasterAction;
        private System.Windows.Forms.Button btnAddMasterAction;
        private System.Windows.Forms.ListBox lsMasterActions;
        private System.Windows.Forms.Label lblBaseValue;
        private System.Windows.Forms.NumericUpDown txtBaseValue;
        private System.Windows.Forms.GroupBox gbDiceAttributes;
        private System.Windows.Forms.Label lblDiceValue;
        private System.Windows.Forms.TextBox txtDiceValue;
    }
}