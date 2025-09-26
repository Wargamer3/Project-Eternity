
namespace ProjectEternity.Editors.AttackEditor
{
    partial class DestructibleTilesEditor
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
            this.gbUniqueType = new System.Windows.Forms.GroupBox();
            this.btnRemoveUniqueType = new System.Windows.Forms.Button();
            this.lsUniqueType = new System.Windows.Forms.ListBox();
            this.btnAddUniqueType = new System.Windows.Forms.Button();
            this.gbStatistics = new System.Windows.Forms.GroupBox();
            this.lblDamage = new System.Windows.Forms.Label();
            this.txtDamage = new System.Windows.Forms.NumericUpDown();
            this.btnAddGenericType = new System.Windows.Forms.Button();
            this.btnRemoveGenericType = new System.Windows.Forms.Button();
            this.gbGenericType = new System.Windows.Forms.GroupBox();
            this.lsGenericType = new System.Windows.Forms.ListBox();
            this.gbUniqueType.SuspendLayout();
            this.gbStatistics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDamage)).BeginInit();
            this.gbGenericType.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbUniqueType
            // 
            this.gbUniqueType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUniqueType.Controls.Add(this.btnRemoveUniqueType);
            this.gbUniqueType.Controls.Add(this.lsUniqueType);
            this.gbUniqueType.Controls.Add(this.btnAddUniqueType);
            this.gbUniqueType.Location = new System.Drawing.Point(218, 12);
            this.gbUniqueType.MinimumSize = new System.Drawing.Size(200, 230);
            this.gbUniqueType.Name = "gbUniqueType";
            this.gbUniqueType.Size = new System.Drawing.Size(200, 230);
            this.gbUniqueType.TabIndex = 0;
            this.gbUniqueType.TabStop = false;
            this.gbUniqueType.Text = "Unique Type";
            // 
            // btnRemoveUniqueType
            // 
            this.btnRemoveUniqueType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveUniqueType.Location = new System.Drawing.Point(119, 198);
            this.btnRemoveUniqueType.Name = "btnRemoveUniqueType";
            this.btnRemoveUniqueType.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveUniqueType.TabIndex = 3;
            this.btnRemoveUniqueType.Text = "Remove";
            this.btnRemoveUniqueType.UseVisualStyleBackColor = true;
            this.btnRemoveUniqueType.Click += new System.EventHandler(this.btnRemoveUniqueType_Click);
            // 
            // lsUniqueType
            // 
            this.lsUniqueType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsUniqueType.FormattingEnabled = true;
            this.lsUniqueType.Location = new System.Drawing.Point(6, 19);
            this.lsUniqueType.Name = "lsUniqueType";
            this.lsUniqueType.Size = new System.Drawing.Size(188, 173);
            this.lsUniqueType.TabIndex = 1;
            this.lsUniqueType.SelectedIndexChanged += new System.EventHandler(this.lsUniqueType_SelectedIndexChanged);
            // 
            // btnAddUniqueType
            // 
            this.btnAddUniqueType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddUniqueType.Location = new System.Drawing.Point(6, 198);
            this.btnAddUniqueType.Name = "btnAddUniqueType";
            this.btnAddUniqueType.Size = new System.Drawing.Size(75, 23);
            this.btnAddUniqueType.TabIndex = 4;
            this.btnAddUniqueType.Text = "Add";
            this.btnAddUniqueType.UseVisualStyleBackColor = true;
            this.btnAddUniqueType.Click += new System.EventHandler(this.btnAddUniqueType_Click);
            // 
            // gbStatistics
            // 
            this.gbStatistics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbStatistics.Controls.Add(this.lblDamage);
            this.gbStatistics.Controls.Add(this.txtDamage);
            this.gbStatistics.Location = new System.Drawing.Point(12, 248);
            this.gbStatistics.Name = "gbStatistics";
            this.gbStatistics.Size = new System.Drawing.Size(406, 59);
            this.gbStatistics.TabIndex = 1;
            this.gbStatistics.TabStop = false;
            this.gbStatistics.Text = "Statistics";
            // 
            // lblDamage
            // 
            this.lblDamage.AutoSize = true;
            this.lblDamage.Location = new System.Drawing.Point(6, 16);
            this.lblDamage.Name = "lblDamage";
            this.lblDamage.Size = new System.Drawing.Size(47, 13);
            this.lblDamage.TabIndex = 1;
            this.lblDamage.Text = "Damage";
            // 
            // txtDamage
            // 
            this.txtDamage.Location = new System.Drawing.Point(59, 14);
            this.txtDamage.Name = "txtDamage";
            this.txtDamage.Size = new System.Drawing.Size(120, 20);
            this.txtDamage.TabIndex = 0;
            this.txtDamage.ValueChanged += new System.EventHandler(this.txtDamage_ValueChanged);
            // 
            // btnAddGenericType
            // 
            this.btnAddGenericType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddGenericType.Location = new System.Drawing.Point(6, 198);
            this.btnAddGenericType.Name = "btnAddGenericType";
            this.btnAddGenericType.Size = new System.Drawing.Size(75, 23);
            this.btnAddGenericType.TabIndex = 2;
            this.btnAddGenericType.Text = "Add";
            this.btnAddGenericType.UseVisualStyleBackColor = true;
            this.btnAddGenericType.Click += new System.EventHandler(this.btnAddGenericType_Click);
            // 
            // btnRemoveGenericType
            // 
            this.btnRemoveGenericType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveGenericType.Location = new System.Drawing.Point(119, 198);
            this.btnRemoveGenericType.Name = "btnRemoveGenericType";
            this.btnRemoveGenericType.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveGenericType.TabIndex = 2;
            this.btnRemoveGenericType.Text = "Remove";
            this.btnRemoveGenericType.UseVisualStyleBackColor = true;
            this.btnRemoveGenericType.Click += new System.EventHandler(this.btnRemoveGenericType_Click);
            // 
            // gbGenericType
            // 
            this.gbGenericType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbGenericType.Controls.Add(this.btnRemoveGenericType);
            this.gbGenericType.Controls.Add(this.lsGenericType);
            this.gbGenericType.Controls.Add(this.btnAddGenericType);
            this.gbGenericType.Location = new System.Drawing.Point(12, 12);
            this.gbGenericType.MinimumSize = new System.Drawing.Size(200, 230);
            this.gbGenericType.Name = "gbGenericType";
            this.gbGenericType.Size = new System.Drawing.Size(200, 230);
            this.gbGenericType.TabIndex = 2;
            this.gbGenericType.TabStop = false;
            this.gbGenericType.Text = "Generic Type";
            // 
            // lsGenericType
            // 
            this.lsGenericType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsGenericType.FormattingEnabled = true;
            this.lsGenericType.Location = new System.Drawing.Point(6, 19);
            this.lsGenericType.Name = "lsGenericType";
            this.lsGenericType.Size = new System.Drawing.Size(188, 173);
            this.lsGenericType.TabIndex = 2;
            this.lsGenericType.SelectedIndexChanged += new System.EventHandler(this.lsGenericType_SelectedIndexChanged);
            // 
            // DestructibleTilesEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 319);
            this.Controls.Add(this.gbGenericType);
            this.Controls.Add(this.gbStatistics);
            this.Controls.Add(this.gbUniqueType);
            this.Name = "DestructibleTilesEditor";
            this.Text = "Destructible Tiles";
            this.gbUniqueType.ResumeLayout(false);
            this.gbStatistics.ResumeLayout(false);
            this.gbStatistics.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDamage)).EndInit();
            this.gbGenericType.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbUniqueType;
        private System.Windows.Forms.Button btnRemoveUniqueType;
        private System.Windows.Forms.ListBox lsUniqueType;
        private System.Windows.Forms.Button btnAddUniqueType;
        private System.Windows.Forms.GroupBox gbStatistics;
        private System.Windows.Forms.Label lblDamage;
        private System.Windows.Forms.NumericUpDown txtDamage;
        private System.Windows.Forms.Button btnAddGenericType;
        private System.Windows.Forms.Button btnRemoveGenericType;
        private System.Windows.Forms.GroupBox gbGenericType;
        private System.Windows.Forms.ListBox lsGenericType;
    }
}