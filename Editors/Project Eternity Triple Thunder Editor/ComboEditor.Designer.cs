namespace ProjectEternity.Editors.ComboEditor
{
    partial class ComboEditor
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panAttackTimeline = new System.Windows.Forms.Panel();
            this.gbComboList = new System.Windows.Forms.GroupBox();
            this.btnSelectCombo = new System.Windows.Forms.Button();
            this.btnAddCombo = new System.Windows.Forms.Button();
            this.btnRemoveCombo = new System.Windows.Forms.Button();
            this.txtEndTime = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStartTime = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.lstComboList = new System.Windows.Forms.ListBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbInstantActivation = new System.Windows.Forms.CheckBox();
            this.cbAnimationType = new System.Windows.Forms.ComboBox();
            this.btnSelectAnimation = new System.Windows.Forms.Button();
            this.txtAnimationLength = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAnimationName = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.gbInput = new System.Windows.Forms.GroupBox();
            this.txtNextInputDelay = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMovementChoice = new System.Windows.Forms.ComboBox();
            this.btnAddInput = new System.Windows.Forms.Button();
            this.lstInput = new System.Windows.Forms.ListBox();
            this.btnRemoveInput = new System.Windows.Forms.Button();
            this.cbAttackChoice = new System.Windows.Forms.ComboBox();
            this.cbRotationType = new System.Windows.Forms.ComboBox();
            this.lblRotationType = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.gbComboList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndTime)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTime)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.gbInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNextInputDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.panAttackTimeline);
            this.groupBox1.Location = new System.Drawing.Point(12, 290);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(679, 52);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Attack Timeline";
            // 
            // panAttackTimeline
            // 
            this.panAttackTimeline.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panAttackTimeline.Location = new System.Drawing.Point(3, 16);
            this.panAttackTimeline.Name = "panAttackTimeline";
            this.panAttackTimeline.Size = new System.Drawing.Size(673, 33);
            this.panAttackTimeline.TabIndex = 0;
            // 
            // gbComboList
            // 
            this.gbComboList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbComboList.Controls.Add(this.btnSelectCombo);
            this.gbComboList.Controls.Add(this.btnAddCombo);
            this.gbComboList.Controls.Add(this.btnRemoveCombo);
            this.gbComboList.Controls.Add(this.txtEndTime);
            this.gbComboList.Controls.Add(this.label2);
            this.gbComboList.Controls.Add(this.txtStartTime);
            this.gbComboList.Controls.Add(this.label1);
            this.gbComboList.Controls.Add(this.lstComboList);
            this.gbComboList.Location = new System.Drawing.Point(259, 27);
            this.gbComboList.Name = "gbComboList";
            this.gbComboList.Size = new System.Drawing.Size(243, 257);
            this.gbComboList.TabIndex = 1;
            this.gbComboList.TabStop = false;
            this.gbComboList.Text = "Next Combos";
            // 
            // btnSelectCombo
            // 
            this.btnSelectCombo.Location = new System.Drawing.Point(134, 227);
            this.btnSelectCombo.Name = "btnSelectCombo";
            this.btnSelectCombo.Size = new System.Drawing.Size(103, 23);
            this.btnSelectCombo.TabIndex = 7;
            this.btnSelectCombo.Text = "Select combo";
            this.btnSelectCombo.UseVisualStyleBackColor = true;
            this.btnSelectCombo.Click += new System.EventHandler(this.btnSelectCombo_Click);
            // 
            // btnAddCombo
            // 
            this.btnAddCombo.Location = new System.Drawing.Point(6, 146);
            this.btnAddCombo.Name = "btnAddCombo";
            this.btnAddCombo.Size = new System.Drawing.Size(103, 23);
            this.btnAddCombo.TabIndex = 6;
            this.btnAddCombo.Text = "Add combo";
            this.btnAddCombo.UseVisualStyleBackColor = true;
            this.btnAddCombo.Click += new System.EventHandler(this.btnAddCombo_Click);
            // 
            // btnRemoveCombo
            // 
            this.btnRemoveCombo.Location = new System.Drawing.Point(134, 146);
            this.btnRemoveCombo.Name = "btnRemoveCombo";
            this.btnRemoveCombo.Size = new System.Drawing.Size(103, 23);
            this.btnRemoveCombo.TabIndex = 5;
            this.btnRemoveCombo.Text = "Remove combo";
            this.btnRemoveCombo.UseVisualStyleBackColor = true;
            this.btnRemoveCombo.Click += new System.EventHandler(this.btnRemoveCombo_Click);
            // 
            // txtEndTime
            // 
            this.txtEndTime.Location = new System.Drawing.Point(117, 201);
            this.txtEndTime.Name = "txtEndTime";
            this.txtEndTime.Size = new System.Drawing.Size(120, 20);
            this.txtEndTime.TabIndex = 4;
            this.txtEndTime.ValueChanged += new System.EventHandler(this.txtEndTime_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 203);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "End time";
            // 
            // txtStartTime
            // 
            this.txtStartTime.Location = new System.Drawing.Point(117, 175);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Size = new System.Drawing.Size(120, 20);
            this.txtStartTime.TabIndex = 2;
            this.txtStartTime.ValueChanged += new System.EventHandler(this.txtStartTime_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 177);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Start time";
            // 
            // lstComboList
            // 
            this.lstComboList.FormattingEnabled = true;
            this.lstComboList.Location = new System.Drawing.Point(6, 19);
            this.lstComboList.Name = "lstComboList";
            this.lstComboList.Size = new System.Drawing.Size(231, 121);
            this.lstComboList.TabIndex = 0;
            this.lstComboList.SelectedIndexChanged += new System.EventHandler(this.lstComboList_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblRotationType);
            this.groupBox3.Controls.Add(this.cbRotationType);
            this.groupBox3.Controls.Add(this.cbInstantActivation);
            this.groupBox3.Controls.Add(this.cbAnimationType);
            this.groupBox3.Controls.Add(this.btnSelectAnimation);
            this.groupBox3.Controls.Add(this.txtAnimationLength);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtAnimationName);
            this.groupBox3.Location = new System.Drawing.Point(12, 27);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(241, 148);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Animation";
            // 
            // cbInstantActivation
            // 
            this.cbInstantActivation.AutoSize = true;
            this.cbInstantActivation.Location = new System.Drawing.Point(9, 121);
            this.cbInstantActivation.Name = "cbInstantActivation";
            this.cbInstantActivation.Size = new System.Drawing.Size(107, 17);
            this.cbInstantActivation.TabIndex = 17;
            this.cbInstantActivation.Text = "Instant activation";
            this.cbInstantActivation.UseVisualStyleBackColor = true;
            // 
            // cbAnimationType
            // 
            this.cbAnimationType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAnimationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAnimationType.FormattingEnabled = true;
            this.cbAnimationType.Items.AddRange(new object[] {
            "Full Animation",
            "Partial Animation",
            "Skeleton Animation"});
            this.cbAnimationType.Location = new System.Drawing.Point(9, 71);
            this.cbAnimationType.Name = "cbAnimationType";
            this.cbAnimationType.Size = new System.Drawing.Size(117, 21);
            this.cbAnimationType.TabIndex = 15;
            // 
            // btnSelectAnimation
            // 
            this.btnSelectAnimation.Location = new System.Drawing.Point(132, 71);
            this.btnSelectAnimation.Name = "btnSelectAnimation";
            this.btnSelectAnimation.Size = new System.Drawing.Size(103, 23);
            this.btnSelectAnimation.TabIndex = 3;
            this.btnSelectAnimation.Text = "Select animation";
            this.btnSelectAnimation.UseVisualStyleBackColor = true;
            this.btnSelectAnimation.Click += new System.EventHandler(this.btnSelectAnimation_Click);
            // 
            // txtAnimationLength
            // 
            this.txtAnimationLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnimationLength.Location = new System.Drawing.Point(108, 45);
            this.txtAnimationLength.Name = "txtAnimationLength";
            this.txtAnimationLength.ReadOnly = true;
            this.txtAnimationLength.Size = new System.Drawing.Size(127, 20);
            this.txtAnimationLength.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Animation length";
            // 
            // txtAnimationName
            // 
            this.txtAnimationName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAnimationName.Location = new System.Drawing.Point(6, 19);
            this.txtAnimationName.Name = "txtAnimationName";
            this.txtAnimationName.ReadOnly = true;
            this.txtAnimationName.Size = new System.Drawing.Size(229, 20);
            this.txtAnimationName.TabIndex = 3;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(703, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // gbInput
            // 
            this.gbInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbInput.Controls.Add(this.txtNextInputDelay);
            this.gbInput.Controls.Add(this.label6);
            this.gbInput.Controls.Add(this.label5);
            this.gbInput.Controls.Add(this.label3);
            this.gbInput.Controls.Add(this.cbMovementChoice);
            this.gbInput.Controls.Add(this.btnAddInput);
            this.gbInput.Controls.Add(this.lstInput);
            this.gbInput.Controls.Add(this.btnRemoveInput);
            this.gbInput.Controls.Add(this.cbAttackChoice);
            this.gbInput.Enabled = false;
            this.gbInput.Location = new System.Drawing.Point(508, 27);
            this.gbInput.Name = "gbInput";
            this.gbInput.Size = new System.Drawing.Size(183, 257);
            this.gbInput.TabIndex = 4;
            this.gbInput.TabStop = false;
            this.gbInput.Text = "Input";
            // 
            // txtNextInputDelay
            // 
            this.txtNextInputDelay.Location = new System.Drawing.Point(6, 231);
            this.txtNextInputDelay.Name = "txtNextInputDelay";
            this.txtNextInputDelay.Size = new System.Drawing.Size(170, 20);
            this.txtNextInputDelay.TabIndex = 14;
            this.txtNextInputDelay.ValueChanged += new System.EventHandler(this.txtNextInputDelay_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 215);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(105, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Next input delay (ms)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Movement choice";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Attack choice";
            // 
            // cbMovementChoice
            // 
            this.cbMovementChoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMovementChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMovementChoice.FormattingEnabled = true;
            this.cbMovementChoice.Items.AddRange(new object[] {
            "Any",
            "None",
            "Up",
            "Down",
            "Left",
            "Right",
            "Foward",
            "Backward",
            "Running",
            "Dash",
            "Airborne"});
            this.cbMovementChoice.Location = new System.Drawing.Point(5, 191);
            this.cbMovementChoice.Name = "cbMovementChoice";
            this.cbMovementChoice.Size = new System.Drawing.Size(171, 21);
            this.cbMovementChoice.TabIndex = 10;
            this.cbMovementChoice.SelectedIndexChanged += new System.EventHandler(this.cbMovementChoice_SelectedIndexChanged);
            // 
            // btnAddInput
            // 
            this.btnAddInput.Location = new System.Drawing.Point(6, 107);
            this.btnAddInput.Name = "btnAddInput";
            this.btnAddInput.Size = new System.Drawing.Size(82, 23);
            this.btnAddInput.TabIndex = 9;
            this.btnAddInput.Text = "Add input";
            this.btnAddInput.UseVisualStyleBackColor = true;
            this.btnAddInput.Click += new System.EventHandler(this.btnAddInput_Click);
            // 
            // lstInput
            // 
            this.lstInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstInput.FormattingEnabled = true;
            this.lstInput.Location = new System.Drawing.Point(6, 19);
            this.lstInput.Name = "lstInput";
            this.lstInput.Size = new System.Drawing.Size(171, 82);
            this.lstInput.TabIndex = 1;
            this.lstInput.SelectedIndexChanged += new System.EventHandler(this.lstInput_SelectedIndexChanged);
            // 
            // btnRemoveInput
            // 
            this.btnRemoveInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveInput.Location = new System.Drawing.Point(94, 107);
            this.btnRemoveInput.Name = "btnRemoveInput";
            this.btnRemoveInput.Size = new System.Drawing.Size(82, 23);
            this.btnRemoveInput.TabIndex = 8;
            this.btnRemoveInput.Text = "Remove input";
            this.btnRemoveInput.UseVisualStyleBackColor = true;
            this.btnRemoveInput.Click += new System.EventHandler(this.btnRemoveInput_Click);
            // 
            // cbAttackChoice
            // 
            this.cbAttackChoice.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbAttackChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAttackChoice.FormattingEnabled = true;
            this.cbAttackChoice.Items.AddRange(new object[] {
            "None",
            "Any Press",
            "Any Hold",
            "Light Press",
            "Light Hold",
            "Heavy Press",
            "Heavy Hold"});
            this.cbAttackChoice.Location = new System.Drawing.Point(5, 151);
            this.cbAttackChoice.Name = "cbAttackChoice";
            this.cbAttackChoice.Size = new System.Drawing.Size(171, 21);
            this.cbAttackChoice.TabIndex = 0;
            this.cbAttackChoice.SelectedIndexChanged += new System.EventHandler(this.cbAttackChoice_SelectedIndexChanged);
            // 
            // cbRotationType
            // 
            this.cbRotationType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRotationType.FormattingEnabled = true;
            this.cbRotationType.Items.AddRange(new object[] {
            "No rotation",
            "Rotate around weapon slot",
            "Rotate around robot"});
            this.cbRotationType.Location = new System.Drawing.Point(114, 100);
            this.cbRotationType.Name = "cbRotationType";
            this.cbRotationType.Size = new System.Drawing.Size(121, 21);
            this.cbRotationType.TabIndex = 8;
            // 
            // lblRotationType
            // 
            this.lblRotationType.AutoSize = true;
            this.lblRotationType.Location = new System.Drawing.Point(6, 103);
            this.lblRotationType.Name = "lblRotationType";
            this.lblRotationType.Size = new System.Drawing.Size(77, 13);
            this.lblRotationType.TabIndex = 8;
            this.lblRotationType.Text = "Rotation Type:";
            // 
            // ComboEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 348);
            this.Controls.Add(this.gbInput);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.gbComboList);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ComboEditor";
            this.Text = "ComboEditor";
            this.Shown += new System.EventHandler(this.ComboEditor_Shown);
            this.groupBox1.ResumeLayout(false);
            this.gbComboList.ResumeLayout(false);
            this.gbComboList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtEndTime)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStartTime)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.gbInput.ResumeLayout(false);
            this.gbInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtNextInputDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox gbComboList;
        private System.Windows.Forms.NumericUpDown txtEndTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown txtStartTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox lstComboList;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtAnimationName;
        private System.Windows.Forms.TextBox txtAnimationLength;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panAttackTimeline;
        private System.Windows.Forms.Button btnSelectAnimation;
        private System.Windows.Forms.Button btnRemoveCombo;
        private System.Windows.Forms.Button btnAddCombo;
        private System.Windows.Forms.Button btnSelectCombo;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox gbInput;
        private System.Windows.Forms.ComboBox cbAttackChoice;
        private System.Windows.Forms.ListBox lstInput;
        private System.Windows.Forms.Button btnAddInput;
        private System.Windows.Forms.Button btnRemoveInput;
        private System.Windows.Forms.ComboBox cbMovementChoice;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtNextInputDelay;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbAnimationType;
        private System.Windows.Forms.CheckBox cbInstantActivation;
        private System.Windows.Forms.Label lblRotationType;
        private System.Windows.Forms.ComboBox cbRotationType;
    }
}