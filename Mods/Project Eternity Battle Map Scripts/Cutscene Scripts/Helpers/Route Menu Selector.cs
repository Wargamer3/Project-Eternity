using System;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class RouteMenuSelector : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService svc = (IWindowsFormsEditorService)
                provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                RouteMenu.RouteParams ActiveParams = (RouteMenu.RouteParams)value;
                RouteEditor NewEditor = new RouteEditor(ActiveParams.Title, ActiveParams.Summary, ActiveParams.Description);
                NewEditor.ShowDialog();
                value = new RouteMenu.RouteParams(NewEditor.txtTitle.Text, NewEditor.txtSummary.Text, NewEditor.txtDescription.Text);
            }
            return value;
        }
    }
}
