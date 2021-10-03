using ScintillaNET;

namespace Roslyn
{
    partial class CodeEditor
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
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.scCodeEditor = new ScintillaNET.Scintilla();
            this.lvErrors = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.scCodeEditor);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvErrors);
            this.splitContainer2.Size = new System.Drawing.Size(585, 365);
            this.splitContainer2.SplitterDistance = 215;
            this.splitContainer2.TabIndex = 2;
            // 
            // scCodeEditor
            // 
            this.scCodeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scCodeEditor.Lexer = ScintillaNET.Lexer.Cpp;
            this.scCodeEditor.Location = new System.Drawing.Point(0, 0);
            this.scCodeEditor.Name = "scCodeEditor";
            this.scCodeEditor.Size = new System.Drawing.Size(581, 211);
            this.scCodeEditor.TabIndex = 1;
            this.scCodeEditor.UseTabs = false;
            this.scCodeEditor.TextChanged += new System.EventHandler(this.scCodeEditor_TextChanged);
            // 
            // lvErrors
            // 
            this.lvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvErrors.Location = new System.Drawing.Point(0, 0);
            this.lvErrors.Name = "lvErrors";
            this.lvErrors.Size = new System.Drawing.Size(581, 142);
            this.lvErrors.TabIndex = 0;
            this.lvErrors.UseCompatibleStateImageBehavior = false;
            this.lvErrors.View = System.Windows.Forms.View.List;
            // 
            // CodeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Name = "CodeEditor";
            this.Size = new System.Drawing.Size(585, 365);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListView lvErrors;
        private Scintilla scCodeEditor;
    }
}