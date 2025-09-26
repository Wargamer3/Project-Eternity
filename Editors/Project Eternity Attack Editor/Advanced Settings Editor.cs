using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Attacks;

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

        public void Save(BinaryWriter BW)
        {
            BW.Write(lvSecondaryAttack.Items.Count);
            for (int S = 0; S < lvSecondaryAttack.Items.Count; S++)
            {
                BW.Write(lvSecondaryAttack.Items[S].Tag.ToString());
            }

            BW.Write(lvChargedAttacks.Items.Count);
            for (int S = 0; S < lvChargedAttacks.Items.Count; S++)
            {
                BW.Write(lvChargedAttacks.Items[S].Tag.ToString());
            }
            if (lvChargedAttacks.Items.Count > 0)
            {
                BW.Write((byte)txtChargeCancelLevel.Value);
            }

            BW.Write((float)txtExplosionRadius.Value);
            if (txtExplosionRadius.Value > 0)
            {
                BW.Write((float)txtExplosionWindPowerAtCenter.Value);
                BW.Write((float)txtExplosionWindPowerAtEdge.Value);
                BW.Write((float)txtExplosionWindPowerToSelfMultiplier.Value);
                BW.Write((float)txtExplosionDamageAtCenter.Value);
                BW.Write((float)txtExplosionDamageAtEdge.Value);
                BW.Write((float)txtExplosionDamageToSelfMultiplier.Value);
            }

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

            BW.Write(PilotSkillCount);

            if (txtPilotSkill1.Text != "None")
                BW.Write(txtPilotSkill1.Text);
            if (txtPilotSkill2.Text != "None")
                BW.Write(txtPilotSkill2.Text);
            if (txtPilotSkill3.Text != "None")
                BW.Write(txtPilotSkill3.Text);
            if (txtPilotSkill4.Text != "None")
                BW.Write(txtPilotSkill4.Text);

            #endregion
        }

        public void Init(Attack LoadedAttack)
        {
            #region Special Attacks

            foreach (Attack ActiveAttack in LoadedAttack.ListSecondaryAttack)
            {
                string AttackName = ActiveAttack.RelativePath;
                string[] ArrayName = AttackName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                ListViewItem NewItem = new ListViewItem(ArrayName[ArrayName.Length - 1]);
                NewItem.Tag = AttackName;
                lvSecondaryAttack.Items.Add(NewItem);
            }

            #endregion

            #region Charged Attacks

            foreach (Attack ActiveAttack in LoadedAttack.ListChargedAttack)
            {
                string AttackName = ActiveAttack.RelativePath;
                string[] ArrayName = AttackName.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                ListViewItem NewItem = new ListViewItem(ArrayName[ArrayName.Length - 1]);
                NewItem.Tag = AttackName;
                lvChargedAttacks.Items.Add(NewItem);
            }

            #endregion

            #region Explosion Attributes

            txtExplosionRadius.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionRadius;
            txtExplosionWindPowerAtCenter.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionWindPowerAtCenter;
            txtExplosionWindPowerAtEdge.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionWindPowerAtEdge;
            txtExplosionWindPowerToSelfMultiplier.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionWindPowerToSelfMultiplier;
            txtExplosionDamageAtCenter.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionDamageAtCenter;
            txtExplosionDamageAtEdge.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionDamageAtEdge;
            txtExplosionDamageToSelfMultiplier.Value = (decimal)LoadedAttack.ExplosionOption.ExplosionDamageToSelfMultiplier;

            #endregion

            #region Attack Attributes

            if (LoadedAttack.ArrayAttackAttributes.Length >= 1)
            {
                txtPilotSkill1.Text = LoadedAttack.ArrayAttackAttributes[0].Name;
            }
            if (LoadedAttack.ArrayAttackAttributes.Length >= 2)
            {
                txtPilotSkill2.Text = LoadedAttack.ArrayAttackAttributes[1].Name;
            }
            if (LoadedAttack.ArrayAttackAttributes.Length >= 3)
            {
                txtPilotSkill3.Text = LoadedAttack.ArrayAttackAttributes[2].Name;
            }
            if (LoadedAttack.ArrayAttackAttributes.Length >= 4)
            {
                txtPilotSkill4.Text = LoadedAttack.ArrayAttackAttributes[3].Name;
            }

            #endregion
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
