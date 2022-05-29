namespace ProjectEternity.GameScreens.AnimationScreen
{
    partial class LightningSpawnerHelper
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
            this.components = new System.ComponentModel.Container();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SpawnViewer = new ProjectEternity.GameScreens.AnimationScreen.LightningSpawnerViewerControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pgLightningNodeProperties = new System.Windows.Forms.PropertyGrid();
            this.gbLightningStructure = new System.Windows.Forms.GroupBox();
            this.tvLightningStructure = new System.Windows.Forms.TreeView();
            this.cmsLightning = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmNewChild = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmNewRoot = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.gbPresets = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbLightningStructure.SuspendLayout();
            this.cmsLightning.SuspendLayout();
            this.gbPresets.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.SpawnViewer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Controls.Add(this.gbLightningStructure);
            this.splitContainer1.Panel2.Controls.Add(this.gbPresets);
            this.splitContainer1.Panel2.Controls.Add(this.btnCancel);
            this.splitContainer1.Panel2.Controls.Add(this.btnConfirm);
            this.splitContainer1.Size = new System.Drawing.Size(723, 377);
            this.splitContainer1.SplitterDistance = 256;
            this.splitContainer1.TabIndex = 1;
            // 
            // SpawnViewer
            // 
            this.SpawnViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SpawnViewer.Location = new System.Drawing.Point(0, 0);
            this.SpawnViewer.Name = "SpawnViewer";
            this.SpawnViewer.Size = new System.Drawing.Size(256, 377);
            this.SpawnViewer.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pgLightningNodeProperties);
            this.groupBox1.Location = new System.Drawing.Point(209, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 333);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lightning node attributes";
            // 
            // pgLightningNodeProperties
            // 
            this.pgLightningNodeProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgLightningNodeProperties.Location = new System.Drawing.Point(3, 16);
            this.pgLightningNodeProperties.Name = "pgLightningNodeProperties";
            this.pgLightningNodeProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgLightningNodeProperties.Size = new System.Drawing.Size(236, 314);
            this.pgLightningNodeProperties.TabIndex = 1;
            this.pgLightningNodeProperties.ToolbarVisible = false;
            // 
            // gbLightningStructure
            // 
            this.gbLightningStructure.Controls.Add(this.tvLightningStructure);
            this.gbLightningStructure.Location = new System.Drawing.Point(3, 69);
            this.gbLightningStructure.Name = "gbLightningStructure";
            this.gbLightningStructure.Size = new System.Drawing.Size(200, 296);
            this.gbLightningStructure.TabIndex = 3;
            this.gbLightningStructure.TabStop = false;
            this.gbLightningStructure.Text = "Lightning structure";
            // 
            // tvLightningStructure
            // 
            this.tvLightningStructure.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvLightningStructure.ContextMenuStrip = this.cmsLightning;
            this.tvLightningStructure.Location = new System.Drawing.Point(6, 19);
            this.tvLightningStructure.Name = "tvLightningStructure";
            this.tvLightningStructure.Size = new System.Drawing.Size(188, 271);
            this.tvLightningStructure.TabIndex = 0;
            this.tvLightningStructure.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvLightningStructure_AfterSelect);
            // 
            // cmsLightning
            // 
            this.cmsLightning.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmNewChild,
            this.tsmNewRoot,
            this.tsmDelete});
            this.cmsLightning.Name = "cmsLightning";
            this.cmsLightning.Size = new System.Drawing.Size(128, 70);
            // 
            // tsmNewChild
            // 
            this.tsmNewChild.Name = "tsmNewChild";
            this.tsmNewChild.Size = new System.Drawing.Size(127, 22);
            this.tsmNewChild.Text = "New child";
            this.tsmNewChild.Click += new System.EventHandler(this.tsmNewChild_Click);
            // 
            // tsmNewRoot
            // 
            this.tsmNewRoot.Name = "tsmNewRoot";
            this.tsmNewRoot.Size = new System.Drawing.Size(127, 22);
            this.tsmNewRoot.Text = "New root";
            this.tsmNewRoot.Click += new System.EventHandler(this.tsmNewRoot_Click);
            // 
            // tsmDelete
            // 
            this.tsmDelete.Name = "tsmDelete";
            this.tsmDelete.Size = new System.Drawing.Size(127, 22);
            this.tsmDelete.Text = "Delete";
            this.tsmDelete.Click += new System.EventHandler(this.tsmDelete_Click);
            // 
            // gbPresets
            // 
            this.gbPresets.Controls.Add(this.comboBox1);
            this.gbPresets.Location = new System.Drawing.Point(3, 12);
            this.gbPresets.Name = "gbPresets";
            this.gbPresets.Size = new System.Drawing.Size(200, 51);
            this.gbPresets.TabIndex = 2;
            this.gbPresets.TabStop = false;
            this.gbPresets.Text = "Presets";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Lightning Tree",
            "Crimson Line",
            "Red Spark Line",
            "Yellow Beam"});
            this.comboBox1.Location = new System.Drawing.Point(6, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(188, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(376, 351);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(282, 351);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 0;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // LightningSpawnerHelper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(723, 377);
            this.Controls.Add(this.splitContainer1);
            this.Name = "LightningSpawnerHelper";
            this.Text = "Lightning Spawner Helper";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.gbLightningStructure.ResumeLayout(false);
            this.cmsLightning.ResumeLayout(false);
            this.gbPresets.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
        public LightningSpawnerViewerControl SpawnViewer;
        private System.Windows.Forms.GroupBox gbPresets;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox gbLightningStructure;
        private System.Windows.Forms.TreeView tvLightningStructure;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip cmsLightning;
        private System.Windows.Forms.ToolStripMenuItem tsmNewChild;
        private System.Windows.Forms.ToolStripMenuItem tsmNewRoot;
        private System.Windows.Forms.ToolStripMenuItem tsmDelete;
        private System.Windows.Forms.PropertyGrid pgLightningNodeProperties;
    }
}
