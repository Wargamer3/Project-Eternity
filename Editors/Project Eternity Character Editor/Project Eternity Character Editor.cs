using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Characters;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class ProjectEternityCharacterEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Personality, SetBattleTheme, AceBonus, Slave, Skill1, Skill2, Skill3, Skill4, Skill5, Skill6, Spirit1, Spirit2, Spirit3, Spirit4, Spirit5, Spirit6 };

        private ItemSelectionChoices ItemSelectionChoice;

        private CharacterQuotesEditor QuoteEditor;
        private CharacterStatsEditor StatsEditor;
        private DetailsEditor frmDetailsEditor;
        private SkillLevelsEditor[] ArraySkillLevelsEditor;
        private RelationshipEditor frmRelationshipEditor;

        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;
        private Dictionary<string, AutomaticSkillTargetType> DicAutomaticSkillTarget;
        private Dictionary<string, ManualSkillTarget> DicManualSkillTarget;

        public ProjectEternityCharacterEditor()
        {
            InitializeComponent();

            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            DicAutomaticSkillTarget = AutomaticSkillTargetType.LoadAllTargetTypes();
            DicManualSkillTarget = ManualSkillTarget.LoadAllTargetTypes();
        }

        public ProjectEternityCharacterEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                cbAirRank.SelectedIndex = 0;
                cbLandRank.SelectedIndex = 0;
                cbSeaRank.SelectedIndex = 0;
                cbSpaceRank.SelectedIndex = 0;
                SaveItem(FilePath, FilePath);
            }

            LoadCharacter(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { "Characters" }, "Characters/", new string[] { ".pec" }, typeof(ProjectEternityCharacterEditor), true, null, true)
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
                if (StatsEditor == null)
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
                    BW.Write((int)StatsEditor.txtMaxLevel.Value);

                    for (int S = 0; S < (int)StatsEditor.txtMaxLevel.Value; ++S)
                    {
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[1].Value)); // MEL
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[2].Value)); // RNG
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[3].Value)); // DEF
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[4].Value)); // SKL
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[5].Value)); // EVA
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[6].Value)); // HIT
                        BW.Write(Convert.ToInt32(StatsEditor.dgvStats.Rows[S].Cells[7].Value)); // SP
                    }
                }

                BW.Write((byte)cbAirRank.SelectedIndex);
                BW.Write((byte)cbLandRank.SelectedIndex);
                BW.Write((byte)cbSeaRank.SelectedIndex);
                BW.Write((byte)cbSpaceRank.SelectedIndex);
                BW.Write((byte)txtPostMVLevel.Value);
                BW.Write((byte)txtReMoveLevel.Value);
                BW.Write((byte)txtChargeCancelLevel.Value);
            }

            if (QuoteEditor == null)
            {
                BW.Write(0);
            }
            else
            {
                BW.Write(QuoteEditor.lsVersusQuotes.Items.Count - 1);
                for (int Q = 1; Q < QuoteEditor.lsVersusQuotes.Items.Count; Q++)
                    BW.Write((string)QuoteEditor.lsVersusQuotes.Items[Q]);
            }
            Character.QuoteSet Quotes;
            //Base quotes
            for (int I = 0; I < 6; I++)
            {
                if (QuoteEditor == null)
                {
                    BW.Write(0);
                    BW.Write(0);
                    BW.Write(string.Empty);
                }
                else
                {
                    Quotes = (Character.QuoteSet)QuoteEditor.lvBaseQuotes.Items[I].Tag;

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

            if (QuoteEditor != null)
            {
                for (int R = 0; R < QuoteEditor.dgvQuoteSets.Rows.Count; R++)
                    if (!string.IsNullOrEmpty((string)QuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue))
                        QuoteCount++;
            }

            BW.Write(QuoteCount);

            if (QuoteEditor != null)
            {
                for (int R = 0; R < QuoteEditor.dgvQuoteSets.Rows.Count; R++)
                {
                    if (!string.IsNullOrEmpty((string)QuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue))
                    {
                        Quotes = (Character.QuoteSet)QuoteEditor.dgvQuoteSets.Rows[R].Tag;

                        BW.Write((string)QuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue);

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
            Character NewCharacter = new Character(Name, null, DicRequirement, DicEffect, DicAutomaticSkillTarget, DicManualSkillTarget);
            QuoteEditor = new CharacterQuotesEditor();
            StatsEditor = new CharacterStatsEditor(NewCharacter);
            frmDetailsEditor = new DetailsEditor();

            frmDetailsEditor.txtPortrait.Text = NewCharacter.PortraitPath;
            for (int B = 0; B < NewCharacter.ArrayPortraitBustPath.Length; ++B)
            {
                frmDetailsEditor.lstBust.Items.Add(NewCharacter.ArrayPortraitBustPath[B]);
            }
            for (int B = 0; B < NewCharacter.ArrayPortraitBoxPath.Length; ++B)
            {
                frmDetailsEditor.lstBox.Items.Add(NewCharacter.ArrayPortraitBoxPath[B]);
            }
            frmDetailsEditor.txtTags.Text = NewCharacter.Tags;

            this.Text = NewCharacter.Name + " - Project Eternity Character Editor";

            //Update the editor's members.
            txtName.Text = NewCharacter.Name;
            txtEXP.Value = NewCharacter.EXPValue;
            cbCanPilot.Checked = NewCharacter.CanPilot;
            txtPersonality.Text = NewCharacter.Personality.Name;
            txtBattleTheme.Text = NewCharacter.BattleThemeName;
            if (NewCharacter.AceBonus != null)
                txtAceBonus.Text = NewCharacter.AceBonus.Name;
            if (NewCharacter.Slave != null)
                txtSlave.Text = NewCharacter.Slave.FullName;

            List<char> ListGrade = new List<char> { 'S', 'A', 'B', 'C', 'D' };

            cbAirRank.SelectedIndex = ListGrade.IndexOf(NewCharacter.TerrainGrade.TerrainGradeAir);
            cbLandRank.SelectedIndex = ListGrade.IndexOf(NewCharacter.TerrainGrade.TerrainGradeLand);
            cbSeaRank.SelectedIndex = ListGrade.IndexOf(NewCharacter.TerrainGrade.TerrainGradeSea);
            cbSpaceRank.SelectedIndex = ListGrade.IndexOf(NewCharacter.TerrainGrade.TerrainGradeSpace);

            txtPostMVLevel.Value = 1;
            txtReMoveLevel.Value = 0;
            txtChargeCancelLevel.Value = NewCharacter.ChargedAttackCancelLevel;

            for (int S = 0; S < NewCharacter.ArrayPilotSpirit.Length; ++S)
            {
                switch (S)
                {
                    case 0:
                        txtPilotSpirit1.Text = NewCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit1SP.Text = NewCharacter.ArrayPilotSpirit[S].SPCost.ToString();
                        txtPilotSpirit1LevelRequired.Text = NewCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 1:
                        txtPilotSpirit2.Text = NewCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit2SP.Text = NewCharacter.ArrayPilotSpirit[S].SPCost.ToString();
                        txtPilotSpirit2LevelRequired.Text = NewCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 2:
                        txtPilotSpirit3.Text = NewCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit3SP.Text = NewCharacter.ArrayPilotSpirit[S].SPCost.ToString();
                        txtPilotSpirit3LevelRequired.Text = NewCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 3:
                        txtPilotSpirit4.Text = NewCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit4SP.Text = NewCharacter.ArrayPilotSpirit[S].SPCost.ToString();
                        txtPilotSpirit4LevelRequired.Text = NewCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 4:
                        txtPilotSpirit5.Text = NewCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit5SP.Text = NewCharacter.ArrayPilotSpirit[S].SPCost.ToString();
                        txtPilotSpirit5LevelRequired.Text = NewCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;

                    case 5:
                        txtPilotSpirit6.Text = NewCharacter.ArrayPilotSpirit[S].FullName;
                        txtPilotSpirit6SP.Text = NewCharacter.ArrayPilotSpirit[S].SPCost.ToString();
                        txtPilotSpirit6LevelRequired.Text = NewCharacter.ArrayPilotSpirit[S].LevelRequirement.ToString();
                        break;
                }
            }

            ArraySkillLevelsEditor = new SkillLevelsEditor[6];
            for (int S = 0; S < NewCharacter.ArrayPilotSkill.Length; ++S)
            {
                if (NewCharacter.ArrayPilotSkill[S] != null)
                {
                    ArraySkillLevelsEditor[S] = new SkillLevelsEditor(NewCharacter.ArrayPilotSkill[S].RelativePath, NewCharacter.ArrayPilotSkillLevels[S]);
                }

                switch (S)
                {
                    case 0:
                        txtPilotSkill1.Text = NewCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill1.Checked = NewCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels1.Enabled = NewCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 1:
                        txtPilotSkill2.Text = NewCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill2.Checked = NewCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels2.Enabled = NewCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 2:
                        txtPilotSkill3.Text = NewCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill3.Checked = NewCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels3.Enabled = NewCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 3:
                        txtPilotSkill4.Text = NewCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill4.Checked = NewCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels4.Enabled = NewCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 4:
                        txtPilotSkill5.Text = NewCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill5.Checked = NewCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels5.Enabled = NewCharacter.ArrayPilotSkill[S] != null;
                        break;

                    case 5:
                        txtPilotSkill6.Text = NewCharacter.ArrayPilotSkill[S].RelativePath;
                        ckLockedSkill6.Checked = NewCharacter.ArrayPilotSkillLocked[S];
                        btnEditLevels6.Enabled = NewCharacter.ArrayPilotSkill[S] != null;
                        break;
                }
            }

            frmRelationshipEditor = new RelationshipEditor(NewCharacter.ArrayRelationshipBonus);

            //Versus names
            for (int I = 0; I < NewCharacter.ListQuoteSetVersusName.Count; I++)
                QuoteEditor.lsVersusQuotes.Items.Add(NewCharacter.ListQuoteSetVersusName[I]);

            //Base quotes
            for (int I = 0; I < 6; I++)
                QuoteEditor.lvBaseQuotes.Items[I].Tag = NewCharacter.ArrayBaseQuoteSet[I];

            //Attack quotes
            for (int i = 0; i < NewCharacter.DicAttackQuoteSet.Count; i++)
            {
                KeyValuePair<string, Character.QuoteSet> NewQuoteSet = NewCharacter.DicAttackQuoteSet.ElementAt(i);
                QuoteEditor.dgvQuoteSets.Rows.Add(NewQuoteSet.Key);
                QuoteEditor.dgvQuoteSets.Rows[i].Tag = NewQuoteSet.Value;
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
                cbAirRank.Enabled = true;
                cbLandRank.Enabled = true;
                cbSeaRank.Enabled = true;
                cbSpaceRank.Enabled = true;
            }
            else
            {
                gbPersonality.Enabled = false;
                cbAirRank.Enabled = false;
                cbLandRank.Enabled = false;
                cbSeaRank.Enabled = false;
                cbSpaceRank.Enabled = false;
            }
        }

        private void btnEditQuotes_Click(object sender, EventArgs e)
        {
            QuoteEditor.ShowDialog();
        }

        private void btnEditStats_Click(object sender, EventArgs e)
        {
            StatsEditor.ShowDialog();
        }

        private void btnPersonality_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Personality;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterPersonalities));
        }

        private void btnSetBattleTheme_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.SetBattleTheme;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathMapBGM));
        }

        private void btnAceBonus_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.AceBonus;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnSelectSlave_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Slave;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
        }

        #region Skills

        private void btnSetSkill1_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill1;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnSetSkill2_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill2;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnSetSkill3_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill3;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnSetSkill4_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill4;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnSetSkill5_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill5;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnSetSkill6_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill6;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
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
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit2_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit2;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit3_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit3;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit4_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit4;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit5_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit5;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
        }

        private void btnSetSpirit6_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Spirit6;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSpirits));
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
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(17);
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
