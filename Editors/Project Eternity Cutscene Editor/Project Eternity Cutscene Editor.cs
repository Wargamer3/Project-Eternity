using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;

namespace ProjectEternity.Editors.CutsceneEditor
{
    public unsafe partial class CutsceneEditor : BaseEditor
    {
        public Cutscene ActiveCutscene { get { return cutsceneViewer.ActiveCutscene; } set { cutsceneViewer.ActiveCutscene = value; } }

        public CutsceneEditor()
        {
            InitializeComponent();
        }

        public CutsceneEditor(string FilePath, object[] Params)
            :this()
        {
            this.FilePath = FilePath;
            if (!File.Exists(FilePath))
            {
                //Create the Part file.
                FileStream FS = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
                BinaryWriter BW = new BinaryWriter(FS);

                BW.Write(0);//Empty Action Script list.
                BW.Write(0);//Empty Data Container list.

                FS.Close();
                BW.Close();
            }

            LoadCutscene(this.FilePath);
        }

        public override EditorInfo[] LoadEditors()
        {
            return new EditorInfo[]
            {
                new EditorInfo(new string[] { "Cutscenes" }, "Cutscenes/", new string[] { ".pec" }, typeof(CutsceneEditor))
            };
        }

        public override void SaveItem(string ItemPath, string ItemName, bool ForceOverwrite = false)
        {
            ActiveCutscene.Save(ItemPath);
        }

        private void LoadCutscene(string CutscenePath)
        {
            string Name = CutscenePath.Substring(0, CutscenePath.Length - 4).Substring(18);
            this.Text = Name + " - Project Eternity Cutscene Editor";

            ActiveCutscene = new Cutscene(null, Name, CutsceneScriptHolder.LoadAllScripts());
            ActiveCutscene.IsInEditor = true;
        }
        
        private void SelectScript(CutsceneScript SelectedScript)
        {
            pgScriptProperties.SelectedObject = SelectedScript;
        }

        private void lstChoices_DoubleClick(object sender, EventArgs e)
        {
            if (!(sender is ListBox))
                return;
            if (((ListBox)sender).SelectedIndex == -1)
                return;

            CutsceneScript NewScript = ((CutsceneScript)((ListBox)sender).SelectedItem).CopyScript(null);
            cutsceneViewer.InitScript(NewScript);
            if (NewScript is CutsceneDataContainer)
            {
                UInt32 NextID = 1;
                for (int S = ActiveCutscene.ListDataContainer.Count - 1; S >= 0; --S)
                    if (ActiveCutscene.ListDataContainer[S].Name == NewScript.Name)
                        NextID++;
                ((CutsceneDataContainer)NewScript).ID = NextID;
                ActiveCutscene.ListDataContainer.Add((CutsceneDataContainer)NewScript);
            }
            else
            {
                ActiveCutscene.AddActionScript((CutsceneActionScript)NewScript);
            }
            cutsceneViewer.DrawScripts();
        }
                
        private void tsmSave_Click(object sender, EventArgs e)
        {
            SaveItem(FilePath, null);
        }

        private void CutsceneEditor_Shown(object sender, EventArgs e)
        {
            foreach (KeyValuePair<string, List<CutsceneScript>> ActiveScripts in CutsceneScriptHolder.GetScriptsByCategory())
            {
                ListBox NewScriptListBox = new ListBox();

                NewScriptListBox.Dock = DockStyle.Fill;
                NewScriptListBox.FormattingEnabled = true;
                NewScriptListBox.Location = new Point(0, 0);
                NewScriptListBox.Name = "lst" + ActiveScripts.Key;
                NewScriptListBox.Size = new Size(248, 243);
                NewScriptListBox.TabIndex = 4;
                NewScriptListBox.DoubleClick += new EventHandler(lstChoices_DoubleClick);
                NewScriptListBox.Items.AddRange(ActiveScripts.Value.ToArray());

                TabPage NewScriptTab = new TabPage();
                NewScriptTab.Controls.Add(NewScriptListBox);
                NewScriptTab.Location = new Point(4, 22);
                NewScriptTab.Name = "tab" + ActiveScripts.Key;
                NewScriptTab.Size = new Size(248, 243);
                NewScriptTab.TabIndex = 4;
                NewScriptTab.Text = ActiveScripts.Key;
                NewScriptTab.UseVisualStyleBackColor = true;

                tabControl1.Controls.Add(NewScriptTab);
            }

            if (cutsceneViewer.ActiveCutscene.DicCutsceneScript.Count == 0)
            {
                cutsceneViewer.ActiveCutscene.DicCutsceneScript = CutsceneScriptHolder.LoadAllScripts();
            }

            cutsceneViewer.ActiveCutscene.LoadForEditor();
            cutsceneViewer.Init(SelectScript);
        }
    }
}
