namespace ProjectEternity.Core.Editor
{
    partial class BaseEditorItem
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
            this.gbItemInformation = new System.Windows.Forms.GroupBox();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblID = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.gbItemInformation.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbItemInformation
            // 
            this.gbItemInformation.Controls.Add(this.txtID);
            this.gbItemInformation.Controls.Add(this.txtName);
            this.gbItemInformation.Controls.Add(this.lblID);
            this.gbItemInformation.Controls.Add(this.lblName);
            this.gbItemInformation.Location = new System.Drawing.Point(12, 27);
            this.gbItemInformation.Name = "gbItemInformation";
            this.gbItemInformation.Size = new System.Drawing.Size(548, 54);
            this.gbItemInformation.TabIndex = 3;
            this.gbItemInformation.TabStop = false;
            this.gbItemInformation.Text = "Item information";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(391, 19);
            this.txtID.Name = "txtID";
            this.txtID.Size = new System.Drawing.Size(151, 20);
            this.txtID.TabIndex = 1;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(50, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(308, 20);
            this.txtName.TabIndex = 0;
            // 
            // lblID
            // 
            this.lblID.AutoSize = true;
            this.lblID.Location = new System.Drawing.Point(364, 22);
            this.lblID.Name = "lblID";
            this.lblID.Size = new System.Drawing.Size(21, 13);
            this.lblID.TabIndex = 13;
            this.lblID.Text = "ID:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 22);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 10;
            this.lblName.Text = "Name:";
            // 
            // BaseEditorItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 438);
            this.Controls.Add(this.gbItemInformation);
            this.Name = "BaseEditorItem";
            this.Text = "Base_Editor_Item";
            this.gbItemInformation.ResumeLayout(false);
            this.gbItemInformation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.GroupBox gbItemInformation;
        protected System.Windows.Forms.TextBox txtID;
        protected System.Windows.Forms.TextBox txtName;
        protected System.Windows.Forms.Label lblID;
        protected System.Windows.Forms.Label lblName;
    }
}