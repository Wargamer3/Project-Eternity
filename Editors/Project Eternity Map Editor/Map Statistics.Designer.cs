namespace ProjectEternity.Editors.MapEditor
{
    partial class MapStatistics
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
            this.lblMapWidth = new System.Windows.Forms.Label();
            this.lblMapHeight = new System.Windows.Forms.Label();
            this.lblTileHeight = new System.Windows.Forms.Label();
            this.lblTileWidth = new System.Windows.Forms.Label();
            this.txtMapName = new System.Windows.Forms.TextBox();
            this.lblMapName = new System.Windows.Forms.Label();
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbCameraStartPosition = new System.Windows.Forms.GroupBox();
            this.lblCameraY = new System.Windows.Forms.Label();
            this.txtCameraStartPositionY = new System.Windows.Forms.NumericUpDown();
            this.lblCameraX = new System.Windows.Forms.Label();
            this.txtCameraStartPositionX = new System.Windows.Forms.NumericUpDown();
            this.btnSetBackgrounds = new System.Windows.Forms.Button();
            this.btnSetForegrounds = new System.Windows.Forms.Button();
            this.gbDescription = new System.Windows.Forms.GroupBox();
            this.btnOpenTranslationFile = new System.Windows.Forms.Button();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.gbPlayers = new System.Windows.Forms.GroupBox();
            this.lblPlayersMax = new System.Windows.Forms.Label();
            this.txtPlayersMax = new System.Windows.Forms.NumericUpDown();
            this.lblPlayersMin = new System.Windows.Forms.Label();
            this.txtPlayersMin = new System.Windows.Forms.NumericUpDown();
            this.txtTileWidth = new System.Windows.Forms.NumericUpDown();
            this.txtTileHeight = new System.Windows.Forms.NumericUpDown();
            this.txtMapWidth = new System.Windows.Forms.NumericUpDown();
            this.txtMapHeight = new System.Windows.Forms.NumericUpDown();
            this.cbCameraType = new System.Windows.Forms.ComboBox();
            this.lblCameraType = new System.Windows.Forms.Label();
            this.gbTimeLimits = new System.Windows.Forms.GroupBox();
            this.rbStopTime = new System.Windows.Forms.RadioButton();
            this.rbLoopLastDay = new System.Windows.Forms.RadioButton();
            this.rbLoopFirstDay = new System.Windows.Forms.RadioButton();
            this.lblHoursInDay = new System.Windows.Forms.Label();
            this.txtHoursInDay = new System.Windows.Forms.NumericUpDown();
            this.lblTimeStart = new System.Windows.Forms.Label();
            this.txtTimeStart = new System.Windows.Forms.NumericUpDown();
            this.rbUseTurns = new System.Windows.Forms.RadioButton();
            this.rbUseRealTime = new System.Windows.Forms.RadioButton();
            this.lblTimeMultiplier = new System.Windows.Forms.Label();
            this.txtlblTimeMultiplier = new System.Windows.Forms.NumericUpDown();
            this.gbTimeUsage = new System.Windows.Forms.GroupBox();
            this.gbMandatoryMutators = new System.Windows.Forms.GroupBox();
            this.dgvMandatoryMutators = new System.Windows.Forms.DataGridView();
            this.clMutatorName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbCameraStartPosition.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionX)).BeginInit();
            this.gbDescription.SuspendLayout();
            this.gbPlayers.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapHeight)).BeginInit();
            this.gbTimeLimits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoursInDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtlblTimeMultiplier)).BeginInit();
            this.gbTimeUsage.SuspendLayout();
            this.gbMandatoryMutators.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMandatoryMutators)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMapWidth
            // 
            this.lblMapWidth.AutoSize = true;
            this.lblMapWidth.Location = new System.Drawing.Point(12, 67);
            this.lblMapWidth.Name = "lblMapWidth";
            this.lblMapWidth.Size = new System.Drawing.Size(97, 13);
            this.lblMapWidth.TabIndex = 4;
            this.lblMapWidth.Text = "Map Width (in tiles)";
            // 
            // lblMapHeight
            // 
            this.lblMapHeight.AutoSize = true;
            this.lblMapHeight.Location = new System.Drawing.Point(115, 67);
            this.lblMapHeight.Name = "lblMapHeight";
            this.lblMapHeight.Size = new System.Drawing.Size(100, 13);
            this.lblMapHeight.TabIndex = 5;
            this.lblMapHeight.Text = "Map Height (in tiles)";
            // 
            // lblTileHeight
            // 
            this.lblTileHeight.AutoSize = true;
            this.lblTileHeight.Location = new System.Drawing.Point(80, 106);
            this.lblTileHeight.Name = "lblTileHeight";
            this.lblTileHeight.Size = new System.Drawing.Size(58, 13);
            this.lblTileHeight.TabIndex = 6;
            this.lblTileHeight.Text = "Tile Height";
            // 
            // lblTileWidth
            // 
            this.lblTileWidth.AutoSize = true;
            this.lblTileWidth.Location = new System.Drawing.Point(12, 106);
            this.lblTileWidth.Name = "lblTileWidth";
            this.lblTileWidth.Size = new System.Drawing.Size(55, 13);
            this.lblTileWidth.TabIndex = 7;
            this.lblTileWidth.Text = "Tile Width";
            // 
            // txtMapName
            // 
            this.txtMapName.Location = new System.Drawing.Point(12, 34);
            this.txtMapName.Name = "txtMapName";
            this.txtMapName.ReadOnly = true;
            this.txtMapName.Size = new System.Drawing.Size(259, 20);
            this.txtMapName.TabIndex = 8;
            this.txtMapName.Text = "New map";
            // 
            // lblMapName
            // 
            this.lblMapName.AutoSize = true;
            this.lblMapName.Location = new System.Drawing.Point(12, 18);
            this.lblMapName.Name = "lblMapName";
            this.lblMapName.Size = new System.Drawing.Size(57, 13);
            this.lblMapName.TabIndex = 9;
            this.lblMapName.Text = "Map name";
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAccept.Location = new System.Drawing.Point(545, 297);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 10;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClose.Location = new System.Drawing.Point(626, 297);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 11;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // gbCameraStartPosition
            // 
            this.gbCameraStartPosition.Controls.Add(this.lblCameraY);
            this.gbCameraStartPosition.Controls.Add(this.txtCameraStartPositionY);
            this.gbCameraStartPosition.Controls.Add(this.lblCameraX);
            this.gbCameraStartPosition.Controls.Add(this.txtCameraStartPositionX);
            this.gbCameraStartPosition.Location = new System.Drawing.Point(12, 151);
            this.gbCameraStartPosition.Name = "gbCameraStartPosition";
            this.gbCameraStartPosition.Size = new System.Drawing.Size(126, 73);
            this.gbCameraStartPosition.TabIndex = 12;
            this.gbCameraStartPosition.TabStop = false;
            this.gbCameraStartPosition.Text = "Camera Start Position";
            // 
            // lblCameraY
            // 
            this.lblCameraY.AutoSize = true;
            this.lblCameraY.Location = new System.Drawing.Point(6, 47);
            this.lblCameraY.Name = "lblCameraY";
            this.lblCameraY.Size = new System.Drawing.Size(17, 13);
            this.lblCameraY.TabIndex = 3;
            this.lblCameraY.Text = "Y:";
            // 
            // txtCameraStartPositionY
            // 
            this.txtCameraStartPositionY.Location = new System.Drawing.Point(37, 45);
            this.txtCameraStartPositionY.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtCameraStartPositionY.Name = "txtCameraStartPositionY";
            this.txtCameraStartPositionY.Size = new System.Drawing.Size(83, 20);
            this.txtCameraStartPositionY.TabIndex = 2;
            // 
            // lblCameraX
            // 
            this.lblCameraX.AutoSize = true;
            this.lblCameraX.Location = new System.Drawing.Point(6, 21);
            this.lblCameraX.Name = "lblCameraX";
            this.lblCameraX.Size = new System.Drawing.Size(17, 13);
            this.lblCameraX.TabIndex = 1;
            this.lblCameraX.Text = "X:";
            // 
            // txtCameraStartPositionX
            // 
            this.txtCameraStartPositionX.Location = new System.Drawing.Point(37, 16);
            this.txtCameraStartPositionX.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtCameraStartPositionX.Name = "txtCameraStartPositionX";
            this.txtCameraStartPositionX.Size = new System.Drawing.Size(83, 20);
            this.txtCameraStartPositionX.TabIndex = 0;
            // 
            // btnSetBackgrounds
            // 
            this.btnSetBackgrounds.Location = new System.Drawing.Point(154, 172);
            this.btnSetBackgrounds.Name = "btnSetBackgrounds";
            this.btnSetBackgrounds.Size = new System.Drawing.Size(117, 23);
            this.btnSetBackgrounds.TabIndex = 13;
            this.btnSetBackgrounds.Text = "Set Backgrounds";
            this.btnSetBackgrounds.UseVisualStyleBackColor = true;
            this.btnSetBackgrounds.Click += new System.EventHandler(this.btnSetBackgrounds_Click);
            // 
            // btnSetForegrounds
            // 
            this.btnSetForegrounds.Location = new System.Drawing.Point(154, 201);
            this.btnSetForegrounds.Name = "btnSetForegrounds";
            this.btnSetForegrounds.Size = new System.Drawing.Size(117, 23);
            this.btnSetForegrounds.TabIndex = 14;
            this.btnSetForegrounds.Text = "Set Foregrounds";
            this.btnSetForegrounds.UseVisualStyleBackColor = true;
            this.btnSetForegrounds.Click += new System.EventHandler(this.btnSetForegrounds_Click);
            // 
            // gbDescription
            // 
            this.gbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbDescription.Controls.Add(this.btnOpenTranslationFile);
            this.gbDescription.Controls.Add(this.txtDescription);
            this.gbDescription.Location = new System.Drawing.Point(478, 12);
            this.gbDescription.Name = "gbDescription";
            this.gbDescription.Size = new System.Drawing.Size(223, 279);
            this.gbDescription.TabIndex = 15;
            this.gbDescription.TabStop = false;
            this.gbDescription.Text = "Description";
            // 
            // btnOpenTranslationFile
            // 
            this.btnOpenTranslationFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenTranslationFile.Location = new System.Drawing.Point(72, 250);
            this.btnOpenTranslationFile.Name = "btnOpenTranslationFile";
            this.btnOpenTranslationFile.Size = new System.Drawing.Size(145, 23);
            this.btnOpenTranslationFile.TabIndex = 11;
            this.btnOpenTranslationFile.Text = "Open Translation File";
            this.btnOpenTranslationFile.UseVisualStyleBackColor = true;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(6, 19);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(211, 228);
            this.txtDescription.TabIndex = 0;
            // 
            // gbPlayers
            // 
            this.gbPlayers.Controls.Add(this.lblPlayersMax);
            this.gbPlayers.Controls.Add(this.txtPlayersMax);
            this.gbPlayers.Controls.Add(this.lblPlayersMin);
            this.gbPlayers.Controls.Add(this.txtPlayersMin);
            this.gbPlayers.Location = new System.Drawing.Point(12, 230);
            this.gbPlayers.Name = "gbPlayers";
            this.gbPlayers.Size = new System.Drawing.Size(259, 47);
            this.gbPlayers.TabIndex = 16;
            this.gbPlayers.TabStop = false;
            this.gbPlayers.Text = "Players";
            // 
            // lblPlayersMax
            // 
            this.lblPlayersMax.AutoSize = true;
            this.lblPlayersMax.Location = new System.Drawing.Point(137, 21);
            this.lblPlayersMax.Name = "lblPlayersMax";
            this.lblPlayersMax.Size = new System.Drawing.Size(30, 13);
            this.lblPlayersMax.TabIndex = 5;
            this.lblPlayersMax.Text = "Max:";
            // 
            // txtPlayersMax
            // 
            this.txtPlayersMax.Location = new System.Drawing.Point(170, 19);
            this.txtPlayersMax.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtPlayersMax.Name = "txtPlayersMax";
            this.txtPlayersMax.Size = new System.Drawing.Size(83, 20);
            this.txtPlayersMax.TabIndex = 4;
            // 
            // lblPlayersMin
            // 
            this.lblPlayersMin.AutoSize = true;
            this.lblPlayersMin.Location = new System.Drawing.Point(6, 21);
            this.lblPlayersMin.Name = "lblPlayersMin";
            this.lblPlayersMin.Size = new System.Drawing.Size(27, 13);
            this.lblPlayersMin.TabIndex = 3;
            this.lblPlayersMin.Text = "Min:";
            // 
            // txtPlayersMin
            // 
            this.txtPlayersMin.Location = new System.Drawing.Point(39, 19);
            this.txtPlayersMin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtPlayersMin.Name = "txtPlayersMin";
            this.txtPlayersMin.Size = new System.Drawing.Size(83, 20);
            this.txtPlayersMin.TabIndex = 2;
            // 
            // txtTileWidth
            // 
            this.txtTileWidth.Location = new System.Drawing.Point(12, 122);
            this.txtTileWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtTileWidth.Name = "txtTileWidth";
            this.txtTileWidth.Size = new System.Drawing.Size(58, 20);
            this.txtTileWidth.TabIndex = 6;
            this.txtTileWidth.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // txtTileHeight
            // 
            this.txtTileHeight.Location = new System.Drawing.Point(80, 122);
            this.txtTileHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtTileHeight.Name = "txtTileHeight";
            this.txtTileHeight.Size = new System.Drawing.Size(58, 20);
            this.txtTileHeight.TabIndex = 17;
            this.txtTileHeight.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            // 
            // txtMapWidth
            // 
            this.txtMapWidth.Location = new System.Drawing.Point(12, 83);
            this.txtMapWidth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMapWidth.Name = "txtMapWidth";
            this.txtMapWidth.Size = new System.Drawing.Size(83, 20);
            this.txtMapWidth.TabIndex = 18;
            this.txtMapWidth.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // txtMapHeight
            // 
            this.txtMapHeight.Location = new System.Drawing.Point(115, 84);
            this.txtMapHeight.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.txtMapHeight.Name = "txtMapHeight";
            this.txtMapHeight.Size = new System.Drawing.Size(83, 20);
            this.txtMapHeight.TabIndex = 19;
            this.txtMapHeight.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // cbCameraType
            // 
            this.cbCameraType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCameraType.FormattingEnabled = true;
            this.cbCameraType.Items.AddRange(new object[] {
            "2D",
            "3D"});
            this.cbCameraType.Location = new System.Drawing.Point(144, 121);
            this.cbCameraType.Name = "cbCameraType";
            this.cbCameraType.Size = new System.Drawing.Size(71, 21);
            this.cbCameraType.TabIndex = 12;
            // 
            // lblCameraType
            // 
            this.lblCameraType.AutoSize = true;
            this.lblCameraType.Location = new System.Drawing.Point(144, 107);
            this.lblCameraType.Name = "lblCameraType";
            this.lblCameraType.Size = new System.Drawing.Size(70, 13);
            this.lblCameraType.TabIndex = 20;
            this.lblCameraType.Text = "Camera Type";
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
            this.gbTimeLimits.Location = new System.Drawing.Point(277, 12);
            this.gbTimeLimits.Name = "gbTimeLimits";
            this.gbTimeLimits.Size = new System.Drawing.Size(195, 139);
            this.gbTimeLimits.TabIndex = 21;
            this.gbTimeLimits.TabStop = false;
            this.gbTimeLimits.Text = "Time limits";
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
            // rbUseTurns
            // 
            this.rbUseTurns.AutoSize = true;
            this.rbUseTurns.Location = new System.Drawing.Point(6, 45);
            this.rbUseTurns.Name = "rbUseTurns";
            this.rbUseTurns.Size = new System.Drawing.Size(70, 17);
            this.rbUseTurns.TabIndex = 14;
            this.rbUseTurns.TabStop = true;
            this.rbUseTurns.Text = "Use turns";
            this.rbUseTurns.UseVisualStyleBackColor = true;
            // 
            // rbUseRealTime
            // 
            this.rbUseRealTime.AutoSize = true;
            this.rbUseRealTime.Location = new System.Drawing.Point(103, 45);
            this.rbUseRealTime.Name = "rbUseRealTime";
            this.rbUseRealTime.Size = new System.Drawing.Size(86, 17);
            this.rbUseRealTime.TabIndex = 13;
            this.rbUseRealTime.TabStop = true;
            this.rbUseRealTime.Text = "Use real time";
            this.rbUseRealTime.UseVisualStyleBackColor = true;
            // 
            // lblTimeMultiplier
            // 
            this.lblTimeMultiplier.AutoSize = true;
            this.lblTimeMultiplier.Location = new System.Drawing.Point(6, 21);
            this.lblTimeMultiplier.Name = "lblTimeMultiplier";
            this.lblTimeMultiplier.Size = new System.Drawing.Size(73, 13);
            this.lblTimeMultiplier.TabIndex = 4;
            this.lblTimeMultiplier.Text = "Time multiplier";
            // 
            // txtlblTimeMultiplier
            // 
            this.txtlblTimeMultiplier.Location = new System.Drawing.Point(133, 19);
            this.txtlblTimeMultiplier.Name = "txtlblTimeMultiplier";
            this.txtlblTimeMultiplier.Size = new System.Drawing.Size(56, 20);
            this.txtlblTimeMultiplier.TabIndex = 3;
            // 
            // gbTimeUsage
            // 
            this.gbTimeUsage.Controls.Add(this.rbUseTurns);
            this.gbTimeUsage.Controls.Add(this.rbUseRealTime);
            this.gbTimeUsage.Controls.Add(this.txtlblTimeMultiplier);
            this.gbTimeUsage.Controls.Add(this.lblTimeMultiplier);
            this.gbTimeUsage.Location = new System.Drawing.Point(277, 157);
            this.gbTimeUsage.Name = "gbTimeUsage";
            this.gbTimeUsage.Size = new System.Drawing.Size(195, 69);
            this.gbTimeUsage.TabIndex = 22;
            this.gbTimeUsage.TabStop = false;
            this.gbTimeUsage.Text = "Time usage";
            // 
            // gbMandatoryMutators
            // 
            this.gbMandatoryMutators.Controls.Add(this.dgvMandatoryMutators);
            this.gbMandatoryMutators.Location = new System.Drawing.Point(277, 232);
            this.gbMandatoryMutators.Name = "gbMandatoryMutators";
            this.gbMandatoryMutators.Size = new System.Drawing.Size(200, 88);
            this.gbMandatoryMutators.TabIndex = 23;
            this.gbMandatoryMutators.TabStop = false;
            this.gbMandatoryMutators.Text = "Mandatory Mutators";
            // 
            // dgvMandatoryMutators
            // 
            this.dgvMandatoryMutators.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMandatoryMutators.ColumnHeadersVisible = false;
            this.dgvMandatoryMutators.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clMutatorName});
            this.dgvMandatoryMutators.Location = new System.Drawing.Point(6, 19);
            this.dgvMandatoryMutators.MultiSelect = false;
            this.dgvMandatoryMutators.Name = "dgvMandatoryMutators";
            this.dgvMandatoryMutators.RowHeadersVisible = false;
            this.dgvMandatoryMutators.Size = new System.Drawing.Size(188, 63);
            this.dgvMandatoryMutators.TabIndex = 1;
            // 
            // clMutatorName
            // 
            this.clMutatorName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clMutatorName.HeaderText = "Name";
            this.clMutatorName.Name = "clMutatorName";
            // 
            // MapStatistics
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 332);
            this.Controls.Add(this.gbMandatoryMutators);
            this.Controls.Add(this.gbTimeUsage);
            this.Controls.Add(this.gbTimeLimits);
            this.Controls.Add(this.lblCameraType);
            this.Controls.Add(this.cbCameraType);
            this.Controls.Add(this.txtMapHeight);
            this.Controls.Add(this.txtMapWidth);
            this.Controls.Add(this.txtTileHeight);
            this.Controls.Add(this.txtTileWidth);
            this.Controls.Add(this.gbPlayers);
            this.Controls.Add(this.gbDescription);
            this.Controls.Add(this.btnSetForegrounds);
            this.Controls.Add(this.btnSetBackgrounds);
            this.Controls.Add(this.gbCameraStartPosition);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnAccept);
            this.Controls.Add(this.lblMapName);
            this.Controls.Add(this.txtMapName);
            this.Controls.Add(this.lblTileWidth);
            this.Controls.Add(this.lblTileHeight);
            this.Controls.Add(this.lblMapHeight);
            this.Controls.Add(this.lblMapWidth);
            this.Name = "MapStatistics";
            this.Text = "Map properties";
            this.gbCameraStartPosition.ResumeLayout(false);
            this.gbCameraStartPosition.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCameraStartPositionX)).EndInit();
            this.gbDescription.ResumeLayout(false);
            this.gbDescription.PerformLayout();
            this.gbPlayers.ResumeLayout(false);
            this.gbPlayers.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPlayersMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTileHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtMapHeight)).EndInit();
            this.gbTimeLimits.ResumeLayout(false);
            this.gbTimeLimits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtHoursInDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtTimeStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtlblTimeMultiplier)).EndInit();
            this.gbTimeUsage.ResumeLayout(false);
            this.gbTimeUsage.PerformLayout();
            this.gbMandatoryMutators.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMandatoryMutators)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblMapWidth;
        private System.Windows.Forms.Label lblMapHeight;
        private System.Windows.Forms.Label lblTileHeight;
        private System.Windows.Forms.Label lblTileWidth;
        private System.Windows.Forms.TextBox txtMapName;
        private System.Windows.Forms.Label lblMapName;
        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.GroupBox gbCameraStartPosition;
        private System.Windows.Forms.Label lblCameraY;
        private System.Windows.Forms.Label lblCameraX;
        private System.Windows.Forms.Button btnSetBackgrounds;
        private System.Windows.Forms.Button btnSetForegrounds;
        private System.Windows.Forms.GroupBox gbDescription;
        private System.Windows.Forms.Button btnOpenTranslationFile;
        private System.Windows.Forms.GroupBox gbPlayers;
        private System.Windows.Forms.Label lblPlayersMax;
        private System.Windows.Forms.Label lblPlayersMin;
        public System.Windows.Forms.TextBox txtDescription;
        public System.Windows.Forms.NumericUpDown txtPlayersMax;
        public System.Windows.Forms.NumericUpDown txtPlayersMin;
        public System.Windows.Forms.NumericUpDown txtTileWidth;
        public System.Windows.Forms.NumericUpDown txtTileHeight;
        public System.Windows.Forms.NumericUpDown txtMapWidth;
        public System.Windows.Forms.NumericUpDown txtMapHeight;
        public System.Windows.Forms.NumericUpDown txtCameraStartPositionY;
        public System.Windows.Forms.NumericUpDown txtCameraStartPositionX;
        private System.Windows.Forms.Label lblCameraType;
        public System.Windows.Forms.ComboBox cbCameraType;
        private System.Windows.Forms.GroupBox gbTimeLimits;
        private System.Windows.Forms.Label lblHoursInDay;
        private System.Windows.Forms.Label lblTimeStart;
        private System.Windows.Forms.Label lblTimeMultiplier;
        private System.Windows.Forms.GroupBox gbTimeUsage;
        public System.Windows.Forms.RadioButton rbUseTurns;
        public System.Windows.Forms.RadioButton rbStopTime;
        public System.Windows.Forms.RadioButton rbUseRealTime;
        public System.Windows.Forms.RadioButton rbLoopLastDay;
        public System.Windows.Forms.RadioButton rbLoopFirstDay;
        public System.Windows.Forms.NumericUpDown txtHoursInDay;
        public System.Windows.Forms.NumericUpDown txtlblTimeMultiplier;
        public System.Windows.Forms.NumericUpDown txtTimeStart;
        private System.Windows.Forms.GroupBox gbMandatoryMutators;
        public System.Windows.Forms.DataGridView dgvMandatoryMutators;
        private System.Windows.Forms.DataGridViewTextBoxColumn clMutatorName;
    }
}