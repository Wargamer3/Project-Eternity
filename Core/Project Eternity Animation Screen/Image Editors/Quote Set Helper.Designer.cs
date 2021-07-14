namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class QuoteSetHelper
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
            this.gbTarget = new System.Windows.Forms.GroupBox();
            this.rbDefender = new System.Windows.Forms.RadioButton();
            this.rbAttacker = new System.Windows.Forms.RadioButton();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.gbQuoteStyle = new System.Windows.Forms.GroupBox();
            this.rbQuoteSetCustomText = new System.Windows.Forms.RadioButton();
            this.rbQuoteSetQuoteSet = new System.Windows.Forms.RadioButton();
            this.rbQuoteSetReaction = new System.Windows.Forms.RadioButton();
            this.rbQuoteSetMoveIn = new System.Windows.Forms.RadioButton();
            this.txtQuoteChoiceValue = new System.Windows.Forms.NumericUpDown();
            this.rbQuoteChoiceFixed = new System.Windows.Forms.RadioButton();
            this.rbQuoteChoiceRandom = new System.Windows.Forms.RadioButton();
            this.lblQuoteChoice = new System.Windows.Forms.Label();
            this.txtCustomText = new System.Windows.Forms.TextBox();
            this.lblQuoteSet = new System.Windows.Forms.Label();
            this.txtQuoteSetName = new System.Windows.Forms.TextBox();
            this.gbQuoteSetList = new System.Windows.Forms.GroupBox();
            this.btnRemoveQuoteSet = new System.Windows.Forms.Button();
            this.btnAddQuoteSet = new System.Windows.Forms.Button();
            this.lstQuoteSet = new System.Windows.Forms.ListBox();
            this.gbCustomText = new System.Windows.Forms.GroupBox();
            this.txtPortraitPath = new System.Windows.Forms.TextBox();
            this.btnSelectPortrait = new System.Windows.Forms.Button();
            this.gbQuoteSet = new System.Windows.Forms.GroupBox();
            this.ckUseLast = new System.Windows.Forms.CheckBox();
            this.gbTarget.SuspendLayout();
            this.gbQuoteStyle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuoteChoiceValue)).BeginInit();
            this.gbQuoteSetList.SuspendLayout();
            this.gbCustomText.SuspendLayout();
            this.gbQuoteSet.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbTarget
            // 
            this.gbTarget.Controls.Add(this.rbDefender);
            this.gbTarget.Controls.Add(this.rbAttacker);
            this.gbTarget.Location = new System.Drawing.Point(12, 12);
            this.gbTarget.Name = "gbTarget";
            this.gbTarget.Size = new System.Drawing.Size(107, 69);
            this.gbTarget.TabIndex = 0;
            this.gbTarget.TabStop = false;
            this.gbTarget.Text = "Target";
            // 
            // rbDefender
            // 
            this.rbDefender.AutoSize = true;
            this.rbDefender.Location = new System.Drawing.Point(12, 42);
            this.rbDefender.Name = "rbDefender";
            this.rbDefender.Size = new System.Drawing.Size(69, 17);
            this.rbDefender.TabIndex = 1;
            this.rbDefender.TabStop = true;
            this.rbDefender.Text = "Defender";
            this.rbDefender.UseVisualStyleBackColor = true;
            this.rbDefender.CheckedChanged += new System.EventHandler(this.rbTarget_CheckedChanged);
            // 
            // rbAttacker
            // 
            this.rbAttacker.AutoSize = true;
            this.rbAttacker.Location = new System.Drawing.Point(12, 19);
            this.rbAttacker.Name = "rbAttacker";
            this.rbAttacker.Size = new System.Drawing.Size(65, 17);
            this.rbAttacker.TabIndex = 0;
            this.rbAttacker.TabStop = true;
            this.rbAttacker.Text = "Attacker";
            this.rbAttacker.UseVisualStyleBackColor = true;
            this.rbAttacker.CheckedChanged += new System.EventHandler(this.rbTarget_CheckedChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(296, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(124, 273);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 1;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // gbQuoteStyle
            // 
            this.gbQuoteStyle.Controls.Add(this.rbQuoteSetCustomText);
            this.gbQuoteStyle.Controls.Add(this.rbQuoteSetQuoteSet);
            this.gbQuoteStyle.Controls.Add(this.rbQuoteSetReaction);
            this.gbQuoteStyle.Controls.Add(this.rbQuoteSetMoveIn);
            this.gbQuoteStyle.Location = new System.Drawing.Point(125, 12);
            this.gbQuoteStyle.Name = "gbQuoteStyle";
            this.gbQuoteStyle.Size = new System.Drawing.Size(246, 69);
            this.gbQuoteStyle.TabIndex = 3;
            this.gbQuoteStyle.TabStop = false;
            this.gbQuoteStyle.Text = "Quote style";
            // 
            // rbQuoteSetCustomText
            // 
            this.rbQuoteSetCustomText.AutoSize = true;
            this.rbQuoteSetCustomText.Location = new System.Drawing.Point(157, 42);
            this.rbQuoteSetCustomText.Name = "rbQuoteSetCustomText";
            this.rbQuoteSetCustomText.Size = new System.Drawing.Size(84, 17);
            this.rbQuoteSetCustomText.TabIndex = 7;
            this.rbQuoteSetCustomText.TabStop = true;
            this.rbQuoteSetCustomText.Text = "Custom Text";
            this.rbQuoteSetCustomText.UseVisualStyleBackColor = true;
            this.rbQuoteSetCustomText.CheckedChanged += new System.EventHandler(this.rbQuoteSet_CheckedChanged);
            // 
            // rbQuoteSetQuoteSet
            // 
            this.rbQuoteSetQuoteSet.AutoSize = true;
            this.rbQuoteSetQuoteSet.Location = new System.Drawing.Point(80, 42);
            this.rbQuoteSetQuoteSet.Name = "rbQuoteSetQuoteSet";
            this.rbQuoteSetQuoteSet.Size = new System.Drawing.Size(71, 17);
            this.rbQuoteSetQuoteSet.TabIndex = 6;
            this.rbQuoteSetQuoteSet.TabStop = true;
            this.rbQuoteSetQuoteSet.Text = "Quote set";
            this.rbQuoteSetQuoteSet.UseVisualStyleBackColor = true;
            this.rbQuoteSetQuoteSet.CheckedChanged += new System.EventHandler(this.rbQuoteSet_CheckedChanged);
            // 
            // rbQuoteSetReaction
            // 
            this.rbQuoteSetReaction.AutoSize = true;
            this.rbQuoteSetReaction.Location = new System.Drawing.Point(6, 42);
            this.rbQuoteSetReaction.Name = "rbQuoteSetReaction";
            this.rbQuoteSetReaction.Size = new System.Drawing.Size(68, 17);
            this.rbQuoteSetReaction.TabIndex = 5;
            this.rbQuoteSetReaction.TabStop = true;
            this.rbQuoteSetReaction.Text = "Reaction";
            this.rbQuoteSetReaction.UseVisualStyleBackColor = true;
            this.rbQuoteSetReaction.CheckedChanged += new System.EventHandler(this.rbQuoteSet_CheckedChanged);
            // 
            // rbQuoteSetMoveIn
            // 
            this.rbQuoteSetMoveIn.AutoSize = true;
            this.rbQuoteSetMoveIn.Location = new System.Drawing.Point(6, 19);
            this.rbQuoteSetMoveIn.Name = "rbQuoteSetMoveIn";
            this.rbQuoteSetMoveIn.Size = new System.Drawing.Size(63, 17);
            this.rbQuoteSetMoveIn.TabIndex = 3;
            this.rbQuoteSetMoveIn.TabStop = true;
            this.rbQuoteSetMoveIn.Text = "Move in";
            this.rbQuoteSetMoveIn.UseVisualStyleBackColor = true;
            this.rbQuoteSetMoveIn.CheckedChanged += new System.EventHandler(this.rbQuoteSet_CheckedChanged);
            // 
            // txtQuoteChoiceValue
            // 
            this.txtQuoteChoiceValue.Location = new System.Drawing.Point(150, 74);
            this.txtQuoteChoiceValue.Name = "txtQuoteChoiceValue";
            this.txtQuoteChoiceValue.Size = new System.Drawing.Size(90, 20);
            this.txtQuoteChoiceValue.TabIndex = 13;
            this.txtQuoteChoiceValue.ValueChanged += new System.EventHandler(this.txtQuoteChoiceValue_ValueChanged);
            // 
            // rbQuoteChoiceFixed
            // 
            this.rbQuoteChoiceFixed.AutoSize = true;
            this.rbQuoteChoiceFixed.Location = new System.Drawing.Point(94, 74);
            this.rbQuoteChoiceFixed.Name = "rbQuoteChoiceFixed";
            this.rbQuoteChoiceFixed.Size = new System.Drawing.Size(50, 17);
            this.rbQuoteChoiceFixed.TabIndex = 12;
            this.rbQuoteChoiceFixed.TabStop = true;
            this.rbQuoteChoiceFixed.Text = "Fixed";
            this.rbQuoteChoiceFixed.UseVisualStyleBackColor = true;
            this.rbQuoteChoiceFixed.CheckedChanged += new System.EventHandler(this.rbQuoteChoiceFixed_CheckedChanged);
            // 
            // rbQuoteChoiceRandom
            // 
            this.rbQuoteChoiceRandom.AutoSize = true;
            this.rbQuoteChoiceRandom.Location = new System.Drawing.Point(6, 74);
            this.rbQuoteChoiceRandom.Name = "rbQuoteChoiceRandom";
            this.rbQuoteChoiceRandom.Size = new System.Drawing.Size(65, 17);
            this.rbQuoteChoiceRandom.TabIndex = 11;
            this.rbQuoteChoiceRandom.TabStop = true;
            this.rbQuoteChoiceRandom.Text = "Random";
            this.rbQuoteChoiceRandom.UseVisualStyleBackColor = true;
            this.rbQuoteChoiceRandom.CheckedChanged += new System.EventHandler(this.rbQuoteChoiceRandom_CheckedChanged);
            // 
            // lblQuoteChoice
            // 
            this.lblQuoteChoice.AutoSize = true;
            this.lblQuoteChoice.Location = new System.Drawing.Point(6, 58);
            this.lblQuoteChoice.Name = "lblQuoteChoice";
            this.lblQuoteChoice.Size = new System.Drawing.Size(71, 13);
            this.lblQuoteChoice.TabIndex = 10;
            this.lblQuoteChoice.Text = "Quote choice";
            // 
            // txtCustomText
            // 
            this.txtCustomText.Location = new System.Drawing.Point(6, 19);
            this.txtCustomText.Name = "txtCustomText";
            this.txtCustomText.Size = new System.Drawing.Size(234, 20);
            this.txtCustomText.TabIndex = 8;
            this.txtCustomText.TextChanged += new System.EventHandler(this.txtCustomText_TextChanged);
            // 
            // lblQuoteSet
            // 
            this.lblQuoteSet.AutoSize = true;
            this.lblQuoteSet.Location = new System.Drawing.Point(6, 16);
            this.lblQuoteSet.Name = "lblQuoteSet";
            this.lblQuoteSet.Size = new System.Drawing.Size(53, 13);
            this.lblQuoteSet.TabIndex = 2;
            this.lblQuoteSet.Text = "Quote set";
            // 
            // txtQuoteSet
            // 
            this.txtQuoteSetName.Location = new System.Drawing.Point(6, 32);
            this.txtQuoteSetName.Name = "txtQuoteSet";
            this.txtQuoteSetName.Size = new System.Drawing.Size(160, 20);
            this.txtQuoteSetName.TabIndex = 1;
            this.txtQuoteSetName.TextChanged += new System.EventHandler(this.txtQuoteSet_TextChanged);
            // 
            // gbQuoteSetList
            // 
            this.gbQuoteSetList.Controls.Add(this.btnRemoveQuoteSet);
            this.gbQuoteSetList.Controls.Add(this.btnAddQuoteSet);
            this.gbQuoteSetList.Controls.Add(this.lstQuoteSet);
            this.gbQuoteSetList.Location = new System.Drawing.Point(12, 87);
            this.gbQuoteSetList.Name = "gbQuoteSetList";
            this.gbQuoteSetList.Size = new System.Drawing.Size(107, 180);
            this.gbQuoteSetList.TabIndex = 4;
            this.gbQuoteSetList.TabStop = false;
            this.gbQuoteSetList.Text = "Quote Set List";
            // 
            // btnRemoveQuoteSet
            // 
            this.btnRemoveQuoteSet.Location = new System.Drawing.Point(6, 149);
            this.btnRemoveQuoteSet.Name = "btnRemoveQuoteSet";
            this.btnRemoveQuoteSet.Size = new System.Drawing.Size(95, 23);
            this.btnRemoveQuoteSet.TabIndex = 6;
            this.btnRemoveQuoteSet.Text = "Remove Quote";
            this.btnRemoveQuoteSet.UseVisualStyleBackColor = true;
            this.btnRemoveQuoteSet.Click += new System.EventHandler(this.btnRemoveQuoteSet_Click);
            // 
            // btnAddQuoteSet
            // 
            this.btnAddQuoteSet.Location = new System.Drawing.Point(6, 120);
            this.btnAddQuoteSet.Name = "btnAddQuoteSet";
            this.btnAddQuoteSet.Size = new System.Drawing.Size(95, 23);
            this.btnAddQuoteSet.TabIndex = 5;
            this.btnAddQuoteSet.Text = "Add Quote";
            this.btnAddQuoteSet.UseVisualStyleBackColor = true;
            this.btnAddQuoteSet.Click += new System.EventHandler(this.btnAddQuoteSet_Click);
            // 
            // lstQuoteSet
            // 
            this.lstQuoteSet.FormattingEnabled = true;
            this.lstQuoteSet.Location = new System.Drawing.Point(6, 19);
            this.lstQuoteSet.Name = "lstQuoteSet";
            this.lstQuoteSet.Size = new System.Drawing.Size(95, 95);
            this.lstQuoteSet.TabIndex = 5;
            this.lstQuoteSet.SelectedIndexChanged += new System.EventHandler(this.lstQuoteSet_SelectedIndexChanged);
            // 
            // gbCustomText
            // 
            this.gbCustomText.Controls.Add(this.txtPortraitPath);
            this.gbCustomText.Controls.Add(this.btnSelectPortrait);
            this.gbCustomText.Controls.Add(this.txtCustomText);
            this.gbCustomText.Location = new System.Drawing.Point(125, 193);
            this.gbCustomText.Name = "gbCustomText";
            this.gbCustomText.Size = new System.Drawing.Size(246, 74);
            this.gbCustomText.TabIndex = 5;
            this.gbCustomText.TabStop = false;
            this.gbCustomText.Text = "Custom Text";
            // 
            // txtPortraitPath
            // 
            this.txtPortraitPath.Location = new System.Drawing.Point(106, 45);
            this.txtPortraitPath.Name = "txtPortraitPath";
            this.txtPortraitPath.ReadOnly = true;
            this.txtPortraitPath.Size = new System.Drawing.Size(134, 20);
            this.txtPortraitPath.TabIndex = 9;
            // 
            // btnSelectPortrait
            // 
            this.btnSelectPortrait.Location = new System.Drawing.Point(6, 45);
            this.btnSelectPortrait.Name = "btnSelectPortrait";
            this.btnSelectPortrait.Size = new System.Drawing.Size(94, 23);
            this.btnSelectPortrait.TabIndex = 7;
            this.btnSelectPortrait.Text = "Select Portait";
            this.btnSelectPortrait.UseVisualStyleBackColor = true;
            this.btnSelectPortrait.Click += new System.EventHandler(this.btnSelectPortrait_Click);
            // 
            // gbQuoteSet
            // 
            this.gbQuoteSet.Controls.Add(this.ckUseLast);
            this.gbQuoteSet.Controls.Add(this.lblQuoteChoice);
            this.gbQuoteSet.Controls.Add(this.txtQuoteChoiceValue);
            this.gbQuoteSet.Controls.Add(this.rbQuoteChoiceFixed);
            this.gbQuoteSet.Controls.Add(this.rbQuoteChoiceRandom);
            this.gbQuoteSet.Controls.Add(this.lblQuoteSet);
            this.gbQuoteSet.Controls.Add(this.txtQuoteSetName);
            this.gbQuoteSet.Location = new System.Drawing.Point(125, 87);
            this.gbQuoteSet.Name = "gbQuoteSet";
            this.gbQuoteSet.Size = new System.Drawing.Size(246, 100);
            this.gbQuoteSet.TabIndex = 6;
            this.gbQuoteSet.TabStop = false;
            this.gbQuoteSet.Text = "Quote Set";
            // 
            // ckUseLast
            // 
            this.ckUseLast.AutoSize = true;
            this.ckUseLast.Location = new System.Drawing.Point(172, 34);
            this.ckUseLast.Name = "ckUseLast";
            this.ckUseLast.Size = new System.Drawing.Size(68, 17);
            this.ckUseLast.TabIndex = 14;
            this.ckUseLast.Text = "Use Last";
            this.ckUseLast.UseVisualStyleBackColor = true;
            this.ckUseLast.CheckedChanged += new System.EventHandler(this.ckUseLast_CheckedChanged);
            // 
            // QuoteSetHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 302);
            this.Controls.Add(this.gbQuoteSet);
            this.Controls.Add(this.gbCustomText);
            this.Controls.Add(this.gbQuoteSetList);
            this.Controls.Add(this.gbQuoteStyle);
            this.Controls.Add(this.gbTarget);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Name = "QuoteSetHelper";
            this.Text = "Quote Set Helper";
            this.gbTarget.ResumeLayout(false);
            this.gbTarget.PerformLayout();
            this.gbQuoteStyle.ResumeLayout(false);
            this.gbQuoteStyle.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtQuoteChoiceValue)).EndInit();
            this.gbQuoteSetList.ResumeLayout(false);
            this.gbCustomText.ResumeLayout(false);
            this.gbCustomText.PerformLayout();
            this.gbQuoteSet.ResumeLayout(false);
            this.gbQuoteSet.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTarget;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox gbQuoteStyle;
        private System.Windows.Forms.RadioButton rbDefender;
        private System.Windows.Forms.RadioButton rbAttacker;
        private System.Windows.Forms.Label lblQuoteSet;
        private System.Windows.Forms.NumericUpDown txtQuoteChoiceValue;
        private System.Windows.Forms.RadioButton rbQuoteChoiceFixed;
        private System.Windows.Forms.RadioButton rbQuoteChoiceRandom;
        private System.Windows.Forms.Label lblQuoteChoice;
        private System.Windows.Forms.TextBox txtCustomText;
        private System.Windows.Forms.RadioButton rbQuoteSetCustomText;
        private System.Windows.Forms.RadioButton rbQuoteSetQuoteSet;
        private System.Windows.Forms.RadioButton rbQuoteSetReaction;
        private System.Windows.Forms.RadioButton rbQuoteSetMoveIn;
        private System.Windows.Forms.GroupBox gbQuoteSetList;
        private System.Windows.Forms.ListBox lstQuoteSet;
        private System.Windows.Forms.Button btnRemoveQuoteSet;
        private System.Windows.Forms.Button btnAddQuoteSet;
        private System.Windows.Forms.GroupBox gbCustomText;
        private System.Windows.Forms.GroupBox gbQuoteSet;
        private System.Windows.Forms.CheckBox ckUseLast;
        private System.Windows.Forms.TextBox txtQuoteSetName;
        private System.Windows.Forms.TextBox txtPortraitPath;
        private System.Windows.Forms.Button btnSelectPortrait;

    }
}