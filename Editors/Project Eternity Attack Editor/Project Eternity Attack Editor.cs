using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;
using ProjectEternity.Editors.ImageViewer;
using System.Drawing;

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
        private DestructibleTilesEditor DestructibleTilesEditor;

        private SolidBrush GridBrush;

        public ProjectEternityAttackEditor()
        {
            InitializeComponent();

            GridBrush = new SolidBrush(dgvTerrainRanks.RowHeadersDefaultCellStyle.ForeColor);

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
                DestructibleTilesEditor = new DestructibleTilesEditor();
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

            #region Primary

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
                    BW.Write((byte)Math.Max(0, PERAttackEditor.cbAttackType.SelectedIndex));

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

            #endregion

            BW.Write((byte)SecondaryProperty);
            BW.Write((byte)txtReMoveLevel.Value);
            BW.Write((byte)txtPostMovementLevel.Value);
            BW.Write((byte)txtPostMovementAccuracyMalus.Value);
            BW.Write((byte)txtPostMovementEvasionBonus.Value);

            BW.Write(ckUseRotation.Checked);
            DestructibleTilesEditor.Save(BW);

            if (rbAttackTypeMelee.Checked)
            {
                BW.Write((byte)1);
            }
            else if (rbAttackTypeSolidBlade.Checked)
            {
                BW.Write((byte)2);
            }
            else if (rbAttackTypeEnergyBlade.Checked)
            {
                BW.Write((byte)3);
            }
            else if (rbAttackTypeSolidShot.Checked)
            {
                BW.Write((byte)4);
            }
            else if (rbAttackTypeEnergyShot.Checked)
            {
                BW.Write((byte)5);
            }
            else if (rbAttackTypeRemote.Checked)
            {
                BW.Write((byte)6);
            }
            else if (rbAttackTypeSpecial.Checked)
            {
                BW.Write((byte)7);
                BW.Write(txtSpecialAttack.Text);
            }
            else
            {
                BW.Write((byte)0);
            }

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

            AdvancedSettings.Save(BW);

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

            Attack ActiveAttack = new Attack(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);
            LoadAttack(ActiveAttack);
        }

        private void LoadAttack(Attack LoadedAttack)
        {
            this.Text = LoadedAttack.RelativePath + " - Project Eternity Attack Editor";
            Editor = new QuoteEditor();
            AdvancedSettings = new AdvancedSettingsEditor();
            DashAttackEditor = new DashAttackEditor();
            KnockbackAttackEditor = new KnockbackAttackEditor();
            MAPAttackEditor = new MAPAttackEditor();
            ALLAttackEditor = new ALLAttackEditor();
            PERAttackEditor = new PERAttackEditor();
            DestructibleTilesEditor = new DestructibleTilesEditor();

            //Create the Part file.
            string ItemName = LoadedAttack.RelativePath;
            string Description = LoadedAttack.Description;

            int MoraleRequirement = LoadedAttack.MoraleRequirement;
            int MinimumRange = LoadedAttack.RangeMinimum;
            int MaximumRange = LoadedAttack.RangeMaximum;
            int Accuracy = LoadedAttack.Accuracy;
            int Critical = LoadedAttack.Critical;

            string AttackType = LoadedAttack.AttackType;

            Dictionary<byte, byte> DicTerrainAttribute = LoadedAttack.DicRankByMovement;

            txtName.Text = ItemName;
            txtDescription.Text = Description;

            txtDamage.Text = LoadedAttack.PowerFormula;
            txtMinDamage.Text = LoadedAttack.MinDamageFormula;
            txtENCost.Value = LoadedAttack.ENCost;
            txtMaximumAmmo.Value = LoadedAttack.MaxAmmo;
            txtAmmoConsumption.Value = LoadedAttack.AmmoConsumption;
            txtMoraleRequirement.Text = MoraleRequirement.ToString();
            txtMinimumRange.Text = MinimumRange.ToString();
            txtMaximumRange.Text = MaximumRange.ToString();
            txtAccuracy.Text = Accuracy.ToString();
            txtCritical.Text = Critical.ToString();
            ckUseRotation.Checked = LoadedAttack.RotationAttributes != null;

            KnockbackAttackEditor.txtEnemyKnockback.Value = (decimal)LoadedAttack.KnockbackAttributes.EnemyKnockback;
            KnockbackAttackEditor.txtSelfKnockback.Value = (decimal)LoadedAttack.KnockbackAttributes.SelfKnockback;

            #region Primary

            MAPAttackEditor.ckFriendlyFire.Checked = LoadedAttack.MAPAttributes.FriendlyFire;
            MAPAttackEditor.txtAttackDelay.Value = LoadedAttack.MAPAttributes.Delay;

            switch (LoadedAttack.Pri)
            {
                case WeaponPrimaryProperty.ALL:
                    rbALL.Checked = true;
                    ALLAttackEditor.txtLevel.Value = LoadedAttack.ALLLevel;
                    break;
                case WeaponPrimaryProperty.Dash:
                    rbDASH.Checked = true;
                    DashAttackEditor.txtMaxDashReach.Value = LoadedAttack.DashMaxReach;
                    DashAttackEditor.txtEnemyKnockback.Value = LoadedAttack.KnockbackAttributes.EnemyKnockback;
                    DashAttackEditor.txtSelfKnockback.Value = LoadedAttack.KnockbackAttributes.SelfKnockback;
                    break;

                case WeaponPrimaryProperty.MAP:
                    rbMAP.Checked = true;
                    MAPAttackEditor.ListAttackchoice = LoadedAttack.MAPAttributes.ListChoice;
                    MAPAttackEditor.txtAttackWidth.Value = MAPAttackEditor.AttackWidth = LoadedAttack.MAPAttributes.Width;
                    MAPAttackEditor.txtAttackHeight.Value = MAPAttackEditor.AttackHeight = LoadedAttack.MAPAttributes.Height;

                    if (LoadedAttack.MAPAttributes.Property == WeaponMAPProperties.Spread)
                        MAPAttackEditor.rbSpread.Checked = true;
                    else if (LoadedAttack.MAPAttributes.Property == WeaponMAPProperties.Direction)
                        MAPAttackEditor.rbDirection.Checked = true;
                    else if (LoadedAttack.MAPAttributes.Property == WeaponMAPProperties.Targeted)
                        MAPAttackEditor.rbTargeted.Checked = true;

                    MAPAttackEditor.ckFriendlyFire.Checked = LoadedAttack.MAPAttributes.FriendlyFire;
                    MAPAttackEditor.txtAttackDelay.Value = LoadedAttack.MAPAttributes.Delay;
                    break;

                case WeaponPrimaryProperty.PER:
                    PERAttackEditor.txtProjectileSpeed.Value = (decimal)LoadedAttack.PERAttributes.ProjectileSpeed;
                    PERAttackEditor.cbAffectedByGravity.Checked = LoadedAttack.PERAttributes.AffectedByGravity;
                    PERAttackEditor.cbCanBeShotDown.Checked = LoadedAttack.PERAttributes.CanBeShotDown;
                    PERAttackEditor.cbHoming.Checked = LoadedAttack.PERAttributes.Homing;
                    PERAttackEditor.txtMaxLifetime.Value = LoadedAttack.PERAttributes.MaxLifetime;
                    PERAttackEditor.cbAttackType.SelectedIndex = (int)LoadedAttack.PERAttributes.AttackType;

                    PERAttackEditor.IsProjectileAnimated = LoadedAttack.PERAttributes.ProjectileAnimation.IsAnimated;
                    PERAttackEditor.txtProjectilePath.Text = LoadedAttack.PERAttributes.ProjectileAnimation.Path;
                    PERAttackEditor.txt3DModelPath.Text = LoadedAttack.PERAttributes.Projectile3DModelPath;

                    PERAttackEditor.txtNumberOfProjectiles.Value = LoadedAttack.PERAttributes.NumberOfProjectiles;
                    PERAttackEditor.txtLateralMaxSpread.Value = (decimal)LoadedAttack.PERAttributes.MaxLateralSpread;
                    PERAttackEditor.txtForwardMaxSpread.Value = (decimal)LoadedAttack.PERAttributes.MaxForwardSpread;
                    PERAttackEditor.txtUpwardMaxSpread.Value = (decimal)LoadedAttack.PERAttributes.MaxUpwardSpread;
                    PERAttackEditor.txtSkillChain.Text = LoadedAttack.PERAttributes.SkillChainName;

                    if (LoadedAttack.PERAttributes.GroundCollision == PERAttackAttributes.GroundCollisions.DestroySelf)
                    {
                        PERAttackEditor.rbDestroySelf.Checked = true;
                    }
                    else if (LoadedAttack.PERAttributes.GroundCollision == PERAttackAttributes.GroundCollisions.Stop)
                    {
                        PERAttackEditor.rbStop.Checked = true;
                    }
                    else if (LoadedAttack.PERAttributes.GroundCollision == PERAttackAttributes.GroundCollisions.Bounce)
                    {
                        PERAttackEditor.rbBounce.Checked = true;
                        PERAttackEditor.txtBounceLimit.Value = LoadedAttack.PERAttributes.BounceLimit;
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

            if ((LoadedAttack.Sec & WeaponSecondaryProperty.SwordCut) == WeaponSecondaryProperty.SwordCut)
                cbSwordCut.Checked = true;

            if ((LoadedAttack.Sec & WeaponSecondaryProperty.ShootDown) == WeaponSecondaryProperty.ShootDown)
                cbShootDown.Checked = true;

            if ((LoadedAttack.Sec & WeaponSecondaryProperty.Partial) == WeaponSecondaryProperty.Partial)
                cbPartialAttack.Checked = true;

            txtReMoveLevel.Value = LoadedAttack.ReMoveLevel;
            txtPostMovementLevel.Value = LoadedAttack.PostMovementLevel;
            txtPostMovementAccuracyMalus.Value = LoadedAttack.PostMovementAccuracyMalus;
            txtPostMovementEvasionBonus.Value = LoadedAttack.PostMovementEvasionBonus;

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
                    RankCell.Items.Add(ActiveRank.ToString());
                }

                MovementCell.Value = ActiveMovement.Name;

                if (LoadedAttack.DicRankByMovement.ContainsKey(M))
                {
                    RankCell.Value = Unit.ListRank[LoadedAttack.DicRankByMovement[M]].ToString();
                }

                dgvTerrainRanks.Rows[NewRowIndex].Cells[0] = MovementCell;
                dgvTerrainRanks.Rows[NewRowIndex].Cells[1] = RankCell;
            }

            #endregion

            AdvancedSettings.Init(LoadedAttack);
            DestructibleTilesEditor.Init(LoadedAttack);

            #region Quotes

            for (int Q = 0; Q < LoadedAttack.ListQuoteSet.Count; Q++)
                Editor.dgvQuotes.Rows.Add(LoadedAttack.ListQuoteSet[Q]);

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

        private void dgvTerrainRanks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            bool validClick = (e.RowIndex != -1 && e.ColumnIndex != -1); //Make sure the clicked row/column is valid.
            var datagridview = sender as DataGridView;

            // Check to make sure the cell clicked is the cell containing the combobox 
            if (datagridview.Rows[e.RowIndex].Cells[0] is DataGridViewComboBoxCell && validClick)
            {
                datagridview.BeginEdit(true);
                ((ComboBox)datagridview.EditingControl).DroppedDown = true;
            }
        }

        private void dgvTerrainRanks_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            dgvTerrainRanks.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvTerrainRanks_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            object o = dgvTerrainRanks.Rows[e.RowIndex].HeaderCell.Value;

            e.Graphics.DrawString(
                o != null ? o.ToString() : "",
                dgvTerrainRanks.Font,
                GridBrush,
                new PointF((float)e.RowBounds.Left + 2, (float)e.RowBounds.Top + 4));
        }

        private void ckUseRotation_CheckedChanged(object sender, EventArgs e)
        {
            btnConfigureRotation.Enabled = ckUseRotation.Checked;
        }

        private void btnConfigureRotation_Click(object sender, EventArgs e)
        {

        }

        private void tsmDestructibleTiles_Click(object sender, EventArgs e)
        {
            DestructibleTilesEditor frmDestructibleTilesEditor = new DestructibleTilesEditor();
            if (frmDestructibleTilesEditor.ShowDialog() == DialogResult.OK)
            {
            }
        }

        private void rbAttackTypeSpecial_CheckedChanged(object sender, EventArgs e)
        {
            txtSpecialAttack.Enabled = rbAttackTypeSpecial.Checked;
        }
    }
}
