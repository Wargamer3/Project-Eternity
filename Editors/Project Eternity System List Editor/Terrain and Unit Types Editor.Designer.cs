
namespace ProjectEternity.Editors.SystemListEditor
{
    partial class TerrainAndUnitTypesEditor
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
            this.gbTerrainTypes = new System.Windows.Forms.GroupBox();
            this.lblUnitMovementTypeAnnulationName = new System.Windows.Forms.Label();
            this.lsTerrainTypes = new System.Windows.Forms.ListBox();
            this.txtUnitMovementTypeAnnulationName = new System.Windows.Forms.TextBox();
            this.lblUnitMovementTypeActivationName = new System.Windows.Forms.Label();
            this.lblTerrainTypeName = new System.Windows.Forms.Label();
            this.txtUnitMovementTypeActivationName = new System.Windows.Forms.TextBox();
            this.txtWallHardness = new System.Windows.Forms.NumericUpDown();
            this.btnRemoveTerrainType = new System.Windows.Forms.Button();
            this.lblWallHardness = new System.Windows.Forms.Label();
            this.txtTerrainTypeName = new System.Windows.Forms.TextBox();
            this.btnAddTerrainType = new System.Windows.Forms.Button();
            this.gbTerrainAttributes = new System.Windows.Forms.GroupBox();
            this.gbRestrictionAttributes = new System.Windows.Forms.GroupBox();
            this.lblENCostPerTurn = new System.Windows.Forms.Label();
            this.txtENCostPerTurn = new System.Windows.Forms.NumericUpDown();
            this.lblENCostToMove = new System.Windows.Forms.Label();
            this.txtENCostToMove = new System.Windows.Forms.NumericUpDown();
            this.dgvTerrainRanks = new System.Windows.Forms.DataGridView();
            this.Rank = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.lblMovementCost = new System.Windows.Forms.Label();
            this.txtMovementCost = new System.Windows.Forms.NumericUpDown();
            this.lblRestrictionCategory = new System.Windows.Forms.Label();
            this.txtEntryCost = new System.Windows.Forms.NumericUpDown();
            this.lblEntryCost = new System.Windows.Forms.Label();
            this.cbRestrictionCategory = new System.Windows.Forms.ComboBox();
            this.gbTerrainRestrictions = new System.Windows.Forms.GroupBox();
            this.tvTerrainRestrictions = new System.Windows.Forms.TreeView();
            this.btnAddSubTerrainRestriction = new System.Windows.Forms.Button();
            this.btnRemoveTerrainRestriction = new System.Windows.Forms.Button();
            this.btnAddTerrainRestriction = new System.Windows.Forms.Button();
            this.gbUnitTypes = new System.Windows.Forms.GroupBox();
            this.lblUnitType = new System.Windows.Forms.Label();
            this.txtUnitType = new System.Windows.Forms.TextBox();
            this.btnRemoveUnitType = new System.Windows.Forms.Button();
            this.btnAddUnitType = new System.Windows.Forms.Button();
            this.lsUnitTypes = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbTerrainTypes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWallHardness)).BeginInit();
            this.gbTerrainAttributes.SuspendLayout();
            this.gbRestrictionAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtENCostPerTurn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtENCostToMove)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTerrainRanks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMovementCost)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntryCost)).BeginInit();
            this.gbTerrainRestrictions.SuspendLayout();
            this.gbUnitTypes.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTerrainTypes
            // 
            this.gbTerrainTypes.Controls.Add(this.lblUnitMovementTypeAnnulationName);
            this.gbTerrainTypes.Controls.Add(this.lsTerrainTypes);
            this.gbTerrainTypes.Controls.Add(this.txtUnitMovementTypeAnnulationName);
            this.gbTerrainTypes.Controls.Add(this.lblUnitMovementTypeActivationName);
            this.gbTerrainTypes.Controls.Add(this.lblTerrainTypeName);
            this.gbTerrainTypes.Controls.Add(this.txtUnitMovementTypeActivationName);
            this.gbTerrainTypes.Controls.Add(this.txtWallHardness);
            this.gbTerrainTypes.Controls.Add(this.btnRemoveTerrainType);
            this.gbTerrainTypes.Controls.Add(this.lblWallHardness);
            this.gbTerrainTypes.Controls.Add(this.txtTerrainTypeName);
            this.gbTerrainTypes.Controls.Add(this.btnAddTerrainType);
            this.gbTerrainTypes.Location = new System.Drawing.Point(6, 19);
            this.gbTerrainTypes.Name = "gbTerrainTypes";
            this.gbTerrainTypes.Size = new System.Drawing.Size(169, 407);
            this.gbTerrainTypes.TabIndex = 0;
            this.gbTerrainTypes.TabStop = false;
            this.gbTerrainTypes.Text = "Terrain Types";
            // 
            // lblUnitMovementTypeAnnulationName
            // 
            this.lblUnitMovementTypeAnnulationName.AutoSize = true;
            this.lblUnitMovementTypeAnnulationName.Location = new System.Drawing.Point(6, 293);
            this.lblUnitMovementTypeAnnulationName.Name = "lblUnitMovementTypeAnnulationName";
            this.lblUnitMovementTypeAnnulationName.Size = new System.Drawing.Size(90, 13);
            this.lblUnitMovementTypeAnnulationName.TabIndex = 10;
            this.lblUnitMovementTypeAnnulationName.Text = "Annulation Action";
            // 
            // lsTerrainTypes
            // 
            this.lsTerrainTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsTerrainTypes.FormattingEnabled = true;
            this.lsTerrainTypes.Location = new System.Drawing.Point(6, 13);
            this.lsTerrainTypes.Name = "lsTerrainTypes";
            this.lsTerrainTypes.Size = new System.Drawing.Size(156, 199);
            this.lsTerrainTypes.TabIndex = 6;
            this.lsTerrainTypes.SelectedIndexChanged += new System.EventHandler(this.lsTerrainTypes_SelectedIndexChanged);
            // 
            // txtUnitMovementTypeAnnulationName
            // 
            this.txtUnitMovementTypeAnnulationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUnitMovementTypeAnnulationName.Location = new System.Drawing.Point(6, 309);
            this.txtUnitMovementTypeAnnulationName.Name = "txtUnitMovementTypeAnnulationName";
            this.txtUnitMovementTypeAnnulationName.Size = new System.Drawing.Size(158, 20);
            this.txtUnitMovementTypeAnnulationName.TabIndex = 11;
            this.txtUnitMovementTypeAnnulationName.TextChanged += new System.EventHandler(this.txtUnitMovementTypeAnnulationName_TextChanged);
            // 
            // lblUnitMovementTypeActivationName
            // 
            this.lblUnitMovementTypeActivationName.AutoSize = true;
            this.lblUnitMovementTypeActivationName.Location = new System.Drawing.Point(6, 254);
            this.lblUnitMovementTypeActivationName.Name = "lblUnitMovementTypeActivationName";
            this.lblUnitMovementTypeActivationName.Size = new System.Drawing.Size(87, 13);
            this.lblUnitMovementTypeActivationName.TabIndex = 8;
            this.lblUnitMovementTypeActivationName.Text = "Activation Action";
            // 
            // lblTerrainTypeName
            // 
            this.lblTerrainTypeName.AutoSize = true;
            this.lblTerrainTypeName.Location = new System.Drawing.Point(8, 215);
            this.lblTerrainTypeName.Name = "lblTerrainTypeName";
            this.lblTerrainTypeName.Size = new System.Drawing.Size(35, 13);
            this.lblTerrainTypeName.TabIndex = 3;
            this.lblTerrainTypeName.Text = "Name";
            // 
            // txtUnitMovementTypeActivationName
            // 
            this.txtUnitMovementTypeActivationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUnitMovementTypeActivationName.Location = new System.Drawing.Point(6, 270);
            this.txtUnitMovementTypeActivationName.Name = "txtUnitMovementTypeActivationName";
            this.txtUnitMovementTypeActivationName.Size = new System.Drawing.Size(158, 20);
            this.txtUnitMovementTypeActivationName.TabIndex = 9;
            this.txtUnitMovementTypeActivationName.TextChanged += new System.EventHandler(this.txtUnitMovementTypeActivationName_TextChanged);
            // 
            // txtWallHardness
            // 
            this.txtWallHardness.Location = new System.Drawing.Point(108, 335);
            this.txtWallHardness.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtWallHardness.Name = "txtWallHardness";
            this.txtWallHardness.Size = new System.Drawing.Size(55, 20);
            this.txtWallHardness.TabIndex = 0;
            this.txtWallHardness.ValueChanged += new System.EventHandler(this.txtWallHardness_ValueChanged);
            // 
            // btnRemoveTerrainType
            // 
            this.btnRemoveTerrainType.Location = new System.Drawing.Point(87, 361);
            this.btnRemoveTerrainType.Name = "btnRemoveTerrainType";
            this.btnRemoveTerrainType.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveTerrainType.TabIndex = 2;
            this.btnRemoveTerrainType.Text = "Remove";
            this.btnRemoveTerrainType.UseVisualStyleBackColor = true;
            this.btnRemoveTerrainType.Click += new System.EventHandler(this.btnRemoveTerrainType_Click);
            // 
            // lblWallHardness
            // 
            this.lblWallHardness.AutoSize = true;
            this.lblWallHardness.Location = new System.Drawing.Point(7, 337);
            this.lblWallHardness.Name = "lblWallHardness";
            this.lblWallHardness.Size = new System.Drawing.Size(76, 13);
            this.lblWallHardness.TabIndex = 1;
            this.lblWallHardness.Text = "Wall Hardness";
            // 
            // txtTerrainTypeName
            // 
            this.txtTerrainTypeName.Location = new System.Drawing.Point(6, 231);
            this.txtTerrainTypeName.Name = "txtTerrainTypeName";
            this.txtTerrainTypeName.Size = new System.Drawing.Size(158, 20);
            this.txtTerrainTypeName.TabIndex = 3;
            this.txtTerrainTypeName.TextChanged += new System.EventHandler(this.txtTerrainTypeName_TextChanged);
            // 
            // btnAddTerrainType
            // 
            this.btnAddTerrainType.Location = new System.Drawing.Point(6, 361);
            this.btnAddTerrainType.Name = "btnAddTerrainType";
            this.btnAddTerrainType.Size = new System.Drawing.Size(75, 23);
            this.btnAddTerrainType.TabIndex = 1;
            this.btnAddTerrainType.Text = "Add";
            this.btnAddTerrainType.UseVisualStyleBackColor = true;
            this.btnAddTerrainType.Click += new System.EventHandler(this.btnAddTerrainType_Click);
            // 
            // gbTerrainAttributes
            // 
            this.gbTerrainAttributes.Controls.Add(this.gbRestrictionAttributes);
            this.gbTerrainAttributes.Controls.Add(this.gbTerrainRestrictions);
            this.gbTerrainAttributes.Controls.Add(this.gbTerrainTypes);
            this.gbTerrainAttributes.Location = new System.Drawing.Point(365, 27);
            this.gbTerrainAttributes.Name = "gbTerrainAttributes";
            this.gbTerrainAttributes.Size = new System.Drawing.Size(571, 411);
            this.gbTerrainAttributes.TabIndex = 3;
            this.gbTerrainAttributes.TabStop = false;
            this.gbTerrainAttributes.Text = "Terrain Attributes";
            // 
            // gbRestrictionAttributes
            // 
            this.gbRestrictionAttributes.Controls.Add(this.lblENCostPerTurn);
            this.gbRestrictionAttributes.Controls.Add(this.txtENCostPerTurn);
            this.gbRestrictionAttributes.Controls.Add(this.lblENCostToMove);
            this.gbRestrictionAttributes.Controls.Add(this.txtENCostToMove);
            this.gbRestrictionAttributes.Controls.Add(this.dgvTerrainRanks);
            this.gbRestrictionAttributes.Controls.Add(this.lblMovementCost);
            this.gbRestrictionAttributes.Controls.Add(this.txtMovementCost);
            this.gbRestrictionAttributes.Controls.Add(this.lblRestrictionCategory);
            this.gbRestrictionAttributes.Controls.Add(this.txtEntryCost);
            this.gbRestrictionAttributes.Controls.Add(this.lblEntryCost);
            this.gbRestrictionAttributes.Controls.Add(this.cbRestrictionCategory);
            this.gbRestrictionAttributes.Location = new System.Drawing.Point(365, 19);
            this.gbRestrictionAttributes.Name = "gbRestrictionAttributes";
            this.gbRestrictionAttributes.Size = new System.Drawing.Size(200, 392);
            this.gbRestrictionAttributes.TabIndex = 4;
            this.gbRestrictionAttributes.TabStop = false;
            this.gbRestrictionAttributes.Text = "Restriction Attributes";
            // 
            // lblENCostPerTurn
            // 
            this.lblENCostPerTurn.AutoSize = true;
            this.lblENCostPerTurn.Location = new System.Drawing.Point(6, 266);
            this.lblENCostPerTurn.Name = "lblENCostPerTurn";
            this.lblENCostPerTurn.Size = new System.Drawing.Size(85, 13);
            this.lblENCostPerTurn.TabIndex = 40;
            this.lblENCostPerTurn.Text = "EN Cost per turn";
            // 
            // txtENCostPerTurn
            // 
            this.txtENCostPerTurn.Location = new System.Drawing.Point(139, 264);
            this.txtENCostPerTurn.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtENCostPerTurn.Name = "txtENCostPerTurn";
            this.txtENCostPerTurn.Size = new System.Drawing.Size(55, 20);
            this.txtENCostPerTurn.TabIndex = 39;
            this.txtENCostPerTurn.ValueChanged += new System.EventHandler(this.OnTerrainAttributeChanged);
            // 
            // lblENCostToMove
            // 
            this.lblENCostToMove.AutoSize = true;
            this.lblENCostToMove.Location = new System.Drawing.Point(6, 240);
            this.lblENCostToMove.Name = "lblENCostToMove";
            this.lblENCostToMove.Size = new System.Drawing.Size(88, 13);
            this.lblENCostToMove.TabIndex = 38;
            this.lblENCostToMove.Text = "EN Cost to Move";
            // 
            // txtENCostToMove
            // 
            this.txtENCostToMove.DecimalPlaces = 2;
            this.txtENCostToMove.Location = new System.Drawing.Point(139, 238);
            this.txtENCostToMove.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtENCostToMove.Name = "txtENCostToMove";
            this.txtENCostToMove.Size = new System.Drawing.Size(55, 20);
            this.txtENCostToMove.TabIndex = 37;
            this.txtENCostToMove.ValueChanged += new System.EventHandler(this.OnTerrainAttributeChanged);
            // 
            // dgvTerrainRanks
            // 
            this.dgvTerrainRanks.AllowUserToAddRows = false;
            this.dgvTerrainRanks.AllowUserToDeleteRows = false;
            this.dgvTerrainRanks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTerrainRanks.ColumnHeadersVisible = false;
            this.dgvTerrainRanks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Rank,
            this.Value});
            this.dgvTerrainRanks.Cursor = System.Windows.Forms.Cursors.Default;
            this.dgvTerrainRanks.Location = new System.Drawing.Point(6, 59);
            this.dgvTerrainRanks.MultiSelect = false;
            this.dgvTerrainRanks.Name = "dgvTerrainRanks";
            this.dgvTerrainRanks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTerrainRanks.ShowCellErrors = false;
            this.dgvTerrainRanks.ShowEditingIcon = false;
            this.dgvTerrainRanks.Size = new System.Drawing.Size(188, 121);
            this.dgvTerrainRanks.TabIndex = 36;
            this.dgvTerrainRanks.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTerrainRanks_CellClick);
            this.dgvTerrainRanks.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvTerrainRanks_CurrentCellDirtyStateChanged);
            this.dgvTerrainRanks.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvTerrainRanks_RowPostPaint);
            // 
            // Rank
            // 
            this.Rank.HeaderText = "Rank";
            this.Rank.Name = "Rank";
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            // 
            // lblMovementCost
            // 
            this.lblMovementCost.AutoSize = true;
            this.lblMovementCost.Location = new System.Drawing.Point(6, 214);
            this.lblMovementCost.Name = "lblMovementCost";
            this.lblMovementCost.Size = new System.Drawing.Size(81, 13);
            this.lblMovementCost.TabIndex = 5;
            this.lblMovementCost.Text = "Movement Cost";
            // 
            // txtMovementCost
            // 
            this.txtMovementCost.DecimalPlaces = 2;
            this.txtMovementCost.Location = new System.Drawing.Point(139, 212);
            this.txtMovementCost.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtMovementCost.Name = "txtMovementCost";
            this.txtMovementCost.Size = new System.Drawing.Size(55, 20);
            this.txtMovementCost.TabIndex = 4;
            this.txtMovementCost.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtMovementCost.ValueChanged += new System.EventHandler(this.OnTerrainAttributeChanged);
            // 
            // lblRestrictionCategory
            // 
            this.lblRestrictionCategory.AutoSize = true;
            this.lblRestrictionCategory.Location = new System.Drawing.Point(6, 16);
            this.lblRestrictionCategory.Name = "lblRestrictionCategory";
            this.lblRestrictionCategory.Size = new System.Drawing.Size(49, 13);
            this.lblRestrictionCategory.TabIndex = 4;
            this.lblRestrictionCategory.Text = "Category";
            // 
            // txtEntryCost
            // 
            this.txtEntryCost.DecimalPlaces = 2;
            this.txtEntryCost.Location = new System.Drawing.Point(139, 186);
            this.txtEntryCost.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.txtEntryCost.Name = "txtEntryCost";
            this.txtEntryCost.Size = new System.Drawing.Size(55, 20);
            this.txtEntryCost.TabIndex = 2;
            this.txtEntryCost.ValueChanged += new System.EventHandler(this.OnTerrainAttributeChanged);
            // 
            // lblEntryCost
            // 
            this.lblEntryCost.AutoSize = true;
            this.lblEntryCost.Location = new System.Drawing.Point(6, 188);
            this.lblEntryCost.Name = "lblEntryCost";
            this.lblEntryCost.Size = new System.Drawing.Size(55, 13);
            this.lblEntryCost.TabIndex = 3;
            this.lblEntryCost.Text = "Entry Cost";
            // 
            // cbRestrictionCategory
            // 
            this.cbRestrictionCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRestrictionCategory.FormattingEnabled = true;
            this.cbRestrictionCategory.Location = new System.Drawing.Point(6, 32);
            this.cbRestrictionCategory.Name = "cbRestrictionCategory";
            this.cbRestrictionCategory.Size = new System.Drawing.Size(163, 21);
            this.cbRestrictionCategory.TabIndex = 6;
            this.cbRestrictionCategory.SelectedIndexChanged += new System.EventHandler(this.OnTerrainAttributeChanged);
            // 
            // gbTerrainRestrictions
            // 
            this.gbTerrainRestrictions.Controls.Add(this.tvTerrainRestrictions);
            this.gbTerrainRestrictions.Controls.Add(this.btnAddSubTerrainRestriction);
            this.gbTerrainRestrictions.Controls.Add(this.btnRemoveTerrainRestriction);
            this.gbTerrainRestrictions.Controls.Add(this.btnAddTerrainRestriction);
            this.gbTerrainRestrictions.Location = new System.Drawing.Point(181, 19);
            this.gbTerrainRestrictions.Name = "gbTerrainRestrictions";
            this.gbTerrainRestrictions.Size = new System.Drawing.Size(178, 401);
            this.gbTerrainRestrictions.TabIndex = 2;
            this.gbTerrainRestrictions.TabStop = false;
            this.gbTerrainRestrictions.Text = "Restrictions";
            // 
            // tvTerrainRestrictions
            // 
            this.tvTerrainRestrictions.Location = new System.Drawing.Point(6, 19);
            this.tvTerrainRestrictions.Name = "tvTerrainRestrictions";
            this.tvTerrainRestrictions.Size = new System.Drawing.Size(166, 293);
            this.tvTerrainRestrictions.TabIndex = 8;
            this.tvTerrainRestrictions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvTerrainRestrictions_AfterSelect);
            // 
            // btnAddSubTerrainRestriction
            // 
            this.btnAddSubTerrainRestriction.Location = new System.Drawing.Point(6, 361);
            this.btnAddSubTerrainRestriction.Name = "btnAddSubTerrainRestriction";
            this.btnAddSubTerrainRestriction.Size = new System.Drawing.Size(166, 23);
            this.btnAddSubTerrainRestriction.TabIndex = 7;
            this.btnAddSubTerrainRestriction.Text = "Add sub category";
            this.btnAddSubTerrainRestriction.UseVisualStyleBackColor = true;
            this.btnAddSubTerrainRestriction.Click += new System.EventHandler(this.btnAddSubTerrainRestriction_Click);
            // 
            // btnRemoveTerrainRestriction
            // 
            this.btnRemoveTerrainRestriction.Location = new System.Drawing.Point(97, 332);
            this.btnRemoveTerrainRestriction.Name = "btnRemoveTerrainRestriction";
            this.btnRemoveTerrainRestriction.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveTerrainRestriction.TabIndex = 5;
            this.btnRemoveTerrainRestriction.Text = "Remove";
            this.btnRemoveTerrainRestriction.UseVisualStyleBackColor = true;
            this.btnRemoveTerrainRestriction.Click += new System.EventHandler(this.btnRemoveTerrainRestriction_Click);
            // 
            // btnAddTerrainRestriction
            // 
            this.btnAddTerrainRestriction.Location = new System.Drawing.Point(6, 332);
            this.btnAddTerrainRestriction.Name = "btnAddTerrainRestriction";
            this.btnAddTerrainRestriction.Size = new System.Drawing.Size(75, 23);
            this.btnAddTerrainRestriction.TabIndex = 4;
            this.btnAddTerrainRestriction.Text = "Add";
            this.btnAddTerrainRestriction.UseVisualStyleBackColor = true;
            this.btnAddTerrainRestriction.Click += new System.EventHandler(this.btnAddTerrainRestriction_Click);
            // 
            // gbUnitTypes
            // 
            this.gbUnitTypes.Controls.Add(this.lblUnitType);
            this.gbUnitTypes.Controls.Add(this.txtUnitType);
            this.gbUnitTypes.Controls.Add(this.btnRemoveUnitType);
            this.gbUnitTypes.Controls.Add(this.btnAddUnitType);
            this.gbUnitTypes.Controls.Add(this.lsUnitTypes);
            this.gbUnitTypes.Location = new System.Drawing.Point(12, 27);
            this.gbUnitTypes.Name = "gbUnitTypes";
            this.gbUnitTypes.Size = new System.Drawing.Size(170, 411);
            this.gbUnitTypes.TabIndex = 35;
            this.gbUnitTypes.TabStop = false;
            this.gbUnitTypes.Text = "Unit Types";
            // 
            // lblUnitType
            // 
            this.lblUnitType.AutoSize = true;
            this.lblUnitType.Location = new System.Drawing.Point(6, 338);
            this.lblUnitType.Name = "lblUnitType";
            this.lblUnitType.Size = new System.Drawing.Size(35, 13);
            this.lblUnitType.TabIndex = 3;
            this.lblUnitType.Text = "Name";
            // 
            // txtUnitType
            // 
            this.txtUnitType.Location = new System.Drawing.Point(6, 354);
            this.txtUnitType.Name = "txtUnitType";
            this.txtUnitType.Size = new System.Drawing.Size(158, 20);
            this.txtUnitType.TabIndex = 3;
            this.txtUnitType.TextChanged += new System.EventHandler(this.txtUnitType_TextChanged);
            // 
            // btnRemoveUnitType
            // 
            this.btnRemoveUnitType.Location = new System.Drawing.Point(87, 380);
            this.btnRemoveUnitType.Name = "btnRemoveUnitType";
            this.btnRemoveUnitType.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveUnitType.TabIndex = 2;
            this.btnRemoveUnitType.Text = "Remove";
            this.btnRemoveUnitType.UseVisualStyleBackColor = true;
            this.btnRemoveUnitType.Click += new System.EventHandler(this.btnRemoveUnitType_Click);
            // 
            // btnAddUnitType
            // 
            this.btnAddUnitType.Location = new System.Drawing.Point(6, 380);
            this.btnAddUnitType.Name = "btnAddUnitType";
            this.btnAddUnitType.Size = new System.Drawing.Size(75, 23);
            this.btnAddUnitType.TabIndex = 1;
            this.btnAddUnitType.Text = "Add";
            this.btnAddUnitType.UseVisualStyleBackColor = true;
            this.btnAddUnitType.Click += new System.EventHandler(this.btnAddUnitType_Click);
            // 
            // lsUnitTypes
            // 
            this.lsUnitTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsUnitTypes.FormattingEnabled = true;
            this.lsUnitTypes.Location = new System.Drawing.Point(6, 19);
            this.lsUnitTypes.Name = "lsUnitTypes";
            this.lsUnitTypes.Size = new System.Drawing.Size(158, 316);
            this.lsUnitTypes.TabIndex = 1;
            this.lsUnitTypes.SelectedIndexChanged += new System.EventHandler(this.lsUnitTypes_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(942, 24);
            this.menuStrip1.TabIndex = 36;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // TerrainAndUnitTypesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 450);
            this.Controls.Add(this.gbUnitTypes);
            this.Controls.Add(this.gbTerrainAttributes);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TerrainAndUnitTypesEditor";
            this.Text = "Terrain_Types_Editor";
            this.gbTerrainTypes.ResumeLayout(false);
            this.gbTerrainTypes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWallHardness)).EndInit();
            this.gbTerrainAttributes.ResumeLayout(false);
            this.gbRestrictionAttributes.ResumeLayout(false);
            this.gbRestrictionAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtENCostPerTurn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtENCostToMove)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTerrainRanks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMovementCost)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtEntryCost)).EndInit();
            this.gbTerrainRestrictions.ResumeLayout(false);
            this.gbUnitTypes.ResumeLayout(false);
            this.gbUnitTypes.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTerrainTypes;
        private System.Windows.Forms.Label lblTerrainTypeName;
        private System.Windows.Forms.TextBox txtTerrainTypeName;
        private System.Windows.Forms.Button btnRemoveTerrainType;
        private System.Windows.Forms.Button btnAddTerrainType;
        private System.Windows.Forms.GroupBox gbTerrainAttributes;
        private System.Windows.Forms.GroupBox gbTerrainRestrictions;
        private System.Windows.Forms.Label lblRestrictionCategory;
        private System.Windows.Forms.ComboBox cbRestrictionCategory;
        private System.Windows.Forms.Button btnRemoveTerrainRestriction;
        private System.Windows.Forms.Button btnAddTerrainRestriction;
        private System.Windows.Forms.GroupBox gbRestrictionAttributes;
        private System.Windows.Forms.Label lblWallHardness;
        private System.Windows.Forms.NumericUpDown txtWallHardness;
        private System.Windows.Forms.TreeView tvTerrainRestrictions;
        private System.Windows.Forms.Button btnAddSubTerrainRestriction;
        private System.Windows.Forms.Label lblMovementCost;
        private System.Windows.Forms.NumericUpDown txtMovementCost;
        private System.Windows.Forms.Label lblEntryCost;
        private System.Windows.Forms.NumericUpDown txtEntryCost;
        private System.Windows.Forms.ListBox lsTerrainTypes;
        private System.Windows.Forms.GroupBox gbUnitTypes;
        private System.Windows.Forms.Label lblUnitType;
        private System.Windows.Forms.TextBox txtUnitType;
        private System.Windows.Forms.Button btnRemoveUnitType;
        private System.Windows.Forms.Button btnAddUnitType;
        private System.Windows.Forms.ListBox lsUnitTypes;
        private System.Windows.Forms.Label lblUnitMovementTypeAnnulationName;
        private System.Windows.Forms.TextBox txtUnitMovementTypeAnnulationName;
        private System.Windows.Forms.Label lblUnitMovementTypeActivationName;
        private System.Windows.Forms.TextBox txtUnitMovementTypeActivationName;
        private System.Windows.Forms.DataGridView dgvTerrainRanks;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rank;
        private System.Windows.Forms.DataGridViewComboBoxColumn Value;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.Label lblENCostPerTurn;
        private System.Windows.Forms.NumericUpDown txtENCostPerTurn;
        private System.Windows.Forms.Label lblENCostToMove;
        private System.Windows.Forms.NumericUpDown txtENCostToMove;
    }
}