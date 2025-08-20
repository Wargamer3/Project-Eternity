
namespace ProjectEternity.Editors.ConquestMapEditor
{
    partial class ConquestSpawnUserControl
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
            this.tabSpawns = new System.Windows.Forms.TabPage();
            this.pgUnit = new System.Windows.Forms.PropertyGrid();
            this.gbUnits = new System.Windows.Forms.GroupBox();
            this.lvUnits = new System.Windows.Forms.ListView();
            this.gbFilter = new System.Windows.Forms.GroupBox();
            this.lblMoveType = new System.Windows.Forms.Label();
            this.cbMoveType = new System.Windows.Forms.ComboBox();
            this.lblFactions = new System.Windows.Forms.Label();
            this.cbFactions = new System.Windows.Forms.ComboBox();
            this.tabBuildings = new System.Windows.Forms.TabPage();
            this.pgBuilding = new System.Windows.Forms.PropertyGrid();
            this.gbBuildings = new System.Windows.Forms.GroupBox();
            this.lvBuildings = new System.Windows.Forms.ListView();
            this.lblBuildingFaction = new System.Windows.Forms.Label();
            this.cbBuildingFaction = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabSpawns.SuspendLayout();
            this.gbUnits.SuspendLayout();
            this.gbFilter.SuspendLayout();
            this.tabBuildings.SuspendLayout();
            this.gbBuildings.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSpawns);
            this.tabControl1.Controls.Add(this.tabBuildings);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(339, 620);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSpawns
            // 
            this.tabSpawns.Controls.Add(this.pgUnit);
            this.tabSpawns.Controls.Add(this.gbUnits);
            this.tabSpawns.Controls.Add(this.gbFilter);
            this.tabSpawns.Controls.Add(this.lblFactions);
            this.tabSpawns.Controls.Add(this.cbFactions);
            this.tabSpawns.Location = new System.Drawing.Point(4, 22);
            this.tabSpawns.Name = "tabSpawns";
            this.tabSpawns.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpawns.Size = new System.Drawing.Size(331, 594);
            this.tabSpawns.TabIndex = 0;
            this.tabSpawns.Text = "Spawns";
            this.tabSpawns.UseVisualStyleBackColor = true;
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
            // gbUnits
            // 
            this.gbUnits.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUnits.Controls.Add(this.lvUnits);
            this.gbUnits.Location = new System.Drawing.Point(6, 107);
            this.gbUnits.Name = "gbUnits";
            this.gbUnits.Size = new System.Drawing.Size(319, 248);
            this.gbUnits.TabIndex = 6;
            this.gbUnits.TabStop = false;
            this.gbUnits.Text = "Units";
            // 
            // lvUnits
            // 
            this.lvUnits.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvUnits.HideSelection = false;
            this.lvUnits.Location = new System.Drawing.Point(6, 19);
            this.lvUnits.Name = "lvUnits";
            this.lvUnits.Size = new System.Drawing.Size(307, 223);
            this.lvUnits.TabIndex = 0;
            this.lvUnits.UseCompatibleStateImageBehavior = false;
            // 
            // gbFilter
            // 
            this.gbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFilter.Controls.Add(this.lblMoveType);
            this.gbFilter.Controls.Add(this.cbMoveType);
            this.gbFilter.Location = new System.Drawing.Point(6, 36);
            this.gbFilter.Name = "gbFilter";
            this.gbFilter.Size = new System.Drawing.Size(319, 65);
            this.gbFilter.TabIndex = 5;
            this.gbFilter.TabStop = false;
            this.gbFilter.Text = "Filter";
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
            // cbMoveType
            // 
            this.cbMoveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMoveType.FormattingEnabled = true;
            this.cbMoveType.Location = new System.Drawing.Point(6, 32);
            this.cbMoveType.Name = "cbMoveType";
            this.cbMoveType.Size = new System.Drawing.Size(61, 21);
            this.cbMoveType.TabIndex = 3;
            // 
            // lblFactions
            // 
            this.lblFactions.AutoSize = true;
            this.lblFactions.Location = new System.Drawing.Point(6, 9);
            this.lblFactions.Name = "lblFactions";
            this.lblFactions.Size = new System.Drawing.Size(47, 13);
            this.lblFactions.TabIndex = 2;
            this.lblFactions.Text = "Factions";
            // 
            // cbFactions
            // 
            this.cbFactions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFactions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFactions.FormattingEnabled = true;
            this.cbFactions.Location = new System.Drawing.Point(59, 9);
            this.cbFactions.Name = "cbFactions";
            this.cbFactions.Size = new System.Drawing.Size(266, 21);
            this.cbFactions.TabIndex = 1;
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
            this.tabBuildings.Size = new System.Drawing.Size(331, 594);
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
            // ConquestSpawnUserControl
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "ConquestSpawnUserControl";
            this.Size = new System.Drawing.Size(349, 635);
            this.tabControl1.ResumeLayout(false);
            this.tabSpawns.ResumeLayout(false);
            this.tabSpawns.PerformLayout();
            this.gbUnits.ResumeLayout(false);
            this.gbFilter.ResumeLayout(false);
            this.gbFilter.PerformLayout();
            this.tabBuildings.ResumeLayout(false);
            this.tabBuildings.PerformLayout();
            this.gbBuildings.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblFactions;
        private System.Windows.Forms.Label lblMoveType;
        private System.Windows.Forms.GroupBox gbUnits;
        private System.Windows.Forms.GroupBox gbFilter;
        public System.Windows.Forms.TabPage tabSpawns;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.ListView lvUnits;
        public System.Windows.Forms.ComboBox cbFactions;
        public System.Windows.Forms.ComboBox cbMoveType;
        public System.Windows.Forms.PropertyGrid pgUnit;
        private System.Windows.Forms.TabPage tabBuildings;
        public System.Windows.Forms.PropertyGrid pgBuilding;
        private System.Windows.Forms.GroupBox gbBuildings;
        public System.Windows.Forms.ListView lvBuildings;
        private System.Windows.Forms.Label lblBuildingFaction;
        public System.Windows.Forms.ComboBox cbBuildingFaction;
    }
}
