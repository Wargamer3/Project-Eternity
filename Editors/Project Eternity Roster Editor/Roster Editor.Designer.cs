namespace ProjectEternity.Editors.RosterEditor
{
    partial class ProjectEternityRosterEditor
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.lstUnits = new System.Windows.Forms.ListBox();
            this.lstCharacters = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnRemoveUnit = new System.Windows.Forms.Button();
            this.btnAddUnit = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnRemoveCharacter = new System.Windows.Forms.Button();
            this.btnAddCharacter = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblHIT = new System.Windows.Forms.Label();
            this.lblEVA = new System.Windows.Forms.Label();
            this.lblSKL = new System.Windows.Forms.Label();
            this.lblDEF = new System.Windows.Forms.Label();
            this.lblRNG = new System.Windows.Forms.Label();
            this.lblMEL = new System.Windows.Forms.Label();
            this.cbShareLevel = new System.Windows.Forms.CheckBox();
            this.cbShareEXP = new System.Windows.Forms.CheckBox();
            this.cbShareHIT = new System.Windows.Forms.CheckBox();
            this.cbShareEVA = new System.Windows.Forms.CheckBox();
            this.cbShareSKL = new System.Windows.Forms.CheckBox();
            this.cbShareDEF = new System.Windows.Forms.CheckBox();
            this.cbShareRNG = new System.Windows.Forms.CheckBox();
            this.cbShareMEL = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.lstCharactersToShareFrom = new System.Windows.Forms.ListBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.tsmSave = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.lblMaxMovement = new System.Windows.Forms.Label();
            this.lblMobility = new System.Windows.Forms.Label();
            this.lblArmor = new System.Windows.Forms.Label();
            this.lblRegenEN = new System.Windows.Forms.Label();
            this.lblMaxEN = new System.Windows.Forms.Label();
            this.lblMaxHP = new System.Windows.Forms.Label();
            this.cbShareMaxMovement = new System.Windows.Forms.CheckBox();
            this.cbShareMobility = new System.Windows.Forms.CheckBox();
            this.cbShareArmor = new System.Windows.Forms.CheckBox();
            this.cbShareRegenEN = new System.Windows.Forms.CheckBox();
            this.cbShareMaxEN = new System.Windows.Forms.CheckBox();
            this.cbShareMaxHP = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.lstUnitsToShareFrom = new System.Windows.Forms.ListBox();
            this.txtEventID = new System.Windows.Forms.TextBox();
            this.gbEventID = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.gbEventID.SuspendLayout();
            this.SuspendLayout();
            // 
            // lstUnits
            // 
            this.lstUnits.FormattingEnabled = true;
            this.lstUnits.Location = new System.Drawing.Point(6, 19);
            this.lstUnits.Name = "lstUnits";
            this.lstUnits.Size = new System.Drawing.Size(125, 173);
            this.lstUnits.TabIndex = 0;
            this.lstUnits.SelectedIndexChanged += new System.EventHandler(this.lstUnits_SelectedIndexChanged);
            // 
            // lstCharacters
            // 
            this.lstCharacters.FormattingEnabled = true;
            this.lstCharacters.Location = new System.Drawing.Point(6, 19);
            this.lstCharacters.Name = "lstCharacters";
            this.lstCharacters.Size = new System.Drawing.Size(122, 238);
            this.lstCharacters.TabIndex = 1;
            this.lstCharacters.SelectedIndexChanged += new System.EventHandler(this.lstCharacters_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnRemoveUnit);
            this.groupBox1.Controls.Add(this.btnAddUnit);
            this.groupBox1.Controls.Add(this.lstUnits);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 255);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Units";
            // 
            // btnRemoveUnit
            // 
            this.btnRemoveUnit.Location = new System.Drawing.Point(7, 226);
            this.btnRemoveUnit.Name = "btnRemoveUnit";
            this.btnRemoveUnit.Size = new System.Drawing.Size(125, 23);
            this.btnRemoveUnit.TabIndex = 5;
            this.btnRemoveUnit.Text = "Remove Unit";
            this.btnRemoveUnit.UseVisualStyleBackColor = true;
            this.btnRemoveUnit.Click += new System.EventHandler(this.btnRemoveUnit_Click);
            // 
            // btnAddUnit
            // 
            this.btnAddUnit.Location = new System.Drawing.Point(6, 197);
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(125, 23);
            this.btnAddUnit.TabIndex = 2;
            this.btnAddUnit.Text = "Add Unit";
            this.btnAddUnit.UseVisualStyleBackColor = true;
            this.btnAddUnit.Click += new System.EventHandler(this.btnAddUnit_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnRemoveCharacter);
            this.groupBox2.Controls.Add(this.btnAddCharacter);
            this.groupBox2.Controls.Add(this.lstCharacters);
            this.groupBox2.Location = new System.Drawing.Point(12, 288);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 322);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Characters";
            // 
            // btnRemoveCharacter
            // 
            this.btnRemoveCharacter.Location = new System.Drawing.Point(6, 289);
            this.btnRemoveCharacter.Name = "btnRemoveCharacter";
            this.btnRemoveCharacter.Size = new System.Drawing.Size(125, 23);
            this.btnRemoveCharacter.TabIndex = 4;
            this.btnRemoveCharacter.Text = "Remove Character";
            this.btnRemoveCharacter.UseVisualStyleBackColor = true;
            this.btnRemoveCharacter.Click += new System.EventHandler(this.btnRemoveCharacter_Click);
            // 
            // btnAddCharacter
            // 
            this.btnAddCharacter.Location = new System.Drawing.Point(6, 260);
            this.btnAddCharacter.Name = "btnAddCharacter";
            this.btnAddCharacter.Size = new System.Drawing.Size(125, 23);
            this.btnAddCharacter.TabIndex = 3;
            this.btnAddCharacter.Text = "Add Character";
            this.btnAddCharacter.UseVisualStyleBackColor = true;
            this.btnAddCharacter.Click += new System.EventHandler(this.btnAddCharacter_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblHIT);
            this.groupBox3.Controls.Add(this.lblEVA);
            this.groupBox3.Controls.Add(this.lblSKL);
            this.groupBox3.Controls.Add(this.lblDEF);
            this.groupBox3.Controls.Add(this.lblRNG);
            this.groupBox3.Controls.Add(this.lblMEL);
            this.groupBox3.Controls.Add(this.cbShareLevel);
            this.groupBox3.Controls.Add(this.cbShareEXP);
            this.groupBox3.Controls.Add(this.cbShareHIT);
            this.groupBox3.Controls.Add(this.cbShareEVA);
            this.groupBox3.Controls.Add(this.cbShareSKL);
            this.groupBox3.Controls.Add(this.cbShareDEF);
            this.groupBox3.Controls.Add(this.cbShareRNG);
            this.groupBox3.Controls.Add(this.cbShareMEL);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox8);
            this.groupBox3.Location = new System.Drawing.Point(298, 288);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(474, 322);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Details";
            // 
            // lblHIT
            // 
            this.lblHIT.AutoSize = true;
            this.lblHIT.Location = new System.Drawing.Point(320, 167);
            this.lblHIT.Name = "lblHIT";
            this.lblHIT.Size = new System.Drawing.Size(31, 13);
            this.lblHIT.TabIndex = 78;
            this.lblHIT.Text = "0000";
            // 
            // lblEVA
            // 
            this.lblEVA.AutoSize = true;
            this.lblEVA.Location = new System.Drawing.Point(320, 144);
            this.lblEVA.Name = "lblEVA";
            this.lblEVA.Size = new System.Drawing.Size(31, 13);
            this.lblEVA.TabIndex = 74;
            this.lblEVA.Text = "0000";
            // 
            // lblSKL
            // 
            this.lblSKL.AutoSize = true;
            this.lblSKL.Location = new System.Drawing.Point(320, 121);
            this.lblSKL.Name = "lblSKL";
            this.lblSKL.Size = new System.Drawing.Size(31, 13);
            this.lblSKL.TabIndex = 77;
            this.lblSKL.Text = "0000";
            // 
            // lblDEF
            // 
            this.lblDEF.AutoSize = true;
            this.lblDEF.Location = new System.Drawing.Point(320, 98);
            this.lblDEF.Name = "lblDEF";
            this.lblDEF.Size = new System.Drawing.Size(31, 13);
            this.lblDEF.TabIndex = 76;
            this.lblDEF.Text = "0000";
            // 
            // lblRNG
            // 
            this.lblRNG.AutoSize = true;
            this.lblRNG.Location = new System.Drawing.Point(320, 75);
            this.lblRNG.Name = "lblRNG";
            this.lblRNG.Size = new System.Drawing.Size(31, 13);
            this.lblRNG.TabIndex = 75;
            this.lblRNG.Text = "0000";
            // 
            // lblMEL
            // 
            this.lblMEL.AutoSize = true;
            this.lblMEL.Location = new System.Drawing.Point(320, 52);
            this.lblMEL.Name = "lblMEL";
            this.lblMEL.Size = new System.Drawing.Size(31, 13);
            this.lblMEL.TabIndex = 73;
            this.lblMEL.Text = "0000";
            // 
            // cbShareLevel
            // 
            this.cbShareLevel.AutoSize = true;
            this.cbShareLevel.Location = new System.Drawing.Point(282, 19);
            this.cbShareLevel.Name = "cbShareLevel";
            this.cbShareLevel.Size = new System.Drawing.Size(83, 17);
            this.cbShareLevel.TabIndex = 72;
            this.cbShareLevel.Text = "Share Level";
            this.cbShareLevel.UseVisualStyleBackColor = true;
            this.cbShareLevel.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareEXP
            // 
            this.cbShareEXP.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareEXP.AutoSize = true;
            this.cbShareEXP.Location = new System.Drawing.Point(390, 19);
            this.cbShareEXP.Name = "cbShareEXP";
            this.cbShareEXP.Size = new System.Drawing.Size(78, 17);
            this.cbShareEXP.TabIndex = 71;
            this.cbShareEXP.Text = "Share EXP";
            this.cbShareEXP.UseVisualStyleBackColor = true;
            this.cbShareEXP.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareHIT
            // 
            this.cbShareHIT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareHIT.AutoSize = true;
            this.cbShareHIT.Location = new System.Drawing.Point(414, 166);
            this.cbShareHIT.Name = "cbShareHIT";
            this.cbShareHIT.Size = new System.Drawing.Size(54, 17);
            this.cbShareHIT.TabIndex = 70;
            this.cbShareHIT.Text = "Share";
            this.cbShareHIT.UseVisualStyleBackColor = true;
            this.cbShareHIT.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareEVA
            // 
            this.cbShareEVA.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareEVA.AutoSize = true;
            this.cbShareEVA.Location = new System.Drawing.Point(414, 143);
            this.cbShareEVA.Name = "cbShareEVA";
            this.cbShareEVA.Size = new System.Drawing.Size(54, 17);
            this.cbShareEVA.TabIndex = 69;
            this.cbShareEVA.Text = "Share";
            this.cbShareEVA.UseVisualStyleBackColor = true;
            this.cbShareEVA.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareSKL
            // 
            this.cbShareSKL.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareSKL.AutoSize = true;
            this.cbShareSKL.Location = new System.Drawing.Point(414, 120);
            this.cbShareSKL.Name = "cbShareSKL";
            this.cbShareSKL.Size = new System.Drawing.Size(54, 17);
            this.cbShareSKL.TabIndex = 68;
            this.cbShareSKL.Text = "Share";
            this.cbShareSKL.UseVisualStyleBackColor = true;
            this.cbShareSKL.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareDEF
            // 
            this.cbShareDEF.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareDEF.AutoSize = true;
            this.cbShareDEF.Location = new System.Drawing.Point(414, 97);
            this.cbShareDEF.Name = "cbShareDEF";
            this.cbShareDEF.Size = new System.Drawing.Size(54, 17);
            this.cbShareDEF.TabIndex = 67;
            this.cbShareDEF.Text = "Share";
            this.cbShareDEF.UseVisualStyleBackColor = true;
            this.cbShareDEF.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareRNG
            // 
            this.cbShareRNG.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareRNG.AutoSize = true;
            this.cbShareRNG.Location = new System.Drawing.Point(414, 74);
            this.cbShareRNG.Name = "cbShareRNG";
            this.cbShareRNG.Size = new System.Drawing.Size(54, 17);
            this.cbShareRNG.TabIndex = 66;
            this.cbShareRNG.Text = "Share";
            this.cbShareRNG.UseVisualStyleBackColor = true;
            this.cbShareRNG.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // cbShareMEL
            // 
            this.cbShareMEL.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareMEL.AutoSize = true;
            this.cbShareMEL.Location = new System.Drawing.Point(414, 51);
            this.cbShareMEL.Name = "cbShareMEL";
            this.cbShareMEL.Size = new System.Drawing.Size(54, 17);
            this.cbShareMEL.TabIndex = 65;
            this.cbShareMEL.Text = "Share";
            this.cbShareMEL.UseVisualStyleBackColor = true;
            this.cbShareMEL.CheckedChanged += new System.EventHandler(this.cbCharacterShare_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Location = new System.Drawing.Point(6, 160);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(132, 138);
            this.groupBox6.TabIndex = 58;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Status Screen Portrait";
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(6, 16);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(132, 138);
            this.groupBox4.TabIndex = 56;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "VN Portrait";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(282, 121);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(30, 13);
            this.label12.TabIndex = 64;
            this.label12.Text = "SKL:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(282, 167);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(28, 13);
            this.label10.TabIndex = 63;
            this.label10.Text = "HIT:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(282, 144);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 13);
            this.label9.TabIndex = 62;
            this.label9.Text = "EVA:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(282, 98);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "DEF:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(282, 75);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 60;
            this.label7.Text = "RNG:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(282, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 59;
            this.label6.Text = "MEL:";
            // 
            // groupBox5
            // 
            this.groupBox5.Location = new System.Drawing.Point(144, 160);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(132, 138);
            this.groupBox5.TabIndex = 57;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Spirits";
            // 
            // groupBox8
            // 
            this.groupBox8.Location = new System.Drawing.Point(144, 16);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(132, 138);
            this.groupBox8.TabIndex = 55;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Skills";
            // 
            // lstCharactersToShareFrom
            // 
            this.lstCharactersToShareFrom.FormattingEnabled = true;
            this.lstCharactersToShareFrom.Location = new System.Drawing.Point(6, 19);
            this.lstCharactersToShareFrom.Name = "lstCharactersToShareFrom";
            this.lstCharactersToShareFrom.Size = new System.Drawing.Size(125, 290);
            this.lstCharactersToShareFrom.TabIndex = 1;
            this.lstCharactersToShareFrom.SelectedIndexChanged += new System.EventHandler(this.lstCharacters_SelectedIndexChanged);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.lstCharactersToShareFrom);
            this.groupBox9.Location = new System.Drawing.Point(155, 288);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(137, 322);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Characters to share from";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmSave});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // tsmSave
            // 
            this.tsmSave.Name = "tsmSave";
            this.tsmSave.Size = new System.Drawing.Size(43, 20);
            this.tsmSave.Text = "Save";
            this.tsmSave.Click += new System.EventHandler(this.tsmSave_Click);
            // 
            // groupBox10
            // 
            this.groupBox10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox10.Controls.Add(this.gbEventID);
            this.groupBox10.Controls.Add(this.lblMaxMovement);
            this.groupBox10.Controls.Add(this.lblMobility);
            this.groupBox10.Controls.Add(this.lblArmor);
            this.groupBox10.Controls.Add(this.lblRegenEN);
            this.groupBox10.Controls.Add(this.lblMaxEN);
            this.groupBox10.Controls.Add(this.lblMaxHP);
            this.groupBox10.Controls.Add(this.cbShareMaxMovement);
            this.groupBox10.Controls.Add(this.cbShareMobility);
            this.groupBox10.Controls.Add(this.cbShareArmor);
            this.groupBox10.Controls.Add(this.cbShareRegenEN);
            this.groupBox10.Controls.Add(this.cbShareMaxEN);
            this.groupBox10.Controls.Add(this.cbShareMaxHP);
            this.groupBox10.Controls.Add(this.label1);
            this.groupBox10.Controls.Add(this.label5);
            this.groupBox10.Controls.Add(this.label4);
            this.groupBox10.Controls.Add(this.label3);
            this.groupBox10.Controls.Add(this.label11);
            this.groupBox10.Controls.Add(this.label13);
            this.groupBox10.Controls.Add(this.groupBox7);
            this.groupBox10.Location = new System.Drawing.Point(298, 27);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(380, 255);
            this.groupBox10.TabIndex = 7;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Details";
            // 
            // lblMaxMovement
            // 
            this.lblMaxMovement.AutoSize = true;
            this.lblMaxMovement.Location = new System.Drawing.Point(257, 138);
            this.lblMaxMovement.Name = "lblMaxMovement";
            this.lblMaxMovement.Size = new System.Drawing.Size(31, 13);
            this.lblMaxMovement.TabIndex = 103;
            this.lblMaxMovement.Text = "0000";
            // 
            // lblMobility
            // 
            this.lblMobility.AutoSize = true;
            this.lblMobility.Location = new System.Drawing.Point(257, 115);
            this.lblMobility.Name = "lblMobility";
            this.lblMobility.Size = new System.Drawing.Size(31, 13);
            this.lblMobility.TabIndex = 102;
            this.lblMobility.Text = "0000";
            // 
            // lblArmor
            // 
            this.lblArmor.AutoSize = true;
            this.lblArmor.Location = new System.Drawing.Point(257, 92);
            this.lblArmor.Name = "lblArmor";
            this.lblArmor.Size = new System.Drawing.Size(31, 13);
            this.lblArmor.TabIndex = 101;
            this.lblArmor.Text = "0000";
            // 
            // lblRegenEN
            // 
            this.lblRegenEN.AutoSize = true;
            this.lblRegenEN.Location = new System.Drawing.Point(257, 69);
            this.lblRegenEN.Name = "lblRegenEN";
            this.lblRegenEN.Size = new System.Drawing.Size(31, 13);
            this.lblRegenEN.TabIndex = 100;
            this.lblRegenEN.Text = "0000";
            // 
            // lblMaxEN
            // 
            this.lblMaxEN.AutoSize = true;
            this.lblMaxEN.Location = new System.Drawing.Point(257, 46);
            this.lblMaxEN.Name = "lblMaxEN";
            this.lblMaxEN.Size = new System.Drawing.Size(31, 13);
            this.lblMaxEN.TabIndex = 99;
            this.lblMaxEN.Text = "0000";
            // 
            // lblMaxHP
            // 
            this.lblMaxHP.AutoSize = true;
            this.lblMaxHP.Location = new System.Drawing.Point(257, 23);
            this.lblMaxHP.Name = "lblMaxHP";
            this.lblMaxHP.Size = new System.Drawing.Size(31, 13);
            this.lblMaxHP.TabIndex = 98;
            this.lblMaxHP.Text = "0000";
            // 
            // cbShareMaxMovement
            // 
            this.cbShareMaxMovement.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareMaxMovement.AutoSize = true;
            this.cbShareMaxMovement.Location = new System.Drawing.Point(320, 137);
            this.cbShareMaxMovement.Name = "cbShareMaxMovement";
            this.cbShareMaxMovement.Size = new System.Drawing.Size(54, 17);
            this.cbShareMaxMovement.TabIndex = 97;
            this.cbShareMaxMovement.Text = "Share";
            this.cbShareMaxMovement.UseVisualStyleBackColor = true;
            // 
            // cbShareMobility
            // 
            this.cbShareMobility.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareMobility.AutoSize = true;
            this.cbShareMobility.Location = new System.Drawing.Point(320, 114);
            this.cbShareMobility.Name = "cbShareMobility";
            this.cbShareMobility.Size = new System.Drawing.Size(54, 17);
            this.cbShareMobility.TabIndex = 96;
            this.cbShareMobility.Text = "Share";
            this.cbShareMobility.UseVisualStyleBackColor = true;
            // 
            // cbShareArmor
            // 
            this.cbShareArmor.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareArmor.AutoSize = true;
            this.cbShareArmor.Location = new System.Drawing.Point(320, 91);
            this.cbShareArmor.Name = "cbShareArmor";
            this.cbShareArmor.Size = new System.Drawing.Size(54, 17);
            this.cbShareArmor.TabIndex = 95;
            this.cbShareArmor.Text = "Share";
            this.cbShareArmor.UseVisualStyleBackColor = true;
            // 
            // cbShareRegenEN
            // 
            this.cbShareRegenEN.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareRegenEN.AutoSize = true;
            this.cbShareRegenEN.Location = new System.Drawing.Point(320, 68);
            this.cbShareRegenEN.Name = "cbShareRegenEN";
            this.cbShareRegenEN.Size = new System.Drawing.Size(54, 17);
            this.cbShareRegenEN.TabIndex = 94;
            this.cbShareRegenEN.Text = "Share";
            this.cbShareRegenEN.UseVisualStyleBackColor = true;
            // 
            // cbShareMaxEN
            // 
            this.cbShareMaxEN.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareMaxEN.AutoSize = true;
            this.cbShareMaxEN.Location = new System.Drawing.Point(320, 45);
            this.cbShareMaxEN.Name = "cbShareMaxEN";
            this.cbShareMaxEN.Size = new System.Drawing.Size(54, 17);
            this.cbShareMaxEN.TabIndex = 93;
            this.cbShareMaxEN.Text = "Share";
            this.cbShareMaxEN.UseVisualStyleBackColor = true;
            // 
            // cbShareMaxHP
            // 
            this.cbShareMaxHP.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cbShareMaxHP.AutoSize = true;
            this.cbShareMaxHP.Location = new System.Drawing.Point(320, 22);
            this.cbShareMaxHP.Name = "cbShareMaxHP";
            this.cbShareMaxHP.Size = new System.Drawing.Size(54, 17);
            this.cbShareMaxHP.TabIndex = 92;
            this.cbShareMaxHP.Text = "Share";
            this.cbShareMaxHP.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(144, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 91;
            this.label1.Text = "Regen EN:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(144, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 90;
            this.label5.Text = "Max Movement:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(144, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 89;
            this.label4.Text = "Mobility:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(144, 92);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 88;
            this.label3.Text = "Armor:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(144, 46);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(48, 13);
            this.label11.TabIndex = 87;
            this.label11.Text = "Max EN:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(144, 23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 13);
            this.label13.TabIndex = 86;
            this.label13.Text = "Max HP:";
            // 
            // groupBox7
            // 
            this.groupBox7.Location = new System.Drawing.Point(6, 19);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(132, 138);
            this.groupBox7.TabIndex = 85;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Unit Sprite";
            // 
            // groupBox11
            // 
            this.groupBox11.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox11.Controls.Add(this.lstUnitsToShareFrom);
            this.groupBox11.Location = new System.Drawing.Point(155, 27);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(137, 255);
            this.groupBox11.TabIndex = 6;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Units to share from";
            // 
            // lstUnitsToShareFrom
            // 
            this.lstUnitsToShareFrom.FormattingEnabled = true;
            this.lstUnitsToShareFrom.Location = new System.Drawing.Point(6, 19);
            this.lstUnitsToShareFrom.Name = "lstUnitsToShareFrom";
            this.lstUnitsToShareFrom.Size = new System.Drawing.Size(125, 225);
            this.lstUnitsToShareFrom.TabIndex = 0;
            this.lstUnitsToShareFrom.SelectedIndexChanged += new System.EventHandler(this.lstUnits_SelectedIndexChanged);
            // 
            // txtEventID
            // 
            this.txtEventID.Location = new System.Drawing.Point(6, 19);
            this.txtEventID.Name = "txtEventID";
            this.txtEventID.Size = new System.Drawing.Size(120, 20);
            this.txtEventID.TabIndex = 104;
            this.txtEventID.TextChanged += new System.EventHandler(this.txtEventID_TextChanged);
            // 
            // gbEventID
            // 
            this.gbEventID.Controls.Add(this.txtEventID);
            this.gbEventID.Location = new System.Drawing.Point(6, 163);
            this.gbEventID.Name = "gbEventID";
            this.gbEventID.Size = new System.Drawing.Size(132, 86);
            this.gbEventID.TabIndex = 105;
            this.gbEventID.TabStop = false;
            this.gbEventID.Text = "Event ID";
            // 
            // ProjectEternityRosterEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 620);
            this.Controls.Add(this.groupBox11);
            this.Controls.Add(this.groupBox10);
            this.Controls.Add(this.groupBox9);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ProjectEternityRosterEditor";
            this.Text = "Roster Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.gbEventID.ResumeLayout(false);
            this.gbEventID.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstUnits;
        private System.Windows.Forms.ListBox lstCharacters;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnRemoveUnit;
        private System.Windows.Forms.Button btnAddUnit;
        private System.Windows.Forms.Button btnRemoveCharacter;
        private System.Windows.Forms.Button btnAddCharacter;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label lblHIT;
        private System.Windows.Forms.Label lblEVA;
        private System.Windows.Forms.Label lblSKL;
        private System.Windows.Forms.Label lblDEF;
        private System.Windows.Forms.Label lblRNG;
        private System.Windows.Forms.Label lblMEL;
        private System.Windows.Forms.CheckBox cbShareLevel;
        private System.Windows.Forms.CheckBox cbShareEXP;
        private System.Windows.Forms.CheckBox cbShareHIT;
        private System.Windows.Forms.CheckBox cbShareEVA;
        private System.Windows.Forms.CheckBox cbShareSKL;
        private System.Windows.Forms.CheckBox cbShareDEF;
        private System.Windows.Forms.CheckBox cbShareRNG;
        private System.Windows.Forms.CheckBox cbShareMEL;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.ListBox lstCharactersToShareFrom;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem tsmSave;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.Label lblMaxMovement;
        private System.Windows.Forms.Label lblMobility;
        private System.Windows.Forms.Label lblArmor;
        private System.Windows.Forms.Label lblRegenEN;
        private System.Windows.Forms.Label lblMaxEN;
        private System.Windows.Forms.Label lblMaxHP;
        private System.Windows.Forms.CheckBox cbShareMaxMovement;
        private System.Windows.Forms.CheckBox cbShareMobility;
        private System.Windows.Forms.CheckBox cbShareArmor;
        private System.Windows.Forms.CheckBox cbShareRegenEN;
        private System.Windows.Forms.CheckBox cbShareMaxEN;
        private System.Windows.Forms.CheckBox cbShareMaxHP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox groupBox11;
        private System.Windows.Forms.ListBox lstUnitsToShareFrom;
        private System.Windows.Forms.GroupBox gbEventID;
        private System.Windows.Forms.TextBox txtEventID;
    }
}

