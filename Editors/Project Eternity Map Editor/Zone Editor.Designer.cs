
namespace ProjectEternity.Editors.MapEditor
{
    partial class ZoneEditor
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
            this.gbTimeOfDay = new System.Windows.Forms.GroupBox();
            this.lblVisibility = new System.Windows.Forms.Label();
            this.cbVisibility = new System.Windows.Forms.ComboBox();
            this.lblWeather = new System.Windows.Forms.Label();
            this.cbWeather = new System.Windows.Forms.ComboBox();
            this.lblTypeOfSky = new System.Windows.Forms.Label();
            this.cbTypeOfSky = new System.Windows.Forms.ComboBox();
            this.gbTimePeriods = new System.Windows.Forms.GroupBox();
            this.txtTimePeriodPassiveSkill = new System.Windows.Forms.TextBox();
            this.btnTimePeriodPassiveSkill = new System.Windows.Forms.Button();
            this.lblTimePeriodPassiveSkill = new System.Windows.Forms.Label();
            this.lblTimePeriodName = new System.Windows.Forms.Label();
            this.txtTimePeriodName = new System.Windows.Forms.TextBox();
            this.txtTimePeriodDayStart = new System.Windows.Forms.NumericUpDown();
            this.lblTimePeriodDayStart = new System.Windows.Forms.Label();
            this.btnRemoveTimePeriod = new System.Windows.Forms.Button();
            this.btnAddTimePeriod = new System.Windows.Forms.Button();
            this.lsTimePeriod = new System.Windows.Forms.ListBox();
            this.txtTimePeriodTimeStart = new System.Windows.Forms.NumericUpDown();
            this.lblTimePeriodTimeStart = new System.Windows.Forms.Label();
            this.gbWind = new System.Windows.Forms.GroupBox();
            this.lblWindDirection = new System.Windows.Forms.Label();
            this.txtWindDirection = new System.Windows.Forms.NumericUpDown();
            this.lblWindSpeed = new System.Windows.Forms.Label();
            this.txtWindSpeed = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTransitionCrossfadeLength = new System.Windows.Forms.Label();
            this.txtTransitionCrossfadeLength = new System.Windows.Forms.NumericUpDown();
            this.lblTransitionEndLength = new System.Windows.Forms.Label();
            this.txtTransitionEndLength = new System.Windows.Forms.NumericUpDown();
            this.lblTransitionStartLength = new System.Windows.Forms.Label();
            this.txtTransitionStartLength = new System.Windows.Forms.NumericUpDown();
            this.gbTimeOfDay.SuspendLayout();
            this.gbTimePeriods.SuspendLayout();
            this.txtTimePeriodPassiveSkill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodDayStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodTimeStart)).BeginInit();
            this.gbWind.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindDirection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindSpeed)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransitionCrossfadeLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransitionEndLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransitionStartLength)).BeginInit();
            this.SuspendLayout();
            // 
            // gbTimeOfDay
            // 
            this.gbTimeOfDay.Controls.Add(this.lblVisibility);
            this.gbTimeOfDay.Controls.Add(this.cbVisibility);
            this.gbTimeOfDay.Controls.Add(this.lblWeather);
            this.gbTimeOfDay.Controls.Add(this.cbWeather);
            this.gbTimeOfDay.Controls.Add(this.lblTypeOfSky);
            this.gbTimeOfDay.Controls.Add(this.cbTypeOfSky);
            this.gbTimeOfDay.Location = new System.Drawing.Point(12, 12);
            this.gbTimeOfDay.Name = "gbTimeOfDay";
            this.gbTimeOfDay.Size = new System.Drawing.Size(215, 107);
            this.gbTimeOfDay.TabIndex = 0;
            this.gbTimeOfDay.TabStop = false;
            this.gbTimeOfDay.Text = "Time of day";
            // 
            // lblVisibility
            // 
            this.lblVisibility.AutoSize = true;
            this.lblVisibility.Location = new System.Drawing.Point(6, 76);
            this.lblVisibility.Name = "lblVisibility";
            this.lblVisibility.Size = new System.Drawing.Size(43, 13);
            this.lblVisibility.TabIndex = 8;
            this.lblVisibility.Text = "Visibility";
            // 
            // cbVisibility
            // 
            this.cbVisibility.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVisibility.FormattingEnabled = true;
            this.cbVisibility.Items.AddRange(new object[] {
            "Regular",
            "Fog of war"});
            this.cbVisibility.Location = new System.Drawing.Point(88, 73);
            this.cbVisibility.Name = "cbVisibility";
            this.cbVisibility.Size = new System.Drawing.Size(121, 21);
            this.cbVisibility.TabIndex = 7;
            this.cbVisibility.SelectedIndexChanged += new System.EventHandler(this.cbVisibility_SelectedIndexChanged);
            // 
            // lblWeather
            // 
            this.lblWeather.AutoSize = true;
            this.lblWeather.Location = new System.Drawing.Point(6, 22);
            this.lblWeather.Name = "lblWeather";
            this.lblWeather.Size = new System.Drawing.Size(48, 13);
            this.lblWeather.TabIndex = 6;
            this.lblWeather.Text = "Weather";
            // 
            // cbWeather
            // 
            this.cbWeather.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWeather.FormattingEnabled = true;
            this.cbWeather.Items.AddRange(new object[] {
            "Regular",
            "Windy",
            "Some clouds",
            "Cloudy",
            "Small rain",
            "Heavy rain",
            "Thunderstorm",
            "Light snow",
            "Heavy snow",
            "Sandstorm",
            "Starfall",
            "Distortion",
            "Cherry blosom",
            "Falling leaves",
            "Fireflies",
            "Minovsky",
            "Getter"});
            this.cbWeather.Location = new System.Drawing.Point(88, 19);
            this.cbWeather.Name = "cbWeather";
            this.cbWeather.Size = new System.Drawing.Size(121, 21);
            this.cbWeather.TabIndex = 5;
            this.cbWeather.SelectedIndexChanged += new System.EventHandler(this.cbWeather_SelectedIndexChanged);
            // 
            // lblTypeOfSky
            // 
            this.lblTypeOfSky.AutoSize = true;
            this.lblTypeOfSky.Location = new System.Drawing.Point(6, 49);
            this.lblTypeOfSky.Name = "lblTypeOfSky";
            this.lblTypeOfSky.Size = new System.Drawing.Size(62, 13);
            this.lblTypeOfSky.TabIndex = 2;
            this.lblTypeOfSky.Text = "Type of sky";
            // 
            // cbTypeOfSky
            // 
            this.cbTypeOfSky.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeOfSky.FormattingEnabled = true;
            this.cbTypeOfSky.Items.AddRange(new object[] {
            "Disabled",
            "Regular",
            "Arctic",
            "Warm/orange"});
            this.cbTypeOfSky.Location = new System.Drawing.Point(88, 46);
            this.cbTypeOfSky.Name = "cbTypeOfSky";
            this.cbTypeOfSky.Size = new System.Drawing.Size(121, 21);
            this.cbTypeOfSky.TabIndex = 1;
            this.cbTypeOfSky.SelectedIndexChanged += new System.EventHandler(this.cbTypeOfSky_SelectedIndexChanged);
            // 
            // gbTimePeriods
            // 
            this.gbTimePeriods.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.gbTimePeriods.Controls.Add(this.txtTimePeriodPassiveSkill);
            this.gbTimePeriods.Controls.Add(this.lblTimePeriodPassiveSkill);
            this.gbTimePeriods.Controls.Add(this.lblTimePeriodName);
            this.gbTimePeriods.Controls.Add(this.txtTimePeriodName);
            this.gbTimePeriods.Controls.Add(this.txtTimePeriodDayStart);
            this.gbTimePeriods.Controls.Add(this.lblTimePeriodDayStart);
            this.gbTimePeriods.Controls.Add(this.btnRemoveTimePeriod);
            this.gbTimePeriods.Controls.Add(this.btnAddTimePeriod);
            this.gbTimePeriods.Controls.Add(this.lsTimePeriod);
            this.gbTimePeriods.Controls.Add(this.txtTimePeriodTimeStart);
            this.gbTimePeriods.Controls.Add(this.lblTimePeriodTimeStart);
            this.gbTimePeriods.Location = new System.Drawing.Point(233, 12);
            this.gbTimePeriods.Name = "gbTimePeriods";
            this.gbTimePeriods.Size = new System.Drawing.Size(212, 305);
            this.gbTimePeriods.TabIndex = 2;
            this.gbTimePeriods.TabStop = false;
            this.gbTimePeriods.Text = "Time periods";
            // 
            // txtTimePeriodPassiveSkill
            // 
            this.txtTimePeriodPassiveSkill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTimePeriodPassiveSkill.Controls.Add(this.btnTimePeriodPassiveSkill);
            this.txtTimePeriodPassiveSkill.Location = new System.Drawing.Point(76, 250);
            this.txtTimePeriodPassiveSkill.Name = "txtTimePeriodPassiveSkill";
            this.txtTimePeriodPassiveSkill.Size = new System.Drawing.Size(130, 20);
            this.txtTimePeriodPassiveSkill.TabIndex = 34;
            // 
            // btnTimePeriodPassiveSkill
            // 
            this.btnTimePeriodPassiveSkill.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnTimePeriodPassiveSkill.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnTimePeriodPassiveSkill.Location = new System.Drawing.Point(105, 0);
            this.btnTimePeriodPassiveSkill.Margin = new System.Windows.Forms.Padding(0);
            this.btnTimePeriodPassiveSkill.Name = "btnTimePeriodPassiveSkill";
            this.btnTimePeriodPassiveSkill.Size = new System.Drawing.Size(21, 16);
            this.btnTimePeriodPassiveSkill.TabIndex = 33;
            this.btnTimePeriodPassiveSkill.TabStop = false;
            this.btnTimePeriodPassiveSkill.Text = "...";
            this.btnTimePeriodPassiveSkill.UseVisualStyleBackColor = true;
            this.btnTimePeriodPassiveSkill.Click += new System.EventHandler(this.btnTimePeriodPassiveSkill_Click);
            // 
            // lblTimePeriodPassiveSkill
            // 
            this.lblTimePeriodPassiveSkill.AutoSize = true;
            this.lblTimePeriodPassiveSkill.Location = new System.Drawing.Point(6, 253);
            this.lblTimePeriodPassiveSkill.Name = "lblTimePeriodPassiveSkill";
            this.lblTimePeriodPassiveSkill.Size = new System.Drawing.Size(64, 13);
            this.lblTimePeriodPassiveSkill.TabIndex = 10;
            this.lblTimePeriodPassiveSkill.Text = "Passive skill";
            // 
            // lblTimePeriodName
            // 
            this.lblTimePeriodName.AutoSize = true;
            this.lblTimePeriodName.Location = new System.Drawing.Point(6, 175);
            this.lblTimePeriodName.Name = "lblTimePeriodName";
            this.lblTimePeriodName.Size = new System.Drawing.Size(35, 13);
            this.lblTimePeriodName.TabIndex = 8;
            this.lblTimePeriodName.Text = "Name";
            // 
            // txtTimePeriodName
            // 
            this.txtTimePeriodName.Location = new System.Drawing.Point(47, 172);
            this.txtTimePeriodName.Name = "txtTimePeriodName";
            this.txtTimePeriodName.Size = new System.Drawing.Size(159, 20);
            this.txtTimePeriodName.TabIndex = 7;
            this.txtTimePeriodName.TextChanged += new System.EventHandler(this.txtTimePeriodName_TextChanged);
            // 
            // txtTimePeriodDayStart
            // 
            this.txtTimePeriodDayStart.Location = new System.Drawing.Point(149, 224);
            this.txtTimePeriodDayStart.Name = "txtTimePeriodDayStart";
            this.txtTimePeriodDayStart.Size = new System.Drawing.Size(57, 20);
            this.txtTimePeriodDayStart.TabIndex = 6;
            this.txtTimePeriodDayStart.ValueChanged += new System.EventHandler(this.txtTimePeriodDayStart_ValueChanged);
            // 
            // lblTimePeriodDayStart
            // 
            this.lblTimePeriodDayStart.AutoSize = true;
            this.lblTimePeriodDayStart.Location = new System.Drawing.Point(6, 226);
            this.lblTimePeriodDayStart.Name = "lblTimePeriodDayStart";
            this.lblTimePeriodDayStart.Size = new System.Drawing.Size(49, 13);
            this.lblTimePeriodDayStart.TabIndex = 5;
            this.lblTimePeriodDayStart.Text = "Day start";
            // 
            // btnRemoveTimePeriod
            // 
            this.btnRemoveTimePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveTimePeriod.Location = new System.Drawing.Point(131, 276);
            this.btnRemoveTimePeriod.Name = "btnRemoveTimePeriod";
            this.btnRemoveTimePeriod.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveTimePeriod.TabIndex = 4;
            this.btnRemoveTimePeriod.Text = "Remove";
            this.btnRemoveTimePeriod.UseVisualStyleBackColor = true;
            this.btnRemoveTimePeriod.Click += new System.EventHandler(this.btnRemoveTimePeriod_Click);
            // 
            // btnAddTimePeriod
            // 
            this.btnAddTimePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddTimePeriod.Location = new System.Drawing.Point(44, 276);
            this.btnAddTimePeriod.Name = "btnAddTimePeriod";
            this.btnAddTimePeriod.Size = new System.Drawing.Size(75, 23);
            this.btnAddTimePeriod.TabIndex = 3;
            this.btnAddTimePeriod.Text = "Add";
            this.btnAddTimePeriod.UseVisualStyleBackColor = true;
            this.btnAddTimePeriod.Click += new System.EventHandler(this.btnAddTimePeriod_Click);
            // 
            // lsTimePeriod
            // 
            this.lsTimePeriod.FormattingEnabled = true;
            this.lsTimePeriod.Location = new System.Drawing.Point(6, 19);
            this.lsTimePeriod.Name = "lsTimePeriod";
            this.lsTimePeriod.Size = new System.Drawing.Size(200, 147);
            this.lsTimePeriod.TabIndex = 2;
            this.lsTimePeriod.SelectedIndexChanged += new System.EventHandler(this.lsTimePeriod_SelectedIndexChanged);
            // 
            // txtTimePeriodTimeStart
            // 
            this.txtTimePeriodTimeStart.Location = new System.Drawing.Point(149, 198);
            this.txtTimePeriodTimeStart.Name = "txtTimePeriodTimeStart";
            this.txtTimePeriodTimeStart.Size = new System.Drawing.Size(57, 20);
            this.txtTimePeriodTimeStart.TabIndex = 1;
            this.txtTimePeriodTimeStart.ValueChanged += new System.EventHandler(this.txtTimePeriodTimeStart_ValueChanged);
            // 
            // lblTimePeriodTimeStart
            // 
            this.lblTimePeriodTimeStart.AutoSize = true;
            this.lblTimePeriodTimeStart.Location = new System.Drawing.Point(6, 200);
            this.lblTimePeriodTimeStart.Name = "lblTimePeriodTimeStart";
            this.lblTimePeriodTimeStart.Size = new System.Drawing.Size(53, 13);
            this.lblTimePeriodTimeStart.TabIndex = 0;
            this.lblTimePeriodTimeStart.Text = "Time start";
            // 
            // gbWind
            // 
            this.gbWind.Controls.Add(this.lblWindDirection);
            this.gbWind.Controls.Add(this.txtWindDirection);
            this.gbWind.Controls.Add(this.lblWindSpeed);
            this.gbWind.Controls.Add(this.txtWindSpeed);
            this.gbWind.Location = new System.Drawing.Point(12, 125);
            this.gbWind.Name = "gbWind";
            this.gbWind.Size = new System.Drawing.Size(215, 70);
            this.gbWind.TabIndex = 9;
            this.gbWind.TabStop = false;
            this.gbWind.Text = "Wind";
            // 
            // lblWindDirection
            // 
            this.lblWindDirection.AutoSize = true;
            this.lblWindDirection.Location = new System.Drawing.Point(6, 43);
            this.lblWindDirection.Name = "lblWindDirection";
            this.lblWindDirection.Size = new System.Drawing.Size(93, 13);
            this.lblWindDirection.TabIndex = 8;
            this.lblWindDirection.Text = "Direction (Degree)";
            // 
            // txtWindDirection
            // 
            this.txtWindDirection.Location = new System.Drawing.Point(133, 41);
            this.txtWindDirection.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtWindDirection.Name = "txtWindDirection";
            this.txtWindDirection.Size = new System.Drawing.Size(56, 20);
            this.txtWindDirection.TabIndex = 7;
            this.txtWindDirection.ValueChanged += new System.EventHandler(this.txtWindDirection_ValueChanged);
            // 
            // lblWindSpeed
            // 
            this.lblWindSpeed.AutoSize = true;
            this.lblWindSpeed.Location = new System.Drawing.Point(6, 17);
            this.lblWindSpeed.Name = "lblWindSpeed";
            this.lblWindSpeed.Size = new System.Drawing.Size(38, 13);
            this.lblWindSpeed.TabIndex = 6;
            this.lblWindSpeed.Text = "Speed";
            // 
            // txtWindSpeed
            // 
            this.txtWindSpeed.Location = new System.Drawing.Point(133, 15);
            this.txtWindSpeed.Name = "txtWindSpeed";
            this.txtWindSpeed.Size = new System.Drawing.Size(56, 20);
            this.txtWindSpeed.TabIndex = 5;
            this.txtWindSpeed.ValueChanged += new System.EventHandler(this.txtWindSpeed_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lblTransitionCrossfadeLength);
            this.groupBox1.Controls.Add(this.txtTransitionCrossfadeLength);
            this.groupBox1.Controls.Add(this.lblTransitionEndLength);
            this.groupBox1.Controls.Add(this.txtTransitionEndLength);
            this.groupBox1.Controls.Add(this.lblTransitionStartLength);
            this.groupBox1.Controls.Add(this.txtTransitionStartLength);
            this.groupBox1.Location = new System.Drawing.Point(12, 201);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 116);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transition";
            // 
            // lblTransitionCrossfadeLength
            // 
            this.lblTransitionCrossfadeLength.AutoSize = true;
            this.lblTransitionCrossfadeLength.Location = new System.Drawing.Point(6, 69);
            this.lblTransitionCrossfadeLength.Name = "lblTransitionCrossfadeLength";
            this.lblTransitionCrossfadeLength.Size = new System.Drawing.Size(86, 13);
            this.lblTransitionCrossfadeLength.TabIndex = 12;
            this.lblTransitionCrossfadeLength.Text = "Crossfade length";
            // 
            // txtTransitionCrossfadeLength
            // 
            this.txtTransitionCrossfadeLength.Location = new System.Drawing.Point(133, 67);
            this.txtTransitionCrossfadeLength.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtTransitionCrossfadeLength.Name = "txtTransitionCrossfadeLength";
            this.txtTransitionCrossfadeLength.Size = new System.Drawing.Size(56, 20);
            this.txtTransitionCrossfadeLength.TabIndex = 11;
            this.txtTransitionCrossfadeLength.ValueChanged += new System.EventHandler(this.txtTransitionCrossfadeLength_ValueChanged);
            // 
            // lblTransitionEndLength
            // 
            this.lblTransitionEndLength.AutoSize = true;
            this.lblTransitionEndLength.Location = new System.Drawing.Point(6, 43);
            this.lblTransitionEndLength.Name = "lblTransitionEndLength";
            this.lblTransitionEndLength.Size = new System.Drawing.Size(58, 13);
            this.lblTransitionEndLength.TabIndex = 8;
            this.lblTransitionEndLength.Text = "End length";
            // 
            // txtTransitionEndLength
            // 
            this.txtTransitionEndLength.Location = new System.Drawing.Point(133, 41);
            this.txtTransitionEndLength.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.txtTransitionEndLength.Name = "txtTransitionEndLength";
            this.txtTransitionEndLength.Size = new System.Drawing.Size(56, 20);
            this.txtTransitionEndLength.TabIndex = 7;
            this.txtTransitionEndLength.ValueChanged += new System.EventHandler(this.txtTransitionEndLength_ValueChanged);
            // 
            // lblTransitionStartLength
            // 
            this.lblTransitionStartLength.AutoSize = true;
            this.lblTransitionStartLength.Location = new System.Drawing.Point(6, 17);
            this.lblTransitionStartLength.Name = "lblTransitionStartLength";
            this.lblTransitionStartLength.Size = new System.Drawing.Size(61, 13);
            this.lblTransitionStartLength.TabIndex = 6;
            this.lblTransitionStartLength.Text = "Start length";
            // 
            // txtTransitionStartLength
            // 
            this.txtTransitionStartLength.Location = new System.Drawing.Point(133, 15);
            this.txtTransitionStartLength.Name = "txtTransitionStartLength";
            this.txtTransitionStartLength.Size = new System.Drawing.Size(56, 20);
            this.txtTransitionStartLength.TabIndex = 5;
            this.txtTransitionStartLength.ValueChanged += new System.EventHandler(this.txtTransitionStartLength_ValueChanged);
            // 
            // ZoneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 329);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbWind);
            this.Controls.Add(this.gbTimePeriods);
            this.Controls.Add(this.gbTimeOfDay);
            this.Name = "ZoneEditor";
            this.Text = "Zone properties";
            this.gbTimeOfDay.ResumeLayout(false);
            this.gbTimeOfDay.PerformLayout();
            this.gbTimePeriods.ResumeLayout(false);
            this.gbTimePeriods.PerformLayout();
            this.txtTimePeriodPassiveSkill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodDayStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodTimeStart)).EndInit();
            this.gbWind.ResumeLayout(false);
            this.gbWind.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindDirection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindSpeed)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransitionCrossfadeLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransitionEndLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransitionStartLength)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTimeOfDay;
        private System.Windows.Forms.Label lblWeather;
        private System.Windows.Forms.ComboBox cbWeather;
        private System.Windows.Forms.Label lblTypeOfSky;
        private System.Windows.Forms.ComboBox cbTypeOfSky;
        private System.Windows.Forms.Label lblVisibility;
        private System.Windows.Forms.ComboBox cbVisibility;
        private System.Windows.Forms.GroupBox gbTimePeriods;
        private System.Windows.Forms.GroupBox gbWind;
        private System.Windows.Forms.Label lblWindDirection;
        private System.Windows.Forms.NumericUpDown txtWindDirection;
        private System.Windows.Forms.Label lblWindSpeed;
        private System.Windows.Forms.NumericUpDown txtWindSpeed;
        private System.Windows.Forms.Button btnRemoveTimePeriod;
        private System.Windows.Forms.Button btnAddTimePeriod;
        private System.Windows.Forms.ListBox lsTimePeriod;
        private System.Windows.Forms.NumericUpDown txtTimePeriodTimeStart;
        private System.Windows.Forms.Label lblTimePeriodTimeStart;
        private System.Windows.Forms.NumericUpDown txtTimePeriodDayStart;
        private System.Windows.Forms.Label lblTimePeriodDayStart;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown txtTransitionEndLength;
        private System.Windows.Forms.Label lblTransitionStartLength;
        private System.Windows.Forms.NumericUpDown txtTransitionStartLength;
        private System.Windows.Forms.Label lblTransitionCrossfadeLength;
        private System.Windows.Forms.NumericUpDown txtTransitionCrossfadeLength;
        private System.Windows.Forms.Label lblTransitionEndLength;
        private System.Windows.Forms.Label lblTimePeriodName;
        private System.Windows.Forms.TextBox txtTimePeriodName;
        private System.Windows.Forms.Label lblTimePeriodPassiveSkill;
        private System.Windows.Forms.TextBox txtTimePeriodPassiveSkill;
        private System.Windows.Forms.Button btnTimePeriodPassiveSkill;
    }
}