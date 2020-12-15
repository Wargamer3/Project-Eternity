namespace ProjectEternity.Editors.SorcererStreetMapEditor
{
    partial class ProjectEternitySorcererStreetEditor
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
            this.tabZones = new System.Windows.Forms.TabPage();
            this.ZoneBox1 = new System.Windows.Forms.GroupBox();
            this.lblZoneColor = new System.Windows.Forms.Label();
            this.panZoneColor = new System.Windows.Forms.Panel();
            this.lblNbUnitToControl = new System.Windows.Forms.Label();
            this.spnNumberOfUnitToControl = new System.Windows.Forms.NumericUpDown();
            this.txtZoneName = new System.Windows.Forms.TextBox();
            this.lblZoneName = new System.Windows.Forms.Label();
            this.ZoneBox2 = new System.Windows.Forms.GroupBox();
            this.lvZones = new System.Windows.Forms.ListView();
            this.btnZone = new System.Windows.Forms.Button();
            this.btnAddZone = new System.Windows.Forms.Button();
            this.tabConsumables = new System.Windows.Forms.TabPage();
            this.gbConsumableAttributes = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbConsumables = new System.Windows.Forms.GroupBox();
            this.lvConsumables = new System.Windows.Forms.ListView();
            this.btnDeleteConsumable = new System.Windows.Forms.Button();
            this.btnAddConsumable = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabToolBox.SuspendLayout();
            this.tabZones.SuspendLayout();
            this.ZoneBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnNumberOfUnitToControl)).BeginInit();
            this.ZoneBox2.SuspendLayout();
            this.tabConsumables.SuspendLayout();
            this.gbConsumableAttributes.SuspendLayout();
            this.gbConsumables.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabToolBox
            // 
            this.tabToolBox.Controls.Add(this.tabZones);
            this.tabToolBox.Controls.Add(this.tabConsumables);
            this.tabToolBox.Size = new System.Drawing.Size(248, 521);
            this.tabToolBox.Controls.SetChildIndex(this.tabConsumables, 0);
            this.tabToolBox.Controls.SetChildIndex(this.tabZones, 0);
            // 
            // tabZones
            // 
            this.tabZones.Controls.Add(this.ZoneBox1);
            this.tabZones.Controls.Add(this.ZoneBox2);
            this.tabZones.Location = new System.Drawing.Point(4, 22);
            this.tabZones.Name = "tabZones";
            this.tabZones.Padding = new System.Windows.Forms.Padding(3);
            this.tabZones.Size = new System.Drawing.Size(240, 495);
            this.tabZones.TabIndex = 1;
            this.tabZones.Text = "Zones";
            this.tabZones.UseVisualStyleBackColor = true;
            // 
            // ZoneBox1
            // 
            this.ZoneBox1.Controls.Add(this.lblZoneColor);
            this.ZoneBox1.Controls.Add(this.panZoneColor);
            this.ZoneBox1.Controls.Add(this.lblNbUnitToControl);
            this.ZoneBox1.Controls.Add(this.spnNumberOfUnitToControl);
            this.ZoneBox1.Controls.Add(this.txtZoneName);
            this.ZoneBox1.Controls.Add(this.lblZoneName);
            this.ZoneBox1.Location = new System.Drawing.Point(0, 183);
            this.ZoneBox1.Name = "ZoneBox1";
            this.ZoneBox1.Size = new System.Drawing.Size(240, 312);
            this.ZoneBox1.TabIndex = 1;
            this.ZoneBox1.TabStop = false;
            this.ZoneBox1.Text = "Zone attributes";
            // 
            // lblZoneColor
            // 
            this.lblZoneColor.AutoSize = true;
            this.lblZoneColor.Location = new System.Drawing.Point(6, 73);
            this.lblZoneColor.Name = "lblZoneColor";
            this.lblZoneColor.Size = new System.Drawing.Size(61, 13);
            this.lblZoneColor.TabIndex = 5;
            this.lblZoneColor.Text = "Zone color:";
            // 
            // panZoneColor
            // 
            this.panZoneColor.BackColor = System.Drawing.Color.White;
            this.panZoneColor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panZoneColor.Enabled = false;
            this.panZoneColor.Location = new System.Drawing.Point(204, 71);
            this.panZoneColor.Name = "panZoneColor";
            this.panZoneColor.Size = new System.Drawing.Size(30, 30);
            this.panZoneColor.TabIndex = 4;
            // 
            // lblNbUnitToControl
            // 
            this.lblNbUnitToControl.AutoSize = true;
            this.lblNbUnitToControl.Location = new System.Drawing.Point(6, 47);
            this.lblNbUnitToControl.Name = "lblNbUnitToControl";
            this.lblNbUnitToControl.Size = new System.Drawing.Size(133, 13);
            this.lblNbUnitToControl.TabIndex = 3;
            this.lblNbUnitToControl.Text = "Number of Units to control:";
            // 
            // spnNumberOfUnitToControl
            // 
            this.spnNumberOfUnitToControl.Enabled = false;
            this.spnNumberOfUnitToControl.Location = new System.Drawing.Point(167, 45);
            this.spnNumberOfUnitToControl.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.spnNumberOfUnitToControl.Name = "spnNumberOfUnitToControl";
            this.spnNumberOfUnitToControl.Size = new System.Drawing.Size(67, 20);
            this.spnNumberOfUnitToControl.TabIndex = 2;
            // 
            // txtZoneName
            // 
            this.txtZoneName.Enabled = false;
            this.txtZoneName.Location = new System.Drawing.Point(101, 19);
            this.txtZoneName.Name = "txtZoneName";
            this.txtZoneName.Size = new System.Drawing.Size(133, 20);
            this.txtZoneName.TabIndex = 1;
            // 
            // lblZoneName
            // 
            this.lblZoneName.AutoSize = true;
            this.lblZoneName.Location = new System.Drawing.Point(6, 22);
            this.lblZoneName.Name = "lblZoneName";
            this.lblZoneName.Size = new System.Drawing.Size(64, 13);
            this.lblZoneName.TabIndex = 0;
            this.lblZoneName.Text = "Zone name:";
            // 
            // ZoneBox2
            // 
            this.ZoneBox2.Controls.Add(this.lvZones);
            this.ZoneBox2.Controls.Add(this.btnZone);
            this.ZoneBox2.Controls.Add(this.btnAddZone);
            this.ZoneBox2.Location = new System.Drawing.Point(0, 0);
            this.ZoneBox2.Name = "ZoneBox2";
            this.ZoneBox2.Size = new System.Drawing.Size(240, 177);
            this.ZoneBox2.TabIndex = 0;
            this.ZoneBox2.TabStop = false;
            this.ZoneBox2.Text = "Zones list";
            // 
            // lvZones
            // 
            this.lvZones.Location = new System.Drawing.Point(6, 20);
            this.lvZones.Name = "lvZones";
            this.lvZones.Size = new System.Drawing.Size(228, 107);
            this.lvZones.TabIndex = 3;
            this.lvZones.UseCompatibleStateImageBehavior = false;
            this.lvZones.View = System.Windows.Forms.View.List;
            // 
            // btnZone
            // 
            this.btnZone.Location = new System.Drawing.Point(123, 133);
            this.btnZone.Name = "btnZone";
            this.btnZone.Size = new System.Drawing.Size(111, 23);
            this.btnZone.TabIndex = 2;
            this.btnZone.Text = "Delete Zone";
            this.btnZone.UseVisualStyleBackColor = true;
            // 
            // btnAddZone
            // 
            this.btnAddZone.Location = new System.Drawing.Point(6, 133);
            this.btnAddZone.Name = "btnAddZone";
            this.btnAddZone.Size = new System.Drawing.Size(111, 23);
            this.btnAddZone.TabIndex = 1;
            this.btnAddZone.Text = "Add Zone";
            this.btnAddZone.UseVisualStyleBackColor = true;
            // 
            // tabConsumables
            // 
            this.tabConsumables.Controls.Add(this.gbConsumableAttributes);
            this.tabConsumables.Controls.Add(this.gbConsumables);
            this.tabConsumables.Location = new System.Drawing.Point(4, 22);
            this.tabConsumables.Name = "tabConsumables";
            this.tabConsumables.Size = new System.Drawing.Size(240, 495);
            this.tabConsumables.TabIndex = 4;
            this.tabConsumables.Text = "Consumables";
            this.tabConsumables.UseVisualStyleBackColor = true;
            // 
            // gbConsumableAttributes
            // 
            this.gbConsumableAttributes.Controls.Add(this.comboBox1);
            this.gbConsumableAttributes.Controls.Add(this.label4);
            this.gbConsumableAttributes.Location = new System.Drawing.Point(0, 183);
            this.gbConsumableAttributes.Name = "gbConsumableAttributes";
            this.gbConsumableAttributes.Size = new System.Drawing.Size(240, 312);
            this.gbConsumableAttributes.TabIndex = 3;
            this.gbConsumableAttributes.TabStop = false;
            this.gbConsumableAttributes.Text = "Consumable attributes";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Consumable type:";
            // 
            // gbConsumables
            // 
            this.gbConsumables.Controls.Add(this.lvConsumables);
            this.gbConsumables.Controls.Add(this.btnDeleteConsumable);
            this.gbConsumables.Controls.Add(this.btnAddConsumable);
            this.gbConsumables.Location = new System.Drawing.Point(0, 0);
            this.gbConsumables.Name = "gbConsumables";
            this.gbConsumables.Size = new System.Drawing.Size(240, 177);
            this.gbConsumables.TabIndex = 2;
            this.gbConsumables.TabStop = false;
            this.gbConsumables.Text = "Zones list";
            // 
            // lvConsumables
            // 
            this.lvConsumables.Location = new System.Drawing.Point(6, 20);
            this.lvConsumables.Name = "lvConsumables";
            this.lvConsumables.Size = new System.Drawing.Size(228, 107);
            this.lvConsumables.TabIndex = 3;
            this.lvConsumables.UseCompatibleStateImageBehavior = false;
            this.lvConsumables.View = System.Windows.Forms.View.List;
            // 
            // btnDeleteConsumable
            // 
            this.btnDeleteConsumable.Location = new System.Drawing.Point(123, 133);
            this.btnDeleteConsumable.Name = "btnDeleteConsumable";
            this.btnDeleteConsumable.Size = new System.Drawing.Size(111, 23);
            this.btnDeleteConsumable.TabIndex = 2;
            this.btnDeleteConsumable.Text = "Delete Consumable";
            this.btnDeleteConsumable.UseVisualStyleBackColor = true;
            // 
            // btnAddConsumable
            // 
            this.btnAddConsumable.Location = new System.Drawing.Point(6, 133);
            this.btnAddConsumable.Name = "btnAddConsumable";
            this.btnAddConsumable.Size = new System.Drawing.Size(111, 23);
            this.btnAddConsumable.TabIndex = 1;
            this.btnAddConsumable.Text = "Add Consumable";
            this.btnAddConsumable.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(113, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // ProjectEternityWorldMapEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(928, 573);
            this.Name = "ProjectEternityWorldMapEditor";
            this.Text = "Form1";
            this.tabToolBox.ResumeLayout(false);
            this.tabZones.ResumeLayout(false);
            this.ZoneBox1.ResumeLayout(false);
            this.ZoneBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spnNumberOfUnitToControl)).EndInit();
            this.ZoneBox2.ResumeLayout(false);
            this.tabConsumables.ResumeLayout(false);
            this.gbConsumableAttributes.ResumeLayout(false);
            this.gbConsumableAttributes.PerformLayout();
            this.gbConsumables.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tabZones;
        private System.Windows.Forms.GroupBox ZoneBox2;
        private System.Windows.Forms.Button btnZone;
        private System.Windows.Forms.Button btnAddZone;
        private System.Windows.Forms.GroupBox ZoneBox1;
        private System.Windows.Forms.TextBox txtZoneName;
        private System.Windows.Forms.Label lblZoneName;
        private System.Windows.Forms.Label lblNbUnitToControl;
        private System.Windows.Forms.NumericUpDown spnNumberOfUnitToControl;
        private System.Windows.Forms.ListView lvZones;
        private System.Windows.Forms.Label lblZoneColor;
        private System.Windows.Forms.Panel panZoneColor;
        private System.Windows.Forms.TabPage tabConsumables;
        private System.Windows.Forms.GroupBox gbConsumableAttributes;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbConsumables;
        private System.Windows.Forms.ListView lvConsumables;
        private System.Windows.Forms.Button btnDeleteConsumable;
        private System.Windows.Forms.Button btnAddConsumable;
    }
}