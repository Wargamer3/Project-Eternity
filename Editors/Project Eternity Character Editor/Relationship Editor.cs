using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.Editors.CharacterEditor
{
    public partial class RelationshipEditor : Form
    {
        private enum ItemSelectionChoices { Bonus };

        private ItemSelectionChoices ItemSelectionChoice;

        private bool AllowEvents;
        private Dictionary<string, BaseSkillRequirement> DicRequirement;
        private Dictionary<string, BaseEffect> DicEffect;

        private List<BaseAutomaticSkill> ListRelationshipSkill;
        private List<BaseAutomaticSkill> ListRelationshipSkillOriginal;

        public RelationshipEditor(BaseAutomaticSkill[] ArrayRelationshipBonus)
        {
            InitializeComponent();
            AllowEvents = false;
            DicRequirement = BaseSkillRequirement.LoadAllRequirements();
            DicEffect = BaseEffect.LoadAllEffects();
            cboRequirementType.Items.AddRange(DicRequirement.Values.OrderBy(x => x.SkillRequirementName).ToArray());

            ListRelationshipSkill = new List<BaseAutomaticSkill>();
            ListRelationshipSkillOriginal = new List<BaseAutomaticSkill>();

            foreach (BaseAutomaticSkill ActiveSkill in ArrayRelationshipBonus)
            {
                ListRelationshipSkill.Add(ActiveSkill);
                ListRelationshipSkillOriginal.Add(ActiveSkill);

                for (int L = 0; L < ActiveSkill.ListSkillLevel.Count; ++L)
                {
                    lstCharacters.Items.Add("Character " + (lstCharacters.Items.Count + 1));
                }
            }
            AllowEvents = true;
        }

        public void Save(BinaryWriter BW)
        {
            BW.Write(ListRelationshipSkill.Count);
            for (int C = 0; C < ListRelationshipSkill.Count; ++C)
            {
                BW.Write(ListRelationshipSkill[C].Name);
                BW.Write(ListRelationshipSkill[C].CurrentLevel);
                ListRelationshipSkill[C].CurrentSkillLevel.ListActivation[0].ListRequirement[0].Save(BW);
            }
        }

        private void lstCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            AllowEvents = false;
            if (lstCharacters.SelectedIndex >= 0)
            {
                gbRelationship.Enabled = true;
                if (ListRelationshipSkill[lstCharacters.SelectedIndex] != null)
                {
                    txtRelationshipBonus.Text = ListRelationshipSkill[lstCharacters.SelectedIndex].Name;
                    txtRelationshipLevel.Enabled = true;
                    txtRelationshipLevel.Value = ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentLevel;
                    txtRelationshipLevel.Maximum = ListRelationshipSkill[lstCharacters.SelectedIndex].ListSkillLevel.Count;
                    cboRequirementType.Text = ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentSkillLevel.ListActivation[0].ListRequirement[0].SkillRequirementName;
                    pgRequirement.SelectedObject = ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentSkillLevel.ListActivation[0].ListRequirement[0];
                    gbRequirement.Enabled = true;
                }
                else
                {
                    txtRelationshipBonus.Text = "";
                    cboRequirementType.SelectedIndex = -1;
                    pgRequirement.SelectedObject = null;
                    txtRelationshipLevel.Enabled = false;
                    gbRequirement.Enabled = false;
                }
            }
            else
            {
                gbRelationship.Enabled = false;
                gbRequirement.Enabled = false;
                txtRelationshipBonus.Text = "";
            }
            AllowEvents = true;
        }

        private void btnAddCharacter_Click(object sender, EventArgs e)
        {
            lstCharacters.Items.Add("Character " + (lstCharacters.Items.Count + 1));
            ListRelationshipSkill.Add(null);
        }

        private void btnRemoveCharacter_Click(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0)
            {
                lstCharacters.Items.RemoveAt(lstCharacters.SelectedIndex);
            }
        }

        private void btnSelectRelationshipBonus_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Bonus;
            ListMenuItemsSelected(BaseEditor.ShowContextMenuWithItem(BaseEditor.GUIRootPathCharacterRelationships));
        }

        private void txtRelationshipLevel_ValueChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            //Copy requirement to the new selected level.
            ListRelationshipSkill[lstCharacters.SelectedIndex].ListSkillLevel[(int)txtRelationshipLevel.Value - 1].ListActivation[0].ListRequirement[0] 
                = ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentSkillLevel.ListActivation[0].ListRequirement[0];

            ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentLevel = (int)txtRelationshipLevel.Value;
        }

        private void cboRequirementType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!AllowEvents)
                return;

            BaseSkillRequirement NewSkillRequirement = ((BaseSkillRequirement)cboRequirementType.SelectedItem).Copy();

            ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentSkillLevel.ListActivation[0].ListRequirement[0] = NewSkillRequirement;
            pgRequirement.SelectedObject = NewSkillRequirement;
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
                    case ItemSelectionChoices.Bonus:
                        if (Items[I] == null)
                            txtRelationshipBonus.Text = "None";
                        else
                        {
                            AllowEvents = false;
                            Name = Items[I].Substring(0, Items[0].Length - 5).Substring(33);
                            BaseAutomaticSkill NewRelationshipSkill = new BaseAutomaticSkill("Content/Characters/Relationships/" + Name + ".pecr", DicRequirement, DicEffect);
                            NewRelationshipSkill.CurrentSkillLevel.ListActivation[0].ListRequirement.Add(DicRequirement["Relationship Requirement"].Copy());
                            ListRelationshipSkill[lstCharacters.SelectedIndex] = NewRelationshipSkill;

                            gbRequirement.Enabled = true;
                            txtRelationshipBonus.Text = Name;
                            txtRelationshipLevel.Enabled = true;
                            txtRelationshipLevel.Value = NewRelationshipSkill.CurrentLevel;
                            txtRelationshipLevel.Maximum = NewRelationshipSkill.ListSkillLevel.Count;
                            cboRequirementType.Text = NewRelationshipSkill.CurrentSkillLevel.ListActivation[0].ListRequirement[0].SkillRequirementName;
                            pgRequirement.SelectedObject = ListRelationshipSkill[lstCharacters.SelectedIndex].CurrentSkillLevel.ListActivation[0].ListRequirement[0];
                            AllowEvents = true;
                        }
                        break;
                }
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            foreach(BaseAutomaticSkill ActiveSkill in ListRelationshipSkill)
            {
                if (ActiveSkill == null)
                {
                    return;
                }
            }

            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ListRelationshipSkill = ListRelationshipSkillOriginal;
        }
    }
}
