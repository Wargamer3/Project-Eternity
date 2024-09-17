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
            this.gbConsumableAttributes = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gbConsumables = new System.Windows.Forms.GroupBox();
            this.lvConsumables = new System.Windows.Forms.ListView();
            this.btnDeleteConsumable = new System.Windows.Forms.Button();
            this.btnAddConsumable = new System.Windows.Forms.Button();
            this.gbConsumableAttributes.SuspendLayout();
            this.gbConsumables.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabToolBox
            // 
            this.tabToolBox.Size = new System.Drawing.Size(256, 521);
            // 
            // BattleMapViewer
            // 
            this.BattleMapViewer.Size = new System.Drawing.Size(660, 521);
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 0;
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
            // 
            // lvConsumables
            // 
            this.lvConsumables.HideSelection = false;
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
            // ProjectEternitySorcererStreetEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(928, 573);
            this.Name = "ProjectEternitySorcererStreetEditor";
            this.Text = "Form1";
            this.gbConsumableAttributes.ResumeLayout(false);
            this.gbConsumableAttributes.PerformLayout();
            this.gbConsumables.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gbConsumableAttributes;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gbConsumables;
        private System.Windows.Forms.ListView lvConsumables;
        private System.Windows.Forms.Button btnDeleteConsumable;
        private System.Windows.Forms.Button btnAddConsumable;
    }
}