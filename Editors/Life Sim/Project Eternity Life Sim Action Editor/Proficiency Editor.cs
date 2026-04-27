using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class ProficiencyEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Base, Trained, Expert, Master, Legendary,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public ProficiencyEditor()
        {
            InitializeComponent();
        }

        public ProficiencyEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadProficiency(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimProficiencies }, "Life Sim/Proficiencies/", new string[] { ".pep" }, typeof(ProficiencyEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Proficiency Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)txtBaseValue.Value);
            BW.Write(txtDiceValue.Text);

            List<PlayerCharacter.CharacterStats> ListStatModifier = new List<PlayerCharacter.CharacterStats>();

            if (ckSTR.Checked)
            {
                ListStatModifier.Add(PlayerCharacter.CharacterStats.STR);
            }
            if (ckDEX.Checked)
            {
                ListStatModifier.Add(PlayerCharacter.CharacterStats.DEX);
            }
            if (ckCON.Checked)
            {
                ListStatModifier.Add(PlayerCharacter.CharacterStats.CON);
            }
            if (ckINT.Checked)
            {
                ListStatModifier.Add(PlayerCharacter.CharacterStats.INT);
            }
            if (ckWIS.Checked)
            {
                ListStatModifier.Add(PlayerCharacter.CharacterStats.WIS);
            }
            if (ckCHA.Checked)
            {
                ListStatModifier.Add(PlayerCharacter.CharacterStats.CHA);
            }

            BW.Write((byte)ListStatModifier.Count);
            foreach (PlayerCharacter.CharacterStats ActiveStat in ListStatModifier)
            {
                BW.Write((byte)ActiveStat);
            }

            BW.Write((byte)lsBaseActions.Items.Count);//Normal
            foreach (string ActiveActionPath in lsBaseActions.Items)
            {
                BW.Write(ActiveActionPath);
            }
            BW.Write((byte)lsTrainedActions.Items.Count);//Trained
            foreach (string ActiveActionPath in lsTrainedActions.Items)
            {
                BW.Write(ActiveActionPath);
            }
            BW.Write((byte)lsExpertActions.Items.Count);//Expert
            foreach (string ActiveActionPath in lsExpertActions.Items)
            {
                BW.Write(ActiveActionPath);
            }
            BW.Write((byte)lsMasterActions.Items.Count);//Master
            foreach (string ActiveActionPath in lsMasterActions.Items)
            {
                BW.Write(ActiveActionPath);
            }
            BW.Write((byte)lsLegendaryActions.Items.Count);//Legendary
            foreach (string ActiveActionPath in lsLegendaryActions.Items)
            {
                BW.Write(ActiveActionPath);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadProficiency(string ProficiencyPath)
        {
            Name = ProficiencyPath.Substring(0, ProficiencyPath.Length - 4).Substring(31);

            Proficiency LoadedProficiency = new Proficiency(Name);

            this.Text = Name + " - Proficiency Editor";

            txtName.Text = LoadedProficiency.Name;
            txtDescription.Text = LoadedProficiency.Description;
            txtBaseValue.Value = LoadedProficiency.BaseValue;
            txtDiceValue.Text = LoadedProficiency.DiceText;

            foreach (PlayerCharacter.CharacterStats ActiveStat in LoadedProficiency.ListStatModifier)
            {
                if (ActiveStat == PlayerCharacter.CharacterStats.STR)
                {
                    ckSTR.Checked = true;
                }
                else if (ActiveStat == PlayerCharacter.CharacterStats.DEX)
                {
                    ckDEX.Checked = true;
                }
                else if (ActiveStat == PlayerCharacter.CharacterStats.CON)
                {
                    ckCON.Checked = true;
                }
                else if (ActiveStat == PlayerCharacter.CharacterStats.INT)
                {
                    ckINT.Checked = true;
                }
                else if (ActiveStat == PlayerCharacter.CharacterStats.WIS)
                {
                    ckWIS.Checked = true;
                }
                else if (ActiveStat == PlayerCharacter.CharacterStats.CHA)
                {
                    ckCHA.Checked = true;
                }
            }

            foreach (string ActiveActionPath in LoadedProficiency.ListBaseActionRelativePath)
            {
                lsBaseActions.Items.Add(ActiveActionPath);
            }

            foreach (string ActiveActionPath in LoadedProficiency.ListTrainedActionRelativePath)
            {
                lsTrainedActions.Items.Add(ActiveActionPath);
            }

            foreach (string ActiveActionPath in LoadedProficiency.ListExpertActionRelativePath)
            {
                lsExpertActions.Items.Add(ActiveActionPath);
            }

            foreach (string ActiveActionPath in LoadedProficiency.ListMasterActionRelativePath)
            {
                lsMasterActions.Items.Add(ActiveActionPath);
            }

            foreach (string ActiveActionPath in LoadedProficiency.ListLegendaryActionRelativePath)
            {
                lsLegendaryActions.Items.Add(ActiveActionPath);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }

        private static void DeleteItemFromList(ListBox ActiveListBox)
        {
            if (ActiveListBox.SelectedIndex >= 0)
            {
                int LastIndex = ActiveListBox.SelectedIndex;

                ActiveListBox.Items.RemoveAt(LastIndex);

                if (ActiveListBox.Items.Count > 0)
                {
                    if (ActiveListBox.Items.Count >= LastIndex)
                    {
                        ActiveListBox.SelectedIndex = ActiveListBox.Items.Count - 1;
                    }
                    else
                    {
                        ActiveListBox.SelectedIndex = LastIndex;
                    }
                }
            }
        }

        #region Buttons

        private void btnAddBaseAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Base;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnRemoveBaseAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsBaseActions);
        }

        private void btnAddTrainedAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Trained;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnRemoveTrainedAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsTrainedActions);
        }

        private void btnAddExpertAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Expert;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnRemoveExpertAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsExpertActions);
        }

        private void btnAddMasterAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Master;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnRemoveMasterAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsMasterActions);
        }

        private void btnAddLegendaryAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Legendary;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnRemoveLegendaryAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsLegendaryActions);
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
                    case ItemSelectionChoices.Base:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsBaseActions.Items.Add(Name);
                        lsBaseActions.SelectedIndex = lsBaseActions.Items.Count - 1;
                        break;

                    case ItemSelectionChoices.Trained:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsTrainedActions.Items.Add(Name);
                        lsTrainedActions.SelectedIndex = lsTrainedActions.Items.Count - 1;
                        break;

                    case ItemSelectionChoices.Expert:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsExpertActions.Items.Add(Name);
                        lsExpertActions.SelectedIndex = lsExpertActions.Items.Count - 1;
                        break;

                    case ItemSelectionChoices.Master:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsMasterActions.Items.Add(Name);
                        lsMasterActions.SelectedIndex = lsMasterActions.Items.Count - 1;
                        break;

                    case ItemSelectionChoices.Legendary:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsLegendaryActions.Items.Add(Name);
                        lsLegendaryActions.SelectedIndex = lsLegendaryActions.Items.Count - 1;
                        break;
                }
            }
        }
    }
}
