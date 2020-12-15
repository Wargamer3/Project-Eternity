using System;
using System.Windows.Forms;
using ProjectEternity.GameScreens.VisualNovelScreen;

namespace ProjectEternity.Editors.VisualNovelEditor
{
    public interface VisualNovelEditorExtension
    {
        void OnClick(IWin32Window Owner);

        void SetCurrentDialog(Dialog CurrentDialog);
    }
}
