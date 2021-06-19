using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Core.Units;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class ProjectEternityAttackEditor : BaseEditor
    {
        private enum ItemSelectionChoices {  Skill1, Skill2, Skill3, Skill4 };

        private ItemSelectionChoices ItemSelectionChoice;

        private QuoteEditor Editor;

        private MAPAttackEditor MAPAttackEditor;

        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;

        public ProjectEternityAttackEditor()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();

            cbAirRank.SelectedIndex = 0;
            cbLandRank.SelectedIndex = 0;
            cbSeaRank.SelectedIndex = 0;
            cbSpaceRank.SelectedIndex = 0;
            txtName.Text = "New Item";
            txtName.ReadOnly = false;
            menuStrip1.Visible = false;
        }

        public ProjectEternityAttackEditor(Attack ActiveAttack)
            : this()
        {
            LoadAttack(ActiveAttack);
        }

        public ProjectEternityAttackEditor(string FilePath, object[] Params)
            : this()
        {
            txtName.ReadOnly = true;
            menuStrip1.Visible = true;

            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadAttack(this.FilePath);
        }

        public override string ToString()
        {
            return txtName.Text;
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[] { new EditorInfo(new string[] { GUIRootPathAttacks }, "Attacks/", new string[] { ".pew" }, typeof(ProjectEternityAttackEditor)) };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveItem(BW);

            FS.Close();
            BW.Close();
        }

        public void SaveItem(BinaryWriter BW)
        {
            string Description = txtDescription.Text;

            string DamageFormula = txtDamage.Text;
            int ENCost = Convert.ToInt32(txtENCost.Value);
            int MaximumAmmo = Convert.ToInt32(txtMaximumAmmo.Value);
            int MoraleRequirement = Convert.ToInt32(txtMoraleRequirement.Value);
            int MinimumRange = Convert.ToInt32(txtMinimumRange.Value);
            int MaximumRange = Convert.ToInt32(txtMaximumRange.Value);
            int Accuracy = Convert.ToInt32(txtAccuracy.Value);
            int Critical = Convert.ToInt32(txtCritical.Value);

            WeaponPrimaryProperty PrimaryProperty = WeaponPrimaryProperty.None;
            if (rbMAP.Checked)
                PrimaryProperty = WeaponPrimaryProperty.MAP;
            else if (rbPLA.Checked)
                PrimaryProperty = WeaponPrimaryProperty.PLA;
            else if (rbALL.Checked)
                PrimaryProperty = WeaponPrimaryProperty.ALL;
            else if (rbBREAK.Checked)
                PrimaryProperty = WeaponPrimaryProperty.Break;
            else if (rbMULTI.Checked)
                PrimaryProperty = WeaponPrimaryProperty.Multi;
            else if (rbDASH.Checked)
                PrimaryProperty = WeaponPrimaryProperty.Dash;

            WeaponSecondaryProperty SecondaryProperty = WeaponSecondaryProperty.None;
            if (cbPostMovement.Checked)
                SecondaryProperty = SecondaryProperty | WeaponSecondaryProperty.PostMovement;
            if (cbSwordCut.Checked)
                SecondaryProperty = SecondaryProperty | WeaponSecondaryProperty.SwordCut;
            if (cbShootDown.Checked)
                SecondaryProperty = SecondaryProperty | WeaponSecondaryProperty.ShootDown;

            int AttackType = 0;
            if (rbAttackTypeMelee.Checked)
                AttackType = 1;
            else if (rbAttackTypeSolidBlade.Checked)
                AttackType = 2;
            else if (rbAttackTypeEnergyBlade.Checked)
                AttackType = 3;
            else if (rbAttackTypeSolidShot.Checked)
                AttackType = 4;
            else if (rbAttackTypeEnergyShot.Checked)
                AttackType = 5;
            else if (rbAttackTypeRemote.Checked)
                AttackType = 6;
            else if (rbAttackTypeSpecial.Checked)
                AttackType = 7;

            int TerrainGradeAir = cbAirRank.SelectedIndex;
            int TerrainGradeLand = cbLandRank.SelectedIndex;
            int TerrainGradeSea = cbSeaRank.SelectedIndex;
            int TerrainGradeSpace = cbSpaceRank.SelectedIndex;

            //Create the Part file.
            BW.Write(Description);

            BW.Write(DamageFormula);
            BW.Write(ENCost);
            BW.Write(MaximumAmmo);
            BW.Write(MoraleRequirement);
            BW.Write(MinimumRange);
            BW.Write(MaximumRange);
            BW.Write(Accuracy);
            BW.Write(Critical);

            BW.Write((byte)PrimaryProperty);
            if (PrimaryProperty == WeaponPrimaryProperty.MAP)
            {
                if (MAPAttackEditor != null)
                {
                    BW.Write(MAPAttackEditor.AttackWidth);
                    BW.Write(MAPAttackEditor.AttackHeight);
                    for (int X = 0; X < MAPAttackEditor.AttackWidth * 2 + 1; X++)
                        for (int Y = 0; Y < MAPAttackEditor.AttackHeight * 2 + 1; Y++)
                            BW.Write(MAPAttackEditor.ListAttackchoice[X][Y]);

                    if (MAPAttackEditor.rbSpread.Checked)
                        BW.Write((byte)WeaponMAPProperties.Spread);
                    else if (MAPAttackEditor.rbDirection.Checked)
                        BW.Write((byte)WeaponMAPProperties.Direction);
                    else if (MAPAttackEditor.rbTargeted.Checked)
                        BW.Write((byte)WeaponMAPProperties.Targeted);

                    BW.Write(MAPAttackEditor.ckFriendlyFire.Checked);
                    BW.Write((int)MAPAttackEditor.txtAttackDelay.Value);
                }
            }
            BW.Write((byte)SecondaryProperty);
            BW.Write(AttackType);

            BW.Write(TerrainGradeAir);
            BW.Write(TerrainGradeLand);
            BW.Write(TerrainGradeSea);
            BW.Write(TerrainGradeSpace);

            #region Skills

            int PilotSkillCount = 0;
            if (txtPilotSkill1.Text != "None")
                ++PilotSkillCount;
            if (txtPilotSkill2.Text != "None")
                ++PilotSkillCount;
            if (txtPilotSkill3.Text != "None")
                ++PilotSkillCount;
            if (txtPilotSkill4.Text != "None")
                ++PilotSkillCount;

            BW.Write(PilotSkillCount);

            if (txtPilotSkill1.Text != "None")
                BW.Write(txtPilotSkill1.Text);
            if (txtPilotSkill2.Text != "None")
                BW.Write(txtPilotSkill2.Text);
            if (txtPilotSkill3.Text != "None")
                BW.Write(txtPilotSkill3.Text);
            if (txtPilotSkill4.Text != "None")
                BW.Write(txtPilotSkill4.Text);

            #endregion

            //Count the actual number of quotes listed.
            int QuoteCount = 0;
            if (Editor != null)
            {
                for (int R = 0; R < Editor.dgvQuotes.Rows.Count; R++)
                    if (!string.IsNullOrEmpty((string)Editor.dgvQuotes.Rows[R].Cells[0].EditedFormattedValue))
                        QuoteCount++;
            }

            BW.Write(QuoteCount);
            if (Editor != null)
            {
                for (int R = 0; R < Editor.dgvQuotes.Rows.Count; R++)
                {
                    if (!string.IsNullOrEmpty((string)Editor.dgvQuotes.Rows[R].Cells[0].EditedFormattedValue))
                        BW.Write((string)Editor.dgvQuotes.Rows[R].Cells[0].EditedFormattedValue);
                }
            }
        }

        private void LoadAttack(string AttackPath)
        {
            string Name = AttackPath.Substring(0, AttackPath.Length - 4).Substring(16);

            Attack ActiveWeapon = new Attack(Name, DicRequirement, DicEffect);
            LoadAttack(ActiveWeapon);
        }

        private void LoadAttack(Attack ActiveWeapon)
        {
            this.Text = ActiveWeapon.FullName + " - Project Eternity Attack Editor";
            Editor = new QuoteEditor();
            MAPAttackEditor = new MAPAttackEditor();

            //Create the Part file.
            string ItemName = ActiveWeapon.FullName;
            string Description = ActiveWeapon.Description;

            string PowerFormula = ActiveWeapon.PowerFormula;
            int ENCost = ActiveWeapon.ENCost;
            int MaximumAmmo = ActiveWeapon.MaxAmmo;
            int MoraleRequirement = ActiveWeapon.MoraleRequirement;
            int MinimumRange = ActiveWeapon.RangeMinimum;
            int MaximumRange = ActiveWeapon.RangeMaximum;
            int Accuracy = ActiveWeapon.Accuracy;
            int Critical = ActiveWeapon.Critical;

            string AttackType = ActiveWeapon.AttackType;

            Dictionary<string, char> DicTerrainAttribute = ActiveWeapon.DicTerrainAttribute;

            txtName.Text = ItemName;
            txtDescription.Text = Description;

            txtDamage.Text = PowerFormula;
            txtENCost.Text = ENCost.ToString();
            txtMaximumAmmo.Text = MaximumAmmo.ToString();
            txtMoraleRequirement.Text = MoraleRequirement.ToString();
            txtMinimumRange.Text = MinimumRange.ToString();
            txtMaximumRange.Text = MaximumRange.ToString();
            txtAccuracy.Text = Accuracy.ToString();
            txtCritical.Text = Critical.ToString();

            #region Primary

            switch (ActiveWeapon.Pri)
            {
                case WeaponPrimaryProperty.MAP:
                    rbMAP.Checked = true;
                    MAPAttackEditor.ListAttackchoice = ActiveWeapon.MAPAttributes.ListChoice;
                    MAPAttackEditor.txtAttackWidth.Value = MAPAttackEditor.AttackWidth = ActiveWeapon.MAPAttributes.Width;
                    MAPAttackEditor.txtAttackHeight.Value = MAPAttackEditor.AttackHeight = ActiveWeapon.MAPAttributes.Height;

                    if (ActiveWeapon.MAPAttributes.Property == WeaponMAPProperties.Spread)
                        MAPAttackEditor.rbSpread.Checked = true;
                    else if (ActiveWeapon.MAPAttributes.Property == WeaponMAPProperties.Direction)
                        MAPAttackEditor.rbDirection.Checked = true;
                    else if (ActiveWeapon.MAPAttributes.Property == WeaponMAPProperties.Targeted)
                        MAPAttackEditor.rbTargeted.Checked = true;

                    MAPAttackEditor.ckFriendlyFire.Checked = ActiveWeapon.MAPAttributes.FriendlyFire;
                    MAPAttackEditor.txtAttackDelay.Value = ActiveWeapon.MAPAttributes.Delay;
                    break;

                case WeaponPrimaryProperty.ALL:
                    rbALL.Checked = true;
                    break;

                case WeaponPrimaryProperty.PLA:
                    rbPLA.Checked = true;
                    break;

                default:
                    rbNone.Checked = true;
                    break;
            }

            #endregion

            #region Secondary

            if ((ActiveWeapon.Sec & WeaponSecondaryProperty.PostMovement) == WeaponSecondaryProperty.PostMovement)
                cbPostMovement.Checked = true;

            if ((ActiveWeapon.Sec & WeaponSecondaryProperty.SwordCut) == WeaponSecondaryProperty.SwordCut)
                cbSwordCut.Checked = true;

            if ((ActiveWeapon.Sec & WeaponSecondaryProperty.ShootDown) == WeaponSecondaryProperty.ShootDown)
                cbShootDown.Checked = true;

            #endregion

            #region Attack type

            switch (AttackType)
            {
                case "Blank":
                    rbAttackTypeBlank.Checked = true;
                    break;

                case "Melee":
                    rbAttackTypeMelee.Checked = true;
                    break;

                case "Solid blade":
                    rbAttackTypeSolidBlade.Checked = true;
                    break;

                case "Energy blade":
                    rbAttackTypeEnergyBlade.Checked = true;
                    break;

                case "Solid shot":
                    rbAttackTypeSolidShot.Checked = true;
                    break;

                case "Energy shot":
                    rbAttackTypeEnergyShot.Checked = true;
                    break;

                case "Remote":
                    rbAttackTypeRemote.Checked = true;
                    break;

                case "Special":
                    rbAttackTypeSpecial.Checked = true;
                    break;
            }

            #endregion

            #region Terrain grades

            #region Air

            switch (DicTerrainAttribute[UnitStats.TerrainAir])
            {
                case '-':
                    cbAirRank.SelectedIndex = 0;
                    break;

                case 'S':
                    cbAirRank.SelectedIndex = 1;
                    break;

                case 'A':
                    cbAirRank.SelectedIndex = 2;
                    break;

                case 'B':
                    cbAirRank.SelectedIndex = 3;
                    break;

                case 'C':
                    cbAirRank.SelectedIndex = 4;
                    break;

                case 'D':
                    cbAirRank.SelectedIndex = 5;
                    break;
            }

            #endregion

            #region Land

            switch (DicTerrainAttribute[UnitStats.TerrainLand])
            {
                case '-':
                    cbLandRank.SelectedIndex = 0;
                    break;

                case 'S':
                    cbLandRank.SelectedIndex = 1;
                    break;

                case 'A':
                    cbLandRank.SelectedIndex = 2;
                    break;

                case 'B':
                    cbLandRank.SelectedIndex = 3;
                    break;

                case 'C':
                    cbLandRank.SelectedIndex = 4;
                    break;

                case 'D':
                    cbLandRank.SelectedIndex = 5;
                    break;
            }

            #endregion

            #region Sea

            switch (DicTerrainAttribute[UnitStats.TerrainSea])
            {
                case '-':
                    cbSeaRank.SelectedIndex = 0;
                    break;

                case 'S':
                    cbSeaRank.SelectedIndex = 1;
                    break;

                case 'A':
                    cbSeaRank.SelectedIndex = 2;
                    break;

                case 'B':
                    cbSeaRank.SelectedIndex = 3;
                    break;

                case 'C':
                    cbSeaRank.SelectedIndex = 4;
                    break;

                case 'D':
                    cbSeaRank.SelectedIndex = 5;
                    break;
            }

            #endregion

            #region Space

            switch (DicTerrainAttribute[UnitStats.TerrainSpace])
            {
                case '-':
                    cbSpaceRank.SelectedIndex = 0;
                    break;

                case 'S':
                    cbSpaceRank.SelectedIndex = 1;
                    break;

                case 'A':
                    cbSpaceRank.SelectedIndex = 2;
                    break;

                case 'B':
                    cbSpaceRank.SelectedIndex = 3;
                    break;

                case 'C':
                    cbSpaceRank.SelectedIndex = 4;
                    break;

                case 'D':
                    cbSpaceRank.SelectedIndex = 5;
                    break;
            }

            #endregion

            #endregion

            #region Attack Attributes

            if (ActiveWeapon.ArrayAttackAttributes.Length >= 1)
            {
                txtPilotSkill1.Text = ActiveWeapon.ArrayAttackAttributes[0].Name;
            }
            if (ActiveWeapon.ArrayAttackAttributes.Length >= 2)
            {
                txtPilotSkill2.Text = ActiveWeapon.ArrayAttackAttributes[1].Name;
            }
            if (ActiveWeapon.ArrayAttackAttributes.Length >= 3)
            {
                txtPilotSkill3.Text = ActiveWeapon.ArrayAttackAttributes[2].Name;
            }
            if (ActiveWeapon.ArrayAttackAttributes.Length >= 4)
            {
                txtPilotSkill4.Text = ActiveWeapon.ArrayAttackAttributes[3].Name;
            }

            #endregion

            #region Quotes

            for (int Q = 0; Q < ActiveWeapon.ListQuoteSet.Count; Q++)
                Editor.dgvQuotes.Rows.Add(ActiveWeapon.ListQuoteSet[Q]);

            #endregion
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void btnEditQuotes_Click(object sender, EventArgs e)
        {
            Editor.ShowDialog();
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            MAPAttackEditor.ShowDialog();
        }

        private void btnSetSkill1_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill1;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAttackAttributes));
        }

        private void btnSetSkill2_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill2;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAttackAttributes));
        }

        private void btnSetSkill3_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill3;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAttackAttributes));
        }

        private void btnSetSkill4_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill4;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAttackAttributes));
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Skill1:
                        if (Items[I] == null)
                            txtPilotSkill1.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill1.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Skill2:
                        if (Items[I] == null)
                            txtPilotSkill2.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill2.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Skill3:
                        if (Items[I] == null)
                            txtPilotSkill3.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill3.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Skill4:
                        if (Items[I] == null)
                            txtPilotSkill4.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill4.Text = Name;
                        }
                        break;
                }
            }
        }

        private void btnEditDamage_Click(object sender, EventArgs e)
        {
            AttackFormulaEditor DamageDialog = new AttackFormulaEditor();
            if (DamageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDamage.Text = DamageDialog.Code;
            }
        }
    }
}
