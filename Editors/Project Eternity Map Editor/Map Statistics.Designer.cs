﻿namespace ProjectEternity.Editors.MapEditor
{
    partial class MapStatistics
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
            this.txtMapWidth = new System.Windows.Forms.TextBox();
            this.txtMapHeight = new System.Windows.Forms.TextBox();
            this.txtTileWidth = new System.Windows.Forms.TextBox();
            this.txtTileHeight = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCameraStartPositionY = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCameraStartPositionX = new System.Windows.Forms.NumericUpDown();
            this.btnSetBackgrounds = new System.Windows.Forms.Button();
            this.btnSetForegrounds = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionX)).BeginInit();
            this.SuspendLayout();
            // 
            // txtMapWidth
            // 
            this.txtMapWidth.Location = new System.Drawing.Point(12, 83);
            this.txtMapWidth.Name = "txtMapWidth";
            this.txtMapWidth.Size = new System.Drawing.Size(97, 20);
            this.txtMapWidth.TabIndex = 0;
            this.txtMapWidth.Text = "50";
            this.txtMapWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGridWidth_KeyPress);
            // 
            // txtMapHeight
            // 
            this.txtMapHeight.Location = new System.Drawing.Point(12, 124);
            this.txtMapHeight.Name = "txtMapHeight";
            this.txtMapHeight.Size = new System.Drawing.Size(97, 20);
            this.txtMapHeight.TabIndex = 1;
            this.txtMapHeight.Text = "50";
            this.txtMapHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGridHeight_KeyPress);
            // 
            // txtTileWidth
            // 
            this.txtTileWidth.Location = new System.Drawing.Point(12, 191);
            this.txtTileWidth.Name = "txtTileWidth";
            this.txtTileWidth.Size = new System.Drawing.Size(97, 20);
            this.txtTileWidth.TabIndex = 2;
            this.txtTileWidth.Text = "32";
            this.txtTileWidth.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCaseWidth_KeyPress);
            // 
            // txtTileHeight
            // 
            this.txtTileHeight.Location = new System.Drawing.Point(12, 230);
            this.txtTileHeight.Name = "txtTileHeight";
            this.txtTileHeight.Size = new System.Drawing.Size(97, 20);
            this.txtTileHeight.TabIndex = 3;
            this.txtTileHeight.Text = "32";
            this.txtTileHeight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCaseHeight_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Map Width (in tiles)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 108);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Map Height (in tiles)";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 214);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Tile Height";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 175);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Tile Width";
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(12, 34);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.ReadOnly = true;
            this.txtMapName.Size = new System.Drawing.Size(259, 20);
            this.txtMapName.TabIndex = 8;
            this.txtMapName.Text = "New map";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Map name";
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(115, 230);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 10;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(196, 230);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtCameraStartPositionY);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtCameraStartPositionX);
            this.groupBox1.Location = new System.Drawing.Point(145, 67);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(126, 73);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Camera Start Position";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Y:";
            // 
            // txtCameraStartPositionY
            // 
            this.txtCameraStartPositionY.Location = new System.Drawing.Point(37, 45);
            this.txtCameraStartPositionY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtCameraStartPositionY.Name = "txtCameraStartPositionY";
            this.txtCameraStartPositionY.Size = new System.Drawing.Size(83, 20);
            this.txtCameraStartPositionY.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "X:";
            // 
            // txtCameraStartPositionX
            // 
            this.txtCameraStartPositionX.Location = new System.Drawing.Point(37, 16);
            this.txtCameraStartPositionX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtCameraStartPositionX.Name = "txtCameraStartPositionX";
            this.txtCameraStartPositionX.Size = new System.Drawing.Size(83, 20);
            this.txtCameraStartPositionX.TabIndex = 0;
            // 
            // btnSetBackgrounds
            // 
            this.btnSetBackgrounds.Location = new System.Drawing.Point(154, 172);
            this.btnSetBackgrounds.Name = "btnSetBackgrounds";
            this.btnSetBackgrounds.Size = new System.Drawing.Size(117, 23);
            this.btnSetBackgrounds.TabIndex = 13;
            this.btnSetBackgrounds.Text = "Set Backgrounds";
            this.btnSetBackgrounds.UseVisualStyleBackColor = true;
            this.btnSetBackgrounds.Click += new System.EventHandler(this.btnSetBackgrounds_Click);
            // 
            // btnSetForegrounds
            // 
            this.btnSetForegrounds.Location = new System.Drawing.Point(154, 201);
            this.btnSetForegrounds.Name = "btnSetForegrounds";
            this.btnSetForegrounds.Size = new System.Drawing.Size(117, 23);
            this.btnSetForegrounds.TabIndex = 14;
            this.btnSetForegrounds.Text = "Set Foregrounds";
            this.btnSetForegrounds.UseVisualStyleBackColor = true;
            this.btnSetForegrounds.Click += new System.EventHandler(this.btnSetForegrounds_Click);
            // 
            // MapStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 263);
            this.Controls.Add(this.btnSetForegrounds);
            this.Controls.Add(this.btnSetBackgrounds);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMapName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTileHeight);
            this.Controls.Add(this.txtTileWidth);
            this.Controls.Add(this.txtMapHeight);
            this.Controls.Add(this.txtMapWidth);
            this.Name = "MapStatistics";
            this.Text = "Map statistics";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionX)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMapWidth;
        private System.Windows.Forms.TextBox txtMapHeight;
        private System.Windows.Forms.TextBox txtTileWidth;
        private System.Windows.Forms.TextBox txtTileHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown txtCameraStartPositionY;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown txtCameraStartPositionX;
        private System.Windows.Forms.Button btnSetBackgrounds;
        private System.Windows.Forms.Button btnSetForegrounds;
    }
}