using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class WeaponEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Trait, Action, Skill, Skin,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public WeaponEditor()
        {
            InitializeComponent();
        }

        public WeaponEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                cbHands.SelectedIndex = 0;
                cbWeaponType.SelectedIndex = 0;
                cbCategory.SelectedIndex = 0;
                cbGroup.SelectedIndex = 0;
                SaveItem(FilePath, FilePath);
            }

            LoadWeapon(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimWeapons }, "Life Sim/Weapons/", new string[] { ".pew" }, typeof(WeaponEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Weapon Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write((byte)lsTraits.Items.Count);
            for (int T = 0; T < lsTraits.Items.Count; ++T)
            {
                BW.Write(lsTraits.Items[T].ToString());
            }

            BW.Write(txtDamage.Text);
            BW.Write(txtBulk.Text);

            BW.Write((byte)cbHands.SelectedIndex);
            BW.Write((byte)cbWeaponType.SelectedIndex);
            BW.Write((byte)cbCategory.SelectedIndex);
            BW.Write((byte)cbGroup.SelectedIndex);

            BW.Write(txtRange.Text);
            BW.Write((byte)txtReload.Value);

            BW.Write((byte)lsActions.Items.Count);
            foreach (string ActiveAction in lsActions.Items)
            {
                BW.Write(ActiveAction);
            }

            FS.Close();
            BW.Close();
        }

        private void LoadWeapon(string WeaponPath)
        {
            LifeSimParams.Init();
            Name = WeaponPath.Substring(0, WeaponPath.Length - 4).Substring(25);

            Weapon LoadedWeapon = new Weapon(Name, null);

            this.Text = Name + " - Weapon Editor";

            txtName.Text = LoadedWeapon.Name;
            txtDescription.Text = LoadedWeapon.Description;

            foreach (string ActiveTrait in LoadedWeapon.ListTraitsRelativePath)
            {
                lsTraits.Items.Add(ActiveTrait);
            }

            txtDamage.Text = LoadedWeapon.Damage;
            txtBulk.Text = LoadedWeapon.Bulk;

            cbHands.SelectedIndex = (byte)LoadedWeapon.HandType;
            cbWeaponType.SelectedIndex = (byte)LoadedWeapon.WeaponType;
            cbCategory.SelectedIndex = (byte)LoadedWeapon.WeaponCategory;
            cbGroup.SelectedIndex = (byte)LoadedWeapon.WeaponGroup;

            txtRange.Text = LoadedWeapon.Range;
            txtReload.Value = LoadedWeapon.Reload;

            for (int A = 0; A < LoadedWeapon.ArrayCharacterActionPath.Length; ++A)
            {
                lsActions.Items.Add(LoadedWeapon.ArrayCharacterActionPath[A]);
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
