using ProjectEternity.Core.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectEternity.Editors.AttackEditor
{
    public partial class AdvancedSettingsEditor : Form
    {
        private enum ItemSelectionChoices { SecondaryAttack, ChargedAttack, Skill1, Skill2, Skill3, Skill4 };

        private ItemSelectionChoices ItemSelectionChoice;

        public AdvancedSettingsEditor()
        {
            InitializeComponent();
        }

        private void btnAddSecondaryAttack_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.SecondaryAttack;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttacks));
        }

        private void btnRemoveSecondaryAttack_Click(object sender, EventArgs e)
        {
            if (lvSecondaryAttack.SelectedItems.Count > 0)
            {
                lvSecondaryAttack.Items.Remove(lvSecondaryAttack.SelectedItems[0]);
            }
        }

        private void btnAddChargedAttack_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.ChargedAttack;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttacks));
        }

        private void btnRemoveChargedAttack_Click(object sender, EventArgs e)
        {
            if (lvChargedAttacks.SelectedItems.Count > 0)
            {
                lvChargedAttacks.Items.Remove(lvChargedAttacks.SelectedItems[0]);
            }
        }

        private void btnSetSkill1_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill1;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttackAttributes));
        }

        private void btnSetSkill2_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill2;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttackAttributes));
        }

        private void btnSetSkill3_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill3;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttackAttributes));
        }

        private void btnSetSkill4_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill4;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathAttackAttributes));
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
                    case ItemSelectionChoices.SecondaryAttack:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(16);
                        string[] ArraySecondaryAttackName = Name.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        ListViewItem NewSecondaryAttackItem = new ListViewItem(ArraySecondaryAttackName[ArraySecondaryAttackName.Length - 1]);
                        NewSecondaryAttackItem.Tag = Name;
                        lvSecondaryAttack.Items.Add(NewSecondaryAttackItem);
                        break;

                    case ItemSelectionChoices.ChargedAttack:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(16);
                        string[] ArrayChargedAttackName = Name.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        ListViewItem NewChargedAttackItem = new ListViewItem(ArrayChargedAttackName[ArrayChargedAttackName.Length - 1]);
                        NewChargedAttackItem.Tag = Name;
                        lvChargedAttacks.Items.Add(NewChargedAttackItem);
                        break;

                    case ItemSelectionChoices.Skill1:
                        if (Items[I] == null)
                            txtPilotSkill1.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill1.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Skill2:
                        if (Items[I] == null)
                            txtPilotSkill2.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill2.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Skill3:
                        if (Items[I] == null)
                            txtPilotSkill3.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill3.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Skill4:
                        if (Items[I] == null)
                            txtPilotSkill4.Text = "None";
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(27);
                            if (Name != null)
                                txtPilotSkill4.Text = Name;
                        }
                        break;
                }
            }
        }
    }
}
