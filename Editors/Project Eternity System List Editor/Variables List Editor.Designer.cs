
namespace ProjectEternity.Editors.SystemListEditor
{
    partial class VariablesListEditor
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
            this.gbParts = new System.Windows.Forms.GroupBox();
            this.btnMoveDownVariable = new System.Windows.Forms.Button();
            this.btnMoveUpVariable = new System.Windows.Forms.Button();
            this.btnRemoveVariable = new System.Windows.Forms.Button();
            this.lstVariables = new System.Windows.Forms.ListBox();
            this.btnAddVariable = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtVariableDefaultValue = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.gbParts.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(463, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbParts
            // 
            this.gbParts.Controls.Add(this.btnMoveDownVariable);
            this.gbParts.Controls.Add(this.btnMoveUpVariable);
            this.gbParts.Controls.Add(this.btnRemoveVariable);
            this.gbParts.Controls.Add(this.lstVariables);
            this.gbParts.Controls.Add(this.btnAddVariable);
            this.gbParts.Location = new System.Drawing.Point(12, 27);
            this.gbParts.Name = "gbParts";
            this.gbParts.Size = new System.Drawing.Size(183, 427);
            this.gbParts.TabIndex = 6;
            this.gbParts.TabStop = false;
            this.gbParts.Text = "Variables";
            // 
            // btnMoveDownVariable
            // 
            this.btnMoveDownVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDownVariable.Location = new System.Drawing.Point(6, 402);
            this.btnMoveDownVariable.Name = "btnMoveDownVariable";
            this.btnMoveDownVariable.Size = new System.Drawing.Size(171, 23);
            this.btnMoveDownVariable.TabIndex = 5;
            this.btnMoveDownVariable.Text = "Move Down Variable";
            this.btnMoveDownVariable.UseVisualStyleBackColor = true;
            this.btnMoveDownVariable.Click += new System.EventHandler(this.btnMoveDownVariable_Click);
            // 
            // btnMoveUpVariable
            // 
            this.btnMoveUpVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUpVariable.Location = new System.Drawing.Point(6, 373);
            this.btnMoveUpVariable.Name = "btnMoveUpVariable";
            this.btnMoveUpVariable.Size = new System.Drawing.Size(171, 23);
            this.btnMoveUpVariable.TabIndex = 4;
            this.btnMoveUpVariable.Text = "Move Up Variable";
            this.btnMoveUpVariable.UseVisualStyleBackColor = true;
            this.btnMoveUpVariable.Click += new System.EventHandler(this.btnMoveUpVariable_Click);
            // 
            // btnRemoveVariable
            // 
            this.btnRemoveVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveVariable.Location = new System.Drawing.Point(6, 344);
            this.btnRemoveVariable.Name = "btnRemoveVariable";
            this.btnRemoveVariable.Size = new System.Drawing.Size(171, 23);
            this.btnRemoveVariable.TabIndex = 3;
            this.btnRemoveVariable.Text = "Remove Variable";
            this.btnRemoveVariable.UseVisualStyleBackColor = true;
            this.btnRemoveVariable.Click += new System.EventHandler(this.btnRemoveVariable_Click);
            // 
            // lstVariables
            // 
            this.lstVariables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstVariables.FormattingEnabled = true;
            this.lstVariables.Location = new System.Drawing.Point(6, 19);
            this.lstVariables.Name = "lstVariables";
            this.lstVariables.Size = new System.Drawing.Size(171, 290);
            this.lstVariables.TabIndex = 0;
            this.lstVariables.SelectedIndexChanged += new System.EventHandler(this.lstVariables_SelectedIndexChanged);
            // 
            // btnAddVariable
            // 
            this.btnAddVariable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddVariable.Location = new System.Drawing.Point(6, 315);
            this.btnAddVariable.Name = "btnAddVariable";
            this.btnAddVariable.Size = new System.Drawing.Size(171, 23);
            this.btnAddVariable.TabIndex = 2;
            this.btnAddVariable.Text = "Add Variable";
            this.btnAddVariable.UseVisualStyleBackColor = true;
            this.btnAddVariable.Click += new System.EventHandler(this.btnAddVariable_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtVariableDefaultValue);
            this.groupBox1.Location = new System.Drawing.Point(201, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(252, 53);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Variable Default Value";
            // 
            // txtVariableDefaultValue
            // 
            this.txtVariableDefaultValue.Location = new System.Drawing.Point(6, 19);
            this.txtVariableDefaultValue.Name = "txtVariableDefaultValue";
            this.txtVariableDefaultValue.Size = new System.Drawing.Size(240, 20);
            this.txtVariableDefaultValue.TabIndex = 0;
            this.txtVariableDefaultValue.TextChanged += new System.EventHandler(this.txtVariableDefaultValue_TextChanged);
            // 
            // Variables_List_Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 465);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbParts);
            this.Controls.Add(this.menuStrip1);
            this.Name = "Variables_List_Editor";
            this.Text = "Variables List Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbParts.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbParts;
        private System.Windows.Forms.Button btnMoveDownVariable;
        private System.Windows.Forms.Button btnMoveUpVariable;
        private System.Windows.Forms.Button btnRemoveVariable;
        private System.Windows.Forms.ListBox lstVariables;
        private System.Windows.Forms.Button btnAddVariable;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtVariableDefaultValue;
    }
}