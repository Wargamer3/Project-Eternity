using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Units;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Characters;
using ProjectEternity.Core.Skill;

namespace ProjectEternity.Editors.RosterEditor
{
    public partial class ProjectEternityRosterEditor : BaseEditor
    {
        public List<Character> ListCharacter;
        public List<Unit> ListUnit;

        private enum ItemSelectionChoices { Unit, Character };

        private ItemSelectionChoices ItemSelectionChoice;

        public ProjectEternityRosterEditor()
        {
            InitializeComponent();
            ListCharacter = new List<Character>();
            ListUnit = new List<Unit>();
        }

        public override EditorInfo[] LoadEditors()
        {
            return null;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            FileStream FS = new FileStream("Content/Roster.bin", FileMode.Create);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.ASCII);

            BW.Write(ListCharacter.Count);
            for (int C = 0; C < ListCharacter.Count; C++)
            {
                BW.Write(ListCharacter[C].FullName);
                BW.Write(ListCharacter[C].ID);

                BW.Write(ListCharacter[C].DicCharacterLink.Count);
                foreach (KeyValuePair<string, Character.CharacterLinkTypes> ActiveLink in ListCharacter[C].DicCharacterLink)
                {
                    BW.Write(ActiveLink.Key);
                    BW.Write((byte)ActiveLink.Value);
                }
            }
            BW.Write(ListUnit.Count);
            for (int U = 0; U < ListUnit.Count; U++)
            {
                BW.Write(ListUnit[U].UnitTypeName);
                BW.Write(ListUnit[U].RelativePath);
                BW.Write(ListUnit[U].ID);

                BW.Write(ListUnit[U].UnitStat.DicUnitLink.Count);
                foreach (KeyValuePair<string, UnitStats.UnitLinkTypes> ActiveLink in ListUnit[U].UnitStat.DicUnitLink)
                {
                    BW.Write(ActiveLink.Key);
                    BW.Write((byte)ActiveLink.Value);
                }
            }

            BW.Close();
            FS.Close();
        }

        public void LoadRoster()
        {
            if (!File.Exists("Content/Roster.bin"))
            {
                return;
            }

            FileStream FS = new FileStream("Content/Roster.bin", FileMode.Open, FileAccess.Read);
            BinaryReader BR = new BinaryReader(FS);

            int ListCharacterCount = BR.ReadInt32();
            ListCharacter = new List<Character>(ListCharacterCount);

            for (int C = 0; C < ListCharacterCount; C++)
            {
                Character NewCharacter = new Character(BR.ReadString(), null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);
                NewCharacter.ID = BR.ReadString();

                lstCharacters.Items.Add(NewCharacter.FullName);
                lstCharactersToShareFrom.Items.Add(NewCharacter.FullName);
                ListCharacter.Add(NewCharacter);

                int DicCharacterLinkCount = BR.ReadInt32();
                for (int L = 0; L < DicCharacterLinkCount; L++)
                {
                    string Key = BR.ReadString();
                    Character.CharacterLinkTypes CharacterLinkType = (Character.CharacterLinkTypes)BR.ReadByte();
                    NewCharacter.DicCharacterLink.Add(Key, CharacterLinkType);
                }
            }

            int ListUnitCount = BR.ReadInt32();
            ListUnit = new List<Unit>(ListUnitCount);

            for (int U = 0; U < ListUnitCount; U++)
            {
                string UnitTypeName = BR.ReadString();
                string UnitName = BR.ReadString();
                string EventID = BR.ReadString();
                Unit NewUnit = Unit.FromType(UnitTypeName, UnitName, null, Unit.DicDefaultUnitType, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);
                NewUnit.ID = EventID;

                lstUnits.Items.Add(NewUnit.ItemName);
                lstUnitsToShareFrom.Items.Add(NewUnit.ItemName);
                ListUnit.Add(NewUnit);

                int DicUnitLinkCount = BR.ReadInt32();
                for (int L = 0; L < DicUnitLinkCount; L++)
                {
                    string Key = BR.ReadString();
                    UnitStats.UnitLinkTypes UnitLinkType = (UnitStats.UnitLinkTypes)BR.ReadByte();
                    NewUnit.UnitStat.DicUnitLink.Add(Key, UnitLinkType);
                }
            }

            BR.Close();
            FS.Close();
        }

