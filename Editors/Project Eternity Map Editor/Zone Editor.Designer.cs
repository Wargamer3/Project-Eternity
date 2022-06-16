
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
            this.gbWeatherControl = new System.Windows.Forms.GroupBox();
            this.lblWeatherUsage = new System.Windows.Forms.Label();
            this.btnWeatherAdvanced = new System.Windows.Forms.Button();
            this.cbWeatherUsage = new System.Windows.Forms.ComboBox();
            this.lblWeatherType = new System.Windows.Forms.Label();
            this.cbWeatherType = new System.Windows.Forms.ComboBox();
            this.btnSkyAdvanced = new System.Windows.Forms.Button();
            this.lblVisibilityType = new System.Windows.Forms.Label();
            this.cbVisibilityType = new System.Windows.Forms.ComboBox();
            this.lblSkyType = new System.Windows.Forms.Label();
            this.cbSkyType = new System.Windows.Forms.ComboBox();
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
            this.btnWindAdvanced = new System.Windows.Forms.Button();
            this.lblWindUsage = new System.Windows.Forms.Label();
            this.cbWindUsage = new System.Windows.Forms.ComboBox();
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
            this.gbVisibilityControl = new System.Windows.Forms.GroupBox();
            this.lblVisibilityUsage = new System.Windows.Forms.Label();
            this.btnVisibilityAdvanced = new System.Windows.Forms.Button();
            this.cbVisibilityUsage = new System.Windows.Forms.ComboBox();
            this.gbSkyControl = new System.Windows.Forms.GroupBox();
            this.lblSkyUsage = new System.Windows.Forms.Label();
            this.cbSkyUsage = new System.Windows.Forms.ComboBox();
            this.gbWeatherControl.SuspendLayout();
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
            this.gbVisibilityControl.SuspendLayout();
            this.gbSkyControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbWeatherControl
            // 
            this.gbWeatherControl.Controls.Add(this.lblWeatherUsage);
            this.gbWeatherControl.Controls.Add(this.btnWeatherAdvanced);
            this.gbWeatherControl.Controls.Add(this.cbWeatherUsage);
            this.gbWeatherControl.Controls.Add(this.lblWeatherType);
            this.gbWeatherControl.Controls.Add(this.cbWeatherType);
            this.gbWeatherControl.Location = new System.Drawing.Point(12, 12);
            this.gbWeatherControl.Name = "gbWeatherControl";
            this.gbWeatherControl.Size = new System.Drawing.Size(299, 73);
            this.gbWeatherControl.TabIndex = 0;
            this.gbWeatherControl.TabStop = false;
            this.gbWeatherControl.Text = "Weather Control";
            // 
            // lblWeatherUsage
            // 
            this.lblWeatherUsage.AutoSize = true;
            this.lblWeatherUsage.Location = new System.Drawing.Point(6, 49);
            this.lblWeatherUsage.Name = "lblWeatherUsage";
            this.lblWeatherUsage.Size = new System.Drawing.Size(38, 13);
            this.lblWeatherUsage.TabIndex = 41;
            this.lblWeatherUsage.Text = "Usage";
            // 
            // btnWeatherAdvanced
            // 
            this.btnWeatherAdvanced.Location = new System.Drawing.Point(215, 19);
            this.btnWeatherAdvanced.Name = "btnWeatherAdvanced";
            this.btnWeatherAdvanced.Size = new System.Drawing.Size(78, 23);
            this.btnWeatherAdvanced.TabIndex = 11;
            this.btnWeatherAdvanced.Text = "Advanced";
            this.btnWeatherAdvanced.UseVisualStyleBackColor = true;
            // 
            // cbWeatherUsage
            // 
            this.cbWeatherUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWeatherUsage.FormattingEnabled = true;
            this.cbWeatherUsage.Items.AddRange(new object[] {
            "Global",
            "Relative",
            "Absolute"});
            this.cbWeatherUsage.Location = new System.Drawing.Point(88, 46);
            this.cbWeatherUsage.Name = "cbWeatherUsage";
            this.cbWeatherUsage.Size = new System.Drawing.Size(121, 21);
            this.cbWeatherUsage.TabIndex = 40;
            // 
            // lblWeatherType
            // 
            this.lblWeatherType.AutoSize = true;
            this.lblWeatherType.Location = new System.Drawing.Point(6, 22);
            this.lblWeatherType.Name = "lblWeatherType";
            this.lblWeatherType.Size = new System.Drawing.Size(31, 13);
            this.lblWeatherType.TabIndex = 6;
            this.lblWeatherType.Text = "Type";
            // 
            // cbWeatherType
            // 
            this.cbWeatherType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWeatherType.FormattingEnabled = true;
            this.cbWeatherType.Items.AddRange(new object[] {
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
            this.cbWeatherType.Location = new System.Drawing.Point(88, 19);
            this.cbWeatherType.Name = "cbWeatherType";
            this.cbWeatherType.Size = new System.Drawing.Size(121, 21);
            this.cbWeatherType.TabIndex = 5;
            this.cbWeatherType.SelectedIndexChanged += new System.EventHandler(this.cbWeatherType_SelectedIndexChanged);
            // 
            // btnSkyAdvanced
            // 
            this.btnSkyAdvanced.Location = new System.Drawing.Point(215, 19);
            this.btnSkyAdvanced.Name = "btnSkyAdvanced";
            this.btnSkyAdvanced.Size = new System.Drawing.Size(78, 23);
            this.btnSkyAdvanced.TabIndex = 12;
            this.btnSkyAdvanced.Text = "Advanced";
            this.btnSkyAdvanced.UseVisualStyleBackColor = true;
            // 
            // lblVisibilityType
            // 
            this.lblVisibilityType.AutoSize = true;
            this.lblVisibilityType.Location = new System.Drawing.Point(6, 22);
            this.lblVisibilityType.Name = "lblVisibilityType";
            this.lblVisibilityType.Size = new System.Drawing.Size(31, 13);
            this.lblVisibilityType.TabIndex = 8;
            this.lblVisibilityType.Text = "Type";
            // 
            // cbVisibilityType
            // 
            this.cbVisibilityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVisibilityType.FormattingEnabled = true;
            this.cbVisibilityType.Items.AddRange(new object[] {
            "Regular",
            "Fog of war"});
            this.cbVisibilityType.Location = new System.Drawing.Point(88, 19);
            this.cbVisibilityType.Name = "cbVisibilityType";
            this.cbVisibilityType.Size = new System.Drawing.Size(121, 21);
            this.cbVisibilityType.TabIndex = 7;
            this.cbVisibilityType.SelectedIndexChanged += new System.EventHandler(this.cbVisibilityType_SelectedIndexChanged);
            // 
            // lblSkyType
            // 
            this.lblSkyType.AutoSize = true;
            this.lblSkyType.Location = new System.Drawing.Point(6, 22);
            this.lblSkyType.Name = "lblSkyType";
            this.lblSkyType.Size = new System.Drawing.Size(31, 13);
            this.lblSkyType.TabIndex = 2;
            this.lblSkyType.Text = "Type";
            // 
            // cbSkyType
            // 
            this.cbSkyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSkyType.FormattingEnabled = true;
            this.cbSkyType.Items.AddRange(new object[] {
            "Disabled",
            "Regular",
            "Arctic",
            "Warm/orange"});
            this.cbSkyType.Location = new System.Drawing.Point(88, 19);
            this.cbSkyType.Name = "cbSkyType";
            this.cbSkyType.Size = new System.Drawing.Size(121, 21);
            this.cbSkyType.TabIndex = 1;
            this.cbSkyType.SelectedIndexChanged += new System.EventHandler(this.cbSkyType_SelectedIndexChanged);
            // 
            // gbTimePeriods
            // 
            this.gbTimePeriods.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
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
            this.gbTimePeriods.Location = new System.Drawing.Point(317, 12);
            this.gbTimePeriods.Name = "gbTimePeriods";
            this.gbTimePeriods.Size = new System.Drawing.Size(212, 441);
            this.gbTimePeriods.TabIndex = 2;
            this.gbTimePeriods.TabStop = false;
            this.gbTimePeriods.Text = "Time periods";
            // 
            // txtTimePeriodPassiveSkill
            // 
            this.txtTimePeriodPassiveSkill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTimePeriodPassiveSkill.Controls.Add(this.btnTimePeriodPassiveSkill);
            this.txtTimePeriodPassiveSkill.Location = new System.Drawing.Point(76, 386);
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
            this.lblTimePeriodPassiveSkill.Location = new System.Drawing.Point(6, 389);
            this.lblTimePeriodPassiveSkill.Name = "lblTimePeriodPassiveSkill";
            this.lblTimePeriodPassiveSkill.Size = new System.Drawing.Size(64, 13);
            this.lblTimePeriodPassiveSkill.TabIndex = 10;
            this.lblTimePeriodPassiveSkill.Text = "Passive skill";
            // 
            // lblTimePeriodName
            // 
            this.lblTimePeriodName.AutoSize = true;
            this.lblTimePeriodName.Location = new System.Drawing.Point(6, 311);
            this.lblTimePeriodName.Name = "lblTimePeriodName";
            this.lblTimePeriodName.Size = new System.Drawing.Size(35, 13);
            this.lblTimePeriodName.TabIndex = 8;
            this.lblTimePeriodName.Text = "Name";
            // 
            // txtTimePeriodName
            // 
            this.txtTimePeriodName.Location = new System.Drawing.Point(47, 308);
            this.txtTimePeriodName.Name = "txtTimePeriodName";
            this.txtTimePeriodName.Size = new System.Drawing.Size(159, 20);
            this.txtTimePeriodName.TabIndex = 7;
            this.txtTimePeriodName.TextChanged += new System.EventHandler(this.txtTimePeriodName_TextChanged);
            // 
            // txtTimePeriodDayStart
            // 
            this.txtTimePeriodDayStart.Location = new System.Drawing.Point(149, 360);
            this.txtTimePeriodDayStart.Name = "txtTimePeriodDayStart";
            this.txtTimePeriodDayStart.Size = new System.Drawing.Size(57, 20);
            this.txtTimePeriodDayStart.TabIndex = 6;
            this.txtTimePeriodDayStart.ValueChanged += new System.EventHandler(this.txtTimePeriodDayStart_ValueChanged);
            // 
            // lblTimePeriodDayStart
            // 
            this.lblTimePeriodDayStart.AutoSize = true;
            this.lblTimePeriodDayStart.Location = new System.Drawing.Point(6, 362);
            this.lblTimePeriodDayStart.Name = "lblTimePeriodDayStart";
            this.lblTimePeriodDayStart.Size = new System.Drawing.Size(49, 13);
            this.lblTimePeriodDayStart.TabIndex = 5;
            this.lblTimePeriodDayStart.Text = "Day start";
            // 
            // btnRemoveTimePeriod
            // 
            this.btnRemoveTimePeriod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveTimePeriod.Location = new System.Drawing.Point(131, 412);
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
            this.btnAddTimePeriod.Location = new System.Drawing.Point(44, 412);
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
            this.lsTimePeriod.Size = new System.Drawing.Size(200, 277);
            this.lsTimePeriod.TabIndex = 2;
            this.lsTimePeriod.SelectedIndexChanged += new System.EventHandler(this.lsTimePeriod_SelectedIndexChanged);
            // 
            // txtTimePeriodTimeStart
            // 
            this.txtTimePeriodTimeStart.Location = new System.Drawing.Point(149, 334);
            this.txtTimePeriodTimeStart.Name = "txtTimePeriodTimeStart";
            this.txtTimePeriodTimeStart.Size = new System.Drawing.Size(57, 20);
            this.txtTimePeriodTimeStart.TabIndex = 1;
            this.txtTimePeriodTimeStart.ValueChanged += new System.EventHandler(this.txtTimePeriodTimeStart_ValueChanged);
            // 
            // lblTimePeriodTimeStart
            // 
            this.lblTimePeriodTimeStart.AutoSize = true;
            this.lblTimePeriodTimeStart.Location = new System.Drawing.Point(6, 336);
            this.lblTimePeriodTimeStart.Name = "lblTimePeriodTimeStart";
            this.lblTimePeriodTimeStart.Size = new System.Drawing.Size(53, 13);
            this.lblTimePeriodTimeStart.TabIndex = 0;
            this.lblTimePeriodTimeStart.Text = "Time start";
            // 
            // gbWind
            // 
            this.gbWind.Controls.Add(this.btnWindAdvanced);
            this.gbWind.Controls.Add(this.lblWindUsage);
            this.gbWind.Controls.Add(this.cbWindUsage);
            this.gbWind.Controls.Add(this.lblWindDirection);
            this.gbWind.Controls.Add(this.txtWindDirection);
            this.gbWind.Controls.Add(this.lblWindSpeed);
            this.gbWind.Controls.Add(this.txtWindSpeed);
            this.gbWind.Location = new System.Drawing.Point(12, 249);
            this.gbWind.Name = "gbWind";
            this.gbWind.Size = new System.Drawing.Size(299, 100);
            this.gbWind.TabIndex = 9;
            this.gbWind.TabStop = false;
            this.gbWind.Text = "Wind";
            // 
            // btnWindAdvanced
            // 
            this.btnWindAdvanced.Location = new System.Drawing.Point(215, 17);
            this.btnWindAdvanced.Name = "btnWindAdvanced";
            this.btnWindAdvanced.Size = new System.Drawing.Size(78, 23);
            this.btnWindAdvanced.TabIndex = 37;
            this.btnWindAdvanced.Text = "Advanced";
            this.btnWindAdvanced.UseVisualStyleBackColor = true;
            // 
            // lblWindUsage
            // 
            this.lblWindUsage.AutoSize = true;
            this.lblWindUsage.Location = new System.Drawing.Point(6, 22);
            this.lblWindUsage.Name = "lblWindUsage";
            this.lblWindUsage.Size = new System.Drawing.Size(38, 13);
            this.lblWindUsage.TabIndex = 36;
            this.lblWindUsage.Text = "Usage";
            // 
            // cbWindUsage
            // 
            this.cbWindUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbWindUsage.FormattingEnabled = true;
            this.cbWindUsage.Items.AddRange(new object[] {
            "Global",
            "Relative",
            "Absolute"});
            this.cbWindUsage.Location = new System.Drawing.Point(88, 19);
            this.cbWindUsage.Name = "cbWindUsage";
            this.cbWindUsage.Size = new System.Drawing.Size(121, 21);
            this.cbWindUsage.TabIndex = 35;
            // 
            // lblWindDirection
            // 
            this.lblWindDirection.AutoSize = true;
            this.lblWindDirection.Location = new System.Drawing.Point(6, 74);
            this.lblWindDirection.Name = "lblWindDirection";
            this.lblWindDirection.Size = new System.Drawing.Size(93, 13);
            this.lblWindDirection.TabIndex = 8;
            this.lblWindDirection.Text = "Direction (Degree)";
            // 
            // txtWindDirection
            // 
            this.txtWindDirection.Location = new System.Drawing.Point(153, 72);
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
            this.lblWindSpeed.Location = new System.Drawing.Point(6, 48);
            this.lblWindSpeed.Name = "lblWindSpeed";
            this.lblWindSpeed.Size = new System.Drawing.Size(38, 13);
            this.lblWindSpeed.TabIndex = 6;
            this.lblWindSpeed.Text = "Speed";
            // 
            // txtWindSpeed
            // 
            this.txtWindSpeed.Location = new System.Drawing.Point(153, 46);
            this.txtWindSpeed.Name = "txtWindSpeed";
            this.txtWindSpeed.Size = new System.Drawing.Size(56, 20);
            this.txtWindSpeed.TabIndex = 5;
            this.txtWindSpeed.ValueChanged += new System.EventHandler(this.txtWindSpeed_ValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.lblTransitionCrossfadeLength);
            this.groupBox1.Controls.Add(this.txtTransitionCrossfadeLength);
            this.groupBox1.Controls.Add(this.lblTransitionEndLength);
            this.groupBox1.Controls.Add(this.txtTransitionEndLength);
            this.groupBox1.Controls.Add(this.lblTransitionStartLength);
            this.groupBox1.Controls.Add(this.txtTransitionStartLength);
            this.groupBox1.Location = new System.Drawing.Point(12, 355);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(299, 98);
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
            this.txtTransitionCrossfadeLength.Location = new System.Drawing.Point(153, 67);
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
            this.txtTransitionEndLength.Location = new System.Drawing.Point(153, 41);
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
            this.txtTransitionStartLength.Location = new System.Drawing.Point(153, 15);
            this.txtTransitionStartLength.Name = "txtTransitionStartLength";
            this.txtTransitionStartLength.Size = new System.Drawing.Size(56, 20);
            this.txtTransitionStartLength.TabIndex = 5;
            this.txtTransitionStartLength.ValueChanged += new System.EventHandler(this.txtTransitionStartLength_ValueChanged);
            // 
            // gbVisibilityControl
            // 
            this.gbVisibilityControl.Controls.Add(this.lblVisibilityUsage);
            this.gbVisibilityControl.Controls.Add(this.btnVisibilityAdvanced);
            this.gbVisibilityControl.Controls.Add(this.cbVisibilityUsage);
            this.gbVisibilityControl.Controls.Add(this.cbVisibilityType);
            this.gbVisibilityControl.Controls.Add(this.lblVisibilityType);
            this.gbVisibilityControl.Location = new System.Drawing.Point(12, 170);
            this.gbVisibilityControl.Name = "gbVisibilityControl";
            this.gbVisibilityControl.Size = new System.Drawing.Size(299, 73);
            this.gbVisibilityControl.TabIndex = 11;
            this.gbVisibilityControl.TabStop = false;
            this.gbVisibilityControl.Text = "Visibility Control";
            // 
            // lblVisibilityUsage
            // 
            this.lblVisibilityUsage.AutoSize = true;
            this.lblVisibilityUsage.Location = new System.Drawing.Point(6, 49);
            this.lblVisibilityUsage.Name = "lblVisibilityUsage";
            this.lblVisibilityUsage.Size = new System.Drawing.Size(38, 13);
            this.lblVisibilityUsage.TabIndex = 39;
            this.lblVisibilityUsage.Text = "Usage";
            // 
            // btnVisibilityAdvanced
            // 
            this.btnVisibilityAdvanced.Location = new System.Drawing.Point(215, 17);
            this.btnVisibilityAdvanced.Name = "btnVisibilityAdvanced";
            this.btnVisibilityAdvanced.Size = new System.Drawing.Size(78, 23);
            this.btnVisibilityAdvanced.TabIndex = 38;
            this.btnVisibilityAdvanced.Text = "Advanced";
            this.btnVisibilityAdvanced.UseVisualStyleBackColor = true;
            // 
            // cbVisibilityUsage
            // 
            this.cbVisibilityUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVisibilityUsage.FormattingEnabled = true;
            this.cbVisibilityUsage.Items.AddRange(new object[] {
            "Global",
            "Relative",
            "Absolute"});
            this.cbVisibilityUsage.Location = new System.Drawing.Point(88, 46);
            this.cbVisibilityUsage.Name = "cbVisibilityUsage";
            this.cbVisibilityUsage.Size = new System.Drawing.Size(121, 21);
            this.cbVisibilityUsage.TabIndex = 38;
            // 
            // gbSkyControl
            // 
            this.gbSkyControl.Controls.Add(this.btnSkyAdvanced);
            this.gbSkyControl.Controls.Add(this.lblSkyUsage);
            this.gbSkyControl.Controls.Add(this.cbSkyUsage);
            this.gbSkyControl.Controls.Add(this.cbSkyType);
            this.gbSkyControl.Controls.Add(this.lblSkyType);
            this.gbSkyControl.Location = new System.Drawing.Point(12, 91);
            this.gbSkyControl.Name = "gbSkyControl";
            this.gbSkyControl.Size = new System.Drawing.Size(299, 73);
            this.gbSkyControl.TabIndex = 40;
            this.gbSkyControl.TabStop = false;
            this.gbSkyControl.Text = "Sky control";
            // 
            // lblSkyUsage
            // 
            this.lblSkyUsage.AutoSize = true;
            this.lblSkyUsage.Location = new System.Drawing.Point(6, 49);
            this.lblSkyUsage.Name = "lblSkyUsage";
            this.lblSkyUsage.Size = new System.Drawing.Size(38, 13);
            this.lblSkyUsage.TabIndex = 39;
            this.lblSkyUsage.Text = "Usage";
            // 
            // cbSkyUsage
            // 
            this.cbSkyUsage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSkyUsage.FormattingEnabled = true;
            this.cbSkyUsage.Items.AddRange(new object[] {
            "Global",
            "Relative",
            "Absolute"});
            this.cbSkyUsage.Location = new System.Drawing.Point(88, 46);
            this.cbSkyUsage.Name = "cbSkyUsage";
            this.cbSkyUsage.Size = new System.Drawing.Size(121, 21);
            this.cbSkyUsage.TabIndex = 38;
            // 
            // ZoneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 465);
            this.Controls.Add(this.gbSkyControl);
            this.Controls.Add(this.gbVisibilityControl);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gbWind);
            this.Controls.Add(this.gbTimePeriods);
            this.Controls.Add(this.gbWeatherControl);
            this.Name = "ZoneEditor";
            this.Text = "Zone properties";
            this.gbWeatherControl.ResumeLayout(false);
            this.gbWeatherControl.PerformLayout();
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
            this.gbVisibilityControl.ResumeLayout(false);
            this.gbVisibilityControl.PerformLayout();
            this.gbSkyControl.ResumeLayout(false);
            this.gbSkyControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbWeatherControl;
        private System.Windows.Forms.Label lblWeatherType;
        private System.Windows.Forms.ComboBox cbWeatherType;
        private System.Windows.Forms.Label lblSkyType;
        private System.Windows.Forms.ComboBox cbSkyType;
        private System.Windows.Forms.Label lblVisibilityType;
        private System.Windows.Forms.ComboBox cbVisibilityType;
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
        private System.Windows.Forms.Button btnSkyAdvanced;
        private System.Windows.Forms.Button btnWeatherAdvanced;
        private System.Windows.Forms.Button btnWindAdvanced;
        private System.Windows.Forms.Label lblWindUsage;
        private System.Windows.Forms.ComboBox cbWindUsage;
        private System.Windows.Forms.Label lblWeatherUsage;
        private System.Windows.Forms.ComboBox cbWeatherUsage;
        private System.Windows.Forms.GroupBox gbVisibilityControl;
        private System.Windows.Forms.Label lblVisibilityUsage;
        private System.Windows.Forms.Button btnVisibilityAdvanced;
        private System.Windows.Forms.ComboBox cbVisibilityUsage;
        private System.Windows.Forms.GroupBox gbSkyControl;
        private System.Windows.Forms.Label lblSkyUsage;
        private System.Windows.Forms.ComboBox cbSkyUsage;
    }
}