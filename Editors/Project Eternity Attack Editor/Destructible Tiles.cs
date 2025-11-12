using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Attacks;
using static ProjectEternity.Core.Attacks.DestructibleTilesAttackAttributes;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class DestructibleTilesEditor : Form
    {
        private enum ItemSelectionChoices { Preset };

        private ItemSelectionChoices ItemSelectionChoice;

        private bool AllowEvents;

        public DestructibleTilesEditor()
        {
            InitializeComponent();

            AllowEvents = true;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write((byte)lsGenericType.Items.Count);
            foreach (GenericAttribute ActiveType in lsGenericType.Items)
            {
                BW.Write((byte)ActiveType.TypeIndex);
                BW.Write(ActiveType.Damage);
            }
            BW.Write((byte)lsUniqueType.Items.Count);
            foreach (UniqueAttribute ActiveType in lsUniqueType.Items)
            {
                BW.Write(ActiveType.TypeName);
                BW.Write(ActiveType.Damage);
            }
        }

        public void Init(Attack LoadedAttack)
        {
            foreach (GenericAttribute ActiveType in LoadedAttack.DestructibleTilesAttributes.ListGenericAttribute)
            {
                lsGenericType.Items.Add(ActiveType);
            }
            foreach (UniqueAttribute ActiveType in LoadedAttack.DestructibleTilesAttributes.ListUniqueAttribute)
            {
                lsUniqueType.Items.Add(ActiveType);
            }
        }

        private void lsGenericType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;

            if (lsGenericType.SelectedIndex >= 0)
            {
                lsUniqueType.SelectedIndex = -1;
                txtDamage.Value = ((GenericAttribute)lsGenericType.SelectedItem).Damage;
            }

            AllowEvents = true;
        }

        private void btnAddGenericType_Click(object sender, EventArgs e)
        {
            DestructibleTileTypePicker frmPicker = new DestructibleTileTypePicker();
            if (frmPicker.ShowDialog() == DialogResult.OK)
            {
                DestructibleTypes TypeSelected = (DestructibleTypes)frmPicker.cbType.SelectedIndex;
                lsGenericType.Items.Add(new GenericAttribute(TypeSelected));
            }
        }

        private void btnRemoveGenericType_Click(object sender, EventArgs e)
        {
            if (lsGenericType.SelectedIndex >= 0)
            {
                lsGenericType.Items.RemoveAt(lsGenericType.SelectedIndex);
            }
        }

        private void lsUniqueType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;

            if (lsUniqueType.SelectedIndex >= 0)
            {
                lsGenericType.SelectedIndex = -1;
                txtDamage.Value = ((UniqueAttribute)lsUniqueType.SelectedItem).Damage;
            }

            AllowEvents = true;
        }

        private void btnAddUniqueType_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Preset;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapDestroyableTilesPresets));
        }

        private void btnRemoveUniqueType_Click(object sender, EventArgs e)
        {
            if (lsUniqueType.SelectedIndex >= 0)
            {
                lsUniqueType.Items.RemoveAt(lsUniqueType.SelectedIndex);
            }
        }

        private void txtDamage_ValueChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
            {
                return;
            }

            if (lsGenericType.SelectedIndex >= 0)
            {
                GenericAttribute SelectedType = (GenericAttribute)lsGenericType.SelectedItem;
                SelectedType.Damage = (int)txtDamage.Value;
            }
            else if (lsUniqueType.SelectedIndex >= 0)
            {
                UniqueAttribute SelectedType = (UniqueAttribute)lsGenericType.SelectedItem;
                SelectedType.Damage = (int)txtDamage.Value;
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                switch (ItemSelectionChoice)
                {
                    case ItemSelectionChoices.Preset:
                        string Name = Items[I].Substring(0, Items[I].Length - 5).Substring(39);
                        lsUniqueType.Items.Add(new UniqueAttribute(Name));
                        break;
                }
            }
        }
    }
}
