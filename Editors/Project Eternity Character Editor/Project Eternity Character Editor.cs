using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;
using System.Windows.Forms;
using ProjectEternity.Core.Units;
using System.Drawing;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class ProjectEternityCharacterEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Personality, SetBattleTheme, AceBonus, Slave, Skill1, Skill2, Skill3, Skill4, Skill5, Skill6, Spirit1, Spirit2, Spirit3, Spirit4, Spirit5, Spirit6 };

        private ItemSelectionChoices ItemSelectionChoice;

        private SolidBrush GridBrush;

        private CharacterQuotesEditor frmQuoteEditor;
        private CharacterStatsEditor frmStatsEditor;
        private DetailsEditor frmDetailsEditor;
        private SkillLevelsEditor[] ArraySkillLevelsEditor;
        private RelationshipEditor frmRelationshipEditor;

        public ProjectEternityCharacterEditor()
        {
            InitializeComponent();

            GridBrush = new SolidBrush(dgvTerrainRanks.RowHeadersDefaultCellStyle.ForeColor);
        }

        public ProjectEternityCharacterEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadCharacter(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { "Characters" }, "Deathmatch/Characters/", new string[] { ".pec" }, typeof(ProjectEternityCharacterEditor), true, null, true)
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Project Eternity Character Editor";

            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            if (frmDetailsEditor == null)
            {
                BW.Write("");
                BW.Write(0);
                BW.Write(0);
                BW.Write("");
            }
            else
            {
                BW.Write(frmDetailsEditor.txtPortrait.Text);
                BW.Write(frmDetailsEditor.lstBust.Items.Count);
                for(int B = 0; B < frmDetailsEditor.lstBust.Items.Count; ++B)
                {
                    BW.Write(frmDetailsEditor.lstBust.Items[B].ToString());
                }

                BW.Write(frmDetailsEditor.lstBox.Items.Count);
                for (int B = 0; B < frmDetailsEditor.lstBox.Items.Count; ++B)
                {
                    BW.Write(frmDetailsEditor.lstBox.Items[B].ToString());
                }

                BW.Write(frmDetailsEditor.txtTags.Text);
            }
            BW.Write((int)txtEXP.Value);
            BW.Write(cbCanPilot.Checked);
            BW.Write(txtBattleTheme.Text);
            BW.Write(txtAceBonus.Text);
            BW.Write(txtPersonality.Text);
            BW.Write(txtSlave.Text);

            #region Spirits

            int PilotSpiritCount = 0;
            if (txtPilotSpirit1.Text != "None")
                ++PilotSpiritCount;
            if (txtPilotSpirit2.Text != "None")
                ++PilotSpiritCount;
            if (txtPilotSpirit3.Text != "None")
                ++PilotSpiritCount;
            if (txtPilotSpirit4.Text != "None")
                ++PilotSpiritCount;
            if (txtPilotSpirit5.Text != "None")
                ++PilotSpiritCount;
            if (txtPilotSpirit6.Text != "None")
                ++PilotSpiritCount;

            BW.Write(PilotSpiritCount);

            if (txtPilotSpirit1.Text != "None")
            {
                BW.Write(txtPilotSpirit1.Text);
                BW.Write(Convert.ToInt32(txtPilotSpirit1SP.Text));
                BW.Write((int)txtPilotSpirit1LevelRequired.Value);
            }
            if (txtPilotSpirit2.Text != "None")
            {
                BW.Write(txtPilotSpirit2.Text);
                BW.Write(Convert.ToInt32(txtPilotSpirit2SP.Text));
                BW.Write((int)txtPilotSpirit2LevelRequired.Value);
            }
            if (txtPilotSpirit3.Text != "None")
            {
                BW.Write(txtPilotSpirit3.Text);
                BW.Write(Convert.ToInt32(txtPilotSpirit3SP.Text));
                BW.Write((int)txtPilotSpirit3LevelRequired.Value);
            }
            if (txtPilotSpirit4.Text != "None")
            {
                BW.Write(txtPilotSpirit4.Text);
                BW.Write(Convert.ToInt32(txtPilotSpirit4SP.Text));
                BW.Write((int)txtPilotSpirit4LevelRequired.Value);
            }
            if (txtPilotSpirit5.Text != "None")
            {
                BW.Write(txtPilotSpirit5.Text);
                BW.Write(Convert.ToInt32(txtPilotSpirit5SP.Text));
                BW.Write((int)txtPilotSpirit5LevelRequired.Value);
            }
            if (txtPilotSpirit6.Text != "None")
            {
                BW.Write(txtPilotSpirit6.Text);
                BW.Write(Convert.ToInt32(txtPilotSpirit6SP.Text));
                BW.Write((int)txtPilotSpirit6LevelRequired.Value);
            }

            #endregion

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
            if (txtPilotSkill5.Text != "None")
                ++PilotSkillCount;
            if (txtPilotSkill6.Text != "None")
                ++PilotSkillCount;

            BW.Write(PilotSkillCount);

            if (txtPilotSkill1.Text != "None")
            {
                BW.Write(txtPilotSkill1.Text);
                BW.Write(ckLockedSkill1.Checked);
                ArraySkillLevelsEditor[0].Save(BW);
            }
            if (txtPilotSkill2.Text != "None")
            {
                BW.Write(txtPilotSkill2.Text);
                BW.Write(ckLockedSkill2.Checked);
                ArraySkillLevelsEditor[1].Save(BW);
            }
            if (txtPilotSkill3.Text != "None")
            {
                BW.Write(txtPilotSkill3.Text);
                BW.Write(ckLockedSkill3.Checked);
                ArraySkillLevelsEditor[2].Save(BW);
            }
            if (txtPilotSkill4.Text != "None")
            {
                BW.Write(txtPilotSkill4.Text);
                BW.Write(ckLockedSkill4.Checked);
                ArraySkillLevelsEditor[3].Save(BW);
            }
            if (txtPilotSkill5.Text != "None")
            {
                BW.Write(txtPilotSkill5.Text);
                BW.Write(ckLockedSkill5.Checked);
                ArraySkillLevelsEditor[4].Save(BW);
            }
            if (txtPilotSkill6.Text != "None")
            {
                BW.Write(txtPilotSkill6.Text);
                BW.Write(ckLockedSkill6.Checked);
                ArraySkillLevelsEditor[5].Save(BW);
            }

            #endregion

            if (frmRelationshipEditor == null)
                BW.Write(0);
            else
                frmRelationshipEditor.Save(BW);

            //If it's a pilot, link its stats, else they won't be needed.
            if (cbCanPilot.Checked)
            {
                if (frmStatsEditor == null)
                {
                    BW.Write(50);

                    for (int S = 0; S < 50; ++S)
                    {
                        BW.Write(0); // MEL
                        BW.Write(0); // RNG
                        BW.Write(0); // DEF
                        BW.Write(0); // SKL
                        BW.Write(0); // EVA
                        BW.Write(0); // HIT
                        BW.Write(0); // SP
                    }
                }
                else
                {
                    BW.Write((int)frmStatsEditor.txtMaxLevel.Value);

                    for (int S = 0; S < (int)frmStatsEditor.txtMaxLevel.Value; ++S)
                    {
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[1].Value)); // MEL
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[2].Value)); // RNG
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[3].Value)); // DEF
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[4].Value)); // SKL
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[5].Value)); // EVA
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[6].Value)); // HIT
                        BW.Write(Convert.ToInt32(frmStatsEditor.dgvStats.Rows[S].Cells[7].Value)); // SP
                    }
                }

                List<Tuple<byte, byte>> ListRankByMovement = new List<Tuple<byte, byte>>();
                foreach (DataGridViewRow ActiveRow in dgvTerrainRanks.Rows)
                {
                    if (ActiveRow.Cells[1].Value != null)
                    {
                        ListRankByMovement.Add(new Tuple<byte, byte>((byte)UnitAndTerrainValues.Default.ListUnitMovement.IndexOf(UnitAndTerrainValues.Default.ListUnitMovement.Find(x => x.Name == ActiveRow.Cells[0].Value.ToString())),
                            (byte)Character.ListGrade.IndexOf(ActiveRow.Cells[1].Value.ToString()[0])));
                    }
                }

                BW.Write(ListRankByMovement.Count);
                foreach (Tuple<byte, byte> ActiveRankByMovement in ListRankByMovement)
                {
                    BW.Write(ActiveRankByMovement.Item1);
                    BW.Write(ActiveRankByMovement.Item2);
                }

                BW.Write((byte)txtPostMVLevel.Value);
                BW.Write((byte)txtReMoveLevel.Value);
                BW.Write((byte)txtChargeCancelLevel.Value);
            }

            if (frmQuoteEditor == null)
            {
                BW.Write(0);
            }
            else
            {
                BW.Write(frmQuoteEditor.lsVersusQuotes.Items.Count - 1);
                for (int Q = 1; Q < frmQuoteEditor.lsVersusQuotes.Items.Count; Q++)
                    BW.Write((string)frmQuoteEditor.lsVersusQuotes.Items[Q]);
            }
            Character.QuoteSet Quotes;
            //Base quotes
            for (int I = 0; I < 6; I++)
            {
                if (frmQuoteEditor == null)
                {
                    BW.Write(0);
                    BW.Write(0);
                    BW.Write(string.Empty);
                }
                else
                {
                    Quotes = (Character.QuoteSet)frmQuoteEditor.lvBaseQuotes.Items[I].Tag;

                    BW.Write(Quotes.ListQuote.Count);
                    for (int Q = 0; Q < Quotes.ListQuote.Count; Q++)
                        BW.Write(Quotes.ListQuote[Q]);

                    //Versus quotes.
                    BW.Write(Quotes.ListQuoteVersus.Count);
                    for (int Q = 0; Q < Quotes.ListQuoteVersus.Count; Q++)
                        BW.Write(Quotes.ListQuoteVersus[Q]);

                    BW.Write(Quotes.PortraitPath);
                }
            }

            //Attack Quotes.

            //Count the actual number of quotes listed.
            int QuoteCount = 0;

            if (frmQuoteEditor != null)
            {
                for (int R = 0; R < frmQuoteEditor.dgvQuoteSets.Rows.Count; R++)
                    if (!string.IsNullOrEmpty((string)frmQuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue))
                        QuoteCount++;
            }

            BW.Write(QuoteCount);

            if (frmQuoteEditor != null)
            {
                for (int R = 0; R < frmQuoteEditor.dgvQuoteSets.Rows.Count; R++)
                {
                    if (!string.IsNullOrEmpty((string)frmQuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue))
                    {
                        Quotes = (Character.QuoteSet)frmQuoteEditor.dgvQuoteSets.Rows[R].Tag;

                        BW.Write((string)frmQuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue);

                        BW.Write(Quotes.ListQuote.Count);
                        for (int Q = 0; Q < Quotes.ListQuote.Count; Q++)
                            BW.Write(Quotes.ListQuote[Q]);

                        //Versus quotes.
                        BW.Write(Quotes.ListQuoteVersus.Count);
                        for (int Q = 0; Q < Quotes.ListQuoteVersus.Count; Q++)
                            BW.Write(Quotes.ListQuoteVersus[Q]);

                        BW.Write(Quotes.PortraitPath);
                    }
                }
            }

            FS.Close();
            BW.Close();
        }

        private void LoadCharacter(string CharacterPath)
        {
            Name = CharacterPath.Substring(0, CharacterPath.Length - 4).Substring(CharacterPath.LastIndexOf("Characters") + 11);
            Character LoadedCharacter = new Character(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);
            frmQuoteEditor = new CharacterQuotesEditor();
            frmStatsEditor = new CharacterStatsEditor(LoadedCharacter);
            frmDetailsEditor = new DetailsEditor();

            frmDetailsEditor.txtPortrait.Text = LoadedCharacter.PortraitPath;
            for (int B = 0; B < LoadedCharacter.ArrayPortraitBustPath.Length; ++B)
            {
                frmDetailsEditor.lstBust.Items.Add(LoadedCharacter.ArrayPortraitBustPath[B]);
            }
            for (int B = 0; B < LoadedCharacter.ArrayPortraitBoxPath.Length; ++B)
            {
                frmDetailsEditor.lstBox.Items.Add(LoadedCharacter.ArrayPortraitBoxPath[B]);
            }
            frmDetailsEditor.txtTags.Text = LoadedCharacter.Tags;

            this.Text = LoadedCharacter.Name + " - Project Eternity Character Editor";

            //Update the editor's members.
            txtName.Text = LoadedCharacter.Name;
            txtEXP.Value = LoadedCharacter.EXPValue;
            cbCanPilot.Checked = LoadedCharacter.CanPilot;
            txtPersonality.Text = LoadedCharacter.Personality.Name;
            txtBattleTheme.Text = LoadedCharacter.BattleThemeName;
            if (LoadedCharacter.AceBonus != null)
                txtAceBonus.Text = LoadedCharacter.AceBonus.Name;
            if (LoadedCharacter.Slave != null)
                txtSlave.Text = LoadedCharacter.Slave.FullName;

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
                foreach (char ActiveRank in Character.ListGrade)
                {
                    RankCell.Items.Add(ActiveRank.ToString());
                }

                MovementCell.Value = ActiveMovement.Name;

                if (LoadedCharacter.DicRankByMovement.ContainsKey(M))
                {
                    RankCell.Value = Character.ListGrade[LoadedCharacter.DicRankByMovement[M]].ToString();
                }

                dgvTerrainRanks.Rows[NewRowIndex].Cells[0] = MovementCell;
                dgvTerrainRanks.Rows[NewRowIndex].Cells[1] = RankCell;
            }

            txtPostMVLevel.Value = 1;
            txtReMoveLevel.Value = 0;
            txtChargeCancelLevel.Value = LoadedCharacter.ChargedAttackCancelLevel;

            for (int S = 0; S < LoadedCharacter.ArrayPilotSpirit.Length; ++S)
            {
                switch (S)
                {
                    case 0:
                        txtPilotSpirit1.Text = LoadedCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit1SP.Text = LoadedCharacter.ArrayPilotSpirit[S].ActivationCost.ToString();
                        txtPilotSpirit1LevelRequired.Text = LoadedCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 1:
                        txtPilotSpirit2.Text = LoadedCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit2SP.Text = LoadedCharacter.ArrayPilotSpirit[S].ActivationCost.ToString();
                        txtPilotSpirit2LevelRequired.Text = LoadedCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 2:
                        txtPilotSpirit3.Text = LoadedCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit3SP.Text = LoadedCharacter.ArrayPilotSpirit[S].ActivationCost.ToString();
                        txtPilotSpirit3LevelRequired.Text = LoadedCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 3:
                        txtPilotSpirit4.Text = LoadedCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit4SP.Text = LoadedCharacter.ArrayPilotSpirit[S].ActivationCost.ToString();
                        txtPilotSpirit4LevelRequired.Text = LoadedCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 4:
                        txtPilotSpirit5.Text = LoadedCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit5SP.Text = LoadedCharacter.ArrayPilotSpirit[S].ActivationCost.ToString();
                        txtPilotSpirit5LevelRequired.Text = LoadedCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 5:
                        txtPilotSpirit6.Text = LoadedCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit6SP.Text = LoadedCharacter.ArrayPilotSpirit[S].ActivationCost.ToString();
                        txtPilotSpirit6LevelRequired.Text = LoadedCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;
                }
            }

            ArraySkillLevelsEditor = new SkillLevelsEditor[6];
            for (int S = 0; S < LoadedCharacter.ArrayPilotSkill.Length; ++S)
            {
                if (LoadedCharacter.ArrayPilotSkill[S] != null)
                {
                    ArraySkillLevelsEditor[S] = new SkillLevelsEditor(LoadedCharacter.ArrayPilotSkill[S].RelativePath, LoadedCharacter.ArrayPilotSkillLevels[S]);
                }

                switch (S)
                {
                    case 0:
                        txtPilotSkill1.Text = LoadedCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill1.Checked = LoadedCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels1.Enabled = LoadedCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 1:
                        txtPilotSkill2.Text = LoadedCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill2.Checked = LoadedCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels2.Enabled = LoadedCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 2:
                        txtPilotSkill3.Text = LoadedCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill3.Checked = LoadedCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels3.Enabled = LoadedCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 3:
                        txtPilotSkill4.Text = LoadedCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill4.Checked = LoadedCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels4.Enabled = LoadedCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 4:
                        txtPilotSkill5.Text = LoadedCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill5.Checked = LoadedCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels5.Enabled = LoadedCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 5:
                        txtPilotSkill6.Text = LoadedCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill6.Checked = LoadedCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels6.Enabled = LoadedCharacter.ArrayPilotSkill[S] != null;
                        break;
                }
            }

            frmRelationshipEditor = new RelationshipEditor(LoadedCharacter.ArrayRelationshipBonus);

            //Versus names
            for (int I = 0; I < LoadedCharacter.ListQuoteSetVersusName.Count; I++)
                frmQuoteEditor.lsVersusQuotes.Items.Add(LoadedCharacter.ListQuoteSetVersusName[I]);

            //Base quotes
            for (int I = 0; I < 6; I++)
                frmQuoteEditor.lvBaseQuotes.Items[I].Tag = LoadedCharacter.ArrayBaseQuoteSet[I];

            //Attack quotes
            for (int i = 0; i < LoadedCharacter.DicAttackQuoteSet.Count; i++)
            {
                KeyValuePair<string, Character.QuoteSet> NewQuoteSet = LoadedCharacter.DicAttackQuoteSet.ElementAt(i);
                frmQuoteEditor.dgvQuoteSets.Rows.Add(NewQuoteSet.Key);
                frmQuoteEditor.dgvQuoteSets.Rows[i].Tag = NewQuoteSet.Value;
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, txtName.Text);
        }

        private void tsmDetails_Click(object sender, EventArgs e)
        {
            frmDetailsEditor.ShowDialog();
        }

        private void tsmRelationships_Click(object sender, EventArgs e)
        {
            frmRelationshipEditor.ShowDialog();
        }

        private void cbCanPilot_CheckedChanged(object sender, EventArgs e)
        {
			//Enable/Disable pilots stats depending if it's a pilot or not.
            if (cbCanPilot.Checked)
            {
                gbPersonality.Enabled = true;
                gbTerrain.Enabled = true;
            }
            else
            {
                gbPersonality.Enabled = false;
                gbTerrain.Enabled = false;
            }
        }

        private void btnEditQuotes_Click(object sender, EventArgs e)
        {
            frmQuoteEditor.ShowDialog();
        }

        private void btnEditStats_Click(object sender, EventArgs e)
        {
            frmStatsEditor.ShowDialog();
        }

        private void btnPersonality_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Personality;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterPersonalities));
        }

        private void btnSetBattleTheme_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.SetBattleTheme;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapBGM));
        }

        private void btnAceBonus_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AceBonus;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnSelectSlave_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Slave;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacters));
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

        #region Skills

        private void btnSetSkill1_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill1;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnSetSkill2_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill2;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnSetSkill3_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill3;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnSetSkill4_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill4;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnSetSkill5_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill5;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnSetSkill6_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill6;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSkills));
        }

        private void btnEditLevels1_Click(object sender, EventArgs e)
        {
            if (ArraySkillLevelsEditor[0] != null)
            {
                ArraySkillLevelsEditor[0].ShowDialog();
            }
        }

        private void btnEditLevels2_Click(object sender, EventArgs e)
        {
            if (ArraySkillLevelsEditor[1] != null)
            {
                ArraySkillLevelsEditor[1].ShowDialog();
            }
        }

        private void btnEditLevels3_Click(object sender, EventArgs e)
        {
            if (ArraySkillLevelsEditor[2] != null)
            {
                ArraySkillLevelsEditor[2].ShowDialog();
            }
        }

        private void btnEditLevels4_Click(object sender, EventArgs e)
        {
            if (ArraySkillLevelsEditor[3] != null)
            {
                ArraySkillLevelsEditor[3].ShowDialog();
            }
        }

        private void btnEditLevels5_Click(object sender, EventArgs e)
        {
            if (ArraySkillLevelsEditor[4] != null)
            {
                ArraySkillLevelsEditor[4].ShowDialog();
            }
        }

        private void btnEditLevels6_Click(object sender, EventArgs e)
        {
            if (ArraySkillLevelsEditor[5] != null)
            {
                ArraySkillLevelsEditor[5].ShowDialog();
            }
        }

        #endregion

        #region Spirits

        private void btnSetSpirit1_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit1;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit2_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit2;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit3_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit3;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit4_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit4;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit5_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit5;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit6_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit6;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathCharacterSpirits));
        }

        #endregion

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            string Name;
            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Personality:
                        Name = Items[I].Substring(0, Items[0].Length - 5).Substring(33);
                        txtPersonality.Text = Name;
                        break;

                    case ItemSelectionChoices.SetBattleTheme:
                        Name = Items[I].Substring(17);
                        txtBattleTheme.Text = Name;
                        break;

                    case ItemSelectionChoices.AceBonus:
                        if (Items[I] == null)
                        {
                            txtAceBonus.Text = "None";
                        }
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(Items[0].LastIndexOf("Skills") + 7);
                            txtAceBonus.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Slave:
                        if (Items[I] == null)
                        {
                            txtSlave.Text = "None";
                        }
                        else
                        {
                            Name = Items[I].Substring(0, Items[0].Length - 4).Substring(19);
                            txtSlave.Text = Name;
                        }
                        break;

                    #region Skills

                    case ItemSelectionChoices.Skill1:
                        if (Items[I] == null)
                            txtPilotSkill1.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                            txtPilotSkill1.Text = Name;
                            ArraySkillLevelsEditor[0] = new SkillLevelsEditor(Name);
                        }
                        break;

                    case ItemSelectionChoices.Skill2:
                        if (Items[I] == null)
                            txtPilotSkill2.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                            txtPilotSkill2.Text = Name;
                            ArraySkillLevelsEditor[1] = new SkillLevelsEditor(Name);
                        }
                        break;

                    case ItemSelectionChoices.Skill3:
                        if (Items[I] == null)
                            txtPilotSkill3.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                            txtPilotSkill3.Text = Name;
                            ArraySkillLevelsEditor[2] = new SkillLevelsEditor(Name);
                        }
                        break;

                    case ItemSelectionChoices.Skill4:
                        if (Items[I] == null)
                            txtPilotSkill4.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                            txtPilotSkill4.Text = Name;
                            ArraySkillLevelsEditor[3] = new SkillLevelsEditor(Name);
                        }
                        break;

                    case ItemSelectionChoices.Skill5:
                        if (Items[I] == null)
                            txtPilotSkill5.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                            txtPilotSkill5.Text = Name;
                            ArraySkillLevelsEditor[4] = new SkillLevelsEditor(Name);
                        }
                        break;

                    case ItemSelectionChoices.Skill6:
                        if (Items[I] == null)
                            txtPilotSkill6.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(26);
                            txtPilotSkill6.Text = Name;
                            ArraySkillLevelsEditor[5] = new SkillLevelsEditor(Name);
                        }
                        break;

                    #endregion

                    #region Spirits

                    case ItemSelectionChoices.Spirit1:
                        if (Items[I] == null)
                            txtPilotSpirit1.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSpirit1.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Spirit2:
                        if (Items[I] == null)
                            txtPilotSpirit2.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSpirit2.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Spirit3:
                        if (Items[I] == null)
                            txtPilotSpirit3.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSpirit3.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Spirit4:
                        if (Items[I] == null)
                            txtPilotSpirit4.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSpirit4.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Spirit5:
                        if (Items[I] == null)
                            txtPilotSpirit5.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSpirit5.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Spirit6:
                        if (Items[I] == null)
                            txtPilotSpirit6.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSpirit6.Text = Name;
                        }
                        break;

                        #endregion
                }
            }
        }
    }
}
