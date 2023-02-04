using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public partial class DialogEditor : Form
    {
        public Cutscene ActiveCutscene { get { return cutsceneViewer.ActiveCutscene; } set { cutsceneViewer.ActiveCutscene = value; } }

        private Dialog ActiveDialog;

        public DialogEditor()
        {
            InitializeComponent();

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
        }

        #region Scripting

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
                UInt32 NextID = 0;
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

        #endregion

        public void InitHeadless()
        {
            cutsceneViewer.Init(null);
        }

        public void InitScript(CutsceneScript NewScript)
        {
            cutsceneViewer.InitScript(NewScript);
        }

        public void SetDialog(Dialog ActiveDialog)
        {
            this.ActiveDialog = ActiveDialog;
            ActiveCutscene = ActiveDialog.CutsceneBefore;
            rbBeforeDialog.Checked = true;
            cutsceneViewer.Init(SelectScript);
            cutsceneViewer.InitCutscene(ActiveCutscene);
            cutsceneViewer.Refresh();
        }

        private void rbBeforeDialog_CheckedChanged(object sender, EventArgs e)
        {
            ActiveCutscene = ActiveDialog.CutsceneBefore;
            cutsceneViewer.InitCutscene(ActiveCutscene);
            cutsceneViewer.Refresh();
        }

        private void rbDuringDialog_CheckedChanged(object sender, EventArgs e)
        {
            ActiveCutscene = ActiveDialog.CutsceneDuring;
            cutsceneViewer.InitCutscene(ActiveCutscene);
            cutsceneViewer.Refresh();
        }

        private void rbAfterDialog_CheckedChanged(object sender, EventArgs e)
        {
            ActiveCutscene = ActiveDialog.CutsceneAfter;
            cutsceneViewer.InitCutscene(ActiveCutscene);
            cutsceneViewer.Refresh();
        }
    }
}
