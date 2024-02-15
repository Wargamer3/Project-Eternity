namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    partial class DetailsEditor
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
            this.txtMapSprite = new System.Windows.Forms.TextBox();
            this.lblMapSprite = new System.Windows.Forms.Label();
            this.btnMapSprite = new System.Windows.Forms.Button();
            this.gbSprites = new System.Windows.Forms.GroupBox();
            this.lblUnitSprite = new System.Windows.Forms.Label();
            this.btnChangeUnitSprite = new System.Windows.Forms.Button();
            this.txtShopSprite = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChange3DModel = new System.Windows.Forms.Button();
            this.txt3DModel = new System.Windows.Forms.TextBox();
            this.gbTags = new System.Windows.Forms.GroupBox();
            this.txtTags = new System.Windows.Forms.TextBox();
            this.gbSprites.SuspendLayout();
            this.gbTags.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMapSprite
            // 
            this.txtMapSprite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMapSprite.Location = new System.Drawing.Point(6, 71);
            this.txtMapSprite.Name = "txtMapSprite";
            this.txtMapSprite.ReadOnly = true;
            this.txtMapSprite.Size = new System.Drawing.Size(239, 20);
            this.txtMapSprite.TabIndex = 0;
            // 
            // lblMapSprite
            // 
            this.lblMapSprite.AutoSize = true;
            this.lblMapSprite.Location = new System.Drawing.Point(6, 55);
            this.lblMapSprite.Name = "lblMapSprite";
            this.lblMapSprite.Size = new System.Drawing.Size(58, 13);
            this.lblMapSprite.TabIndex = 1;
            this.lblMapSprite.Text = "Map Sprite";
            // 
            // btnMapSprite
            // 
            this.btnMapSprite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMapSprite.Location = new System.Drawing.Point(251, 69);
            this.btnMapSprite.Name = "btnMapSprite";
            this.btnMapSprite.Size = new System.Drawing.Size(128, 23);
            this.btnMapSprite.TabIndex = 2;
            this.btnMapSprite.Text = "Change Map Sprite";
            this.btnMapSprite.UseVisualStyleBackColor = true;
            this.btnMapSprite.Click += new System.EventHandler(this.btnChangeMapSprite_Click);
            // 
            // gbSprites
            // 
            this.gbSprites.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSprites.Controls.Add(this.lblUnitSprite);
            this.gbSprites.Controls.Add(this.btnChangeUnitSprite);
            this.gbSprites.Controls.Add(this.txtShopSprite);
            this.gbSprites.Controls.Add(this.label1);
            this.gbSprites.Controls.Add(this.btnChange3DModel);
            this.gbSprites.Controls.Add(this.txt3DModel);
            this.gbSprites.Controls.Add(this.lblMapSprite);
            this.gbSprites.Controls.Add(this.btnMapSprite);
            this.gbSprites.Controls.Add(this.txtMapSprite);
            this.gbSprites.Location = new System.Drawing.Point(12, 12);
            this.gbSprites.Name = "gbSprites";
            this.gbSprites.Size = new System.Drawing.Size(385, 137);
            this.gbSprites.TabIndex = 3;
            this.gbSprites.TabStop = false;
            this.gbSprites.Text = "Sprites";
            // 
            // lblUnitSprite
            // 
            this.lblUnitSprite.AutoSize = true;
            this.lblUnitSprite.Location = new System.Drawing.Point(6, 16);
            this.lblUnitSprite.Name = "lblUnitSprite";
            this.lblUnitSprite.Size = new System.Drawing.Size(62, 13);
            this.lblUnitSprite.TabIndex = 10;
            this.lblUnitSprite.Text = "Shop Sprite";
            // 
            // btnChangeUnitSprite
            // 
            this.btnChangeUnitSprite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeUnitSprite.Location = new System.Drawing.Point(251, 30);
            this.btnChangeUnitSprite.Name = "btnChangeUnitSprite";
            this.btnChangeUnitSprite.Size = new System.Drawing.Size(128, 23);
            this.btnChangeUnitSprite.TabIndex = 11;
            this.btnChangeUnitSprite.Text = "Change Shop Sprite";
            this.btnChangeUnitSprite.UseVisualStyleBackColor = true;
            this.btnChangeUnitSprite.Click += new System.EventHandler(this.btnChangeShopSprite_Click);
            // 
            // txtShopSprite
            // 
            this.txtShopSprite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtShopSprite.Location = new System.Drawing.Point(6, 32);
            this.txtShopSprite.Name = "txtShopSprite";
            this.txtShopSprite.ReadOnly = true;
            this.txtShopSprite.Size = new System.Drawing.Size(239, 20);
            this.txtShopSprite.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "3D Model";
            // 
            // btnChange3DModel
            // 
            this.btnChange3DModel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChange3DModel.Location = new System.Drawing.Point(251, 108);
            this.btnChange3DModel.Name = "btnChange3DModel";
            this.btnChange3DModel.Size = new System.Drawing.Size(128, 23);
            this.btnChange3DModel.TabIndex = 8;
            this.btnChange3DModel.Text = "Change 3D Model";
            this.btnChange3DModel.UseVisualStyleBackColor = true;
            this.btnChange3DModel.Click += new System.EventHandler(this.btnChange3DModel_Click);
            // 
            // txt3DModel
            // 
            this.txt3DModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt3DModel.Location = new System.Drawing.Point(6, 110);
            this.txt3DModel.Name = "txt3DModel";
            this.txt3DModel.ReadOnly = true;
            this.txt3DModel.Size = new System.Drawing.Size(239, 20);
            this.txt3DModel.TabIndex = 6;
            // 
            // gbTags
            // 
            this.gbTags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTags.Controls.Add(this.txtTags);
            this.gbTags.Location = new System.Drawing.Point(12, 155);
            this.gbTags.Name = "gbTags";
            this.gbTags.Size = new System.Drawing.Size(385, 109);
            this.gbTags.TabIndex = 5;
            this.gbTags.TabStop = false;
            this.gbTags.Text = "Tags";
            // 
            // txtTags
            // 
            this.txtTags.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTags.Location = new System.Drawing.Point(3, 16);
            this.txtTags.Multiline = true;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(379, 90);
            this.txtTags.TabIndex = 0;
            // 
            // DetailsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 276);
            this.Controls.Add(this.gbTags);
            this.Controls.Add(this.gbSprites);
            this.Name = "DetailsEditor";
            this.Text = "Details";
            this.gbSprites.ResumeLayout(false);
            this.gbSprites.PerformLayout();
            this.gbTags.ResumeLayout(false);
            this.gbTags.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblMapSprite;
        private System.Windows.Forms.Button btnMapSprite;
        private System.Windows.Forms.GroupBox gbSprites;
        public System.Windows.Forms.TextBox txtMapSprite;
        private System.Windows.Forms.GroupBox gbTags;
        public System.Windows.Forms.TextBox txtTags;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnChange3DModel;
        public System.Windows.Forms.TextBox txt3DModel;
        private System.Windows.Forms.Label lblUnitSprite;
        private System.Windows.Forms.Button btnChangeUnitSprite;
        public System.Windows.Forms.TextBox txtShopSprite;
    }
}