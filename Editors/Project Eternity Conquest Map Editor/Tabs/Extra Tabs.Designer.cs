
namespace ProjectEternity.Editors.ConquestMapEditor
{
    partial class ExtraTabsUserControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabUnits = new System.Windows.Forms.TabPage();
            this.pgUnit = new System.Windows.Forms.PropertyGrid();
            this.gbUnitsUnits = new System.Windows.Forms.GroupBox();
            this.lvUnitsUnits = new System.Windows.Forms.ListView();
            this.gbUnitsFilter = new System.Windows.Forms.GroupBox();
            this.lblMoveType = new System.Windows.Forms.Label();
            this.cbUnitsMoveType = new System.Windows.Forms.ComboBox();
            this.lblUnitsFactions = new System.Windows.Forms.Label();
            this.cbUnitsFactions = new System.Windows.Forms.ComboBox();
            this.tabBuildings = new System.Windows.Forms.TabPage();
            this.pgBuilding = new System.Windows.Forms.PropertyGrid();
            this.gbBuildings = new System.Windows.Forms.GroupBox();
            this.lvBuildings = new System.Windows.Forms.ListView();
            this.lblBuildingFaction = new System.Windows.Forms.Label();
            this.cbBuildingFaction = new System.Windows.Forms.ComboBox();
            this.tabScenery = new System.Windows.Forms.TabPage();
            this.gbSceneryTiletset = new System.Windows.Forms.GroupBox();
            this.btnScenery3DTileAttributes = new System.Windows.Forms.Button();
            this.btnSceneryTileAttributes = new System.Windows.Forms.Button();
            this.btnSceneryRemoveTileset = new System.Windows.Forms.Button();
            this.btnSceneryAddTileset = new System.Windows.Forms.Button();
            this.lblTileset = new System.Windows.Forms.Label();
            this.cboSceneryTilesets = new System.Windows.Forms.ComboBox();
            this.gbSceneryViewer = new System.Windows.Forms.GroupBox();
            this.lvIScenery = new System.Windows.Forms.ListView();
            this.gbSceneryAutotile = new System.Windows.Forms.GroupBox();
            this.btnSceneryRemoveAutotile = new System.Windows.Forms.Button();
            this.btnSceneryAddAutotile = new System.Windows.Forms.Button();
            this.lblAutotile = new System.Windows.Forms.Label();
            this.cboSceneryAutotile = new System.Windows.Forms.ComboBox();
            this.tabSpawns = new System.Windows.Forms.TabPage();
            this.txtSpawnsPlayer = new System.Windows.Forms.NumericUpDown();
            this.gbSpawnsBuildings = new System.Windows.Forms.GroupBox();
            this.lvSpawnsBuildings = new System.Windows.Forms.ListView();
            this.gbSpawnsUnits = new System.Windows.Forms.GroupBox();
            this.lvSpawnsUnits = new System.Windows.Forms.ListView();
            this.lblSpawnsPlayer = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabUnits.SuspendLayout();
            this.gbUnitsUnits.SuspendLayout();
            this.gbUnitsFilter.SuspendLayout();
            this.tabBuildings.SuspendLayout();
            this.gbBuildings.SuspendLayout();
            this.tabScenery.SuspendLayout();
            this.gbSceneryTiletset.SuspendLayout();
            this.gbSceneryViewer.SuspendLayout();
            this.gbSceneryAutotile.SuspendLayout();
            this.tabSpawns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpawnsPlayer)).BeginInit();
            this.gbSpawnsBuildings.SuspendLayout();
            this.gbSpawnsUnits.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabUnits);
            this.tabControl1.Controls.Add(this.tabBuildings);
            this.tabControl1.Controls.Add(this.tabScenery);
            this.tabControl1.Controls.Add(this.tabSpawns);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(339, 632);
            this.tabControl1.TabIndex = 0;
            // 
            // tabUnits
            // 
            this.tabUnits.Controls.Add(this.pgUnit);
            this.tabUnits.Controls.Add(this.gbUnitsUnits);
            this.tabUnits.Controls.Add(this.gbUnitsFilter);
            this.tabUnits.Controls.Add(this.lblUnitsFactions);
            this.tabUnits.Controls.Add(this.cbUnitsFactions);
            this.tabUnits.Location = new System.Drawing.Point(4, 22);
            this.tabUnits.Name = "tabUnits";
            this.tabUnits.Padding = new System.Windows.Forms.Padding(3);
            this.tabUnits.Size = new System.Drawing.Size(331, 606);
            this.tabUnits.TabIndex = 0;
            this.tabUnits.Text = "Units";
            this.tabUnits.UseVisualStyleBackColor = true;
            // 
            // pgUnit
            // 
            this.pgUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgUnit.Location = new System.Drawing.Point(6, 361);
            this.pgUnit.Name = "pgUnit";
            this.pgUnit.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgUnit.Size = new System.Drawing.Size(319, 227);
            this.pgUnit.TabIndex = 7;
            this.pgUnit.ToolbarVisible = false;
            // 
            // gbUnitsUnits
            // 
            this.gbUnitsUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUnitsUnits.Controls.Add(this.lvUnitsUnits);
            this.gbUnitsUnits.Location = new System.Drawing.Point(6, 107);
            this.gbUnitsUnits.Name = "gbUnitsUnits";
            this.gbUnitsUnits.Size = new System.Drawing.Size(319, 248);
            this.gbUnitsUnits.TabIndex = 6;
            this.gbUnitsUnits.TabStop = false;
            this.gbUnitsUnits.Text = "Units";
            // 
            // lvUnitsUnits
            // 
            this.lvUnitsUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvUnitsUnits.HideSelection = false;
            this.lvUnitsUnits.Location = new System.Drawing.Point(6, 19);
            this.lvUnitsUnits.Name = "lvUnitsUnits";
            this.lvUnitsUnits.Size = new System.Drawing.Size(307, 223);
            this.lvUnitsUnits.TabIndex = 0;
            this.lvUnitsUnits.UseCompatibleStateImageBehavior = false;
            // 
            // gbUnitsFilter
            // 
            this.gbUnitsFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUnitsFilter.Controls.Add(this.lblMoveType);
            this.gbUnitsFilter.Controls.Add(this.cbUnitsMoveType);
            this.gbUnitsFilter.Location = new System.Drawing.Point(6, 36);
            this.gbUnitsFilter.Name = "gbUnitsFilter";
            this.gbUnitsFilter.Size = new System.Drawing.Size(319, 65);
            this.gbUnitsFilter.TabIndex = 5;
            this.gbUnitsFilter.TabStop = false;
            this.gbUnitsFilter.Text = "Filter";
            // 
            // lblMoveType
            // 
            this.lblMoveType.AutoSize = true;
            this.lblMoveType.Location = new System.Drawing.Point(6, 16);
            this.lblMoveType.Name = "lblMoveType";
            this.lblMoveType.Size = new System.Drawing.Size(61, 13);
            this.lblMoveType.TabIndex = 4;
            this.lblMoveType.Text = "Move Type";
            // 
            // cbUnitsMoveType
            // 
            this.cbUnitsMoveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnitsMoveType.FormattingEnabled = true;
            this.cbUnitsMoveType.Location = new System.Drawing.Point(6, 32);
            this.cbUnitsMoveType.Name = "cbUnitsMoveType";
            this.cbUnitsMoveType.Size = new System.Drawing.Size(61, 21);
            this.cbUnitsMoveType.TabIndex = 3;
            // 
            // lblUnitsFactions
            // 
            this.lblUnitsFactions.AutoSize = true;
            this.lblUnitsFactions.Location = new System.Drawing.Point(6, 9);
            this.lblUnitsFactions.Name = "lblUnitsFactions";
            this.lblUnitsFactions.Size = new System.Drawing.Size(47, 13);
            this.lblUnitsFactions.TabIndex = 2;
            this.lblUnitsFactions.Text = "Factions";
            // 
            // cbUnitsFactions
            // 
            this.cbUnitsFactions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUnitsFactions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUnitsFactions.FormattingEnabled = true;
            this.cbUnitsFactions.Location = new System.Drawing.Point(59, 9);
            this.cbUnitsFactions.Name = "cbUnitsFactions";
            this.cbUnitsFactions.Size = new System.Drawing.Size(266, 21);
            this.cbUnitsFactions.TabIndex = 1;
            // 
            // tabBuildings
            // 
            this.tabBuildings.Controls.Add(this.pgBuilding);
            this.tabBuildings.Controls.Add(this.gbBuildings);
            this.tabBuildings.Controls.Add(this.lblBuildingFaction);
            this.tabBuildings.Controls.Add(this.cbBuildingFaction);
            this.tabBuildings.Location = new System.Drawing.Point(4, 22);
            this.tabBuildings.Name = "tabBuildings";
            this.tabBuildings.Padding = new System.Windows.Forms.Padding(3);
            this.tabBuildings.Size = new System.Drawing.Size(331, 606);
            this.tabBuildings.TabIndex = 1;
            this.tabBuildings.Text = "Buildings";
            this.tabBuildings.UseVisualStyleBackColor = true;
            // 
            // pgBuilding
            // 
            this.pgBuilding.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgBuilding.Location = new System.Drawing.Point(3, 361);
            this.pgBuilding.Name = "pgBuilding";
            this.pgBuilding.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgBuilding.Size = new System.Drawing.Size(319, 227);
            this.pgBuilding.TabIndex = 11;
            this.pgBuilding.ToolbarVisible = false;
            // 
            // gbBuildings
            // 
            this.gbBuildings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbBuildings.Controls.Add(this.lvBuildings);
            this.gbBuildings.Location = new System.Drawing.Point(6, 34);
            this.gbBuildings.Name = "gbBuildings";
            this.gbBuildings.Size = new System.Drawing.Size(319, 321);
            this.gbBuildings.TabIndex = 10;
            this.gbBuildings.TabStop = false;
            this.gbBuildings.Text = "Buildings";
            // 
            // lvBuildings
            // 
            this.lvBuildings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvBuildings.HideSelection = false;
            this.lvBuildings.Location = new System.Drawing.Point(6, 19);
            this.lvBuildings.Name = "lvBuildings";
            this.lvBuildings.Size = new System.Drawing.Size(307, 296);
            this.lvBuildings.TabIndex = 0;
            this.lvBuildings.UseCompatibleStateImageBehavior = false;
            // 
            // lblBuildingFaction
            // 
            this.lblBuildingFaction.AutoSize = true;
            this.lblBuildingFaction.Location = new System.Drawing.Point(6, 15);
            this.lblBuildingFaction.Name = "lblBuildingFaction";
            this.lblBuildingFaction.Size = new System.Drawing.Size(42, 13);
            this.lblBuildingFaction.TabIndex = 9;
            this.lblBuildingFaction.Text = "Faction";
            // 
            // cbBuildingFaction
            // 
            this.cbBuildingFaction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBuildingFaction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBuildingFaction.FormattingEnabled = true;
            this.cbBuildingFaction.Location = new System.Drawing.Point(59, 7);
            this.cbBuildingFaction.Name = "cbBuildingFaction";
            this.cbBuildingFaction.Size = new System.Drawing.Size(266, 21);
            this.cbBuildingFaction.TabIndex = 8;
            // 
            // tabScenery
            // 
            this.tabScenery.Controls.Add(this.gbSceneryTiletset);
            this.tabScenery.Controls.Add(this.gbSceneryViewer);
            this.tabScenery.Controls.Add(this.gbSceneryAutotile);
            this.tabScenery.Location = new System.Drawing.Point(4, 22);
            this.tabScenery.Name = "tabScenery";
            this.tabScenery.Padding = new System.Windows.Forms.Padding(3);
            this.tabScenery.Size = new System.Drawing.Size(331, 606);
            this.tabScenery.TabIndex = 2;
            this.tabScenery.Text = "Scenery";
            this.tabScenery.UseVisualStyleBackColor = true;
            // 
            // gbSceneryTiletset
            // 
            this.gbSceneryTiletset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSceneryTiletset.Controls.Add(this.btnScenery3DTileAttributes);
            this.gbSceneryTiletset.Controls.Add(this.btnSceneryTileAttributes);
            this.gbSceneryTiletset.Controls.Add(this.btnSceneryRemoveTileset);
            this.gbSceneryTiletset.Controls.Add(this.btnSceneryAddTileset);
            this.gbSceneryTiletset.Controls.Add(this.lblTileset);
            this.gbSceneryTiletset.Controls.Add(this.cboSceneryTilesets);
            this.gbSceneryTiletset.Location = new System.Drawing.Point(6, 378);
            this.gbSceneryTiletset.Name = "gbSceneryTiletset";
            this.gbSceneryTiletset.Size = new System.Drawing.Size(315, 122);
            this.gbSceneryTiletset.TabIndex = 12;
            this.gbSceneryTiletset.TabStop = false;
            this.gbSceneryTiletset.Text = "Tileset";
            // 
            // btnScenery3DTileAttributes
            // 
            this.btnScenery3DTileAttributes.Location = new System.Drawing.Point(160, 88);
            this.btnScenery3DTileAttributes.Name = "btnScenery3DTileAttributes";
            this.btnScenery3DTileAttributes.Size = new System.Drawing.Size(140, 23);
            this.btnScenery3DTileAttributes.TabIndex = 8;
            this.btnScenery3DTileAttributes.Text = "3D Tile Attributes";
            this.btnScenery3DTileAttributes.UseVisualStyleBackColor = true;
            // 
            // btnSceneryTileAttributes
            // 
            this.btnSceneryTileAttributes.Location = new System.Drawing.Point(9, 88);
            this.btnSceneryTileAttributes.Name = "btnSceneryTileAttributes";
            this.btnSceneryTileAttributes.Size = new System.Drawing.Size(140, 23);
            this.btnSceneryTileAttributes.TabIndex = 7;
            this.btnSceneryTileAttributes.Text = "Tile Attributes";
            this.btnSceneryTileAttributes.UseVisualStyleBackColor = true;
            // 
            // btnSceneryRemoveTileset
            // 
            this.btnSceneryRemoveTileset.Location = new System.Drawing.Point(75, 59);
            this.btnSceneryRemoveTileset.Name = "btnSceneryRemoveTileset";
            this.btnSceneryRemoveTileset.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryRemoveTileset.TabIndex = 3;
            this.btnSceneryRemoveTileset.Text = "Remove";
            this.btnSceneryRemoveTileset.UseVisualStyleBackColor = true;
            // 
            // btnSceneryAddTileset
            // 
            this.btnSceneryAddTileset.Location = new System.Drawing.Point(9, 59);
            this.btnSceneryAddTileset.Name = "btnSceneryAddTileset";
            this.btnSceneryAddTileset.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryAddTileset.TabIndex = 2;
            this.btnSceneryAddTileset.Text = "Add";
            this.btnSceneryAddTileset.UseVisualStyleBackColor = true;
            // 
            // lblTileset
            // 
            this.lblTileset.AutoSize = true;
            this.lblTileset.Location = new System.Drawing.Point(6, 16);
            this.lblTileset.Name = "lblTileset";
            this.lblTileset.Size = new System.Drawing.Size(23, 13);
            this.lblTileset.TabIndex = 1;
            this.lblTileset.Text = "List";
            // 
            // cboSceneryTilesets
            // 
            this.cboSceneryTilesets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSceneryTilesets.FormattingEnabled = true;
            this.cboSceneryTilesets.Location = new System.Drawing.Point(9, 32);
            this.cboSceneryTilesets.Name = "cboSceneryTilesets";
            this.cboSceneryTilesets.Size = new System.Drawing.Size(291, 21);
            this.cboSceneryTilesets.TabIndex = 0;
            // 
            // gbSceneryViewer
            // 
            this.gbSceneryViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSceneryViewer.Controls.Add(this.lvIScenery);
            this.gbSceneryViewer.Location = new System.Drawing.Point(6, 6);
            this.gbSceneryViewer.Name = "gbSceneryViewer";
            this.gbSceneryViewer.Size = new System.Drawing.Size(319, 366);
            this.gbSceneryViewer.TabIndex = 11;
            this.gbSceneryViewer.TabStop = false;
            this.gbSceneryViewer.Text = "Scenery";
            // 
            // lvIScenery
            // 
            this.lvIScenery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvIScenery.HideSelection = false;
            this.lvIScenery.Location = new System.Drawing.Point(6, 19);
            this.lvIScenery.Name = "lvIScenery";
            this.lvIScenery.Size = new System.Drawing.Size(307, 341);
            this.lvIScenery.TabIndex = 0;
            this.lvIScenery.UseCompatibleStateImageBehavior = false;
            // 
            // gbSceneryAutotile
            // 
            this.gbSceneryAutotile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSceneryAutotile.Controls.Add(this.btnSceneryRemoveAutotile);
            this.gbSceneryAutotile.Controls.Add(this.btnSceneryAddAutotile);
            this.gbSceneryAutotile.Controls.Add(this.lblAutotile);
            this.gbSceneryAutotile.Controls.Add(this.cboSceneryAutotile);
            this.gbSceneryAutotile.Location = new System.Drawing.Point(6, 506);
            this.gbSceneryAutotile.Name = "gbSceneryAutotile";
            this.gbSceneryAutotile.Size = new System.Drawing.Size(319, 94);
            this.gbSceneryAutotile.TabIndex = 5;
            this.gbSceneryAutotile.TabStop = false;
            this.gbSceneryAutotile.Text = "Autotile";
            // 
            // btnSceneryRemoveAutotile
            // 
            this.btnSceneryRemoveAutotile.Location = new System.Drawing.Point(72, 59);
            this.btnSceneryRemoveAutotile.Name = "btnSceneryRemoveAutotile";
            this.btnSceneryRemoveAutotile.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryRemoveAutotile.TabIndex = 3;
            this.btnSceneryRemoveAutotile.Text = "Remove";
            this.btnSceneryRemoveAutotile.UseVisualStyleBackColor = true;
            // 
            // btnSceneryAddAutotile
            // 
            this.btnSceneryAddAutotile.Location = new System.Drawing.Point(6, 59);
            this.btnSceneryAddAutotile.Name = "btnSceneryAddAutotile";
            this.btnSceneryAddAutotile.Size = new System.Drawing.Size(60, 23);
            this.btnSceneryAddAutotile.TabIndex = 2;
            this.btnSceneryAddAutotile.Text = "Add";
            this.btnSceneryAddAutotile.UseVisualStyleBackColor = true;
            // 
            // lblAutotile
            // 
            this.lblAutotile.AutoSize = true;
            this.lblAutotile.Location = new System.Drawing.Point(6, 16);
            this.lblAutotile.Name = "lblAutotile";
            this.lblAutotile.Size = new System.Drawing.Size(23, 13);
            this.lblAutotile.TabIndex = 1;
            this.lblAutotile.Text = "List";
            // 
            // cboSceneryAutotile
            // 
            this.cboSceneryAutotile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSceneryAutotile.FormattingEnabled = true;
            this.cboSceneryAutotile.Location = new System.Drawing.Point(9, 32);
            this.cboSceneryAutotile.Name = "cboSceneryAutotile";
            this.cboSceneryAutotile.Size = new System.Drawing.Size(279, 21);
            this.cboSceneryAutotile.TabIndex = 0;
            // 
            // tabSpawns
            // 
            this.tabSpawns.Controls.Add(this.txtSpawnsPlayer);
            this.tabSpawns.Controls.Add(this.gbSpawnsBuildings);
            this.tabSpawns.Controls.Add(this.gbSpawnsUnits);
            this.tabSpawns.Controls.Add(this.lblSpawnsPlayer);
            this.tabSpawns.Location = new System.Drawing.Point(4, 22);
            this.tabSpawns.Name = "tabSpawns";
            this.tabSpawns.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpawns.Size = new System.Drawing.Size(331, 606);
            this.tabSpawns.TabIndex = 3;
            this.tabSpawns.Text = "Spawns";
            this.tabSpawns.UseVisualStyleBackColor = true;
            // 
            // txtSpawnsPlayer
            // 
            this.txtSpawnsPlayer.Location = new System.Drawing.Point(48, 7);
            this.txtSpawnsPlayer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtSpawnsPlayer.Name = "txtSpawnsPlayer";
            this.txtSpawnsPlayer.Size = new System.Drawing.Size(60, 20);
            this.txtSpawnsPlayer.TabIndex = 5;
            this.txtSpawnsPlayer.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gbSpawnsBuildings
            // 
            this.gbSpawnsBuildings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSpawnsBuildings.Controls.Add(this.lvSpawnsBuildings);
            this.gbSpawnsBuildings.Location = new System.Drawing.Point(6, 290);
            this.gbSpawnsBuildings.Name = "gbSpawnsBuildings";
            this.gbSpawnsBuildings.Size = new System.Drawing.Size(319, 248);
            this.gbSpawnsBuildings.TabIndex = 11;
            this.gbSpawnsBuildings.TabStop = false;
            this.gbSpawnsBuildings.Text = "Buildings";
            // 
            // lvSpawnsBuildings
            // 
            this.lvSpawnsBuildings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSpawnsBuildings.HideSelection = false;
            this.lvSpawnsBuildings.Location = new System.Drawing.Point(6, 19);
            this.lvSpawnsBuildings.Name = "lvSpawnsBuildings";
            this.lvSpawnsBuildings.Size = new System.Drawing.Size(307, 223);
            this.lvSpawnsBuildings.TabIndex = 0;
            this.lvSpawnsBuildings.UseCompatibleStateImageBehavior = false;
            // 
            // gbSpawnsUnits
            // 
            this.gbSpawnsUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSpawnsUnits.Controls.Add(this.lvSpawnsUnits);
            this.gbSpawnsUnits.Location = new System.Drawing.Point(6, 36);
            this.gbSpawnsUnits.Name = "gbSpawnsUnits";
            this.gbSpawnsUnits.Size = new System.Drawing.Size(319, 248);
            this.gbSpawnsUnits.TabIndex = 10;
            this.gbSpawnsUnits.TabStop = false;
            this.gbSpawnsUnits.Text = "Units";
            // 
            // lvSpawnsUnits
            // 
            this.lvSpawnsUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvSpawnsUnits.HideSelection = false;
            this.lvSpawnsUnits.Location = new System.Drawing.Point(6, 19);
            this.lvSpawnsUnits.Name = "lvSpawnsUnits";
            this.lvSpawnsUnits.Size = new System.Drawing.Size(307, 223);
            this.lvSpawnsUnits.TabIndex = 0;
            this.lvSpawnsUnits.UseCompatibleStateImageBehavior = false;
            // 
            // lblSpawnsPlayer
            // 
            this.lblSpawnsPlayer.AutoSize = true;
            this.lblSpawnsPlayer.Location = new System.Drawing.Point(6, 9);
            this.lblSpawnsPlayer.Name = "lblSpawnsPlayer";
            this.lblSpawnsPlayer.Size = new System.Drawing.Size(36, 13);
            this.lblSpawnsPlayer.TabIndex = 8;
            this.lblSpawnsPlayer.Text = "Player";
            // 
            // ExtraTabsUserControl
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "ExtraTabsUserControl";
            this.Size = new System.Drawing.Size(349, 635);
            this.tabControl1.ResumeLayout(false);
            this.tabUnits.ResumeLayout(false);
            this.tabUnits.PerformLayout();
            this.gbUnitsUnits.ResumeLayout(false);
            this.gbUnitsFilter.ResumeLayout(false);
            this.gbUnitsFilter.PerformLayout();
            this.tabBuildings.ResumeLayout(false);
            this.tabBuildings.PerformLayout();
            this.gbBuildings.ResumeLayout(false);
            this.tabScenery.ResumeLayout(false);
            this.gbSceneryTiletset.ResumeLayout(false);
            this.gbSceneryTiletset.PerformLayout();
            this.gbSceneryViewer.ResumeLayout(false);
            this.gbSceneryAutotile.ResumeLayout(false);
            this.gbSceneryAutotile.PerformLayout();
            this.tabSpawns.ResumeLayout(false);
            this.tabSpawns.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtSpawnsPlayer)).EndInit();
            this.gbSpawnsBuildings.ResumeLayout(false);
            this.gbSpawnsUnits.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblUnitsFactions;
        private System.Windows.Forms.Label lblMoveType;
        private System.Windows.Forms.GroupBox gbUnitsUnits;
        private System.Windows.Forms.GroupBox gbUnitsFilter;
        public System.Windows.Forms.TabPage tabUnits;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.ListView lvUnitsUnits;
        public System.Windows.Forms.ComboBox cbUnitsFactions;
        public System.Windows.Forms.ComboBox cbUnitsMoveType;
        public System.Windows.Forms.PropertyGrid pgUnit;
        private System.Windows.Forms.TabPage tabBuildings;
        public System.Windows.Forms.PropertyGrid pgBuilding;
        private System.Windows.Forms.GroupBox gbBuildings;
        public System.Windows.Forms.ListView lvBuildings;
        private System.Windows.Forms.Label lblBuildingFaction;
        public System.Windows.Forms.ComboBox cbBuildingFaction;
        private System.Windows.Forms.TabPage tabScenery;
        private System.Windows.Forms.GroupBox gbSceneryViewer;
        public System.Windows.Forms.ListView lvIScenery;
        private System.Windows.Forms.GroupBox gbSceneryTiletset;
        public System.Windows.Forms.Button btnScenery3DTileAttributes;
        public System.Windows.Forms.Button btnSceneryTileAttributes;
        public System.Windows.Forms.Button btnSceneryRemoveTileset;
        public System.Windows.Forms.Button btnSceneryAddTileset;
        private System.Windows.Forms.Label lblTileset;
        public System.Windows.Forms.ComboBox cboSceneryTilesets;
        private System.Windows.Forms.GroupBox gbSceneryAutotile;
        public System.Windows.Forms.Button btnSceneryRemoveAutotile;
        public System.Windows.Forms.Button btnSceneryAddAutotile;
        private System.Windows.Forms.Label lblAutotile;
        public System.Windows.Forms.ComboBox cboSceneryAutotile;
        private System.Windows.Forms.TabPage tabSpawns;
        private System.Windows.Forms.GroupBox gbSpawnsBuildings;
        public System.Windows.Forms.ListView lvSpawnsBuildings;
        private System.Windows.Forms.GroupBox gbSpawnsUnits;
        public System.Windows.Forms.ListView lvSpawnsUnits;
        private System.Windows.Forms.Label lblSpawnsPlayer;
        public System.Windows.Forms.NumericUpDown txtSpawnsPlayer;
    }
}
