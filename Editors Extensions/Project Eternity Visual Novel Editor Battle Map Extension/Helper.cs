using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.EditorsExtensions.VisualNovelEditorExtension
{
    public class Helper : Editors.VisualNovelEditor.VisualNovelEditorExtension
    {
        private readonly Editors.VisualNovelEditor.ProjectEternityVisualNovelEditor.InitScriptDelegate InitScript;

        private DialogOptionsEditor Editor;
        private Dialog CurrentDialog;

        public Helper(Editors.VisualNovelEditor.ProjectEternityVisualNovelEditor.InitScriptDelegate InitScript)
        {
            this.InitScript = InitScript;
            Editor = new DialogOptionsEditor(InitScript);
        }

        public void OnClick(IWin32Window Owner)
        {
            if (Editor == null || Editor.IsDisposed)
            {
                Editor = new DialogOptionsEditor(InitScript);
                Editor.SetCurrentDialog(CurrentDialog);
            }

            if (!Editor.Visible)
            {
                Editor.OnClick(Owner);
            }
        }

        public void SetCurrentDialog(Dialog CurrentDialog)
        {
            this.CurrentDialog = CurrentDialog;
            Editor.SetCurrentDialog(CurrentDialog);
        }

        public override string ToString()
        {
            return "Dialog Options";
        }
    }
}