        #region Characters

        private void cbCharacterShare_CheckedChanged(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0 && lstCharactersToShareFrom.SelectedIndex >= 0)
            {
                Character ActiveCharacter = ListCharacter[lstCharacters.SelectedIndex];
                string OtherCharacterName = ListCharacter[lstCharactersToShareFrom.SelectedIndex].FullName;

                if (!ActiveCharacter.DicCharacterLink.ContainsKey(OtherCharacterName))
                    ActiveCharacter.DicCharacterLink.Add(OtherCharacterName, Character.CharacterLinkTypes.None);

                Character.CharacterLinkTypes CharacterLinkType = Character.CharacterLinkTypes.None;

                if (cbShareLevel.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.Level;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.Level;

                if (cbShareEXP.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.EXP;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.EXP;

                if (cbShareMEL.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.MEL;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.MEL;

                if (cbShareRNG.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.RNG;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.RNG;

                if (cbShareDEF.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.DEF;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.DEF;

                if (cbShareSKL.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.SKL;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.SKL;

                if (cbShareEVA.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.EVA;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.EVA;

                if (cbShareHIT.Checked)
                    CharacterLinkType = CharacterLinkType | Character.CharacterLinkTypes.HIT;
                else
                    CharacterLinkType = CharacterLinkType & ~Character.CharacterLinkTypes.HIT;

                ActiveCharacter.DicCharacterLink[OtherCharacterName] = CharacterLinkType;
            }
        }

        private void txtCharacterEventID_TextChanged(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0)
            {
                Character ActiveCharacter = ListCharacter[lstCharacters.SelectedIndex];
                ActiveCharacter.ID = txtCharacterEventID.Text;
            }
        }

        private void btnAddCharacter_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Character;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacters));
        }

        private void btnRemoveCharacter_Click(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0)
            {
                ListCharacter.RemoveAt(lstCharacters.SelectedIndex);
                lstCharactersToShareFrom.Items.RemoveAt(lstCharacters.SelectedIndex);
                lstCharacters.Items.RemoveAt(lstCharacters.SelectedIndex);
            }
        }

        private void lstCharacters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCharacters.SelectedIndex >= 0)
            {
                Character ActiveCharacter = ListCharacter[lstCharacters.SelectedIndex];
                lblMEL.Text = ActiveCharacter.MEL.ToString();
                lblRNG.Text = ActiveCharacter.RNG.ToString();
                lblDEF.Text = ActiveCharacter.DEF.ToString();
                lblSKL.Text = ActiveCharacter.SKL.ToString();
                lblEVA.Text = ActiveCharacter.EVA.ToString();
                lblHIT.Text = ActiveCharacter.HIT.ToString();
                txtCharacterEventID.Text = ActiveCharacter.ID;

                Character.CharacterLinkTypes CharacterLinkType = Character.CharacterLinkTypes.None;

                if (lstCharactersToShareFrom.SelectedIndex >= 0)
                {
                    string OtherCharacterName = ListCharacter[lstCharactersToShareFrom.SelectedIndex].FullName;
                    if (ActiveCharacter.DicCharacterLink.ContainsKey(OtherCharacterName))
                    {
                        CharacterLinkType = ActiveCharacter.DicCharacterLink[OtherCharacterName];
                    }

                    if ((CharacterLinkType & Character.CharacterLinkTypes.Level) == Character.CharacterLinkTypes.Level)
                        cbShareLevel.Checked = true;
                    else
                        cbShareLevel.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.EXP) == Character.CharacterLinkTypes.EXP)
                        cbShareEXP.Checked = true;
                    else
                        cbShareEXP.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.MEL) == Character.CharacterLinkTypes.MEL)
                        cbShareMEL.Checked = true;
                    else
                        cbShareMEL.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.RNG) == Character.CharacterLinkTypes.RNG)
                        cbShareRNG.Checked = true;
                    else
                        cbShareRNG.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.DEF) == Character.CharacterLinkTypes.DEF)
                        cbShareDEF.Checked = true;
                    else
                        cbShareDEF.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.SKL) == Character.CharacterLinkTypes.SKL)
                        cbShareSKL.Checked = true;
                    else
                        cbShareSKL.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.EVA) == Character.CharacterLinkTypes.EVA)
                        cbShareEVA.Checked = true;
                    else
                        cbShareEVA.Checked = false;

                    if ((CharacterLinkType & Character.CharacterLinkTypes.HIT) == Character.CharacterLinkTypes.HIT)
                        cbShareHIT.Checked = true;
                    else
                        cbShareHIT.Checked = false;
                }
            }
        }

        #endregion

        #region Units

        private void cbUnitShare_CheckedChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex >= 0 && lstUnitsToShareFrom.SelectedIndex >= 0)
            {
                Unit ActiveUnit = ListUnit[lstUnits.SelectedIndex];
                string OtherUnitName = ListUnit[lstUnitsToShareFrom.SelectedIndex].UnitStat.Name;

                if (!ActiveUnit.UnitStat.DicUnitLink.ContainsKey(OtherUnitName))
                    ActiveUnit.UnitStat.DicUnitLink.Add(OtherUnitName, UnitStats.UnitLinkTypes.None);

                UnitStats.UnitLinkTypes UnitLinkType = UnitStats.UnitLinkTypes.None;

                if (cbShareMaxHP.Checked)
                    UnitLinkType = UnitLinkType | UnitStats.UnitLinkTypes.MaxHP;
                else
                    UnitLinkType = UnitLinkType & ~UnitStats.UnitLinkTypes.MaxHP;

                if (cbShareMaxEN.Checked)
                    UnitLinkType = UnitLinkType | UnitStats.UnitLinkTypes.MaxEN;
                else
                    UnitLinkType = UnitLinkType & ~UnitStats.UnitLinkTypes.MaxEN;

                if (cbShareRegenEN.Checked)
                    UnitLinkType = UnitLinkType | UnitStats.UnitLinkTypes.RegenEN;
                else
                    UnitLinkType = UnitLinkType & ~UnitStats.UnitLinkTypes.RegenEN;

                if (cbShareArmor.Checked)
                    UnitLinkType = UnitLinkType | UnitStats.UnitLinkTypes.Armor;
                else
                    UnitLinkType = UnitLinkType & ~UnitStats.UnitLinkTypes.Armor;

                if (cbShareMobility.Checked)
                    UnitLinkType = UnitLinkType | UnitStats.UnitLinkTypes.Mobility;
                else
                    UnitLinkType = UnitLinkType & ~UnitStats.UnitLinkTypes.Mobility;

                if (cbShareMaxMovement.Checked)
                    UnitLinkType = UnitLinkType | UnitStats.UnitLinkTypes.MaxMovement;
                else
                    UnitLinkType = UnitLinkType & ~UnitStats.UnitLinkTypes.MaxMovement;

                ActiveUnit.UnitStat.DicUnitLink[OtherUnitName] = UnitLinkType;
            }
        }

        private void txtUnitEventID_TextChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex >= 0)
            {
                Unit ActiveUnit = ListUnit[lstUnits.SelectedIndex];
                ActiveUnit.ID = txtUnitEventID.Text;
            }
        }

        private void btnAddUnit_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Unit;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathUnits));
        }

        private void btnRemoveUnit_Click(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex >= 0)
            {
                ListUnit.RemoveAt(lstUnits.SelectedIndex);
                lstUnitsToShareFrom.Items.RemoveAt(lstUnits.SelectedIndex);
                lstUnits.Items.RemoveAt(lstUnits.SelectedIndex);
            }
        }

        private void lstUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex >= 0)
            {
                Unit ActiveUnit = ListUnit[lstUnits.SelectedIndex];
                lblMaxHP.Text = ActiveUnit.MaxHP.ToString();
                lblMaxEN.Text = ActiveUnit.MaxEN.ToString();
                lblRegenEN.Text = ActiveUnit.RegenEN.ToString();
                lblArmor.Text = ActiveUnit.Armor.ToString();
                lblMobility.Text = ActiveUnit.Mobility.ToString();
                lblMaxMovement.Text = ActiveUnit.MaxMovement.ToString();
                txtUnitEventID.Text = ActiveUnit.ID;

                UnitStats.UnitLinkTypes UnitLinkType = UnitStats.UnitLinkTypes.None;

                if (lstUnitsToShareFrom.SelectedIndex >= 0)
                {
                    string OtherUnitName = ListUnit[lstUnitsToShareFrom.SelectedIndex].UnitStat.Name;
                    if (ActiveUnit.UnitStat.DicUnitLink.ContainsKey(OtherUnitName))
                    {
                        UnitLinkType = ActiveUnit.UnitStat.DicUnitLink[OtherUnitName];
                    }

                    if ((UnitLinkType & UnitStats.UnitLinkTypes.MaxHP) == UnitStats.UnitLinkTypes.MaxHP)
                        cbShareMaxHP.Checked = true;
                    else
                        cbShareMaxHP.Checked = false;

                    if ((UnitLinkType & UnitStats.UnitLinkTypes.MaxEN) == UnitStats.UnitLinkTypes.MaxEN)
                        cbShareMaxEN.Checked = true;
                    else
                        cbShareMaxEN.Checked = false;

                    if ((UnitLinkType & UnitStats.UnitLinkTypes.RegenEN) == UnitStats.UnitLinkTypes.RegenEN)
                        cbShareRegenEN.Checked = true;
                    else
                        cbShareRegenEN.Checked = false;

                    if ((UnitLinkType & UnitStats.UnitLinkTypes.Armor) == UnitStats.UnitLinkTypes.Armor)
                        cbShareArmor.Checked = true;
                    else
                        cbShareArmor.Checked = false;

                    if ((UnitLinkType & UnitStats.UnitLinkTypes.Mobility) == UnitStats.UnitLinkTypes.Mobility)
                        cbShareMobility.Checked = true;
                    else
                        cbShareMobility.Checked = false;

                    if ((UnitLinkType & UnitStats.UnitLinkTypes.MaxMovement) == UnitStats.UnitLinkTypes.MaxMovement)
                        cbShareMaxMovement.Checked = true;
                    else
                        cbShareMaxMovement.Checked = false;
                }
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
                    case ItemSelectionChoices.Unit:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(Items[I].LastIndexOf("Units") + 6);
                        if (Name != null)
                        {
                            string[] UnitInfo = Name.Split(new[] { "/" }, StringSplitOptions.None);
                            Unit NewUnit = Unit.FromType(UnitInfo[0], Name.Remove(0, UnitInfo[0].Length + 1), null, Unit.DicDefaultUnitType, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

                            lstUnits.Items.Add(Name);
                            lstUnitsToShareFrom.Items.Add(Name);
                            ListUnit.Add(NewUnit);
                        }
                        break;

                    case ItemSelectionChoices.Character:
                        if (Items[I] != null)
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 4).Substring(Items[I].LastIndexOf("Characters") + 11);
                            Character NewCharacter = new Character(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

                            lstCharacters.Items.Add(Name);
                            lstCharactersToShareFrom.Items.Add(Name);
                            ListCharacter.Add(NewCharacter);
                        }
                        break;
                }
            }
        }
    }
}
