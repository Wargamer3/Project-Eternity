using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using ProjectEternity.Core.Editor;
using ProjectEternity.GameScreens.SorcererStreetScreen;

namespace ProjectEternity.Editors.CardEditor
{
    public partial class SpellCardEditor : BaseEditor
    {
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

            BW.Write((int)txtMagicCost.Value);
            BW.Write((byte)cboRarity.SelectedIndex);

            BW.Write(lstSkill.Items.Count);
            for (int S = 0; S < lstSkill.Items.Count; ++S)
            {
                BW.Write(lstSkill.Items[S].ToString());
            }

            FS.Close();
            BW.Close();
        }

        private void LoadCard(string UnitPath)
        {
            Name = UnitPath.Substring(0, UnitPath.Length - 4).Substring(39);
            SpellCard LoadedCard = new SpellCard(Name, null);

            this.Text = LoadedCard.Name + " - Project Eternity Spell Card Editor";

            txtName.Text = LoadedCard.Name;
            txtDescription.Text = LoadedCard.Description;

            txtMagicCost.Value = LoadedCard.MagicCost;
            cboRarity.SelectedIndex = (int)LoadedCard.Rarity;

            for (int S = 0; S < LoadedCard.ListSkill.Count; ++S)
            {
                lstSkill.Items.Add(LoadedCard.ListSkill[S].Name);
            }
        }

        private void btnAddSkill_Click(object sender, EventArgs e)
        {
            ListMenuItemsSelected(ShowContextMenuWithItem(GUIRootPathCharacterSkills));
        }

        private void btnRemoveSkill_Click(object sender, EventArgs e)
        {
            if (lstSkill.SelectedIndex >= 0)
            {
                lstSkill.Items.RemoveAt(lstSkill.SelectedIndex);
            }
        }

        protected void ListMenuItemsSelected(List<string> Items)
        {
            if (Items == null)
                return;

            for (int I = 0; I < Items.Count; I++)
            {
                string Name = Items[I].Substring(0, Items[I].Length - 4).Substring(24);
                lstSkill.Items.Add(Name);
            }
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }
    }
}
