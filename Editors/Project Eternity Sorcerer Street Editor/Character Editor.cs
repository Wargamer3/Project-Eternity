using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.SorcererStreetScreen;
using ProjectEternity.Editors.ImageViewer;

namespace ProjectEternity.Editors.SorcererStreetCharacterEditor
{
    public partial class CharacterEditor : BaseEditor
    {
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
            }
            else
            {
                BW.Write(frmQuoteEditor.lsVersusQuotes.Items.Count - 1);
                for (int Q = 1; Q < frmQuoteEditor.lsVersusQuotes.Items.Count; Q++)
                    BW.Write((string)frmQuoteEditor.lsVersusQuotes.Items[Q]);
            }

            QuoteSet Quotes;
            //Base quotes
            for (int I = 0; I < 6; I++)
            {
                if (frmQuoteEditor == null)
                {
                    BW.Write(0);
                    BW.Write(0);
                    BW.Write(string.Empty);
                }
                else
                {
                    Quotes = (QuoteSet)frmQuoteEditor.lvBaseQuotes.Items[I].Tag;

                    BW.Write(Quotes.ListQuote.Count);
                    for (int Q = 0; Q < Quotes.ListQuote.Count; Q++)
                        BW.Write(Quotes.ListQuote[Q]);

                    //Versus quotes.
                    BW.Write(Quotes.ListQuoteVersus.Count);
                    for (int Q = 0; Q < Quotes.ListQuoteVersus.Count; Q++)
                        BW.Write(Quotes.ListQuoteVersus[Q]);

                    BW.Write(Quotes.PortraitPath);
                }
            }

            //Attack Quotes.

            //Count the actual number of quotes listed.
            int QuoteCount = 0;

            if (frmQuoteEditor != null)
            {
                for (int R = 0; R < frmQuoteEditor.dgvQuoteSets.Rows.Count; R++)
                    if (!string.IsNullOrEmpty((string)frmQuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue))
                        QuoteCount++;
            }

            BW.Write(QuoteCount);

            if (frmQuoteEditor != null)
            {
                for (int R = 0; R < frmQuoteEditor.dgvQuoteSets.Rows.Count; R++)
                {
                    if (!string.IsNullOrEmpty((string)frmQuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue))
                    {
                        Quotes = (QuoteSet)frmQuoteEditor.dgvQuoteSets.Rows[R].Tag;

                        BW.Write((string)frmQuoteEditor.dgvQuoteSets.Rows[R].Cells[0].EditedFormattedValue);

                        BW.Write(Quotes.ListQuote.Count);
                        for (int Q = 0; Q < Quotes.ListQuote.Count; Q++)
                            BW.Write(Quotes.ListQuote[Q]);

                        //Versus quotes.
                        BW.Write(Quotes.ListQuoteVersus.Count);
                        for (int Q = 0; Q < Quotes.ListQuoteVersus.Count; Q++)
                            BW.Write(Quotes.ListQuoteVersus[Q]);

                        BW.Write(Quotes.PortraitPath);
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

            //Versus names
            for (int I = 0; I < LoadedCharacter.ListQuoteSetVersusName.Count; I++)
                frmQuoteEditor.lsVersusQuotes.Items.Add(LoadedCharacter.ListQuoteSetVersusName[I]);

            //Base quotes
            for (int I = 0; I < 6; I++)
                frmQuoteEditor.lvBaseQuotes.Items[I].Tag = LoadedCharacter.ArrayBaseQuoteSet[I];

            //Attack quotes
            for (int i = 0; i < LoadedCharacter.DicAttackQuoteSet.Count; i++)
            {
                KeyValuePair<string, QuoteSet> NewQuoteSet = LoadedCharacter.DicAttackQuoteSet.ElementAt(i);
                frmQuoteEditor.dgvQuoteSets.Rows.Add(NewQuoteSet.Key);
                frmQuoteEditor.dgvQuoteSets.Rows[i].Tag = NewQuoteSet.Value;
            }
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
    }
}
