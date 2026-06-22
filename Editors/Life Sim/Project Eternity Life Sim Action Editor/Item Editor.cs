using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class ItemEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Trait, Action, Skill, Skin, };

        private ItemSelectionChoices ItemSelectionChoice;

        public ItemEditor()
        {
            InitializeComponent();
        }

        public ItemEditor(string FilePath, object[] Params)
            : this()
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimItems }, "Life Sim/Items/", new string[] { ".pei" }, typeof(ItemEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Item Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)lsTraits.Items.Count);
            for (int T = 0; T < lsTraits.Items.Count; ++T)
            {
                BW.Write(lsTraits.Items[T].ToString());
            }

            BW.Write(txtBulk.Text);
            BW.Write((byte)0);

            BW.Write((byte)lsActions.Items.Count);
            foreach (string ActiveAction in lsActions.Items)
            {
                BW.Write(ActiveAction);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadCharacter(string ItemPath)
        {
            Name = ItemPath.Substring(0, ItemPath.Length - 4).Substring(23);

            Item LoadedItem = new Item(Name, null);

            this.Text = Name + " - Item Editor";

            txtName.Text = LoadedItem.Name;
            txtDescription.Text = LoadedItem.Description;

            foreach (string ActiveTrait in LoadedItem.ListTraitsRelativePath)
            {
                lsTraits.Items.Add(ActiveTrait);
            }

            txtBulk.Text = LoadedItem.Bulk;
            txtTags.Text = string.Join(", ", LoadedItem.ArrayTags);

            for (int A = 0; A < LoadedItem.ArrayCharacterActionPath.Length; ++A)
            {
                lsActions.Items.Add(LoadedItem.ArrayCharacterActionPath[A]);
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

        private void btnAddTrait_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Trait;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimTraits));
        }

        private void btnRemoveTrait_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsTraits);
        }

        private void btnAddAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Action;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnDeleteAction_Click(object sender, EventArgs e)
        {
            DeleteItemFromList(lsActions);
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
                    case ItemSelectionChoices.Trait:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(24);
                        lsTraits.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Action:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsActions.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Skill:
                        break;
                }
            }
        }
    }
}
