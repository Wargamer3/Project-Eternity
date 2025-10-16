using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectEternity.Core.Editor;
using ProjectEternity.Core.Scripts;
using ProjectEternity.GameScreens.VisualNovelScreen;
using static ProjectEternity.Core.Scripts.ScriptingScriptHolder;
using static ProjectEternity.GameScreens.BattleMapScreen.ExtraBattleMapCutsceneScriptHolder;

namespace ProjectEternity.EditorsExtensions.VisualNovelEditorExtension
{
    public partial class DialogOptionsEditor : Form
    {
        private enum ItemSelectionChoices { BGM, SFX };

        private readonly Editors.VisualNovelEditor.ProjectEternityVisualNovelEditor.InitScriptDelegate InitScript;

        private ItemSelectionChoices ItemSelectionChoice;
        private Dialog CurrentDialog;

        public DialogOptionsEditor(Editors.VisualNovelEditor.ProjectEternityVisualNovelEditor.InitScriptDelegate InitScript)
        {
            InitializeComponent();

            this.InitScript = InitScript;
            CurrentDialog = null;
        }

        public void OnClick(IWin32Window Owner)
        {
            Show(Owner);
        }

        public void SetCurrentDialog(Dialog CurrentDialog)
        {
            gbSoundOptions.Enabled = true;
            this.CurrentDialog = CurrentDialog;
            txtBGM.Text = GetBGMScriptPath();
            txtSFX.Text = GetSFXScriptPath();
        }

        private void btnChangeBGM_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.BGM;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathMapBGM));
        }

        private void btnChangeSFX_Click(object sender, EventArgs e)
        {
            ItemSelectionChoice = ItemSelectionChoices.SFX;
            ListMenuItemsSelected(EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathSFX));
        }

        private string GetBGMScriptPath()
        {
            Cutscene ActiveCutscene = CurrentDialog.CutsceneBefore;

            foreach (CutsceneActionScript ActiveScript in ActiveCutscene.DicActionScript.Values)
            {
                if (ActiveScript is ScriptPlayMapTheme)
                {
                    ScriptPlayMapTheme ActivePlayMapTheme = (ScriptPlayMapTheme)ActiveScript;
                    return ActivePlayMapTheme.BGMPath;
                }
            }

            return string.Empty;
        }

        private string GetSFXScriptPath()
        {
            Cutscene ActiveCutscene = CurrentDialog.CutsceneBefore;

            foreach (CutsceneActionScript ActiveScript in ActiveCutscene.DicActionScript.Values)
            {
                if (ActiveScript is ScriptPlaySFX)
                {
                    ScriptPlaySFX ActivePlayMapTheme = (ScriptPlaySFX)ActiveScript;
                    return ActivePlayMapTheme.SFXPath;
                }
            }

            return string.Empty;
        }

        private void SetBGMScriptPath(string BGMPath)
        {
            Cutscene ActiveCutscene = CurrentDialog.CutsceneBefore;

            bool ExistingScriptFound = false;

            foreach (CutsceneActionScript ActiveScript in ActiveCutscene.DicActionScript.Values)
            {
                if (ActiveScript is ScriptPlayMapTheme)
                {
                    ScriptPlayMapTheme ActivePlayMapTheme = (ScriptPlayMapTheme)ActiveScript;
                    ActivePlayMapTheme.BGMPath = BGMPath;
                    ExistingScriptFound = true;
                    break;
                }
            }

            txtBGM.Text = BGMPath;
            if (!ExistingScriptFound)
            {
                ScriptCutsceneBehavior NewCutsceneBehavior = new ScriptCutsceneBehavior();
                ActiveCutscene.AddActionScript(NewCutsceneBehavior);

                ScriptPlayMapTheme NewPlayMapTheme = new ScriptPlayMapTheme();
                NewPlayMapTheme.ScriptSize.Location = new System.Drawing.Point(170, 0);
                NewPlayMapTheme.BGMPath = BGMPath;
                InitScript(NewPlayMapTheme);
                ActiveCutscene.AddActionScript(NewPlayMapTheme);

                NewCutsceneBehavior.ArrayEvents[0].Add(new EventInfo(ActiveCutscene.DicActionScript.Count - 1, 0));
            }
        }

        private void SetSFXScriptPath(string SFXPath)
        {
            Cutscene ActiveCutscene = CurrentDialog.CutsceneBefore;

            bool ExistingScriptFound = false;

            foreach (CutsceneActionScript ActiveScript in ActiveCutscene.DicActionScript.Values)
            {
                if (ActiveScript is ScriptPlaySFX)
                {
                    ScriptPlaySFX ActivePlayMapTheme = (ScriptPlaySFX)ActiveScript;
                    ActivePlayMapTheme.SFXPath = SFXPath;
                    ExistingScriptFound = true;
                    break;
                }
            }

            txtSFX.Text = SFXPath;
            if (!ExistingScriptFound)
            {
                ScriptCutsceneBehavior NewCutsceneBehavior = new ScriptCutsceneBehavior();
                ActiveCutscene.AddActionScript(NewCutsceneBehavior);

                ScriptPlaySFX NewPlayMapTheme = new ScriptPlaySFX();
                NewPlayMapTheme.ScriptSize.Location = new System.Drawing.Point(170, 0);
                NewPlayMapTheme.SFXPath = SFXPath;
                InitScript(NewPlayMapTheme);
                ActiveCutscene.AddActionScript(NewPlayMapTheme);

                NewCutsceneBehavior.ArrayEvents[0].Add(new EventInfo(ActiveCutscene.DicActionScript.Count - 1, 0));
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
                    case ItemSelectionChoices.BGM:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(17);
                        SetBGMScriptPath(Name);
                        break;

                    case ItemSelectionChoices.SFX:
                        Name = Items[I].Substring(0, Items[I].Length - 4).Substring(12);
                        SetSFXScriptPath(Name);
                        break;
                }
            }
        }
    }
}
