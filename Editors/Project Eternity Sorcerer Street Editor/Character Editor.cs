using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    public partial class CharacterEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Spell, Skill, Skin,  };

        private ItemSelectionChoices ItemSelectionChoice;

        private DetailsEditor frmDetailsEditor;
        private CharacterQuotesEditor frmQuoteEditor;
        private RelationshipEditor frmRelationshipEditor;

        public CharacterEditor()
        {
            InitializeComponent();
            frmDetailsEditor = new DetailsEditor();
        }

        public CharacterEditor(string FilePath, object[] Params)
            : this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadCharacter(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { GUIRootPathSorcererStreetCharacters }, "Sorcerer Street/Characters/", new string[] { ".pec" }, typeof(CharacterEditor), true, null, true),
                new EditorInfo(new string[] { GUIRootPathSorcererStreetShopSprites }, "Sorcerer Street/Shop Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathSorcererStreetMapSprites }, "Sorcerer Street/Map Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { GUIRootPathSorcererStreetCharacterModels }, "Sorcerer Street/Models/Characters/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false, null, true),
            };

            return Info;
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Character Editor";

            //Create the Part file.
            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);
            BW.Write((int)txtPrice.Value);

            if (frmDetailsEditor == null)
            {
                BW.Write("");
                BW.Write("");
                BW.Write("");
                BW.Write("");
            }
            else
            {
                BW.Write(frmDetailsEditor.txtShopSprite.Text);
                BW.Write(frmDetailsEditor.txtMapSprite.Text);
                BW.Write(frmDetailsEditor.txt3DModel.Text);
                BW.Write(frmDetailsEditor.txtTags.Text);
            }

            #region Skills

            BW.Write((byte)lsSpells.Items.Count);

            foreach(ManualSkill ActiveSpell in lsSpells.Items)
            {
                BW.Write(ActiveSpell.FullName);
                BW.Write(ActiveSpell.ActivationCost);
            }

            BW.Write((byte)lsPassiveSkills.Items.Count);

            foreach (BaseAutomaticSkill ActiveSpell in lsPassiveSkills.Items)
            {
                BW.Write(ActiveSpell.RelativePath);
            }

            #endregion

            BW.Write((byte)lsWhitelist.Items.Count);

            foreach (string ActiveItem in lsWhitelist.Items)
            {
                BW.Write(ActiveItem);
            }

            BW.Write((byte)lsBlacklist.Items.Count);

            foreach (string ActiveItem in lsBlacklist.Items)
            {
                BW.Write(ActiveItem);
            }

            BW.Write((byte)lsSkins.Items.Count);

            foreach (PlayerCharacterSkin ActiveSkin in lsSkins.Items)
            {
                BW.Write(ActiveSkin.SkinPath);
                BW.Write(ActiveSkin.Locked);
            }

            BW.Write((byte)lsAIBooks.Items.Count);

            foreach (string ActiveBook in lsAIBooks.Items)
            {
                BW.Write(ActiveBook);
            }

            if (frmRelationshipEditor == null)
                BW.Write(0);
            else
                frmRelationshipEditor.Save(BW);

            #region Quotes

            if (frmQuoteEditor == null)
            {
                BW.Write(0);
                BW.Write(0);
                BW.Write(0);
            }
            else
            {
                BW.Write(frmQuoteEditor.lsMapQuotes.Items.Count - 1);
                for (int Q = 1; Q < frmQuoteEditor.lsMapQuotes.Items.Count; Q++)
                    BW.Write((string)frmQuoteEditor.lsMapQuotes.Items[Q]);

                BW.Write(frmQuoteEditor.lsVersusQuotes.Items.Count - 1);
                for (int Q = 1; Q < frmQuoteEditor.lsVersusQuotes.Items.Count; Q++)
                    BW.Write((string)frmQuoteEditor.lsVersusQuotes.Items[Q]);


                BW.Write((byte)frmQuoteEditor.lvBaseQuotes.Items.Count);

                QuoteSet Quotes;
                //Base quotes
                for (int I = 0; I < frmQuoteEditor.lvBaseQuotes.Items.Count; I++)
                {
                    if (frmQuoteEditor == null)
                    {
                        BW.Write(0);
                    }
                    else
                    {
                        Quotes = (QuoteSet)frmQuoteEditor.lvBaseQuotes.Items[I].Tag;
                        Quotes.Write(BW);
                    }
                }
            }

            #endregion

            FS.Close();
            BW.Close();
        }

        private void LoadCharacter(string CharacterPath)
        {
            Name = CharacterPath.Substring(0, CharacterPath.Length - 4).Substring(35);

            PlayerCharacter LoadedCharacter = new PlayerCharacter(Name, null, BaseSkillRequirement.DicDefaultRequirement, BaseEffect.DicDefaultEffect, AutomaticSkillTargetType.DicDefaultTarget, ManualSkillTarget.DicDefaultTarget);

            this.Text = LoadedCharacter.Name + " - Character Editor";

            txtName.Text = LoadedCharacter.Name;
            txtDescription.Text = LoadedCharacter.Description;
            txtPrice.Value = LoadedCharacter.Price;

            frmQuoteEditor = new CharacterQuotesEditor();
            frmDetailsEditor = new DetailsEditor();
            frmRelationshipEditor = new RelationshipEditor(LoadedCharacter.ArrayRelationshipBonus);

            frmDetailsEditor.txtMapSprite.Text = LoadedCharacter.SpriteMapPath;
            frmDetailsEditor.txtShopSprite.Text = LoadedCharacter.SpriteShopPath;
            frmDetailsEditor.txt3DModel.Text = LoadedCharacter.Model3DPath;
            frmDetailsEditor.txtTags.Text = LoadedCharacter.Tags;

            for (int S = 0; S < LoadedCharacter.ArraySpell.Length; ++S)
            {
                lsSpells.Items.Add(LoadedCharacter.ArraySpell[S]);
            }

            for (int S = 0; S < LoadedCharacter.ArraySkill.Length; ++S)
            {
                lsPassiveSkills.Items.Add(LoadedCharacter.ArraySkill[S]);
            }

            for (int W = 0; W < LoadedCharacter.ListWhitelist.Count; ++W)
            {
                lsWhitelist.Items.Add(LoadedCharacter.ListWhitelist[W]);
            }

            for (int B = 0; B < LoadedCharacter.ListBlacklist.Count; ++B)
            {
                lsBlacklist.Items.Add(LoadedCharacter.ListBlacklist[B]);
            }

            for (int S = 1; S < LoadedCharacter.ListSkin.Count; ++S)
            {
                lsSkins.Items.Add(LoadedCharacter.ListSkin[S]);
            }

            //Map names
            for (int I = 0; I < LoadedCharacter.ListQuoteSetMapName.Count; I++)
                frmQuoteEditor.lsMapQuotes.Items.Add(LoadedCharacter.ListQuoteSetMapName[I]);

            //Versus names
            for (int I = 0; I < LoadedCharacter.ListQuoteSetVersusName.Count; I++)
                frmQuoteEditor.lsVersusQuotes.Items.Add(LoadedCharacter.ListQuoteSetVersusName[I]);

            //Base quotes
            for (int I = 0; I < LoadedCharacter.ArrayBaseQuoteSet.Length; I++)
                frmQuoteEditor.lvBaseQuotes.Items[I].Tag = LoadedCharacter.ArrayBaseQuoteSet[I];
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileName(FilePath));
        }

        private void tsmDetails_Click(object sender, EventArgs e)
        {
            frmDetailsEditor.ShowDialog();
        }

        private void tsmQuotes_Click(object sender, EventArgs e)
        {
            frmQuoteEditor.ShowDialog();
        }

        private void tsmRelationships_Click(object sender, EventArgs e)
        {
            frmRelationshipEditor.ShowDialog();
        }

        #region Skills

        private void lsSpells_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddSpell_Click(object sender, EventArgs e)
        {

        }

        private void btnSetSpell_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteSpell_Click(object sender, EventArgs e)
        {

        }

        private void txtSpellCost_ValueChanged(object sender, EventArgs e)
        {

        }

        private void lsPassiveSkills_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddSkill_Click(object sender, EventArgs e)
        {

        }

        private void btnSetSkill_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteSkill_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Whitelist

        private void lsWhitelist_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddWhitelist_Click(object sender, EventArgs e)
        {

        }

        private void btnSetWhitelist_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteWhitelist_Click(object sender, EventArgs e)
        {

        }

        private void lsBlacklist_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddBlacklist_Click(object sender, EventArgs e)
        {

        }

        private void btnSetBlacklist_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteBlacklist_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Skins

        private void lsSkins_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lsSkins.SelectedIndex < 0)
            {
                return;
            }

            PlayerCharacterSkin SelectedSkin = (PlayerCharacterSkin)lsSkins.Items[lsSkins.SelectedIndex];
            txtSkinName.Text = SelectedSkin.SkinPath;
            ckLockedSkin.Checked = SelectedSkin.Locked;
        }

        private void btnAddSkin_Click(object sender, EventArgs e)
        {
            lsSkins.Items.Add(new PlayerCharacterSkin("Select A Skin"));
        }

        private void btnSetSkin_Click(object sender, EventArgs e)
        {
            if (lsSkins.SelectedIndex < 0)
            {
                return;
            }

            ItemSelectionChoice = ItemSelectionChoices.Skin;
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathSorcererStreetCharacters));
        }

        private void btnDeleteSkin_Click(object sender, EventArgs e)
        {
            if (lsSkins.SelectedIndex < 0)
            {
                return;
            }

            int SelectedIndex = lsSkins.SelectedIndex;
            lsSkins.Items.RemoveAt(lsSkins.SelectedIndex);
            if (lsSkins.Items.Count > 0)
            {
                if (SelectedIndex > 0)
                {
                    lsSkins.SelectedIndex = SelectedIndex - 1;
                }
                else
                {

                    lsSkins.SelectedIndex = 0;
                }
            }
        }

        private void ckLockedSkin_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Books

        private void lsAIBooks_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddAIBook_Click(object sender, EventArgs e)
        {

        }

        private void btnSetAIBook_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteAIBook_Click(object sender, EventArgs e)
        {

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
                    case ItemSelectionChoices.Spell:
                        break;

                    case ItemSelectionChoices.Skill:
                        break;

                    case ItemSelectionChoices.Skin:
                        if (Items[I] != null)
                        {
                            Name = Items[I].Substring(0, Items[I].Length - 4).Substring(35);
                            PlayerCharacterSkin SelectedSkin = (PlayerCharacterSkin)lsSkins.Items[lsSkins.SelectedIndex];
                            SelectedSkin.SkinPath = Name;
                            txtSkinName.Text = Name;
                            lsSkins.Items[lsSkins.SelectedIndex] = SelectedSkin;
                        }
                        break;
                }
            }
        }
    }
}
