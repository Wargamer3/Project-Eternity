
namespace ProjectEternity.Editors.MapEditor
{
    partial class DefaultGameModesConditions
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
            this.gbMandatoryMutators = new System.Windows.Forms.GroupBox();
            this.dgvMandatoryMutators = new System.Windows.Forms.DataGridView();
            this.clMutatorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbGameModes = new System.Windows.Forms.GroupBox();
            this.btnRemoveGameMode = new System.Windows.Forms.Button();
            this.lstGameModes = new System.Windows.Forms.ListBox();
            this.btnAddGameMode = new System.Windows.Forms.Button();
            this.cbGameMode = new System.Windows.Forms.ComboBox();
            this.pgGameModeAttributes = new System.Windows.Forms.PropertyGrid();
            this.gbGameModeAttributes = new System.Windows.Forms.GroupBox();
            this.lblGameModeAttributes = new System.Windows.Forms.Label();
            this.lblGameMode = new System.Windows.Forms.Label();
            this.gbPlayers = new System.Windows.Forms.GroupBox();
            this.lblMaxSquads = new System.Windows.Forms.Label();
            this.txtMaxSquadsPerPlayer = new System.Windows.Forms.NumericUpDown();
            this.lblPlayersMax = new System.Windows.Forms.Label();
            this.txtPlayersMax = new System.Windows.Forms.NumericUpDown();
            this.lblPlayersMin = new System.Windows.Forms.Label();
            this.txtPlayersMin = new System.Windows.Forms.NumericUpDown();
            this.gbMandatoryMutators.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMandatoryMutators)).BeginInit();
            this.gbGameModes.SuspendLayout();
            this.gbGameModeAttributes.SuspendLayout();
            this.gbPlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxSquadsPerPlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMin)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMandatoryMutators
            // 
            this.gbMandatoryMutators.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbMandatoryMutators.Controls.Add(this.dgvMandatoryMutators);
            this.gbMandatoryMutators.Location = new System.Drawing.Point(510, 142);
            this.gbMandatoryMutators.Name = "gbMandatoryMutators";
            this.gbMandatoryMutators.Size = new System.Drawing.Size(187, 149);
            this.gbMandatoryMutators.TabIndex = 24;
            this.gbMandatoryMutators.TabStop = false;
            this.gbMandatoryMutators.Text = "Mandatory Mutators";
            // 
            // dgvMandatoryMutators
            // 
            this.dgvMandatoryMutators.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvMandatoryMutators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMandatoryMutators.ColumnHeadersVisible = false;
            this.dgvMandatoryMutators.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clMutatorName});
            this.dgvMandatoryMutators.Location = new System.Drawing.Point(6, 19);
            this.dgvMandatoryMutators.MultiSelect = false;
            this.dgvMandatoryMutators.Name = "dgvMandatoryMutators";
            this.dgvMandatoryMutators.RowHeadersVisible = false;
            this.dgvMandatoryMutators.Size = new System.Drawing.Size(175, 124);
            this.dgvMandatoryMutators.TabIndex = 1;
            // 
            // clMutatorName
            // 
            this.clMutatorName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clMutatorName.HeaderText = "Name";
            this.clMutatorName.Name = "clMutatorName";
            // 
            // gbGameModes
            // 
            this.gbGameModes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbGameModes.Controls.Add(this.btnRemoveGameMode);
            this.gbGameModes.Controls.Add(this.lstGameModes);
            this.gbGameModes.Controls.Add(this.btnAddGameMode);
            this.gbGameModes.Location = new System.Drawing.Point(12, 12);
            this.gbGameModes.Name = "gbGameModes";
            this.gbGameModes.Size = new System.Drawing.Size(194, 279);
            this.gbGameModes.TabIndex = 25;
            this.gbGameModes.TabStop = false;
            this.gbGameModes.Text = "Game Modes";
            // 
            // btnRemoveGameMode
            // 
            this.btnRemoveGameMode.Location = new System.Drawing.Point(113, 250);
            this.btnRemoveGameMode.Name = "btnRemoveGameMode";
            this.btnRemoveGameMode.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveGameMode.TabIndex = 28;
            this.btnRemoveGameMode.Text = "Remove";
            this.btnRemoveGameMode.UseVisualStyleBackColor = true;
            this.btnRemoveGameMode.Click += new System.EventHandler(this.btnRemoveGameMode_Click);
            // 
            // lstGameModes
            // 
            this.lstGameModes.FormattingEnabled = true;
            this.lstGameModes.Location = new System.Drawing.Point(6, 19);
            this.lstGameModes.Name = "lstGameModes";
            this.lstGameModes.Size = new System.Drawing.Size(182, 225);
            this.lstGameModes.TabIndex = 27;
            this.lstGameModes.SelectedIndexChanged += new System.EventHandler(this.lstGameModes_SelectedIndexChanged);
            // 
            // btnAddGameMode
            // 
            this.btnAddGameMode.Location = new System.Drawing.Point(6, 250);
            this.btnAddGameMode.Name = "btnAddGameMode";
            this.btnAddGameMode.Size = new System.Drawing.Size(75, 23);
            this.btnAddGameMode.TabIndex = 26;
            this.btnAddGameMode.Text = "Add";
            this.btnAddGameMode.UseVisualStyleBackColor = true;
            this.btnAddGameMode.Click += new System.EventHandler(this.btnAddGameMode_Click);
            // 
            // cbGameMode
            // 
            this.cbGameMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbGameMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGameMode.FormattingEnabled = true;
            this.cbGameMode.Location = new System.Drawing.Point(9, 32);
            this.cbGameMode.Name = "cbGameMode";
            this.cbGameMode.Size = new System.Drawing.Size(277, 21);
            this.cbGameMode.TabIndex = 26;
            this.cbGameMode.SelectedIndexChanged += new System.EventHandler(this.cbGameMode_SelectedIndexChanged);
            // 
            // pgGameModeAttributes
            // 
            this.pgGameModeAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgGameModeAttributes.Location = new System.Drawing.Point(6, 72);
            this.pgGameModeAttributes.Name = "pgGameModeAttributes";
            this.pgGameModeAttributes.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgGameModeAttributes.Size = new System.Drawing.Size(280, 201);
            this.pgGameModeAttributes.TabIndex = 27;
            this.pgGameModeAttributes.ToolbarVisible = false;
            // 
            // gbGameModeAttributes
            // 
            this.gbGameModeAttributes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbGameModeAttributes.Controls.Add(this.lblGameModeAttributes);
            this.gbGameModeAttributes.Controls.Add(this.lblGameMode);
            this.gbGameModeAttributes.Controls.Add(this.pgGameModeAttributes);
            this.gbGameModeAttributes.Controls.Add(this.cbGameMode);
            this.gbGameModeAttributes.Location = new System.Drawing.Point(212, 12);
            this.gbGameModeAttributes.Name = "gbGameModeAttributes";
            this.gbGameModeAttributes.Size = new System.Drawing.Size(292, 279);
            this.gbGameModeAttributes.TabIndex = 26;
            this.gbGameModeAttributes.TabStop = false;
            this.gbGameModeAttributes.Text = "Game Mode Attributes";
            // 
            // lblGameModeAttributes
            // 
            this.lblGameModeAttributes.AutoSize = true;
            this.lblGameModeAttributes.Location = new System.Drawing.Point(6, 56);
            this.lblGameModeAttributes.Name = "lblGameModeAttributes";
            this.lblGameModeAttributes.Size = new System.Drawing.Size(51, 13);
            this.lblGameModeAttributes.TabIndex = 29;
            this.lblGameModeAttributes.Text = "Attributes";
            // 
            // lblGameMode
            // 
            this.lblGameMode.AutoSize = true;
            this.lblGameMode.Location = new System.Drawing.Point(6, 16);
            this.lblGameMode.Name = "lblGameMode";
            this.lblGameMode.Size = new System.Drawing.Size(68, 13);
            this.lblGameMode.TabIndex = 28;
            this.lblGameMode.Text = "Game Mode:";
            // 
            // gbPlayers
            // 
            this.gbPlayers.Controls.Add(this.lblMaxSquads);
            this.gbPlayers.Controls.Add(this.txtMaxSquadsPerPlayer);
            this.gbPlayers.Controls.Add(this.lblPlayersMax);
            this.gbPlayers.Controls.Add(this.txtPlayersMax);
            this.gbPlayers.Controls.Add(this.lblPlayersMin);
            this.gbPlayers.Controls.Add(this.txtPlayersMin);
            this.gbPlayers.Location = new System.Drawing.Point(510, 12);
            this.gbPlayers.Name = "gbPlayers";
            this.gbPlayers.Size = new System.Drawing.Size(187, 105);
            this.gbPlayers.TabIndex = 27;
            this.gbPlayers.TabStop = false;
            this.gbPlayers.Text = "Players";
            // 
            // lblMaxSquads
            // 
            this.lblMaxSquads.AutoSize = true;
            this.lblMaxSquads.Location = new System.Drawing.Point(6, 73);
            this.lblMaxSquads.Name = "lblMaxSquads";
            this.lblMaxSquads.Size = new System.Drawing.Size(69, 13);
            this.lblMaxSquads.TabIndex = 7;
            this.lblMaxSquads.Text = "Max Squads:";
            // 
            // txtMaxSquadsPerPlayer
            // 
            this.txtMaxSquadsPerPlayer.Location = new System.Drawing.Point(107, 71);
            this.txtMaxSquadsPerPlayer.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMaxSquadsPerPlayer.Name = "txtMaxSquadsPerPlayer";
            this.txtMaxSquadsPerPlayer.Size = new System.Drawing.Size(80, 20);
            this.txtMaxSquadsPerPlayer.TabIndex = 6;
            // 
            // lblPlayersMax
            // 
            this.lblPlayersMax.AutoSize = true;
            this.lblPlayersMax.Location = new System.Drawing.Point(6, 47);
            this.lblPlayersMax.Name = "lblPlayersMax";
            this.lblPlayersMax.Size = new System.Drawing.Size(67, 13);
            this.lblPlayersMax.TabIndex = 5;
            this.lblPlayersMax.Text = "Max Players:";
            // 
            // txtPlayersMax
            // 
            this.txtPlayersMax.Location = new System.Drawing.Point(107, 45);
            this.txtPlayersMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtPlayersMax.Name = "txtPlayersMax";
            this.txtPlayersMax.Size = new System.Drawing.Size(80, 20);
            this.txtPlayersMax.TabIndex = 4;
            // 
            // lblPlayersMin
            // 
            this.lblPlayersMin.AutoSize = true;
            this.lblPlayersMin.Location = new System.Drawing.Point(6, 21);
            this.lblPlayersMin.Name = "lblPlayersMin";
            this.lblPlayersMin.Size = new System.Drawing.Size(64, 13);
            this.lblPlayersMin.TabIndex = 3;
            this.lblPlayersMin.Text = "Min Players:";
            // 
            // txtPlayersMin
            // 
            this.txtPlayersMin.Location = new System.Drawing.Point(107, 19);
            this.txtPlayersMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtPlayersMin.Name = "txtPlayersMin";
            this.txtPlayersMin.Size = new System.Drawing.Size(80, 20);
            this.txtPlayersMin.TabIndex = 2;
            // 
            // DefaultGameModesConditions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 303);
            this.Controls.Add(this.gbPlayers);
            this.Controls.Add(this.gbGameModeAttributes);
            this.Controls.Add(this.gbGameModes);
            this.Controls.Add(this.gbMandatoryMutators);
            this.Name = "DefaultGameModesConditions";
            this.Text = "Default Game Modes Conditions";
            this.gbMandatoryMutators.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMandatoryMutators)).EndInit();
            this.gbGameModes.ResumeLayout(false);
            this.gbGameModeAttributes.ResumeLayout(false);
            this.gbGameModeAttributes.PerformLayout();
            this.gbPlayers.ResumeLayout(false);
            this.gbPlayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxSquadsPerPlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMin)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMandatoryMutators;
        public System.Windows.Forms.DataGridView dgvMandatoryMutators;
        private System.Windows.Forms.DataGridViewTextBoxColumn clMutatorName;
        private System.Windows.Forms.GroupBox gbGameModes;
        private System.Windows.Forms.ComboBox cbGameMode;
        private System.Windows.Forms.PropertyGrid pgGameModeAttributes;
        private System.Windows.Forms.Button btnRemoveGameMode;
        private System.Windows.Forms.Button btnAddGameMode;
        private System.Windows.Forms.GroupBox gbGameModeAttributes;
        private System.Windows.Forms.Label lblGameModeAttributes;
        private System.Windows.Forms.Label lblGameMode;
        private System.Windows.Forms.GroupBox gbPlayers;
        private System.Windows.Forms.Label lblPlayersMax;
        public System.Windows.Forms.NumericUpDown txtPlayersMax;
        private System.Windows.Forms.Label lblPlayersMin;
        public System.Windows.Forms.NumericUpDown txtPlayersMin;
        private System.Windows.Forms.Label lblMaxSquads;
        public System.Windows.Forms.NumericUpDown txtMaxSquadsPerPlayer;
        public System.Windows.Forms.ListBox lstGameModes;
    }
}