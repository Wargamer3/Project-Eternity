using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Item;
using ProjectEternity.Core.Skill;
using ProjectEternity.Core.Editor;
using ProjectEternity.Editors.ImageViewer;
using ProjectEternity.GameScreens.LifeSimScreen;

namespace ProjectEternity.Editors.LifeSimCharacterEditor
{
    public partial class TraitEditor : BaseEditor
    {
        readonly List<TraitEffect> ListEffect = new List<TraitEffect>();

        public TraitEditor()
        {
            InitializeComponent();
        }

        public TraitEditor(string FilePath, object[] Params)
            : this()
        {
            LifeSimCharacterParams.Init();
            List<string> Names = new List<string>(TraitEffect.DicEffect.Keys);
            Names.Sort(StringComparer.Ordinal);
            cboEffect.Items.AddRange(Names.ToArray());
            if (cboEffect.Items.Count > 0)
            {
                cboEffect.SelectedIndex = 0;
            }

            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
                SaveItem(FilePath, FilePath);
            }

            LoadTrait(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            EditorInfo[] Info = new EditorInfo[]
            {
                new EditorInfo(new string[] { EditorHelper.GUIRootPathLifeSimTraits }, "Life Sim/Traits/", new string[] { ".pet" }, typeof(TraitEditor), true, null, false),
            };

            return Info;
        }

        public void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            this.Text = ItemName + " - Trait Editor";

            FileStream FS = new FileStream(ItemPath, FileMode.Create, FileAccess.Write);
            BinaryWriter BW = new BinaryWriter(FS);

            Trait Saving = new Trait();
            Saving.Name = txtName.Text;
            Saving.Description = txtDescription.Text;
            Saving.ListEffect = ListEffect;
            Saving.Write(BW);

            FS.Close();
            BW.Close();
        }

        private void LoadTrait(string TraitPath)
        {
            Name = TraitPath.Substring(0, TraitPath.Length - 4).Substring(24);

            Trait LoadedTrait = new Trait(Name);

            this.Text = Name + " - Trait Editor";

            txtName.Text = LoadedTrait.Name;
            txtDescription.Text = LoadedTrait.Description;
            ListEffect.Clear();
            lstEffects.Items.Clear();
            if (LoadedTrait.ListEffect != null)
            {
                for (int i = 0; i < LoadedTrait.ListEffect.Count; i++)
                {
                    ListEffect.Add(LoadedTrait.ListEffect[i]);
                    lstEffects.Items.Add(LoadedTrait.ListEffect[i].Label);
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (cboEffect.SelectedItem == null)
            {
                return;
            }

            TraitEffect Effect = TraitEffect.Create(cboEffect.SelectedItem.ToString());
            if (Effect == null)
            {
                return;
            }

            ListEffect.Add(Effect);
            lstEffects.Items.Add(Effect.Label);
            lstEffects.SelectedIndex = ListEffect.Count - 1;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            int Index = lstEffects.SelectedIndex;
            if (Index < 0 || Index >= ListEffect.Count)
            {
                return;
            }

            ListEffect.RemoveAt(Index);
            lstEffects.Items.RemoveAt(Index);
        }

        private void lstEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Index = lstEffects.SelectedIndex;
            if (Index < 0 || Index >= ListEffect.Count)
            {
                pgEffect.SelectedObject = null;
                return;
            }

            pgEffect.SelectedObject = ListEffect[Index];
        }

        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, Path.GetFileNameWithoutExtension(FilePath));
        }
    }
}
