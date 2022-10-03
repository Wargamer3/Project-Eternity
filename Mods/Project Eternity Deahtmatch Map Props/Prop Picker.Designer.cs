
namespace ProjectEternity.GameScreens.DeathmatchMapScreen
{
    partial class PropPicker
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
            this.PropsContainer = new System.Windows.Forms.SplitContainer();
            this.tabPropsChoices = new System.Windows.Forms.TabControl();
            this.tabInteractiveProps = new System.Windows.Forms.TabPage();
            this.lsInteractiveProps = new System.Windows.Forms.ListBox();
            this.tabPhysicalProps = new System.Windows.Forms.TabPage();
            this.lsPhysicalProps = new System.Windows.Forms.ListBox();
            this.tabVisualProps = new System.Windows.Forms.TabPage();
            this.lsVisualProps = new System.Windows.Forms.ListBox();
            this.pgPropProperties = new System.Windows.Forms.PropertyGrid();
            ((System.ComponentModel.ISupportInitialize)(this.PropsContainer)).BeginInit();
            this.PropsContainer.Panel1.SuspendLayout();
            this.PropsContainer.Panel2.SuspendLayout();
            this.PropsContainer.SuspendLayout();
            this.tabPropsChoices.SuspendLayout();
            this.tabInteractiveProps.SuspendLayout();
            this.tabPhysicalProps.SuspendLayout();
            this.tabVisualProps.SuspendLayout();
            this.SuspendLayout();
            // 
            // PropsContainer
            // 
            this.PropsContainer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.PropsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropsContainer.Location = new System.Drawing.Point(0, 0);
            this.PropsContainer.Name = "PropsContainer";
            this.PropsContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // PropsContainer.Panel1
            // 
            this.PropsContainer.Panel1.Controls.Add(this.tabPropsChoices);
            // 
            // PropsContainer.Panel2
            // 
            this.PropsContainer.Panel2.Controls.Add(this.pgPropProperties);
            this.PropsContainer.Size = new System.Drawing.Size(389, 450);
            this.PropsContainer.SplitterDistance = 220;
            this.PropsContainer.TabIndex = 9;
            // 
            // tabPropsChoices
            // 
            this.tabPropsChoices.Controls.Add(this.tabInteractiveProps);
            this.tabPropsChoices.Controls.Add(this.tabPhysicalProps);
            this.tabPropsChoices.Controls.Add(this.tabVisualProps);
            this.tabPropsChoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabPropsChoices.Location = new System.Drawing.Point(0, 0);
            this.tabPropsChoices.Multiline = true;
            this.tabPropsChoices.Name = "tabPropsChoices";
            this.tabPropsChoices.SelectedIndex = 0;
            this.tabPropsChoices.Size = new System.Drawing.Size(385, 216);
            this.tabPropsChoices.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.tabPropsChoices.TabIndex = 0;
            // 
            // tabInteractiveProps
            // 
            this.tabInteractiveProps.Controls.Add(this.lsInteractiveProps);
            this.tabInteractiveProps.Location = new System.Drawing.Point(4, 22);
            this.tabInteractiveProps.Name = "tabInteractiveProps";
            this.tabInteractiveProps.Size = new System.Drawing.Size(377, 190);
            this.tabInteractiveProps.TabIndex = 2;
            this.tabInteractiveProps.Text = "Interactive";
            this.tabInteractiveProps.UseVisualStyleBackColor = true;
            // 
            // lsInteractiveProps
            // 
            this.lsInteractiveProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsInteractiveProps.FormattingEnabled = true;
            this.lsInteractiveProps.Location = new System.Drawing.Point(0, 0);
            this.lsInteractiveProps.Name = "lsInteractiveProps";
            this.lsInteractiveProps.Size = new System.Drawing.Size(377, 190);
            this.lsInteractiveProps.TabIndex = 0;
            this.lsInteractiveProps.SelectedIndexChanged += new System.EventHandler(this.lsInteractiveProps_SelectedIndexChanged);
            // 
            // tabPhysicalProps
            // 
            this.tabPhysicalProps.Controls.Add(this.lsPhysicalProps);
            this.tabPhysicalProps.Location = new System.Drawing.Point(4, 22);
            this.tabPhysicalProps.Name = "tabPhysicalProps";
            this.tabPhysicalProps.Size = new System.Drawing.Size(377, 190);
            this.tabPhysicalProps.TabIndex = 0;
            this.tabPhysicalProps.Text = "Physical";
            this.tabPhysicalProps.UseVisualStyleBackColor = true;
            // 
            // lsPhysicalProps
            // 
            this.lsPhysicalProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsPhysicalProps.FormattingEnabled = true;
            this.lsPhysicalProps.Location = new System.Drawing.Point(0, 0);
            this.lsPhysicalProps.Name = "lsPhysicalProps";
            this.lsPhysicalProps.Size = new System.Drawing.Size(377, 190);
            this.lsPhysicalProps.TabIndex = 1;
            this.lsPhysicalProps.SelectedIndexChanged += new System.EventHandler(this.lsInteractiveProps_SelectedIndexChanged);
            // 
            // tabVisualProps
            // 
            this.tabVisualProps.Controls.Add(this.lsVisualProps);
            this.tabVisualProps.Location = new System.Drawing.Point(4, 22);
            this.tabVisualProps.Name = "tabVisualProps";
            this.tabVisualProps.Size = new System.Drawing.Size(377, 190);
            this.tabVisualProps.TabIndex = 1;
            this.tabVisualProps.Text = "Visual";
            this.tabVisualProps.UseVisualStyleBackColor = true;
            // 
            // lsVisualProps
            // 
            this.lsVisualProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsVisualProps.FormattingEnabled = true;
            this.lsVisualProps.Location = new System.Drawing.Point(0, 0);
            this.lsVisualProps.Name = "lsVisualProps";
            this.lsVisualProps.Size = new System.Drawing.Size(377, 190);
            this.lsVisualProps.TabIndex = 0;
            this.lsVisualProps.SelectedIndexChanged += new System.EventHandler(this.lsInteractiveProps_SelectedIndexChanged);
            // 
            // pgPropProperties
            // 
            this.pgPropProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgPropProperties.Location = new System.Drawing.Point(0, 0);
            this.pgPropProperties.Name = "pgPropProperties";
            this.pgPropProperties.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.pgPropProperties.Size = new System.Drawing.Size(385, 222);
            this.pgPropProperties.TabIndex = 0;
            this.pgPropProperties.ToolbarVisible = false;
            // 
            // PropPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 450);
            this.Controls.Add(this.PropsContainer);
            this.Name = "PropPicker";
            this.Text = "Prop Picker";
            this.PropsContainer.Panel1.ResumeLayout(false);
            this.PropsContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PropsContainer)).EndInit();
            this.PropsContainer.ResumeLayout(false);
            this.tabPropsChoices.ResumeLayout(false);
            this.tabInteractiveProps.ResumeLayout(false);
            this.tabPhysicalProps.ResumeLayout(false);
            this.tabVisualProps.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer PropsContainer;
        private System.Windows.Forms.TabControl tabPropsChoices;
        private System.Windows.Forms.TabPage tabInteractiveProps;
        private System.Windows.Forms.ListBox lsInteractiveProps;
        private System.Windows.Forms.TabPage tabPhysicalProps;
        private System.Windows.Forms.ListBox lsPhysicalProps;
        private System.Windows.Forms.TabPage tabVisualProps;
        private System.Windows.Forms.ListBox lsVisualProps;
        public System.Windows.Forms.PropertyGrid pgPropProperties;
    }
}