namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class MapDestinationEditor
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
            this.gbMap = new System.Windows.Forms.GroupBox();
            this.BattleMapViewer = new ProjectEternity.GameScreens.BattleMapScreen.BattleMapViewerControl();
            this.btnPreviewMap = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.gbMap.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbMap
            // 
            this.gbMap.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMap.Controls.Add(this.BattleMapViewer);
            this.gbMap.Location = new System.Drawing.Point(12, 12);
            this.gbMap.Name = "gbMap";
            this.gbMap.Size = new System.Drawing.Size(509, 259);
            this.gbMap.TabIndex = 0;
            this.gbMap.TabStop = false;
            this.gbMap.Text = "Map";
            // 
            // BattleMapViewer
            // 
            this.BattleMapViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BattleMapViewer.Location = new System.Drawing.Point(3, 16);
            this.BattleMapViewer.Name = "BattleMapViewer";
            this.BattleMapViewer.Size = new System.Drawing.Size(503, 240);
            this.BattleMapViewer.TabIndex = 0;
            this.BattleMapViewer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BattleMapViewer_MouseDown);
            this.BattleMapViewer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BattleMapViewer_MouseMove);
            this.BattleMapViewer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BattleMapViewer_MouseUp);
            // 
            // btnPreviewMap
            // 
            this.btnPreviewMap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPreviewMap.Location = new System.Drawing.Point(12, 277);
            this.btnPreviewMap.Name = "btnPreviewMap";
            this.btnPreviewMap.Size = new System.Drawing.Size(116, 23);
            this.btnPreviewMap.TabIndex = 1;
            this.btnPreviewMap.Text = "Preview Map";
            this.btnPreviewMap.UseVisualStyleBackColor = true;
            this.btnPreviewMap.Click += new System.EventHandler(this.btnPreviewMap_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(362, 277);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(443, 277);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MapDestinationEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 312);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnPreviewMap);
            this.Controls.Add(this.gbMap);
            this.Name = "MapDestinationEditor";
            this.Text = "Map Destination Editor";
            this.gbMap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMap;
        private System.Windows.Forms.Button btnPreviewMap;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnCancel;
        public BattleMapScreen.BattleMapViewerControl BattleMapViewer;
    }
}