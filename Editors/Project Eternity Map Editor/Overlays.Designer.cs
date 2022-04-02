
namespace ProjectEternity.Editors.MapEditor
{
    partial class MapOverlays
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
            this.lblTimeMultiplier = new System.Windows.Forms.Label();
            this.txtlblTimeMultiplier = new System.Windows.Forms.NumericUpDown();
            this.lblTypeOfSky = new System.Windows.Forms.Label();
            this.cbTypeOfSky = new System.Windows.Forms.ComboBox();
            this.gbTimeLimits = new System.Windows.Forms.GroupBox();
            this.lblHoursInDay = new System.Windows.Forms.Label();
            this.txtHoursInDay = new System.Windows.Forms.NumericUpDown();
            this.lblTimeStart = new System.Windows.Forms.Label();
            this.txtTimeStart = new System.Windows.Forms.NumericUpDown();
            this.gbTimePeriods = new System.Windows.Forms.GroupBox();
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
            this.rbLoopFirstDay = new System.Windows.Forms.RadioButton();
            this.rbLoopLastDay = new System.Windows.Forms.RadioButton();
            this.rbStopTime = new System.Windows.Forms.RadioButton();
            this.rbUseRealTime = new System.Windows.Forms.RadioButton();
            this.rbUseTurns = new System.Windows.Forms.RadioButton();
            this.gbTimeOfDay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtlblTimeMultiplier)).BeginInit();
            this.gbTimeLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoursInDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeStart)).BeginInit();
            this.gbTimePeriods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodDayStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodTimeStart)).BeginInit();
            this.gbWind.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindDirection)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindSpeed)).BeginInit();
            this.SuspendLayout();
            // 
            // gbTimeOfDay
            // 
            this.gbTimeOfDay.Controls.Add(this.rbUseTurns);
            this.gbTimeOfDay.Controls.Add(this.rbUseRealTime);
            this.gbTimeOfDay.Controls.Add(this.lblVisibility);
            this.gbTimeOfDay.Controls.Add(this.cbVisibility);
            this.gbTimeOfDay.Controls.Add(this.lblWeather);
            this.gbTimeOfDay.Controls.Add(this.cbWeather);
            this.gbTimeOfDay.Controls.Add(this.lblTimeMultiplier);
            this.gbTimeOfDay.Controls.Add(this.txtlblTimeMultiplier);
            this.gbTimeOfDay.Controls.Add(this.lblTypeOfSky);
            this.gbTimeOfDay.Controls.Add(this.cbTypeOfSky);
            this.gbTimeOfDay.Location = new System.Drawing.Point(213, 12);
            this.gbTimeOfDay.Name = "gbTimeOfDay";
            this.gbTimeOfDay.Size = new System.Drawing.Size(215, 151);
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
            // lblTimeMultiplier
            // 
            this.lblTimeMultiplier.AutoSize = true;
            this.lblTimeMultiplier.Location = new System.Drawing.Point(6, 102);
            this.lblTimeMultiplier.Name = "lblTimeMultiplier";
            this.lblTimeMultiplier.Size = new System.Drawing.Size(73, 13);
            this.lblTimeMultiplier.TabIndex = 4;
            this.lblTimeMultiplier.Text = "Time multiplier";
            // 
            // txtlblTimeMultiplier
            // 
            this.txtlblTimeMultiplier.Location = new System.Drawing.Point(89, 100);
            this.txtlblTimeMultiplier.Name = "txtlblTimeMultiplier";
            this.txtlblTimeMultiplier.Size = new System.Drawing.Size(56, 20);
            this.txtlblTimeMultiplier.TabIndex = 3;
            this.txtlblTimeMultiplier.ValueChanged += new System.EventHandler(this.txtlblTimeMultiplier_ValueChanged);
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
            "Arctic"});
            this.cbTypeOfSky.Location = new System.Drawing.Point(88, 46);
            this.cbTypeOfSky.Name = "cbTypeOfSky";
            this.cbTypeOfSky.Size = new System.Drawing.Size(121, 21);
            this.cbTypeOfSky.TabIndex = 1;
            this.cbTypeOfSky.SelectedIndexChanged += new System.EventHandler(this.cbTypeOfSky_SelectedIndexChanged);
            // 
            // gbTimeLimits
            // 
            this.gbTimeLimits.Controls.Add(this.rbStopTime);
            this.gbTimeLimits.Controls.Add(this.rbLoopLastDay);
            this.gbTimeLimits.Controls.Add(this.rbLoopFirstDay);
            this.gbTimeLimits.Controls.Add(this.lblHoursInDay);
            this.gbTimeLimits.Controls.Add(this.txtHoursInDay);
            this.gbTimeLimits.Controls.Add(this.lblTimeStart);
            this.gbTimeLimits.Controls.Add(this.txtTimeStart);
            this.gbTimeLimits.Location = new System.Drawing.Point(12, 12);
            this.gbTimeLimits.Name = "gbTimeLimits";
            this.gbTimeLimits.Size = new System.Drawing.Size(195, 151);
            this.gbTimeLimits.TabIndex = 1;
            this.gbTimeLimits.TabStop = false;
            this.gbTimeLimits.Text = "Time limits";
            // 
            // lblHoursInDay
            // 
            this.lblHoursInDay.AutoSize = true;
            this.lblHoursInDay.Location = new System.Drawing.Point(6, 43);
            this.lblHoursInDay.Name = "lblHoursInDay";
            this.lblHoursInDay.Size = new System.Drawing.Size(66, 13);
            this.lblHoursInDay.TabIndex = 8;
            this.lblHoursInDay.Text = "Hours in day";
            // 
            // txtHoursInDay
            // 
            this.txtHoursInDay.Location = new System.Drawing.Point(133, 41);
            this.txtHoursInDay.Name = "txtHoursInDay";
            this.txtHoursInDay.Size = new System.Drawing.Size(56, 20);
            this.txtHoursInDay.TabIndex = 7;
            this.txtHoursInDay.Value = new decimal(new int[] {
            24,
            0,
            0,
            0});
            // 
            // lblTimeStart
            // 
            this.lblTimeStart.AutoSize = true;
            this.lblTimeStart.Location = new System.Drawing.Point(6, 17);
            this.lblTimeStart.Name = "lblTimeStart";
            this.lblTimeStart.Size = new System.Drawing.Size(53, 13);
            this.lblTimeStart.TabIndex = 6;
            this.lblTimeStart.Text = "Time start";
            // 
            // txtTimeStart
            // 
            this.txtTimeStart.Location = new System.Drawing.Point(133, 15);
            this.txtTimeStart.Name = "txtTimeStart";
            this.txtTimeStart.Size = new System.Drawing.Size(56, 20);
            this.txtTimeStart.TabIndex = 5;
            // 
            // gbTimePeriods
            // 
            this.gbTimePeriods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTimePeriods.Controls.Add(this.txtTimePeriodDayStart);
            this.gbTimePeriods.Controls.Add(this.lblTimePeriodDayStart);
            this.gbTimePeriods.Controls.Add(this.btnRemoveTimePeriod);
            this.gbTimePeriods.Controls.Add(this.btnAddTimePeriod);
            this.gbTimePeriods.Controls.Add(this.lsTimePeriod);
            this.gbTimePeriods.Controls.Add(this.txtTimePeriodTimeStart);
            this.gbTimePeriods.Controls.Add(this.lblTimePeriodTimeStart);
            this.gbTimePeriods.Location = new System.Drawing.Point(434, 12);
            this.gbTimePeriods.Name = "gbTimePeriods";
            this.gbTimePeriods.Size = new System.Drawing.Size(177, 257);
            this.gbTimePeriods.TabIndex = 2;
            this.gbTimePeriods.TabStop = false;
            this.gbTimePeriods.Text = "Time periods";
            // 
            // txtTimePeriodDayStart
            // 
            this.txtTimePeriodDayStart.Location = new System.Drawing.Point(111, 198);
            this.txtTimePeriodDayStart.Name = "txtTimePeriodDayStart";
            this.txtTimePeriodDayStart.Size = new System.Drawing.Size(57, 20);
            this.txtTimePeriodDayStart.TabIndex = 6;
            this.txtTimePeriodDayStart.ValueChanged += new System.EventHandler(this.txtTimePeriodDayStart_ValueChanged);
            // 
            // lblTimePeriodDayStart
            // 
            this.lblTimePeriodDayStart.AutoSize = true;
            this.lblTimePeriodDayStart.Location = new System.Drawing.Point(6, 200);
            this.lblTimePeriodDayStart.Name = "lblTimePeriodDayStart";
            this.lblTimePeriodDayStart.Size = new System.Drawing.Size(49, 13);
            this.lblTimePeriodDayStart.TabIndex = 5;
            this.lblTimePeriodDayStart.Text = "Day start";
            // 
            // btnRemoveTimePeriod
            // 
            this.btnRemoveTimePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveTimePeriod.Location = new System.Drawing.Point(96, 228);
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
            this.btnAddTimePeriod.Location = new System.Drawing.Point(9, 228);
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
            this.lsTimePeriod.Size = new System.Drawing.Size(162, 147);
            this.lsTimePeriod.TabIndex = 2;
            // 
            // txtTimePeriodTimeStart
            // 
            this.txtTimePeriodTimeStart.Location = new System.Drawing.Point(111, 172);
            this.txtTimePeriodTimeStart.Name = "txtTimePeriodTimeStart";
            this.txtTimePeriodTimeStart.Size = new System.Drawing.Size(57, 20);
            this.txtTimePeriodTimeStart.TabIndex = 1;
            this.txtTimePeriodTimeStart.ValueChanged += new System.EventHandler(this.txtTimePeriodTimeStart_ValueChanged);
            // 
            // lblTimePeriodTimeStart
            // 
            this.lblTimePeriodTimeStart.AutoSize = true;
            this.lblTimePeriodTimeStart.Location = new System.Drawing.Point(6, 174);
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
            this.gbWind.Location = new System.Drawing.Point(213, 169);
            this.gbWind.Name = "gbWind";
            this.gbWind.Size = new System.Drawing.Size(215, 74);
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
            // rbLoopFirstDay
            // 
            this.rbLoopFirstDay.AutoSize = true;
            this.rbLoopFirstDay.Location = new System.Drawing.Point(6, 67);
            this.rbLoopFirstDay.Name = "rbLoopFirstDay";
            this.rbLoopFirstDay.Size = new System.Drawing.Size(127, 17);
            this.rbLoopFirstDay.TabIndex = 10;
            this.rbLoopFirstDay.TabStop = true;
            this.rbLoopFirstDay.Text = "Loop back to first day";
            this.rbLoopFirstDay.UseVisualStyleBackColor = true;
            // 
            // rbLoopLastDay
            // 
            this.rbLoopLastDay.AutoSize = true;
            this.rbLoopLastDay.Location = new System.Drawing.Point(6, 90);
            this.rbLoopLastDay.Name = "rbLoopLastDay";
            this.rbLoopLastDay.Size = new System.Drawing.Size(127, 17);
            this.rbLoopLastDay.TabIndex = 11;
            this.rbLoopLastDay.TabStop = true;
            this.rbLoopLastDay.Text = "Loop back to last day";
            this.rbLoopLastDay.UseVisualStyleBackColor = true;
            // 
            // rbStopTime
            // 
            this.rbStopTime.AutoSize = true;
            this.rbStopTime.Location = new System.Drawing.Point(6, 113);
            this.rbStopTime.Name = "rbStopTime";
            this.rbStopTime.Size = new System.Drawing.Size(105, 17);
            this.rbStopTime.TabIndex = 12;
            this.rbStopTime.TabStop = true;
            this.rbStopTime.Text = "Stop time on end";
            this.rbStopTime.UseVisualStyleBackColor = true;
            // 
            // rbUseRealTime
            // 
            this.rbUseRealTime.AutoSize = true;
            this.rbUseRealTime.Location = new System.Drawing.Point(123, 126);
            this.rbUseRealTime.Name = "rbUseRealTime";
            this.rbUseRealTime.Size = new System.Drawing.Size(86, 17);
            this.rbUseRealTime.TabIndex = 13;
            this.rbUseRealTime.TabStop = true;
            this.rbUseRealTime.Text = "Use real time";
            this.rbUseRealTime.UseVisualStyleBackColor = true;
            this.rbUseRealTime.CheckedChanged += new System.EventHandler(this.rbUseRealTime_CheckedChanged);
            // 
            // rbUseTurns
            // 
            this.rbUseTurns.AutoSize = true;
            this.rbUseTurns.Location = new System.Drawing.Point(6, 126);
            this.rbUseTurns.Name = "rbUseTurns";
            this.rbUseTurns.Size = new System.Drawing.Size(70, 17);
            this.rbUseTurns.TabIndex = 14;
            this.rbUseTurns.TabStop = true;
            this.rbUseTurns.Text = "Use turns";
            this.rbUseTurns.UseVisualStyleBackColor = true;
            this.rbUseTurns.CheckedChanged += new System.EventHandler(this.rbUseTurns_CheckedChanged);
            // 
            // MapOverlays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 281);
            this.Controls.Add(this.gbWind);
            this.Controls.Add(this.gbTimePeriods);
            this.Controls.Add(this.gbTimeLimits);
            this.Controls.Add(this.gbTimeOfDay);
            this.Name = "MapOverlays";
            this.Text = "Overlays";
            this.gbTimeOfDay.ResumeLayout(false);
            this.gbTimeOfDay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtlblTimeMultiplier)).EndInit();
            this.gbTimeLimits.ResumeLayout(false);
            this.gbTimeLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoursInDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeStart)).EndInit();
            this.gbTimePeriods.ResumeLayout(false);
            this.gbTimePeriods.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodDayStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimePeriodTimeStart)).EndInit();
            this.gbWind.ResumeLayout(false);
            this.gbWind.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindDirection)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtWindSpeed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbTimeOfDay;
        private System.Windows.Forms.Label lblWeather;
        private System.Windows.Forms.ComboBox cbWeather;
        private System.Windows.Forms.Label lblTimeMultiplier;
        private System.Windows.Forms.NumericUpDown txtlblTimeMultiplier;
        private System.Windows.Forms.Label lblTypeOfSky;
        private System.Windows.Forms.ComboBox cbTypeOfSky;
        private System.Windows.Forms.GroupBox gbTimeLimits;
        private System.Windows.Forms.Label lblHoursInDay;
        private System.Windows.Forms.NumericUpDown txtHoursInDay;
        private System.Windows.Forms.Label lblTimeStart;
        private System.Windows.Forms.NumericUpDown txtTimeStart;
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
        private System.Windows.Forms.RadioButton rbUseTurns;
        private System.Windows.Forms.RadioButton rbUseRealTime;
        private System.Windows.Forms.RadioButton rbStopTime;
        private System.Windows.Forms.RadioButton rbLoopLastDay;
        private System.Windows.Forms.RadioButton rbLoopFirstDay;
    }
}