using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;
using System.Windows.Forms;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class AncestryEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Trait, Language, };

        private ItemSelectionChoices ItemSelectionChoice;
        private bool AllowEvents;

        public AncestryEditor()
        {
            InitializeComponent();

            AllowEvents = true;
        }

        public AncestryEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadAncestry(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimAncestries }, "Life Sim/Ancestries/", new string[] { ".pea" }, typeof(AncestryEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Ancestry Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);
            BW.Write((byte)txtBaseHP.Value);
            BW.Write((byte)txtSpeed.Value);
            BW.Write((byte)cbSize.SelectedIndex);

            BW.Write((byte)lsTraits.Items.Count);
            foreach (string ActiveTrait in lsTraits.Items)
            {
                BW.Write(ActiveTrait);
            }

            BW.Write((byte)lsLanguages.Items.Count);
            foreach (string ActiveLanguage in lsLanguages.Items)
            {
                BW.Write(ActiveLanguage);
            }

            BW.Write((byte)lsUnlockables.Items.Count);
            foreach (Unlockable ActiveUnlockable in lsUnlockables.Items)
            {
                ActiveUnlockable.Write(BW);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadAncestry(string AncestryPath)
        {
            LifeSimParams.Init();
            foreach (UnlcokableItemType ActiveUnlockableType in LifeSimParams.DicUnlockableItemTypeByName.Values)
            {
                cbUnlockableType.Items.Add(ActiveUnlockableType);
            }
            foreach (UnlockRequirementEvaluator ActiveRequirement in LifeSimParams.DicRequirementByName.Values)
            {
                cbRequirementType.Items.Add(ActiveRequirement);
            }

            Name = AncestryPath.Substring(0, AncestryPath.Length - 4).Substring(28);

            CharacterAncestry LoadedAncestry = new CharacterAncestry(Name, null);

            this.Text = Name + " - Ancestry Editor";

            txtName.Text = LoadedAncestry.Name;
            txtDescription.Text = LoadedAncestry.Description;

            txtBaseHP.Value = LoadedAncestry.BaseHP;
            txtSpeed.Value = LoadedAncestry.Speed;
            cbSize.SelectedIndex = (byte)LoadedAncestry.Size;

            foreach (string ActiveTrait in LoadedAncestry.ListTraitsRelativePath)
            {
                lsTraits.Items.Add(ActiveTrait);
            }

            foreach (string ActiveLanguage in LoadedAncestry.ListLanguageRelativePath)
            {
                lsLanguages.Items.Add(ActiveLanguage);
            }
            
            foreach (Unlockable ActiveUnlockable in LoadedAncestry.ListUnlockable)
            {
                lsUnlockables.Items.Add(ActiveUnlockable);
            }

            if (lsUnlockables.Items.Count > 0)
            {
                lsUnlockables.SelectedIndex = 0;
            }

            cbUnlockableType.Enabled = lsUnlockables.Items.Count > 0;
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

        #region Lists

        private void btnAddTrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Trait;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimTraits));
        }

        private void btnRemoveTrait_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsTraits);
        }

        private void btnAddLanguage_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Language;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimLanguages));
        }

        private void btnRemoveLanguage_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsLanguages);
        }

        private void btnAddUnlockable_Click(object sender, EventArgs e)
        {
            Unlockable NewUnlockable = new Unlockable();
            NewUnlockable.ItemToUnlock = (UnlcokableItemType)cbUnlockableType.Items[0];
            lsUnlockables.Items.Add(NewUnlockable);
            lsUnlockables.SelectedIndex = lsUnlockables.Items.Count - 1;

            cbUnlockableType.Enabled = true;
        }

        private void btnDeleteUnlockable_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsUnlockables);

            cbUnlockableType.Enabled = lsUnlockables.Items.Count > 0;
            cbUnlockableType.Text = string.Empty;
            cbUnlockableType.SelectedIndex = -1;
        }

        private void lsUnlockables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lsUnlockables.SelectedItem != null)
            {
                AllowEvents = false;

                Unlockable SelectedUnlockable = (Unlockable)lsUnlockables.SelectedItem;

                cbUnlockableType.Text = SelectedUnlockable.ItemToUnlock.ToString();
                pgUnlockable.SelectedObject = SelectedUnlockable.ItemToUnlock;

                gbUnlockableRequirements.Enabled = true;

                lsUnlockRequirements.Items.Clear();

                foreach (UnlockRequirementEvaluator ActiveRequirement in ((Unlockable)lsUnlockables.SelectedItem).ListUnlockRequirement)
                {
                    lsUnlockRequirements.Items.Add(ActiveRequirement);
                }

                AllowEvents = true;

                if (lsUnlockRequirements.Items.Count > 0)
                {
                    lsUnlockRequirements.SelectedIndex = 0;
                }
            }
            else
            {
                cbUnlockableType.Enabled = false;
                pgUnlockable.SelectedObject = null;
                gbUnlockableRequirements.Enabled = false;

                lsUnlockRequirements.Items.Clear();
                pgRequirement.SelectedObject = null;
                cbRequirementType.Enabled = false;
                cbRequirementType.SelectedIndex = -1;
            }
        }

        private void cbUnlockableType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lsUnlockables.SelectedItem != null)
            {
                AllowEvents = false;

                Unlockable SelectedUnlockable = (Unlockable)lsUnlockables.SelectedItem;

                SelectedUnlockable.ItemToUnlock = (UnlcokableItemType)cbUnlockableType.SelectedItem;
                lsUnlockables.Items[lsUnlockables.SelectedIndex] = lsUnlockables.Items[lsUnlockables.SelectedIndex];
                pgUnlockable.SelectedObject = SelectedUnlockable.ItemToUnlock;

                AllowEvents = true;
            }
        }

        private void lsUnlockRequirements_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lsUnlockRequirements.SelectedItem != null)
            {
                AllowEvents = false;

                Unlockable SelectedUnlockable = (Unlockable)lsUnlockables.SelectedItem;

                cbRequirementType.Text = SelectedUnlockable.ListUnlockRequirement[lsUnlockRequirements.SelectedIndex].ToString();
                pgRequirement.SelectedObject = SelectedUnlockable.ListUnlockRequirement[lsUnlockRequirements.SelectedIndex];

                AllowEvents = true;
            }
            else
            {
                pgRequirement.SelectedObject = null;
                cbRequirementType.Enabled = false;
            }
        }

        private void btnAddUnlockRequirement_Click(object sender, EventArgs e)
        {
            if (lsUnlockables.SelectedItem != null)
            {
                UnlockRequirementEvaluator NewRequirement = (UnlockRequirementEvaluator)cbRequirementType.Items[0];

                Unlockable SelectedUnlockable = (Unlockable)lsUnlockables.SelectedItem;

                SelectedUnlockable.ListUnlockRequirement.Add(NewRequirement);
                lsUnlockRequirements.Items.Add(NewRequirement);
                lsUnlockRequirements.SelectedIndex = lsUnlockRequirements.Items.Count - 1;

                cbRequirementType.Enabled = true;
            }
        }

        private void btnDeleteUnlockRequirement_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsUnlockRequirements);

            cbRequirementType.Enabled = lsUnlockRequirements.Items.Count > 0;
            cbRequirementType.Text = string.Empty;
            cbRequirementType.SelectedIndex = -1;
        }

        private void cbRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            if (lsUnlockRequirements.SelectedItem != null)
            {
                AllowEvents = false;

                Unlockable SelectedUnlockable = (Unlockable)lsUnlockables.SelectedItem;

                SelectedUnlockable.ListUnlockRequirement[lsUnlockRequirements.SelectedIndex] = (UnlockRequirementEvaluator)cbRequirementType.SelectedItem;
                lsUnlockRequirements.Items[lsUnlockRequirements.SelectedIndex] = lsUnlockRequirements.Items[lsUnlockRequirements.SelectedIndex];
                pgRequirement.SelectedObject = SelectedUnlockable.ListUnlockRequirement[lsUnlockRequirements.SelectedIndex];

                AllowEvents = true;
            }
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
                    case ItemSelectionChoices.Trait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(24);
                        lsTraits.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Language:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                        lsLanguages.Items.Add(Name);
                        break;
                }
            }
        }
    }
}
