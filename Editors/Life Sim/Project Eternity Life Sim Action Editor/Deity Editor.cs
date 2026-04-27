using System;
using System.IO;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class DeityEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Category,  };

        private ItemSelectionChoices ItemSelectionChoice;

        public DeityEditor()
        {
            InitializeComponent();
        }

        public DeityEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadDeity(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimCharacterDeities }, "Life Sim/Deities/", new string[] { ".ped" }, typeof(DeityEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Deity Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            BW.Write(txtCategory.Text);
            BW.Write(txtEdicts.Text);
            BW.Write(txtAnathema.Text);
            BW.Write(txtAreasOfConcern.Text);
            BW.Write(txtReligiousSymbol.Text);
            BW.Write(txtSacredAnimal.Text);
            BW.Write(txtSacredColors.Text);

            BW.Write(rbDeity.Checked);
            if (rbDeity.Checked)
            {
                BW.Write(txtParentPanthenon.Text);
            }
            else
            {
                BW.Write((byte)lsPanthenonMembers.Items.Count);
                for (int M = 0; M < lsPanthenonMembers.Items.Count; ++M)
                {
                    BW.Write(lsPanthenonMembers.Items[M].ToString());
                }
            }

            BW.Write(txtDivineAttribute.Text);
            BW.Write(txtDivineFont.Text);
            BW.Write(txtDivineSanctification.Text);
            BW.Write(txtDivineSkill.Text);
            BW.Write(txtFavoredWeapon.Text);

            BW.Write((byte)lsDomains.Items.Count);
            for (int M = 0; M < lsDomains.Items.Count; ++M)
            {
                BW.Write(lsDomains.Items[M].ToString());
            }

            BW.Write((byte)lsAlternateDomains.Items.Count);
            for (int M = 0; M < lsAlternateDomains.Items.Count; ++M)
            {
                BW.Write(lsAlternateDomains.Items[M].ToString());
            }

            BW.Write((byte)lsClericSpells.Items.Count);
            for (int M = 0; M < lsClericSpells.Items.Count; ++M)
            {
                BW.Write(lsClericSpells.Items[M].ToString());
                BW.Write(((CharacterAction)lsClericSpells.Items[M]).LockedLevel);
            }

            BW.Write(txtDivineIntercessionDescription.Text);
            BW.Write(txtMinorBoon.Text);
            BW.Write(txtModerateBoon.Text);
            BW.Write(txtMajorBoon.Text);
            BW.Write(txtMinorCurse.Text);
            BW.Write(txtModerateCurse.Text);
            BW.Write(txtMajorCurse.Text);

            FS.Close();
            BW.Close();
        }

        private void LoadDeity(string DeityPath)
        {
            Name = DeityPath.Substring(0, DeityPath.Length - 4).Substring(25);

            CharacterDeity LoadedDeity = new CharacterDeity(Name, null);

            this.Text = Name + " - Deity Editor";

            txtName.Text = LoadedDeity.Name;
            txtDescription.Text = LoadedDeity.Description;

            txtCategory.Text = LoadedDeity.DeityCategoryRelativePath;
            txtEdicts.Text = LoadedDeity.DeityEdicts;
            txtAnathema.Text = LoadedDeity.DeityAnathema;
            txtAreasOfConcern.Text = LoadedDeity.AreaOfCocern;
            txtReligiousSymbol.Text = LoadedDeity.ReligiousSymbol;
            txtSacredAnimal.Text = LoadedDeity.SacredAnimal;
            txtSacredColors.Text = LoadedDeity.SacredColors;

            if (LoadedDeity.ParentPantheon != null)
            {
                txtParentPanthenon.Text = LoadedDeity.ParentPantheon;
            }
            else
            {
                foreach (string ActiveMember in LoadedDeity.ListPantheonMember)
                {
                    lsPanthenonMembers.Items.Add(ActiveMember);
                }
            }

            txtDivineAttribute.Text = LoadedDeity.DevoteeBenefits.DivineAttributeRelativePath;
            txtDivineFont.Text = LoadedDeity.DevoteeBenefits.DivineFontRelativePath;
            txtDivineSanctification.Text = LoadedDeity.DevoteeBenefits.DivineSanctification;
            txtDivineSkill.Text = LoadedDeity.DevoteeBenefits.DivineSkillRelativePath;
            txtFavoredWeapon.Text = LoadedDeity.DevoteeBenefits.FavoriteWeapon;

            foreach (string ActiveDomain in LoadedDeity.DevoteeBenefits.ListDomainRelativePath)
            {
                lsDomains.Items.Add(ActiveDomain);
            }

            foreach (string ActiveDomain in LoadedDeity.DevoteeBenefits.ListAlternateDomainRelativePath)
            {
                lsDomains.Items.Add(ActiveDomain);
            }

            foreach (CharacterAction ActiveAction in LoadedDeity.DevoteeBenefits.ListSpell)
            {
                lsClericSpells.Items.Add(ActiveAction);
            }

            txtDivineIntercessionDescription.Text = LoadedDeity.DivineIntercession.Description;
            txtMinorBoon.Text = LoadedDeity.DivineIntercession.MinorBoon;
            txtModerateBoon.Text = LoadedDeity.DivineIntercession.ModerateBoon;
            txtMajorBoon.Text = LoadedDeity.DivineIntercession.MajorBoon;
            txtMinorCurse.Text = LoadedDeity.DivineIntercession.MinorCurse;
            txtModerateCurse.Text = LoadedDeity.DivineIntercession.ModerateCurse;
            txtMajorCurse.Text = LoadedDeity.DivineIntercession.MajorCurse;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }

        private void btnSetCategory_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Category;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterDeityCategories));
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
                    case ItemSelectionChoices.Category:
                        Name = Items[I].Substring(0, Items[I].Length - 5).Substring(34);
                        txtCategory.Text = Name;
                        break;
                }
            }
        }

        private void lsActions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
