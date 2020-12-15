namespace ProjectEternity.Editors.Map
{
    partial class Visual_Novel_Creator
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lstTriggers = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lstEvents = new System.Windows.Forms.ListBox();
            this.btnAddTrigger = new System.Windows.Forms.Button();
            this.btnDeleteTrigger = new System.Windows.Forms.Button();
            this.btnMoveUpTrigger = new System.Windows.Forms.Button();
            this.btnMoveDownTrigger = new System.Windows.Forms.Button();
            this.btnMoveDownEvent = new System.Windows.Forms.Button();
            this.btnMoveUpEvent = new System.Windows.Forms.Button();
            this.btnDeleteEvent = new System.Windows.Forms.Button();
            this.btnAddEvent = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(12, 353);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 48);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Visual Novel Info";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Visual Novel Name:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(112, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(142, 20);
            this.textBox1.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnMoveDownEvent);
            this.groupBox2.Controls.Add(this.btnMoveUpEvent);
            this.groupBox2.Controls.Add(this.btnDeleteEvent);
            this.groupBox2.Controls.Add(this.btnAddEvent);
            this.groupBox2.Controls.Add(this.btnMoveDownTrigger);
            this.groupBox2.Controls.Add(this.btnMoveUpTrigger);
            this.groupBox2.Controls.Add(this.btnDeleteTrigger);
            this.groupBox2.Controls.Add(this.btnAddTrigger);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lstEvents);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.lstTriggers);
            this.groupBox2.Location = new System.Drawing.Point(12, 66);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(617, 252);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Scripting";
            // 
            // lstTriggers
            // 
            this.lstTriggers.FormattingEnabled = true;
            this.lstTriggers.Location = new System.Drawing.Point(6, 43);
            this.lstTriggers.Name = "lstTriggers";
            this.lstTriggers.Size = new System.Drawing.Size(129, 160);
            this.lstTriggers.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Triggers";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(235, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Events";
            // 
            // lstEvents
            // 
            this.lstEvents.FormattingEnabled = true;
            this.lstEvents.Location = new System.Drawing.Point(235, 43);
            this.lstEvents.Name = "lstEvents";
            this.lstEvents.Size = new System.Drawing.Size(129, 160);
            this.lstEvents.TabIndex = 4;
            // 
            // btnAddTrigger
            // 
            this.btnAddTrigger.Location = new System.Drawing.Point(141, 43);
            this.btnAddTrigger.Name = "btnAddTrigger";
            this.btnAddTrigger.Size = new System.Drawing.Size(88, 23);
            this.btnAddTrigger.TabIndex = 6;
            this.btnAddTrigger.Text = "Add Trigger";
            this.btnAddTrigger.UseVisualStyleBackColor = true;
            // 
            // btnDeleteTrigger
            // 
            this.btnDeleteTrigger.Location = new System.Drawing.Point(141, 72);
            this.btnDeleteTrigger.Name = "btnDeleteTrigger";
            this.btnDeleteTrigger.Size = new System.Drawing.Size(88, 23);
            this.btnDeleteTrigger.TabIndex = 7;
            this.btnDeleteTrigger.Text = "Delete Trigger";
            this.btnDeleteTrigger.UseVisualStyleBackColor = true;
            // 
            // btnMoveUpTrigger
            // 
            this.btnMoveUpTrigger.Location = new System.Drawing.Point(141, 151);
            this.btnMoveUpTrigger.Name = "btnMoveUpTrigger";
            this.btnMoveUpTrigger.Size = new System.Drawing.Size(88, 23);
            this.btnMoveUpTrigger.TabIndex = 8;
            this.btnMoveUpTrigger.Text = "Move Up";
            this.btnMoveUpTrigger.UseVisualStyleBackColor = true;
            // 
            // btnMoveDownTrigger
            // 
            this.btnMoveDownTrigger.Location = new System.Drawing.Point(141, 180);
            this.btnMoveDownTrigger.Name = "btnMoveDownTrigger";
            this.btnMoveDownTrigger.Size = new System.Drawing.Size(88, 23);
            this.btnMoveDownTrigger.TabIndex = 9;
            this.btnMoveDownTrigger.Text = "Move Down";
            this.btnMoveDownTrigger.UseVisualStyleBackColor = true;
            // 
            // btnMoveDownEvent
            // 
            this.btnMoveDownEvent.Location = new System.Drawing.Point(370, 180);
            this.btnMoveDownEvent.Name = "btnMoveDownEvent";
            this.btnMoveDownEvent.Size = new System.Drawing.Size(88, 23);
            this.btnMoveDownEvent.TabIndex = 13;
            this.btnMoveDownEvent.Text = "Move Down";
            this.btnMoveDownEvent.UseVisualStyleBackColor = true;
            // 
            // btnMoveUpEvent
            // 
            this.btnMoveUpEvent.Location = new System.Drawing.Point(370, 151);
            this.btnMoveUpEvent.Name = "btnMoveUpEvent";
            this.btnMoveUpEvent.Size = new System.Drawing.Size(88, 23);
            this.btnMoveUpEvent.TabIndex = 12;
            this.btnMoveUpEvent.Text = "Move Up";
            this.btnMoveUpEvent.UseVisualStyleBackColor = true;
            // 
            // btnDeleteEvent
            // 
            this.btnDeleteEvent.Location = new System.Drawing.Point(370, 72);
            this.btnDeleteEvent.Name = "btnDeleteEvent";
            this.btnDeleteEvent.Size = new System.Drawing.Size(88, 23);
            this.btnDeleteEvent.TabIndex = 11;
            this.btnDeleteEvent.Text = "Delete Event";
            this.btnDeleteEvent.UseVisualStyleBackColor = true;
            // 
            // btnAddEvent
            // 
            this.btnAddEvent.Location = new System.Drawing.Point(370, 43);
            this.btnAddEvent.Name = "btnAddEvent";
            this.btnAddEvent.Size = new System.Drawing.Size(88, 23);
            this.btnAddEvent.TabIndex = 10;
            this.btnAddEvent.Text = "Add Event";
            this.btnAddEvent.UseVisualStyleBackColor = true;
            // 
            // Visual_Novel_Creator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 466);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnAccept);
            this.Name = "Visual_Novel_Creator";
            this.Text = "Visual_Novel_Creator";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lstEvents;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstTriggers;
        private System.Windows.Forms.Button btnMoveDownTrigger;
        private System.Windows.Forms.Button btnMoveUpTrigger;
        private System.Windows.Forms.Button btnDeleteTrigger;
        private System.Windows.Forms.Button btnAddTrigger;
        private System.Windows.Forms.Button btnMoveDownEvent;
        private System.Windows.Forms.Button btnMoveUpEvent;
        private System.Windows.Forms.Button btnDeleteEvent;
        private System.Windows.Forms.Button btnAddEvent;
    }
}