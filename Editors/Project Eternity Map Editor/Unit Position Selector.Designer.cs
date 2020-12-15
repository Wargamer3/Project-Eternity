namespace ProjectEternity.Editors.MapEditor
{
    partial class Unit_Position_Selector
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
            this.gbUnit = new System.Windows.Forms.GroupBox();
            this.gbStatistics = new System.Windows.Forms.GroupBox();
            this.txtBaseMovement = new System.Windows.Forms.TextBox();
            this.txtBaseMobility = new System.Windows.Forms.TextBox();
            this.txtBaseArmor = new System.Windows.Forms.TextBox();
            this.txtBaseEN = new System.Windows.Forms.TextBox();
            this.txtBaseHP = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lstPosition = new System.Windows.Forms.ListBox();
            this.bthChooseUnit = new System.Windows.Forms.Button();
            this.txtUnit = new System.Windows.Forms.TextBox();
            this.lblUnit = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.gbCharacter = new System.Windows.Forms.GroupBox();
            this.btnRemoveCharacter = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnAddCharacter = new System.Windows.Forms.Button();
            this.lstCharacters = new System.Windows.Forms.ListBox();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbUnit.SuspendLayout();
            this.gbStatistics.SuspendLayout();
            this.gbCharacter.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbUnit
            // 
            this.gbUnit.Controls.Add(this.gbStatistics);
            this.gbUnit.Controls.Add(this.lstPosition);
            this.gbUnit.Controls.Add(this.bthChooseUnit);
            this.gbUnit.Controls.Add(this.txtUnit);
            this.gbUnit.Controls.Add(this.lblUnit);
            this.gbUnit.Controls.Add(this.lblPosition);
            this.gbUnit.Location = new System.Drawing.Point(12, 12);
            this.gbUnit.Name = "gbUnit";
            this.gbUnit.Size = new System.Drawing.Size(427, 208);
            this.gbUnit.TabIndex = 0;
            this.gbUnit.TabStop = false;
            this.gbUnit.Text = "Unit";
            // 
            // gbStatistics
            // 
            this.gbStatistics.Controls.Add(this.txtBaseMovement);
            this.gbStatistics.Controls.Add(this.txtBaseMobility);
            this.gbStatistics.Controls.Add(this.txtBaseArmor);
            this.gbStatistics.Controls.Add(this.txtBaseEN);
            this.gbStatistics.Controls.Add(this.txtBaseHP);
            this.gbStatistics.Controls.Add(this.label5);
            this.gbStatistics.Controls.Add(this.label4);
            this.gbStatistics.Controls.Add(this.label3);
            this.gbStatistics.Controls.Add(this.label2);
            this.gbStatistics.Controls.Add(this.label6);
            this.gbStatistics.Location = new System.Drawing.Point(9, 46);
            this.gbStatistics.Name = "gbStatistics";
            this.gbStatistics.Size = new System.Drawing.Size(251, 152);
            this.gbStatistics.TabIndex = 5;
            this.gbStatistics.TabStop = false;
            this.gbStatistics.Text = "Statistics";
            // 
            // txtBaseMovement
            // 
            this.txtBaseMovement.Location = new System.Drawing.Point(99, 123);
            this.txtBaseMovement.Name = "txtBaseMovement";
            this.txtBaseMovement.ReadOnly = true;
            this.txtBaseMovement.Size = new System.Drawing.Size(143, 20);
            this.txtBaseMovement.TabIndex = 19;
            // 
            // txtBaseMobility
            // 
            this.txtBaseMobility.Location = new System.Drawing.Point(99, 97);
            this.txtBaseMobility.Name = "txtBaseMobility";
            this.txtBaseMobility.ReadOnly = true;
            this.txtBaseMobility.Size = new System.Drawing.Size(143, 20);
            this.txtBaseMobility.TabIndex = 18;
            // 
            // txtBaseArmor
            // 
            this.txtBaseArmor.Location = new System.Drawing.Point(99, 71);
            this.txtBaseArmor.Name = "txtBaseArmor";
            this.txtBaseArmor.ReadOnly = true;
            this.txtBaseArmor.Size = new System.Drawing.Size(143, 20);
            this.txtBaseArmor.TabIndex = 17;
            // 
            // txtBaseEN
            // 
            this.txtBaseEN.Location = new System.Drawing.Point(99, 45);
            this.txtBaseEN.Name = "txtBaseEN";
            this.txtBaseEN.ReadOnly = true;
            this.txtBaseEN.Size = new System.Drawing.Size(143, 20);
            this.txtBaseEN.TabIndex = 16;
            // 
            // txtBaseHP
            // 
            this.txtBaseHP.Location = new System.Drawing.Point(99, 19);
            this.txtBaseHP.Name = "txtBaseHP";
            this.txtBaseHP.ReadOnly = true;
            this.txtBaseHP.Size = new System.Drawing.Size(143, 20);
            this.txtBaseHP.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 126);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Base Movement:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Base Mobility:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Base Armor:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Base EN:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Base HP:";
            // 
            // lstPosition
            // 
            this.lstPosition.FormattingEnabled = true;
            this.lstPosition.Location = new System.Drawing.Point(266, 38);
            this.lstPosition.Name = "lstPosition";
            this.lstPosition.Size = new System.Drawing.Size(150, 160);
            this.lstPosition.TabIndex = 4;
            // 
            // bthChooseUnit
            // 
            this.bthChooseUnit.Location = new System.Drawing.Point(179, 17);
            this.bthChooseUnit.Name = "bthChooseUnit";
            this.bthChooseUnit.Size = new System.Drawing.Size(75, 23);
            this.bthChooseUnit.TabIndex = 2;
            this.bthChooseUnit.Text = "Choose Unit";
            this.bthChooseUnit.UseVisualStyleBackColor = true;
            this.bthChooseUnit.Click += new System.EventHandler(this.bthChooseUnit_Click);
            // 
            // txtUnit
            // 
            this.txtUnit.Location = new System.Drawing.Point(41, 19);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.ReadOnly = true;
            this.txtUnit.Size = new System.Drawing.Size(118, 20);
            this.txtUnit.TabIndex = 3;
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(6, 22);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(29, 13);
            this.lblUnit.TabIndex = 2;
            this.lblUnit.Text = "Unit:";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoSize = true;
            this.lblPosition.Location = new System.Drawing.Point(263, 22);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(52, 13);
            this.lblPosition.TabIndex = 2;
            this.lblPosition.Text = "Positions:";
            // 
            // gbCharacter
            // 
            this.gbCharacter.Controls.Add(this.btnRemoveCharacter);
            this.gbCharacter.Controls.Add(this.btnMoveDown);
            this.gbCharacter.Controls.Add(this.btnMoveUp);
            this.gbCharacter.Controls.Add(this.btnAddCharacter);
            this.gbCharacter.Controls.Add(this.lstCharacters);
            this.gbCharacter.Location = new System.Drawing.Point(445, 12);
            this.gbCharacter.Name = "gbCharacter";
            this.gbCharacter.Size = new System.Drawing.Size(260, 132);
            this.gbCharacter.TabIndex = 1;
            this.gbCharacter.TabStop = false;
            this.gbCharacter.Text = "Characters";
            // 
            // btnRemoveCharacter
            // 
            this.btnRemoveCharacter.Location = new System.Drawing.Point(165, 106);
            this.btnRemoveCharacter.Name = "btnRemoveCharacter";
            this.btnRemoveCharacter.Size = new System.Drawing.Size(89, 23);
            this.btnRemoveCharacter.TabIndex = 7;
            this.btnRemoveCharacter.Text = "Remove Character";
            this.btnRemoveCharacter.UseVisualStyleBackColor = true;
            this.btnRemoveCharacter.Click += new System.EventHandler(this.btnRemoveCharacter_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Location = new System.Drawing.Point(165, 77);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(89, 23);
            this.btnMoveDown.TabIndex = 6;
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.UseVisualStyleBackColor = true;
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Location = new System.Drawing.Point(165, 48);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(89, 23);
            this.btnMoveUp.TabIndex = 5;
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.UseVisualStyleBackColor = true;
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnAddCharacter
            // 
            this.btnAddCharacter.Location = new System.Drawing.Point(165, 19);
            this.btnAddCharacter.Name = "btnAddCharacter";
            this.btnAddCharacter.Size = new System.Drawing.Size(89, 23);
            this.btnAddCharacter.TabIndex = 4;
            this.btnAddCharacter.Text = "Add Character";
            this.btnAddCharacter.UseVisualStyleBackColor = true;
            this.btnAddCharacter.Click += new System.EventHandler(this.btnAddCharacter_Click);
            // 
            // lstCharacters
            // 
            this.lstCharacters.FormattingEnabled = true;
            this.lstCharacters.Location = new System.Drawing.Point(6, 19);
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.Size = new System.Drawing.Size(153, 108);
            this.lstCharacters.TabIndex = 2;
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(445, 150);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(113, 23);
            this.btnAccept.TabIndex = 2;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(592, 150);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(113, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Unit_Position_Selector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(728, 236);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.gbCharacter);
            this.Controls.Add(this.gbUnit);
            this.Name = "Unit_Position_Selector";
            this.Text = "Unit_Position_Selector";
            this.gbUnit.ResumeLayout(false);
            this.gbUnit.PerformLayout();
            this.gbStatistics.ResumeLayout(false);
            this.gbStatistics.PerformLayout();
            this.gbCharacter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbUnit;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.GroupBox gbCharacter;
        private System.Windows.Forms.Button bthChooseUnit;
        private System.Windows.Forms.TextBox txtUnit;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.ListBox lstCharacters;
        private System.Windows.Forms.Button btnRemoveCharacter;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnAddCharacter;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbStatistics;
        private System.Windows.Forms.ListBox lstPosition;
        private System.Windows.Forms.TextBox txtBaseMovement;
        private System.Windows.Forms.TextBox txtBaseMobility;
        private System.Windows.Forms.TextBox txtBaseArmor;
        private System.Windows.Forms.TextBox txtBaseEN;
        private System.Windows.Forms.TextBox txtBaseHP;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
    }
}