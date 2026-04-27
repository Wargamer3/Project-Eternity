using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class CharacterEditor : BaseEditor
    {
        private enum ItemSelectionChoices { Nothing, Ancestry, Background, Class, Deity, Language, Action, Skill, Skin,  };

        private ItemSelectionChoices ItemSelectionChoice;

        private DetailsEditor frmDetailsEditor;
        private CharacterQuotesEditor frmQuoteEditor;
        private RelationshipEditor frmRelationshipEditor;

        private PlayerCharacter LoadedCharacter;

        public CharacterEditor()
        {
            InitializeComponent();
            ItemSelectionChoice = ItemSelectionChoices.Nothing;
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
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimCharacters }, "Life Sim/Characters/", new string[] { ".pec" }, typeof(CharacterEditor), true, null, true),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimPortraitSprites }, "Life Sim/Characters/Portrait Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimMapSprites }, "Life Sim/Characters/Map Sprites/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false),
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimCharacterModels }, "Life Sim/Characters/Models/", new string[] { ".xnb" }, typeof(ProjectEternityImageViewer), false, null, true),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Character Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            BW.Write(txtName.Text);
            BW.Write(txtDescription.Text);

            if (frmDetailsEditor == null)
            {
                BW.Write("");
                BW.Write("");
                BW.Write("");
                BW.Write("");
            }
            else
            {
                BW.Write(frmDetailsEditor.txtPortraitSprite.Text);
                BW.Write(frmDetailsEditor.txtMapSprite.Text);
                BW.Write(frmDetailsEditor.txt3DModel.Text);
                BW.Write(frmDetailsEditor.txtTags.Text);
            }

            BW.Write(LoadedCharacter.Ancestry.RelativePath);
            BW.Write(LoadedCharacter.Background.RelativePath);
            BW.Write(LoadedCharacter.Class.RelativePath);
            BW.Write(LoadedCharacter.Deity.RelativePath);
            BW.Write((byte)txtAge.Value);
            BW.Write((byte)cbSex.SelectedIndex);

            BW.Write((byte)lsLanguages.Items.Count);
            foreach (string ActiveLanguage in lsLanguages.Items)
            {
                BW.Write(ActiveLanguage);
            }

            #region Skills

            BW.Write((byte)lsActions.Items.Count);
            foreach(string ActiveAction in lsActions.Items)
            {
                BW.Write(ActiveAction);
            }

            BW.Write((byte)lsPassiveSkills.Items.Count);
            foreach (string ActiveSpell in lsPassiveSkills.Items)
            {
                BW.Write(ActiveSpell);
            }

            #endregion

            if (frmRelationshipEditor == null)
            {
                BW.Write((byte)0);
            }
            else
            {
                frmRelationshipEditor.Save(BW);
            }

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
            LifeSimParams.Init();
            Name = CharacterPath.Substring(0, CharacterPath.Length - 4).Substring(28);

            LoadedCharacter = new PlayerCharacter(Name, null);

            this.Text = Name + " - Character Editor";

            txtName.Text = LoadedCharacter.Name;
            txtDescription.Text = LoadedCharacter.Description;

            txtAncestry.Text = LoadedCharacter.Ancestry.Name;
            txtBackground.Text = LoadedCharacter.Background.Name;
            txtClass.Text = LoadedCharacter.Class.Name;
            txtDeity.Text = LoadedCharacter.Deity.Name;
            txtAge.Value = LoadedCharacter.Age;
            cbSex.SelectedIndex = (int)LoadedCharacter.CharacterSex;

            frmQuoteEditor = new CharacterQuotesEditor();
            frmDetailsEditor = new DetailsEditor();
            frmRelationshipEditor = new RelationshipEditor(LoadedCharacter.ArrayRelationshipBonus);

            frmDetailsEditor.txtMapSprite.Text = LoadedCharacter.SpriteMapPath;
            frmDetailsEditor.txtPortraitSprite.Text = LoadedCharacter.SpritPortraitPath;
            frmDetailsEditor.txt3DModel.Text = LoadedCharacter.Model3DPath;
            frmDetailsEditor.txtTags.Text = LoadedCharacter.Tags;

            UpdateStats();

            for (int A = 0; A < LoadedCharacter.ArrayCharacterActionPath.Length; ++A)
            {
                lsActions.Items.Add(LoadedCharacter.ArrayCharacterActionPath[A]);
            }

            for (int S = 0; S < LoadedCharacter.ArrayPassiveSkill.Length; ++S)
            {
                lsPassiveSkills.Items.Add(LoadedCharacter.ArrayPassiveSkill[S]);
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

        private void btnSetAncestry_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Ancestry;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimAncestries));
        }

        private void btnSetBackground_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Background;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterBackgrounds));
        }

        private void btnSetClass_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Class;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterClasses));
        }

        private void btnSetDeity_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Deity;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterDeities));
        }

        private void btnAddLanguage_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Language;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimLanguages));
        }

        private void btnRemoveLanguage_Click(object sender, EventArgs e)
        {

        }

        #region Skills

        private void lsActions_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddAction_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.Action;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathLifeSimCharacterActions));
        }

        private void btnDeleteAction_Click(object sender, EventArgs e)
        {

        }

        private void lsPassiveSkills_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAddSkill_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteSkill_Click(object sender, EventArgs e)
        {

        }

        #endregion

        private void UpdateStats()
        {
            LoadedCharacter.Init();
            txtHP.Text = "" + LoadedCharacter.MaxHP;
            txtSTR.Text = "" + LoadedCharacter.STR;
            txtCON.Text = "" + LoadedCharacter.CON;
            txtDEX.Text = "" + LoadedCharacter.DEX;
            txtINT.Text = "" + LoadedCharacter.INT;
            txtWIS.Text = "" + LoadedCharacter.WIS;
            txtCHA.Text = "" + LoadedCharacter.CHA;
            foreach (KeyValuePair<string, ProficiencyLink> ActiveProficiency in LoadedCharacter.DicProficiencyLevelByName)
            {
                if (ActiveProficiency.Key == "Perception")
                {
                    txtPerception.Text = ActiveProficiency.Value.ProficiencyRank.ToString();
                }
                else if (ActiveProficiency.Key == "Fortitude")
                {
                    txtFortitude.Text = ActiveProficiency.Value.ProficiencyRank.ToString();
                }
                else if (ActiveProficiency.Key == "Reflex")
                {
                    txtReflex.Text = ActiveProficiency.Value.ProficiencyRank.ToString();
                }
                else if (ActiveProficiency.Key == "Will")
                {
                    txtWill.Text = ActiveProficiency.Value.ProficiencyRank.ToString();
                }
            }
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
                    case ItemSelectionChoices.Ancestry:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(28);
                        txtAncestry.Text = Name;
                        CharacterAncestry LoadedAncestry = new CharacterAncestry(Name, null);
                        LoadedCharacter.Ancestry = LoadedAncestry;
                        break;

                    case ItemSelectionChoices.Background:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(39);
                        txtBackground.Text = Name;
                        CharacterBackground LoadedBackground = new CharacterBackground(Name);
                        LoadedCharacter.Background = LoadedBackground;
                        break;

                    case ItemSelectionChoices.Class:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(25);
                        txtClass.Text = Name;
                        CharacterClass LoadedClass = new CharacterClass(Name, null);
                        LoadedCharacter.Class = LoadedClass;
                        break;

                    case ItemSelectionChoices.Deity:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(25);
                        txtDeity.Text = Name;
                        CharacterDeity LoadedDeity = new CharacterDeity(Name, null);
                        LoadedCharacter.Deity = LoadedDeity;
                        break;

                    case ItemSelectionChoices.Language:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(27);
                        lsLanguages.Items.Add(Name);
                        break;

                    case ItemSelectionChoices.Action:
                        Name = Items[I].Substring(0, Items[0].Length - 4).Substring(35);
                        lsActions.Items.Add(Name);
                        break;
                }

                UpdateStats();
            }
        }
    }
}
