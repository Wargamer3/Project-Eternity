using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Editors.ImageViewer;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class ProjectEternityAttackEditor : BaseEditor
    {
        private QuoteEditor Editor;

        private AdvancedSettingsEditor AdvancedSettings;
        private DashAttackEditor DashAttackEditor;
        private KnockbackAttackEditor KnockbackAttackEditor;
        private MAPAttackEditor MAPAttackEditor;
        private ALLAttackEditor ALLAttackEditor;
        private PERAttackEditor PERAttackEditor;

        public ProjectEternityAttackEditor()
        {
            InitializeComponent();

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
                AdvancedSettings = new AdvancedSettingsEditor();
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
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathAttacks }, "Attacks/", new string[] { ".pew" }, typeof(ProjectEternityAttackEditor)),
                new EditorInfo(new string[] { GUIRootPathAttackModels }, "Attacks/Models/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false, null, true),
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            SaveItem(BW);

            FS.Close();
            BW.Close();
        }

        public void SaveItem(BinaryWriter BW)
        {
            string Description = txtDescription.Text;

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
            else if (rbPER.Checked)
                PrimaryProperty = WeaponPrimaryProperty.PER;

            WeaponSecondaryProperty SecondaryProperty = WeaponSecondaryProperty.None;
            if (cbSwordCut.Checked)
                SecondaryProperty = SecondaryProperty | WeaponSecondaryProperty.SwordCut;
            if (cbShootDown.Checked)
                SecondaryProperty = SecondaryProperty | WeaponSecondaryProperty.ShootDown;
            if (cbPartialAttack.Checked)
                SecondaryProperty = SecondaryProperty | WeaponSecondaryProperty.Partial;

            byte AttackType = 0;
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

            //Create the Part file.
            BW.Write(Description);

            BW.Write(txtDamage.Text);
            BW.Write(txtMinDamage.Text);
            BW.Write((byte)txtENCost.Value);
            BW.Write((byte)txtMaximumAmmo.Value);
            BW.Write((byte)txtAmmoConsumption.Value);
            BW.Write((byte)txtMoraleRequirement.Value);
            BW.Write((byte)txtMinimumRange.Value);
            BW.Write((byte)txtMaximumRange.Value);
            BW.Write((sbyte)txtAccuracy.Value);
            BW.Write((sbyte)txtCritical.Value);

            BW.Write((byte)PrimaryProperty);
            if (PrimaryProperty == WeaponPrimaryProperty.Dash)
            {
                BW.Write((byte)DashAttackEditor.txtMaxDashReach.Value);
                BW.Write((byte)DashAttackEditor.txtEnemyKnockback.Value);
                BW.Write((byte)DashAttackEditor.txtSelfKnockback.Value);
            }
            else if (PrimaryProperty == WeaponPrimaryProperty.ALL)
            {
                BW.Write((byte)ALLAttackEditor.txtLevel.Value);
            }
            else if (PrimaryProperty == WeaponPrimaryProperty.MAP)
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
            else if (PrimaryProperty == WeaponPrimaryProperty.PER)
            {
                if (PERAttackEditor != null)
                {
                    BW.Write((float)PERAttackEditor.txtProjectileSpeed.Value);
                    BW.Write(PERAttackEditor.cbAffectedByGravity.Checked);
                    BW.Write(PERAttackEditor.cbCanBeShotDown.Checked);
                    BW.Write(PERAttackEditor.cbHoming.Checked);
                    BW.Write((byte)PERAttackEditor.txtMaxLifetime.Value);
                    BW.Write((byte)PERAttackEditor.cbAttackType.SelectedIndex);

                    BW.Write(PERAttackEditor.IsProjectileAnimated);
                    BW.Write(PERAttackEditor.txtProjectilePath.Text);
                    BW.Write(PERAttackEditor.txt3DModelPath.Text);
                    BW.Write((byte)PERAttackEditor.txtNumberOfProjectiles.Value);
                    BW.Write((float)PERAttackEditor.txtLateralMaxSpread.Value);
                    BW.Write((float)PERAttackEditor.txtForwardMaxSpread.Value);
                    BW.Write((float)PERAttackEditor.txtUpwardMaxSpread.Value);

                    BW.Write(PERAttackEditor.txtSkillChain.Text);

                    if (PERAttackEditor.rbDestroySelf.Checked)
                    {
                        BW.Write((byte)PERAttackAttributes.GroundCollisions.DestroySelf);
                    }
                    else if (PERAttackEditor.rbStop.Checked)
                    {
                        BW.Write((byte)PERAttackAttributes.GroundCollisions.Stop);
                    }
                    else if (PERAttackEditor.rbBounce.Checked)
                    {
                        BW.Write((byte)PERAttackAttributes.GroundCollisions.Bounce);
                        BW.Write((byte)PERAttackEditor.txtBounceLimit.Value);
                    }
                }
            }
            else if (KnockbackAttackEditor != null)
            {
                BW.Write((byte)KnockbackAttackEditor.txtEnemyKnockback.Value);
                BW.Write((byte)KnockbackAttackEditor.txtSelfKnockback.Value);
            }
            else
            {
                BW.Write((byte)0);
                BW.Write((byte)0);
            }

            BW.Write((byte)SecondaryProperty);
            BW.Write((byte)txtReMoveLevel.Value);
            BW.Write((byte)txtPostMovementLevel.Value);
            BW.Write((byte)txtPostMovementAccuracyMalus.Value);
            BW.Write((byte)txtPostMovementEvasionBonus.Value);

            BW.Write((byte)AttackType);

            List<Tuple<byte, byte>> ListRankByMovement = new List<Tuple<byte, byte>>();
            foreach (DataGridViewRow ActiveRow in dgvTerrainRanks.Rows)
            {
                if (ActiveRow.Cells[1].Value != null)
                {
                    ListRankByMovement.Add(new Tuple<byte, byte>((byte)UnitAndTerrainValues.Default.ListUnitMovement.IndexOf(UnitAndTerrainValues.Default.ListUnitMovement.Find(x => x.Name == ActiveRow.Cells[0].Value.ToString())),
                        (byte)Unit.ListRank.IndexOf(ActiveRow.Cells[1].Value.ToString()[0])));
                }
            }

            BW.Write(ListRankByMovement.Count);
            foreach (Tuple<byte, byte> ActiveRankByMovement in ListRankByMovement)
            {
                BW.Write(ActiveRankByMovement.Item1);
                BW.Write(ActiveRankByMovement.Item2);
            }

            BW.Write(AdvancedSettings.lvSecondaryAttack.Items.Count);
            for (int S = 0; S < AdvancedSettings.lvSecondaryAttack.Items.Count; S++)
            {
                BW.Write(AdvancedSettings.lvSecondaryAttack.Items[S].Tag.ToString());
            }

            BW.Write(AdvancedSettings.lvChargedAttacks.Items.Count);
            for (int S = 0; S < AdvancedSettings.lvChargedAttacks.Items.Count; S++)
            {
                BW.Write(AdvancedSettings.lvChargedAttacks.Items[S].Tag.ToString());
            }
            if (AdvancedSettings.lvChargedAttacks.Items.Count > 0)
            {
                BW.Write((byte)AdvancedSettings.txtChargeCancelLevel.Value);
            }

            BW.Write((float)AdvancedSettings.txtExplosionRadius.Value);
            if (AdvancedSettings.txtExplosionRadius.Value > 0)
            {
                BW.Write((float)AdvancedSettings.txtExplosionWindPowerAtCenter.Value);
                BW.Write((float)AdvancedSettings.txtExplosionWindPowerAtEdge.Value);
                BW.Write((float)AdvancedSettings.txtExplosionWindPowerToSelfMultiplier.Value);
                BW.Write((float)AdvancedSettings.txtExplosionDamageAtCenter.Value);
                BW.Write((float)AdvancedSettings.txtExplosionDamageAtEdge.Value);
                BW.Write((float)AdvancedSettings.txtExplosionDamageToSelfMultiplier.Value);
            }

            #region Skills

            int PilotSkillCount = 0;
            if (AdvancedSettings.txtPilotSkill1.Text != "None")
                ++PilotSkillCount;
            if (AdvancedSettings.txtPilotSkill2.Text != "None")
                ++PilotSkillCount;
            if (AdvancedSettings.txtPilotSkill3.Text != "None")
                ++PilotSkillCount;
            if (AdvancedSettings.txtPilotSkill4.Text != "None")
                ++PilotSkillCount;

            BW.Write(PilotSkillCount);

            if (AdvancedSettings.txtPilotSkill1.Text != "None")
                BW.Write(AdvancedSettings.txtPilotSkill1.Text);
            if (AdvancedSettings.txtPilotSkill2.Text != "None")
                BW.Write(AdvancedSettings.txtPilotSkill2.Text);
            if (AdvancedSettings.txtPilotSkill3.Text != "None")
                BW.Write(AdvancedSettings.txtPilotSkill3.Text);
            if (AdvancedSettings.txtPilotSkill4.Text != "None")
                BW.Write(AdvancedSettings.txtPilotSkill4.Text);

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

            Attack ActiveWeapon = new Attack(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);
            LoadAttack(ActiveWeapon);
        }

        private void LoadAttack(Attack ActiveWeapon)
        {
            this.Text = ActiveWeapon.RelativePath + " - Project Eternity Attack Editor";
            Editor = new QuoteEditor();
            AdvancedSettings = new AdvancedSettingsEditor();
            DashAttackEditor = new DashAttackEditor();
            KnockbackAttackEditor = new KnockbackAttackEditor();
            MAPAttackEditor = new MAPAttackEditor();
            ALLAttackEditor = new ALLAttackEditor();
            PERAttackEditor = new PERAttackEditor();

            //Create the Part file.
            string ItemName = ActiveWeapon.RelativePath;
            string Description = ActiveWeapon.Description;

            int MoraleRequirement = ActiveWeapon.MoraleRequirement;
            int MinimumRange = ActiveWeapon.RangeMinimum;
            int MaximumRange = ActiveWeapon.RangeMaximum;
            int Accuracy = ActiveWeapon.Accuracy;
            int Critical = ActiveWeapon.Critical;

            string AttackType = ActiveWeapon.AttackType;

            Dictionary<byte, byte> DicTerrainAttribute = ActiveWeapon.DicRankByMovement;

            txtName.Text = ItemName;
            txtDescription.Text = Description;

            txtDamage.Text = ActiveWeapon.PowerFormula;
            txtMinDamage.Text = ActiveWeapon.MinDamageFormula;
            txtENCost.Value = ActiveWeapon.ENCost;
            txtMaximumAmmo.Value = ActiveWeapon.MaxAmmo;
            txtAmmoConsumption.Value = ActiveWeapon.AmmoConsumption;
            txtMoraleRequirement.Text = MoraleRequirement.ToString();
            txtMinimumRange.Text = MinimumRange.ToString();
            txtMaximumRange.Text = MaximumRange.ToString();
            txtAccuracy.Text = Accuracy.ToString();
            txtCritical.Text = Critical.ToString();

            KnockbackAttackEditor.txtEnemyKnockback.Value = (decimal)ActiveWeapon.KnockbackAttributes.EnemyKnockback;
            KnockbackAttackEditor.txtSelfKnockback.Value = (decimal)ActiveWeapon.KnockbackAttributes.SelfKnockback;

            #region Primary

            MAPAttackEditor.ckFriendlyFire.Checked = ActiveWeapon.MAPAttributes.FriendlyFire;
            MAPAttackEditor.txtAttackDelay.Value = ActiveWeapon.MAPAttributes.Delay;

            switch (ActiveWeapon.Pri)
            {
                case WeaponPrimaryProperty.ALL:
                    rbALL.Checked = true;
                    ALLAttackEditor.txtLevel.Value = ActiveWeapon.ALLLevel;
                    break;
                case WeaponPrimaryProperty.Dash:
                    rbDASH.Checked = true;
                    DashAttackEditor.txtMaxDashReach.Value = ActiveWeapon.DashMaxReach;
                    DashAttackEditor.txtEnemyKnockback.Value = ActiveWeapon.KnockbackAttributes.EnemyKnockback;
                    DashAttackEditor.txtSelfKnockback.Value = ActiveWeapon.KnockbackAttributes.SelfKnockback;
                    break;

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

                case WeaponPrimaryProperty.PER:
                    PERAttackEditor.txtProjectileSpeed.Value = (decimal)ActiveWeapon.PERAttributes.ProjectileSpeed;
                    PERAttackEditor.cbAffectedByGravity.Checked = ActiveWeapon.PERAttributes.AffectedByGravity;
                    PERAttackEditor.cbCanBeShotDown.Checked = ActiveWeapon.PERAttributes.CanBeShotDown;
                    PERAttackEditor.cbHoming.Checked = ActiveWeapon.PERAttributes.Homing;
                    PERAttackEditor.txtMaxLifetime.Value = ActiveWeapon.PERAttributes.MaxLifetime;
                    PERAttackEditor.cbAttackType.SelectedIndex = (int)ActiveWeapon.PERAttributes.AttackType;

                    PERAttackEditor.IsProjectileAnimated = ActiveWeapon.PERAttributes.ProjectileAnimation.IsAnimated;
                    PERAttackEditor.txtProjectilePath.Text = ActiveWeapon.PERAttributes.ProjectileAnimation.Path;
                    PERAttackEditor.txt3DModelPath.Text = ActiveWeapon.PERAttributes.Projectile3DModelPath;

                    PERAttackEditor.txtNumberOfProjectiles.Value = ActiveWeapon.PERAttributes.NumberOfProjectiles;
                    PERAttackEditor.txtLateralMaxSpread.Value = (decimal)ActiveWeapon.PERAttributes.MaxLateralSpread;
                    PERAttackEditor.txtForwardMaxSpread.Value = (decimal)ActiveWeapon.PERAttributes.MaxForwardSpread;
                    PERAttackEditor.txtUpwardMaxSpread.Value = (decimal)ActiveWeapon.PERAttributes.MaxUpwardSpread;
                    PERAttackEditor.txtSkillChain.Text = ActiveWeapon.PERAttributes.SkillChainName;

                    if (ActiveWeapon.PERAttributes.GroundCollision == PERAttackAttributes.GroundCollisions.DestroySelf)
                    {
                        PERAttackEditor.rbDestroySelf.Checked = true;
                    }
                    else if (ActiveWeapon.PERAttributes.GroundCollision == PERAttackAttributes.GroundCollisions.Stop)
                    {
                        PERAttackEditor.rbStop.Checked = true;
                    }
                    else if (ActiveWeapon.PERAttributes.GroundCollision == PERAttackAttributes.GroundCollisions.Bounce)
                    {
                        PERAttackEditor.rbBounce.Checked = true;
                        PERAttackEditor.txtBounceLimit.Value = ActiveWeapon.PERAttributes.BounceLimit;
                    }

                    rbPER.Checked = true;
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

            if ((ActiveWeapon.Sec & WeaponSecondaryProperty.SwordCut) == WeaponSecondaryProperty.SwordCut)
                cbSwordCut.Checked = true;

            if ((ActiveWeapon.Sec & WeaponSecondaryProperty.ShootDown) == WeaponSecondaryProperty.ShootDown)
                cbShootDown.Checked = true;

            if ((ActiveWeapon.Sec & WeaponSecondaryProperty.Partial) == WeaponSecondaryProperty.Partial)
                cbPartialAttack.Checked = true;

            txtReMoveLevel.Value = ActiveWeapon.ReMoveLevel;
            txtPostMovementLevel.Value = ActiveWeapon.PostMovementLevel;
            txtPostMovementAccuracyMalus.Value = ActiveWeapon.PostMovementAccuracyMalus;
            txtPostMovementEvasionBonus.Value = ActiveWeapon.PostMovementEvasionBonus;

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

            for (byte M = 0; M < UnitAndTerrainValues.Default.ListUnitMovement.Count; M++)
            {
                UnitMovementType ActiveMovement = UnitAndTerrainValues.Default.ListUnitMovement[M];
                int NewRowIndex = dgvTerrainRanks.Rows.Add();

                DataGridViewComboBoxCell MovementCell = new DataGridViewComboBoxCell();
                DataGridViewComboBoxCell RankCell = new DataGridViewComboBoxCell();

                foreach (UnitMovementType ActiveMovementChoice in UnitAndTerrainValues.Default.ListUnitMovement)
                {
                    MovementCell.Items.Add(ActiveMovementChoice.Name);
                }
                foreach (char ActiveRank in Unit.ListRank)
                {
                    RankCell.Items.Add(ActiveRank);
                }

                MovementCell.Value = ActiveMovement.Name;

                if (ActiveWeapon.DicRankByMovement.ContainsKey(M))
                {
                    RankCell.Value = Unit.ListRank[ActiveWeapon.DicRankByMovement[M]];
                }

                dgvTerrainRanks.Rows[NewRowIndex].Cells[0] = MovementCell;
                dgvTerrainRanks.Rows[NewRowIndex].Cells[1] = RankCell;
            }

            #endregion

            #region Special Attacks

            foreach (Attack ActiveAttack in ActiveWeapon.ListSecondaryAttack)
            {
                string AttackName = ActiveAttack.RelativePath;
                string[] ArrayName = AttackName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                ListViewItem NewItem = new ListViewItem(ArrayName[ArrayName.Length - 1]);
                NewItem.Tag = AttackName;
                AdvancedSettings.lvSecondaryAttack.Items.Add(NewItem);
            }

            #endregion

            #region Charged Attacks

            foreach (Attack ActiveAttack in ActiveWeapon.ListChargedAttack)
            {
                string AttackName = ActiveAttack.RelativePath;
                string[] ArrayName = AttackName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                ListViewItem NewItem = new ListViewItem(ArrayName[ArrayName.Length - 1]);
                NewItem.Tag = AttackName;
                AdvancedSettings.lvChargedAttacks.Items.Add(NewItem);
            }

            #endregion

            #region Explosion Attributes

            AdvancedSettings.txtExplosionRadius.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionRadius;
            AdvancedSettings.txtExplosionWindPowerAtCenter.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionWindPowerAtCenter;
            AdvancedSettings.txtExplosionWindPowerAtEdge.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionWindPowerAtEdge;
            AdvancedSettings.txtExplosionWindPowerToSelfMultiplier.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionWindPowerToSelfMultiplier;
            AdvancedSettings.txtExplosionDamageAtCenter.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionDamageAtCenter;
            AdvancedSettings.txtExplosionDamageAtEdge.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionDamageAtEdge;
            AdvancedSettings.txtExplosionDamageToSelfMultiplier.Value = (decimal)ActiveWeapon.ExplosionOption.ExplosionDamageToSelfMultiplier;

            #endregion

            #region Attack Attributes

            if (ActiveWeapon.ArrayAttackAttributes.Length >= 1)
            {
                AdvancedSettings.txtPilotSkill1.Text = ActiveWeapon.ArrayAttackAttributes[0].Name;
            }
            if (ActiveWeapon.ArrayAttackAttributes.Length >= 2)
            {
                AdvancedSettings.txtPilotSkill2.Text = ActiveWeapon.ArrayAttackAttributes[1].Name;
            }
            if (ActiveWeapon.ArrayAttackAttributes.Length >= 3)
            {
                AdvancedSettings.txtPilotSkill3.Text = ActiveWeapon.ArrayAttackAttributes[2].Name;
            }
            if (ActiveWeapon.ArrayAttackAttributes.Length >= 4)
            {
                AdvancedSettings.txtPilotSkill4.Text = ActiveWeapon.ArrayAttackAttributes[3].Name;
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

        private void btnEditDamage_Click(object sender, EventArgs e)
        {
            AttackFormulaEditor DamageDialog = new AttackFormulaEditor();
            if (DamageDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtDamage.Text = DamageDialog.Code;
            }
        }

        private void btnConfigure_Click(object sender, EventArgs e)
        {
            if (rbDASH.Checked)
            {
                DashAttackEditor.ShowDialog();
            }
            else if (rbALL.Checked)
            {
                ALLAttackEditor.ShowDialog();
            }
            else if (rbMAP.Checked)
            {
                MAPAttackEditor.ShowDialog();
            }
            else if (rbPER.Checked)
            {
                PERAttackEditor.ShowDialog();
            }
            else
            {
                KnockbackAttackEditor.ShowDialog();
            }
        }

        private void tsmAdvanced_Click(object sender, EventArgs e)
        {
            AdvancedSettings.ShowDialog();
        }
    }
}
