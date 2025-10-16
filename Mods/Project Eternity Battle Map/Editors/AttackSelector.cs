using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using ProjectEternity.Core.Editor;

namespace ProjectEternity.GameScreens.BattleMapScreen
{
    public class AttackSelector : UITypeEditor
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
                List<string> Items = EditorHelper.ShowContextMenuWithItem(EditorHelper.GUIRootPathAttacks);

                if (Items != null)
                {
                    value = Items[0].Substring(0, Items[0].Length - 4).Substring(16);
                }
            }
            return value;
        }
    }
}