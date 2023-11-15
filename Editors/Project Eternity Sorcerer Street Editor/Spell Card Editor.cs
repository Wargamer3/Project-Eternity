using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.CardEditor
{
    public partial class SpellCardEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Skill, Animation };

        private ItemSelectionChoices ItemSelectionChoice;

        public SpellCardEditor()
        {
            InitializeComponent();
            FilePath = null;

            cboRarity.SelectedIndex = 0;
        }

        public SpellCardEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                cboRarity.SelectedIndex = 0;
                cboType.SelectedIndex = 0;
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadCard(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathSorcererStreetCardsSpells, GUIRootPathSorcererStreetCards }, "Sorcerer Street/Spell Cards/", new string[] { ".pec" }, typeof(SpellCardEditor)),
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS, Encoding.UTF8);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);
            BW.Write(txtTags.Text);
            BW.Write(cbDoublecast.Checked);

            BW.Write((int)txtMagicCost.Value);
            BW.Write((byte)txtCardSacrificed.Value);
            BW.Write((byte)cboRarity.SelectedIndex);
            BW.Write((byte)cboType.SelectedIndex);

            BW.Write(txtSkill.Text);
            BW.Write(txtActivationAnimation.Text);

            FS.Close();
            BW.Close();
        }

        private void LoadCard(string UnitPath)
        {
            Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(36);
            SpellCard LoadedCard = new SpellCard(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = LoadedCard.Name + " - Project Eternity Spell Card Editor";

            txtName.Text = LoadedCard.Name;
            txtDescription.Text = LoadedCard.Description;
            txtTags.Text = LoadedCard.Tags;
            cbDoublecast.Checked = LoadedCard.Doublecast;

            txtMagicCost.Value = LoadedCard.MagicCost;
            txtCardSacrificed.Value = LoadedCard.DiscardCost;
            cboRarity.SelectedIndex = (int)LoadedCard.Rarity;
            cboType.SelectedIndex = (int)LoadedCard.SpellType;

            txtSkill.Text = LoadedCard.SkillChainName;
            txtActivationAnimation.Text = LoadedCard.SpellActivationAnimationPath;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void btnSetSkill_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathSorcererStreetSpells));
        }

        private void btnSetActivationAnimation_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Animation;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathAnimations));
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
                    case ItemSelectionChoices.Skill:
                        if (Items[I] == null)
                        {
                            txtSkill.Text = string.Empty;
                        }
                        else
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 4).Substring(31);
                            txtSkill.Text = Name;
                        }
                        break;

                    case ItemSelectionChoices.Animation:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(19);
                        txtActivationAnimation.Text = Name;
                        break;
                }
            }
        }
    }
}
