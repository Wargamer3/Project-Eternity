namespace ProjectEternity.Editors.SceneEditor
{
    partial class SceneProperties
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
            this.gbMaxSceneEvent = new System.Windows.Forms.GroupBox();
            this.txtMaxSceneEvent = new System.Windows.Forms.NumericUpDown();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.gbMaxSceneEvent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxSceneEvent)).BeginInit();
            this.SuspendLayout();
            // 
            // gbMaxSceneEvent
            // 
            this.gbMaxSceneEvent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMaxSceneEvent.Controls.Add(this.txtMaxSceneEvent);
            this.gbMaxSceneEvent.Location = new System.Drawing.Point(12, 12);
            this.gbMaxSceneEvent.Name = "gbMaxSceneEvent";
            this.gbMaxSceneEvent.Size = new System.Drawing.Size(260, 45);
            this.gbMaxSceneEvent.TabIndex = 0;
            this.gbMaxSceneEvent.TabStop = false;
            this.gbMaxSceneEvent.Text = "Maximum Scene Events";
            // 
            // txtMaxSceneEvent
            // 
            this.txtMaxSceneEvent.Location = new System.Drawing.Point(6, 19);
            this.txtMaxSceneEvent.Name = "txtMaxSceneEvent";
            this.txtMaxSceneEvent.Size = new System.Drawing.Size(248, 20);
            this.txtMaxSceneEvent.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(197, 63);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnConfirm
            // 
            this.btnConfirm.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnConfirm.Location = new System.Drawing.Point(116, 63);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            // 
            // SceneProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 98);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.gbMaxSceneEvent);
            this.Name = "SceneProperties";
            this.Text = "Scene Properties";
            this.gbMaxSceneEvent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMaxSceneEvent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbMaxSceneEvent;
        public System.Windows.Forms.NumericUpDown txtMaxSceneEvent;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnConfirm;
    }
}