using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using static ProjectEternity.Core.Attacks.DestructibleTilesAttackAttributes;
using ProjectEternity.Core.Attacks;

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
            BW.Write((byte)lsUniqueType.Items.Count);
        }

        public void Init(Attack loadedAttack)
        {
        }

        private void lsGenericType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AllowEvents = false;

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

            AllowEvents = true;
        }

        private void btnAddUniqueType_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Preset;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathMaDestroyableTilesPresets));
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
