
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
            this.tabAreas = new System.Windows.Forms.TabPage();
            this.gbAreasTiles = new System.Windows.Forms.GroupBox();
            this.lvAreasTiles = new System.Windows.Forms.ListView();
            this.gbAreas = new System.Windows.Forms.GroupBox();
            this.txtAreaName = new System.Windows.Forms.TextBox();
            this.lblAreaName = new System.Windows.Forms.Label();
            this.btnRemoveArea = new System.Windows.Forms.Button();
            this.btnAddArea = new System.Windows.Forms.Button();
            this.lblAreas = new System.Windows.Forms.Label();
            this.cbAreas = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabAreas.SuspendLayout();
            this.gbAreasTiles.SuspendLayout();
            this.gbAreas.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabAreas);
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(339, 620);
            this.tabControl1.TabIndex = 0;
            // 
            // tabAreas
            // 
            this.tabAreas.Controls.Add(this.gbAreasTiles);
            this.tabAreas.Controls.Add(this.gbAreas);
            this.tabAreas.Location = new System.Drawing.Point(4, 22);
            this.tabAreas.Name = "tabAreas";
            this.tabAreas.Padding = new System.Windows.Forms.Padding(3);
            this.tabAreas.Size = new System.Drawing.Size(331, 594);
            this.tabAreas.TabIndex = 2;
            this.tabAreas.Text = "Areas";
            this.tabAreas.UseVisualStyleBackColor = true;
            // 
            // gbAreasTiles
            // 
            this.gbAreasTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAreasTiles.Controls.Add(this.lvAreasTiles);
            this.gbAreasTiles.Location = new System.Drawing.Point(6, 131);
            this.gbAreasTiles.Name = "gbAreasTiles";
            this.gbAreasTiles.Size = new System.Drawing.Size(319, 457);
            this.gbAreasTiles.TabIndex = 11;
            this.gbAreasTiles.TabStop = false;
            this.gbAreasTiles.Text = "Tiles";
            // 
            // lvAreasTiles
            // 
            this.lvAreasTiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvAreasTiles.HideSelection = false;
            this.lvAreasTiles.Location = new System.Drawing.Point(6, 19);
            this.lvAreasTiles.Name = "lvAreasTiles";
            this.lvAreasTiles.Size = new System.Drawing.Size(307, 432);
            this.lvAreasTiles.TabIndex = 0;
            this.lvAreasTiles.UseCompatibleStateImageBehavior = false;
            // 
            // gbAreas
            // 
            this.gbAreas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbAreas.Controls.Add(this.txtAreaName);
            this.gbAreas.Controls.Add(this.lblAreaName);
            this.gbAreas.Controls.Add(this.btnRemoveArea);
            this.gbAreas.Controls.Add(this.btnAddArea);
            this.gbAreas.Controls.Add(this.lblAreas);
            this.gbAreas.Controls.Add(this.cbAreas);
            this.gbAreas.Location = new System.Drawing.Point(6, 6);
            this.gbAreas.Name = "gbAreas";
            this.gbAreas.Size = new System.Drawing.Size(319, 119);
            this.gbAreas.TabIndex = 5;
            this.gbAreas.TabStop = false;
            this.gbAreas.Text = "Aeras";
            // 
            // txtAreaName
            // 
            this.txtAreaName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAreaName.Location = new System.Drawing.Point(75, 63);
            this.txtAreaName.Name = "txtAreaName";
            this.txtAreaName.Size = new System.Drawing.Size(238, 20);
            this.txtAreaName.TabIndex = 5;
            // 
            // lblAreaName
            // 
            this.lblAreaName.AutoSize = true;
            this.lblAreaName.Location = new System.Drawing.Point(6, 66);
            this.lblAreaName.Name = "lblAreaName";
            this.lblAreaName.Size = new System.Drawing.Size(63, 13);
            this.lblAreaName.TabIndex = 4;
            this.lblAreaName.Text = "Area Name:";
            // 
            // btnRemoveArea
            // 
            this.btnRemoveArea.Location = new System.Drawing.Point(75, 89);
            this.btnRemoveArea.Name = "btnRemoveArea";
            this.btnRemoveArea.Size = new System.Drawing.Size(60, 23);
            this.btnRemoveArea.TabIndex = 3;
            this.btnRemoveArea.Text = "Remove";
            this.btnRemoveArea.UseVisualStyleBackColor = true;
            // 
            // btnAddArea
            // 
            this.btnAddArea.Location = new System.Drawing.Point(9, 89);
            this.btnAddArea.Name = "btnAddArea";
            this.btnAddArea.Size = new System.Drawing.Size(60, 23);
            this.btnAddArea.TabIndex = 2;
            this.btnAddArea.Text = "Add";
            this.btnAddArea.UseVisualStyleBackColor = true;
            // 
            // lblAreas
            // 
            this.lblAreas.AutoSize = true;
            this.lblAreas.Location = new System.Drawing.Point(6, 16);
            this.lblAreas.Name = "lblAreas";
            this.lblAreas.Size = new System.Drawing.Size(23, 13);
            this.lblAreas.TabIndex = 1;
            this.lblAreas.Text = "List";
            // 
            // cbAreas
            // 
            this.cbAreas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAreas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAreas.FormattingEnabled = true;
            this.cbAreas.Location = new System.Drawing.Point(9, 32);
            this.cbAreas.Name = "cbAreas";
            this.cbAreas.Size = new System.Drawing.Size(304, 21);
            this.cbAreas.TabIndex = 0;
            // 
            // ExtraTabsUserControl
            // 
            this.Controls.Add(this.tabControl1);
            this.Name = "ExtraTabsUserControl";
            this.Size = new System.Drawing.Size(349, 635);
            this.tabControl1.ResumeLayout(false);
            this.tabAreas.ResumeLayout(false);
            this.gbAreasTiles.ResumeLayout(false);
            this.gbAreas.ResumeLayout(false);
            this.gbAreas.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabAreas;
        private System.Windows.Forms.GroupBox gbAreasTiles;
        public System.Windows.Forms.ListView lvAreasTiles;
        private System.Windows.Forms.GroupBox gbAreas;
        public System.Windows.Forms.Button btnRemoveArea;
        public System.Windows.Forms.Button btnAddArea;
        private System.Windows.Forms.Label lblAreas;
        public System.Windows.Forms.ComboBox cbAreas;
        private System.Windows.Forms.Label lblAreaName;
        public System.Windows.Forms.TextBox txtAreaName;
    }
}
