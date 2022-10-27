using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.CardEditor
{
    public partial class ItemCardEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Skill, Animation };

        private ItemSelectionChoices ItemSelectionChoice;

        public ItemCardEditor()
        {
            InitializeComponent();
            FilePath = null;

            cboRarity.SelectedIndex = 0;
        }

        public ItemCardEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
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
                new EditorInfo(new string[] { GUIRootPathSorcererStreetCardsItems, GUIRootPathSorcererStreetCards }, "Sorcerer Street/Item Cards/", new string[] { ".pec" }, typeof(ItemCardEditor)),
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

            BW.Write((int)txtMagicCost.Value);
            BW.Write((byte)cboRarity.SelectedIndex);
            BW.Write((byte)cboCategory.SelectedIndex);

            BW.Write(txtSkill.Text);
            BW.Write(txtActivationAnimation.Text);

            FS.Close();
            BW.Close();
        }

        private void LoadCard(string UnitPath)
        {
            Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(35);
            ItemCard LoadedCard = new ItemCard(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget);

            this.Text = LoadedCard.Name + " - Project Eternity Item Card Editor";

            txtName.Text = LoadedCard.Name;
            txtDescription.Text = LoadedCard.Description;
            txtSkill.Text = LoadedCard.SkillChainName;
            txtActivationAnimation.Text = LoadedCard.ItemActivationAnimationPath;


            txtMagicCost.Value = LoadedCard.MagicCost;
            cboRarity.SelectedIndex = (int)LoadedCard.Rarity;
            cboCategory.SelectedIndex = (int)LoadedCard.ItemType;
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void btnSetSkill_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Skill;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathSorcererStreetSkillChains));
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
                            Name = Items[I].Substring(0, Items[I].Length - 5).Substring(37);
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
