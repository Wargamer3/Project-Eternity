namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class PolygonCutterHelper
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.PolygonCutterViewer = new ProjectEternity.GameScreens.AnimationScreen.PolygonCutterViewerControl();
            this.btnMultiTool = new System.Windows.Forms.RadioButton();
            this.btnSelectPolygon = new System.Windows.Forms.RadioButton();
            this.btnSplitPolygonTool = new System.Windows.Forms.RadioButton();
            this.btnSplitTriangle = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.PolygonCutterViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnSplitTriangle);
            this.splitContainer1.Panel2.Controls.Add(this.btnSplitPolygonTool);
            this.splitContainer1.Panel2.Controls.Add(this.btnSelectPolygon);
            this.splitContainer1.Panel2.Controls.Add(this.btnMultiTool);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnConfirm);
            this.splitContainer1.Size = new System.Drawing.Size(450, 377);
            this.splitContainer1.SplitterDistance = 271;
            this.splitContainer1.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(97, 351);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(3, 351);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // PolygonCutterViewer
            // 
            this.PolygonCutterViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PolygonCutterViewer.Location = new System.Drawing.Point(0, 0);
            this.PolygonCutterViewer.Name = "PolygonCutterViewer";
            this.PolygonCutterViewer.Size = new System.Drawing.Size(267, 373);
            this.PolygonCutterViewer.TabIndex = 2;
            this.PolygonCutterViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PolygonCutterViewer_MouseDown);
            this.PolygonCutterViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PolygonCutterViewer_MouseMove);
            this.PolygonCutterViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PolygonCutterViewer_MouseUp);
            // 
            // btnMultiTool
            // 
            this.btnMultiTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnMultiTool.AutoSize = true;
            this.btnMultiTool.Location = new System.Drawing.Point(3, 10);
            this.btnMultiTool.Name = "btnMultiTool";
            this.btnMultiTool.Size = new System.Drawing.Size(63, 23);
            this.btnMultiTool.TabIndex = 6;
            this.btnMultiTool.TabStop = true;
            this.btnMultiTool.Text = "Multi Tool";
            this.btnMultiTool.UseVisualStyleBackColor = true;
            this.btnMultiTool.CheckedChanged += new System.EventHandler(this.btnMultiTool_CheckedChanged);
            // 
            // btnSelectPolygon
            // 
            this.btnSelectPolygon.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSelectPolygon.AutoSize = true;
            this.btnSelectPolygon.Location = new System.Drawing.Point(80, 10);
            this.btnSelectPolygon.Name = "btnSelectPolygon";
            this.btnSelectPolygon.Size = new System.Drawing.Size(88, 23);
            this.btnSelectPolygon.TabIndex = 7;
            this.btnSelectPolygon.TabStop = true;
            this.btnSelectPolygon.Text = "Select Polygon";
            this.btnSelectPolygon.UseVisualStyleBackColor = true;
            this.btnSelectPolygon.CheckedChanged += new System.EventHandler(this.btnSelectPolygon_CheckedChanged);
            // 
            // btnSplitPolygonTool
            // 
            this.btnSplitPolygonTool.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSplitPolygonTool.AutoSize = true;
            this.btnSplitPolygonTool.Location = new System.Drawing.Point(3, 39);
            this.btnSplitPolygonTool.Name = "btnSplitPolygonTool";
            this.btnSplitPolygonTool.Size = new System.Drawing.Size(78, 23);
            this.btnSplitPolygonTool.TabIndex = 8;
            this.btnSplitPolygonTool.TabStop = true;
            this.btnSplitPolygonTool.Text = "Split Polygon";
            this.btnSplitPolygonTool.UseVisualStyleBackColor = true;
            this.btnSplitPolygonTool.CheckedChanged += new System.EventHandler(this.btnSplitPolygonTool_CheckedChanged);
            // 
            // btnSplitTriangle
            // 
            this.btnSplitTriangle.Appearance = System.Windows.Forms.Appearance.Button;
            this.btnSplitTriangle.AutoSize = true;
            this.btnSplitTriangle.Location = new System.Drawing.Point(90, 39);
            this.btnSplitTriangle.Name = "btnSplitTriangle";
            this.btnSplitTriangle.Size = new System.Drawing.Size(78, 23);
            this.btnSplitTriangle.TabIndex = 9;
            this.btnSplitTriangle.TabStop = true;
            this.btnSplitTriangle.Text = "Split Triangle";
            this.btnSplitTriangle.UseVisualStyleBackColor = true;
            this.btnSplitTriangle.CheckedChanged += new System.EventHandler(this.btnSplitTriangle_CheckedChanged);
            // 
            // PolygonCutterHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 377);
            this.Controls.Add(this.splitContainer1);
            this.Name = "PolygonCutterHelper";
            this.Text = "Polygon Cutter Helper";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        public PolygonCutterViewerControl PolygonCutterViewer;
        private System.Windows.Forms.RadioButton btnSplitTriangle;
        private System.Windows.Forms.RadioButton btnSplitPolygonTool;
        private System.Windows.Forms.RadioButton btnSelectPolygon;
        private System.Windows.Forms.RadioButton btnMultiTool;
    }
}